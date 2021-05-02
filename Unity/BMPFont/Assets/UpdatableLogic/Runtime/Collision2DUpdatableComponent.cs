using ILRuntime.CLR.Method;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encoder
{
    public class Collision2DUpdatableComponent : UpdatableComponent
    {
        /* field */




        /* ctor */
        protected override void Awake()
        {
            base.Awake();
        }

        private IMethod m_OnEnableMethod;
        private void OnEnable()
        {
            if (m_OnEnableMethod != null)
                ILRuntimeService.InvokeMethod(m_OnEnableMethod, Instance, EmptyParameters);
        }
        private IMethod m_OnDisableMethod;
        private void OnDisable()
        {
            if (m_OnDisableMethod != null)
                ILRuntimeService.InvokeMethod(m_OnDisableMethod, Instance, EmptyParameters);
        }

        /* func */
        private IMethod m_UpdateMethod;
        private void Update()
        {
            if (m_UpdateMethod != null)
                ILRuntimeService.InvokeMethod(m_UpdateMethod, Instance, EmptyParameters);
        }

        private IMethod m_OnCollisionEnter2D;
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (m_OnCollisionEnter2D != null)
                ILRuntimeService.InvokeMethod(m_OnCollisionEnter2D, Instance, new object[] { collision });
        }
        private IMethod m_OnCollisionExit2D;
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (m_OnCollisionExit2D != null)
                ILRuntimeService.InvokeMethod(m_OnCollisionExit2D, Instance, new object[] { collision });
        }
        private IMethod m_OnCollisionStay2D;
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (m_OnCollisionStay2D != null)
                ILRuntimeService.InvokeMethod(m_OnCollisionStay2D, Instance, new object[] { collision });
        }
        private IMethod m_OnTriggerEnter2D;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (m_OnTriggerEnter2D != null)
                ILRuntimeService.InvokeMethod(m_OnTriggerEnter2D, Instance, new object[] { collision });
        }
        private IMethod m_OnTriggerExit2D;
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (m_OnTriggerExit2D != null)
                ILRuntimeService.InvokeMethod(m_OnTriggerExit2D, Instance, new object[] { collision });
        }
        private IMethod m_OnTriggerStay2D;
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (m_OnTriggerStay2D != null)
                ILRuntimeService.InvokeMethod(m_OnTriggerStay2D, Instance, new object[] { collision });
        }

        protected override void CacheComponentMethods()
        {
            Dictionary<string, IMethod> typeMethods = CustomComponentMethodBuffer.ComponentMethodsCache[ILTypeFullName];
            typeMethods.TryGetValue(nameof(OnEnable), out m_OnEnableMethod);
            typeMethods.TryGetValue(nameof(OnDisable), out m_OnDisableMethod);
            typeMethods.TryGetValue(nameof(Update), out m_UpdateMethod);

            typeMethods.TryGetValue(nameof(OnCollisionEnter2D), out m_OnCollisionEnter2D);
            typeMethods.TryGetValue(nameof(OnCollisionExit2D), out m_OnCollisionExit2D);
            typeMethods.TryGetValue(nameof(OnCollisionStay2D), out m_OnCollisionStay2D);
            typeMethods.TryGetValue(nameof(OnTriggerEnter2D), out m_OnTriggerEnter2D);
            typeMethods.TryGetValue(nameof(OnTriggerExit2D), out m_OnTriggerExit2D);
            typeMethods.TryGetValue(nameof(OnTriggerStay2D), out m_OnTriggerStay2D);
        }
    }
}