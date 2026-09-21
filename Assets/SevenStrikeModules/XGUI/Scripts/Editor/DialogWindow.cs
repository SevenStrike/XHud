/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGUI - Unity Editor界面可视化组件工具
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
namespace SevenStrikeModules.XGUI.Editor
{
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// 弹窗对话框参数
    /// </summary>
    public struct XGUIDialogInfo
    {
        /// <summary>
        /// 弹窗类型
        /// </summary>
        public XGUIDialogType type;
        /// <summary>
        /// 窗口标题
        /// </summary>
        public string windowtitle;
        /// <summary>
        /// 主标题
        /// </summary>
        public string title;
        /// <summary>
        /// 内容
        /// </summary>
        public string msg;
        /// <summary>
        /// 选项按钮
        /// </summary>
        public string[] options;
        /// <summary>
        /// 主要按钮索引
        /// </summary>
        public int PrimaryIndex;
    }

    [System.Serializable]
    public class XGUIDialogListDatas
    {
        [SerializeField]
        /// <summary>
        /// 列表项标题 位于列表项开头
        /// </summary>
        public string Title;
        [SerializeField]
        /// <summary>
        /// 列表项子标题 位于列表项中间
        /// </summary>
        public string SubTitle;
        [SerializeField]
        /// <summary>
        /// 列表项消息内容 位于列表项最后
        /// </summary>
        public string Message;
        [SerializeField]
        /// <summary>
        /// 目标源物体
        /// </summary>
        public UnityEngine.Object SourceObject;

        public XGUIDialogListDatas()
        {

        }

        /// <summary>
        /// 设置对话框数据列表项内容
        /// </summary>
        /// <param name="title"></param>
        /// <param name="subTitle"></param>
        /// <param name="message"></param>
        public XGUIDialogListDatas(string title, string subTitle, string message)
        {
            Title = title;
            SubTitle = subTitle;
            Message = message;
        }
    }

    public class DialogWindow : EditorWindow
    {
        private SerializedObject BaseObject;
        private SerializedProperty
            sp_DataList;

        public XGUIDialogButtonMode XGUIDialogButtonMode;
        public XGUIDialogType XGUIDialogMode;

        /// <summary>
        /// 数据列表
        /// </summary>
        public ReorderableList ReorderableList;
        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 28;
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 8;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 DataList_Scroller;
        /// <summary>
        /// 选中项索引号
        /// </summary>
        public int SelectedIndex;
        [SerializeField]
        public List<XGUIDialogListDatas> DataList = new List<XGUIDialogListDatas>();

        /// <summary>
        /// 窗口内容
        /// </summary>
        public string Title = "";
        /// <summary>
        /// 窗口内容
        /// </summary>
        public string Message = "";
        /// <summary>
        /// 主要按钮索引
        /// </summary>
        private int PrimaryIndex = 0;
        /// <summary>
        /// 按钮文本 - 确认
        /// </summary>
        public string Text_Ok = "";
        /// <summary>
        /// 按钮文本 - 取消
        /// </summary>
        public string Text_Cancel = "";
        /// <summary>
        /// 按钮文本 - 辅助
        /// </summary>
        public string Text_Alt = "";
        /// <summary>
        /// 按钮文本 - 其他
        /// </summary>
        public string Text_Other = "";
        /// <summary>
        /// 按钮文本 - 特别
        /// </summary>
        public string Text_Special = "";
        /// <summary>
        /// 提交输入框文本 - 名称
        /// </summary>
        public string Text_Inputfield_Name = "Name";
        /// <summary>
        /// 提交输入框文本 - 内容
        /// </summary>
        public string Text_Inputfield_Data = "Datas";
        /// <summary>
        /// 提交参数输入控件名称 - 参数名称
        /// </summary>
        public string Text_Submit_Name_Text = "ArgName";
        /// <summary>
        /// 提交参数输入控件名称 - 数据
        /// </summary>
        public string Text_Submit_Data_Text = "ArgData";

