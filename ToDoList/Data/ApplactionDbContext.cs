using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.Data
{
    public class ApplactionDbContext : DbContext
    {
        public DbSet<ToDoItem> ToDoItems { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ToDoList;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True");
        }
    }
}
