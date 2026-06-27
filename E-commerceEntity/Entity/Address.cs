using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace E_commerceEntity.Entity
{
    public class Address : IEntity
    {

        public int AddressId { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public APP_User User { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public bool IsDefault { get; set; }

        public ICollection<Order> Orders { get; set; }

    }
}
