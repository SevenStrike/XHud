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
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// 使用自动布局插入一个固定大小的空白间距。
        /// </summary>
        /// <param name="Val">间距的像素值。</param>
        /// <remarks>
        /// 该方法是对 <see cref="GUILayout.Space(float)"/> 的直接封装，用于在布局中插入固定大小的空白区域。
        /// 常用于在水平或垂直布局中控制元素之间的间隔。
        /// </remarks>
        public static void layout_space(float Val)
        {
            GUILayout.Space(Val);
        }
        /// <summary>
        /// 使用自动布局插入一个弹性空白间距，用于填充剩余空间。
        /// </summary>
        /// <remarks>
        /// 该方法是对 <see cref="GUILayout.FlexibleSpace"/> 的直接封装，用于在布局中创建可伸缩的空白区域。
        /// 在水平或垂直布局中，弹性空间会占据所有剩余可用空间，从而实现元素的对齐（如左对齐、右对齐、居中等）。
        /// <para>
        /// 通常与 <see cref="layout_group_start"/> 和 <see cref="layout_group_end"/> 配合使用，实现复杂的布局对齐效果。
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// XGUI.layout_group_start(XGUIContainerType.Horizontal);
        /// XGUI.layout_label("左对齐文本");
        /// XGUI.layout_flexspace(); // 占据中间剩余空间
        /// XGUI.layout_label("右对齐文本");
        /// XGUI.layout_group_end(XGUIContainerType.Horizontal);
        /// </code>
        /// </example>
        public static void layout_flexspace()
        {
            GUILayout.FlexibleSpace();
        }
    }
}