using SignatureVerificationSystem.SettingsModels;
using SignatureVerificationSystem.Models.Enums;
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
using SignatureVerificationSystem.Models;
using SignatureVerificationSystem.Utils;

namespace SignatureVerificationSystem.Windows.SettingsWindows
{
    /// <summary>
    /// Interaction logic for ModelPanelUserControl.xaml
    /// </summary>
    public partial class ModelPanelUserControl : UserControl, INotifyPropertyChanged
    {
        #region Member Fields

        private ModelPanelProperties modelPanelProperties { get; set; } = new ModelPanelProperties();
        public ModelPanelProperties ModelPanelProperties
        {
            get
            {
                return modelPanelProperties;
            }
            set
            {
                if (value != modelPanelProperties)
                {
                    modelPanelProperties = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ModelPanelProperties"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        public ModelPanelUserControl()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += ModelPanelUserControl_Loaded;
        }

        #endregion

        #region Loaded
        private void ModelPanelUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillImageSizeComboBox();
            FillNumberOfClassesComboBox();
            FillSupportSetComboBox();
            ModelPanelProperties.ToPanel(Settings.ModelParams);
        }

        #endregion

        #region Helper Procedures

        private void FillImageSizeComboBox()
        {
            ImageSizeComboBox.Items.Add(HelperProcedures.ModelParametersImageSizeConvert(ImageSize.Size_55x55));
            ImageSizeComboBox.Items.Add(HelperProcedures.ModelParametersImageSizeConvert(ImageSize.Size_105x105));
        }

        private void FillNumberOfClassesComboBox()
        {
            NumberOfClassesComboBox.Items.Add(HelperProcedures.ModelParametersNumberOfClassesConvert(NumberOfClasess.Twenty));
            NumberOfClassesComboBox.Items.Add(HelperProcedures.ModelParametersNumberOfClassesConvert(NumberOfClasess.Five));
        }

        private void FillSupportSetComboBox()
        {
            SupportSetComboBox.Items.Add(HelperProcedures.ModelParametersSupportSetConvert(SupportSet.Five));
            SupportSetComboBox.Items.Add(HelperProcedures.ModelParametersSupportSetConvert(SupportSet.One));
        }
        #endregion
    }
}