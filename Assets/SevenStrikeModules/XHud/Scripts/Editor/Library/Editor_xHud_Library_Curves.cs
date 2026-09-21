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
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [System.Serializable]
    public class CurveFrame
    {
        public float time;
        public float value;
        public float inTangent;
        public float inWeight;
        public float outTangent;
        public float outWeight;
        public WeightedMode weightedMode;
    }

    [System.Serializable]
    public class CurveNode
    {
        public string Name;
        public CurveFrame[] Frames;
    }

    [System.Serializable]
    public class CurveLib
    {
        public List<CurveNode> Nodes;
    }

    [CustomEditor(typeof(XHud_Library_Curves))]
    public class Editor_XHud_Library_Curves : Editor
    {
        #region 组件 / 列表
        private XHud_Library_Curves BaseScript;
        /// <summary>
        /// 曲线列表
        /// </summary>
        private ReorderableList CurveInfoList;
        #endregion

        #region 序列化属性
        private SerializedProperty
            sp_SelectedIndex,
            sp_CurveLibrary,
            sp_LibraryName,
            sp_LocationSelectedIndex,
            sp_Highlight,
            sp_SoundInfoList_Original_Scroller,
            sp_itemHeight,
            sp_visibleItemCount,
            sp_Find,
            sp_UsePreviewAutoStop;
        #endregion

        #region 图标
        private Texture2D import_p, import_r, export_p, export_r, clear_p, clear_r, create_p, create_r, delete_p, delete_r, play_p, play_r, leftarr_p, leftarr_r, rightarr_p, rightarr_r, stop_p, stop_r;
        #endregion     

        /// <summary>
        /// 屏蔽颜色
        /// </summary>
        private Color blocked_col;
        /// <summary>
        /// 屏蔽颜色透明度
        /// </summary>
        private float blocked_alp = 0.45f;
        /// <summary>
        /// 预览标题文字
        /// </summary>
        public string PreviewHeader = "CurvePreview";

        private void OnEnable()
        {
            BaseScript = (XHud_Library_Curves)target;

            #region 获取序列化属性
            sp_CurveLibrary = serializedObject.FindProperty("CurveLibrary");
            sp_LocationSelectedIndex = serializedObject.FindProperty("LocationSelectedIndex");
            sp_UsePreviewAutoStop = serializedObject.FindProperty("UsePreviewAutoStop");
            sp_LibraryName = serializedObject.FindProperty("LibraryName");
            sp_Highlight = serializedObject.FindProperty("Highlight");
            sp_SelectedIndex = serializedObject.FindProperty("SelectedIndex");
            sp_SoundInfoList_Original_Scroller = serializedObject.FindProperty("SoundInfoList_Original_Scroller");
            sp_itemHeight = serializedObject.FindProperty("itemHeight");
            sp_visibleItemCount = serializedObject.FindProperty("visibleItemCount");
            sp_Find = serializedObject.FindProperty("Find");
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
            sp_itemHeight.floatValue = 30;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 15;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            blocked_col = new Color(0, 0, 0, blocked_alp);

            #region ReorderableList - CurveInfoList
            CurveInfoList = new ReorderableList(serializedObject, sp_CurveLibrary, true, true, true, true);
            CurveInfoList.drawElementCallback = CurveInfoList_Original_DrawElementCallback;
            #endregion
        }

        private void OnDisable()
        {
            sp_SoundInfoList_Original_Scroller.vector2Value = Vector2.zero;
            sp_SoundInfoList_Original_Scroller.serializedObject.ApplyModifiedProperties();

            sp_SelectedIndex.intValue = -1;
            sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

            sp_LocationSelectedIndex.intValue = -1;
            sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();

            sp_Find.stringValue = string.Empty;
            sp_Find.serializedObject.ApplyModifiedProperties();

            sp_Highlight.stringValue = string.Empty;
            sp_Highlight.serializedObject.ApplyModifiedProperties();
        }

        #region CurveInfoList_Original      

        Rect drawelement_rect;
        Rect scrollview_rect;
        Rect item_rect;
        Color SelectedBg = new Color(0, 0, 0, 0.2f);

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
        /// 绘制元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void CurveInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_CurveLibrary.GetArrayElementAtIndex(index);

            SerializedProperty sp_name = prop.FindPropertyRelative("Name");
            SerializedProperty sp_curve = prop.FindPropertyRelative("Curve");

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
            drawelement_rect.Set(rect.x + 42, rect.y + 6, rect.width - (sp_LocationSelectedIndex.intValue == -1 ? 200 : 235), 20);
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
                if (BaseScript.act_on_CurveChanged != null)
                    BaseScript.act_on_CurveChanged();
            }
            #endregion

            GUI.enabled = true;

            if (!sp_name.stringValue.Contains(sp_Highlight.stringValue))
            {
                drawelement_rect.Set(rect.x - 20, rect.y, rect.width + 20, rect.height);
                EditorGUI.DrawRect(drawelement_rect, blocked_col);
            }

            if (index == sp_LocationSelectedIndex.intValue)
            {
                drawelement_rect.Set(rect.width - 180, rect.y + 3, 50, 20);
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
        /// 绘制列表
        /// </summary>
        private void DrawCurveInfoList_Original()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visibleItemCount.intValue * sp_itemHeight.floatValue);
            sp_SoundInfoList_Original_Scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_SoundInfoList_Original_Scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, CurveInfoList.count * sp_itemHeight.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_SoundInfoList_Original_Scroller.vector2Value.y / sp_itemHeight.floatValue);
            int endIndex = Mathf.CeilToInt((sp_SoundInfoList_Original_Scroller.vector2Value.y + scrollview_rect.height) / sp_itemHeight.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < CurveInfoList.count; i++)
            {
                SerializedProperty prop = sp_CurveLibrary.GetArrayElementAtIndex(i);
                SerializedProperty sp_name = prop.FindPropertyRelative("Name");

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                // 如果当前元素被选中，绘制选中效果
                if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                {
                    if (sp_SelectedIndex.intValue == i)
                    {
                        // 高亮标记表示选中
                        item_rect.Set(1, i * sp_itemHeight.floatValue + 12, 5, 5);
                        EditorGUI.DrawRect(item_rect, XHud_Dashboard.Theme_Primary);
                        // 高亮背景表示选中
                        item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width + 20, sp_itemHeight.floatValue);
                        EditorGUI.DrawRect(item_rect, SelectedBg);
                    }
                }

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                CurveInfoList.drawElementCallback.Invoke(item_rect, i, i == CurveInfoList.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                        {
                            // 更新选中项
                            sp_SelectedIndex.intValue = i;
                            sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

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
        /// 列表移除
        /// </summary>
        /// <param name="list"></param>
        private void CurveInfoList_Original_Remove(ReorderableList list)
        {
            if (list.count <= 0)
                return;
            sp_CurveLibrary.DeleteArrayElementAtIndex(list.index);
            if (BaseScript.act_on_CurveRemoved != null)
                BaseScript.act_on_CurveRemoved();
        }

        /// <summary>
        /// 列表增加
        /// </summary>
        /// <param name="list"></param>
        private void CurveInfoList_Original_Add(ReorderableList list)
        {
            if (list.count <= 0)
            {
                if (BaseScript.CurveLibrary == null)
                    BaseScript.CurveLibrary = new List<xHud_LibraryArg_Curve>();
                BaseScript.CurveLibrary.Add(new XHud.xHud_LibraryArg_Curve("曲线" + BaseScript.CurveLibrary.Count, AnimationCurve.EaseInOut(0, 0, 1, 1)));
            }
            else
            {
                sp_CurveLibrary.InsertArrayElementAtIndex(list.index);
                SerializedProperty prop = sp_CurveLibrary.GetArrayElementAtIndex(list.index);
                SerializedProperty sp_name = prop.FindPropertyRelative("Name");
                sp_name.stringValue = $"新曲线_{list.index}";


                sp_CurveLibrary.serializedObject.ApplyModifiedProperties();
            }

            if (BaseScript.act_on_CurveAdded != null)
                BaseScript.act_on_CurveAdded(BaseScript.CurveLibrary[BaseScript.CurveLibrary.Count - 1].Name, BaseScript.CurveLibrary[BaseScript.CurveLibrary.Count - 1].Curve);
        }
        #endregion

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                title_text: "XHud  -  曲线库",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping,
                title_offset: new Vector2(-10, 0),
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 参数区
            XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
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
                    BaseScript.CurveLibrary_Location_Find(sp_Find.stringValue);
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
                       bg_fill: XGUIFilled.缺口纯色边框,
                       bg_color: XGUIColor.亮白,
                       bg_color_gui: XHud_Dashboard.Theme_Group,
                       title: "快捷功能",
                       title_size: XGUIFontSize.M,
                       title_text_color: XHud_Dashboard.Theme_Primary,
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
                            themecolor: XHud_Dashboard.Theme_Primary);
                        if (res == "暂不")
                            return;

                        sp_CurveLibrary.ClearArray();
                        sp_CurveLibrary.serializedObject.ApplyModifiedProperties();
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
                        CurveInfoList_Original_Add(CurveInfoList);
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
                        CurveInfoList_Original_Remove(CurveInfoList);
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                }
            }
            #endregion

            #region 曲线列表
            XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.Theme_Group,
               title: "曲线列表",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.Theme_Primary,
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(10, 10, 15, 15));

            DrawCurveInfoList_Original();

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 曲线库导出
        /// </summary>
        private void Curves_Export()
        {
            CurveLib lib = new CurveLib();
            lib.Nodes = new List<CurveNode>();

            for (int i = 0; i < BaseScript.CurveLibrary.Count; i++)
            {
                xHud_LibraryArg_Curve info = BaseScript.CurveLibrary[i];

                Keyframe[] frames = info.Curve.keys;

                CurveNode Node = new CurveNode();
                Node.Name = info.Name;
                Node.Frames = new CurveFrame[frames.Length];
                for (int s = 0; s < frames.Length; s++)
                {
                    CurveFrame cs = new CurveFrame();
                    cs.time = frames[s].time;
                    cs.value = frames[s].value;
                    cs.inTangent = frames[s].inTangent;
                    cs.inWeight = frames[s].inWeight;
                    cs.outTangent = frames[s].outTangent;
                    cs.outWeight = frames[s].outWeight;
                    cs.weightedMode = frames[s].weightedMode;
                    Node.Frames[s] = cs;
                }
                lib.Nodes.Add(Node);
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
        /// 曲线库导入
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
                themecolor: XHud_Dashboard.Theme_Primary);
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

                        int size = sp_CurveLibrary.arraySize;

                        for (int i = 0; i < lib.Nodes.Count; i++)
                        {
                            string name = lib.Nodes[i].Name;

                            Keyframe[] frames = new Keyframe[lib.Nodes[i].Frames.Length];

                            for (int s = 0; s < frames.Length; s++)
                            {
                                frames[s] = new Keyframe(
                                    lib.Nodes[i].Frames[s].time,
                                    lib.Nodes[i].Frames[s].value,
                                    lib.Nodes[i].Frames[s].inTangent,
                                    lib.Nodes[i].Frames[s].outTangent,
                                    lib.Nodes[i].Frames[s].inWeight,
                                    lib.Nodes[i].Frames[s].outWeight);
                            }

                            AnimationCurve curve = new AnimationCurve(frames);

                            xHud_LibraryArg_Curve info = new xHud_LibraryArg_Curve(name, curve);

                            sp_CurveLibrary.InsertArrayElementAtIndex(i + size);
                            SerializedProperty sp_cur = sp_CurveLibrary.GetArrayElementAtIndex(i + size);
                            SerializedProperty sp_name = sp_cur.FindPropertyRelative("Name");
                            SerializedProperty sp_curve = sp_cur.FindPropertyRelative("Curve");

                            sp_name.stringValue = info.Name;
                            sp_curve.animationCurveValue = info.Curve;

                            sp_name.serializedObject.ApplyModifiedProperties();
                            sp_curve.serializedObject.ApplyModifiedProperties();
                            sp_cur.serializedObject.ApplyModifiedProperties();
                        }
                        sp_CurveLibrary.serializedObject.ApplyModifiedProperties();

                        if (BaseScript.act_on_CurveChanged != null)
                            BaseScript.act_on_CurveChanged();
                    }
                    break;
                case "替换":
                    path = EditorUtility.OpenFilePanel("读取曲线库数据文件", Application.dataPath, "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        string content = File.ReadAllText(path);
                        CurveLib lib = JsonUtility.FromJson<CurveLib>(content);

                        sp_CurveLibrary.ClearArray();

                        for (int i = 0; i < lib.Nodes.Count; i++)
                        {
                            string name = lib.Nodes[i].Name;

                            Keyframe[] frames = new Keyframe[lib.Nodes[i].Frames.Length];

                            for (int s = 0; s < frames.Length; s++)
                            {
                                frames[s] = new Keyframe(
                                    lib.Nodes[i].Frames[s].time,
                                    lib.Nodes[i].Frames[s].value,
                                    lib.Nodes[i].Frames[s].inTangent,
                                    lib.Nodes[i].Frames[s].outTangent,
                                    lib.Nodes[i].Frames[s].inWeight,
                                    lib.Nodes[i].Frames[s].outWeight);
                            }

                            AnimationCurve curve = new AnimationCurve(frames);

                            xHud_LibraryArg_Curve info = new xHud_LibraryArg_Curve(name, curve);

                            sp_CurveLibrary.InsertArrayElementAtIndex(i);
                            SerializedProperty sp_cur = sp_CurveLibrary.GetArrayElementAtIndex(i);
                            SerializedProperty sp_name = sp_cur.FindPropertyRelative("Name");
                            SerializedProperty sp_curve = sp_cur.FindPropertyRelative("Curve");

                            sp_name.stringValue = info.Name;
                            sp_curve.animationCurveValue = info.Curve;

                            sp_name.serializedObject.ApplyModifiedProperties();
                            sp_curve.serializedObject.ApplyModifiedProperties();
                            sp_cur.serializedObject.ApplyModifiedProperties();
                        }
                        sp_CurveLibrary.serializedObject.ApplyModifiedProperties();

                        if (BaseScript.act_on_CurveChanged != null)
                            BaseScript.act_on_CurveChanged();
                    }
                    break;
            }
        }
        #endregion
    }
}