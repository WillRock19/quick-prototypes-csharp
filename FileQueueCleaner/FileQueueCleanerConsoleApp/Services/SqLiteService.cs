//using System.Text;

//namespace FileQueueErrorsFixer.Services
//{
//    internal class SqLiteService
//    {
//        private const string ConnectionString = "Data Source=localdb.db";

//        public async Task InsertProcessedLettersAsync() 
//        {
//            // Create a connection
//            var query = @"INSERT INTO users 
//            (
//                Name, Age
//            )
//            VALUES 
//            (
//                'Alice', 30
//            );";

//            using (var connection = new SqliteConnection(ConnectionString))
//            {
//                connection.Open();

//                // Insert data
//                var insertCmd = connection.CreateCommand();
                
//                insertCmd.CommandText = query;
//                await insertCmd.ExecuteNonQueryAsync();

//                // Query data
//                var selectCmd = connection.CreateCommand();
//                selectCmd.CommandText = "SELECT * FROM users";

//                using (var reader = selectCmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        Console.WriteLine($"Id: {reader.GetInt32(0)}, Name: {reader.GetString(1)}, Age: {reader.GetInt32(2)}");
//                    }
//                }
//            }
//        }

//        public async Task CreateProcessedLettersTableIfDoesNotExistAsync() 
//        {
//            // Create a connection
//            var query = @"CREATE TABLE IF NOT EXISTS ProcessedLetters (
//                Id INTEGER PRIMARY KEY AUTOINCREMENT,
//                LetterId INTEGER NOT NULL,
//                LetterInvalidMemberEnrollmentId INTEGER NOT NULL
//            );";

//            using (var connection = new SqliteConnection(ConnectionString))
//            {
//                connection.Open();
//                var createTableCmd = connection.CreateCommand();

//                createTableCmd.CommandText = query;
//                await createTableCmd.ExecuteNonQueryAsync();  
//            }
//        }

//        public async Task<bool> SomeLetterWasAlreadyProcessed() 
//        {
//            // Create a connection
//            using (var connection = new SqliteConnection(ConnectionString))
//            {
//                connection.Open();

//                // Query data
//                var selectCmd = connection.CreateCommand();
//                selectCmd.CommandText = "SELECT TOP 1 * FROM ProcessedLetters";

//                using var reader = await selectCmd.ExecuteReaderAsync();
//                return reader.HasRows;
//            }
//        }
//    }
//}
