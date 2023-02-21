using ELIXIRETD.DATA.CORE.INTERFACES.BORROWED_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.BORROWED_REPOSITORY
{
    public class BorrowedRepository : IBorrowedItem
    {
        private readonly StoreContext _context;

        public BorrowedRepository(StoreContext context)
        {
            _context = context;
                
        }

       
        public async Task<bool> AddBorrowReceipt(BorrowedReceipt receipt)
        {
            await _context.BorrowedReceipts.AddAsync(receipt);
            return true;
        }

        public async Task<bool> AddBorrowReceiptInWarehouse(Warehouse_Receiving warehouse)
        {
           await _context.WarehouseReceived.AddAsync(warehouse);
            return true;
        }

        public async Task<bool> InActiveBorrowedReceipt(BorrowedReceipt borrowed)
        {
            var existing = await _context.BorrowedReceipts.Where(x => x.Id == borrowed.Id)
                                                          .FirstOrDefaultAsync();

            var existingwh = await _context.WarehouseReceived.Where(x => x.BorrowedReceiptId == borrowed.Id)
                                                             .ToListAsync();

            if (existing == null)
                return false;

            existing.IsActive = false;

            foreach(var items in existingwh)
            {
                items.IsActive = false;
            }
            return true;

        }

        public async Task<bool> ActivateMiscellaenousReceipt(BorrowedReceipt borrowed)
        {
            var existing = await _context.BorrowedReceipts.Where(x => x.Id == borrowed.Id)
                                                          .FirstOrDefaultAsync();

            var existingwh = await _context.WarehouseReceived.Where(x => x.BorrowedReceiptId == borrowed.Id)
                                                             .ToListAsync();


            if (existing == null)
                return false;

            existing.IsActive = true;

            foreach(var items in existingwh)
            {

                items.IsActive = true;
            }

            return true;
                                                             
        }



        //========================================================== Borrowed ========================================================================







        public async Task<bool> AddBorrowedIssue(BorrowedIssue borrowed)
        {
            await _context.BorrowedIssues.AddAsync(borrowed);

            return true;
        }





        public async Task<IReadOnlyList<GetAvailableStocksForBorrowedIssue_Dto>> GetAvailableStocksForBorrowedIssue(string itemcode)
        {
            var getWarehouseStocks = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                               .GroupBy(x => new
                                                               {

                                                                   x.Id,
                                                                   x.ItemCode,
                                                                   x.ActualGood,
                                                                   x.ActualReceivingDate

                                                               }).Select(x => new WarehouseInventory
                                                               {

                                                                   WarehouseId = x.Key.Id,
                                                                   ItemCode = x.Key.ItemCode,
                                                                   ActualGood = x.Key.ActualGood,
                                                                   RecievingDate = x.Key.ActualReceivingDate.ToString()

                                                               });

            var moveorderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                                                  .Where(x => x.IsPrepared == true)
                                                  .GroupBy(x => new
                                                  {

                                                      x.warehouseId,
                                                      x.ItemCode

                                                  }).Select(x => new MoveOrderInventory
                                                  {

                                                      WarehouseId = x.Key.warehouseId,
                                                      ItemCode = x.Key.ItemCode,
                                                      QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                                                  });


            var issueOut = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                            .GroupBy(x => new
                                                            {

                                                                x.ItemCode,
                                                                x.WareHouseId

                                                            }).Select(x => new ItemStocksDto
                                                            {
                                                                ItemCode = x.Key.ItemCode,
                                                                Out = x.Sum(x => x.Quantity),
                                                                warehouseId = x.Key.WareHouseId

                                                            });

            var BorrowedOut = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                           .GroupBy(x => new
                                                           {
                                                               x.ItemCode,
                                                               x.WarehouseId

                                                           }).Select(x => new ItemStocksDto
                                                           {
                                                               ItemCode = x.Key.ItemCode,
                                                               Out = x.Sum(x => x.Quantity),
                                                               warehouseId = x.Key.WarehouseId

                                                           });

            var getAvailable = (from warehouse in getWarehouseStocks
                                join Moveorder in moveorderOut
                                on warehouse.WarehouseId equals Moveorder.WarehouseId
                                into leftJ1
                                from Moveorder in leftJ1.DefaultIfEmpty()

                                join issue in issueOut
                                on warehouse.WarehouseId equals issue.warehouseId
                                into leftJ2
                                from issue in leftJ2.DefaultIfEmpty()

                                join borrowOut in BorrowedOut
                                on warehouse.WarehouseId equals borrowOut.warehouseId
                                into leftJ3
                                from borrowOut in leftJ3.DefaultIfEmpty()

                                group new
                                {

                                    warehouse,
                                    Moveorder,
                                    issue,
                                    borrowOut
                                }

                                by new
                                {

                                    warehouse.WarehouseId,
                                    warehouse.ItemCode,
                                    warehouse.RecievingDate,
                                    WarehouseActualGood = warehouse.WarehouseId != null ? warehouse.ActualGood : 0,
                                    MoveOrderOut = Moveorder.QuantityOrdered != null ? Moveorder.QuantityOrdered : 0,
                                    IssueOut = issue.Out != null ? issue.Out : 0,
                                    BorrowedOut = borrowOut.Out != null ? borrowOut.Out : 0


                                }into total
                                
                                
                                select new GetAvailableStocksForBorrowedIssue_Dto
                                {

                                    WarehouseId = total.Key.WarehouseId,
                                    ItemCode = total.Key.ItemCode,
                                    RemainingStocks = total.Key.WarehouseActualGood - total.Key.MoveOrderOut - total.Key.IssueOut - total.Key.BorrowedOut,
                                    ReceivingDate  = total.Key.RecievingDate

                                }).Where(x => x.RemainingStocks !=0)
                                   .Where(x => x.ItemCode == itemcode);

            return await getAvailable.ToListAsync();

        }









        //======================================================== Validation ================================================================

        public async Task<bool> ValidateBorrowReceiptIssue(BorrowedReceipt borrowed)
        {
            var validate = await _context.WarehouseReceived.Where(x => x.BorrowedReceiptId == borrowed.Id)
                                                     .ToListAsync();


            foreach(var items in validate)
            {
                var issue = await _context.BorrowedIssueDetails.Where(x => x.WarehouseId == items.Id)
                                                                    .FirstOrDefaultAsync();

                if (issue != null)
                    return false;

            }
            return true;
        }

      
    }
}
