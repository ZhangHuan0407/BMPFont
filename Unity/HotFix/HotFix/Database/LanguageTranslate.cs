using System;

namespace HotFix.Database
{
    /// <summary>
    /// 语言转译
    /// </summary>
    public class LanguageTranslate : IDataItem<string>
    {
        /* field */
        public string MeaningString { get; set; }
        public string TranslateString { get; set; }

        public string PrimaryKey => throw new NotImplementedException();

        public bool DataItemIsEqual(IDataItem<object> dataItem)
        {
            if (dataItem is LanguageTranslate languageTranslate)
                return MeaningString.Equals(languageTranslate.MeaningString);
            else
                return false;
        }
    }
}