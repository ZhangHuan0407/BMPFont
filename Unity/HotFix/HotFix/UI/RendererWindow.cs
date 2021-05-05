using Encoder;
using System;
using System.Collections.Generic;
using Tween;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI
{
    [BindingUpdatableComponent(BindingUpdatableComponentAttribute.ContainerComponent)]
    public class RendererWindow
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
            Title = "放大按钮")]
        private Button m_PlusButton;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "缩小按钮")]
        private Button m_MinusButton;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "最大缩放比例")]
        private float m_MaxScale;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "最小缩放比例")]
        private float m_MinScale;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "展示内容")]
        private RectTransform m_ScrollContent;

        private UpdatableComponent m_UpdatableComponent;

        /* inter */
        protected GameObject gameObject;
        protected Transform transform;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /* ctor */
        public RendererWindow(Dictionary<string, object> deserializeDictionary)
        {
            m_UpdatableComponent = deserializeDictionary[nameof(UpdatableComponent)] as UpdatableComponent ?? throw new ArgumentException($"{nameof(UpdatableComponent)} is null.");
            gameObject = m_UpdatableComponent.gameObject;
            transform = m_UpdatableComponent.transform;

            if (deserializeDictionary.TryGetValue(nameof(m_WindowAnimator), out object windowAnimator_object)
                && windowAnimator_object is Animator windowAnimator_animator)
                m_WindowAnimator = windowAnimator_animator;
            else
                m_WindowAnimator = null;

            if (deserializeDictionary.TryGetValue(nameof(m_PlusButton), out object plusButton_object)
                && plusButton_object is Button plusButton_button)
                m_PlusButton = plusButton_button;
            else
                m_PlusButton = null;

            if (deserializeDictionary.TryGetValue(nameof(m_MinusButton), out object minusButton_object)
                && minusButton_object is Button minusButton_button)
                m_MinusButton = minusButton_button;
            else
                m_MinusButton = null;

            if (deserializeDictionary.TryGetValue(nameof(m_MaxScale), out object maxScale_object)
                && maxScale_object is float maxScale_float)
                m_MaxScale = maxScale_float;
            else
                m_MaxScale = 3f;

            if (deserializeDictionary.TryGetValue(nameof(m_MinScale), out object minScale_object)
                && minScale_object is float minScale_float)
                m_MinScale = minScale_float;
            else
                m_MinScale = 0.3f;

            if (deserializeDictionary.TryGetValue(nameof(m_ScrollContent), out object scrollContent_object)
                && scrollContent_object is RectTransform scrollContent_trans)
                m_ScrollContent = scrollContent_trans;
            else
                m_ScrollContent = null;
        }
        public static RendererWindow OpenWindow()
        {
            GameObject go = GameSystemData.Instance.InstantiateGo(nameof(RendererWindow));
            RendererWindow window = null;
            foreach (UpdatableComponent updatableComponent in go.GetComponents<UpdatableComponent>())
            {
                if (typeof(RendererWindow).FullName.Equals(updatableComponent.ILTypeFullName))
                {
                    window = updatableComponent.InstanceObject as RendererWindow;
                    break;
                }
            }
            if (window is null)
                throw new NullReferenceException($"Not found {nameof(RendererWindow)} in {nameof(UpdatableComponent)}.");
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

        [InvokeAction(IsRuntimeAction = true)]
        public void OnClickMinusButton()
        {
            Debug.Log($"{nameof(RendererWindow)}.{nameof(OnClickMinusButton)}");
            Vector3 scale = m_ScrollContent.localScale * 1.3f;
            if (scale.x >= m_MaxScale)
                return;
            else
                m_ScrollContent.localScale = scale;
        }

        [InvokeAction(IsRuntimeAction = true)]
        public void OnClickPlusButton()
        {
            Debug.Log($"{nameof(RendererWindow)}.{nameof(OnClickPlusButton)}");
            Vector3 scale = m_ScrollContent.localScale / 1.3f;
            if (scale.x <= m_MinScale)
                return;
            else
                m_ScrollContent.localScale = scale;
        }
    }
}