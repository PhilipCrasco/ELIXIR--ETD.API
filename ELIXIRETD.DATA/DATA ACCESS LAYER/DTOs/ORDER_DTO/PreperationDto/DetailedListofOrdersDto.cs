﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.PreperationDto
{
    public class DetailedListofOrdersDto
    {
        public string CustomerName { get; set; }
        public string Department { get; set; }
        public string OrderDate { get; set; }
        public string DateNeeded { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public decimal QuantityOrder { get; set; }



    }
}