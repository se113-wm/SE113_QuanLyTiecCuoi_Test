using System;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class AppUserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string GroupId { get; set; }

        // Navigation property
        public UserGroupDTO UserGroup { get; set; }
    }
}