using Microsoft.AspNetCore.Identity;
using robot_controller_api.Models;
using Npgsql;
using robot_controller_api.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace robot_controller_api.Persistence
{
    public class UserADO : IUserModelDataAccess
    {
        private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=1234;Database=postgres";

        public List<UserModel> GetAllUsers()
        {
            var users = new List<UserModel>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM public.user", conn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var user = ReadUser(dr);
                users.Add(user);
            }
            return users;
        }

        public List<UserModel> GetAllAdmins()
        {
            var users = new List<UserModel>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM public.user WHERE role = 'admin'", conn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var user = ReadUser(dr);
                users.Add(user);
            }
            return users;
        }

        public UserModel GetUserById(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM public.user WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return ReadUser(dr);
            }
            return null;
        }

        public UserModel GetUserByEmail(string email)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM public.user WHERE email = @Email", conn);
            cmd.Parameters.AddWithValue("email", email);
            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return ReadUser(dr);
            }
            return null;
        }

        public UserModel AddUser(UserModel newUser)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(@"INSERT INTO public.user (email, firstname, lastname, passwordhash, description, role, createddate, modifieddate) 
                                               VALUES (@Email, @FirstName, @LastName, @PasswordHash, @Description, @Role, @CreatedDate, @ModifiedDate)
                                               RETURNING id", conn);

            cmd.Parameters.AddWithValue("Email", newUser.Email);
            cmd.Parameters.AddWithValue("FirstName", newUser.FirstName);
            cmd.Parameters.AddWithValue("LastName", newUser.LastName);
            cmd.Parameters.AddWithValue("PasswordHash", newUser.PasswordHash);
            cmd.Parameters.AddWithValue("Description", (object)newUser.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("Role", newUser.Role);
            cmd.Parameters.AddWithValue("CreatedDate", newUser.CreatedDate);
            cmd.Parameters.AddWithValue("ModifiedDate", newUser.ModifiedDate);

            newUser.Id = (int)cmd.ExecuteScalar();
            return newUser;
        }

        public void UpdateUser(UserModel updatedUser)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(@"UPDATE public.user 
                                                SET firstname = @FirstName, lastname = @LastName, 
                                                    description = @Description, role = @Role, modifieddate = @ModifiedDate
                                                WHERE id = @Id", conn);

            cmd.Parameters.AddWithValue("FirstName", updatedUser.FirstName);
            cmd.Parameters.AddWithValue("LastName", updatedUser.LastName);
            cmd.Parameters.AddWithValue("Description", (object)updatedUser.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("Role", updatedUser.Role);
            cmd.Parameters.AddWithValue("ModifiedDate", updatedUser.ModifiedDate);

            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("DELETE FROM public.user WHERE id = @Id", conn);
            cmd.Parameters.AddWithValue("Id", id);
            cmd.ExecuteNonQuery();
        }

        public void PatchUser(int id, LoginModel loginModel)
        {
            var user = GetUserById(id);
            if (user == null)
            {
                throw new InvalidOperationException($"User with Id '{id}' not found.");
            }

            var hasher = new EncodeSHA();
            var pwHash = hasher.HashPassword(loginModel.Password);

            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(@"UPDATE public.user 
                                                SET email = @Email, passwordhash = @PasswordHash, modifieddate = @ModifiedDate
                                                WHERE id = @Id", conn);

            cmd.Parameters.AddWithValue("Email", loginModel.Email);
            cmd.Parameters.AddWithValue("PasswordHash", pwHash);
            cmd.Parameters.AddWithValue("ModifiedDate", DateTime.UtcNow);

            cmd.ExecuteNonQuery();
        }

        private UserModel ReadUser(NpgsqlDataReader dr)
        {
            return new UserModel
            {
                Id = (int)dr["id"],
                Email = (string)dr["email"],
                FirstName = (string)dr["firstname"],
                LastName = (string)dr["lastname"],
                PasswordHash = (string)dr["passwordhash"],
                Description = dr["description"] as string,
                Role = (string)dr["role"],
                CreatedDate = (DateTime)dr["createddate"],
                ModifiedDate = (DateTime)dr["modifieddate"]
            };
        }
    }
}
