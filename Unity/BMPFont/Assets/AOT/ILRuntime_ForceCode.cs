using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;

namespace Encoder.AOT
{
    public static class ILRuntime_ForceCode
    {
        public static readonly Type[] Types = new Type[]
        {
            typeof(List<IMethod>),
            typeof(HashSet<IMethod>),
            typeof(Dictionary<object, IMethod>),
            typeof(KeyValuePair<object, IMethod>),
            typeof(LinkedList<IMethod>),
            typeof(LinkedListNode<IMethod>),
            typeof(IEnumerable<IMethod>),
            typeof(Queue<IMethod>),
            typeof(Stack<IMethod>),
            typeof(IMethod[]),

            typeof(List<IType>),
            typeof(HashSet<IType>),
            typeof(Dictionary<object, IType>),
            typeof(KeyValuePair<object, IType>),
            typeof(LinkedList<IType>),
            typeof(LinkedListNode<IType>),
            typeof(IEnumerable<IType>),
            typeof(Queue<IType>),
            typeof(Stack<IType>),
            typeof(IType[]),

            typeof(List<ILType>),
            typeof(HashSet<ILType>),
            typeof(Dictionary<object, ILType>),
            typeof(KeyValuePair<object, ILType>),
            typeof(LinkedList<ILType>),
            typeof(LinkedListNode<ILType>),
            typeof(IEnumerable<ILType>),
            typeof(Queue<ILType>),
            typeof(Stack<ILType>),
            typeof(ILType[]),

            typeof(List<ILTypeInstance>),
            typeof(HashSet<ILTypeInstance>),
            typeof(Dictionary<object, ILTypeInstance>),
            typeof(KeyValuePair<object, ILTypeInstance>),
            typeof(LinkedList<ILTypeInstance>),
            typeof(LinkedListNode<ILTypeInstance>),
            typeof(IEnumerable<ILTypeInstance>),
            typeof(Queue<ILTypeInstance>),
            typeof(Stack<ILTypeInstance>),
            typeof(ILTypeInstance[]),
        };
    }
}