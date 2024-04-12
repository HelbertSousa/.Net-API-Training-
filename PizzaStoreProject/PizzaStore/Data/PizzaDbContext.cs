using Microsoft.EntityFrameworkCore;
using PizzaStore.Models;

namespace PizzaStore.Data
{
    public class PizzaDbContext(DbContextOptions<PizzaDbContext> options) : DbContext(options)
    {
        public DbSet<Pizza> Pizzas { get; set; } = null!;
    }
}