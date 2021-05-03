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
        /// <summary>
        /// <see cref="MakeSureWindow"/> 预制体
        /// </summary>
        public static GameObject Prefab;

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
        public string TitleText
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
        internal List<FileSystemInfo> FileSystemInfos;

        /// <summary>
        /// 当前展示的文件和文件夹项
        /// </summary>
        internal List<IFileAndDirectoryItem> FileAndDirectoryItems;
        /// <summary>
        /// 缓存不显示的文件项
        /// </summary>
        internal Stack<IFileAndDirectoryItem> FileItemsCache;
        /// <summary>
        /// 缓存不显示的文件夹项
        /// </summary>
        internal Stack<IFileAndDirectoryItem> DirectoryItemsCache;

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

        /* func */
        [InvokeAction(IsRuntimeAction = true)]
        public void OnEnable()
        {
            m_WindowAnimator?.Play("Appear");
            RefreshUI();
        }

        /// <summary>
        /// 强制刷新 UI，使得当前展示的可选项信息是基于<see cref="SelectedFileSystemInfo"/>得到的
        /// </summary>
        [InvokeAction(IsRuntimeAction = true)]
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

            // 无有效文件夹选择
            if (selectedDirectory is null
                || !Directory.Exists(selectedDirectory.FullName))
                return;

            foreach (string directoryPath in Directory.GetDirectories(selectedDirectory.FullName))
            {
                FileSystemInfos.Add(new DirectoryInfo(directoryPath));
                FileAndDirectoryItems.Add(GetDirectoryItem());
            }
            foreach (string filePath in Directory.GetFiles(selectedDirectory.FullName))
            {
                FileSystemInfos.Add(new FileInfo(filePath));
                FileAndDirectoryItems.Add(GetFileItem());
            }

            for (int index = 0; index < FileAndDirectoryItems.Count; index++)
            {
                IFileAndDirectoryItem item = FileAndDirectoryItems[index];
                FileSystemInfo fileSystemInfo = FileSystemInfos[index];
                item.FileName = fileSystemInfo.Name;
                item.DateTime = fileSystemInfo.LastWriteTime.ToString();
                item.Window = this;
                item.gameObject.SetActive(true);
                item.NotSelected();
            }
        }

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

        /// <summary>
        /// 上一次的单击时间
        /// </summary>
        private float m_LastClickTime = 0f;
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
                if (fileAndDirectoryItem != item)
                    fileAndDirectoryItem.NotSelected();
            }

            // 算作单击
            if (Time.time - m_LastClickTime > 0.5f
                || m_LastTouchItem != item)
            {
                m_LastTouchItem = item;
                m_LastClickTime = Time.time;
            }
            else
            {
                SelectedFileSystemInfo = FileSystemInfos[item.Index];
                RefreshUI();
            }
        }

        [InvokeAction(IsRuntimeAction = true)]
        public void OnClickOKButton()
        {
            Debug.Log($"Invoke {nameof(MakeSureWindow)}.{nameof(OnClickOKButton)}");
            m_OKButton.interactable = false;
            m_CancleButton.interactable = false;
            if (m_WindowAnimator)
            {
                m_WindowAnimator.Play("Disappear");
                ILRuntimeService.StartILCoroutine(WaitToDestroy());
            }
            else
                UnityEngine.Object.Destroy(gameObject);
        }

        [InvokeAction(IsRuntimeAction = true)]
        public void OnClickCancleButton()
        {
            Debug.Log($"Invoke {nameof(MakeSureWindow)}.{nameof(OnClickCancleButton)}");
            m_OKButton.interactable = false;
            m_CancleButton.interactable = false;
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
                    Debug.Log("WaitToDestroy");
                    yield break;
                }
                else
                    yield return null;
            }
        }
    }
}