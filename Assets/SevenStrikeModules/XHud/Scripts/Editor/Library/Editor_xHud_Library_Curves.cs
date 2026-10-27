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
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    #region 数据模型
    /// <summary>
    /// 曲线关键帧的序列化数据。
    /// <para/>
    /// 用于将 <see cref="Keyframe"/> 序列化为 JSON，以便导入 / 导出曲线库。
    /// </summary>
    [System.Serializable]
    public class CurveFrame
    {
        /// <summary>
        /// 关键帧时间。
        /// </summary>
        public float time;
        /// <summary>
        /// 关键帧数值。
        /// </summary>
        public float value;
        /// <summary>
        /// 入切线。
        /// </summary>
        public float in_tangent;
        /// <summary>
        /// 入权重。
        /// </summary>
        public float in_weight;
        /// <summary>
        /// 出切线。
        /// </summary>
        public float out_tangent;
        /// <summary>
        /// 出权重。
        /// </summary>
        public float out_weight;
        /// <summary>
        /// 权重模式。
        /// </summary>
        public WeightedMode weighted_mode;
    }
    /// <summary>
    /// 曲线节点的序列化数据。
    /// <para/>
    /// 每条曲线包含名称与全部关键帧。
    /// </summary>
    [System.Serializable]
    public class CurveNode
    {
        /// <summary>
        /// 曲线名称。
        /// </summary>
        public string name;
        /// <summary>
        /// 曲线关键帧数组。
        /// </summary>
        public CurveFrame[] frames;
    }
    /// <summary>
    /// 曲线库的完整序列化数据结构。
    /// <para/>
    /// 序列化为 JSON 后写入磁盘，用于曲线库的导入 / 导出。
    /// </summary>
    [System.Serializable]
    public class CurveLib
    {
        /// <summary>
        /// 曲线节点列表。
        /// </summary>
        public List<CurveNode> nodes;
    }
    #endregion

    [CustomEditor(typeof(XHud_Library_Curves))]
    public class Editor_XHud_Library_Curves : Editor
    {
        #region 字段 - 组件 / 列表
        /// <summary>
        /// 目标脚本实例。
        /// <para/>
        /// 在 <see cref="OnEnable"/> 中缓存。
        /// </summary>
        private XHud_Library_Curves base_script;
        /// <summary>
        /// 曲线列表（可重排列表）。
        /// <para/>
        /// 用于驱动 ReorderableList 的绘制与交互。
        /// </summary>
        private ReorderableList curve_info_list;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 选中项索引属性
        /// </summary>
        private SerializedProperty sp_selected_index;
        /// <summary>
        /// 曲线库列表属性
        /// </summary>
        private SerializedProperty sp_library_curves;
        /// <summary>
        /// 库名称属性
        /// </summary>
        private SerializedProperty sp_library_name;
        /// <summary>
        /// 定位选中项索引属性
        /// </summary>
        private SerializedProperty sp_location_selected_index;
        /// <summary>
        /// 筛选高亮字符串属性
        /// </summary>
        private SerializedProperty sp_highlight;
        /// <summary>
        /// 列表滚动位置属性
        /// </summary>
        private SerializedProperty sp_sound_infolist_original_scroller;
        /// <summary>
        /// 列表项高度属性
        /// </summary>
        private SerializedProperty sp_item_height;
        /// <summary>
        /// 可视区域显示元素数量属性
        /// </summary>
        private SerializedProperty sp_visible_item_count;
        /// <summary>
        /// 查找字符串属性
        /// </summary>
        private SerializedProperty sp_find;
        /// <summary>
        /// 预览自动停止开关属性
        /// </summary>
        private SerializedProperty sp_UsePreviewAutoStop;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 导入按钮按下图标
        /// </summary>
        private Texture2D import_p;
        /// <summary>
        /// 导入按钮常态图标
        /// </summary>
        private Texture2D import_r;
        /// <summary>
        /// 导出按钮按下图标
        /// </summary>
        private Texture2D export_p;
        /// <summary>
        /// 导出按钮常态图标
        /// </summary>
        private Texture2D export_r;
        /// <summary>
        /// 清空按钮按下图标
        /// </summary>
        private Texture2D clear_p;
        /// <summary>
        /// 清空按钮常态图标
        /// </summary>
        private Texture2D clear_r;
        /// <summary>
        /// 创建按钮按下图标
        /// </summary>
        private Texture2D create_p;
        /// <summary>
        /// 创建按钮常态图标
        /// </summary>
        private Texture2D create_r;
        /// <summary>
        /// 删除按钮按下图标
        /// </summary>
        private Texture2D delete_p;
        /// <summary>
        /// 删除按钮常态图标
        /// </summary>
        private Texture2D delete_r;
        /// <summary>
        /// 播放按钮按下图标
        /// </summary>
        private Texture2D play_p;
        /// <summary>
        /// 播放按钮常态图标
        /// </summary>
        private Texture2D play_r;
        /// <summary>
        /// 左箭头按下图标
        /// </summary>
        private Texture2D leftarr_p;
        /// <summary>
        /// 左箭头常态图标
        /// </summary>
        private Texture2D leftarr_r;
        /// <summary>
        /// 右箭头按下图标
        /// </summary>
        private Texture2D rightarr_p;
        /// <summary>
        /// 右箭头常态图标
        /// </summary>
        private Texture2D rightarr_r;
        /// <summary>
        /// 停止按钮按下图标
        /// </summary>
        private Texture2D stop_p;
        /// <summary>
        /// 停止按钮常态图标
        /// </summary>
        private Texture2D stop_r;
        #endregion

        #region 字段 - 参数
        /// <summary>
        /// 屏蔽颜色。
        /// <para/>
        /// 用于在过滤模式下叠加在非匹配项上的半透明遮罩。
        /// </summary>
        private Color blocked_col;
        /// <summary>
        /// 屏蔽颜色透明度。
        /// </summary>
        private float blocked_alp = 0.45f;
        /// <summary>
        /// 预览标题文字。
        /// </summary>
        public string PreviewHeader = "CurvePreview";
        #endregion

        #region 字段 - 绘制缓存
        /// <summary>
        /// 绘制元素矩形。
        /// </summary>
        private Rect drawelement_rect;
        /// <summary>
        /// 滚动视图矩形。
        /// </summary>
        private Rect scrollview_rect;
        /// <summary>
        /// 列表项矩形。
        /// </summary>
        private Rect item_rect;
        /// <summary>
        /// 选中背景色。
        /// </summary>
        private Color SelectedBg = new Color(0, 0, 0, 0.2f);
        #endregion

        #region 生命周期
        /// <summary>
        /// Unity 启用回调。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>缓存目标脚本实例；</description></item>
        /// <item><description>获取全部序列化属性；</description></item>
        /// <item><description>加载全部图标资源；</description></item>
        /// <item><description>初始化列表项高度与可见数量；</description></item>
        /// <item><description>构造 ReorderableList。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            base_script = (XHud_Library_Curves)target;

            #region 获取序列化属性
            sp_library_curves = serializedObject.FindProperty(nameof(XHud_Library_Curves.library_curves));
            sp_location_selected_index = serializedObject.FindProperty(nameof(XHud_Library_Curves.location_selected_index));
            sp_library_name = serializedObject.FindProperty(nameof(XHud_Library_Curves.library_name));
            sp_highlight = serializedObject.FindProperty(nameof(XHud_Library_Curves.highlight));
            sp_selected_index = serializedObject.FindProperty(nameof(XHud_Library_Curves.selected_index));
            sp_sound_infolist_original_scroller = serializedObject.FindProperty(nameof(XHud_Library_Curves.sound_infolist_original_scroller));
            sp_item_height = serializedObject.FindProperty(nameof(XHud_Library_Curves.item_height));
            sp_visible_item_count = serializedObject.FindProperty(nameof(XHud_Library_Curves.visible_item_count));
            sp_find = serializedObject.FindProperty(nameof(XHud_Library_Curves.find));
            #endregion

            #region 获取图标
            import_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/import_p");
            import_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/import_r");
            export_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/export_p");
            export_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/export_r");
            clear_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/clear_p");
            clear_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/clear_r");
            create_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/create_p");
            create_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/create_r");
            delete_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/delete_p");
            delete_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/delete_r");
            play_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/play_p");
            play_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/play_r");
            leftarr_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/leftarr_p");
            leftarr_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/leftarr_r");
            rightarr_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/rightarr_p");
            rightarr_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/rightarr_r");
            stop_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/stop_p");
            stop_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_curve/stop_r");
            #endregion

            //设定列表项高度值
            sp_item_height.floatValue = 30;
            sp_item_height.serializedObject.ApplyModifiedProperties();

            sp_visible_item_count.intValue = 15;
            sp_visible_item_count.serializedObject.ApplyModifiedProperties();

            blocked_col = new Color(0, 0, 0, blocked_alp);

            #region ReorderableList
            curve_info_list = new ReorderableList(serializedObject, sp_library_curves, true, true, true, true);
            curve_info_list.drawElementCallback = CurveInfoList_Original_DrawElementCallback;
            #endregion
        }
        /// <summary>
        /// Unity 禁用回调。
        /// <para/>
        /// 清理运行时状态：
        /// <list type="bullet">
        /// <item><description>重置滚动位置；</description></item>
        /// <item><description>清空选中索引与定位索引；</description></item>
        /// <item><description>清空查找与过滤字符串。</description></item>
        /// </list>
        /// </summary>
        private void OnDisable()
        {
            sp_sound_infolist_original_scroller.vector2Value = Vector2.zero;
            sp_sound_infolist_original_scroller.serializedObject.ApplyModifiedProperties();

            sp_selected_index.intValue = -1;
            sp_selected_index.serializedObject.ApplyModifiedProperties();

            sp_location_selected_index.intValue = -1;
            sp_location_selected_index.serializedObject.ApplyModifiedProperties();

            sp_find.stringValue = string.Empty;
            sp_find.serializedObject.ApplyModifiedProperties();

            sp_highlight.stringValue = string.Empty;
            sp_highlight.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 绘制 Inspector 主入口。
        /// <para/>
        /// 按自上而下的顺序绘制所有面板：
        /// 标题 → 参数 → 快捷功能 → 曲线列表。
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                title_text: "XHud  -  曲线库",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping_ellipsis,
                title_offset: new Vector2(-10, 0),
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 参数区
            XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "参数",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15));

            #region 库名称
            XGUI.layout_property_field(
                title: "库名称",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 90,
                prop: sp_library_name,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 过滤（包含）
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
                title: "过滤（包含）",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 90,
                prop: sp_highlight,
                prop_margin: new RectOffset(0, 0, 5, 0));
            if (XGUI.ChangedCheck_End())
            {
                sp_location_selected_index.intValue = -1;
                sp_location_selected_index.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 查找（精确）
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
                title: "查找（精确）",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 90,
                prop: sp_find,
                prop_margin: new RectOffset(0, 0, 5, 0));
            if (XGUI.ChangedCheck_End())
            {
                if (!string.IsNullOrEmpty(sp_find.stringValue))
                    base_script.CurveLibrary_Location_Find(sp_find.stringValue);
                else
                {
                    sp_selected_index.intValue = -1;
                    sp_selected_index.serializedObject.ApplyModifiedProperties();

                    sp_location_selected_index.intValue = -1;
                    sp_location_selected_index.serializedObject.ApplyModifiedProperties();
                }
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 快捷功能
            if (string.IsNullOrEmpty(sp_highlight.stringValue))
            {
                if (!Application.isPlaying)
                {
                    XGUI.layout_group_start(
                       type: XGUIContainerType.Horizontal,
                       bg_fill: XGUIGroup.缺口纯色边框,
                       bg_color: XGUIColor.亮白,
                       bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                       title: "快捷功能",
                       title_size: XGUIFontSize.M,
                       title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                       title_clipping: TextClipping.Clip,
                       padding: new RectOffset(15, 15, 20, 15));

                    #region 读取曲线库
                    if (XGUI.layout_button(
                        tooltip: "读取曲线库",
                        tex_release: import_r,
                        tex_press: import_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        Curves_Import();
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 导出曲线库
                    if (XGUI.layout_button(
                        tooltip: "导出曲线库",
                        tex_release: export_r,
                        tex_press: export_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        Curves_Export();
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 清空曲线库
                    if (XGUI.layout_button(
                        tooltip: "清空曲线库",
                        tex_release: clear_r,
                        tex_press: clear_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 曲线库消息",
                            title: "清空所有曲线",
                            msg: "是否清空所有曲线项？请注意！如果您的场景中或是预制体中的脚本用到了该曲线库中的曲线，清空后会导致曲线信息丢失，请谨慎操作！",
                            ok: "清空",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        if (res == "暂不")
                            return;

                        sp_library_curves.ClearArray();
                        sp_library_curves.serializedObject.ApplyModifiedProperties();
                        return;
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 添加曲线
                    if (XGUI.layout_button(
                        tooltip: "添加曲线",
                        tex_release: create_r,
                        tex_press: create_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        CurveInfoList_Original_Add(curve_info_list);
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 删除曲线
                    if (XGUI.layout_button(
                        tooltip: "删除曲线",
                        tex_release: delete_r,
                        tex_press: delete_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        CurveInfoList_Original_Remove(curve_info_list);
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                }
            }
            #endregion

            #region 曲线列表
            XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIGroup.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
               title: "曲线列表",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(10, 10, 15, 15));

            DrawCurveInfoList_Original();

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 列表绘制
        /// <summary>
        /// 根据高亮过滤条件屏蔽或启用 GUI。
        /// <para/>
        /// 若名称不包含 <see cref="sp_highlight"/> 的当前值，则禁用 GUI。
        /// </summary>
        /// <param name="name">待检查的名称</param>
        private void BlockGUI(string name)
        {
            if (!name.Contains(sp_highlight.stringValue))
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
        }
        /// <summary>
        /// 绘制元素回调。
        /// <para/>
        /// 绘制内容：
        /// <list type="bullet">
        /// <item><description>序号（灰色，D2 格式）；</description></item>
        /// <item><description>曲线名称输入框；</description></item>
        /// <item><description>曲线编辑器；</description></item>
        /// <item><description>过滤遮罩与定位标记。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rect">绘制矩形</param>
        /// <param name="index">索引</param>
        /// <param name="isActive">是否激活</param>
        /// <param name="isFocused">是否聚焦</param>
        private void CurveInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_library_curves.GetArrayElementAtIndex(index);

            SerializedProperty sp_name = prop.FindPropertyRelative("name");
            SerializedProperty sp_curve = prop.FindPropertyRelative("curve");

            #region 序号
            drawelement_rect.Set(rect.x + 16, rect.y + 4, 30, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(index.ToString("D2")),
                text_color: Color.gray,
                size: XGUIFontSize.S,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion

            BlockGUI(sp_name.stringValue);

            #region 曲线名
            drawelement_rect.Set(rect.x + 42, rect.y + 6, rect.width - (sp_location_selected_index.intValue == -1 ? 200 : 235), 20);
            sp_name.stringValue = XGUI.gui_inputfield(
                     rect: drawelement_rect,
                     //title: "Bk",
                     prop: sp_name.stringValue,
                     text_wrap: true,
                     field_fontsize: XGUIFontSize.M,
                     field_text_offset: Vector2.zero,
                     field_height: XGUI.GetSingleLineHeight(),
                     field_text_color: Color.white,
                     //title_width: 40,
                     //status_icon: "icon_field_status",
                     //status_icon_color: Color.magenta,
                     field_text_font: XGUI.GetFont("xg-medium"),
                     field_text_style: FontStyle.Normal,
                     field_text_anchor: TextAnchor.UpperLeft,
                     field_padding: new RectOffset(5, 5, 0, 0),
                     field_margin: new RectOffset(0, 0, 0, 0));
            sp_name.serializedObject.ApplyModifiedProperties();
            #endregion

            BlockGUI(sp_name.stringValue);

            #region 曲线
            XGUI.ChangedCheck_Start();
            drawelement_rect.Set(rect.width - 120, rect.y + 5, 100, 20);
            sp_curve.animationCurveValue = XGUI.gui_curvefield(
                    rect: drawelement_rect,
                    prop: sp_curve.animationCurveValue,
                     //title: "曲线-转向",
                     //title_width: 60,
                     //title_color: Color.white,
                     //icon: "icon_field_status",
                     //icon_color: Color.green,
                     //state_title: "说明：",
                     //state_value: $"#{XGUI_Utilitys.ConvertColorToHexString(BaseScript.field_color)}",
                     //state_value_color: BaseScript.ThemeColor_Primary,
                     setcurve: (curve) =>
                     {
                         sp_curve.animationCurveValue = curve;
                     },
                     useutility: false);
            sp_curve.serializedObject.ApplyModifiedProperties();
            if (XGUI.ChangedCheck_End())
            {
                if (base_script.act_on_curve_changed != null)
                    base_script.act_on_curve_changed();
            }
            #endregion

            GUI.enabled = true;

            if (!sp_name.stringValue.Contains(sp_highlight.stringValue))
            {
                drawelement_rect.Set(rect.x - 20, rect.y, rect.width + 20, rect.height);
                EditorGUI.DrawRect(drawelement_rect, blocked_col);
            }

            if (index == sp_location_selected_index.intValue)
            {
                drawelement_rect.Set(rect.width - 180, rect.y + 3, 50, 20);
                XGUI.gui_label(
                    rect: drawelement_rect,
                    text: new GUIContent("已定位"),
                    text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    size: XGUIFontSize.S,
                    clipping: TextClipping.Clip,
                    anchor: TextAnchor.MiddleCenter,
                    offset: new Vector2(0, 0),
                    font_style: FontStyle.Normal);
            }
        }
        /// <summary>
        /// 绘制曲线列表。
        /// <para/>
        /// 只绘制滚动视图可视区域内的元素，以优化大列表性能。
        /// </summary>
        private void DrawCurveInfoList_Original()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visible_item_count.intValue * sp_item_height.floatValue);
            sp_sound_infolist_original_scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_sound_infolist_original_scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, curve_info_list.count * sp_item_height.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_sound_infolist_original_scroller.vector2Value.y / sp_item_height.floatValue);
            int endIndex = Mathf.CeilToInt((sp_sound_infolist_original_scroller.vector2Value.y + scrollview_rect.height) / sp_item_height.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < curve_info_list.count; i++)
            {
                SerializedProperty prop = sp_library_curves.GetArrayElementAtIndex(i);
                SerializedProperty sp_name = prop.FindPropertyRelative("name");

                item_rect.Set(0, i * sp_item_height.floatValue, scrollview_rect.width, sp_item_height.floatValue);

                // 如果当前元素被选中，绘制选中效果
                if (sp_name.stringValue.Contains(sp_highlight.stringValue))
                {
                    if (sp_selected_index.intValue == i)
                    {
                        // 高亮标记表示选中
                        item_rect.Set(1, i * sp_item_height.floatValue + 12, 5, 5);
                        EditorGUI.DrawRect(item_rect, XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        // 高亮背景表示选中
                        item_rect.Set(0, i * sp_item_height.floatValue, scrollview_rect.width + 20, sp_item_height.floatValue);
                        EditorGUI.DrawRect(item_rect, SelectedBg);
                    }
                }

                item_rect.Set(0, i * sp_item_height.floatValue, scrollview_rect.width, sp_item_height.floatValue);

                curve_info_list.drawElementCallback.Invoke(item_rect, i, i == curve_info_list.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (sp_name.stringValue.Contains(sp_highlight.stringValue))
                        {
                            // 更新选中项
                            sp_selected_index.intValue = i;
                            sp_selected_index.serializedObject.ApplyModifiedProperties();

                            // 标记界面需要更新
                            GUI.changed = true;
                        }
                    }
                }

                // 绘制元素
                EditorGUI.PropertyField(item_rect, prop, GUIContent.none);
            }

            GUI.EndScrollView();
        }
        /// <summary>
        /// 列表移除项。
        /// <para/>
        /// 移除当前 ReorderableList 索引对应的曲线项，并触发移除回调。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void CurveInfoList_Original_Remove(ReorderableList list)
        {
            if (list.count <= 0)
                return;
            sp_library_curves.DeleteArrayElementAtIndex(list.index);
            if (base_script.act_on_curve_removed != null)
                base_script.act_on_curve_removed();
        }
        /// <summary>
        /// 列表增加项。
        /// <para/>
        /// 若列表为空则直接向底层集合追加；否则在当前位置插入新元素并初始化默认值。
        /// 最后触发添加回调。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void CurveInfoList_Original_Add(ReorderableList list)
        {
            if (list.count <= 0)
            {
                if (base_script.library_curves == null)
                    base_script.library_curves = new List<xHud_LibraryArg_Curve>();
                base_script.library_curves.Add(new XHud.xHud_LibraryArg_Curve("曲线" + base_script.library_curves.Count, AnimationCurve.EaseInOut(0, 0, 1, 1)));
            }
            else
            {
                sp_library_curves.InsertArrayElementAtIndex(list.index);
                SerializedProperty prop = sp_library_curves.GetArrayElementAtIndex(list.index);
                SerializedProperty sp_name = prop.FindPropertyRelative("name");
                sp_name.stringValue = $"新曲线_{list.index}";


                sp_library_curves.serializedObject.ApplyModifiedProperties();
            }

            if (base_script.act_on_curve_added != null)
                base_script.act_on_curve_added(base_script.library_curves[base_script.library_curves.Count - 1].name, base_script.library_curves[base_script.library_curves.Count - 1].curve);
        }
        #endregion

        #region 工具 - 曲线库
        /// <summary>
        /// 曲线库导出。
        /// <para/>
        /// 将当前曲线库的全部曲线（含全部关键帧数据）序列化为 JSON 文件并写入磁盘。
        /// </summary>
        private void Curves_Export()
        {
            CurveLib lib = new CurveLib();
            lib.nodes = new List<CurveNode>();

            for (int i = 0; i < base_script.library_curves.Count; i++)
            {
                xHud_LibraryArg_Curve info = base_script.library_curves[i];

                Keyframe[] frames = info.curve.keys;

                CurveNode Node = new CurveNode();
                Node.name = info.name;
                Node.frames = new CurveFrame[frames.Length];
                for (int s = 0; s < frames.Length; s++)
                {
                    CurveFrame cs = new CurveFrame();
                    cs.time = frames[s].time;
                    cs.value = frames[s].value;
                    cs.in_tangent = frames[s].inTangent;
                    cs.in_weight = frames[s].inWeight;
                    cs.out_tangent = frames[s].outTangent;
                    cs.out_weight = frames[s].outWeight;
                    cs.weighted_mode = frames[s].weightedMode;
                    Node.frames[s] = cs;
                }
                lib.nodes.Add(Node);
            }

            string path = EditorUtility.SaveFilePanel("", Application.dataPath, "HudCurves", "json");

            if (string.IsNullOrEmpty(path))
                return;

            if (!string.IsNullOrEmpty(path))
            {
                FileInfo info = new FileInfo(path);
                StreamWriter sw = info.CreateText();

                string message = JsonUtility.ToJson(lib, true);

                sw.Write(message);
                sw.Close();
                sw.Dispose();
            }
        }
        /// <summary>
        /// 曲线库导入。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>弹窗询问用户选择"追加"或"替换"模式；</description></item>
        /// <item><description>打开文件选择面板选取 JSON 文件；</description></item>
        /// <item><description>解析关键帧数据并写入曲线库列表；</description></item>
        /// <item><description>触发曲线变更回调。</description></item>
        /// </list>
        /// </summary>
        private void Curves_Import()
        {
            string path = "";
            string res = XGUI.dialog(
                type: XGUIDialogType.警告,
                windowtitle: "XHud - 曲线库消息",
                title: "读取曲线数据",
                msg: "根据您的需要选择导入曲线数据的方式，如果是追加则会在当前曲线库的基础上后续叠加导入的曲线项，如果是替换则会完全替换当前曲线库的所有曲线项！",
                ok: "追加",
                cancel: "替换",
                alt: "暂不",
                PrimaryIndex: 2,
                usemodal: true,
                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
            if (res == "暂不")
            {
                return;
            }

            switch (res)
            {
                case "追加":
                    path = EditorUtility.OpenFilePanel("读取曲线库数据文件", Application.dataPath, "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        string content = File.ReadAllText(path);

                        CurveLib lib = JsonUtility.FromJson<CurveLib>(content);

                        int size = sp_library_curves.arraySize;

                        for (int i = 0; i < lib.nodes.Count; i++)
                        {
                            string name = lib.nodes[i].name;

                            Keyframe[] frames = new Keyframe[lib.nodes[i].frames.Length];

                            for (int s = 0; s < frames.Length; s++)
                            {
                                frames[s] = new Keyframe(
                                    lib.nodes[i].frames[s].time,
                                    lib.nodes[i].frames[s].value,
                                    lib.nodes[i].frames[s].in_tangent,
                                    lib.nodes[i].frames[s].out_tangent,
                                    lib.nodes[i].frames[s].in_weight,
                                    lib.nodes[i].frames[s].out_weight);
                            }

                            AnimationCurve curve = new AnimationCurve(frames);

                            xHud_LibraryArg_Curve info = new xHud_LibraryArg_Curve(name, curve);

                            sp_library_curves.InsertArrayElementAtIndex(i + size);
                            SerializedProperty sp_cur = sp_library_curves.GetArrayElementAtIndex(i + size);
                            SerializedProperty sp_name = sp_cur.FindPropertyRelative("name");
                            SerializedProperty sp_curve = sp_cur.FindPropertyRelative("curve");

                            sp_name.stringValue = info.name;
                            sp_curve.animationCurveValue = info.curve;

                            sp_name.serializedObject.ApplyModifiedProperties();
                            sp_curve.serializedObject.ApplyModifiedProperties();
                            sp_cur.serializedObject.ApplyModifiedProperties();
                        }
                        sp_library_curves.serializedObject.ApplyModifiedProperties();

                        if (base_script.act_on_curve_changed != null)
                            base_script.act_on_curve_changed();
                    }
                    break;
                case "替换":
                    path = EditorUtility.OpenFilePanel("读取曲线库数据文件", Application.dataPath, "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        string content = File.ReadAllText(path);
                        CurveLib lib = JsonUtility.FromJson<CurveLib>(content);

                        sp_library_curves.ClearArray();

                        for (int i = 0; i < lib.nodes.Count; i++)
                        {
                            string name = lib.nodes[i].name;

                            Keyframe[] frames = new Keyframe[lib.nodes[i].frames.Length];

                            for (int s = 0; s < frames.Length; s++)
                            {
                                frames[s] = new Keyframe(
                                    lib.nodes[i].frames[s].time,
                                    lib.nodes[i].frames[s].value,
                                    lib.nodes[i].frames[s].in_tangent,
                                    lib.nodes[i].frames[s].out_tangent,
                                    lib.nodes[i].frames[s].in_weight,
                                    lib.nodes[i].frames[s].out_weight);
                            }

                            AnimationCurve curve = new AnimationCurve(frames);

                            xHud_LibraryArg_Curve info = new xHud_LibraryArg_Curve(name, curve);

                            sp_library_curves.InsertArrayElementAtIndex(i);
                            SerializedProperty sp_cur = sp_library_curves.GetArrayElementAtIndex(i);
                            SerializedProperty sp_name = sp_cur.FindPropertyRelative("name");
                            SerializedProperty sp_curve = sp_cur.FindPropertyRelative("curve");

                            sp_name.stringValue = info.name;
                            sp_curve.animationCurveValue = info.curve;

                            sp_name.serializedObject.ApplyModifiedProperties();
                            sp_curve.serializedObject.ApplyModifiedProperties();
                            sp_cur.serializedObject.ApplyModifiedProperties();
                        }
                        sp_library_curves.serializedObject.ApplyModifiedProperties();

                        if (base_script.act_on_curve_changed != null)
                            base_script.act_on_curve_changed();
                    }
                    break;
            }
        }
        #endregion
    }
}