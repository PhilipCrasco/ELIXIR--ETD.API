using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;

namespace ELIXIRETD.DATA.CORE.INTERFACES.Orders
{
    public interface IOrdering
    {
        Task<bool> AddNewOrders(Ordering Orders);
        Task<PagedList<OrderDto>> GetAllListofOrdersPagination(UserParams userParams);
        Task<IReadOnlyList<OrderDto>> GetAllListofOrders(string Customer);
        Task<IReadOnlyList<OrderDto>> OrderSummary(string DateFrom, string DateTo);
        Task<bool> SchedulePreparedDate(Ordering orders);
        Task<bool> GenerateNumber(GenerateOrderNo generate);


        
        Task<bool> ValidateExistOrderandItemCode(int TransactId, string ItemCode);
        Task<bool> ValidateDateNeeded(Ordering orders);
        Task<bool> ValidateCompanyCode(string CompanyCode);
        Task<bool> ValidateCustomerName(string Customer);
        Task<bool> ValidateLocation(string Location);
        Task<bool> ValidateCustomerType(string Department);
        Task<bool> ValidateUom(string Uom);
        Task<bool> ValidateItemCode (string ItemCode);
        Task<bool> ValidateItemCategories(string Itemcateg);



    }
}
