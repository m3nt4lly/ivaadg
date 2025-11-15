using ivaadg.Models;
using ivaadg.Services;
using System.Windows.Controls;

namespace ivaadg.Pages
{
    /// <summary>
    /// Логика взаимодействия для Manager.xaml
    /// </summary>
    public partial class Manager : Page
    {
        public Manager(User? user, string? role)
        {
            InitializeComponent();
            
            if (user != null)
            {
                tbGreeting.Text = UserGreetingService.GetFullGreeting(user.GetFullName());
            }
        }
    }
}

