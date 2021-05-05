using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreEStoreData
{
    public class ProductComment : BaseEntity
    {
        [Display(Name = "Yorum")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Text { get; set; }

        [Display(Name = "Değerlendirme")]
        [Range(1,5, ErrorMessage = "Lütfen bir değerlendirme puanı veriniz!")]
        public int Score { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public override void Build(ModelBuilder builder)
        {
            builder.Entity<ProductComment>(entity =>
            {
                entity
                   .Property(p => p.Text)
                   .IsRequired();
            });
        }
    }
}
