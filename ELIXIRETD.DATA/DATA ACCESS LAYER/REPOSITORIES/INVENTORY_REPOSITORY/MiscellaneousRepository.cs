using ELIXIRETD.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.INVENTORY_REPOSITORY
{
    public class MiscellaneousRepository :IMiscellaneous
    {
        private readonly StoreContext _context;

        public MiscellaneousRepository( StoreContext context)
        {

           _context = context;

        }

        public async Task<bool> AddMiscellaneousReceipt(MiscellaneousReceipt receipt)
        {

            receipt.IsActive = true;
            receipt.PreparedDate = DateTime.Now;

            await _context.MiscellaneousReceipts.AddAsync(receipt); 
            return true;
        }

        public async Task<bool> AddMiscellaneousReceiptInWarehouse(Warehouse_Receiving receive)
        {
            await _context.WarehouseReceived.AddAsync(receive);
            return true;
        }






        // ================================================================= Validation =====================================================

        public async Task<bool> ValidateMiscellaneousInIssue(MiscellaneousReceipt receipt)
        {
            var validate = await _context.WarehouseReceived.Where(x => x.MiscellanousReceiptId == receipt.Id)
                                                           .ToListAsync();

            foreach(var items in validate)
            {
                var issue = await _context.MiscellaneousIssueDetail.Where(x => x.WareHouseId == items.Id )
                                                                   .FirstOrDefaultAsync();

                if (issue != null)
                    return false;
              
            }
            return true;
        }





    }
}
