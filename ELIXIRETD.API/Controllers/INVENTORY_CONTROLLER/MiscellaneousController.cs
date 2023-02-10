using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIRETD.DATA.SERVICES;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.INVENTORY_CONTROLLER
{

    public class MiscellaneousController : BaseApiController
    {

        private readonly IUnitOfWork _unitofwork;

        public MiscellaneousController(IUnitOfWork unitOfWork)
        {
            _unitofwork= unitOfWork;
        }


        [HttpPost]
        [Route("AddNewMiscellaneousReceipt")]

        public async Task<IActionResult> AddNewMiscellaneousReceipt([FromBody] MiscellaneousReceipt receipt)
        {
            await _unitofwork.miscellaneous.AddMiscellaneousReceipt(receipt);
            await _unitofwork.CompleteAsync();

            return Ok(receipt);
        }

        [HttpPost]
        [Route("AddNewMiscellaneousReceiptInWarehouse")]

        public async Task<IActionResult> AddNewMiscellaneousReceiptInWarehouse([FromBody] Warehouse_Receiving[] receive)
        {
            foreach(Warehouse_Receiving items in receive)
            {
                items.IsActive = true;
                items.ReceivingDate = DateTime.Now;
                items.IsWarehouseReceived = true;
                items.TransactionType = "MiscellaneousReceipt";

                await _unitofwork.miscellaneous.AddMiscellaneousReceiptInWarehouse(items);
                await _unitofwork.CompleteAsync();
                

            }

            return Ok("SuccessfullyAd new miscellaneous receipt in wareouse!");

        }

        [HttpPut]
        [Route("InActiveReceipt")]
        public async Task<IActionResult> InActiveReceipt([FromBody] MiscellaneousReceipt receipt)
        {
            var validate = await _unitofwork.miscellaneous.ValidateMiscellaneousInIssue(receipt);

            if (validate == false)
                return BadRequest("InActive Failed, you already use the receiving id");

            await _unitofwork.miscellaneous.InActiveMiscellaneousReceipt(receipt);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully InActive!");
  
        }

        [HttpPut]
        [Route("ActivateReceipt")]
        public async Task<IActionResult> ActivateReceipt([FromBody] MiscellaneousReceipt receipt)
        {
            await _unitofwork.miscellaneous.ActivateMiscellaenousReceipt(receipt);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully active receipt");
        }

        [HttpGet]
        [Route("GetAllMReceiptWithPagination")]
        public async Task<ActionResult<IEnumerable<GetAllMReceiptWithPaginationdDto>>> GetAllMiscellaneousReceiptPagination([FromQuery] UserParams userParams, [FromQuery] bool status)
        {
            var receipt = await _unitofwork.miscellaneous.GetAllMReceiptWithPaginationd(userParams, status);

            Response.AddPaginationHeader(receipt.CurrentPage, receipt.PageSize, receipt.TotalCount, receipt.TotalPages, receipt.HasNextPage, receipt.HasPreviousPage);

            var receiptResult = new
            {
                receipt,
                receipt.CurrentPage,
                receipt.PageSize,
                receipt.TotalCount,
                receipt.TotalPages,
                receipt.HasNextPage,
                receipt.HasPreviousPage
            };

            return Ok(receiptResult);
        }

        [HttpGet]
        [Route("GetAllMiscellaneousReceiptPaginationOrig")]
        public async Task<ActionResult<IEnumerable<GetAllMReceiptWithPaginationdDto>>> GetAllMiscellaneousReceiptPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search, [FromQuery] bool status)
        {

            if (search == null)

                return await GetAllMiscellaneousReceiptPagination(userParams, status);

            var receipt = await _unitofwork.miscellaneous.GetAllMReceiptWithPaginationOrig(userParams, search, status);

            Response.AddPaginationHeader(receipt.CurrentPage, receipt.PageSize, receipt.TotalCount, receipt.TotalPages, receipt.HasNextPage, receipt.HasPreviousPage);

            var receiptResult = new
            {
                receipt,
                receipt.CurrentPage,
                receipt.PageSize,
                receipt.TotalCount,
                receipt.TotalPages,
                receipt.HasNextPage,
                receipt.HasPreviousPage
            };

            return Ok(receiptResult);
        }













    }
}
