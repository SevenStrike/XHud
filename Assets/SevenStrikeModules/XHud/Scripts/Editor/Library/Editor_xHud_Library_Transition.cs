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

    [CustomEditor(typeof(XHud_Library_Transition))]
    public class Editor_XHud_Library_Transition : Editor
    {
        #region 字段 - 组件 / 列表
        /// <summary>
        /// 目标脚本实例。
        /// <para/>
        /// 在 <see cref="OnEnable"/> 中缓存。
        /// </summary>
        private XHud_Library_Transition base_script;
        /// <summary>
        /// 转场列表（可重排列表）。
        /// <para/>
        /// 用于驱动 ReorderableList 的绘制与交互。
        /// </summary>
        private ReorderableList transition_info_list;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 转场库列表属性
        /// </summary>
        private SerializedProperty sp_library_transitions;
        /// <summary>
        /// 库名称属性
        /// </summary>
        private SerializedProperty sp_library_name;
        /// <summary>
        /// 选中项索引属性
        /// </summary>
        private SerializedProperty sp_selected_index;
        /// <summary>
        /// 定位选中项索引属性
        /// </summary>
        private SerializedProperty sp_location_selected_index;
        /// <summary>
        /// 转场信息列表滚动位置属性
        /// </summary>
        private SerializedProperty sp_transition_info_list_original_scroller;
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
        /// 筛选高亮字符串属性
        /// </summary>
        private SerializedProperty sp_highlight;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 警告图标
        /// </summary>
        private Texture2D warnIcon;
        /// <summary>
        /// 详情按钮常态图标
        /// </summary>
        private Texture2D btn_icon_details_released;
        /// <summary>
        /// 详情按钮按下图标
        /// </summary>
        private Texture2D btn_icon_details_press;
        /// <summary>
        /// 分析按钮按下图标
        /// </summary>
        private Texture2D analyze_p;
        /// <summary>
        /// 分析按钮常态图标
        /// </summary>
        private Texture2D analyze_r;
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
        /// 帧信息 - 跳帧图标
        /// </summary>
        private Texture2D skip;
        /// <summary>
        /// 帧信息 - 总帧图标
        /// </summary>
        private Texture2D total;
        /// <summary>
        /// 帧信息 - 末帧图标
        /// </summary>
        private Texture2D last;
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
        /// <item><description>初始化屏蔽色；</description></item>
        /// <item><description>构造 ReorderableList。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            base_script = (XHud_Library_Transition)target;

            #region 获取序列帧属性
            sp_library_transitions = serializedObject.FindProperty(nameof(XHud_Library_Transition.library_transitions));
            sp_library_name = serializedObject.FindProperty(nameof(XHud_Library_Transition.library_name));
            sp_highlight = serializedObject.FindProperty(nameof(XHud_Library_Transition.highlight));
            sp_selected_index = serializedObject.FindProperty(nameof(XHud_Library_Transition.selected_index));
            sp_location_selected_index = serializedObject.FindProperty(nameof(XHud_Library_Transition.location_selected_index));
            sp_transition_info_list_original_scroller = serializedObject.FindProperty(nameof(XHud_Library_Transition.transition_info_list_original_scroller));
            sp_item_height = serializedObject.FindProperty(nameof(XHud_Library_Transition.item_height));
            sp_visible_item_count = serializedObject.FindProperty(nameof(XHud_Library_Transition.visible_item_count));
            sp_find = serializedObject.FindProperty(nameof(XHud_Library_Transition.find));
            #endregion

            #region 获取图标
            warnIcon = XGUI.GetBuiltInIcon("console.warnicon");
            btn_icon_details_released = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/detail_r");
            btn_icon_details_press = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/detail_p");
            analyze_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/analyze_p");
            analyze_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/analyze_r");
            clear_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/clear_p");
            clear_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/clear_r");
            create_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/create_p");
            create_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/create_r");
            delete_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/delete_p");
            delete_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/delete_r");
            skip = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/skip");
            total = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/total");
            last = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/last");
            #endregion

            //设定列表项高度值
            sp_item_height.floatValue = 75;
            sp_item_height.serializedObject.ApplyModifiedProperties();

            sp_visible_item_count.intValue = 6;
            sp_visible_item_count.serializedObject.ApplyModifiedProperties();

            blocked_col = new Color(0, 0, 0, blocked_alp);

            #region ReorderableList
            transition_info_list = new ReorderableList(serializedObject, sp_library_transitions, true, true, true, true);
            transition_info_list.drawElementCallback = TransitionInfoList_Original_DrawElementCallback;
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
            if (target != null)
            {
                sp_transition_info_list_original_scroller.vector2Value = Vector2.zero;
                sp_transition_info_list_original_scroller.serializedObject.ApplyModifiedProperties();

                sp_selected_index.intValue = -1;
                sp_selected_index.serializedObject.ApplyModifiedProperties();

                sp_location_selected_index.intValue = -1;
                sp_location_selected_index.serializedObject.ApplyModifiedProperties();

                sp_find.stringValue = string.Empty;
                sp_find.serializedObject.ApplyModifiedProperties();

                sp_highlight.stringValue = string.Empty;
                sp_highlight.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 绘制 Inspector 主入口。
        /// <para/>
        /// 按自上而下的顺序绘制所有面板：
        /// 标题 → 参数 → 快捷功能 → 转场库列表。
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
                title_text: "XHud  -  转场库",
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
                    base_script.TransitionLibrary_Location_Find(sp_find.stringValue);
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

                    #region 分析帧数据
                    if (XGUI.layout_button(
                        tooltip: "分析帧数据",
                        tex_release: analyze_r,
                        tex_press: analyze_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        for (int i = 0; i < base_script.library_transitions.Count; i++)
                        {
                            XHud_LibraryArg_Transition node = base_script.library_transitions[i];
                            if (node.frames == null || node.frames.Count <= 0)
                            {
                                node.total_frames_count = 0;
                                node.last_frame_index = 0;
                                node.res = Vector2Int.zero;
                                continue;
                            }
                            else
                            {
                                node.total_frames_count = node.frames.Count;
                                node.last_frame_index = node.frames.Count - 1;
                                for (int s = 0; s < node.frames.Count; s++)
                                {
                                    // 获取纹理的导入设置
                                    TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(node.frames[s])) as TextureImporter;

                                    if (importer != null)
                                    {
                                        if (importer.npotScale != TextureImporterNPOTScale.None)
                                        {
                                            // 修改 m_NPOTScale 设置为 无_None
                                            importer.npotScale = TextureImporterNPOTScale.None;

                                            // 保存修改后的设置
                                            importer.SaveAndReimport();
                                        }
                                    }
                                }
                                node.res = new Vector2Int(node.frames[0].width, node.frames[0].height);
                            }
                        }
                        return;
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 导出元素动效参数模版
                    if (XGUI.layout_button(
                        tooltip: "导出元素动效参数模版",
                        tex_release: clear_r,
                        tex_press: clear_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        string res = XGUI.dialog(
                         type: XGUIDialogType.警告,
                         windowtitle: "XHud - 转场库消息",
                         title: "清空所有转场",
                         msg: "是否清空所有转场项？请注意！如果您的场景中转场组件用到了该转场库中的转场效果项，清空后会导致转场信息丢失，请谨慎操作！",
                         ok: "清空",
                         cancel: "暂不",
                         PrimaryIndex: 0,
                         usemodal: true,
                         themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                        if (res == "暂不")
                            return;

                        for (int i = 0; i < sp_library_transitions.arraySize; i++)
                        {
                            SerializedProperty frames = sp_library_transitions.GetArrayElementAtIndex(i).FindPropertyRelative("frames");
                            frames.ClearArray();
                            frames.serializedObject.ApplyModifiedProperties();
                        }

                        sp_library_transitions.ClearArray();
                        sp_library_transitions.serializedObject.ApplyModifiedProperties();
                        return;
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 添加项
                    if (XGUI.layout_button(
                        tooltip: "添加项",
                        tex_release: create_r,
                        tex_press: create_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        TransitionInfoList_Original_Add(transition_info_list);
                        return;
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 删除项
                    if (XGUI.layout_button(
                        tooltip: "删除项",
                        tex_release: delete_r,
                        tex_press: delete_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        TransitionInfoList_Original_Remove(transition_info_list);
                        return;
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                }
            }
            #endregion

            #region 转场库列表
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "转场库列表",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15));

            DrawTransitionInfoList_Original();

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
        /// <item><description>标识名称（主题色）；</description></item>
        /// <item><description>动效说明；</description></item>
        /// <item><description>帧信息（总帧数 / 末帧索引 / 跳帧）；</description></item>
        /// <item><description>序列帧缺失警告图标；</description></item>
        /// <item><description>查看详细信息按钮；</description></item>
        /// <item><description>右键菜单（修改 / 克隆 / 获取）；</description></item>
        /// <item><description>过滤遮罩与定位标记。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rect">绘制矩形</param>
        /// <param name="index">索引</param>
        /// <param name="isActive">是否激活</param>
        /// <param name="isFocused">是否聚焦</param>
        private void TransitionInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_library_transitions.GetArrayElementAtIndex(index);
            SerializedProperty sp_name = prop.FindPropertyRelative("name");
            SerializedProperty sp_des = prop.FindPropertyRelative("description");
            SerializedProperty sp_TotalFramesCount = prop.FindPropertyRelative("total_frames_count");
            SerializedProperty sp_LastFrameIndex = prop.FindPropertyRelative("last_frame_index");
            SerializedProperty sp_SkipFrame = prop.FindPropertyRelative("skip_frame");
            SerializedProperty sp_Res = prop.FindPropertyRelative("res");
            SerializedProperty sp_Frames = prop.FindPropertyRelative("frames");

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            #region 序号
            drawelement_rect.Set(rect.x + 15, rect.y + 5, 30, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(index.ToString("D2")),
                text_color: Color.gray,
                size: XGUIFontSize.S,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion

            #region 标识名称
            drawelement_rect.Set(rect.x + 40, rect.y + 5, rect.width - 140, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(sp_name.stringValue),
                text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion

            #region 动效说明
            drawelement_rect.Set(rect.x + 40, rect.y + 25, rect.width - 160, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(sp_des.stringValue),
                text_color: Color.white * 0.65f,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion


            #region 帧信息

            float AddedHeight = 16;

            drawelement_rect.Set(rect.x + 40, rect.y + 36 + AddedHeight, 14, 14);
            XGUI.gui_icon(
                  rect: drawelement_rect,
                  icon: total,
                  color: Color.white);

            drawelement_rect.Set(rect.x + 110, rect.y + 36 + AddedHeight, 14, 14);
            XGUI.gui_icon(
                  rect: drawelement_rect,
                  icon: last,
                  color: Color.white);

            drawelement_rect.Set(rect.x + 180, rect.y + 36 + AddedHeight, 14, 14);
            XGUI.gui_icon(
                  rect: drawelement_rect,
                  icon: skip,
                  color: Color.white);

            drawelement_rect.Set(rect.x + 70, rect.y + 33 + AddedHeight, 80, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(sp_TotalFramesCount.intValue.ToString()),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);

            drawelement_rect.Set(rect.x + 140, rect.y + 33 + AddedHeight, 80, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(sp_LastFrameIndex.intValue.ToString()),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);

            drawelement_rect.Set(rect.x + 210, rect.y + 33 + AddedHeight, 80, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(sp_SkipFrame.intValue.ToString()),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion

            #region 转场项的序列帧状态
            if (sp_Frames.arraySize <= 0)
            {
                GUIContent content = new GUIContent();
                content.image = warnIcon;
                content.tooltip = "未找到任何序列帧图像";
                drawelement_rect.Set(rect.x + 15, rect.y + 35, 15, 15);

                XGUI.gui_label(
                    rect: drawelement_rect,
                    text: content,
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    offset: new Vector2(0, 0),
                    font_style: FontStyle.Normal);
            }
            #endregion

            BlockGUI(sp_name.stringValue);

            #region 辅助菜单
            // 检测鼠标点击事件
            Event e = Event.current;
            drawelement_rect.Set(rect.x, rect.y, rect.width, rect.height);
            // 检测鼠标点击事件
            if (e.type == EventType.MouseDown && e.button == 1 && drawelement_rect.Contains(e.mousePosition))
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("E (修改转场资源)"), false, () =>
                {
                    OpenParameterSetter(base_script.library_transitions[index], index);
                });
                menu.AddItem(new GUIContent("C (克隆转场资源)"), false, () =>
                {
                    sp_library_transitions.InsertArrayElementAtIndex(index);
                    sp_library_transitions.serializedObject.ApplyModifiedProperties();
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("S (获取转场资源)"), false, () =>
                {
                    //GUIUtility.systemCopyBuffer = sp_name.stringValue;
                });
                // 显示右键菜单
                menu.ShowAsContext();

                e.Use();
            }
            #endregion

            #region 查看详细信息
            drawelement_rect.Set(rect.width - 50, rect.y + 14, 22, 22);
            if (XGUI.gui_button(
                rect: drawelement_rect,
                tooltip: $"查看详细信息",
                tex_release: btn_icon_details_released,
                tex_press: btn_icon_details_press,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                focus_name: "fav_btn"))
            {
                OpenParameterSetter(base_script.library_transitions[index], index);
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
                drawelement_rect.Set(rect.width - 125, rect.y + 15, 50, 20);
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
        /// 绘制转场库列表。
        /// <para/>
        /// 只绘制滚动视图可视区域内的元素，以优化大列表性能。
        /// </summary>
        private void DrawTransitionInfoList_Original()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visible_item_count.intValue * sp_item_height.floatValue);
            sp_transition_info_list_original_scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_transition_info_list_original_scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, transition_info_list.count * sp_item_height.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_transition_info_list_original_scroller.vector2Value.y / sp_item_height.floatValue);
            int endIndex = Mathf.CeilToInt((sp_transition_info_list_original_scroller.vector2Value.y + scrollview_rect.height) / sp_item_height.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < transition_info_list.count; i++)
            {
                SerializedProperty prop = sp_library_transitions.GetArrayElementAtIndex(i);
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
                        item_rect.Set(0, i * sp_item_height.floatValue, scrollview_rect.width, sp_item_height.floatValue);
                        EditorGUI.DrawRect(item_rect, SelectedBg);
                    }
                }

                item_rect.Set(0, i * sp_item_height.floatValue, scrollview_rect.width, sp_item_height.floatValue);

                transition_info_list.drawElementCallback.Invoke(item_rect, i, i == transition_info_list.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (sp_name.stringValue.Contains(sp_highlight.stringValue))
                        {
                            // 更新选中项
                            sp_selected_index.intValue = i;

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
        /// 移除当前选中索引对应的转场项。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void TransitionInfoList_Original_Remove(ReorderableList list)
        {
            if (list.count <= 0)
                return;
            sp_library_transitions.DeleteArrayElementAtIndex(sp_selected_index.intValue);
            sp_library_transitions.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 列表增加项。
        /// <para/>
        /// 若列表为空则插入到索引 0；否则插入到当前位置。
        /// 新项默认名称 <c>NewTransition</c>，帧数据全部初始化。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void TransitionInfoList_Original_Add(ReorderableList list)
        {
            SerializedProperty prop = null;
            if (sp_library_transitions.arraySize <= 0)
            {
                sp_library_transitions.InsertArrayElementAtIndex(0);
                prop = sp_library_transitions.GetArrayElementAtIndex(0);
            }
            else
            {
                sp_library_transitions.InsertArrayElementAtIndex(list.index);
                prop = sp_library_transitions.GetArrayElementAtIndex(list.index);
            }

            prop.FindPropertyRelative("name").stringValue = "NewTransition";
            prop.FindPropertyRelative("res").vector2IntValue = Vector2Int.zero;
            prop.FindPropertyRelative("frames").ClearArray();
            prop.FindPropertyRelative("description").stringValue = "";
            prop.FindPropertyRelative("total_frames_count").intValue = 0;
            prop.FindPropertyRelative("last_frame_index").intValue = 0;
            prop.FindPropertyRelative("skip_frame").intValue = 1;

            sp_library_transitions.serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 工具 - 转场修改器
        /// <summary>
        /// 打开转场资源修改器窗口。
        /// <para/>
        /// 将指定转场项与其原始数据传入修改器，用于编辑转场名称、描述与序列帧。
        /// </summary>
        /// <param name="info">转场数据</param>
        /// <param name="index">索引</param>
        public void OpenParameterSetter(XHud_LibraryArg_Transition info, int index)
        {
            Editor_XHud_LibrarySetTool_Transition window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Transition>(true);

            window.titleContent = new GUIContent("XHud - 转场资源修改器");
            XGUI.CenterEditorWindow(new Vector2Int(800, 680), window);
            window.SetTitle("转场资源修改器");
            window.SetInfo(info.name, info.description);
            window.Set_TransitionNode(info);
            window.Set_OriginTransitionNode(info);
            window.ModifiedIndex = index;
            window.SetButtonText("更新", "取消");
            window.Set_Target_Hud_TransitionLibrary(base_script);
            //window.ShowModal();
            window.Show();
        }
        #endregion
    }
}