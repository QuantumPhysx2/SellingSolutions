using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SellingSolutions.Models;

namespace SellingSolutions.Models
{
    public class SellingSolutionsContext : DbContext
    {
        public SellingSolutionsContext (DbContextOptions<SellingSolutionsContext> options)
            : base(options)
        {
        }

        public DbSet<SellingSolutions.Models.Product> Product { get; set; }

        public DbSet<SellingSolutions.Models.Cart> Cart { get; set; }

        public DbSet<SellingSolutions.Models.Sales> Sales { get; set; }
    }
}
