﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO
{
    public class GetAvailableStocksForIssueDto
    {

        public int WarehouseId { get; set; }
        public string ItemCode { get; set; }

        public decimal ActualGood { get; set; }
        public string ReceivingDate { get; set; }

    }
}
