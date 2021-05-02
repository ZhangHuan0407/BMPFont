namespace Encoder.Editor
{
    public interface IUpdatableComponentEditor
    {
        /* field */
        UpdatableComponent UpdatableComponent { get; set; }

        /* func */
        /// <summary>
        /// Unity 初始化面板时或重新定向组件类型时调用
        /// </summary>
        void OnEnable();
        /// <summary>
        /// Unity 面板退出时或重新定向组件类型前调用
        /// </summary>
        void OnDiable();
        /// <summary>
        /// 对序列化内容进行渲染
        /// </summary>
        void OnInspectorGUI();
    }
}