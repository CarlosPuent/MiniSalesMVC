using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Rsm.MiniSalesOrder.Application.Interfaces;
using Rsm.MiniSalesOrder.Web.Models;

namespace Rsm.MiniSalesOrder.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly ISalesOrderService _salesOrderService;

        public HomeController(
            ICustomerService customerService,
            IProductService productService,
            ISalesOrderService salesOrderService)
        {
            _customerService = customerService;
            _productService = productService;
            _salesOrderService = salesOrderService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalCustomers = await _customerService.GetCustomerCountAsync(),
                TotalProducts = await _productService.GetProductCountAsync(),
                TotalOrders = await _salesOrderService.GetTotalOrderCountAsync(),
                OrdersByStatus = await _salesOrderService.GetOrderCountsByStatusAsync()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}