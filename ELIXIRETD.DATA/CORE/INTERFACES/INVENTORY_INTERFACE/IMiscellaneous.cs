using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
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
        Task<bool> InActiveMiscellaneousReceipt(MiscellaneousReceipt receipt);
        Task<bool> ActivateMiscellaenousReceipt (MiscellaneousReceipt receipt);
        Task<PagedList<GetAllMReceiptWithPaginationdDto>> GetAllMReceiptWithPaginationd(UserParams userParams, bool status);

        Task<PagedList<GetAllMReceiptWithPaginationdDto>> GetAllMReceiptWithPaginationOrig(UserParams userParams, string search, bool status);





             //================================ Validation ====================================================

       Task<bool> ValidateMiscellaneousInIssue (MiscellaneousReceipt receipt);


    }
}
