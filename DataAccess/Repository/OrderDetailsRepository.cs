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
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {

        ApplicationDbContext _db; 

        public OrderDetailsRepository(ApplicationDbContext db): base(db)
        {
            _db = db; 
        }
        public void Update(OrderDetails orderDetails)
        {
            _db.Update(orderDetails);
        }
    }
}
