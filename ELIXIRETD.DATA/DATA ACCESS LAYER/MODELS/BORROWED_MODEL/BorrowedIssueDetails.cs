﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL
{
    public  class BorrowedIssueDetails : BaseEntity 
    {

        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }

        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }


        public string Uom { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "Date")]
        public DateTime BorrowedDate { get; set; }

        public DateTime PreparedDate { get; set; }

        public string PreparedBy { get; set; }

        public int WarehouseId { get; set; }    

        public int BorrowedPKey { get; set; }

        public string Remarks { get; set; }

        public bool IsActive { get; set; }

        public bool ? IsTransact { get; set; }

        public bool ? IsReturned { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Consume { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ReturnQuantity { get; set; }




    }
}
