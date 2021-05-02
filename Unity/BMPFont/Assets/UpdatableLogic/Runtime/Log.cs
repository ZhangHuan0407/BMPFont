using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Encoder
{
    /// <summary>
    /// 发送日志到阿里云，大约7800条1元
    /// </summary>
    public static class Log
    {
        /* const */
#if UNITY_EDITOR
#pragma warning disable CS0649
        private static bool WriteLogOnly { get; set; }
#pragma warning restore CS0649
#endif

        /* field */
        private static StringBuilder m_Builder;
        /// <summary>
        /// 日志服务前缀地址
        /// </summary>
        public static string UriBase = "http://encoder-test.cn-zhangjiakou.log.aliyuncs.com/logstores/encoder-test/track?APIVersion=0.6.0";
        /// <summary>
        /// 针对普通日志的附加参数委托
        /// </summary>
        public static Action<Dictionary<string, object>> AppendLogParameter;
        /// <summary>
        /// 针对报错日志的附加参数委托
        /// </summary>
        public static Action<Dictionary<string, object>> AppendErrorParameter;
        /// <summary>
        /// 普通日志在此处排队，如果触发异常或日志发送，立即被先行发送
        /// </summary>
        public static readonly Queue<string> LogQueue = new Queue<string>();

        /* func */
        /// <summary>
        /// 发送报错日志
        /// </summary>
        /// <param name="message">期望发送的数据信息</param>
        /// <returns>发送任务</returns>
        public static Task SendError(Dictionary<string, object> message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (!message.ContainsKey(nameof(Exception)))
                message.Add(nameof(Exception), "Undefined Exception.");
            AppendErrorParameter?.Invoke(message);
            StringBuilder builder = m_Builder ?? new StringBuilder();
            m_Builder = null;
            builder.Append(UriBase);
            foreach (KeyValuePair<string, object> pair in message)
                builder.Append($"&{pair.Key}={pair.Value}");

            Task result;
            string content = builder.ToString();
#if UNITY_EDITOR
            if (WriteLogOnly)
            {
                result = Task.Delay(100);
            }
            else
#endif
            {
                LogQueueDequeue();
                WebRequest request = WebRequest.Create(content);
                result = request.GetResponseAsync();
            }

            builder.Length = 0;
            m_Builder = builder;
            return result;
        }

        /// <summary>
        /// 发送普通日志
        /// <para>请不要直接调用此函数，而是使用 Unity.Log </para>
        /// </summary>
        /// <param name="message">期望发送的数据信息</param>
        /// <returns>发送任务</returns>
        public static void SendMessage(Dictionary<string, object> message, bool appendParameter = true)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (appendParameter)
                AppendLogParameter?.Invoke(message);
            StringBuilder builder = m_Builder ?? new StringBuilder();
            m_Builder = null;
            builder.Append(UriBase);
            foreach (KeyValuePair<string, object> pair in message)
                builder.Append($"&{pair.Key}={pair.Value}");

            string content = builder.ToString();
            LogQueue.Enqueue(content);
            while (LogQueue.Count > 20)
                LogQueue.Dequeue();

            builder.Length = 0;
            m_Builder = builder;
        }
        internal static void SendMessage_Internal(Dictionary<string, object> message, bool appendParameter)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (appendParameter)
                AppendLogParameter?.Invoke(message);
            StringBuilder builder = m_Builder ?? new StringBuilder();
            m_Builder = null;
            builder.Append(UriBase);
            foreach (KeyValuePair<string, object> pair in message)
                builder.Append($"&{pair.Key}={pair.Value}");

            string content = builder.ToString();
#if UNITY_EDITOR
            if (WriteLogOnly)
                return;
#endif
            WebRequest request = WebRequest.Create(content);
            request.GetResponseAsync();

            builder.Length = 0;
            m_Builder = builder;
        }
        public static void LogQueueDequeue()
        { 
            while (LogQueue.Count > 0)
                WebRequest.Create(LogQueue.Dequeue());
        }

        internal static void ApplicationLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            Dictionary<string, object> message = new Dictionary<string, object>()
            {
                { "condition", condition },
                { "stackTrace", stackTrace },
                { "type", type },
                { "project", "BMPFont" },
                { "time", DateTime.UtcNow.ToString("dd HH:mm:ss") },
            };
            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    SendError(message);
                    break;
                case LogType.Warning:
                    SendMessage_Internal(message, true);
                    break;
                case LogType.Log:
                    SendMessage(message);
                    break;
                default:
                    SendMessage(message);
                    break;
            }
        }
    }
}