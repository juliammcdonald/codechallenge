using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        // Added as a primary key
        public String CompensationId { get; set; }
        // Added to match convention of other API methods. One of EmployeeId or Employee will always be non-null
        public String EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public double Salary { get; set; } 
        public DateTime EffectiveDate { get; set; }
    }
}
