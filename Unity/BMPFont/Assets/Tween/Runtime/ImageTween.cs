using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tween
{
    public static class ImageTween
    {
        internal static IEnumerator<Tweener> DoColor_Internal(TimeTweener tweener, Graphic graphic, Color targetColor, Color? overrideStartColor)
        {
            if (!graphic)
            {
                yield return null;
                yield break;
            }

            float deltaTime = Time.deltaTime;
            Color startColor = overrideStartColor ?? graphic.color;
            tweener.CumulativeTime += deltaTime;
            graphic.color = tweener.Normalized * (targetColor - startColor) + startColor;

            while (tweener.Normalized < 1f)
            {
                yield return tweener;
                if (!graphic)
                {
                    yield return null;
                    yield break;
                }
                deltaTime = Time.deltaTime;
                tweener.CumulativeTime += deltaTime;
                graphic.color = tweener.Normalized * (targetColor - startColor) + startColor;
            }
        }
        public static Tweener DoColor(this Graphic graphic, Color targetColor, float duration, Color? overrideStartColor = null)
        {
            if (graphic == null)
                throw new ArgumentNullException(nameof(graphic));
            if (duration < 0f)
                throw new ArgumentException(nameof(duration));

            TimeTweener tweener = new TimeTweener()
            {
                DurationTime = duration,
                TimeType = TweenerTimeType.ScaleTime,
            };
            tweener.Enumerator = DoColor_Internal(tweener, graphic, targetColor, overrideStartColor);
            return tweener;
        }

        internal static IEnumerator<Tweener> DoFillAmount_Internal(TimeTweener tweener, Image image, float fillAmount, float? overrideStartFillAmout)
        {
            if (!image)
            {
                yield return null;
                yield break;
            }

            float deltaTime = Time.deltaTime;
            float startFillAmount = overrideStartFillAmout ?? image.fillAmount;
            tweener.CumulativeTime += deltaTime;
            image.fillAmount = tweener.Normalized * (fillAmount - startFillAmount) + startFillAmount;

            while (tweener.Normalized < 1f)
            {
                yield return tweener;
                if (!image)
                {
                    yield return null;
                    yield break;
                }
                deltaTime = Time.deltaTime;
                tweener.CumulativeTime += deltaTime;
                image.fillAmount = tweener.Normalized * (fillAmount - startFillAmount) + startFillAmount;
            }
        }
        public static Tweener DoFillAmount(this Image image, float fillAmount, float duration, float? overrideStartFillAmout = null)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (fillAmount < 0f)
                throw new ArgumentException(nameof(fillAmount));
            else if (fillAmount > 1f)
                fillAmount = 1f;
            if (duration < 0f)
                throw new ArgumentException(nameof(duration));

            TimeTweener tweener = new TimeTweener()
            {
                DurationTime = duration,
                TimeType = TweenerTimeType.ScaleTime,
            };
            tweener.Enumerator = DoFillAmount_Internal(tweener, image, fillAmount, overrideStartFillAmout);
            return tweener;
        }
    }
}