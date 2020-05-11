using BulkyBook.DataAccess.Data;
using DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {

        ApplicationDbContext _db; 

        public CategoryRepository(ApplicationDbContext db): base(db)
        {
            _db = db; 
        }
        public void Update(Category category)
        {
            var categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == category.Id);
            if(categoryFromDb != null)
            {
                categoryFromDb.Name = category.Name;
            }
        }
    }
}
