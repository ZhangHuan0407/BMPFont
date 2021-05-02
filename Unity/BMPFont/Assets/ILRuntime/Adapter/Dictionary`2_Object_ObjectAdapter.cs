#pragma warning disable CS0108, CS0649
using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Encoder.ILAdapter
{   
    public class Dictionary_2_Object_ObjectAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo<System.Runtime.Serialization.SerializationInfo, System.Runtime.Serialization.StreamingContext> mGetObjectData_0 = new CrossBindingMethodInfo<System.Runtime.Serialization.SerializationInfo, System.Runtime.Serialization.StreamingContext>("GetObjectData");
        static CrossBindingMethodInfo<System.Object> mOnDeserialization_1 = new CrossBindingMethodInfo<System.Object>("OnDeserialization");
        static CrossBindingFunctionInfo<System.Collections.ICollection> mget_Keys_2 = new CrossBindingFunctionInfo<System.Collections.ICollection>("get_Keys");
        static CrossBindingFunctionInfo<System.Collections.ICollection> mget_Values_3 = new CrossBindingFunctionInfo<System.Collections.ICollection>("get_Values");
        static CrossBindingFunctionInfo<System.Object, System.Boolean> mContains_4 = new CrossBindingFunctionInfo<System.Object, System.Boolean>("Contains");
        static CrossBindingFunctionInfo<System.Boolean> mget_IsReadOnly_5 = new CrossBindingFunctionInfo<System.Boolean>("get_IsReadOnly");
        static CrossBindingFunctionInfo<System.Boolean> mget_IsFixedSize_6 = new CrossBindingFunctionInfo<System.Boolean>("get_IsFixedSize");
        static CrossBindingFunctionInfo<System.Collections.IDictionaryEnumerator> mGetEnumerator_7 = new CrossBindingFunctionInfo<System.Collections.IDictionaryEnumerator>("GetEnumerator");
        static CrossBindingMethodInfo<System.Array, System.Int32> mCopyTo_8 = new CrossBindingMethodInfo<System.Array, System.Int32>("CopyTo");
        static CrossBindingFunctionInfo<System.Object> mget_SyncRoot_9 = new CrossBindingFunctionInfo<System.Object>("get_SyncRoot");
        static CrossBindingFunctionInfo<System.Boolean> mget_IsSynchronized_10 = new CrossBindingFunctionInfo<System.Boolean>("get_IsSynchronized");
        static CrossBindingMethodInfo<System.Collections.Generic.KeyValuePair<System.Object, System.Object>> mAdd_11 = new CrossBindingMethodInfo<System.Collections.Generic.KeyValuePair<System.Object, System.Object>>("Add");
        static CrossBindingFunctionInfo<System.Collections.Generic.KeyValuePair<System.Object, System.Object>, System.Boolean> mContains_12 = new CrossBindingFunctionInfo<System.Collections.Generic.KeyValuePair<System.Object, System.Object>, System.Boolean>("Contains");
        static CrossBindingMethodInfo<System.Collections.Generic.KeyValuePair<System.Object, System.Object>[], System.Int32> mCopyTo_13 = new CrossBindingMethodInfo<System.Collections.Generic.KeyValuePair<System.Object, System.Object>[], System.Int32>("CopyTo");
        static CrossBindingFunctionInfo<System.Collections.Generic.KeyValuePair<System.Object, System.Object>, System.Boolean> mRemove_14 = new CrossBindingFunctionInfo<System.Collections.Generic.KeyValuePair<System.Object, System.Object>, System.Boolean>("Remove");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Collections.Generic.Dictionary<System.Object, System.Object>);
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

        public class Adapter : System.Collections.Generic.Dictionary<System.Object, System.Object>, CrossBindingAdaptorType
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

            public System.Boolean Contains(System.Object key)
            {
                return mContains_4.Invoke(this.instance, key);
            }

            public System.Collections.IDictionaryEnumerator GetEnumerator()
            {
                return mGetEnumerator_7.Invoke(this.instance);
            }

            public void CopyTo(System.Array array, System.Int32 index)
            {
                mCopyTo_8.Invoke(this.instance, array, index);
            }

            public void Add(System.Collections.Generic.KeyValuePair<System.Object, System.Object> item)
            {
                mAdd_11.Invoke(this.instance, item);
            }

            public System.Boolean Contains(System.Collections.Generic.KeyValuePair<System.Object, System.Object> item)
            {
                return mContains_12.Invoke(this.instance, item);
            }

            public void CopyTo(System.Collections.Generic.KeyValuePair<System.Object, System.Object>[] array, System.Int32 arrayIndex)
            {
                mCopyTo_13.Invoke(this.instance, array, arrayIndex);
            }

            public System.Boolean Remove(System.Collections.Generic.KeyValuePair<System.Object, System.Object> item)
            {
                return mRemove_14.Invoke(this.instance, item);
            }

            public System.Collections.ICollection Keys
            {
            get
            {
                return mget_Keys_2.Invoke(this.instance);

            }
            }

            public System.Collections.ICollection Values
            {
            get
            {
                return mget_Values_3.Invoke(this.instance);

            }
            }

            public System.Boolean IsReadOnly
            {
            get
            {
                return mget_IsReadOnly_5.Invoke(this.instance);

            }
            }

            public System.Boolean IsFixedSize
            {
            get
            {
                return mget_IsFixedSize_6.Invoke(this.instance);

            }
            }

            public System.Object SyncRoot
            {
            get
            {
                return mget_SyncRoot_9.Invoke(this.instance);

            }
            }

            public System.Boolean IsSynchronized
            {
            get
            {
                return mget_IsSynchronized_10.Invoke(this.instance);

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
