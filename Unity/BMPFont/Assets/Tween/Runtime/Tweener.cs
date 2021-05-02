﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Tween
{
    public abstract class Tweener : IEnumerable<Tweener>
    {
        /* field */
        /// <summary>
        /// 反馈当前 Tweener 实例的状态
        /// </summary>
        public TweenerState State { get; internal set; }

        /// <summary>
        /// 迭代器实例，对于激活的 Tweener 每帧一次调用
        /// <para>如果当前没有执行完，返回实例自身。</para>
        /// <para>如果操作对象已经不存在，返回 null。</para>
        /// <para>如果当前执行完，返回 NextTweener。</para>
        /// </summary>
        internal IEnumerator<Tweener> Enumerator;
        public event Action OnUpdate_Handle;
        public event Action OnComplete_Handle;
        /// <summary>
        /// 链式 Tweener 的头部实例
        /// <para>首个 Tweener 实例 HeadTweener 指向自己</para>
        /// </summary>
        public Tweener HeadTweener { get; protected set; }
        /// <summary>
        /// 完成后指向的下一个 Tweener 实例，可能为空
        /// </summary>
        public Tweener NextTweener { get; set; }

        /* inter */
        /// <summary>
        /// 获得当前 Tweener 实例的归一化进度，值范围 [0, 1]
        /// <para>在子继承中实现</para>
        /// </summary>
        public abstract float Normalized { get; }
        /// <summary>
        /// 获得当前 Tweener 实例的状态码，具体含义查询 TweenerState
        /// </summary>
        public int StateCode => (int)State;

        /* ctor */
        public Tweener()
        {
            State = TweenerState.WaitForActivation;
            HeadTweener = this;
        }

        /* func */
        /// <summary>
        /// 将当前 Tweener
        /// </summary>
        /// <returns>是否启动成功</returns>
        public Tweener DoIt()
        {
#if UNITY_EDITOR
            if (TweenService.Instance != null)
                TweenService.Instance.Behaviour.AddTweenerTimes++;
#endif
            TweenService.Add(HeadTweener);
            return this;
        }
        /// <summary>
        /// 添加一个每次更新时回掉的函数
        /// 调用时 State => TweenerState.Finish || TweenerState.IsRunnning
        /// </summary>
        /// <param name="action">回掉的函数</param>
        /// <returns>当前实例，接着链式加内容</returns>
        public Tweener OnUpdate(Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));
            OnUpdate_Handle += action;
            return this;
        }
        public Tweener OnComplete(Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));
            OnComplete_Handle += action;
            return this;
        }
        public Tweener Then(Tweener nextTweener)
        {
            if (NextTweener != null)
                NextTweener.HeadTweener = NextTweener;
            NextTweener = nextTweener;
            nextTweener.HeadTweener = HeadTweener;
            return nextTweener;
        }

        /// <summary>
        /// 从当前实例的 Head 实例出发，向 NextTweener 方向查找，将第一个能够停止的 Tweener 停止
        /// </summary>
        /// <param name="selectTweener">实际停止的 Tweener 实例或 null</param>
        /// <returns>成功停止了任何的 Tweener</returns>
        public bool FromHeadToEndIfNeedStop(out Tweener selectTweener)
        {
#if UNITY_EDITOR
            if (TweenService.Instance != null)
                TweenService.Instance.Behaviour.StopTweenerTimes++;
#endif
            foreach (Tweener tweener in this)
            {
                switch (tweener.State)
                {
                    case TweenerState.Error:
                    case TweenerState.AssetHaveBeenDestroy:
                    case TweenerState.Stop:
                        selectTweener = tweener;
                        return false;
                    case TweenerState.IsRunnning:
                    case TweenerState.WaitForActivation:
                        selectTweener = tweener;
                        TweenService.Remove(tweener);
                        return true;
                    default:
                        break;
                }
            }
            selectTweener = null;
            return false;
        }
        public bool FromHeadToEndIfNeedRestore(out Tweener selectTweener)
        {
#if UNITY_EDITOR
            if (TweenService.Instance != null)
                TweenService.Instance.Behaviour.RestoreTweenerTimes++;
#endif
            foreach (Tweener tweener in this)
            {
                switch (tweener.State)
                {
                    case TweenerState.Error:
                    case TweenerState.AssetHaveBeenDestroy:
                        selectTweener = tweener;
                        return false;
                    case TweenerState.Stop:
                        selectTweener = tweener;
                        TweenService.Restore(tweener);
                        return true;
                    default:
                        break;
                }
            }
            selectTweener = null;
            return false;
        }

        internal void UpdateTween()
        {
            OnUpdate_Handle?.Invoke();
        }
        internal virtual void CompleteTween()
        {
            OnComplete_Handle?.Invoke();
        }

        /* IEnumerable */
        public IEnumerator<Tweener> GetEnumerator()
        {
            Tweener tweener = HeadTweener;
            do
            {
                yield return tweener;
                tweener = tweener.NextTweener;
            } while (tweener != null);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}