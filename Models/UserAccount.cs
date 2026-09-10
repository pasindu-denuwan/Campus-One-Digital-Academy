using System;

namespace CampusOneDigitalAcademy.Models
{
    /// <summary>
    /// Represents a system user for authentication.
    /// </summary>
    public class UserAccount
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public UserAccount()
        {
            Role = "User";
        }

        public UserAccount(string username, string password, string role = "User", int userId = 0)
        {
            UserId = userId;
            Username = username;
            Password = password;
            Role = role;
        }
    }
}
