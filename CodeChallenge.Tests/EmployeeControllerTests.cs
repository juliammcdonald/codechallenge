
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{ 
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetReportingStructure_JohnLennon_Returns_Four()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";
            var expectedReports = 4;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/reportingstructure/{employeeId}");
            var response = getRequestTask.Result;
            Console.WriteLine(response);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(expectedFirstName, reportingStructure.Employee.FirstName);
            Assert.AreEqual(expectedLastName, reportingStructure.Employee.LastName);
            Assert.AreEqual(expectedReports, reportingStructure.NumberOfReports);
        }

        [TestMethod]
        public void GetReportingStructure_PaulMcCartney_Returns_Zero()
        {
            // Arrange
            var employeeId = "b7839309-3348-463b-a7e3-5de1c168beb3";
            var expectedFirstName = "Paul";
            var expectedLastName = "McCartney";
            var expectedReports = 0;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/reportingstructure/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(expectedFirstName, reportingStructure.Employee.FirstName);
            Assert.AreEqual(expectedLastName, reportingStructure.Employee.LastName);
            Assert.AreEqual(expectedReports, reportingStructure.NumberOfReports);
        }

        [TestMethod]
        public void GetReportingStructure_Returns_NotFound()
        {
            // Arrange
            var employeeId = "invalid_id";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/reportingstructure/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var employeeId = "c0c2293d-16bd-4603-8e08-638a9d18b22c";
            var compensation = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = 100000.00,
                EffectiveDate = new System.DateTime(2024, 02, 01)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation/",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newComp = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newComp.CompensationId);
            Assert.AreEqual(compensation.EmployeeId, newComp.EmployeeId);
            Assert.AreEqual(compensation.Salary, newComp.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newComp.EffectiveDate);
        }

        // Added to test GetCompensation listing
        [TestMethod]
        public void CreateCompensation_Returns_Created2()
        {
            // Arrange
            var employeeId = "c0c2293d-16bd-4603-8e08-638a9d18b22c";
            var compensation = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = 95000.00,
                EffectiveDate = new System.DateTime(2022, 03, 01)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newComp = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newComp.CompensationId);
            Assert.AreEqual(compensation.EmployeeId, newComp.EmployeeId);
            Assert.AreEqual(compensation.Salary, newComp.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newComp.EffectiveDate);
        }

        // Added to test GetCompensation listing
        [TestMethod]
        public void CreateCompensation_Returns_Created3()
        {
            // Arrange
            var employeeId = "c0c2293d-16bd-4603-8e08-638a9d18b22c";
            var compensation = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = 60000.03,
                EffectiveDate = new System.DateTime(2005, 12, 01)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newComp = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newComp.CompensationId);
            Assert.AreEqual(compensation.EmployeeId, newComp.EmployeeId);
            Assert.AreEqual(compensation.Salary, newComp.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newComp.EffectiveDate);
        }

        [TestMethod]
        public void CreateCompensation_Returns_NotFound()
        {
            // Arrange
            var employeeId = "Invalid_Id";
            var compensation = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = 551234,
                EffectiveDate = new System.DateTime(2024, 04, 23)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetCompensation_Returns_CompensationList()
        {
            // Arrange
            var employeeId = "c0c2293d-16bd-4603-8e08-638a9d18b22c";
            var listLength = 3;

            var comp1 = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = 60000.03,
                EffectiveDate = new System.DateTime(2005, 12, 01)
            };
            var comp2 = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = 95000.00,
                EffectiveDate = new System.DateTime(2022, 03, 01)
            };
            var comp3 = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = 100000.00,
                EffectiveDate = new System.DateTime(2024, 02, 01)
            };
           
            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/compensation/{employeeId}");
            var response = getRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensationList = response.DeserializeContent<List<Compensation>>();
            Assert.AreEqual(listLength, compensationList.Count);
            // verifies accuracy of salary and order by date
            Assert.AreEqual(comp1.Salary, compensationList[0].Salary);
            Assert.AreEqual(comp2.Salary, compensationList[1].Salary);
            Assert.AreEqual(comp3.Salary, compensationList[2].Salary);
        }

        [TestMethod]
        public void GetCompensation_Returns_NotFound()
        {
            // Arrange
            var employeeId = "invalid_id";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Moved to end so that resulting direct reports changes do not affect ReportingStructure testing
        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }
    }
}
