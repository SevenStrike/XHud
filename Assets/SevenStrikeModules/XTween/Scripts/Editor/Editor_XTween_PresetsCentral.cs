/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XTween - Unity 高性能动画架构插件
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
namespace SevenStrikeModules.XTween.Editor
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    [Serializable]
    public class PresetItemGUIStruct
    {
        public XTweenPresetBase preset;
        public XTweenTypes type;
        public bool isPressing;

        public PresetItemGUIStruct Clone()
        {
            PresetItemGUIStruct g = new PresetItemGUIStruct();
            g.type = type;
            g.isPressing = isPressing;
            g.preset = preset.Clone();

            return g;
        }
    }

    public class Editor_XTween_PresetsCentral : EditorWindow
    {
        private static Editor_XTween_PresetsCentral window;
        public static XTween_Config XTweenConfig;

        /// <summary>
        /// 图标
        /// </summary>
        private Texture2D
            logo,
            referbg,
            sep,
            selectionMark,
            btn_edit,
            btn_edit_press,
            btn_edit_ok,
            btn_edit_ok_press,
            btn_edit_cancel,
            btn_edit_cancel_press,
            btn_favourite_r,
            btn_favourite_p,
            btn_apply,
            btn_apply_press,
            icon_search,
            liquid;
        /// <summary>
        /// 分类图标
        /// </summary>
        private Texture2D[] icons;
        /// <summary>
        /// 参数编辑模式
        /// </summary>
        private bool IsEditorMode;

        string[] ease_names;
        string[] loop_types_name;
        string[] rottype_names;
        string[] rotmode_names;
        string[] rotslerp_names;
        string[] rotspace_names;
        string[] shaketype_names;

        #region left
        float left_width = 60;
        float left_height_margin_bottom = 20;
        Rect left_rect;
        #endregion

        #region typebtns
        string[] tweentypes_names = new string[13]
        {
            "To",
            "Path",
            "Position",
            "Rotation",
            "Scale",
            "Fill",
            "Size",
            "Tiled",
            "Text",
            "TmpText",
            "Alpha",
            "Shake",
            "Color"
        };
        string[] tweentypes_names_cn = new string[13]
{
            "原始",
            "路径",
            "位置",
            "旋转",
            "缩放",
            "填充",
            "尺寸",
            "平铺",
            "文字",
            "Tmp文字",
            "透明度",
            "抖动",
            "颜色"
};
        float tweentypes_size = 48;
        float tweentypes_dis = 30;
        string tweentypes_lastSelectionName = "";
        #endregion

        #region selectionmark
        Rect selectionmark_lastSelectionRect;
        float selectionmark_offset = 2;
        #endregion

        #region middle
        float middle_strartpos = 70;
        float middle_width_margin_right = 290;
        float middle_height_margin_bottom = 20;
        Rect middle_rect;
        float scroll_item_height = 60;
        float scroll_item_distance = 3;
        private Vector2 scrollPosition;
        private Rect scrollViewRect;
        private Rect viewRect;
        private List<PresetItemGUIStruct> loadedpresets = new List<PresetItemGUIStruct>();
        private XTweenTypes lastXtweenType;
        private PresetItemGUIStruct SelectedPresetItem;
        private PresetItemGUIStruct SelectedPresetItemForEditor;
        private Color ItemColor;
        private Color ItemPressColor;
        bool isFavouriteMode;
        public string PresetSearchString
        {
            get
            {
                return m_PresetSearchString;
            }

            set
            {
                if (value != m_PresetSearchString)
                {
                    InSearchmode = true;
                    m_PresetSearchString = value;
                    SearchPreset(m_PresetSearchString);

                    //ClearFocus();
                    SetEditorMode(false);
                }
            }
        }
        private string m_PresetSearchString;
        public bool InSearchmode;
        private bool SearchFieldFocused = false;
        #endregion

        #region right
        float right_width = 272;
        float right_height_margin_bottom = 20;
        Rect right_rect;
        #endregion

        [MenuItem("Assets/XTween/X 预设中心（Presets Central)")]
        public static void OpenXTweenPresetsCentral()
        {
            #region 获取配置文件            
            XTweenConfig = XTween_Dashboard.GetXTweenConfig();
            #endregion

            window = (Editor_XTween_PresetsCentral)EditorWindow.GetWindow(typeof(Editor_XTween_PresetsCentral), false, "XTween 预设中心", true);

            if (XTweenConfig.Datas.PresetCentralWindowSize == Vector2.zero)
                XTweenConfig.Datas.PresetCentralWindowSize = new Vector2(950, 800);

            // 获取当前屏幕的分辨率
            int screenWidth = Screen.currentResolution.width;
            int screenHeight = Screen.currentResolution.height;
            // 获取记忆窗口尺寸
            Vector2 size = XTweenConfig.Datas.PresetCentralWindowSize;

            // 计算窗口位置（屏幕中心）
            Rect windowRect = new Rect((screenWidth - size.x) / 2.0f, (screenHeight - size.y) / 2.0f, size.x, size.y);

            // 更新窗口位置和大小
            window.position = windowRect;
            window.minSize = new Vector2(356, 800);
            window.Show();
        }

        private void OnEnable()
        {
            #region 检查预设文件
            XTween_PresetManager.preset_JsonFile_Checker();
            #endregion

            #region 获取配置文件
            XTweenConfig = XTween_Dashboard.GetXTweenConfig();
            #endregion

            #region 图标获取
            referbg = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/referbg");
            sep = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/sep_sg");
            logo = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/logo");
            selectionMark = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/selection");

            btn_edit = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/btn_edit");
            btn_edit_press = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/btn_edit_press");
            btn_favourite_r = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/btn_favo_r");
            btn_favourite_p = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/btn_favo_p");
            btn_apply = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/btn_apply");
            btn_apply_press = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/btn_apply_press");
            icon_search = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/icon_search");
            liquid = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/liquid");

            btn_edit_ok = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/btn_edit_ok");
            btn_edit_ok_press = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/btn_edit_ok_press");
            btn_edit_cancel = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/btn_edit_cancel");
            btn_edit_cancel_press = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/btn_edit_cancel_press");

            #region 分类图标获取
            string[] icon_paths = AssetDatabase.FindAssets("t:Texture2D", new string[1] { $"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_presets/icons" });

            icons = new Texture2D[icon_paths.Length];
            for (int i = 0; i < icons.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(icon_paths[i]);
                icons[i] = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
            }
            #endregion
            #endregion

            #region 预设分类指示器初始化
            tweentypes_lastSelectionName = XTweenConfig.Datas.PresetSelectionMark_LastTypeName;
            selectionmark_lastSelectionRect = XTweenConfig.Datas.PresetSelectionMark_LastRect;

            // 分类指示器光标坐标设置
            SelectionTypeMark_OriginalRectSet(selectionmark_lastSelectionRect);
            #endregion

            #region item 颜色指定
            ItemColor = XGUI_Utilitys.HexString_To_Color("1a1a1a");
            ItemPressColor = XGUI_Utilitys.HexString_To_Color("3a3a3a");
            #endregion

            ease_names = Enum.GetNames(typeof(EaseMode));
            rottype_names = Enum.GetNames(typeof(XTweenTypes_Rotations));
            loop_types_name = Enum.GetNames(typeof(XTween_LoopType));
            rotmode_names = Enum.GetNames(typeof(XTweenRotationMode));
            rotslerp_names = Enum.GetNames(typeof(XTweenRotateLerpType));
            rotspace_names = Enum.GetNames(typeof(XTweenRotationSpace));
            shaketype_names = Enum.GetNames(typeof(XTweenTypes_Shakes));

            EditorApplication.delayCall += () =>
            {
                if (XTweenConfig.Datas.PresetInFavouriteMode)
                {
                    // 再次重新打开预设星标列表
                    OpenFavouritePresets();
                }
                else
                {
                    if (!string.IsNullOrEmpty(XTweenConfig.Datas.PresetSelectionMark_LastTypeName))
                    {
                        XTweenTypes xt = (XTweenTypes)XTweenTypeFromString(XTweenConfig.Datas.PresetSelectionMark_LastTypeName);
                        //Debug.Log(xt);
                        XTweenPresetContainer container = LoadPresetsContainer(xt);
                        PresetsAppendToList(container);
                    }
                }
            };
        }

        private void Update()
        {
            //Repaint();
        }

        private void OnGUI()
        {
            // 深色背景
            XGUI.gui_box(
                rect: new Rect(0, 0, position.width, position.height),
                bg: referbg,
                bg_color_gui: Color.white * 0.4f,
                offset: new Vector2(0, 0));

            Rect rect = new Rect(0, 0, position.width, position.height);

            #region 标头
            #region 图标
            Rect rect_icon = new Rect(23, 15, logo.width, logo.height);
            XGUI.gui_icon(
                rect: rect_icon,
                icon: logo,
                padding: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                color: Color.white);
            #endregion

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            #region 大标题
            Rect rect_title = new Rect(rect.x + 78, rect.y + 10, rect.width - 80, 30);
            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent("XTween 预设中心"),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping,
                font: XGUI.GetFont("xg-heavy"));
            #endregion

            #region 菜单
            Event event_menu = Event.current;
            if (rect_icon.Contains(event_menu.mousePosition))
            {
                if (event_menu.type == EventType.MouseDown && event_menu.button == (int)MouseButton.RightMouse)
                {

                    // 创建右键菜单
                    GenericMenu menu = new GenericMenu();

                    // 清空所有预设
                    menu.AddItem(new GUIContent("C 清空预设"), false, () =>
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XTween预设管理器消息",
                            title: "清空预设",
                            msg: $"确定要清空所有预设文件吗？此操作会删除所有预设数据，将每个类型的预设文件重置为空列表！此操作不可逆！请谨慎操作！",
                            ok: "清空",
                            cancel: "暂不",
                            themecolor: XTween_Dashboard.Theme_Primary,
                            PrimaryIndex: 0);

                        if (res == "清空")
                        {
                            bool success = ClearAllPresetFiles();

                            if (success)
                            {
                                // 清空当前显示的列表
                                loadedpresets.Clear();

                                // 隐藏分类指示器
                                SelectionTypeMark_OriginalRectSet(new Rect(-100, -100, 0, 0));

                                // 清空选中的预设
                                SelectedPresetItem = null;
                                SelectedPresetItemForEditor = null;

                                // 退出编辑模式
                                SetEditorMode(false);

                                // 刷新界面
                                Repaint();

                                // 提示用户操作成功
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XTween预设管理器消息",
                                    title: "清空完成",
                                    msg: $"所有预设文件已清空！",
                                    ok: "明白",
                                    themecolor: XTween_Dashboard.Theme_Primary,
                                    PrimaryIndex: 0);
                            }
                        }
                    });

                    // 导出所有预设
                    menu.AddItem(new GUIContent("E 导出预设"), false, () =>
                    {
                        string path = EditorUtility.SaveFolderPanel("XTween预设管理器预设导出", EditorApplication.applicationPath, "");
                        //Debug.Log(path);
                        bool x = XTween_PresetManager.preset_ExportAllPresets(path);

                        if (x)
                        {
                            EditorApplication.delayCall += () =>
                            {
                                string res = XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XTween预设管理器消息",
                                    title: "导出预设",
                                    msg: $"成功导出所有预设文件！",
                                    ok: "明白",
                                    cancel: "查看",
                                    themecolor: XTween_Dashboard.Theme_Primary,
                                    PrimaryIndex: 0);

                                if (res == "查看")
                                {
                                    // 检查路径是否有效
                                    if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                                    {
                                        try
                                        {
                                            // 使用 System.Diagnostics.Process 直接打开文件夹（不选中任何文件）
                                            System.Diagnostics.Process.Start(path);

                                            XGUI_Utilitys.Console("XTween预设管理器消息", $"已打开导出文件夹: {path}", XGUIMsgState.确认);
                                        }
                                        catch (Exception e)
                                        {
                                            Debug.LogError($"打开文件夹失败: {e.Message}");

                                            // 备用方案：使用 Application.OpenURL
                                            try
                                            {
                                                Application.OpenURL("file://" + path);
                                            }
                                            catch
                                            {
                                                // 如果都失败，至少告诉用户路径
                                                XGUI.dialog(
                                                    type: XGUIDialogType.错误,
                                                    windowtitle: "XTween预设管理器消息",
                                                    title: "打开文件夹失败",
                                                    msg: $"无法自动打开文件夹，请手动访问：\n{path}",
                                                    ok: "明白",
                                                    cancel: "",
                                                    themecolor: XTween_Dashboard.Theme_Primary,
                                                    PrimaryIndex: 0);
                                            }
                                        }
                                    }
                                }
                            };
                        }
                    });

                    // 导入所有预设
                    menu.AddItem(new GUIContent("I 导入预设"), false, () =>
                    {
                        loadedpresets.Clear();
                        SelectionTypeMark_OriginalRectSet(new Rect(-100, -100, 0, 0));

                        // 弹出文件夹选择对话框
                        string importPath = EditorUtility.OpenFolderPanel("选择要导入的预设文件夹", EditorApplication.applicationPath, "");

                        if (string.IsNullOrEmpty(importPath))
                        {
                            // 用户取消了选择
                            return;
                        }

                        EditorApplication.delayCall += () =>
                        {
                            // 确认导入操作
                            string confirmRes = XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XTween预设管理器消息",
                                title: "导入预设",
                                msg: $"确定要从以下文件夹导入预设吗？{importPath} 。注意：导入操作会覆盖同名的预设文件！",
                                ok: "取消",
                                cancel: "导入",
                                themecolor: XTween_Dashboard.Theme_Primary,
                                PrimaryIndex: 0);

                            if (confirmRes != "导入")
                                return;

                            // 执行导入
                            bool importSuccess = ImportPresetsFromFolder(importPath);

                            if (importSuccess)
                            {
                                // 刷新当前显示的预设列表
                                if (!isFavouriteMode && !string.IsNullOrEmpty(tweentypes_lastSelectionName))
                                {
                                    XTweenTypes xt = (XTweenTypes)XTweenTypeFromString(tweentypes_lastSelectionName);
                                    XTweenPresetContainer container = LoadPresetsContainer(xt);
                                    PresetsAppendToList(container);
                                }
                                else if (isFavouriteMode)
                                {
                                    OpenFavouritePresets();
                                }
                                Repaint();
                            }
                        };
                    });

                    // 显示菜单
                    menu.ShowAsContext();

                    event_menu.Use();
                }
            }
            #endregion
            #endregion

            #region Favourite
            Rect rect_fav;
            if (!isSize_HideInfo())
                rect_fav = new Rect(rect.x + 500, rect.y + 10, btn_favourite_r.width, btn_favourite_r.height);
            else
                rect_fav = new Rect(rect.x + rect.width - 65, rect.y + 10, btn_favourite_r.width, btn_favourite_r.height);

            if (XGUI.gui_button(
                rect: rect_fav,
                tooltip: "收藏的预设",
                tex_release: btn_favourite_r,
                tex_press: btn_favourite_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                focus_name: "fav_btn"))
            {
                ClearFocus();
                SetEditorMode(false);

                PresetSearchString = null;
                InSearchmode = false;
                OpenFavouritePresets();
            }
            #endregion

            #region Search
            if (isSize_HideSearchWidth())
            {
                // 输入框 - 名称
                GUI.SetNextControlName("SearchTextField");

                Rect preset_name_rect = new Rect(rect.x + 250, rect.y + 13, 230, XGUI.GetSingleLineHeight() + 8);
                PresetSearchString = XGUI.gui_inputfield(
                    rect: preset_name_rect,
                    prop: PresetSearchString,
                    text_wrap: false,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 28,
                    field_text_color: Color.white,
                    title_width: 40,
                    //status_icon: "icon_field_status",
                    //status_icon_color: Color.green,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    field_padding: new RectOffset(30, 5, 0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0));

                // 检测当前哪个控件获得焦点
                string focusedControl = GUI.GetNameOfFocusedControl();

                // 根据焦点状态执行相应逻辑
                if (focusedControl == "SearchTextField")
                {
                    if (!SearchFieldFocused)
                    {
                        SearchFieldFocused = true;

                        // 在这里执行获得焦点时的逻辑
                        InSearchmode = true;
                        loadedpresets.Clear();
                        SearchPreset(m_PresetSearchString);
                    }
                }
                else
                {
                    SearchFieldFocused = false;
                }

                Rect preset_name_icon_rect = new Rect(preset_name_rect.x, preset_name_rect.y - 2, icon_search.width, icon_search.height);
                XGUI.gui_icon(
                    rect: preset_name_icon_rect,
                    icon: icon_search,
                    padding: new RectOffset(0, 0, 0, 0),
                    border: new RectOffset(0, 0, 0, 0),
                    color: Color.white);
            }
            #endregion

            #region  left
            // left_rect 基础坐标
            left_rect = new Rect(rect.x + 5, rect.y + 50, left_width, rect.height - 80 - left_height_margin_bottom);
            // 预设分类按钮列表
            Draw_TweenTypeButtons(left_rect);

            #region 分割线
            Rect leftpanel_rect_sep = new Rect(left_rect.x + left_rect.width + 1, left_rect.y + 18, sep.width, rect.height - right_height_margin_bottom - 70);
            XGUI.gui_icon(
               rect: leftpanel_rect_sep,
               icon: sep,
               padding: new RectOffset(0, 0, 0, 0),
               border: new RectOffset(0, 0, 90, 90),
               color: Color.white * 0.6f);
            #endregion

            #region 分类选择光标
            XGUI.gui_icon(
                rect: selectionmark_lastSelectionRect,
                icon: selectionMark,
                padding: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                color: XTween_Dashboard.Theme_Primary);
            #endregion

            #endregion

            #region middle
            // middle_rect 基础坐标
            if (!isSize_HideInfo())
            {
                middle_strartpos = 70;
                middle_width_margin_right = 290;
                middle_height_margin_bottom = 20;
            }
            else
            {
                middle_strartpos = 70;
                middle_width_margin_right = 0;
                middle_height_margin_bottom = 20;
            }

            middle_rect = new Rect(left_rect.x + middle_strartpos, left_rect.y, rect.width - middle_width_margin_right - middle_strartpos, rect.height - 80 - middle_height_margin_bottom);

            // 绘制列表
            Draw_PresetsScrollView_VirtualScrollOptmize(middle_rect);
            // 绘制当前分类的预设统计数量
            Draw_PresetsCountStatistic(middle_rect);
            #endregion

            #region right
            // right_rect 基础坐标
            right_rect = new Rect(rect.x + (rect.width - right_width), rect.y + 15, right_width, rect.height - right_height_margin_bottom - 10);
            if (!isSize_HideInfo())
            {
                if (!IsEditorMode)
                    Draw_Info(right_rect);
                else
                    Draw_Editor(right_rect);
            }
            #endregion

            Repaint();

            //Debug.Log(position);
        }

        #region Draw      
        /// <summary>
        /// 动画分类按钮
        /// </summary>
        /// <param name="rect"></param>
        private void Draw_TweenTypeButtons(Rect rect)
        {
            Rect o = rect;
            float e = 6;
            for (int i = 0; i < tweentypes_names.Length; i++)
            {
                if (i == 0)
                    o.Set(o.x + e, o.y + tweentypes_dis, tweentypes_size, tweentypes_size);
                else
                    o.Set(o.x, o.y + tweentypes_size, tweentypes_size, tweentypes_size);

                #region 点击了类型按钮后逻辑
                if (XGUI.gui_button(
                    rect: o,
                    tooltip: tweentypes_names_cn[i],
                    tex_release: GetTweenTypeBtnIcon(tweentypes_names[i]),
                    tex_press: GetTweenTypeBtnIcon_Pressed(tweentypes_names[i]),
                    tex_gui_color: Color.white,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    focus_name: "fav_btn"))
                {
                    ClearFocus();
                    SetEditorMode(false);

                    TweenTypeBtnClickedEvent(o, tweentypes_names[i]);
                }
                #endregion
            }
        }
        /// <summary>
        /// 绘制预设列表（优化版：虚拟滚动）
        /// </summary>
        /// <param name="rect"></param>
        private void Draw_PresetsScrollView_VirtualScrollOptmize(Rect rect)
        {
            // 定义滚动视图的区域
            scrollViewRect = new Rect(rect.x, rect.y + 25, rect.width, rect.height - 35);

            // 计算总内容高度
            float itemTotalHeight = scroll_item_height + scroll_item_distance;
            if (isSize_HideInfo())
                itemTotalHeight = scroll_item_height + scroll_item_distance;
            float totalHeight = loadedpresets.Count * itemTotalHeight;

            // 可见区域的高度
            float viewportHeight = scrollViewRect.height;

            // 开始滚动视图 - 注意这里viewRect的高度要设置为totalHeight
            viewRect = new Rect(0, 0, scrollViewRect.width - 20, totalHeight);
            scrollPosition = GUI.BeginScrollView(scrollViewRect, scrollPosition, viewRect);

            // 获取当前事件
            Event e = Event.current;

            #region 虚拟滚动：计算可见范围
            // 计算当前滚动位置对应的第一个项目的索引
            int startIndex = Mathf.FloorToInt(scrollPosition.y / itemTotalHeight);
            // 计算可见区域内最多能显示多少个项目
            int visibleCount = Mathf.CeilToInt(viewportHeight / itemTotalHeight) + 2; // +2 作为缓冲区，避免滚动时出现空白
                                                                                      // 确保索引不越界
            startIndex = Mathf.Max(0, startIndex);
            int endIndex = Mathf.Min(loadedpresets.Count, startIndex + visibleCount);
            #endregion

            // 只绘制可见范围内的项目
            for (int i = startIndex; i < endIndex; i++)
            {
                PresetItemGUIStruct pret = loadedpresets[i];

                #region 计算项目的实际Y位置（基于真实索引）
                float itemY = i * itemTotalHeight;
                Rect itemRect = new Rect(5, itemY, viewRect.width - 10, scroll_item_height);
                Rect itemRect_clicked = new Rect(5, itemY, viewRect.width - 210, scroll_item_height);

                if (isSize_HideInfo())
                    itemRect_clicked = new Rect(5, itemY, viewRect.width, scroll_item_height / 2 + 15);
                #endregion

                #region 检测鼠标点击
                if (itemRect_clicked.Contains(e.mousePosition))
                {
                    if (!isSize_HideInfo())
                    {
                        if (e.type == EventType.MouseDown && e.button == (int)MouseButton.LeftMouse)
                        {
                            ClearFocus();
                            SetEditorMode(false);

                            SelectedPresetItem = pret;
                            pret.isPressing = true;
                            e.Use();
                            Repaint();
                        }
                        if (e.type == EventType.MouseUp && e.button == (int)MouseButton.LeftMouse)
                        {
                            pret.isPressing = false;
                            e.Use();
                            Repaint();
                        }
                        if (e.type == EventType.MouseDrag && e.button == (int)MouseButton.LeftMouse)
                        {
                            pret.isPressing = false;
                            e.Use();
                            Repaint();
                        }
                    }
                    if (e.type == EventType.MouseDown && e.button == (int)MouseButton.RightMouse)
                    {
                        // 保存当前选中的预设
                        SelectedPresetItem = pret;

                        // 创建右键菜单
                        GenericMenu menu = new GenericMenu();

                        // 添加菜单项
                        if (!isSize_HideInfo())
                            menu.AddItem(new GUIContent("E 编辑预设"), false, OnEditPreset, pret);
                        menu.AddItem(new GUIContent("A 应用预设"), false, OnApplyPreset, pret);
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("D 删除预设"), false, OnDeletePreset, pret);
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent($"{(pret.preset.IsFavourite ? "S 取消星标" : "S 设为星标")}"), pret.preset.IsFavourite, OnToggleFavourite, pret);

                        // 显示菜单
                        menu.ShowAsContext();

                        e.Use();
                    }
                }
                #endregion

                #region 绘制背景
                if (pret.isPressing)
                    XGUI.gui_box(
                        rect: itemRect,
                        bg: XGUI.GetBtnFillTexture(XGUIFilled.实体, XGUIColor.亮白),
                        bg_color_gui: ItemPressColor,
                        border: new RectOffset(15, 15, 15, 15),
                        offset: new Vector2(0, 0));
                else
                    XGUI.gui_box(
                        rect: itemRect,
                        bg: XGUI.GetBtnFillTexture(XGUIFilled.实体, XGUIColor.亮白),
                        bg_color_gui: ItemColor,
                        border: new RectOffset(15, 15, 15, 15),
                        offset: new Vector2(0, 0));
                #endregion

                #region 绘制图标（根据模式决定是否显示）
                if (isFavouriteMode || InSearchmode)
                {
                    Rect rect_icon = new Rect(itemRect.x + 8, itemRect.y + 12, tweentypes_size * 0.7f, tweentypes_size * 0.7f);
                    string nm = pret.type.ToString().Split(new char[1] { '_' })[1];

                    XGUI.gui_icon(
                        rect: rect_icon,
                        icon: GetTweenTypeBtnIcon(nm),
                        padding: new RectOffset(0, 0, 0, 0),
                        border: new RectOffset(0, 0, 0, 0),
                        color: Color.white * 0.6f);
                }
                #endregion

                #region 间距判定
                float dis = 26;
                if (InSearchmode || isFavouriteMode)
                {
                    dis = 50;
                }
                #endregion

                #region 绘制标题 & 解释
                Rect rect_title = new Rect(itemRect.x + dis, itemRect.y + 5, 280, 25);
                if (isSize_HideInfo())
                    if (InSearchmode || isFavouriteMode)
                    {
                        rect_title = new Rect(itemRect.x + dis, itemRect.y + 5, itemRect.width / 2.7f, 25);
                    }
                    else
                        rect_title = new Rect(itemRect.x + dis, itemRect.y + 5, itemRect.width / 2, 25);

