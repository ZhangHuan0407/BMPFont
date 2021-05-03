using UnityEngine;

namespace HotFix.UI.Component
{
    /// <summary>
    /// 表述一个文件或文件夹实例
    /// </summary>
    public interface IFileAndDirectoryItem
    {
        /* field */
        public string DateTime { get; set; }
        public string FileName { get; set; }
        public int Index { get; set; }
        public FileAndDirectoryWindow Window { get; set; }

        /* inter */
        public GameObject gameObject { get; }
        public Transform transform { get; }

        /* func */
        public void NotSelected();
    }
}