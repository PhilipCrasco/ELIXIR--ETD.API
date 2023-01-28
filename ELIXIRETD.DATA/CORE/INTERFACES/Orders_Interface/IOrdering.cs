using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using System.Threading.Tasks;

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
        Task<bool> EditQuantityOrder(Ordering orders);
        Task<IReadOnlyList<OrderDto>> GetAllListPreparedDate();
        Task <bool> ApprovePreparedDate(Ordering orders);
        Task<bool> RejectPreparedDate(Ordering orders);
        Task<IReadOnlyList<OrderDto>> GetAllListOfCancelOrders();
        Task<bool> ReturnCancelOrdersInList( Ordering orders);
        Task<IReadOnlyList<OrderDto>> DetailedListOfOrders (string customer);
        Task<IReadOnlyList<OrderDto>> GetAllListForApprovalOfSchedule();
        Task<IReadOnlyList<OrderDto>> GetAllOrdersForScheduleApproval(int Id);
        Task<IReadOnlyList<OrderDto>> GetAllApprovedOrdersForCalendar();
        Task<bool> CancelOrders(Ordering orders);
        Task<OrderDto> GetMoveOrderDetailsForMoveOrder(int orderId);
        Task<bool> PrepareItemForMoveOrder(MoveOrder orders);
        Task<IReadOnlyList<DtoMoveOrder>> ListOfPreparedItemsForMoveOrder(int id);
        Task<IReadOnlyList<OrderDto>> ListOfOrdersForMoveOrder(int id);

        Task<PagedList<OrderDto>> GetAllListForMoveOrderPagination(UserParams userParams);

        Task<IReadOnlyList<OrderDto>> TotalListOfApprovedPreparedDate(string customername);

        Task<ItemStocksDto> GetFirstNeeded(string itemCode);

        Task<ItemStocksDto> GetActualItemQuantityInWarehouse(int id, string itemcode);

        Task<IReadOnlyList<OrderDto>> GetAllOutOfStockByItemCodeAndOrderDate(string itemcode, string orderdate);

        Task<bool> ApprovalForMoveOrders(MoveOrder moveorder);
        Task<IReadOnlyList<DtoMoveOrder>> ViewMoveOrderForApproval(int id);

        Task<PagedList<DtoMoveOrder>> ForApprovalMoveOrderPagination(UserParams userParams);
        Task<PagedList<DtoMoveOrder>> ForApprovalMoveOrderPaginationOrig(UserParams userParams, string search);

        Task<PagedList<DtoMoveOrder>> ApprovedMoveOrderPagination (UserParams userParams);
        
        Task<PagedList<DtoMoveOrder>> ApprovedMoveOrderPaginationOrig (UserParams userParams, string search);

        Task<DtoMoveOrder> GetAllApprovedMoveOrder(int id);

        Task<bool> CancelMoveOrder(MoveOrder moveOrder);
        Task<bool> UpdatePrintStatus(MoveOrder moveorder);
        Task <bool> CancelControlInMoveOrder (Ordering orders);
        Task<bool> ReturnMoveOrderForApproval(MoveOrder moveorder);
        Task<bool> RejectApproveMoveOrder(MoveOrder moveOrder);
        Task<bool> RejectForMoveOrder(MoveOrder moveOrder);

        Task<PagedList<DtoMoveOrder>> RejectedMoveOrderPagination(UserParams userParams);
        Task<PagedList<DtoMoveOrder>> RejectedMoveOrderPaginationOrig(UserParams userParams, string search);

        Task<IReadOnlyList<OrderDto>> TotalListForTransactMoveOrder(bool status);








        //============================ Validation ====================================================================
        Task<bool> ValidateExistOrderandItemCode(int TransactId, string ItemCode);
        Task<bool> ValidateDateNeeded(Ordering orders);
        Task<bool> ValidateCompanyCode(string CompanyCode);
        Task<bool> ValidateCustomerName(string Customer);
        Task<bool> ValidateLocation(string Location);
        Task<bool> ValidateCustomerType(string Department);
        Task<bool> ValidateUom(string Uom);
        Task<bool> ValidateItemCode (string ItemCode);
       



    }
}
