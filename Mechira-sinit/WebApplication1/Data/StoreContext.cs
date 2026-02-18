using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }
        public DbSet<User> useres => Set<User>();
        public DbSet<Gift> gifts => Set<Gift>();
        public DbSet<Donor> donors => Set<Donor>();
        public DbSet<Order> orders => Set<Order>();
        public DbSet<Category> categories => Set<Category>();
        public DbSet<Lottery> lotteries => Set<Lottery>();
    }
}
