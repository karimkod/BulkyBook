using BulkyBook.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader category);
    }
}
