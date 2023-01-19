using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.WAREHOUSE_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE
{
    public interface IWarehouseReceiveRepository
    {

        Task<bool> AddNewReceivingDetails(Warehouse_Receiving receive);
        Task<bool> EditReceivingDetails(Warehouse_Receiving receive);
        Task<bool> CancelPo(PoSummary summary);
        Task<PagedList<WarehouseReceivingDto>> GetAllPoSummaryWithPagination(UserParams userParams);
        Task<PagedList<WarehouseReceivingDto>> GetPoSummaryByStatusWithPaginationOrig(UserParams userParams, string search);
        Task<PagedList<WarehouseReceivingDto>> GetAllCancelledPOWithPagination(UserParams userParams);
        Task<PagedList<WarehouseReceivingDto>> GetAllCancelledPOWithPaginationOrig(UserParams userParams, string search);



    }
}
