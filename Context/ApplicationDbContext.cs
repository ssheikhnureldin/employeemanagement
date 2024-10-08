using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeMangement.Context
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>contextOptions)

        : base(contextOptions)
        {

        }
        // Code - Approach

        public DbSet<Employee> Employees { get; set; }

    }
}
