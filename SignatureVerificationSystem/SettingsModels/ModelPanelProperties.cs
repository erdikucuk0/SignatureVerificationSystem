using SignatureVerificationSystem.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignatureVerificationSystem.SettingsModels
{
    public class ModelPanelProperties : INotifyPropertyChanged
    {
        #region Member Fields

        private string imageSize { get; set; } = null;
        public string ImageSize
        {
            get
            {
                return imageSize;
            }
            set
            {
                if (imageSize != value)
                {
                    imageSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageSize"));
                }
                
            }  
        }

        private string numberOfClasses { get; set; } = "";
        public string NumberOfClasses
        {
            get
            {
                return numberOfClasses;
            }
            set
            {
                if (numberOfClasses != value)
                {
                    numberOfClasses = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NumberOfClasses"));
                }

            }
        }

        private string supportSet { get; set; } = "string";
        public string SupportSet
        {
            get
            {
                return supportSet;
            }
            set
            {
                if (supportSet != value)
                {
                    supportSet = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SupportSet"));
                }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Helper Procedures

        public ModelParameters ToConfiguration()
        {
            return new ModelParameters(ImageSize, Convert.ToInt32(NumberOfClasses), Convert.ToInt32(SupportSet));
        }

        public void ToPanel(ModelParameters modelParameters)
        {
            ImageSize = modelParameters.ImageSize;
            NumberOfClasses = modelParameters.NumberOfClasses.ToString();
            SupportSet = modelParameters.SupportSet.ToString();
        }

        #endregion
    }
}
