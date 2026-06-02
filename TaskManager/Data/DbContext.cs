using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace TaskManager.Data
{
    public class DbContext
    {
        private const string DatabaseFileName = "taskmanager.db";
        private static bool _initialized;

        public static SQLiteConnection CreateConnection()
        {
            Database_Create_ALPHA();
            var connection = new SQLiteConnection($"Data Source={DatabaseFileName};Version=3;");
            connection.Open();
            return connection;
        }
        public static void Database_Create_ALPHA()
        {            
            if (_initialized) return;

            var shouldSeed = !File.Exists(DatabaseFileName);
            if (shouldSeed) SQLiteConnection.CreateFile(DatabaseFileName);

            using (var connection = new SQLiteConnection($"Data Source={DatabaseFileName};Version=3;"))
            {
                connection.Open();
                ExecuteSql(connection, @"
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Login TEXT NOT NULL UNIQUE,
    Password TEXT NOT NULL,
    Name TEXT NOT NULL,
    Role INTEGER NOT NULL
);

CREATE TABLE IF NOT EXISTS TaskItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Title TEXT NOT NULL,
    Category TEXT NOT NULL,
    IsCompleted INTEGER NOT NULL DEFAULT 0,
    CreatedByUserId INTEGER NOT NULL,
    AssignedToUserId INTEGER NULL,
    IsForAllWorkers INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY(CreatedByUserId) REFERENCES Users(Id),
    FOREIGN KEY(AssignedToUserId) REFERENCES Users(Id)
);");

                MigrateOldDB(connection);

                var usersCount = Convert.ToInt32(ExecuteScalar(connection, "SELECT COUNT(*) FROM Users;"));
                if (shouldSeed || usersCount == 0)
                {
                    Seed(connection);
                }
            }

            _initialized = true;
        }

        private static void MigrateOldDB(SQLiteConnection connection)
        {
            AddColumn(connection, "TaskItems", "Name", "TEXT NOT NULL DEFAULT ''");
            AddColumn(connection, "TaskItems", "IsCompleted", "INTEGER NOT NULL DEFAULT 0");

            ExecuteSql(connection, "UPDATE TaskItems SET Name = Title WHERE (Name IS NULL OR Name = '') AND Title IS NOT NULL;");
            if (ColumnExists(connection, "TaskItems", "Status"))
            {
                ExecuteSql(connection, "UPDATE TaskItems SET IsCompleted = 1 WHERE Status = 3;");
            }
        }
        private static void AddColumn(SQLiteConnection connection, string tableName, string columnName, string columnDefinition)
        {
            if (ColumnExists(connection, tableName, columnName)) return;
            ExecuteSql(connection, $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition};");
        }
        private static bool ColumnExists(SQLiteConnection connection, string tableName, string columnName)
        {
            using (var command = new SQLiteCommand($"PRAGMA table_info({tableName});", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader.GetString(1) == columnName) return true;
                }
            }
            return false;
        }
        private static void Seed(SQLiteConnection connection)
        {
            ExecuteSql(connection, @"
INSERT OR IGNORE INTO Users (Login, Password, Name, Role) VALUES
('manager', '1234', 'Руководитель', 1),
('worker1', '1234', 'Работник', 2),
('worker2', '1234', 'Работник 2', 2);");
     
        }

        private static void ExecuteSql(SQLiteConnection connection, string sql)
        {
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static object ExecuteScalar(SQLiteConnection connection, string sql)
        {
            using (var command = new SQLiteCommand(sql, connection))
            {
                return command.ExecuteScalar();
            }
        }

    }
}