using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tween
{
    public static class TransformTween
    {
        internal static IEnumerator<Tweener> DoPosition_Internal(TimeTweener tweener, Transform transform, Vector3 targetPosition, Vector3? overrideStartPosition)
        {
            if (!transform)
            {
                yield return null;
                yield break;
            }

            float deltaTime = Time.deltaTime;
            Vector3 startPosition = overrideStartPosition ?? transform.position;
            tweener.CumulativeTime += deltaTime;
            transform.position = tweener.Normalized * (targetPosition - startPosition) + startPosition;

            while (tweener.Normalized < 1f)
            {
                yield return tweener;
                if (!transform)
                {
                    yield return null;
                    yield break;
                }
                deltaTime = Time.deltaTime;
                tweener.CumulativeTime += deltaTime;
                transform.position = tweener.Normalized * (targetPosition - startPosition) + startPosition;
            }
        }
        public static Tweener DoPosition(this Transform transform, Vector3 targetPosition, float duration, Vector3? overrideStartPosition = null)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform));
            if (duration < 0f)
                throw new ArgumentException(nameof(duration));

            TimeTweener tweener = new TimeTweener()
            {
                DurationTime = duration,
                TimeType = TweenerTimeType.ScaleTime,
            };
            tweener.Enumerator = DoPosition_Internal(tweener, transform, targetPosition, overrideStartPosition);
            return tweener;
        }

        internal static IEnumerator<Tweener> DoLocalPosition_Internal(TimeTweener tweener, Transform transform, Vector3 targetPosition, Vector3? overrideStartLocalPosition)
        {
            if (!transform)
            {
                yield return null;
                yield break;
            }

            float deltaTime = Time.deltaTime;
            Vector3 startPosition = overrideStartLocalPosition ?? transform.localPosition;
            tweener.CumulativeTime += deltaTime;
            transform.localPosition = tweener.Normalized * (targetPosition - startPosition) + startPosition;

            while (tweener.Normalized < 1f)
            {
                yield return tweener;
                if (!transform)
                {
                    yield return null;
                    yield break;
                }
                deltaTime = Time.deltaTime;
                tweener.CumulativeTime += deltaTime;
                transform.localPosition = tweener.Normalized * (targetPosition - startPosition) + startPosition;
            }
        }
        public static Tweener DoLocalPosition(this Transform transform, Vector3 targetPosition, float duration, Vector3? overrideStartLocalPosition = null)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform));
            if (duration < 0f)
                throw new ArgumentException(nameof(duration));

            TimeTweener tweener = new TimeTweener()
            {
                DurationTime = duration,
                TimeType = TweenerTimeType.ScaleTime,
            };
            tweener.Enumerator = DoLocalPosition_Internal(tweener, transform, targetPosition, overrideStartLocalPosition);
            return tweener;
        }

        internal static IEnumerator<Tweener> DoLocalScale_Internal(TimeTweener tweener, Transform transform, Vector3 targetLocalScale, Vector3? overrideStartLocalScale)
        {
            if (!transform)
            {
                yield return null;
                yield break;
            }

            float deltaTime = Time.deltaTime;
            Vector3 startLocalScale = overrideStartLocalScale ?? transform.localScale;
            tweener.CumulativeTime += deltaTime;
            transform.localScale = tweener.Normalized * (targetLocalScale - startLocalScale) + startLocalScale;

            while (tweener.Normalized < 1f)
            {
                yield return tweener;
                if (!transform)
                {
                    yield return null;
                    yield break;
                }
                deltaTime = Time.deltaTime;
                tweener.CumulativeTime += deltaTime;
                transform.localScale = tweener.Normalized * (targetLocalScale - startLocalScale) + startLocalScale;
            }
        }
        public static Tweener DoLocalScale(this Transform transform, Vector3 targetLocalScale, float duration, Vector3? overrideStartLocalScale = null)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform));
            if (duration < 0f)
                throw new ArgumentException(nameof(duration));

            TimeTweener tweener = new TimeTweener()
            {
                DurationTime = duration,
                TimeType = TweenerTimeType.ScaleTime,
            };
            tweener.Enumerator = DoLocalScale_Internal(tweener, transform, targetLocalScale, overrideStartLocalScale);
            return tweener;
        }

        internal static IEnumerator<Tweener> DoLocalEuler_Internal(TimeTweener tweener, Transform transform, Vector3 targetEuler, Vector3 startEuler)
        {
            if (!transform)
            {
                yield return null;
                yield break;
            }

            float deltaTime = Time.deltaTime;
            tweener.CumulativeTime += deltaTime;
            transform.localEulerAngles = tweener.Normalized * (targetEuler - startEuler) + startEuler;

            while (tweener.Normalized < 1f)
            {
                yield return tweener;
                if (!transform)
                {
                    yield return null;
                    yield break;
                }
                deltaTime = Time.deltaTime;
                tweener.CumulativeTime += deltaTime;
                transform.localEulerAngles = tweener.Normalized * (targetEuler - startEuler) + startEuler;
            }
        }
        /// <summary>
        /// 对一个 Transform 对象创建补间动画实例
        /// </summary>
        /// <param name="transform">执行补间动画的实例</param>
        /// <param name="targetEuler">结束时欧拉角度</param>
        /// <param name="duration">旋转持续时间</param>
        /// <param name="startEuler">因为存在优角劣角问题，不能自行获取起始欧拉角度</param>
        /// <returns>Tweener 实例</returns>
        public static Tweener DoLocalEuler(this Transform transform, Vector3 targetEuler, float duration, Vector3 startEuler)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform));
            if (duration < 0f)
                throw new ArgumentException(nameof(duration));

            TimeTweener tweener = new TimeTweener()
            {
                DurationTime = duration,
                TimeType = TweenerTimeType.ScaleTime,
            };
            tweener.Enumerator = DoLocalEuler_Internal(tweener, transform, targetEuler, startEuler);
            return tweener;
        }
    }
}