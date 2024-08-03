using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using wcom.Models;

namespace wcom.Models
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext appDbContext;

        public EmployeeRepository( AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task<Employee> AddEmployee(Employee employee)
        {
            var result=await appDbContext.Employees.AddAsync(employee);
            await appDbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Employee> DeleteEmployee(int employeeID)
        {
            var result= await appDbContext.Employees.FirstOrDefaultAsync(e=>e.EmployeeId==employeeID);
            if (result != null)
            {
                appDbContext.Employees.Remove(result);
                await appDbContext.SaveChangesAsync();
                return result;
            }
            return null;
        }

        public async Task<Employee> GetEmployee(int employeeID)
        {
            //return await appDbContext.Employees.Include(e=>e.department).FirstOrDefaultAsync(e => e.EmployeeId == employeeID);
            return await appDbContext.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeID);

        }

        public async Task<Employee> GetEmployeeByEmail(string email)
        {
            return await appDbContext.Employees.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            return await appDbContext.Employees.ToListAsync();
        }

        public async Task<IEnumerable<Employee>> Search(string name, Gender? gender)
        {
            IQueryable<Employee> query = appDbContext.Employees;
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.FirstName.Contains(name)||e.LastName.Contains(name));
            }
            if (gender!=null)
            {
                query = query.Where(e => e.Gender==gender);
            }
            return await query.ToListAsync();
        }

        public async Task<Employee> UpdateEmployee(Employee employee)
        {
            var result= await appDbContext.Employees.FirstOrDefaultAsync(e=>e.EmployeeId == employee.EmployeeId);
            if(result!=null)
            {
                result.FirstName = employee.FirstName;
                result.LastName = employee.LastName;
                result.Email = employee.Email;
                result.DateOfBirth = employee.DateOfBirth;
                result.Gender = employee.Gender;
                result.DepartmentId = employee.DepartmentId;
                result.PhotoPath = employee.PhotoPath?? "images/jon.png";
                await appDbContext.SaveChangesAsync();
                return result;
            }
            return null;
        }

       
    }
}
