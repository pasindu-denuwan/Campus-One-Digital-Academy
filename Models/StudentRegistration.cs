using System;

namespace CampusOneDigitalAcademy.Models
{
    /// <summary>
    /// Represents a student registration record corresponding to the Registration table in the Student database.
    /// </summary>
    public class StudentRegistration
    {
        public int RegNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public int MobilePhone { get; set; }
        public int HomePhone { get; set; }
        public string ParentName { get; set; }
        public string NIC { get; set; }
        public int ContactNo { get; set; }

        public StudentRegistration()
        {
            DateOfBirth = DateTime.Now.AddYears(-18);
            Gender = "Male";
        }
    }
}
