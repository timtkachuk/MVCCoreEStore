﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStoreData
{
    public enum Genders
    {
        Male, Female
    }

    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        public Genders? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public virtual ICollection<Rayon> Rayons { get; set; } = new HashSet<Rayon>();
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
        public virtual ICollection<Brand> Brands { get; set; } = new HashSet<Brand>();
        public virtual ICollection<Banner> Banners { get; set; } = new HashSet<Banner>();
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public virtual ICollection<ProductComment> ProductComments { get; set; } = new HashSet<ProductComment>();
        public virtual ICollection<ShoppingCartItem> ShoppingcartItems { get; set; } = new HashSet<ShoppingCartItem>();

    }
}
