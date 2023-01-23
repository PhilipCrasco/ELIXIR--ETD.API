using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL
{
    public class MoveOrder : BaseEntity
    {
        public int OrderNo { get; set; }

        public string Company { get; set; }
        public string Department { get; set; }
        public string CustomerName { get; set; }

        public string ItemCode { get; set; }
        public string Uom { get; set; }

        [Column (TypeName ="decimal(18,2)")]
        public decimal QuantityOrdered { get; set; }

        public string Category { get; set; }


        [Column(TypeName = "Date")]
        public DateTime OrderDate { get; set; }

        [Column(TypeName ="Date")]
        public DateTime DateNeeded { get; set; }
        
        public int warehouseId { get; set; }

        public bool IsActive { get; set; }

        public bool? IsApprove { get; set; }
        public DateTime? ApprovedDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? ApproveDateTempo { get; set; }

        public bool IsPrepared { get; set; }
        public bool? IsCancel { get; set; }
        public string CancelBy { get; set; }

        public DateTime? CancelledDate { get; set; }

        public int OrderNoPkey { get; set; }
        public bool? IsReject { get; set; }
        public string RejectBy { get; set; }
        
        public DateTime? RejectedDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? RejectedDateTempo { get; set; }

        public string Remarks { get; set; }

        public bool IsTransact { get; set; }

        public bool? IsPrint { get; set; }

        public bool? IsApproveReject { get; set; }

        public string BatchNo { get; set; }


    }
}
