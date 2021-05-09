using Encoder;
using System;
using System.Collections.Generic;
using UnityEngine;

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

        private UpdatableComponent m_UpdatableComponent;

        /* inter */
        protected GameObject gameObject;
        protected Transform transform;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释


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
        }

        /* func */
        [MarkingAction(IsRuntimeAction = true)]
        public void Update()
        {

        }

        [MarkingAction(IsRuntimeAction = true)]
        public void Fold()
        {

        }
        [MarkingAction(IsRuntimeAction = true)]
        public void Unfold()
        {

        }

        public void RefreshUI()
        {

        }
    }
}