namespace ivaadg.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string? UserLogin { get; set; }
        public string? UserPassword { get; set; }
        public int RoleId { get; set; }

        public virtual Role? Role { get; set; }
    }
}

