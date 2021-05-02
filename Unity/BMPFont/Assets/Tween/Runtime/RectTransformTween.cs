using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tween
{
    public static class RectTransformTween
    {
        internal static IEnumerator<Tweener> DoAnchoredPosition_Internal(TimeTweener tweener, RectTransform rectTransform, Vector2 targetPosition, Vector2? overrideStartAnchoredPosition)
        {
            if (!rectTransform)
            {
                yield return null;
                yield break;
            }

            float deltaTime = Time.deltaTime;
            Vector2 startPosition = overrideStartAnchoredPosition ?? rectTransform.anchoredPosition;
            tweener.CumulativeTime += deltaTime;
            rectTransform.anchoredPosition = tweener.Normalized * (targetPosition - startPosition) + startPosition;

            while (tweener.Normalized < 1f)
            {
                yield return tweener;
                if (!rectTransform)
                {
                    yield return null;
                    yield break;
                }
                deltaTime = Time.deltaTime;
                tweener.CumulativeTime += deltaTime;
                rectTransform.anchoredPosition = tweener.Normalized * (targetPosition - startPosition) + startPosition;
            }
        }
        public static Tweener DoAnchoredPosition(this RectTransform rectTransform, Vector3 targetPosition, float duration, Vector2? overrideStartAnchoredPosition = null)
        {
            if (rectTransform == null)
                throw new ArgumentNullException(nameof(rectTransform));
            if (duration < 0f)
                throw new ArgumentException(nameof(duration));

            TimeTweener tweener = new TimeTweener()
            {
                DurationTime = duration,
                TimeType = TweenerTimeType.ScaleTime,
            };
            tweener.Enumerator = DoAnchoredPosition_Internal(tweener, rectTransform, targetPosition, overrideStartAnchoredPosition);
            return tweener;
        }
    }
}