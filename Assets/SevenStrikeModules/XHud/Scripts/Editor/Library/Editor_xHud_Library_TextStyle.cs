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
    using TMPro;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    #region 数据模型
    /// <summary>
    /// 文字样式导出容器。
    /// <para/>
    /// 序列化为 JSON 后写入磁盘，用于文字样式库的导入 / 导出。
    /// </summary>
    public class ExportTextStyles
    {
        /// <summary>
        /// 文字样式库列表。
        /// </summary>
        public List<XHud_LibraryArg_TextStyle> TextStyleLibrary = new List<XHud_LibraryArg_TextStyle>();
    }
    #endregion

    #region 编辑器主体
    /// <summary>
    /// XHud 文字样式库的自定义编辑器。
    /// <para/>
    /// 提供样式列表的可视化编辑、右键菜单（修改 / 获取 / 粘贴 / 克隆）、导入导出（含字体资源拷贝）、
    /// 预览、增删清空等功能。
    /// </summary>
    [CustomEditor(typeof(XHud_Library_TextStyle))]
    public class Editor_XHud_Library_TextStyle : Editor
    {
        #region 字段 - 组件 / 列表
        /// <summary>
        /// 目标脚本实例。
        /// <para/>
        /// 在 <see cref="OnEnable"/> 中缓存。
        /// </summary>
        private XHud_Library_TextStyle BaseScript;
        /// <summary>
        /// 文字样式库列表（可重排列表）。
        /// <para/>
        /// 用于驱动 ReorderableList 的绘制与交互。
        /// </summary>
        private ReorderableList sp_TextStyleLibraryList;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 本 Inspector 用到的全部序列化属性引用。
        /// <para/>
        /// 统一在 <see cref="OnEnable"/> 里初始化。
        /// <para/>
        /// 覆盖：
        /// <list type="bullet">
        /// <item><description>文字样式库列表；</description></item>
        /// <item><description>预览字体 / 颜色 / 样式 / 内容；</description></item>
        /// <item><description>选中 / 定位索引；</description></item>
        /// <item><description>过滤 / 查找字符串；</description></item>
        /// <item><description>列表滚动位置与项高；</description></item>
        /// <item><description>库名称。</description></item>
        /// </list>
        /// </summary>
        private SerializedProperty
            sp_TextStyleLibrary,
            sp_PreviewFontColor,
            sp_Preview_Font,
            sp_PreviewTypeText,
            sp_LibraryName,
            sp_itemHeight,
            sp_visibleItemCount,
            sp_ColorInfoList_Original_Scroller,
            sp_LocationSelectedIndex,
            sp_SelectedIndex,
            sp_Find,
            sp_Highlight,
            sp_PreviewFontStyle,
            sp_PreviewContent;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 标题 / 按钮 / 图标资源缓存。
        /// <para/>
        /// 统一在 <see cref="OnEnable"/> 里加载。
        /// </summary>
        private Texture2D warnIcon, btn_icon_details_released, btn_icon_details_press, import_p, import_r, export_p, export_r, clear_p, clear_r, create_p, create_r, delete_p, delete_r;
        #endregion

        #region 字段 - 参数
        /// <summary>
        /// 警告图标 GUIContent 缓存。
        /// <para/>
        /// 用于字体丢失时的提示。
        /// </summary>
        private GUIContent warning_content;
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
        /// 是否显示预览面板。
        /// <para/>
        /// 有列表项被选中时置为 true。
        /// </summary>
        bool sws;
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
        /// <item><description>初始化屏蔽色；</description></item>
        /// <item><description>加载全部图标资源；</description></item>
        /// <item><description>初始化预览文字类型；</description></item>
        /// <item><description>初始化列表项高度与可见数量；</description></item>
        /// <item><description>构造 ReorderableList。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            BaseScript = (XHud_Library_TextStyle)target;

            #region 获取序列化属性
            sp_TextStyleLibrary = serializedObject.FindProperty("TextStyleLibrary");
            sp_LibraryName = serializedObject.FindProperty("LibraryName");
            sp_PreviewContent = serializedObject.FindProperty("PreviewContent");
            sp_PreviewFontColor = serializedObject.FindProperty("PreviewFontColor");
            sp_itemHeight = serializedObject.FindProperty("itemHeight");
            sp_visibleItemCount = serializedObject.FindProperty("visibleItemCount");
            sp_ColorInfoList_Original_Scroller = serializedObject.FindProperty("ColorInfoList_Original_Scroller");
            sp_LocationSelectedIndex = serializedObject.FindProperty("LocationSelectedIndex");
            sp_SelectedIndex = serializedObject.FindProperty("SelectedIndex");
            sp_Find = serializedObject.FindProperty("Find");
            sp_Highlight = serializedObject.FindProperty("Highlight");
            sp_Preview_Font = serializedObject.FindProperty("Preview_Font");
            sp_PreviewTypeText = serializedObject.FindProperty("PreviewTypeText");
            sp_PreviewFontStyle = serializedObject.FindProperty("PreviewStyle");
            #endregion

            blocked_col = new Color(0, 0, 0, blocked_alp);

            #region 获取图标
            btn_icon_details_released = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style/detail_r");
            btn_icon_details_press = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style/detail_p");
            import_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style/import_p");
            import_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style/import_r");
            export_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style/export_p");
            export_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style/export_r");
            clear_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style/clear_p");
            clear_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style/clear_r");
            create_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style/create_p");
            create_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style/create_r");
            delete_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style/delete_p");
            delete_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style/delete_r");
            warnIcon = XGUI.GetBuiltInIcon("console.warnicon");
            #endregion

            sp_PreviewTypeText.stringValue = "-";
            sp_PreviewTypeText.serializedObject.ApplyModifiedProperties();

            //设定列表项高度值
            sp_itemHeight.floatValue = 61;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 7;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            #region ReorderableList - TextStyleInfoList
            sp_TextStyleLibraryList = new ReorderableList(serializedObject, sp_TextStyleLibrary, true, true, true, true);
            sp_TextStyleLibraryList.drawElementCallback = TextStyleInfoList_Original_DrawElementCallback;
            sp_TextStyleLibraryList.onAddCallback = TextStyleInfoList_Original_AddCallback;
            sp_TextStyleLibraryList.onRemoveCallback = TextStyleInfoList_Original_RemoveCallback;
            #endregion
        }
        /// <summary>
        /// Unity 禁用回调。
        /// <para/>
        /// 清理运行时状态：
        /// <list type="bullet">
        /// <item><description>重置滚动位置；</description></item>
        /// <item><description>清空选中索引与定位索引；</description></item>
        /// <item><description>清空查找与过滤字符串；</description></item>
        /// <item><description>重置预览文字类型、颜色与样式。</description></item>
        /// </list>
        /// </summary>
        private void OnDisable()
        {
            sp_ColorInfoList_Original_Scroller.vector2Value = Vector2.zero;
            sp_ColorInfoList_Original_Scroller.serializedObject.ApplyModifiedProperties();

            sp_SelectedIndex.intValue = -1;
            sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

            sp_LocationSelectedIndex.intValue = -1;
            sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();

            sp_Find.stringValue = string.Empty;
            sp_Find.serializedObject.ApplyModifiedProperties();

            sp_Highlight.stringValue = string.Empty;
            sp_Highlight.serializedObject.ApplyModifiedProperties();

            sp_PreviewTypeText.stringValue = string.Empty;
            sp_PreviewTypeText.serializedObject.ApplyModifiedProperties();

            sp_PreviewFontColor.colorValue = Color.white;
            sp_PreviewFontColor.serializedObject.ApplyModifiedProperties();

            sp_PreviewFontStyle.enumValueIndex = (int)FontStyle.Normal;
            sp_PreviewFontStyle.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 绘制 Inspector 主入口。
        /// <para/>
        /// 按自上而下的顺序绘制所有面板：
        /// 标题 → 参数 → 快捷功能 → 样式列表。
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
                title_text: "XHud  -  字体样式库",
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
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "参数",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15));

            #region 库名称
            XGUI.layout_property_field(
                title: "库名称",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 90,
                prop: sp_LibraryName,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 过滤（包含）
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
                title: "过滤（包含）",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 90,
                prop: sp_Highlight,
                prop_margin: new RectOffset(0, 0, 5, 0));
            if (XGUI.ChangedCheck_End())
            {
                sp_LocationSelectedIndex.intValue = -1;
                sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 查找（精确）
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
                title: "查找（精确）",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 90,
                prop: sp_Find,
                prop_margin: new RectOffset(0, 0, 5, 0));
            if (XGUI.ChangedCheck_End())
            {
                if (!string.IsNullOrEmpty(sp_Find.stringValue))
                    BaseScript.TextStyleLibrary_Location_Find(sp_Find.stringValue);
                else
                {
                    sp_SelectedIndex.intValue = -1;
                    sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

                    sp_LocationSelectedIndex.intValue = -1;
                    sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();
                }
            }
            #endregion

            #region 样例文字内容
            XGUI.layout_property_field(
                title: "样例文字内容",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 90,
                prop: sp_PreviewContent,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 快捷功能
            if (string.IsNullOrEmpty(sp_Highlight.stringValue))
            {
                if (!Application.isPlaying)
                {
                    XGUI.layout_group_start(
                        type: XGUIContainerType.Horizontal,
                        bg_fill: XGUIGroup.缺口纯色边框,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Group,
                        title: "快捷功能",
                        title_size: XGUIFontSize.M,
                        title_text_color: XHud_Dashboard.Theme_Primary,
                        title_clipping: TextClipping.Clip,
                        padding: new RectOffset(15, 15, 20, 15));

                    #region 导入字体样式模版
                    if (XGUI.layout_button(
                        tooltip: "导入字体样式模版",
                        tex_release: import_r,
                        tex_press: import_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        ImportTextStyles();
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 导出字体样式模版
                    if (XGUI.layout_button(
                        tooltip: "导出字体样式模版",
                        tex_release: export_r,
                        tex_press: export_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        ExportTextStyles();
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 清空所有文字样式模版
                    if (XGUI.layout_button(
                        tooltip: "清空所有文字样式模版",
                        tex_release: clear_r,
                        tex_press: clear_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 文字样式库消息",
                            title: "清空所有字体样式",
                            msg: "是否清空所有字体样式项？请注意！如果您的场景中或是预制体中的文字组件用到了该字体样式库中的字体样式，清空后会导致组件的字体样式信息丢失，请谨慎操作！",
                            ok: "清空",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);

                        if (res == "暂不")
                            return;

                        sp_TextStyleLibrary.ClearArray();
                        sp_TextStyleLibrary.serializedObject.ApplyModifiedProperties();
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
                        TextStyleInfoList_Original_Add(sp_TextStyleLibraryList);
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
                        TextStyleInfoList_Original_Remove(sp_TextStyleLibraryList);
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                }
            }
            #endregion

            #region 样式列表
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "样式列表",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15));

            DrawTextStyleInfoList_Original();

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 列表绘制
        /// <summary>
        /// 根据高亮过滤条件屏蔽或启用 GUI。
        /// <para/>
        /// 若名称不包含 <see cref="sp_Highlight"/> 的当前值，则禁用 GUI。
        /// </summary>
        /// <param name="name">待检查的名称</param>
        private void BlockGUI(string name)
        {
            if (!name.Contains(sp_Highlight.stringValue))
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
        }
        /// <summary>
        /// 列表添加回调。
        /// <para/>
        /// 在 ReorderableList 完成添加后触发文字样式变更回调。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void TextStyleInfoList_Original_AddCallback(ReorderableList list)
        {
            if (BaseScript.act_on_TextStyleChanged != null)
                BaseScript.act_on_TextStyleChanged();
        }
        /// <summary>
        /// 列表移除回调。
        /// <para/>
        /// 在 ReorderableList 完成移除后触发文字样式变更回调。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void TextStyleInfoList_Original_RemoveCallback(ReorderableList list)
        {
            if (BaseScript.act_on_TextStyleChanged != null)
                BaseScript.act_on_TextStyleChanged();
        }
        /// <summary>
        /// 绘制元素回调。
        /// <para/>
        /// 绘制内容：
        /// <list type="bullet">
        /// <item><description>序号（灰色，D2 格式）；</description></item>
        /// <item><description>样式名称（含 T / M 类型标记）；</description></item>
        /// <item><description>样式解释；</description></item>
        /// <item><description>样式颜色与渐变色块（TmpText）；</description></item>
        /// <item><description>字体丢失警告图标；</description></item>
        /// <item><description>查看详细信息按钮；</description></item>
        /// <item><description>右键菜单（修改 / 获取 / 粘贴 / 克隆）；</description></item>
        /// <item><description>过滤遮罩与定位标记。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rect">绘制矩形</param>
        /// <param name="index">索引</param>
        /// <param name="isActive">是否激活</param>
        /// <param name="isFocused">是否聚焦</param>
        private void TextStyleInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_TextStyleLibrary.GetArrayElementAtIndex(index);

            SerializedProperty sp_name = prop.FindPropertyRelative("Name");
            SerializedProperty sp_type = prop.FindPropertyRelative("Type");
            SerializedProperty sp_tmpfont = prop.FindPropertyRelative("tmp_font");
            SerializedProperty sp_tmpgra = prop.FindPropertyRelative("gra_Used");
            SerializedProperty sp_tmp_gramode = prop.FindPropertyRelative("gra_ColorMode");
            SerializedProperty sp_tmp_gra_col_a = prop.FindPropertyRelative("gra_A");
            SerializedProperty sp_tmp_gra_col_b = prop.FindPropertyRelative("gra_B");
            SerializedProperty sp_tmp_gra_col_c = prop.FindPropertyRelative("gra_C");
            SerializedProperty sp_tmp_gra_col_d = prop.FindPropertyRelative("gra_D");
            SerializedProperty sp_font = prop.FindPropertyRelative("Font");
            SerializedProperty sp_descrpt = prop.FindPropertyRelative("Description");
            SerializedProperty sp_fontcolor = prop.FindPropertyRelative("FontColor");
            SerializedProperty sp_tmpfontcolor = prop.FindPropertyRelative("tmp_color");

            xHud_TextType ttype = (xHud_TextType)sp_type.enumValueIndex;

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            #region 序号
            drawelement_rect.Set(rect.x + 15, rect.y + 8, 30, 20);
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

            string h_color = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
            string t_color = XGUI_Utilitys.Color_To_HexString(XGUI.GetXGUIColor(XGUIColor.警示黄), true);
            string m_color = XGUI_Utilitys.Color_To_HexString(XGUI.GetXGUIColor(XGUIColor.工业蓝), true);

            #region 样式名称
            drawelement_rect.Set(rect.x + 35, rect.y + 8, rect.width - (string.IsNullOrEmpty(sp_Find.stringValue) ? 95 : 155), 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent($"<b><color={(ttype == xHud_TextType.Text ? t_color : m_color)}>{(ttype == xHud_TextType.Text ? "T " : "M")}</color></b>    <color=#ffffff>|</color>    <color={h_color}>{sp_name.stringValue}</color>"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion

            #region 样式解释
            drawelement_rect.Set(rect.x + 36, rect.y + 33, rect.width - (string.IsNullOrEmpty(sp_Find.stringValue) ? 130 : 160), 22);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(sp_descrpt.stringValue),
                text_color: Color.white * 0.75f,
                size: XGUIFontSize.S,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion

            #region 样式颜色
            drawelement_rect.Set(rect.width - 41.5f, rect.y + 35, 15, 2);
            XGUI.gui_box(drawelement_rect, ttype == xHud_TextType.Text ? sp_fontcolor.colorValue : sp_tmpfontcolor.colorValue);
            #endregion

            #region 渐变色块
            if (ttype == xHud_TextType.TmpText)
            {
                if (sp_tmpgra.boolValue)
                {
                    ColorMode col_mod = (ColorMode)sp_tmp_gramode.enumValueIndex;
                    switch (col_mod)
                    {
                        case ColorMode.Single:
                            drawelement_rect.Set(rect.width - 41.5f, rect.y + 45, 15, 2);
                            XGUI.gui_box(drawelement_rect, sp_tmp_gra_col_a.colorValue);
                            break;
                        case ColorMode.HorizontalGradient:
                            drawelement_rect.Set(rect.width - 41.5f, rect.y + 45, 10, 2);
                            XGUI.gui_box(drawelement_rect, sp_tmp_gra_col_a.colorValue);
                            drawelement_rect.Set(rect.width - 34, rect.y + 45, 7.5f, 2);
                            XGUI.gui_box(drawelement_rect, sp_tmp_gra_col_b.colorValue);
                            break;
                        case ColorMode.VerticalGradient:
                            drawelement_rect.Set(rect.width - 41.5f, rect.y + 45, 7.5f, 2);
                            XGUI.gui_box(drawelement_rect, sp_tmp_gra_col_a.colorValue);
                            drawelement_rect.Set(rect.width - 34, rect.y + 45, 7.5f, 2);
                            XGUI.gui_box(drawelement_rect, sp_tmp_gra_col_b.colorValue);
                            break;
                        case ColorMode.FourCornersGradient:
                            drawelement_rect.Set(rect.width - 41.5f, rect.y + 45, 3.75f, 2);
                            XGUI.gui_box(drawelement_rect, sp_tmp_gra_col_a.colorValue);
                            drawelement_rect.Set(rect.width - 37.75f, rect.y + 45, 3.75f, 2);
                            XGUI.gui_box(drawelement_rect, sp_tmp_gra_col_b.colorValue);
                            drawelement_rect.Set(rect.width - 34f, rect.y + 45, 3.75f, 2);
                            XGUI.gui_box(drawelement_rect, sp_tmp_gra_col_c.colorValue);
                            drawelement_rect.Set(rect.width - 30.25f, rect.y + 45, 3.75f, 2);
                            XGUI.gui_box(drawelement_rect, sp_tmp_gra_col_d.colorValue);
                            break;
                    }
                }
            }
            #endregion

            BlockGUI(sp_name.stringValue);

            #region 字体文件丢失判定
            bool missing_font = false;

            if (ttype == xHud_TextType.Text && sp_font.objectReferenceValue == null)
                missing_font = true;

            if (ttype == xHud_TextType.TmpText && sp_tmpfont.objectReferenceValue == null)
                missing_font = true;

            warning_content = new GUIContent();
            warning_content.image = missing_font ? warnIcon : null;
            warning_content.tooltip = (missing_font ? (ttype == xHud_TextType.Text ? "已丢失字体资源" : "已丢失Tmp字体资源") : null);
            drawelement_rect.Set(rect.x + 15, rect.y + 35, 15, 15);
            XGUI.gui_icon(
                rect: drawelement_rect,
                gui_content: warning_content,
                color: Color.white);
            #endregion

            #region 查看详细信息
            drawelement_rect.Set(rect.width - 45, rect.y + 4, 22, 22);
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
                XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
                info.CopyData(BaseScript.TextStyleLibrary[index]);
                Open_Hud_Library_TextStyle_Setter(info, index);
                return;
            }
            #endregion

            GUI.enabled = true;

            BlockGUI(sp_name.stringValue);

            drawelement_rect.Set(rect.x, rect.y, rect.width - 50, rect.height);
            // 检测鼠标点击事件
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1 && drawelement_rect.Contains(e.mousePosition))
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("E (修改样式)"), false, () =>
                {
                    XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
                    info.CopyData(BaseScript.TextStyleLibrary[index]);
                    Open_Hud_Library_TextStyle_Setter(info, index);
                    return;
                });
                menu.AddItem(new GUIContent("C (获取样式)"), false, () =>
                {
                    string json = JsonUtility.ToJson(BaseScript.TextStyleLibrary[index]);
                    XGUI.x_Editor_Data_Set_With_String("xData_XHud_TextStyle", json);

                    XGUI.dialog(
                        type: XGUIDialogType.确认,
                        windowtitle: "XHud - 文字样式库消息",
                        title: "文字样式复制",
                        msg: $"已将当前文字样式 {sp_name.stringValue} 存入 xData 中！",
                        ok: "好的",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.Theme_Primary);
                });
                menu.AddItem(new GUIContent("V (粘贴样式)"), false, () =>
                {
                    XHud_LibraryArg_TextStyle info = JsonUtility.FromJson<XHud_LibraryArg_TextStyle>(XGUI.x_Editor_Data_Get_With_String("xData_XHud_TextStyle"));

                    // 使用 Undo.RecordObject 来记录对目标对象的修改
                    Undo.RecordObject(BaseScript, "Paste Style");

                    BaseScript.TextStyleLibrary[index].CopyData_Ignored_LibraryToggle(info);

                    XGUI.dialog(
                        type: XGUIDialogType.确认,
                        windowtitle: "XHud - 文字样式库消息",
                        title: "文字样式粘贴",
                        msg: "已从 xData 中获取文字样式并覆盖到当前文字样式项！",
                        ok: "好的",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.Theme_Primary);
                });

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("S (克隆样式)"), false, () =>
                {
                    // 使用 Undo.RecordObject 来记录对目标对象的修改
                    Undo.RecordObject(sp_TextStyleLibrary.serializedObject.targetObject, "Clone Style");

                    sp_TextStyleLibrary.InsertArrayElementAtIndex(index);
                    sp_TextStyleLibrary.serializedObject.ApplyModifiedProperties();
                    Repaint();
                });
                // 显示右键菜单
                menu.ShowAsContext();

                e.Use(); // 标记事件已被处理，防止其他操作处理该事件
            }

            GUI.enabled = true;

            if (!sp_name.stringValue.Contains(sp_Highlight.stringValue))
            {
                drawelement_rect.Set(rect.x - 20, rect.y, rect.width + 20, rect.height);
                XGUI.gui_box(drawelement_rect, blocked_col);
            }

            if (index == sp_LocationSelectedIndex.intValue)
            {
                drawelement_rect.Set(rect.width - 105, rect.y + 8, 50, 20);
                XGUI.gui_label(
                    rect: drawelement_rect,
                    text: new GUIContent("已定位"),
                    text_color: XHud_Dashboard.Theme_Primary,
                    size: XGUIFontSize.S,
                    clipping: TextClipping.Clip,
                    anchor: TextAnchor.MiddleCenter,
                    offset: new Vector2(0, 0),
                    font_style: FontStyle.Normal);
            }
        }
        /// <summary>
        /// 绘制文字样式列表。
        /// <para/>
        /// 只绘制滚动视图可视区域内的元素，以优化大列表性能。
        /// 单击列表项时更新右侧预览信息（字体 / 颜色 / 样式 / 类型）。
        /// </summary>
        private void DrawTextStyleInfoList_Original()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visibleItemCount.intValue * sp_itemHeight.floatValue);
            sp_ColorInfoList_Original_Scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_ColorInfoList_Original_Scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, sp_TextStyleLibraryList.count * sp_itemHeight.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_ColorInfoList_Original_Scroller.vector2Value.y / sp_itemHeight.floatValue);
            int endIndex = Mathf.CeilToInt((sp_ColorInfoList_Original_Scroller.vector2Value.y + scrollview_rect.height) / sp_itemHeight.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < sp_TextStyleLibraryList.count; i++)
            {
                SerializedProperty prop = sp_TextStyleLibrary.GetArrayElementAtIndex(i);
                SerializedProperty sp_name = prop.FindPropertyRelative("Name");
                SerializedProperty sp_type = prop.FindPropertyRelative("Type");
                SerializedProperty sp_fontcolor = prop.FindPropertyRelative("FontColor");
                SerializedProperty sp_tmpfontcolor = prop.FindPropertyRelative("tmp_color");
                SerializedProperty sp_size = prop.FindPropertyRelative("Size");
                SerializedProperty sp_tmp_size = prop.FindPropertyRelative("tmp_size");
                SerializedProperty sp_font = prop.FindPropertyRelative("Font");
                SerializedProperty sp_Style = prop.FindPropertyRelative("Style");

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                // 如果当前元素被选中，绘制选中效果
                if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                {
                    if (sp_SelectedIndex.intValue == i)
                    {
                        // 高亮标记表示选中
                        item_rect.Set(1, i * sp_itemHeight.floatValue + 15, 5, 5);
                        //itemmark_rect = new Rect(item_rect.x + 1, item_rect.y + 15, 5, 5);
                        EditorGUI.DrawRect(item_rect, XHud_Dashboard.Theme_Primary);
                        // 高亮背景表示选中
                        item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width + 20, sp_itemHeight.floatValue);
                        //itemmarkBg_rect = new Rect(item_rect.x, item_rect.y, item_rect.width + 20, item_rect.height);
                        EditorGUI.DrawRect(item_rect, SelectedBg);
                        sws = true;
                    }
                }

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                sp_TextStyleLibraryList.drawElementCallback.Invoke(item_rect, i, i == sp_TextStyleLibraryList.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                        {
                            // 更新选中项
                            sp_SelectedIndex.intValue = i;

                            if ((xHud_TextType)sp_type.enumValueIndex == xHud_TextType.Text)
                            {
                                sp_PreviewFontColor.colorValue = sp_fontcolor.colorValue;
                                sp_PreviewTypeText.stringValue = "Hud Text";
                                sp_PreviewFontStyle.enumValueIndex = sp_Style.enumValueIndex;
                            }
                            else if ((xHud_TextType)sp_type.enumValueIndex == xHud_TextType.TmpText)
                            {
                                sp_PreviewFontColor.colorValue = sp_tmpfontcolor.colorValue;
                                sp_PreviewTypeText.stringValue = "Hud TmpText";
                                sp_PreviewFontStyle.enumValueIndex = (int)FontStyle.Normal;
                            }
                            sp_Preview_Font.objectReferenceValue = sp_font.objectReferenceValue;
                            sp_Preview_Font.serializedObject.ApplyModifiedProperties();

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
        /// 移除当前 ReorderableList 索引对应的文字样式项。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void TextStyleInfoList_Original_Remove(ReorderableList list)
        {
            if (list.count <= 0)
                return;
            sp_TextStyleLibrary.DeleteArrayElementAtIndex(list.index);
            sp_TextStyleLibrary.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 列表增加项。
        /// <para/>
        /// 弹窗询问用户创建 Text 还是 TmpText 类型的样式项，
        /// 并根据类型初始化全部默认参数（字体 / 尺寸 / 锚点 / 换行 / 富文本等）。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void TextStyleInfoList_Original_Add(ReorderableList list)
        {
            string res = XGUI.dialog(
                type: XGUIDialogType.警告,
                windowtitle: "XHud - 文字样式库消息",
                title: "创建样式类型",
                msg: "您希望创建那种文字样式模版？",
                ok: "Text",
                cancel: "TmpText",
                PrimaryIndex: 0,
                usemodal: true,
                themecolor: XHud_Dashboard.Theme_Primary);
            SerializedProperty prop = null;
            if (list.count <= 0)
            {
                sp_TextStyleLibrary.InsertArrayElementAtIndex(0);
                prop = sp_TextStyleLibrary.GetArrayElementAtIndex(0);
            }
            else
            {
                sp_TextStyleLibrary.InsertArrayElementAtIndex(list.count);
                prop = sp_TextStyleLibrary.GetArrayElementAtIndex(list.count - 1);
            }

            SerializedProperty sp_Name = prop.FindPropertyRelative("Name");
            SerializedProperty sp_Des = prop.FindPropertyRelative("Description");
            SerializedProperty sp_Type = prop.FindPropertyRelative("Type");

            string path = AssetDatabase.GetAssetPath(BaseScript);
            string font_Path = path.Substring(0, path.IndexOf("XHud") + 4);


            if (res == "Text")
                sp_Type.enumValueIndex = (int)xHud_TextType.Text;
            else
                sp_Type.enumValueIndex = (int)xHud_TextType.TmpText;

            xHud_TextType type = (xHud_TextType)sp_Type.enumValueIndex;

            sp_Name.stringValue = $"NewTextStyle_{list.index}_{type.ToString()}";
            sp_Des.stringValue = $"这里是样式说明文字内容...";

            if (type == xHud_TextType.Text)
            {
                ///---------------Text
                SerializedProperty sp_Font = prop.FindPropertyRelative("Font");
                SerializedProperty sp_Font_AssetPath = prop.FindPropertyRelative("Font_AssetPath");
                SerializedProperty sp_FontSize = prop.FindPropertyRelative("Size");
                SerializedProperty sp_FontStyle = prop.FindPropertyRelative("Style");
                SerializedProperty sp_ContentAnchor = prop.FindPropertyRelative("ContentAnchor");
                SerializedProperty sp_LineHeight = prop.FindPropertyRelative("LineHeight");
                SerializedProperty sp_Overflow_Horizon = prop.FindPropertyRelative("Overflow_Horizon");
                SerializedProperty sp_Overflow_Vertical = prop.FindPropertyRelative("Overflow_Vertical");
                SerializedProperty sp_RichText = prop.FindPropertyRelative("RichText");
                SerializedProperty sp_BestFit = prop.FindPropertyRelative("BestFit");
                SerializedProperty sp_Fit_Min = prop.FindPropertyRelative("Fit_Min");
                SerializedProperty sp_Fit_Max = prop.FindPropertyRelative("Fit_Max");
                SerializedProperty sp_Color = prop.FindPropertyRelative("FontColor");

                sp_Font.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Font>(Path.Combine(font_Path, "Fonts/Text/sx_light.otf"));
                sp_FontSize.floatValue = 50;
                sp_FontStyle.enumValueIndex = (int)FontStyle.Normal;
                sp_ContentAnchor.enumValueIndex = (int)ContentAnchor.中心;
                sp_LineHeight.floatValue = 1;
                sp_Overflow_Horizon.enumValueIndex = (int)HorizontalWrapMode.Overflow;
                sp_Overflow_Vertical.enumValueIndex = (int)VerticalWrapMode.Overflow;
                sp_RichText.boolValue = true;
                sp_BestFit.boolValue = false;
                sp_Fit_Min.intValue = 0;
                sp_Fit_Max.intValue = 100;
                sp_Color.colorValue = Color.white;
            }
            else
            {
                ///---------------TmpText
                SerializedProperty sp_tmp_rich = prop.FindPropertyRelative("tmp_rich");
                SerializedProperty sp_tmp_contentwrap = prop.FindPropertyRelative("tmp_contentwrap");
                SerializedProperty sp_tmp_overflow = prop.FindPropertyRelative("tmp_overflow");
                SerializedProperty sp_tmp_style = prop.FindPropertyRelative("tmp_style");
                SerializedProperty sp_tmp_anchor = prop.FindPropertyRelative("tmp_anchor");
                SerializedProperty sp_tmp_font = prop.FindPropertyRelative("tmp_font");
                SerializedProperty sp_tmp_fontasset_assetpath = prop.FindPropertyRelative("tmp_fontasset_assetpath");
                SerializedProperty sp_tmp_size = prop.FindPropertyRelative("tmp_size");
                SerializedProperty sp_tmp_space_character = prop.FindPropertyRelative("tmp_space_character");
                SerializedProperty sp_tmp_space_word = prop.FindPropertyRelative("tmp_space_word");
                SerializedProperty sp_tmp_space_lineheight = prop.FindPropertyRelative("tmp_space_lineheight");
                SerializedProperty sp_tmp_space_paragraph = prop.FindPropertyRelative("tmp_space_paragraph");
                SerializedProperty sp_tmp_WrappingRatios = prop.FindPropertyRelative("tmp_WrappingRatios");
                SerializedProperty sp_tmp_contentmargin = prop.FindPropertyRelative("tmp_contentmargin");
                SerializedProperty sp_tmp_color = prop.FindPropertyRelative("tmp_color");

                sp_tmp_rich.boolValue = true;
                sp_tmp_contentwrap.enumValueIndex = (int)TextWrappingModes.Normal;
                sp_tmp_overflow.enumValueIndex = (int)TextOverflowModes.Overflow;
                sp_tmp_style.enumValueIndex = (int)FontStyles.Normal;
                sp_tmp_anchor.enumValueIndex = (int)TmpContentAnchor.中心;
                sp_tmp_font.objectReferenceValue = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Path.Combine(font_Path, "Fonts/Tmp/sx_light SDF.asset"));
                sp_tmp_size.floatValue = 50;
                sp_tmp_space_character.floatValue = 0;
                sp_tmp_space_word.floatValue = 0;
                sp_tmp_space_lineheight.floatValue = 0;
                sp_tmp_space_paragraph.floatValue = 0;
                sp_tmp_WrappingRatios.floatValue = 0;
                sp_tmp_contentmargin.vector4Value = Vector4.zero;
                sp_tmp_color.colorValue = Color.white;
            }

            prop.serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 工具 - 文字样式库
        /// <summary>
        /// 导出文字样式表。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>创建 TextStyleLibrary / Dynamic / TmpAsset 目录结构；</description></item>
        /// <item><description>遍历样式库，将字体文件 / Tmp 字体资源拷贝到对应目录；</description></item>
        /// <item><description>将样式库序列化为 JSON 并写入磁盘。</description></item>
        /// </list>
        /// </summary>
        private void ExportTextStyles()
        {
            string path = EditorUtility.SaveFilePanel("", Application.dataPath, "TextStyleLibrary", "json");
            if (string.IsNullOrEmpty(path))
                return;

            string ext_path = Path.GetDirectoryName(path);
            string folder_path = Directory.CreateDirectory(ext_path + "/TextStyleLibrary").FullName;
            string dynamic_folder_path = Directory.CreateDirectory(ext_path + "/TextStyleLibrary/Dynamic").FullName;
            string tmpasset_folder_path = Directory.CreateDirectory(ext_path + "/TextStyleLibrary/TmpAsset").FullName;
            string filename = Path.GetFileName(path);
            string save_path = Path.Combine(folder_path, filename);

            ExportTextStyles styles = new ExportTextStyles();
            styles.TextStyleLibrary = BaseScript.TextStyleLibrary;

            for (int i = 0; i < styles.TextStyleLibrary.Count; i++)
            {
                if (styles.TextStyleLibrary[i].Type == xHud_TextType.Text)
                {
                    if (styles.TextStyleLibrary[i].Font != null)
                    {
                        string asset_path = AssetDatabase.GetAssetPath(styles.TextStyleLibrary[i].Font);
                        string ab_name = Path.GetFileName(asset_path);
                        string font_path = Path.Combine(dynamic_folder_path, ab_name);
                        string font_source_path = Application.dataPath + asset_path.Replace("Assets", "");
                        styles.TextStyleLibrary[i].Font_AssetPath = font_path;
                        File.Copy(font_source_path, font_path, true);
                    }
                }
                else
                {
                    if (styles.TextStyleLibrary[i].tmp_font != null)
                    {
                        string asset_path = AssetDatabase.GetAssetPath(styles.TextStyleLibrary[i].tmp_font);
                        string ab_name = Path.GetFileName(asset_path);
                        string font_path = Path.Combine(tmpasset_folder_path, ab_name);
                        string font_source_path = Application.dataPath + asset_path.Replace("Assets", "");
                        styles.TextStyleLibrary[i].tmp_fontasset_assetpath = font_path;
                        File.Copy(font_source_path, font_path, true);
                    }
                }
            }

            string json = JsonUtility.ToJson(styles);

            File.WriteAllText(save_path, json);
        }
        /// <summary>
        /// 导入文字样式表。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>弹窗询问用户选择"追加"或"替换"模式；</description></item>
        /// <item><description>打开文件选择面板选取 JSON 文件；</description></item>
        /// <item><description>解析并写入文字样式库列表；</description></item>
        /// <item><description>询问是否导入附带的字体文件 / Tmp 字体资源；</description></item>
        /// <item><description>将字体文件拷贝到用户指定目录并刷新资源数据库。</description></item>
        /// </list>
        /// </summary>
        private void ImportTextStyles()
        {
            string res = XGUI.dialog(
                type: XGUIDialogType.警告,
                windowtitle: "XHud - 文字样式库消息",
                title: "读取字体样式数据",
                msg: "根据您的需要选择导入字体样式数据的方式，如果是追加则会在当前字体样式库的基础上后续叠加导入的字体样式项，如果是替换则会完全替换当前字体样式库的所有字体样式项！",
                ok: "追加",
                cancel: "替换",
                alt: "暂不",
                PrimaryIndex: 2,
                usemodal: true,
                themecolor: XHud_Dashboard.Theme_Primary);

            if (res == "暂不")
            {
                return;
            }
            string path = EditorUtility.OpenFilePanel("请选择要导入的文字样式库Json文件", Application.dataPath, "json");

            if (string.IsNullOrEmpty(path))
                return;

            string filename = Path.GetFileNameWithoutExtension(path);

            ExportTextStyles styles = new ExportTextStyles();

            switch (res)
            {
                case "追加":
                    if (string.IsNullOrEmpty(path))
                    {
                        return;
                    }
                    else
                    {
                        string json = File.ReadAllText(path);
                        styles = JsonUtility.FromJson<ExportTextStyles>(json);

                        for (int i = 0; i < styles.TextStyleLibrary.Count; i++)
                        {
                            BaseScript.TextStyleLibrary.Add(styles.TextStyleLibrary[i]);
                        }
                    }
                    break;
                case "替换":
                    if (!string.IsNullOrEmpty(path))
                    {
                        string json = File.ReadAllText(path);
                        styles = JsonUtility.FromJson<ExportTextStyles>(json);
                        BaseScript.TextStyleLibrary = styles.TextStyleLibrary;
                    }
                    break;
            }

            string res_tp = XGUI.dialog(
                type: XGUIDialogType.警告,
                windowtitle: "XHud - 文字样式库消息",
                title: "导入字体",
                msg: "是否需要导入文字样式附带的Dynamic字体文件或者是Tmp字体资源？",
                ok: "暂不",
                cancel: "导入",
                PrimaryIndex: 1,
                usemodal: true,
                themecolor: XHud_Dashboard.Theme_Primary);
            if (res_tp == "不需要")
            {
                return;
            }

            string importfont_save_path = EditorUtility.SaveFolderPanel("字体文件或资源的保存路径", Application.dataPath, "");
            if (string.IsNullOrEmpty(importfont_save_path))
            {
                Repaint();

                AssetDatabase.Refresh();
                return;
            }

            string importfont_save_folder = importfont_save_path.Substring(importfont_save_path.IndexOf("Assets"));

            for (int i = 0; i < styles.TextStyleLibrary.Count; i++)
            {
                if (styles.TextStyleLibrary[i].Type == xHud_TextType.Text)
                {
                    string font_absolute_path = styles.TextStyleLibrary[i].Font_AssetPath;
                    string font_absolute_filename = Path.GetFileName(styles.TextStyleLibrary[i].Font_AssetPath);
                    string imp_font_filepath = Application.dataPath.Replace("Assets", "") + importfont_save_folder + "/" + font_absolute_filename;
                    if (File.Exists(font_absolute_path))
                        File.Copy(font_absolute_path, imp_font_filepath, true);
                }
                else
                {
                    string fontasset_absolute_path = styles.TextStyleLibrary[i].tmp_fontasset_assetpath;
                    string fontasset_absolute_filename = Path.GetFileName(styles.TextStyleLibrary[i].tmp_fontasset_assetpath);
                    string imp_fontasset_filepath = Application.dataPath.Replace("Assets", "") + importfont_save_folder + "/" + fontasset_absolute_filename;
                    if (File.Exists(fontasset_absolute_path))
                        File.Copy(fontasset_absolute_path, imp_fontasset_filepath, true);
                }
            }

            Repaint();
            AssetDatabase.Refresh();
        }
        #endregion

        #region 工具 - 文字样式修改器
        /// <summary>
        /// 打开文字样式修改器窗口。
        /// <para/>
        /// 将指定文字样式传入修改器，用于编辑样式名称、描述与全部字体参数。
        /// </summary>
        /// <param name="info">文字样式数据</param>
        /// <param name="index">索引</param>
        public void Open_Hud_Library_TextStyle_Setter(XHud_LibraryArg_TextStyle info, int index)
        {
            Editor_XHud_LibrarySetTool_TextStyle window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_TextStyle>(false);

            window.titleContent = new GUIContent("XHud - 文字样式修改器");

            XGUI.CenterEditorWindow(new Vector2Int(348, info.Type == xHud_TextType.Text ? 820 : 800), window);

            window.SetStyle(info);
            window.ModifiedIndex = index;
            window.SetOriginStyle(info);
            window.OriginLibName = info.Name;
            window.SetInfo(info.Name, info.Description);
            window.SetLibrarySetterMode(LibrarySetterMode.修改库源参数);
            window.SetButtonText("更新", "取消");
            window.SetTitle("文字样式修改器");
            window.Set_Target_Hud_TextStyleLibrary(BaseScript);
            //window.ShowModal();
            window.Show();
        }
        #endregion

        #region 预览
        /// <summary>
        /// 是否有可用的预览界面。
        /// <para/>
        /// 有列表项被选中时返回 true。
        /// </summary>
        /// <returns>true 表示可以显示预览</returns>
        public override bool HasPreviewGUI()
        {
            return sws;
        }
        /// <summary>
        /// 获取预览面板标题。
        /// </summary>
        /// <returns>预览标题</returns>
        public override GUIContent GetPreviewTitle()
        {
            return new GUIContent("HudTextStyle");
        }
        /// <summary>
        /// 绘制预览工具栏。
        /// </summary>
        public override void OnPreviewSettings()
        {
            base.OnPreviewSettings();
        }
        /// <summary>
        /// 绘制预览 GUI（静态）。
        /// <para/>
        /// 当前实现为空。
        /// </summary>
        /// <param name="rect">预览区域矩形</param>
        /// <param name="background">背景样式</param>
        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            //util_XHUDGUI.Gui_Icon(new Rect(rect.x, rect.y, rect.height * 0.75f, rect.height * 0.75f), util_XHUDGUI.GetIcon("Icon_Assets_Font_Iso"));
        }
        /// <summary>
        /// 绘制可交互预览 GUI。
        /// <para/>
        /// 使用当前样式的字体、颜色与预览内容绘制示例文字，
        /// 并在底部显示样式类型（Hud Text / Hud TmpText）。
        /// </summary>
        /// <param name="r">预览区域矩形</param>
        /// <param name="background">背景样式</param>
        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            XGUI.gui_label(
                rect: new Rect(r.x, r.y - 15, r.width, r.height),
                text: new GUIContent(sp_PreviewContent.stringValue),
                text_color: sp_PreviewFontColor.colorValue,
                size: XGUIFontSize.XL,
                clipping: clipping,
                anchor: TextAnchor.MiddleCenter,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal,
                font: (Font)sp_Preview_Font.objectReferenceValue);

            XGUI.gui_label(
                rect: new Rect((r.width / 2) - 50, r.height - 15, 100, 20),
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_color_gui: Color.white,
                text: new GUIContent(sp_PreviewTypeText.stringValue),
                text_color: Color.white * 0.75f,
                size: XGUIFontSize.S,
                clipping: clipping,
                anchor: TextAnchor.MiddleCenter,
                offset: new Vector2(0, -2),
                font_style: FontStyle.Normal);
        }
        #endregion
    }
    #endregion
}