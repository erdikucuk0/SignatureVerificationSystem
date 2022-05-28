using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignatureVerificationSystem.Models
{
    public static class Paths
    {
        #region General Path

        public static readonly string APPLICATION_DATA = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SignatureVerificationSystem");

        #endregion

        #region Configuration Path

        public static readonly string ModelConfiguration = Path.Combine(APPLICATION_DATA, "ModelConfigurations.json");
        public static readonly string LanguageConfiguration = Path.Combine(APPLICATION_DATA, "LanguageConfigurations.json");

        public static readonly string SupportSetFileNames = Path.Combine(APPLICATION_DATA, "SupportSetFileNames.json");
        public static readonly string QuerySetFileName = Path.Combine(APPLICATION_DATA, "QuerySetFileName.json");

        #endregion

        #region Images Path

        public static readonly string Images = Path.Combine(APPLICATION_DATA, "Images");
        public static readonly string SupportSetImages = Path.Combine(Images, "SupportSet");
        public static readonly string QuerySetImages = Path.Combine(Images, "QuerySet");

        #endregion

        #region Python Path

        public static readonly string PythonExe = @"C:\Users\erdik\AppData\Local\Programs\Python\Python38\python.exe";
        public static readonly string PythonScript = Path.Combine(APPLICATION_DATA, "classifier.py");

        #endregion

        #region Txt Files Path

        public static readonly string EstimatedClass = Path.Combine(Paths.APPLICATION_DATA, "estimated_class.txt");
        public static readonly string EstimatedDistances = Path.Combine(APPLICATION_DATA, "estimated_distances.txt");
        public static readonly string ClassNames = Path.Combine(Paths.APPLICATION_DATA, "class_names.txt");

        #endregion
    }
}