#if UNITY_6000_0_OR_NEWER
                // Unity 6+ 使用 Ellipsis
                TextClipping clipping = TextClipping.Ellipsis;
#else
            // Unity 2021.1 之前使用 Clip
            TextClipping clipping = TextClipping.Clip;
#endif
                XGUI.gui_label(
                    rect: rect_title,
                    text: new GUIContent(pret.preset.Name),
                    text_color: Color.white,
                    size: XGUIFontSize.BX,
                    anchor: TextAnchor.MiddleLeft,
                    clipping: clipping,
                    font: XGUI.GetFont("xg-medium"));

                // 绘制解释
                Rect rect_des = new Rect(itemRect.x + dis, itemRect.y + itemRect.height - 25 - 5, 300, 25);
                if (isSize_HideInfo())
                    if (InSearchmode || isFavouriteMode)
                    {
                        rect_des = new Rect(itemRect.x + dis, itemRect.y + itemRect.height - 25 - 5, itemRect.width / 2.5f, 25);
                    }
                    else
                        rect_des = new Rect(itemRect.x + dis, itemRect.y + itemRect.height - 25 - 5, itemRect.width / 1.8f, 25);

                XGUI.gui_label(
                    rect: rect_des,
                    text: new GUIContent(pret.preset.Description),
                    text_color: Color.white * 0.75f,
                    size: XGUIFontSize.B,
                    anchor: TextAnchor.MiddleLeft,
                    clipping: clipping,
                    font: XGUI.GetFont("xg-regular"));
                #endregion

                float offset = 20;

                // 修改参数按钮
                if (!isSize_HideInfo())
                {
                    Rect rect_btn_edit = new Rect(itemRect.width - 160 - offset, itemRect.y + 12, btn_edit.width, btn_edit.height);
                    if (XGUI.gui_button(
                        rect: rect_btn_edit,
                        tooltip: "",
                        tex_release: btn_edit,
                        tex_press: btn_edit_press,
                        tex_gui_color: Color.white,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        focus_name: "edit_btn"))
                    {
                        ClearFocus();

                        SelectedPresetItem = pret;
                        SetEditorMode(true);
                    }
                }

                // 应用参数按钮
                Rect rect_btn_apply = new Rect(itemRect.width - 100 - offset, itemRect.y + 12, btn_apply.width, btn_apply.height);
                if (isSize_HideInfo())
                    rect_btn_apply = new Rect(itemRect.width - 60 - offset, itemRect.y + 12, btn_apply.width, btn_apply.height);
                if (XGUI.gui_button(
                    rect: rect_btn_apply,
                    tooltip: "",
                    tex_release: btn_apply,
                    tex_press: btn_apply_press,
                    tex_gui_color: Color.white,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    focus_name: "apply_btn"))
                {
                    ClearFocus();

                    ApplyToController(pret.preset);
                }

                // 星标按钮
                Rect rect_btn_favo = new Rect(itemRect.width - 40 - offset, itemRect.y + 12, btn_favourite_r.width, btn_favourite_r.height);
                if (isSize_HideInfo())
                    rect_btn_favo = new Rect(itemRect.width - 20 - offset, itemRect.y + 12, btn_favourite_r.width, btn_favourite_r.height);
                if (XGUI.gui_button(
                    rect: rect_btn_favo,
                    tooltip: "",
                    tex_release: pret.preset.IsFavourite ? btn_favourite_r : btn_favourite_p,
                    tex_press: pret.preset.IsFavourite ? btn_favourite_r : btn_favourite_p,
                    tex_gui_color: Color.white,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    focus_name: "fav_btn"))
                {
                    ClearFocus();

                    pret.preset.IsFavourite = !pret.preset.IsFavourite;
                    LastTweenPresetsSaved();

                    // 如果是星标模式下，点击星标按钮则会去除对应类中的预设星标效果，并重新刷新Favourite列表
                    if (isFavouriteMode)
                    {
                        XTweenPresetContainer pre_con = XTween_PresetManager.preset_Container_Load(pret.type);
                        for (int c = 0; c < pre_con.Presets.Count; c++)
                        {
                            XTweenPresetBase pre = pre_con.Presets[c];
                            if (pre.Name == pret.preset.Name)
                            {
                                pre.IsFavourite = pret.preset.IsFavourite;
                            }
                        }

                        // 将当前预设参数列表替换到目标Json文件中
                        XTween_PresetManager.preset_Container_Save_Replace(pret.type, pre_con.Presets);

                        //// 再次重新打开预设星标列表
                        //OpenFavouritePresets();
                    }
                }
            }

            GUI.EndScrollView();
        }
        /// <summary>
        /// 显示当前分类的预设总数
        /// </summary>
        private void Draw_PresetsCountStatistic(Rect rect)
        {
            Rect rect_sta = new Rect(rect.x + rect.width - 230, rect.y - 3, 200, 25);

            XGUI.gui_label(
                rect: rect_sta,
                text: new GUIContent($"当前预设数量： {loadedpresets.Count}"),
                text_color: Color.white * 0.75f,
                size: XGUIFontSize.B,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleRight,
                font: XGUI.GetFont("xg-medium"));
        }
        /// <summary>
        /// 刷新绘制参数
        /// </summary>
        /// <param name="pret"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Draw_Info(Rect rect)
        {
            Rect rect_info = new Rect(rect.x, rect.y, rect.width - 15, rect.height);
            float left_margin = 20;
            XGUI.gui_box(
                rect: rect_info,
                bg: XGUI.GetFillTexture(XGUIFilled.实体, XGUIColor.亮白),
                bg_color_gui: Color.white * 0.35f,
                border: new RectOffset(10, 10, 10, 10));

            if (SelectedPresetItem == null)
            {
                Rect rect_tip_title = new Rect(rect_info.x + ((rect_info.width / 2) - 90), rect_info.y + ((rect_info.height / 2) - 15), 180, 30);
                XGUI.gui_label(
                    rect: rect_tip_title,
                    text: new GUIContent("暂无预览信息"),
                    text_color: Color.white * 0.7f,
                    size: XGUIFontSize.B,
                    clipping: TextClipping.Clip,
                    anchor: TextAnchor.MiddleCenter,
                    font_style: FontStyle.Bold,
                    font: XGUI.GetFont("xg-medium"));

                Rect rect_tip_subtitle = new Rect(rect_info.x + ((rect_info.width / 2) - 90), rect_info.y + ((rect_info.height / 2) - 15) + 28, 180, 30);
                XGUI.gui_label(
                   rect: rect_tip_subtitle,
                   text: new GUIContent("请先选中一个预设"),
                   text_color: Color.white * 0.5f,
                   size: XGUIFontSize.M,
                   clipping: TextClipping.Clip,
                   anchor: TextAnchor.MiddleCenter,
                   font_style: FontStyle.Bold,
                   font: XGUI.GetFont("xg-medium"));

                return;
            }

#if UNITY_6000_0_OR_NEWER
            // Unity 6+ 使用 Ellipsis
            TextClipping clipping = TextClipping.Ellipsis;
#else
            // Unity 2021.1 之前使用 Clip
            TextClipping clipping = TextClipping.Clip;
#endif

            #region 名称
            Rect rect_pre_name = new Rect(rect_info.x + left_margin, rect_info.y + 12, rect_info.width - 40, 30);
            XGUI.gui_label(
                rect: rect_pre_name,
                text: new GUIContent(SelectedPresetItem.preset.Name),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleLeft,
                font_style: FontStyle.Bold,
                font: XGUI.GetFont("xg-medium"));
            #endregion

            #region 解释
            Rect rect_pre_des = new Rect(rect_info.x + left_margin, rect_info.y + 45, rect_info.width - 40, 80);
            XGUI.gui_label(
               rect: rect_pre_des,
               text: new GUIContent(SelectedPresetItem.preset.Description),
               text_color: Color.white,
               wrap: true,
               size: XGUIFontSize.M,
               clipping: clipping,
               anchor: TextAnchor.UpperLeft,
               font_style: FontStyle.Normal,
               font: XGUI.GetFont("xg-regular"));
            #endregion

            #region 分割线
            Rect rect_sep = new Rect(rect_info.x + left_margin, rect_info.y + 140, rect_info.width - 40, 80);
            XGUI.gui_seperator(
                rect: rect_sep,
                thickness: 1,
                color: Color.white * 0.5f);
            #endregion

            #region 参数标题
            Rect rect_pam_title = new Rect(rect_info.x + left_margin, rect_info.y + 150, 185, 30);
            XGUI.gui_label(
              rect: rect_pam_title,
              text: new GUIContent("参数概览"),
              text_color: Color.white,
              size: XGUIFontSize.L,
              clipping: TextClipping.Clip,
              anchor: TextAnchor.MiddleLeft,
              font_style: FontStyle.Bold,
              font: XGUI.GetFont("xg-medium"));
            #endregion

            #region  预设分类图标
            Rect rect_pam_icon = new Rect(rect_info.x + ((rect_info.width - tweentypes_size) + 10), rect_info.y + 18, tweentypes_size * 0.4f, tweentypes_size * 0.4f);
            string nm = SelectedPresetItem.type.ToString().Split(new char[1] { '_' })[1];
            XGUI.gui_icon(
              rect: rect_pam_icon,
              icon: GetTweenTypeBtnIcon($"{nm}_big"),
              color: Color.white * 0.5f);
            #endregion

            #region 参数集
            float start = 160;
            float height_offset = 20;
            Color color = Color.white * 0.8f;

            Rect rect_prop = new Rect(rect_info.x + left_margin, rect_info.y + start + height_offset, rect_info.width - (left_margin * 2) + 5, 25);

            DrawInfo(rect_prop, 0, "耗时", $"{SelectedPresetItem.preset.Duration} s");
            DrawInfo(rect_prop, height_offset * 1, "延迟", $"{SelectedPresetItem.preset.Delay} s");
            DrawInfo(rect_prop, height_offset * 2, "随机延迟", $"{(SelectedPresetItem.preset.UseRandomDelay ? "是" : "否")}");
            DrawInfo(rect_prop, height_offset * 3, "循环数", $"{(SelectedPresetItem.preset.LoopCount)} 次");
            DrawInfo(rect_prop, height_offset * 4, "循环方式", $"{SelectedPresetItem.preset.LoopType}");
            DrawInfo(rect_prop, height_offset * 5, "循环延迟", $"{SelectedPresetItem.preset.LoopDelay} s");
            DrawInfo(rect_prop, height_offset * 6, "相对模式", $"{(SelectedPresetItem.preset.IsRelative ? "是" : "否")}");
            DrawInfo(rect_prop, height_offset * 7, "自动杀死", $"{(SelectedPresetItem.preset.IsAutoKill ? "是" : "否")}");
            DrawInfo(rect_prop, height_offset * 8, "缓动方式", $"{SelectedPresetItem.preset.EaseMode}");
            DrawInfo(rect_prop, height_offset * 9, "使用曲线", $"{(SelectedPresetItem.preset.UseCurve ? "是" : "否")}");
            DrawInfo(rect_prop, height_offset * 10, "使用起始", $"{(SelectedPresetItem.preset.UseFromMode ? "是" : "否")}");

            Rect rect_pam_sep = new Rect(rect_prop.x, rect_prop.y + height_offset * 11 + 13, rect_prop.width, 10);

            if (SelectedPresetItem.preset is XTweenPreset_To to)
            {
                Draw_Seperate(rect_pam_sep);

                switch (to.ToType)
                {
                    case XTweenTypes_To.整数_Int:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{to.EndValue_Int}");
                        if (to.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{to.FromValue_Int}");
                        break;
                    case XTweenTypes_To.浮点数_Float:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{to.EndValue_Float}");
                        if (to.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{to.FromValue_Float}");
                        break;
                    case XTweenTypes_To.字符串_String:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{to.EndValue_String}");
                        if (to.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{to.FromValue_String}");
                        break;
                    case XTweenTypes_To.二维向量_Vector2:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{to.EndValue_Vector2}");
                        if (to.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{to.FromValue_Vector2}");
                        break;
                    case XTweenTypes_To.三维向量_Vector3:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{to.EndValue_Vector3}");
                        if (to.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{to.FromValue_Vector3}");
                        break;
                    case XTweenTypes_To.四维向量_Vector4:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{to.EndValue_Vector4}");
                        if (to.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{to.FromValue_Vector4}");
                        break;
                    case XTweenTypes_To.颜色_Color:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{to.EndValue_Color}");
                        if (to.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{to.FromValue_Color}");
                        break;
                }
            }
            if (SelectedPresetItem.preset is XTweenPreset_Path path)
            {
                Draw_Seperate(rect_pam_sep);

                DrawInfo(rect_prop, height_offset * 12, "路径名称", path.PathName);
            }
            if (SelectedPresetItem.preset is XTweenPreset_Position pos)
            {
                switch (pos.PositionType)
                {
                    case XTweenTypes_Positions.锚点位置_AnchoredPosition:
                        Draw_Seperate(rect_pam_sep);

                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{pos.EndValue_Vector2}");
                        if (pos.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{pos.FromValue_Vector2}");
                        break;
                    case XTweenTypes_Positions.锚点位置3D_AnchoredPosition3D:
                        Draw_Seperate(rect_pam_sep);

                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{pos.EndValue_Vector3}");
                        if (pos.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{pos.FromValue_Vector3}");
                        break;
                }
            }
            if (SelectedPresetItem.preset is XTweenPreset_Rotation rot)
            {
                Draw_Seperate(rect_pam_sep);

                DrawInfo(rect_prop, height_offset * 12, "旋转类型", $"{rot.RotationType}");
                DrawInfo(rect_prop, height_offset * 13, "旋转方式", $"{rot.RotationMode}");
                DrawInfo(rect_prop, height_offset * 14, "坐标空间", $"{rot.RotationSpace}");
                DrawInfo(rect_prop, height_offset * 15, "平滑模式", $"{rot.RotateLerpMode}");

                switch (rot.RotationType)
                {
                    case XTweenTypes_Rotations.欧拉角度_Euler:
                        DrawInfo(rect_prop, height_offset * 16, "目标值", $"{rot.EndValue_Euler}");
                        if (rot.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 17, "起始值", $"{rot.FromValue_Euler}");
                        break;
                    case XTweenTypes_Rotations.四元数_Quaternion:
                        DrawInfo(rect_prop, height_offset * 16, "目标值", $"{rot.EndValue_Quaternion}");
                        if (rot.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 17, "起始值", $"{rot.FromValue_Quaternion}");
                        break;
                }
            }
            if (SelectedPresetItem.preset is XTweenPreset_Scale scale)
            {
                Draw_Seperate(rect_pam_sep);

                DrawInfo(rect_prop, height_offset * 12, "目标值", $"{scale.EndValue}");
                if (scale.UseFromMode)
                    DrawInfo(rect_prop, height_offset * 13, "起始值", $"{scale.FromValue}");
            }
            if (SelectedPresetItem.preset is XTweenPreset_Fill fill)
            {
                Draw_Seperate(rect_pam_sep);

                DrawInfo(rect_prop, height_offset * 12, "目标值", $"{fill.EndValue}");
                if (fill.UseFromMode)
                    DrawInfo(rect_prop, height_offset * 13, "起始值", $"{fill.FromValue}");
            }
            if (SelectedPresetItem.preset is XTweenPreset_Size size)
            {
                Draw_Seperate(rect_pam_sep);

                DrawInfo(rect_prop, height_offset * 12, "目标值", $"{size.EndValue}");
                if (size.UseFromMode)
                    DrawInfo(rect_prop, height_offset * 13, "起始值", $"{size.FromValue}");
            }
            if (SelectedPresetItem.preset is XTweenPreset_Tiled tiled)
            {
                Draw_Seperate(rect_pam_sep);

                DrawInfo(rect_prop, height_offset * 12, "目标值", $"{tiled.EndValue}");
                if (tiled.UseFromMode)
                    DrawInfo(rect_prop, height_offset * 13, "起始值", $"{tiled.FromValue}");
            }
            if (SelectedPresetItem.preset is XTweenPreset_Text text)
            {
                Draw_Seperate(rect_pam_sep);

                switch (text.TextType)
                {
                    case XTweenTypes_Text.文字尺寸_FontSize:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{text.EndValue_Int}");
                        if (text.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{text.FromValue_Int}");
                        break;
                    case XTweenTypes_Text.文字行高_LineHeight:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{text.EndValue_Float}");
                        if (text.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{text.FromValue_Float}");
                        break;
                    case XTweenTypes_Text.文字颜色_Color:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{text.EndValue_Color}", text.EndValue_Color);
                        if (text.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{text.FromValue_Color}", text.FromValue_Color);
                        break;
                    case XTweenTypes_Text.文字内容_Content:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{text.EndValue_String}");
                        if (text.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{text.FromValue_String}");
                        break;
                }
            }
            if (SelectedPresetItem.preset is XTweenPreset_TmpText tmp)
            {
                Draw_Seperate(rect_pam_sep);

                switch (tmp.TmpTextType)
                {
                    case XTweenTypes_TmpText.文字尺寸_FontSize:
                    case XTweenTypes_TmpText.文字行高_LineHeight:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{tmp.EndValue_Float}");
                        if (tmp.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{tmp.FromValue_Float}");
                        break;
                    case XTweenTypes_TmpText.文字颜色_Color:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{tmp.EndValue_Color}", tmp.EndValue_Color);
                        if (tmp.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{tmp.FromValue_Color}", tmp.FromValue_Color);
                        break;
                    case XTweenTypes_TmpText.文字内容_Content:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{tmp.EndValue_String}");
                        if (tmp.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{tmp.FromValue_String}");
                        break;
                    case XTweenTypes_TmpText.文字边距_Margin:
                        DrawInfo(rect_prop, height_offset * 12, "目标值", $"{tmp.EndValue_Vector4}");
                        if (tmp.UseFromMode)
                            DrawInfo(rect_prop, height_offset * 13, "起始值", $"{tmp.FromValue_Vector4}");
                        break;
                }
            }
            if (SelectedPresetItem.preset is XTweenPreset_Alpha alp)
            {
                Draw_Seperate(rect_pam_sep);

                DrawInfo(rect_prop, height_offset * 12, "目标值", $"{alp.EndValue}");
                if (alp.UseFromMode)
                    DrawInfo(rect_prop, height_offset * 13, "起始值", $"{alp.FromValue}");
            }
            if (SelectedPresetItem.preset is XTweenPreset_Shake shake)
            {
                Draw_Seperate(rect_pam_sep);

                DrawInfo(rect_prop, height_offset * 12, "震动幅度", $"{shake.Vibrato}");
                DrawInfo(rect_prop, height_offset * 13, "随机化", $"{shake.Randomness}");
                DrawInfo(rect_prop, height_offset * 14, "过渡震动", $"{shake.FadeShake}");

                switch (shake.ShakeType)
                {
                    case XTweenTypes_Shakes.位置_Position:
                        DrawInfo(rect_prop, height_offset * 15, "目标值", $"{shake.Strength_Vector3}");
                        break;
                    case XTweenTypes_Shakes.尺寸_Size:
                        DrawInfo(rect_prop, height_offset * 15, "目标值", $"{shake.Strength_Vector2}");
                        break;
                    case XTweenTypes_Shakes.旋转_Rotation:
                        DrawInfo(rect_prop, height_offset * 15, "目标值", $"{shake.Strength_Vector3}");
                        break;
                    case XTweenTypes_Shakes.缩放_Scale:
                        DrawInfo(rect_prop, height_offset * 15, "目标值", $"{shake.Strength_Vector3}");
                        break;
                }
            }
            if (SelectedPresetItem.preset is XTweenPreset_Color col)
            {
                Draw_Seperate(rect_pam_sep);
                DrawInfo(rect_prop, height_offset * 12, "目标值", $"{col.EndValue}", col.EndValue);
                if (col.UseFromMode)
                    DrawInfo(rect_prop, height_offset * 13, "起始值", $"{col.FromValue}", col.FromValue);
            }
            #endregion

            #region 液晶显示

            #region 液晶屏
            Rect rect_liquid = new Rect(rect.x + (rect.width / 2) - (liquid.width / 2), rect.y + (rect.height - liquid.height - 50), liquid.width, liquid.height);
            // 测试显示
            //XGUI.gui_box(liquid_root, Color.red * 0.5f);
            XGUI.gui_icon(
                rect: rect_liquid,
                icon: liquid,
                color: Color.white);
            #endregion

            if (!SelectedPresetItem.preset.UseCurve)
            {
                #region 缓动曲线背景
                Texture2D tex_ease_bg = XGUI.GetBasedIcon($"EaseGraph/bg");
                Rect rect_ease_bg = new Rect(rect_liquid.x + (rect_liquid.width / 2) - (tex_ease_bg.width / 2), rect_liquid.y + (rect_liquid.height / 2) - (tex_ease_bg.height / 2), tex_ease_bg.width, tex_ease_bg.height);
                XGUI.gui_icon(
                    rect: rect_ease_bg,
                    icon: tex_ease_bg,
                    color: Color.white);
                #endregion

                #region 缓动曲线
                Texture2D tex_ease = XGUI.GetBasedIcon($"EaseGraph/{SelectedPresetItem.preset.EaseMode}");
                Rect rect_ease = new Rect(rect_ease_bg.x, rect_ease_bg.y, tex_ease.width, tex_ease.height);
                XGUI.gui_icon(
                    rect: rect_ease,
                    icon: tex_ease,
                    color: XTween_Dashboard.Theme_Primary);
                #endregion

                #region 缓动名称
                Rect rect_ease_name = new Rect(rect_liquid.x + (rect_liquid.width / 2) - 60, rect_liquid.y + 15, 120, XGUI.GetSingleLineHeight());
                XGUI.gui_label(
                    rect: rect_ease_name,
                    text: new GUIContent(SelectedPresetItem.preset.EaseMode.ToString()),
                    text_color: Color.white,
                    size: XGUIFontSize.S,
                    clipping: TextClipping.Clip,
                    anchor: TextAnchor.MiddleCenter,
                    font_style: FontStyle.Normal,
                    font: XGUI.GetFont("xg-medium"));
                #endregion
            }
            else
            {
                float w = 140;
                float h = 70;
                Rect rect_curve_bg = new Rect(rect_liquid.x + (rect_liquid.width / 2) - (w / 2), rect_liquid.y + (rect_liquid.height / 2) - (h / 2) - 10, w, h);
                //Editor_XTween_GUI.Gui_CurveField(rect_curve_bg, SelectedPresetItem.preset.Curve);

                GUI.enabled = false;
                SelectedPresetItem.preset.Curve = Draw_Editor_Field(rect_curve_bg, GUIContent.none, SelectedPresetItem.preset.Curve);
                GUI.enabled = true;
            }

            #endregion

            #region 功能按钮
            float int_dis = 10;
            float int_offset = -5;

            // 修改参数按钮
            rect_info.Set(rect.x + int_offset + ((rect.width / 2) - btn_edit.width - int_dis), rect.y + rect.height - 45, btn_edit.width, btn_edit.height);
            if (XGUI.gui_button(
                rect: rect_info,
                tooltip: "编辑预设参数",
                tex_release: btn_edit,
                tex_press: btn_edit_press,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                focus_name: "fav_btn"))
            {
                ClearFocus();
                SetEditorMode(true);
            }

            // 应用参数按钮
            rect_info.Set(rect.x + int_offset + ((rect.width / 2) + btn_apply.width - int_dis), rect.y + rect.height - 45, btn_apply.width, btn_apply.height);
            if (XGUI.gui_button(
                rect: rect_info,
                tooltip: "应用到动画控制器",
                tex_release: btn_apply,
                tex_press: btn_apply_press,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                focus_name: "fav_btn"))
            {
                ClearFocus();

                ApplyToController(SelectedPresetItem.preset);
            }
            #endregion
        }
        /// <summary>
        /// 绘制参数修改器
        /// </summary>
        private void Draw_Editor(Rect rect)
        {
            Rect rect_editor = new Rect(rect.x, rect.y, rect.width - 15, rect.height);
            float left_margin = 20;

            XGUI.gui_box(
                 rect: rect_editor,
                 bg: XGUI.GetFillTexture(XGUIFilled.实体, XGUIColor.亮白),
                 bg_color_gui: Color.white * 0.35f,
                 border: new RectOffset(10, 10, 10, 10));

            XTweenPresetBase pre = SelectedPresetItemForEditor.preset;

            // 测试区域
            //XGUI.gui_box(rect_editor, Color.red * 0.3f);

            #region 名称
            Rect rect_editor_name = new Rect(rect_editor.x + left_margin, rect_editor.y + 18, 55, XGUI.GetSingleLineHeight());
            XGUI.gui_label(
               rect: rect_editor_name,
               text: new GUIContent("名称"),
               text_color: Color.white,
               size: XGUIFontSize.M,
               clipping: TextClipping.Clip,
               anchor: TextAnchor.MiddleLeft,
               font_style: FontStyle.Normal,
               font: XGUI.GetFont("xg-medium"));

            Rect rect_editor_name_inputfield = new Rect(rect_editor.x + left_margin + 40, rect_editor.y + 18, 218 - 40, XGUI.GetSingleLineHeight());
            pre.Name = XGUI.gui_inputfield(
                    rect: rect_editor_name_inputfield,
                    prop: pre.Name,
                    text_wrap: false,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 20,
                    field_text_color: Color.white,
                    title_width: 40,
                    //status_icon: "icon_field_status",
                    //status_icon_color: Color.green,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    field_padding: new RectOffset(5, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0));
            #endregion

            #region 说明
            Rect rect_editor_des = new Rect(rect_editor.x + left_margin, rect_editor.y + 48, 55, XGUI.GetSingleLineHeight());
            XGUI.gui_label(
               rect: rect_editor_des,
               text: new GUIContent("说明"),
               text_color: Color.white,
               size: XGUIFontSize.M,
               clipping: TextClipping.Clip,
               anchor: TextAnchor.MiddleLeft,
               font_style: FontStyle.Normal,
               font: XGUI.GetFont("xg-medium"));

            Rect rect_editor_des_inputfield = new Rect(rect_editor.x + left_margin + 40, rect_editor.y + 48, 218 - 40, XGUI.GetSingleLineHeight());
            pre.Description = XGUI.gui_inputfield(
                     rect: rect_editor_des_inputfield,
                     prop: pre.Description,
                     text_wrap: true,
                     field_fontsize: XGUIFontSize.M,
                     field_text_offset: Vector2.zero,
                     field_height: 20,
                     field_text_color: Color.white,
                     title_width: 40,
                     //status_icon: "icon_field_status",
                     //status_icon_color: Color.magenta,
                     field_text_font: XGUI.GetFont("xg-medium"),
                     field_text_style: FontStyle.Normal,
                     field_text_anchor: TextAnchor.UpperLeft,
                     field_padding: new RectOffset(5, 0, 0, 0),
                     field_margin: new RectOffset(0, 0, 0, 0));
            #endregion

            #region 分割线
            Rect rect_sep = new Rect(rect_editor.x + left_margin, rect_editor.y + 85, rect_editor.width - 40, 80);
            Draw_Seperate(rect_sep);
            #endregion

            float int_dis = 10;
            float int_offset = -5;

            float start = 100;
            float offset = 24;
            Color color = Color.white * 0.8f;

            Rect rect_root = new Rect(rect_editor.x + left_margin, rect_editor.y + start, rect_editor.width - 40, XGUI.GetSingleLineHeight());

            // 测试区域
            //XGUI.gui_box(rect_root, Color.green * 0.3f);

            pre.Duration = Draw_Editor_Field(rect_root, new GUIContent("耗时"), pre.Duration);
            rect_root.y += offset;
            pre.Delay = Draw_Editor_Field(rect_root, new GUIContent("延迟"), pre.Delay);
            rect_root.y += offset;
            pre.RandomDelay.Min = Draw_Editor_Field(rect_root, new GUIContent("最小延迟"), pre.RandomDelay.Min);
            rect_root.y += offset;
            pre.RandomDelay.Max = Draw_Editor_Field(rect_root, new GUIContent("最大延迟"), pre.RandomDelay.Max);
            rect_root.y += offset;
            pre.Curve = Draw_Editor_Field(rect_root, new GUIContent("曲线"), pre.Curve);
            rect_root.y += offset;
            pre.LoopCount = Draw_Editor_Field(rect_root, new GUIContent("循环次数"), pre.LoopCount);
            rect_root.y += offset;
            pre.LoopDelay = Draw_Editor_Field(rect_root, new GUIContent("循环延迟"), pre.LoopDelay);
            rect_root.y += offset;

            string _ease = Draw_Editor_Popup(rect_root, "缓动参数", pre.EaseMode.ToString(), ease_names);
            pre.EaseMode = (EaseMode)Enum.Parse(typeof(EaseMode), _ease);

            rect_root.y += offset;

            string _looptype = Draw_Editor_Popup(rect_root, "循环模式", pre.LoopType.ToString(), loop_types_name);
            pre.LoopType = (XTween_LoopType)Enum.Parse(typeof(XTween_LoopType), _looptype);

            rect_root.y += offset;

            pre.UseRandomDelay = XGUI.gui_toggle(
                rect: rect_root,
                title: "随机延迟",
                title_color: Color.white,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(0, 0, 0, 0),
                title_width: 45,
                tog_interval: 10,
                prop: pre.UseRandomDelay,
                tog_style: XGUIToggleStyle.实体,
                tog_bg_off_color: Color.gray,
                tog_bg_on_color: XTween_Dashboard.Theme_Primary,
                tog_handler_off_color: Color.white,
                tog_handler_on_color: Color.white);

            rect_root.y += offset;

            pre.IsRelative = XGUI.gui_toggle(
                rect: rect_root,
                title: "相对模式",
                title_color: Color.white,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(0, 0, 0, 0),
                title_width: 45,
                tog_interval: 10,
                prop: pre.IsRelative,
                tog_style: XGUIToggleStyle.实体,
                tog_bg_off_color: Color.gray,
                tog_bg_on_color: XTween_Dashboard.Theme_Primary,
                tog_handler_off_color: Color.white,
                tog_handler_on_color: Color.white);

            rect_root.y += offset;

            pre.IsAutoKill = XGUI.gui_toggle(
                rect: rect_root,
                title: "自动杀死",
                title_color: Color.white,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(0, 0, 0, 0),
                title_width: 45,
                tog_interval: 10,
                prop: pre.IsAutoKill,
                tog_style: XGUIToggleStyle.实体,
                tog_bg_off_color: Color.gray,
                tog_bg_on_color: XTween_Dashboard.Theme_Primary,
                tog_handler_off_color: Color.white,
                tog_handler_on_color: Color.white);

            rect_root.y += offset;

            pre.UseCurve = XGUI.gui_toggle(
                rect: rect_root,
                title: "使用曲线",
                title_color: Color.white,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(0, 0, 0, 0),
                title_width: 45,
                tog_interval: 10,
                prop: pre.UseCurve,
                tog_style: XGUIToggleStyle.实体,
                tog_bg_off_color: Color.gray,
                tog_bg_on_color: XTween_Dashboard.Theme_Primary,
                tog_handler_off_color: Color.white,
                tog_handler_on_color: Color.white);

            rect_root.y += offset;

            pre.UseFromMode = XGUI.gui_toggle(
                rect: rect_root,
                title: "指定起始",
                title_color: Color.white,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(0, 0, 0, 0),
                title_width: 45,
                tog_interval: 10,
                prop: pre.UseFromMode,
                tog_style: XGUIToggleStyle.实体,
                tog_bg_off_color: Color.gray,
                tog_bg_on_color: XTween_Dashboard.Theme_Primary,
                tog_handler_off_color: Color.white,
                tog_handler_on_color: Color.white);

            rect_root.y += offset;

            pre.IsFavourite = XGUI.gui_toggle(
                rect: rect_root,
                title: "喜爱星标",
                title_color: Color.white,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(0, 0, 0, 0),
                title_width: 45,
                tog_interval: 10,
                prop: pre.IsFavourite,
                tog_style: XGUIToggleStyle.实体,
                tog_bg_off_color: Color.gray,
                tog_bg_on_color: XTween_Dashboard.Theme_Primary,
                tog_handler_off_color: Color.white,
                tog_handler_on_color: Color.white);

            Rect rect_pam_sep = new Rect(rect_editor.x + left_margin, rect_editor.y + 475, rect_editor.width - 40, 80);

            if (pre is XTweenPreset_To to)
            {
                Draw_Seperate(rect_pam_sep);

                rect_root.y += offset * 2.3f;

                switch (to.ToType)
                {
                    case XTweenTypes_To.整数_Int:
                        to.EndValue_Int = Draw_Editor_Field(rect_root, new GUIContent("目标值"), to.EndValue_Int);
                        break;
                    case XTweenTypes_To.浮点数_Float:
                        to.EndValue_Float = Draw_Editor_Field(rect_root, new GUIContent("目标值"), to.EndValue_Float);
                        break;
                    case XTweenTypes_To.字符串_String:
                        to.EndValue_String = Draw_Editor_Field(rect_root, new GUIContent("目标值"), to.EndValue_String);
                        break;
                    case XTweenTypes_To.二维向量_Vector2:
                        to.EndValue_Vector2 = Draw_Editor_Field(rect_root, new GUIContent("目标值"), to.EndValue_Vector2);
                        break;
                    case XTweenTypes_To.三维向量_Vector3:
                        to.EndValue_Vector3 = Draw_Editor_Field(rect_root, new GUIContent("目标值"), to.EndValue_Vector3);
                        break;
                    case XTweenTypes_To.四维向量_Vector4:
                        to.EndValue_Vector4 = Draw_Editor_Field(rect_root, new GUIContent("目标值"), to.EndValue_Vector4);
                        break;
                    case XTweenTypes_To.颜色_Color:
                        to.EndValue_Color = Draw_Editor_Field(rect_root, new GUIContent("目标值"), to.EndValue_Color);
                        break;
                }
                if (to.UseFromMode)
                {
                    switch (to.ToType)
                    {
                        case XTweenTypes_To.整数_Int:
                            rect_root.y += offset;
                            to.FromValue_Int = Draw_Editor_Field(rect_root, new GUIContent("起始值"), to.FromValue_Int);
                            break;
                        case XTweenTypes_To.浮点数_Float:
                            rect_root.y += offset;
                            to.FromValue_Float = Draw_Editor_Field(rect_root, new GUIContent("起始值"), to.FromValue_Float);
                            break;
                        case XTweenTypes_To.字符串_String:
                            rect_root.y += offset;
                            to.FromValue_String = Draw_Editor_Field(rect_root, new GUIContent("起始值"), to.FromValue_String);
                            break;
                        case XTweenTypes_To.二维向量_Vector2:
                            rect_root.y += offset * 2;
                            to.FromValue_Vector2 = Draw_Editor_Field(rect_root, new GUIContent("起始值"), to.FromValue_Vector2);
                            break;
                        case XTweenTypes_To.三维向量_Vector3:
                            rect_root.y += offset * 2;
                            to.FromValue_Vector3 = Draw_Editor_Field(rect_root, new GUIContent("起始值"), to.FromValue_Vector3);
                            break;
                        case XTweenTypes_To.四维向量_Vector4:
                            rect_root.y += offset * 2;
                            to.FromValue_Vector4 = Draw_Editor_Field(rect_root, new GUIContent("起始值"), to.FromValue_Vector4);
                            break;
                        case XTweenTypes_To.颜色_Color:
                            rect_root.y += offset;
                            to.FromValue_Color = Draw_Editor_Field(rect_root, new GUIContent("起始值"), to.FromValue_Color);
                            break;
                    }
                }
            }

            if (pre is XTweenPreset_Path path)
            {
                Draw_Seperate(rect_pam_sep);

                rect_root.y += offset * 2.3f;
                path.PathName = Draw_Editor_Field(rect_root, new GUIContent("路径名称"), path.PathName);
            }

            if (pre is XTweenPreset_Position pos)
            {
                Draw_Seperate(rect_pam_sep);

                rect_root.y += offset * 2.3f;

                switch (pos.PositionType)
                {
                    case XTweenTypes_Positions.锚点位置_AnchoredPosition:
                        pos.EndValue_Vector2 = Draw_Editor_Field(rect_root, new GUIContent("目标值"), pos.EndValue_Vector2);
                        break;
                    case XTweenTypes_Positions.锚点位置3D_AnchoredPosition3D:
                        pos.EndValue_Vector3 = Draw_Editor_Field(rect_root, new GUIContent("目标值"), pos.EndValue_Vector3);
                        break;
                }
                if (pos.UseFromMode)
                {
                    switch (pos.PositionType)
                    {
                        case XTweenTypes_Positions.锚点位置_AnchoredPosition:
                            rect_root.y += offset * 2;
                            pos.FromValue_Vector2 = Draw_Editor_Field(rect_root, new GUIContent("起始值"), pos.FromValue_Vector2);
                            break;
                        case XTweenTypes_Positions.锚点位置3D_AnchoredPosition3D:
                            rect_root.y += offset * 2;
                            pos.FromValue_Vector3 = Draw_Editor_Field(rect_root, new GUIContent("起始值"), pos.FromValue_Vector3);
                            break;
                    }
                }
            }

            if (pre is XTweenPreset_Rotation rot)
            {
                Draw_Seperate(rect_pam_sep);

                rect_root.y += offset * 2.3f;

                string _rotation_type = Draw_Editor_Popup(rect_root, "旋转模式", rot.RotationType.ToString(), rottype_names);
                rot.RotationType = (XTweenTypes_Rotations)Enum.Parse(typeof(XTweenTypes_Rotations), _rotation_type);

                rect_root.y += offset;

                string _rotation_mode = Draw_Editor_Popup(rect_root, "旋转方式", rot.RotationMode.ToString(), rotmode_names);
                rot.RotationMode = (XTweenRotationMode)Enum.Parse(typeof(XTweenRotationMode), _rotation_mode);

                rect_root.y += offset;

                string _rotation_lerp_mode = Draw_Editor_Popup(rect_root, "旋转过渡", rot.RotateLerpMode.ToString(), rotslerp_names);
                rot.RotateLerpMode = (XTweenRotateLerpType)Enum.Parse(typeof(XTweenRotateLerpType), _rotation_lerp_mode);

                rect_root.y += offset;

                string _rotation_space = Draw_Editor_Popup(rect_root, "旋转空间", rot.RotationSpace.ToString(), rotspace_names);
                rot.RotationSpace = (XTweenRotationSpace)Enum.Parse(typeof(XTweenRotationSpace), _rotation_space);

                Rect rect_pam_sep_rot = new Rect(rect_editor.x + left_margin, rect_editor.y + 600, rect_editor.width - 40, 80);
                Draw_Seperate(rect_pam_sep_rot);

                rect_root.y += offset * 2.3f;

                switch (rot.RotationType)
                {
                    case XTweenTypes_Rotations.欧拉角度_Euler:
                        rot.EndValue_Euler = Draw_Editor_Field(rect_root, new GUIContent("目标值"), rot.EndValue_Euler);
                        break;
                    case XTweenTypes_Rotations.四元数_Quaternion:
                        Quaternion qua = rot.EndValue_Quaternion;
                        Vector4 v = new Vector4(qua.x, qua.y, qua.z, qua.w);
                        Vector4 s = Draw_Editor_Field(rect_root, new GUIContent("目标值"), v);
                        rot.EndValue_Quaternion = new Quaternion(s.x, s.y, s.z, s.w);
                        break;
                }
                if (rot.UseFromMode)
                {
                    rect_root.y += offset * 2;
                    switch (rot.RotationType)
                    {
                        case XTweenTypes_Rotations.欧拉角度_Euler:
                            rot.FromValue_Euler = Draw_Editor_Field(rect_root, new GUIContent("起始值"), rot.FromValue_Euler);
                            break;
                        case XTweenTypes_Rotations.四元数_Quaternion:
                            Quaternion qua = rot.FromValue_Quaternion;
                            Vector4 v = new Vector4(qua.x, qua.y, qua.z, qua.w);
                            Vector4 s = Draw_Editor_Field(rect_root, new GUIContent("起始值"), v);
                            rot.FromValue_Quaternion = new Quaternion(s.x, s.y, s.z, s.w);
                            break;
                    }
                }
            }

            if (pre is XTweenPreset_Scale scale)
            {
                Draw_Seperate(rect_pam_sep);

                rect_root.y += offset * 2.3f;
                scale.EndValue = Draw_Editor_Field(rect_root, new GUIContent("目标值"), scale.EndValue);

                if (scale.UseFromMode)
                {
                    rect_root.y += offset * 2;
                    scale.FromValue = Draw_Editor_Field(rect_root, new GUIContent("起始值"), scale.FromValue);
                }
            }

            if (pre is XTweenPreset_Fill fill)
            {
                Draw_Seperate(rect_pam_sep);

                rect_root.y += offset * 2.3f;
                fill.EndValue = Draw_Editor_Field(rect_root, new GUIContent("目标值"), fill.EndValue);

                if (fill.UseFromMode)
                {
                    rect_root.y += offset;
                    fill.FromValue = Draw_Editor_Field(rect_root, new GUIContent("起始值"), fill.FromValue);
                }
            }

            if (pre is XTweenPreset_Size size)
            {
                Draw_Seperate(rect_pam_sep);

                rect_root.y += offset * 2.3f;
                size.EndValue = Draw_Editor_Field(rect_root, new GUIContent("目标值"), size.EndValue);

                if (size.UseFromMode)
                {
                    rect_root.y += offset * 2.3f;
                    size.FromValue = Draw_Editor_Field(rect_root, new GUIContent("起始值"), size.FromValue);
                }
            }

            if (pre is XTweenPreset_Tiled tiled)
            {
                Draw_Seperate(rect_pam_sep);

                rect_root.y += offset * 2.3f;
                tiled.EndValue = Draw_Editor_Field(rect_root, new GUIContent("目标值"), tiled.EndValue);

                if (tiled.UseFromMode)
                {
                    rect_root.y += offset;
                    tiled.FromValue = Draw_Editor_Field(rect_root, new GUIContent("起始值"), tiled.FromValue);
                }
            }

            if (pre is XTweenPreset_Text text)
            {
                Draw_Seperate(rect_pam_sep);

                rect_root.y += offset * 2.3f;

                switch (text.TextType)
                {
                    case XTweenTypes_Text.文字尺寸_FontSize:
                        text.EndValue_Int = Draw_Editor_Field(rect_root, new GUIContent("目标值"), text.EndValue_Int);
                        break;
                    case XTweenTypes_Text.文字行高_LineHeight:
                        text.EndValue_Float = Draw_Editor_Field(rect_root, new GUIContent("目标值"), text.EndValue_Float);
                        break;
                    case XTweenTypes_Text.文字颜色_Color:
                        text.EndValue_Color = Draw_Editor_Field(rect_root, new GUIContent("目标值"), text.EndValue_Color);
                        break;
                    case XTweenTypes_Text.文字内容_Content:
                        text.EndValue_String = Draw_Editor_Field(rect_root, new GUIContent("目标值"), text.EndValue_String);
                        break;
                }

                if (text.UseFromMode)
                {
                    rect_root.y += offset;

                    switch (text.TextType)
                    {
                        case XTweenTypes_Text.文字尺寸_FontSize:
                            text.FromValue_Int = Draw_Editor_Field(rect_root, new GUIContent("起始值"), text.FromValue_Int);
                            break;
                        case XTweenTypes_Text.文字行高_LineHeight:
                            text.FromValue_Float = Draw_Editor_Field(rect_root, new GUIContent("起始值"), text.FromValue_Float);
                            break;
                        case XTweenTypes_Text.文字颜色_Color:
                            text.FromValue_Color = Draw_Editor_Field(rect_root, new GUIContent("起始值"), text.FromValue_Color);
                            break;
                        case XTweenTypes_Text.文字内容_Content:
                            text.FromValue_String = Draw_Editor_Field(rect_root, new GUIContent("起始值"), text.FromValue_String);
                            break;
                    }
                }
            }

            if (pre is XTweenPreset_TmpText tmp)
            {
                Draw_Seperate(rect_pam_sep);

                rect_root.y += offset * 2.3f;

                switch (tmp.TmpTextType)
                {
                    case XTweenTypes_TmpText.文字尺寸_FontSize:
                        tmp.EndValue_Float = Draw_Editor_Field(rect_root, new GUIContent("目标值"), tmp.EndValue_Float);
                        break;
                    case XTweenTypes_TmpText.文字行高_LineHeight:
                        tmp.EndValue_Float = Draw_Editor_Field(rect_root, new GUIContent("目标值"), tmp.EndValue_Float);
                        break;
                    case XTweenTypes_TmpText.文字颜色_Color:
                        tmp.EndValue_Color = Draw_Editor_Field(rect_root, new GUIContent("目标值"), tmp.EndValue_Color);
                        break;
                    case XTweenTypes_TmpText.文字内容_Content:
                        tmp.EndValue_String = Draw_Editor_Field(rect_root, new GUIContent("目标值"), tmp.EndValue_String);
                        break;
                    case XTweenTypes_TmpText.文字边距_Margin:
                        tmp.EndValue_Vector4 = Draw_Editor_Field(rect_root, new GUIContent("目标值"), tmp.EndValue_Vector4);
                        break;
                }

                if (tmp.UseFromMode)
                {
                    switch (tmp.TmpTextType)
                    {
                        case XTweenTypes_TmpText.文字尺寸_FontSize:
                            rect_root.y += offset;
                            tmp.FromValue_Float = Draw_Editor_Field(rect_root, new GUIContent("起始值"), tmp.FromValue_Float);
                            break;
                        case XTweenTypes_TmpText.文字行高_LineHeight:
                            rect_root.y += offset;
                            tmp.FromValue_Float = Draw_Editor_Field(rect_root, new GUIContent("起始值"), tmp.FromValue_Float);
                            break;
                        case XTweenTypes_TmpText.文字颜色_Color:
                            rect_root.y += offset;
                            tmp.FromValue_Color = Draw_Editor_Field(rect_root, new GUIContent("起始值"), tmp.FromValue_Color);
                            break;
                        case XTweenTypes_TmpText.文字内容_Content:
                            rect_root.y += offset;
                            tmp.FromValue_String = Draw_Editor_Field(rect_root, new GUIContent("起始值"), tmp.FromValue_String);
                            break;
                        case XTweenTypes_TmpText.文字边距_Margin:
                            rect_root.y += offset * 2;
                            tmp.FromValue_Vector4 = Draw_Editor_Field(rect_root, new GUIContent("起始值"), tmp.FromValue_Vector4);
                            break;
                    }
                }
            }

            if (pre is XTweenPreset_Alpha alpha)
            {
                Draw_Seperate(rect_pam_sep);

                rect_root.y += offset * 2.3f;
                alpha.EndValue = Draw_Editor_Field(rect_root, new GUIContent("目标值"), alpha.EndValue);

                if (alpha.UseFromMode)
                {
                    rect_root.y += offset;
                    alpha.FromValue = Draw_Editor_Field(rect_root, new GUIContent("起始值"), alpha.FromValue);
                }
            }

            if (pre is XTweenPreset_Shake shake)
            {
                Draw_Seperate(rect_pam_sep);

                rect_root.y += offset * 2.3f;

                string shake_type = Draw_Editor_Popup(rect_root, "抖动类型", shake.ShakeType.ToString(), shaketype_names);
                shake.ShakeType = (XTweenTypes_Shakes)Enum.Parse(typeof(XTweenTypes_Shakes), shake_type);

                rect_root.y += offset;

                shake.FadeShake = XGUI.gui_toggle(
                    rect: rect_root,
                    title: "抖动过渡",
                    title_color: Color.white,
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 0, 0, 0),
                    title_width: 45,
                    tog_interval: 10,
                    prop: shake.FadeShake,
                    tog_style: XGUIToggleStyle.实体,
                    tog_bg_off_color: Color.gray,
                    tog_bg_on_color: XTween_Dashboard.Theme_Primary,
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white);

                rect_root.y += offset;

                shake.Vibrato = Draw_Editor_Field(rect_root, new GUIContent("抖动幅度"), shake.Vibrato);

                rect_root.y += offset;

                shake.Randomness = Draw_Editor_Field(rect_root, new GUIContent("随机化"), shake.Randomness);

                Rect rect_pam_sep_rot = new Rect(rect_editor.x + left_margin, rect_editor.y + 600, rect_editor.width - 40, 80);
                Draw_Seperate(rect_pam_sep_rot);

                rect_root.y += offset * 2.3f;

                switch (shake.ShakeType)
                {
                    case XTweenTypes_Shakes.位置_Position:
                        shake.Strength_Vector3 = Draw_Editor_Field(rect_root, new GUIContent("目标值"), shake.Strength_Vector3);
                        break;
                    case XTweenTypes_Shakes.旋转_Rotation:
                        shake.Strength_Vector3 = Draw_Editor_Field(rect_root, new GUIContent("目标值"), shake.Strength_Vector3);
                        break;
                    case XTweenTypes_Shakes.缩放_Scale:
                        shake.Strength_Vector3 = Draw_Editor_Field(rect_root, new GUIContent("目标值"), shake.Strength_Vector3);
                        break;
                    case XTweenTypes_Shakes.尺寸_Size:
                        shake.Strength_Vector2 = Draw_Editor_Field(rect_root, new GUIContent("目标值"), shake.Strength_Vector2);
                        break;
                }
            }

            if (pre is XTweenPreset_Color col)
            {
                Draw_Seperate(rect_pam_sep);

                rect_root.y += offset * 2.3f;
                col.EndValue = Draw_Editor_Field(rect_root, new GUIContent("目标值"), col.EndValue);

                if (col.UseFromMode)
                {
                    rect_root.y += offset;
                    col.FromValue = Draw_Editor_Field(rect_root, new GUIContent("起始值"), col.FromValue);
                }
            }

            #region 操作按钮
            // 按钮 - 保存修改参数
            rect_editor.Set(rect.x + int_offset + ((rect.width / 2) + btn_edit_ok.width - int_dis), rect.y + rect.height - 50, btn_edit_ok.width, btn_edit_ok.height);
            if (XGUI.gui_button(
                rect: rect_editor,
                tooltip: "收藏的预设",
                tex_release: btn_edit_ok,
                tex_press: btn_edit_ok_press,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                focus_name: "fav_btn"))
            {
                ClearFocus();

                EditorApplication.delayCall += () =>
                {
                    #region 将修改的参数更新并保存到对应的预设类
                    string res = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween预设管理器消息",
                        title: "修改预设",
                        msg: $"您确定要修改分类为：<color=#{XGUI_Utilitys.Color_To_HexString(XTween_Dashboard.Theme_Primary)}> {SelectedPresetItem.type} </color>且名称为：<color=#{XGUI_Utilitys.Color_To_HexString(XTween_Dashboard.Theme_Primary)}> {SelectedPresetItem.preset.Name} </color>的预设参数吗？此修改动作不可逆！请谨慎操作！",
                        ok: "修改",
                        cancel: "暂不",
                        themecolor: XTween_Dashboard.Theme_Primary,
                        PrimaryIndex: 0);

                    if (res == "修改")
                    {
                        // 匹配目标预设项
                        XTweenPresetContainer con = XTween_PresetManager.preset_Container_Load(SelectedPresetItem.type);
                        for (int i = 0; i < con.Presets.Count; i++)
                        {
                            if (con.Presets[i].Name == SelectedPresetItem.preset.Name)
                            {
                                // 脱离引用克隆参数给目标预设
                                con.Presets[i] = pre.Clone();
                            }
                        }

                        // 将当前预设参数列表替换到目标Json文件中
                        XTween_PresetManager.preset_Container_Save_Replace(SelectedPresetItem.type, con.Presets);

                        // 重新载入预设到列表
                        PresetsAppendToList(con);

                        //Debug.Log("已修改：" + SelectedPresetItem.type + "    ----------->>>    " + SelectedPresetItem.preset.Name);
                        // 刷新选择中的预设以返回显示参数模式时的正确参数
                        SelectedPresetItem = new PresetItemGUIStruct()
                        {
                            isPressing = false,
                            preset = pre.Clone(),
                            type = SelectedPresetItemForEditor.type
                        };

                        // 清空修改状态
                        SelectedPresetItemForEditor = null;
                        SetEditorMode(false);

                    };
                    #endregion
                };
            }

            // 按钮 - 取消修改参数
            rect_editor.Set(rect.x + int_offset + ((rect.width / 2) - btn_edit_cancel.width - int_dis), rect.y + rect.height - 50, btn_edit_cancel.width, btn_edit_cancel.height);
            if (XGUI.gui_button(
               rect: rect_editor,
               tooltip: "收藏的预设",
               tex_release: btn_edit_cancel,
               tex_press: btn_edit_cancel_press,
               tex_gui_color: Color.white,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 0),
               focus_name: "fav_btn"))
            {
                ClearFocus();
                SetEditorMode(false);
            }
            #endregion
        }
        #endregion

        #region PresetsButtonLogic
        /// <summary>
        /// 点击分类预设按钮的逻辑
        /// </summary>
        /// <param name="o"></param>
        /// <param name="type"></param>
        private void TweenTypeBtnClickedEvent(Rect o, string type)
        {
            #region  检测是否重复点击同类型按钮
            // 在非星标模式下点击预设分类按钮切换时才会检测当前分类的重复性点击逻辑
            if (!isFavouriteMode)
            {
                if (tweentypes_lastSelectionName == type)
                    return;
            }

            if (!InSearchmode)
                // 保存上一次的预设参数
                LastTweenPresetsSaved();

            PresetSearchString = null;

            InSearchmode = false;

            isFavouriteMode = false;

            // 将新的动画预设类型赋值
            tweentypes_lastSelectionName = type;
            #endregion

            ClearFocus();

            // 将字符串解析成XTweenTypes枚举类
            XTweenTypes xt = (XTweenTypes)XTweenTypeFromString(type);

            // 读取指定枚举类型的预设容器
            XTweenPresetContainer container = LoadPresetsContainer(o, xt);
            PresetsAppendToList(container);
        }
        #endregion

        #region FavouritePresets
        /// <summary>
        /// 将所有星标预设加入列表中
        /// </summary>
        private void FavouritePresetsAddToList()
        {
            // 获取指定类型的所有预设类
            XTweenPresetContainer con_to = LoadPresetsContainer(XTweenTypes.原生动画_To);
            XTweenPresetContainer con_path = LoadPresetsContainer(XTweenTypes.路径_Path);
            XTweenPresetContainer con_pos = LoadPresetsContainer(XTweenTypes.位置_Position);
            XTweenPresetContainer con_rot = LoadPresetsContainer(XTweenTypes.旋转_Rotation);
            XTweenPresetContainer con_scl = LoadPresetsContainer(XTweenTypes.缩放_Scale);
            XTweenPresetContainer con_fill = LoadPresetsContainer(XTweenTypes.填充_Fill);
            XTweenPresetContainer con_size = LoadPresetsContainer(XTweenTypes.尺寸_Size);
            XTweenPresetContainer con_tiled = LoadPresetsContainer(XTweenTypes.平铺_Tiled);
            XTweenPresetContainer con_text = LoadPresetsContainer(XTweenTypes.文字_Text);
            XTweenPresetContainer con_tmp = LoadPresetsContainer(XTweenTypes.文字_TmpText);
            XTweenPresetContainer con_alp = LoadPresetsContainer(XTweenTypes.透明度_Alpha);
            XTweenPresetContainer con_shake = LoadPresetsContainer(XTweenTypes.震动_Shake);
            XTweenPresetContainer con_color = LoadPresetsContainer(XTweenTypes.颜色_Color);

            // 将类型的所有星标预设类加入列表
            FavouritePresetsAppendToList(con_to);
            FavouritePresetsAppendToList(con_path);
            FavouritePresetsAppendToList(con_pos);
            FavouritePresetsAppendToList(con_rot);
            FavouritePresetsAppendToList(con_scl);
            FavouritePresetsAppendToList(con_fill);
            FavouritePresetsAppendToList(con_size);
            FavouritePresetsAppendToList(con_tiled);
            FavouritePresetsAppendToList(con_text);
            FavouritePresetsAppendToList(con_tmp);
            FavouritePresetsAppendToList(con_alp);
            FavouritePresetsAppendToList(con_shake);
            FavouritePresetsAppendToList(con_color);
        }
        #endregion

        #region PresetLogic
        /// <summary>
        /// 获取指定类型的所有动画预设
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private XTweenPresetContainer LoadPresetsContainer(Rect rect, XTweenTypes type)
        {
            // 分类指示器光标坐标设置
            SelectionTypeMark_ReactSet(rect);

            // 获取指定类型的所有预设类
            XTweenPresetContainer container = XTween_PresetManager.preset_Container_Load(type);
            return container;
        }
        /// <summary>
        /// 获取指定类型的所有动画预设
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private XTweenPresetContainer LoadPresetsContainer(XTweenTypes type)
        {
            // 获取指定类型的所有预设类
            XTweenPresetContainer container = XTween_PresetManager.preset_Container_Load(type);
            return container;
        }
        /// <summary>
        /// 将容器中的所有预设添加到列表中
        /// </summary>
        /// <param name="container"></param>
        private void PresetsAppendToList(XTweenPresetContainer container)
        {
            if (loadedpresets == null)
                loadedpresets = new List<PresetItemGUIStruct>();
            loadedpresets.Clear();
            for (int i = 0; i < container.Presets.Count; i++)
            {
                PresetItemGUIStruct strc = new PresetItemGUIStruct();
                strc.isPressing = false;
                strc.preset = container.Presets[i];
                strc.type = (XTweenTypes)Enum.Parse(typeof(XTweenTypes), container.Type);
                loadedpresets.Add(strc);
            }
        }
        /// <summary>
        /// 将容器中的所有星标预设添加到列表中
        /// </summary>
        /// <param name="container"></param>
        private void FavouritePresetsAppendToList(XTweenPresetContainer container)
        {
            if (loadedpresets == null)
                loadedpresets = new List<PresetItemGUIStruct>();

            for (int i = 0; i < container.Presets.Count; i++)
            {
                PresetItemGUIStruct strc = new PresetItemGUIStruct();
                strc.isPressing = false;
                strc.type = (XTweenTypes)Enum.Parse(typeof(XTweenTypes), container.Type);
                strc.preset = container.Presets[i];
                if (strc.preset.IsFavourite)
                {
                    loadedpresets.Add(strc);
                }
            }
        }
        /// <summary>
        /// 保存上一次的预设类型参数文件数据
        /// </summary>
        private void LastTweenPresetsSaved()
        {
            if (isFavouriteMode)
                return;

            // 将字符串解析成XTweenTypes枚举类
            lastXtweenType = (XTweenTypes)XTweenTypeFromString(tweentypes_lastSelectionName);

            if (loadedpresets != null && loadedpresets.Count > 0)
            {
                List<XTweenPresetBase> pres = new List<XTweenPresetBase>();
                for (int i = 0; i < loadedpresets.Count; i++)
                {
                    PresetItemGUIStruct strc = loadedpresets[i];
                    pres.Add(strc.preset);
                }

                // 将当前预设参数列表替换到目标Json文件中
                XTween_PresetManager.preset_Container_Save_Replace(lastXtweenType, pres);
            }
        }
        /// <summary>
        /// 打开星标的预设列表
        /// </summary>
        /// <param name="fav_rect"></param>
        private void OpenFavouritePresets()
        {
            // 在进入星标模式前先保存最后一次选中的预设类型的光标坐标信息
            SaveConfig();

            // 明确进入星标模式
            isFavouriteMode = true;

            // 确保预设列表实例化
            if (loadedpresets == null)
                loadedpresets = new List<PresetItemGUIStruct>();
            // 确保先清空列表
            loadedpresets.Clear();

            // 设置光标
            SelectionTypeMark_OriginalRectSet(new Rect(-50, -50, 0, 0));
            // 将所有星标预设加入列表中
            FavouritePresetsAddToList();
        }
        #endregion

        #region SelectionTypemark
        /// <summary>
        /// 分类指示器光标 - 坐标设置（GUI同步联动赋值）
        /// </summary>
        /// <param name="rect"></param>
        private void SelectionTypeMark_ReactSet(Rect rect)
        {
            selectionmark_lastSelectionRect.Set(rect.x + tweentypes_size - selectionmark_offset, rect.y + (tweentypes_size / 2) - (selectionMark.height / 2), selectionMark.width, selectionMark.height);
        }
        /// <summary>
        /// 分类指示器光标 - 坐标设置（直接赋值，用于初始化）
        /// </summary>
        /// <param name="rect"></param>
        private void SelectionTypeMark_OriginalRectSet(Rect rect)
        {
            selectionmark_lastSelectionRect.Set(rect.x, rect.y, selectionMark.width, selectionMark.height);
        }
        /// <summary>
        /// 分类指示器光标 - 坐标偏移
        /// </summary>
        /// <param name="rect"></param>
        private void SelectionTypeMark_OffsetSet(float val)
        {
            selectionmark_offset = val;
        }
        #endregion

        #region ConfigSave
        /// <summary>
        /// 设置最后一次点击的类型保存配置
        /// </summary>
        private void SaveConfig()
        {
            XTweenConfig.Datas.PresetInFavouriteMode = isFavouriteMode;

            if (!isFavouriteMode)
                // 记录最后一次选择的预设名称
                XTweenConfig.Datas.PresetSelectionMark_LastTypeName = tweentypes_lastSelectionName;
            if (!isFavouriteMode)
                // 记录最后一次选择的预设Rect坐标信息
                XTweenConfig.Datas.PresetSelectionMark_LastRect = selectionmark_lastSelectionRect;

            // 记录最后一次窗口的尺寸
            XTweenConfig.Datas.PresetCentralWindowSize = position.size;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        #endregion

        #region SearchPreset
        private void SearchPreset(string m_PresetSearchString)
        {
            if (string.IsNullOrEmpty(m_PresetSearchString)) { return; }

            if (loadedpresets == null)
                loadedpresets = new List<PresetItemGUIStruct>();
            loadedpresets.Clear();

            List<XTweenPresetContainer> cons = XTween_PresetManager.preset_Container_GetAll();
            for (int i = 0; i < cons.Count; i++)
            {
                for (int s = 0; s < cons[i].Presets.Count; s++)
                {
                    XTweenPresetBase pre = cons[i].Presets[s];
                    if (pre.Name.Contains(m_PresetSearchString))
                    {
                        PresetItemGUIStruct strc = new PresetItemGUIStruct();
                        strc.preset = pre;
                        strc.type = (XTweenTypes)Enum.Parse(typeof(XTweenTypes), cons[i].Type);
                        strc.isPressing = false;
                        loadedpresets.Add(strc);
                    }
                }
            }
        }
        #endregion

        #region Others
        /// <summary>
        /// 清空焦点
        /// </summary>
        private static void ClearFocus()
        {
            GUI.FocusControl(null);
        }
        /// <summary>
        /// 获取动画预设类型按钮类型图标
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Texture2D GetTweenTypeBtnIcon(string name)
        {
            Texture2D tex = null;
            for (int i = 0; i < icons.Length; i++)
            {
                if (icons[i].name == name)
                {
                    tex = icons[i];
                }
            }
            return tex;
        }
        /// <summary>
        /// 获取动画预设类型按钮类型图标（按下状态）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Texture2D GetTweenTypeBtnIcon_Pressed(string name)
        {
            Texture2D tex = null;
            for (int i = 0; i < icons.Length; i++)
            {
                if (icons[i].name == name + "_press")
                {
                    tex = icons[i];
                }
            }
            return tex;
        }
        /// <summary>
        /// 将字符串名称解析为XTweenTypes枚举类
        /// </summary>
        /// <param name="typeString"></param>
        /// <returns></returns>
        private XTweenTypes XTweenTypeFromString(string typeString)
        {
            XTweenTypes types = XTweenTypes.无_None;

            // 获取指定类型的所有预设类
            string[] s = Enum.GetNames(typeof(XTweenTypes));

            for (int i = 0; i < s.Length; i++)
            {
                string a = s[i];
                string t = a.Split(new char[1] { '_' })[1];
                if (t == typeString)
                {
                    types = (XTweenTypes)Enum.Parse(typeof(XTweenTypes), a);
                }
            }
            return types;
        }
        /// <summary>
        /// 获取预设文件的目标文件夹路径
        /// </summary>
        /// <returns>目标文件夹的完整物理路径</returns>
        private string GetPresetsTargetFolder()
        {
            string rootPath = XTween_Dashboard.Get_path_XTween_Presets_Path();
            return rootPath;
        }
        /// <summary>
        /// 从指定文件夹导入预设文件
        /// </summary>
        /// <param name="importPath">要导入的文件夹路径</param>
        /// <returns>导入是否成功</returns>
        private bool ImportPresetsFromFolder(string importPath)
        {
            if (!Directory.Exists(importPath))
            {
                XGUI.dialog(
                    type: XGUIDialogType.错误,
                    windowtitle: "XTween预设管理器消息",
                    title: "导入失败",
                    msg: $"选择的文件夹不存在：{importPath}",
                    ok: "明白",
                    themecolor: XTween_Dashboard.Theme_Primary,
                    PrimaryIndex: 0);
                return false;
            }

            try
            {
                // 获取目标文件夹路径
                string targetFolder = GetPresetsTargetFolder();

                // 确保目标文件夹存在
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                // 获取所有JSON文件
                string[] jsonFiles = Directory.GetFiles(importPath, "*.json");

                if (jsonFiles.Length == 0)
                {
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween预设管理器消息",
                        title: "导入失败",
                        msg: $"选择的文件夹中没有找到JSON文件：{importPath}",
                        ok: "明白",
                        themecolor: XTween_Dashboard.Theme_Primary,
                        PrimaryIndex: 0);
                    return false;
                }

                int successCount = 0;
                int failCount = 0;
                List<string> importedFiles = new List<string>();

                foreach (string jsonFile in jsonFiles)
                {
                    try
                    {
                        // 读取JSON内容
                        string jsonContent = File.ReadAllText(jsonFile);

                        // 解析JSON以验证格式并获取类型信息
                        var container = JsonUtility.FromJson<XTweenPresetContainer>(jsonContent);

                        if (container == null || string.IsNullOrEmpty(container.Type))
                        {
                            Debug.LogWarning($"跳过无效的预设文件: {Path.GetFileName(jsonFile)}");
                            failCount++;
                            continue;
                        }

                        // 从类型字符串中提取文件名（如 "透明度_Alpha" -> "alpha"）
                        XTweenTypes t = (XTweenTypes)Enum.Parse(typeof(XTweenTypes), container.Type);
                        string fileName = XTween_PresetManager.GetFileNameFromType(t);

                        // 构建目标文件路径
                        string targetPath = Path.Combine(targetFolder, $"xtween_presets_{fileName}.json");

                        // 写入文件（覆盖已存在的文件）
                        File.WriteAllText(targetPath, jsonContent);

                        importedFiles.Add(targetPath);
                        successCount++;

                        XGUI_Utilitys.Console("XTween预设管理器消息", $"已导入预设文件: xtween_presets_{fileName}.json", XGUIMsgState.确认);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"导入文件失败 {Path.GetFileName(jsonFile)}: {e.Message}");
                        failCount++;
                    }
                }

                // 刷新AssetDatabase，使新文件在编辑器中可见
                AssetDatabase.Refresh();

                // 重新检查所有预设文件（确保文件结构完整）
                XTween_PresetManager.preset_JsonFile_Checker();

                // 显示导入结果
                string resultMsg = $"导入完成！成功：{successCount} 个，失败：{failCount} 个";

                if (failCount > 0)
                {
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween预设管理器消息",
                        title: "导入结果",
                        msg: resultMsg + " 部分文件导入失败，请查看控制台日志。",
                        ok: "明白",
                        themecolor: XTween_Dashboard.Theme_Primary,
                        PrimaryIndex: 0);
                }
                else
                {
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween预设管理器消息",
                        title: "导入成功",
                        msg: resultMsg,
                        ok: "明白",
                        themecolor: XTween_Dashboard.Theme_Primary,
                        PrimaryIndex: 0);
                }

                XGUI_Utilitys.Console("XTween预设管理器消息", resultMsg, failCount > 0 ? XGUIMsgState.警告 : XGUIMsgState.确认);

                return successCount > 0;
            }
            catch (Exception e)
            {
                Debug.LogError($"导入预设失败: {e.Message}");
                XGUI.dialog(
                    type: XGUIDialogType.错误,
                    windowtitle: "XTween预设管理器消息",
                    title: "导入失败",
                    msg: $"导入过程中发生错误： {e.Message}",
                    ok: "明白",
                    themecolor: XTween_Dashboard.Theme_Primary,
                    PrimaryIndex: 0);
                return false;
            }
        }
        /// <summary>
        /// 清空所有预设文件的内容
        /// </summary>
        /// <returns>是否全部清空成功</returns>
        private bool ClearAllPresetFiles()
        {
#if UNITY_EDITOR
            try
            {
                // 获取所有预设类型
                XTweenTypes[] allTypes = new XTweenTypes[]
                {
            XTweenTypes.透明度_Alpha,
            XTweenTypes.原生动画_To,
            XTweenTypes.路径_Path,
            XTweenTypes.位置_Position,
            XTweenTypes.旋转_Rotation,
            XTweenTypes.缩放_Scale,
            XTweenTypes.尺寸_Size,
            XTweenTypes.震动_Shake,
            XTweenTypes.颜色_Color,
            XTweenTypes.填充_Fill,
            XTweenTypes.平铺_Tiled,
            XTweenTypes.文字_Text,
            XTweenTypes.文字_TmpText
                };

                int successCount = 0;
                int failCount = 0;

                foreach (XTweenTypes type in allTypes)
                {
                    try
                    {
                        // 创建空的预设容器
                        XTweenPresetContainer emptyContainer = new XTweenPresetContainer
                        {
                            Type = type.ToString(),
                            Presets = new List<XTweenPresetBase>()
                        };

                        // 序列化为JSON
                        string jsonContent = JsonUtility.ToJson(emptyContainer, true);

                        // 保存到文件
                        string fileName = XTween_PresetManager.GetFileNameFromType(type);
                        string fullPath = XTween_PresetManager.GetPresetFilePath(fileName);

                        // 确保目录存在
                        string directory = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        // 写入文件
                        File.WriteAllText(fullPath, jsonContent);

                        successCount++;

                        XGUI_Utilitys.Console("XTween预设管理器消息", $"已清空预设文件: xtween_presets_{fileName}.json", XGUIMsgState.确认);
                    }
                    catch (Exception e)
                    {
                        failCount++;
                        Debug.LogError($"清空预设文件失败 {type}: {e.Message}");
                    }
                }

                // 刷新AssetDatabase
                AssetDatabase.Refresh();

                // 输出结果
                string resultMsg = $"清空预设完成！成功：{successCount} 个，失败：{failCount} 个";
                XGUI_Utilitys.Console("XTween预设管理器消息", resultMsg, failCount > 0 ? XGUIMsgState.警告 : XGUIMsgState.确认);

                return failCount == 0;
            }
            catch (Exception e)
            {
                XGUI.dialog(
                    type: XGUIDialogType.错误,
                    windowtitle: "XTween预设管理器消息",
                    title: "清空失败",
                    msg: $"清空过程中发生错误：\n{e.Message}",
                    ok: "明白",
                    themecolor: XTween_Dashboard.Theme_Primary,
                    PrimaryIndex: 0);
                return false;
            }
#else
    return false;
#endif
        }
        private void DrawInfo(Rect rect = default, float offset = 0, string title = null, string subtitle = null, Color value_color = default)
        {
            rect.y += offset;

            XGUI.gui_state_displayer_text(
                rect: rect,
                title: new GUIContent(title),
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_color: Color.white * 0.85f,
                subtitle: new GUIContent(subtitle),
                subtitle_size: XGUIFontSize.M,
                subtitle_color: value_color,
                padding: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0));
        }
        #endregion

        #region Apply
        /// <summary>
        /// 将预设应用到动画控制器上
        /// </summary>
        /// <param name="preset"></param>
        public static void ApplyToController(XTweenPresetBase preset)
        {
            GameObject[] objs = Selection.gameObjects;

            if (objs.Length <= 0)
            {
                string cdr = XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XTween预设管理器消息",
                    title: "无效应用",
                    msg: $"请先至少选中一个动画控制器物体再应用预设！",
                    ok: "明白",
                    themecolor: XTween_Dashboard.Theme_Primary,
                    PrimaryIndex: 0);
                return;
            }

            for (int i = 0; i < objs.Length; i++)
            {
                XGUI_Utilitys.Console("XTween预设管理器消息", $"已应用预设 '{preset.Name}' 到动画控制器！", XGUIMsgState.确认);
                XTween_Controller con = objs[i].GetComponent<XTween_Controller>();

                Undo.RecordObject(con, $"Apply preset for {con.gameObject.GetInstanceID()}");
                con.preset_Apply_To_Controller(preset);
            }
        }
        #endregion

        #region  MenuAction
        /// <summary>
        /// 编辑预设
        /// </summary>
        /// <param name="userData"></param>
        private void OnEditPreset(object userData)
        {
            PresetItemGUIStruct pret = userData as PresetItemGUIStruct;
            if (pret != null)
            {
                Debug.Log($"编辑预设: {pret.preset.Name}");
            }
            SetEditorMode(true);
        }
        /// <summary>
        /// 应用预设
        /// </summary>
        /// <param name="userData"></param>
        private void OnApplyPreset(object userData)
        {
            PresetItemGUIStruct pret = userData as PresetItemGUIStruct;
            if (pret != null)
            {
                ApplyToController(pret.preset);
            }
        }
        /// <summary>
        /// 删除预设
        /// </summary>
        /// <param name="userData"></param>
        private void OnDeletePreset(object userData)
        {
            PresetItemGUIStruct pret = userData as PresetItemGUIStruct;
            if (pret != null)
            {
                EditorApplication.delayCall += () =>
                {
                    string cdr = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween预设管理器消息",
                        title: "删除预设",
                        msg: $"确定要删除预设 '{pret.preset.Name}' 吗？此操作不可逆！请谨慎操作！",
                        ok: "删除",
                        cancel: "暂不",
                        themecolor: XTween_Dashboard.Theme_Primary,
                        PrimaryIndex: 1);

                    // 确认删除
                    if (cdr == "删除")
                    {
                        // 先获取基础参数
                        XTweenTypes pre_type = pret.type;
                        string pre_name = pret.preset.Name;

                        // 从列表中移除项
                        loadedpresets.Remove(pret);

                        //Debug.Log(pre_type + "/" + pre_name);

                        // 从json中移除
                        XTween_PresetManager.preset_Container_DeletePreset(pre_type, pre_name);

                        // ✅ 重新加载当前类型的预设列表
                        if (!isFavouriteMode)
                        {
                            XTweenPresetContainer container = XTween_PresetManager.preset_Container_Load(pre_type);
                            PresetsAppendToList(container);
                        }
                        else
                        {
                            // 如果是星标模式，重新加载所有星标预设
                            OpenFavouritePresets();
                        }


                        if (SelectedPresetItem == pret)
                            SelectedPresetItem = null;
                        Repaint();
                    }
                };
            }
        }
        /// <summary>
        /// 预设星标标记操作
        /// </summary>
        /// <param name="userData"></param>
        private void OnToggleFavourite(object userData)
        {
            PresetItemGUIStruct pret = userData as PresetItemGUIStruct;
            if (pret != null)
            {
                pret.preset.IsFavourite = !pret.preset.IsFavourite;

                if (isFavouriteMode)
                {
                    //// 刷新星标列表
                    //OpenFavouritePresets();
                }
                Repaint();
            }
        }
        /// <summary>
        /// 设置编辑器模式
        /// </summary>
        /// <param name="state"></param>
        private void SetEditorMode(bool state)
        {
            IsEditorMode = state;

            if (state)
            {
                if (SelectedPresetItem != null)
                {
                    SelectedPresetItemForEditor = SelectedPresetItem.Clone();
                }
            }
            else
            {
                SelectedPresetItemForEditor = null;
            }
        }
        #endregion

        #region WindowSizeStatus
        private bool isSize_HideInfo()
        {
            if (position.width < 927)
                return true;
            else
                return false;
        }
        private bool isSize_HideSearchWidth()
        {
            if (position.width >= 635)
                return true;
            else
                return false;
        }

        #endregion

        #region DrawField
        private void Draw_Seperate(Rect rect)
        {
            #region 分割线
            XGUI.gui_seperator(
                rect: rect,
                thickness: 1,
                color: Color.white * 0.5f);
            #endregion
        }

        private float Draw_Editor_Field(Rect rect, GUIContent title, float value)
        {
            return XGUI.gui_inputfield(
                    rect: rect,
                    title: title.text,
                    prop: value,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 20,
                    field_text_color: Color.white,
                    title_width: 80,
                    //status_icon: "icon_field_status",
                    //status_icon_color: Color.red,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    field_padding: new RectOffset(5, 5, 0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0));
        }

        private int Draw_Editor_Field(Rect rect, GUIContent title, int value)
        {
            return XGUI.gui_inputfield(
                     rect: rect,
                     title: title.text,
                     prop: value,
                     field_fontsize: XGUIFontSize.M,
                     field_text_offset: Vector2.zero,
                     field_height: 20,
                     field_text_color: Color.white,
                     title_width: 80,
                     //status_icon: "icon_field_status",
                     //status_icon_color: Color.red,
                     field_text_font: XGUI.GetFont("xg-medium"),
                     field_text_style: FontStyle.Normal,
                     field_text_anchor: TextAnchor.MiddleLeft,
                     field_padding: new RectOffset(5, 5, 0, 0),
                     field_margin: new RectOffset(0, 0, 0, 0));
        }

        private string Draw_Editor_Field(Rect rect, GUIContent title, string value)
        {
            return XGUI.gui_inputfield(
                    rect: rect,
                    title: title.text,
                    prop: value,
                    text_wrap: false,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 20,
                    field_text_color: Color.white,
                    title_width: 80,
                    //status_icon: "icon_field_status",
                    //status_icon_color: Color.green,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    field_padding: new RectOffset(5, 5, 0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0));
        }

        private Vector2 Draw_Editor_Field(Rect rect, GUIContent title, Vector2 value)
        {
            return XGUI.gui_inputfield(
                     rect: rect,
                     title: title.text,
                     prop: value,
                     //status_icon: "icon_field_status",
                     //status_icon_color: Color.red,
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_width: 80,
                     title_anchor: TextAnchor.MiddleLeft);
        }

        private Vector3 Draw_Editor_Field(Rect rect, GUIContent title, Vector3 value)
        {
            return XGUI.gui_inputfield(
                     rect: rect,
                     title: title.text,
                     prop: value,
                     //status_icon: "icon_field_status",
                     //status_icon_color: Color.red,
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_anchor: TextAnchor.MiddleLeft,
                     title_width: 80);
        }

        private Vector4 Draw_Editor_Field(Rect rect, GUIContent title, Vector4 value)
        {
            return XGUI.gui_inputfield(
                     rect: rect,
                     title: title.text,
                     prop: value,
                     //status_icon: "icon_field_status",
                     //status_icon_color: Color.red,
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_anchor: TextAnchor.MiddleLeft,
                     title_width: 80);
        }

        private Quaternion Draw_Editor_Field(Rect rect, GUIContent title, Quaternion value)
        {
            return XGUI.gui_inputfield(
                     rect: rect,
                     title: title.text,
                     prop: value,
                     //status_icon: "icon_field_status",
                     //status_icon_color: Color.red,
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_anchor: TextAnchor.MiddleLeft,
                     title_width: 80);
        }

        private Color Draw_Editor_Field(Rect rect, GUIContent title, Color value)
        {
            return XGUI.gui_inputfield(
                     rect: rect,
                     title: title.text,
                     prop: value,
                     //status_icon: "icon_field_status",
                     //status_icon_color: Color.red,
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_anchor: TextAnchor.MiddleLeft,
                     title_width: 80);
        }

        private AnimationCurve Draw_Editor_Field(Rect rect, GUIContent title, AnimationCurve value)
        {
            return XGUI.gui_inputfield(
                     rect: rect,
                     title: title.text,
                     prop: value,
                     //status_icon: "icon_field_status",
                     //status_icon_color: Color.red,
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_anchor: TextAnchor.MiddleLeft,
                     title_width: 80);
        }

        private string Draw_Editor_Popup(Rect rect, string title, string value, string[] options)
        {
            return XGUI.gui_string_popup(
                  rect: rect,
                  title: title,
                  title_size: XGUIFontSize.M,
                  title_font_style: FontStyle.Normal,
                  title_padding: new RectOffset(0, 0, 0, 0),
                  title_width: 80,
                  prop: value,
                  options: options,
                  opt_text_size: XGUIFontSize.M,
                  opt_text_color: Color.black,
                  opt_text_padding: new RectOffset(10, 10, 0, 0),
                  opt_anchor: TextAnchor.MiddleCenter,
                  opt_font_style: FontStyle.Normal,
                  opt_bg_fill: XGUIFilled.实体,
                  opt_bg_color: XGUIColor.亮白,
                  opt_bg_color_gui: XTween_Dashboard.Theme_Primary,
                  icon_arrow_color: Color.black);
        }
        #endregion

        private void OnDestroy()
        {
            SaveConfig();
        }
    }
}
