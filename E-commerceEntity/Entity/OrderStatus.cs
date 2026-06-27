using System;
using System.Collections.Generic;
using System.Text;

namespace E_commerceEntity.Entity
{
    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Shipping = 4,
        Delivered = 8,
        Cancelled = 16
    }
}
