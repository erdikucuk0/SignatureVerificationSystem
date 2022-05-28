using SignatureVerificationSystem.Models.Enums;
using SignatureVerificationSystem.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignatureVerificationSystem.SettingsModels
{
    public class LanguagePanelProperties : INotifyPropertyChanged
    {
        #region Member Fields

        private string language { get; set; } = LanguageEnum.English.ToString();
        public string Language
        {
            get
            {
                return language;
            }
            set
            {
                if (language != value)
                {
                    language = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Language"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public LanguageParameters ToConfiguration()
        {
            return new LanguageParameters(language);
        }

        public void ToPanel(LanguageParameters languageParameters)
        {
            Language = languageParameters.Language;
        }
    }
}