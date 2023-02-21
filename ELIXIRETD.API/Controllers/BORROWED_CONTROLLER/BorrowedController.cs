using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIRETD.DATA.SERVICES;
using FluentValidation.AspNetCore;

namespace ELIXIRETD.API.Controllers.BORROWED_CONTROLLER
{

    public class BorrowedController : BaseApiController
    {

        private readonly IUnitOfWork _unitofwork;

        public BorrowedController(IUnitOfWork unitOfWork)
        {
            _unitofwork = unitOfWork;
        }


        [HttpPost]
        [Route("AddNewBorrowedReceipts")]
        public async Task<IActionResult> AddNewBorrowedReceipts([FromBody] BorrowedReceipt receipt)
        {
            receipt.IsActive = true;
            receipt.PreparedDate = DateTime.Now;

            await _unitofwork.Borrowed.AddBorrowReceipt(receipt);
            await _unitofwork.CompleteAsync();

            return Ok(receipt);
        }



        [HttpPost]
        [Route("AddNewBorrowedReceiptInWarehouse")]
        public async Task<IActionResult> AddNewBorrowedReceiptInWarehouse([FromBody] Warehouse_Receiving[] warehouse )
        {
            DateTime datenow = DateTime.Now;

            foreach (Warehouse_Receiving items in warehouse)
            {
                items.IsActive = true;
                items.ReceivingDate= datenow;
                items.IsWarehouseReceived = true;
                items.TransactionType = "BorrowedReceipt";
                await _unitofwork.Borrowed.AddBorrowReceiptInWarehouse(items);
                await _unitofwork.CompleteAsync();

               
            }

            return Ok(" Successfully add new Borrow Request in warehouse");
        }

        [HttpPut]
        [Route("InActiveReceipt")]
        public async Task<IActionResult> InActiveReceipt([FromBody] BorrowedReceipt borrowed)
        {

            var validate = await _unitofwork.Borrowed.ValidateBorrowReceiptIssue(borrowed);

            if (validate == false)
                return BadRequest("Inactive failed, you already use the receiving id");

            await _unitofwork.Borrowed.InActiveBorrowedReceipt(borrowed);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully inactive receipt!");

        }

        [HttpPut]
        [Route("ActivateReceipt")]
        public async Task<IActionResult> ActivateReceipt([FromBody] BorrowedReceipt borrowed)
        {

            await _unitofwork.Borrowed.ActivateMiscellaenousReceipt(borrowed);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully active receipt!");
        }










        //======================================================== Borrowed Issue ===========================================================================


        [HttpGet]
        [Route("GetAllAvailableStocksForBorrowedIsssue")]
        public async Task<IActionResult> GetAllAvailableStocksForBorrowedIsssue([FromQuery] string itemcode)
        {

            var borrow = await _unitofwork.Borrowed.GetAvailableStocksForBorrowedIssue(itemcode);

            return Ok (borrow);
        }









    }
}
