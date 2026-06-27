using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace E_commerceEntity.Entity
{
    public class ShoppingCart : IEntity
    {
        public int Id { get; set; }
        [ForeignKey(nameof(User))]

        public string UserId { get; set; }

        public APP_User User { get; set; }
        public ICollection<Cart_Item> Items { get; set; } = new HashSet<Cart_Item>();
    }
}
