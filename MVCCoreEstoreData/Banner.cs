using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCCoreEStoreData
{
    public class Banner : SortableBaseEntity
    {
        [Display(Name = "Görsel")]
        public string Image { get; set; }
        [Display(Name = "Url")]
        public string Url { get; set; }
        [Display(Name = "İlk Tarih")]
        [DisplayFormat(DataFormatString = "{0:d.MM.yyyy}")]
        public DateTime? DateStart { get; set; }
        [Display(Name = "Son Tarih")]
        [DisplayFormat(DataFormatString = "{0:d.MM.yyyy}")]
        public DateTime? DateEnd { get; set; }
        [NotMapped]
        [Display(Name = "Görsel")]
        public IFormFile ImageFile { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public override void Build(ModelBuilder builder)
        {
            builder.Entity<Banner>(entity =>
            {
                entity
                .Property(p => p.Image)
                .IsRequired()
                .IsUnicode(false);
            });
        }

    }
}
