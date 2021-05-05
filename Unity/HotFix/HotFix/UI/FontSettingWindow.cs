using Encoder;
using System;
using System.Collections.Generic;
using Tween;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI
{
    [BindingUpdatableComponent(BindingUpdatableComponentAttribute.ContainerComponent)]
    public class FontSettingWindow
    {
        /* const */

        /* field */
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "窗体动画控制")]
        private Animator m_WindowAnimator;

        private UpdatableComponent m_UpdatableComponent;

        /* inter */
        protected GameObject gameObject;
        protected Transform transform;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /* ctor */
        public FontSettingWindow(Dictionary<string, object> deserializeDictionary)
        {
            m_UpdatableComponent = deserializeDictionary[nameof(UpdatableComponent)] as UpdatableComponent ?? throw new ArgumentException($"{nameof(UpdatableComponent)} is null.");
            gameObject = m_UpdatableComponent.gameObject;
            transform = m_UpdatableComponent.transform;

            if (deserializeDictionary.TryGetValue(nameof(m_WindowAnimator), out object windowAnimator_object)
                && windowAnimator_object is Animator windowAnimator_animator)
                m_WindowAnimator = windowAnimator_animator;
            else
                m_WindowAnimator = null;
        }
        public static FontSettingWindow OpenWindow()
        {
            GameObject go = GameSystemData.Instance.InstantiateGo(nameof(FontSettingWindow));
            FontSettingWindow window = null;
            foreach (UpdatableComponent updatableComponent in go.GetComponents<UpdatableComponent>())
            {
                if (typeof(FontSettingWindow).FullName.Equals(updatableComponent.ILTypeFullName))
                {
                    window = updatableComponent.InstanceObject as FontSettingWindow;
                    break;
                }
            }
            if (window is null)
                throw new NullReferenceException($"Not found {nameof(FontSettingWindow)} in {nameof(UpdatableComponent)}.");
            return window;
        }

        /* func */
        [InvokeAction(IsRuntimeAction = true)]
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
    }
}