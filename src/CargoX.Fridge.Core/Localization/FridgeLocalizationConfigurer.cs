using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace CargoX.Fridge.Localization
{
    public static class FridgeLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(FridgeConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(FridgeLocalizationConfigurer).GetAssembly(),
                        "CargoX.Fridge.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
