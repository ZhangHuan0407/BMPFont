using ILRuntime.CLR.Method;
using System.Collections.Generic;

namespace Encoder
{
    public class CommonUpdatableComponent : UpdatableComponent
    {
        /* field */

        /* ctor */
        protected override void Awake()
        {
            base.Awake();
        }

        private IMethod m_StartMethod;
        private void Start()
        {
            if (m_StartMethod != null)
                ILRuntimeService.InvokeMethod(m_StartMethod, Instance, EmptyParameters);
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

        private IMethod m_OnDestroyMethod;
        private void OnDestroy()
        {
            if (m_OnDestroyMethod != null)
                ILRuntimeService.InvokeMethod(m_OnDestroyMethod, Instance, EmptyParameters);
        }

        /* func */
        private IMethod m_UpdateMethod;
        private void Update()
        {
            if (m_UpdateMethod != null)
                ILRuntimeService.InvokeMethod(m_UpdateMethod, Instance, EmptyParameters);
        }

        protected override void CacheComponentMethods()
        {
            Dictionary<string, IMethod> typeMethods = UpdatableComponentsBuffer.ComponentMethodsCache[ILTypeFullName];
            typeMethods.TryGetValue(nameof(Start), out m_StartMethod);
            typeMethods.TryGetValue(nameof(OnEnable), out m_OnEnableMethod);
            typeMethods.TryGetValue(nameof(OnDisable), out m_OnDisableMethod);
            typeMethods.TryGetValue(nameof(OnDestroy), out m_OnDestroyMethod);
            typeMethods.TryGetValue(nameof(Update), out m_UpdateMethod);
        }
    }
}