using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL
{
    public class Warehouse_Receiving : BaseEntity
    {
        public int PoSummaryId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int PoNumber { get; set; }
        public string Uom { get; set; }
        public string Supplier { get; set; }

        [Column(TypeName = "Date")]
        public DateTime ReceivingDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualDelivered { get; set; }

        public string LotCategory { get; set; }

        public string TransactionType { get; set; }

        public int? MiscellanousReceiptId { get; set; }

        public string Reason { get; set; }

        public bool IsActive { get; set; } = true;

                
    }
}
