namespace Swipe2Try.Core.Models
{
    public class Role
    {
        public string RoleID { get; set; }
        public string RoleName { get; set; }

        public Role() { }

        public Role(string roleID, string roleName)
        {
            RoleID = roleID;
            RoleName = roleName;
        }
    }
} 