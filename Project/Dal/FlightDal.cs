using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Project.Models;

namespace Project.Dal
{
    public class FlightDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Flight>().ToTable("flights");
        }

        public DbSet<Flight> Flights { get; set; }
    }
}