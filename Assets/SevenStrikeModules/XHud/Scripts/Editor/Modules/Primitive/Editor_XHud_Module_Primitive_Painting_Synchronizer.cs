/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XHud - Unity UGUI 高级管理架构插件
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
namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Primitive_Painting_Synchronizer))]
    public class Editor_XHud_Module_Primitive_Painting_Synchronizer : Editor
    {
        #region 字段 - 组件 / 列表
        /// <summary>
        /// 当前编辑的目标脚本实例。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中由 <c>target</c> 强制转换获得。</description></item>
        /// <item><description>仅用于单选模式下的直接读写操作。</description></item>
        /// </list>
        /// </summary>
        private XHud_Module_Primitive_Painting_Synchronizer BaseScript;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 本编辑器所使用的图标资源集合。
        /// <para/>
        /// <list type="bullet">
        /// <item><description><c>icon_main</c>：标题主图标。</description></item>
        /// <item><description><c>icon_lib</c>：色卡库图标。</description></item>
        /// <item><description><c>icon_local</c>：本地色图标。</description></item>
        /// <item><description><c>icon_connect</c>：同步连接图标。</description></item>
        /// </list>
        /// </summary>
        private Texture2D icon_main, icon_lib, icon_local, icon_connect;
        #endregion

        #region 字段 - GUI 参数
        /// <summary>
        /// 原始脚本参数显示开关。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>控制是否显示原始脚本参数区域。</description></item>
        /// </list>
        /// </summary>
        private bool OriginalDisplay;
        #endregion

        #region 字段 - 批量化操作
        /// <summary>
        /// 批量选择脚本数组。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>Targets_Get</c> 中根据 <c>targets</c> 填充。</description></item>
        /// <item><description>单选时长度为 1，多选时长度等于选中对象数量。</description></item>
        /// </list>
        /// </summary>
        Editor_XHud_Module_Primitive_Painting_Synchronizer[] SelectedObjects;
        #endregion

        #region 生命周期
        /// <summary>
        /// Unity 编辑器启用回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>初始化目标脚本引用。</description></item>
        /// <item><description>加载图标资源。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            BaseScript = (XHud_Module_Primitive_Painting_Synchronizer)target;
            #region 获取图标          
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting_synchronizer/icon_main");
            icon_lib = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting_synchronizer/icon_lib");
            icon_local = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting_synchronizer/icon_local");
            icon_connect = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting_synchronizer/icon_connect");
            #endregion
        }
        /// <summary>
        /// 绘制 Inspector 界面。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制标题横幅。</description></item>
        /// <item><description>绘制同步指示状态组。</description></item>
        /// <item><description>绘制源脚本折叠组。</description></item>
        /// </list>
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            string h_color = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary));
            TextClipping clipping = XGUI.TryEllipsisClipping();
            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_text: "XHud  -  图元  >  配色同步器",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping,
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion
            #region 同步指示状态
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "同步指示状态",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 25, 15));
            XGUI.layout_flexspace();
            XGUI.layout_icon(
                icon: icon_lib,
                icon_alignment: XGUIIconAlignment.左,
                layout_margin: new RectOffset(0, 0, 15, 15));
            XGUI.layout_space(10);
            XGUI.layout_icon(
                icon: icon_connect,
                width: 75,
                icon_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                icon_offset: new Vector2(0, -6),
                icon_alignment: XGUIIconAlignment.默认,
                layout_margin: new RectOffset(0, 0, 15, 15));
            XGUI.layout_space(10);
            XGUI.layout_icon(
                icon: icon_local,
                icon_alignment: XGUIIconAlignment.右,
                layout_margin: new RectOffset(0, 0, 15, 15));
            XGUI.layout_flexspace();
            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion
            #region 源脚本
            BaseScript.fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_based);
            if (!BaseScript.fold_based)
            {
                DrawDefaultInspector();
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 工具 - 批量化操作
        /// <summary>
        /// 获取所有批量脚本目标。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>多选时创建与 <c>targets</c> 等长的数组并逐个填充。</description></item>
        /// <item><description>单选时创建长度为 1 的数组并填充当前目标。</description></item>
        /// </list>
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new Editor_XHud_Module_Primitive_Painting_Synchronizer[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (Editor_XHud_Module_Primitive_Painting_Synchronizer)t;
                }
            }
            else
            {
                SelectedObjects = new Editor_XHud_Module_Primitive_Painting_Synchronizer[targets.Length];
                SelectedObjects[0] = (Editor_XHud_Module_Primitive_Painting_Synchronizer)target;
            }
        }
        /// <summary>
        /// 判断是否是多选状态。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>当 <c>SelectedObjects</c> 为 null 时返回 false。</description></item>
        /// <item><description>当 <c>SelectedObjects.Length > 1</c> 时返回 true。</description></item>
        /// </list>
        /// </summary>
        /// <returns>是否为多选状态。</returns>
        private bool Targets_Selected()
        {
            if (SelectedObjects == null)
                return false;
            if (SelectedObjects.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}