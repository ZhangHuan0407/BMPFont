using ILRuntime.CLR.Method;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encoder
{
    public class ContainerUpdatableComponent : UpdatableComponent
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
        protected override void CacheComponentMethods()
        {
            Dictionary<string, IMethod> typeMethods = UpdatableComponentsBuffer.ComponentMethodsCache[ILTypeFullName];
            typeMethods.TryGetValue(nameof(OnEnable), out m_OnEnableMethod);
            typeMethods.TryGetValue(nameof(OnDisable), out m_OnDisableMethod);
        }
    }
}