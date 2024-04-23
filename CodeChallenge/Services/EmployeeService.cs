using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;
using Microsoft.AspNetCore.Components.Forms;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }
        /// <summary>
        /// Private helper method to fetch an Employee with its
        /// DirectReports list. Validates employee is non-null/empty
        /// </summary>
        /// <param name="id">EmployeeID</param>
        /// <returns>Employee corresponding to id with DirectReports object, null otherwise</returns>
        private Employee GetByIdWithDR(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetByIdWithDR(id);
            }
            return null;
        }

        /// <summary>
        /// Calculates the reporting structure of the specified employee.
        /// Calls a separate method to recursively sum the sizes of
        /// DirectReports lists
        /// </summary>
        /// <param name="employee">the Employee object whose total reports are to be calculated. Only requires employeeId</param>
        /// <returns>ReportingStructure object containing the Employee and calculated int NumberOfReports</returns>
        public ReportingStructure GetReportingStructure(Employee employee)
        {
            // Calculate number of reports
            var sum = GetDirectReports(employee);

            // Add results to a reporting structure object
            ReportingStructure structure = new ReportingStructure()
            {
                Employee = employee,
                NumberOfReports = sum
            };

            return structure;
        }

        // Assumption: An employee cannot have two direct bosses
        /// <summary>
        /// Private helper method to sum the total reports of an
        /// employee. Assumes that an employee cannot have two direct
        /// bosses, thus the org chart is a valid tree. A better
        /// long-term solution would be to annotate each employee
        /// with its total reports and update to the root of
        /// the org chart on addition of an employee.
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        private int GetDirectReports(Employee employee)
        {
            employee = GetByIdWithDR(employee.EmployeeId);
            
            if(employee.DirectReports == null || employee.DirectReports.Count == 0)
            {
                return 0;
            }
            int sum = employee.DirectReports.Count; // direct reports

            foreach(Employee emp in employee.DirectReports)
            {
                sum += GetDirectReports(emp); // direct reports of direct reports
            }
            return sum;
        }
        /// <summary>
        /// Creates a new Compensation object for an existing employee.
        /// Either EmployeeId or Employee must be non-null. Assumes
        /// that EmployeeId and Employee.EmployeeId match if both
        /// non-null. 
        /// </summary>
        /// <param name="compensation">the Compensation object to be added</param>
        /// <returns>Compensation object that has been added, null otherwise</returns>
        public Compensation CreateComp(Compensation compensation)
        {
            if(compensation != null &&compensation.Employee == null)
            {
                if (compensation.Employee == null)
                {
                    Employee emp = GetById(compensation.EmployeeId);
                    if (emp == null) return null;
                    compensation.Employee = emp;
                }
                _employeeRepository.AddComp(compensation);
                _employeeRepository.SaveAsync().Wait();

                return compensation;
            }

            return null;
        }

        /// <summary>
        /// Fetches the historical list of Compensations for a 
        /// specified employee, based on employeeId, from the 
        /// Compensation table.
        /// </summary>
        /// <param name="empId">the employeeId of the employee to be queried</param>
        /// <returns>a list of all historical Compensation objects, ordered by date</returns>
        public List<Compensation> GetCompByEmpId(string empId)
        {
            if (!String.IsNullOrEmpty(empId))
            {
                return _employeeRepository.GetCompByEmpId(empId);
            }

            return null;
        }
    }
}
