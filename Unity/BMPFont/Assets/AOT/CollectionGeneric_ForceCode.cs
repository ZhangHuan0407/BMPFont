using AssetBundleUpdate;
using System;
using System.Collections.Generic;
using Tween;
using UnityEngine;

namespace Encoder.AOT
{
    public static class CollectionGeneric_ForceCode
    {
        public static readonly Type[] Types = new Type[]
        {
            typeof(List<Exception>),
            typeof(HashSet<Exception>),
            typeof(Dictionary<object, Exception>),
            typeof(KeyValuePair<object, Exception>),
            typeof(LinkedList<Exception>),
            typeof(LinkedListNode<Exception>),
            typeof(IEnumerable<Exception>),
            typeof(Queue<Exception>),
            typeof(Stack<Exception>),
            typeof(Exception[]),

            typeof(List<AssetBundleInfo>),
            typeof(HashSet<AssetBundleInfo>),
            typeof(Dictionary<object, AssetBundleInfo>),
            typeof(KeyValuePair<object, AssetBundleInfo>),
            typeof(Dictionary<string, AssetBundleInfo>),
            typeof(LinkedList<AssetBundleInfo>),
            typeof(LinkedListNode<AssetBundleInfo>),
            typeof(IEnumerable<AssetBundleInfo>),
            typeof(Queue<AssetBundleInfo>),
            typeof(Stack<AssetBundleInfo>),
            typeof(AssetBundleInfo[]),

            typeof(List<Tweener>),
            typeof(HashSet<Tweener>),
            typeof(Dictionary<object, Tweener>),
            typeof(KeyValuePair<object, Tweener>),
            typeof(LinkedList<Tweener>),
            typeof(LinkedListNode<Tweener>),
            typeof(IEnumerable<Tweener>),
            typeof(Queue<Tweener>),
            typeof(Stack<Tweener>),
            typeof(Tweener[]),

            typeof(List<Coroutine>),
            typeof(HashSet<Coroutine>),
            typeof(Dictionary<object, Coroutine>),
            typeof(KeyValuePair<object, Coroutine>),
            typeof(LinkedList<Coroutine>),
            typeof(LinkedListNode<Coroutine>),
            typeof(IEnumerable<Coroutine>),
            typeof(Queue<Coroutine>),
            typeof(Stack<Coroutine>),
            typeof(Tweener[]),

            typeof(List<GameObject>),
            typeof(HashSet<GameObject>),
            typeof(Dictionary<object, GameObject>),
            typeof(KeyValuePair<object, GameObject>),
            typeof(LinkedList<GameObject>),
            typeof(LinkedListNode<GameObject>),
            typeof(IEnumerable<GameObject>),
            typeof(Queue<GameObject>),
            typeof(Stack<GameObject>),
            typeof(GameObject[]),

            typeof(List<Transform>),
            typeof(HashSet<Transform>),
            typeof(Dictionary<object, Transform>),
            typeof(KeyValuePair<object, Transform>),
            typeof(LinkedList<Transform>),
            typeof(LinkedListNode<Transform>),
            typeof(IEnumerable<Transform>),
            typeof(Queue<Transform>),
            typeof(Stack<Transform>),
            typeof(Transform[]),

            typeof(List<MonoBehaviour>),
            typeof(HashSet<MonoBehaviour>),
            typeof(Dictionary<object, MonoBehaviour>),
            typeof(KeyValuePair<object, MonoBehaviour>),
            typeof(LinkedList<MonoBehaviour>),
            typeof(LinkedListNode<MonoBehaviour>),
            typeof(IEnumerable<MonoBehaviour>),
            typeof(Queue<MonoBehaviour>),
            typeof(Stack<MonoBehaviour>),
            typeof(MonoBehaviour[]),

            typeof(List<Component>),
            typeof(HashSet<Component>),
            typeof(Dictionary<object, Component>),
            typeof(KeyValuePair<object, Component>),
            typeof(LinkedList<Component>),
            typeof(LinkedListNode<Component>),
            typeof(IEnumerable<Component>),
            typeof(Queue<Component>),
            typeof(Stack<Component>),
            typeof(Component[]),
        };
    }
}