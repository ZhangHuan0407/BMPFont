using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using ILRuntime.Runtime.Enviorment;
using System.IO;
using ILRuntime.Runtime.Intepreter;
using System.Collections;

namespace Encoder.Editor
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ILRuntimeCrossBinding
    {
        [MenuItem("ILRuntime/Generate Crossbind Adapter")]
        private static void GenerateCrossbindAdapter()
        {
            //由于跨域继承特殊性太多，自动生成无法实现完全无副作用生成，所以这里提供的代码自动生成主要是给大家生成个初始模版，简化大家的工作
            //大多数情况直接使用自动生成的模版即可，如果遇到问题可以手动去修改生成后的文件，因此这里需要大家自行处理是否覆盖的问题
            Directory.CreateDirectory("Assets/ILRuntime/Adapter");
            WriteOneAdapter(typeof(Exception));
            WriteOneAdapter(typeof(IEnumerable<object>));
            WriteOneAdapter(typeof(IEnumerable));
            WriteOneAdapter(typeof(IEnumerator<object>));
            WriteOneAdapter(typeof(IEnumerator));
            WriteOneAdapter(typeof(IEnumerable<ILTypeInstance>));
            WriteOneAdapter(typeof(IEnumerator<ILTypeInstance>));
            WriteOneAdapter(typeof(HashSet<object>));
            WriteOneAdapter(typeof(List<object>));
            WriteOneAdapter(typeof(Queue<object>));
            WriteOneAdapter(typeof(Stack<object>));
            WriteOneAdapter(typeof(LinkedList<object>));
            WriteOneAdapter(typeof(Dictionary<object, object>));
            AssetDatabase.Refresh();
        }

        private static StringBuilder m_StringBuilder;
        private static void WriteOneAdapter(Type type)
        {
            m_StringBuilder = m_StringBuilder?.Clear() ?? new StringBuilder();
            m_StringBuilder.Append($"Assets/ILRuntime/Adapter/{type.Name}");
            foreach (Type genericType in type.GetGenericArguments())
                m_StringBuilder.Append($"_{genericType.Name}");
            m_StringBuilder.Append("Adapter.cs");
            string filePath = m_StringBuilder.ToString();
            if (File.Exists(filePath))
                return;
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("#pragma warning disable CS0108, CS0649");
                sw.WriteLine(CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(type, "Encoder.ILAdapter"));
                sw.WriteLine("#pragma warning restore CS0108, CS0649");
            }
        }
    }
}