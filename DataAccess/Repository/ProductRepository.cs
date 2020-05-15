using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repository
{
    public class ProductRepository: Repository<Product>,IProductRepository 
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            Product productFromDb = _db.Products.Find(product.Id); 

            if(productFromDb != null)
            {
                productFromDb.Title = product.Title;
                productFromDb.Author = product.Author;
                productFromDb.ISBN = product.ISBN;
                productFromDb.Category = product.Category;
                productFromDb.CategoryId = product.CategoryId;
                productFromDb.CoverType = product.CoverType;
                productFromDb.CoverTypeId = product.CoverTypeId;
                productFromDb.Price = product.Price;
                productFromDb.ListPrice = product.ListPrice;
                productFromDb.Price50 = product.Price50;
                productFromDb.Price100 = product.Price100;
                productFromDb.Description = product.Description;
                productFromDb.ImageUrl = product.ImageUrl;

            }
        }


    }
}
