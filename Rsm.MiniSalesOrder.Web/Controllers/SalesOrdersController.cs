using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rsm.MiniSalesOrder.Application.Interfaces;
using Rsm.MiniSalesOrder.Domain.Entities;
using Rsm.MiniSalesOrder.Domain.Enums;
using Rsm.MiniSalesOrder.Web.Models;

namespace Rsm.MiniSalesOrder.Web.Controllers
{
    public class SalesOrdersController : Controller
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IShipmentService _shipmentService;
        private readonly IInvoiceService _invoiceService;

        public SalesOrdersController(
            ISalesOrderService salesOrderService,
            ICustomerService customerService,
            IProductService productService,
            IShipmentService shipmentService,
            IInvoiceService invoiceService)
        {
            _salesOrderService = salesOrderService;
            _customerService = customerService;
            _productService = productService;
            _shipmentService = shipmentService;
            _invoiceService = invoiceService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _salesOrderService.GetAllSalesOrdersAsync();
            var viewModels = orders.Select(MapSalesOrderToViewModel).ToList();

            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _salesOrderService.GetSalesOrderByIdAsync(id);
            if (order is null)
            {
                return NotFound();
            }

            return View(MapSalesOrderToViewModel(order));
        }

        public async Task<IActionResult> Create()
        {
            await LoadCustomersAsync();
            return View(new SalesOrderCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesOrderCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await LoadCustomersAsync(viewModel.CustomerId);
                return View(viewModel);
            }

            var order = new SalesOrder
            {
                CustomerId = viewModel.CustomerId,
                Notes = viewModel.Notes ?? string.Empty
            };

            var createdOrder = await _salesOrderService.CreateSalesOrderAsync(order);

            TempData["SuccessMessage"] = "Orden creada correctamente.";
            return RedirectToAction(nameof(Details), new { id = createdOrder.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _salesOrderService.GetSalesOrderByIdAsync(id);
            if (order is null)
            {
                return NotFound();
            }

            var viewModel = new SalesOrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer is null
                    ? string.Empty
                    : $"{order.Customer.FirstName} {order.Customer.LastName}",
                OrderDate = order.OrderDate,
                Status = order.Status,
                Subtotal = order.Subtotal,
                Tax = order.Tax,
                Total = order.Total,
                Notes = order.Notes
            };

            await LoadCustomersAsync(order.CustomerId);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SalesOrderViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await LoadCustomersAsync(viewModel.CustomerId);
                return View(viewModel);
            }

            var order = new SalesOrder
            {
                Id = viewModel.Id,
                Notes = viewModel.Notes ?? string.Empty
            };

            var updatedOrder = await _salesOrderService.UpdateSalesOrderAsync(order);
            if (updatedOrder is null)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Orden actualizada correctamente.";
            return RedirectToAction(nameof(Details), new { id = viewModel.Id });
        }

