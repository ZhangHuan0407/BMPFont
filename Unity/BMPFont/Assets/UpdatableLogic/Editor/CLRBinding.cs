using Encoder.ILAdapter;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 防止此命名空间不存在
namespace Encoder.ILAdapter { }
namespace Encoder.Editor
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public static class CLRBinding
    {
        // 不知道为什么一直报错，在外面的项目直至打表后粘贴进来
        [MenuItem("ILRuntime/Generate CLR Binding Code by Analysis")]
        private static void GenerateCLRBindingByAnalysis()
        {
            //用新的分析热更dll调用引用来生成绑定代码
            ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
            using (System.IO.FileStream fs = new System.IO.FileStream("Assets/UpdatableLogic/HotFix/HotFix.dll.bytes", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                domain.LoadAssembly(fs);

                //Crossbind Adapter is needed to generate the correct binding code
                ILRuntimeService.InitILRuntime_Coroutine(domain).GetEnumerator().MoveNext();
                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(
                    domain, "Assets/ILRuntime/Generated");
            }

            AssetDatabase.Refresh();
        }
    }
}