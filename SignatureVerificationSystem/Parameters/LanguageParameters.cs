using SignatureVerificationSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignatureVerificationSystem.Parameters
{
    public class LanguageParameters
    {
        #region Member Fields

        public string Language { get; set; } = LanguageEnum.English.ToString();

        #endregion

        #region Constructors

        public LanguageParameters() { }

        public LanguageParameters(string language)
        {
            Language = language;
        }

        #endregion
    }
}