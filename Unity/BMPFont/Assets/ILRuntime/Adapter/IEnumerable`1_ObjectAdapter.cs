#pragma warning disable CS0108, CS0649
using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Encoder.ILAdapter
{   
    public class IEnumerable_1_ObjectAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<IEnumerator<object>> mGetEnumerator_0 = new CrossBindingFunctionInfo<IEnumerator<object>>("GetEnumerator");
        static CrossBindingFunctionInfo<IEnumerator<object>> mGetEnumerator_1 = new CrossBindingFunctionInfo<IEnumerator<object>>("System.Collections.IEnumerable.GetEnumerator");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(IEnumerable<object>);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : IEnumerable<System.Object>, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public IEnumerator<object> GetEnumerator() => mGetEnumerator_0.Invoke(this.instance);

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }

            IEnumerator IEnumerable.GetEnumerator() => mGetEnumerator_1.Invoke(this.instance);
        }
    }
}

#pragma warning restore CS0108, CS0649
