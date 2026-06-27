using E_commerceEntity.DataBase;
using E_commerceEntity.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_commerceEntity.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private E_CommerceContext context;
        public Repository<APP_User> User { get; set; }
        public Repository<Address> Address { get; set; }
        public Repository<Order> Order { get; set; }
        public Repository<Order_Item> Order_Item { get; set; }
        public Repository<Product> Product { get; set; }
        public Repository<Category> Category { get; set; }
        public Repository<ShoppingCart> ShoppingCart { get; set; }
        public Repository<Cart_Item> Cart_Item { get; set; }
        public UnitOfWork(E_CommerceContext context)
        {
            this.context = context;
            User = new Repository<APP_User>(context);
            Address = new Repository<Address>(context);

            Order = new Repository<Order>(context);
            Order_Item = new Repository<Order_Item>(context);

            Product = new Repository<Product>(context);
            Category = new Repository<Category>(context);

            ShoppingCart = new Repository<ShoppingCart>(context);
            Cart_Item = new Repository<Cart_Item>(context);


        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
