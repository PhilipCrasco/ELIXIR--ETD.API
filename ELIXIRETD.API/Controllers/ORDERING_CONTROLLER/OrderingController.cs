using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using System.Net.WebSockets;

namespace ELIXIRETD.API.Controllers.ORDERING_CONTROLLER
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderingController : ControllerBase
    {
        private readonly IUnitOfWork _unitofwork;
        public OrderingController(IUnitOfWork unitOfWork)
        {
            _unitofwork= unitOfWork;
        }

        [HttpPost]
        [Route("AddNewOrders")]
        public async Task<IActionResult> AddNewOrders([FromBody] Ordering[] order)
        {
            if (ModelState.IsValid != true )
              return new JsonResult("Something went Wrong!") { StatusCode = 500 };
            {
                
                List<Ordering> DuplicateList = new List<Ordering>();
                List<Ordering> AvailableImport = new List<Ordering>();
                List<Ordering> CustomerNameNotExist = new List<Ordering>();
                List<Ordering> CompanyCodeNotExist = new List<Ordering>();
                List<Ordering> LocationNotExist = new List<Ordering>();
                List<Ordering> DepartmentNotExist = new List<Ordering>();
                List<Ordering> UomNotExist = new List<Ordering>();
                List<Ordering> ItemCodesExist = new List<Ordering>();
                List<Ordering> PreviousDateNeeded = new List<Ordering>();

                foreach (Ordering items in order)
                {

                    var validateOrderNoAndItemcode = await _unitofwork.Orders.ValidateExistOrderandItemCode(items.TrasactId, items.ItemCode);
                    var validateDateNeeded = await _unitofwork.Orders.ValidateDateNeeded(items);
                    var validateCompanyCode = await _unitofwork.Orders.ValidateCompanyCode(items.Company);
                    var validateCustomerName = await _unitofwork.Orders.ValidateCustomerName(items.CustomerName);
                    var validateLocation = await _unitofwork.Orders.ValidateLocation(items.Location);
                    var validateDepartment = await _unitofwork.Orders.ValidateCustomerType(items.Department);
                    var validateItemCode = await _unitofwork.Orders.ValidateItemCode(items.ItemCode);
                    var validateUom = await _unitofwork.Orders.ValidateUom(items.Uom);

                    if (validateOrderNoAndItemcode == true)
                    {
                        DuplicateList.Add(items);
                    }
                    else if (validateDateNeeded == false)
                    {
                        PreviousDateNeeded.Add(items);
                    }
                    else if (validateCompanyCode == false)
                    {
                        CompanyCodeNotExist.Add(items);
                    }
                    else if (validateCustomerName == false)
                    {
                        CustomerNameNotExist.Add(items);
                    }
                    else if(validateLocation == false)
                    {
                        LocationNotExist.Add(items);
                    }
                    else if (validateDepartment == false)
                    {
                        DepartmentNotExist.Add(items);
                    }
                    else if (validateItemCode == false)
                    {
                        ItemCodesExist.Add(items);
                    }
                   
                    else if (validateUom == false)
                    {
                        UomNotExist.Add(items);
                    }
                    else
                        AvailableImport.Add(items);
                    await _unitofwork.Orders.AddNewOrders(items);

                }

                var resultList = new
                {
                   AvailableImport,
                   DuplicateList,
                   CompanyCodeNotExist,
                   LocationNotExist,
                   DepartmentNotExist,
                   ItemCodesExist,
                   UomNotExist,
                   CustomerNameNotExist,
                   PreviousDateNeeded
                };

                if ( DuplicateList.Count == 0 && CompanyCodeNotExist.Count == 0  && CustomerNameNotExist.Count == 0 && LocationNotExist.Count == 0 && DepartmentNotExist.Count == 0  && ItemCodesExist.Count == 0 && UomNotExist.Count == 0 && PreviousDateNeeded.Count == 0 )
                {
                    await _unitofwork.CompleteAsync();
                    return Ok("Successfully Add!");
                }
                else
                {
                    return BadRequest(resultList);
                }
            }
           
        }


        // ===================================== Prepared Schedule ============================================================

        [HttpGet]
        [Route("GetAllListofOrdersPagination")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAlllistofOrdersPagination([FromQuery] UserParams userParams)
        {
            var orders = await _unitofwork.Orders.GetAllListofOrdersPagination(userParams);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize , orders.TotalCount , orders.TotalPages, orders.HasNextPage , orders.HasPreviousPage );

            var orderResult = new
            {
                orders, 
                orders.CurrentPage,
                orders.PageSize,
                orders.TotalCount,
                orders.TotalPages,
                orders.HasNextPage,
                orders.HasPreviousPage
            };
            
            return Ok(orderResult);
        }

        [HttpGet]
        [Route("GetAllListofOrders")]
        public async Task<IActionResult> GetAllListofOrders([FromQuery] string customer)
        {
            var orders = await _unitofwork.Orders.GetAllListofOrders(customer);

            return Ok(orders);
        }

        [HttpPut]
        [Route("SchedulePreparedOrderedDate")]
        public async Task<IActionResult> SchedulePreparedOrderedDate([FromBody] Ordering[] order)
        {
            var generate = new GenerateOrderNo();
            generate.IsActive = true;

            await _unitofwork.Orders.GenerateNumber(generate);
            await _unitofwork.CompleteAsync();
            foreach (Ordering items in order )
            {
                items.OrderNoPKey = generate.Id;
                await _unitofwork.Orders.SchedulePreparedDate(items);
            }
            await _unitofwork.CompleteAsync();
            return new JsonResult("Successfully schedule orders");
        }

        [HttpPut]
        [Route("EditOrderQuantity")]
        public async Task<IActionResult> EditOrderQuantity([FromBody] Ordering order)
        {
            await _unitofwork.Orders.EditQuantityOrder(order);
            await _unitofwork.CompleteAsync();
            return new JsonResult("Successfully edit Order Quantity");
        }

        [HttpPut]
        [Route("CancelOrders")]
        public async Task<IActionResult> Cancelorders([FromBody] Ordering orders)
        {
            var existing = await _unitofwork.Orders.CancelOrders(orders);

            if (existing == false)
                return BadRequest("Order ID is not existing");


            await _unitofwork.CompleteAsync();
            return Ok("successfully cancel orders");
        }

        [HttpPut]
        [Route("ReturnCancelledOrders")]
        public async Task<IActionResult> ReturnCancelledOrders([FromBody] Ordering orders)
        {
            var validate = await _unitofwork.Orders.ReturnCancelOrdersInList(orders);

            if (validate == false)
                return BadRequest("Orders is not exist");

            await _unitofwork.CompleteAsync();
            return Ok("Succesfully Retrun Cancel Orders");
        }

        [HttpGet]
        [Route("GetAllListOfCancelledOrders")]
        public async Task<IActionResult> GetAllListOfCancelledOrders()
        {
            var orders = await _unitofwork.Orders.GetAllListOfCancelOrders();
            return Ok(orders);
        }
        
        //============================= Prepared Ordering ===========================================================================

        [HttpPut]
        [Route("GetAllListofPrepared")]
        public async Task<IActionResult> GetAlllistofPreparedSched ()
        {
            await _unitofwork.Orders.GetAllListPreparedDate();
            await _unitofwork.CompleteAsync();

            return new JsonResult("Sucessfully schedules ordered");
        }

        [HttpGet]
        [Route("GetAllListForApprovalOfSchedule")]
        public async Task<IActionResult> GetAllListforApprovalOfSchedule()
        {
            var orders = await _unitofwork.Orders.GetAllListForApprovalOfSchedule();
            return Ok(orders);
        }

        [HttpGet]
        [Route("GetAllOrdersForScheduleApproval")]

        public async Task<IActionResult> GetallOrdersForScheduleApproval ([FromQuery]int id)
        {
            var orders = await _unitofwork.Orders.GetAllOrdersForScheduleApproval(id);

            return Ok(orders);

        }

        [HttpPut]
        [Route("ApprovePreparedDate")]
        public async Task<IActionResult> ApprovedpreparedDate (Ordering orders)
        {
            await _unitofwork.Orders.ApprovePreparedDate(orders);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully approved date!");
        }

        [HttpPut]
        [Route("RejectPreparedDate")]
        public async Task<IActionResult> Rejectdate(Ordering orders)
        {
            await _unitofwork.Orders.RejectPreparedDate(orders);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully reject prepared date!");
        }

        [HttpGet]
        [Route("DetailedListOfOrders")]
        public async Task<IActionResult> DetailedListofOrders([FromQuery]string customer)
        {
            var orders = await _unitofwork.Orders.DetailedListOfOrders(customer);
            return Ok(orders);
        }


        [HttpGet]
        [Route("OrderSummary")]

        public async Task<IActionResult> Ordersummary([FromQuery] string DateFrom , [FromQuery] string DateTo)
        {
            var orders = await _unitofwork.Orders.OrderSummary(DateFrom, DateTo);
            return Ok(orders);

        }

        [HttpGet]
        [Route("GetAllApprovedOrdersForCalendar")]
        public async Task<IActionResult> GetallApprovedOrdersforCalendar ()
        {
            var orders = await _unitofwork.Orders.GetAllApprovedOrdersForCalendar();
            return Ok(orders);
        }

        







    }
}