        /// <summary>
        /// 图标 - 帮助
        /// </summary>
        Texture2D icon_Help;
        /// <summary>
        /// 图标 - 警告
        /// </summary>
        Texture2D icon_Warning;
        /// <summary>
        /// 图标 - 错误
        /// </summary>
        Texture2D icon_Error;
        /// <summary>
        /// 图标 - 修改
        /// </summary>
        Texture2D icon_Modified;
        /// <summary>
        /// 图标 - 通知
        /// </summary>
        Texture2D icon_Notice;
        /// <summary>
        /// 图标 - 确认
        /// </summary>
        Texture2D icon_Confirm;

        /// <summary>
        /// 回调函数 - Ok
        /// </summary>
        public Action<string> Callback_Ok;
        /// <summary>
        /// 回调函数 - Cancel
        /// </summary>
        public Action<string> Callback_Cancel;
        /// <summary>
        /// 回调函数 - Alt
        /// </summary>
        public Action<string> Callback_Alt;
        /// <summary>
        /// 回调函数 - Other
        /// </summary>
        public Action<string> Callback_Other;
        /// <summary>
        /// 回调函数 - Special
        /// </summary>
        public Action<string> Callback_Special;
        /// <summary>
        /// 回调函数 - Apply
        /// </summary>
        public Action<string, string, string> Callback_Submit;

        Color SepLineColor = new Color(1, 1, 1, 0.15f);
        Color MessageColor = new Color(1, 1, 1, 1f);
        Color DateTimeColor = new Color(1, 1, 1, 0.55f);
        Color ThemePrimaryColor = new Color(1, 1, 1, 1);

        /// <summary>
        /// 按钮宽度
        /// </summary>
        private float ButtonWidth = 90;
        /// <summary>
        /// 按钮高度
        /// </summary>
        private float ButtonHeight = 25;
        /// <summary>
        /// 按钮间距
        /// </summary>
        private float ButtonDistance = 5;

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);

            sp_DataList = BaseObject.FindProperty("DataList");

            icon_Help = XGUI.GetIcon("Dialogs/icon_help");
            icon_Warning = XGUI.GetIcon("Dialogs/icon_warning");
            icon_Error = XGUI.GetIcon("Dialogs/icon_error");
            icon_Modified = XGUI.GetIcon("Dialogs/icon_modified");
            icon_Notice = XGUI.GetIcon("Dialogs/icon_notice");
            icon_Confirm = XGUI.GetIcon("Dialogs/icon_confirm");

