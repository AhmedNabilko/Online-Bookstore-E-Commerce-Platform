using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace E_commerceEntity.Entity
{
    public class Order : IEntity
    {
        public int OrderId { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public APP_User User { get; set; }


        [ForeignKey(nameof(Address))]
        public int AddressId { get; set; }
        public Address Address { get; set; }


        public string OrderNumber { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<Order_Item> OrderItems { get; set; }

    }
}
