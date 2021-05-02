#pragma warning disable CS0108, CS0649
using System;
using System.Collections;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Encoder.ILAdapter
{
    public class IEnumerator_1_ILTypeInstanceAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<ILRuntime.Runtime.Intepreter.ILTypeInstance> mget_Current_0 = new CrossBindingFunctionInfo<ILRuntime.Runtime.Intepreter.ILTypeInstance>("get_Current");
        static CrossBindingFunctionInfo<System.Boolean> mMoveNext_1 = new CrossBindingFunctionInfo<System.Boolean>("MoveNext");
        static CrossBindingMethodInfo mReset_2 = new CrossBindingMethodInfo("Reset");
        static CrossBindingMethodInfo mDispose_3 = new CrossBindingMethodInfo("Dispose");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Collections.Generic.IEnumerator<ILRuntime.Runtime.Intepreter.ILTypeInstance>);
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

        public class Adapter : System.Collections.Generic.IEnumerator<ILRuntime.Runtime.Intepreter.ILTypeInstance>, CrossBindingAdaptorType
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

            public ILRuntime.Runtime.Intepreter.ILTypeInstance Current
            {
                get
                {
                    return mget_Current_0.Invoke(this.instance);

                }
            }

            object IEnumerator.Current => Current;

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
