using Microsoft.EntityFrameworkCore;
using SeviceLayer.Context;
using SeviceLayer.Models;

namespace SeviceLayer.Services
{
    public class EmployeeService
    {
        private readonly AppDbContext _context = new();

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<List<Employee>> GetBaseInfoAboutEmployess()
        {
            return await _context.Employees
                .Include(d => d.Department)
                .Include(p => p.Position)
                .ToListAsync();
        }

        public async Task<bool> UpdateEmployee(Employee employee)
        {
            try
            {
                var existing = await _context.Employees.FindAsync(employee.Id);
                if (existing == null) return false;

                existing.Surname = employee.Surname;
                existing.Name = employee.Name;
                existing.Patronymic = employee.Patronymic;
                existing.Birthday = employee.Birthday;
                existing.Phone = employee.Phone;
                existing.Email = employee.Email;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
