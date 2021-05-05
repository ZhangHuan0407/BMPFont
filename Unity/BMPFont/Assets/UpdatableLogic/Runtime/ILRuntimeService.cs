using AssetBundleUpdate;
using Encoder.ILAdapter;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Generated;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Encoder
{
    public class ILRuntimeService : MonoBehaviour, IDisposable
    {
        /* const */
        private const string HotfixAssetBundleName = "updatablelogic/hotfix.assetbundle";
        private const string DLLAssetName = "HotFix.dll";
        private const string PDBAssetName = "HotFix.pdb";

        /* field */
        public static ILRuntimeService Instance { get; internal set; }
        public static Func<IMethod, object, object[], object> InvokeMethod;
        private AppDomain m_AppDomain;
        public AppDomain AppDomain
        {
            get
            {
                if (m_AppDomain is null)
                    m_AppDomain = new AppDomain();
                return m_AppDomain;
            }
            private set => m_AppDomain = value;
        }

        private MemoryStream m_DLLStream;
        private MemoryStream m_PDBStream;
        
        /// <summary>
        /// 已经抛弃或即将抛弃此实例
        /// </summary>
        public bool Abort { get; private set; }
        /// <summary>
        /// 标记此实例即将被抛弃，就仅仅是标记而已
        /// </summary>
        public void AbortInstance() => Abort = true;

        /* inter */

        /* ctor */
        private void Awake()
        {
            Instance = Instance ?? this;
            Abort = false;
        }
        private void OnEnable()
        {
            if (Abort)
                enabled = false;
        }
        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            Dispose();
        }

        /* func */
        /// <summary>
        /// 从资源服务器更新最新的代码资源包，并加载代码
        /// <para>执行比较耗时，请使用携程</para>
        /// </summary>
        public IEnumerable LoadAssemblyFromAssetBundle_Coroutine()
        {
            if (AssetBundlePool.Instance is null)
            {
                Debug.LogError($"{nameof(AssetBundlePool)} don't have instance.");
                yield break;
            }

            AssetBundlePool.LoadAssetBundle(HotfixAssetBundleName, true);
            while (!AssetBundlePool.AssetBundleIsReady(HotfixAssetBundleName))
                yield return null;

#if UNITY_EDITOR
            Debug.Log($"Load AssetBundle : {HotfixAssetBundleName}, start to LoadAssembly.");
#endif

            ResourceLoadState state = ResourceLoadState.WaitForAssetLoaded;
            m_DLLStream = new MemoryStream();
            m_PDBStream = new MemoryStream();

            AssetBundlePool.LoadAsset(HotfixAssetBundleName, DLLAssetName,
                (TextAsset textAsset) =>
                {
                    if (!textAsset)
                    {
                        state = ResourceLoadState.AssetNotFound;
                        throw new FileNotFoundException($"Not found file : {DLLAssetName} in AssetBundle : {HotfixAssetBundleName}");
                    }
                    byte[] buffer = textAsset.bytes;
                    m_DLLStream.Write(buffer, 0, buffer.Length);
                    state = ResourceLoadState.LoadSuccessful;
                });
            while (state == ResourceLoadState.WaitForAssetLoaded)
                yield return null;
            if (state != ResourceLoadState.LoadSuccessful)
                goto DisposeStream;

            // 对 dllStream 执行解密

            state = ResourceLoadState.WaitForAssetLoaded;
            AssetBundlePool.LoadAsset(HotfixAssetBundleName, PDBAssetName,
                (TextAsset textAsset) =>
                {
                    if (!textAsset)
                    {
                        state = ResourceLoadState.AssetNotFound;
                        throw new FileNotFoundException($"Not found file : {PDBAssetName} in AssetBundle : {HotfixAssetBundleName}");
                    }
                    byte[] buffer = textAsset.bytes;
                    m_PDBStream.Write(buffer, 0, buffer.Length);
                    state = ResourceLoadState.LoadSuccessful;
                });
            while (state == ResourceLoadState.WaitForAssetLoaded)
                yield return null;
            if (state != ResourceLoadState.LoadSuccessful)
                goto DisposeStream;

            // 对 pdbStream 执行解密

            m_DLLStream.Position = 0L;
            m_PDBStream.Position = 0L;
            AppDomain.LoadAssembly(m_DLLStream, m_PDBStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
#if UNITY_EDITOR
            AppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            InvokeMethod = AppDomain.Invoke;

        DisposeStream:
            switch (state)
            {
                case ResourceLoadState.LoadSuccessful:
                    break;
                case ResourceLoadState.AssetBundleNotFound:
                case ResourceLoadState.AssetNotFound:
                    throw new Exception(state.ToString());
                default:
                    throw new Exception(state.ToString());
            }
        }
        /// <summary>
        /// 初始化适配器并注册 CLR 绑定
        /// </summary>
        public static IEnumerable InitILRuntime_Coroutine(AppDomain domain)
        {
            //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
            domain.RegisterCrossBindingAdaptor(new Dictionary_2_Object_ObjectAdapter());
            domain.RegisterCrossBindingAdaptor(new ExceptionAdapter());
            domain.RegisterCrossBindingAdaptor(new HashSet_1_ObjectAdapter());
            domain.RegisterCrossBindingAdaptor(new IEnumerable_1_ILTypeInstanceAdapter());
            domain.RegisterCrossBindingAdaptor(new IEnumerable_1_ObjectAdapter());
            domain.RegisterCrossBindingAdaptor(new IEnumerator_1_ILTypeInstanceAdapter());
            domain.RegisterCrossBindingAdaptor(new IEnumerator_1_ObjectAdapter());
            domain.RegisterCrossBindingAdaptor(new LinkedList_1_ObjectAdapter());
            domain.RegisterCrossBindingAdaptor(new List_1_ObjectAdapter());
            domain.RegisterCrossBindingAdaptor(new Queue_1_ObjectAdapter());
            domain.RegisterCrossBindingAdaptor(new Stack_1_ObjectAdapter());
            yield return null;
            domain.DelegateManager.RegisterMethodDelegate<int>();
            domain.DelegateManager.RegisterMethodDelegate<float>();
            domain.DelegateManager.RegisterMethodDelegate<double>();
            domain.DelegateManager.RegisterMethodDelegate<string>();
            domain.DelegateManager.RegisterMethodDelegate<IEnumerable<object>>();
            domain.DelegateManager.RegisterMethodDelegate<object>();
            domain.DelegateManager.RegisterMethodDelegate<UnityEngine.Object>();
            domain.DelegateManager.RegisterMethodDelegate<GameObject>();
            domain.DelegateManager.RegisterMethodDelegate<TextAsset>();
            domain.DelegateManager.RegisterMethodDelegate<Sprite>();
            domain.DelegateManager.RegisterMethodDelegate<Texture>();
            domain.DelegateManager.RegisterMethodDelegate<Texture2D>();
            domain.DelegateManager.RegisterMethodDelegate<Material>();
            domain.DelegateManager.RegisterMethodDelegate<ScriptableObject>();
            domain.DelegateManager.RegisterMethodDelegate<Font>();
            domain.DelegateManager.RegisterMethodDelegate<UpdatableComponent>();
            domain.DelegateManager.RegisterMethodDelegate<Action>();
            domain.DelegateManager.RegisterMethodDelegate<Dictionary<string, object>>();
            domain.DelegateManager.RegisterMethodDelegate<UpdatableComponent, string>();
            domain.DelegateManager.RegisterMethodDelegate<object, object>();
            domain.DelegateManager.RegisterMethodDelegate<object, object, object>();
            domain.DelegateManager.RegisterMethodDelegate<object, object, object, object>();

            domain.DelegateManager.RegisterFunctionDelegate<string, object>();
            domain.DelegateManager.RegisterFunctionDelegate<object, object>();
            domain.DelegateManager.RegisterFunctionDelegate<object, object, object>();
            domain.DelegateManager.RegisterFunctionDelegate<object, object, object, object>();
            domain.DelegateManager.RegisterFunctionDelegate<object, object, object, object, object>();

            CLRBindings.Initialize(domain);
            yield return null;
        }
        /// <summary>
        /// 预热热更新类型，强制加载资源包
        /// </summary>
        public IEnumerable PrewarmGame_Coroutine()
        {
            if (Abort)
                yield break;
            
            IType gamePrewarmType = AppDomain.LoadedTypes["HotFix.GamePrewarm"] ?? throw new NotFoundBindingTypeException("Hotfix.GamePrewarm");
            ILTypeInstance gamePrewarm = ((ILType)gamePrewarmType).Instantiate() ?? throw new NullReferenceException(nameof(gamePrewarm));
            
            // 预热热更新类型
            IMethod method = gamePrewarmType.GetMethod("GetPrewarmTypeArray", 0) ?? throw new NotFoundBindingMethodException("GetPrewarmTypeArray");
            string[] prewarmTypeArray = AppDomain.Invoke(method, gamePrewarm) as string[];
            for (int index = 0; index < prewarmTypeArray.Length; index++)
                AppDomain.Prewarm(prewarmTypeArray[index]);
            yield return null;

            // 载入常驻资源包
            method = gamePrewarmType.GetMethod("GetPreloadAssetBundlesArray", 0) ?? throw new NotFoundBindingMethodException("GetPreloadAssetBundlesArray");
            string[] preloadAssetBundlesArray = AppDomain.Invoke(method, gamePrewarm) as string[];
            for (int index = 0; index < preloadAssetBundlesArray.Length; index++)
            {
                string assetBundleName = preloadAssetBundlesArray[index];
                AssetBundlePool.LoadAssetBundle(assetBundleName, true);
                while (!AssetBundlePool.AssetBundleIsReady(assetBundleName))
                    yield return null;
            }
        }
        public IEnumerable LoadCustomComponentInfo_Coroutine()
        {
            if (Abort)
                yield break;

            bool wait = true;
            AssetBundlePool.LoadAsset<TextAsset>(HotfixAssetBundleName, "CustomComponentInfo", LoadCallback);
            while (wait)
                yield return null;

            void LoadCallback(TextAsset customComponentInfo)
            {
                CustomComponentMethodBuffer.ClearAndRecreateCache(customComponentInfo.text);
                wait = false;
            }
        }
        public IEnumerable StartGame_Coroutine()
        {
            if (Abort)
                yield break;

            yield return null;
            IType gameStartType = AppDomain.LoadedTypes["HotFix.GameStart"] ?? throw new NotFoundBindingTypeException("Hotfix.GameStart");
            ILTypeInstance gameStart = ((ILType)gameStartType).Instantiate() ?? throw new NullReferenceException(nameof(gameStart));
            IMethod method = gameStartType.GetMethod("Main", 0) ?? throw new NotFoundBindingMethodException("Main");
            AppDomain.Invoke(method, gameStart);
        }

        public static ILTypeInstance CreateUpdatableComponentInstance(string typeFullName, Dictionary<string, object> deserializeDictionary) => Instance.CreateInstance(typeFullName, deserializeDictionary);
        internal ILTypeInstance CreateInstance(string typeFullName, params object[] args)
        {
            if (string.IsNullOrEmpty(typeFullName))
                throw new ArgumentException(nameof(typeFullName));
            return AppDomain.Instantiate(typeFullName, args) ?? throw new ArgumentNullException(typeFullName);
        }

        public static bool TryFindMethod(string typeFullName, string methodName, int paramCount, out IMethod method) => Instance.TryFindMethod_Internal(typeFullName, methodName, paramCount, out method);
        internal bool TryFindMethod_Internal(string typeFullName, string methodName, int paramCount, out IMethod method)
        {
            IType type = AppDomain.GetType(typeFullName);
            if (type != null)
            {
                method = type.GetMethod(methodName, paramCount);
                return true;
            }
            else
            {
                method = null;
                return false;
            }
        }

        public static Coroutine StartILCoroutine(IEnumerator<object> enumerator) => Instance.StartILCoroutine_Internal(enumerator);
        internal Coroutine StartILCoroutine_Internal(IEnumerator<object> enumerator)
        {
            if (enumerator is null)
                throw new ArgumentNullException(nameof(enumerator));

            if (Instance.Abort)
                return null;
            else
                return StartCoroutine(enumerator);
        }
        public static void StopILCoroutine(Coroutine coroutine) => Instance.StopILCoroutine_Internal(coroutine);
        internal void StopILCoroutine_Internal(Coroutine coroutine)
        {
            // 或许在抛弃 ILRuntimeService 且未释放时，尝试添加携程，并立即尝试停止此 null 携程
            if (coroutine is null)
                return;

            StopCoroutine(coroutine);
        }

        /* IDisposable */
        public void Dispose()
        {
            AbortInstance();
            if (Instance == this)
                Instance = null;
            InvokeMethod = null;
            m_AppDomain = null;
            m_DLLStream?.Dispose();
            m_PDBStream?.Dispose();
            StopAllCoroutines();
        }
    }
}