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
        private static string CustomComponentInfoFilePath;
        /// <summary>
        /// 自定义组件信息(Editor版本)默认输出路径
        /// </summary>
        private static string CustomComponentInfoEditorFilePath;

        /* func */
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0)
                CustomComponentInfoFilePath = args[0];
            CustomComponentInfoFilePath = CustomComponentInfoFilePath ?? "../../BMPFont/Assets/UpdatableLogic/Hotfix/CustomComponentInfo.txt";
            if (args.Length > 1)
                CustomComponentInfoEditorFilePath = args[1];
            CustomComponentInfoEditorFilePath = CustomComponentInfoEditorFilePath ?? "../../BMPFont/Assets/UpdatableLogic/Editor/CustomComponentInfo_Editor.json";

            var customComponentTypes = ExportCustomComponentFunction.GetAllCustomComponentTypes();
            CustomComponentInfo_Editor[] editorInfos = ExportCustomComponentFunction.FromTypeToInfo(customComponentTypes);
            string editorInfo = JsonConvert.SerializeObject(editorInfos, Formatting.Indented);
            File.WriteAllText(CustomComponentInfoEditorFilePath, editorInfo);
            Console.WriteLine($"Write Custom Component Info(Editor) at {CustomComponentInfoEditorFilePath}");

            CustomComponentInfo[] runtimeInfos = ExportCustomComponentFunction.FromInfoToRuntimeInfo(editorInfos);
            StringBuilder stringBuilder = new StringBuilder().AppendLine("[");
            for (int index = 0; index < runtimeInfos.Length; index++)
            {
                CustomComponentInfo customComponentInfo = runtimeInfos[index];
                stringBuilder.Append(JsonConvert.SerializeObject(customComponentInfo, Formatting.None));
                if (index < runtimeInfos.Length - 1)
                    stringBuilder.AppendLine(",");
            }

            stringBuilder.AppendLine().AppendLine("]");
            File.WriteAllText(CustomComponentInfoFilePath, stringBuilder.ToString());
            Console.WriteLine($"Write Custom Component Info at {CustomComponentInfoFilePath}");

            if (args.Length == 0)
            {
                Console.WriteLine("Finish");
                Console.ReadLine();
            }
        }
    }
}
