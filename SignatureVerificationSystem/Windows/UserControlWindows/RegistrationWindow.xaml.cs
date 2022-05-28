using SignatureVerificationSystem.Models.Authorization;
using SignatureVerificationSystem.Resources;
using SignatureVerificationSystem.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    // Interaction logic for RegistrationWindow.xaml
    // </summary>
    public partial class RegistrationWindow : Window
    {
        #region Constructor

        public RegistrationWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region UI Events

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(UsernameTextBox.Text) || string.IsNullOrEmpty(PasswordBox.Password) || string.IsNullOrEmpty(PrivateKeyPasswordBox.Password))
                {
                    RegisterInfoTextBlock.Text = LocalizedStrings.Instance["RegisterInfoTextBlock"];
                    RegisterInfoTextBlock.Visibility = Visibility.Visible;
                }

                else
                {
                    var isUserExist = DBHelper.Instance.Users.FirstOrDefault(x => x.Username == UsernameTextBox.Text);

                    if (isUserExist != null)
                    {
                        MessageBox.Show(LocalizedStrings.Instance["EnteredExistingUsername"], LocalizedStrings.Instance["FailedToRegister"]);
                        return;
                    }

                    var hashedKey = ComputeSha256Hash(PrivateKeyPasswordBox.Password);

                    if (hashedKey != "b609da0d98ca3d382c0b62d1976a1dde7cfcd100d7c130699ec1c88e25e30544")
                    {
                        MessageBox.Show(LocalizedStrings.Instance["IncorrectPrivateKey"], LocalizedStrings.Instance["FailedToRegister"]);
                        return;
                    }

                    var hashedPassword = ComputeSha256Hash(PasswordBox.Password);

                    var user = new User { Username = UsernameTextBox.Text, Password = hashedPassword, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };

                    var result = DBHelper.Instance.Users.Add(user);
                    DBHelper.Instance.SaveChangesAsync();

                    if (result.State.ToString().Equals("Added"))
                    {
                        MessageBox.Show(LocalizedStrings.Instance["RegisteredSuccessfully"], LocalizedStrings.Instance["RegistrationSuccessful"]);
                        Close();
                    }
                    else
                        MessageBox.Show(string.Format(LocalizedStrings.Instance["CheckDatabase"], result.State.ToString()), LocalizedStrings.Instance["FailedToRegister"]);
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Helper Procedures

        public string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        #endregion
    }
}