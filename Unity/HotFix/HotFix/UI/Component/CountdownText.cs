using Encoder;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI.Component
{
    /// <summary>
    /// 倒计时文本组件
    /// </summary>
    [CheckComponent(typeof(Text))]
    [BindingUpdatableComponent(BindingUpdatableComponentAttribute.CommonComponent)]
    public class CountdownText
    {
        /* field */
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        [InspectorInfo(
            State = ItemSerializableState.SerializeIt,
            Title = "使用空字符串代替倒计时结束",
            ToolTip = "小于 0 的状态下使用空字符串还是 00:00:00")]
        private bool m_UseEmptyIfLessZero = true;

        private int m_TimeStamp = 0;
        [InspectorInfo(
            State = ItemSerializableState.ShowInInspectorOnly,
            Title = "UTC 时间戳")]
        public int TimeStamp
        {
            get => m_TimeStamp;
            set
            {
                m_TimeStamp = value;
                SetTextValue();
            }
        }

        [InspectorInfo(Title = "字符串格式化")]
        public string StringFormat { get; set; } = string.Empty;

        private UpdatableComponent m_UpdatableComponent;

        /// <summary>
        /// 上一次界面倒计时刷新的时间(Unity Time)
        /// </summary>
        private float m_LastUpdateTime = 0;

        /* inter */
        private Text m_Text => gameObject.GetComponent<Text>();
        protected GameObject gameObject;
        protected Transform transform;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /* ctor */
        public CountdownText(Dictionary<string, object> deserializeDictionary)
        {
            m_UpdatableComponent = deserializeDictionary[nameof(UpdatableComponent)] as UpdatableComponent ?? throw new ArgumentException($"{nameof(UpdatableComponent)} is null.");
            gameObject = m_UpdatableComponent.gameObject;
            transform = m_UpdatableComponent.transform;

            if (deserializeDictionary.TryGetValue(nameof(m_UseEmptyIfLessZero), out object useEmptyString_object)
                && useEmptyString_object is string useEmptyString_string
                && bool.TryParse(useEmptyString_string, out bool useEmptyString_bool))
                m_UseEmptyIfLessZero = useEmptyString_bool;
            else
                m_UseEmptyIfLessZero = true;

            if (deserializeDictionary.TryGetValue(nameof(StringFormat), out object stringFormat_object)
                && stringFormat_object is string stringFormat_string)
                StringFormat = stringFormat_string;
            else
                StringFormat = string.Empty;
        }

        /* func */
        [MarkingAction(IsRuntimeAction = true)]
        public void Update()
        {
            if (Time.time - m_LastUpdateTime > 0.5f)
                SetTextValue();
        }

        [MarkingAction(IsEditorAction = true)]
        public void SetTextValue()
        {
            m_LastUpdateTime = Time.time;
            TimeSpan deltaTime = TimeSpan.FromSeconds(TimeStamp) - (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0));
            string content;
            if (deltaTime.TotalSeconds < 0)
            {
                if (m_UseEmptyIfLessZero)
                    content = string.Empty;
                else
                    content = TimeSpan.Zero.ToString(StringFormat);
            }
            else
                content = deltaTime.ToString(StringFormat);
            m_Text.text = content;
        }
    }
}