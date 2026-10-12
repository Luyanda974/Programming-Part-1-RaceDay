using Microsoft.EntityFrameworkCore;
using RaceDay.Models;

namespace RaceDay.Data
{//start of namespace
    public class RaceDayContext : DbContext
    {//start of class

       public RaceDayContext(DbContextOptions<RaceDayContext>options) : base(options) {
        
        }
        
       public DbSet<User> Users { get; set; } 


    }//end of namespace 
}//end of namespace
