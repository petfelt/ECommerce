using Microsoft.EntityFrameworkCore;
using ECommerce.Models;

namespace ECommerce.Models
{
    public class ECommerceContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public ECommerceContext(DbContextOptions<ECommerceContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Invitations> Invitations { get; set; }
        public DbSet<Connections> Connections { get; set; }

    }
}