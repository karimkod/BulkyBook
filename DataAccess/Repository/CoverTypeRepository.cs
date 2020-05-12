using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repository
{
    class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _db;
        public CoverTypeRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(CoverType coverType)
        {
            var entityFromDb = _db.CoverTypes.Find(coverType.Id);
            if(entityFromDb != null)
            {
                entityFromDb.Name = coverType.Name;
            }
        }
    }
}
