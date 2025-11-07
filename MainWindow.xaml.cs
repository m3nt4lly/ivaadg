using ivaadg.Pages;
using System;
using System.Windows;

namespace ivaadg
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            fr_content.Content = new Autho();

        }
        
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            fr_content.GoBack();
        }

        private void frContent_ContentRendered(object sender, EventArgs e)
        {
            if (fr_content.CanGoBack)
            {
                btnBack.Visibility = Visibility.Visible;
            }
            else
            {
                btnBack.Visibility = Visibility.Hidden;
            }
        }
    }
}