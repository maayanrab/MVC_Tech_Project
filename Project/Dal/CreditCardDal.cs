using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Project.Models;

namespace Project.Dal
{
    public class CreditCardDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CreditCard>().ToTable("credit_cards");
        }

        public DbSet<CreditCard> CreditCards { get; set; }
    }
}