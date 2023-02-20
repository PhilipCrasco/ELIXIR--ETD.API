using ELIXIRETD.DATA.CORE.INTERFACES.BORROWED_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;

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
