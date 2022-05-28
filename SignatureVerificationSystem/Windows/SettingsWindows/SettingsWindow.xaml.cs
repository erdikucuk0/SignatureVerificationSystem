using SignatureVerificationSystem.Models;
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
using WPFLocalizeExtension.Engine;

namespace SignatureVerificationSystem.Windows.SettingsWindows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        #region Constructor

        public SettingsWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        #endregion

        #region UI Events

        private void SaveAllChanges_Click(object sender, RoutedEventArgs e)
        {
            SaveModelConfiguration();
            SaveLanguageConfiguration();
        }

        private void MenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ModelPanel.Visibility = Visibility.Hidden;
            LanguagePanel.Visibility = Visibility.Hidden;

            switch (MenuListView.SelectedIndex)
            {
                case 0:
                    ModelPanel.Visibility = Visibility.Visible;
                    break;

                case 1:
                    LanguagePanel.Visibility = Visibility.Visible;
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region Helper Procedures

        private void SaveModelConfiguration()
        {
            Settings.ModelParams = ModelPanelUserContol.ModelPanelProperties.ToConfiguration();
            Settings.SaveModelConfiguration();
        }

        private void SaveLanguageConfiguration()
        {
            Settings.LanguageParams = LanguagePanelUserControl.LanguagePanelProperties.ToConfiguration();
            Settings.SaveLanguageConfiguration();

            if (Settings.LanguageParams.Language == "English")
                LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo("en-US");
            else
                LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo("tr-TR");
        }

        #endregion
    }
}