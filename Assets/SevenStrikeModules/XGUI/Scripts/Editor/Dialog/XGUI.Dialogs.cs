/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGUI - Unity Editor界面可视化组件工具
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
namespace SevenStrikeModules.XGUI.Editor
{
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 表示对话框回传的数据集合
    /// </summary>
    /// <remarks>
    /// 该类型用于 <see cref="dialog_submit"/> 方法的回调，包含用户输入的名称和描述信息以及状态
    /// </remarks>
    public class dialog_listdata
    {
        /// <summary>
        /// 获取或设置输入的名称。
        /// </summary>
        public string name;

        /// <summary>
        /// 获取或设置输入的描述信息。
        /// </summary>
        public string description;

        /// <summary>
        /// 获取或设置对话框的操作状态（如 "ok"、"cancel" 等）。
        /// </summary>
        public string state;
    }

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// 显示一个标准的模态或非模态对话框
        /// </summary>
        /// <param name="type">对话框的类型（通知、警告、错误），影响图标显示。</param>
        /// <param name="windowtitle">对话框窗口的标题栏文本。</param>
        /// <param name="title">对话框正文中的标题文本。</param>
        /// <param name="msg">对话框的正文消息内容。</param>
        /// <param name="ok">确认按钮的文本。如果为 <c>null</c> 或空，则不显示该按钮。</param>
        /// <param name="cancel">取消按钮的文本。如果为 <c>null</c> 或空，则不显示该按钮。</param>
        /// <param name="alt">备选按钮的文本。如果为 <c>null</c> 或空，则不显示该按钮。</param>
        /// <param name="other">其他按钮的文本。如果为 <c>null</c> 或空，则不显示该按钮。</param>
        /// <param name="special">特殊按钮的文本。如果为 <c>null</c> 或空，则不显示该按钮。</param>
        /// <param name="PrimaryIndex">指定默认高亮按钮的索引（0 表示第一个按钮）。</param>
        /// <param name="usemodal">是否使用模态窗口。如果为 <c>true</c>，对话框将阻止用户与编辑器其他部分交互。</param>
        /// <param name="themecolor">对话框的主题色。传入 <see cref="Color.clear"/> 时使用默认白色主题。</param>
        /// <param name="on_selected">用户点击任意按钮时的回调委托，参数为按钮对应的文本标识。</param>
        /// <returns>用户点击的按钮对应的文本标识。通常为 <paramref name="ok"/>、<paramref name="cancel"/> 等传入值。</returns>
        /// <example>
        /// <code>
        /// string result = XGUI.dialog(
        ///     type: XGUIDialogType.警告,
        ///     windowtitle: "确认删除",
        ///     title: "删除文件",
        ///     msg: "确定要删除选中的文件吗？此操作不可恢复！",
        ///     ok: "确定删除",
        ///     cancel: "取消",
        ///     usemodal: true,
        ///     on_selected: (r) => Debug.Log($"用户选择了: {r}")
        /// );
        /// if (result == "确定删除")
        /// {
        ///     // 执行删除逻辑
        /// }
        /// </code>
        /// </example>
        public static string dialog(XGUIDialogType type = XGUIDialogType.通知, string windowtitle = null, string title = null, string msg = null, string ok = null, string cancel = null, string alt = null, string other = null, string special = null, int PrimaryIndex = 0, bool usemodal = true, Color themecolor = default, Action<string> on_selected = null)
        {
            if (themecolor == Color.clear)
                themecolor = Color.white;

            string res = "";
            DialogWindow window = EditorWindow.GetWindow<DialogWindow>(true);
            window.titleContent = new GUIContent(windowtitle);

            if (!string.IsNullOrEmpty(ok))
                window.XGUIDialogButtonMode = XGUIDialogButtonMode.单个按钮;
            if (!string.IsNullOrEmpty(ok) && !string.IsNullOrEmpty(cancel))
                window.XGUIDialogButtonMode = XGUIDialogButtonMode.两个按钮;
            if (!string.IsNullOrEmpty(ok) && !string.IsNullOrEmpty(cancel) && !string.IsNullOrEmpty(alt))
                window.XGUIDialogButtonMode = XGUIDialogButtonMode.三个按钮;
            if (!string.IsNullOrEmpty(ok) && !string.IsNullOrEmpty(cancel) && !string.IsNullOrEmpty(alt) && !string.IsNullOrEmpty(other))
                window.XGUIDialogButtonMode = XGUIDialogButtonMode.四个按钮;
            if (!string.IsNullOrEmpty(ok) && !string.IsNullOrEmpty(cancel) && !string.IsNullOrEmpty(alt) && !string.IsNullOrEmpty(other) && !string.IsNullOrEmpty(special))
                window.XGUIDialogButtonMode = XGUIDialogButtonMode.五个按钮;

            XGUI.CenterEditorWindow(new Vector2Int(545, 235), window);

            // 回调消息接收
            window.Callback_Ok = (r) =>
            {
                res = r;
                if (on_selected != null)
                    on_selected(res);
            };
            window.Callback_Cancel = (r) =>
            {
                res = r;
                if (on_selected != null)
                    on_selected(res);
            };
            window.Callback_Alt = (r) =>
            {
                res = r;
                if (on_selected != null)
                    on_selected(res);
            };
            window.Callback_Other = (r) =>
            {
                res = r;
                if (on_selected != null)
                    on_selected(res);
            };
            window.Callback_Special = (r) =>
            {
                res = r;
                if (on_selected != null)
                    on_selected(res);
            };
            window.SetInfo(type, title, msg, ok, cancel, alt, other, special, "", "", PrimaryIndex);
            window.SetThemeColor(themecolor);

            if (usemodal)
                // 显示模态窗口弹窗
                window.ShowModal();
            else
                window.Show();

            // 反馈选择消息
            return res;
        }
        /// <summary>
        /// 显示一个包含名称和描述输入框的提交对话框
        /// </summary>
        /// <param name="type">对话框的类型（通知、警告、错误），影响图标显示。</param>
        /// <param name="windowtitle">对话框窗口的标题栏文本。</param>
        /// <param name="title">对话框正文中的标题文本。</param>
        /// <param name="msg">对话框的正文消息内容。</param>
        /// <param name="ok">确认按钮的文本。</param>
        /// <param name="cancel">取消按钮的文本。</param>
        /// <param name="PrimaryIndex">指定默认高亮按钮的索引（0 表示第一个按钮）。</param>
        /// <param name="usemodal">是否使用模态窗口。如果为 <c>true</c>，对话框将阻止用户与编辑器其他部分交互。</param>
        /// <param name="themecolor">对话框的主题色。传入 <see cref="Color.clear"/> 时使用默认白色主题。</param>
        /// <param name="arg_name_text">名称输入框的占位提示文本或标签。</param>
        /// <param name="arg_data_text">描述输入框的占位提示文本或标签。</param>
        /// <param name="on_selected">用户提交或取消时的回调委托，参数为包含用户输入数据的 <see cref="dialog_listdata"/> 对象。</param>
        /// <returns>包含用户输入的名称、描述和操作状态的 <see cref="dialog_listdata"/> 对象。</returns>
        /// <remarks>
        /// 该对话框提供两个文本输入字段，适合需要用户输入名称和描述信息的场景。
        /// 回调中的 <see cref="dialog_listdata.state"/> 属性表示用户操作类型（"ok" 表示提交，"cancel" 表示取消）
        /// </remarks>
        /// <example>
        /// <code>
        /// var data = XGUI.dialog_submit(
        ///     type: XGUIDialogType.通知,
        ///     windowtitle: "创建新项目",
        ///     title: "输入项目信息",
        ///     msg: "请填写新项目的名称和描述",
        ///     ok: "创建",
        ///     cancel: "取消",
        ///     arg_name_text: "项目名称",
        ///     arg_data_text: "项目描述",
        ///     on_selected: (d) => {
        ///         if (d.state == "ok")
        ///         {
        ///             Debug.Log($"创建项目: {d.name}, 描述: {d.description}");
        ///         }
        ///     }
        /// );
        /// </code>
        /// </example>
        public static dialog_listdata dialog_submit(XGUIDialogType type, string windowtitle, string title, string msg, string ok, string cancel, int PrimaryIndex = 0, bool usemodal = true, Color themecolor = default, string arg_name_text = null, string arg_data_text = null, Action<dialog_listdata> on_selected = null)
        {
            if (themecolor == Color.clear)
                themecolor = Color.white;

            dialog_listdata res = new dialog_listdata();
            DialogWindow window = EditorWindow.GetWindow<DialogWindow>(true);
            window.titleContent = new GUIContent(windowtitle);

            window.XGUIDialogButtonMode = XGUIDialogButtonMode.参数提交;

            XGUI.CenterEditorWindow(new Vector2Int(545, 430), window);

            // 回调消息接收
            window.Callback_Submit = (r, d, s) =>
            {
                res.state = s;
                res.name = r;
                res.description = d;
                if (on_selected != null)
                    on_selected(res);
            };
            window.Callback_Cancel = (r) =>
            {
                res.state = r;

                if (on_selected != null)
                    on_selected(res);
            };
            window.SetInfo(type, title, msg, ok, cancel, "", "", "", arg_name_text, arg_data_text, PrimaryIndex);
            window.SetThemeColor(themecolor);

            if (usemodal)
                // 显示模态窗口弹窗
                window.ShowModal();
            else
                window.Show();

            // 反馈选择消息
            return res;
        }
        /// <summary>
        /// 显示一个包含列表视图的对话框，允许用户从选项列表中选择
        /// </summary>
        /// <param name="datas">要在列表中显示的选项数据数组，每个选项包含显示文本和值。</param>
        /// <param name="type">对话框的类型（通知、警告、错误），影响图标显示。</param>
        /// <param name="windowtitle">对话框窗口的标题栏文本。</param>
        /// <param name="title">对话框正文中的标题文本。</param>
        /// <param name="msg">对话框的正文消息内容。</param>
        /// <param name="ok">确认按钮的文本。如果为 <c>null</c> 或空，则不显示该按钮。</param>
        /// <param name="cancel">取消按钮的文本。如果为 <c>null</c> 或空，则不显示该按钮。</param>
        /// <param name="alt">备选按钮的文本。如果为 <c>null</c> 或空，则不显示该按钮。</param>
        /// <param name="other">其他按钮的文本。如果为 <c>null</c> 或空，则不显示该按钮。</param>
        /// <param name="special">特殊按钮的文本。如果为 <c>null</c> 或空，则不显示该按钮。</param>
        /// <param name="PrimaryIndex">指定默认高亮按钮的索引（0 表示第一个按钮）。</param>
        /// <param name="usemodal">是否使用模态窗口。如果为 <c>true</c>，对话框将阻止用户与编辑器其他部分交互。</param>
        /// <param name="themecolor">对话框的主题色。传入 <see cref="Color.clear"/> 时使用默认白色主题。</param>
        /// <param name="on_selected">用户点击任意按钮或选择列表项时的回调委托，参数为选中的值或按钮文本标识。</param>
        /// <returns>用户选择的值或点击的按钮对应的文本标识。</returns>
        /// <remarks>
        /// 该对话框在消息区域下方显示一个可滚动的选项列表，用户可以通过点击选择列表中的项
        /// 当用户点击任意按钮时，选中的列表项值会作为结果返回。
        /// </remarks>
        /// <example>
        /// <code>
        /// var options = new XGUIDialogListDatas[]
        /// {
        ///     new XGUIDialogListDatas { value = "option1", display = "选项一" },
        ///     new XGUIDialogListDatas { value = "option2", display = "选项二" },
        ///     new XGUIDialogListDatas { value = "option3", display = "选项三" }
        /// };
        /// string selected = XGUI.dialog_listview(
        ///     datas: options,
        ///     type: XGUIDialogType.通知,
        ///     windowtitle: "选择目标",
        ///     title: "请选择一个选项",
        ///     msg: "从下方列表中选择一个项目",
        ///     ok: "确认",
        ///     cancel: "取消",
        ///     usemodal: true,
        ///     on_selected: (r) => Debug.Log($"用户选择了: {r}")
        /// );
        /// </code>
        /// </example>
        public static string dialog_listview(XGUIDialogListDatas[] datas, XGUIDialogType type = XGUIDialogType.通知, string windowtitle = null, string title = null, string msg = null, string ok = null, string cancel = null, string alt = null, string other = null, string special = null, int PrimaryIndex = 0, bool usemodal = true, Color themecolor = default, Action<string> on_selected = null, bool show_index = true)
        {
            if (themecolor == Color.clear)
                themecolor = Color.white;

            string res = "";
            DialogWindow window = EditorWindow.GetWindow<DialogWindow>(true);
            window.titleContent = new GUIContent(windowtitle);
            if (!string.IsNullOrEmpty(ok))
                window.XGUIDialogButtonMode = XGUIDialogButtonMode.单个按钮;
            if (!string.IsNullOrEmpty(ok) && !string.IsNullOrEmpty(cancel))
                window.XGUIDialogButtonMode = XGUIDialogButtonMode.两个按钮;
            if (!string.IsNullOrEmpty(ok) && !string.IsNullOrEmpty(cancel) && !string.IsNullOrEmpty(alt))
                window.XGUIDialogButtonMode = XGUIDialogButtonMode.三个按钮;
            if (!string.IsNullOrEmpty(ok) && !string.IsNullOrEmpty(cancel) && !string.IsNullOrEmpty(alt) && !string.IsNullOrEmpty(other))
                window.XGUIDialogButtonMode = XGUIDialogButtonMode.四个按钮;
            if (!string.IsNullOrEmpty(ok) && !string.IsNullOrEmpty(cancel) && !string.IsNullOrEmpty(alt) && !string.IsNullOrEmpty(other) && !string.IsNullOrEmpty(special))
                window.XGUIDialogButtonMode = XGUIDialogButtonMode.五个按钮;

            XGUI.CenterEditorWindow(new Vector2Int(545, 420), window);

            // 回调消息接收
            window.Callback_Ok = (r) =>
            {
                res = r;
                if (on_selected != null)
                    on_selected(res);
            };
            window.Callback_Cancel = (r) =>
            {
                res = r;
                if (on_selected != null)
                    on_selected(res);
            };
            window.Callback_Alt = (r) =>
            {
                res = r;
                if (on_selected != null)
                    on_selected(res);
            };
            window.Callback_Other = (r) =>
            {
                res = r;
                if (on_selected != null)
                    on_selected(res);
            };
            window.Callback_Special = (r) =>
            {
                res = r;
                if (on_selected != null)
                    on_selected(res);
            };
            window.SetInfo(type, title, msg, ok, cancel, alt, other, special, "", "", PrimaryIndex);
            window.SetList(datas, show_index);
            window.SetThemeColor(themecolor);

            if (usemodal)
                // 显示模态窗口弹窗
                window.ShowModal();
            else
                window.Show();

            // 反馈选择消息
            return res;
        }
    }
}