﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO
{
    public class GetAllMReceiptWithPaginationdDto
    {

        public int Id { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierName { get; set;}

        public decimal TotalQuantity { get; set; }

        public string PreparedDate { get; set; }

        public string Remarks { get; set; }

        public string PreparedBy { get; set; }

        public bool IsActive { get; set; }
    }
}
