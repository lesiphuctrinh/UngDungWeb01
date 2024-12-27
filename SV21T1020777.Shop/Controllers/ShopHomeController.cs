using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020777.BusinessLayers;
using SV21T1020777.Shop.AppCodes;
using SV21T1020777.Shop.Models;
using System.Diagnostics;

namespace SV21T1020777.Shop.Controllers
{
    [Authorize]
    public class ShopHomeController : Controller
    {
        public const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        private const int PAGE_SIZE = 6;

        private readonly ILogger<ShopHomeController> _logger;

        public ShopHomeController(ILogger<ShopHomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if(condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    CategoryID = 0,
                    SearchValue = ""
                };
            }
            return View(condition);
        }
        public IActionResult Search(ProductSearchInput condition)
        {
            int rowCount = 0;
            var data = ProductDataService.ListOfProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "", condition.CategoryID);
            var model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                CategoryID = condition.CategoryID,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
