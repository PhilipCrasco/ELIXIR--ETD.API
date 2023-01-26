using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;

namespace ELIXIRETD.API.Controllers.IMPORT_CONTROLLER
{

    public class ImportController : BaseApiController
    {
        private IUnitOfWork _unitOfWork;

        public ImportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpPost]
        [Route("AddNewPOSummary")]
        public async Task<IActionResult> AddNewPo([FromBody] PoSummary[] posummary)
        {

            if (ModelState.IsValid)
            {

                List<PoSummary> duplicateList = new List<PoSummary>();
                List<PoSummary> availableImport = new List<PoSummary>();
                List<PoSummary> supplierNotExist = new List<PoSummary>();
                List<PoSummary> itemcodeNotExist = new List<PoSummary>(); 
                List<PoSummary> uomCodeNotExist = new List<PoSummary>();

                foreach (PoSummary items in posummary)
                {

                    var validateSupplier = await _unitOfWork.Imports.CheckSupplier(items.VendorName);
                    var validateItemCode = await _unitOfWork.Imports.CheckItemCode(items.ItemCode);
                    var validatePoandItem = await _unitOfWork.Imports.ValidatePOAndItemcodeManual(items.PO_Number, items.ItemCode);
                    var validateUom = await _unitOfWork.Imports.CheckUomCode(items.Uom);

                    if (validatePoandItem == true)
                    {
                        duplicateList.Add(items);
                    }

                    else if (validateSupplier == false)
                    {
                        supplierNotExist.Add(items);
                    }

                    else if (validateUom == false)
                    {
                        uomCodeNotExist.Add(items);
                    }

                    else if (validateItemCode == false)
                    {
                        itemcodeNotExist.Add(items);
                    }

                    else
                        availableImport.Add(items);

                    await _unitOfWork.Imports.AddNewPORequest(items);

                }

                var resultList = new
                {
                    availableImport,
                    duplicateList,
                    supplierNotExist,
                    itemcodeNotExist,
                    uomCodeNotExist
                };

                if (duplicateList.Count == 0 && supplierNotExist.Count == 0 && itemcodeNotExist.Count == 0 && uomCodeNotExist.Count == 0)
                {
                    await _unitOfWork.CompleteAsync();
                    return Ok("Successfully Add!");
                }

                else
                {

                    return BadRequest(resultList);
                }
            }
            return new JsonResult("Something went Wrong!") { StatusCode = 500 };
        }





    }

}
