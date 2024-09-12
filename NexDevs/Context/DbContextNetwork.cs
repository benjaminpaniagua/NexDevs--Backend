using Microsoft.EntityFrameworkCore;

namespace NexDevs.Context
{
    public class DbContextNetwork: DbContext
    {
        public DbContextNetwork(DbContextOptions<DbContextNetwork> options) : base(options)
        {
            
        }

        //Add models here
    }
}
