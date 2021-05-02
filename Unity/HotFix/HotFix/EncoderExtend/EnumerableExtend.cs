using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace HotFix.EncoderExtend
{
    public static class EnumerableExtend
    {
        #region Fix IEnumerable can't get IEnumerator
        /*
            // Try to fix it in adaptor, it's not work.
            IEnumerable AA()
            {
                yield return 1;
                yield break;
            }
            IEnumerable a = AA(); // can get a, it's not null
            Debug.Log($"a, {a.GetType().FullName} : {a}"); // a, HotFix.GameStart/<AA>d__5 : HotFix.GameStart/<AA>d__5

            
            object b = a.GetEnumerator();
            Debug.Log($"b, {b?.GetType().FullName} : {b}"); // b,  : 
            
            b = (a as IEnumerable<object>).GetEnumerator();
            Debug.Log($"b, {b?.GetType().FullName} : {b}"); // b,  : 
            
            b = a.GetEnumeratorByReflect();
            Debug.Log($"b, {b?.GetType().FullName} : {b}"); // b, HotFix.GameStart/<AA>d__5 : HotFix.GameStart/<AA>d__5

            System.Reflection.MethodInfo method = a.GetType().GetMethod(
                "System.Collections.Generic.IEnumerable<System.Object>.GetEnumerator");
            b = method.Invoke(a, new object[0]);
            Debug.Log($"b, {b?.GetType().FullName} : {b}"); // b, HotFix.GameStart/<AA>d__5 : HotFix.GameStart/<AA>d__5
         */

        public static IEnumerator GetEnumeratorByReflect(this IEnumerable enumerable)
        {
            MethodInfo method = enumerable.GetType().GetMethod("System.Collections.Generic.IEnumerable<System.Object>.GetEnumerator");
            return method.Invoke(enumerable, new object[0]) as IEnumerator;
        }
        public static IEnumerator GetEnumeratorByReflect(this IEnumerable<object> enumerable)
        {
            MethodInfo method = enumerable.GetType().GetMethod("System.Collections.Generic.IEnumerable<System.Object>.GetEnumerator");
            return method.Invoke(enumerable, new object[0]) as IEnumerator;
        }
        #endregion
    }
}