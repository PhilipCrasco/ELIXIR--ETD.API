using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIRETD.API.Controllers.INVENTORY_CONTROLLER
{

    public class MiscellaneousController : BaseApiController
    {

        private readonly IUnitOfWork _unitofwork;

        public MiscellaneousController(IUnitOfWork unitOfWork)
        {
            _unitofwork= unitOfWork;
        }




    }
}
