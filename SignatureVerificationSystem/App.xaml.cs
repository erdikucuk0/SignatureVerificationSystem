using SignatureVerificationSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFLocalizeExtension.Engine;

namespace SignatureVerificationSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Settings.LoadSettings();

            if (Settings.LanguageParams.Language == "English")
                LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo("en-US");
            else
                LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo("tr-TR");
        }
    }
}
