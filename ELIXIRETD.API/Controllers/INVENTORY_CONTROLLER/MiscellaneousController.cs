using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
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


       
        








    }
}
