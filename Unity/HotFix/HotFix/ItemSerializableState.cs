namespace HotFix
{
    /// <summary>
    /// 一项内容的序列化状态
    /// </summary>
    public enum ItemSerializableState
    {
        /// <summary>
        /// 启用默认策略，
        /// <para>Field : Public 默认 <see cref="SerializeIt"/>，NonPublic 默认<see cref="ShowInInspectorOnly"/></para>
        /// <para>Property : Public 默认 <see cref="SerializeIt"/>, NonPublic 拒绝序列化</para>
        /// </summary>
        Default,
        /// <summary>
        /// 序列化此字段或属性，它的数据将被保存，并显示在面板中
        /// </summary>
        SerializeIt,
        /// <summary>
        /// 不序列化此字段或属性，但在面板上显示它
        /// </summary>
        ShowInInspectorOnly,
    }
}
