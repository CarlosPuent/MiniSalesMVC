using Microsoft.AspNetCore.Mvc;
using Rsm.MiniSalesOrder.Application.Interfaces;
using Rsm.MiniSalesOrder.Domain.Entities;
using Rsm.MiniSalesOrder.Web.Models;

namespace Rsm.MiniSalesOrder.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            var viewModels = products.Select(MapToViewModel).ToList();

            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            return View(MapToViewModel(product));
        }

        public IActionResult Create()
        {
            return View(new ProductViewModel
            {
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var product = new Product
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                UnitPrice = viewModel.UnitPrice,
                Stock = viewModel.Stock,
                IsActive = viewModel.IsActive
            };

            await _productService.CreateProductAsync(product);

            TempData["SuccessMessage"] = "Producto creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            return View(MapToViewModel(product));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var product = new Product
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description,
                UnitPrice = viewModel.UnitPrice,
                Stock = viewModel.Stock,
                IsActive = viewModel.IsActive
            };

            var updatedProduct = await _productService.UpdateProductAsync(product);
            if (updatedProduct is null)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Producto actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            return View(MapToViewModel(product));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
            {
                ModelState.AddModelError(string.Empty, "No se puede eliminar el producto porque está siendo utilizado en órdenes.");
                return View("Delete", MapToViewModel(product));
            }

            TempData["SuccessMessage"] = "Producto eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private static ProductViewModel MapToViewModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                Stock = product.Stock,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };
        }
    }
}