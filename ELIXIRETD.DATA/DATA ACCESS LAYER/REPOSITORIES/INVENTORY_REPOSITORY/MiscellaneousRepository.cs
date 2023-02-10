using ELIXIRETD.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.INVENTORY_REPOSITORY
{
    public class MiscellaneousRepository :IMiscellaneous
    {
        private readonly StoreContext _context;

        public MiscellaneousRepository( StoreContext context)
        {

           _context = context;

        }






    }
}
