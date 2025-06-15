using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace TestABP.EntityFrameworkCore
{
    public static class TestABPDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<TestABPDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<TestABPDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
