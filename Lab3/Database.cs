using System.Data.SqlClient;

namespace Lab3
{
    public class Database
    {
        private static string connectionString = Environment.GetEnvironmentVariable("connectionString");

        public static string SelectGroup(long chatId)
        {
            string result;
            using(SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string sqlQuery = $"SELECT [group] FROM Groups WHERE chatId = {chatId};";
                    SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);

                    SqlDataReader reader = command.ExecuteReader();

                    reader.Read();
                    result = reader[0].ToString();
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    throw ex;
                }
            }
            return result;
        }

        public static void InsertGroup(long chatId, string group)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string sqlQuery = $"INSERT INTO Groups (chatId, [group]) VALUES ({chatId}, {group});";
                    SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    throw ex;
                }
            }
        }

        public static void UpdateGroup(long chatId, string group)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string sqlQuery = $"UPDATE Groups SET [group] = {group} WHERE chatId = {chatId}";
                    SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    throw ex;
                }
            }
        }

        public static bool IsPresent(long chatId)
        {
            bool result;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();
                    string sqlQuery = $"SELECT COUNT(*) FROM Groups WHERE chatId = {chatId}";
                    SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
                    int count = int.Parse(command.ExecuteScalar().ToString());
                    result = count > 0;

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    throw ex;
                }
            }
            return result;
        }
    }
}
