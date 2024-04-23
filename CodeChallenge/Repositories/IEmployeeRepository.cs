using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);
        Employee GetByIdWithDR(string id);
        Employee Add(Employee employee);
        Employee Remove(Employee employee);
        List<Compensation> GetCompByEmpId(String empId);
        Compensation AddComp(Compensation compensation);
        Task SaveAsync();
    }
}