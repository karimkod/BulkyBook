using BulkyBook.DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repository
{
    public class CompanyRepository: Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

        public void Update(Company company)
        {
            var companyFromDb = _db.Companies.Find(company.Id); 
            if(companyFromDb != null)
            {
                companyFromDb.Name = company.Name;
                companyFromDb.PhoneNumber = company.PhoneNumber;
                companyFromDb.PostalCode = company.PostalCode;
                companyFromDb.State = company.State;
                companyFromDb.City = company.City;
                companyFromDb.StreetAddress = company.StreetAddress;
                companyFromDb.IsAuthorizedCompany = company.IsAuthorizedCompany;
            }
        }
    }
}
