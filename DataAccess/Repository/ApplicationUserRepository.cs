using BulkyBook.DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repository
{
    public class ApplicationUserRepository: Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

    }
}
