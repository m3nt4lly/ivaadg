using ivaadg.Models;
using ivaadg.Services;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ivaadg.Pages
{
    /// <summary>
    /// Логика взаимодействия для Autho.xaml
    /// </summary>
    public partial class Autho : Page
    {
        int click;
        
        public Autho()
        {
            InitializeComponent();
            click = 0;
            ClearFields();
        }

        private void btnEnterGuest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client(null, null));
        }

        private void GenerateCapctcha()
        {
            tbCaptcha.Visibility = Visibility.Visible;
            tblCaptcha.Visibility = Visibility.Visible;
            
            string capctchaText = CaptchaGenerator.GenerateCaptchaText(6);
            tblCaptcha.Text = capctchaText;
            tblCaptcha.TextDecorations = System.Windows.TextDecorations.Strikethrough;
        }

        private void HideCaptcha()
        {
            tbCaptcha.Visibility = Visibility.Hidden;
            tblCaptcha.Visibility = Visibility.Hidden;
            tbCaptcha.Clear();
            tblCaptcha.Text = "Капча";
        }

        private void ClearFields()
        {
            tbLogin.Clear();
            tbPassword.Clear();
            tbCaptcha.Clear();
            HideCaptcha();
            click = 0;
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            click += 1;
            string login = tbLogin.Text.Trim();
            string password = tbPassword.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            try
            {
                // Хешируем пароль перед проверкой в БД
                string hashedPassword = Hash.HashPassword(password);
                
                RullEntities db = RullEntities.GetContext();

                // Проверяем подключение - пробуем загрузить данные
                try
                {
                    // Пробуем загрузить всех пользователей для диагностики
                    var allUsers = db.User.ToList();
                    var allRoles = db.Role.ToList();
                    
                    // Отладочная информация (можно убрать после исправления)
                    System.Diagnostics.Debug.WriteLine($"Найдено пользователей в БД: {allUsers.Count}");
                    System.Diagnostics.Debug.WriteLine($"Найдено ролей в БД: {allRoles.Count}");
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных из БД:\n{ex.Message}\n\nПроверьте подключение к базе данных.");
                    return;
                }

                // Проверяем подключение и наличие пользователя
                var user = db.User.Where(x => x.UserLogin == login && x.UserPassword == hashedPassword).FirstOrDefault();

                if (click == 1)
                {
                    if (user != null && user.Role != null)
                    {
                        MessageBox.Show("Вы вошли под: " + user.Role.RoleName?.ToString());
                        ClearFields();
                        HideCaptcha();
                        LoadPage(user.Role.RoleName ?? "", user);
                    }
                    else
                    {
                        // Отладочная информация
                        try
                        {
                            var allUsersList = db.User.ToList();
                            var userExists = db.User.Any(x => x.UserLogin == login);
                            
                            if (!userExists)
                            {
                                string usersInfo = allUsersList.Count > 0 
                                    ? $"Найдено пользователей в БД: {allUsersList.Count}\nЛогины: {string.Join(", ", allUsersList.Select(u => u.UserLogin))}"
                                    : "В базе данных нет пользователей!";
                                    
                                MessageBox.Show($"Пользователь с логином '{login}' не найден в базе данных.\n\n{usersInfo}\n\nПроверьте:\n1. Выполнен ли SQL скрипт CreateDatabase.sql\n2. Правильность логина\n3. Названия таблиц в БД (должны быть 'User' и 'Role')");
                            }
                            else
                            {
                                // Проверяем хеш пароля
                                var dbUser = db.User.FirstOrDefault(x => x.UserLogin == login);
                                if (dbUser != null)
                                {
                                    MessageBox.Show($"Неверный пароль!\n\nВведенный хеш: {hashedPassword}\nХеш в БД: {dbUser.UserPassword}\n\nПроверьте правильность пароля или пересоздайте пользователя в БД.");
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show($"Ошибка при проверке пользователей:\n{ex.Message}\n\nВозможно, проблема с названиями таблиц или подключением к БД.");
                        }
                        tbPassword.Clear();
                        GenerateCapctcha();
                    }
                }
                else if (click > 1)
                {
                    string captchaInput = tbCaptcha.Text.Trim();
                    string captchaExpected = tblCaptcha.Text;

                    if (user != null && user.Role != null && captchaInput == captchaExpected)
                    {
                        MessageBox.Show("Вы вошли под: " + user.Role.RoleName?.ToString());
                        ClearFields();
                        HideCaptcha();
                        LoadPage(user.Role.RoleName ?? "", user);
                    }
                    else
                    {
                        if (user == null)
                        {
                            MessageBox.Show("Неверный логин или пароль!");
                        }
                        else if (captchaInput != captchaExpected)
                        {
                            MessageBox.Show("Неверная капча!");
                        }
                        ClearFields();
                        GenerateCapctcha();
                    }
                }
            }
            catch (System.Data.Entity.Core.EntityException ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных!\n\n{ex.Message}\n\nПроверьте:\n1. Запущен ли SQL Server\n2. Создана ли база данных RullDB\n3. Правильность строки подключения в App.config");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        private void LoadPage(string _role, User user)
        {
            click = 0;
            switch (_role)
            {
                case "Клиент":
                    NavigationService.Navigate(new Client(user, _role));
                    break;
                case "Администратор":
                    NavigationService.Navigate(new Admin(user, _role));
                    break;
                case "Менеджер":
                    NavigationService.Navigate(new Manager(user, _role));
                    break;
                default:
                    MessageBox.Show($"Роль '{_role}' не найдена. Переход на страницу клиента.");
                    NavigationService.Navigate(new Client(user, _role));
                    break;
            }
        }
    }
}

