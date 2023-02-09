﻿using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.PreperationDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.Orders
{
    public interface IOrdering
    {
        Task<bool> AddNewOrders(Ordering Orders);
        Task<PagedList<GetAllListofOrdersPaginationDto>> GetAllListofOrdersPagination(UserParams userParams);
        Task<IReadOnlyList<GetAllListofOrdersDto>> GetAllListofOrders(string Customer);
        Task<IReadOnlyList<OrderSummaryDto>> OrderSummary(string DateFrom, string DateTo);
        Task<bool> SchedulePreparedDate(Ordering orders);
        Task<bool> GenerateNumber(GenerateOrderNo generate);
        Task<bool> EditQuantityOrder(Ordering orders);
        Task <bool> ApprovePreparedDate(Ordering orders);
        Task<bool> RejectPreparedDate(Ordering orders);
        Task<IReadOnlyList<GetAllListCancelOrdersDto>> GetAllListOfCancelOrders();
        Task<bool> ReturnCancelOrdersInList( Ordering orders);
        Task<IReadOnlyList<DetailedListofOrdersDto>> DetailedListOfOrders (string customer);
        Task<IReadOnlyList<GetallApproveDto>> GetAllListForApprovalOfSchedule();
        Task<IReadOnlyList<GetallOrderfroScheduleApproveDto>> GetAllOrdersForScheduleApproval(int Id);
        Task<IReadOnlyList<GetAllCalendarApproveDto>> GetAllApprovedOrdersForCalendar();
        Task<bool> CancelOrders(Ordering orders);
        Task<GetMoveOrderDetailsForMoveOrderDto> GetMoveOrderDetailsForMoveOrder(int orderId);
        Task<bool> PrepareItemForMoveOrder(MoveOrder orders);
        Task<IReadOnlyList<ListOfPreparedItemsForMoveOrderDto>> ListOfPreparedItemsForMoveOrder(int id);
        Task<IReadOnlyList<ListOfOrdersForMoveOrderDto>> ListOfOrdersForMoveOrder(int id);

        Task<PagedList<GetAllListForMoveOrderPaginationDto>> GetAllListForMoveOrderPagination(UserParams userParams);

        Task<IReadOnlyList<TotalListOfApprovedPreparedDateDto>> TotalListOfApprovedPreparedDate(string customername);

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

        Task<IReadOnlyList<DtoMoveOrder>> ListOfMoveOrdersForTransact(int orderid);

        Task<bool> TransanctListOfMoveOrders(TransactMoveOrder transact);

        Task<bool> ValidatePrepareDate(Ordering orders);











        //============================ Validation ====================================================================
        Task<bool> ValidateExistOrderandItemCode(int TransactId, string ItemCode , string customerName);
        Task<bool> ValidateDateNeeded(Ordering orders);
        Task<bool> ValidateCompanyCode(string CompanyCode);
        Task<bool> ValidateCustomerName(string Customer);
        Task<bool> ValidateLocation(string Location);
        Task<bool> ValidateCustomerType(string Department);
        Task<bool> ValidateUom(string Uom);
        Task<bool> ValidateItemCode (string ItemCode);
       



    }
}
