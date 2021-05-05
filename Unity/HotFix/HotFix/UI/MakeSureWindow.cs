using Encoder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI
{
    [BindingUpdatableComponent(BindingUpdatableComponentAttribute.ContainerComponent)]
    public class MakeSureWindow
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
        public string TitleText
        {
            get {
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
            Title = "提示内容")]
        private Text m_ContentText;
        private string ContentText
        {
            get
            {
                if (m_ContentText)
                    return m_ContentText.text;
                else
                    return null;
            }
            set
            {
                if (m_ContentText)
                    m_ContentText.text = value;
            }
        }

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "确认按钮")]
        private Button m_YesButton;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "拒绝按钮")]
        private Button m_NoButton;

        private UpdatableComponent m_UpdatableComponent;


        public Action<MakeSureWindow> YesCallback;
        public Action<MakeSureWindow> NoCallback;

        /* inter */
        protected GameObject gameObject;
        protected Transform transform;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /* ctor */
        public MakeSureWindow(Dictionary<string, object> deserializeDictionary)
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

            if (deserializeDictionary.TryGetValue(nameof(m_ContentText), out object contentText_object)
                && contentText_object is Text contentText_text)
                m_ContentText = contentText_text;
            else
                m_ContentText = null;

            if (deserializeDictionary.TryGetValue(nameof(m_YesButton), out object yesButton_object)
                && yesButton_object is Button yesButton_button)
                m_YesButton = yesButton_button;
            else
                m_YesButton = null;

            if (deserializeDictionary.TryGetValue(nameof(m_NoButton), out object noButton_object)
                && noButton_object is Button noButton_button)
                m_NoButton = noButton_button;
            else
                m_NoButton = null;
        }
        public static MakeSureWindow OpenWindow()
        {
            GameObject go = GameSystemData.Instance.InstantiateGo(nameof(RendererWindow));
            MakeSureWindow window = null;
            foreach (UpdatableComponent updatableComponent in go.GetComponents<UpdatableComponent>())
            {
                if (typeof(MakeSureWindow).FullName.Equals(updatableComponent.ILTypeFullName))
                {
                    window = updatableComponent.InstanceObject as MakeSureWindow;
                    break;
                }
            }
            if (window is null)
                throw new NullReferenceException($"Not found {nameof(MakeSureWindow)} in {nameof(UpdatableComponent)}.");
            return window;
        }

        /* func */
        [InvokeAction(IsRuntimeAction = true)]
        public void OnEnable()
        {
            m_WindowAnimator?.Play("Appear");
        }

        [InvokeAction(IsRuntimeAction = true)]
        public void OnClickYesButton()
        {
            Debug.Log($"Invoke {nameof(MakeSureWindow)}.{nameof(OnClickYesButton)}");
            m_YesButton.interactable = false;
            m_NoButton.interactable = false;
            YesCallback?.Invoke(this);
            if (m_WindowAnimator)
            {
                m_WindowAnimator.Play("Disappear");
                ILRuntimeService.StartILCoroutine(WaitToDestroy());
            }
            else
                UnityEngine.Object.Destroy(gameObject);
        }

        [InvokeAction(IsRuntimeAction = true)]
        public void OnClickNoButton()
        {
            Debug.Log($"Invoke {nameof(MakeSureWindow)}.{nameof(OnClickNoButton)}");
            m_YesButton.interactable = false;
            m_NoButton.interactable = false;
            NoCallback?.Invoke(this);
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