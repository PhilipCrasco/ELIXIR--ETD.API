using ELIXIRETD.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.BORROWED_INTERFACE
{
    public interface IBorrowedItem
    {

        Task<bool> AddBorrowReceiptInWarehouse(Warehouse_Receiving warehouse);
        Task<bool> AddBorrowReceipt(BorrowedReceipt receipt);
        Task<bool> InActiveBorrowedReceipt(BorrowedReceipt borrowed);
        Task<bool> ActivateMiscellaenousReceipt(BorrowedReceipt borrowed);


        Task<IReadOnlyList<GetAvailableStocksForBorrowedIssue_Dto>> GetAvailableStocksForBorrowedIssue(string itemcode);
        Task<bool> AddBorrowedIssue(BorrowedIssue borrowed);








        Task<bool> ValidateBorrowReceiptIssue(BorrowedReceipt borrowed);

    }
}
