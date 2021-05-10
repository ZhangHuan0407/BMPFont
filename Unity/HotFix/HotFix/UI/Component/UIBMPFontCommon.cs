using Encoder;
using HotFix.EncoderExtend;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI.Component
{
    /// <summary>
    /// 包装<see cref="BMPFontCommon"/>的界面显示
    /// </summary>
    [CheckComponent(typeof(Animator))]
    [BindingUpdatableComponent(BindingUpdatableComponentAttribute.CommonComponent)]
    public class UIBMPFontCommon
    {
        /* field */
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "组件动画控制")]
        private Animator m_Animator;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "折叠状态图片")]
        private Image m_FoldImage;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "展开状态图片")]
        private Image m_UnfoldImage;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "LineHelght")]
        private Text m_LineHelght;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "Base")]
        private Text m_Base;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "Scale")]
        private Text m_Scale;

        private UpdatableComponent m_UpdatableComponent;


        /// <summary>
        /// 当前是否处于折叠状态
        /// </summary>
        public bool IsFold { get; private set; }

        /* inter */
        protected GameObject gameObject;
        protected Transform transform;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释


        /// <summary>
        /// <see cref="BMPFont"/> 普通信息数据
        /// </summary>
        public BMPFontCommon BMPFontCommon { get; set; }

        /* ctor */
        public UIBMPFontCommon(Dictionary<string, object> deserializeDictionary)
        {
            m_UpdatableComponent = deserializeDictionary[nameof(UpdatableComponent)] as UpdatableComponent ?? throw new ArgumentException($"{nameof(UpdatableComponent)} is null.");
            gameObject = m_UpdatableComponent.gameObject;
            transform = m_UpdatableComponent.transform;

            if (deserializeDictionary.TryGetValue(nameof(m_Animator), out object windowAnimator_object)
                && windowAnimator_object is Animator windowAnimator_animator)
                m_Animator = windowAnimator_animator;
            else
                m_Animator = null;

            if (deserializeDictionary.TryGetValue(nameof(m_FoldImage), out object foldImage_object)
                && foldImage_object is Image foldImage_image)
                m_FoldImage = foldImage_image;
            else
                m_FoldImage = null;

            if (deserializeDictionary.TryGetValue(nameof(m_UnfoldImage), out object unfoldImage_object)
                && unfoldImage_object is Image unfoldImage_image)
                m_UnfoldImage = unfoldImage_image;
            else
                m_UnfoldImage = null;

            deserializeDictionary.TryPushValue(nameof(m_LineHelght), out m_LineHelght);
            deserializeDictionary.TryPushValue(nameof(m_Base), out m_Base);
            deserializeDictionary.TryPushValue(nameof(m_Scale), out m_Scale);
        }

        /* func */
        [MarkingAction(IsRuntimeAction = true)]
        public void OnEnable()
        {
            Fold();
        }

        [MarkingAction(IsRuntimeAction = true)]
        public void Fold()
        {
            IsFold = true;
            m_Animator.SetBool(nameof(IsFold), true);
            m_FoldImage.enabled = true;
            m_UnfoldImage.enabled = false;
        }
        [MarkingAction(IsRuntimeAction = true)]
        public void Unfold()
        {
            IsFold = false;
            m_Animator.SetBool(nameof(IsFold), false);
            m_FoldImage.enabled = false;
            m_UnfoldImage.enabled = true;
        }

        public void RefreshUI()
        {
            if (BMPFontCommon is null
                || BMPFontCommon.HaveError)
            {
                Fold();
                return;
            }

            Debug.Log($"{nameof(BMPFontCommon)} Refresh UI!");
        }

        [MarkingAction(IsRuntimeAction = true)]
        public void OnClickTitleButton()
        {
            Debug.Log($"{nameof(UIBMPFontCommon)}.{nameof(OnClickTitleButton)}");
            if (BMPFontCommon is null
                || BMPFontCommon.HaveError)
                Fold();
            else if (IsFold)
                Unfold();
            else
                Fold();
        }
    }
}