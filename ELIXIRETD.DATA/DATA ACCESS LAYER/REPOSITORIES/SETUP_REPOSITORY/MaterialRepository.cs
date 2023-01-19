using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.USER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class MaterialRepository : IMaterialRepository
    {
        private new readonly StoreContext _context;

        public MaterialRepository(StoreContext context)
        {
            _context = context;
        }


        public async Task<IReadOnlyList<MaterialDto>> GetAllActiveMaterials()
        {
            var materials = _context.Materials.Where(x => x.IsActive == true)
                                              .Select(x => new MaterialDto
                                              {
                                                  Id = x.Id, 
                                                  ItemCode = x.ItemCode, 
                                                  ItemDescription = x.ItemDescription, 
                                                  ItemCategory = x.ItemCategory.ItemCategoryName,
                                                  ItemCategoryId = x.ItemCategoryId, 
                                                  BufferLevel = x.BufferLevel,
                                                  Uom = x.Uom.UomCode,
                                                  UomId = x.UomId,
                                                  DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                  AddedBy = x.AddedBy,
                                                  IsActive = x.IsActive
                                              });

            return await materials.ToListAsync();
        }

        public async Task<IReadOnlyList<MaterialDto>> GetAllInActiveMaterials()
        {
            var materials = _context.Materials.Where(x => x.IsActive == false)
                                             .Select(x => new MaterialDto
                                             {
                                                 Id = x.Id,
                                                 ItemCode = x.ItemCode,
                                                 ItemDescription = x.ItemDescription,
                                                 ItemCategory = x.ItemCategory.ItemCategoryName,
                                                 ItemCategoryId = x.ItemCategoryId,
                                                 BufferLevel = x.BufferLevel,
                                                 Uom = x.Uom.UomCode,
                                                 UomId = x.UomId,
                                                 DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                 AddedBy = x.AddedBy,
                                                 IsActive = x.IsActive 
                                             });

            return await materials.ToListAsync();
        }


   
        public async Task<bool> AddMaterial(Material materials)
        {
            await _context.AddAsync(materials);

            return true;
        }


        public async Task<bool> UpdateMaterial(Material materials)
        {
            var exisitngMaterial = await _context.Materials.Where(x => x.Id == materials.Id)
                                                           .FirstOrDefaultAsync();

            exisitngMaterial.ItemDescription = materials.ItemDescription;
            exisitngMaterial.ItemCategoryId = materials.ItemCategoryId;
            exisitngMaterial.UomId = materials.UomId;
            exisitngMaterial.BufferLevel = materials.BufferLevel;

            return true;

        }

        public async Task<bool> ActivateMaterial(Material materials)
        {
            var existingMaterial = await _context.Materials.Where(x => x.Id == materials.Id)
                                                           .FirstOrDefaultAsync();

            existingMaterial.IsActive = true;

            return true;
        }

        public async Task<bool> InActiveMaterial(Material materials)
        {
            var existingMaterial = await _context.Materials.Where(x => x.Id == materials.Id)
                                                           .FirstOrDefaultAsync();

            existingMaterial.IsActive = false;

            return true;
        }

      

        public async Task<PagedList<MaterialDto>> GetAllMaterialWithPagination(bool status, UserParams userParams)
        {
            var materials = _context.Materials.Where(x => x.IsActive == status)
                                              .OrderBy(x => x.ItemCode)
                                              .Select(x => new MaterialDto
                                             {
                                                 Id = x.Id,
                                                 ItemCode = x.ItemCode,
                                                 ItemDescription = x.ItemDescription,
                                                 ItemCategory = x.ItemCategory.ItemCategoryName,
                                                 ItemCategoryId = x.ItemCategoryId,
                                                 BufferLevel = x.BufferLevel,
                                                 Uom = x.Uom.UomCode,
                                                 UomId = x.UomId,
                                                 DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                 AddedBy = x.AddedBy,
                                                 IsActive = x.IsActive
                                             });

            return await PagedList<MaterialDto>.CreateAsync(materials, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<MaterialDto>> GetMaterialWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var materials = _context.Materials.Where(x => x.IsActive == status)
                                              .OrderBy(x => x.ItemCode)
                                            .Select(x => new MaterialDto
                                            {
                                                Id = x.Id,
                                                ItemCode = x.ItemCode,
                                                ItemDescription = x.ItemDescription,
                                                ItemCategory = x.ItemCategory.ItemCategoryName,
                                                ItemCategoryId = x.ItemCategoryId,
                                                BufferLevel = x.BufferLevel,
                                                Uom = x.Uom.UomCode,
                                                UomId = x.UomId,
                                                DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                AddedBy = x.AddedBy,
                                                IsActive = x.IsActive
                                            }).Where(x => x.ItemCode.ToLower()
                                              .Contains(search.Trim().ToLower()));
            
            return await PagedList<MaterialDto>.CreateAsync(materials, userParams.PageNumber, userParams.PageSize);

        }



        //---------------ITEM CATEGORY---------------



        public async Task<IReadOnlyList<ItemCategoryDto>> GetAllActiveItemCategory()
        {
            var categories =  _context.ItemCategories.Where(x => x.IsActive == true)
                                        .Select(x => new ItemCategoryDto
                                        {
                                              Id = x.Id,
                                              ItemCategoryName = x.ItemCategoryName,
                                              AddedBy = x.AddedBy,
                                              DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

                                          });
            return await categories.ToListAsync();
        }

        public async Task<IReadOnlyList<ItemCategoryDto>> GetAllInActiveItemCategory()
        {
            var categories = _context.ItemCategories.Where(x => x.IsActive == false)
                                      .Select(x => new ItemCategoryDto
                                      {
                                          Id = x.Id,
                                          ItemCategoryName = x.ItemCategoryName,
                                          AddedBy = x.AddedBy,
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

                                      });
            return await categories.ToListAsync();
        }


        public async Task<bool> AddNewItemCategory(ItemCategory category)
        {
            await _context.AddAsync(category);

            return true;
        }

        public async Task<bool> UpdateItemCategory(ItemCategory category)
        {
            var existingCategory = await _context.ItemCategories.Where(x => x.Id == category.Id)
                                                                .FirstOrDefaultAsync();

            existingCategory.ItemCategoryName = category.ItemCategoryName;

            return true;
        }

        public async Task<bool> InActiveItemCategory(ItemCategory category)
        {
            var existingCategory = await _context.ItemCategories.Where(x => x.Id == category.Id)
                                                        .FirstOrDefaultAsync();

            existingCategory.IsActive = false;

            return true;
        }

        public async Task<bool> ActivateItemCategory(ItemCategory category)
        {
            var existingCategory = await _context.ItemCategories.Where(x => x.Id == category.Id)
                                                          .FirstOrDefaultAsync();

            existingCategory.IsActive = true;

            return true;

        }


        public async Task<PagedList<ItemCategoryDto>> GetAllItemCategoryWithPagination(bool status, UserParams userParams)
        {
            var categories = _context.ItemCategories.Where(x => x.IsActive == status)
                                                    .OrderByDescending(x => x.DateAdded)
                                                     .Select(x => new ItemCategoryDto
                                                     {
                                                         Id = x.Id,
                                                         ItemCategoryName = x.ItemCategoryName,
                                                         AddedBy = x.AddedBy,
                                                         DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                         IsActive = x.IsActive
                                                     });

            return await PagedList<ItemCategoryDto>.CreateAsync(categories, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<ItemCategoryDto>> GetItemCategoryWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var categories = _context.ItemCategories.Where(x => x.IsActive == status)
                                                    .OrderByDescending(x => x.DateAdded)
                                                     .Select(x => new ItemCategoryDto
                                                     {
                                                         Id = x.Id,
                                                         ItemCategoryName = x.ItemCategoryName,
                                                         AddedBy = x.AddedBy,
                                                         DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                         IsActive = x.IsActive
                                                     }).Where(x => x.ItemCategoryName.ToLower()
                                                       .Contains(search.Trim().ToLower()));

            return await PagedList<ItemCategoryDto>.CreateAsync(categories, userParams.PageNumber, userParams.PageSize);
        }





        //-----------VALIDATION----------



        public async Task<bool> ValidateItemCategoryId(int id)
        {
            var validateExisting = await _context.ItemCategories.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateUOMId(int id)
        {
            var validateExisting = await _context.Uoms.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> ItemCodeExist(string itemcode)
        {
            return await _context.Materials.AnyAsync(x => x.ItemCode == itemcode);
        }

        public async Task<bool> ItemCategoryExist(string category)
        {
            return await _context.ItemCategories.AnyAsync(x => x.ItemCategoryName == category);
        }
    }
}
