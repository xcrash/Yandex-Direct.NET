using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Yandex.Direct.Configuration
{
    internal class YandexDirectSection : ConfigurationSection
    {
        #region Default Instance

        protected YandexDirectSection() { }

        const string SectionPath = "yandex.direct";
        static YandexDirectSection Instance;
        
        public static YandexDirectSection Default
        {
            [DebuggerStepThrough]
            get
            {
                if (Instance == null)
                {
                    var exeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    Instance = exeConfiguration.GetSection(SectionPath) as YandexDirectSection;
                    if (Instance == null)
                        throw new ConfigurationErrorsException(SectionPath + " not found in configuration file");
                }

                return Instance;
            }
        }

        #endregion
    }
}
