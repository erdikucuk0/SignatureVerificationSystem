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
    /// <summary>
    /// Interaction logic for ManageUsersWindow.xaml
    /// </summary>
    public partial class ManageUsersWindow : Window
    {
        #region Member Fields

        private List<User> Users = new List<User>();

        #endregion

        #region Constructor

        public ManageUsersWindow()
        {
            InitializeComponent();
            Loaded += ManageUsersWindow_Loaded;
        }

        #endregion

        #region Loaded

        private void ManageUsersWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FillUpUsersList();
        }

        #endregion

        #region UI Events

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var userToRemove = DBHelper.Instance.Users.FirstOrDefault(x => x.Id.ToString() == ((TextBlock)((StackPanel)UsersListView.SelectedItem).Children[0]).Text);

            DBHelper.Instance.Users.Remove(userToRemove);
            await DBHelper.Instance.SaveChangesAsync();

            Users.Clear();

            FillUpUsersList();
            return;
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button == SortAsIDButton)
            {
                UsernameArrow.Visibility = Visibility.Hidden;

                if (IdArrow.Visibility == Visibility.Hidden || IdArrow.Kind == MaterialDesignThemes.Wpf.PackIconKind.ArrowDropDown)
                {
                    IdArrow.Visibility = Visibility.Visible;
                    IdArrow.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDropUp;

                    //Sort Ascending
                    Users = Users.OrderBy(x => x.Id).ToList();
                    FillUpUsersListView();
                    return;
                }

                else if (IdArrow.Kind == MaterialDesignThemes.Wpf.PackIconKind.ArrowDropUp)
                {
                    IdArrow.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDropDown;

                    //Sort Descending
                    Users = Users.OrderByDescending(x => x.Id).ToList();
                    FillUpUsersListView();
                    return;
                }
            }

            if (button == SortAsUsernameButton)
            {
                IdArrow.Visibility = Visibility.Hidden;

                if (UsernameArrow.Visibility == Visibility.Hidden || UsernameArrow.Kind == MaterialDesignThemes.Wpf.PackIconKind.ArrowDropDown)
                {
                    UsernameArrow.Visibility = Visibility.Visible;
                    UsernameArrow.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDropUp;

                    //Sort Ascending
                    Users = Users.OrderBy(x => x.Username).ToList();
                    FillUpUsersListView();
                    return;
                }

                else if (UsernameArrow.Visibility == Visibility.Visible)
                {
                    UsernameArrow.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDropDown;

                    //Sort Descending
                    Users = Users.OrderByDescending(x => x.Username).ToList();
                    FillUpUsersListView();
                    return;
                }
            }
        }

        #endregion

        #region Helper Procedures

        private void FillUpUsersList()
        {
            foreach (var item in DBHelper.Instance.Users)
            {
                Users.Add(new User { Id = item.Id, Username = item.Username, Role = item.Role });
            }

            FillUpUsersListView();
        }

        private void FillUpUsersListView()
        {
            UsersListView.Items.Clear();

            foreach (var item in Users)
            {
                StackPanel stackPanel = new StackPanel()
                {
                    Orientation = Orientation.Horizontal
                };

                ContextMenu contextmenu = new ContextMenu();

                MaterialDesignThemes.Wpf.PackIcon delete_icon = new MaterialDesignThemes.Wpf.PackIcon()
                {
                    Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete
                };

                MenuItem delete = new MenuItem
                {
                    Header = LocalizedStrings.Instance["DeleteMenuItemHeader"],
                    Icon = delete_icon
                };
                delete.Click += new RoutedEventHandler(Delete_Click);

                if (SessionHelper.CurrentUser.Role > item.Role || SessionHelper.CurrentUser.Id == item.Id)
                    delete.IsEnabled = false;

                contextmenu.Items.Add(delete);

                TextBlock id_textBlock = new TextBlock()
                {
                    Text = item.Id.ToString(),
                    FontSize = 15,
                    Width = 290,
                    Height = 30,
                };

                TextBlock username_textBlock = new TextBlock()
                {
                    Text = item.Username,
                    FontSize = 15,
                    Width = 290,
                    Height = 30
                };                

                stackPanel.Children.Add(id_textBlock);
                stackPanel.Children.Add(username_textBlock);

                stackPanel.ContextMenu = contextmenu;

                UsersListView.Items.Add(stackPanel);
            }
        }

        #endregion
    }
}