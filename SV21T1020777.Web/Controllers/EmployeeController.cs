using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020777.BusinessLayers;
using SV21T1020777.DomainModels;
using SV21T1020777.Web.AppCodes;
using SV21T1020777.Web.Models;

namespace SV21T1020777.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR}")]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 12;
        private const string EMPLOYEE_SEARCH_CONDITION = "EmployeeSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfEmployees(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            EmployeeSearchResult model = new EmployeeSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên mới";
            var data = new Employee()
            {
                EmployeeID = 0, // khi gán id bằng 0 thì bổ sung
                IsWorking = false,
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var data = CommonDataService.GetEmployee(id);
            if(data == null )
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Employee data, string _BirthDate, IFormFile? _Photo)
        {
            try
            {
                // xử lí ngày sinh
                DateTime? d = _BirthDate.ToDateTime();
                if (d.HasValue)
                {
                    // Kiểm tra xem ngày có nằm trong khoảng cho phép của SQL Server không
                    if (d.Value >= new DateTime(1753, 1, 1) && d.Value <= new DateTime(9999, 12, 31))
                    {
                        data.BirthDate = d.Value;

                        // Kiểm tra thêm điều kiện về tuổi (phải từ 18 trở lên)
                        if (d.Value > DateTime.Now.AddYears(-18))
                        {
                            ModelState.AddModelError(nameof(data.BirthDate), "Nhân viên phải từ 18 tuổi trở lên");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh phải nằm trong khoảng từ 1/1/1753 đến 31/12/9999");
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh không hợp lệ");
                }

                // xử lí ảnh
                if (_Photo != null)
                {
                    // Lưu ảnh
                    string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                    string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images/employees", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        _Photo.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }
                    //kiểm soát dữ liệu đầu vào
                    ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên mới" : "Cập nhật thông tin nhân viên";
                //Kiểm tra dữ liệu đầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào ModelState
                if (string.IsNullOrWhiteSpace(data.FullName))
                    ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không để trống");
                if (string.IsNullOrWhiteSpace(data.Phone))
                    ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại nhân viên");
                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email nhân viên");
                if (string.IsNullOrWhiteSpace(data.Address))
                    ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ nhân viên");
                if (string.IsNullOrWhiteSpace(data.Photo))
                    ModelState.AddModelError(nameof(data.Photo), "Vui lòng cung cấp ảnh cho nhân viên");

                // dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay không?
                if (ModelState.IsValid == false)
                {
                    return View("Edit", data);
                }

                if (data.EmployeeID == 0)
                {
                    //Thêm
                    int id = CommonDataService.AddEmployee(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    // Cập nhật
                    bool result = CommonDataService.UpdateEmployee(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
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
            if(Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetEmployee(id);
            if(data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
    }
}
