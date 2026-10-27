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
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.UI;

    public partial class Editor_XHud_Manager : Editor
    {
        #region 绘制 - RMS
        /// <summary>
        /// 绘制「RMS」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>列表为空时显示说明标签。</description></item>
        /// <item><description>绘制可重排的分辨率节点列表。</description></item>
        /// <item><description>提供「清空列表」按钮。</description></item>
        /// </list>
        /// </summary>
        private void Draw_RMS()
        {
            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            #region RMS说明
            if (sp_rms_nodes.arraySize <= 0)
            {
                string msg = $"如果开发的应用布局元素需要<color={hexcol}>同时针对多个分辨率</color>进行匹配和定位，那么请在此添加目标分辨率";

                XGUI.layout_label(
                    text: msg,
                    bg_fill: XGUIGroup.实体,
                    bg_color: XGUIColor.深空灰,
                    size: XGUIFontSize.S,
                    anchor: TextAnchor.MiddleLeft,
                    text_color: Color.white * 0.85f,
                    offset: new Vector2(0, 0),
                    padding: new RectOffset(10, 10, 10, 10),
                    margin: new RectOffset(0, 0, 6, 0),
                    clipping: TextClipping.Clip,
                    wrap: true,
                    font: XGUI.GetFont("xg-regular"),
                    font_style: FontStyle.Normal);
            }
            #endregion

            XGUI.layout_space(5);

            if (Application.isPlaying)
                XGUI.SetEnabled(false);

            #region 分辨率列表
            list_rms.DoLayoutList();
            #endregion

            XGUI.layout_space(5);

            #region 清空列表
            if (XGUI.layout_button(
                text: "清空列表",
                tooltip: "清空分辨率列表",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_color_gui: Color.white,
                button_text_color: color_important,
                press_fill: XGUIGroup.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.gray,
                font_size: XGUIFontSize.M,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                height: 25,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 管理器消息",
                    title: "RMS列表",
                    msg: "如果清空该列表可能会导致元素匹配分辨率数据异常！请确定是否继续该操作？",
                    ok: "清空",
                    cancel: "暂不",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    on_selected: (d) =>
                    {
                        if (d == "清空")
                        {
                            sp_rms_nodes.ClearArray();
                            sp_rms_nodes.serializedObject.ApplyModifiedProperties();
                        }
                    });
                return;
            }
            #endregion

            XGUI.SetEnabled(true);
        }
        #endregion

        #region 绘制 - RMS 列表
        /// <summary>
        /// RMS 列表 - 初始化。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>构造 <c>List_RMS</c> 并绑定全部具名回调。</description></item>
        /// </list>
        /// </summary>
        private void Init_RMSList()
        {
            list_rms = new ReorderableList(serializedObject, sp_rms_nodes)
            {
                draggable = false,
                displayAdd = !Application.isPlaying,
                displayRemove = !Application.isPlaying,
                drawHeaderCallback = RMS_DrawHeader,
                drawElementCallback = RMS_DrawElement,
                elementHeightCallback = RMS_ElementHeight,
                onAddCallback = RMS_OnAdd,
                onRemoveCallback = RMS_OnRemove,
                onSelectCallback = RMS_OnSelect
            };
        }
        /// <summary>
        /// RMS 列表 - 绘制表头。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在列表顶部绘制「分辨率列表」标签。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rect">绘制区域。</param>
        private void RMS_DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "分辨率列表");
        }
        /// <summary>
        /// RMS 列表 - 绘制元素。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制分辨率标识与分辨率值两个字段。</description></item>
        /// <item><description>绘制与当前分辨率是否匹配的状态图标。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rect">绘制区域。</param>
        /// <param name="index">元素索引。</param>
        /// <param name="isActive">是否激活。</param>
        /// <param name="isFocused">是否聚焦。</param>
        private void RMS_DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty sp_node = sp_rms_nodes.GetArrayElementAtIndex(index);
            SerializedProperty sp_node_indicator = sp_node.FindPropertyRelative("Indicator");
            SerializedProperty sp_node_res = sp_node.FindPropertyRelative("Res");
            if (sp_node != null)
            {
                if (XGUI.GetCurrentWindowWidth() > 120)
                {
                    XGUI.gui_property_field(
                    rect: new Rect(rect.x + 30, rect.y + 3, (XGUI.GetCurrentWindowWidth() > 180 ? (rect.width / 2) : rect.width) - 40, 20),
                    title: null,
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 80,
                    prop: sp_node_indicator);
                }

                if (XGUI.GetCurrentWindowWidth() > 180)
                {
                    XGUI.gui_property_field(
                    rect: new Rect(rect.x + (rect.width - (rect.width / 2)), rect.y + 3, (rect.width / 2) - 5, 20),
                    title: null,
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 80,
                    prop: sp_node_res);
                }

                XGUI.gui_icon(
                    rect: new Rect(rect.x + 5, rect.y + 3, 14, 14),
                    icon: rmsicon,
                    color: sp_cv_screen_res.vector2Value != sp_node_res.vector2Value ? Color.gray : XHud_Dashboard.themes_get_color(ThemeColorType.primary));
            }
        }
        /// <summary>
        /// RMS 列表 - 元素高度。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>返回 1.5 倍行高。</description></item>
        /// </list>
        /// </summary>
        /// <param name="index">元素索引。</param>
        /// <returns>元素高度。</returns>
        private float RMS_ElementHeight(int index)
        {
            return 1.5f * line_height;
        }
        /// <summary>
        /// RMS 列表 - 添加回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在列表末尾插入一个空元素。</description></item>
        /// </list>
        /// </summary>
        /// <param name="list">列表实例。</param>
        private void RMS_OnAdd(ReorderableList list)
        {
            sp_rms_nodes.InsertArrayElementAtIndex(list.count);
        }
        /// <summary>
        /// RMS 列表 - 移除回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>删除当前选中索引处的元素。</description></item>
        /// </list>
        /// </summary>
        /// <param name="list">列表实例。</param>
        private void RMS_OnRemove(ReorderableList list)
        {
            sp_rms_nodes.DeleteArrayElementAtIndex(list.index);
        }
        /// <summary>
        /// RMS 列表 - 选中回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>同步当前分辨率标识并切换分辨率。</description></item>
        /// </list>
        /// </summary>
        /// <param name="list">列表实例。</param>
        private void RMS_OnSelect(ReorderableList list)
        {
            SerializedProperty indicator = sp_rms_nodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Indicator");
            sp_rms_current_solution.stringValue = indicator.stringValue;
            xHud_SwitchResolution(indicator.stringValue);
        }
        #endregion

        #region 工具 - 分辨率
        /// <summary>
        /// 刷新分辨率。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>将 <c>ScreenRes</c> 同步到 CanvasScaler 与 <c>CanvasScalerScreenSize</c>。</description></item>
        /// </list>
        /// </summary>
        private void RefreshResolution()
        {
            CanvasScaler scaler = (CanvasScaler)sp_cv_scaler.objectReferenceValue;
            scaler.referenceResolution = sp_cv_screen_res.vector2Value;
            sp_sf_canvas_scaler_screen_size.vector2Value = sp_cv_screen_res.vector2Value;
        }
        /// <summary>
        /// 切换分辨率。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>按标识在 <c>RMS_Nodes</c> 中查找对应分辨率并应用。</description></item>
        /// <item><description>尝试在 GameView 中切换到匹配的尺寸。</description></item>
        /// </list>
        /// </summary>
        /// <param name="indicator">分辨率标识。</param>
        public void xHud_SwitchResolution(string indicator)
        {
            for (int i = 0; i < sp_rms_nodes.arraySize; i++)
            {
                SerializedProperty resroot = sp_rms_nodes.GetArrayElementAtIndex(i);
                SerializedProperty x_res = resroot.FindPropertyRelative("Res");
                SerializedProperty x_indicator = resroot.FindPropertyRelative("Indicator");

                if (indicator == x_indicator.stringValue)
                {
                    sp_cv_screen_res.vector2Value = x_res.vector2Value;

                    RefreshResolution();
                    // 设置编辑器的分辨率
                    if (SizeExists(GameViewSizeGroupType.Standalone, (int)sp_cv_screen_res.vector2Value.x, (int)sp_cv_screen_res.vector2Value.y))
                    {
                        SetSize(FindSize(GameViewSizeGroupType.Standalone, (int)sp_cv_screen_res.vector2Value.x, (int)sp_cv_screen_res.vector2Value.y));
                    }
                    else
                    {
                        Debug.Log("不存在目标分辨率项！");
                    }
                }
                xHud_EditorUpdate_HelperVisual();
            }
        }
        #endregion
    }
}