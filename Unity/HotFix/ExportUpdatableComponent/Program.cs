using System;
using System.IO;
using System.Text;
using Encoder;
using Newtonsoft.Json;

namespace ExportUpdatableComponent
{
    internal class Program
    {
        /* field */
        /// <summary>
        /// 自定义组件信息默认输出路径
        /// </summary>
        private static string UpdatableComponentsInfoFilePath;
        /// <summary>
        /// 自定义组件信息(Editor版本)默认输出路径
        /// </summary>
        private static string UpdatableComponentsInfoEditorFilePath;

        /* func */
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0)
                UpdatableComponentsInfoFilePath = args[0];
            UpdatableComponentsInfoFilePath = UpdatableComponentsInfoFilePath ?? "../../BMPFont/Assets/UpdatableLogic/Hotfix/UpdatableComponentInfo.txt";
            if (args.Length > 1)
                UpdatableComponentsInfoEditorFilePath = args[1];
            UpdatableComponentsInfoEditorFilePath = UpdatableComponentsInfoEditorFilePath ?? "../../BMPFont/Assets/UpdatableLogic/Editor/UpdatableComponentInfo_Editor.json";

            var customComponentTypes = ExportUpdatableComponentFunction.GetAllUpdatableComponentTypes();
            UpdatableComponentInfo_Editor[] editorInfos = ExportUpdatableComponentFunction.FromTypeToInfo(customComponentTypes);
            string editorInfo = JsonConvert.SerializeObject(editorInfos, Formatting.Indented);
            File.WriteAllText(UpdatableComponentsInfoEditorFilePath, editorInfo);
            Console.WriteLine($"Write Updatable Component Info(Editor) at {UpdatableComponentsInfoEditorFilePath}");

            UpdatableComponentInfo[] runtimeInfos = ExportUpdatableComponentFunction.FromInfoToRuntimeInfo(editorInfos);
            StringBuilder stringBuilder = new StringBuilder().AppendLine("[");
            for (int index = 0; index < runtimeInfos.Length; index++)
            {
                UpdatableComponentInfo customComponentInfo = runtimeInfos[index];
                stringBuilder.Append(JsonConvert.SerializeObject(customComponentInfo, Formatting.None));
                if (index < runtimeInfos.Length - 1)
                    stringBuilder.AppendLine(",");
            }

            stringBuilder.AppendLine().AppendLine("]");
            File.WriteAllText(UpdatableComponentsInfoFilePath, stringBuilder.ToString());
            Console.WriteLine($"Write Updatable Component Info at {UpdatableComponentsInfoFilePath}");

            if (args.Length == 0)
            {
                Console.WriteLine("Finish");
                Console.ReadLine();
            }
        }
    }
}
