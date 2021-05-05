﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStoreData
{
    public class ProductPicture : AppEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public String Picture { get; set; }
        public virtual Product Product { get; set; }

        public override void Build(ModelBuilder builder)
        {
            builder.Entity<ProductPicture>(entity =>
            {
                entity
                   .Property(p => p.Picture)
                   .IsUnicode(false)
                   .IsRequired();
            });
        }
    }
}
