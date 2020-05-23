using BulkyBook.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        void Update(OrderDetails category);
    }
}
