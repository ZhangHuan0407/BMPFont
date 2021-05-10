using Encoder;
using HotFix.UI.Component;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI
{
    [BindingUpdatableComponent(BindingUpdatableComponentAttribute.ContainerComponent)]
    public class FileAndDirectoryWindow
    {
        /* const */

        /* field */
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "窗体动画控制")]
        private Animator m_WindowAnimator;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "标题")]
        private Text m_TitleText;
        public string Title
        {
            get
            {
                if (m_TitleText)
                    return m_TitleText.text;
                else
                    return null;
            }
            set
            {
                if (m_TitleText)
                    m_TitleText.text = value;
            }
        }

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "全路径")]
        private InputField m_FullPathText;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "确定按钮")]
        private Button m_OKButton;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "取消按钮")]
        private Button m_CancleButton;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "滑动区域挂载位置")]
        private RectTransform m_ScrollContentTrans;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "缓存节点")]
        private Transform m_CacheTrans;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "文件对象预制体")]
        private GameObject m_FileItemPrefab;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "文件夹对象预制体")]
        private GameObject m_DirectoryItemPrefab;

        private UpdatableComponent m_UpdatableComponent;


        /// <summary>
        /// 当前选择的文件系统信息项
        /// </summary>
        public FileSystemInfo SelectedFileSystemInfo;
        /// <summary>
        /// 当前展示的文件系统信息项
        /// </summary>
        private List<FileSystemInfo> FileSystemInfos;

        /// <summary>
        /// 当前展示的文件和文件夹项
        /// </summary>
        private List<IFileAndDirectoryItem> FileAndDirectoryItems;
        /// <summary>
        /// 缓存不显示的文件项
        /// </summary>
        private Stack<IFileAndDirectoryItem> FileItemsCache;
        /// <summary>
        /// 缓存不显示的文件夹项
        /// </summary>
        private Stack<IFileAndDirectoryItem> DirectoryItemsCache;

        public Action<FileAndDirectoryWindow> OKCallback;
        public Action<FileAndDirectoryWindow> CancleCallback;

        /* inter */
        protected GameObject gameObject;
        protected Transform transform;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /* ctor */
        public FileAndDirectoryWindow(Dictionary<string, object> deserializeDictionary)
        {
            m_UpdatableComponent = deserializeDictionary[nameof(UpdatableComponent)] as UpdatableComponent ?? throw new ArgumentException($"{nameof(UpdatableComponent)} is null.");
            gameObject = m_UpdatableComponent.gameObject;
            transform = m_UpdatableComponent.transform;

            if (deserializeDictionary.TryGetValue(nameof(m_WindowAnimator), out object windowAnimator_object)
                && windowAnimator_object is Animator windowAnimator_animator)
                m_WindowAnimator = windowAnimator_animator;
            else
                m_WindowAnimator = null;

            if (deserializeDictionary.TryGetValue(nameof(m_TitleText), out object titleText_object)
                && titleText_object is Text titleText_text)
                m_TitleText = titleText_text;
            else
                m_TitleText = null;

            if (deserializeDictionary.TryGetValue(nameof(m_FullPathText), out object fullPathText_object)
                && fullPathText_object is InputField fullPathText_text)
                m_FullPathText = fullPathText_text;
            else
                m_FullPathText = null;

            if (deserializeDictionary.TryGetValue(nameof(m_OKButton), out object okButton_object)
                && okButton_object is Button okButton_button)
                m_OKButton = okButton_button;
            else
                m_OKButton = null;

            if (deserializeDictionary.TryGetValue(nameof(m_CancleButton), out object cancleButton_object)
                && cancleButton_object is Button cancleButton_button)
                m_CancleButton = cancleButton_button;
            else
                m_CancleButton = null;

            if (deserializeDictionary.TryGetValue(nameof(m_CacheTrans), out object cache_object)
                && cache_object is RectTransform cache_transform)
                m_CacheTrans = cache_transform;
            else
                m_CacheTrans = null;

            if (deserializeDictionary.TryGetValue(nameof(m_ScrollContentTrans), out object scrollContentTrans_object)
                && scrollContentTrans_object is RectTransform scrollContent_transform)
                m_ScrollContentTrans = scrollContent_transform;
            else
                m_ScrollContentTrans = null;

            if (deserializeDictionary.TryGetValue(nameof(m_FileItemPrefab), out object fileItemPrefab_object)
                && fileItemPrefab_object is GameObject fileItem_gameObject)
                m_FileItemPrefab = fileItem_gameObject;
            else
                m_FileItemPrefab = null;

            if (deserializeDictionary.TryGetValue(nameof(m_DirectoryItemPrefab), out object directoryItemPrefab_object)
                && directoryItemPrefab_object is GameObject directoryItem_gameObject)
                m_DirectoryItemPrefab = directoryItem_gameObject;
            else
                m_DirectoryItemPrefab = null;


            FileSystemInfos = new List<FileSystemInfo>();

            FileAndDirectoryItems = new List<IFileAndDirectoryItem>();
            FileItemsCache = new Stack<IFileAndDirectoryItem>();
            DirectoryItemsCache = new Stack<IFileAndDirectoryItem>();
        }
        /// <summary>
        /// 开启一个文件与文件夹选择窗体
        /// </summary>
        /// <param name="fileSystemInfo">指向的文件系统信息，默认指向当前文件夹</param>
        /// <returns>窗体实例</returns>
        public static FileAndDirectoryWindow OpenWindow(FileSystemInfo fileSystemInfo = null)
        {
            if (fileSystemInfo is null)
                fileSystemInfo = new DirectoryInfo(Environment.CurrentDirectory);

            // 当一个文件不存在时，不要试图拿取文件夹
            if (!fileSystemInfo.Exists)
                fileSystemInfo = null;

            GameObject go = GameSystemData.Instance.InstantiateGo(nameof(FileAndDirectoryWindow));
            FileAndDirectoryWindow window = null;
            foreach (UpdatableComponent updatableComponent in go.GetComponents<UpdatableComponent>())
            {
                if (typeof(FileAndDirectoryWindow).FullName.Equals(updatableComponent.ILTypeFullName))
                {
                    window = updatableComponent.InstanceObject as FileAndDirectoryWindow;
                    break;
                }
            }
            if (window is null)
                throw new NullReferenceException($"Not found {nameof(FileAndDirectoryWindow)} in {nameof(UpdatableComponent)}.");
            
            window.SelectedFileSystemInfo = fileSystemInfo;
            window.RefreshUI();
            return window;
        }

        /* func */
        [MarkingAction(IsRuntimeAction = true)]
        public void OnEnable()
        {
            m_WindowAnimator?.Play("Appear");
            RefreshUI();
        }

        /// <summary>
        /// 强制刷新 UI，使得当前展示的可选项信息是基于<see cref="SelectedFileSystemInfo"/>得到的
        /// </summary>
        public void RefreshUI()
        {
            foreach (IFileAndDirectoryItem item in FileAndDirectoryItems)
            {
                item.transform.SetParent(m_CacheTrans, false);
                item.gameObject.SetActive(false);
                if (item is FileItem)
                    FileItemsCache.Push(item);
                else if (item is DirectoryItem)
                    DirectoryItemsCache.Push(item);
            }
            FileSystemInfos.Clear();
            FileAndDirectoryItems.Clear();

            // 获取选择的文件夹
            DirectoryInfo selectedDirectory = null;
            if (SelectedFileSystemInfo is FileInfo selectedFileInfo && selectedFileInfo.Exists)
                selectedDirectory = selectedFileInfo.Directory;
            else if (SelectedFileSystemInfo is DirectoryInfo directoryInfo && directoryInfo.Exists)
                selectedDirectory = directoryInfo;

            if (m_FullPathText)
                m_FullPathText.text = SelectedFileSystemInfo?.FullName;

            // 无有效文件夹选择
            if (selectedDirectory is null
                || !Directory.Exists(selectedDirectory.FullName))
                return;

            // 第一项是返回上一级
            if (selectedDirectory.Parent != null)
            {
                IFileAndDirectoryItem item = GetDirectoryItem();
                item.ItemName = "../";
                FileSystemInfos.Add(selectedDirectory.Parent);
                FileAndDirectoryItems.Add(item);
            }
            foreach (string directoryPath in Directory.GetDirectories(selectedDirectory.FullName))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                IFileAndDirectoryItem item = GetDirectoryItem();
                item.ItemName = directoryInfo.Name;
                FileSystemInfos.Add(directoryInfo);
                FileAndDirectoryItems.Add(item);
            }
            foreach (string filePath in Directory.GetFiles(selectedDirectory.FullName))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                IFileAndDirectoryItem item = GetFileItem();
                item.ItemName = fileInfo.Name;
                FileSystemInfos.Add(fileInfo);
                FileAndDirectoryItems.Add(item);
            }

            for (int index = 0; index < FileAndDirectoryItems.Count; index++)
            {
                IFileAndDirectoryItem item = FileAndDirectoryItems[index];
                FileSystemInfo fileSystemInfo = FileSystemInfos[index];
                item.DateTime = $"{fileSystemInfo.LastWriteTime:yy/MM/dd HH:mm:ss}";
                item.Index = index;
                item.Window = this;
                item.transform.SetParent(m_ScrollContentTrans, false);
                item.gameObject.SetActive(true);
                item.NotSelected();
            }
        }

        /// <summary>
        /// 从缓存中拿取一个文件夹对象，如果没有则创建一个
        /// </summary>
        private IFileAndDirectoryItem GetDirectoryItem()
        {
            if (DirectoryItemsCache.Count > 0)
                return DirectoryItemsCache.Pop();
            else
            {
                GameObject go = UnityEngine.Object.Instantiate(m_DirectoryItemPrefab);
                foreach (UpdatableComponent updatableComponent in go.GetComponents<UpdatableComponent>())
                {
                    if (typeof(DirectoryItem).FullName.Equals(updatableComponent.ILTypeFullName))
                        return updatableComponent.InstanceObject as IFileAndDirectoryItem;
                }
                // 没有找到必要的组件
                throw new NullReferenceException($"Not found {nameof(DirectoryItem)} in {nameof(UpdatableComponent)}.");
            }
        }
        /// <summary>
        /// 从缓存中拿取一个文件对象，如果没有则创建一个
        /// </summary>
        private IFileAndDirectoryItem GetFileItem()
        {
            if (FileItemsCache.Count > 0)
                return FileItemsCache.Pop();
            else
            {
                GameObject go = UnityEngine.Object.Instantiate(m_FileItemPrefab);
                foreach (UpdatableComponent updatableComponent in go.GetComponents<UpdatableComponent>())
                {
                    if (typeof(FileItem).FullName.Equals(updatableComponent.ILTypeFullName))
                        return updatableComponent.InstanceObject as IFileAndDirectoryItem;
                }
                // 没有找到必要的组件
                throw new NullReferenceException($"Not found {nameof(FileItem)} in {nameof(UpdatableComponent)}.");
            }
        }

        private IFileAndDirectoryItem m_LastTouchItem;
        /// <summary>
        /// 单击界面上的这一选项
        /// </summary>
        /// <param name="item">目标选项</param>
        public void TouchThisItem(IFileAndDirectoryItem item)
        {
            for (int index = 0; index < FileAndDirectoryItems.Count; index++)
            {
                IFileAndDirectoryItem fileAndDirectoryItem = FileAndDirectoryItems[index];
                if (fileAndDirectoryItem == item)
                    fileAndDirectoryItem.Selected();
                else
                    fileAndDirectoryItem.NotSelected();
            }

            if (m_LastTouchItem != item)
            {
                m_LastTouchItem = item;
                if (item is FileItem)
                {
                    SelectedFileSystemInfo = FileSystemInfos[item.Index];
                    m_FullPathText.text = SelectedFileSystemInfo.FullName;
                }
            }
            else
            {
                SelectedFileSystemInfo = FileSystemInfos[item.Index];
                RefreshUI();
            }
        }

        [MarkingAction(IsRuntimeAction = true)]
        public void OnClickRefreshButton()
        {
            string fullPath = m_FullPathText.text ?? string.Empty.Replace('\\', '/');
            // 视作文件夹
            if (Directory.Exists(fullPath))
                SelectedFileSystemInfo = new DirectoryInfo(fullPath);
            else
                SelectedFileSystemInfo = new FileInfo(fullPath);
            
            RefreshUI();
        }

        [MarkingAction(IsRuntimeAction = true)]
        public void OnClickOKButton()
        {
            Debug.Log($"Invoke {nameof(MakeSureWindow)}.{nameof(OnClickOKButton)}");
            m_LastTouchItem = null;
            m_OKButton.interactable = false;
            m_CancleButton.interactable = false;
            OKCallback?.Invoke(this);
            if (m_WindowAnimator)
            {
                m_WindowAnimator.Play("Disappear");
                ILRuntimeService.StartILCoroutine(WaitToDestroy());
            }
            else
                UnityEngine.Object.Destroy(gameObject);
        }

        [MarkingAction(IsRuntimeAction = true)]
        public void OnClickCancleButton()
        {
            Debug.Log($"Invoke {nameof(MakeSureWindow)}.{nameof(OnClickCancleButton)}");
            m_LastTouchItem = null;
            m_OKButton.interactable = false;
            m_CancleButton.interactable = false;
            CancleCallback?.Invoke(this);
            if (m_WindowAnimator)
            {
                m_WindowAnimator.Play("Disappear");
                ILRuntimeService.StartILCoroutine(WaitToDestroy());
            }
            else
                UnityEngine.Object.Destroy(gameObject);
        }

        private IEnumerator<object> WaitToDestroy()
        {
            while (m_UpdatableComponent
                && m_WindowAnimator)
            {
                if (m_WindowAnimator.GetCurrentAnimatorStateInfo(0).IsName("DisappearIdle"))
                {
                    UnityEngine.Object.Destroy(gameObject);
                    yield break;
                }
                else
                    yield return null;
            }
        }
    }
}