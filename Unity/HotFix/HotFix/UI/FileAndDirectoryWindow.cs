using Encoder;
using System;
using System.Collections.Generic;
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

        private UpdatableComponent m_UpdatableComponent;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "文件对象预制体")]
        private GameObject m_FileItemPrefab;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "文件夹对象预制体")]
        private GameObject m_DirectoryItemPrefab;

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
        }

        /* func */
        [InvokeAction(IsRuntimeAction = true)]
        public void OnEnable()
        {
            m_WindowAnimator?.Play("Appear");
        }

        [InvokeAction(IsRuntimeAction = true)]
        public void OnClickOKButton()
        {
            Debug.Log($"Invoke {nameof(MakeSureWindow)}.{nameof(OnClickOKButton)}");
            m_OKButton.interactable = false;
            m_CancleButton.interactable = false;
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