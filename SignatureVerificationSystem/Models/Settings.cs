using SignatureVerificationSystem.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignatureVerificationSystem.Models
{
    public static class Settings
    {
        #region Member Fields

        public static ModelParameters ModelParams = new ModelParameters();
        public static LanguageParameters LanguageParams = new LanguageParameters();

        #endregion

        #region Load

        public static T LoadSetting<T>(string configurationFilePath)
        {
            var configString = File.ReadAllText(configurationFilePath);
            var settingObject = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(configString);
            return settingObject;
        }

        public static bool LoadSettings()
        {
            try
            {
                LoadModelConfiguration();
                LoadLanguageConfiguration();
               
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static void LoadModelConfiguration()
        {
            if (File.Exists(Paths.ModelConfiguration))
                ModelParams = LoadSetting<ModelParameters>(Paths.ModelConfiguration);
        }

        public static void LoadLanguageConfiguration()
        {
            if (File.Exists(Paths.LanguageConfiguration))
                LanguageParams = LoadSetting<LanguageParameters>(Paths.LanguageConfiguration);
        }

        #endregion

        #region Save

        public static void SaveSettings(object obj, string path)
        {
            var jsonObject = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            File.WriteAllText(path, jsonObject);
        }

        public static void SaveModelConfiguration()
        {
            SaveSettings(ModelParams, Paths.ModelConfiguration);
        }

        public static void SaveLanguageConfiguration()
        {
            SaveSettings(LanguageParams, Paths.LanguageConfiguration);
        }

        #endregion
    }
}