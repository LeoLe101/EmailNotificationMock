
using Microsoft.EntityFrameworkCore;

namespace EmailServer
{

    /// <summary>
    /// This class generate/initilize the database for the application before
    /// the application is fully running
    /// </summary>
    public static class Seeding
    {
        public static void SeedDb(ModelBuilder modelBuilder)
        {
            // Do Something Here when you have an idea of what to do...
            // This will be used before the application is fully up and running. Might be good to initialize Mock Data
        }
    }
}