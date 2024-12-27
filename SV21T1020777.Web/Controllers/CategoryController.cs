using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020777.BusinessLayers;
using SV21T1020777.DomainModels;
using SV21T1020777.Web.AppCodes;
using SV21T1020777.Web.Models;

namespace SV21T1020777.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR},{WebUserRoles.EMPLOYEE}")]
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 5;
        private const string CATEGORY_SEARCH_CONDITION = "CategorySearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);

        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfCategories(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            CategorySearchResult model = new CategorySearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CATEGORY_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng";
            var data = new Category()
            {
                CategoryID = 0, // khi id bằng 0 sẽ bổ sung mới
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng";
            var data = CommonDataService.GetCategory(id);
            if(data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Category data)
        {
            try
            {
                //kiểm soát dữ liệu đầu vào
                ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng" : "Cập nhật thông tin loại hàng";
                //Kiểm tra dữ liệu đầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào ModelState
                if (string.IsNullOrWhiteSpace(data.CategoryName))
                    ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng không để trống");
                if (string.IsNullOrWhiteSpace(data.Description))
                    ModelState.AddModelError(nameof(data.Description), "Thông tin mô tả không để trống");

                //dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay không?
                if (ModelState.IsValid == false)
                {
                    return View("Edit", data);
                }

                if (data.CategoryID == 0)
                {
                    //Add
                    int id = CommonDataService.AddCategory(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.CategoryName), "Loại hàng bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    //Update
                    bool result = CommonDataService.UpdateCategory(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.CategoryName), "Loại hàng bị trùng");
                        return View("Edit", data);
                    }
                }

                return RedirectToAction("Index");
            }catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View("Edit", data);
            }
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetCategory(id);
            if(data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
    }
}
