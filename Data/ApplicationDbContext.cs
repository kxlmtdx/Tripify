using Microsoft.EntityFrameworkCore;
using TourFlow.Models;

namespace TourFlow.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Accounts { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
         : base(options)
        {

        }
    }
}
