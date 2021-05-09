using Encoder;
using HotFix.UI.Component;
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

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "BMP Font Common 属性")]
        private UpdatableComponent m_UIBMPFontCommon;
        internal UIBMPFontCommon UIBMPFontCommon;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "BMP Font Info 属性")]
        private UpdatableComponent m_UIBMPFontInfo;
        internal UIBMPFontInfo UIBMPFontInfo;

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

            if (deserializeDictionary.TryGetValue(nameof(m_UIBMPFontInfo), out object uiBMPFontInfo_object)
                && uiBMPFontInfo_object is UpdatableComponent uiBMPFontInfo_component
                && uiBMPFontInfo_component.InstanceObject != null)
                UIBMPFontInfo = uiBMPFontInfo_component.InstanceObject as UIBMPFontInfo;
            else
                UIBMPFontInfo = null;
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

            BMPFont.LoadedNewBMPFont_Handle += window.RefreshUI;
            BMPFont.DisposeBMPFont_Handle += window.RefreshUI;

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
            BMPFont.LoadedNewBMPFont_Handle -= RefreshUI;
            BMPFont.DisposeBMPFont_Handle -= RefreshUI;

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

        public void RefreshUI()
        {

        }
    }
}