using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL
{
    public class BorrowedItems : BaseEntity
    {
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }

        
    }
}
