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
        [SerializeField] private bool _showAdvancedOptions = false;

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

        // 序列化属性
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
        private SerializedProperty _showAdvancedOptionsProp;

        #region 窗口入口
        /// <summary>
        /// 打开窗口的菜单入口
        /// </summary>
        [MenuItem("Tools/XBinder/根据 Json 内容解析到 C# 实体类")]
        public static void ShowWindow()
        {
            var window = GetWindow<Editor_XBinder_JsonToEntity>("Json 内容解析到 C# 实体类工具");
            window.minSize = new Vector2(400, 650);
            window.maxSize = window.minSize;
        }
        #endregion

        #region 生命周期
        /// <summary>
        /// 窗口启用时初始化序列化对象和属性
        /// </summary>
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _jsonFileProp = _serializedObject.FindProperty(nameof(_jsonFile));
            _rootClassNameProp = _serializedObject.FindProperty(nameof(_rootClassName));
            _namespaceNameProp = _serializedObject.FindProperty(nameof(_namespaceName));
            _outputFolderProp = _serializedObject.FindProperty(nameof(_outputFolder));
            _autoGenerateFileNameProp = _serializedObject.FindProperty(nameof(_autoGenerateFileName));

            _showAdvancedOptionsProp = _serializedObject.FindProperty(nameof(_showAdvancedOptions));
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

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Json 转 C# 类实体结构生成工具", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "拖入 .json 文件到窗口内，或通过选择框指定文件；点击 [生成 C# 代码] 预览，然后点击 [保存为 .cs 文件] 导出。",
                MessageType.Info);

            EditorGUILayout.Space(5);

            // ---- 配置区 ----
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(_jsonFileProp, new GUIContent("Json 文件"));
            EditorGUILayout.PropertyField(_rootClassNameProp, new GUIContent("根类名"));
            EditorGUILayout.PropertyField(_namespaceNameProp, new GUIContent("命名空间 (可空)"));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_outputFolderProp, new GUIContent("输出文件夹"));
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string selected = EditorUtility.OpenFolderPanel("选择输出文件夹", "Assets", "");
                if (!string.IsNullOrEmpty(selected))
                {
                    // 转成相对工程路径
                    if (selected.StartsWith(Application.dataPath))
                        _outputFolderProp.stringValue = "Assets" + selected.Substring(Application.dataPath.Length);
                    else
                        _outputFolderProp.stringValue = selected;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(_autoGenerateFileNameProp, new GUIContent("自动根据根类名生成文件名"));

            EditorGUILayout.Space(5);
            _showAdvancedOptions = EditorGUILayout.Foldout(_showAdvancedOptions, "高级修复选项", true, EditorStyles.foldoutHeader);
            if (_showAdvancedOptions)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("JSON 修复", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_fixStringifiedJsonProp, new GUIContent("自动修复嵌套字符串 JSON"));
                EditorGUILayout.PropertyField(_cleanCommentsAndTrailingCommasProp, new GUIContent("清理注释与尾随逗号"));
                EditorGUILayout.PropertyField(_handleMixedTypesProp, new GUIContent("混用类型回退为 string"));
                EditorGUILayout.PropertyField(_smartBoolDetectionProp, new GUIContent("智能识别布尔值 (0/1, \"true\")"));

                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("类型推断", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_useLongForLargeNumbersProp, new GUIContent("超长数字自动转 long"));
                EditorGUILayout.PropertyField(_useFloatForDecimalsProp, new GUIContent("小数自动转 float"));
                EditorGUILayout.PropertyField(_warnOnEmptyArrayProp, new GUIContent("空数组附加警告注释"));

                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("代码生成", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_addSerializableAttributeProp, new GUIContent("自动添加 [Serializable] 与 using"));
                EditorGUILayout.PropertyField(_singularizeArrayClassNamesProp, new GUIContent("数组字段类名单数化"));
                EditorGUILayout.PropertyField(_maxClassNameLengthProp, new GUIContent("类名最大长度"));

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

            // ---- 文件内容状态提示 ----
            if (_jsonFile == null)
            {
                EditorGUILayout.HelpBox("尚未选择 Json 文件。可将 .json 文件拖入本窗口，或使用上方的选择框指定。", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox($"已选择: {AssetDatabase.GetAssetPath(_jsonFile)}\n大小: {_jsonFile.text.Length} 字符", MessageType.None);
            }

            EditorGUILayout.Space(5);

            // ---- 按钮区 ----
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color(0.4f, 0.85f, 0.5f);
            if (GUILayout.Button("生成 C# 代码", GUILayout.Height(30)))
            {
                GenerateCode();
            }
            GUI.backgroundColor = Color.white;

            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(_generatedCode)))
            {
                if (GUILayout.Button("保存为 .cs 文件", GUILayout.Height(30), GUILayout.Width(160)))
                {
                    SaveToFile();
                }
            }

            if (GUILayout.Button("清空", GUILayout.Height(30), GUILayout.Width(60)))
            {
                _generatedCode = "";
                _jsonFile = null;
                _serializedObject.Update();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // ---- 输出预览区 ----
            if (!string.IsNullOrEmpty(_generatedCode))
            {
                EditorGUILayout.LabelField("生成结果预览:", EditorStyles.boldLabel);
                _outputScroll = EditorGUILayout.BeginScrollView(_outputScroll, GUILayout.ExpandHeight(true));
                EditorGUILayout.TextArea(_generatedCode, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
            }

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