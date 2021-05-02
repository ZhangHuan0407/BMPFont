using ILRuntime.Runtime.Intepreter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Encoder
{
#if UNITY_EDITOR
    public abstract class UpdatableComponent : MonoBehaviour, ISerializationCallbackReceiver
#else
    public abstract class UpdatableComponent : MonoBehaviour
#endif
    {
        /* const */
        protected static readonly object[] EmptyParameters = new object[0];
        private static readonly Dictionary<string, object> DeserializeDictionary = new Dictionary<string, object>();

        /* field */
        [Tooltip("绑定的运行时动态类型，使用类型全名称")]
        public string ILTypeFullName;
        public string StructData = "{}";
        public UnityEngine.Object[] SerializableObject = new UnityEngine.Object[0];
        public string[] SerializableObjectName = new string[0];

        private ILTypeInstance m_Instance;
        public object InstanceObject => m_Instance;
        public ILTypeInstance Instance
        {
            get
            {
                if (BeforeInit
                    && !string.IsNullOrEmpty(ILTypeFullName))
                {
                    DeserializeDictionary.Clear();
                    for (int index = 0; index < SerializableObjectName.Length; index++)
                        DeserializeDictionary.Add(SerializableObjectName[index], SerializableObject[index]);
                    foreach (var pair in JsonConvert.DeserializeObject<Dictionary<string, string>>(StructData))
                        DeserializeDictionary.Add(pair.Key, pair.Value);
                    DeserializeDictionary.Add(nameof(UpdatableComponent), this);
                    SerializableObject = null;
                    SerializableObjectName = null;
                    StructData = null;
                    m_Instance = ILRuntimeService.CreateUpdatableComponentInstance(ILTypeFullName, DeserializeDictionary);
                    DeserializeDictionary.Clear();
                    CacheComponentMethods();
                }
                return m_Instance;
            }
            protected set => m_Instance = value;
        }

        /* inter */
        public bool BeforeInit => m_Instance is null;

        /* ctor */
        public virtual void Reset()
        {
            StructData = StructData ?? "{}";
            SerializableObject = SerializableObject ?? new UnityEngine.Object[0];
            SerializableObjectName = SerializableObjectName ?? new string[0];
        }
        protected virtual void Awake()
        {
            // 如果使用 AddComponent 添加组件，首次 Awake 时还没有为 ILTypeFullName 赋值
            // 如果已经可以初始化，这里会立即实例化，否则将延迟至下一个尝试点。
            // 如果依赖于Unity的Enable等激活事件，必须在添加组件前反激活节点
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                Debug.LogError($"Encoder.UpdatableComponent is not support ExecuteInEditMode.");

            // 实例化后初始化参数已经置为null，仅未初始化的情况下检测
            if (BeforeInit)
            {
                if (SerializableObject is null)
                    throw new ArgumentNullException(nameof(SerializableObject));
                if (SerializableObjectName is null)
                    throw new ArgumentNullException(nameof(SerializableObjectName));
            }
#endif
            _ = Instance;
        }

        /* func */
        public void InvokeAction(string actionName)
        {
            if (Instance.GetType().GetMethod(actionName) is MethodInfo methodInfo)
            {
#if UNITY_EDITOR
                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                if (parameterInfos.Length > 0
                    && !parameterInfos[0].HasDefaultValue)
                    Debug.LogError($"Try to Invoke actionName : {actionName}, but it's not a Action");
#endif
                methodInfo.Invoke(Instance, new object[0]);
            }
            else
                Debug.LogError("Not Found Method Exception.");
        }
        /// <summary>
        /// 实例化成功后，调用以缓存组件内可能存在的方法
        /// </summary>
        protected abstract void CacheComponentMethods();

#if UNITY_EDITOR
        /* ISerializationCallbackReceiver */
        public Action OnBeforeSerialize_Handle;
        /// <summary>
        /// 不知道为什么 Unity 每帧疯狂调用这个序列化准备接口
        /// 目前 UpdatableComponentEditor.OnBeforeSerialize_Callback 单帧会进行多次序列化准备
        /// </summary>
        public void OnBeforeSerialize() => OnBeforeSerialize_Handle?.Invoke();
        public void OnAfterDeserialize()
        {
        }
#endif
    }
}