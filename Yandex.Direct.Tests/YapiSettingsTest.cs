using Yandex.Direct;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Yandex.Direct.Tests
{
    
    
    /// <summary>
    ///This is a test class for YapiSettingsTest and is intended
    ///to contain all YapiSettingsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class YapiSettingsTest
    {
        [TestMethod, Description("Секция yandex.direct читается")]
        public void SettingsCanBeReadFromConfiguration()
        {
            var settings = YapiSettings.FromConfiguration();
            settings.CertificatePath.ShouldNotBeNull();
            settings.Language.ShouldBe(YapiLanguage.Russian);
        }
    }
}
