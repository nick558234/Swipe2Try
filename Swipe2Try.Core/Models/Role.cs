namespace Swipe2Try.Core.Models
{
    public class Role
    {
        public string RoleID { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;

        public Role() { }

        public Role(string roleID, string roleName)
        {
            RoleID = roleID;
            RoleName = roleName;
        }
    }
} 