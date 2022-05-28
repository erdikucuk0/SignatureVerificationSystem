using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLocalizeExtension.Engine;

namespace SignatureVerificationSystem.Resources
{
    public class LocalizedStrings
    {
        public LocalizedStrings() { }

        public static LocalizedStrings Instance { get; } = new LocalizedStrings();

        public void SetCulture(string cultureCode)
        {
            var cultureInfo = new CultureInfo(cultureCode);
            LocalizeDictionary.Instance.Culture = cultureInfo;
        }

        public string this[string key]
        {
            get
            {
                var result = LocalizeDictionary.Instance.GetLocalizedObject("SignatureVerificationSystem", "Strings", key, LocalizeDictionary.Instance.Culture);
                return result as string;
            }
        }
    }
}