using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetBundleUpdate
{
    [DisallowMultipleComponent]
    public class AssetBundlePool : MonoBehaviour, IDisposable
    {
        /* field */
#if UNITY_EDITOR
#pragma warning disable CS0649
        /// <summary>
        /// 使用 Editor 资源加载系统，而不是热更新资源
        /// 运行期间修改此值，执行结果未知
        /// </summary>
        private static bool UseEditorAssetSystem { get; set; }
#pragma warning restore CS0649
#endif
        public static AssetBundlePool Instance { get; internal set; }

        /// <summary>
        /// 后台下载线程交互锁
        /// </summary>
        public object LockFactor = new object();
        private List<IEnumerator<bool>> m_AssetLoadedCoroutine;
        protected List<IEnumerator<bool>> AssetLoadedCoroutine
        {
            get => m_AssetLoadedCoroutine = m_AssetLoadedCoroutine ?? new List<IEnumerator<bool>>();
            set => m_AssetLoadedCoroutine = value;
        }

        /// <summary>
        /// 更新摘要
        /// </summary>
        internal UpdateSummery UpdateSummery;
        [Tooltip("找不到上次的Uri 前缀地址时，使用的默认 Uri 前缀地址")]
        [SerializeField]
        private string UriBase;

        private Dictionary<string, AssetBundle> m_LoadedAssetBundles;
        /// <summary>
        /// 内存中已经加载的资源包
        /// </summary>
        protected Dictionary<string, AssetBundle> LoadedAssetBundles
        {
            get => m_LoadedAssetBundles = m_LoadedAssetBundles ?? new Dictionary<string, AssetBundle>();
            set => m_LoadedAssetBundles = value;
        }
        private Dictionary<string, AssetBundleUpdateTask> m_UpdateAssetBundles;
        /// <summary>
        /// 正在进行更新的资源包
        /// </summary>
        protected Dictionary<string, AssetBundleUpdateTask> UpdateAssetBundles
        {
            get => m_UpdateAssetBundles = m_UpdateAssetBundles ?? new Dictionary<string, AssetBundleUpdateTask>();
            set => m_UpdateAssetBundles = value;
        }
        private Dictionary<string, AssetBundleInfo> m_AllAssetBundleInfos;
        /// <summary>
        /// 所有资源包的信息
        /// </summary>
        protected Dictionary<string, AssetBundleInfo> AllAssetBundleInfos
        {
            get => m_AllAssetBundleInfos = m_AllAssetBundleInfos ?? new Dictionary<string, AssetBundleInfo>();
            set => m_AllAssetBundleInfos = value;
        }

        /// <summary>
        /// 已经抛弃或即将抛弃此实例
        /// </summary>
        public bool Abort { get; private set; }
        /// <summary>
        /// 标记此实例即将被抛弃，就仅仅是标记而已
        /// </summary>
        public void AbortInstance() => Abort = true;

        /* inter */
        /// <summary>
        /// 返回当前资源包更新任务总数量
        /// <para>已生成未开启，正在执行，即将完成，即将抛弃的任务总数</para>
        /// </summary>
        public int UpdateTasksCount => UpdateAssetBundles.Count;

        /* ctor */
        private void Awake()
        {
#if UNITY_EDITOR
            Debug.Log($"{nameof(AssetBundlePool)}.{nameof(UseEditorAssetSystem)} => {UseEditorAssetSystem}");
#endif
            if (string.IsNullOrWhiteSpace(UriBase))
                Debug.LogError($"{nameof(UriBase)} is null or white space.");
            Instance = Instance ?? this;

            // 注意：新玩家首帧之前访问文件夹，文件夹可能不存在
            Directory.CreateDirectory(AssetBundleInfo.AssetBundleDirectory);
            Directory.CreateDirectory(AssetBundleInfo.AssetBundleTempDirectory);

            Abort = false;
        }
        private void OnEnable()
        {
            if (Abort)
                enabled = false;
        }
        public void SetAssetBundleInfos(UpdateSummery updateSummery)
        {
            if (updateSummery is null)
                throw new ArgumentNullException(nameof(updateSummery));
            UpdateSummery = updateSummery;

            if (AllAssetBundleInfos.Count > 0)
                throw new Exception($"{nameof(AllAssetBundleInfos)}.Count > 0, have set it value before invoke {nameof(SetAssetBundleInfos)}");
            string UriBase = UpdateSummery.GetUriBase();
            AssetBundleUpdateTask.HttpClient.Value.BaseAddress = new Uri(UriBase);
            foreach (AssetBundleInfo assetBundleInfo in updateSummery.RecordInfos)
            {
                AllAssetBundleInfos.Add(assetBundleInfo.Name, assetBundleInfo);
            }
        }
        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;

            Dispose();
        }

        /* func */
        private void Update()
        {
            lock (LockFactor)
            {
                if (AssetLoadedCoroutine is null
                    || AssetLoadedCoroutine.Count == 0)
                    return;

                List<IEnumerator<bool>> coroutine = AssetLoadedCoroutine;
                AssetLoadedCoroutine = new List<IEnumerator<bool>>();
                foreach (IEnumerator<bool> enumerator in coroutine)
                {
                    try
                    {
                        if (enumerator.MoveNext() && !enumerator.Current)
                            AssetLoadedCoroutine.Add(enumerator);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }

        /// <summary>
        /// 目标名称的资源包及其所有依赖资源包已经更新到当前资源版本，且已加载到内存
        /// </summary>
        /// <param name="assetBundleName">目标名称的资源包的名称</param>
        /// <returns>是否加载完成</returns>
        public static bool AssetBundleIsReady(string assetBundleName) =>
            Instance.AllAssetBundleInfos[assetBundleName].HaveLoaded;
        internal bool AssetBundleIsReady_Internal(AssetBundleInfo assetBundleInfo)
        {
            if (assetBundleInfo is null)
                throw new ArgumentNullException(nameof(assetBundleInfo));

            foreach (string dependencyAssetBundleName in assetBundleInfo.AllDependencies)
                if (!LoadedAssetBundles.ContainsKey(dependencyAssetBundleName))
                    return false;
            return true;
        }
        /// <summary>
        /// 目标名称的资源包已经更新到当前资源版本，且已加载到内存
        /// <para>返回 true 不代表依赖资源包已加载</para>
        /// </summary>
        /// <param name="assetBundleName">目标名称的资源包的名称</param>
        /// <returns>是否加载完成</returns>
        public static bool AssetBundleIsLoaded(string assetBundleName) =>
            Instance.LoadedAssetBundles.ContainsKey(assetBundleName);

        /// <summary>
        /// 创建目标资源包的更新任务
        /// </summary>
        /// <param name="assetBundleName">目标资源包</param>
        /// <param name="directlyDownload">直接开始下载，跳过本地缓存检测</param>
        public static void UpdateAssetBundle(string assetBundleName, bool directlyDownload = false) =>
            Instance.UpdateAssetBundle_Internal(assetBundleName, directlyDownload);
        internal void UpdateAssetBundle_Internal(string assetBundleName, bool directlyDownload)
        {
            if (Abort)
                return;
            if (!AllAssetBundleInfos.TryGetValue(assetBundleName, out AssetBundleInfo assetBundleInfo))
                throw new Exception($"Not found assetBundleName : {assetBundleName}");

#if UNITY_EDITOR
            if (UseEditorAssetSystem)
            {
                assetBundleInfo.HaveUpdate = true;
                return;
            }
#endif

            // 正在更新或已经更新过了
            if (UpdateAssetBundles.ContainsKey(assetBundleName)
                || assetBundleInfo.HaveUpdate)
                return;

            AssetBundleUpdateTask task = new AssetBundleUpdateTask(assetBundleInfo)
            {
                DownloadFromServer = directlyDownload,
                CheckAsset = true,
            };
            task.AfterUpdate_Handle += () =>
            {
                lock (LockFactor)
                {
                    AssetLoadedCoroutine.Add(UpdateSymbolAfterUpdate());
                }
            };

            UpdateAssetBundles.Add(assetBundleName, task);
            task.RunInTask();

            IEnumerator<bool> UpdateSymbolAfterUpdate()
            {
                if (task.Abort)
                {
                    if (UpdateAssetBundles.ContainsKey(assetBundleInfo.Name))
                        UpdateAssetBundles.Remove(assetBundleInfo.Name);
                }
                else
                {
                    UpdateAssetBundles.Remove(assetBundleInfo.Name);
                    assetBundleInfo.HaveUpdate = true;
                }
                yield break;
            }
        }

        public static void LoadAssetBundle(string assetBundleName, bool directReference) => Instance.LoadAssetBundle_Internal(assetBundleName, directReference);
        internal void LoadAssetBundle_Internal(string assetBundleName, bool directReference)
        {
            if (Abort)
                return;
            if (!AllAssetBundleInfos.TryGetValue(assetBundleName, out AssetBundleInfo assetBundleInfo))
                throw new Exception($"Not found assetBundleName : {assetBundleName}");

#if UNITY_EDITOR
            if (UseEditorAssetSystem)
            {
                assetBundleInfo.HaveUpdate = true;
                LoadedAssetBundles.Add(assetBundleName, null);
                assetBundleInfo.HaveLoaded = true;
                return;
            }
#endif
            // 一个 LoadedAssetBundles 包含的资源包，有可能是 LoadAssetBundle(name, true)加载的，
            // 也可能是 LoadAssetBundle(name, true)加载的。面对对二种情况当前资源包的依赖资源包可能未加载。
            SetReferenceCount(assetBundleInfo, 1, directReference);
            if (directReference)
            {
                foreach (string dependencyAssetBundleName in assetBundleInfo.AllDependencies)
                    LoadAssetBundle_Internal(dependencyAssetBundleName, false);
            }

            if (LoadedAssetBundles.ContainsKey(assetBundleName))
                return;

            AssetBundleUpdateTask task = new AssetBundleUpdateTask(assetBundleInfo)
            {
                CheckAsset = !assetBundleInfo.HaveUpdate,
            };
            task.AfterUpdate_Handle += () =>
            {
                lock (LockFactor)
                {
                    AssetLoadedCoroutine.Add(LoadAssetBundleAfterUpdate());
                }
            };
            UpdateAssetBundles.Add(assetBundleName, task);
            task.RunInTask();

            IEnumerator<bool> LoadAssetBundleAfterUpdate()
            {
                if (task.Abort)
                {
                    if (UpdateAssetBundles.ContainsKey(assetBundleInfo.Name))
                        UpdateAssetBundles.Remove(assetBundleInfo.Name);
                    yield break;
                }
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(assetBundleInfo.FilePath);
                while (!request.isDone)
                    yield return false;
                assetBundleInfo.HaveUpdate = true;
                LoadedAssetBundles.Add(assetBundleInfo.Name, request.assetBundle);
                foreach (string dependencyAssetBundleName in assetBundleInfo.AllDependencies)
                {
                    while (!LoadedAssetBundles.ContainsKey(dependencyAssetBundleName))
                    {
                        if (task.Abort)
                        {
                            if (UpdateAssetBundles.ContainsKey(assetBundleInfo.Name))
                                UpdateAssetBundles.Remove(assetBundleInfo.Name);
                            yield break;
                        }
                        yield return false;
                    }
                }
                while (task.Abort
                    || !AssetBundleIsReady_Internal(assetBundleInfo))
                    yield return false;
                if (UpdateAssetBundles.ContainsKey(assetBundleInfo.Name))
                    UpdateAssetBundles.Remove(assetBundleInfo.Name);
                assetBundleInfo.HaveLoaded = true;
            }
        }

        public static void LoadAsset<T>(string assetBundleName, string name, Action<T> callback) where T : UnityEngine.Object => 
            Instance.LoadAsset_Internal<T>(assetBundleName, name, (obj) => callback(obj as T));
        public static void LoadAsset<T>(string assetBundleName, string name, Action<object> callback) where T : UnityEngine.Object =>
            Instance.LoadAsset_Internal<T>(assetBundleName, name, callback);
        internal void LoadAsset_Internal<T>(string assetBundleName, string name, Action<object> callback) where T : UnityEngine.Object
        {
            if (Abort)
                return;
            if (!AllAssetBundleInfos.TryGetValue(assetBundleName, out AssetBundleInfo assetBundleInfo))
                throw new Exception($"Not found assetBundleName : {assetBundleName}");

            lock (LockFactor)
            {
#if UNITY_EDITOR
                if (UseEditorAssetSystem)
                {
                    AssetLoadedCoroutine.Add(LoadAssetInEditor());
                    return;
                }
#endif
                AssetLoadedCoroutine.Add(LoadAssetAfterAssetBundleLoaded());
            }


#if UNITY_EDITOR
            IEnumerator<bool> LoadAssetInEditor()
            {
                string[] allAssetPath = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, name);
                if (allAssetPath.Length == 0)
                {
                    Debug.LogError($"Not found asset at path. Name : {name} in AssetBundle : {assetBundleName}.");
                    yield break;
                }
                else if (allAssetPath.Length != 1)
                {
                    Debug.LogError($"Name : {name} in AssetBundle : {assetBundleName} is more than one.");
                    yield break;
                }
                yield return false;
                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(allAssetPath[0]);
                callback(asset);
            }
#endif
            IEnumerator<bool> LoadAssetAfterAssetBundleLoaded()
            {
                if (!assetBundleInfo.HaveLoaded)
                {
                    Debug.LogWarning($"Asset Bundle : {assetBundleInfo.Name} become resident Asset Bundle.");
                    LoadAssetBundle_Internal(assetBundleName, true);
                    while (!assetBundleInfo.HaveLoaded)
                        yield return false;
                }

                if (!LoadedAssetBundles.TryGetValue(assetBundleName, out AssetBundle assetBundle))
                    throw new Exception($"{assetBundleName} is loaded, but it's not exists in {nameof(LoadedAssetBundles)}.If AssetBundlePool has dispose, {nameof(AssetLoadedCoroutine)} need clear immediately.");

                AssetBundleRequest request = assetBundle.LoadAssetAsync<T>(name);
                while (!request.isDone)
                    yield return false;
                callback(request.asset);
            }
        }

        /// <summary>
        /// 尽快抛弃白名单以外的任务
        /// </summary>
        /// <param name="whiteList">资源包白名单</param>
        public void AbortUpdateAssetBundleAndLoadedTasks(IEnumerable<string> whiteList)
        {
            if (whiteList is null)
                throw new ArgumentNullException(nameof(whiteList));

            HashSet<string> whiteListHash = new HashSet<string>(whiteList);
            foreach (KeyValuePair<string, AssetBundleUpdateTask> updateTask in UpdateAssetBundles)
            {
                if (whiteListHash.Contains(updateTask.Key))
                    continue;

                updateTask.Value.Abort = true;
            }
            AssetLoadedCoroutine.Clear();
        }
        /// <summary>
        /// 清除所有资源包的引用标记，并基于提供的根引用资源包重新标记引用计数
        /// <para>调用重新标记资源包，不会打断正在执行的加载任务和加载携程，但会对引用计数 > 0 且已加载的资源包做卸载处理</para>
        /// </summary>
        /// <param name="rootAssetBundles">根引用资源包，它们的资源包及其引用的资源包都会处于引用计数 > 0 状态</param>
        /// <param name="unloadAllLoadedObject">对于引用计数将为 0 的资源包，是否执行已加载内容的卸载</param>
        public void MarkSweepAssetBundles(IEnumerable<string> rootAssetBundles, bool unloadAllLoadedObject)
        {
            if (rootAssetBundles is null)
                throw new ArgumentNullException(nameof(rootAssetBundles));

            foreach (AssetBundleInfo assetBundleInfo in AllAssetBundleInfos.Values)
            {
                assetBundleInfo.AllReferenceCount = 0;
                assetBundleInfo.DirectReferenceCount = 0;
            }
            foreach (string assetBundleName in rootAssetBundles)
            {
                AssetBundleInfo assetBundleInfo = AllAssetBundleInfos[assetBundleName];
                SetReferenceCount(assetBundleInfo, 1, true);
            }

            foreach (AssetBundleInfo assetBundleInfo in AllAssetBundleInfos.Values)
            {
                if (assetBundleInfo.AllReferenceCount < 1
                    && LoadedAssetBundles.TryGetValue(assetBundleInfo.Name, out AssetBundle assetbundle))
                {
                    LoadedAssetBundles.Remove(assetBundleInfo.Name);
                    assetbundle.Unload(unloadAllLoadedObject);
                    assetBundleInfo.HaveLoaded = false;
                }
            }
        }

        /// <summary>
        /// 变更资源包引用计数
        /// </summary>
        /// <param name="assetBundleName">资源包名称</param>
        /// <param name="delta">引用计数变更值</param>
        /// <param name="singleAssetBundle">仅变更单资源包计数，而不变更其依赖资源包引用计数</param>
        /// <param name="unloadAllLoadedObject">如果触发清理，是否强制清除资源</param>
        public static void SetReferenceCount(string assetBundleName, int delta, bool singleAssetBundle, bool unloadAllLoadedObject) =>
            Instance.SetReferenceCount_Internal(assetBundleName, delta, singleAssetBundle, unloadAllLoadedObject);
        internal void SetReferenceCount_Internal(string assetBundleName, int delta, bool singleAssetBundle, bool unloadAllLoadedObject)
        {
            if (Abort)
                return;
            if (!AllAssetBundleInfos.TryGetValue(assetBundleName, out AssetBundleInfo assetBundleInfo))
                throw new Exception($"Not found assetBundleName : {assetBundleName}");

            SetReferenceCount(assetBundleInfo, delta, singleAssetBundle, unloadAllLoadedObject);
        }
        public void SetReferenceCount(
            AssetBundleInfo assetBundleInfo, int delta,
            bool singleAssetBundle, bool unloadAllLoadedObject = false)
        {
            assetBundleInfo.DirectReferenceCount += delta;
            assetBundleInfo.AllReferenceCount += delta;
            if (assetBundleInfo.AllReferenceCount < 1)
                UnloadOrAbortAssetBundle(assetBundleInfo);
            if (singleAssetBundle)
                return;
            foreach (string referenceAssetBundleName in assetBundleInfo.AllDependencies)
            {
                AssetBundleInfo dependencyAssetBundleInfo = AllAssetBundleInfos[referenceAssetBundleName];
                dependencyAssetBundleInfo.AllReferenceCount += delta;
                if (dependencyAssetBundleInfo.AllReferenceCount < 1)
                    UnloadOrAbortAssetBundle(dependencyAssetBundleInfo);
            }

            void UnloadOrAbortAssetBundle(AssetBundleInfo targetAssetBundleInfo)
            {
                if (LoadedAssetBundles.TryGetValue(targetAssetBundleInfo.Name, out AssetBundle assetbundle))
                {
                    assetbundle.Unload(unloadAllLoadedObject);
                    LoadedAssetBundles.Remove(targetAssetBundleInfo.Name);
                }
                // 还未加载完成
                else if (UpdateAssetBundles.TryGetValue(targetAssetBundleInfo.Name, out AssetBundleUpdateTask task))
                    task.Abort = true;
                targetAssetBundleInfo.HaveLoaded = false;
            }
        }

        /// <summary>
        /// 执行一个清除下载缓存文件夹下所有文件的异步任务
        /// <para>读写文件同时调用，会引发异常</para>
        /// <para>初次发现更新任务执行一次，清理非强制下载的没完成的临时断点文件。</para>
        /// </summary>
        /// <param name="force">强制清除缓存</param>
        /// <returns>后台执行的任务</returns>
        public Task ClearAllFilesInTempDirectory(bool force)
        {
            // Get last record version in main thread.
            int lastRecordVersion = PlayerPrefs.GetInt(UpdateSummery.UpdateSummeryVersionKey);
            return Task.Run(() => 
            {
                try
                {
                    if (!Directory.Exists(AssetBundleInfo.AssetBundleTempDirectory))
                    {
                        Directory.CreateDirectory(AssetBundleInfo.AssetBundleTempDirectory);
                        return;
                    }
                    // 更新目标版本没有变更，不需要清除缓存
                    if (!force
                        && lastRecordVersion == UpdateSummery.Version)
                        return;

                    Directory.Delete(AssetBundleInfo.AssetBundleTempDirectory, true);
                    Directory.CreateDirectory(AssetBundleInfo.AssetBundleTempDirectory);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            });
        }
        /// <summary>
        /// 清除下载缓存文件夹下所有非最新版本的资源包
        /// <para>读写文件同时调用，会引发异常</para>
        /// <para>初次发现更新任务执行一次，清理非强制下载的没完成的临时断点文件。</para>
        /// </summary>
        public IEnumerable ClearAllFilesInTempDirectory_Coroutine()
        {
            if (Abort)
                yield break;
            Task task = ClearAllFilesInTempDirectory(false);
            while (!task.IsCompleted)
                yield return null;
        }
        /// <summary>
        /// 执行一个清除下载缓存文件夹下所有文件的异步任务
        /// <para>读写文件同时调用，会引发异常</para>
        /// <para>确认更新完成后，下次启动游戏或资源包系统重新载入时，删除已经过期的资源包</para>
        /// </summary>
        /// <returns>后台执行的任务</returns>
        public Task ClearOldAssetBundlesInDirectory()
        {
            return Task.Run(() =>
            {
                try
                {
                    if (Abort)
                        return;
                    if (UpdateSummery == null)
                        throw new NullReferenceException(nameof(UpdateSummery));

                    if (!Directory.Exists(AssetBundleInfo.AssetBundleDirectory))
                    {
                        Directory.CreateDirectory(AssetBundleInfo.AssetBundleDirectory);
                        return;
                    }

                    HashSet<string> keepAssetBundlePath = new HashSet<string>();
                    foreach (AssetBundleInfo assetBundleInfo in UpdateSummery.RecordInfos)
                        keepAssetBundlePath.Add(assetBundleInfo.FilePath);
                    DeleteOldAssetBundle(AssetBundleInfo.AssetBundleDirectory);

                    void DeleteOldAssetBundle(string directoryPath)
                    {
                        foreach (string filePath in Directory.GetFiles(directoryPath, "*.assetbundle"))
                        {
                            if (keepAssetBundlePath.Contains(filePath))
                                continue;
                            else
                                File.Delete(filePath);
                        }
                        foreach (string childDirectoryPath in Directory.GetDirectories(directoryPath))
                            DeleteOldAssetBundle(childDirectoryPath);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            });
        }
        /// <summary>
        /// 清除下载缓存文件夹下所有文件
        /// <para>读写文件同时调用，会引发异常</para>
        /// <para>确认更新完成后，下次启动游戏或资源包系统重新载入时，删除已经过期的资源包</para>
        /// </summary>
        public IEnumerable ClearOldAssetBundlesInDirectory_Coroutine()
        {
            if (Abort)
                yield break;
            if (UpdateSummery == null)
                throw new NullReferenceException(nameof(UpdateSummery));

            Task task = ClearOldAssetBundlesInDirectory();
            while (!task.IsCompleted)
                yield return null;
        }

        /// <summary>
        /// 从远程服务器获取最新的更新摘要
        /// </summary>
        public IEnumerable GetLatestUpdateSummery_Coroutine()
        {
            if (Abort)
                yield break;
            if (UpdateSummery != null)
                throw new Exception($"Have set {nameof(UpdateSummery)} value, before invoke {nameof(GetLatestUpdateSummery_Coroutine)}.");

#if UNITY_EDITOR
            if (UseEditorAssetSystem)
            {
                string[] assetBundleNames = UnityEditor.AssetDatabase.GetAllAssetBundleNames();
                AssetBundleInfo[] assetBundleInfos = new AssetBundleInfo[assetBundleNames.Length];
                for (int index = 0; index < assetBundleNames.Length; index++)
                {
                    assetBundleInfos[index] = new AssetBundleInfo()
                    {
                        Name = assetBundleNames[index],
                        Version = 1,
                    };
                }
                UpdateSummery virtualUpdateSummery = new UpdateSummery()
                {
                    RecordInfos = assetBundleInfos,
                    ForceReserveAssetBundles = new HashSet<string>()
                    {
                        // 代码所在资源包，AssetBundlePool 无法引用 Encoder 命名空间，因此强制指定
                        "updatablelogic/hotfix.assetbundle", 
                    },
                    ForceUpdateAssetBundles = new HashSet<string>(),
                    IPV4UriBase = "http://127.0.0.1/",
                };
                SetAssetBundleInfos(virtualUpdateSummery);
                yield break;
            }
#endif
            // 下载最新的 UpdateSummery 并启用
            UriBase = PlayerPrefs.GetString(UpdateSummery.UpdateSummeryUriBaseKey, UriBase);
            AssetBundleUpdateTask.HttpClient.Value.BaseAddress = new Uri(UriBase);
            Task<byte[]> task = UpdateSummery.DownloadUpdateSummery();
            while (!task.IsCompleted)
                yield return null;
            UpdateSummery downloadUpdateSummery = UpdateSummery.DeserializeFromGZip(task.Result);
            PlayerPrefs.SetString(UpdateSummery.UpdateSummeryUriBaseKey, downloadUpdateSummery.GetUriBase());
            SetAssetBundleInfos(downloadUpdateSummery);
            yield return null;
        }
        /// <summary>
        /// 更新强制更新资源包
        /// </summary>
        public IEnumerable UpdateForceUpdateAssetBundles_Coroutine()
        {
            if (Abort)
                yield break;
            if (UpdateSummery == null)
                throw new NullReferenceException(nameof(UpdateSummery));

#if UNITY_EDITOR
            if (UseEditorAssetSystem)
                yield break;
#endif
            PlayerPrefs.SetInt(UpdateSummery.UpdateSummeryVersionKey, UpdateSummery.Version);
            yield return null;
            foreach (string forceUpdateAssetBundle in UpdateSummery.ForceUpdateAssetBundles)
            {
                UpdateAssetBundle(forceUpdateAssetBundle);
                while (UpdateTasksCount > 1)
                    yield return null;
            }

            while (UpdateTasksCount != 0)
                yield return null;
#if UNITY_EDITOR
            Debug.Log($"Have update force AssetBundles. Count : {UpdateSummery.ForceUpdateAssetBundles.Count}");
#endif
        }
        /// <summary>
        /// 开始更新强制储存资源包
        /// </summary>
        public IEnumerable StartUpdateReserveAssetBundles_Coroutine()
        {
            if (UpdateSummery == null)
                throw new NullReferenceException(nameof(UpdateSummery));

            lock (LockFactor)
            {
#if UNITY_EDITOR
                if (UseEditorAssetSystem)
                        yield break;
#endif
                AssetLoadedCoroutine.Add(UpdateReserveAssetBundles());
                yield return null;
            }

            IEnumerator<bool> UpdateReserveAssetBundles()
            {
                foreach (string forceReserveAssetBundle in UpdateSummery.ForceReserveAssetBundles)
                {
                    UpdateAssetBundle(forceReserveAssetBundle);
                    while (UpdateTasksCount > 0)
                        yield return false;
                }
            }
        }

        /* IDisposable */
        public void Dispose()
        {
            Abort = true;

            lock (LockFactor)
            {
                if (m_AssetLoadedCoroutine != null)
                {
                    m_AssetLoadedCoroutine.Clear();
                    m_AssetLoadedCoroutine = null;
                }
            }

            if (m_LoadedAssetBundles != null)
            {
                foreach (AssetBundle assetBundle in m_LoadedAssetBundles.Values)
                {
#if UNITY_EDITOR
                    // UseEditorAssetSystem 状态下 assetBundle 为 null
                    if (UseEditorAssetSystem)
                        continue;
#endif
                    assetBundle.Unload(false);
                }
                m_LoadedAssetBundles.Clear();
                m_LoadedAssetBundles = null;
            }
            if (m_UpdateAssetBundles != null)
            {
                foreach (AssetBundleUpdateTask task in m_UpdateAssetBundles.Values)
                {
                    task.Abort = true;
                    // 强制杀掉后台下载任务
                    // 其实 ThrowIfCancellationRequested 就可以了，Abort 变更是为了让读到的状态变更
                    task.CancellationToken.ThrowIfCancellationRequested();
                }
                m_UpdateAssetBundles.Clear();
                m_UpdateAssetBundles = null;
            }
            if (m_AllAssetBundleInfos != null)
            {
                foreach (AssetBundleInfo assetBundleInfo in m_AllAssetBundleInfos.Values)
                {
                    assetBundleInfo.AllReferenceCount = 0;
                    assetBundleInfo.DirectReferenceCount = 0;
                    assetBundleInfo.HaveLoaded = false;
                }
                m_AllAssetBundleInfos.Clear();
                m_AllAssetBundleInfos = null;
            }
        }
    }
}