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
    using UnityEditor;
    using UnityEngine;

    public class Editor_XHud_LibrarySetTool_ElementLib_CountsSet : EditorWindow
    {
        private SerializedObject BaseObject;
        private SerializedProperty sp_InitializeCounts;
        [SerializeField]
        private int InitializeCounts;
        [SerializeField]
        private XHud_Library_Element TargetElementLibrary;

        private Texture2D icon_logo;

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);

            sp_InitializeCounts = BaseObject.FindProperty("InitializeCounts");

            icon_logo = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_element_setter/icon");
        }

        private void OnGUI()
        {
            BaseObject.Update();

            #region 抬头
            Rect rect = new Rect(0, 0, position.width, position.height);

            // 图标
            Rect rect_icon = new Rect(15, 15, icon_logo.width, icon_logo.height);
            XGUI.gui_icon(
                rect: rect_icon,
                icon: icon_logo,
                padding: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                color: Color.white);

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            // 大标题
            Rect rect_title = new Rect(rect.x + 70, rect.y + 10, rect.width - 80, 30);
            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent("预加载数量修改"),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping,
                font: XGUI.GetFont("xg-heavy"));

            // 分割线
            Rect rect_seperate = new Rect(rect.x + 68, rect.y + 43, 200, 1);
            XGUI.gui_seperator(
                rect: rect_seperate,
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            // 小标题
            Rect rect_subtitle = new Rect(rect.x + 18, rect.y + 45, rect.width, 30);
            XGUI.gui_label(
                rect: rect_subtitle,
                text: new GUIContent("此面板可以修改目标元素库的统一预生成数量！"),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                clipping: clipping);
            #endregion

            Event ce = Event.current;
            if (ce.type == EventType.MouseDown)
            {
                // 取消当前拥有键盘焦点的控件
                GUI.FocusControl(null);
                Repaint();
            }

            #region 目标元素库名称
            XGUI.layout_label(
                text: TargetElementLibrary.LibraryName,
                bg_fill: XGUIFilled.无,
                bg_color: XGUIColor.无,
                size: XGUIFontSize.M,
                anchor: TextAnchor.MiddleCenter,
                text_color: XHud_Dashboard.Theme_Primary,
                padding: new RectOffset(10, 10, 0, 0),
                margin: new RectOffset(0, 0, 95, 0),
                clipping: TextClipping.Clip,
                font: XGUI.GetFont("xg-bold"),
                font_style: FontStyle.Bold);
            #endregion

            XGUI.layout_space(25);

            #region 预加载数量
            XGUI.layout_property_field(
                title: "预加载数量",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 100,
                //status_icon: "icon_field_status",
                //status_icon_color: CameraCutter_Near.floatValue != 0, XHud_Dashboard.Theme_Primary : Color.red,
                prop_padding: new RectOffset(20, 20, 0, 0),
                prop: sp_InitializeCounts);
            #endregion

            XGUI.layout_space(10);

            Buttons();

            Event e = Event.current;
            // 检测点击事件
            if (e.type == EventType.MouseDown)
            {
                // 检查点击位置是否在窗口内
                if (!new Rect(rect.width - 240, rect.y + 190, 215, 20).Contains(e.mousePosition))
                {
                    GUI.FocusControl(null); // 取消所有控件的焦点
                }
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }

            if (BaseObject.targetObject != null)
                BaseObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 控件按钮
        /// </summary>
        private void Buttons()
        {
            XGUI.layout_group_start(
              type: XGUIContainerType.Horizontal,
              margin: new RectOffset(5, 5, 0, 0),
              padding: new RectOffset(10, 10, 15, 15));

            if (XGUI.layout_button(
                text: "关闭",
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white,
                button_text_color: Color.black,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                height: XGUI.GetSingleLineHeight() + 10,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                Close();
            }

            XGUI.layout_space(10);

            if (Application.isPlaying)
                XGUI.SetEnabled(false);
            else
                XGUI.SetEnabled(true);

            if (XGUI.layout_button(
                text: "更新",
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Primary,
                button_text_color: Color.black,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                height: XGUI.GetSingleLineHeight() + 10,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                ModifyTargetLibPreloadCounts(TargetElementLibrary);
            }
            GUI.backgroundColor = Color.white;
            XGUI.SetEnabled(true);

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
        }

        /// <summary>
        /// 记录目标元素库名称
        /// </summary>
        /// <param name="name"></param>
        public void SetTargetLib(XHud_Library_Element lib)
        {
            TargetElementLibrary = lib;
        }

        private void ModifyTargetLibPreloadCounts(XHud_Library_Element lib)
        {
            #region 更新元素库目标元素参数
            // 关键：记录目标 ScriptableObject 的状态
            var targetLibrary = lib;
            Undo.RecordObject(targetLibrary, "Modify Preload Counts");

            // 修改预加载数量
            for (int s = 0; s < targetLibrary.ElementLibrary.Count; s++)
            {
                targetLibrary.ElementLibrary[s].InitializeCount = sp_InitializeCounts.intValue;
            }

            // 标记为已修改
            EditorUtility.SetDirty(targetLibrary);
            #endregion

            Close();
            return;
        }
    }
}
