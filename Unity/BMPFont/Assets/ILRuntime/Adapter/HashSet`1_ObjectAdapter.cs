#pragma warning disable CS0108, CS0649
using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Encoder.ILAdapter
{   
    public class HashSet_1_ObjectAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo<System.Runtime.Serialization.SerializationInfo, System.Runtime.Serialization.StreamingContext> mGetObjectData_0 = new CrossBindingMethodInfo<System.Runtime.Serialization.SerializationInfo, System.Runtime.Serialization.StreamingContext>("GetObjectData");
        static CrossBindingMethodInfo<System.Object> mOnDeserialization_1 = new CrossBindingMethodInfo<System.Object>("OnDeserialization");
        static CrossBindingFunctionInfo<System.Collections.IEnumerator> mGetEnumerator_2 = new CrossBindingFunctionInfo<System.Collections.IEnumerator>("GetEnumerator");
        static CrossBindingFunctionInfo<System.Boolean> mget_IsReadOnly_3 = new CrossBindingFunctionInfo<System.Boolean>("get_IsReadOnly");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Collections.Generic.HashSet<System.Object>);
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

        public class Adapter : System.Collections.Generic.HashSet<System.Object>, CrossBindingAdaptorType
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

            public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            {
                if (mGetObjectData_0.CheckShouldInvokeBase(this.instance))
                    base.GetObjectData(info, context);
                else
                    mGetObjectData_0.Invoke(this.instance, info, context);
            }

            public override void OnDeserialization(System.Object sender)
            {
                if (mOnDeserialization_1.CheckShouldInvokeBase(this.instance))
                    base.OnDeserialization(sender);
                else
                    mOnDeserialization_1.Invoke(this.instance, sender);
            }

            public System.Collections.IEnumerator GetEnumerator()
            {
                return mGetEnumerator_2.Invoke(this.instance);
            }

            public System.Boolean IsReadOnly
            {
            get
            {
                return mget_IsReadOnly_3.Invoke(this.instance);

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
