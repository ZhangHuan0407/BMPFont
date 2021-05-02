namespace HotFix.Database
{
    /// <summary>
    /// 语言资源包项
    /// </summary>
    public class LanguageAssetBundle : IDataItem<string>
    {
        /* field */
        /// <summary>
        /// 语言名称
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 界面语言包
        /// </summary>
        public string InterfaceLanguageAssetBundleName { get; set; }

        /* inter */
        public string PrimaryKey => Language;

        /* func */
        public bool DataItemIsEqual(IDataItem<object> dataItem)
        {
            if (dataItem is LanguageAssetBundle languageAssetBundle)
                return Language.Equals(languageAssetBundle.Language);
            else
                return false;
        }
    }
}