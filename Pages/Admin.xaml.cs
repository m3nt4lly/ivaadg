using ivaadg.Models;
using ivaadg.Services;
using System.Windows.Controls;

namespace ivaadg.Pages
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Page
    {
        public Admin(User? user, string? role)
        {
            InitializeComponent();
            
            if (user != null)
            {
                tbGreeting.Text = UserGreetingService.GetFullGreeting(user.GetFullName());
            }
        }
    }
}

