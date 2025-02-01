﻿#nullable disable

namespace CMS.Domain.Entities
{
    public class Criminal
    {
        public Criminal()
        {
            Pictures = new HashSet<CriminalPictures>();
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CriminalID { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string NationalID { get; set; }
        public string Address { get; set; }
        public string Offenses { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public bool WatchlistStatus { get; set; }

        public ICollection<CriminalPictures> Pictures { get; set; }
    }
}
