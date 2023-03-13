using ELIXIRETD.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.INVENTORY_REPOSITORY
{
    public class MRPInvetoryRepository : IMRPInventory
    {
        private readonly StoreContext _context;

        public MRPInvetoryRepository(StoreContext context)
        {
            _context= context;
        }

        public async Task<IReadOnlyList<DtoGetAllAvailableInRawmaterialInventory>> GetAllAvailableInRawmaterialInventory()
        {
            return await _context.WarehouseReceived
              .GroupBy(x => new
            {
                  x.ItemCode,
                  x.ItemDescription,
                  x.LotCategory,
                  x.Uom,
                  x.IsWarehouseReceived,

            }).Select(inventory => new DtoGetAllAvailableInRawmaterialInventory
            {
                ItemCode = inventory.Key.ItemCode,
                ItemDescription = inventory.Key.ItemDescription,
                LotCategory = inventory.Key.LotCategory,
                Uom = inventory.Key.Uom,
                SOH = inventory.Sum(x => x.ActualGood),
                ReceiveIn = inventory.Sum(x => x.ActualGood),
                RejectOrder= inventory.Sum(x => x.TotalReject),
                IsWarehouseReceived = inventory.Key.IsWarehouseReceived

            }).OrderBy(x => x.ItemCode)
            .Where(x => x.IsWarehouseReceived == true)
            .ToListAsync();

        }

        public async Task<IReadOnlyList<DtoMRP>> MRPInventory()
        {
            throw new NotImplementedException();
        }
    }
}
