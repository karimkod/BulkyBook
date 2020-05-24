using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        ISP_Call SP_Call { get; }

        ICoverTypeRepository CoverType { get; }

        IProductRepository Product { get; }

        ICompanyRepository Company { get; }

        IApplicationUserRepository ApplicationUser { get; }


        public IShoppingCartRepository ShoppingCart { get; }

        public IOrderHeaderRepository OrderHeader { get;}

        public IOrderDetailsRepository OrderDetails { get; }

        void Save();
    }
}
