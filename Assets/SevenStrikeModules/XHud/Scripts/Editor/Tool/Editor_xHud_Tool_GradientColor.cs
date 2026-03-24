namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public class Editor_xHud_Tool_GradientColor : EditorWindow
    {
        private SerializedObject BaseObject;
        private SerializedProperty sp_ColorGradient, sp_Step, sp_Color_Start, sp_Color_End;

        /// <summary>
        /// 提取颜色列表
        /// </summary>
        public ReorderableList ReorderableList_GradientColors;

        private static Editor_xHud_Tool_GradientColor window;

        [SerializeField]
        public List<Color> ColorGradient = new List<Color>();

        private Texture2D logo, btn_save_r, btn_save_p;

        [SerializeField]
        private int Step = 20;
        [SerializeField]
        Color Color_Start = xHud_Dashboard.Theme_Primary;
        [SerializeField]
        Color Color_End = Color.black;

        Color SepLineColor = new Color(1, 1, 1, 0.15f);
        Color MessageColor = new Color(1, 1, 1, 0.62f);

        Rect Sepline_rect;
        Rect Title_rect;
        Rect Icon_rect;

        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 26;
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 20;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 GradientColors_Scroller;

        /// <summary>
        /// 选中项索引号
        /// </summary>
        public int SelectedIndex = 0;

        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;

        [MenuItem("Tools/XHud/ColorGradientTools #g")]
        static void Init()
        {
            window = (Editor_xHud_Tool_GradientColor)EditorWindow.GetWindow(typeof(Editor_xHud_Tool_GradientColor), true, "XHUD渐变色卡生成工具", true);
            Editor_xHudGUI.CenterEditorWindow(new Vector2Int(360, 800), window);
            window.Show();
        }

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);
            sp_ColorGradient = BaseObject.FindProperty("ColorGradient");
            sp_Step = BaseObject.FindProperty("Step");
            sp_Color_Start = BaseObject.FindProperty("Color_Start");
            sp_Color_End = BaseObject.FindProperty("Color_End");

            logo = Editor_xHudGUI.GetIcon("Icons_ColorCaptureTool/logo");
            btn_save_r = Editor_xHudGUI.GetIcon("Icons_ColorCaptureTool/btn_save_r");
            btn_save_p = Editor_xHudGUI.GetIcon("Icons_ColorCaptureTool/btn_save_p");

            Font_Bold = Editor_xHudGUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_xHudGUI.GetFont("SS_Editor_Dialog");

            #region ReorderableList - 色表
            ReorderableList_GradientColors = new ReorderableList(BaseObject, sp_ColorGradient, false, false, false, false);
            ReorderableList_GradientColors.drawElementCallback = GradientColors_DrawElementCallback;
            #endregion

            CreateGradient(Color_Start, Color_End, Step);
        }

        private void OnDestroy()
        {
            GetWindow<SceneView>().Focus();
        }

        #region 绘制渐变颜色列表元素

        Rect index_rect;
        Rect scrollview_rect;
        Rect item_rect;
        Rect itemmark_rect;
        Rect itemmarkBg_rect;

        /// <summary>
        /// 绘制列表项 - 提取色
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void GradientColors_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            index_rect = new Rect(rect.x + 15, rect.y + 5, 30, 20);

            SerializedProperty prop = sp_ColorGradient.GetArrayElementAtIndex(index);

            Editor_xHudGUI.Gui_Labelfield(index_rect, index.ToString("D2"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 40, rect.y + 3, rect.width - 40 - 20, 20), "", "", prop);
        }

        /// <summary>
        /// 列表显示 - 提取色
        /// </summary>
        private void GradientColors_Drawer()
        {
            // 绘制滚动视图
            scrollview_rect = new Rect(15, 220, 325, 550);
            GradientColors_Scroller = GUI.BeginScrollView(scrollview_rect, GradientColors_Scroller, new Rect(0, 0, scrollview_rect.width - 325, ColorGradient.Count * itemHeight), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(GradientColors_Scroller.y / itemHeight);
            int endIndex = Mathf.CeilToInt((GradientColors_Scroller.y + scrollview_rect.height) / itemHeight);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < ReorderableList_GradientColors.count; i++)
            {
                SerializedProperty prop = sp_ColorGradient.GetArrayElementAtIndex(i);

                item_rect = new Rect(0, i * itemHeight, scrollview_rect.width, itemHeight);

                // 如果当前元素被选中，绘制选中效果
                if (SelectedIndex == i)
                {
                    itemmark_rect = new Rect(item_rect.x + 1, item_rect.y + 11, 5, 5);
                    // 高亮标记表示选中
                    EditorGUI.DrawRect(itemmark_rect, xHud_Dashboard.Theme_Primary);
                    itemmarkBg_rect = new Rect(item_rect.x, item_rect.y, item_rect.width + 20, item_rect.height);
                    // 高亮背景表示选中
                    EditorGUI.DrawRect(itemmarkBg_rect, new Color(0, 0, 0, 0.2f));
                }

                ReorderableList_GradientColors.drawElementCallback.Invoke(item_rect, i, i == ReorderableList_GradientColors.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        // 更新选中项
                        SelectedIndex = i;

                        // 标记界面需要更新
                        GUI.changed = true;
                    }
                }
            }

            GUI.EndScrollView();
        }
        #endregion

        private void OnGUI()
        {
            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

            BaseObject.Update();

            #region 抬头
            Rect rect = new Rect(0, 0, position.width, position.height);

            Icon_rect = new Rect(15, 15, 48, 48);

            Editor_xHudGUI.Gui_Icon(Icon_rect, logo);

            Title_rect = new Rect(rect.x + 85, rect.y + 15, rect.width - 80, 30);
            Editor_xHudGUI.Gui_Labelfield(Title_rect, "XHUD渐变色卡生成工具", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 20, Font_Bold);

            Sepline_rect = new Rect(rect.x + 85, rect.y + 60, 200, 1);
            Editor_xHudGUI.Gui_Box(Sepline_rect, SepLineColor);

            Editor_xHudGUI.Gui_Labelfield_Thin_WrapClip(new Rect(rect.x + 18, rect.y + 80, rect.width - 38, rect.height), "此工具可根据指定的颜色来生成双色渐变色，并保存为色卡库来使用！", HudFilled.无, HudColor.无, MessageColor, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true, Font_Light);
            #endregion

            #region 参数
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 18, rect.y + 140, 100, 20), "起始色", sp_Color_Start, 0, 45);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 140, rect.y + 140, 100, 20), "结束色", sp_Color_End, 0, 45);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 18, rect.y + 175, 320, 20), "渐变步数", sp_Step, 0, 60);
            #endregion

            #region 计算按钮

            if (Editor_xHudGUI.Gui_Button(new Rect(rect.width - 45, rect.y + 140, 18, 18), btn_save_r, btn_save_p, true, "", "", Color.white))
            {
                SaveColors();
            }
            #endregion

            CreateGradient(Color_Start, Color_End, Step);

            GradientColors_Drawer();

            BaseObject.ApplyModifiedProperties();

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }
        }

        #region 生成渐变色

        private void CreateGradient(Color startColor, Color endColor, int steps)
        {
            ColorGradient.Clear();
            ColorGradient = RangeGradient_Creator(startColor, endColor, Step);
        }

        List<Color> RangeGradient_Creator(Color startColor, Color endColor, int steps)
        {
            List<Color> colors = new List<Color>();

            // 提取起始颜色和结束颜色的HSV值
            float startH, startS, startV;
            float endH, endS, endV;

            Color.RGBToHSV(startColor, out startH, out startS, out startV);
            Color.RGBToHSV(endColor, out endH, out endS, out endV);

            // 计算色调的最短路径
            float hueDistance = endH - startH;
            if (hueDistance > 0.5f) hueDistance -= 1.0f; // 逆时针
            if (hueDistance < -0.5f) hueDistance += 1.0f; // 顺时针

            for (int i = 0; i < steps; i++)
            {
                float t = (float)i / (steps - 1); // 插值参数，范围从0到1

                // 在HSV空间中插值
                float hue = startH + hueDistance * t;
                if (hue < 0) hue += 1.0f; // 环形处理
                if (hue > 1) hue -= 1.0f; // 环形处理

                float saturation = Mathf.Lerp(startS, endS, t);
                float value = Mathf.Lerp(startV, endV, t);

                // 将HSV转换回RGB
                Color color = Color.HSVToRGB(hue, saturation, value);
                colors.Add(color);
            }

            return colors;
        }
        #endregion

        #region 保存颜色为色卡
        /// <summary>
        /// 制成颜色库备用
        /// </summary>
        /// <param name="folder"></param>
        private void SaveColors()
        {
            string hexcol = xHud_Utilitys.Color_To_HexColor(xHud_Dashboard.Theme_Primary, true);

            string state = Editor_xHudGUI.Open(xHudDialogType.帮助, "ColorCaptureTool颜色提取工具通知", "导出为渐变色色卡库", $"是否要为当前的<color={hexcol}>渐变色列表</color>创建色卡库？", "创建", "暂不", 0);

            if (state == "创建")
            {
                string mode = Editor_xHudGUI.Open(xHudDialogType.帮助, "ColorCaptureTool颜色提取工具通知", "请选保存模式", $"如果选择替换模式，会将目标色卡库的所有色卡项全部清空替换！请谨慎操作！", "替换", "追加", "新增", 2);

                switch (mode)
                {
                    case "替换":
                        string path_op = EditorUtility.OpenFilePanel("选择需要替换的色卡库文件", Application.dataPath, "asset");
                        string path_op_folder = path_op.Substring(Application.dataPath.Length - 6);
                        xHud_Library_Colors lib_op = AssetDatabase.LoadAssetAtPath<xHud_Library_Colors>(path_op_folder);
                        lib_op.ColorLibrary.Clear();

                        for (int i = 0; i < ColorGradient.Count; i++)
                        {
                            lib_op.ColorsLibrary_AddColor(xHud_Utilitys.Color_To_HexColor(ColorGradient[i]), ColorGradient[i], "-");
                        }
                        break;
                    case "追加":
                        string path_add = EditorUtility.OpenFilePanel("选择需要追加的色卡库文件", Application.dataPath, "asset");
                        string path_add_folder = path_add.Substring(Application.dataPath.Length - 6);
                        xHud_Library_Colors lib_add = AssetDatabase.LoadAssetAtPath<xHud_Library_Colors>(path_add_folder);

                        for (int i = 0; i < ColorGradient.Count; i++)
                        {
                            lib_add.ColorsLibrary_AddColor(xHud_Utilitys.Color_To_HexColor(ColorGradient[i]), ColorGradient[i], "-");
                        }
                        break;
                    case "新增":
                        xHud_Library_Colors lib_color_gradient = ScriptableObject.CreateInstance<xHud_Library_Colors>();
                        lib_color_gradient.LibraryName = $"ColorsLibrary_Gradient";

                        //加入颜色到各个类型的色板中
                        for (int i = 0; i < ColorGradient.Count; i++)
                        {
                            lib_color_gradient.ColorsLibrary_AddColor("Gradient_" + i, ColorGradient[i], "-");
                        }

                        string path = EditorUtility.SaveFilePanel("", Application.dataPath, "", "asset");
                        string path_folder = path.Substring(Application.dataPath.Length - 6);

                        AssetDatabase.CreateAsset(lib_color_gradient, path_folder);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        break;
                }
            }
            else
            {
                return;
            }
        }
        #endregion
    }
}