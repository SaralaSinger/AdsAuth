using System.Collections.Generic;
using System.Data.SqlClient;

namespace AdsAuth.Data
{
    public class Repository
    {
        private string _connectionString;
        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users VALUES (@name, @email, @passwordHash)";
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public void AddAd(Ad ad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Ads VALUES (@userId, @phone, @description, @date)";
            cmd.Parameters.AddWithValue("@userId", ad.UserId);
            cmd.Parameters.AddWithValue("@phone", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@description", ad.Description);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public List<Ad> GetAllAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads";
            var list = new List<Ad>();
            connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    UserId = (int)reader["UserId"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Description = (string)reader["Description"],
                    Date = (DateTime)reader["Date"],
                });
            }
            connection.Close();
            return list;
        }
        public List<Ad> GetAdsByUser(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads WHERE UserId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var list = new List<Ad>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    UserId = (int)reader["UserId"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Description = (string)reader["Description"],
                    Date = (DateTime)reader["Date"],
                });
            }
            connection.Close();
            return list;
        }
        public string GetUserName(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Name FROM Users WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return (string)reader["Name"];
        }
        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isValid)
            {
                return null;
            }

            return user;
        }
        private User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return new User
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],
                PasswordHash = (string)reader["PasswordHash"],
            };
        }
        public int GetUserIdByEmail(string email)
        {
            if(email == null)
            {
                return -1;
            }
            var user = GetByEmail(email);
            return user.Id;
        }
        public void DeleteAd(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Ads WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        }
    
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
    public class Ad
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public bool CanDelete { get; set; }
    }

}