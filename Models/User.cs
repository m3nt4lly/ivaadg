namespace ivaadg.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string? UserLogin { get; set; }
        public string? UserPassword { get; set; }
        public int RoleId { get; set; }
        public string? UserLastName { get; set; }
        public string? UserFirstName { get; set; }
        public string? UserMiddleName { get; set; }

        public virtual Role? Role { get; set; }

        /// <summary>
        /// Получить полное ФИО пользователя
        /// </summary>
        public string GetFullName()
        {
            var parts = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(UserLastName))
                parts.Add(UserLastName);
            
            if (!string.IsNullOrWhiteSpace(UserFirstName))
                parts.Add(UserFirstName);
            
            if (!string.IsNullOrWhiteSpace(UserMiddleName))
                parts.Add(UserMiddleName);
            
            return parts.Count > 0 ? string.Join(" ", parts) : "Пользователь";
        }
    }
}

