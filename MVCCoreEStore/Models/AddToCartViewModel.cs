﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStore.Models
{
    public class AddToCartViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
