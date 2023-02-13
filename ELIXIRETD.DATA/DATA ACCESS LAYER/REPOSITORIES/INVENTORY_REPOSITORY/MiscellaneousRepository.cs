using ELIXIRETD.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
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

        public async Task<bool> InActiveMiscellaneousReceipt(MiscellaneousReceipt receipt)
        {
            var existing = await _context.MiscellaneousReceipts.Where(x => x.Id == receipt.Id)
                                                               .FirstOrDefaultAsync();

            var existingWH = await _context.WarehouseReceived.Where(x => x.MiscellanousReceiptId == receipt.Id)
                                                            .ToListAsync();


            if (existing == null)
                return false;

            existing.IsActive = false;

            foreach (var items in existingWH)
            {
                items.IsActive = false;
            }
            return true;

        }

        public async Task<bool> ActivateMiscellaenousReceipt(MiscellaneousReceipt receipt)
        {
            var existing = await _context.MiscellaneousReceipts.Where(x => x.Id == receipt.Id)
                                                               .FirstOrDefaultAsync();

            var existingWH = await _context.WarehouseReceived.Where(x => x.MiscellanousReceiptId == receipt.Id)
                                                             .ToListAsync();

            if (existing == null) 
                return false;

            existing.IsActive = true;

            foreach(var items in existingWH)
            {
                items.IsActive = true;
            }
            return true;    
        }

        public async Task<PagedList<GetAllMReceiptWithPaginationdDto>> GetAllMReceiptWithPaginationd(UserParams userParams, bool status)
        {
            var receipt = _context.MiscellaneousReceipts.OrderByDescending(x => x.PreparedDate)
                                                        .Where(x => x.IsActive == status)
                                                        .Select(x => new GetAllMReceiptWithPaginationdDto
                                                        {

                                                            Id= x.Id,
                                                            SupplierCode = x.SupplierCode,
                                                            SupplierName = x.supplier,
                                                            TotalQuantity= x.TotalQuantity,
                                                            PreparedBy = x.PreparedBy,
                                                            PreparedDate = x.PreparedDate.ToString("MM/dd/yyyy"),
                                                            Remarks = x.Remarks,
                                                            IsActive= x.IsActive
                                                            
                                                        });

            return await PagedList<GetAllMReceiptWithPaginationdDto>.CreateAsync(receipt, userParams.PageNumber , userParams.PageSize);
        }

        public async Task<PagedList<GetAllMReceiptWithPaginationdDto>> GetAllMReceiptWithPaginationOrig(UserParams userParams, string search, bool status)
        {
            var receipt = _context.MiscellaneousReceipts.OrderByDescending(x => x.PreparedDate)
                                                        .Where(x => x.IsActive == status)
                                                        .Select(x => new GetAllMReceiptWithPaginationdDto
                                                        {

                                                            Id = x.Id,
                                                            SupplierCode = x.SupplierCode,
                                                            SupplierName = x.supplier,
                                                            TotalQuantity = x.TotalQuantity,
                                                            PreparedBy = x.PreparedBy,
                                                            PreparedDate = x.PreparedDate.ToString("MM/dd/yyyy"),
                                                            Remarks = x.Remarks,
                                                            IsActive = x.IsActive

                                                        }).Where(x => (Convert.ToString(x.Id)).ToLower()
                                                          .Contains(search.Trim().ToLower()));

            return await PagedList<GetAllMReceiptWithPaginationdDto>.CreateAsync(receipt , userParams.PageNumber , userParams.PageSize);

        }


        public async Task<IReadOnlyList<GetWarehouseDetailsByMReceiptDto>> GetWarehouseDetailsByMReceipt(int id)
        {

            var receipt = _context.WarehouseReceived
                      .Where(x => x.MiscellanousReceiptId == id && x.IsActive == true)
                      .GroupJoin(_context.MiscellaneousReceipts, warehouse => warehouse.MiscellanousReceiptId, receiptparents => receiptparents.Id, (warehouse, receiptparents) => new { warehouse, receiptparents })
                      .SelectMany(x => x.receiptparents.DefaultIfEmpty(), (x, receiptparents) => new { x.warehouse, receiptparents })
                      .Select(x => new GetWarehouseDetailsByMReceiptDto
                      {

                          Id = x.receiptparents.Id,
                          WarehouseId = x.warehouse.Id,
                          Itemcode = x.warehouse.ItemCode,
                          ItemDescription = x.warehouse.ItemDescription,
                          TotalQuantity = x.warehouse.ActualGood,
                          SupplierCode = x.receiptparents.SupplierCode,
                          SupplierName = x.receiptparents.supplier,
                          PreparedDate = x.receiptparents.PreparedBy,
                          Remarks = x.receiptparents.Remarks

                      });

            return await receipt.ToListAsync();

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
