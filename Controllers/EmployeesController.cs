using wcom.Models;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace wcom.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("/api/[Controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetEmployees()
        {
            try
            {
                return Ok(await employeeRepository.GetEmployees());
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, "error retrieving data from db");
            }

        }
        [HttpGet("{search}")]
        public async Task<ActionResult<IEnumerable<Employee>>> Search(string name,  Gender? gender)
        {
            try
            {
              var result=await  employeeRepository.Search(name, gender);
                if(result.Any())
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetEmployee(int id)
        {
            try
            {
                var result = await employeeRepository.GetEmployee(id);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "error retrieving data from db ");
            }
        }
        [HttpPost]
        public async Task<ActionResult> CreateEmployee(Employee employee)
        {
            try
            {
                if (employee == null) { return BadRequest(); }

                var emp = await employeeRepository.GetEmployeeByEmail(employee.Email);
                if (emp != null)
                {
                    ModelState.AddModelError("email", "email already in user");
                    return BadRequest(ModelState);
                }
                var createdEmployee = await employeeRepository.AddEmployee(employee);
                return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.EmployeeId }, createdEmployee);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "error retrieving data from db");
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(int id)
        {
            try {
                var employeeToDelete = await employeeRepository.GetEmployee(id);
                if (employeeToDelete == null) return NotFound($"Employee with Id = {id} not found");

                return await employeeRepository.DeleteEmployee(id);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "error retrieving data from db " + ex.Message);
            }
        }

        //[HttpPut("{id:int}")]
        [HttpPut]
        public async Task<ActionResult<Employee>> UpdateEmployee([FromBody] Employee employee)
        {
            try
            {
                var employeeToUpdate = await employeeRepository.GetEmployee(employee.EmployeeId); 
                if (employeeToUpdate == null)
                    {
                        return NotFound($"Employee with Id = {employee.EmployeeId} not found");
                    }

                return await employeeRepository.UpdateEmployee(employee);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }

        }
       

    }
}
