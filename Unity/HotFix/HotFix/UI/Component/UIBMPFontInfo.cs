using Encoder;
using HotFix.EncoderExtend;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI.Component
{
    /// <summary>
    /// 包装<see cref="BMPFontInfo"/>的界面显示
    /// </summary>
    [CheckComponent(typeof(Animator))]
    [BindingUpdatableComponent(BindingUpdatableComponentAttribute.CommonComponent)]
    public class UIBMPFontInfo
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
            Title = "Face")]
        private Text m_FaceText;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "Size")]
        private Text m_SizeText;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "Bold")]
        private Toggle m_BoldToggle;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "Italic")]
        private Toggle m_ItalicToggle;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "CharSet")]
        private Text m_CharSetText;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "Unicode")]
        private Toggle m_UnicodeToggle;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "StretchH")]
        private Text m_StretchHText;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "Smooth")]
        private Toggle m_SmoothToggle;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "AA")]
        private Toggle m_AAToggle;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "Padding")]
        private Text m_PaddingText;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "Spacing")]
        private Text m_SpacingText;

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
        /// <see cref="BMPFont"/> 信息数据
        /// </summary>
        public BMPFontInfo BMPFontInfo { get; set; }

        /* ctor */
        public UIBMPFontInfo(Dictionary<string, object> deserializeDictionary)
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

            deserializeDictionary.TryPushValue(nameof(m_FaceText), out m_FaceText);
            deserializeDictionary.TryPushValue(nameof(m_SizeText), out m_SizeText);
            deserializeDictionary.TryPushValue(nameof(m_BoldToggle), out m_BoldToggle);
            deserializeDictionary.TryPushValue(nameof(m_ItalicToggle), out m_ItalicToggle);
            deserializeDictionary.TryPushValue(nameof(m_CharSetText), out m_CharSetText);
            deserializeDictionary.TryPushValue(nameof(m_UnicodeToggle), out m_UnicodeToggle);
            deserializeDictionary.TryPushValue(nameof(m_StretchHText), out m_StretchHText);
            deserializeDictionary.TryPushValue(nameof(m_SmoothToggle), out m_SmoothToggle);
            deserializeDictionary.TryPushValue(nameof(m_AAToggle), out m_AAToggle);
            deserializeDictionary.TryPushValue(nameof(m_PaddingText), out m_PaddingText);
            deserializeDictionary.TryPushValue(nameof(m_SpacingText), out m_SpacingText);
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
            if (BMPFontInfo is null
                || BMPFontInfo.HaveError)
            {
                Fold();
                return;
            }

            Debug.Log($"{nameof(BMPFontInfo)} Refresh UI!");
        }

        [MarkingAction(IsRuntimeAction = true)]
        public void OnClickTitleButton()
        {
            Debug.Log($"{nameof(UIBMPFontCommon)}.{nameof(OnClickTitleButton)}");
            if (BMPFontInfo is null
                || BMPFontInfo.HaveError)
                Fold();
            else if (IsFold)
                Unfold();
            else
                Fold();
        }
    }
}