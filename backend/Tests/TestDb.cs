using Infrastructure.Persistence.Contexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    // Gives each test its own fresh, empty SQLite database in memory
    public static class TestDb
    {
        public static AppDbContext Create()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();                       // in-memory DB lives as long as the connection is open

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var db = new AppDbContext(options);
            db.Database.EnsureCreated();             // build tables, FKs and unique index from the model
            return db;
        }
    }
}