using Microsoft.EntityFrameworkCore;
using RaceDay.Models;

namespace RaceDay.Data
{//start of namespace
    public class RaceDayContext : DbContext
    {//start of class

       public RaceDayContext(DbContextOptions<RaceDayContext>options) : base(options) {
        
        }
        
        //represents the user table
       public DbSet<User> Users { get; set; } 

        //represents the Event type table
        public DbSet<EventType> EventTypes { get; set; }

        //represents the event table
        public DbSet<Event> Events { get; set; }

        //represents the category table
        public DbSet<Categorys> Categories { get; set; }

        //represents the Enrollment table
        public DbSet<Enrollment> Enrollments { get; set; }

        //reperesents the result table
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>().HasIndex(user => user.Email).IsUnique();

         modelBuilder.Entity<EventType>().HasIndex(eventType => eventType.TypeName).IsUnique();

        modelBuilder.Entity<Event>().Property(eventItem => eventItem.EventDate).HasColumnType("date");

            modelBuilder.Entity<Event>().Property(eventItem => eventItem.Distance).HasColumnType("decimal(6,2)");

            modelBuilder.Entity<Enrollment>().Property(enrollment => enrollment.EnrollmentDate).HasColumnType("date");

            // Link each event to the user who organises it.
            modelBuilder.Entity<Event>().HasOne<User>().WithMany().HasForeignKey(eventItem => eventItem.OrganiserID).OnDelete(DeleteBehavior.NoAction);

            // Link each event to its event type.
            modelBuilder.Entity<Event>().HasOne<EventType>().WithMany().HasForeignKey(eventItem => eventItem.EventTypeID).OnDelete(DeleteBehavior.NoAction);

            // Link each category to its event.
            modelBuilder.Entity<Categorys>().HasOne<Event>().WithMany().HasForeignKey(category => category.EventID).OnDelete(DeleteBehavior.NoAction);

            // Link each enrollment to its participant.
            modelBuilder.Entity<Enrollment>().HasOne<User>().WithMany().HasForeignKey(enrollment => enrollment.ParticipantID).OnDelete(DeleteBehavior.NoAction);

            // Link each enrollment to its event.
            modelBuilder.Entity<Enrollment>().HasOne<Event>().WithMany().HasForeignKey(enrollment => enrollment.EventID).OnDelete(DeleteBehavior.NoAction);

            // Link each enrollment to its selected category.
            modelBuilder.Entity<Enrollment>().HasOne<Categorys>().WithMany().HasForeignKey(enrollment => enrollment.CategoryID).OnDelete(DeleteBehavior.NoAction);

            // Link each result to its enrollment.
            modelBuilder.Entity<Result>().HasOne<Enrollment>().WithMany().HasForeignKey(result => result.EnrollmentID).OnDelete(DeleteBehavior.NoAction);
        }

    }//end of namespace 
}//end of namespace
