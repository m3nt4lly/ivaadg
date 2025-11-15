using ivaadg.Models;
using ivaadg.Services;
using System.Windows.Controls;

namespace ivaadg.Pages
{
    /// <summary>
    /// Логика взаимодействия для Client.xaml
    /// </summary>
    public partial class Client : Page
    {
        public Client(User? user, string? role)
        {
            InitializeComponent();
            
            if (user != null)
            {
                tbGreeting.Text = UserGreetingService.GetFullGreeting(user.GetFullName());
            }
        }
    }
}

