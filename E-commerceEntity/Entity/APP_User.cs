

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace E_commerceEntity.Entity
{
    public class APP_User : IdentityUser, IEntity
    {
        [Required]
        [MaxLength(80)]
        public string FullName { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ShoppingCart ShoppingCart { get; set; }


    }
}
