using ELIXIRETD.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.WAREHOUSE_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;

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
        //public async Task<bool> EditReceivingDetails(Warehouse_Receiving receive)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<PagedList<CancelledPoDto>> GetAllCancelledPOWithPagination(UserParams userParams)
        {
            //var cancelpo = (from posummary in _context.PoSummaries
            //                join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id into leftJ
            //                from receive in leftJ.DefaultIfEmpty()


            //                select new CancelledPoDto
            //                {
            //                    Id = posummary.Id,
            //                    PO_Number = posummary.PO_Number,
            //                    ItemCode = posummary.ItemCode,
            //                    ItemDescription = posummary.ItemDescription,
            //                    Supplier = posummary.VendorName,
            //                    QuantityOrdered = posummary.Ordered,
            //                    QuantityCancel = receive != null ? receive.Actual_Delivered : 0,
            //                    QuantityGood = receive != null ? receive.Actual_Delivered : 0,
            //                    DateCancelled = posummary.Date_Cancellation.ToString(),
            //                    Remarks = posummary.Reason,
            //                    IsActive = posummary.IsActive
            //                }).Where(x => x.IsActive == false)
            //                  .Where(x => x.DateCancelled != null)
            //                  .Where(x => x.Remarks != null);

            var cancelpo = _context.PoSummaries.Where(x => x.IsActive == false)
                                               .Where(x => x.IsCancelled == true)
                                               .Where(x => x.Reason != null)
                                               .Select(x => new CancelledPoDto
                                               {
                                                   Id = x.Id,
                                                   PO_Number = x.PO_Number,
                                                   ItemCode = x.ItemCode,
                                                   ItemDescription = x.ItemDescription,
                                                   Supplier = x.VendorName,
                                                   //QuantityOrdered = x.Ordered,
                                                   //QuantityCancel = receive != null ? receive.Actual_Delivered : 0,
                                                   //QuantityGood = receive != null ? receive.Actual_Delivered : 0,
                                                   DateCancelled = x.DateCancelled.ToString(),
                                                   Remarks = x.Reason,
                                                   IsActive = x.IsActive
                                               });

            return await PagedList<CancelledPoDto>.CreateAsync(cancelpo, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CancelledPoDto>> GetAllCancelledPOWithPaginationOrig(UserParams userParams, string search)
        {
            var cancelpo = _context.PoSummaries.Where(x => x.IsActive == false)
                                           .Where(x => x.IsCancelled == true)
                                           .Where(x => x.Reason != null)
                                           .Select(x => new CancelledPoDto
                                           {
                                               Id = x.Id,
                                               PO_Number = x.PO_Number,
                                               ItemCode = x.ItemCode,
                                               ItemDescription = x.ItemDescription,
                                               Supplier = x.VendorName,
                                               //QuantityOrdered = x.Ordered,
                                               //QuantityCancel = receive != null ? receive.Actual_Delivered : 0,
                                               //QuantityGood = receive != null ? receive.Actual_Delivered : 0,
                                               DateCancelled = x.DateCancelled.ToString(),
                                               Remarks = x.Reason,
                                               IsActive = x.IsActive
                                           }).Where(x => Convert.ToString(x.PO_Number).ToLower()
                                             .Contains(search.Trim().ToLower()));

            return await PagedList<CancelledPoDto>.CreateAsync(cancelpo, userParams.PageNumber, userParams.PageSize);
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
                                                    .Where(x => Convert.ToString(x.ItemDescription).ToLower()
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
            existingPo.CancelBy = summary.CancelBy;
            existingPo.IsCancelled = true;
            existingPo.DateCancelled = DateTime.Now;


            return true;
        }

        public async Task<PagedList<RejectWarehouseReceivingDto>> RejectRawMaterialsByWarehousePagination(UserParams userParams)
        {
            //var qcreceiving = (from posummary in _context.PoSummaries
            //                   join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
            //                   select new
            //                   {
            //                       Id = receive.Id,
            //                       PO_Number = posummary.PO_Number,
            //                       ItemCode = posummary.ItemCode,
            //                       ItemDescription = posummary.ItemDescription,
            //                       Supplier = posummary.VendorName,
            //                       Uom = posummary.UOM,
            //                       QuantityOrderded = posummary.Ordered
            //                   });


            var warehousereject = (from warehouse in _context.WarehouseReceived
                                   join rejectwarehouse in _context.WarehouseReject
                                   on warehouse.Id equals rejectwarehouse.WarehouseReceivingId into leftJ
                                   from rejectwarehouse in leftJ.DefaultIfEmpty()
                                       //where warehouse.ConfirmRejectbyWarehouse == true &&
                                       //warehouse.IsWarehouseReceive == true &&
                                       //warehouse.ConfirmRejectbyQc == false

                                       //join qc in qcreceiving on warehouse.QcReceivingId equals qc.Id

                                   group rejectwarehouse by new
                                   {
                                       warehouse.PoNumber,
                                       warehouse.ItemCode,
                                       warehouse.ItemDescription,
                                       warehouse.Supplier,
                                       warehouse.Uom,
                                       warehouse.Id,
                                       warehouse.ReceivingDate,
                                       warehouse.TotalReject,
                                       warehouse.Reason,
                                       warehouse.ConfirmRejectByWarehouse,
                                       warehouse.IsWarehouseReceived

                                   } into total

                                   select new RejectWarehouseReceivingDto
                                   {
                                       Id = total.Key.Id,
                                       PO_Number = total.Key.PoNumber,
                                       ItemCode = total.Key.ItemCode,
                                       ItemDescription = total.Key.ItemDescription,
                                       Supplier = total.Key.Supplier,
                                       Uom = total.Key.Uom,
                                       //QuantityOrdered = total.Key.QuantityOrderded,
                                       //ActualGood = total.Key.QuantityGood - total.Sum(x => x.Quantity),
                                       //QcReceivingId = total.Key.QcReceivingId,   
                                       ReceivingDate = total.Key.ReceivingDate.ToString(),
                                       ActualReject = total.Key.TotalReject,
                                       Remarks = total.Key.Reason,
                                       //ConfirmRejectByQc = total.Key.ConfirmRejectbyQc,
                                       //ConfirmRejectByWarehouse = total.Key.ConfirmRejectbyWarehouse,
                                       //IsWarehouseReceived = total.Key.IsWarehouseReceive

                                   });

            return await PagedList<RejectWarehouseReceivingDto>.CreateAsync(warehousereject, userParams.PageNumber, userParams.PageSize);


        }

        public Task<PagedList<RejectWarehouseReceivingDto>> RejectRawMaterialsByWarehousePaginationOrig(UserParams userParams, string search)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidatePoId(int id)
        {
            var validateExisting = await _context.PoSummaries.Where(x => x.Id == id)
                                                           .Where(x => x.IsActive == true)
                                                           .FirstOrDefaultAsync();
            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateActualRemaining(Warehouse_Receiving receiving)
        {
            var validateActualRemaining = await (from posummary in _context.PoSummaries
                                                 join receive in _context.WarehouseReceived on posummary.Id equals receive.PoSummaryId into leftJ
                                                 from receive in leftJ.DefaultIfEmpty()
                                                 where posummary.IsActive == true
                                                 select new PoSummaryChecklistDto
                                                 {
                                                     Id = posummary.Id,
                                                     PO_Number = posummary.PO_Number,
                                                     ItemCode = posummary.ItemCode,
                                                     ItemDescription = posummary.ItemDescription,
                                                     Supplier = posummary.VendorName,
                                                     UOM = posummary.Uom,
                                                     QuantityOrdered = posummary.Ordered,
                                                     ActualGood = receive != null && receive.IsActive != false ? receive.ActualDelivered : 0,
                                                     IsActive = posummary.IsActive,
                                                     ActualRemaining = 0,
                                                     IsQcReceiveIsActive = receive != null ? receive.IsActive : true
                                                 })
                                                        .GroupBy(x => new
                                                        {
                                                            x.Id,
                                                            x.PO_Number,
                                                            x.ItemCode,
                                                            x.ItemDescription,
                                                            x.UOM,
                                                            x.QuantityOrdered,
                                                            x.IsActive,
                                                            x.IsQcReceiveIsActive
                                                        })
                                                   .Select(receive => new PoSummaryChecklistDto
                                                   {
                                                       Id = receive.Key.Id,
                                                       PO_Number = receive.Key.PO_Number,
                                                       ItemCode = receive.Key.ItemCode,
                                                       ItemDescription = receive.Key.ItemDescription,
                                                       UOM = receive.Key.UOM,
                                                       QuantityOrdered = receive.Key.QuantityOrdered,
                                                       ActualGood = receive.Sum(x => x.ActualGood),
                                                       ActualRemaining = ((receive.Key.QuantityOrdered + (receive.Key.QuantityOrdered / 100) * 10) - (receive.Sum(x => x.ActualGood))),
                                                       IsActive = receive.Key.IsActive,
                                                       IsQcReceiveIsActive = receive.Key.IsQcReceiveIsActive
                                                   }).Where(x => x.IsQcReceiveIsActive == true)
                                                     .FirstOrDefaultAsync(x => x.Id == receiving.PoSummaryId);

            if (validateActualRemaining == null)
                return true;

            if (validateActualRemaining.ActualRemaining < receiving.ActualDelivered)
                return false;

            return true;

        }

        public async Task<bool> ReturnPoInAvailableList(PoSummary summary)
        {
            var existingInfo = await _context.PoSummaries.Where(x => x.Id == summary.Id)
                                                       .FirstOrDefaultAsync();
            if (existingInfo == null)
                return false;

            existingInfo.IsActive = true;
            existingInfo.DateCancelled = null;
            existingInfo.Reason = null;

            return true;
        }

        public async Task<PagedList<WarehouseReceivingDto>> ListOfWarehouseReceivingIdWithPagination(UserParams userParams)
        {

            var warehouseInventory = _context.WarehouseReceived.OrderBy(x => x.ActualReceivingDate)
                .Select(x => new WarehouseReceivingDto
                {

                    Id = x.Id,
                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemDescription,
                    ActualGood = x.ActualDelivered
                });

            return await PagedList<WarehouseReceivingDto>.CreateAsync(warehouseInventory, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<WarehouseReceivingDto>> ListOfWarehouseReceivingIdWithPaginationOrig(UserParams userParams, string search)
        {
            var warehouseInventory = _context.WarehouseReceived.OrderBy(x => x.ActualReceivingDate)
               .Select(x => new WarehouseReceivingDto
               {
                   Id = x.Id,
                   ItemCode = x.ItemCode,
                   ItemDescription = x.ItemDescription,
                   ActualGood = x.ActualDelivered
               }).Where(x => x.ItemCode.ToLower()
                 .Contains(search.Trim().ToLower()));

            return await PagedList<WarehouseReceivingDto>.CreateAsync(warehouseInventory, userParams.PageNumber, userParams.PageSize);
        }
    }
}
