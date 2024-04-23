using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        /// <summary>
        /// Calculates the total number of reports for a specified user
        /// and returns a ReportingStructure object. Does not persist
        /// ReportingStructure object.
        /// </summary>
        /// <param name="id">EmployeeId</param>
        /// <returns>If valid, ReportingStructure obejct containing employee and numberOfReports; NotFound otherwise</returns>
        [Route("reportingstructure/{id}")]
        [HttpGet]
        public IActionResult GetReportingStructureById(String id)
        {
            _logger.LogDebug($"Recieved reporting structure get request for employee '{id}'");
            
            var employee = _employeeService.GetById(id);
            if (employee == null) return NotFound(id);

            // employee service call to make new reporting structure
            var reportingStructure = _employeeService.GetReportingStructure(employee);

            return Ok(reportingStructure);
        }

        /// <summary>
        /// Creates a new Compensation object for an existing employee
        /// based on Compensation object specified in Body. 
        /// Assumes the Compensation object is valid, but checks for 
        /// null values and invalid employees. Adds Compensation object
        /// to Compensation table. To match the convention of other
        /// methods, an employee object or employeeId may be provided.
        /// Assumes that employee object and employeeId match if both
        /// are provided.
        /// </summary>
        /// <param name="comp">Compensation data to be added. Includes employeeId, employee, salary, and effectiveDate. One of employeeId or employee must be non-null.</param>
        /// <returns>If valid Compensation, the newly created Compensation object compensationId; NotFound otherwise</returns>
        [Route("compensation")]
        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation comp)
        {
            if (comp == null || (comp.EmployeeId == null && comp.Employee == null)) return NotFound();
            
            _logger.LogDebug($"Recieved compensation create request for '{comp.EmployeeId}' '{comp.Salary}' '{comp.EffectiveDate}'");
            
            var newCompensation = _employeeService.CreateComp(comp);

            if(newCompensation == null) return NotFound();

            return CreatedAtRoute("getCompensationByEmpId", new { id = newCompensation.CompensationId }, comp);
        }

        /// <summary>
        /// Returns the entire Compensation history for a specified
        /// employeeId as a List. Another way to interpret this task 
        /// is returning the current valid Compensation object based
        /// on effectiveDate. 
        /// </summary>
        /// <param name="id">employeeId</param>
        /// <returns>If valid id, list of Compensations for that employee; NotFound otherwise</returns>
        [Route("compensation/{id}", Name="getCompensationByEmpId")]
        [HttpGet]
        public IActionResult GetCompensationByEmpId(String id)
        {
            _logger.LogDebug($"Received compensation get request for employee '{id}'");

            var comp = _employeeService.GetCompByEmpId(id);

            if (comp == null)
                return NotFound();

            return Ok(comp);
        }
    }
}
