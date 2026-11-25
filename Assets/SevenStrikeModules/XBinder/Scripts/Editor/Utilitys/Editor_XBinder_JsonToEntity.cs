/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XBinder - Unity 高级数据绑定管理插件
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
namespace SevenStrikeModules.XBinder.Editor
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    #region 编辑器窗口
    /// <summary>
    /// Json 转 C# 类实体结构生成器窗口
    /// </summary>
    public class Editor_XBinder_JsonToEntity : EditorWindow
    {
        #region 字段
        /// <summary>
        /// 选中的 Json 文件（TextAsset）
        /// </summary>
        [SerializeField]
        private TextAsset _jsonFile;
        /// <summary>
        /// 生成的根类名
        /// </summary>
        [SerializeField]
        private string _rootClassName = "Entity";
        /// <summary>
        /// 生成的命名空间（可为空）
        /// </summary>
        [SerializeField]
        private string _namespaceName = "";
        /// <summary>
        /// 输出文件夹路径
        /// </summary>
        [SerializeField]
        private string _outputFolder = "Assets/SevenStrikeModules/XBinder/ConvertedClass/";
        /// <summary>
        /// 生成后的 C# 代码缓存
        /// </summary>
        private string _generatedCode = "";
        /// <summary>
        /// 输出预览区滚动位置
        /// </summary>
        private Vector2 _outputScroll;
        /// <summary>
        /// 菜单面板索引
        /// </summary>
        private int panel_index;
        /// <summary>
        /// 是否自动根据根类名生成文件名
        /// </summary>
        [SerializeField]
        private bool _autoGenerateFileName = true;
        /// <summary>
        /// 当前窗口绑定的序列化对象
        /// </summary>
        private SerializedObject _serializedObject;
        /// <summary>
        /// Json 文件字段的序列化属性
        /// </summary>
        private SerializedProperty _jsonFileProp;
        /// <summary>
        /// 根类名字段的序列化属性
        /// </summary>
        private SerializedProperty _rootClassNameProp;
        /// <summary>
        /// 命名空间字段的序列化属性
        /// </summary>
        private SerializedProperty _namespaceNameProp;
        /// <summary>
        /// 输出文件夹字段的序列化属性
        /// </summary>
        private SerializedProperty _outputFolderProp;
        /// <summary>
        /// 自动生成文件名字段的序列化属性
        /// </summary>
        private SerializedProperty _autoGenerateFileNameProp;
        #endregion

        #region 高级修复选项字段
        [Header("JSON 修复")]
        [SerializeField] private bool _fixStringifiedJson = true;
        [SerializeField] private bool _cleanCommentsAndTrailingCommas = true;
        [SerializeField] private bool _handleMixedTypes = true;
        [SerializeField] private bool _smartBoolDetection = true;

        [Header("类型推断")]
        [SerializeField] private bool _useLongForLargeNumbers = true;
        [SerializeField] private bool _useFloatForDecimals = true;
        [SerializeField] private bool _warnOnEmptyArray = true;

        [Header("代码生成")]
        [SerializeField] private bool _addSerializableAttribute = true;
        [SerializeField] private bool _singularizeArrayClassNames = true;
        [SerializeField] private int _maxClassNameLength = 40;
        #endregion

        #region 序列化属性
        private SerializedProperty _fixStringifiedJsonProp;
        private SerializedProperty _cleanCommentsAndTrailingCommasProp;
        private SerializedProperty _handleMixedTypesProp;
        private SerializedProperty _smartBoolDetectionProp;
        private SerializedProperty _useLongForLargeNumbersProp;
        private SerializedProperty _useFloatForDecimalsProp;
        private SerializedProperty _warnOnEmptyArrayProp;
        private SerializedProperty _addSerializableAttributeProp;
        private SerializedProperty _singularizeArrayClassNamesProp;
        private SerializedProperty _maxClassNameLengthProp;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 标题主图标
        /// </summary>
        private Texture2D icon_main;
        #endregion

        #region 折叠
        private bool fold_option;
        #endregion

        #region 窗口入口
        /// <summary>
        /// 打开窗口的菜单入口
        /// </summary>
        [MenuItem("Tools/XBinder/Json 解析到 C# 实体类工具")]
        public static void ShowWindow()
        {
            var window = GetWindow<Editor_XBinder_JsonToEntity>("Json 解析到 C# 实体类工具");
            window.minSize = new Vector2(350, 750);
            window.maxSize = window.minSize;
        }
        #endregion

        #region 生命周期
        /// <summary>
        /// 窗口启用时初始化序列化对象和属性
        /// </summary>
        private void OnEnable()
        {
            #region 获取序列化属性
            _serializedObject = new SerializedObject(this);
            _jsonFileProp = _serializedObject.FindProperty(nameof(_jsonFile));
            _rootClassNameProp = _serializedObject.FindProperty(nameof(_rootClassName));
            _namespaceNameProp = _serializedObject.FindProperty(nameof(_namespaceName));
            _outputFolderProp = _serializedObject.FindProperty(nameof(_outputFolder));
            _autoGenerateFileNameProp = _serializedObject.FindProperty(nameof(_autoGenerateFileName));

            _fixStringifiedJsonProp = _serializedObject.FindProperty(nameof(_fixStringifiedJson));
            _cleanCommentsAndTrailingCommasProp = _serializedObject.FindProperty(nameof(_cleanCommentsAndTrailingCommas));
            _handleMixedTypesProp = _serializedObject.FindProperty(nameof(_handleMixedTypes));
            _smartBoolDetectionProp = _serializedObject.FindProperty(nameof(_smartBoolDetection));
            _useLongForLargeNumbersProp = _serializedObject.FindProperty(nameof(_useLongForLargeNumbers));
            _useFloatForDecimalsProp = _serializedObject.FindProperty(nameof(_useFloatForDecimals));
            _warnOnEmptyArrayProp = _serializedObject.FindProperty(nameof(_warnOnEmptyArray));
            _addSerializableAttributeProp = _serializedObject.FindProperty(nameof(_addSerializableAttribute));
            _singularizeArrayClassNamesProp = _serializedObject.FindProperty(nameof(_singularizeArrayClassNames));
            _maxClassNameLengthProp = _serializedObject.FindProperty(nameof(_maxClassNameLength));
            #endregion

            #region 获取图标
            icon_main = XGUI.GetCustomIcon($"{XBinder_Dashboard.get_path_xbinder_gui()}gui_json_to_entity/icon_main");
            Debug.Log($"{XBinder_Dashboard.get_path_xbinder_gui()}gui_json_to_entity/icon_main");
            #endregion
        }
        #endregion

        #region 绘制GUI
        /// <summary>
        /// 绘制编辑器窗口的 GUI
        /// </summary>
        private void OnGUI()
        {
            if (_serializedObject == null)
                OnEnable();
            _serializedObject.Update();

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            string hexcol = XGUI_Utilitys.Color_To_HexString(XBinder_Dashboard.theme_primary, true);

            #region 抬头
            Rect rect = new Rect(0, 0, position.width, position.height);

            // 图标
            Rect rect_icon = new Rect(15, 15, icon_main.width, icon_main.height);
            XGUI.gui_icon(
            rect: rect_icon,
                icon: icon_main,
                padding: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                color: Color.white);

            // 大标题
            Rect rect_title = new Rect(rect.x + 70, rect.y + 10, rect.width - 80, 30);
            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent("Json 转 C# 实体工具"),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping_ellipsis,
                font: XGUI.GetFont("xg-heavy"));

            // 分割线
            Rect rect_seperate = new Rect(rect.x + 68, rect.y + 43, 200, 1);
            XGUI.gui_seperator(
                rect: rect_seperate,
                thickness: 1,
                color: XBinder_Dashboard.theme_sepline,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            // 小标题
            Rect rect_subtitle = new Rect(rect.x + 18, rect.y + 53, rect.width, 30);
            XGUI.gui_label(
                rect: rect_subtitle,
                text: new GUIContent("使用此工具快速分析json文件并创建c#实体"),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                anchor: TextAnchor.UpperLeft,
                clipping: clipping_ellipsis);
            #endregion

            XGUI.layout_space(100);

            panel_index = XGUI.layout_toolbar(
                index: ref panel_index,
                names: new string[] { "参数", "选项" },
                bg_normal: XGUIGroup.纯色边框,
                bg_selected: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_gui_color: Color.black * 0.5f,
                text_color_normal: Color.white,
                text_color_selected: XBinder_Dashboard.theme_primary,
                bar_height: 25,
                text_anchor: TextAnchor.MiddleCenter,
                text_padding: new RectOffset(10, 10, 0, 4),
                bar_margin: new RectOffset(18, 18, 0, 0),
                bar_offset: new Vector2(0, 0),
                text_offset: new Vector2(0, 0),
                text_font: XGUI.GetFont("xg-regular"),
                text_fontstyle: FontStyle.Normal,
                bg_width_offset: 5,
                bg_height_offset: 2,
                navigate_style: true,
                navigate_style_bg: XGUIGroup.纯色边框,
                navigate_style_bg_color: Color.black * 0.5f);

            if (panel_index == 0)
            {
                #region 参数
                XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XBinder_Dashboard.theme_group,
                    title: "参数",
                    title_size: XGUIFontSize.M,
                    title_text_color: XBinder_Dashboard.theme_primary,
                    title_clipping: TextClipping.Clip,
                    margin: new RectOffset(10, 10, 0, 0),
                    padding: new RectOffset(10, 10, 15, 15));

                #region 说明
                string msg = $"拖入 <color={hexcol}>.json</color> 文件到窗口内，或通过选择框指定文件；点击 <color={hexcol}>生成 C# 代码</color> 预览，然后点击 <color={hexcol}>保存为 .cs 文件</color> 导出";

                XGUI.layout_label(
                    text: msg,
                    bg_fill: XGUIGroup.实体,
                    bg_color: XGUIColor.深空灰,
                    size: XGUIFontSize.S,
                    anchor: TextAnchor.MiddleLeft,
                    text_color: Color.white * 0.85f,
                    offset: new Vector2(0, 0),
                    padding: new RectOffset(10, 10, 10, 10),
                    margin: new RectOffset(0, 0, 6, 10),
                    clipping: TextClipping.Clip,
                    wrap: true,
                    font: XGUI.GetFont("xg-regular"),
                    font_style: FontStyle.Normal);
                #endregion

                #region Json 文件
                XGUI.layout_property_field(
                      title: "Json 文件",
                      title_size: XGUIFontSize.M,
                      title_hover_color: Color.white,
                      title_width: 90,
                      prop: _jsonFileProp,
                      prop_padding: new RectOffset(0, 0, 0, 0),
                      prop_margin: new RectOffset(0, 0, 0, 10));
                #endregion

                #region 根类名
                XGUI.layout_property_field(
                      title: "根类名",
                      title_size: XGUIFontSize.M,
                      title_hover_color: Color.white,
                      title_width: 90,
                      prop: _rootClassNameProp,
                      prop_padding: new RectOffset(0, 0, 0, 0),
                      prop_margin: new RectOffset(0, 0, 0, 10));
                #endregion

                #region 命名空间 (可空)
                XGUI.layout_property_field(
                      title: "命名空间 (可空)",
                      title_size: XGUIFontSize.M,
                      title_hover_color: Color.white,
                      title_width: 90,
                      prop: _namespaceNameProp,
                      prop_padding: new RectOffset(0, 0, 0, 0),
                      prop_margin: new RectOffset(0, 0, 0, 10));
                #endregion

                #region 输出文件夹
                XGUI.layout_path_selector(
                    path_type: XGUIPathType.文件夹,
                    prop: ref _outputFolder,
                    title: "输出文件夹",
                    //title_color: Color.white,
                    title_width: 162,
                    text_wrap: false,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: new Vector2(0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0),
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_text_color: Color.white,
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    filter: null,
                    filter_title: "选择文件夹",
                    btn_color: XBinder_Dashboard.theme_primary,
                    btn_fontsize: XGUIFontSize.M,
                    default_name: "default",
                    status_icon: "icon_dot",
                    status_icon_color: XGUI_Utilitys.PathExists(_outputFolder) ? Color.green : Color.red,
                    on_path_changed: (val) =>
                    {
                        _outputFolder = val;

                    });
                if (!string.IsNullOrEmpty(_outputFolder))
                {
                    // 转成相对工程路径
                    if (_outputFolder.StartsWith(Application.dataPath))
                        _outputFolderProp.stringValue = "Assets" + _outputFolder.Substring(Application.dataPath.Length);
                    else
                        _outputFolderProp.stringValue = _outputFolder;
                }
                #endregion

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion
            }
            else if (panel_index == 1)
            {
                #region 选项
                fold_option = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XBinder_Dashboard.theme_group,
                    title: "选项",
                    title_size: XGUIFontSize.M,
                    title_text_color: XBinder_Dashboard.theme_primary,
                    title_clipping: TextClipping.Clip,
                    margin: new RectOffset(10, 10, 0, 0),
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: fold_option);

                if (!fold_option)
                {
                    #region 自动根据根类名生成文件名
                    DrawToggle("自动根据根类名生成文件名", _autoGenerateFileNameProp, 150, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                    #endregion

                    #region 自动修复嵌套字符串 JSON
                    DrawToggle("自动修复嵌套字符串 JSON", _fixStringifiedJsonProp, 150, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                    #endregion

                    #region 清理注释与尾随逗号
                    DrawToggle("清理注释与尾随逗号", _cleanCommentsAndTrailingCommasProp, 150, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                    #endregion

                    #region 混用类型回退为 string
                    DrawToggle("混用类型回退为 string", _handleMixedTypesProp, 150, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                    #endregion

                    #region 智能识别布尔值 (0/1, \"true\")")
                    DrawToggle("智能识别布尔值 (0/1, \\\"true\\\")\")", _smartBoolDetectionProp, 150, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                    #endregion

                    #region 超长数字自动转 long
                    DrawToggle("超长数字自动转 long", _useLongForLargeNumbersProp, 150, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                    #endregion

                    #region 小数自动转 float
                    DrawToggle("小数自动转 float", _useFloatForDecimalsProp, 150, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                    #endregion

                    #region 空数组附加警告注释
                    DrawToggle("空数组附加警告注释", _warnOnEmptyArrayProp, 150, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                    #endregion

                    #region 自动添加 [Serializable] 与 using
                    DrawToggle("自动添加 [Serializable] 与 using", _addSerializableAttributeProp, 150, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                    #endregion

                    #region 数组字段类名单数化
                    DrawToggle("数组字段类名单数化", _singularizeArrayClassNamesProp, 150, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                    #endregion

                    #region 类名最大长度
                    DrawToggle("类名最大长度", _maxClassNameLengthProp, 150, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                    #endregion
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion
            }

            #region 状态
            XGUI.layout_group_start(
            type: XGUIContainerType.Vertical,
            bg_fill: XGUIGroup.缺口纯色边框,
            bg_color: XGUIColor.亮白,
            bg_color_gui: XBinder_Dashboard.theme_group,
            title: "状态",
            title_size: XGUIFontSize.M,
            title_text_color: XBinder_Dashboard.theme_primary,
            title_clipping: TextClipping.Clip,
            margin: new RectOffset(10, 10, 0, 0),
            padding: new RectOffset(10, 10, 15, 15));

            XGUI.layout_helpbox(
                state: _jsonFile == null ? XGUIHelboxState.警告 : XGUIHelboxState.通知,
                title_text: _jsonFile == null ? "尚未选择 Json 文件。可将 .json 文件拖入本窗口，或使用上方的选择框指定！" : $"已选择: {AssetDatabase.GetAssetPath(_jsonFile)}\n大小: {_jsonFile.text.Length} 字符",
                title_size: XGUIFontSize.M,
                title_style: FontStyle.Normal,
                wrap: true,
                title_clipping: TextClipping.Clip,
                title_color: Color.white * 0.75f);

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 按钮
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XBinder_Dashboard.theme_group,
                title: "操作",
                title_size: XGUIFontSize.M,
                title_text_color: XBinder_Dashboard.theme_primary,
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(10, 10, 0, 0),
                padding: new RectOffset(10, 10, 15, 15));

            #region 生成实体类
            if (XGUI.layout_button(
                text: "生成实体类",
                tooltip: "",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XBinder_Dashboard.theme_primary,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(XBinder_Dashboard.theme_primary) ? Color.black : Color.white,
                press_fill: XGUIGroup.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.M,
                anchor: TextAnchor.MiddleCenter,
                height: 25,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                GenerateCode();
            }
            #endregion

            #region 保存实体类
            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(_generatedCode)))
            {
                if (XGUI.layout_button(
                    text: "保存实体类",
                    tooltip: "",
                    bg_fill: XGUIGroup.实体,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XBinder_Dashboard.theme_primary,
                    button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(XBinder_Dashboard.theme_primary) ? Color.black : Color.white,
                    press_fill: XGUIGroup.实体,
                    press_color: XGUIColor.深空灰,
                    press_text_color: Color.white,
                    font_size: XGUIFontSize.M,
                    anchor: TextAnchor.MiddleCenter,
                    height: 25,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    button_text_font: XGUI.GetFont("xg-medium")))
                {
                    SaveToFile();
                }
            }
            #endregion

            #region 清空
            if (XGUI.layout_button(
                text: "清空",
                tooltip: "",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XBinder_Dashboard.theme_primary,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(XBinder_Dashboard.theme_primary) ? Color.black : Color.white,
                press_fill: XGUIGroup.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.M,
                anchor: TextAnchor.MiddleCenter,
                height: 25,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                _generatedCode = "";
                _jsonFile = null;
                _serializedObject.Update();
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 实体类预览
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XBinder_Dashboard.theme_group,
                title: "实体类预览",
                title_size: XGUIFontSize.M,
                title_text_color: XBinder_Dashboard.theme_primary,
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(10, 10, 0, 10),
                padding: new RectOffset(10, 10, 15, 15));

            if (!string.IsNullOrEmpty(_generatedCode))
            {
                _outputScroll = EditorGUILayout.BeginScrollView(_outputScroll, GUILayout.ExpandHeight(true));
                EditorGUILayout.TextArea(_generatedCode, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion         

            _serializedObject.ApplyModifiedProperties();
            HandleDragAndDrop();
        }
        #endregion

        #region 拖拽支持
        /// <summary>
        /// 处理窗口内的拖拽事件：将 Project 中的 .json 文件拖入窗口后自动绑定
        /// </summary>
        private void HandleDragAndDrop()
        {
            Event evt = Event.current;

            // 只处理拖拽相关事件
            if (evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform)
                return;

            // 检查拖拽对象中是否包含 .json 文件
            var jsonPaths = new List<string>();
            foreach (string path in DragAndDrop.paths)
            {
                if (path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    jsonPaths.Add(path);
            }

            if (jsonPaths.Count == 0)
                return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                string firstJsonPath = jsonPaths[0];
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(firstJsonPath);
                if (textAsset != null)
                {
                    _jsonFile = textAsset;
                    _generatedCode = "";
                    _serializedObject.Update();
                    Repaint();
                }
                else
                {
                    XGUI_Utilitys.Console("XBinder 消息", $"[JsonToClassGenerator] 无法加载为 TextAsset: {firstJsonPath}", XGUIMsgState.错误);
                }
            }

            evt.Use();
        }
        #endregion

        #region 解析 Json + 生成代码
        /// <summary>
        /// 读取所选 Json 文件并生成 C# 代码，结果存入 _generatedCode
        /// </summary>
        private void GenerateCode()
        {
            if (_jsonFile == null)
            {
                XGUI.dialog(
                  type: XGUIDialogType.错误,
                  windowtitle: "XBinder - Json解析到C#类",
                  title: "生成失败",
                  msg: $"请先拖入或选择一个 Json 文件！",
                  ok: "明白",
                  PrimaryIndex: 0,
                  usemodal: true,
                  themecolor: Color.white);
                return;
            }

            string jsonText = _jsonFile.text;
            if (string.IsNullOrWhiteSpace(jsonText))
            {
                XGUI.dialog(
                  type: XGUIDialogType.警告,
                  windowtitle: "XBinder - Json解析到C#类",
                  title: "生成失败",
                  msg: $"Json 文件内容为空！",
                  ok: "明白",
                  PrimaryIndex: 0,
                  usemodal: true,
                  themecolor: Color.white);
                return;
            }

            try
            {
                // 清洗非标准 JSON
                if (_cleanCommentsAndTrailingCommas)
                    jsonText = CleanNonStandardJson(jsonText);

                // 解析
                var parser = new SimpleJsonParser(jsonText, _fixStringifiedJson);
                object root = parser.Parse();

                // ==========================================================
                // 计算根类名与嵌套类前缀
                // 规则：
                //  - 用户未改根类名（为空或默认 "Entity"）时，自动拼 "{文件名}_Entity"
                //  - 用户手动改了根类名，则尊重用户输入
                //  - 嵌套类前缀 = 根类名 + "_"
                // ==========================================================
                string fileBaseName = _jsonFile != null ? _jsonFile.name : "Entity";

                string rootName = _rootClassName;
                if (string.IsNullOrWhiteSpace(rootName) || rootName == "Entity")
                    rootName = fileBaseName + "_Entity";

                string classPrefix = rootName + "_";

                // 生成代码（传入所有选项）
                var generator = new ClassGenerator(
                    rootName,
                    _namespaceName,
                    _addSerializableAttribute,
                    _handleMixedTypes,
                    _useLongForLargeNumbers,
                    _useFloatForDecimals,
                    _warnOnEmptyArray,
                    _singularizeArrayClassNames,
                    _maxClassNameLength,
                    _smartBoolDetection,
                    classPrefix
                );

                _generatedCode = generator.Generate(root);

                XGUI_Utilitys.Console("XBinder 消息", $"[JsonToClassGenerator] 生成成功！", XGUIMsgState.确认);
            }
            catch (Exception e)
            {
                _generatedCode = "";

                XGUI_Utilitys.Console("XBinder 消息", $"[JsonToClassGenerator] 解析失败: {e}", XGUIMsgState.错误);

                XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XBinder - Json解析到C#类",
                    title: "解析失败",
                    msg: $"失败的原因为：{e.Message}",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: Color.white);
            }
        }
        /// <summary>
        /// 清理非标准 JSON：注释与尾随逗号
        /// </summary>
        private static string CleanNonStandardJson(string json)
        {
            // 移除多行注释
            json = Regex.Replace(json, @"/\*[\s\S]*?\*/", "");
            // 移除单行注释（避免误伤 http://）
            json = Regex.Replace(json, @"(?<!:)//[^\n]*", "");
            // 移除尾随逗号
            json = Regex.Replace(json, @",\s*([}\]])", "$1");
            return json;
        }
        /// <summary>
        /// 将生成的代码保存为 .cs 文件（弹出保存面板让用户选择位置）
        /// </summary>
        private void SaveToFile()
        {
            if (string.IsNullOrEmpty(_generatedCode)) return;

            // 默认文件名
            string defaultFileName = _autoGenerateFileName ? SanitizeFileName($"{_jsonFile.name}_{_rootClassName}") : "GeneratedClass";

            // 默认目录：优先使用配置的输出文件夹，若不存在则回退到工程根目录
            string defaultDir = _outputFolder;
            if (string.IsNullOrEmpty(defaultDir) || !Directory.Exists(defaultDir))
            {
                // 若配置的是 Assets 相对路径，尝试转成绝对路径
                if (!string.IsNullOrEmpty(defaultDir) && defaultDir.StartsWith("Assets"))
                    defaultDir = Path.GetFullPath(defaultDir);
                if (string.IsNullOrEmpty(defaultDir) || !Directory.Exists(defaultDir))
                    defaultDir = Application.dataPath;
            }

            // 弹出保存文件面板，让用户自己决定保存位置
            string path = EditorUtility.SaveFilePanel(
                "保存 C# 类文件",
                defaultDir,
                defaultFileName,
                "cs");

            // 用户取消
            if (string.IsNullOrEmpty(path))
                return;

            try
            {
                // 确保目标目录存在
                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllText(path, _generatedCode, Encoding.UTF8);
                AssetDatabase.Refresh();

                // 若保存在工程内，转成相对路径用于展示
                string displayPath = path;
                if (path.StartsWith(Application.dataPath))
                    displayPath = "Assets" + path.Substring(Application.dataPath.Length);

                XGUI.dialog(
                       type: XGUIDialogType.警告,
                       windowtitle: "XBinder - Json解析到C#类",
                       title: "保存成功",
                       msg: $"已保存到:\n{displayPath}",
                       ok: "明白",
                       PrimaryIndex: 0,
                       usemodal: true,
                       themecolor: Color.white);

                XGUI_Utilitys.Console("XBinder 消息", $"[JsonToClassGenerator] 保存至: {displayPath}", XGUIMsgState.确认);
            }
            catch (Exception e)
            {
                XGUI.dialog(
                       type: XGUIDialogType.错误,
                       windowtitle: "XBinder - Json解析到C#类",
                       title: "保存失败",
                       msg: $"保存过程中发生错误：{e.Message}",
                       ok: "明白",
                       PrimaryIndex: 0,
                       usemodal: true,
                       themecolor: Color.white);

                XGUI_Utilitys.Console("XBinder 消息", $"[JsonToClassGenerator] 保存失败: {e}", XGUIMsgState.错误);
            }
        }
        /// <summary>
        /// 清理文件名中的非法字符
        /// </summary>
        /// <param name="name">原始文件名</param>
        /// <returns>合法文件名</returns>
        private static string SanitizeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }
        #endregion

        #region 绘制控件
        /// <summary>
        /// 通用方法：绘制开关。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>封装 <c>XGUI.layout_toggle</c> 的常用参数。</description></item>
        /// <item><description>支持自定义开/关背景色、手柄色与变更回调。</description></item>
        /// </list>
        /// </summary>
        /// <param name="title">开关标题。</param>
        /// <param name="prop">绑定的序列化属性。</param>
        /// <param name="width">标题宽度。</param>
        /// <param name="style">开关样式。</param>
        /// <param name="color_bg_on">开启时背景色。</param>
        /// <param name="color_bg_off">关闭时背景色。</param>
        /// <param name="color_on">开启时手柄色。</param>
        /// <param name="color_off">关闭时手柄色。</param>
        /// <param name="act_on_changed">值变更回调。</param>
        private void DrawToggle(string title, SerializedProperty prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(0, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: style,
                tog_padding: new RectOffset(5, 8, 0, 0),
                tog_margin: new RectOffset(0, 0, 0, 5),
                tog_mixed_options: new string[] { "禁用", "启用" },
                tog_mixed_text_size: XGUIFontSize.M,
                tog_mixed_text_color: Color.black,
                tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                tog_mixed_font_style: FontStyle.Normal,
                tog_bg_off_color: color_bg_off,
                tog_bg_on_color: color_bg_on,
                tog_handler_off_color: color_off,
                tog_handler_on_color: color_on,
                tog_mixed_bg_color_gui: XBinder_Dashboard.theme_primary,
                act_on_changed: act_on_changed);
        }
        #endregion
    }
    #endregion

    #region json 解析器
    /// <summary>
    /// 简易 Json 解析器，支持 object / array / string / number / bool / null
    /// </summary>
    public class SimpleJsonParser
    {
        #region 字段
        /// <summary>
        /// 待解析的 Json 文本
        /// </summary>
        private readonly string _text;
        /// <summary>
        /// 当前解析位置
        /// </summary>
        private int _pos;
        private readonly bool _enableFixStringifiedJson;
        #endregion

        #region 构造
        /// <summary>
        /// 构造解析器
        /// </summary>
        /// <param name="text">待解析的 Json 文本</param>
        public SimpleJsonParser(string text, bool enableFix = true)
        {
            _text = text;
            _enableFixStringifiedJson = enableFix;
        }
        #endregion

        #region 解析入口
        /// <summary>
        /// 解析 Json 文本并返回解析结果
        /// 结果类型: object -> Dictionary&lt;string, object&gt;，array -> List&lt;object&gt;，
        /// number -> double，string -> string，bool -> bool，null -> null
        /// </summary>
        /// <returns>解析后的根对象</returns>
        public object Parse()
        {
            _pos = 0;
            SkipWhitespace();
            object result = ParseValue();
            SkipWhitespace();
            if (_pos < _text.Length)
                throw new Exception($"解析完成后仍有未解析内容, 位置 {_pos}");
            return result;
        }
        #endregion

        #region 值解析
        /// <summary>
        /// 解析任意 Json 值
        /// </summary>
        /// <returns>解析后的值</returns>
        private object ParseValue()
        {
            SkipWhitespace();
            if (_pos >= _text.Length)
                throw new Exception("意外的 Json 结束");

            char c = _text[_pos];
            switch (c)
            {
                case '{': return ParseObject();
                case '[': return ParseArray();
                case '"':
                    string strValue = ParseString();
                    if (_enableFixStringifiedJson)
                        return TryParseStringifiedJson(strValue);
                    return strValue;
                case 't': Expect("true"); return true;
                case 'f': Expect("false"); return false;
                case 'n': Expect("null"); return null;
                default:
                    if (c == '-' || char.IsDigit(c)) return ParseNumber();
                    throw new Exception($"无法识别的字符 '{c}' 位置 {_pos}");
            }
        }
        /// <summary>
        /// 解析 Json 对象
        /// </summary>
        /// <returns>解析后的字典</returns>
        private Dictionary<string, object> ParseObject()
        {
            var dict = new Dictionary<string, object>();
            _pos++; // {
            SkipWhitespace();
            if (_pos < _text.Length && _text[_pos] == '}') { _pos++; return dict; }

            while (true)
            {
                SkipWhitespace();
                if (_text[_pos] != '"') throw new Exception($"对象键必须是字符串, 位置 {_pos}");
                string key = ParseString();
                SkipWhitespace();
                if (_text[_pos] != ':') throw new Exception($"缺少 ':', 位置 {_pos}");
                _pos++;
                dict[key] = ParseValue();
                SkipWhitespace();

                if (_text[_pos] == ',') { _pos++; continue; }
                if (_text[_pos] == '}') { _pos++; break; }
                throw new Exception($"对象解析错误, 位置 {_pos}");
            }
            return dict;
        }
        /// <summary>
        /// 解析 Json 数组
        /// </summary>
        /// <returns>解析后的列表</returns>
        private List<object> ParseArray()
        {
            var list = new List<object>();
            _pos++; // [
            SkipWhitespace();
            if (_pos < _text.Length && _text[_pos] == ']') { _pos++; return list; }

            while (true)
            {
                list.Add(ParseValue());
                SkipWhitespace();
                if (_text[_pos] == ',') { _pos++; continue; }
                if (_text[_pos] == ']') { _pos++; break; }
                throw new Exception($"数组解析错误, 位置 {_pos}");
            }
            return list;
        }
        /// <summary>
        /// 解析 Json 字符串
        /// </summary>
        /// <returns>解析后的字符串</returns>
        private string ParseString()
        {
            _pos++; // "
            var sb = new StringBuilder();
            while (_pos < _text.Length)
            {
                char c = _text[_pos++];
                if (c == '"') return sb.ToString();
                if (c == '\\')
                {
                    if (_pos >= _text.Length) break;
                    char e = _text[_pos++];
                    switch (e)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            string hex = _text.Substring(_pos, 4);
                            sb.Append((char)Convert.ToInt32(hex, 16));
                            _pos += 4;
                            break;
                        default: sb.Append(e); break;
                    }
                }
                else sb.Append(c);
            }
            throw new Exception("字符串未闭合");
        }
        /// <summary>
        /// 解析 Json 数字
        /// </summary>
        /// <returns>解析后的 double 值</returns>
        private object ParseNumber()
        {
            int start = _pos;
            if (_text[_pos] == '-') _pos++;
            bool hasDotOrExp = false;

            while (_pos < _text.Length && (char.IsDigit(_text[_pos]) || _text[_pos] == '.' ||
                   _text[_pos] == 'e' || _text[_pos] == 'E' || _text[_pos] == '+' || _text[_pos] == '-'))
            {
                if (_text[_pos] == '.' || _text[_pos] == 'e' || _text[_pos] == 'E')
                    hasDotOrExp = true;
                _pos++;
            }

            string s = _text.Substring(start, _pos - start);

            // ==========================================
            // 核心修复：整数优先返回 long，避免 double 精度丢失
            // ==========================================
            if (!hasDotOrExp)
            {
                if (long.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out long longValue))
                    return longValue;
            }

            return double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 期望匹配指定 token
        /// </summary>
        /// <param name="token">期望的 token</param>
        private void Expect(string token)
        {
            if (_pos + token.Length > _text.Length || _text.Substring(_pos, token.Length) != token)
                throw new Exception($"期望 '{token}', 位置 {_pos}");
            _pos += token.Length;
        }
        /// <summary>
        /// 跳过空白字符
        /// </summary>
        private void SkipWhitespace()
        {
            while (_pos < _text.Length && char.IsWhiteSpace(_text[_pos])) _pos++;
        }
        /// <summary>
        /// 尝试将字符串化的 JSON 还原为对象/数组
        /// </summary>
        /// <param name="str">可能是 JSON 的字符串</param>
        /// <returns>解析后的对象，如果失败则返回原字符串</returns>
        private object TryParseStringifiedJson(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            string trimmed = str.Trim();

            // 关键：如果外层多包了一层引号，先剥掉
            if (trimmed.StartsWith("\"") && trimmed.Length > 1)
                trimmed = trimmed.Substring(1);
            if (trimmed.EndsWith("\"") && trimmed.Length > 1)
                trimmed = trimmed.Substring(0, trimmed.Length - 1);

            trimmed = trimmed.Trim();

            bool isLikelyObject = trimmed.StartsWith("{") && trimmed.EndsWith("}");
            bool isLikelyArray = trimmed.StartsWith("[") && trimmed.EndsWith("]");

            if (!isLikelyObject && !isLikelyArray) return str;

            try
            {
                var nestedParser = new SimpleJsonParser(trimmed);
                object result = nestedParser.Parse();

                if (result is string s && s == str) return str;

                return result;
            }
            catch
            {
                return str;
            }
        }
        #endregion
    }
    #endregion

    #region 类代码生成器
    /// <summary>
    /// 根据解析后的 Json 结构生成 C# 类代码
    /// </summary>
    public class ClassGenerator
    {
        #region 字段
        /// <summary>
        /// 根类名
        /// </summary>
        private readonly string _rootClassName;
        /// <summary>
        /// 生成的命名空间
        /// </summary>
        private readonly string _namespaceName;
        /// <summary>
        /// 代码输出缓冲
        /// </summary>
        private readonly StringBuilder _sb = new StringBuilder();
        /// <summary>
        /// 已使用的类名集合（避免重名）
        /// </summary>
        private readonly HashSet<string> _usedClassNames = new HashSet<string>();
        /// <summary>
        /// 待处理的类集合（预留）
        /// </summary>
        private readonly List<string> _pendingClasses = new List<string>();
        private readonly bool _addSerializable;
        private readonly bool _handleMixedTypes;
        private readonly bool _useLongForLargeNumbers;
        private readonly bool _useFloatForDecimals;
        private readonly bool _warnOnEmptyArray;
        private readonly bool _singularizeArrayClassNames;
        private readonly int _maxClassNameLength;
        private readonly bool _smartBoolDetection;
        /// <summary>
        /// 类名前缀（如 "SectorEnergy_Entity_"），用于让所有嵌套类带上文件名标识
        /// </summary>
        private readonly string _classNamePrefix;
        #endregion

        #region 构造
        /// <summary>
        /// 构造代码生成器
        /// </summary>
        public ClassGenerator(
            string rootClassName,
            string namespaceName,
            bool addSerializable,
            bool handleMixedTypes,
            bool useLongForLargeNumbers,
            bool useFloatForDecimals,
            bool warnOnEmptyArray,
            bool singularizeArrayClassNames,
            int maxClassNameLength,
            bool smartBoolDetection,
            string classNamePrefix)
        {
            _rootClassName = string.IsNullOrWhiteSpace(rootClassName) ? "RootObject" : rootClassName;
            _namespaceName = namespaceName;

            // 赋值给新增的变量
            _addSerializable = addSerializable;
            _handleMixedTypes = handleMixedTypes;
            _useLongForLargeNumbers = useLongForLargeNumbers;
            _useFloatForDecimals = useFloatForDecimals;
            _warnOnEmptyArray = warnOnEmptyArray;
            _singularizeArrayClassNames = singularizeArrayClassNames;
            _maxClassNameLength = maxClassNameLength;
            _smartBoolDetection = smartBoolDetection;
            _classNamePrefix = classNamePrefix ?? string.Empty;
        }
        #endregion

        #region 生成入口
        /// <summary>
        /// 根据根对象生成完整的 C# 代码文本
        /// </summary>
        /// <param name="root">解析后的 Json 根对象</param>
        /// <returns>完整的 C# 代码</returns>
        public string Generate(object root)
        {
            _sb.Clear();
            _usedClassNames.Clear();
            _pendingClasses.Clear();

            //执行预扫描，收集所有字段的类型
            _fieldTypes.Clear();
            PreScan(root, _rootClassName);

            // 处理根对象
            string rootTypeName;
            if (root is Dictionary<string, object> rootDict)
            {
                // 根类名直接用 _rootClassName，不再走前缀拼接
                // （前缀仅作用于嵌套类，根类由调用方保证已是 "{文件名}_Entity"）
                rootTypeName = MakeUniqueClassName(_rootClassName, applyPrefix: false);
                BuildClass(rootTypeName, rootDict);
            }
            else
            {
                throw new Exception("根节点必须是 Json 对象 { ... }");
            }

            var header = new StringBuilder();
            header.AppendLine("/*");
            header.AppendLine(" * ============================================================================");
            header.AppendLine(" * ⚠  此代码文件由 XBinder 的 Json 转C#实体工具自动生成，在正式使用前请检查代码的类的完整性以及数据类型是否匹配 ⚠");
            header.AppendLine(" * ============================================================================");
            header.AppendLine(" * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.");
            header.AppendLine(" */");
            header.AppendLine();
            header.AppendLine("using System;");
            header.AppendLine("using System.Collections.Generic;");
            header.AppendLine("using UnityEngine;");
            header.AppendLine();
            if (!string.IsNullOrWhiteSpace(_namespaceName))
            {
                header.AppendLine($"namespace {_namespaceName}");
                header.AppendLine("{");
            }

            var body = new StringBuilder();
            body.AppendLine(_sb.ToString().TrimEnd());

            var footer = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(_namespaceName))
            {
                footer.AppendLine("}");
            }

            return header.ToString() + body + footer;
        }
        #endregion

        #region 类名管理
        /// <summary>
        /// 生成唯一的类名，避免重名
        /// </summary>
        /// <param name="baseName">基础类名</param>
        /// <param name="applyPrefix">是否拼接前缀（根类不拼，嵌套类拼）</param>
        /// <returns>唯一类名</returns>
        private string MakeUniqueClassName(string baseName, bool applyPrefix = true)
        {
            baseName = ToPascalCase(baseName);

            // 先截断，再拼前缀，避免前缀把长度撑爆
            if (baseName.Length > _maxClassNameLength)
                baseName = baseName.Substring(0, _maxClassNameLength);

            // 拼前缀（前缀本身不做 PascalCase，保持文件名原样，如 SectorEnergy_Entity_）
            if (applyPrefix && !string.IsNullOrEmpty(_classNamePrefix))
                baseName = _classNamePrefix + baseName;

            string name = baseName;
            int i = 1;
            while (_usedClassNames.Contains(name))
                name = baseName + i++;
            _usedClassNames.Add(name);
            return name;
        }
        #endregion

        #region 类结构生成
        /// <summary>
        /// 根据字段字典构建一个类的代码，并递归生成嵌套类
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="fields">字段字典</param>
        private void BuildClass(string className, Dictionary<string, object> fields)
        {
            var sb = new StringBuilder();
            if (_addSerializable)
                sb.AppendLine("[Serializable]");
            sb.AppendLine($"public class {className}");
            sb.AppendLine("{");

            var nestedClasses = new List<(string className, Dictionary<string, object> dict)>();

            // 混用检测的 key = 父类名 + "." + 字段名，只在本类内生效
            string fieldTypeKeyPrefix = className + ".";

            foreach (var kv in fields)
            {
                string fieldName = ToCamelCase(kv.Key);
                string baseName = ToPascalCase(kv.Key);

                // 检查该字段是否在本类内出现过不同类型
                // 过滤掉 null，防止因为空值导致被误判为混用类型
                if (_handleMixedTypes && _fieldTypes.TryGetValue(fieldTypeKeyPrefix + kv.Key, out var types))
                {
                    // 移除 null 后再判断
                    types.Remove("null");

                    if (types.Count > 1)
                    {
                        sb.AppendLine($"    public string {fieldName}; // ⚠ 类型混用，已回退为 string");
                        continue;
                    }
                }

                if (_singularizeArrayClassNames)
                {
                    if (baseName.EndsWith("List")) baseName = baseName.Substring(0, baseName.Length - 4);
                    else if (baseName.EndsWith("Array")) baseName = baseName.Substring(0, baseName.Length - 5);
                }

                TypeResult typeResult = ResolveType(kv.Value, baseName);

                // 空数组警告
                if (_warnOnEmptyArray && kv.Value is List<object> lst && lst.Count == 0)
                    sb.AppendLine("    // ⚠ 注意：JSON 中为空数组，请手动确认泛型类型");

                // null 值警告
                if (kv.Value == null && _smartBoolDetection) // 随便借用个开关，或者一直提示
                    sb.AppendLine("    // ⚠ 注意：JSON 中该字段全部为 null，请手动指定具体类型");

                sb.AppendLine($"    public {typeResult.TypeName} {fieldName};");

                if (typeResult.NestedClass != null)
                    nestedClasses.Add(typeResult.NestedClass.Value);
            }

            sb.AppendLine("}");
            _sb.AppendLine(sb.ToString());

            foreach (var n in nestedClasses)
                BuildClass(n.className, n.dict);
        }
        #endregion

        #region 类型解析
        /// <summary>
        /// 类型解析结果：类型名 + 需要额外生成的嵌套类（若有）
        /// </summary>
        private struct TypeResult
        {
            /// <summary>
            /// C# 类型名
            /// </summary>
            public string TypeName;
            /// <summary>
            /// 需要额外生成的嵌套类信息（类名 + 字段字典）
            /// </summary>
            public (string className, Dictionary<string, object> dict)? NestedClass;
        }
        /// <summary>
        /// 根据 Json 值推断 C# 类型；若值为对象或对象数组，同时返回需要生成的嵌套类信息
        /// </summary>
        private TypeResult ResolveType(object value, string suggestedClassName)
        {
            if (value == null)
                return new TypeResult { TypeName = "object" };

            if (_smartBoolDetection)
            {
                // 仅针对字符串形式的 "true" 或 "false" 进行智能识别
                // 不再去碰 long（数字 0/1），防止将状态码(status:1)、类型(type:0)误判为 bool
                if (value is string sBool)
                {
                    string lowerStr = sBool.ToLowerInvariant();
                    if (lowerStr == "true" || lowerStr == "false")
                    {
                        return new TypeResult { TypeName = "bool" };
                    }
                }
            }

            if (value is string)
                return new TypeResult { TypeName = "string" };

            if (value is bool)
                return new TypeResult { TypeName = "bool" };

            if (value is long l)
            {
                // 👇 修改：受 _useLongForLargeNumbers 开关控制
                if (!_useLongForLargeNumbers || (l >= int.MinValue && l <= int.MaxValue))
                    return new TypeResult { TypeName = "int" };
                return new TypeResult { TypeName = "long" };
            }

            if (value is double d)
            {
                if (d == Math.Floor(d) && d >= int.MinValue && d <= int.MaxValue)
                    return new TypeResult { TypeName = "int" };
                if (d == Math.Floor(d) && d >= long.MinValue && d <= long.MaxValue)
                    return new TypeResult { TypeName = "long" };

                // 👇 修改：受 _useFloatForDecimals 开关控制
                return new TypeResult { TypeName = _useFloatForDecimals ? "float" : "double" };
            }

            // 对象 -> 生成嵌套类
            if (value is Dictionary<string, object> dict)
            {
                string className = MakeUniqueClassName(suggestedClassName);
                return new TypeResult
                {
                    TypeName = className,
                    NestedClass = (className, dict)
                };
            }

            // 数组 -> List<T>
            if (value is List<object> list)
            {
                if (list.Count == 0)
                    return new TypeResult { TypeName = "List<object>" };

                object first = list[0];

                if (first is Dictionary<string, object> firstDict)
                {
                    string className = MakeUniqueClassName(suggestedClassName);
                    return new TypeResult
                    {
                        TypeName = $"List<{className}>",
                        NestedClass = (className, firstDict)
                    };
                }

                if (first is string) return new TypeResult { TypeName = "List<string>" };
                if (first is bool) return new TypeResult { TypeName = "List<bool>" };

                if (first is long firstLong)
                {
                    if (!_useLongForLargeNumbers || (firstLong >= int.MinValue && firstLong <= int.MaxValue))
                        return new TypeResult { TypeName = "List<int>" };
                    return new TypeResult { TypeName = "List<long>" };
                }

                if (first is double dl)
                {
                    if (dl == Math.Floor(dl) && dl >= int.MinValue && dl <= int.MaxValue)
                        return new TypeResult { TypeName = "List<int>" };
                    return new TypeResult { TypeName = _useFloatForDecimals ? "List<float>" : "List<double>" };
                }

                return new TypeResult { TypeName = "List<object>" };
            }

            return new TypeResult { TypeName = "object" };
        }
        #endregion

        #region 命名转换
        /// <summary>
        /// 将任意字符串转换为 PascalCase 类名
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>PascalCase 字符串</returns>
        private static string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return "Class";
            string[] parts = Regex.Split(input, @"[^a-zA-Z0-9]+|(?<=[a-z])(?=[A-Z])");
            var sb = new StringBuilder();
            foreach (var p in parts)
            {
                if (string.IsNullOrEmpty(p)) continue;
                sb.Append(char.ToUpper(p[0]));
                if (p.Length > 1) sb.Append(p.Substring(1));
            }
            string result = sb.ToString();
            if (result.Length == 0) return "Class";
            if (char.IsDigit(result[0])) result = "_" + result;
            return result;
        }
        /// <summary>
        /// 将任意字符串转换为 camelCase 字段名
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>camelCase 字符串</returns>
        private static string ToCamelCase(string input)
        {
            string pascal = ToPascalCase(input);
            if (string.IsNullOrEmpty(pascal)) return "field";
            // 简单处理：第一个字符转小写
            char[] arr = pascal.ToCharArray();
            arr[0] = char.ToLower(arr[0]);
            return new string(arr);
        }
        #endregion

        private Dictionary<string, HashSet<string>> _fieldTypes = new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// 预扫描：收集每个类下每个字段出现过的类型
        /// key = 父类名 + "." + 字段名，避免跨类误判
        /// </summary>
        private void PreScan(object node, string parentClassName)
        {
            if (node is Dictionary<string, object> dict)
            {
                foreach (var kv in dict)
                {
                    string type = InferTypeName(kv.Value);
                    string key = parentClassName + "." + kv.Key;
                    if (!_fieldTypes.TryGetValue(key, out var set))
                        _fieldTypes[key] = set = new HashSet<string>();
                    set.Add(type);

                    // 递归：若值是对象或对象数组，父类名取对应嵌套类的建议类名
                    if (kv.Value is Dictionary<string, object>)
                    {
                        PreScan(kv.Value, parentClassName + "_" + ToPascalCase(kv.Key));
                    }
                    else if (kv.Value is List<object> list)
                    {
                        foreach (var item in list)
                        {
                            if (item is Dictionary<string, object>)
                                PreScan(item, parentClassName + "_" + ToPascalCase(kv.Key));
                        }
                    }
                }
            }
            else if (node is List<object> list)
            {
                foreach (var item in list) PreScan(item, parentClassName);
            }
        }

        private string InferTypeName(object v)
        {
            if (v == null) return "null";
            if (v is bool) return "bool";
            if (v is string) return "string";

            // 核心修改：将 long 和 double 统一视为 "number"
            if (v is long || v is double) return "number";

            if (v is List<object>) return "array";
            if (v is Dictionary<string, object>) return "object";
            return "unknown";
        }
    }
    #endregion
}