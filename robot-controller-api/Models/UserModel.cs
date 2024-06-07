namespace robot_controller_api.Models
{
    using System;
    using System.Text.Json.Serialization;

    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public string? Description { get; set; }
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public UserModel(int id, string email, string firstName, string lastName, string passwordHash, string? description, string? role, DateTime createdDate, DateTime modifiedDate)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;
            Description = description;
            Role = role;
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
        }

        public UserModel() { }
    }

}
