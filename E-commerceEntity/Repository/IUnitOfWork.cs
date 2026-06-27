using E_commerceEntity.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_commerceEntity.Repository
{
    public interface IUnitOfWork
    {
        public Repository<APP_User> User { get; set; }
        public Repository<Address> Address { get; set; }
        public Repository<Order> Order { get; set; }

        public Repository<Order_Item> Order_Item { get; set; }

        public Repository<Product> Product { get; set; }

        public Repository<Category> Category { get; set; }
        public Repository<ShoppingCart> ShoppingCart { get; set; }

        public Repository<Cart_Item> Cart_Item { get; set; }



        public void Save();
    }
}
