using System;
using System.ComponentModel.DataAnnotations;

namespace MVCCoreEStoreData
{
    public abstract class BaseEntity : AppEntity
    {
        public int Id { get; set; }

        [Display(Name = "Yayında")]
        public bool Enabled { get; set; }
        [Display(Name = "Eklenme Tarihi")]
        public DateTime Date { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
