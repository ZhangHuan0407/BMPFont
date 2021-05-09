using Encoder;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI
{
    [BindingUpdatableComponent(BindingUpdatableComponentAttribute.ContainerComponent)]
    public class MessageWindow
    {
        /* const */
        public static MessageWindow ActiveMessageWindow;

        /* field */
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "窗体动画控制")]
        private Animator m_WindowAnimator;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "文字内容")]
        private Text m_ContentText;

        private UpdatableComponent m_UpdatableComponent;


        private StringBuilder m_MessageBuilder;

        /* inter */
        protected GameObject gameObject;
        protected Transform transform;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /* ctor */
        public MessageWindow(Dictionary<string, object> deserializeDictionary)
        {
            m_UpdatableComponent = deserializeDictionary[nameof(UpdatableComponent)] as UpdatableComponent ?? throw new ArgumentException($"{nameof(UpdatableComponent)} is null.");
            gameObject = m_UpdatableComponent.gameObject;
            transform = m_UpdatableComponent.transform;

            if (deserializeDictionary.TryGetValue(nameof(m_WindowAnimator), out object windowAnimator_object)
                && windowAnimator_object is Animator windowAnimator_animator)
                m_WindowAnimator = windowAnimator_animator;
            else
                m_WindowAnimator = null;

            if (deserializeDictionary.TryGetValue(nameof(m_ContentText), out object contentText_object)
                && contentText_object is Text contentText_text)
                m_ContentText = contentText_text;
            else
                m_ContentText = null;

            m_MessageBuilder = new StringBuilder();
        }
        public static MessageWindow OpenWindow()
        {
            GameObject go = GameSystemData.Instance.InstantiateGo(nameof(MessageWindow));
            MessageWindow window = null;
            foreach (UpdatableComponent updatableComponent in go.GetComponents<UpdatableComponent>())
            {
                if (typeof(MessageWindow).FullName.Equals(updatableComponent.ILTypeFullName))
                {
                    window = updatableComponent.InstanceObject as MessageWindow;
                    break;
                }
            }
            if (window is null)
                throw new NullReferenceException($"Not found {nameof(MessageWindow)} in {nameof(UpdatableComponent)}.");
            ActiveMessageWindow = window;
            return window;
        }

        /* func */
        [MarkingAction(IsRuntimeAction = true)]
        public void OnEnable()
        {
            m_WindowAnimator?.Play("Appear");
        }

        [MarkingAction(IsRuntimeAction = true)]
        public void OnClickCloseButton()
        {
            Debug.Log($"Invoke {nameof(MessageWindow)}.{nameof(OnClickCloseButton)}");
            ActiveMessageWindow = null;
            m_WindowAnimator.Play("Disappear");
            ILRuntimeService.StartILCoroutine(WaitToDestroy());
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

        public void AppendLineAndRefresh(string content)
        {
            m_MessageBuilder.AppendLine(content);
            m_ContentText.text = m_MessageBuilder.ToString();
        }
    }
}