using Microsoft.AspNetCore.Mvc;
using Rsm.MiniSalesOrder.Application.Interfaces;
using Rsm.MiniSalesOrder.Domain.Entities;
using Rsm.MiniSalesOrder.Web.Models;

namespace Rsm.MiniSalesOrder.Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var viewModels = customers.Select(MapToViewModel).ToList();

            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer is null)
            {
                return NotFound();
            }

            return View(MapToViewModel(customer));
        }

        public IActionResult Create()
        {
            return View(new CustomerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var customer = new Customer
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                Phone = viewModel.Phone
            };

            await _customerService.CreateCustomerAsync(customer);

            TempData["SuccessMessage"] = "Cliente creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer is null)
            {
                return NotFound();
            }

            return View(MapToViewModel(customer));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var customer = new Customer
            {
                Id = viewModel.Id,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                Phone = viewModel.Phone
            };

            var updatedCustomer = await _customerService.UpdateCustomerAsync(customer);
            if (updatedCustomer is null)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Cliente actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer is null)
            {
                return NotFound();
            }

            return View(MapToViewModel(customer));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer is null)
            {
                return NotFound();
            }

            var deleted = await _customerService.DeleteCustomerAsync(id);
            if (!deleted)
            {
                ModelState.AddModelError(string.Empty, "No se puede eliminar el cliente porque tiene órdenes asociadas.");
                return View("Delete", MapToViewModel(customer));
            }

            TempData["SuccessMessage"] = "Cliente eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private static CustomerViewModel MapToViewModel(Customer customer)
        {
            return new CustomerViewModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                CreatedAt = customer.CreatedAt
            };
        }
    }
}