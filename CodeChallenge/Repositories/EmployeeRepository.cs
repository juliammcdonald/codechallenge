using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRepository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            return _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        /// <summary>
        /// Fetches a list of compensation objects from the database
        /// Checks that employee exists based on employeeId
        /// </summary>
        /// <param name="empId">employeeId</param>
        /// <returns>If valid employee and existing Compensation objects, list of Compensation objects; null otherwise</returns>
        public List<Compensation> GetCompByEmpId(String empId)
        {
            Employee employee = GetById(empId);
            if (employee != null)
            {
                return _employeeContext.Compensation.Where(c => c.Employee.EmployeeId == empId).OrderBy(c => c.EffectiveDate).ToList();
            }
            return null;
        }

        /// <summary>
        /// Adds a validated Compensation object to the database.
        /// Generates a guid identifier and adds to object.
        /// </summary>
        /// <param name="compensation">new Compensation object</param>
        /// <returns>Compensation object that has been added</returns>
        public Compensation AddComp(Compensation compensation)
        {
            compensation.CompensationId = Guid.NewGuid().ToString();
            _employeeContext.Compensation.Add(compensation);
            return compensation;
        }

        /// <summary>
        /// GetById does not return the DirectReports object by default
        /// so this method has been added. Used only when DirectReports
        /// are required.
        /// </summary>
        /// <param name="id">employeeId</param>
        /// <returns>Employee object containing all fields including DirectReports list</returns>
        public Employee GetByIdWithDR(string id)
        {
            return _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
        }
    }
}
