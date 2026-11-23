/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XBinder - Unity 高级数据绑定管理插件
 * 项目启动：2025年8月
 * 官方网站：http://sevenstrike.com/
 * 授权协议：GNU Affero General Public License Version 3 (AGPL 3.0)
 * 协议说明：
 * 1. 你可以自由使用、修改、分发本插件的源代码，但必须保留此版权注释
 * 2. 基于本插件修改后的衍生作品，必须同样遵循 AGPL 3.0 授权协议
 * 3. 若将本插件用于网络服务（如云端Unity编辑器、在线动效生成工具），必须公开修改后的完整源代码
 * 4. 完整协议文本可查阅：https://www.gnu.org/licenses/agpl-3.0.html
 * ============================================================================
 * 违反本注释保留要求，将违反 AGPL 3.0 授权协议，需承担相应法律责任
 */
namespace SevenStrikeModules.XBinder.Example
{
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Networking;

    public class web_json_loader_get : MonoBehaviour
    {
        // 接口地址：不再是 .json 文件路径，而是你的 POST 接口
        public string r_url = "http://192.168.1.8/";
        public string r_name;

        void Start()
        {
            Request();
        }

        public void Request()
        {
            StartCoroutine(DownloadFile($"{r_url}{r_name}"));
        }

        IEnumerator DownloadFile(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // 发送请求并等待返回
                yield return request.SendWebRequest();

                // 检查网络错误和 HTTP 错误
                if (request.result == UnityWebRequest.Result.Success)
                {
                    XGUI_Utilitys.Console("XBinder - JsonLoader 网络请求消息", $"请求成功！文件内容：\n{request.downloadHandler.text}", XGUIMsgState.确认);

                    // 拿到原始文本
                    string jsonText = request.downloadHandler.text;
                    XGUI_Utilitys.Console("XBinder - JsonLoader 网络请求消息", $"请求完成 Raw :\n{jsonText}", XGUIMsgState.通知);

                    string normalizedJson = NormalizeJson(jsonText);
                    XGUI_Utilitys.Console("XBinder - JsonLoader 网络请求消息", $"请求完成 Fomat :\n{normalizedJson}", XGUIMsgState.通知);
                }
                else
                {
                    Debug.LogError($"获取失败: {request.error}");
                }
            }
        }

        /// <summary>
        /// 通用规范化：若 JSON 的 "data" 字段是字符串（内含 JSON），则解包替换为真正 JSON。
        /// 支持 data 内是对象 {} 或数组 [] 两种情况。
        /// </summary>
        private string NormalizeJson(string rawJson)
        {
            int dataKeyIdx = rawJson.IndexOf("\"data\"", StringComparison.Ordinal);
            if (dataKeyIdx < 0) return rawJson;

            int colonIdx = rawJson.IndexOf(':', dataKeyIdx);
            if (colonIdx < 0) return rawJson;

            int valueStart = colonIdx + 1;
            while (valueStart < rawJson.Length && char.IsWhiteSpace(rawJson[valueStart]))
                valueStart++;

            // data 的值不是字符串（已经是对象/数组/null），无需处理
            if (valueStart >= rawJson.Length || rawJson[valueStart] != '"')
                return rawJson;

            // 找到字符串结束引号（处理转义）
            int valueEnd = valueStart + 1;
            while (valueEnd < rawJson.Length)
            {
                if (rawJson[valueEnd] == '\\') { valueEnd += 2; continue; }
                if (rawJson[valueEnd] == '"') break;
                valueEnd++;
            }
            if (valueEnd >= rawJson.Length) return rawJson;

            // 取出带引号的字符串内容（不含首尾引号）
            string escaped = rawJson.Substring(valueStart + 1, valueEnd - valueStart - 1);

            // 反转义
            string innerJson = escaped
                .Replace("\\\"", "\"")
                .Replace("\\/", "/")
                .Replace("\\\\", "\\")
                .Replace("\\n", "\n")
                .Replace("\\r", "\r")
                .Replace("\\t", "\t");

            // 拼回：before + innerJson + after
            string before = rawJson.Substring(0, valueStart);
            string after = rawJson.Substring(valueEnd + 1);
            return before + innerJson + after;
        }
    }
}