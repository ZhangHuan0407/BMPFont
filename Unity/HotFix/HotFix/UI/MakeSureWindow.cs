using Encoder;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI
{
    [BindingUpdatableComponent(BindingUpdatableComponentAttribute.ContainerComponent)]
    public class MakeSureWindow
    {
        /* const */
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
            get => m_TitleText?.text;
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
            get => m_ContentText?.text;
            set
            {
                if (m_ContentText)
                    m_ContentText.text = value;
            }
        }

        private UpdatableComponent m_UpdatableComponent;

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
        }

        /* func */
        [InvokeAction(IsRuntimeAction = true)]
        public void OnClickYesButton()
        {
            Debug.Log($"Invoke {nameof(MakeSureWindow)}.{nameof(OnClickYesButton)}");
        }
        [InvokeAction(IsRuntimeAction = true)]
        public void OnClickNoButton()
        {
            Debug.Log($"Invoke {nameof(MakeSureWindow)}.{nameof(OnClickNoButton)}");
        }
    }
}