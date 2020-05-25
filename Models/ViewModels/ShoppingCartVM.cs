using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels
{
    public class ShoppingCartVM
    {
        public OrderHeader orderHeader { get; set; }
        public IEnumerable<ShoppingCart> shoppingCarts { get; set; }

    }
}
