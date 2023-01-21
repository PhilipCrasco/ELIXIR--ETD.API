using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO
{
    public class ItemStocksDto
    {

        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
       public DateTime DateReceived { get; set; } 
       public decimal ActualGood { get; set; }     
        public decimal Remaining { get; set; }
        public decimal TotalMoveOrder { get; set; }
    }
}
