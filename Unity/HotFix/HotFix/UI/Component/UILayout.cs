using Encoder;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix.UI.Component
{
    public abstract class UILayout
    {
        /* field */
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        [InspectorInfo(Title = "当前参与布局的子节点")]
        public List<RectTransform> ChildsInLayout { get; private set; }

        protected GameObject gameObject;
        protected RectTransform transform;
        protected UpdatableComponent UpdatableComponent;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /* ctor */
        public UILayout(Dictionary<string, object> deserializeDictionary)
        {
            UpdatableComponent = deserializeDictionary[nameof(UpdatableComponent)] as UpdatableComponent ?? throw new ArgumentException($"{nameof(UpdatableComponent)} is null.");
            gameObject = UpdatableComponent.gameObject;
            transform = UpdatableComponent.transform as RectTransform;
        }

        /* func */
        /// <summary>
        /// 清空当前所有指向的布局子节点，并立即将当前 gameObject 的子节点按照顺序作为布局子节点
        /// </summary>
        public void OnlyAllChildsInLayout()
        {
            if (!transform)
                return;
            ChildsInLayout.Clear();
            foreach (RectTransform child in transform)
                ChildsInLayout.Add(child);
        }
    }
}