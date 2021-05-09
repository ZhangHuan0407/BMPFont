using Encoder;
using System;
using System.Collections.Generic;
using UnityEngine;

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
        }
        [MarkingAction(IsRuntimeAction = true)]
        public void Unfold()
        {
            IsFold = false;
            m_Animator.SetBool(nameof(IsFold), false);
        }

        public void RefreshUI()
        {
            if (BMPFontInfo is null
                || BMPFontInfo.HaveError)
            {
                Fold();
                return;
            }

            Debug.Log("Refresh!");
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