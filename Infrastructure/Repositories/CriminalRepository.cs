using CMS.Domain.Entities;
using CMS.Domain.Interfaces;
using CMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.Repositories
{
    public class CriminalRepository : ICriminal
    {
        private readonly ApplicationDbContext _db;
        public CriminalRepository(ApplicationDbContext db) 
        {
            _db = db;
        }

        public async void AddCrimnalRecord(Criminal criminal)
        {
            try
            {
                await _db.Criminals.AddAsync(criminal);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when adding the criminal information. " +  ex.Message);
            }
        }

        public async Task<List<Criminal>> GetAllCriminalRecords()
        {
            try
            {
                return await _db.Criminals.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when getting the list of criminals. " + ex.Message);
            }
        }

        public async void AddCriminalImages(CriminalPictures criminalPictures)
        {
            //try
            //{
                await _db.CriminalPictures.AddAsync(criminalPictures);
                await _db.SaveChangesAsync();
            //}
            //catch(Exception ex)
            //{
            //    throw new Exception("An error occured when adding the criminal images. " + ex.Message);
            //}
        }
    }
}
