using ivaadg.Models;
using ivaadg.Services;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace ivaadg.Pages
{
    /// <summary>
    /// Логика взаимодействия для Autho.xaml
    /// </summary>
    public partial class Autho : Page
    {
        private int failedAttempts = 0;
        private DispatcherTimer lockoutTimer;
        private int remainingSeconds = 0;
        private const int MAX_FAILED_ATTEMPTS = 3;
        private const int LOCKOUT_DURATION_SECONDS = 10;

        public Autho()
        {
            InitializeComponent();
            InitializeLockoutTimer();
            ClearFields();
        }

        private void InitializeLockoutTimer()
        {
            lockoutTimer = new DispatcherTimer();
            lockoutTimer.Interval = TimeSpan.FromSeconds(1);
            lockoutTimer.Tick += LockoutTimer_Tick;
        }

        private void LockoutTimer_Tick(object sender, EventArgs e)
        {
            remainingSeconds--;
            UpdateTimerDisplay();

            if (remainingSeconds <= 0)
            {
                lockoutTimer.Stop();
                UnlockControls();
            }
        }

        private void UpdateTimerDisplay()
        {
            if (remainingSeconds > 0)
            {
                tbTimer.Text = $"Осталось времени: {remainingSeconds} сек.";
                tbTimer.Foreground = System.Windows.Media.Brushes.Red;
                tbTimer.Visibility = Visibility.Visible;
            }
            else
            {
                tbTimer.Visibility = Visibility.Hidden;
            }
        }

        private void LockControls()
        {
            tbLogin.IsEnabled = false;
            tbPassword.IsEnabled = false;
            tbCaptcha.IsEnabled = false;
            btnEnter.IsEnabled = false;
            btnEnterGuest.IsEnabled = false;
        }

        private void UnlockControls()
        {
            tbLogin.IsEnabled = true;
            tbPassword.IsEnabled = true;
            tbCaptcha.IsEnabled = true;
            btnEnter.IsEnabled = true;
            btnEnterGuest.IsEnabled = true;
            failedAttempts = 0;
            tbTimer.Visibility = Visibility.Hidden;
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
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
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

                // Проверяем подключение и наличие пользователя
                var user = db.User.Where(x => x.UserLogin == login && x.UserPassword == hashedPassword).FirstOrDefault();

                bool isCaptchaRequired = failedAttempts >= 1;
                bool isCaptchaValid = true;

                if (isCaptchaRequired)
                {
                    string captchaInput = tbCaptcha.Text.Trim();
                    string captchaExpected = tblCaptcha.Text;
                    isCaptchaValid = captchaInput == captchaExpected;
                }

                if (user != null && user.Role != null && (!isCaptchaRequired || isCaptchaValid))
                {
                    // Проверка рабочего времени (только для сотрудников, не для клиентов)
                    if (user.Role.RoleName != "Клиент" && !UserGreetingService.IsWorkingHours())
                    {
                        MessageBox.Show("Доступ запрещен!\n\nРабочее время: с 10:00 до 19:00.\nПопробуйте войти в рабочее время.", 
                            "Вне рабочего времени", MessageBoxButton.OK, MessageBoxImage.Warning);
                        tbPassword.Clear();
                        if (isCaptchaRequired)
                        {
                            tbCaptcha.Clear();
                            GenerateCapctcha();
                        }
                        return;
                    }

                    // Успешная авторизация
                    MessageBox.Show("Вы вошли под: " + user.Role.RoleName?.ToString());
                    failedAttempts = 0;
                    ClearFields();
                    HideCaptcha();
                    LoadPage(user.Role.RoleName ?? "", user);
                }
                else
                {
                    // Неудачная попытка
                    failedAttempts++;

                    if (failedAttempts >= MAX_FAILED_ATTEMPTS)
                    {
                        // Блокировка на 10 секунд
                        remainingSeconds = LOCKOUT_DURATION_SECONDS;
                        LockControls();
                        lockoutTimer.Start();
                        UpdateTimerDisplay();
                        MessageBox.Show($"Превышено количество неудачных попыток входа!\n\nОкно заблокировано на {LOCKOUT_DURATION_SECONDS} секунд.");
                    }
                    else
                    {
                        // Показываем причину неудачи
                        if (user == null)
                        {
                            MessageBox.Show("Неверный логин или пароль!");
                        }
                        else if (isCaptchaRequired && !isCaptchaValid)
                        {
                            MessageBox.Show("Неверная капча!");
                        }

                        // Показываем капчу после первой неудачной попытки
                        if (failedAttempts >= 1 && !isCaptchaRequired)
                        {
                            GenerateCapctcha();
                        }
                    }

                    tbPassword.Clear();
                    if (isCaptchaRequired)
                    {
                        tbCaptcha.Clear();
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
            failedAttempts = 0;
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

