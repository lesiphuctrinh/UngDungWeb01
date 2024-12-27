using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020777.BusinessLayers;
using SV21T1020777.DomainModels;
using SV21T1020777.Web.AppCodes;
using SV21T1020777.Web.Models;

namespace SV21T1020777.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR},{WebUserRoles.EMPLOYEE}")]
    public class ShipperController : Controller
    {
        private const int PAGE_SIZE = 2;
        private const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfShippers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            ShipperSearchResult model = new ShipperSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng";
            var data = new Shipper()
            {
                ShipperID = 0, // khi gán id bằng 0 thì bổ sung
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin người giao hàng";
            var data = CommonDataService.GetShipper(id);
            if(data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            try
            {
                //kiểm soát dữ liệu đầu vào
                ViewBag.Title = data.ShipperID == 0 ? "Bổ sung người giao hàng" : "Cập nhật thông tin người giao hàng";
                //kiểm tra dữ liệu đầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu vào ModelSate
                if (string.IsNullOrWhiteSpace(data.ShipperName))
                {
                    ModelState.AddModelError(nameof(data.ShipperName), "Tên người giao hàng không để trống");
                }
                if (string.IsNullOrWhiteSpace(data.Phone))
                {
                    ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại người giao hàng");
                }

                //dựa vào thuộc tính IsValid của ModelSate để biết có tồn tại lỗi hay không?
                if (ModelState.IsValid == false)
                {
                    return View("Edit", data);
                }

                if (data.ShipperID == 0)
                {
                    // Thêm
                    CommonDataService.AddShipper(data);
                }
                else
                {
                    // Cập nhật
                    CommonDataService.UpdateShipper(data);
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
            if(Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetShipper(id);
            if(data == null)
                  return RedirectToAction("Index");
            return View(data);
        }
    }
}
