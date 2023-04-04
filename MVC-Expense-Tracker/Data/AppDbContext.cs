using Microsoft.EntityFrameworkCore;
using MVC_Expense_Tracker.Models;

namespace MVC_Expense_Tracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) 
        {
        }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}
