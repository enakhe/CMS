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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CMS.Presentation.Forms.FaceDetector
{
    public partial class FaceDetectorPage : Page
    {
        private readonly VideoCapture? _capture;
        private readonly CascadeClassifier _faceDetector;
        private readonly CriminalUsecaces _criminalUsecaces;
        private readonly IServiceProvider _serviceProvider;
        private bool _isMatching = false;

        public FaceDetectorPage(CriminalUsecaces criminalUsecaces, IServiceProvider serviceProvider)
        {
            _criminalUsecaces = criminalUsecaces;
            _serviceProvider = serviceProvider;
            InitializeComponent();

            try
            {
                _capture = new VideoCapture(0);
                _faceDetector = LoadDetectionModel();
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
                MessageBox.Show("Face detection model not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new FileNotFoundException("Cascade file not found.");
            }

            return new CascadeClassifier(cascadePath);
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
                using Mat frame = new();
                _capture.Retrieve(frame);

                using Bitmap bitmap = frame.ToImage<Bgr, byte>().ToBitmap();
                Dispatcher.Invoke(() => CameraPreview.Source = BitmapToImageSource(bitmap));
                await DetectFace(frame);
            };
            _capture.Start();
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using MemoryStream ms = new();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new();
            image.BeginInit();
            image.StreamSource = new MemoryStream(ms.ToArray());
            image.EndInit();
            return image;
        }

        private async Task DetectFace(Mat frame)
        {
            if (_faceDetector == null) return;

            using var image = frame.ToImage<Bgr, byte>();
            using var grayImage = new Mat();
            CvInvoke.CvtColor(frame, grayImage, ColorConversion.Bgr2Gray);

            var faces = _faceDetector.DetectMultiScale(grayImage, 1.1, 5);

            foreach (var face in faces)
            {
                CvInvoke.Rectangle(image, face, new MCvScalar(0, 255, 0), 2);
                using Mat detectedFace = new Mat(grayImage, face);
                //await CompareFace(detectedFace);
            }

            using Bitmap bitmap = image.ToBitmap();
            Dispatcher.Invoke(() => CameraPreview.Source = BitmapToImageSource(bitmap));
        }

        private void PlayAlarm()
        {
            string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "audio", "audio.wav");

            if (!File.Exists(audioPath))
                return;

            using SoundPlayer player = new(audioPath);
            player.Play();
        }

        private async Task CompareFace(Mat capturedFace)
        {
            //if (_isMatching) return;
            //_isMatching = true;

            using var scope = _serviceProvider.CreateScope();
            var criminalRepository = scope.ServiceProvider.GetRequiredService<ICriminal>();
            List<Domain.Entities.CriminalPictures> criminals = await criminalRepository.GetCriminalImages();
            if (criminals.Count == 0) return;

            var faceRecognizer = new Emgu.CV.Face.LBPHFaceRecognizer(1, 8, 8, 8, 100);
            List<Image<Gray, byte>> trainingImages = new();
            List<int> labels = new();

            foreach (var criminal in criminals)
            {
                List<byte[]> imagesToCompare = new() { criminal.Mugshot };
                imagesToCompare.AddRange(criminal.AdditionalPictures);

                foreach (var imageData in imagesToCompare)
                {
                    using Mat storedImage = new Mat();
                    CvInvoke.Imdecode(imageData, ImreadModes.Grayscale, storedImage);
                    if (storedImage.IsEmpty) continue;

                    trainingImages.Add(storedImage.ToImage<Gray, byte>());
                    labels.Add(int.Parse(criminal.CriminalID));
                }
            }

            if (trainingImages.Count == 0) return;

            using var vectorOfLabels = new VectorOfInt(labels.ToArray());
            using var vectorOfMats = new VectorOfMat(trainingImages.Select(img => img.Mat).ToArray());

            faceRecognizer.Train(vectorOfMats, vectorOfLabels);

            var result = faceRecognizer.Predict(capturedFace.ToImage<Gray, byte>());
            if (result.Label != -1 && result.Distance < 90)
            {
                Dispatcher.Invoke(() => MatchResult.Text = $"Criminal ID: {result.Label}");
            }
        }
    }
}
