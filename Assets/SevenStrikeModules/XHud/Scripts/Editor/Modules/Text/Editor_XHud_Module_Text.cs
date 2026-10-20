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
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using TextEditor = UnityEditor.UI.TextEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Text))]
    public class Editor_XHud_Module_Text : TextEditor
    {
        #region 字段 - 组件
        /// <summary>
        /// 目标脚本实例。
        /// <para/>
        /// 在 <see cref="OnEnable"/> 中缓存。
        /// </summary>
        private XHud_Module_Text BaseScript;
        /// <summary>
        /// XHud 管理器实例。
        /// <para/>
        /// 用于访问文字样式库。
        /// </summary>
        private XHud_Manager HudManager;
        #endregion

        #region 字段 - 折叠状态
        /// <summary>
        /// 原始脚本参数显示开关。
        /// </summary>
        private bool OriginalDisplay;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 标识名称。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_Indicator;
        /// <summary>
        /// 样式名称。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_StyleName;
        /// <summary>
        /// 尺寸全局联动开关。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_SyncGlobalFontSize;
        /// <summary>
        /// 实时匹配库开关。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_StyleLibSynching;
        /// <summary>
        /// 文字样式信息数据。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_TextStyleInfo;
        /// <summary>
        /// 文字内容。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_Text;
        /// <summary>
        /// 颜色接管者。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_TextColorOwner;
        /// <summary>
        /// 文字样式。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_txt_style;
        /// <summary>
        /// 内容锚点。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_txt_content_anchor;
        /// <summary>
        /// 字体。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_txt_font;
        /// <summary>
        /// 尺寸。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_txt_size;
        /// <summary>
        /// 行高。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_txt_line_height;
        /// <summary>
        /// 水平溢出。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_txt_overflow_h;
        /// <summary>
        /// 垂直溢出。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_txt_overflow_v;
        /// <summary>
        /// 富文本支持。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_txt_rich;
        /// <summary>
        /// 自适应尺寸。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_txt_bestfit;
        /// <summary>
        /// 自适应最小尺寸。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_txt_fit_min;
        /// <summary>
        /// 自适应最大尺寸。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_txt_fit_max;
        /// <summary>
        /// 字体颜色。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_txt_font_color;
        /// <summary>
        /// 几何对齐。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_txt_geometre_align;
        /// <summary>
        /// 库同步 - 对齐。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_align;
        /// <summary>
        /// 库同步 - 字体。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_font;
        /// <summary>
        /// 库同步 - 样式。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_style;
        /// <summary>
        /// 库同步 - 尺寸。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_size;
        /// <summary>
        /// 库同步 - 颜色。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_color;
        /// <summary>
        /// 库同步 - 富文本。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_rich;
        /// <summary>
        /// 库同步 - 行高。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_line;
        /// <summary>
        /// 库同步 - 水平溢出。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_overflow_h;
        /// <summary>
        /// 库同步 - 垂直溢出。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_overflow_v;
        /// <summary>
        /// 库同步 - 自适应尺寸。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_bestfit;
        /// <summary>
        /// 库同步 - 射线检测。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_raycast;
        /// <summary>
        /// 库同步 - 遮罩。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_maskable;
        /// <summary>
        /// 库同步 - 几何对齐。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_geometre_align;
        /// <summary>
        /// 射线检测。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_Raycast;
        /// <summary>
        /// 遮罩。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_Maskable;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 定位按钮释放图标。
        /// </summary>
        private Texture2D locate_r;
        /// <summary>
        /// 定位按钮按下图标。
        /// </summary>
        private Texture2D locate_p;
        /// <summary>
        /// 窗口主图标。
        /// </summary>
        private Texture2D icon_main;
        /// <summary>
        /// 保存按钮释放图标。
        /// </summary>
        private Texture2D save_r;
        /// <summary>
        /// 保存按钮按下图标。
        /// </summary>
        private Texture2D save_p;
        /// <summary>
        /// 打开库按钮释放图标。
        /// </summary>
        private Texture2D openlib_r;
        /// <summary>
        /// 打开库按钮按下图标。
        /// </summary>
        private Texture2D openlib_p;
        /// <summary>
        /// 刷新按钮释放图标。
        /// </summary>
        private Texture2D update_r;
        /// <summary>
        /// 刷新按钮按下图标。
        /// </summary>
        private Texture2D update_p;
        /// <summary>
        /// 颜色接管者选择器参数。
        /// </summary>
        [NonSerialized]
        private clickselector_args[] clickselector_args = new clickselector_args[4];
        #endregion

        #region 字段 - 选项文字
        /// <summary>
        /// 开关选项文字（关闭 / 开启）。
        /// </summary>
        private string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        /// <summary>
        /// 尺寸同步选项文字（原生 / 增量）。
        /// </summary>
        private string[] stroptions_syncsize = new string[] { "原生", "增量" };
        /// <summary>
        /// 颜色同步选项文字（原生 / 接管）。
        /// </summary>
        private string[] stroptions_synccolor = new string[] { "原生", "接管" };
        /// <summary>
        /// 样式同步选项文字（原生 / 同步）。
        /// </summary>
        private string[] stroptions_syncstyle = new string[] { "原生", "同步" };
        /// <summary>
        /// 颜色接管者名称。
        /// </summary>
        private string[] tooltip_owner_names = new string[4] { "文字库", "配色器", "动画器", "自身 / 手动" };
        #endregion

        #region 工具 - 批量化操作
        /// <summary>
        /// 当前 Inspector 选中的所有目标组件缓存。
        /// </summary>
        private XHud_Module_Text[] SelectedObjects;
        /// <summary>
        /// 缓存当前 Inspector 选中的所有目标组件到 <see cref="SelectedObjects"/>。
        /// <para/>
        /// 多选时逐个转换，单选时只填充一个元素。
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Text[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Text)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Text[targets.Length];
                SelectedObjects[0] = (XHud_Module_Text)target;
            }
        }
        /// <summary>
        /// 判断当前是否处于多选状态。
        /// </summary>
        /// <returns>true 表示选中了多个组件</returns>
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

        #region 生命周期
        /// <summary>
        /// 禁用预览窗口
        /// </summary>
        /// <returns></returns>
        public override bool HasPreviewGUI()
        {
            // 返回 false 即隐藏预览窗口
            return true;
        }
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            string h_color = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary);

            XGUI.gui_icon(
                rect: new Rect(r.x + (r.width / 2) - 30, r.y + (r.height / 2) - 40, 60, 60),
                icon: clickselector_args[sp_TextColorOwner.enumValueIndex].btn_icon_r);

            XGUI.gui_label(
                rect: new Rect(r.x + (r.width / 2) - (r.width / 2), r.y + (r.height / 2) + 15, r.width, XGUI.GetSingleLineHeight()),
                text: new GUIContent($"文字颜色接管者为：<color=#{h_color}> {tooltip_owner_names[sp_TextColorOwner.enumValueIndex]}</color>"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleCenter,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
        }
        /// <summary>
        /// Unity 启用回调。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>调用基类 OnEnable；</description></item>
        /// <item><description>获取 XHud 管理器；</description></item>
        /// <item><description>缓存目标脚本；</description></item>
        /// <item><description>绑定全部序列化属性；</description></item>
        /// <item><description>加载全部图标资源与颜色接管者选择器；</description></item>
        /// <item><description>缓存所有选中组件；</description></item>
        /// <item><description>初始化默认样式名称。</description></item>
        /// </list>
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            HudManager = XHud_Dashboard.GetManager();

            BaseScript = (XHud_Module_Text)target;

            #region 获取序列化变量
            sp_Indicator = serializedObject.FindProperty("Indicator");
            sp_StyleName = serializedObject.FindProperty("StyleName");
            sp_StyleLibSynching = serializedObject.FindProperty("StyleLibSynching");
            sp_SyncGlobalFontSize = serializedObject.FindProperty("SyncGlobalFontSize");
            sp_TextStyleInfo = serializedObject.FindProperty("TextStyleInfo");
            sp_Text = serializedObject.FindProperty("m_Text");
            sp_TextColorOwner = serializedObject.FindProperty("TextColorOwner");

            sp_txt_style = sp_TextStyleInfo.FindPropertyRelative("Style");
            sp_txt_content_anchor = sp_TextStyleInfo.FindPropertyRelative("ContentAnchor");
            sp_txt_font = sp_TextStyleInfo.FindPropertyRelative("Font");
            sp_txt_size = sp_TextStyleInfo.FindPropertyRelative("Size");
            sp_txt_line_height = sp_TextStyleInfo.FindPropertyRelative("LineHeight");
            sp_txt_overflow_h = sp_TextStyleInfo.FindPropertyRelative("Overflow_Horizon");
            sp_txt_overflow_v = sp_TextStyleInfo.FindPropertyRelative("Overflow_Vertical");
            sp_txt_rich = sp_TextStyleInfo.FindPropertyRelative("RichText");
            sp_txt_bestfit = sp_TextStyleInfo.FindPropertyRelative("BestFit");
            sp_txt_fit_min = sp_TextStyleInfo.FindPropertyRelative("Fit_Min");
            sp_txt_fit_max = sp_TextStyleInfo.FindPropertyRelative("Fit_Max");
            sp_txt_font_color = sp_TextStyleInfo.FindPropertyRelative("FontColor");
            sp_txt_geometre_align = sp_TextStyleInfo.FindPropertyRelative("GeometreAlign");

            //---------------------------------LibraryToggle---------------------------------//
            sp_LibStyle_Effect_align = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_align");
            sp_LibStyle_Effect_font = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_font");
            sp_LibStyle_Effect_style = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_style");
            sp_LibStyle_Effect_size = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_size");
            sp_LibStyle_Effect_color = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_color");
            sp_LibStyle_Effect_rich = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_rich");
            sp_LibStyle_Effect_line = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_line");
            sp_LibStyle_Effect_overflow_h = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_overflow_h");
            sp_LibStyle_Effect_overflow_v = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_overflow_v");
            sp_LibStyle_Effect_bestfit = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_bestfit");
            sp_LibStyle_Effect_raycast = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_raycast");
            sp_LibStyle_Effect_maskable = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_maskable");
            sp_LibStyle_Effect_geometre_align = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_geometre_align");
            //--------------------------------- Features ---------------------------------//
            sp_Raycast = sp_TextStyleInfo.FindPropertyRelative("Raycast");
            sp_Maskable = sp_TextStyleInfo.FindPropertyRelative("Maskable");
            #endregion

            #region 获取图标
            locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_text/locate_r");
            locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_text/locate_p");
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_text/icon_main");
            save_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_text/save_r");
            save_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_text/save_p");
            openlib_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_text/openlib_r");
            openlib_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_text/openlib_p");
            update_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_text/update_r");
            update_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_text/update_p");

            for (int i = 0; i < clickselector_args.Length; i++)
            {
                clickselector_args[i] = new clickselector_args();
                clickselector_args[i].btn_icon_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_text/icon_color_owner_r_{i}");
                clickselector_args[i].btn_icon_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_text/icon_color_owner_p_{i}");
                clickselector_args[i].btn_tooltip = tooltip_owner_names[i];
            }
            #endregion

            Targets_Get();

            if (HudManager != null)
            {
                if (HudManager.Hud_TextStyleLibrary != null)
                {
                    if (string.IsNullOrEmpty(sp_StyleName.stringValue))
                    {
                        sp_StyleName.stringValue = HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(xHud_TextType.Text).Name;
                        sp_StyleName.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }
        /// <summary>
        /// Unity 禁用回调。
        /// <para/>
        /// 直接调用基类实现。
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        /// <summary>
        /// 绘制 Inspector 主入口。
        /// <para/>
        /// 按自上而下的顺序绘制所有面板：
        /// 标题 → 快捷功能 → 颜色接管者 → 选项 → 文字样式库 → 库同步项 →
        /// 文字内容 → 参数 → 右键菜单 → 源脚本。
        /// </summary>
        public override void OnInspectorGUI()
        {
            string h_color = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary);

            serializedObject.Update();

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XHud_Dashboard.Theme_Primary,
                title_text: "XHud  -  文字",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: XGUI.TryEllipsisClipping(),
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            #region 快捷功能
            XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIGroup.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: XHud_Dashboard.Theme_Group,
              title: "快捷功能",
              title_size: XGUIFontSize.M,
              title_text_color: XHud_Dashboard.Theme_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(15, 15, 20, 15));

            #region 编组
            XGUI.layout_group_start(
               type: XGUIContainerType.Horizontal,
               absolute_margin: true,
               absolute_padding: true,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 0));

            if (!Targets_Selected())
            {
                if (HudManager.Hud_TextStyleLibrary)
                {
                    #region 将当前样式保存到字体库
                    if (XGUI.layout_button(
                    tooltip: "将当前样式保存到字体库",
                    tex_release: save_r,
                    tex_press: save_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: 14,
                    height: 14))
                    {
                        if (Application.isPlaying)
                            return;

                        ///---新建文字模版信息类
                        XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
                        info.Type = xHud_TextType.Text;
                        info.Style = (FontStyle)sp_txt_style.enumValueIndex;
                        info.ContentAnchor = (ContentAnchor)sp_txt_content_anchor.enumValueIndex;
                        info.Font = sp_txt_font.objectReferenceValue as Font;
                        info.Size = sp_txt_size.floatValue;
                        info.LineHeight = sp_txt_line_height.floatValue;
                        info.Overflow_Horizon = (HorizontalWrapMode)sp_txt_overflow_h.enumValueIndex;
                        info.Overflow_Vertical = (VerticalWrapMode)sp_txt_overflow_v.enumValueIndex;
                        info.RichText = sp_txt_rich.boolValue;
                        info.GeometreAlign = sp_txt_geometre_align.boolValue;
                        info.BestFit = sp_txt_bestfit.boolValue;
                        info.Fit_Min = sp_txt_fit_min.intValue;
                        info.Fit_Max = sp_txt_fit_max.intValue;
                        info.FontColor = sp_txt_font_color.colorValue;
                        info.Raycast = sp_Raycast.boolValue;
                        info.Maskable = sp_Maskable.boolValue;

                        Open_Hud_Library_TextStyle_Setter(info);
                    }
                    #endregion

                    XGUI.layout_flexspace();

                    #region 查看当前字体库
                    if (XGUI.layout_button(
                    tooltip: "查看当前字体库",
                    tex_release: openlib_r,
                    tex_press: openlib_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: 14,
                    height: 14))
                    {
                        if (HudManager.Hud_TextStyleLibrary != null)
                            EditorUtility.OpenPropertyEditor(HudManager.Hud_TextStyleLibrary);
                    }
                    #endregion
                }
                if (HudManager.Hud_TextStyleLibrary != null && !HudManager.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                {
                    XGUI.layout_flexspace();

                    #region 刷新样式
                    if (XGUI.layout_button(
                        tooltip: "根据选择的快速样式刷新文字样式",
                        tex_release: update_r,
                        tex_press: update_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        UpdateTextStyle_WithTarget();
                        return;
                    }
                    #endregion
                }
            }
            else
            {
                #region 批量刷新样式
                if (!HudManager.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                {
                    if (XGUI.layout_button(
                        tooltip: "根据选择的快速样式刷新文字样式",
                        tex_release: update_r,
                        tex_press: update_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        UpdateTextStyle_WithTarget();
                        return;
                    }
                }
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 颜色接管者
            XGUI.layout_group_start(
                 type: XGUIContainerType.Vertical,
                 bg_fill: XGUIGroup.缺口纯色边框,
                 bg_color: XGUIColor.亮白,
                 bg_color_gui: XHud_Dashboard.Theme_Group,
                 title: "颜色接管者",
                 title_size: XGUIFontSize.M,
                 title_text_color: XHud_Dashboard.Theme_Primary,
                 title_clipping: TextClipping.Clip,
                 padding: new RectOffset(10, 10, 15, 15),
                 foldout: BaseScript.fold_option);

            XGUI.layout_label(
                           text: $"文字颜色由  <color=#{h_color}>{tooltip_owner_names[sp_TextColorOwner.enumValueIndex]}</color>  接管",
                           size: XGUIFontSize.S,
                           anchor: TextAnchor.MiddleCenter,
                           text_color: Color.white,
                           offset: new Vector2(0, 0),
                           padding: new RectOffset(0, 0, 0, 0),
                           margin: new RectOffset(0, 0, 6, 0),
                           font_style: FontStyle.Normal,
                           clipping: TextClipping.Clip);

            sp_TextColorOwner.enumValueIndex = XGUI.layout_clickselector(
                index: sp_TextColorOwner.enumValueIndex,
                icon_btns: clickselector_args,
                btn_color_normal: Color.white,
                btn_color_selected: XHud_Dashboard.Theme_Primary,
                btn_height: 60,
                padding: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0)
                );

            sp_TextColorOwner.serializedObject.ApplyModifiedProperties();

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            BaseScript.fold_option = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_option);

            if (!BaseScript.fold_option)
            {
                #region 射线检测
                XGUI.SetEnabled(!sp_LibStyle_Effect_raycast.boolValue || !sp_StyleLibSynching.boolValue);
                DrawLabeledToggle("射线检测", "", sp_Raycast, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                XGUI.SetEnabled(true);
                #endregion

                #region 遮罩
                XGUI.SetEnabled(!sp_LibStyle_Effect_maskable.boolValue || !sp_StyleLibSynching.boolValue);
                DrawLabeledToggle("遮罩", "", sp_Maskable, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                XGUI.SetEnabled(true);
                #endregion

                #region 富文本支持
                XGUI.SetEnabled(!sp_LibStyle_Effect_rich.boolValue || !sp_StyleLibSynching.boolValue);
                DrawLabeledToggle("富文本支持", "", sp_txt_rich, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                XGUI.SetEnabled(true);
                #endregion

                #region 几何对齐
                XGUI.SetEnabled(!sp_LibStyle_Effect_geometre_align.boolValue || !sp_StyleLibSynching.boolValue);
                DrawLabeledToggle("几何对齐", "", sp_txt_geometre_align, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                XGUI.SetEnabled(true);
                #endregion

                #region 自适应尺寸
                XGUI.SetEnabled(!sp_LibStyle_Effect_size.boolValue && !sp_LibStyle_Effect_bestfit.boolValue || !sp_StyleLibSynching.boolValue);
                DrawLabeledToggle("自适应尺寸", "", sp_txt_bestfit, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) =>
                {
                    if (b)
                    {
                        sp_txt_overflow_h.enumValueIndex = (int)HorizontalWrapMode.Wrap;
                        sp_txt_overflow_v.enumValueIndex = (int)VerticalWrapMode.Truncate;
                    }
                });
                XGUI.SetEnabled(true);
                #endregion

                #region 尺寸全局联动
                DrawLabeledToggle("尺寸全局联动", "", sp_SyncGlobalFontSize, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncsize, (b) => { });
                #endregion

                #region 实时匹配库
                if (HudManager != null && HudManager.Hud_TextStyleLibrary != null)
                {
                    if (!HudManager.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                    {
                        DrawLabeledToggle("实时匹配库", "", sp_StyleLibSynching, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) =>
                        {
                            sp_StyleLibSynching.boolValue = b;
                            sp_StyleLibSynching.serializedObject.ApplyModifiedProperties();

                            if (sp_StyleLibSynching.boolValue)
                            {
                                sp_TextColorOwner.enumValueIndex = (int)TextColorOwner.StyleLibrary;
                            }
                        });
                    }
                    else
                    {
                        XGUI.layout_label(
                            text: "字体样式库为空",
                            size: XGUIFontSize.S,
                            anchor: TextAnchor.MiddleCenter,
                            text_color: Color.white * 0.85f,
                            offset: new Vector2(0, 0),
                            padding: new RectOffset(0, 0, 0, 0),
                            margin: new RectOffset(0, 0, 6, 0),
                            clipping: TextClipping.Clip);
                    }
                }
                else
                {
                    sp_StyleLibSynching.boolValue = false;
                    sp_StyleLibSynching.serializedObject.ApplyModifiedProperties();

                    XGUI.layout_label(
                          text: "您未在Hud管理器中配置或指定字体样式库",
                          size: XGUIFontSize.S,
                          anchor: TextAnchor.MiddleCenter,
                          text_color: Color.white * 0.85f,
                          offset: new Vector2(0, 0),
                          padding: new RectOffset(0, 0, 0, 0),
                          margin: new RectOffset(0, 0, 6, 0),
                          clipping: TextClipping.Clip);
                }
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 文字样式库
            XGUI.layout_group_start(
                 type: XGUIContainerType.Vertical,
                 bg_fill: XGUIGroup.缺口纯色边框,
                 bg_color: XGUIColor.亮白,
                 bg_color_gui: XHud_Dashboard.Theme_Group,
                 title: "文字样式库",
                 title_size: XGUIFontSize.M,
                 title_text_color: XHud_Dashboard.Theme_Primary,
                 title_clipping: TextClipping.Clip,
                 padding: new RectOffset(10, 10, 15, 15),
                 foldout: BaseScript.fold_option);

            #region 文字样式库
            if (HudManager != null && HudManager.Hud_TextStyleLibrary != null)
            {
                if (HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetCount(xHud_TextType.Text) > 0)
                {
                    XGUI.layout_group_start(
                        type: XGUIContainerType.Horizontal,
                        bg_fill: XGUIGroup.透明,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Group,
                        absolute_margin: true,
                        absolute_padding: true,
                        padding: new RectOffset(5, 5, 5, 15));

                    string[] str_fotlib_ItemsName = HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetAllNames_With_Text();

                    sp_StyleName.stringValue = XGUI.layout_string_popup(
                        //title: "文字样式库",
                        //title_width: 0,
                        //title_size: XGUIFontSize.M,
                        //title_anchor: TextAnchor.MiddleLeft,
                        prop: sp_StyleName,
                        options: str_fotlib_ItemsName,
                        opt_text_size: XGUIFontSize.M,
                        opt_text_color: Color.black,
                        opt_text_padding: new RectOffset(10, 10, 0, 0),
                        opt_anchor: TextAnchor.MiddleLeft,
                        opt_font_style: FontStyle.Normal,
                        opt_bg_fill: XGUIGroup.实体,
                        opt_bg_color: XGUIColor.亮白,
                        opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                        margin: new RectOffset(0, 0, 5, 5),
                        padding: new RectOffset(0, 0, 0, 0),
                        title_margin: new RectOffset(0, 0, 0, 0),
                        icon_arrow_color: Color.black,
                        act_on_changed: (s) =>
                        {
                            sp_StyleName.stringValue = s;
                            sp_StyleName.serializedObject.ApplyModifiedProperties();

                            if (Targets_Selected())
                            {
                                for (int i = 0; i < SelectedObjects.Length; i++)
                                {
                                    if (Application.isPlaying)
                                    {
                                        SelectedObjects[i].TextStyleInfo.CopyData_Ignored_LibraryToggle(HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(sp_StyleName.stringValue, xHud_TextType.Text));
                                    }
                                    else
                                    {
                                        SelectedObjects[i].TextStyleInfo.CopyData_Ignored_LibraryToggle(HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(sp_StyleName.stringValue, xHud_TextType.Text));
                                    }
                                }
                            }
                            else
                            {
                                if (Application.isPlaying)
                                {
                                    BaseScript.TextStyleInfo.CopyData_Ignored_LibraryToggle(HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(s, xHud_TextType.Text));
                                }
                                else
                                {
                                    BaseScript.TextStyleInfo.CopyData_Ignored_LibraryToggle(HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(s, xHud_TextType.Text));
                                }
                            }
                        });

                    if (!Targets_Selected())
                    {
                        XGUI.layout_space(10);

                        if (XGUI.layout_button(
                            tooltip: "定位到样式库",
                            tex_release: locate_r,
                            tex_press: locate_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: 14,
                            height: 14))
                        {
                            //获取目标元素的索引号
                            int index = 0;
                            for (int k = 0; k < str_fotlib_ItemsName.Length; k++)
                            {
                                if (str_fotlib_ItemsName[k] == sp_StyleName.stringValue)
                                {
                                    index = k;
                                    break;
                                }
                            }
                            //定位到元素库中的对应当前元素
                            //打开目标元素库
                            Editor_XHud_MenuItemsAction_OpenLibrary.open_textstyle();
                            HudManager.Hud_TextStyleLibrary.TextStyleLibrary_Location(str_fotlib_ItemsName[index]);
                        }
                    }
                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                }
                else
                {
                    XGUI.layout_label(
                         text: "字体样式库为空",
                         size: XGUIFontSize.S,
                         anchor: TextAnchor.MiddleCenter,
                         text_color: Color.white * 0.85f,
                         offset: new Vector2(0, 0),
                         padding: new RectOffset(0, 0, 0, 0),
                         margin: new RectOffset(0, 0, 6, 0),
                         clipping: TextClipping.Clip);
                }
            }
            else
            {
                XGUI.layout_label(
                          text: "您未在Hud管理器中配置或指定字体样式库",
                          size: XGUIFontSize.S,
                          anchor: TextAnchor.MiddleCenter,
                          text_color: Color.white * 0.85f,
                          offset: new Vector2(0, 0),
                          padding: new RectOffset(0, 0, 0, 0),
                          margin: new RectOffset(0, 0, 6, 0),
                          clipping: TextClipping.Clip);
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 库同步项
            if (sp_StyleLibSynching.boolValue)
            {
                BaseScript.fold_sync = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "库同步项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_sync);

                if (!BaseScript.fold_sync)
                {
                    #region 射线检测
                    DrawLabeledToggle("射线检测", "", sp_LibStyle_Effect_raycast, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 遮罩
                    DrawLabeledToggle("遮罩", "", sp_LibStyle_Effect_maskable, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 富文本
                    DrawLabeledToggle("富文本", "", sp_LibStyle_Effect_rich, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 尺寸
                    DrawLabeledToggle("尺寸", "", sp_LibStyle_Effect_size, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 自适应尺寸
                    DrawLabeledToggle("自适应尺寸", "", sp_LibStyle_Effect_bestfit, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    if (sp_LibStyle_Effect_bestfit.boolValue)
                    {
                        sp_txt_bestfit.boolValue = true;
                        sp_txt_bestfit.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    #region 对齐
                    DrawLabeledToggle("对齐", "", sp_LibStyle_Effect_align, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 几何对齐
                    DrawLabeledToggle("几何对齐", "", sp_LibStyle_Effect_geometre_align, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 行高
                    DrawLabeledToggle("行高", "", sp_LibStyle_Effect_line, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 字体
                    DrawLabeledToggle("字体", "", sp_LibStyle_Effect_font, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 样式
                    DrawLabeledToggle("样式", "", sp_LibStyle_Effect_style, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 颜色
                    DrawLabeledToggle("颜色", "", sp_LibStyle_Effect_color, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 水平溢出
                    DrawLabeledToggle("水平溢出", "", sp_LibStyle_Effect_overflow_h, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 垂直溢出
                    DrawLabeledToggle("垂直溢出", "", sp_LibStyle_Effect_overflow_v, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion
                }
                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 文字内容
            XGUI.layout_group_start(
            type: XGUIContainerType.Vertical,
            bg_fill: XGUIGroup.缺口纯色边框,
            bg_color: XGUIColor.亮白,
            bg_color_gui: XHud_Dashboard.Theme_Group,
            title: "文字内容",
            title_size: XGUIFontSize.M,
            title_text_color: XHud_Dashboard.Theme_Primary,
            title_clipping: TextClipping.Clip,
            padding: new RectOffset(10, 10, 10, 10));

            XGUI.ChangedCheck_Start();

            #region 文字内容
            sp_Text.stringValue = XGUI.layout_inputfield(
                prop: sp_Text.stringValue,
                text_wrap: true,
                field_fontsize: XGUIFontSize.M,
                field_text_offset: Vector2.zero,
                field_height: 80,
                field_padding: new RectOffset(0, 0, 5, 5),
                field_margin: new RectOffset(0, 0, 5, 5),
                field_text_color: Color.white,
                //field_text_font: XGUI.GetFont("xg-medium"),
                field_text_style: FontStyle.Normal,
                field_text_anchor: TextAnchor.UpperLeft);

            sp_Text.serializedObject.ApplyModifiedProperties();
            #endregion

            if (XGUI.ChangedCheck_End())
            {
                if (Targets_Selected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        SelectedObjects[i].text = sp_Text.stringValue;
                    }
                }
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 参数
            BaseScript.fold_param = XGUI.layout_group_start(
            type: XGUIContainerType.Vertical,
            bg_fill: XGUIGroup.缺口纯色边框,
            bg_color: XGUIColor.亮白,
            bg_color_gui: XHud_Dashboard.Theme_Group,
            title: "参数",
            title_size: XGUIFontSize.M,
            title_text_color: XHud_Dashboard.Theme_Primary,
            title_clipping: TextClipping.Clip,
            padding: new RectOffset(10, 10, 15, 15),
            foldout: BaseScript.fold_param);

            if (!BaseScript.fold_param)
            {
                XGUI.ChangedCheck_Start();

                #region 标识
                XGUI.layout_property_field(
                    title: "标识",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_Indicator,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 字体
                XGUI.SetEnabled(!sp_LibStyle_Effect_font.boolValue || !sp_StyleLibSynching.boolValue);
                XGUI.layout_property_field(
                    title: "字体",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_txt_font,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 锚点
                XGUI.SetEnabled(!sp_LibStyle_Effect_align.boolValue || !sp_StyleLibSynching.boolValue);
                XGUI.layout_property_field(
                    title: "锚点",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_txt_content_anchor,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 水平溢出
                XGUI.SetEnabled(!sp_LibStyle_Effect_overflow_h.boolValue || !sp_StyleLibSynching.boolValue);
                XGUI.layout_property_field(
                    title: "水平溢出",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_txt_overflow_h,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 垂直溢出
                XGUI.SetEnabled(!sp_LibStyle_Effect_overflow_v.boolValue || !sp_StyleLibSynching.boolValue);
                XGUI.layout_property_field(
                    title: "垂直溢出",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_txt_overflow_v,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 尺寸
                XGUI.SetEnabled(!sp_LibStyle_Effect_size.boolValue || !sp_StyleLibSynching.boolValue);
                XGUI.layout_property_field(
                    title: "尺寸",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_txt_size,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 自适应尺寸
                XGUI.SetEnabled(sp_txt_bestfit.boolValue);
                XGUI.layout_property_field(
                    title: "最小尺寸",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_txt_fit_min,
                    prop_margin: new RectOffset(5, 5, 0, 5));

                XGUI.layout_property_field(
                  title: "最大尺寸",
                  title_size: XGUIFontSize.M,
                  title_hover_color: XHud_Dashboard.Theme_Primary,
                  title_width: 90,
                  prop: sp_txt_fit_max,
                  prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 样式
                XGUI.SetEnabled(!sp_LibStyle_Effect_style.boolValue || !sp_StyleLibSynching.boolValue);
                XGUI.layout_property_field(
                    title: "样式",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_txt_style,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 行高
                XGUI.SetEnabled(!sp_LibStyle_Effect_line.boolValue || !sp_StyleLibSynching.boolValue);
                XGUI.layout_property_field(
                    title: "行高",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_txt_line_height,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 颜色
                XGUI.SetEnabled(!sp_LibStyle_Effect_color.boolValue || !sp_StyleLibSynching.boolValue);
                XGUI.layout_property_field(
                    title: "颜色",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_txt_font_color,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 颜色接管者
                XGUI.layout_property_field(
                    title: "颜色接管者",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_TextColorOwner,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                if (XGUI.ChangedCheck_End())
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SelectedObjects[i].font = (Font)sp_txt_font.objectReferenceValue;
                            ContentAnchor anchor = (ContentAnchor)sp_txt_content_anchor.enumValueIndex;
                            SelectedObjects[i].TextStyleInfo.txt_Set_Alignment(anchor);
                            HorizontalWrapMode hor = (HorizontalWrapMode)sp_txt_overflow_h.enumValueIndex;
                            SelectedObjects[i].TextStyleInfo.txt_Set_Overflow(hor);
                            VerticalWrapMode ver = (VerticalWrapMode)sp_txt_overflow_v.enumValueIndex;
                            SelectedObjects[i].TextStyleInfo.txt_Set_Overflow(ver);
                        }
                    }
                    else
                    {
                        ContentAnchor anchor = (ContentAnchor)sp_txt_content_anchor.enumValueIndex;
                        BaseScript.TextStyleInfo.txt_Set_Alignment(anchor);
                        HorizontalWrapMode hor = (HorizontalWrapMode)sp_txt_overflow_h.enumValueIndex;
                        BaseScript.TextStyleInfo.txt_Set_Overflow(hor);
                        VerticalWrapMode ver = (VerticalWrapMode)sp_txt_overflow_v.enumValueIndex;
                        BaseScript.TextStyleInfo.txt_Set_Overflow(ver);
                    }
                }
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 右键菜单
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("组件样式操作"));
                menu.AddItem(new GUIContent("C (复制样式)"), false, () =>
                {
                    XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
                    info.CopyData_Ignored_LibraryToggle(BaseScript.TextStyleInfo);

                    string json = JsonUtility.ToJson(info);

                    XGUI.x_Editor_Data_Set_With_String("xData_XHud_TextStyle", json);

                    XGUI.dialog(
                        type: XGUIDialogType.确认,
                        windowtitle: "XHud - 文字组件消息",
                        title: "文字样式复制",
                        msg: $"已将当前文字样式存入 xData ！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.Theme_Primary);
                });
                menu.AddItem(new GUIContent("V (粘贴样式)"), false, () =>
                {
                    XHud_LibraryArg_TextStyle info = JsonUtility.FromJson<XHud_LibraryArg_TextStyle>(XGUI.x_Editor_Data_Get_With_String("xData_XHud_TextStyle"));

                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            // 使用 Undo.RecordObject 来记录对目标对象的修改
                            Undo.RecordObject(SelectedObjects[i], "Paste Style Name");
                            SelectedObjects[i].TextStyleInfo.CopyData(info);
                        }

                        XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 文字组件消息",
                            title: "文字样式粘贴",
                            msg: $"已从 xData 中获取文字样式并应用到当前所选的文字组件！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                    }
                    else
                    {
                        // 使用 Undo.RecordObject 来记录对目标对象的修改
                        Undo.RecordObject(BaseScript, "Paste Style Name");
                        BaseScript.TextStyleInfo.CopyData(info);

                        XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 文字组件消息",
                            title: "文字样式粘贴",
                            msg: $"已从 xData 中获取文字样式并应用到当前文字组件！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                    }
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("文字样式库"));
                menu.AddItem(new GUIContent("X (识别)"), false, () =>
                {
                    XHud_LibraryArg_TextStyle info = JsonUtility.FromJson<XHud_LibraryArg_TextStyle>(XGUI.x_Editor_Data_Get_With_String("xData_XHud_TextStyle"));

                    if (Targets_Selected())
                    {
                        List<XGUIDialogListDatas> datas = new List<XGUIDialogListDatas>();

                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            XGUIDialogListDatas data = new XGUIDialogListDatas();

                            if (info.Type != xHud_TextType.Text)
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.帮助,
                                    windowtitle: "XHud - 文字组件消息",
                                    title: "文字样式识别",
                                    msg: $"从 XData 中识别的文字样式似乎并不是用于 {SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} ) 的文字组件的规范样式！请检查您复制的样式类型是否和当前组件一致？",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

                                data.Title = info.Name;
                                data.SubTitle = "文字样式类型未能匹配到";
                                data.Message = $"{SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} )";

                                datas.Add(data);
                                continue;
                            }
                            else
                            {
                                // 使用 Undo.RecordObject 来记录对目标对象的修改
                                Undo.RecordObject(SelectedObjects[i], "Paste Style Name");

                                SelectedObjects[i].TextStyleInfo.CopyData_Ignored_LibraryToggle(info);

                                data.Title = info.Name;
                                data.SubTitle = "文字样式已应用到";
                                data.Message = $"{SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} )";
                                datas.Add(data);
                            }
                        }

                        XGUI.dialog_listview(
                            datas: datas.ToArray(),
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 文字组件消息",
                            title: "文字样式识别",
                            msg: $"已从 xData 中识别到文字样式并应用到如下文字组件中！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                    }
                    else
                    {
                        if (info.Type != xHud_TextType.Text)
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.帮助,
                                windowtitle: "XHud - 文字组件消息",
                                title: "文字样式识别",
                                msg: $"从 xData 中识别的文字样式似乎并不是用于当前文字组件的规范样式！请检查您复制的样式类型是否和当前组件一致？",
                                ok: "明白",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.Theme_Primary);
                            return;
                        }

                        // 使用 Undo.RecordObject 来记录对目标对象的修改
                        Undo.RecordObject(BaseScript, "Paste Style Name");

                        BaseScript.TextStyleInfo.CopyData_Ignored_LibraryToggle(info);

                        XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 文字组件消息",
                            title: "文字样式识别",
                            msg: $"已从 xData 中识别到文字样式并应用到当前文字组件！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                    }
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Q (折叠编组)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SelectedObjects[i].GroupFold(true);
                        }
                    }
                    else
                    {
                        BaseScript.GroupFold(true);
                    }
                });
                menu.AddItem(new GUIContent("W (展开编组)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SelectedObjects[i].GroupFold(false);
                        }
                    }
                    else
                    {
                        BaseScript.GroupFold(false);
                    }
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("同步开关"));
                menu.AddItem(new GUIContent("S (全部同步)"), false, () =>
                {
                    sp_LibStyle_Effect_align.boolValue = true;
                    sp_LibStyle_Effect_font.boolValue = true;
                    sp_LibStyle_Effect_style.boolValue = true;
                    sp_LibStyle_Effect_size.boolValue = true;
                    sp_LibStyle_Effect_color.boolValue = true;
                    sp_LibStyle_Effect_rich.boolValue = true;
                    sp_LibStyle_Effect_line.boolValue = true;
                    sp_LibStyle_Effect_overflow_h.boolValue = true;
                    sp_LibStyle_Effect_overflow_v.boolValue = true;
                    sp_LibStyle_Effect_bestfit.boolValue = true;
                    sp_LibStyle_Effect_raycast.boolValue = true;
                    sp_LibStyle_Effect_maskable.boolValue = true;

                    sp_LibStyle_Effect_align.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_font.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_style.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_size.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_color.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_rich.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_line.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_overflow_h.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_overflow_v.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_bestfit.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_raycast.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_maskable.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("D (全部关闭)"), false, () =>
                {
                    sp_LibStyle_Effect_align.boolValue = false;
                    sp_LibStyle_Effect_font.boolValue = false;
                    sp_LibStyle_Effect_style.boolValue = false;
                    sp_LibStyle_Effect_size.boolValue = false;
                    sp_LibStyle_Effect_color.boolValue = false;
                    sp_LibStyle_Effect_rich.boolValue = false;
                    sp_LibStyle_Effect_line.boolValue = false;
                    sp_LibStyle_Effect_overflow_h.boolValue = false;
                    sp_LibStyle_Effect_overflow_v.boolValue = false;
                    sp_LibStyle_Effect_bestfit.boolValue = false;
                    sp_LibStyle_Effect_raycast.boolValue = false;
                    sp_LibStyle_Effect_maskable.boolValue = false;

                    sp_LibStyle_Effect_align.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_font.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_style.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_size.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_color.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_rich.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_line.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_overflow_h.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_overflow_v.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_bestfit.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_raycast.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_maskable.serializedObject.ApplyModifiedProperties();
                });
                // 显示右键菜单
                menu.ShowAsContext();

                e.Use(); // 标记事件已被处理，防止其他操作处理该事件
            }
            #endregion

            #region 源脚本
            BaseScript.fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
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

        #region 工具 - 刷新文字样式
        /// <summary>
        /// 根据文字样式名称索引刷新文字样式。
        /// <para/>
        /// 单选时刷新当前组件；多选时逐个刷新所有选中组件。
        /// 若指定名称在样式库中不存在，则弹窗询问是否使用首个样式。
        /// </summary>
        private void UpdateTextStyle_WithTarget()
        {
            XHud_LibraryArg_TextStyle s_info = null;
            if (Targets_Selected())
            {
                var s = targets;
                foreach (var t in s)
                {
                    XHud_Module_Text tt = (XHud_Module_Text)t;

                    if (HudManager.Hud_TextStyleLibrary.TextStyle_Library_NameIsValid(tt.StyleName))
                    {
                        s_info = HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(tt.StyleName, xHud_TextType.Text);
                    }
                    else
                    {
                        string res = XGUI.dialog(
                             type: XGUIDialogType.警告,
                             windowtitle: "XHud - 文字组件消息",
                             title: "刷新文字库样式",
                             msg: $"似乎 {tt.name} ( {tt.Indicator} ) 文字组件的样式库名称并不在文字样式库中存在，请检查文字样式库名称是否合法，或者可以快速选择文字样式库默认首个样式。是否要选择首个样式？",
                             ok: "暂不",
                             cancel: "选择",
                             PrimaryIndex: 0,
                             usemodal: true,
                             themecolor: XHud_Dashboard.Theme_Primary);

                        if (res == "选择")
                        {
                            s_info = HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(xHud_TextType.Text);

                            // 使用 Undo.RecordObject 来记录对目标对象的修改
                            Undo.RecordObject(tt, "Change TextStyle Name");

                            tt.StyleName = s_info.Name;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    tt.TextStyleInfo.CopyData_Ignored_LibraryToggle(s_info);
                }
            }
            else
            {
                if (HudManager.Hud_TextStyleLibrary.TextStyle_Library_NameIsValid(sp_StyleName.stringValue))
                    s_info = HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(sp_StyleName.stringValue, xHud_TextType.Text);
                else
                {
                    string res = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 文字组件消息",
                        title: "刷新文字库样式",
                        msg: $"似乎您指定的样式库名称并不在文字样式库中存在，请检查文字样式库名称是否合法，或者可以快速选择文字样式库默认首个样式。是否要选择首个样式？",
                        ok: "暂不",
                        cancel: "选择",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.Theme_Primary);

                    if (res == "选择")
                    {
                        s_info = HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(xHud_TextType.Text);

                        // 使用 Undo.RecordObject 来记录对目标对象的修改
                        Undo.RecordObject(sp_StyleName.serializedObject.targetObject, "Change TextStyle Name");

                        sp_StyleName.stringValue = s_info.Name;
                        sp_StyleName.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    }
                    else
                    {
                        return;
                    }
                }

                BaseScript.TextStyleInfo.CopyData_Ignored_LibraryToggle(s_info);
                return;
            }
        }
        #endregion

        #region 工具 - 打开字体样式添加器
        /// <summary>
        /// 打开字体样式采集器窗口。
        /// <para/>
        /// 将当前组件的样式信息与目标样式库传入采集器，用于保存为新的样式模版。
        /// </summary>
        /// <param name="info">文字样式数据</param>
        public void Open_Hud_Library_TextStyle_Setter(XHud_LibraryArg_TextStyle info)
        {
            Editor_XHud_LibrarySetTool_TextStyle window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_TextStyle>(true);

            window.titleContent = new GUIContent("XHud - 文字样式采集器");
            XGUI.CenterEditorWindow(new Vector2Int(348, info.Type == xHud_TextType.Text ? 820 : 800), window);

            window.SetComponent(BaseScript);
            window.SetStyle(info);
            window.SetOriginStyle(info);
            window.Set_Target_Hud_TextStyleLibrary(HudManager.Hud_TextStyleLibrary);
            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetButtonText("添加", "取消");
            window.SetTitle("文字样式采集器");
            //window.ShowModal();
            window.Show();
        }
        #endregion

        #region 绘制控件
        /// <summary>
        /// 通用方法：绘制带标题的开关控件。
        /// <para/>
        /// 封装 <see cref="XGUI.layout_toggle"/>，统一标题宽度、字体、混合选项样式。
        /// </summary>
        private void DrawLabeledToggle(string title = null, string tooltip = null, SerializedProperty prop = null, float width = 0, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, string[] options = null, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                tooltip: tooltip,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(5, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: style,
                tog_padding: new RectOffset(0, 9, 0, 0),
                tog_margin: new RectOffset(0, 0, 0, 5),
                tog_mixed_options: options,
                tog_mixed_text_size: XGUIFontSize.M,
                tog_mixed_text_color: Color.black,
                tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                tog_mixed_font_style: FontStyle.Normal,
                tog_bg_off_color: color_bg_off,
                tog_bg_on_color: color_bg_on,
                tog_handler_off_color: color_off,
                tog_handler_on_color: color_on,
                tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
                act_on_changed: act_on_changed);
        }
        #endregion
    }
}