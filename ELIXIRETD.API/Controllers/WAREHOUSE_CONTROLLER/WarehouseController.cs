using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.WAREHOUSE_CONTROLLER
{

    public class WarehouseController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost]
        [Route("AddNewReceivingInformationInPO")]
        public async Task<IActionResult> CreateNewCustomer(Warehouse_Receiving receive)
        {
            if (ModelState.IsValid)
            {

                await _unitOfWork.Receives.AddNewReceivingDetails(receive);
                await _unitOfWork.CompleteAsync();

                return Ok("Successfully Add!");
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }


        [HttpGet]
        [Route("GetAllAvailablePoWithPagination")]
        public async Task<ActionResult<IEnumerable<PoSummaryDto>>> GetAllPoWithPagination([FromQuery] UserParams userParams)
        {
            var posummary = await _unitOfWork.Receives.GetAllPoSummaryWithPagination(userParams);

            Response.AddPaginationHeader(posummary.CurrentPage, posummary.PageSize, posummary.TotalCount, posummary.TotalPages, posummary.HasNextPage, posummary.HasPreviousPage);

            var posummaryResult = new
            {
                posummary,
                posummary.CurrentPage,
                posummary.PageSize,
                posummary.TotalCount,
                posummary.TotalPages,
                posummary.HasNextPage,
                posummary.HasPreviousPage
            };

            return Ok(posummaryResult);
        }


        [HttpGet]
        [Route("GetAllAvailablePoWithPaginationOrig")]
        public async Task<ActionResult<IEnumerable<PoSummaryDto>>> GetAllAvailablePoWithPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllPoWithPagination(userParams);

            var posummary = await _unitOfWork.Receives.GetPoSummaryByStatusWithPaginationOrig(userParams, search);

            Response.AddPaginationHeader(posummary.CurrentPage, posummary.PageSize, posummary.TotalCount, posummary.TotalPages, posummary.HasNextPage, posummary.HasPreviousPage);

            var posummaryResult = new
            {
                posummary,
                posummary.CurrentPage,
                posummary.PageSize,
                posummary.TotalCount,
                posummary.TotalPages,
                posummary.HasNextPage,
                posummary.HasPreviousPage
            };

            return Ok(posummaryResult);
        }


        [HttpPut]
        [Route("CancelPO")]
        public async Task<IActionResult> CancelPO([FromBody] PoSummary summary)
        {

            var validate = await _unitOfWork.Receives.CancelPo(summary);

            if (validate == false)
                return BadRequest("Cancel failed, you have materials for receiving in warehouse!");

            await _unitOfWork.Receives.CancelPo(summary);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully Cancelled PO!");
        }

    }
}
