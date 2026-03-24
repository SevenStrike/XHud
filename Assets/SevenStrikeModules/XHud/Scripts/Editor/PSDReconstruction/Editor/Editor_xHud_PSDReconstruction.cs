namespace SevenStrikeModules.XHud
{
    using Newtonsoft.Json;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Hud;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using TMPro;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.TextCore.LowLevel;
    using Application = UnityEngine.Application;
    using Color = UnityEngine.Color;
    using Debug = UnityEngine.Debug;
    using Directory = System.IO.Directory;
    using File = System.IO.File;
    using Font = UnityEngine.Font;
    using Image = UnityEngine.UI.Image;
    using Object = UnityEngine.Object;
    using Path = System.IO.Path;
    using TextAsset = UnityEngine.TextAsset;

    [System.Serializable]
    public class xHud_PSDR_FontDataComparer : IEqualityComparer<xHud_PSDR_FontData>
    {
        public bool Equals(xHud_PSDR_FontData x, xHud_PSDR_FontData y)
        {
            if (x == null || y == null)
                return false;

            return x.Name == y.Name && x.Family == y.Family && x.Style == y.Style;
        }

        public int GetHashCode(xHud_PSDR_FontData obj)
        {
            if (obj == null)
                return 0;

            int hash = 17;
            hash = hash * 23 + (obj.Name?.GetHashCode() ?? 0);
            hash = hash * 23 + (obj.Family?.GetHashCode() ?? 0);
            hash = hash * 23 + (obj.Style?.GetHashCode() ?? 0);
            return hash;
        }
    }

    [System.Serializable]
    public class xHud_PSDR_FontData
    {
        public string Name;
        public string Family;
        public string Style;
        public string Path;
        public Font Font;
        public TMP_FontAsset FontAsset;
        public string Content;
    }

    [System.Serializable]
    public class xHud_PSDR_FontsFontPathInfo
    {
        public string FileName;
        public string Path;
        public string RelativePath;
        public string Extension;
    }

    [System.Serializable]
    public class xHud_PSDR_SystemFontData
    {
        public string Name;
        public string Path;
    }

    public enum xHud_PSDR__UseDebug
    {
        禁用,
        启用
    }

    public enum xHud_PSDR_PureSprite
    {
        原图模式,
        纯净模式
    }

    /// <summary>
    /// 处理XHudExporter导出的图元数据为UGUI的UI元素
    /// </summary>
    public class Editor_xHud_PSDReconstruction : EditorWindow
    {
        static Editor_xHud_PSDReconstruction Window;

        private SerializedObject SerializedObject;

        [SerializeField]
        public TextAsset LayersData;

        [SerializeField]
        public Object LayersFolder;

        private PSDR_Root LayerStructure;

        [SerializeField]
        private xHud_PSDR_TextLayerMode TextLayerMode;

        [SerializeField]
        private xHud_PSDR_TextLayerTypes TextLayerType;

#pragma warning disable 0414
        [SerializeField]
        xHud_PSDR__UseDebug DebugMode = xHud_PSDR__UseDebug.启用;
#pragma warning restore 0414

#pragma warning disable 0414
        [SerializeField]
        xHud_PSDR_PureSprite PureSprite = xHud_PSDR_PureSprite.原图模式;
#pragma warning restore 0414

        #region 序列化属性
        private SerializedProperty sp_LayersData, sp_LayersFolder, sp_LayoutMode, sp_TextLayerMode, sp_TextLayerType, sp_DebugMode, sp_PureSprite;
        #endregion

        private string LayerDataPath;

        public xHud_PSDR_FontData[] LayerFontDatas;

        public xHud_PSDR_FontsFontPathInfo[] FontsFontPathInfos;

        private int AddedSpace_Component_Text = 0;

        #region 图标
        private Texture2D Icon_sprites, Icon_debug, Icon_consmode, Icon_textmode, Icon_texttype, Icon_json;
        #endregion

        // 文件夹路径
        public string folderPath;
        // 存储提取的主颜色
        public List<Color> dominantColors = new List<Color>();
        // 颜色相似度阈值（0到1之间）
        public float colorThreshold = 0.0001f;

        [MenuItem("Tools/XHud/PSD Reconstruction #c")]
        static void Init()
        {
            Window = (Editor_xHud_PSDReconstruction)EditorWindow.GetWindow(typeof(Editor_xHud_PSDReconstruction), true, "XHud PSD Reconstruction", true);
            Window.minSize = new Vector2(450, 490);
            Window.maxSize = Window.minSize;
            Window.Show();
        }

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            SerializedObject = new SerializedObject(this);

            #region 获取序列化属性
            sp_LayersData = SerializedObject.FindProperty("LayersData");
            sp_LayersFolder = SerializedObject.FindProperty("LayersFolder");
            sp_TextLayerMode = SerializedObject.FindProperty("TextLayerMode");
            sp_TextLayerType = SerializedObject.FindProperty("TextLayerType");
            sp_DebugMode = SerializedObject.FindProperty("DebugMode");
            sp_PureSprite = SerializedObject.FindProperty("PureSprite");
            #endregion

            #region 获取图标
            Icon_sprites = Editor_xHudGUI.GetIcon("Icons_Hud_Reconstruction/Icon_sprites");
            Icon_debug = Editor_xHudGUI.GetIcon("Icons_Hud_Reconstruction/Icon_debug");
            Icon_json = Editor_xHudGUI.GetIcon("Icons_Hud_Reconstruction/Icon_json");
            Icon_consmode = Editor_xHudGUI.GetIcon("Icons_Hud_Reconstruction/Icon_consmode");
            Icon_textmode = Editor_xHudGUI.GetIcon("Icons_Hud_Reconstruction/Icon_textmode");
            Icon_texttype = Editor_xHudGUI.GetIcon("Icons_Hud_Reconstruction/Icon_texttype");
            #endregion           
        }

        private void OnGUI()
        {
            SerializedObject.Update();

            #region 必要资源
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 10, "必要资源", xHud_Dashboard.Theme_Primary);

            #region 图层数据     
            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, Color.white, 0);
            Editor_xHudGUI.Gui_Layout_Space(10);
            EditorGUI.BeginChangeCheck();
            Editor_xHudGUI.Gui_Layout_Property_Field_WithIcon("图元数据", Icon_json, 15, new Vector2(0, 0), ref sp_LayersData, 100);
            if (EditorGUI.EndChangeCheck())
            {
                AssignLayerData(sp_LayersData.objectReferenceValue);
            }
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(10);

            #region 图层目录     
            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, Color.white, 0);
            Editor_xHudGUI.Gui_Layout_Space(10);
            EditorGUI.BeginChangeCheck();
            Editor_xHudGUI.Gui_Layout_Property_Field_WithIcon("图元目录", Icon_sprites, 15, new Vector2(0, 0), ref sp_LayersFolder, 100);
            if (EditorGUI.EndChangeCheck())
            {
                AssignLayerFolder(sp_LayersFolder.objectReferenceValue);
                return;
            }
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(10);

            #region 图层目录     
            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, Color.white, 0);
            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Property_Field_WithIcon("图元色彩模式", Icon_sprites, 15, new Vector2(0, 0), ref sp_PureSprite, 100);
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(10);

            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, Color.white, 0);
            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Property_Field_WithIcon("调试信息", Icon_debug, 15, new Vector2(0, 0), ref sp_DebugMode, 100);
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();

            Editor_xHudGUI.Gui_Layout_Space(10);

            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, Color.white, 0);
            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Property_Field_WithIcon("文字形式", Icon_textmode, 15, new Vector2(0, 0), ref sp_TextLayerMode, 100);
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();

            if ((xHud_PSDR_TextLayerMode)sp_TextLayerMode.enumValueIndex == xHud_PSDR_TextLayerMode.文字组件化)
            {
                Editor_xHudGUI.Gui_Layout_Space(10);

                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, Color.white, 0);
                Editor_xHudGUI.Gui_Layout_Space(10);
                EditorGUI.BeginChangeCheck();
                Editor_xHudGUI.Gui_Layout_Property_Field_WithIcon("文字组件", Icon_texttype, 15, new Vector2(0, 0), ref sp_TextLayerType, 100);
                if (EditorGUI.EndChangeCheck())
                {
                    xHud_PSDR_TextLayerTypes types = (xHud_PSDR_TextLayerTypes)sp_TextLayerType.enumValueIndex;

                    if (types == xHud_PSDR_TextLayerTypes.HudTmpText)
                    {
                        if (LayerFontDatas != null && LayerFontDatas.Length > 0)
                        {
                            string path_font = LayerDataPath + "/Fonts";

                            ///展示图层数据包含的字体列表
                            for (int i = 0; i < LayerFontDatas.Length; i++)
                            {
                                if (LayerFontDatas[i].Font != null)
                                {
                                    string extension_tmp = ".asset";
                                    string path_tmpfont_folder = path_font + "/" + LayerFontDatas[i].Name + " SDF" + extension_tmp;
                                    if (AssetDatabase.AssetPathExists(path_tmpfont_folder))
                                    {
                                        LayerFontDatas[i].FontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path_tmpfont_folder);
                                    }
                                    else
                                    {
                                        LayerFontDatas[i].FontAsset = CreateFontAsset(LayerFontDatas[i].Font, " SDF", 1024, 1024, LayerFontDatas[i].Content);
                                    }
                                }
                            }
                        }
                    }
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();

                AddedSpace_Component_Text = 30;
            }
            else
            {
                AddedSpace_Component_Text = 0;
            }

            Editor_xHudGUI.Gui_Layout_Vertical_End(10);
            #endregion

            #region 字体列表
            if ((xHud_PSDR_TextLayerMode)sp_TextLayerMode.enumValueIndex == xHud_PSDR_TextLayerMode.文字组件化)
            {
                if (sp_LayersData.objectReferenceValue != null)
                {
                    Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 10, "字体列表", xHud_Dashboard.Theme_Primary);

                    if (LayerFontDatas != null && LayerFontDatas.Length > 0)
                    {
                        ///展示图层数据包含的字体列表
                        for (int i = 0; i < LayerFontDatas.Length; i++)
                        {
                            SerializedProperty prop_fontdata = SerializedObject.FindProperty("LayerFontDatas");
                            prop_fontdata.serializedObject.Update();

                            SerializedProperty prop_root = prop_fontdata.GetArrayElementAtIndex(i);
                            SerializedProperty prop_Name = prop_root.FindPropertyRelative("Name");
                            SerializedProperty prop_Family = prop_root.FindPropertyRelative("Family");
                            SerializedProperty prop_Style = prop_root.FindPropertyRelative("Style");
                            SerializedProperty prop_Font = prop_root.FindPropertyRelative("Font");
                            SerializedProperty prop_FontAsset = prop_root.FindPropertyRelative("FontAsset");

                            #region 字体列表     
                            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, Color.white, 0);
                            Editor_xHudGUI.Gui_Layout_Space(10);
                            Editor_xHudGUI.Gui_Layout_Labelfield("字体： ", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft);
                            Editor_xHudGUI.Gui_Layout_Space(0);
                            Editor_xHudGUI.Gui_Layout_Labelfield(prop_Family.stringValue, HudFilled.无, HudColor.无, new Color(1, 1, 1, 0.75f), TextAnchor.MiddleLeft);
                            Editor_xHudGUI.Gui_Layout_FlexSpace();
                            Editor_xHudGUI.Gui_Layout_Labelfield(prop_Name.stringValue, HudFilled.无, HudColor.无, xHud_Dashboard.Theme_Primary, TextAnchor.MiddleRight);
                            Editor_xHudGUI.Gui_Layout_Labelfield($"( {prop_Style.stringValue} )", HudFilled.无, HudColor.无, Color.yellow, TextAnchor.MiddleRight);
                            Editor_xHudGUI.Gui_Layout_Horizontal_End();

                            xHud_PSDR_TextLayerMode TextPixel = (xHud_PSDR_TextLayerMode)sp_TextLayerMode.enumValueIndex;

                            xHud_PSDR_TextLayerTypes TextTypes = (xHud_PSDR_TextLayerTypes)sp_TextLayerType.enumValueIndex;

                            if (TextTypes == xHud_PSDR_TextLayerTypes.HudText)
                            {
                                EditorGUI.BeginChangeCheck();
                                if (prop_Font.objectReferenceValue == null)
                                    GUI.backgroundColor = Color.red;
                                string title = "字体文件";
                                if (prop_Font.objectReferenceValue == null)
                                    title = "请指定字体文件";
                                Editor_xHudGUI.Gui_Layout_Property_Field(title, prop_Font, 70);
                                GUI.backgroundColor = Color.white;
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (prop_Font.objectReferenceValue != null)
                                    {
                                        string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud PSD Reconstruction消息", "指定字体", $"确认指定 {prop_Name.stringValue} 字体资源吗？指定后会自动更正字体资源名称！", "指定", "暂不", 1);
                                        if (res == "指定")
                                        {
                                            RenameFontAsset(prop_Font.objectReferenceValue, prop_Name.stringValue);
                                        }
                                        else
                                        {
                                            prop_Font.objectReferenceValue = null;
                                            prop_Font.serializedObject.ApplyModifiedProperties();
                                        }
                                    }
                                }
                            }
                            else if (TextTypes == xHud_PSDR_TextLayerTypes.HudTmpText)
                            {
                                EditorGUI.BeginChangeCheck();
                                if (prop_Font.objectReferenceValue == null)
                                    GUI.backgroundColor = Color.red;
                                string title = "字体文件";
                                if (prop_Font.objectReferenceValue == null)
                                    title = "请指定字体文件";
                                Editor_xHudGUI.Gui_Layout_Property_Field(title, prop_FontAsset, 70);
                                GUI.backgroundColor = Color.white;
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (prop_FontAsset.objectReferenceValue != null)
                                    {
                                        string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud PSD Reconstruction消息", "指定字体", $"确认指定 {prop_Name.stringValue} Tmp字体资源吗？指定后会自动更正字体资源名称！", "指定", "暂不", 1);
                                        if (res == "指定")
                                        {
                                            RenameFontAsset(prop_FontAsset.objectReferenceValue, prop_Name.stringValue);
                                        }
                                        else
                                        {
                                            prop_FontAsset.objectReferenceValue = null;
                                            prop_FontAsset.serializedObject.ApplyModifiedProperties();
                                        }
                                    }
                                }
                            }

                            Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);

                            #endregion


                            prop_fontdata.serializedObject.ApplyModifiedProperties();
                        }
                    }

                    Editor_xHudGUI.Gui_Layout_Vertical_End();

                    Window.minSize = new Vector2(450, 490 + (LayerFontDatas.Length * 71.16f) + 45 + AddedSpace_Component_Text);
                    Window.maxSize = Window.minSize;
                }
                else
                {
                    Window.minSize = new Vector2(450, 490 + AddedSpace_Component_Text);
                    Window.maxSize = Window.minSize;
                }
            }
            else
            {
                Window.minSize = new Vector2(450, 490);
                Window.maxSize = Window.minSize;
            }
            #endregion

            #region 信息预览
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 10, "信息预览", xHud_Dashboard.Theme_Primary);
            Rect rect = GUILayoutUtility.GetLastRect();

            Editor_xHudGUI.Gui_Layout_Space(60);

            if (sp_LayersData.objectReferenceValue == null)
            {
                Editor_xHudGUI.Gui_Layout_Labelfield("暂无数据信息", HudFilled.无, HudColor.无, Editor_xHudGUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter, 12);
            }
            else
            {
                Editor_xHudGUI.Gui_Layout_Space(24);

                Rect baserect = new Rect(rect.x, rect.y, 330, 100);

                float offset = 12;
                float baseheight = 10;

                #region Name
                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 10, baserect.y + offset + baseheight, 60, 25), "原始名称：",
                    HudFilled.无, HudColor.无,
                    xHud_Dashboard.Theme_Primary,
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);

                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 70, baserect.y + offset + baseheight, 60, 25), LayerStructure.structure.name,
                    HudFilled.无, HudColor.无,
                    new Color(1, 1, 1, 0.8f),
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);
                #endregion

                #region Res
                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 10, baserect.y + offset + (baseheight * 4), 60, 25), "分辨率：",
                    HudFilled.无, HudColor.无,
                    xHud_Dashboard.Theme_Primary,
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);

                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 70, baserect.y + offset + (baseheight * 4), 60, 25), LayerStructure.structure.size,
                    HudFilled.无, HudColor.无,
                    new Color(1, 1, 1, 0.8f),
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);
                #endregion

                #region Layers
                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 10, baserect.y + offset + (baseheight * 7), 60, 25), "图层数：",
                    HudFilled.无, HudColor.无,
                    xHud_Dashboard.Theme_Primary,
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);

                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 70, baserect.y + offset + (baseheight * 7), 60, 25), LayerStructure.structure.count_layer.ToString(),
                    HudFilled.无, HudColor.无,
                    new Color(1, 1, 1, 0.8f),
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);
                #endregion

                #region Group
                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 10, baserect.y + offset + (baseheight * 10), 60, 25), "编组数：",
                    HudFilled.无, HudColor.无,
                    xHud_Dashboard.Theme_Primary,
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);

                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 70, baserect.y + offset + (baseheight * 10), 60, 25), LayerStructure.structure.count_group.ToString(),
                    HudFilled.无, HudColor.无,
                    new Color(1, 1, 1, 0.8f),
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);
                #endregion

                #region Dpi
                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 200, baserect.y + offset + baseheight, 60, 25), "像素：",
                    HudFilled.无, HudColor.无,
                    xHud_Dashboard.Theme_Primary,
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);

                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 260, baserect.y + offset + baseheight, 60, 25), LayerStructure.structure.dpi.ToString(),
                    HudFilled.无, HudColor.无,
                    new Color(1, 1, 1, 0.8f),
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);
                #endregion

                #region ColorMode
                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 200, baserect.y + offset + (baseheight * 4), 60, 25), "颜色模式：",
                    HudFilled.无, HudColor.无,
                    xHud_Dashboard.Theme_Primary,
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);

                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 260, baserect.y + offset + (baseheight * 4), 60, 25), LayerStructure.structure.colormode,
                    HudFilled.无, HudColor.无,
                    new Color(1, 1, 1, 0.8f),
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);
                #endregion

                #region Date
                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 200, baserect.y + offset + (baseheight * 7), 60, 25), "日期：",
                    HudFilled.无, HudColor.无,
                    xHud_Dashboard.Theme_Primary,
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);

                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 260, baserect.y + offset + (baseheight * 7), 60, 25), LayerStructure.structure.datetime.date,
                    HudFilled.无, HudColor.无,
                    new Color(1, 1, 1, 0.8f),
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);
                #endregion

                #region Time
                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 200, baserect.y + offset + (baseheight * 10), 60, 25), "时间：",
                    HudFilled.无, HudColor.无,
                    xHud_Dashboard.Theme_Primary,
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);

                Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(baserect.x + 260, baserect.y + offset + (baseheight * 10), 60, 25), LayerStructure.structure.datetime.time,
                    HudFilled.无, HudColor.无,
                    new Color(1, 1, 1, 0.8f),
                    TextAnchor.MiddleLeft,
                    new Vector2(0, 0),
                    11);
                #endregion

            }

            Editor_xHudGUI.Gui_Layout_Space(60);

            Editor_xHudGUI.Gui_Layout_Vertical_End(10);
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(10);

            #region 初始化结构
            if (Application.isPlaying)
                GUI.enabled = false;
            else
                GUI.enabled = true;
            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, Color.white, 0);
            Editor_xHudGUI.Gui_Layout_Space(10);
            if ((xHud_PSDR_TextLayerMode)sp_TextLayerMode.enumValueIndex == xHud_PSDR_TextLayerMode.文字组件化)
            {
                if (Editor_xHudGUI.Gui_Layout_Button("查看字体库", "", HudFilled.实体, HudColor.深空灰, Color.white, 35, new RectOffset(), new Vector2(0, 0)))
                {
                    OpenFontLib();
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
            }
            if (Editor_xHudGUI.Gui_Layout_Button("重建图元结构", "", HudFilled.实体, HudColor.深空灰, Color.white, 35, new RectOffset(), new Vector2(0, 0)))
            {
                ReConstructure(sp_LayersFolder.objectReferenceValue);
            }
            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();
            GUI.enabled = true;
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(10);

            SerializedObject.ApplyModifiedProperties();
        }

        // 判断颜色是否与已有的颜色相近
        private bool IsColorSimilar(Color color)
        {
            foreach (Color existingColor in dominantColors)
            {
                if (ColorDistance(color, existingColor) < colorThreshold)
                {
                    // 颜色相近，视为重复
                    return true;
                }
            }
            // 没有相近的颜色
            return false;
        }

        // 计算两个颜色之间的欧几里得距离
        private float ColorDistance(Color c1, Color c2)
        {
            return Mathf.Sqrt(
                Mathf.Pow(c1.r - c2.r, 2) +
                Mathf.Pow(c1.g - c2.g, 2) +
                Mathf.Pow(c1.b - c2.b, 2)
            );
        }

        // 获取图片的主颜色
        private Color GetTextureColor(string filePath)
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2); // 创建一个临时纹理
            texture.LoadImage(fileData); // 加载图片数据到纹理

            // 获取纹理的像素数据
            Color[] pixels = texture.GetPixels();

            // 统计每种颜色的出现次数
            Dictionary<Color, int> colorCount = new Dictionary<Color, int>();
            foreach (Color pixel in pixels)
            {
                Color keyColor = new Color(pixel.r, pixel.g, pixel.b, 1.0f); // 忽略透明度
                if (colorCount.ContainsKey(keyColor))
                {
                    colorCount[keyColor]++;
                }
                else
                {
                    colorCount[keyColor] = 1;
                }
            }

            // 找到出现次数最多的颜色
            Color dominantColor = Color.clear;
            int maxCount = 0;
            foreach (var pair in colorCount)
            {
                if (pair.Value > maxCount)
                {
                    dominantColor = pair.Key;
                    maxCount = pair.Value;
                }
            }

            return dominantColor;
        }

        /// <summary>
        /// 指定图层文件夹目录
        /// </summary>
        private void AssignLayerFolder(Object folder)
        {
            if (folder == null)
                return;
            if ((xHud_PSDR__UseDebug)sp_DebugMode.enumValueIndex == xHud_PSDR__UseDebug.启用)
                xHud_Utilitys.Func_PrintInfo("XHud PSD Reconstruction 通知", "已指定图层文件夹目录： " + folder.name, HudMsgState.确认);

            string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud PSD Reconstruction消息", "提取图层色卡", "是否需要为您根据所有图元来创建一个颜色库作为备用资源？", "创建", "暂不", 1);
            if (res == "创建")
            {
                CreateColorLibWithSprites(folder);
            }
        }

        /// <summary>
        /// 获取所有图元的颜色并制成颜色库备用
        /// </summary>
        /// <param name="folder"></param>
        private void CreateColorLibWithSprites(Object folder)
        {
            string paths = AssetDatabase.GetAssetPath(folder);
            string libpathroot = Path.GetDirectoryName(paths);
            string libname = Path.GetFileName(libpathroot);

            //创建颜色库
            xHud_Library_Colors lib_color = ScriptableObject.CreateInstance<xHud_Library_Colors>();
            lib_color.LibraryName = libname;

            // 获取文件夹下所有图片文件
            string[] imageFiles = Directory.GetFiles(paths, "*.*", SearchOption.TopDirectoryOnly);

            List<string> validExtensions = new List<string> { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };

            // 使用 HashSet 来排除重复颜色
            HashSet<Color> uniqueColors = new HashSet<Color>();

            foreach (string file in imageFiles)
            {
                string filename = Path.GetFileNameWithoutExtension(file);

                /// 判断该文件是否是Text，如果是就是用文字颜色
                bool istext = false;
                Color textcolor = Color.white;
                for (int i = 0; i < LayerStructure.structure.layers.Count; i++)
                {
                    string idname = $"{LayerStructure.structure.layers[i].id}_{LayerStructure.structure.layers[i].type.ToString().ToLower()}_{LayerStructure.structure.layers[i].name}";
                    if (idname == filename)
                    {
                        istext = true;
                        PSDR_Ft_color_rgba rgba = LayerStructure.structure.layers[i].ft_color_rgba;
                        textcolor = xHud_Utilitys.Color_From_RGBA(rgba.r, rgba.g, rgba.b, rgba.a);
                        break;
                    }
                }

                string extension = Path.GetExtension(file).ToLower();
                if (validExtensions.Contains(extension))
                {
                    Color dominantColor = Color.white;

                    if (istext)
                        dominantColor = textcolor;
                    else
                        dominantColor = GetTextureColor(file);

                    if (!IsColorSimilar(dominantColor))
                    {
                        dominantColors.Add(dominantColor);
                        lib_color.ColorsLibrary_AddColor($"{libname}_{Path.GetFileNameWithoutExtension(file)}", dominantColor, $"{libname}的颜色说明");
                    }
                }
            }

            AssetDatabase.CreateAsset(lib_color, Path.Combine(libpathroot, libname + ".asset"));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 指定图层数据文件
        /// </summary>
        private void AssignLayerData(Object file)
        {
            if (file == null)
                return;
            if ((xHud_PSDR__UseDebug)sp_DebugMode.enumValueIndex == xHud_PSDR__UseDebug.启用)
            {
                xHud_Utilitys.Func_PrintInfo("XHud PSD Reconstruction 通知", "已指定图层数据文件： " + file.name, HudMsgState.确认);
            }

            LayerDataPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(file));

            TextAsset data = file as TextAsset;
            LayerStructure = JsonConvert.DeserializeObject<PSDR_Root>(data.text);
            LayerFontDatas = GetLayerFontNames();

            #region 获取Fonts文件夹下所有字体文件完整路径以及名称和后缀类型
            string fontfilepaths = Path.Combine(Application.dataPath.Replace("/Assets", ""), LayerDataPath.Replace("/Assets", ""), "Fonts");
            if (Directory.Exists(fontfilepaths))
            {
                string[] allFiles = Directory.GetFiles(fontfilepaths);

                // 筛选出.ttf、.ttc和.otf文件
                string[] fontFiles = allFiles.Where(file =>
                    file.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".ttc", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".otf", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".woff", StringComparison.OrdinalIgnoreCase)
                ).ToArray();

                #region 暂存文件路径
                FontsFontPathInfos = new xHud_PSDR_FontsFontPathInfo[fontFiles.Length];

                for (int i = 0; i < fontFiles.Length; i++)
                {
                    if ((xHud_PSDR__UseDebug)sp_DebugMode.enumValueIndex == xHud_PSDR__UseDebug.启用)
                        xHud_Utilitys.Func_PrintInfo("XHud PSD Reconstruction 通知", $"已获取字体文件完整路径:  {fontFiles[i]}", HudMsgState.确认);

                    xHud_PSDR_FontsFontPathInfo info = new xHud_PSDR_FontsFontPathInfo();
                    info.FileName = Path.GetFileNameWithoutExtension(fontFiles[i]);
                    info.Path = fontFiles[i];
                    info.RelativePath = fontFiles[i].Substring(fontFiles[i].IndexOf("Assets"));
                    info.Extension = Path.GetExtension(fontFiles[i]);
                    FontsFontPathInfos[i] = info;
                }
                #endregion

            }
            #endregion

            #region 检测是否存在文字资源
            string path_font = LayerDataPath + "/Fonts";
            if (AssetDatabase.IsValidFolder(path_font))
            {
                for (int i = 0; i < LayerFontDatas.Length; i++)
                {
                    string extension = ".TTC";
                    if (FontsFontPathInfos != null && FontsFontPathInfos.Length > 0)
                    {
                        for (int s = 0; s < FontsFontPathInfos.Length; s++)
                        {
                            if (LayerFontDatas[i].Name == FontsFontPathInfos[s].FileName)
                            {
                                extension = FontsFontPathInfos[s].Extension;
                                break;
                            }
                        }
                    }
                    string path_font_folder = path_font + "/" + LayerFontDatas[i].Name + extension;
                    if (AssetDatabase.AssetPathExists(path_font_folder))
                    {
                        LayerFontDatas[i].Font = AssetDatabase.LoadAssetAtPath<Font>(path_font_folder);
                    }

                    string extension_tmp = ".asset";
                    string path_tmpfont_folder = path_font + "/" + LayerFontDatas[i].Name + " SDF" + extension_tmp;
                    if (AssetDatabase.AssetPathExists(path_tmpfont_folder))
                    {
                        LayerFontDatas[i].FontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path_tmpfont_folder);
                    }
                }
            }
            else
            {
                string FontPathGUID = AssetDatabase.CreateFolder(LayerDataPath, "Fonts");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            #endregion

            #region 总内容统计

            for (int s = 0; s < LayerStructure.structure.layers.Count; s++)
            {
                for (int i = 0; i < LayerFontDatas.Length; i++)
                {
                    if (LayerStructure.structure.layers[s].ft_name == LayerFontDatas[i].Name)
                    {
                        LayerFontDatas[i].Content += LayerStructure.structure.layers[s].text;
                    }
                }
            }

            //for (int i = 0; i < LayerFontDatas.Length; i++)
            //{
            //    sp_DebugMode.Log(LayerFontDatas[i].文字内容_Content);
            //}
            #endregion
        }

        /// <summary>
        /// 去色处理并将不透明部分调整为纯白色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Texture2D Desaturator(Texture2D input)
        {
            EnsureTextureReadWrite(input, true);

            TextureFormat targetFormat = TextureFormat.RGBA32;
            // 创建一个新的Texture2D对象，大小和格式与输入纹理相同
            Texture2D output = new Texture2D(input.width, input.height, targetFormat, false);

            // 获取输入纹理的像素数据
            Color[] pixels = input.GetPixels();

            EnsureTextureReadWrite(input, false);

            // 遍历每个像素
            for (int i = 0; i < pixels.Length; i++)
            {
                Color pixel = pixels[i];
                float alpha = pixel.a; // 获取当前像素的透明度

                // 如果像素不透明（透明度_Alpha > 0），将其颜色值设置为纯白色
                if (alpha > 0)
                {
                    pixels[i] = new Color(1.0f, 1.0f, 1.0f, alpha); // 设置为纯白色并保留透明度
                }
                else
                {
                    pixels[i] = new Color(0, 0, 0, 0); // 透明部分保持不变
                }
            }

            // 将处理后的像素数据应用到输出纹理
            output.SetPixels(pixels);
            output.Apply();


            return output;
        }

        /// <summary>
        /// 使用AssetDatabase保存Texture2D为PNG文件
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="path"></param>
        public static void SaveTexture(Texture2D texture, string path)
        {
            // 将Texture2D转换为PNG格式的字节数组
            byte[] bytes = texture.EncodeToPNG();

            // 将字节数组保存为文件
            File.WriteAllBytes(path, bytes);

            // 刷新AssetDatabase以使新资源可见
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 将PNG文件导入为Sprite资源，并设置Sprite Mode为Single
        /// </summary>
        /// <param name="assetPath"></param>
        public static void ConvertSprite(string assetPath)
        {
            // 获取TextureImporter
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null)
            {
                // 设置Texture Type为Sprite (2D and UI)
                importer.textureType = TextureImporterType.Sprite;
                // 设置Sprite Mode为Single
                importer.spriteImportMode = SpriteImportMode.Single;
                // 设置其他相关属性（可选）
                importer.spritePixelsPerUnit = 100; // 根据需要调整
                importer.isReadable = true; // 如果需要从脚本访问纹理数据，需要设置为true

                // 重新导入纹理以应用设置
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }
            else
            {
                Debug.LogError("Failed to get TextureImporter for the asset.");
            }
        }

        /// <summary>
        /// 确保贴图的Read/Write属性正确设置。
        /// 如果贴图的Read/Write未开启，则开启；如果已开启，则关闭。
        /// </summary>
        /// <param name="texture">需要处理的Texture2D对象。</param>
        public static void EnsureTextureReadWrite(Texture2D texture, bool state)
        {
            if (texture == null)
            {
                //sp_DebugMode.LogError("Texture is not assigned.");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(texture);
            if (string.IsNullOrEmpty(assetPath))
            {
                //sp_DebugMode.LogError("Texture is not an asset in the project.");
                return;
            }

            // 获取TextureImporter
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                //sp_DebugMode.LogError("Failed to get TextureImporter for the asset.");
                return;
            }

            if (importer.isReadable == state)
                return;

            importer.isReadable = state; // 开启Read/Write

            // 重新导入贴图以应用更改
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            //sp_DebugMode.Log($"Texture Read/Write property updated for: {assetPath}");
        }

        /// <summary>
        /// 重建结构
        /// </summary>
        /// <param name="folder"></param>
        private void ReConstructure(Object folder)
        {
            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

            if (sp_LayersFolder.objectReferenceValue == null || sp_LayersData.objectReferenceValue == null)
            {
                if ((xHud_PSDR__UseDebug)sp_DebugMode.enumValueIndex == xHud_PSDR__UseDebug.启用)
                    xHud_Utilitys.Func_PrintInfo("XHud PSD Reconstruction 通知", "请检查图元目录和重建数据是否正确指定了？ ", HudMsgState.警告);
                return;
            }

            string StructureName = "";
            if ((xHud_PSDR_TextLayerMode)sp_TextLayerMode.enumValueIndex == xHud_PSDR_TextLayerMode.文字组件化)
            {
                if ((xHud_PSDR_TextLayerTypes)sp_TextLayerType.enumValueIndex == xHud_PSDR_TextLayerTypes.HudText)
                {
                    StructureName = $"Reconstruction - {LayerStructure.structure.name} - Hud-Text";
                }
                else
                {
                    StructureName = $"Reconstruction - {LayerStructure.structure.name} - Hud-TmpText";
                }
            }
            else
            {
                if ((xHud_PSDR_PureSprite)sp_PureSprite.enumValueIndex == xHud_PSDR_PureSprite.原图模式)
                    StructureName = $"Reconstruction - {LayerStructure.structure.name} - Pixel - Original";
                else
                    StructureName = $"Reconstruction - {LayerStructure.structure.name} - Pixel - Pure";
            }

            string path_folder = AssetDatabase.GetAssetPath(folder);

            ///读取所有图层元素Sprite
            Object[] layers = new Texture2D[LayerStructure.structure.layers.Count];
            for (int i = 0; i < LayerStructure.structure.layers.Count; i++)
            {
                PSDR_Layers lay = LayerStructure.structure.layers[i];
                string path_obj = path_folder + $"/{lay.id}_{lay.type}_{lay.name}.png";

                layers[i] = AssetDatabase.LoadAssetAtPath(path_obj, typeof(Object));

                if (lay.type == xHud_PSDR_LayerType.shp)
                {
                    if ((xHud_PSDR_PureSprite)sp_PureSprite.enumValueIndex == xHud_PSDR_PureSprite.纯净模式)
                    {
                        Texture2D tex = layers[i] as Texture2D;
                        // 获取源贴图的路径和名称
                        string directory = Path.GetDirectoryName(path_obj);
                        // 获取文件名（不含扩展名）
                        string fileName = Path.GetFileNameWithoutExtension(path_obj);
                        // 新文件名
                        string newFileName = $"{fileName}_Desaturated.png";
                        // 新路径
                        string newPath = Path.Combine(directory, newFileName);

                        if (!AssetDatabase.AssetPathExists(newPath))
                        {
                            // 处理贴图去饱和度
                            Texture2D outputTexture = Desaturator(layers[i] as Texture2D);

                            // 将处理后的Texture2D保存
                            SaveTexture(outputTexture, newPath);

                            //// 将PNG文件导入为Sprite资源
                            ConvertSprite(newPath);
                        }
                    }
                }
            }

            ///设置图层图片为Sprite之后再根据图层数据创建图层元素
            SetTexturesToSpriteType(layers, () =>
            {
                ///创建根目录
                GameObject structure = new GameObject();
                structure.name = StructureName;
                RectTransform struct_rect = structure.AddComponent<RectTransform>();
                struct_rect.SetParent(mgr.hm_Layout_GetAnchor(HudAnchor.中心));
                struct_rect.anchoredPosition3D = Vector3.zero;
                struct_rect.localEulerAngles = Vector3.zero;
                struct_rect.localScale = Vector3.one;
                Vector2 size = xHud_Utilitys.Vector2_From_String(LayerStructure.structure.size);
                struct_rect.sizeDelta = size;

                List<xHudGUI_Dialog_ListDatas> Datas = new List<xHudGUI_Dialog_ListDatas>();
                string hexcol = xHud_Utilitys.Color_To_HexColor(xHud_Dashboard.Theme_Primary, true);

                for (int i = 0; i < LayerStructure.structure.layers.Count; i++)
                {
                    ///获取图层数据
                    PSDR_Layers lay = LayerStructure.structure.layers[i];
                    GameObject layerObject = null;

                    ///如果是Group只创建（组）
                    if (lay.type == xHud_PSDR_LayerType.group)
                    {
                        GameObject grp_obj = new GameObject();
                        grp_obj.name = lay.name;

                        xHud_PSDR_LayerInfo grp_info = grp_obj.AddComponent<xHud_PSDR_LayerInfo>();
                        grp_info.Layer = lay;

                        /* 图层数据中的Position是指在PS中图层相对画布的边距距离值，而不是相对于编组的相对边距距离值，
                         * 所以要有效解决编组元素相对编组的坐标系数值对不上的问题,要提前一步先将物体放在重建根物体下根据画布校正边距位置后再放入对应的编组中
                         */
                        grp_obj.transform.SetParent(struct_rect);

                        ///RectTransform坐标设置
                        RectTransform rectTransform = grp_obj.AddComponent<RectTransform>();
                        rectTransform.sizeDelta = new Vector2(lay.size.width, lay.size.height);
                        ///根据锚点校正图层元素位置
                        Vector2 cor_pos = Calculate_Position(new Vector2(lay.size.width, lay.size.height), new Vector2(lay.position.x, lay.position.y));
                        rectTransform.anchoredPosition3D = new Vector3(cor_pos.x, cor_pos.y, 0);
                        rectTransform.localEulerAngles = Vector3.zero;
                        rectTransform.localScale = Vector3.one;

                        ///父物体层级
                        /* 如果当前的图层数据的父物体为空并且其ID也为0的话就直接放入根物体下
                         * 否则就从根物体下获取所有 LayerInfo 脚本的物体并从其中筛选出目标名称的父物体
                         * */
                        if (lay.parent.name == "" && lay.parent.id == 0)
                            grp_obj.transform.SetParent(struct_rect);
                        else
                        {
                            RectTransform rect_parent = GetTargetParent(struct_rect, grp_info);
                            if (rect_parent != null)
                                grp_obj.transform.SetParent(rect_parent);
                            else
                                grp_obj.transform.SetParent(struct_rect);
                        }

                        rectTransform.SetAsFirstSibling();

                        Undo.RegisterCreatedObjectUndo(grp_obj, "CreateGroup_" + lay.id);

                        layerObject = grp_obj;
                    }
                    else
                    {
                        ///获取Sprite资源路径
                        string path_obj = path_folder + $"/{lay.id}_{lay.type}_{lay.name}.png";

                        ///创建图层元素
                        GameObject img_obj = new GameObject();
                        img_obj.name = lay.name;

                        xHud_PSDR_LayerInfo img_info = img_obj.AddComponent<xHud_PSDR_LayerInfo>();
                        img_info.Layer = lay;

                        Sprite layer_sprite = null;

                        #region  分类特殊化设定
                        if (lay.type == xHud_PSDR_LayerType.pix)
                        {
                            ///读取Sprite资源
                            layer_sprite = (Sprite)AssetDatabase.LoadAssetAtPath(path_obj, typeof(Sprite));

                            ///Sprite内容设置
                            Image img = img_obj.AddComponent<Image>();
                            img.sprite = layer_sprite;

                            ///图层元素Image的颜色&透明度设定
                            Color col_img = img.color;
                            col_img.a = lay.opacity;
                            img.color = col_img;
                        }
                        else if (lay.type == xHud_PSDR_LayerType.txt)
                        {
                            ///判断是否使用像素化文字还是组件化文字
                            xHud_PSDR_TextLayerMode TextPixel = (xHud_PSDR_TextLayerMode)sp_TextLayerMode.enumValueIndex;

                            if (TextPixel == xHud_PSDR_TextLayerMode.文字像素化)
                            {
                                ///读取Sprite资源
                                layer_sprite = (Sprite)AssetDatabase.LoadAssetAtPath(path_obj, typeof(Sprite));

                                ///Sprite内容设置
                                Image img = img_obj.AddComponent<Image>();
                                img.sprite = layer_sprite;

                                ///图层元素Image的颜色&透明度设定
                                Color col_img = img.color;
                                col_img = Color.white;
                                img.color = col_img;
                            }
                            else
                            {
                                ///判断是否使用 "HudText" 文字还是 "HudTmpText" 文字
                                xHud_PSDR_TextLayerTypes TextTypes = (xHud_PSDR_TextLayerTypes)sp_TextLayerType.enumValueIndex;

                                if (TextTypes == xHud_PSDR_TextLayerTypes.HudText)
                                {
                                    xHud_Module_Text text = img_obj.AddComponent<xHud_Module_Text>();
                                    ///Text内容设置
                                    text.txt_Set_Content(lay.text);

                                    ///图层元素Image的颜色&透明度设定
                                    Color col_text = text.color;
                                    col_text.r = lay.ft_color_clamp_rgba.r;
                                    col_text.g = lay.ft_color_clamp_rgba.g;
                                    col_text.b = lay.ft_color_clamp_rgba.b;
                                    col_text.a = lay.ft_color_clamp_rgba.a;
                                    text.color = col_text;

                                    text.TextStyleInfo.txt_Set_FontColor(col_text);

                                    switch (lay.ft_align)
                                    {
                                        case "left":
                                            text.TextStyleInfo.txt_Set_Alignment(ContentAnchor.左);
                                            text.TextStyleInfo.ContentAnchor = ContentAnchor.左;
                                            break;
                                        case "right":
                                            text.TextStyleInfo.txt_Set_Alignment(ContentAnchor.右);
                                            text.TextStyleInfo.ContentAnchor = ContentAnchor.右;
                                            break;
                                        case "center":
                                            text.TextStyleInfo.txt_Set_Alignment(ContentAnchor.中心);
                                            text.TextStyleInfo.ContentAnchor = ContentAnchor.中心;
                                            break;
                                    }

                                    ///设置内容溢出
                                    text.TextStyleInfo.txt_Set_Overflow(HorizontalWrapMode.Overflow);
                                    text.TextStyleInfo.txt_Set_Overflow(VerticalWrapMode.Overflow);
                                    ///设置文字尺寸
                                    text.TextStyleInfo.txt_Set_FontSize((int)lay.ft_size);

                                    if (lay.ft_isWrapText)
                                    {
                                        text.TextStyleInfo.txt_Set_Overflow(HorizontalWrapMode.Wrap);
                                    }

                                    if (lay.ft_leading != 0)
                                    {
                                        text.TextStyleInfo.txt_Set_FontLineHeight(lay.ft_leading / 10);
                                    }

                                    ///设置文字字体 - 按照字体名称来匹配
                                    for (int d = 0; d < LayerFontDatas.Length; d++)
                                    {
                                        if (lay.ft_name == LayerFontDatas[d].Name)
                                        {
                                            text.TextStyleInfo.Font = LayerFontDatas[d].Font;
                                            text.font = LayerFontDatas[d].Font;
                                            text.TextStyleInfo.txt_Set_Font(LayerFontDatas[d].Font);
                                        }
                                    }
                                }
                                else if (TextTypes == xHud_PSDR_TextLayerTypes.HudTmpText)
                                {
                                    xHud_Module_TmpText text = img_obj.AddComponent<xHud_Module_TmpText>();

                                    ///设置文字字体
                                    for (int d = 0; d < LayerFontDatas.Length; d++)
                                    {
                                        if (lay.ft_name == LayerFontDatas[d].Name)
                                        {
                                            if (LayerFontDatas[d].FontAsset != null)
                                            {
                                                text.TextStyleInfo.tmp_font = LayerFontDatas[d].FontAsset;
                                                text.font = LayerFontDatas[d].FontAsset;
                                                text.TextStyleInfo.tmp_Set_FontAsset(LayerFontDatas[d].FontAsset);
                                            }
                                        }
                                    }

                                    ///判断是否是多行还是单行
                                    if (lay.ft_isWrapText)
                                        text.TextStyleInfo.tmp_Set_WordWrappingMode(TextWrappingModes.Normal);
                                    else
                                        text.TextStyleInfo.tmp_Set_WordWrappingMode(TextWrappingModes.NoWrap);

                                    ///判断是否是全大写
                                    if (lay.ft_isuppercase)
                                        text.TextStyleInfo.tmp_Set_FontStyle(FontStyles.UpperCase);

                                    ///Text内容设置
                                    text.tmp_Set_Content(lay.text);

                                    ///图层元素Image的颜色&透明度设定
                                    Color col_text = text.color;
                                    col_text.r = lay.ft_color_clamp_rgba.r;
                                    col_text.g = lay.ft_color_clamp_rgba.g;
                                    col_text.b = lay.ft_color_clamp_rgba.b;
                                    col_text.a = lay.ft_color_clamp_rgba.a;
                                    text.color = col_text;

                                    ///设置文字颜色
                                    text.TextStyleInfo.tmp_Set_FontColor(col_text);

                                    ///设置对齐方式
                                    switch (lay.ft_align)
                                    {
                                        case "left":
                                            text.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.左);
                                            break;
                                        case "right":
                                            text.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.右);
                                            break;
                                        case "center":
                                            text.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.中心);
                                            break;
                                    }

                                    //if (lay.ft_leading != 0)
                                    //    text.Text_Set_SpacingSet_Line(lay.ft_leading);

                                    ///设置字符间距
                                    text.TextStyleInfo.tmp_Set_SpacingSet_Character(lay.ft_space * 0.1f);
                                    ///设置内容溢出
                                    text.TextStyleInfo.tmp_Set_Overflow(TextOverflowModes.Overflow);
                                    ///设置文字尺寸
                                    text.TextStyleInfo.tmp_Set_FontSize((int)lay.ft_size);

                                    //text.tmp_Syncing_Style();
                                }
                            }
                        }
                        else if (lay.type == xHud_PSDR_LayerType.shp)
                        {
                            if ((xHud_PSDR_PureSprite)sp_PureSprite.enumValueIndex == xHud_PSDR_PureSprite.纯净模式)
                            {
                                path_obj = path_folder + $"/{lay.id}_{lay.type}_{lay.name}_Desaturated.png";
                            }

                            ///读取Sprite资源
                            layer_sprite = (Sprite)AssetDatabase.LoadAssetAtPath(path_obj, typeof(Sprite));

                            if ((xHud_PSDR_PureSprite)sp_PureSprite.enumValueIndex == xHud_PSDR_PureSprite.纯净模式)
                            {
                                EnsureTextureReadWrite(layer_sprite.texture, false);
                            }

                            ///Sprite内容设置
                            Image img = img_obj.AddComponent<Image>();
                            img.sprite = layer_sprite;

                            ///图层元素Image的颜色&透明度设定
                            Color col_img = img.color;
                            if ((xHud_PSDR_PureSprite)sp_PureSprite.enumValueIndex == xHud_PSDR_PureSprite.纯净模式)
                            {
                                col_img.r = lay.color_clamp_rgba.r;
                                col_img.g = lay.color_clamp_rgba.g;
                                col_img.b = lay.color_clamp_rgba.b;
                                col_img.a = lay.color_clamp_rgba.a;
                                img.color = col_img;
                                //sp_DebugMode.Log(img.color);
                            }
                            else
                            {
                                col_img = Color.white;
                                col_img.a = lay.opacity * 0.01f;
                                img.color = col_img;
                                //sp_DebugMode.Log(img.color);
                            }
                        }
                        else if (lay.type == xHud_PSDR_LayerType.smt)
                        {
                            ///读取Sprite资源
                            layer_sprite = (Sprite)AssetDatabase.LoadAssetAtPath(path_obj, typeof(Sprite));

                            ///Sprite内容设置
                            Image img = img_obj.AddComponent<Image>();
                            img.sprite = layer_sprite;

                            ///图层元素Image的颜色&透明度设定
                            Color col_img = img.color;
                            col_img.a = lay.opacity;
                            img.color = col_img;
                        }
                        #endregion

                        /* 图层数据中的Position是指在PS中图层相对画布的边距距离值，而不是相对于编组的相对边距距离值，
                         * 所以要有效解决图层元素相对编组的坐标系数值对不上的问题,要提前一步先将物体放在重建根物体下根据画布校正边距位置后再放入对应的编组中
                         */
                        img_obj.transform.SetParent(struct_rect);

                        ///RectTransform坐标设置
                        RectTransform rectTransform = img_obj.GetComponent<RectTransform>();
                        if (rectTransform == null)
                        {
                            img_obj.AddComponent<RectTransform>();
                        }

                        ///文字还是要按照常规Json数据尺寸来设置
                        if (lay.type == xHud_PSDR_LayerType.txt)
                        {
                            rectTransform.sizeDelta = new Vector2(lay.size.width, lay.size.height);
                        }
                        else
                        {
                            ///只有图层不是空的
                            if (layer_sprite != null)
                            {
                                ///这里需要判断一下，因为有些PS里的图层可能会加上一层遮罩，所以在PS导出时尺寸是按照没有遮罩的尺寸为基准的，所以这里如果图层没有Mask遮罩那么就是用Json里的尺寸数据，否则使用导出的图片自身尺寸数据
                                rectTransform.sizeDelta = lay.mask ? new Vector2(layer_sprite.texture.width, layer_sprite.texture.height) : new Vector2(lay.size.width, lay.size.height);
                            }
                        }

                        ///根据锚点校正图层元素位置
                        Vector2 cor_pos = Calculate_Position(new Vector2(lay.size.width, lay.size.height), new Vector2(lay.position.x, lay.position.y));
                        rectTransform.anchoredPosition3D = new Vector3(cor_pos.x, cor_pos.y, 0);
                        rectTransform.localEulerAngles = Vector3.zero;
                        rectTransform.localScale = Vector3.one;

                        ///父物体层级
                        /* 如果当前的图层数据的父物体为空并且其ID也为0的话就直接放入根物体下
                         * 否则就从根物体下获取所有 LayerInfo 脚本的物体并从其中筛选出目标名称的父物体
                         * */
                        if (lay.parent.name == "" && lay.parent.id == 0)
                        {
                            img_obj.transform.SetParent(struct_rect);
                        }
                        else
                        {
                            RectTransform rect_parent = GetTargetParent(struct_rect, img_info);
                            if (rect_parent != null)
                                img_obj.transform.SetParent(rect_parent);
                            else
                                img_obj.transform.SetParent(struct_rect);
                        }
                        rectTransform.SetAsFirstSibling();

                        layerObject = img_obj;
                        Undo.RegisterCreatedObjectUndo(img_obj, "CreateLayer_" + lay.id);
                    }

                    ///统计带有遮罩的图层列表
                    if (lay.mask)
                    {
                        xHudGUI_Dialog_ListDatas dataitem = new xHudGUI_Dialog_ListDatas();

                        dataitem.Title = $"<color=#fff>{lay.name}</color>";
                        dataitem.SubTitle = $"{lay.type}";
                        dataitem.Message = "带有Mask遮罩";
                        dataitem.SourceObject = layerObject;
                        Datas.Add(dataitem);
                    }
                }

                Editor_xHudGUI.Open(Datas.ToArray(), xHudDialogType.警告, "XHud PSD Reconstruction消息", "重建特殊化警告", "因为在PS中未涂层添加了Mask遮罩而并未栅格化图层，可能导致列表中的这些图层的位置会有些许偏移，请手动矫正或者在PS中栅格化他们！", "明白", 0, false);

                Undo.RegisterCreatedObjectUndo(structure, "CreateReconstruction");
            });
        }

        /// <summary>
        /// 字符串去重
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string RemoveDuplicateCharacters(string input)
        {
            HashSet<char> charSet = new HashSet<char>();
            foreach (char c in input)
            {
                charSet.Add(c);
            }

            return string.Concat(charSet);
        }

        /// <summary>
        /// 将目标文件夹中的图元转换为Sprite图形
        /// </summary>
        /// <param name="Texs"></param>
        private void SetTexturesToSpriteType(Object[] Texs, Action callback)
        {
            // 获取选中的所有纹理资源
            foreach (var asset in Texs)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                if (string.IsNullOrEmpty(assetPath)) continue;

                // 获取纹理的导入设置
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer == null) continue;

                // 设置纹理类型为 Sprite (2D and UI)
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single; // 设置为单个精灵模式
                importer.mipmapEnabled = false; // 禁用 Mipmap
                importer.isReadable = false; // 设置为不可读
                importer.wrapMode = TextureWrapMode.Clamp; // 设置纹理包裹模式为 Clamp
                importer.filterMode = FilterMode.Bilinear; // 设置纹理过滤模式为双线性过滤

                // 应用更改
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }
            if ((xHud_PSDR__UseDebug)sp_DebugMode.enumValueIndex == xHud_PSDR__UseDebug.启用)
                xHud_Utilitys.Func_PrintInfo("XHud PSD Reconstruction 通知", "已将目标文件夹中的图元转换为Sprite图形！", HudMsgState.通知);
            Repaint();
            AssetDatabase.Refresh();
            callback?.Invoke();
        }

        /// <summary>
        /// 获取目标编组父物体
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private RectTransform GetTargetParent(RectTransform parent, xHud_PSDR_LayerInfo info)
        {
            xHud_PSDR_LayerInfo[] layerInfos = parent.GetComponentsInChildren<xHud_PSDR_LayerInfo>();

            if (layerInfos.Length > 0)
            {
                RectTransform rect = null;
                for (int i = 0; i < layerInfos.Length; i++)
                {
                    if (layerInfos[i].Layer.name == info.Layer.parent.name && layerInfos[i].Layer.id == info.Layer.parent.id)
                    {
                        rect = layerInfos[i].transform.GetComponent<RectTransform>();
                        break;
                    }
                }
                return rect;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取重建根物体
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private Transform GetRestructionRoot(string name)
        {
            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

            Transform parent = mgr.hm_Layout_GetAnchor(HudAnchor.中心);

            Transform rect = null;
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform obj = parent.GetChild(i);
                if (obj.name == name)
                {
                    rect = obj;
                    break;
                }
            }
            return rect;
        }

        /// <summary>
        /// 计算位置边距值
        /// </summary>
        /// <param name="Anchor"></param>
        /// <param name="LayerSize"></param>
        /// <param name="LayerOffset"></param>
        /// <returns></returns>
        private Vector2 Calculate_Position(Vector2 LayerSize, Vector2 LayerOffset)
        {
            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

            Vector2 pos = Vector2.zero;

            ///自由定位 - 位置_Position - 正向X轴
            pos.x = (0 - mgr.ScreenRes.x / 2) + (LayerSize.x / 2) + LayerOffset.x;
            ///自由定位 - 位置_Position - 正向Y轴
            pos.y = (mgr.ScreenRes.y / 2) - (LayerSize.y / 2) - LayerOffset.y;

            return pos;
        }

        /// <summary>
        /// 获取数据文件里的所有不重复的字体名称
        /// </summary>
        /// <returns></returns>
        private xHud_PSDR_FontData[] GetLayerFontNames()
        {
            /// 使用自定义比较器
            HashSet<xHud_PSDR_FontData> uniqueDatas = new HashSet<xHud_PSDR_FontData>(new xHud_PSDR_FontDataComparer());

            for (int i = 0; i < LayerStructure.structure.layers.Count; i++)
            {
                PSDR_Layers lay = LayerStructure.structure.layers[i];

                if (lay.type == xHud_PSDR_LayerType.txt)
                {
                    xHud_PSDR_FontData fd = new xHud_PSDR_FontData();
                    if (!string.IsNullOrEmpty(lay.ft_name))
                        fd.Name = lay.ft_name;
                    if (!string.IsNullOrEmpty(lay.ft_family))
                        fd.Family = lay.ft_family;
                    if (!string.IsNullOrEmpty(lay.ft_style))
                        fd.Style = lay.ft_style;

                    /// 添加到 HashSet 中，自动去重
                    uniqueDatas.Add(fd);
                }
            }

            return uniqueDatas.ToArray(); // 转换为数组返回
        }

        /// <summary>
        /// 重命名字体资源名称
        /// </summary>
        private void RenameFontAsset(Object asset, string name)
        {
            ///资源路径
            string x_Path = AssetDatabase.GetAssetPath(asset);
            ///获取后缀名
            string Extension = Path.GetExtension(x_Path);
            /// 构造新路径
            string newname = name + Extension;

            Debug.Log($"{x_Path}  -  {Extension}  -  {newname}");

            // 调用 RenameAsset 方法
            string result = AssetDatabase.RenameAsset(x_Path, newname);
            if (string.IsNullOrEmpty(result))
            {
                if ((xHud_PSDR__UseDebug)sp_DebugMode.enumValueIndex == xHud_PSDR__UseDebug.启用)
                    xHud_Utilitys.Func_PrintInfo("XHud PSD Reconstruction 通知", "资源已成功重命名为： " + newname, HudMsgState.确认);
            }
            else
            {
                if ((xHud_PSDR__UseDebug)sp_DebugMode.enumValueIndex == xHud_PSDR__UseDebug.启用)
                    xHud_Utilitys.Func_PrintInfo("XHud PSD Reconstruction 通知", "重命名失败： " + result, HudMsgState.错误);
            }

            // 刷新资源数据库
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 打开字体库目录
        /// </summary>
        private void OpenFontLib()
        {
            try
            {
                // Windows 系统字体文件夹路径
                string fontsFolderPath = @"C:\Windows\Fonts";

                // 使用 Process.Start 打开字体文件夹
                Process.Start(new ProcessStartInfo
                {
                    FileName = fontsFolderPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                if ((xHud_PSDR__UseDebug)sp_DebugMode.enumValueIndex == xHud_PSDR__UseDebug.启用)
                    xHud_Utilitys.Func_PrintInfo("XHud PSD Reconstruction 通知", $"打开字体文件夹错误！ {ex.Message}", HudMsgState.错误);
            }
        }

        /// <summary>
        /// 创建Tmp字体资源
        /// </summary>
        /// <param name="target"></param>
        private TMP_FontAsset CreateFontAsset(Font font, string suffix, int width, int height, string precontent)
        {
            // 选择字体文件
            if (font == null)
            {
                if ((xHud_PSDR__UseDebug)sp_DebugMode.enumValueIndex == xHud_PSDR__UseDebug.启用)
                    xHud_Utilitys.Func_PrintInfo("XHud PSD Reconstruction 通知", "请先制定一个Font字体！ ", HudMsgState.错误);
                return null;
            }

            // 获取字体文件路径
            string fullPath = AssetDatabase.GetAssetPath(font);
            if (!File.Exists(fullPath))
            {
                if ((xHud_PSDR__UseDebug)sp_DebugMode.enumValueIndex == xHud_PSDR__UseDebug.启用)
                    xHud_Utilitys.Func_PrintInfo("XHud PSD Reconstruction 通知", $"在 {fullPath} 路径下未找到 {font.name} 字体！", HudMsgState.错误);
                return null;
            }

            // 设置自定义字符集
            string customCharacters = precontent;

            // 创建 TMP_FontAsset
            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font, 128, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);

            if (fontAsset == null)
            {
                if ((xHud_PSDR__UseDebug)sp_DebugMode.enumValueIndex == xHud_PSDR__UseDebug.启用)
                    xHud_Utilitys.Func_PrintInfo("XHud PSD Reconstruction 通知", "创建FontAsset 失败！", HudMsgState.错误);
                return null;
            }

            // 设置字体资源名称
            fontAsset.name = font.name + suffix;

            // 保存 TMP_FontAsset
            string savePath = Path.GetDirectoryName(fullPath);
            string assetPath = Path.Combine(savePath, font.name + suffix + ".asset");
            //sp_DebugMode.Log($"{savePath}  -  {assetPath}");
            AssetDatabase.CreateAsset(fontAsset, assetPath);

            // 创建图集添加到资源中            
            fontAsset.atlasTextures = new Texture2D[1];
            Texture2D texture = new Texture2D(width, height, TextureFormat.Alpha8, false);
            texture.name = "Atlas";
            fontAsset.atlasTextures[0] = texture;
            AssetDatabase.AddObjectToAsset(texture, fontAsset);

            //创建材质添加到资源中
            Shader default_Shader = Shader.Find("TextMeshPro/Distance Field");
            Material tmp_material = new Material(default_Shader);
            tmp_material.name = "Material";
            tmp_material.SetTexture(ShaderUtilities.ID_MainTex, texture);
            tmp_material.SetFloat(ShaderUtilities.ID_TextureWidth, width);
            tmp_material.SetFloat(ShaderUtilities.ID_TextureHeight, height);
            tmp_material.SetFloat(ShaderUtilities.ID_GradientScale, 10);
            tmp_material.SetFloat(ShaderUtilities.ID_WeightNormal, fontAsset.normalStyle);
            tmp_material.SetFloat(ShaderUtilities.ID_WeightBold, fontAsset.boldStyle);
            fontAsset.material = tmp_material;
            AssetDatabase.AddObjectToAsset(tmp_material, fontAsset);

            //清空资源内容
            fontAsset.ClearFontAssetData();

            //添加字符串内容
            bool success = fontAsset.TryAddCharacters(customCharacters, out string missingCharacters);

            if (success)
            {
                if ((xHud_PSDR__UseDebug)sp_DebugMode.enumValueIndex == xHud_PSDR__UseDebug.启用)
                    xHud_Utilitys.Func_PrintInfo("XHud PSD Reconstruction 通知", $"已将字符串内容 {customCharacters} 添加到 {fontAsset.name} TMP字体资源图集中！", HudMsgState.确认);
            }
            else
            {
                if ((xHud_PSDR__UseDebug)sp_DebugMode.enumValueIndex == xHud_PSDR__UseDebug.启用)
                    xHud_Utilitys.Func_PrintInfo("XHud PSD Reconstruction 通知", $"未能将字符串内容 {missingCharacters} 添加到 {fontAsset.name} TMP字体资源图集中！", HudMsgState.警告);
            }

            // 刷新资源数据库
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //sp_DebugMode.Log($"TMP_FontAsset created successfully at: {assetPath}");

            return fontAsset;
        }
    }
}