using BulkyBook.DataAccess.Data;
using DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {

        ApplicationDbContext _db; 

        public OrderHeaderRepository(ApplicationDbContext db): base(db)
        {
            _db = db; 
        }
        public void Update(OrderHeader orderHeader)
        {
            _db.Update(orderHeader);
        }
    }
}
