using Encoder;
using System;
using System.Collections.Generic;
using System.IO;
using Tween;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI
{
    [BindingUpdatableComponent(BindingUpdatableComponentAttribute.ContainerComponent)]
    public class MenuWindow
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
            Title = "加载字体按钮")]
        private Button m_LoadFontButton;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "退出按钮")]
        private Button m_ExitButton;

        private UpdatableComponent m_UpdatableComponent;

        /* inter */
        protected GameObject gameObject;
        protected Transform transform;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /* ctor */
        public MenuWindow(Dictionary<string, object> deserializeDictionary)
        {
            m_UpdatableComponent = deserializeDictionary[nameof(UpdatableComponent)] as UpdatableComponent ?? throw new ArgumentException($"{nameof(UpdatableComponent)} is null.");
            gameObject = m_UpdatableComponent.gameObject;
            transform = m_UpdatableComponent.transform;

            if (deserializeDictionary.TryGetValue(nameof(m_WindowAnimator), out object windowAnimator_object)
                && windowAnimator_object is Animator windowAnimator_animator)
                m_WindowAnimator = windowAnimator_animator;
            else
                m_WindowAnimator = null;

            if (deserializeDictionary.TryGetValue(nameof(m_LoadFontButton), out object loadFontButton_object)
                && loadFontButton_object is Button loadFontButton_button)
                m_LoadFontButton = loadFontButton_button;
            else
                m_LoadFontButton = null;

            if (deserializeDictionary.TryGetValue(nameof(m_ExitButton), out object exitButton_object)
                && exitButton_object is Button exitButton_button)
                m_ExitButton = exitButton_button;
            else
                m_ExitButton = null;
        }
        public static MenuWindow OpenWindow()
        {
            GameObject go = GameSystemData.Instance.InstantiateGo(nameof(MenuWindow));
            MenuWindow window = null;
            foreach (UpdatableComponent updatableComponent in go.GetComponents<UpdatableComponent>())
            {
                if (typeof(MenuWindow).FullName.Equals(updatableComponent.ILTypeFullName))
                {
                    window = updatableComponent.InstanceObject as MenuWindow;
                    break;
                }
            }
            if (window is null)
                throw new NullReferenceException($"Not found {nameof(MenuWindow)} in {nameof(UpdatableComponent)}.");
            return window;
        }

        /* func */
        [MarkingAction(IsRuntimeAction = true)]
        public void OnEnable()
        {
            m_WindowAnimator?.Play("Appear");
        }

        public void StartToClose()
        {
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

        [MarkingAction(IsRuntimeAction = true)]
        public void OnClickLoadFontButton()
        {
            Debug.Log($"{nameof(MenuWindow)}.{nameof(OnClickLoadFontButton)}");

            string latestBMPFontPath = Setting.Instance.LatestBMPFontPath;
            FileInfo fontFileInfo = null;
            if (!string.IsNullOrWhiteSpace(latestBMPFontPath))
                fontFileInfo = new FileInfo(latestBMPFontPath);
            FileAndDirectoryWindow window = FileAndDirectoryWindow.OpenWindow(fontFileInfo);
            window.Title = "读取一个 *.fnt 文件";
            window.OKCallback += (_) => 
            {
                string fontPath = window.SelectedFileSystemInfo.FullName;
                Debug.Log($"{window.SelectedFileSystemInfo.GetType().Name} : {fontPath}");
                Setting.Instance.LatestBMPFontPath = fontPath;
                Setting.Instance.SaveOutPlayerPrefs();
                GameSystemData.Instance.Font?.Dispose();
                BMPFont font = new BMPFont();
                font.LoadFontFromFile(window.SelectedFileSystemInfo.FullName);
                font.UseIt();
            };
        }

        [MarkingAction(IsRuntimeAction = true)]
        public void OnClickExitButton()
        {
            Debug.Log($"{nameof(MenuWindow)}.{nameof(OnClickExitButton)}");
            Application.Quit();
        }
    }
}