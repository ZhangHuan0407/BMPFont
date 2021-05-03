using Encoder;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI.Component
{
    [BindingUpdatableComponent(BindingUpdatableComponentAttribute.ContainerComponent)]
    public class DirectoryItem : IFileAndDirectoryItem
    {
        /* field */
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "动画控制")]
        private Animator m_Animator;

        [InspectorInfo(
            State = ItemSerializableState.ShowInInspectorOnly,
            Title = "索引序号",
            ToolTip = "在当前文件夹中的索引序号")]
        public int Index { get; set; }

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "文件修改日期")]
        private Text m_DateTimeText;
        public string DateTime
        {
            get
            {
                if (m_DateTimeText)
                    return m_DateTimeText.text;
                else
                    return null;
            }

            set
            {
                if (m_DateTimeText)
                    m_DateTimeText.text = value;
            }
        }

        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "文件名称")]
        private Text m_FileNameText;
        public string FileName
        {
            get
            {
                if (m_FileNameText)
                    return m_FileNameText.text;
                else
                    return null;
            }

            set
            {
                if (m_FileNameText)
                    m_FileNameText.text = value;
            }
        }

        private float m_LastClickTime = 0;
        /// <summary>
        /// 上一次的单击时间
        /// </summary>
        public float LastClickTime
        {
            get => m_LastClickTime;
            set => m_LastClickTime = value;
        }

        private UpdatableComponent m_UpdatableComponent;


        public FileAndDirectoryWindow Window { get; set; }

        /* inter */
        public GameObject gameObject { get; }
        public Transform transform { get; }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /* ctor */
        public DirectoryItem(Dictionary<string, object> deserializeDictionary)
        {
            m_UpdatableComponent = deserializeDictionary[nameof(UpdatableComponent)] as UpdatableComponent ?? throw new ArgumentException($"{nameof(UpdatableComponent)} is null.");
            gameObject = m_UpdatableComponent.gameObject;
            transform = m_UpdatableComponent.transform;

            if (deserializeDictionary.TryGetValue(nameof(m_Animator), out object animator_object)
                && animator_object is Animator animator_animator)
                m_Animator = animator_animator;
            else
                m_Animator = null;

            if (deserializeDictionary.TryGetValue(nameof(m_DateTimeText), out object dateTimeText_object)
                && dateTimeText_object is Text dateTime_text)
                m_DateTimeText = dateTime_text;
            else
                m_DateTimeText = null;

            if (deserializeDictionary.TryGetValue(nameof(m_FileNameText), out object fileNameText_object)
                && fileNameText_object is Text fileName_text)
                m_FileNameText = fileName_text;
            else
                m_FileNameText = null;
        }

        /* func */
        [InvokeAction(IsRuntimeAction = true)]
        public void OnClickButtonSelected()
        {
            Debug.Log($"{nameof(FileItem)}.{nameof(OnClickButtonSelected)}");
            Window.TouchThisItem(this);
        }
        /// <summary>
        /// 选中状态(仅动画展示)
        /// </summary>
        public void Selected()
        {
            m_Animator?.Play(nameof(Selected));
        }
        /// <summary>
        /// 取消选中状态(仅动画展示)
        /// </summary>
        public void NotSelected()
        {
            m_Animator?.Play(nameof(NotSelected));
        }
    }
}