using Microsoft.EntityFrameworkCore;
using Rsm.MiniSalesOrder.Application.Interfaces;
using Rsm.MiniSalesOrder.Domain.Entities;
using Rsm.MiniSalesOrder.Infrastructure.Data;

namespace Rsm.MiniSalesOrder.Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            customer.CreatedAt = DateTime.UtcNow;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer?> UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = await _context.Customers.FindAsync(customer.Id);
            if (existingCustomer is null)
            {
                return null;
            }

            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.LastName = customer.LastName;
            existingCustomer.Email = customer.Email;
            existingCustomer.Phone = customer.Phone;

            await _context.SaveChangesAsync();
            return existingCustomer;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is null)
            {
                return false;
            }

            var hasOrders = await _context.SalesOrders.AnyAsync(o => o.CustomerId == id);
            if (hasOrders)
            {
                return false;
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetCustomerCountAsync()
        {
            return await _context.Customers.CountAsync();
        }
    }
}