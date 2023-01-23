using ELIXIRETD.DATA.CORE.INTERFACES.Orders;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.OrderingRepository
{
    public class OrderingRepository :IOrdering
    {
        private readonly StoreContext _context;

        public OrderingRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddNewOrders(Ordering Orders)
        {

            var existingInfo = await _context.Materials.Where(x => x.ItemCode == Orders.ItemCode)
                                                        .FirstOrDefaultAsync();

            if (existingInfo == null)
                return false;

            Orders.ItemdDescription = existingInfo.ItemDescription;

            await _context.Orders.AddAsync(Orders);
            return true;
          
        }

        //Schedule Prepare

        public async Task<PagedList<OrderDto>> GetAllListofOrdersPagination(UserParams userParams)
        {
            var orders = _context.Orders.OrderBy(x => x.OrderDate)
                                        .GroupBy(x => new
                                        {
                                            x.CustomerName,
                                            x.IsActive,
                                            x.PreparedDate
                                        }).Where(x => x.Key.IsActive == true)
                                          .Where(x => x.Key.PreparedDate == null)

                                          .Select(x => new OrderDto
                                          {
                                              CustomerName = x.Key.CustomerName,
                                              IsActive = x.Key.IsActive
                                          });

            return await PagedList<OrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<IReadOnlyList<OrderDto>> GetAllListofOrders(string Customer)
        {
            var datenow = DateTime.Now;

            var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                              .GroupBy(x => new
                {
                    x.ItemCode,
                }).Select(x => new WarehouseInventory
                {
                    ItemCode = x.Key.ItemCode,
                    ActualGood = x.Sum(x => x.ActualDelivered)
                });

            var getOrderingReserve = _context.Orders.Where(x => x.IsActive == true)
                                                    .Where(x => x.PreparedDate != null)
            .GroupBy(x => new
            {
                x.ItemCode,

            }).Select(x => new OrderingInventory
            {
                ItemCode = x.Key.ItemCode,
                QuantityOrdered = x.Sum(x => x.QuantityOrdered)
            });

            var getReserve = getWarehouseStock
                .GroupJoin(getOrderingReserve, warehouse => warehouse.ItemCode, ordering => ordering.ItemCode, (warehouse, ordering) => new { warehouse, ordering })
                .SelectMany(x => x.ordering.DefaultIfEmpty(), (x, ordering) => new { x.warehouse, ordering })
                .GroupBy(x => x.warehouse.ItemCode)
                .Select(total => new ReserveInventory
                {

                    ItemCode = total.Key,
                    Reserve = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) -
                              (total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered))
                              
                });

            var orders = _context.Orders
                .OrderBy(x => x.DateNeeded)
                .Where(ordering => ordering.CustomerName == Customer && ordering.PreparedDate == null && ordering.IsActive == true)
                .GroupJoin(getReserve, ordering => ordering.ItemCode, warehouse => warehouse.ItemCode, (ordering, warehouse) => new { ordering, warehouse })
                .SelectMany(x => x.warehouse.DefaultIfEmpty(), (x, warehouse) => new { x.ordering, warehouse })
                .GroupBy(x => new
                {
                    x.ordering.Id,
                    x.ordering.OrderDate,
                    x.ordering.DateNeeded,
                    x.ordering.CustomerName,
                    x.ordering.Company,
                    x.ordering.Category,
                    x.ordering.ItemCode,
                    x.ordering.ItemdDescription,
                    x.ordering.Uom,
                    x.ordering.QuantityOrdered,
                    x.ordering.IsActive,
                    x.ordering.IsPrepared,
                    Reserve = x.warehouse.Reserve != null ? x.warehouse.Reserve : 0

                }).Select(total => new OrderDto
                {
                    Id = total.Key.Id,
                    OrderDate = total.Key.OrderDate.ToString("MM/dd/yyyy"),
                    DateNeeded = total.Key.DateNeeded.ToString("MM/dd/yyyy"),
                    CustomerName = total.Key.CustomerName,
                    Company = total.Key.Company,
                    Category = total.Key.Category,
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemdDescription,
                    Uom = total.Key.Uom,
                    QuantityOrder = total.Key.QuantityOrdered,
                    IsActive = total.Key.IsActive,
                    IsPrepared = total.Key.IsPrepared,
                    StockOnHand = total.Key.Reserve

                });
               
            return await orders.ToListAsync();

        }

        public async Task<bool> GenerateNumber(GenerateOrderNo generate)
        {
            await _context.GenerateOrders.AddAsync(generate);

            return true;
        }


        public async Task<bool> SchedulePreparedDate(Ordering orders)
        {
            var existingOrder = await _context.Orders.Where(x => x.Id == orders.Id)
                                                     .FirstOrDefaultAsync();
            if (existingOrder == null)
                return false;


            existingOrder.PreparedDate = orders.PreparedDate;
            existingOrder.OrderNoPKey = orders.OrderNoPKey;

            return true;
        }

        public async Task<bool> EditQuantityOrder(Ordering orders)
        {
            var existingOrder = await _context.Orders.Where(x => x.Id == orders.Id)
                                                     .FirstOrDefaultAsync();

            if (existingOrder == null)
                return false;

            existingOrder.QuantityOrdered = orders.QuantityOrdered;

            return true;
        }

        public async Task<bool> CancelOrders(Ordering orders)
        {
            var existing = await _context.Orders.Where(x => x.Id == orders.Id)
                                                 .Where(x => x.IsActive == true)
                                                 .FirstOrDefaultAsync();

            if(existing == null) 
                return false;

            existing.IsActive = false;
            existing.IsCancelBy = orders.IsCancelBy;
            existing.IsCancel = true;
            existing.CancelDate = DateTime.Now;
            existing.Remarks = orders.Remarks;

            return true;
        }

        public async  Task<IReadOnlyList<OrderDto>> GetAllListOfCancelOrders()
        {
            var cancelled = _context.Orders.Where(x => x.CancelDate != null)
                                           .Where(x => x.IsActive == false)
                                           .Where(x => x.IsCancelBy != null)
                                           .Select(x => new OrderDto
                                           {
                                               Id = x.Id,
                                               CustomerName = x.CustomerName,
                                               Category = x.Category,
                                               QuantityOrder = x.QuantityOrdered,
                                               OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                                               DateNeeded= x.DateNeeded.ToString("MM/dd/yyyy"),
                                               PreparedDate = x.PreparedDate.ToString(),
                                               CancelDate = x.CancelDate.ToString(),
                                               CancelBy = x.IsCancelBy

                                           });
            return await cancelled.ToListAsync();
        }
        public async  Task<bool> ReturnCancelOrdersInList(Ordering orders)
        {
            var existing = await _context.Orders.Where(x => x.Id == orders.Id)
                                                .Where(x => x.IsActive == false)
                                                .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            existing.IsActive = true;
            existing.IsCancelBy = null;
            existing.IsCancel = null;
            existing.Remarks= null;
            existing.CancelDate = null;

            return true;
        }
        // Preparation

        public async Task<IReadOnlyList<OrderDto>> GetAllListPreparedDate()
        {
            var orders = _context.Orders.Select(x => new OrderDto
            {
                Id = x.Id,

                CustomerName = x.CustomerName,
                Category = x.Category,
                QuantityOrder = x.QuantityOrdered,
                OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                DateNeeded = x.DateNeeded.ToString("MM/dd/yyyy"),
                PreparedDate = x.PreparedDate.ToString(),
                IsApproved = x.IsApproved != null

            });
            return await orders.Where(x => x.IsApproved != true)
                                        .ToListAsync();

        }

        public async  Task<IReadOnlyList<OrderDto>> GetAllListForApprovalOfSchedule()
        {
            var orders = _context.Orders.GroupBy(x => new
            {
                x.OrderNoPKey,
                x.CustomerName,
                x.Company,
                x.Department,
                x.PreparedDate,
                x.IsApproved,
                x.IsActive

            }).Where(x => x.Key.IsApproved == null)
              .Where(x => x.Key.PreparedDate != null)
              .Where(x => x.Key.IsActive == true)
              .Select(x => new OrderDto
              {
                  OrderNo =x.Key.OrderNoPKey,
                  CustomerName = x.Key.CustomerName,
                  Department = x.Key.Department,
                  Category = x.Key.Company,
                  TotalOrders = x.Sum(x => x.QuantityOrdered),
                  PreparedDate = x.Key.PreparedDate.ToString()
                  
              });

            return await orders.ToListAsync();

        }

        public async Task<IReadOnlyList<OrderDto>> GetAllOrdersForScheduleApproval(int Id)
        {
            var orders = _context.Orders.OrderBy(x => x.PreparedDate)
                                        .Where(x => x.OrderNo == Id)
                                        .Where(x => x.IsApproved == false)
                                        .Select(x => new OrderDto
                                        {
                                            OrderNo = x.OrderNoPKey,
                                            OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                                            DateNeeded = x.DateNeeded.ToString("MM/dd/yyyy"),
                                            CustomerName = x.CustomerName,
                                            Department = x.Department,
                                            ItemCode = x.ItemCode,
                                            ItemDescription = x.ItemdDescription,
                                            Uom = x.Uom,
                                            QuantityOrder = x.QuantityOrdered,
                                            IsApproved = x.IsApproved != null

                                        });
            return await orders.ToListAsync();

        }

        public async Task<bool> ApprovePreparedDate(Ordering orders)
        {
            var order = await _context.Orders.Where(x => x.OrderNoPKey == orders.OrderNoPKey)
                                             .Where(x => x.IsActive == true)
                                             .ToListAsync();

            foreach (var items in order)
            {
                items.IsApproved = true;
                items.ApprovedDate = DateTime.Now;
                items.RejectBy = null;
                items.RejectedDate = null;
                items.Remarks = null;
            }

            return true;
        }

        public async Task<bool> RejectPreparedDate(Ordering orders)
        {
            var order = await _context.Orders.Where(x => x.OrderNoPKey == orders.OrderNoPKey)
                                             .Where(x => x.IsActive == true)
                                             .ToListAsync();
            foreach (var items in order)
            {
                items.IsReject = true;
                items.RejectBy = orders.RejectBy;
                items.IsActive = true;
                items.Remarks = orders.Remarks;
                items.RejectedDate = DateTime.Now;
                items.PreparedDate = null;
                items.OrderNoPKey = 0;
            }
            return true;
        }

        public async Task<IReadOnlyList<OrderDto>> OrderSummary(string DateFrom, string DateTo)
        {

            var Totalramaining = _context.WarehouseReceived.GroupBy(x => new
            {
                x.ItemCode,
                x.ItemDescription,
                x.ActualDelivered

            }).Select(x => new ItemStocksDto
            {
                ItemCode = x.Key.ItemCode,
                ItemDescription = x.Key.ItemDescription,
                Remaining = x.Key.ActualDelivered
            });


            var totalOrders = _context.Orders.GroupBy(x => new
            {
                x.ItemCode,
                x.IsPrepared,
                x.IsActive

            }).Select(x => new OrderDto
            {
                ItemCode = x.Key.ItemCode,
                TotalOrders = x.Sum(x => x.QuantityOrdered),
                IsPrepared = x.Key.IsPrepared


            }).Where(x => x.IsPrepared == false);

            var orders = _context.Orders
                .Where(ordering => ordering.OrderDate >= DateTime.Parse(DateFrom) && ordering.OrderDate <= DateTime.Parse(DateTo))
                .GroupJoin(Totalramaining, ordering => ordering.ItemCode, warehouse => warehouse.ItemCode, (ordering, warehouse) => new { ordering, warehouse })
                .SelectMany(x => x.warehouse.DefaultIfEmpty(), (x, warehouse) => new { x.ordering, warehouse })
                .GroupBy(x =>  new 
                {
                    x.ordering.Id,
                    x.ordering.OrderDate,
                    x.ordering.DateNeeded,
                    x.ordering.CustomerName,
                    x.ordering.Department,
                    x.ordering.Category,
                    x.ordering.ItemCode,
                    x.ordering.ItemdDescription,
                    x.ordering.Uom,
                    x.ordering.QuantityOrdered,
                    x.ordering.IsActive,
                    x.ordering.IsPrepared,
                    x.ordering.PreparedDate,
                    x.ordering.IsApproved

                }).Select(total => new OrderDto
                {
                    Id = total.Key.Id,
                    OrderDate = total.Key.OrderDate.ToString("MM/dd/yyyy"),
                    DateNeeded = total.Key.DateNeeded.ToString("MM/dd/yyyy"),
                    CustomerName = total.Key.CustomerName,
                    Department = total.Key.Department,
                    Category = total.Key.Category,
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemdDescription,
                    Uom = total.Key.Uom,
                    QuantityOrder = total.Key.QuantityOrdered,
                    IsActive = total.Key.IsActive,
                    IsPrepared = total.Key.IsPrepared,
                    StockOnHand = total.Sum(x => x.warehouse.Remaining),
                    Difference = total.Sum(x => x.warehouse.Remaining) - total.Key.QuantityOrdered,
                    PreparedDate = total.Key.PreparedDate.ToString(),
                    IsApproved = total.Key.IsApproved != null


                });


            return await orders.ToListAsync();
        }

        public async Task<IReadOnlyList<OrderDto>> DetailedListOfOrders( string customer)
        {
            var orders = _context.Orders.Where(x => x.CustomerName == customer)
                                        .Select(x => new OrderDto
                                        {
                                            OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                                            DateNeeded = x.DateNeeded.ToString("MM/dd/yyyy"),
                                            CustomerName = x.CustomerName,
                                            Department = x.Department,
                                            ItemCode = x.ItemCode,
                                            ItemDescription = x.ItemdDescription,
                                            Uom = x.Uom,
                                            QuantityOrder  = x.QuantityOrdered

                                        });
            return await orders.ToListAsync();

        }
        public async Task<IReadOnlyList<OrderDto>> GetAllApprovedOrdersForCalendar()
        {
            var orders = _context.Orders.GroupBy(x => new
            {
                x.OrderNoPKey,
                x.CustomerName,
                x.Company,
                x.Department,
                x.PreparedDate,
                x.IsApproved,
                x.IsMove,
                x.IsReject,
                x.Remarks

            }).Where(x => x.Key.IsApproved == true)
              .Where(x => x.Key.PreparedDate != null)
              .Where(x => x.Key.IsMove == false)

              .Select(x => new OrderDto
              {

                  Id = x.Key.OrderNoPKey,
                  CustomerName = x.Key.CustomerName,
                  Company = x.Key.Company,
                  Category = x.Key.Department,
                  TotalOrders = x.Sum(x => x.QuantityOrdered),
                  PreparedDate = x.Key.PreparedDate.ToString(),
                  IsMove = x.Key.IsMove,
                  IsReject = x.Key.IsReject != null,
                  Remarks = x.Key.Remarks


              });

            return await orders.ToListAsync();

        }
// =========================================== MoveOrder ==============================================================================

        public async Task<OrderDto> GetMoveOrderDetailsForMoveOrder(int orderId)
        {
            var orders = _context.Orders.Select(x => new OrderDto
            {
                Id = x.Id,
                OrderNo = x.OrderNoPKey,
                CustomerName = x.CustomerName,
                Department= x.Department,
                Company = x.Company,
                ItemDescription = x.ItemdDescription,
                Uom = x.Uom,
                QuantityOrder = x.QuantityOrdered,
                Category = x.Category,
                OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                DateNeeded = x.DateNeeded.ToString("MM/dd/yyyy"),
                PreparedDate = x.PreparedDate.ToString()

            });

            return await orders.Where(x => x.Id == orderId)
                               .FirstOrDefaultAsync();
                                           
        }

        public async Task<bool> PrepareItemForMoveOrder(MoveOrder orders)
        {
            await _context.MoveOrders.AddAsync(orders);
             return true;

        }







        //================================= Validation ==============================================================================

        public async Task<bool> ValidateCompanyCode(string CompanyCode)
        {
            var validate = await _context.Companies.Where(x => x.CompanyCode == CompanyCode)
                                                  .Where(x => x.IsActive == true)
                                                  .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }


        public async Task<bool> ValidateDateNeeded(Ordering orders)
        {
            var dateNow = DateTime.Now;

            if(Convert.ToDateTime(orders.DateNeeded).Date < dateNow.Date) 
                return false;
            return true;

        }

        public async Task<bool> ValidateExistOrderandItemCode(int TransactId, string ItemCode)
        {
            var validate = await _context.Orders.Where(x => x.TrasactId == TransactId)
                                                    .Where(x => x.ItemCode == ItemCode)
                                                    .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateUom(string Uom)
        {
            var validate = await _context.Uoms.Where(x => x.UomDescription == Uom)
                                                  .Where(x => x.IsActive == true)
                                                  .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateItemCode(string ItemCode)
        {
            var validate = await _context.Materials.Where(x => x.ItemCode == ItemCode )
                                                .Where(x => x.IsActive == true)
                                                .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateCustomerName(string Customer)
        {
            var validate = await _context.Customers.Where(x => x.CustomerName == Customer )
                                                 .Where(x => x.IsActive == true)
                                                 .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateLocation(string Location)
        {
            var validate = await _context.Locations.Where(x => x.LocationName == Location)
                                               .Where(x => x.IsActive == true)
                                               .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateCustomerType(string Department)
        {
            var validate = await _context.CustomerTypes.Where(x => x.CustomerName == Department)
                                             .Where(x => x.IsActive == true)
                                             .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

       
    }
}
