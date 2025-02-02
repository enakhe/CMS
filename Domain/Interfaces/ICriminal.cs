using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Interfaces
{
    public interface ICriminal
    {
        void AddCrimnalRecord(Criminal criminal);
        Task<List<Criminal>> GetAllCriminalRecords();
        void AddCriminalImages(CriminalPictures criminalPictures);
    }
}