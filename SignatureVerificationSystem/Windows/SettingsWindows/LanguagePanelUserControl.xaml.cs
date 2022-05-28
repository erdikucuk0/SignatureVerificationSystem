using SignatureVerificationSystem.Models;
using SignatureVerificationSystem.Models.Enums;
using SignatureVerificationSystem.SettingsModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SignatureVerificationSystem.Windows.SettingsWindows
{
    /// <summary>
    /// Interaction logic for LanguagePanelUserControl.xaml
    /// </summary>
    public partial class LanguagePanelUserControl : UserControl, INotifyPropertyChanged
    {
        #region Member Fields

        private LanguagePanelProperties languagePanelProperties { get; set; } = new LanguagePanelProperties();
        public LanguagePanelProperties LanguagePanelProperties
        {
            get
            {
                return languagePanelProperties;
            }
            set
            {
                if (value != languagePanelProperties)
                {
                    languagePanelProperties = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LanguagePanelProperties"));

                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        public LanguagePanelUserControl()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += LanguagePanelUserControl_Loaded;
        }

        private void LanguagePanelUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillLanguageComboBox();
            LanguagePanelProperties.ToPanel(Settings.LanguageParams);
        }

        #endregion

        #region Helper Procedures

        private void FillLanguageComboBox()
        {
            LanguageComboBox.Items.Add(LanguageEnum.English.ToString());
            LanguageComboBox.Items.Add(LanguageEnum.Türkçe.ToString());
        }

        #endregion
    }
}
