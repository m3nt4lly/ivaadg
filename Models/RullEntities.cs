using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;

namespace ivaadg.Models
{
    public partial class RullEntities : DbContext
    {
        private static RullEntities? _context;

        public RullEntities()
            : base("name=RullEntities")
        {
            // Отключаем проверку модели, так как БД создается через SQL скрипт
            Database.SetInitializer<RullEntities>(null);
        }

        public virtual DbSet<Role> Role { get; set; } = null!;
        public virtual DbSet<User> User { get; set; } = null!;

        public static RullEntities GetContext()
        {
            _context ??= new RullEntities();
            return _context!;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Указываем имена таблиц в БД (единственное число)
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");

            modelBuilder.Entity<Role>()
                .HasMany(e => e.User)
                .WithRequired(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .WillCascadeOnDelete(false);
        }
    }
}

