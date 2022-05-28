using SignatureVerificationSystem.Models.Authorization;
using SignatureVerificationSystem.Resources;
using SignatureVerificationSystem.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SignatureVerificationSystem.Windows.UserControlWindows
{
    // <summary>
    // Interaction logic for LoginWindow.xaml
    // </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Loaded += LoginWindow_Loaded;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (SessionHelper.IsLoggedIn)
            {
                LoginInfoTextBlock.Foreground = Brushes.Black;
                LoginInfoTextBlock.Text = string.Format(LocalizedStrings.Instance["InfoTextBlock"], SessionHelper.CurrentUser.Username);
                LoginInfoTextBlock.Visibility = Visibility.Visible;
                UsernameTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Collapsed;
                LoginButton.Content = LocalizedStrings.Instance["LogoutButton"];
                LoginButton.Width = 150;
                LoginButton.HorizontalAlignment = HorizontalAlignment.Center;
                LoginButton.Margin = new Thickness(0, 65, 0, 65);
                RegisterButton.Visibility = Visibility.Hidden;
            }
            else
            {
                UsernameTextBox.Focus();
            }

            KeyDown += LoginWindow_KeyDown;
        }

        private void LoginWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                CheckLogin();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            CheckLogin();
        }

        private void CheckLogin()
        {
            if (SessionHelper.IsLoggedIn)
            {
                SessionHelper.CurrentUser = null;
                Close();
                return;
            }

            if (!string.IsNullOrEmpty(UsernameTextBox.Text) && !string.IsNullOrEmpty(PasswordBox.Password))
            {
                try
                {
                    UsernameTextBox.IsEnabled = false;
                    PasswordBox.IsEnabled = false;
                    LoginButton.IsEnabled = false;
                    var user = DBHelper.Instance.Users.FirstOrDefault(x => x.Username == UsernameTextBox.Text);

                    if (user != null && SecurityHelper.VerifyHash(PasswordBox.Password, user.Password))
                    {
                        SessionHelper.CurrentUser = user;
                        Close();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    UsernameTextBox.IsEnabled = true;
                    PasswordBox.IsEnabled = true;
                    LoginButton.IsEnabled = true;
                }
            }

            LoginInfoTextBlock.Text = LocalizedStrings.Instance["LoginFailed"];
            LoginInfoTextBlock.Visibility = Visibility.Visible;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registrationWindow = new RegistrationWindow();
            registrationWindow.ShowDialog();
        }
    }
}