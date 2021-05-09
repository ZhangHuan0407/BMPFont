using ILRuntime.CLR.Method;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encoder
{
    public class LogicUpdatableComponent : UpdatableComponent
    {
        /* const */

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

        private IMethod m_LateUpdateMethod;
        private void LateUpdate()
        {
            if (m_LateUpdateMethod != null)
                ILRuntimeService.InvokeMethod(m_LateUpdateMethod, Instance, EmptyParameters);
        }

        private IMethod m_FixedUpdateMethod;
        private void FixedUpdate()
        {
            if (m_FixedUpdateMethod != null)
                ILRuntimeService.InvokeMethod(m_FixedUpdateMethod, Instance, EmptyParameters);
        }

        protected override void CacheComponentMethods()
        {
            Dictionary<string, IMethod> typeMethods = UpdatableComponentsBuffer.ComponentMethodsCache[ILTypeFullName];
            typeMethods.TryGetValue(nameof(OnEnable), out m_OnEnableMethod);
            typeMethods.TryGetValue(nameof(OnDisable), out m_OnDisableMethod);
            typeMethods.TryGetValue(nameof(Update), out m_UpdateMethod);
            typeMethods.TryGetValue(nameof(LateUpdate), out m_LateUpdateMethod);
            typeMethods.TryGetValue(nameof(FixedUpdate), out m_FixedUpdateMethod);
        }
    }
}