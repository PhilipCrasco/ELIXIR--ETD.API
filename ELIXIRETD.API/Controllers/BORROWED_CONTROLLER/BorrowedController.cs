using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL;
using ELIXIRETD.DATA.SERVICES;

namespace ELIXIRETD.API.Controllers.BORROWED_CONTROLLER
{

    public class BorrowedController : BaseApiController
    {

        private readonly IUnitOfWork _unitofwork;

        public BorrowedController(IUnitOfWork unitOfWork)
        {
            _unitofwork = unitOfWork;
        }


        [HttpGet]
        [Route("GetAllBorrowedIssueWithPagination")]
        public async Task<ActionResult<IEnumerable<GetAllBorrowedReceiptWithPaginationDto>>> GetAllBorrowedIssueWithPagination([FromBody] UserParams userParams , [FromQuery] bool status)
        {
            var issue = await _unitofwork.Borrowed.GetAllBorrowedReceiptWithPagination(userParams, status);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);
            
        }


        [HttpGet]
        [Route("GetAllBorrowedIssueWithPaginationOrig")]
        public async Task<ActionResult<IEnumerable<GetAllBorrowedReceiptWithPaginationDto>>> GetAllBorrowedIssueWithPaginationOrig ([FromQuery] UserParams userParams, [FromQuery] string search, [FromQuery] bool status)
        {
            if (search == null)

                return await GetAllBorrowedIssueWithPagination(userParams, status);

            var issue = await _unitofwork.Borrowed.GetAllBorrowedIssuetWithPaginationOrig(userParams, search, status);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }


        [HttpPost]
        [Route("AddNewBorrowedIssues")]
        public async Task<IActionResult> AddNewBorrowedIssues([FromBody] BorrowedIssue borrowed)
        {
            borrowed.IsActive = true;
            borrowed.PreparedDate= DateTime.Now;
            borrowed.IsTransact = true;

            await _unitofwork.Borrowed.AddBorrowedIssue(borrowed);
            await _unitofwork.CompleteAsync();

            return Ok(borrowed);

        }

        [HttpPost]
        [Route("AddNewBorrowedIssueDetails")]
        public async Task<IActionResult> AddNewBorrowedIssueDetails ([FromBody] BorrowedIssueDetails borrowed)
        {
            borrowed.IsActive= true;

            borrowed.PreparedDate= DateTime.Now;

            await _unitofwork.Borrowed.AddBorrowedIssueDetails(borrowed);
            await _unitofwork.CompleteAsync();

            return Ok("Successfully add new borrowed issue!");

        }



        [HttpGet]
        [Route("GetAllAvailableStocksForBorrowedIsssue")]
        public async Task<IActionResult> GetAllAvailableStocksForBorrowedIsssue([FromQuery] string itemcode)
        {

            var borrow = await _unitofwork.Borrowed.GetAvailableStocksForBorrowedIssue(itemcode);

            return Ok (borrow);
        }



        [HttpPut]
        [Route("UpdateBorrowedIssuePKey")]
        public async Task<IActionResult> UpdateBorrowedIssuePKey([FromBody] BorrowedIssueDetails[] borrowed)
        {
            foreach(BorrowedIssueDetails items in borrowed)
            {
                items.IsActive= true;
                items.PreparedDate = DateTime.Now;

                await _unitofwork.Borrowed.UpdateIssuePKey(items);
            }

            await _unitofwork.CompleteAsync();

            return Ok(borrowed);
        }


        [HttpPut]
        [Route("InActiveIssue")]
        public async Task<IActionResult> InActiveIssue([FromBody] BorrowedIssue borrowed)
        {

            await _unitofwork.Borrowed.InActiveBorrowedIssues(borrowed);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully inactive borrowed issue!");
        }

        [HttpPut]
        [Route("ActiveIssue")]
        public async Task<IActionResult> ActiveIssue([FromBody] BorrowedIssue borrowed)
        {

            await _unitofwork.Borrowed.ActiveBorrowedIssues(borrowed);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully active borrowed issue!");
        }

        [HttpGet]
        [Route("GetAllDetailsInBorrowedIssue")]
        public async Task<IActionResult> GetAllDetailsInBorrowedIssue([FromQuery] int id)
        {

            var borrow = await _unitofwork.Borrowed.GetAllDetailsInBorrowedIssue(id);

            return Ok(borrow);
        }

        [HttpGet]
        [Route("GetAllActiveBorrowedIssueTransaction")]
        public async Task<IActionResult> GetAllActiveBorrowedIssueTransaction([FromQuery] int empid)
        {

            var issue = await _unitofwork.Borrowed.GetAllAvailableIssue(empid);

            return Ok(issue);

        }

        [HttpPut]
        [Route("CancelItemCodeInBorrowedIssue")]
        public async Task<IActionResult> CancelItemCodeInBorrowedIssue([FromBody] BorrowedIssueDetails[] borrowed)
        {

            foreach(BorrowedIssueDetails items in borrowed)
            {
                await _unitofwork.Borrowed.CancelIssuePerItemCode(items);
                await _unitofwork.CompleteAsync();  
            }

            return new JsonResult("Successfully cancelled transaction!");
        }











    }
}
