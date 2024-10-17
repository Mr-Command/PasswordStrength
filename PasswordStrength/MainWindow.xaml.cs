using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace PasswordStrength
{
    public partial class MainWindow : Window
    {
        private TextBox passwordTextBox;
        private readonly string[] securityTips = new string[]
        {
        "🔒 Şifrenizi hiç kimseyle paylaşmayın",
        "🔑 Her hesap için farklı şifre kullanın",
        "🔄 Şifrelerinizi düzenli olarak değiştirin",
        "⚠️ Kişisel bilgilerinizi şifre olarak kullanmayın",
        "✔️ İki faktörlü doğrulama kullanın",
        "📱 Şifrelerinizi güvenli bir şekilde saklayın"
        };

        private int currentTipIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            UpdateTipDisplay();
            passwordTextBox = new TextBox
            {
                Visibility = Visibility.Collapsed
            };
            Grid.SetRow(passwordTextBox, 1);
            Grid.SetColumn(passwordTextBox, 0);
            ((Grid)passwordBox.Parent).Children.Add(passwordTextBox);

            passwordTextBox.TextChanged += PasswordTextBox_TextChanged;
        }

        private void UpdateTipDisplay()
        {
            currentTip.Text = securityTips[currentTipIndex];
            tipCounter.Text = $"{currentTipIndex + 1}/{securityTips.Length}";

            prevTipButton.IsEnabled = currentTipIndex > 0;
            nextTipButton.IsEnabled = currentTipIndex < securityTips.Length - 1;
        }

        private void PrevTipButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentTipIndex > 0)
            {
                currentTipIndex--;
                UpdateTipDisplay();
            }
        }

        private void NextTipButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentTipIndex < securityTips.Length - 1)
            {
                currentTipIndex++;
                UpdateTipDisplay();
            }
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            string password = showPasswordButton.IsChecked == true ?
                            passwordTextBox.Text :
                            passwordBox.Password;
            UpdatePasswordStrength(password);
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            passwordBox.Password = passwordTextBox.Text;
        }

        private void UpdatePasswordStrength(string password)
        {
            int strength = 0;

            bool hasLength = password.Length >= 8;
            lengthCheck.Text = (hasLength ? "✅" : "❌") + " En az 8 karakter";
            if (hasLength) strength += 20;
            if (password.Length >= 14) strength += 10;

            bool hasUpper = Regex.IsMatch(password, @"[A-Z]");
            upperCheck.Text = (hasUpper ? "✅" : "❌") + " En az 1 büyük harf";
            if (hasUpper) strength += 20;

            bool hasLower = Regex.IsMatch(password, @"[a-z]");
            lowerCheck.Text = (hasLower ? "✅" : "❌") + " En az 1 küçük harf";
            if (hasLower) strength += 10;

            bool hasNumber = Regex.IsMatch(password, @"[0-9]");
            numberCheck.Text = (hasNumber ? "✅" : "❌") + " En az 1 sayı";
            if (hasNumber) strength += 15;

            bool hasSpecial = Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]");
            specialCheck.Text = (hasSpecial ? "✅" : "❌") + " En az 1 özel karakter";
            if (hasSpecial) strength += 25;

            var animation = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 0,
                To = strength,
                Duration = TimeSpan.FromSeconds(1)
            };
            strengthBar.BeginAnimation(ProgressBar.ValueProperty, animation);

            if (strength < 40)
            {
                strengthBar.Foreground = Brushes.Red;
                strengthText.Text = "Zayıf Şifre";
                strengthText.Foreground = Brushes.Red;
            }
            else if (strength < 70)
            {
                strengthBar.Foreground = Brushes.Yellow;
                strengthText.Text = "Orta Güçte Şifre";
                strengthText.Foreground = Brushes.Orange;
            }
            else
            {
                strengthBar.Foreground = Brushes.Green;
                strengthText.Text = "Güçlü Şifre";
                strengthText.Foreground = Brushes.Green;
            }
        }

        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (showPasswordButton.IsChecked == true)
            {
                passwordTextBox.Text = passwordBox.Password;
                passwordBox.Visibility = Visibility.Collapsed;
                passwordTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                passwordBox.Password = passwordTextBox.Text;
                passwordBox.Visibility = Visibility.Visible;
                passwordTextBox.Visibility = Visibility.Collapsed;
            }
        }

        private void SuggestPassword_Click(object sender, RoutedEventArgs e)
        {
            string suggestedPassword = GenerateStrongPassword();

            passwordTextBox.Text = suggestedPassword;

            passwordBox.Visibility = Visibility.Collapsed;
            passwordTextBox.Visibility = Visibility.Visible;

            showPasswordButton.IsChecked = true;

            UpdatePasswordStrength(suggestedPassword);
        }

        private string GenerateStrongPassword()
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string numberChars = "0123456789";
            const string specialChars = "!@#$%^&*()";

            var random = new Random();
            var password = new StringBuilder();

            password.Append(upperChars[random.Next(upperChars.Length)]);
            password.Append(lowerChars[random.Next(lowerChars.Length)]);
            password.Append(numberChars[random.Next(numberChars.Length)]);
            password.Append(specialChars[random.Next(specialChars.Length)]);

            string allChars = upperChars + lowerChars + numberChars + specialChars;
            for (int i = 0; i < 8; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            return new string(password.ToString().ToCharArray().OrderBy(x => random.Next()).ToArray());
        }
    }
}