            #region ReorderableList
            ReorderableList = new ReorderableList(BaseObject, sp_DataList, true, true, true, true);
            ReorderableList.drawElementCallback = DrawElementCallback;
            #endregion
        }

        private void OnGUI()
        {
#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            Rect rect = new Rect(0, 0, position.width, position.height);

            Rect rect_icon = new Rect(26, 15, 48, 48);

            switch (XGUIDialogMode)
            {
                case XGUIDialogType.通知:
                    XGUI.gui_icon(rect_icon, icon_Notice);
                    break;
                case XGUIDialogType.确认:
                    XGUI.gui_icon(rect_icon, icon_Confirm);
                    break;
                case XGUIDialogType.修改:
                    XGUI.gui_icon(rect_icon, icon_Modified);
                    break;
                case XGUIDialogType.警告:
                    XGUI.gui_icon(rect_icon, icon_Warning);
                    break;
                case XGUIDialogType.错误:
                    XGUI.gui_icon(rect_icon, icon_Error);
                    break;
                case XGUIDialogType.帮助:
                    XGUI.gui_icon(rect_icon, icon_Help);
                    break;
            }

            #region 对话框标题文字
            Rect rect_title = new Rect(rect.x + 100, rect.y + 15, rect.width - 80, 30);
            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent(Title),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Bold,
                font: XGUI.GetFont("xg-medium"));
            #endregion

            #region 对话框标题分割线
            Rect rect_seperate = new Rect(rect.x + 102, rect.y + 60, 200, 1);
            XGUI.gui_seperator(
                rect: rect_seperate,
                thickness: 1,
                color: SepLineColor);
            #endregion

            #region 对话框内容文字
            Rect rect_msg = new Rect(rect.x + 26, rect.y + 90, rect.width - 45, XGUI.GetSingleLineHeight());
            XGUI.gui_label(
                rect: rect_msg,
                text: new GUIContent(Message),
                text_color: MessageColor,
                size: XGUIFontSize.B,
                clipping: TextClipping.Overflow,
                anchor: TextAnchor.UpperLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Normal);
            #endregion

            #region 对话框日期时间
            Rect rect_date = new Rect(rect.x + 150, rect.y + 15, rect.width - 180, XGUI.GetSingleLineHeight());
            XGUI.gui_label(
              rect: rect_date,
              text: new GUIContent(DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss:ff")),
              text_color: DateTimeColor,
              size: XGUIFontSize.M,
              clipping: clipping,
              anchor: TextAnchor.MiddleRight,
              offset: new Vector2(0, 0),
              font_style: FontStyle.Normal);
            #endregion

            BaseObject.Update();

            #region 对话框如果是参数提交类 - 则显示 "名称" & "内容" 的输入框
            if (XGUIDialogButtonMode == XGUIDialogButtonMode.参数提交)
            {
                // 输入框 - 名称
                Rect preset_name_rect = new Rect(rect.x + 26, rect.y + 160, rect.width - 45, XGUI.GetSingleLineHeight() + 5);
                Text_Inputfield_Name = XGUI.gui_inputfield(
                    rect: preset_name_rect,
                    prop: Text_Inputfield_Name,
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
                    field_text_anchor: TextAnchor.MiddleLeft);

                // 标签 - 名称
                Rect rect_p_name = new Rect(preset_name_rect.x, preset_name_rect.y - 30, 200, 20);
                XGUI.gui_label(
                    rect: rect_p_name,
                    text: new GUIContent(Text_Submit_Name_Text),
                    text_color: Color.white,
                    size: XGUIFontSize.B,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    offset: new Vector2(0, 0),
                    wrap: true,
                    font_style: FontStyle.Normal);

                // 输入框 - 数据
                Rect preset_des_rect = new Rect(rect.x + 26, rect.y + 230, rect.width - 45, 120);
                Text_Inputfield_Data = XGUI.gui_inputfield(
                     rect: preset_des_rect,
                     prop: Text_Inputfield_Data,
                     text_wrap: true,
                     field_fontsize: XGUIFontSize.M,
                     field_text_offset: Vector2.zero,
                     field_height: 120,
                     field_text_color: Color.white,
                     title_width: 40,
                     //status_icon: "icon_field_status",
                     //status_icon_color: Color.magenta,
                     field_text_font: XGUI.GetFont("xg-medium"),
                     field_text_style: FontStyle.Normal,
                     field_text_anchor: TextAnchor.UpperLeft);

                // 标签 - 数据
                Rect rect_p_des = new Rect(preset_des_rect.x, preset_des_rect.y - 30, 200, 20);
                XGUI.gui_label(
                    rect: rect_p_des,
                    text: new GUIContent(Text_Submit_Data_Text),
                    text_color: Color.white,
                    size: XGUIFontSize.B,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    offset: new Vector2(0, 0),
                    wrap: true,
                    font_style: FontStyle.Normal);
            }
            #endregion

            #region 列表
            if (DataList.Count > 0)
            {
                Rect rect_totalcount = new Rect(rect.x + (rect.width - 60), rect.y + 90, 20, XGUI.GetSingleLineHeight());
                XGUI.gui_label(
                    rect: rect_totalcount,
                    text: new GUIContent(DataList.Count.ToString()),
                    text_color: ThemePrimaryColor,
                    size: XGUIFontSize.M,
                    clipping: TextClipping.Overflow,
                    anchor: TextAnchor.MiddleRight,
                    offset: new Vector2(0, 0),
                    font_style: FontStyle.Normal);

                Rect rect_unit = new Rect(rect.x + (rect.width - 40), rect.y + 90, 20, XGUI.GetSingleLineHeight());
                XGUI.gui_label(
                    rect: rect_unit,
                    text: new GUIContent(" 项"),
                    text_color: Color.white * 0.7f,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleRight,
                    offset: new Vector2(0, 0),
                    font_style: FontStyle.Normal);

                XGUI.layout_space(130);
                DrawDataList();
                XGUI.layout_space(6);
            }
            else
            {
                if (XGUIDialogButtonMode == XGUIDialogButtonMode.参数提交)
                    XGUI.layout_space(380);
                else
                    XGUI.layout_space(185);
            }
            #endregion

            BaseObject.ApplyModifiedProperties();

            #region 按钮控件
            string FocusName = Text_Ok + PrimaryIndex;

            switch (PrimaryIndex)
            {
                case 0:
                    FocusName = Text_Ok + PrimaryIndex;
                    break;
                case 1:
                    FocusName = Text_Cancel + PrimaryIndex;
                    break;
                case 2:
                    FocusName = Text_Alt + PrimaryIndex;
                    break;
                case 3:
                    FocusName = Text_Other + PrimaryIndex;
                    break;
                case 4:
                    FocusName = Text_Special + PrimaryIndex;
                    break;
            }

            if (XGUIDialogButtonMode != XGUIDialogButtonMode.参数提交)
                EditorGUI.FocusTextInControl(FocusName);

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(20, 20, 0, 0));

            XGUI.layout_flexspace();
            switch (XGUIDialogButtonMode)
            {
                case XGUIDialogButtonMode.单个按钮:
                    DialogType_BtnMode_1();
                    break;
                case XGUIDialogButtonMode.两个按钮:
                    DialogType_BtnMode_2();
                    break;
                case XGUIDialogButtonMode.三个按钮:
                    DialogType_BtnMode_3();
                    break;
                case XGUIDialogButtonMode.四个按钮:
                    DialogType_BtnMode_4();
                    break;
                case XGUIDialogButtonMode.五个按钮:
                    DialogType_BtnMode_5();
                    break;
                case XGUIDialogButtonMode.参数提交:
                    DialogType_ArgSubmit();
                    break;
            }
            XGUI.layout_group_end(XGUIContainerType.Horizontal);
            #endregion
            Repaint();
        }

        private void OnDisable()
        {

        }

        private void OnDestroy()
        {
            GetWindow<SceneView>().Focus();
        }

        #region ColorInfoList_Original      
        Rect index_rect;
        Rect msg_rect;
        Rect title_rect;
        Rect sub_rect;
        Rect scrollview_rect;
        Rect item_rect;
        Rect itemmark_rect;
        Rect itemmarkBg_rect;

        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            index_rect = new Rect(rect.x + 15, rect.y + 5, 30, 20);
            title_rect = new Rect(rect.x + 39, rect.y + 5, 160, 20);
            msg_rect = new Rect(rect.x + 77, rect.y + 5, rect.width - 115, 20);
            sub_rect = new Rect((rect.width / 2) - 50, rect.y + 5, 100, 20);

            SerializedProperty prop = sp_DataList.GetArrayElementAtIndex(index);
            SerializedProperty sp_title = prop.FindPropertyRelative("Title");
            SerializedProperty sp_sub = prop.FindPropertyRelative("SubTitle");
            SerializedProperty sp_msg = prop.FindPropertyRelative("Message");

