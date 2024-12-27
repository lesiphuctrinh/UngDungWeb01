using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020777.BusinessLayers;
using SV21T1020777.DomainModels;
using SV21T1020777.Web.AppCodes;
using SV21T1020777.Web.Models;

namespace SV21T1020777.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR},{WebUserRoles.EMPLOYEE}")]
    public class SupplierController : Controller
    {
        private const int PAGE_SIZE = 2;
        private const string SUPPLIER_SEARCH_CONDITION = "SupplierSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfSupplier(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            SupplierSearchResult model = new SupplierSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            var data = new Supplier()
            {
                SupplierID = 0, // Khi gan id bang 0 se bo sung
            };
          return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
            var data = CommonDataService.GetSupplier(id);
            if(data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Supplier data)
        {
            try
            {
                //kiểm soát dữ liệu đầu vào
                ViewBag.Title = data.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhật thông tin nhà cung cấp";
                //kiểm tra dữ liệu đầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào ModelSate
                if (string.IsNullOrWhiteSpace(data.SupplierName))
                    ModelState.AddModelError(nameof(data.SupplierName), "Tên nhà cung cấp không để trống");
                if (string.IsNullOrWhiteSpace(data.ContactName))
                    ModelState.AddModelError(nameof(data.ContactName), "Thông tin giao dịch không để trống");
                if (string.IsNullOrWhiteSpace(data.Phone))
                    ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại nhà cung cấp");
                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email nhà cung cấp");
                if (string.IsNullOrWhiteSpace(data.Address))
                    ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ nhà cung cấp");
                if (string.IsNullOrEmpty(data.Province))
                    ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành cho nhà cung cấp");

                //dựa vào thuộc tính IsValid của ModelSate để biết có tồn tại lỗi hay không?
                if (ModelState.IsValid == false)
                {
                    return View("Edit", data);
                }

                if (data.SupplierID == 0)
                {
                    //Thêm
                    int id = CommonDataService.AddSupplier(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    // Cập nhật
                    bool result = CommonDataService.UpdateSupplier(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View("Edit", data);
            }
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }

            var data = CommonDataService.GetSupplier(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }
}
