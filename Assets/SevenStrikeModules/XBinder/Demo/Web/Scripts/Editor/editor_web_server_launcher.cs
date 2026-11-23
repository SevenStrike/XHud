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
namespace SevenStrikeModules.XBinder.Example.Editor
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using System.Diagnostics;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public class editor_web_server_launcher : EditorWindow
    {
        // 你的 Node 项目相对路径（根据截图填写的）
        private static string nodeProjectPath = $"{XBinder_Dashboard.Get_Path_XBinder_Demo()}Web/Servers/NodeJS/Requester";
        private static string nodeEntryScript = "web_listener.js";

        [MenuItem("Tools/XBinder/网络数据源模拟服务")]
        public static void StartNodeServer()
        {
            // 将 Unity 相对路径转为绝对路径
            string absolutePath = Path.GetFullPath(nodeProjectPath);

            // 检查路径是否存在
            if (!Directory.Exists(absolutePath))
            {
                XGUI.dialog(
                    type: XGUIDialogType.错误,
                    windowtitle: "XBinder - Node 网络数据源模拟服务",
                    title: "执行错误",
                    msg: $"找不到指定的 Node 目录：\n{absolutePath} ！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: Color.white);
                return;
            }

            // 检查 js 文件是否存在
            if (!File.Exists(Path.Combine(absolutePath, nodeEntryScript)))
            {
                XGUI.dialog(
                    type: XGUIDialogType.错误,
                    windowtitle: "XBinder - Node 网络数据源模拟服务",
                    title: "执行错误",
                    msg: $"找不到入口文件：\n{Path.Combine(absolutePath, nodeEntryScript)}",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: Color.white);
                return;
            }

            // 配置 CMD 启动参数
            // /k 表示执行完命令后保留窗口不关闭，方便你看日志（如果不需要保留，可以改成 /c）
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/k node {nodeEntryScript}";
            // 确保在此目录下运行 node
            startInfo.WorkingDirectory = absolutePath;
            // 允许启动外部程序
            startInfo.UseShellExecute = true;
            // 如果你不想弹出窗口，可以取消注释这行
            //startInfo.WindowStyle = ProcessWindowStyle.Minimized; 

            try
            {
                Process.Start(startInfo);
                XGUI_Utilitys.Console("XBinder - JsonLoader 网络请求反馈", $"Node 服务已启动，路径：{absolutePath}", XGUIMsgState.确认);
            }
            catch (System.Exception e)
            {
                XGUI.dialog(
                   type: XGUIDialogType.错误,
                   windowtitle: "XBinder - Node 网络数据源模拟服务",
                   title: "启动失败",
                   msg: $"无法启动 Node 服务，请确保你已安装 Node.js。\n\n错误信息：{e.Message}",
                   ok: "明白",
                   PrimaryIndex: 0,
                   usemodal: true,
                   themecolor: Color.white);
            }
        }
    }
}