#if UNITY_6000_0_OR_NEWER
            // Unity 6+ 使用 Ellipsis
            TextClipping clipping = TextClipping.Ellipsis;
#else
    // Unity 2021.1 之前使用 Clip
    TextClipping clipping = TextClipping.Clip;
#endif
            // 列表项 - 序号
            XGUI.gui_label(
                  rect: index_rect,
                  text: new GUIContent(index.ToString("D2")),
                  text_color: ThemePrimaryColor,
                  size: XGUIFontSize.M,
                  clipping: clipping,
                  anchor: TextAnchor.MiddleLeft,
                  offset: new Vector2(0, 0),
                  font_style: FontStyle.Normal);
            // 列表项 - 标题
            XGUI.gui_label(
                  rect: title_rect,
                  text: new GUIContent(sp_title.stringValue),
                  text_color: Color.white,
                  size: XGUIFontSize.M,
                  clipping: clipping,
                  anchor: TextAnchor.MiddleLeft,
                  offset: new Vector2(0, 0),
                  font_style: FontStyle.Normal);
            // 列表项 - 副标题
            XGUI.gui_label(
                  rect: sub_rect,
                  text: new GUIContent(sp_sub.stringValue),
                  text_color: Color.white * 0.7f,
                  size: XGUIFontSize.M,
                  clipping: clipping,
                  anchor: TextAnchor.MiddleCenter,
                  offset: new Vector2(0, 0),
                  font_style: FontStyle.Normal);
            // 列表项 - 内容消息
            XGUI.gui_label(
                  rect: msg_rect,
                  text: new GUIContent(sp_msg.stringValue),
                  text_color: ThemePrimaryColor,
                  size: XGUIFontSize.M,
                  clipping: clipping,
                  anchor: TextAnchor.MiddleRight,
                  offset: new Vector2(0, 0),
                  font_style: FontStyle.Normal);
        }
        /// <summary>
        /// 绘制列表
        /// </summary>
        private void DrawDataList()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, visibleItemCount * itemHeight);
            DataList_Scroller = GUI.BeginScrollView(scrollview_rect, DataList_Scroller, new Rect(-15, 0, scrollview_rect.width - 50, DataList.Count * itemHeight), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(DataList_Scroller.y / itemHeight);
            int endIndex = Mathf.CeilToInt((DataList_Scroller.y + scrollview_rect.height) / itemHeight);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < ReorderableList.count; i++)
            {
                SerializedProperty prop = sp_DataList.GetArrayElementAtIndex(i);

                item_rect = new Rect(0, i * itemHeight, scrollview_rect.width, itemHeight);

                // 如果当前元素被选中，绘制选中效果
                if (SelectedIndex == i)
                {
                    itemmark_rect = new Rect(item_rect.x + 1, item_rect.y + 11, 5, 5);
                    // 高亮标记表示选中
                    EditorGUI.DrawRect(itemmark_rect, ThemePrimaryColor);
                    itemmarkBg_rect = new Rect(item_rect.x, item_rect.y, item_rect.width + 20, item_rect.height);
                    // 高亮背景表示选中
                    EditorGUI.DrawRect(itemmarkBg_rect, new Color(0, 0, 0, 0.2f));
                }

                ReorderableList.drawElementCallback.Invoke(item_rect, i, i == ReorderableList.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        // 更新选中项
                        SelectedIndex = i;

                        SerializedProperty prop_sourceobject = prop.FindPropertyRelative("SourceObject");
                        if (prop_sourceobject.objectReferenceValue != null)
                        {
                            EditorGUIUtility.PingObject(prop_sourceobject.objectReferenceValue);
                        }

                        // 标记界面需要更新
                        GUI.changed = true;
                    }
                }
            }

            GUI.EndScrollView();
        }
        #endregion

        /// <summary>
        ///1个按钮模式
        /// </summary>
        private void DialogType_BtnMode_1()
        {
            if (XGUI.layout_button(
                text: Text_Ok,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: ThemePrimaryColor,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Ok + PrimaryIndex))
            {
                Close();

                // 调用回调函数
                Callback_Ok?.Invoke(Text_Ok);
            }
        }
        /// <summary>
        /// 2个按钮模式
        /// </summary>
        private void DialogType_BtnMode_2()
        {
            if (XGUI.layout_button(
                text: Text_Cancel,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 1 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Cancel + PrimaryIndex))
            {
                Close();
                // 调用回调函数
                Callback_Cancel?.Invoke(Text_Cancel);
            }

            XGUI.layout_space(ButtonDistance);

            if (XGUI.layout_button(
                text: Text_Ok,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 0 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Ok + PrimaryIndex))
            {
                Close();
                // 调用回调函数
                Callback_Ok?.Invoke(Text_Ok);
            }
            GUI.backgroundColor = Color.white;
        }
        /// <summary>
        /// 3个按钮模式
        /// </summary>
        private void DialogType_BtnMode_3()
        {
            if (XGUI.layout_button(
                text: Text_Alt,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 2 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Alt + PrimaryIndex))
            {
                Close();

                // 调用回调函数
                Callback_Alt?.Invoke(Text_Alt);
            }
            GUI.backgroundColor = Color.white;
            XGUI.layout_space(ButtonDistance);
            if (XGUI.layout_button(
                text: Text_Cancel,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 1 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Cancel + PrimaryIndex))
            {
                Close();

                // 调用回调函数
                Callback_Cancel?.Invoke(Text_Cancel);
            }
            GUI.backgroundColor = Color.white;
            XGUI.layout_space(ButtonDistance);
            if (XGUI.layout_button(
                text: Text_Ok,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 0 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Ok + PrimaryIndex))
            {
                Close();

                // 调用回调函数
                Callback_Ok?.Invoke(Text_Ok);
            }
            GUI.backgroundColor = Color.white;
        }
        /// <summary>
        /// 4个按钮模式
        /// </summary>
        private void DialogType_BtnMode_4()
        {
            if (XGUI.layout_button(
              text: Text_Other,
              tooltip: "",
              bg_fill: XGUIFilled.实体,
              bg_color: XGUIColor.亮白,
              bg_color_gui: PrimaryIndex == 3 ? ThemePrimaryColor : Color.white,
              button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
              press_fill: XGUIFilled.实体,
              press_color: XGUIColor.深空灰,
              press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
              font_size: XGUIFontSize.B,
              anchor: TextAnchor.MiddleCenter,
              margin: new RectOffset(0, 0, 0, 0),
              padding: new RectOffset(0, 0, 0, 0),
              width: ButtonWidth,
              height: ButtonHeight,
              button_text_font: XGUI.GetFont("xg-medium"),
              focus_name: Text_Other + PrimaryIndex))
            {
                Close();

                // 调用回调函数
                Callback_Other?.Invoke(Text_Other);
            }
            GUI.backgroundColor = Color.white;
            XGUI.layout_space(ButtonDistance);
            if (XGUI.layout_button(
                text: Text_Alt,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 2 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Alt + PrimaryIndex))
            {
                Close();

                // 调用回调函数
                Callback_Alt?.Invoke(Text_Alt);
            }
            GUI.backgroundColor = Color.white;
            XGUI.layout_space(ButtonDistance);
            if (XGUI.layout_button(
                text: Text_Cancel,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 1 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Cancel + PrimaryIndex))
            {
                Close();

                // 调用回调函数
                Callback_Cancel?.Invoke(Text_Cancel);
            }
            GUI.backgroundColor = Color.white;
            XGUI.layout_space(ButtonDistance);
            if (XGUI.layout_button(
                text: Text_Ok,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 0 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Ok + PrimaryIndex))
            {
                Close();

                // 调用回调函数
                Callback_Ok?.Invoke(Text_Ok);
            }
            GUI.backgroundColor = Color.white;
        }
        /// <summary>
        /// 5个按钮模式
        /// </summary>
        private void DialogType_BtnMode_5()
        {
            if (XGUI.layout_button(
                text: Text_Special,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 4 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Special + PrimaryIndex))
            {
                Close();

                // 调用回调函数
                Callback_Special?.Invoke(Text_Special);
            }
            GUI.backgroundColor = Color.white;
            XGUI.layout_space(ButtonDistance);
            if (XGUI.layout_button(
                text: Text_Other,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 3 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Other + PrimaryIndex))
            {
                Close();

                // 调用回调函数
                Callback_Other?.Invoke(Text_Other);
            }
            GUI.backgroundColor = Color.white;
            XGUI.layout_space(ButtonDistance);
            if (XGUI.layout_button(
                text: Text_Alt,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 2 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Alt + PrimaryIndex))
            {
                Close();

                // 调用回调函数
                Callback_Alt?.Invoke(Text_Alt);
            }
            GUI.backgroundColor = Color.white;
            XGUI.layout_space(ButtonDistance);
            if (XGUI.layout_button(
                text: Text_Cancel,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 1 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Cancel + PrimaryIndex))
            {
                Close();

                // 调用回调函数
                Callback_Cancel?.Invoke(Text_Cancel);
            }
            GUI.backgroundColor = Color.white;
            XGUI.layout_space(ButtonDistance);
            if (XGUI.layout_button(
                text: Text_Ok,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 0 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Ok + PrimaryIndex))
            {
                Close();

                // 调用回调函数
                Callback_Ok?.Invoke(Text_Ok);
            }
            GUI.backgroundColor = Color.white;
        }
        /// <summary>
        /// 参数提交模式
        /// </summary>
        private void DialogType_ArgSubmit()
        {
            if (XGUI.layout_button(
                text: Text_Cancel,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 1 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Cancel + PrimaryIndex))
            {
                Close();
                // 调用回调函数
                Callback_Cancel?.Invoke(Text_Cancel);
            }
            GUI.backgroundColor = Color.white;
            XGUI.layout_space(ButtonDistance);
            if (XGUI.layout_button(
                text: Text_Ok,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: PrimaryIndex == 0 ? ThemePrimaryColor : Color.white,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(ThemePrimaryColor) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: ButtonWidth,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: Text_Ok + PrimaryIndex))
            {
                Close();
                // 调用回调函数
                Callback_Submit?.Invoke(Text_Inputfield_Name, Text_Inputfield_Data, Text_Ok);
            }
            GUI.backgroundColor = Color.white;
        }
        /// <summary>
        /// 设置对话框信息
        /// </summary>
        /// <param name="Type">模式</param>
        /// <param name="x_Title">面板标题</param>
        /// <param name="x_Msg">面板内容</param>
        /// <param name="x_Ok">确认文字</param>
        /// <param name="x_Cancel">取消文字</param>
        /// <param name="x_Alt">辅助文字</param>
        /// <param name="x_Other">其他文字</param>
        /// <param name="x_Special">特别文字</param>
        public void SetInfo(XGUIDialogType Type, string x_Title = "", string x_Msg = "", string x_Ok = "", string x_Cancel = "", string x_Alt = "", string x_Other = "", string x_Special = "", string x_submit_name = "", string x_submit_data = "", int x_PrimaryIndex = 0)
        {
            PrimaryIndex = x_PrimaryIndex;
            XGUIDialogMode = Type;
            Title = x_Title;
            Message = x_Msg;
            Text_Ok = x_Ok;
            Text_Cancel = x_Cancel;
            Text_Alt = x_Alt;
            Text_Other = x_Other;
            Text_Special = x_Special;
            Text_Submit_Name_Text = x_submit_name;
            Text_Submit_Data_Text = x_submit_data;
        }
        /// <summary>
        /// 设置数据列表
        /// </summary>
        /// <param name="datas"></param>
        public void SetList(XGUIDialogListDatas[] datas)
        {
            if (DataList == null)
                DataList = new List<XGUIDialogListDatas>();
            DataList.Clear();
            for (int i = 0; i < datas.Length; i++)
            {
                DataList.Add(datas[i]);
            }
        }
        /// <summary>
        /// 设置主题色
        /// </summary>
        /// <param name="datas"></param>
        public void SetThemeColor(Color color = default)
        {
            if (color == Color.clear)
                color = Color.white;

            ThemePrimaryColor = color;
        }
    }
}
