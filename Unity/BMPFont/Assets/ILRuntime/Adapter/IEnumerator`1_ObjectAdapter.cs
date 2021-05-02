#pragma warning disable CS0108, CS0649
using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Encoder.ILAdapter
{   
    public class IEnumerator_1_ObjectAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Object> mget_Current_0 = new CrossBindingFunctionInfo<System.Object>("get_Current");
        static CrossBindingFunctionInfo<System.Boolean> mMoveNext_1 = new CrossBindingFunctionInfo<System.Boolean>("MoveNext");
        static CrossBindingMethodInfo mReset_2 = new CrossBindingMethodInfo("Reset");
        static CrossBindingMethodInfo mDispose_3 = new CrossBindingMethodInfo("Dispose");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Collections.Generic.IEnumerator<System.Object>);
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

        public class Adapter : System.Collections.Generic.IEnumerator<System.Object>, CrossBindingAdaptorType
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

            public System.Boolean MoveNext()
            {
                return mMoveNext_1.Invoke(this.instance);
            }

            public void Reset()
            {
                mReset_2.Invoke(this.instance);
            }

            public void Dispose()
            {
                mDispose_3.Invoke(this.instance);
            }

            public System.Object Current
            {
            get
            {
                return mget_Current_0.Invoke(this.instance);

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
