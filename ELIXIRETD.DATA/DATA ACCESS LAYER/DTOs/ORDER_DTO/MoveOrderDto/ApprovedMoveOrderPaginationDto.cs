﻿using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto
{
    public class ApprovedMoveOrderPaginationDto
    {
        public int OrderNo { get; set; }

        public string CustomerName { get; set; }
        public string Department { get; set; }

        public string Category { get; set; }

        public decimal Quantity { get; set; }

        public string PreparedDate { get; set; }

        public bool IsApprove { get; set; }

        public bool IsPrepared { get; set; }

        public string ApprovedDate { get; set; }

        public bool IsPrint { get; set; }

        public bool IsTransact { get; set; }

    }
}
