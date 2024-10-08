using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EmployeeManagement.Controllers;
using EmployeeManagement.Models;
using EmployeeMangement.Context;

namespace EmployeeManagement.Tests
{
    public class EmployeeControllerTests
    {
        private EmployeeController _controller;
        private ApplicationDbContext _context;

        public EmployeeControllerTests()
        {
            // Use the InMemoryDatabase for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new ApplicationDbContext(options);
            SeedDatabase();
            _controller = new EmployeeController(_context);
        }

        // Seed the in-memory database with sample data
       private void SeedDatabase()
{
    // Clear existing data to avoid conflicts during multiple test runs
    _context.Employees.RemoveRange(_context.Employees);
    _context.SaveChanges();

    var employees = new List<Employee>
    {
        new Employee { FirstName = "John", LastName = "Doe", Email = "john@example.com", Department = "HR" },
        new Employee { FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", Department = "IT" }
    };

    // Let EF Core automatically generate Ids
    _context.Employees.AddRange(employees);
    _context.SaveChanges();
}

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfEmployees()
        {
            // Act
            var result = await _controller.Index(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Employee>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithCorrectEmployee()
        {
            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Employee>(viewResult.ViewData.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("John", model.FirstName);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_PostAddsEmployee_ReturnsRedirectToIndex()
        {
            // Arrange
            var newEmployee = new Employee { FirstName = "Alice", LastName = "Wonder", Email = "alice@example.com", Department = "Finance" };

            // Act
            var result = await _controller.Create(newEmployee);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal(3, _context.Employees.Count());  // Ensure the employee was added
        }
        
    }
}