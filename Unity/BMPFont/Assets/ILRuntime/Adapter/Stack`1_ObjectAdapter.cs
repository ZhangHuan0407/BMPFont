#pragma warning disable CS0108, CS0649
using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Encoder.ILAdapter
{   
    public class Stack_1_ObjectAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Collections.IEnumerator> mGetEnumerator_0 = new CrossBindingFunctionInfo<System.Collections.IEnumerator>("GetEnumerator");
        static CrossBindingMethodInfo<System.Array, System.Int32> mCopyTo_1 = new CrossBindingMethodInfo<System.Array, System.Int32>("CopyTo");
        static CrossBindingFunctionInfo<System.Object> mget_SyncRoot_2 = new CrossBindingFunctionInfo<System.Object>("get_SyncRoot");
        static CrossBindingFunctionInfo<System.Boolean> mget_IsSynchronized_3 = new CrossBindingFunctionInfo<System.Boolean>("get_IsSynchronized");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Collections.Generic.Stack<System.Object>);
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

        public class Adapter : System.Collections.Generic.Stack<System.Object>, CrossBindingAdaptorType
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

            public System.Collections.IEnumerator GetEnumerator()
            {
                return mGetEnumerator_0.Invoke(this.instance);
            }

            public void CopyTo(System.Array array, System.Int32 index)
            {
                mCopyTo_1.Invoke(this.instance, array, index);
            }

            public System.Object SyncRoot
            {
            get
            {
                return mget_SyncRoot_2.Invoke(this.instance);

            }
            }

            public System.Boolean IsSynchronized
            {
            get
            {
                return mget_IsSynchronized_3.Invoke(this.instance);

            }
            }

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
        }
    }
}

#pragma warning restore CS0108, CS0649
