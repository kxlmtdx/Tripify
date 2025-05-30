﻿using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Xml.Linq;
using TourFlow.Models;

namespace TourFlow.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountType> AccountsType { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourType> ToursTypes { get; set; }

        public DbSet<Airline> Airlines { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<BookingStatusType> BookingStatusTypes { get; set; }
        public DbSet<User_Document> User_Documents { get; set; }
        public DbSet<DocumentType> Documents_Type { get; set; }
        public DbSet<FlightTicket> Flight_Tickets { get; set; }
        public DbSet<FlightType> Flight_Types { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.AccountType)
                .WithMany()
                .HasForeignKey(a => a.Account_Type_Id);

            modelBuilder.Entity<Hotel>()
                .HasOne(h => h.Direction)
                .WithMany()
                .HasForeignKey(h => h.Direction_Id);

            modelBuilder.Entity<Tour>()
                .HasOne(t => t.Direction)
                .WithMany()
                .HasForeignKey(t => t.Direction_Id);

            modelBuilder.Entity<Tour>()
                .HasOne(t => t.Hotel)
                .WithMany()
                .HasForeignKey(t => t.Hotel_Id);

            modelBuilder.Entity<Tour>()
                .HasOne(t => t.TourType)
                .WithMany()
                .HasForeignKey(t => t.Tour_Type_Id);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Account)
                .WithMany(a => a.Bookings)
                .HasForeignKey(b => b.Account_Id);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Tour)
                .WithMany()
                .HasForeignKey(b => b.Tour_Id);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Airline)
                .WithMany()
                .HasForeignKey(b => b.Airline_Id);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.BookingStatusType)
                .WithMany()
                .HasForeignKey(b => b.Booking_Status_Id);

            modelBuilder.Entity<User_Document>()
                .HasOne(d => d.Account)
                .WithMany(a => a.User_Documents)
                .HasForeignKey(d => d.Account_Id);

            modelBuilder.Entity<User_Document>()
                .HasOne(d => d.Document_Type)
                .WithMany()
                .HasForeignKey(d => d.Document_Type_Id);

            modelBuilder.Entity<FlightTicket>()
               .HasOne(ft => ft.Booking)
               .WithMany(b => b.FlightTickets)
               .HasForeignKey(ft => ft.Booking_Id);

            modelBuilder.Entity<FlightTicket>()
                .HasOne(ft => ft.FlightType)
                .WithMany()
                .HasForeignKey(ft => ft.Flight_Type_Id);

            modelBuilder.Entity<FlightTicket>()
                .HasOne(ft => ft.Airline)
                .WithMany(a => a.FlightTickets)
                .HasForeignKey(ft => ft.Airline_Id);

            modelBuilder.Entity<DocumentType>().HasData(
                new DocumentType { Document_Type_Id = 1, Document_Type = "Паспорт РФ" },
                new DocumentType { Document_Type_Id = 4, Document_Type = "Загранпаспорт" }
            );

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Hotel)
                .WithMany()
                .HasForeignKey(b => b.Hotel_Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
