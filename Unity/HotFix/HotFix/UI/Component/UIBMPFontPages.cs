using Encoder;
using HotFix.EncoderExtend;
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
    public class UIBMPFontPages
    {
        /* field */
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
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
            Title = "内容父节点")]
        private Transform m_ContentTrans;

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "单页预制体")]
        private GameObject m_PagePrefab;

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
        /// BMP 页信息
        /// </summary>
        public List<BMPFontPage> Pages { get; set; }

        /* ctor */
        public UIBMPFontPages(Dictionary<string, object> deserializeDictionary)
        {
            deserializeDictionary.TryPushValue(nameof(UpdatableComponent), out m_UpdatableComponent);
            gameObject = m_UpdatableComponent.gameObject;
            transform = m_UpdatableComponent.transform;

            deserializeDictionary.TryPushValue(nameof(m_FoldImage), out m_FoldImage);
            deserializeDictionary.TryPushValue(nameof(m_UnfoldImage), out m_UnfoldImage);
            deserializeDictionary.TryPushValue(nameof(m_ContentTrans), out m_ContentTrans);
            deserializeDictionary.TryPushValue(nameof(m_PagePrefab), out m_PagePrefab);
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
            m_FoldImage.enabled = true;
            m_UnfoldImage.enabled = false;
        }
        [MarkingAction(IsRuntimeAction = true)]
        public void Unfold()
        {
            IsFold = false;
            m_FoldImage.enabled = false;
            m_UnfoldImage.enabled = true;
        }

        [MarkingAction(IsRuntimeAction = true)]
        public void OnClickTitleButton()
        {
            Debug.Log($"{nameof(UIBMPFontCommon)}.{nameof(OnClickTitleButton)}");
            if (Pages is null)
                Fold();
            else if (IsFold)
                Unfold();
            else
                Fold();
        }
    }
}