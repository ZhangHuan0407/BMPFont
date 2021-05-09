using AssetBundleUpdate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tween;
using UnityEngine;
using UnityEngine.Events;

namespace Encoder
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AssetBundlePool), typeof(ILRuntimeService), typeof(TweenService))]
    public class UpdatePipeLine : MonoBehaviour
    {
        internal class OneTask
        {
            /* field */
            internal readonly string Label;
            internal readonly IEnumerator Task;

            /* ctor */
            internal OneTask(string label, Func<IEnumerable> task)
            {
                if (string.IsNullOrWhiteSpace(label))
                    throw new ArgumentException($"“{nameof(label)}”不能为 null 或空白。", nameof(label));
                if (task is null)
                    throw new ArgumentNullException(nameof(task));

                Label = label;
                Task = task().GetEnumerator();
            }
        }

        /* field */
        public static UpdatePipeLine Instance;

        public bool DeleteLatestABUpdator;
        [SerializeField]
        private AssetBundlePool m_AssetBundlePool;
        [SerializeField]
        private ILRuntimeService m_ILRuntimeService;
        [SerializeField]
        private TweenService m_TweenService;
        /// <summary>
        /// 任务执行队列
        /// </summary>
        private Queue<OneTask> UpdatorTaskQueue;
        /// <summary>
        /// 强制更新包即将开始下载
        /// </summary>
        public UnityEvent UpdatorStartUpdate_Handle;
        /// <summary>
        /// 强制更新包已完成下载
        /// </summary>
        public UnityEvent UpdatorFinishUpdate_Handle;

        public string LatestTaskLable { get; private set; }
        public Exception UpdatorTaskException { get; private set; }

        /* ctor */
        private void Reset()
        {
            DeleteLatestABUpdator = true;
            m_AssetBundlePool = GetComponent<AssetBundlePool>();
            m_AssetBundlePool.enabled = false;
            m_ILRuntimeService = GetComponent<ILRuntimeService>();
            m_ILRuntimeService.enabled = false;
            m_TweenService = GetComponent<TweenService>();
            m_TweenService.enabled = false;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            // 更新流程
            UpdatorTaskQueue = new Queue<OneTask>()
            {
                new OneTask(nameof(AbortIfRequested),                                       AbortIfRequested),
                new OneTask(nameof(SetInstanceAndEnableComponent),                          SetInstanceAndEnableComponent),
                new OneTask(nameof(AssetBundlePool.GetLatestUpdateSummery_Coroutine),       m_AssetBundlePool.GetLatestUpdateSummery_Coroutine),
                new OneTask(nameof(InitParameterInMainThread),                              InitParameterInMainThread),
                new OneTask(nameof(AssetBundlePool.ClearAllFilesInTempDirectory_Coroutine), m_AssetBundlePool.ClearAllFilesInTempDirectory_Coroutine),
                new OneTask(nameof(AssetBundlePool.ClearOldAssetBundlesInDirectory_Coroutine), m_AssetBundlePool.ClearOldAssetBundlesInDirectory_Coroutine),
                new OneTask(nameof(UpdatorStartUpdate),                                     UpdatorStartUpdate),
                new OneTask(nameof(AssetBundlePool.UpdateForceUpdateAssetBundles_Coroutine), m_AssetBundlePool.UpdateForceUpdateAssetBundles_Coroutine),
                new OneTask(nameof(AssetBundlePool.StartUpdateReserveAssetBundles_Coroutine), m_AssetBundlePool.StartUpdateReserveAssetBundles_Coroutine),
                new OneTask(nameof(UpdatorFinishUpdate),                                    UpdatorFinishUpdate),
                new OneTask(nameof(ILRuntimeService.LoadAssemblyFromAssetBundle_Coroutine), m_ILRuntimeService.LoadAssemblyFromAssetBundle_Coroutine),
                new OneTask(nameof(ILRuntimeService.InitILRuntime_Coroutine),               m_ILRuntimeService.InitILRuntime_Coroutine),
                new OneTask(nameof(ILRuntimeService.PrewarmGame_Coroutine),                 m_ILRuntimeService.PrewarmGame_Coroutine),
                new OneTask(nameof(ILRuntimeService.LoadUpdatableComponentInfo_Coroutine),  m_ILRuntimeService.LoadUpdatableComponentInfo_Coroutine),
                new OneTask(nameof(ILRuntimeService.StartGame_Coroutine),                   m_ILRuntimeService.StartGame_Coroutine),
            };

            // 日志输出
            Application.logMessageReceived += Log.ApplicationLogMessageReceived;
        }

        private IEnumerator Start()
        {
            while (UpdatorTaskQueue.Count > 0 && UpdatorTaskException is null)
            {
                yield return StartCoroutine(CatchTaskException(UpdatorTaskQueue.Dequeue()));
            }
            if (UpdatorTaskException != null)
            {
                Debug.LogError(UpdatorTaskException);
            }
        }
        private void OnDestroy()
        {
            // 防止复数次订阅
            Application.logMessageReceived -= Log.ApplicationLogMessageReceived;
        }

        /* func */
        private IEnumerator CatchTaskException(OneTask oneTask)
        {
#if UNITY_EDITOR
            Debug.Log(oneTask.Label);
#endif
            bool circleContinue = true;
            LatestTaskLable = oneTask.Label;
            while (circleContinue)
            {
                try
                {
                    circleContinue = oneTask.Task.MoveNext();
                }
                catch (Exception e)
                {
                    circleContinue = false;
                    UpdatorTaskException = e;
                }
                yield return oneTask.Task.Current;
            }
        }
        internal IEnumerable InitParameterInMainThread()
        {
            _ = AssetBundleInfo.AssetBundleDirectory;
            _ = AssetBundleInfo.AssetBundleTempDirectory;
            _ = AssetBundleUpdateTask.HttpClient.Value;

            if (TweenService.Instance == null)
                Debug.LogError($"{nameof(TweenService)}.{nameof(TweenService.Instance)} is null");
            if (ILRuntimeService.Instance == null)
                Debug.LogError($"{nameof(ILRuntimeService)}.{nameof(ILRuntimeService.Instance)} is null");
            yield break;
        }
        internal IEnumerable AbortIfRequested()
        {
            if (!DeleteLatestABUpdator || Instance == null)
                yield break;

            Instance.StopAllCoroutines();

            if (TweenService.Instance != null)
            {
                TweenService.Instance.AbortInstance();
                TweenService.Instance.Dispose();
                yield return null;
            }

            if (ILRuntimeService.Instance != null)
            {
                ILRuntimeService.Instance.AbortInstance();
                ILRuntimeService.Instance.Dispose();
                yield return null;
            }

            UpdatableComponentsBuffer.ComponentMethodsCache = null;

            if (AssetBundlePool.Instance != null)
            {
                AssetBundlePool.Instance.AbortInstance();
                AssetBundlePool.Instance.AbortUpdateAssetBundleAndLoadedTasks(new string[0]);
                while (AssetBundlePool.Instance.UpdateTasksCount > 0)
                    yield return null;
                AssetBundlePool.Instance.MarkSweepAssetBundles(new string[0], true);
                AssetBundlePool.Instance.Dispose();
                yield return null;
            }
            Destroy(Instance.gameObject);
            yield break;
        }
        internal IEnumerable SetInstanceAndEnableComponent()
        {
            if (Instance == null)
                Instance = this;
            else
                Debug.LogError($"{nameof(UpdatePipeLine)}.{nameof(Instance)} is not null.");
            m_TweenService.enabled = true;
            m_AssetBundlePool.enabled = true;
            m_ILRuntimeService.enabled = true;
            yield return null;
        }
        internal IEnumerable UpdatorStartUpdate()
        {
            UpdatorStartUpdate_Handle?.Invoke();
            yield break;
        }
        internal IEnumerable UpdatorFinishUpdate()
        {
            UpdatorFinishUpdate_Handle?.Invoke();
            yield break;
        }
    }
}