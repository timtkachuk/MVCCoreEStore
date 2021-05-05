using System.ComponentModel.DataAnnotations;

namespace MVCCoreEStoreData
{
    public abstract class SortableBaseEntity : BaseEntity
    {
        [Display(Name = "Sıralama")]
        public int SortOrder { get; set; }

    }
}
