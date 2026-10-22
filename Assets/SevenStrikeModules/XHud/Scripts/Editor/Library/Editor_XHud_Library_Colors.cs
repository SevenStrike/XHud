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
    /// 用于复制色卡信息的临时数据结构。
    /// <para/>
    /// 作为编辑器剪贴板数据载体，通过 JSON 序列化在色卡库与图元配色器之间传递。
    /// </summary>
    [System.Serializable]
    public class CopyHudColor
    {
        /// <summary>
        /// 数据类型标识。
        /// <para/>
        /// 固定为 <c>"HudCopyColor"</c>，用于接收端校验数据来源。
        /// </summary>
        public string type;
        /// <summary>
        /// 颜色值（RGBA 格式字符串）。
        /// <para/>
        /// 格式：<c>r,g,b,a</c>。
        /// </summary>
        public string color;
        /// <summary>
        /// 色卡名称。
        /// </summary>
        public string name;
        /// <summary>
        /// 色卡在库中的索引。
        /// </summary>
        public int index;
    }
    /// <summary>
    /// 导出色卡时的单项数据。
    /// <para/>
    /// 每条记录包含名称、颜色值字符串与十六进制表示。
    /// </summary>
    [System.Serializable]
    public class ExportItem
    {
        /// <summary>
        /// 颜色值字符串。
        /// <para/>
        /// 格式：<c>r,g,b,a</c>。
        /// </summary>
        public string color;
        /// <summary>
        /// 色卡名称。
        /// </summary>
        public string name;
        /// <summary>
        /// 十六进制颜色字符串。
        /// </summary>
        public string hex;
    }
    /// <summary>
    /// 导出色卡库的完整数据结构。
    /// <para/>
    /// 序列化为 JSON 后写入磁盘，用于色卡库的导入 / 导出。
    /// </summary>
    [System.Serializable]
    public class ExportColorLib
    {
        /// <summary>
        /// 色卡信息列表。
        /// </summary>
        public List<ExportItem> ColorsInfo;
    }
    #endregion

    #region 编辑器主体
    /// <summary>
    /// XHud 色卡库的自定义编辑器。
    /// <para/>
    /// 提供色卡列表的可视化编辑、右键菜单操作、导入 / 导出、批量生成等功能。
    /// </summary>
    [CustomEditor(typeof(XHud_Library_Colors))]
    public class Editor_XHud_Library_Colors : Editor
    {
        #region 字段 - 组件 / 列表
        /// <summary>
        /// 目标脚本实例。
        /// <para/>
        /// 在 <see cref="OnEnable"/> 中缓存。
        /// </summary>
        private XHud_Library_Colors BaseScript;
        /// <summary>
        /// 颜色列表（可重排列表）。
        /// <para/>
        /// 用于驱动 ReorderableList 的绘制与交互。
        /// </summary>
        public ReorderableList ColorInfoList_Original;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 列表项高度。
        /// </summary>
        private SerializedProperty sp_itemHeight;
        /// <summary>
        /// 可见列表项数量。
        /// </summary>
        private SerializedProperty sp_visibleItemCount;
        /// <summary>
        /// 色卡列表滚动位置。
        /// </summary>
        private SerializedProperty sp_ColorInfoList_Original_Scroller;
        /// <summary>
        /// 定位选中索引。
        /// </summary>
        private SerializedProperty sp_LocationSelectedIndex;
        /// <summary>
        /// 当前选中索引。
        /// </summary>
        private SerializedProperty sp_SelectedIndex;
        /// <summary>
        /// 原始色卡库列表。
        /// </summary>
        private SerializedProperty sp_ColorLibrary_Original;
        /// <summary>
        /// 精确查找字符串。
        /// </summary>
        private SerializedProperty sp_Find;
        /// <summary>
        /// 过滤高亮字符串。
        /// </summary>
        private SerializedProperty sp_Highlight;
        /// <summary>
        /// 库名称。
        /// </summary>
        private SerializedProperty sp_LibraryName;
        #endregion

        #region 字段 - 预览
        /// <summary>
        /// 选中的颜色。
        /// </summary>
        private Color SelectedColor;
        /// <summary>
        /// 颜色信息文本。
        /// </summary>
        private string ColorInfo;
        /// <summary>
        /// 预览图片数组。
        /// </summary>
        private Texture2D[] ReferImages;
        /// <summary>
        /// 预览标题文字。
        /// </summary>
        public string PreviewHeader = "HudColorPreview";
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 调色板图标。
        /// </summary>
        private Texture2D PalletIcon;
        /// <summary>
        /// 导入按钮按下图标。
        /// </summary>
        private Texture2D import_p;
        /// <summary>
        /// 导入按钮释放图标。
        /// </summary>
        private Texture2D import_r;
        /// <summary>
        /// 导出按钮按下图标。
        /// </summary>
        private Texture2D export_p;
        /// <summary>
        /// 导出按钮释放图标。
        /// </summary>
        private Texture2D export_r;
        /// <summary>
        /// 清空按钮按下图标。
        /// </summary>
        private Texture2D clear_p;
        /// <summary>
        /// 清空按钮释放图标。
        /// </summary>
        private Texture2D clear_r;
        /// <summary>
        /// 创建按钮按下图标。
        /// </summary>
        private Texture2D create_p;
        /// <summary>
        /// 创建按钮释放图标。
        /// </summary>
        private Texture2D create_r;
        /// <summary>
        /// 删除按钮按下图标。
        /// </summary>
        private Texture2D delete_p;
        /// <summary>
        /// 删除按钮释放图标。
        /// </summary>
        private Texture2D delete_r;
        /// <summary>
        /// 随机按钮按下图标。
        /// </summary>
        private Texture2D random_p;
        /// <summary>
        /// 随机按钮释放图标。
        /// </summary>
        private Texture2D random_r;
        #endregion

        #region 字段 - 参数
        /// <summary>
        /// 多倍尺寸。
        /// <para/>
        /// 用于导出色板 PNG 时每个色块的像素宽度。
        /// </summary>
        private int MultipleSize = 16;
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
        /// <item><description>构造 ReorderableList。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            BaseScript = (XHud_Library_Colors)target;

            #region 获取序列化属性
            sp_ColorLibrary_Original = serializedObject.FindProperty("ColorLibrary");
            sp_LibraryName = serializedObject.FindProperty("LibraryName");
            sp_Highlight = serializedObject.FindProperty("Highlight");
            sp_SelectedIndex = serializedObject.FindProperty("SelectedIndex");
            sp_LocationSelectedIndex = serializedObject.FindProperty("LocationSelectedIndex");
            sp_itemHeight = serializedObject.FindProperty("itemHeight");
            sp_visibleItemCount = serializedObject.FindProperty("visibleItemCount");
            sp_ColorInfoList_Original_Scroller = serializedObject.FindProperty("ColorInfoList_Original_Scroller");
            sp_Find = serializedObject.FindProperty("Find");
            #endregion

            SelectedColor = Color.white;

            blocked_col = new Color(0, 0, 0, blocked_alp);

            #region 获取图标
            import_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_colors/import_p");
            import_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_colors/import_r");
            export_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_colors/export_p");
            export_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_colors/export_r");
            clear_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_colors/clear_p");
            clear_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_colors/clear_r");
            create_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_colors/create_p");
            create_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_colors/create_r");
            delete_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_colors/delete_p");
            delete_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_colors/delete_r");
            random_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_colors/random_p");
            random_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_colors/random_r");
            PalletIcon = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_colors/palletmark");
            #endregion

            //设定列表项高度值
            sp_itemHeight.floatValue = 30;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 15;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            #region ReorderableList - ColorInfoList
            ColorInfoList_Original = new ReorderableList(serializedObject, sp_ColorLibrary_Original, true, true, true, true);
            ColorInfoList_Original.drawElementCallback = ColorInfoList_Original_DrawElementCallback;
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
            }
        }
        /// <summary>
        /// 绘制 Inspector 主入口。
        /// <para/>
        /// 按自上而下的顺序绘制所有面板：
        /// 标题 → 参数 → 快捷功能 → 色卡列表。
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
                title_text: "XHud  -  色卡库",
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
                prop: sp_LibraryName,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 过滤（包含）
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
                title: "过滤（包含）",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
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
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 90,
                prop: sp_Find,
                prop_margin: new RectOffset(0, 0, 5, 0));
            if (XGUI.ChangedCheck_End())
            {
                if (!string.IsNullOrEmpty(sp_Find.stringValue))
                    BaseScript.ColorsLibrary_Location_Find(sp_Find.stringValue);
                else
                {
                    sp_SelectedIndex.intValue = -1;
                    sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

                    sp_LocationSelectedIndex.intValue = -1;
                    sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();
                }
            }
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
                        bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                        title: "快捷功能",
                        title_size: XGUIFontSize.M,
                        title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_clipping: TextClipping.Clip,
                        padding: new RectOffset(15, 15, 20, 15));

                    #region 读取色卡
                    if (XGUI.layout_button(
                        tooltip: "读取色卡",
                        tex_release: import_r,
                        tex_press: import_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        LoadPallets();
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 导出色卡
                    if (XGUI.layout_button(
                        tooltip: "导出色卡",
                        tex_release: export_r,
                        tex_press: export_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        ExportPallets();
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 清空色卡
                    if (XGUI.layout_button(
                        tooltip: "清空色卡",
                        tex_release: clear_r,
                        tex_press: clear_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 色卡库消息",
                            title: "清空所有色卡",
                            msg: "是否清空所有色卡项？请注意！如果您的场景中或是预制体中的图元配色器用到了该色卡库中的色卡，清空后会导致图元配色器的色卡信息丢失，请谨慎操作！",
                            ok: "清空",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        if (res == "暂不")
                            return;
                        sp_ColorLibrary_Original.ClearArray();
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 添加色卡项
                    if (XGUI.layout_button(
                        tooltip: "添加色卡项",
                        tex_release: create_r,
                        tex_press: create_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        ColorInfoList_Original_Add(ColorInfoList_Original);
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 删除色卡项
                    if (XGUI.layout_button(
                        tooltip: "删除色卡项",
                        tex_release: delete_r,
                        tex_press: delete_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        ColorInfoList_Original_Remove(ColorInfoList_Original);
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 生成常用色卡颜色
                    if (XGUI.layout_button(
                        tooltip: "生成常用色卡颜色",
                        tex_release: random_r,
                        tex_press: random_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        string res = XGUI.dialog(
                           type: XGUIDialogType.警告,
                           windowtitle: "XHud - 色卡库消息",
                           title: "生成随机色卡",
                           msg: "是否要为色卡库随机生成一套颜色？生成后会覆盖当前的所有色卡项，请谨慎操作！",
                           ok: "暂不",
                           cancel: "基础色",
                           alt: "高级灰",
                           other: "通用色",
                           PrimaryIndex: 1,
                           usemodal: true,
                           themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                        if (res == "暂不")
                            return;

                        string res_m = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 色卡库消息",
                            title: "生成随机色卡",
                            msg: "请选择生成模式！如果选择替换会覆盖当前的所有色卡项，请谨慎操作！",
                            ok: "追加",
                            cancel: "替换",
                            PrimaryIndex: 1,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                        switch (res)
                        {
                            case "基础色":
                                BaseScript.ColorsLibrary_CreateColors_BaseColor(res_m);
                                break;
                            case "高级灰":
                                BaseScript.ColorsLibrary_CreateColors_AdvancedGrayColor(res_m);
                                break;
                            case "通用色":
                                BaseScript.ColorsLibrary_CreateColors_General(res_m);
                                break;
                        }
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                }
            }
            #endregion

            #region 色卡列表
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "色卡列表",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15));

            DrawColorInfoList_Original();

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
        /// 绘制元素回调。
        /// <para/>
        /// 绘制内容：
        /// <list type="bullet">
        /// <item><description>序号（灰色，D2 格式）；</description></item>
        /// <item><description>色卡名称（以颜色值着色）；</description></item>
        /// <item><description>颜色拾取器；</description></item>
        /// <item><description>右键菜单（获取 / 修改 / 克隆 / 拷贝等）；</description></item>
        /// <item><description>过滤遮罩与定位标记。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rect">绘制矩形</param>
        /// <param name="index">索引</param>
        /// <param name="isActive">是否激活</param>
        /// <param name="isFocused">是否聚焦</param>
        private void ColorInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_ColorLibrary_Original.GetArrayElementAtIndex(index);

            SerializedProperty sp_name = prop.FindPropertyRelative("Name");
            SerializedProperty sp_color = prop.FindPropertyRelative("Color");

            #region 序号
            drawelement_rect.Set(rect.x + 20, rect.y + 4, 30, 20);
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

            #region 图标
            //drawelement_rect.Set(rect.x + 2, rect.y + 3, 12, 12);
            //XGUI.gui_icon(
            //    rect: drawelement_rect,
            //    icon: PalletIcon,
            //    color: sp_color.colorValue);
            #endregion

            BlockGUI(sp_name.stringValue);

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            #region 名称
            drawelement_rect.Set(rect.x + 42, rect.y + 4, rect.width - 160, 20);
            XGUI.gui_label(
              rect: drawelement_rect,
              text: new GUIContent(sp_name.stringValue),
              text_color: sp_color.colorValue,
              size: XGUIFontSize.M,
              clipping: clipping,
              anchor: TextAnchor.MiddleLeft,
              offset: new Vector2(0, 0),
              font_style: FontStyle.Normal);
            #endregion

            BlockGUI(sp_name.stringValue);

            #region 颜色
            drawelement_rect.Set(rect.width - 80, rect.y + 6, 60, 20);
            sp_color.colorValue = XGUI.gui_colorfield(
                rect: drawelement_rect,
                prop: sp_color.colorValue,
                icon: null,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(sp_color.colorValue)}",
                state_value_color: sp_color.colorValue);

            sp_color.serializedObject.ApplyModifiedProperties();
            #endregion

            BlockGUI(sp_name.stringValue);

            #region 检测鼠标点击事件
            Event e = Event.current;
            drawelement_rect.Set(rect.x, rect.y, rect.width - 50, rect.height);
            if (e.type == EventType.MouseDown && e.button == 1 && drawelement_rect.Contains(e.mousePosition))
            {
                // 更新选中项
                sp_SelectedIndex.intValue = index;

                SelectedColor = sp_color.colorValue;

                ColorInfo = "R: " + SelectedColor.r.ToString("F2") + " | G: " + SelectedColor.g.ToString("F2") + " | B: " + SelectedColor.b.ToString("F2");

                PreviewHeader = sp_name.stringValue;

                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("S (获取色卡信息)"), false, () =>
                {
                    CopyHudColor chc = new CopyHudColor();
                    chc.color = sp_color.colorValue.r + "," + sp_color.colorValue.g + "," + sp_color.colorValue.b + "," + sp_color.colorValue.a;
                    chc.index = index;
                    chc.name = sp_name.stringValue;
                    chc.type = "HudCopyColor";
                    string json = JsonUtility.ToJson(chc);

                    XGUI.x_Editor_Data_Set_With_String("xData_ColorInfo", json);

                    string hexcol = XGUI_Utilitys.Color_To_HexString(sp_color.colorValue, true);

                    XGUI.dialog(
                       type: XGUIDialogType.确认,
                       windowtitle: "XHud - 色卡库消息",
                       title: "获取色卡信息",
                       msg: $"<color={hexcol}>{sp_name.stringValue} </color>色卡信息已就绪！请选择需要识别的图元配色器后右键菜单点击识别色卡即可应用该色卡颜色！",
                       ok: "明白",
                       PrimaryIndex: 0,
                       usemodal: true,
                       themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                });
                menu.AddItem(new GUIContent("E (修改色卡信息)"), false, () =>
                {
                    xHud_LibraryArg_Color info = new xHud_LibraryArg_Color();
                    info.Name = sp_name.stringValue;
                    info.Color = sp_color.colorValue;
                    OpenLibrarySetTool(info, sp_name.stringValue, index);
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("C (克隆色卡项)"), false, () =>
                {
                    sp_ColorLibrary_Original.InsertArrayElementAtIndex(index);
                    sp_ColorLibrary_Original.serializedObject.ApplyModifiedProperties();
                    Repaint();
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("D (拷贝色卡的 Hex 颜色)"), false, () =>
                {
                    GUIUtility.systemCopyBuffer = XGUI_Utilitys.Color_To_HexString(sp_color.colorValue, true).Split(new char[1] { '#' })[1];

                    string hexcol = XGUI_Utilitys.Color_To_HexString(sp_color.colorValue, true);

                    XGUI.dialog(
                        type: XGUIDialogType.确认,
                        windowtitle: "XHud - 色卡库消息",
                        title: "获取色卡信息",
                        msg: $"<color={hexcol}>{sp_name.stringValue} </color>Hex 色卡信息 {hexcol} 已拷贝到系统剪贴板！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                });
                menu.AddItem(new GUIContent("A (拷贝色卡的 R G B 颜色)"), false, () =>
                {
                    string hexcol = XGUI_Utilitys.Color_To_HexString(sp_color.colorValue, true);

                    string mode = XGUI.dialog(
                        type: XGUIDialogType.帮助,
                        windowtitle: "XHud - 色卡库消息",
                        title: "获取色卡信息",
                        msg: $"请选择您要获取色卡 <color={hexcol}>{sp_name.stringValue} </color>的 R G B 信息的模式！",
                        ok: "色卡值",
                        cancel: "代码块",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                    if (mode == "代码块")
                    {
                        string color = $"Color col = new Color({sp_color.colorValue.r}f, {sp_color.colorValue.g}f, {sp_color.colorValue.b}f);";
                        GUIUtility.systemCopyBuffer = color;
                        XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 色卡库消息",
                            title: "获取色卡信息",
                            msg: $"<color={hexcol}>{sp_name.stringValue} </color>R G B 色卡代码块 {color} 已拷贝到系统剪贴板！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                    else
                    {
                        string rgb = $"{sp_color.colorValue.r},{sp_color.colorValue.g},{sp_color.colorValue.b}";
                        GUIUtility.systemCopyBuffer = rgb;
                        XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 色卡库消息",
                            title: "获取色卡信息",
                            msg: $"<color={hexcol}>{sp_name.stringValue} </color>R G B 色卡信息 {rgb} 已拷贝到系统剪贴板！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                });
                menu.AddItem(new GUIContent("X (拷贝色卡的 R G B A 颜色)"), false, () =>
                {
                    string hexcol = XGUI_Utilitys.Color_To_HexString(sp_color.colorValue, true);

                    string mode = XGUI.dialog(
                          type: XGUIDialogType.帮助,
                          windowtitle: "XHud - 色卡库消息",
                          title: "获取色卡信息",
                          msg: $"请选择您要获取色卡 <color={hexcol}>{sp_name.stringValue} </color>的 R G B A 信息的模式！",
                          ok: "色卡值",
                          cancel: "代码块",
                          PrimaryIndex: 0,
                          usemodal: true,
                          themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                    if (mode == "代码块")
                    {
                        string color = $"Color col = new Color({sp_color.colorValue.r}f, {sp_color.colorValue.g}f, {sp_color.colorValue.b}f, {sp_color.colorValue.a}f);";
                        GUIUtility.systemCopyBuffer = color;
                        XGUI.dialog(
                           type: XGUIDialogType.确认,
                           windowtitle: "XHud - 色卡库消息",
                           title: "获取色卡信息",
                           msg: $"<color={hexcol}>{sp_name.stringValue} </color>R G B A 色卡代码块 {color} 已拷贝到系统剪贴板！",
                           ok: "明白",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                    else
                    {
                        string rgba = $"{sp_color.colorValue.r},{sp_color.colorValue.g},{sp_color.colorValue.b},{sp_color.colorValue.a}";
                        GUIUtility.systemCopyBuffer = rgba;
                        XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 色卡库消息",
                            title: "获取色卡信息",
                            msg: $"<color={hexcol}>{sp_name.stringValue} </color>R G B A 色卡信息 {rgba} 已拷贝到系统剪贴板！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                });
                // 显示右键菜单
                menu.ShowAsContext();

                e.Use(); // 标记事件已被处理，防止其他操作处理该事件
            }

            GUI.enabled = true;
            #endregion

            if (!sp_name.stringValue.Contains(sp_Highlight.stringValue))
            {
                drawelement_rect.Set(rect.x - 20, rect.y, rect.width + 20, rect.height);
                XGUI.gui_box(drawelement_rect, blocked_col);
            }

            if (index == sp_LocationSelectedIndex.intValue)
            {
                drawelement_rect.Set(rect.width - 145, rect.y + 5, 50, 20);
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
        /// 绘制色卡列表。
        /// <para/>
        /// 只绘制滚动视图可视区域内的元素，以优化大列表性能。
        /// </summary>
        private void DrawColorInfoList_Original()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visibleItemCount.intValue * sp_itemHeight.floatValue);
            sp_ColorInfoList_Original_Scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_ColorInfoList_Original_Scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, ColorInfoList_Original.count * sp_itemHeight.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_ColorInfoList_Original_Scroller.vector2Value.y / sp_itemHeight.floatValue);
            int endIndex = Mathf.CeilToInt((sp_ColorInfoList_Original_Scroller.vector2Value.y + scrollview_rect.height) / sp_itemHeight.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < ColorInfoList_Original.count; i++)
            {
                SerializedProperty prop = sp_ColorLibrary_Original.GetArrayElementAtIndex(i);
                SerializedProperty sp_color = prop.FindPropertyRelative("Color");
                SerializedProperty sp_name = prop.FindPropertyRelative("Name");

                // 如果当前元素被选中，绘制选中效果
                if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                {
                    if (sp_SelectedIndex.intValue == i)
                    {
                        // 高亮标记表示选中
                        item_rect.Set(2, i * sp_itemHeight.floatValue + 9, 12, 12);

                        #region 图标
                        XGUI.gui_icon(
                            rect: item_rect,
                            icon: PalletIcon,
                            color: sp_color.colorValue);
                        #endregion

                        // 高亮背景表示选中
                        item_rect.Set(1, i * sp_itemHeight.floatValue, scrollview_rect.width + 20, sp_itemHeight.floatValue);
                        EditorGUI.DrawRect(item_rect, SelectedBg);
                    }
                }

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                ColorInfoList_Original.drawElementCallback.Invoke(item_rect, i, i == ColorInfoList_Original.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                        {
                            // 更新选中项
                            sp_SelectedIndex.intValue = i;

                            SelectedColor = sp_color.colorValue;

                            ColorInfo = "R: " + SelectedColor.r.ToString("F2") + " | G: " + SelectedColor.g.ToString("F2") + " | B: " + SelectedColor.b.ToString("F2");

                            PreviewHeader = sp_name.stringValue;

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
        /// 移除当前 ReorderableList 索引对应的色卡项。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void ColorInfoList_Original_Remove(ReorderableList list)
        {
            if (list.count <= 0)
                return;
            sp_ColorLibrary_Original.DeleteArrayElementAtIndex(list.index);
        }
        /// <summary>
        /// 列表增加项。
        /// <para/>
        /// 若列表为空则直接向底层集合追加；否则在当前位置插入新元素并初始化默认值。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void ColorInfoList_Original_Add(ReorderableList list)
        {
            if (list.count <= 0)
            {
                if (BaseScript.ColorLibrary == null)
                    BaseScript.ColorLibrary = new List<xHud_LibraryArg_Color>();
                BaseScript.ColorLibrary.Add(new XHud.xHud_LibraryArg_Color("颜色" + BaseScript.ColorLibrary.Count, Color.white));
            }
            else
            {
                sp_ColorLibrary_Original.InsertArrayElementAtIndex(list.index);
                SerializedProperty prop = sp_ColorLibrary_Original.GetArrayElementAtIndex(list.index);
                SerializedProperty prop_name = prop.FindPropertyRelative("Name");
                SerializedProperty prop_color = prop.FindPropertyRelative("Color");
                SerializedProperty prop_des = prop.FindPropertyRelative("Description");

                prop_name.stringValue = "颜色" + sp_ColorLibrary_Original.arraySize;
                prop_color.colorValue = Color.white;
                prop_des.stringValue = "颜色文字说明";

                prop.serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion

        #region 工具 - 色卡库
        /// <summary>
        /// 打开色卡修改器窗口。
        /// <para/>
        /// 将指定色卡的名称与颜色传入修改器，并设置为"修改库源参数"模式。
        /// </summary>
        /// <param name="info">色卡信息</param>
        /// <param name="originname">原始名称</param>
        /// <param name="index">索引</param>
        public void OpenLibrarySetTool(xHud_LibraryArg_Color info, string originname, int index)
        {
            Editor_XHud_LibrarySetTool_Color window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Color>(false);

            window.titleContent = new GUIContent("XHud - 色卡修改器");
            XGUI.CenterEditorWindow(new Vector2Int(348, 730), window, true);

            window.SetInfo(info.Name, info.Color);
            window.ModifiedIndex = index;
            window.SetLibrarySetterMode(LibrarySetterMode.修改库源参数);
            window.Set_OriginColorName(originname);
            window.SetButtonText("更新", "取消");
            window.SetTitle("色卡修改器");
            window.SetTarget_Hud_ColorsLibrary(BaseScript);
            //window.ShowModal();
            window.Show();
        }
        /// <summary>
        /// 导出色板（PNG 图片 + JSON 数据）。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>将色卡库渲染为一张横向色板贴图；</description></item>
        /// <item><description>保存为 PNG 文件；</description></item>
        /// <item><description>将色卡信息序列化为 JSON 并保存到同目录。</description></item>
        /// </list>
        /// </summary>
        private void ExportPallets()
        {
            if (BaseScript.ColorLibrary != null && BaseScript.ColorLibrary.Count > 0)
            {
                Texture2D Pallet = new Texture2D(MultipleSize * BaseScript.ColorLibrary.Count, MultipleSize, TextureFormat.ARGB32, true, true);
                for (int i = 0; i < BaseScript.ColorLibrary.Count; i++)
                {
                    for (int x = MultipleSize * i; x < MultipleSize * (i + 1); x++)
                    {
                        for (int y = 0; y < MultipleSize; y++)
                        {
                            Pallet.SetPixel(x, y, BaseScript.ColorLibrary[i].Color);
                        }
                    }
                }
                Pallet.Apply();

                byte[] PalletData = Pallet.EncodeToPNG();
                string path = EditorUtility.SaveFilePanel("", Application.dataPath, "Pallet", "png");

                if (string.IsNullOrEmpty(path))
                    return;

                if (!string.IsNullOrEmpty(path))
                {
                    FileStream Fs = File.Open(path, FileMode.OpenOrCreate);
                    Fs.Write(PalletData, 0, PalletData.Length);
                    Fs.Close();

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                ExportColorLib lib = new ExportColorLib();
                lib.ColorsInfo = new List<ExportItem>();

                for (int i = 0; i < BaseScript.ColorLibrary.Count; i++)
                {
                    ExportItem item = new ExportItem();
                    item.color = XGUI_Utilitys.Color_To_String(BaseScript.ColorLibrary[i].Color);
                    item.hex = XGUI_Utilitys.Color_To_HexString(BaseScript.ColorLibrary[i].Color);
                    item.name = BaseScript.ColorLibrary[i].Name;

                    lib.ColorsInfo.Add(item);
                }

                string json = JsonUtility.ToJson(lib, true);

                string pathjson = Path.GetDirectoryName(path);
                string filename = Path.GetFileNameWithoutExtension(path);
                File.WriteAllText(pathjson + "/" + filename + ".json", json);
            }
        }
        /// <summary>
        /// 读取色板 JSON 数据（支持追加或替换模式）。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>弹窗询问用户选择"追加"或"替换"模式；</description></item>
        /// <item><description>打开文件选择面板选取 JSON 文件；</description></item>
        /// <item><description>解析并写入色卡库列表。</description></item>
        /// </list>
        /// </summary>
        private void LoadPallets()
        {
            string path = "";

            string res = XGUI.dialog(
                          type: XGUIDialogType.警告,
                          windowtitle: "XHud - 色卡库消息",
                          title: "读取色卡数据",
                          msg: "根据您的需要选择导入色卡数据的方式，如果是追加则会在当前色卡库的基础上后续叠加导入的色卡项，如果是替换则会完全替换当前色卡库的所有色卡项！",
                          ok: "追加",
                          cancel: "替换",
                          alt: "暂不",
                          PrimaryIndex: 2,
                          usemodal: true,
                          themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
            if (res == "暂不")
                return;

            switch (res)
            {
                case "追加":
                    path = EditorUtility.OpenFilePanel("读取色板信息文件", Application.dataPath, "json");
                    if (!string.IsNullOrEmpty(path))
                    {

                        string json = File.ReadAllText(path);

                        ExportColorLib lib = JsonUtility.FromJson<ExportColorLib>(json);
                        int count = lib.ColorsInfo.Count;

                        int size = sp_ColorLibrary_Original.arraySize;

                        for (int i = 0; i < count; i++)
                        {
                            sp_ColorLibrary_Original.InsertArrayElementAtIndex(i + size);

                            SerializedProperty sp_item = sp_ColorLibrary_Original.GetArrayElementAtIndex(i + size);
                            SerializedProperty sp_name = sp_item.FindPropertyRelative("Name");
                            SerializedProperty sp_color = sp_item.FindPropertyRelative("Color");

                            sp_name.stringValue = lib.ColorsInfo[i].name;
                            sp_color.colorValue = XGUI_Utilitys.String_To_Color(lib.ColorsInfo[i].color, false);

                            sp_name.serializedObject.ApplyModifiedProperties();
                            sp_color.serializedObject.ApplyModifiedProperties();
                            sp_item.serializedObject.ApplyModifiedProperties();
                        }
                        sp_ColorLibrary_Original.serializedObject.ApplyModifiedProperties();
                    }
                    break;
                case "替换":
                    path = EditorUtility.OpenFilePanel("读取色板信息文件", Application.dataPath, "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        sp_ColorLibrary_Original.ClearArray();

                        string json = File.ReadAllText(path);

                        ExportColorLib lib = JsonUtility.FromJson<ExportColorLib>(json);
                        int count = lib.ColorsInfo.Count;

                        for (int i = 0; i < count; i++)
                        {
                            sp_ColorLibrary_Original.InsertArrayElementAtIndex(i);

                            SerializedProperty sp_item = sp_ColorLibrary_Original.GetArrayElementAtIndex(i);
                            SerializedProperty sp_name = sp_item.FindPropertyRelative("Name");
                            SerializedProperty sp_color = sp_item.FindPropertyRelative("Color");

                            sp_name.stringValue = lib.ColorsInfo[i].name;
                            sp_color.colorValue = XGUI_Utilitys.String_To_Color(lib.ColorsInfo[i].color, false);
                            sp_name.serializedObject.ApplyModifiedProperties();
                            sp_color.serializedObject.ApplyModifiedProperties();
                            sp_item.serializedObject.ApplyModifiedProperties();
                        }
                        sp_ColorLibrary_Original.serializedObject.ApplyModifiedProperties();
                    }
                    break;
            }
        }
        #endregion
    }
    #endregion
}