using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL
{
    public class MiscellaneousIssue : BaseEntity
    {
        public string Customer { get; set; }

        public string Customercode { get; set; }

        [Column(TypeName ="decimal(18,2)")]
        public decimal TotalQuantity { get; set; }

        public DateTime PreparedDate { get; set; }

        public string PreparedBy { get; set; }

        public string Details { get; set; }
        public string Remarks { get; set; }

        public bool IsActive { get; set; }
        public bool? IsTransact { get; set; }

    }




}
