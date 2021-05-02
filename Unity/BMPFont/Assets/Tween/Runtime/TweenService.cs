using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tween
{
    public class TweenService : MonoBehaviour, IDisposable
    {
#if UNITY_EDITOR
        /* field */
        internal TweenerBehaviourRecord Behaviour;
        private Queue<TweenerBehaviourRecord> BehaviourHistory;
#endif

        public static TweenService Instance { get; internal set; }

        private LinkedList<Tweener> m_Tweeners;
        protected LinkedList<Tweener> Tweeners
        {
            get => m_Tweeners = m_Tweeners ?? new LinkedList<Tweener>();
            set => m_Tweeners = value;
        }

        private LinkedList<Tweener> m_NextFrameTweeners;
        protected LinkedList<Tweener> NextFrameTweeners
        {
            get => m_NextFrameTweeners = m_NextFrameTweeners ?? new LinkedList<Tweener>();
            set => m_NextFrameTweeners = value;
        }

        /* inter */

        /* ctor */
        private void Awake()
        {
#if UNITY_EDITOR
            BehaviourHistory = new Queue<TweenerBehaviourRecord>();
#endif

            Instance = Instance ?? this;
            DontDestroyOnLoad(gameObject);
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
            if (Instance != this)
            {
                enabled = false;
                return;
            }
#if UNITY_EDITOR
            BehaviourHistory.Enqueue(Behaviour);
            Behaviour = default;
            if (BehaviourHistory.Count > 10)
                BehaviourHistory.Dequeue();
#endif
            LinkedList<Tweener> temp = Tweeners;
            Tweeners = NextFrameTweeners;
            NextFrameTweeners = temp;

            while (Tweeners.Count > 0)
            {
                Tweener tweener = Tweeners.First.Value;
                Tweeners.RemoveFirst();
                try
                {
                    bool haveNext = tweener.Enumerator.MoveNext();
                    Tweener current = tweener.Enumerator.Current;
                    tweener.UpdateTween();
                    // Update 回掉中停 Tweener
                    if (tweener.State == TweenerState.Stop)
                        haveNext = false;
                    // 资源被摧毁导致停止
                    else if (current == null)
                        tweener.State = TweenerState.AssetHaveBeenDestroy;
                    else if (tweener.Normalized >= 1f)
                    {
                        tweener.State = TweenerState.Finish;
                        tweener.CompleteTween();
                    }
                    if (current != null && haveNext)
                        NextFrameTweeners.AddLast(current);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    if (tweener != null)
                        tweener.State = TweenerState.Error;
                }
            }
        }

        public static void Add(Tweener tweener, bool force = false)
        {
            if (tweener is null)
                throw new ArgumentNullException(nameof(tweener));
            Instance.Add_Internal(tweener, force);
        }
        internal void Add_Internal(Tweener tweener, bool force)
        {
            if (tweener.State == TweenerState.WaitForActivation
                || force)
            {
#if UNITY_EDITOR
                Behaviour.AddTweenerSuccessTimes++;
#endif
                tweener.State = TweenerState.IsRunnning;
                NextFrameTweeners.AddLast(tweener);
            }
        }

        public static void Remove(Tweener tweener)
        {
            if (tweener is null)
                throw new ArgumentNullException(nameof(tweener));
            Instance.Remove_Internal(tweener);
        }
        internal void Remove_Internal(Tweener tweener)
        {
            if (tweener.State == TweenerState.WaitForActivation)
            {
#if UNITY_EDITOR
                Behaviour.StopTweenerSuccessTimes++;
#endif
                tweener.State = TweenerState.Stop;
            }
            else if (tweener.State == TweenerState.IsRunnning)
            {
#if UNITY_EDITOR
                Behaviour.StopTweenerSuccessTimes++;
#endif
                tweener.State = TweenerState.Stop;
                bool haveRemove = NextFrameTweeners.Remove(tweener) || Tweeners.Remove(tweener);
                if (!haveRemove)
                    Debug.LogError($"Not found tweener in list, but it's state is running.");
            }
            else if (tweener.State == TweenerState.Finish
                && tweener.NextTweener != null)
            {
#if UNITY_EDITOR
                Behaviour.StopTweenerSuccessTimes++;
#endif
                tweener.NextTweener.State = TweenerState.Stop;
            }
        }

        public static void Restore(Tweener tweener)
        {
            if (tweener is null)
                throw new ArgumentNullException(nameof(tweener));
            Instance.Restore_Internal(tweener);
        }
        internal void Restore_Internal(Tweener tweener)
        {
            if (tweener.State == TweenerState.Stop)
            {
#if UNITY_EDITOR
                Behaviour.RestoreTweenerSuccessTimes++;
#endif
                tweener.State = TweenerState.IsRunnning;
                NextFrameTweeners.AddLast(tweener);
            }
        }

        /* IDisposable */
        public void Dispose()
        {
            if (m_Tweeners != null)
            {
                foreach (Tweener tweener in m_Tweeners)
                {
                    if (tweener.State == TweenerState.IsRunnning)
                        tweener.State = TweenerState.Stop;
                }
                m_Tweeners = null;
            }
            if (m_NextFrameTweeners != null)
            {
                foreach (Tweener tweener in m_NextFrameTweeners)
                {
                    if (tweener.State == TweenerState.IsRunnning)
                        tweener.State = TweenerState.Stop;
                }
                m_NextFrameTweeners = null;
            }
        }
    }
}