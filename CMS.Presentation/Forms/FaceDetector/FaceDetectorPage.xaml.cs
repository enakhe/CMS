#nullable enable

using CMS.Application.UseCases.Criminal;
using CMS.Domain.Interfaces;
using CMS.Infrastructure.Data;
using CMS.Infrastructure.Repositories;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CMS.Presentation.Forms.FaceDetector
{
    public partial class FaceDetectorPage : Page, IDisposable
    {
        private readonly VideoCapture? _capture;
        private readonly CascadeClassifier _faceDetector;
        private readonly CriminalUsecaces _criminalUsecaces;
        private readonly IServiceProvider _serviceProvider;
        private readonly SemaphoreSlim _processingLock = new(1);
        private readonly BlockingCollection<BitmapImage> _frameBuffer = new(2);
        private readonly CancellationTokenSource _cancellationSource = new();
        private readonly RateLimiter _rateLimiter;

        private bool _isMatching;
        private bool _isDisposed;

        // Security and performance constants
        private const int MAX_FACES_PER_FRAME = 5;
        private const int MIN_FACE_SIZE = 50;
        private const int MAX_FACE_SIZE = 1000;
        private const double FACE_MATCH_THRESHOLD = 100.0;
        private const int FRAME_PROCESSING_TIMEOUT_MS = 100;

        public FaceDetectorPage(
            CriminalUsecaces criminalUsecaces,
            IServiceProvider serviceProvider)
        {
            _criminalUsecaces = criminalUsecaces;
            _serviceProvider = serviceProvider;

            InitializeComponent();

            try
            {
                _rateLimiter = new RateLimiter(
                    maxRequests: 10,
                    perTimeSpan: TimeSpan.FromSeconds(1)
                );

                _capture = new VideoCapture(0);
                _faceDetector = LoadDetectionModel();

                // Start UI update loop
                Task.Run(UpdateUILoop, _cancellationSource.Token);

                StartCamera();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Camera initialization failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private CascadeClassifier LoadDetectionModel()
        {
            string cascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "haarcascades", "haarcascade_frontalface_default.xml");

            if (!File.Exists(cascadePath))
            {
                var ex = new FileNotFoundException("Cascade file not found.");
                MessageBox.Show("Face detection model not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw ex;
            }

            var classifier = new CascadeClassifier(cascadePath);
            if (classifier.Equals(null))
            {
                var ex = new InvalidOperationException("Failed to load cascade classifier");
                throw ex;
            }

            return classifier;
        }

        private void StartCamera()
        {
            if (_capture == null)
            {
                MessageBox.Show("Camera not initialized!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _capture.ImageGrabbed += async (s, e) =>
            {
                try
                {
                    using Mat frame = new();
                    _capture.Retrieve(frame);
                    await ProcessFrame(frame);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error processing camera frame", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
            _capture.Start();
        }

        private async Task ProcessFrame(Mat frame)
        {
            if (_isDisposed) return;

            try
            {
                if (!await _rateLimiter.TryAcquireAsync())
                {
                    return;
                }

                if (!await _processingLock.WaitAsync(TimeSpan.FromMilliseconds(FRAME_PROCESSING_TIMEOUT_MS)))
                {
                    return;
                }

                try
                {
                    await DetectFace(frame);
                }
                finally
                {
                    _processingLock.Release();
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                await Task.Delay(1000);
            }
        }

        private async Task DetectFace(Mat frame)
        {
            if (_faceDetector == null) return;

            if (frame == null || frame.Width < 100 || frame.Height < 100)
            {
                return;
            }

            using var image = frame.ToImage<Bgr, byte>();
            using var grayImage = new Mat();
            CvInvoke.CvtColor(frame, grayImage, ColorConversion.Bgr2Gray);

            var faces = _faceDetector.DetectMultiScale(
                grayImage,
                1.1,
                5,
                System.Drawing.Size.Empty,
                new System.Drawing.Size(MIN_FACE_SIZE, MIN_FACE_SIZE)
            );

            faces = faces.Take(MAX_FACES_PER_FRAME)
                        .Where(f => f.Width <= MAX_FACE_SIZE && f.Height <= MAX_FACE_SIZE)
                        .ToArray();

            foreach (var face in faces)
            {
                CvInvoke.Rectangle(image, face, new MCvScalar(0, 255, 0), 2);

                using var detectedFace = new Mat(grayImage, face);
                if (await PerformLivenessCheck(detectedFace))
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    await CompareFace(detectedFace, cts.Token);
                }
            }

            using var bitmap = image.ToBitmap();
            var imageSource = BitmapToImageSource(bitmap);
            if (!_frameBuffer.TryAdd(imageSource))
            {
                MessageBox.Show("Frame buffer full, skipping frame", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task<bool> PerformLivenessCheck(Mat face)
        {
            try
            {
                using var equalizedFace = new Mat();
                CvInvoke.EqualizeHist(face, equalizedFace);

                var stdDev = new MCvScalar();
                var mean = new MCvScalar();
                CvInvoke.MeanStdDev(equalizedFace, ref mean, ref stdDev);

                if (stdDev.V0 < 30)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task CompareFace(Mat capturedFace, CancellationToken cancellationToken)
        {
            if (_isMatching) return;
            _isMatching = true;

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var criminalRepository = scope.ServiceProvider.GetRequiredService<ICriminal>();
                var criminals = await criminalRepository.GetCriminalImages();

                if (criminals.Count == 0)
                {
                    return;
                }

                var faceRecognizer = new Emgu.CV.Face.LBPHFaceRecognizer(1, 8, 8, 8, FACE_MATCH_THRESHOLD);
                var trainingImages = new List<Image<Gray, byte>>();
                var labels = new List<int>();

                foreach (var criminal in criminals)
                {
                    var imagesToCompare = new List<byte[]> { criminal.Mugshot };
                    imagesToCompare.AddRange(criminal.AdditionalPictures);

                    foreach (var imageData in imagesToCompare)
                    {
                        using var storedImage = new Mat();
                        CvInvoke.Imdecode(imageData, ImreadModes.Grayscale, storedImage);
                        if (storedImage.IsEmpty) continue;

                        trainingImages.Add(storedImage.ToImage<Gray, byte>());
                        labels.Add(int.Parse(criminal.CriminalID));
                    }
                }

                if (trainingImages.Count == 0)
                {
                    MessageBox.Show("No valid training images found", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using var vectorOfLabels = new VectorOfInt(labels.ToArray());
                using var vectorOfMats = new VectorOfMat(trainingImages.Select(img => img.Mat).ToArray());

                faceRecognizer.Train(vectorOfMats, vectorOfLabels);

                var result = faceRecognizer.Predict(capturedFace.ToImage<Gray, byte>());
                if (result.Label != -1 && result.Distance < FACE_MATCH_THRESHOLD)
                {
                    MessageBox.Show($"Match found: {result.Label}", "Criminal", MessageBoxButton.OK, MessageBoxImage.Information);
                    Dispatcher.Invoke(() =>
                    {
                        MatchResult.Text = $"Criminal ID: {result.Label}";
                        PlayAlarm();
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error comparing faces", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isMatching = false;
            }
        }

        private async Task UpdateUILoop()
        {
            try
            {
                while (!_cancellationSource.Token.IsCancellationRequested)
                {
                    var frame = _frameBuffer.Take(_cancellationSource.Token);
                    Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            CameraPreview.Source = frame;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error updating UI", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    });

                    await Task.Delay(16, _cancellationSource.Token); // ~60fps max
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error in UI update loop", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using var memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze(); // Important for cross-thread usage

            return bitmapImage;
        }

        private void PlayAlarm()
        {
            try
            {
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "audio", "audio.wav");

                if (!File.Exists(audioPath))
                {
                    return;
                }

                using SoundPlayer player = new(audioPath);
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error playing alarm sound", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _cancellationSource.Cancel();
            _processingLock.Dispose();
            _frameBuffer.Dispose();
            _faceDetector.Dispose();
            _capture?.Dispose();
            _cancellationSource.Dispose();

            _isDisposed = true;

            GC.SuppressFinalize(this);
        }
    }

    public class RateLimiter
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly Queue<DateTime> _requestTimes = new();
        private readonly int _maxRequests;
        private readonly TimeSpan _perTimeSpan;

        public RateLimiter(int maxRequests, TimeSpan perTimeSpan)
        {
            _maxRequests = maxRequests;
            _perTimeSpan = perTimeSpan;
            _semaphore = new SemaphoreSlim(1);
        }

        public async Task<bool> TryAcquireAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                var now = DateTime.UtcNow;
                while (_requestTimes.Count > 0 && now - _requestTimes.Peek() > _perTimeSpan)
                {
                    _requestTimes.Dequeue();
                }

                if (_requestTimes.Count >= _maxRequests)
                {
                    return false;
                }

                _requestTimes.Enqueue(now);
                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}