﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO
{
    public class PoSummaryDto
    {
        public int Id { get; set; }
        public int PR_Number { get; set; }
        public string PR_Date { get; set; }
        public int PO_Number { get; set; }
        public string PO_Date { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal Ordered { get; set; }
        public decimal Delivered { get; set; }
        public decimal Billed { get; set; }
        public string UOM { get; set; }
        public decimal UnitPrice { get; set; }
        public string VendorName { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
