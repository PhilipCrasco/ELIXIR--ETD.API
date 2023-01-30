using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;

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

                    if (order.Count(x => x.TrasactId == items.TrasactId && x.ItemCode == items.ItemCode && x.CustomerName == items.CustomerName) > 1)
                    {
                        DuplicateList.Add(items);
                    }
                    else
                    {
                        var validateOrderNoAndItemcode = await _unitofwork.Orders.ValidateExistOrderandItemCode(items.TrasactId, items.ItemCode , items.CustomerName);
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
                        else if (validateLocation == false)
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

        // =============================================== MoveOrder =========================================================================

        [HttpPost]
        [Route("PrepareItemForMoveOrder")]
        public async Task<IActionResult> PrepareItemforMoveOrder([FromBody] MoveOrder order)
        {
            var details = await _unitofwork.Orders.GetMoveOrderDetailsForMoveOrder(order.OrderNoPkey);

            if (details == null)
                return BadRequest("No Prepare MoveOrder  Available ");

            order.OrderNoPkey = details.Id;
            order.OrderDate = Convert.ToDateTime(details.OrderDate);
            order.DateNeeded = Convert.ToDateTime(details.DateNeeded);
            order.PreparedDate = Convert.ToDateTime(details.PreparedDate);
            order.CustomerName= details.CustomerName;
            order.Department = details.Department;
            order.Company = details.Company;
            order.ItemCode = details.ItemCode;
            order.ItemDescription = details.ItemDescription;
            order.Uom = details.Uom; 
            order.Category = details.Category;
            order.IsActive = true;
            order.IsPrepared = true;

            if (details == null)
                return BadRequest("negats");

            await _unitofwork.Orders.PrepareItemForMoveOrder(order);
            await _unitofwork.CompleteAsync();

            return Ok(order);
        }

        [HttpGet]
        [Route("GetAllListForMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllListForMoveOrderPagination([FromQuery] UserParams userParams )
        {
            var orders = await _unitofwork.Orders.GetAllListForMoveOrderPagination(userParams);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage);

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
        [Route("ListOfPreparedItemsForMoveOrder")]
        public async Task<IActionResult> ListOfPreparedItemsForMoveOrder([FromQuery] int id)
        {

            var orders = await _unitofwork.Orders.ListOfPreparedItemsForMoveOrder(id);

            return Ok(orders);

        }

        [HttpGet]
        [Route("GetAllListOfOrdersForMoveOrder")]
        public async Task<IActionResult> GetAllListOfOrdersForMoveOrder([FromQuery] int id)
        {
            var orders = await _unitofwork.Orders.ListOfOrdersForMoveOrder(id);
            return Ok(orders);
        }

        [HttpGet]
        [Route ("GetAllListOfApprovedPreparedforMoveOrder")]
        public async Task<IActionResult> GetAllListOfApprovedPreparedforMoveOrder([FromQuery] string customername)
        {
            var order = await _unitofwork.Orders.TotalListOfApprovedPreparedDate(customername);

                return Ok(order);
        }

        [HttpGet]
        [Route("GetAvailableStockFromWarehouse")]
        public async Task <IActionResult> GetAvailableStockFromWarehouse([FromQuery] int id, [FromQuery] string itemcode)
        {
            var orders = await _unitofwork.Orders.GetActualItemQuantityInWarehouse(id, itemcode);

            var getFirstrecieve = await _unitofwork.Orders.GetFirstNeeded(itemcode);

            var resultList = new
            {
                orders,
                getFirstrecieve.warehouseId

            };

            return Ok(resultList);
        }



        [HttpGet]
        [Route("GetAllOutOfStockByItemCodeAndOrderDate")]
        public async Task<IActionResult> GetAllOutOfStockByItemCodeAndOrderDate([FromQuery] string itemcode, [FromQuery] string orderdate)
        {
            var orders = await _unitofwork.Orders.GetAllOutOfStockByItemCodeAndOrderDate(itemcode, orderdate);

            return Ok(orders);

        }

        [HttpPut]
        [Route(" CancelPreparedItems")]
        public async Task<IActionResult> CancelPreparedItems([FromBody] MoveOrder moveorder)
        {
            var order = await _unitofwork.Orders.CancelMoveOrder(moveorder);

            if (order == false)
                return BadRequest("No existing Prepared Items");

            await _unitofwork.CompleteAsync();
            return Ok(order);

        }

        [HttpPut]
        [Route("UpdatePrintStatus")]
        public async Task<IActionResult> UpdatePrintStatus([FromBody] MoveOrder moveorder)
        {

            await _unitofwork.Orders.UpdatePrintStatus(moveorder);
            await _unitofwork.CompleteAsync();

            return Ok(moveorder);
        }




        //============================================= Move Order Preparation ===================================================

        [HttpGet]
        [Route("ViewMoveOrderForApproval")]
        public async Task<IActionResult> ViewMoveOrderForApproval([FromQuery] int id)
        {
            var orders = await _unitofwork.Orders.ViewMoveOrderForApproval(id);
            return Ok(orders);
        }



        [HttpPut]
        [Route("ApproveListOfMoveOrder")]
        public async Task<IActionResult> ApprovalListofMoveOrder([FromBody] MoveOrder moveOrder)
        {
            await _unitofwork.Orders.ApprovalForMoveOrders(moveOrder);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully Approved List for move order!");

        }

        [HttpGet]
        [Route("GetAllForApprovalMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<DtoMoveOrder>>> GEtAllForApprovalMoveOrderPagination([FromQuery] UserParams userParams )
        {
            var moveorder = await _unitofwork.Orders.ForApprovalMoveOrderPagination(userParams);
            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage

            };

            return Ok(moveorderResult);
        }

        [HttpGet]
        [Route("GetAllForApprovalMoveOrderPaginationOrig")]
        public async Task<ActionResult<IEnumerable<DtoMoveOrder>>> GetallForApprovalMoveOrderPaginationOrig([FromQuery] UserParams userParams , [FromQuery] string search)
        {
            if (search == null)
                return await GEtAllForApprovalMoveOrderPagination(userParams);

            var moveorder = await _unitofwork.Orders.ForApprovalMoveOrderPaginationOrig(userParams, search);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage

            };

            return Ok(moveorderResult);
        }

        [HttpGet]
        [Route("ApprovedMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<DtoMoveOrder>>> ApprovedMoveOrderPagination([FromQuery] UserParams userParams)
        {
            var moveorder = await _unitofwork.Orders.ApprovedMoveOrderPagination(userParams);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);

        }

        [HttpGet]
        [Route("ApprovedMoveOrderPaginationOrig")]
        public async Task<ActionResult<IEnumerable<DtoMoveOrder>>> ApprovedMoveOrderPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await ApprovedMoveOrderPagination(userParams);

            var moveorder = await _unitofwork.Orders.ApprovedMoveOrderPaginationOrig(userParams, search);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);
        }

        [HttpGet]
        [Route("GetAllApprovedMoveOrder")]
        public async Task<IActionResult>GetAllApprovedMoveOrder([FromQuery] int id)
        {
            var orders = await _unitofwork.Orders.GetAllApprovedMoveOrder(id);


            return Ok(orders);
        }

        [HttpPut]
        [Route("CancelOrdersInMoveOrder")]
        public async Task<IActionResult> CancelOrdersInMoveOrder([FromBody] Ordering order)
        {

            await _unitofwork.Orders.CancelControlInMoveOrder(order);

            await _unitofwork.CompleteAsync();

            return Ok("Successfully cancel orders");

        }

        [HttpPut]
        [Route("ReturnMoveOrderForApproval")]
        public async Task<IActionResult> ReturnMoveOrderForApproval([FromBody] MoveOrder moveorder)
        {

            await _unitofwork.Orders.ReturnMoveOrderForApproval(moveorder);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully return list for move order!");
        }

        [HttpPut]
        [Route("RejectApproveListOfMoveOrder")]
        public async Task<IActionResult> RejectApproveListOfMoveOrder([FromBody] MoveOrder moveorder)
        {
            await _unitofwork.Orders.RejectApproveMoveOrder(moveorder);
            await _unitofwork.CompleteAsync();
            return new JsonResult("Successfully reject approved list for move order!");
        }

        [HttpPut]
        [Route("RejectListOfMoveOrder")]
        public async Task<IActionResult> RejectListOfMoveOrder([FromBody] MoveOrder moveorder)
        {

            await _unitofwork.Orders.RejectForMoveOrder(moveorder);
            await _unitofwork.CompleteAsync();

            return new JsonResult("Successfully reject list for move order!");
        }

        [HttpGet]
        [Route("RejectedMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<DtoMoveOrder>>> RejectedMoveOrderPagination([FromQuery] UserParams userParams)
        {
            var moveorder = await _unitofwork.Orders.RejectedMoveOrderPagination(userParams);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);
        }


        [HttpGet]
        [Route("RejectedMoveOrderPaginationOrig")]
        public async Task<ActionResult<IEnumerable<DtoMoveOrder>>> RejectedMoveOrderPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await RejectedMoveOrderPagination(userParams);

            var moveorder = await _unitofwork.Orders.RejectedMoveOrderPaginationOrig(userParams, search);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount, moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return Ok(moveorderResult);
        }

        //==================================== Transact move Order ==================================================

        [HttpGet]
        [Route("GetTotalListForMoveOrder")]
        public async Task<IActionResult> GetTotalListForMoveOrder([FromQuery] bool status)
        {

            var orders = await _unitofwork.Orders.TotalListForTransactMoveOrder(status);

            return Ok(orders);

        }

        [HttpGet]
        [Route("ListOfMoveOrdersForTransact")]
        public async Task<IActionResult> ListOfMoveOrdersForTransact([FromQuery] int orderid)
        {

            var orders = await _unitofwork.Orders.ListOfMoveOrdersForTransact(orderid);

            return Ok(orders);

        }


        [HttpPost]
        [Route("TransactListOfMoveOrders")]
        public async Task<IActionResult> TransactListOfMoveOrders([FromBody] TransactMoveOrder[] transact)
        {

            foreach (TransactMoveOrder items in transact)
            {

                items.IsActive = true;
                items.IsTransact = true;
                items.PreparedDate = DateTime.Now;

                await _unitofwork.Orders.TransanctListOfMoveOrders(items);
            }

            await _unitofwork.CompleteAsync();

            return Ok(transact);

        }







    }
}
