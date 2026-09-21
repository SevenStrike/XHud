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
    using SevenStrikeModules.XTween;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Path = System.IO.Path;

    [System.Serializable]
    public class ExportMotions
    {
        public List<XHud.XHud_LibraryArg_Motion> ElementMotionList;
    }

    [CustomEditor(typeof(XHud_Library_Motion))]
    public class Editor_XHud_Library_Motion : Editor
    {
        #region 组件 / 列表
        /// <summary>
        /// 基础脚本
        /// </summary>
        private XHud_Library_Motion BaseScript;
        /// <summary>
        /// 动效参数列表
        /// </summary>
        private ReorderableList ElementMotionList;
        #endregion

        #region 序列化属性
        /// <summary>
        /// 序列化 - 动效参数列表
        /// </summary>
        private SerializedProperty sp_ElementMotionList, sp_SelectedIndex, sp_LocationSelectedIndex, sp_ElementMotionList_Scroller, sp_LibraryName, sp_itemHeight, sp_visibleItemCount, sp_Find, sp_Highlight;
        #endregion

        /// <summary>
        /// 屏蔽颜色
        /// </summary>
        private Color blocked_col;
        /// <summary>
        /// 屏蔽颜色透明度
        /// </summary>
        private float blocked_alp = 0.45f;

        #region 图标
        private Texture2D btn_icon_details_released, btn_icon_details_press, Icon_eleparam_type_move, Icon_eleparam_type_rot, import_p, import_r, export_p, export_r, clear_p, clear_r, create_p, create_r, delete_p, delete_r;
        #endregion

        private void OnEnable()
        {
            BaseScript = (XHud_Library_Motion)target;

            #region 获取序列化属性
            sp_LibraryName = serializedObject.FindProperty("LibraryName");
            sp_ElementMotionList = serializedObject.FindProperty("ElementMotionList");
            sp_SelectedIndex = serializedObject.FindProperty("SelectedIndex");
            sp_LocationSelectedIndex = serializedObject.FindProperty("LocationSelectedIndex");
            sp_ElementMotionList_Scroller = serializedObject.FindProperty("ElementMotionList_Scroller");
            sp_itemHeight = serializedObject.FindProperty("itemHeight");
            sp_visibleItemCount = serializedObject.FindProperty("visibleItemCount");
            sp_Find = serializedObject.FindProperty("Find");
            sp_Highlight = serializedObject.FindProperty("Highlight");
            #endregion

            #region 获取图标
            btn_icon_details_released = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/detail_r");
            btn_icon_details_press = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/detail_p");
            Icon_eleparam_type_move = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/mover");
            Icon_eleparam_type_rot = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/rotator");
            import_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/import_p");
            import_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/import_r");
            export_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/export_p");
            export_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/export_r");
            clear_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/clear_p");
            clear_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/clear_r");
            create_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/create_p");
            create_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/create_r");
            delete_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/delete_p");
            delete_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion/delete_r");
            #endregion

            //设定列表项高度值
            sp_itemHeight.floatValue = 80;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 6;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            blocked_col = new Color(0, 0, 0, blocked_alp);

            #region ReorderableList - ElementMotionList
            ElementMotionList = new ReorderableList(serializedObject, sp_ElementMotionList, true, true, true, true);
            ElementMotionList.drawElementCallback = MotionList_DrawElementCallback;
            #endregion
        }

        #region ElementMotionList_Original      

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
        private void MotionList_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_ElementMotionList.GetArrayElementAtIndex(index);
            SerializedProperty sp_name = prop.FindPropertyRelative("Name");
            SerializedProperty sp_descript = prop.FindPropertyRelative("Des");
            SerializedProperty sp_mode = prop.FindPropertyRelative("Mode");
            SerializedProperty sp_crc = prop.FindPropertyRelative("Crc");
            SerializedProperty sp_rec = prop.FindPropertyRelative("Rec");

            SerializedProperty movement_type_crc = sp_crc.FindPropertyRelative("Movement.Movement");
            SerializedProperty sp_anchor_crc = sp_crc.FindPropertyRelative("anchor");
            SerializedProperty rotation_type_crc = sp_crc.FindPropertyRelative("Rotation.Rotation");

            SerializedProperty movement_type_rec = sp_rec.FindPropertyRelative("Movement.Movement");
            SerializedProperty rotation_type_rec = sp_rec.FindPropertyRelative("Rotation.Rotation");

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

            #region 运动形式            
            #region 位置
            drawelement_rect.Set(rect.x + 40, rect.y + 55, 15, 15);
            XGUI.gui_icon(
                rect: drawelement_rect,
                icon: Icon_eleparam_type_move,
                color: movement_type_crc.enumValueIndex == 0 ? new Color(0.4f, 0.4f, 0.4f, 0.9f) : Color.white);
            #endregion

            #region 旋转
            drawelement_rect.Set(rect.x + 70, rect.y + 55, 18, 18);
            XGUI.gui_icon(
                rect: drawelement_rect,
                icon: Icon_eleparam_type_rot,
                color: rotation_type_crc.enumValueIndex == 0 ? new Color(0.4f, 0.4f, 0.4f, 0.9f) : Color.white);
            #endregion
            #endregion

            string h_color = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
            string t_color = XGUI_Utilitys.Color_To_HexString(XGUI.GetXGUIColor(XGUIColor.警示黄), true);
            string m_color = XGUI_Utilitys.Color_To_HexString(XGUI.GetXGUIColor(XGUIColor.工业蓝), true);

            #region 样式名称
            drawelement_rect.Set(rect.x + 40, rect.y + 5, rect.width - (string.IsNullOrEmpty(sp_Find.stringValue) ? 95 : 155), 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent($"<b><color={(sp_mode.intValue == 0 ? t_color : m_color)}>{(sp_mode.intValue == 0 ? "C " : "R")}</color></b>    <color=#ffffff>|</color>    <color={h_color}>{sp_name.stringValue}</color>"),
                text_color: Color.white,
            size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion

            #region 动效说明
            drawelement_rect.Set(rect.x + 39, rect.y + 30, rect.width - 140, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent($"{sp_descript.stringValue}"),
                text_color: Color.white * 0.75f,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion

            #region 参数简要
            if (sp_mode.intValue == 0)
            {
                string motion_name = "";
                string rot_name = "";
                string text = "";

                HudMotion_Movement movement_crc_value = (HudMotion_Movement)movement_type_crc.enumValueIndex;
                HudMotion_Rotation rotation_crc_value = (HudMotion_Rotation)rotation_type_crc.enumValueIndex;
                if (movement_crc_value != HudMotion_Movement.A_无运动)
                {
                    motion_name = movement_crc_value.ToString().Split(new char[1] { '_' })[1];
                }
                else
                {
                    motion_name = "无运动";
                }

                if (rotation_crc_value != HudMotion_Rotation.A_无旋转)
                {
                    string[] rot_names = rotation_crc_value.ToString().Split(new char[1] { '_' });
                    rot_name = rot_names.Length >= 2 ? rot_names[1] + "_" + rot_names[2] : rot_names[1];
                }
                else
                {
                    rot_name = "无旋转";
                }

                text = $"{motion_name}  /  {rot_name}";
                drawelement_rect.Set(rect.x + 100, rect.y + 52, rect.width - 180, 20);

                XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(text),
                text_color: XGUI.GetXGUIColor(XGUIColor.警示黄),
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);

                #region 锚点
                XHudAnchor crc_anchor = (XHudAnchor)sp_anchor_crc.enumValueIndex;
                string anchoricon = "";
                switch (crc_anchor)
                {
                    case XHudAnchor.底层:
                        anchoricon = "B_Bold";
                        break;
                    case XHudAnchor.上:
                        anchoricon = "U_Bold";
                        break;
                    case XHudAnchor.下:
                        anchoricon = "D_Bold";
                        break;
                    case XHudAnchor.左:
                        anchoricon = "L_Bold";
                        break;
                    case XHudAnchor.右:
                        anchoricon = "R_Bold";
                        break;
                    case XHudAnchor.中心:
                        anchoricon = "C_Bold";
                        break;
                    case XHudAnchor.左上:
                        anchoricon = "L_U_Bold";
                        break;
                    case XHudAnchor.左下:
                        anchoricon = "L_D_Bold";
                        break;
                    case XHudAnchor.右上:
                        anchoricon = "R_U_Bold";
                        break;
                    case XHudAnchor.右下:
                        anchoricon = "R_D_Bold";
                        break;
                    case XHudAnchor.顶层:
                        anchoricon = "T_Bold";
                        break;
                    default:
                        break;
                }

                drawelement_rect.Set(rect.width - 48, rect.y + 50, 18, 18);

                XGUI.gui_icon(
                    rect: drawelement_rect,
                    icon: XGUI.GetBasedIcon($"Anchor/{anchoricon}"),
                    color: Color.gray * 0.9f);
                #endregion
            }
            else if (sp_mode.intValue == 1)
            {
                string motion_name = "";
                string rot_name = "";
                string text = "";

                HudMotion_Movement movement_rec_value = (HudMotion_Movement)movement_type_rec.enumValueIndex;
                HudMotion_Rotation rotation_rec_value = (HudMotion_Rotation)rotation_type_rec.enumValueIndex;
                if (movement_rec_value != HudMotion_Movement.A_无运动)
                {
                    motion_name = movement_rec_value.ToString().Split(new char[1] { '_' })[1];
                }
                else
                {
                    motion_name = "无运动";
                }

                if (rotation_rec_value != HudMotion_Rotation.A_无旋转)
                {
                    string[] rot_names = rotation_rec_value.ToString().Split(new char[1] { '_' });
                    rot_name = rot_names.Length >= 2 ? rot_names[1] + "_" + rot_names[2] : rot_names[1];
                }
                else
                {
                    rot_name = "无旋转";
                }

                text = $"{motion_name}  /  {rot_name}";
                drawelement_rect.Set(rect.x + 100, rect.y + 52, rect.width - 180, 20);

                XGUI.gui_label(
                   rect: drawelement_rect,
                   text: new GUIContent(text),
                   text_color: XGUI.GetXGUIColor(XGUIColor.工业蓝),
                   size: XGUIFontSize.M,
                   clipping: clipping,
                   anchor: TextAnchor.MiddleLeft,
                   offset: new Vector2(0, 0),
                   font_style: FontStyle.Normal);
            }
            #endregion

            BlockGUI(sp_name.stringValue);

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
                XHud.XHud_LibraryArg_Motion motion = new XHud.XHud_LibraryArg_Motion(BaseScript.ElementMotionList[index]);
                OpenParameterSetter(motion, (HudElementMotionType)motion.Mode, sp_name.stringValue, index);
            }
            #endregion

            BlockGUI(sp_name.stringValue);

            // 首个项不加分割线
            if (index != 0)
                XGUI.gui_seperator(
                    rect: new Rect(rect.x + 30, rect.y + 1, rect.width - 70, 1),
                    thickness: 1,
                    color: Color.gray * 0.75f);

            #region 辅助菜单
            drawelement_rect.Set(rect.x, rect.y, rect.width - 60, rect.height);
            // 检测鼠标点击事件
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1 && drawelement_rect.Contains(e.mousePosition))
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("D (获取代码块)"), false, () =>
                {
                    string str_codeblock = "";
                    string Mode = "";

                    if (sp_mode.intValue == 0)
                    {
                        Mode = "生成动效参数代码块";
                        Motion_Creator hc = MotionParam_Convert_Crc(sp_crc);
                        #region 代码块
                        str_codeblock = $"Motion_Creator cre = new Motion_Creator();\ncre.anchor = XHudAnchor. {hc.anchor.ToString()} ;\n cre.Movement = new MotionNode_Movement();\ncre.Movement.Movement = HudMotion_Movement. {hc.Movement.Movement.ToString()} ;\ncre.Movement.Distance = {hc.Movement.Distance}f;\ncre.Movement.Duration = {hc.Movement.Duration}f;\ncre.Movement.Delay = {hc.Movement.Delay}f;\ncre.Movement.CurveName =\"\";\ncre.Movement.Curve = XHud_Dashboard.HudManagerGet().Hud_Curves.CurveLibrary_GetCurve(cre.Movement.CurveName);\ncre.Movement.Ease = EaseMode. {hc.Movement.Ease.ToString()} ;\n cre.Rotation = new MotionNode_Rotation();\ncre.Rotation.Rotation = HudMotion_Rotation. {hc.Rotation.Rotation.ToString()} ;\ncre.Rotation.Degree = {hc.Rotation.Degree}f;\ncre.Rotation.Duration = {hc.Rotation.Duration}f;\ncre.Rotation.Delay = {hc.Rotation.Delay}f;\ncre.Rotation.CurveName = \"\";\ncre.Rotation.Curve = XHud_Dashboard.HudManagerGet().Hud_Curves.CurveLibrary_GetCurve(cre.Rotation.CurveName);\ncre.Rotation.Ease = EaseMode. {hc.Rotation.Ease.ToString()} ;\n cre.Alpha = new MotionNode_Alpha();\ncre.Alpha.Duration = {hc.Alpha.Duration}f;\ncre.Alpha.Delay = {hc.Alpha.Delay}f;\ncre.Alpha.CurveName =\"\";\ncre.Alpha.Curve = XHud_Dashboard.HudManagerGet().Hud_Curves.CurveLibrary_GetCurve(cre.Alpha.CurveName);\ncre.Alpha.Ease = EaseMode. {hc.Alpha.Ease} ;";
                        #endregion
                    }
                    if (sp_mode.intValue == 1)
                    {
                        Mode = "回收动效参数代码块";
                        Motion_Recycler hr = MotionParam_Convert_Rec(sp_rec);
                        #region 代码块
                        str_codeblock = $"Motion_Recycler rec = new Motion_Recycler();\n rec.Movement = new MotionNode_Movement();\n rec.Movement.Movement = HudMotion_Movement. {hr.Movement.Movement.ToString()} ;\n rec.Movement.Distance = {hr.Movement.Distance}f;\n rec.Movement.Duration = {hr.Movement.Duration}f;\n rec.Movement.Delay = {hr.Movement.Delay}f;\n rec.Movement.CurveName = \"\";\n rec.Movement.Curve = XHud_Dashboard.HudManagerGet().Hud_Curves.CurveLibrary_GetCurve(rec.Movement.CurveName);\n rec.Movement.Ease = EaseMode. {hr.Movement.Ease.ToString()} ;\n rec.Rotation = new MotionNode_Rotation();\n rec.Rotation.Rotation = HudMotion_Rotation. {hr.Rotation.Rotation.ToString()} ;\n rec.Rotation.Degree = {hr.Rotation.Degree}f;\n rec.Rotation.Duration = {hr.Rotation.Duration}f;\n rec.Rotation.Delay = {hr.Rotation.Delay}f;\n rec.Rotation.CurveName = \"\";\n rec.Rotation.Curve = XHud_Dashboard.HudManagerGet().Hud_Curves.CurveLibrary_GetCurve(rec.Rotation.CurveName);\n rec.Rotation.Ease = EaseMode. {hr.Rotation.Ease.ToString()} ;\n rec.Alpha = new MotionNode_Alpha();\n rec.Alpha.Duration = {hr.Alpha.Duration}f;\n rec.Alpha.Delay = {hr.Alpha.Delay}f;\n rec.Alpha.CurveName = \"\";\n rec.Alpha.Curve = XHud_Dashboard.HudManagerGet().Hud_Curves.CurveLibrary_GetCurve(rec.Alpha.CurveName);\n rec.Alpha.Ease = EaseMode. {hr.Alpha.Ease} ;";
                        #endregion                       
                    }
                    GUIUtility.systemCopyBuffer = str_codeblock;

                    XGUI.dialog(
                            type: XGUIDialogType.通知,
                            windowtitle: "XHud - 元素动效库消息",
                            title: "拷贝动效代码块",
                            msg: $"已拷贝元素代码块 {Mode} 到系统剪贴板 ！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                });
                menu.AddItem(new GUIContent("E (修改动效)"), false, () =>
                {
                    XHud.XHud_LibraryArg_Motion motion = new XHud.XHud_LibraryArg_Motion(BaseScript.ElementMotionList[index]);
                    OpenParameterSetter(motion, (HudElementMotionType)motion.Mode, sp_name.stringValue, index);
                    return;
                });
                menu.AddItem(new GUIContent("S (复制动效)"), false, () =>
                {
                    string str_jsons = "";
                    string Mode = "";

                    if (sp_mode.intValue == 0)
                    {
                        Mode = "生成动效";

                        str_jsons = JsonUtility.ToJson(MotionParam_Convert_Crc(sp_crc));
                    }
                    if (sp_mode.intValue == 1)
                    {
                        Mode = "回收动效";
                        str_jsons = JsonUtility.ToJson(MotionParam_Convert_Rec(sp_rec));
                    }

                    XGUI.x_Editor_Data_Set_With_String("xData_MotionArgs", str_jsons);

                    XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 元素动效库消息",
                            title: "拷贝动效代码块",
                            msg: $"已拷贝 {Mode} - {sp_name.stringValue} 到 xData ！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                });
                menu.AddItem(new GUIContent("R (粘贴动效)"), false, () =>
                {
                    int orimode = sp_mode.intValue;
                    string buffer = XGUI.x_Editor_Data_Get_With_String("xData_MotionArgs");
                    if (buffer.Contains("anchor"))//粘贴生成参数
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 元素动效库消息",
                            title: "粘贴生成动效",
                            msg: $"是否确定要将生成动效粘贴到该动效项？",
                            ok: "粘贴",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);

                        if (res == "暂不")
                        {
                            return;
                        }

                        sp_mode.intValue = 0;
                        Motion_Creator crc = JsonUtility.FromJson<Motion_Creator>(buffer);

                        sp_crc.FindPropertyRelative("anchor").enumValueIndex = (int)crc.anchor;

                        sp_crc.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)crc.Movement.Movement;
                        sp_crc.FindPropertyRelative("Movement.Distance").floatValue = crc.Movement.Distance;
                        sp_crc.FindPropertyRelative("Movement.Duration").floatValue = crc.Movement.Duration;
                        sp_crc.FindPropertyRelative("Movement.Delay").floatValue = crc.Movement.Delay;
                        sp_crc.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)crc.Movement.Ease;
                        sp_crc.FindPropertyRelative("Movement.Curve").animationCurveValue = crc.Movement.Curve;
                        sp_crc.FindPropertyRelative("Movement.CurveName").stringValue = crc.Movement.CurveName;

                        sp_crc.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)crc.Rotation.Rotation;
                        sp_crc.FindPropertyRelative("Rotation.Degree").floatValue = crc.Rotation.Degree;
                        sp_crc.FindPropertyRelative("Rotation.Duration").floatValue = crc.Rotation.Duration;
                        sp_crc.FindPropertyRelative("Rotation.Delay").floatValue = crc.Rotation.Delay;
                        sp_crc.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)crc.Rotation.Ease;
                        sp_crc.FindPropertyRelative("Rotation.Curve").animationCurveValue = crc.Rotation.Curve;
                        sp_crc.FindPropertyRelative("Rotation.CurveName").stringValue = crc.Rotation.CurveName;

                        sp_crc.FindPropertyRelative("Alpha.Duration").floatValue = crc.Alpha.Duration;
                        sp_crc.FindPropertyRelative("Alpha.Curve").animationCurveValue = crc.Alpha.Curve;
                        sp_crc.FindPropertyRelative("Alpha.CurveName").stringValue = crc.Alpha.CurveName;
                        sp_crc.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)crc.Alpha.Ease;
                        sp_crc.FindPropertyRelative("Alpha.Delay").floatValue = crc.Alpha.Delay;

                        sp_crc.serializedObject.ApplyModifiedProperties();

                        string tip = "";

                        if (orimode == 0)
                            tip = $"已更新 {sp_name.stringValue} 的 \"生成\" 动效参数!";
                        else
                            tip = $"已将原始为 \"回收\" 参数的 {sp_name.stringValue} 动效类型更新为粘贴的 \"生成\" 动效参数!";

                        XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 元素动效库消息",
                            title: "粘贴动效",
                            msg: tip,
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                    }
                    else//粘贴回收参数
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.帮助,
                            windowtitle: "XHud - 元素动效库消息",
                            title: "粘贴回收动效",
                            msg: $"是否确定要将回收动效粘贴到该动效项？",
                            ok: "粘贴",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                        if (res == "暂不")
                        {
                            return;
                        }
                        sp_mode.intValue = 1;
                        Motion_Recycler rec = JsonUtility.FromJson<Motion_Recycler>(buffer);

                        sp_rec.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)rec.Movement.Movement;
                        sp_rec.FindPropertyRelative("Movement.Distance").floatValue = rec.Movement.Distance;
                        sp_rec.FindPropertyRelative("Movement.Duration").floatValue = rec.Movement.Duration;
                        sp_rec.FindPropertyRelative("Movement.Delay").floatValue = rec.Movement.Delay;
                        sp_rec.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)rec.Movement.Ease;
                        sp_rec.FindPropertyRelative("Movement.Curve").animationCurveValue = rec.Movement.Curve;
                        sp_rec.FindPropertyRelative("Movement.CurveName").stringValue = rec.Movement.CurveName;

                        sp_rec.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)rec.Rotation.Rotation;
                        sp_rec.FindPropertyRelative("Rotation.Degree").floatValue = rec.Rotation.Degree;
                        sp_rec.FindPropertyRelative("Rotation.Duration").floatValue = rec.Rotation.Duration;
                        sp_rec.FindPropertyRelative("Rotation.Delay").floatValue = rec.Rotation.Delay;
                        sp_rec.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)rec.Rotation.Ease;
                        sp_rec.FindPropertyRelative("Rotation.Curve").animationCurveValue = rec.Rotation.Curve;
                        sp_rec.FindPropertyRelative("Rotation.CurveName").stringValue = rec.Rotation.CurveName;

                        sp_rec.FindPropertyRelative("Alpha.Duration").floatValue = rec.Alpha.Duration;
                        sp_rec.FindPropertyRelative("Alpha.Curve").animationCurveValue = rec.Alpha.Curve;
                        sp_rec.FindPropertyRelative("Alpha.CurveName").stringValue = rec.Alpha.CurveName;
                        sp_rec.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)rec.Alpha.Ease;
                        sp_rec.FindPropertyRelative("Alpha.Delay").floatValue = rec.Alpha.Delay;

                        sp_rec.serializedObject.ApplyModifiedProperties();

                        string tip = "";

                        if (orimode == 1)
                            tip = $"已更新 {sp_name.stringValue} 的 \"回收\" 动效参数!";
                        else
                            tip = $"已将原始为 \"生成\" 参数的 {sp_name.stringValue} 动效类型更新为粘贴的 \"回收\" 动效参数!";

                        XGUI.dialog(
                           type: XGUIDialogType.确认,
                           windowtitle: "XHud - 元素动效库消息",
                           title: "粘贴回收动效",
                           msg: tip,
                           ok: "明白",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.Theme_Primary);
                    }
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("X (导出动效)"), false, () =>
                {
                    string json = "";
                    string Mode = "";
                    string path = "";

                    switch (sp_mode.intValue)
                    {
                        case 0:
                            Mode = "元素生成动效参数";
                            Motion_Creator hc = MotionParam_Convert_Crc(sp_crc);
                            ExportMotions eem_crc = new ExportMotions();
                            eem_crc.ElementMotionList = new List<XHud.XHud_LibraryArg_Motion>();
                            XHud.XHud_LibraryArg_Motion motion_crc = new XHud.XHud_LibraryArg_Motion();
                            motion_crc.Name = sp_name.stringValue;
                            motion_crc.Mode = sp_mode.intValue;
                            motion_crc.Des = sp_descript.stringValue;
                            motion_crc.Crc = hc;
                            eem_crc.ElementMotionList.Add(motion_crc);

                            json = JsonUtility.ToJson(eem_crc);
                            path = EditorUtility.SaveFilePanel("请选择导出动效参数Json文件的路径", Application.dataPath, $"ElementMotion_CRC_{sp_LibraryName.stringValue}_{sp_name.stringValue}", "json");

                            if (!string.IsNullOrEmpty(path))
                            {
                                string res = XGUI.dialog(
                                    type: XGUIDialogType.帮助,
                                    windowtitle: "XHud - 元素动效库消息",
                                    title: "导出生成动效",
                                    msg: $"是否确定要将回收动效参数导出到该目录下？",
                                    ok: "导出",
                                    cancel: "暂不",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

                                if (res == "导出")
                                {
                                    File.WriteAllText(path, json);

                                    XGUI.dialog(
                                        type: XGUIDialogType.确认,
                                        windowtitle: "XHud - 元素动效库消息",
                                        title: "导出生成动效",
                                        msg: $"已导出 {Mode} - {sp_name.stringValue} 到Json文件！",
                                        ok: "明白",
                                        PrimaryIndex: 0,
                                        usemodal: true,
                                        themecolor: XHud_Dashboard.Theme_Primary);
                                }
                            }
                            break;
                        case 1:
                            Mode = "元素回收动效参数";
                            Motion_Recycler hr = MotionParam_Convert_Rec(sp_rec);
                            ExportMotions eem_rec = new ExportMotions();
                            eem_rec.ElementMotionList = new List<XHud.XHud_LibraryArg_Motion>();
                            XHud.XHud_LibraryArg_Motion motion_rec = new XHud.XHud_LibraryArg_Motion();
                            motion_rec.Name = sp_name.stringValue;
                            motion_rec.Mode = sp_mode.intValue;
                            motion_rec.Des = sp_descript.stringValue;
                            motion_rec.Rec = hr;
                            eem_rec.ElementMotionList.Add(motion_rec);

                            json = JsonUtility.ToJson(eem_rec);
                            path = EditorUtility.SaveFilePanel("请选择导出动效参数Json文件的路径", Application.dataPath, $"ElementMotion_REC_{sp_LibraryName.stringValue}_{sp_name.stringValue}", "json");

                            if (!string.IsNullOrEmpty(path))
                            {
                                string res = XGUI.dialog(
                                        type: XGUIDialogType.确认,
                                        windowtitle: "XHud - 元素动效库消息",
                                        title: "导出回收动效",
                                        msg: $"是否确定要将回收动效参数导出到该目录下？",
                                        ok: "导出",
                                        cancel: "暂不",
                                        PrimaryIndex: 0,
                                        usemodal: true,
                                        themecolor: XHud_Dashboard.Theme_Primary);

                                if (res == "导出")
                                {
                                    File.WriteAllText(path, json);

                                    XGUI.dialog(
                                        type: XGUIDialogType.确认,
                                        windowtitle: "XHud - 元素动效库消息",
                                        title: "导出回收动效",
                                        msg: $"已导出 {Mode} - {sp_name.stringValue} 到Json文件？",
                                        ok: "明白",
                                        PrimaryIndex: 0,
                                        usemodal: true,
                                        themecolor: XHud_Dashboard.Theme_Primary);
                                }
                            }
                            break;
                    }
                });
                // 显示右键菜单
                menu.ShowAsContext();

                e.Use();
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
                drawelement_rect.Set(rect.width - 120, rect.y + 15, 50, 20);
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
        /// 创建便于导出参数的数据结构 - MotionCreate
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        private Motion_Creator MotionParam_Convert_Crc(SerializedProperty prop)
        {
            Motion_Creator hc = new Motion_Creator();
            hc.anchor = (XHudAnchor)prop.FindPropertyRelative("anchor").enumValueIndex;
            hc.Movement.Movement = (HudMotion_Movement)prop.FindPropertyRelative("Movement.Movement").enumValueIndex;
            hc.Movement.Distance = prop.FindPropertyRelative("Movement.Distance").floatValue;
            hc.Movement.Duration = prop.FindPropertyRelative("Movement.Duration").floatValue;
            hc.Movement.Delay = prop.FindPropertyRelative("Movement.Delay").floatValue;
            hc.Movement.Curve = prop.FindPropertyRelative("Movement.Curve").animationCurveValue;
            hc.Movement.CurveName = prop.FindPropertyRelative("Movement.CurveName").stringValue;
            hc.Movement.Ease = (EaseMode)prop.FindPropertyRelative("Movement.Ease").enumValueIndex;
            hc.Rotation.Rotation = (HudMotion_Rotation)prop.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
            hc.Rotation.Degree = prop.FindPropertyRelative("Rotation.Degree").floatValue;
            hc.Rotation.Duration = prop.FindPropertyRelative("Rotation.Duration").floatValue;
            hc.Rotation.Delay = prop.FindPropertyRelative("Rotation.Delay").floatValue;
            hc.Rotation.Curve = prop.FindPropertyRelative("Rotation.Curve").animationCurveValue;
            hc.Rotation.CurveName = prop.FindPropertyRelative("Rotation.CurveName").stringValue;
            hc.Rotation.Ease = (EaseMode)prop.FindPropertyRelative("Rotation.Ease").enumValueIndex;
            hc.Alpha.Duration = prop.FindPropertyRelative("Alpha.Duration").floatValue;
            hc.Alpha.Delay = prop.FindPropertyRelative("Alpha.Delay").floatValue;
            hc.Alpha.Curve = prop.FindPropertyRelative("Alpha.Curve").animationCurveValue;
            hc.Alpha.CurveName = prop.FindPropertyRelative("Alpha.CurveName").stringValue;
            hc.Alpha.Ease = (EaseMode)prop.FindPropertyRelative("Alpha.Ease").enumValueIndex;

            return hc;
        }

        /// <summary>
        /// 创建便于导出参数的数据结构 - MotionRecycle
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        private Motion_Recycler MotionParam_Convert_Rec(SerializedProperty prop)
        {
            Motion_Recycler hr = new Motion_Recycler();
            hr.Movement.Movement = (HudMotion_Movement)prop.FindPropertyRelative("Movement.Movement").enumValueIndex;
            hr.Movement.Distance = prop.FindPropertyRelative("Movement.Distance").floatValue;
            hr.Movement.Duration = prop.FindPropertyRelative("Movement.Duration").floatValue;
            hr.Movement.Delay = prop.FindPropertyRelative("Movement.Delay").floatValue;
            hr.Movement.Curve = prop.FindPropertyRelative("Movement.Curve").animationCurveValue;
            hr.Movement.CurveName = prop.FindPropertyRelative("Movement.CurveName").stringValue;
            hr.Movement.Ease = (EaseMode)prop.FindPropertyRelative("Movement.Ease").enumValueIndex;
            hr.Rotation.Rotation = (HudMotion_Rotation)prop.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
            hr.Rotation.Degree = prop.FindPropertyRelative("Rotation.Degree").floatValue;
            hr.Rotation.Duration = prop.FindPropertyRelative("Rotation.Duration").floatValue;
            hr.Rotation.Delay = prop.FindPropertyRelative("Rotation.Delay").floatValue;
            hr.Rotation.Curve = prop.FindPropertyRelative("Rotation.Curve").animationCurveValue;
            hr.Rotation.CurveName = prop.FindPropertyRelative("Rotation.CurveName").stringValue;
            hr.Rotation.Ease = (EaseMode)prop.FindPropertyRelative("Rotation.Ease").enumValueIndex;
            hr.Alpha.Duration = prop.FindPropertyRelative("Alpha.Duration").floatValue;
            hr.Alpha.Delay = prop.FindPropertyRelative("Alpha.Delay").floatValue;
            hr.Alpha.Curve = prop.FindPropertyRelative("Alpha.Curve").animationCurveValue;
            hr.Alpha.CurveName = prop.FindPropertyRelative("Alpha.CurveName").stringValue;
            hr.Alpha.Ease = (EaseMode)prop.FindPropertyRelative("Alpha.Ease").enumValueIndex;

            return hr;
        }

        /// <summary>
        /// 绘制列表
        /// </summary>
        private void DrawMotionList()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visibleItemCount.intValue * sp_itemHeight.floatValue);
            sp_ElementMotionList_Scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_ElementMotionList_Scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, ElementMotionList.count * sp_itemHeight.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_ElementMotionList_Scroller.vector2Value.y / sp_itemHeight.floatValue);
            int endIndex = Mathf.CeilToInt((sp_ElementMotionList_Scroller.vector2Value.y + scrollview_rect.height) / sp_itemHeight.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < ElementMotionList.count; i++)
            {
                SerializedProperty prop = sp_ElementMotionList.GetArrayElementAtIndex(i);
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
                ElementMotionList.drawElementCallback.Invoke(item_rect, i, i == ElementMotionList.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                        {
                            // 更新选中项
                            sp_SelectedIndex.intValue = i;

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
        private void MotionList_Remove(ReorderableList list)
        {
            if (list.count <= 0)
                return;

            sp_ElementMotionList.DeleteArrayElementAtIndex(list.index);
            sp_ElementMotionList.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 列表增加
        /// </summary>
        /// <param name="list"></param>
        private void MotionList_Add(ReorderableList list)
        {
            SerializedProperty prop = null;
            if (sp_ElementMotionList.arraySize <= 0)
            {
                sp_ElementMotionList.InsertArrayElementAtIndex(0);
                prop = sp_ElementMotionList.GetArrayElementAtIndex(0);
            }
            else
            {
                sp_ElementMotionList.InsertArrayElementAtIndex(list.index);
                prop = sp_ElementMotionList.GetArrayElementAtIndex(list.index);

            }
            prop.FindPropertyRelative("Name").stringValue = "NewMotion";
            prop.FindPropertyRelative("Mode").intValue = 0;
            prop.FindPropertyRelative("Des").stringValue = "...";

            prop.FindPropertyRelative("Crc").FindPropertyRelative("anchor").enumValueIndex = (int)XHudAnchor.中心;

            prop.FindPropertyRelative("Crc").FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.S_从下至上;
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Movement.Distance").floatValue = 100;
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Movement.Duration").floatValue = 1;
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Movement.Delay").floatValue = 0;
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Movement.CurveName").stringValue = "";
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.OutQuart;
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Rotation.Degree").floatValue = 0;
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Rotation.Duration").floatValue = 1;
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Rotation.Delay").floatValue = 0;
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Rotation.CurveName").stringValue = "";
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.OutQuart;
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Alpha.Duration").floatValue = 1;
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Alpha.Delay").floatValue = 0;
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Alpha.CurveName").stringValue = "";
            prop.FindPropertyRelative("Crc").FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.OutQuart;

            prop.FindPropertyRelative("Rec").FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.D_从上至下;
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Movement.Distance").floatValue = 100;
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Movement.Duration").floatValue = 1;
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Movement.Delay").floatValue = 0;
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Movement.CurveName").stringValue = "";
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.OutQuart;
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Rotation.Degree").floatValue = 0;
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Rotation.Duration").floatValue = 1;
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Rotation.Delay").floatValue = 0;
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Rotation.CurveName").stringValue = "";
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.OutQuart;
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Alpha.Duration").floatValue = 1;
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Alpha.Delay").floatValue = 0;
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Alpha.CurveName").stringValue = "";
            prop.FindPropertyRelative("Rec").FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.OutQuart;

            prop.serializedObject.ApplyModifiedProperties();
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
                title_text: "XHud  -  元素动效库",
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
                    BaseScript.ElementMotionLibrary_Location_Find(sp_Find.stringValue);
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

                    #region 导入元素动效参数模版
                    if (XGUI.layout_button(
                        tooltip: "导入元素动效参数模版",
                        tex_release: import_r,
                        tex_press: import_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        ImportMotions();
                        return;
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 导出元素动效参数模版
                    if (XGUI.layout_button(
                        tooltip: "导出元素动效参数模版",
                        tex_release: export_r,
                        tex_press: export_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        ExportMotions();
                        return;
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 移除所有元素动效参数模版
                    if (XGUI.layout_button(
                        tooltip: "移除所有元素动效参数模版",
                        tex_release: clear_r,
                        tex_press: clear_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素动效库消息",
                            title: "清空动效项",
                            msg: "此操作会清空所有动效模版，确认要这样做吗？",
                            ok: "暂不",
                            cancel: "清空",
                            PrimaryIndex: 1,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);

                        if (res == "暂不")
                        {
                            return;
                        }
                        sp_ElementMotionList.ClearArray();
                        sp_ElementMotionList.serializedObject.ApplyModifiedProperties();
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
                        MotionList_Add(ElementMotionList);
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
                        MotionList_Remove(ElementMotionList);
                        return;
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                }
            }
            #endregion

            #region 元素动效库列表
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "元素动效库列表",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15));

            DrawMotionList();

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        private void OnDisable()
        {
            if (target != null)
            {
                sp_ElementMotionList_Scroller.vector2Value = Vector2.zero;
                sp_ElementMotionList_Scroller.serializedObject.ApplyModifiedProperties();

                sp_LocationSelectedIndex.intValue = -1;
                sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();

                sp_SelectedIndex.intValue = -1;
                sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

                sp_Find.stringValue = string.Empty;
                sp_Find.serializedObject.ApplyModifiedProperties();

                sp_Highlight.stringValue = string.Empty;
                sp_Highlight.serializedObject.ApplyModifiedProperties();
            }
        }

        #region 辅助
        /// <summary>
        /// 导入动效参数
        /// </summary>
        private void ImportMotions()
        {
            string res = XGUI.dialog(
                          type: XGUIDialogType.警告,
                          windowtitle: "XHud - 元素动效库消息",
                          title: "读取动效数据",
                          msg: "根据您的需要选择导入动效数据的方式，如果是追加则会在当前动效库的基础上后续叠加导入的动效项，如果是替换则会完全替换当前动效库的所有动效项！",
                          ok: "追加",
                          cancel: "替换",
                          alt: "暂不",
                          PrimaryIndex: 1,
                          usemodal: true,
                          themecolor: XHud_Dashboard.Theme_Primary);

            if (res == "暂不")
            {
                return;
            }

            string path = EditorUtility.OpenFilePanel("请选择要导入的动效参数模版Json文件", Application.dataPath, "json");

            if (string.IsNullOrEmpty(path))
                return;

            string filename = Path.GetFileNameWithoutExtension(path);

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
                        ExportMotions info = JsonUtility.FromJson<ExportMotions>(json);
                        for (int i = 0; i < info.ElementMotionList.Count; i++)
                        {
                            BaseScript.ElementMotion_Add(info.ElementMotionList[i]);
                        }
                    }
                    break;
                case "替换":
                    if (!string.IsNullOrEmpty(path))
                    {
                        string json = File.ReadAllText(path);
                        BaseScript.ElementMotionList = JsonUtility.FromJson<ExportMotions>(json).ElementMotionList;
                    }
                    break;
            }
        }

        /// <summary>
        /// 导出动效参数
        /// </summary>
        private void ExportMotions()
        {
            string path_rec = EditorUtility.SaveFilePanel("请选择导出动效参数文件的路径", Application.dataPath, "ElementMotions_" + sp_LibraryName.stringValue, "json");

            if (string.IsNullOrEmpty(path_rec))
            {
                return;
            }

            string res = XGUI.dialog(
                type: XGUIDialogType.帮助,
                windowtitle: "XHud - 元素动效库消息",
                title: "导出动效模板",
                msg: "是否确定要将动效参数导出到该目录下？",
                ok: "暂不",
                cancel: "导出",
                PrimaryIndex: 1,
                usemodal: true,
                themecolor: XHud_Dashboard.Theme_Primary);

            if (res == "暂不")
                return;

            ExportMotions eem = new ExportMotions();
            eem.ElementMotionList = BaseScript.ElementMotionList;

            string json = JsonUtility.ToJson(eem);
            File.WriteAllText(path_rec, json);

            XGUI.dialog(
                type: XGUIDialogType.确认,
                windowtitle: "XHud - 元素动效库消息",
                title: "导出动效模板",
                msg: "已导出动效列表 ！",
                ok: "明白",
                PrimaryIndex: 0,
                usemodal: true,
                themecolor: XHud_Dashboard.Theme_Primary);
        }

        /// <summary>
        /// 动效参数修改器
        /// </summary>
        public void OpenParameterSetter(XHud.XHud_LibraryArg_Motion motion, HudElementMotionType type, string motionName, int index)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(false);

            window.titleContent = new GUIContent("XHud - 元素动效资源修改器");
            XGUI.CenterEditorWindow(new Vector2Int(348, type == HudElementMotionType.Creator ? 850 : 780), window);

            window.SetElementMotion(motion.Crc);
            window.SetElementMotion(motion.Rec);
            window.SetElementMotionType(type);
            window.SetLibrarySetterMode(LibrarySetterMode.修改库源参数);
            window.SetButtonText("更新", "取消");
            window.SetTitle("元素动效资源修改器");
            window.SetInfo(motion.Name, motion.Des);
            window.ModifiedIndex = index;
            window.SetTarget_Hud_MotionLibrary(BaseScript);
            //window.ShowModal();
            window.Show();
        }
        #endregion
    }
}