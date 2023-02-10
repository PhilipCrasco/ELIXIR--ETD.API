using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.INVENTORY_INTERFACE
{
    public interface IMiscellaneous
    {

         Task<bool> AddMiscellaneousReceipt(MiscellaneousReceipt receipt);
        Task<bool> AddMiscellaneousReceiptInWarehouse(Warehouse_Receiving receive);
        Task<bool> ValidateMiscellaneousInIssue (MiscellaneousReceipt receipt);






    }
}
