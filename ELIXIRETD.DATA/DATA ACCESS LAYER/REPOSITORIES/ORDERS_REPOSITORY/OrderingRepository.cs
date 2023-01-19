using ELIXIRETD.DATA.CORE.INTERFACES.Orders;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;

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

            var getReserve = (from warehouse in getWarehouseStock
                              join ordering in getOrderingReserve
                              on warehouse.ItemCode equals ordering.ItemCode
                              into leftJ1
                              from ordering in leftJ1.DefaultIfEmpty()

                              group new
                              {
                                  warehouse,
                                  ordering
                              }
                              by new
                              {
                                  warehouse.ItemCode,

                              } into total

                              select new ReserveInventory
                              {

                                  ItemCode = total.Key.ItemCode,
                                  Reserve = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) -
                                  total.Sum(x=> x.ordering.QuantityOrdered == null ? 0 :x.ordering.QuantityOrdered)

                              });

            var orders = (from ordering in _context.Orders
                          where ordering.CustomerName == Customer && ordering.PreparedDate == null && ordering.IsActive == true
                          join warehouse in getReserve
                          on ordering.ItemCode equals warehouse.ItemCode
                          into leftJ
                          from warehouse in leftJ.DefaultIfEmpty()

                          group new
                          {
                              ordering,
                              warehouse
                          }
                          by new
                          {
                              ordering.Id,
                              ordering.OrderDate,
                              ordering.DateNeeded,
                              ordering.CustomerName,
                              ordering.Company,
                              ordering.Category,
                              ordering.ItemCode,
                              ordering.ItemdDescription,
                              ordering.Uom,
                              ordering.QuantityOrdered,
                              ordering.IsActive,
                              ordering.IsPrepared,
                              Reserve = warehouse.Reserve != null ? warehouse.Reserve : 0


                          } into total

                          orderby total.Key.DateNeeded ascending

                          select new OrderDto
                          {
                              Id = total.Key.Id,
                              OrderDate = total.Key.OrderDate.ToString("MM/dd/yyyy"),
                              DateNeeded = total.Key.DateNeeded.ToString("MM/dd/yyyy"),
                              CustomerName = total.Key.CustomerName,
                              Company = total.Key.Company,
                              Category = total.Key.Category,
                              ItemCode = total.Key.ItemCode,
                              ItemDescription  = total.Key.ItemdDescription,
                              Uom = total.Key.Uom,
                              QuantityOrder = total.Key.QuantityOrdered,
                              IsActive = total.Key.IsActive,
                              IsPrepared = total.Key.IsPrepared,
                              StockOnHand = total.Key.Reserve

                          });

            return await orders.ToListAsync();

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

        public async Task<bool> GenerateNumber(GenerateOrderNo generate)
        {
            await _context.GenerateOrders.AddAsync(generate);

            return true;
        }

        public Task<IReadOnlyList<OrderDto>> OrderSummary(string DateFrom, string DateTo)
        {
            throw new NotImplementedException();
        }






        //Validation==============================================================================

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

        public async Task<bool> ValidateItemCategories(string Itemcateg)
        {
            var validate = await _context.ItemCategories.Where(x => x.ItemCategoryName == Itemcateg)
                                             .Where(x => x.IsActive == true)
                                             .FirstOrDefaultAsync();
            if (validate == null)
                return false;

            return true;
        }

      

      
    }
}
