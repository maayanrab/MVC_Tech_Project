using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Project.Models;

namespace Project.Dal
{
    public class TicketDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Ticket>().ToTable("tickets");
        }

        public DbSet<Ticket> Tickets { get; set; }
    }
}