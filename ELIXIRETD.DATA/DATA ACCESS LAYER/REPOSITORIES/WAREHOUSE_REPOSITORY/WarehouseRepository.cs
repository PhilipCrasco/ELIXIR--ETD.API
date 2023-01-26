using ELIXIRETD.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.WAREHOUSE_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.WAREHOUSE_REPOSITORY
{
    public class WarehouseRepository : IWarehouseReceiveRepository
    {
        private readonly StoreContext _context;

        public WarehouseRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddNewReceivingDetails(Warehouse_Receiving receive)
        {
            await _context.WarehouseReceived.AddAsync(receive);

            return true;
        }
        public async Task<bool> EditReceivingDetails(Warehouse_Receiving receive)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<WarehouseReceivingDto>> GetAllCancelledPOWithPagination(UserParams userParams)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<WarehouseReceivingDto>> GetAllCancelledPOWithPaginationOrig(UserParams userParams, string search)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<WarehouseReceivingDto>> GetAllPoSummaryWithPagination(UserParams userParams)
        {
            var poSummary = (from posummary in _context.PoSummaries
                             where posummary.IsActive == true
                             join warehouse in _context.WarehouseReceived
                             on posummary.Id equals warehouse.PoSummaryId into leftJ
                             from receive in leftJ.DefaultIfEmpty()

                             select new WarehouseReceivingDto
                             {
                                 Id = posummary.Id,
                                 PoNumber = posummary.PO_Number,
                                 PoDate = posummary.PO_Date,
                                 PrNumber = posummary.PR_Number,
                                 PrDate = posummary.PR_Date,
                                 ItemCode = posummary.ItemCode,
                                 ItemDescription = posummary.ItemDescription,
                                 Supplier = posummary.VendorName,
                                 Uom = posummary.Uom,
                                 QuantityOrdered = posummary.Ordered,
                                 IsActive = posummary.IsActive,
                                 ActualRemaining = 0,
                                 ActualGood = receive != null && receive.IsActive != false ? receive.ActualDelivered : 0,

                             }).GroupBy(x => new
                             {
                                 x.Id,
                                 x.PoNumber,
                                 x.PoDate,
                                 x.PrNumber,
                                 x.PrDate,
                                 x.ItemCode,
                                 x.ItemDescription,
                                 x.Uom,
                                 x.Supplier,
                                 x.QuantityOrdered,
                                 x.IsActive

                             })
                                                     .Select(receive => new WarehouseReceivingDto
                                                     {
                                                         Id = receive.Key.Id,
                                                         PoNumber = receive.Key.PoNumber,
                                                         PoDate = receive.Key.PoDate,
                                                         PrNumber = receive.Key.PrNumber,
                                                         PrDate = receive.Key.PrDate,
                                                         ItemCode = receive.Key.ItemCode,
                                                         ItemDescription = receive.Key.ItemDescription,
                                                         Uom = receive.Key.Uom,
                                                         Supplier = receive.Key.Supplier,
                                                         QuantityOrdered = receive.Key.QuantityOrdered,
                                                         ActualGood = receive.Sum(x => x.ActualGood),
                                                         ActualRemaining = receive.Key.QuantityOrdered - (receive.Sum(x => x.ActualGood)),
                                                         IsActive = receive.Key.IsActive,
                                                     })
                                                     .OrderBy(x => x.PoNumber)
                                                     .Where(x => x.ActualRemaining != 0 && (x.ActualRemaining > 0))
                                                     .Where(x => x.IsActive == true);

            return await PagedList<WarehouseReceivingDto>.CreateAsync(poSummary, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<WarehouseReceivingDto>> GetPoSummaryByStatusWithPaginationOrig(UserParams userParams, string search)
        {
            var poSummary = (from posummary in _context.PoSummaries
                             where posummary.IsActive == true
                             join warehouse in _context.WarehouseReceived
                             on posummary.Id equals warehouse.PoSummaryId into leftJ
                             from receive in leftJ.DefaultIfEmpty()

                             select new WarehouseReceivingDto
                             {
                                 Id = posummary.Id,
                                 PoNumber = posummary.PO_Number,
                                 PoDate = posummary.PO_Date,
                                 PrNumber = posummary.PR_Number,
                                 PrDate = posummary.PR_Date,
                                 ItemCode = posummary.ItemCode,
                                 ItemDescription = posummary.ItemDescription,
                                 Supplier = posummary.VendorName,
                                 Uom = posummary.Uom,
                                 QuantityOrdered = posummary.Ordered,
                                 IsActive = posummary.IsActive,
                                 ActualRemaining = 0,
                                 ActualGood = receive != null && receive.IsActive != false ? receive.ActualDelivered : 0,

                             }).GroupBy(x => new
                             {
                                 x.Id,
                                 x.PoNumber,
                                 x.PoDate,
                                 x.PrNumber,
                                 x.PrDate,
                                 x.ItemCode,
                                 x.ItemDescription,
                                 x.Uom,
                                 x.Supplier,
                                 x.QuantityOrdered,
                                 x.IsActive

                             })
                                                  .Select(receive => new WarehouseReceivingDto
                                                  {
                                                      Id = receive.Key.Id,
                                                      PoNumber = receive.Key.PoNumber,
                                                      PoDate = receive.Key.PoDate,
                                                      PrNumber = receive.Key.PrNumber,
                                                      PrDate = receive.Key.PrDate,
                                                      ItemCode = receive.Key.ItemCode,
                                                      ItemDescription = receive.Key.ItemDescription,
                                                      Uom = receive.Key.Uom,
                                                      Supplier = receive.Key.Supplier,
                                                      QuantityOrdered = receive.Key.QuantityOrdered,
                                                      ActualGood = receive.Sum(x => x.ActualGood),
                                                      ActualRemaining = receive.Key.QuantityOrdered - (receive.Sum(x => x.ActualGood)),
                                                      IsActive = receive.Key.IsActive,


                                                  }).OrderBy(x => x.PoNumber)
                                                    .Where(x => x.ActualRemaining != 0 && (x.ActualRemaining > 0))
                                                    .Where(x => x.IsActive == true)
                                                    .Where(x => Convert.ToString(x.PoNumber).ToLower()
                                                    .Contains(search.Trim().ToLower()));

            return await PagedList<WarehouseReceivingDto>.CreateAsync(poSummary, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<bool> CancelPo(PoSummary summary)
        {
            var existingPo = await _context.PoSummaries.Where(x => x.Id == summary.Id)
                                                       .FirstOrDefaultAsync();

            existingPo.IsActive = false;
            existingPo.DateCancelled = DateTime.Now;
            existingPo.Reason = summary.Reason;

            return true;
        }




    }
}