        public async Task<IActionResult> AddItem(int id)
        {
            var order = await _salesOrderService.GetSalesOrderByIdAsync(id);
            if (order is null)
            {
                return NotFound();
            }

            if (order.Status == SalesOrderStatus.Shipped || order.Status == SalesOrderStatus.Invoiced)
            {
                TempData["ErrorMessage"] = "No se pueden agregar artículos a una orden enviada o facturada.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var viewModel = new SalesOrderItemViewModel
            {
                SalesOrderId = id
            };

            await LoadProductsAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(SalesOrderItemViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await LoadProductsAsync(viewModel.ProductId);
                return View(viewModel);
            }

            var item = new SalesOrderItem
            {
                SalesOrderId = viewModel.SalesOrderId,
                ProductId = viewModel.ProductId,
                Quantity = viewModel.Quantity
            };

            var updatedOrder = await _salesOrderService.AddItemToOrderAsync(viewModel.SalesOrderId, item);
            if (updatedOrder is null)
            {
                ModelState.AddModelError(string.Empty, "No se pudo agregar el artículo. Verifica stock, producto y estado de la orden.");
                await LoadProductsAsync(viewModel.ProductId);
                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Artículo agregado correctamente.";
            return RedirectToAction(nameof(Details), new { id = viewModel.SalesOrderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int orderId, int itemId)
        {
            var updatedOrder = await _salesOrderService.RemoveItemFromOrderAsync(orderId, itemId);
            if (updatedOrder is null)
            {
                TempData["ErrorMessage"] = "No se pudo eliminar el artículo de la orden.";
                return RedirectToAction(nameof(Details), new { id = orderId });
            }

            TempData["SuccessMessage"] = "Artículo eliminado correctamente.";
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, SalesOrderStatus newStatus)
        {
            var updatedOrder = await _salesOrderService.UpdateOrderStatusAsync(id, newStatus);
            if (updatedOrder is null)
            {
                TempData["ErrorMessage"] = "No se pudo cambiar el estado de la orden. Verifica el flujo y los requisitos previos.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = "Estado de la orden actualizado correctamente.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> CreateShipment(int id)
        {
            var order = await _salesOrderService.GetSalesOrderByIdAsync(id);
            if (order is null || order.Status != SalesOrderStatus.Processing)
            {
                return NotFound();
            }

            var existingShipment = await _shipmentService.GetShipmentByOrderIdAsync(id);
            if (existingShipment is not null)
            {
                TempData["ErrorMessage"] = "La orden ya tiene un envío registrado.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var viewModel = new ShipmentViewModel
            {
                SalesOrderId = id,
                ShippingDate = DateTime.UtcNow
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShipment(ShipmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var shipment = new Shipment
            {
                SalesOrderId = viewModel.SalesOrderId,
                ShippingAddress = viewModel.ShippingAddress,
                TrackingNumber = viewModel.TrackingNumber,
                Carrier = viewModel.Carrier
            };

            var createdShipment = await _shipmentService.CreateShipmentAsync(shipment);
            if (createdShipment is null)
            {
                ModelState.AddModelError(string.Empty, "No se pudo registrar el envío para esta orden.");
                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Envío registrado correctamente.";
            return RedirectToAction(nameof(Details), new { id = viewModel.SalesOrderId });
        }

        public async Task<IActionResult> CreateInvoice(int id)
        {
            var order = await _salesOrderService.GetSalesOrderByIdAsync(id);
            if (order is null || order.Status != SalesOrderStatus.Shipped)
            {
                return NotFound();
            }

            var existingInvoice = await _invoiceService.GetInvoiceByOrderIdAsync(id);
            if (existingInvoice is not null)
            {
                TempData["ErrorMessage"] = "La orden ya tiene una factura generada.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var viewModel = new InvoiceViewModel
            {
                SalesOrderId = id,
                InvoiceDate = DateTime.UtcNow,
                Amount = order.Total
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvoice(InvoiceViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var invoice = new Invoice
            {
                SalesOrderId = viewModel.SalesOrderId
            };

            var createdInvoice = await _invoiceService.CreateInvoiceAsync(invoice);
            if (createdInvoice is null)
            {
                ModelState.AddModelError(string.Empty, "No se pudo generar la factura para esta orden.");
                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Factura generada correctamente.";
            return RedirectToAction(nameof(Details), new { id = viewModel.SalesOrderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInvoice(int id, bool paid)
        {
            var invoice = await _invoiceService.GetInvoiceByOrderIdAsync(id);
            if (invoice is null)
            {
                return NotFound();
            }

            invoice.Paid = paid;

            var updatedInvoice = await _invoiceService.UpdateInvoiceAsync(invoice);
            if (updatedInvoice is null)
            {
                TempData["ErrorMessage"] = "No se pudo actualizar el estado de la factura.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = paid
                ? "Factura marcada como pagada."
                : "Estado de factura actualizado.";

            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task LoadCustomersAsync(int? selectedCustomerId = null)
        {
            var customers = await _customerService.GetAllCustomersAsync();

            ViewBag.Customers = new SelectList(
                customers.Select(c => new
                {
                    c.Id,
                    FullName = $"{c.FirstName} {c.LastName}"
                }),
                "Id",
                "FullName",
                selectedCustomerId
            );
        }

        private async Task LoadProductsAsync(int? selectedProductId = null)
        {
            var products = await _productService.GetAllProductsAsync();

            ViewBag.Products = new SelectList(
                products
                    .Where(p => p.IsActive && p.Stock > 0)
                    .Select(p => new
                    {
                        p.Id,
                        Name = $"{p.Name} (Stock: {p.Stock})"
                    }),
                "Id",
                "Name",
                selectedProductId
            );
        }

        private static SalesOrderViewModel MapSalesOrderToViewModel(SalesOrder order)
        {
            return new SalesOrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer is null
                    ? string.Empty
                    : $"{order.Customer.FirstName} {order.Customer.LastName}",
                OrderDate = order.OrderDate,
                Status = order.Status,
                Subtotal = order.Subtotal,
                Tax = order.Tax,
                Total = order.Total,
                Notes = order.Notes,
                Items = order.Items.Select(i => new SalesOrderItemViewModel
                {
                    Id = i.Id,
                    SalesOrderId = i.SalesOrderId,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.LineTotal
                }).ToList(),
                Shipment = order.Shipment is null
                    ? null
                    : new ShipmentViewModel
                    {
                        Id = order.Shipment.Id,
                        SalesOrderId = order.Shipment.SalesOrderId,
                        ShippingAddress = order.Shipment.ShippingAddress,
                        ShippingDate = order.Shipment.ShippingDate,
                        TrackingNumber = order.Shipment.TrackingNumber,
                        Carrier = order.Shipment.Carrier,
                        Status = order.Shipment.Status
                    },
                Invoice = order.Invoice is null
                    ? null
                    : new InvoiceViewModel
                    {
                        Id = order.Invoice.Id,
                        SalesOrderId = order.Invoice.SalesOrderId,
                        InvoiceNumber = order.Invoice.InvoiceNumber,
                        InvoiceDate = order.Invoice.InvoiceDate,
                        Amount = order.Invoice.Amount,
                        Paid = order.Invoice.Paid
                    }
            };
        }
    }
}