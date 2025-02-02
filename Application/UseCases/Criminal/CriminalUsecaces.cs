using CMS.Domain.Entities;
using CMS.Domain.Interfaces;

namespace CMS.Application.UseCases.Criminal
{
    public class CriminalUsecaces
    {
        private readonly ICriminal _criminalRepository;

        public CriminalUsecaces(ICriminal criminalRepository)
        {
            _criminalRepository = criminalRepository;
        }

        public void AddCriminalRecord(Domain.Entities.Criminal criminal)
        {
            _criminalRepository.AddCrimnalRecord(criminal);
        }

        public Task<List<Domain.Entities.Criminal>> GetAllCriminalRecords()
        {
            return _criminalRepository.GetAllCriminalRecords();
        }

        public void AddCriminalImages(CriminalPictures criminalPictures)
        {
            _criminalRepository.AddCriminalImages(criminalPictures);
        }
    }
}
