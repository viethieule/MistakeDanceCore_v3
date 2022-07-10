using AppRoleName = Application.Users.RoleName;

namespace Application.Users
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public bool IsAdmin => RoleName == AppRoleName.Admin;
        public bool IsReceptionist => RoleName == AppRoleName.Receptionist;
        public bool IsCollaborator => RoleName == AppRoleName.Collaborator;
        public bool IsMember => RoleName == AppRoleName.Member;
    }
}