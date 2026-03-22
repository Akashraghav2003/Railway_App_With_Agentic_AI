using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Entity
{
    public class RailwayContext : DbContext
    {
        
        
        public RailwayContext(DbContextOptions<RailwayContext> options): base(options){}
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Cancellation> Cancellations { get; set; } 
        public DbSet<TrainClass> TrainClasses { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Passenger>()
                .HasOne(p => p.Reservation)
                .WithMany(r => r.Passenger)
                .HasForeignKey(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserID);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Train)
                .WithMany(t => t.Reservations)
                .HasForeignKey(r => r.TrainId);


  
            modelBuilder.Entity<Train>()
                .HasMany(t => t.Reservations)
                .WithOne(r => r.Train)
                .HasForeignKey(r => r.TrainId);

            
            modelBuilder.Entity<Train>()
                .HasMany(t => t.TrainClasses)
                .WithOne(tc => tc.Train)
                .HasForeignKey(tc => tc.TrainId);


            modelBuilder.Entity<User>()
                .HasMany(u => u.Reservations)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserID);
        }
    }
}

