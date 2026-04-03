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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEngine;

    public struct HSV
    {
        public float H; // 色相 (Hue)
        public float S; // 饱和度 (Saturation)
        public float V; // 明度 (Value)

        public HSV(float hue, float saturation, float value)
        {
            H = hue;
            S = saturation;
            V = value;
        }
    }

    [System.Serializable]
    public class XHud_Emotions
    {
        // 积极情绪列表
        public string Pos { get; set; }

        // 消极情绪列表
        public string Neg { get; set; }
    }

    [System.Serializable]
    public class XHud_ColorEmotionNode
    {
        [SerializeField]
        // 颜色名称
        public string ColorName;
        [SerializeField]
        // 积极情绪列表
        public List<string> Pos;
        [SerializeField]
        // 色环角度
        public string Hue;
        [SerializeField]
        // 消极情绪列表
        public List<string> Neg;
    }

    [System.Serializable]
    public class XHud_ColorEmotions
    {
        [SerializeField]
        // 颜色情绪列表
        public List<XHud_ColorEmotionNode> Colors;
    }

    public class Editor_XHud_LibrarySetTool_Color : EditorWindow
    {
        private SerializedObject BaseObject;
        private SerializedProperty
            sp_ColorName,
            sp_OriginColorName,
            sp_ColorDescription,
            sp_Color;

        private Texture2D icon_libsetter_color;

        private Texture2D leftarr_r;
        private Texture2D leftarr_p;
        private Texture2D rightarr_r;
        private Texture2D rightarr_p;

        private float PreviewDataDuration = 0.035f;
        private int PreviewData_Index = 0;
        private EditorCoroutine PreviewCoroutine;
        private Texture2D PreviewTex;

        public LibrarySetterMode LibrarySetterMode;

        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;

        public XHud_ColorEmotions ColorEmotions;
        /// <summary>
        /// 源配色器
        /// </summary>
        public XHud_Module_Primitive_Painting SourcePainting;
        [SerializeField]
        public string ColorName;
        [SerializeField]
        public string DateTimes;
        [SerializeField]
        public string ColorDescription;
        [SerializeField]
        public Color Color;

        /// <summary>
        /// 按钮宽度
        /// </summary>
        private float ButtonWidth = 110;
        /// <summary>
        /// 按钮高度
        /// </summary>
        private float ButtonHeight = 25;
        /// <summary>
        /// 按钮间距
        /// </summary>
        private float ButtonDistance = 15;

        [SerializeField]
        public bool UseBg = true;

        [SerializeField]
        public string ButtonText_Ok;
        [SerializeField]
        public string ButtonText_Cancel;
        [SerializeField]
        public string OriginColorName;

        Color SepLineColor = new Color(1, 1, 1, 0.15f);
        Color MessageColor = new Color(1, 1, 1, 0.62f);
        Color DateTimeColor = new Color(1, 1, 1, 0.42f);

        Color PreviewBgColor = Color.black;

        Rect draw_rect;

        public int ModifiedIndex;

        string Title;

        public XHud_LibrarySetTool_Color_PreviewData[] PreviewDatas;

        private XHud_Library_Colors Target_Hud_ColorsLibrary;

        private void OnDisable()
        {
            StopPreviewUpdate();
        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);
            sp_ColorName = BaseObject.FindProperty("ColorName");
            sp_ColorDescription = BaseObject.FindProperty("ColorDescription");
            sp_Color = BaseObject.FindProperty("Color");
            sp_OriginColorName = BaseObject.FindProperty("OriginColorName");

            icon_libsetter_color = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Color_Setter/logo");

            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Dialog");

            leftarr_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Color_Setter/leftarr_p");
            leftarr_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Color_Setter/leftarr_r");
            rightarr_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Color_Setter/rightarr_p");
            rightarr_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Color_Setter/rightarr_r");

            ColorDescription = "颜色说明内容";
            ColorName = "颜色名称";

            TextAsset emotions = AssetDatabase.LoadAssetAtPath<TextAsset>($"{XHud_Dashboard.Get_Path_XHUD_CONFIG_Path()}XHudColorEmotions.json");
            ColorEmotions = JsonUtility.FromJson<XHud_ColorEmotions>(emotions.text);
            for (int i = 0; i < ColorEmotions.Colors.Count; i++)
            {
                Debug.Log($"{ColorEmotions.Colors[i].ColorName} | {ColorEmotions.Colors[i].Neg} | {ColorEmotions.Colors[i].Hue} | {ColorEmotions.Colors[i].Pos}");
            }
            PreviewData_Index = Editor_XHud_GUI.EditorData_Get_With_Int("XED_HudAnimator_Set_previewtexs_index");

            StartPreviewUpdate();
        }

        private void OnDestroy()
        {
            GetWindow<SceneView>().Focus();
        }

        private void OnGUI()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            BaseObject.Update();

            Rect rect = new Rect(0, 0, position.width, position.height);
            draw_rect.Set(0, 0, position.width, position.height);

            draw_rect.Set(26, 15, 48, 48);
            Editor_XHud_GUI.Gui_Icon(draw_rect, icon_libsetter_color);

            draw_rect.Set(rect.x + 100, rect.y + 15, rect.width - 80, 30);
            Editor_XHud_GUI.Gui_Labelfield(draw_rect, Title, HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 20, Font_Bold);

            draw_rect.Set(rect.x + 102, rect.y + 60, 200, 1);
            Editor_XHud_GUI.Gui_Box(draw_rect, SepLineColor);

            draw_rect.Set(rect.x + 26, rect.y + 80, rect.width - 45, rect.height);
            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(draw_rect, "以下为待入库的颜色参数与效果的预览，您可以检查即将入库的颜色是否符合您的要求，同时您可以再次调整即将入库的颜色！", HudFilled.无, HudColor.无, MessageColor, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true, Font_Light);

            DateTimes = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss:ff");
            draw_rect.Set(rect.x + 150, rect.y + 15, rect.width - 180, rect.height);
            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(draw_rect, DateTimes, HudFilled.无, HudColor.无, DateTimeColor, TextAnchor.UpperRight, new Vector2(0, 0), 13, true, Font_Light);

            #region 颜色入库信息
            string colorhex = XHud_Utilitys.Color_To_HexColor(sp_Color.colorValue, true);

            draw_rect.Set(rect.width - 280, rect.y + 150, (rect.width / 2) - 55, 20);
            sp_Color.colorValue = Editor_XHud_GUI.Gui_ColorField(draw_rect, sp_Color.colorValue);

            string col_string = XHud_Utilitys.Color_To_String(sp_Color.colorValue);
            string col_hex = XHud_Utilitys.Color_To_HexColor(sp_Color.colorValue, true);
            float[] col_array = XHud_Utilitys.Color_To_FloatArray(sp_Color.colorValue);

            draw_rect.Set(rect.width - 278, rect.y + 190, (rect.width / 2) - 55, 20);
            Editor_XHud_GUI.Gui_Labelfield(draw_rect, $"颜色字符串 ：{col_string}", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true);
            draw_rect.Set(rect.width - 278, rect.y + 220, (rect.width / 2) - 55, 20);
            Editor_XHud_GUI.Gui_Labelfield(draw_rect, $"16进制颜色 ：{col_hex}", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true);
            draw_rect.Set(rect.width - 278, rect.y + 250, (rect.width / 2) - 55, 20);
            Editor_XHud_GUI.Gui_Labelfield(draw_rect, "通道颜色 ：", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true);
            draw_rect.Set(rect.width - 265, rect.y + 275, (rect.width / 2) - 55, 15);
            Editor_XHud_GUI.Gui_Labelfield(draw_rect, $"R: {col_array[0].ToString("F2")}        G: {col_array[1].ToString("F2")}        B: {col_array[2].ToString("F2")}        A: {col_array[3].ToString("F2")}", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true);

            draw_rect.Set(rect.width - 278, rect.y + 275, 4, 12);
            Editor_XHud_GUI.Gui_Box(draw_rect, new Color(col_array[0], 0, 0));
            draw_rect.Set(rect.width - 215, rect.y + 275, 4, 12);
            Editor_XHud_GUI.Gui_Box(draw_rect, new Color(0, col_array[1], 0));
            draw_rect.Set(rect.width - 148, rect.y + 275, 4, 12);
            Editor_XHud_GUI.Gui_Box(draw_rect, new Color(0, 0, col_array[2]));
            draw_rect.Set(rect.width - 85, rect.y + 275, 4, 12);
            Editor_XHud_GUI.Gui_Box(draw_rect, new Color(1, 1, 1, col_array[3]));

            #region 入库名称
            Color LibName_color = Color.white;
            if (string.IsNullOrEmpty(sp_ColorName.stringValue))
            {
                LibName_color = Color.gray;
                sp_ColorName.stringValue = "颜色名称";
            }
            else
            {
                if (sp_ColorName.stringValue == "颜色名称")
                    LibName_color = Color.gray;
                else
                    LibName_color = Color.white;
            }

            draw_rect.Set(rect.width - 280, rect.height - 230, (rect.width / 2) - 55, 50);
            sp_ColorName.stringValue = Editor_XHud_GUI.Gui_TextField(draw_rect, sp_ColorName.stringValue, LibName_color, 12);
            sp_ColorName.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 说明文字
            Color Description_Color = Color.white;
            if (string.IsNullOrEmpty(sp_ColorDescription.stringValue))
            {
                Description_Color = Color.gray;
                sp_ColorDescription.stringValue = "颜色说明内容";
            }
            else
            {
                if (sp_ColorDescription.stringValue == "颜色说明内容")
                    Description_Color = Color.gray;
                else
                    Description_Color = Color.white;
            }
            draw_rect.Set(rect.width - 280, rect.height - 175, (rect.width / 2) - 55, 60);
            sp_ColorDescription.stringValue = Editor_XHud_GUI.Gui_TextField(draw_rect, sp_ColorDescription.stringValue, Description_Color, 12);
            sp_ColorDescription.serializedObject.ApplyModifiedProperties();

            #endregion

            #endregion

            #region 情绪说明
            string tone = GetColorTone(sp_Color.colorValue);
            XHud_Emotions emo = GetEmotions(tone);
            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 30, rect.height - 100, 200, 15), $"色系：<color={colorhex}> {tone} </color>", HudFilled.无, HudColor.无, Color.white, false, Color.blue, TextAnchor.UpperLeft, Vector2.zero, 15, Font_Bold);

            Editor_XHud_GUI.Gui_Labelfield_Thin(new Rect(rect.x + 30, rect.height - 75, 200, 15), $"+ ：{emo.Pos}", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.UpperLeft, new Vector2(0, 0), 12, false, false, true);

            Editor_XHud_GUI.Gui_Labelfield_Thin(new Rect(rect.x + 30, rect.height - 45, 200, 15), $"- ：{emo.Neg}", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.UpperLeft, new Vector2(0, 0), 12, false, false, true);
            #endregion

            #region 预览
            Rect rect_preview = new Rect(rect.x + 25, rect.y + 150, 300, 265);
            Editor_XHud_GUI.Gui_Group(rect_preview, HudFilled.纯色边框, HudColor.亮白, "着色效果预览", new Vector2(25, -8), Color.white, Font_Light);
            float margin = 10;

            draw_rect.Set(rect_preview.x + 10, rect_preview.y + 10, rect_preview.width - 20, rect_preview.height - 20);
            if (UseBg)
            {
                #region 预览背景
                if (XHud_Utilitys.GetBrightnessLimite(sp_Color.colorValue, 0.2f))
                {
                    PreviewBgColor = Color.black;
                }
                else
                {
                    PreviewBgColor = Color.white * 0.88f;
                }
                Editor_XHud_GUI.Gui_Box(draw_rect, PreviewBgColor);
                #endregion

            }
            #region 预览图片（尺寸300 x 265）
            GUI.backgroundColor = sp_Color.colorValue;
            Editor_XHud_GUI.Gui_Icon(draw_rect, PreviewTex);
            GUI.backgroundColor = Color.white;
            #endregion

            #region 背景切换
            draw_rect.Set(rect_preview.width / 2 - 6, rect_preview.y + 198 + margin, 64, 64);
            string bgstatu = "显示背景";
            if (UseBg)
            {
                bgstatu = "显示背景";
            }
            else
            {
                bgstatu = "无背景";
            }
            if (Editor_XHud_GUI.Gui_Button(draw_rect, null, null, TextAnchor.MiddleCenter, false, bgstatu, "", Color.clear, Color.white, HudFilled.透明, 12))
            {
                UseBg = !UseBg;
            }
            #endregion

            #region 翻页按钮           
            draw_rect.Set(rect_preview.x + 5, rect_preview.y + 198 + margin, 64, 64);
            if (Editor_XHud_GUI.Gui_Button(draw_rect, leftarr_r, leftarr_p, true, "", "", Color.white))
            {
                if (PreviewData_Index <= 0)
                {
                    PreviewData_Index = PreviewDatas.Length - 1;
                }
                else

                {
                    PreviewData_Index--;
                }
                StopPreviewUpdate();
                StartPreviewUpdate();
                Editor_XHud_GUI.EditorData_Set_With_Int("XED_HudAnimator_Set_previewtexs_index", PreviewData_Index);
            }

            draw_rect.Set(rect_preview.width - 45, rect_preview.y + 198 + margin, 64, 64);
            if (Editor_XHud_GUI.Gui_Button(draw_rect, rightarr_r, rightarr_p, true, "", "", Color.white))
            {
                if (PreviewData_Index >= PreviewDatas.Length - 1)
                {
                    PreviewData_Index = 0;
                }
                else

                {
                    PreviewData_Index++;
                }
                StopPreviewUpdate();
                StartPreviewUpdate();
                Editor_XHud_GUI.EditorData_Set_With_Int("XED_HudAnimator_Set_previewtexs_index", PreviewData_Index);
            }
            #endregion

            GUI.backgroundColor = Color.white;
            #endregion

            BaseObject.ApplyModifiedProperties();

            Repaint();

            Editor_XHud_GUI.Gui_Layout_Space(480);

            DialogType_Buttons();

            Event e = Event.current;
            // 检测点击事件
            if (e.type == EventType.MouseDown)
            {
                draw_rect.Set(rect.width - 280, rect.height - 175, (rect.width / 2) - 55, 60);
                // 检查点击位置是否在窗口内
                if (!draw_rect.Contains(e.mousePosition))
                {
                    GUI.FocusControl(null); // 取消所有控件的焦点
                    Repaint(); // 重新绘制窗口
                }
                draw_rect.Set(rect.width - 280, rect.height - 230, (rect.width / 2) - 55, 50);
                // 检查点击位置是否在窗口内
                if (!draw_rect.Contains(e.mousePosition))
                {
                    GUI.FocusControl(null); // 取消所有控件的焦点
                    Repaint(); // 重新绘制窗口
                }
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }
        }

        #region 辅助
        /// <summary>
        /// 发送到元素库
        /// </summary>
        private void SendToLibrary()
        {
            if (sp_ColorName.stringValue == "颜色名称")
            {
                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 色卡库采集器消息", "未填写名称", "请为色卡添加一个名称！", "明白");
                return;
            }

            string colorhex = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);
            bool exist = Target_Hud_ColorsLibrary.ColorsLibrary_IsExist(sp_ColorName.stringValue);

            if (exist)
            {
                string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 色卡库采集器消息", "存在重复色卡名称", $"名称为<color={colorhex}> {sp_ColorName.stringValue} </color>的色卡已经存在于色卡库中！", "重命名", 0);
                if (res == "重命名")
                {
                    return;
                }
            }
            else
            {
                xHud_LibraryArg_Color info = new xHud_LibraryArg_Color();
                info.Name = sp_ColorName.stringValue;
                info.Description = sp_ColorDescription.stringValue;
                info.Color = sp_Color.colorValue;
                Target_Hud_ColorsLibrary.ColorsLibrary_AddColor(info);

                Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 色卡库采集器消息", "已添加到色卡库", $"已将名称为<color={colorhex}> {sp_ColorName.stringValue} </color>的色卡添加到色卡库中！", "明白");
                Close();

                string res = Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 色卡库采集器消息", "存入色卡库", "色卡存入完成，是否要打开色卡库进行查看？", "不用", "查看色卡库", 0);
                if (res == "查看色卡库")
                {
                    EditorUtility.OpenPropertyEditor(Target_Hud_ColorsLibrary);
                }
            }

            string indicator = SourcePainting.controller.GetIndicator();

            string res_saved_turnon = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 色卡库采集器消息", "色卡库模式设定", $"是否要将 {(string.IsNullOrEmpty(indicator) ? SourcePainting.transform.name : indicator)} 色卡模式开启？", "开启", "暂不", 0);
            if (res_saved_turnon == "开启")
            {
                SourcePainting.SyncLibraryColor = true;
                int id = Target_Hud_ColorsLibrary.ColorsLibrary_GetColorCount() - 1;
                SourcePainting.ColoriseName = Target_Hud_ColorsLibrary.ColorsLibrary_GetColorNames()[id];
            }
            this.Close();
        }

        private void UpdateToLibrary()
        {
            string colorhex = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            xHud_LibraryArg_Color info = new xHud_LibraryArg_Color();
            info.Name = sp_ColorName.stringValue;
            info.Description = sp_ColorDescription.stringValue;
            info.Color = sp_Color.colorValue;
            Target_Hud_ColorsLibrary.ColorsLibrary_ReplaceColorAndName(ModifiedIndex, sp_OriginColorName.stringValue, sp_ColorName.stringValue, sp_Color.colorValue, sp_ColorDescription.stringValue);

            Close();
        }

        /// <summary>
        /// 获取目标文件夹下的指定类型所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        List<T> LoadAllAssetsAtPathWithPattern<T>(string path, string pattern) where T : UnityEngine.Object
        {
            List<T> _out = new();

            string root_path = Directory.GetParent(Application.dataPath) + "/" + path;

            if (!Directory.Exists(root_path))
            {
                Debug.LogWarning("Path doesn't exist");
                return _out;
            }

            string[] fileEntries = Directory.GetFiles(root_path, $"*{pattern}");

            foreach (string FileName in fileEntries)
            {
                string[] filepath = FileName.Split(Directory.GetParent(Application.dataPath).FullName + "/");
                _out.Add(AssetDatabase.LoadAssetAtPath<T>(filepath[1]));
            }

            return _out;
        }

        /// <summary>
        /// 设置按钮文字
        /// </summary>
        /// <param name="ok"></param>
        /// <param name="cancel"></param>
        public void SetButtonText(string ok, string cancel)
        {
            ButtonText_Cancel = cancel;
            ButtonText_Ok = ok;
        }

        /// <summary>
        /// 设置模版基础信息（针对从库源修改更新使用）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="decription"></param>
        /// <param name="color"></param>
        public void SetInfo(string name, string decription, Color color)
        {
            sp_ColorName.stringValue = name;
            sp_ColorDescription.stringValue = decription;
            sp_Color.colorValue = color;

            sp_ColorName.serializedObject.ApplyModifiedProperties();
            sp_ColorDescription.serializedObject.ApplyModifiedProperties();
            sp_Color.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            Title = title;
        }

        /// <summary>
        /// 设置原始名称
        /// </summary>
        /// <param name="name"></param>
        public void Set_OriginColorName(string name)
        {
            OriginColorName = name;
        }

        /// <summary>
        /// 设置为添加到库模式还是修改库源参数模式
        /// </summary>
        /// <param name="mode"></param>
        public void SetLibrarySetterMode(LibrarySetterMode mode)
        {
            LibrarySetterMode = mode;
        }

        /// <summary>
        /// 设置源
        /// </summary>
        /// <param name="painting"></param>
        public void SetPainting(XHud_Module_Primitive_Painting painting)
        {
            SourcePainting = painting;
        }

        /// <summary>
        /// 设置目标库
        /// </summary>
        /// <param name="lib"></param>
        public void SetTarget_Hud_ColorsLibrary(XHud_Library_Colors lib)
        {
            Target_Hud_ColorsLibrary = lib;
        }

        /// <summary>
        /// 控件按钮
        /// </summary>
        private void DialogType_Buttons()
        {
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();

            if (Editor_XHud_GUI.Gui_Layout_Button(ButtonText_Cancel, "", HudFilled.实体, HudColor.亮白, Color.black, 12, ButtonWidth, ButtonHeight, Font_Light, ButtonText_Cancel))
            {
                Close();
            }
            Editor_XHud_GUI.Gui_Layout_Space(ButtonDistance);
            GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
            if (Editor_XHud_GUI.Gui_Layout_Button(ButtonText_Ok, "", HudFilled.实体, HudColor.亮白, XHud_Utilitys.GetBrightnessLimite(XHud_Dashboard.Theme_Primary) ? Color.black : Color.white, 12, ButtonWidth, ButtonHeight, Font_Light, ButtonText_Ok))
            {
                if (LibrarySetterMode == LibrarySetterMode.添加到库)
                {
                    SendToLibrary();
                }
                else if (LibrarySetterMode == LibrarySetterMode.修改库源参数)
                {
                    UpdateToLibrary();
                }
                return;
            }
            GUI.backgroundColor = Color.white;

            Editor_XHud_GUI.Gui_Layout_Space(25);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
        }

        // 将 RGB 转换为 HSV
        private static void RGBToHSV(Color color, out float hue, out float saturation, out float value)
        {
            float r = color.r;
            float g = color.g;
            float b = color.b;

            float max = Mathf.Max(r, g, b);
            float min = Mathf.Min(r, g, b);
            float delta = max - min;

            hue = 0;
            if (delta != 0)
            {
                if (max == r)
                {
                    hue = (g - b) / delta;
                }
                else if (max == g)
                {
                    hue = 2 + (b - r) / delta;
                }
                else
                {
                    hue = 4 + (r - g) / delta;
                }

                hue *= 60;
                if (hue < 0) hue += 360;
            }

            saturation = max == 0 ? 0 : delta / max;
            value = max;
        }

        public string GetColorTone(Color color)
        {
            float hue, saturation, value;
            RGBToHSV(color, out hue, out saturation, out value);

            // 根据饱和度和明度判断是否为灰色系、黑色系或白色系
            if (saturation <= 0f) // 低饱和度
            {
                if (value > 0.8f) return "白色系"; // 高明度
                if (value < 0.2f) return "黑色系"; // 低明度
                return "灰色系"; // 中等明度
            }

            // 根据色相和色环范围判断颜色的色系
            if (hue >= 0 && hue < 10) return "枫叶红"; // 0°-10°
            if (hue >= 10 && hue < 20) return "石榴红"; // 10°-20°
            if (hue >= 20 && hue < 30) return "红砂岩"; // 20°-30°
            if (hue >= 30 && hue < 40) return "红珊瑚"; // 30°-40°
            if (hue >= 40 && hue < 50) return "橙砂岩"; // 40°-50°
            if (hue >= 50 && hue < 60) return "琥珀橙"; // 50°-60°
            if (hue >= 60 && hue < 70) return "橙珊瑚"; // 60°-70°
            if (hue >= 70 && hue < 80) return "橙石竹"; // 70°-80°
            if (hue >= 80 && hue < 90) return "金菊黄"; // 80°-90°
            if (hue >= 90 && hue < 100) return "玉兰黄"; // 90°-100°
            if (hue >= 100 && hue < 110) return "石竹黄"; // 100°-110°
            if (hue >= 110 && hue < 120) return "水晶黄"; // 110°-120°
            if (hue >= 120 && hue < 130) return "竹影绿"; // 120°-130°
            if (hue >= 130 && hue < 140) return "松石绿"; // 130°-140°
            if (hue >= 140 && hue < 150) return "橄榄绿"; // 140°-150°
            if (hue >= 150 && hue < 160) return "苔藓绿"; // 150°-160°
            if (hue >= 160 && hue < 170) return "青石蓝"; // 160°-170°
            if (hue >= 170 && hue < 180) return "天青蓝"; // 170°-180°
            if (hue >= 180 && hue < 190) return "冬青蓝"; // 180°-190°
            if (hue >= 190 && hue < 200) return "梅青蓝"; // 190°-200°
            if (hue >= 200 && hue < 210) return "莓果蓝"; // 200°-210°
            if (hue >= 210 && hue < 220) return "宝石蓝"; // 210°-220°
            if (hue >= 220 && hue < 230) return "鸢尾蓝"; // 220°-230°
            if (hue >= 230 && hue < 240) return "海藻蓝"; // 230°-240°
            if (hue >= 240 && hue < 250) return "罗兰紫"; // 240°-250°
            if (hue >= 250 && hue < 260) return "紫晶紫"; // 250°-260°
            if (hue >= 260 && hue < 270) return "丁香紫"; // 260°-270°
            if (hue >= 270 && hue < 280) return "玫瑰紫"; // 270°-280°
            if (hue >= 280 && hue < 290) return "粉晶紫"; // 280°-290°
            if (hue >= 290 && hue < 300) return "玫瑰粉"; // 290°-300°
            if (hue >= 300 && hue < 310) return "桃花粉"; // 300°-310°
            if (hue >= 310 && hue < 320) return "热粉红"; // 310°-320°
            if (hue >= 320 && hue < 330) return "草莓粉"; // 320°-330°
            if (hue >= 330 && hue < 340) return "胭脂粉"; // 330°-340°
            if (hue >= 340 && hue < 350) return "肉色粉"; // 340°-350°
            if (hue >= 350 && hue < 360) return "浅粉红"; // 350°-360°

            return "未知色";
        }

        public XHud_Emotions GetEmotions(string col)
        {
            XHud_Emotions emo = new XHud_Emotions();
            for (int i = 0; i < ColorEmotions.Colors.Count; i++)
            {
                if (col == ColorEmotions.Colors[i].ColorName)
                {
                    for (int s = 0; s < ColorEmotions.Colors[i].Pos.Count; s++)
                    {
                        if (s == ColorEmotions.Colors[i].Pos.Count - 1)
                            emo.Pos += ColorEmotions.Colors[i].Pos[s];
                        else
                            emo.Pos += ColorEmotions.Colors[i].Pos[s] + "  /  ";
                    }
                    for (int s = 0; s < ColorEmotions.Colors[i].Neg.Count; s++)
                    {
                        if (s == ColorEmotions.Colors[i].Neg.Count - 1)
                            emo.Neg += ColorEmotions.Colors[i].Neg[s];
                        else
                            emo.Neg += ColorEmotions.Colors[i].Neg[s] + "  /  ";
                    }
                }
            }

            return emo;
        }

        /// <summary>
        /// 开始预览
        /// </summary>
        public void StartPreviewUpdate()
        {
            PreviewCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(PreviewUpdater());
        }

        /// <summary>
        /// 停止预览
        /// </summary>
        public void StopPreviewUpdate()
        {
            EditorCoroutineUtility.StopCoroutine(PreviewCoroutine);
        }

        IEnumerator PreviewUpdater()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            string path = $"{XHud_Dashboard.Get_Path_XHUD_GUISTYLE_Path()}Icon/Icons_XHud_Library_Color_Setter/samples";
            //获取预览序列帧
            PreviewDatas = LoadAllAssetsAtPathWithPattern<XHud_LibrarySetTool_Color_PreviewData>(path, ".asset").ToArray();

            var waitForOneSecond = new EditorWaitForSeconds(PreviewDataDuration);
            while (true)
            {
                for (int i = 0; i < PreviewDatas[PreviewData_Index].Textures.Count; i++)
                {
                    PreviewTex = PreviewDatas[PreviewData_Index].Textures[i];
                    yield return waitForOneSecond;
                }
            }
        }
        #endregion
    }
}