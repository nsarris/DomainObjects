using Dynamix.Reflection;
using System.Collections.Generic;

namespace FluentValidation.Resources
{
    public static class HackLanguageManager
    {
        public static void AddLanguage(Language language)
        {

            var languageManager = ValidatorOptions.LanguageManager;
            var field = languageManager.GetType().GetFieldEx("_languages", BindingFlagsEx.AllInstance);
            var languages = (Dictionary<string, Language>)field.Get(languageManager);
            languages.Add(language.Name, language);
        }
    }
}