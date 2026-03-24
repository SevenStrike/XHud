namespace SevenStrikeModules.XHud.Utilitys
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Hud;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Random = UnityEngine.Random;
    using RangeAttribute = UnityEngine.RangeAttribute;

    [System.Serializable]
    public class xHud_Tool_ColorCaptureNode
    {
        [SerializeField]
        public Color color_theme;
        [SerializeField]
        /// <summary>
        /// 互补色
        /// </summary>
        public Color color_complementary;
        [SerializeField]
        /// <summary>
        /// 近似色
        /// </summary>
        public Color color_approximate;
        [SerializeField]
        /// <summary>
        /// 类似色
        /// </summary>
        public Color color_analogous;
        [SerializeField]
        /// <summary>
        /// 低饱和色
        /// </summary>
        public Color color_advancedgray;
    }

    public enum xHud_Tool_CaptureColorType
    {
        主题色,
        互补色,
        近似色,
        类似色,
        高级灰
    }

    public class Editor_xHud_Tool_ColorCapture : EditorWindow
    {
        private SerializedObject BaseObject;
        private SerializedProperty sp_ReferImage, sp_ColorCaptureNode, sp_TargetColors, sp_MaxIterations, sp_ConvergenceThreshold, sp_SampleInterval, sp_ColorSimilarityThreshold, sp_ApproximateOffset, sp_AnalogousOffset, sp_GrayOffset, sp_GrayBright;

        /// <summary>
        /// 提取颜色列表
        /// </summary>
        public ReorderableList ReorderableList_ExtrctionColors;

        private static Editor_xHud_Tool_ColorCapture window;

        public List<xHud_Tool_ColorCaptureNode> ColorCaptureNode = new List<xHud_Tool_ColorCaptureNode>();

        #region 主题色
        public int TargetColors = 30; // 目标颜色数量
        public int MaxIterations = 20; // 最大迭代次数
        [Range(0, 1)]
        public float ConvergenceThreshold = 0.12f; // 收敛阈值
        [Range(1, 100)]
        public int SampleInterval = 15; // 采样间隔
        [Range(0, 1)]
        public float ColorSimilarityThreshold = 0.02f; // 颜色相似度阈值（0到1之间）
        #endregion

        #region 近似色
        [Range(0, 360)]
        public float ApproximateOffset = 30f;
        #endregion

        #region 类似色
        [Range(0, 360)]
        public float Analogousffset = 60f;
        #endregion

        #region 高级灰色
        [Range(0, 1)]
        public float GrayOffset = 0.45f;
        [Range(0, 1)]
        public float GrayBright = 0.72f;
        #endregion

        private Texture2D logo, btn_ext_color_r, btn_ext_color_p, btn_save_r, btn_save_p, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p;

        [SerializeField]
        private Texture2D ReferImage;

        [SerializeField]
        private Texture2D[] ReferImages;

        Color SepLineColor = new Color(1, 1, 1, 0.15f);
        Color MessageColor = new Color(1, 1, 1, 0.62f);

        Rect Sepline_rect;
        Rect Title_rect;
        Rect Icon_rect;

        private bool UseCustomTexture;

        private int referIndex = 0;

        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 25;
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 8;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 ExtrctionColors_Scroller;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 ConvertColors_Scroller;

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

        [MenuItem("Tools/XHud/ColorCaptureTools #x")]
        static void Init()
        {
            window = (Editor_xHud_Tool_ColorCapture)EditorWindow.GetWindow(typeof(Editor_xHud_Tool_ColorCapture), true, "XHUD图片色调提取工具", true);
            Editor_xHudGUI.CenterEditorWindow(new Vector2Int(800, 620), window);
            window.Show();
        }

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);
            sp_ReferImage = BaseObject.FindProperty("ReferImage");
            sp_ColorCaptureNode = BaseObject.FindProperty("ColorCaptureNode");
            sp_TargetColors = BaseObject.FindProperty("TargetColors");
            sp_MaxIterations = BaseObject.FindProperty("MaxIterations");
            sp_ConvergenceThreshold = BaseObject.FindProperty("ConvergenceThreshold");
            sp_SampleInterval = BaseObject.FindProperty("SampleInterval");
            sp_ColorSimilarityThreshold = BaseObject.FindProperty("ColorSimilarityThreshold");
            sp_ApproximateOffset = BaseObject.FindProperty("ApproximateOffset");
            sp_AnalogousOffset = BaseObject.FindProperty("Analogousffset");
            sp_GrayOffset = BaseObject.FindProperty("GrayOffset");
            sp_GrayBright = BaseObject.FindProperty("GrayBright");

            logo = Editor_xHudGUI.GetIcon("Icons_ColorCaptureTool/logo");
            btn_ext_color_r = Editor_xHudGUI.GetIcon("Icons_ColorCaptureTool/btn_ext_color_r");
            btn_ext_color_p = Editor_xHudGUI.GetIcon("Icons_ColorCaptureTool/btn_ext_color_p");
            btn_save_r = Editor_xHudGUI.GetIcon("Icons_ColorCaptureTool/btn_save_r");
            btn_save_p = Editor_xHudGUI.GetIcon("Icons_ColorCaptureTool/btn_save_p");

            left_arrow_r = Editor_xHudGUI.GetIcon("Icons_ColorCaptureTool/left_arrow_r");
            left_arrow_p = Editor_xHudGUI.GetIcon("Icons_ColorCaptureTool/left_arrow_p");
            right_arrow_r = Editor_xHudGUI.GetIcon("Icons_ColorCaptureTool/right_arrow_r");
            right_arrow_p = Editor_xHudGUI.GetIcon("Icons_ColorCaptureTool/right_arrow_p");


            Font_Bold = Editor_xHudGUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_xHudGUI.GetFont("SS_Editor_Dialog");

            ReferImages = new Texture2D[10];
            for (int i = 0; i < ReferImages.Length; i++)
            {
                ReferImages[i] = AssetDatabase.LoadAssetAtPath<Texture2D>($"{xHud_Dashboard.Get_GUIStyle_Path()}Icon/Icons_ColorCaptureTool/ReferImages/ReferImg_{i}.png");
            }

            ReferImage = ReferImages[referIndex];

            #region ReorderableList - 色表
            ReorderableList_ExtrctionColors = new ReorderableList(BaseObject, sp_ColorCaptureNode, false, false, false, false);
            ReorderableList_ExtrctionColors.drawElementCallback = ExtrctionColors_DrawElementCallback;
            #endregion

            TextureColors_Get();
        }

        private void OnDestroy()
        {
            GetWindow<SceneView>().Focus();
        }

        #region 绘制提取颜色列表元素

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
        private void ExtrctionColors_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            index_rect = new Rect(rect.x + 15, rect.y + 5, 30, 20);

            SerializedProperty prop = sp_ColorCaptureNode.GetArrayElementAtIndex(index);
            SerializedProperty sp_color_theme = prop.FindPropertyRelative("color_theme");
            SerializedProperty sp_color_approximate = prop.FindPropertyRelative("color_approximate");
            SerializedProperty sp_color_complementary = prop.FindPropertyRelative("color_complementary");
            SerializedProperty sp_color_analogous = prop.FindPropertyRelative("color_analogous");
            SerializedProperty sp_color_advancedgray = prop.FindPropertyRelative("color_advancedgray");


            Editor_xHudGUI.Gui_Labelfield(index_rect, index.ToString("D2"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);

            float interval = 35;

            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 40, rect.y, 45, 20), "", "", sp_color_theme);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 80 + interval, rect.y, 45, 20), "", "", sp_color_complementary);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 120 + interval * 2, rect.y, 45, 20), "", "", sp_color_approximate);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 160 + interval * 3, rect.y, 45, 20), "", "", sp_color_analogous);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 200 + interval * 4, rect.y, 45, 20), "", "", sp_color_advancedgray);
        }

        /// <summary>
        /// 列表显示 - 提取色
        /// </summary>
        private void ExtrctionColors_Drawer()
        {
            // 绘制滚动视图
            scrollview_rect = new Rect(355, 160, 426, 428);
            ExtrctionColors_Scroller = GUI.BeginScrollView(scrollview_rect, ExtrctionColors_Scroller, new Rect(0, 0, scrollview_rect.width - 400, ColorCaptureNode.Count * itemHeight), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(ExtrctionColors_Scroller.y / itemHeight);
            int endIndex = Mathf.CeilToInt((ExtrctionColors_Scroller.y + scrollview_rect.height) / itemHeight);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < ReorderableList_ExtrctionColors.count; i++)
            {
                SerializedProperty prop = sp_ColorCaptureNode.GetArrayElementAtIndex(i);

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

                ReorderableList_ExtrctionColors.drawElementCallback.Invoke(item_rect, i, i == ReorderableList_ExtrctionColors.index, true);

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
            Editor_xHudGUI.Gui_Labelfield(Title_rect, "XHUD图片色调提取工具", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 20, Font_Bold);

            Sepline_rect = new Rect(rect.x + 85, rect.y + 60, 200, 1);
            Editor_xHudGUI.Gui_Box(Sepline_rect, SepLineColor);

            Editor_xHudGUI.Gui_Labelfield_Thin_WrapClip(new Rect(rect.x + 18, rect.y + 80, rect.width - 38, rect.height), "此工具可根据指定的图片来分析出主题色调，并且通过主题色调分离并拓展出：互补色、近似色、类似色和高级灰色来提供颜色创意，按键：G 再次生成", HudFilled.无, HudColor.无, MessageColor, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true, Font_Light);
            #endregion

            #region 参考图片 / 参数
            Editor_xHudGUI.Gui_Icon(new Rect(rect.x + 18, rect.y + 140, 320, 190), ReferImage);

            EditorGUI.BeginChangeCheck();
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 18, rect.y + 340, 320, 20), "参考图片", sp_ReferImage, 0, 110);
            if (EditorGUI.EndChangeCheck())
            {
                UseCustomTexture = true;
            }

            if (!UseCustomTexture)
            {
                #region 图片控件
                if (Editor_xHudGUI.Gui_Button(new Rect(rect.x + 20, rect.y + 290, 32, 32), left_arrow_r, left_arrow_p, true, "", "", Color.white))
                {
                    if (referIndex <= 0)
                        referIndex = ReferImages.Length - 1;
                    else
                        referIndex--;

                    ReferImage = ReferImages[referIndex];
                    TextureColors_Get();
                }

                if (Editor_xHudGUI.Gui_Button(new Rect(rect.x + 300, rect.y + 290, 32, 32), right_arrow_r, right_arrow_p, true, "", "", Color.white))
                {
                    if (referIndex >= ReferImages.Length - 1)
                        referIndex = 0;
                    else
                        referIndex++;
                    ReferImage = ReferImages[referIndex];
                    TextureColors_Get();
                }
                #endregion
            }

            Editor_xHudGUI.Gui_Box(new Rect(rect.x + 18, rect.y + 375, 320, 1), Color.gray * 0.75f);

            float Added = 30;
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 18, rect.y + 365 + Added, 150, 20), "提取数量上限", sp_TargetColors, 0, 90);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 188, rect.y + 365 + Added, 150, 20), "最大迭代色", sp_MaxIterations, 0, 80);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 18, rect.y + 390 + Added, 320, 20), "收敛阈值", sp_ConvergenceThreshold, 0, 110);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 18, rect.y + 415 + Added, 320, 20), "采样间隔", sp_SampleInterval, 0, 110);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 18, rect.y + 440 + Added, 320, 20), "颜色相似阈值", sp_ColorSimilarityThreshold, 0, 110);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 18, rect.y + 465 + Added, 320, 20), "近似色偏移", sp_ApproximateOffset, 0, 110);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 18, rect.y + 490 + Added, 320, 20), "类似色偏移", sp_AnalogousOffset, 0, 110);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 18, rect.y + 515 + Added, 320, 20), "高级灰浓度", sp_GrayOffset, 0, 110);
            Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 18, rect.y + 540 + Added, 320, 20), "高级灰明度", sp_GrayBright, 0, 110);
            #endregion

            #region 计算按钮
            if (Editor_xHudGUI.Gui_Button(new Rect(rect.width - 60, rect.y + 20, 18, 18), btn_ext_color_r, btn_ext_color_p, true, "", "", Color.white))
            {
                TextureColors_Get();
            }

            if (Editor_xHudGUI.Gui_Button(new Rect(rect.width - 110, rect.y + 20, 18, 18), btn_save_r, btn_save_p, true, "", "", Color.white))
            {
                SaveColors();
            }
            #endregion

            #region 颜色列表标题
            float baseoffset = rect.x + 385;
            float interval = 75.5f;

            Editor_xHudGUI.Gui_Labelfield_Thin_WrapClip(new Rect(baseoffset, rect.y + 120, 60, 25), "主题色", HudFilled.无, HudColor.无, xHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, new Vector2(0, 0), 12, true, Font_Light);

            Editor_xHudGUI.Gui_Labelfield_Thin_WrapClip(new Rect(baseoffset + interval, rect.y + 120, 60, 25), "互补色", HudFilled.无, HudColor.无, xHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, new Vector2(0, 0), 12, true, Font_Light);

            Editor_xHudGUI.Gui_Labelfield_Thin_WrapClip(new Rect(baseoffset + interval * 2, rect.y + 120, 60, 25), "近似色", HudFilled.无, HudColor.无, xHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, new Vector2(0, 0), 12, true, Font_Light);

            Editor_xHudGUI.Gui_Labelfield_Thin_WrapClip(new Rect(baseoffset + interval * 3, rect.y + 120, 60, 25), "类似色", HudFilled.无, HudColor.无, xHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, new Vector2(0, 0), 12, true, Font_Light);

            Editor_xHudGUI.Gui_Labelfield_Thin_WrapClip(new Rect(baseoffset + interval * 4, rect.y + 120, 60, 25), "高级灰", HudFilled.无, HudColor.无, xHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, new Vector2(0, 0), 12, true, Font_Light);
            #endregion

            ExtrctionColors_Drawer();

            BaseObject.ApplyModifiedProperties();

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.G)
            {
                TextureColors_Get();
                e.Use();
            }
        }

        #region 提取颜色
        private void TextureColors_Get()
        {
            ColorCaptureNode.Clear();

            if (ReferImage == null)
            {
                Debug.LogError("Texture is not assigned.");
                return;
            }


            // 提取采样后的像素颜色
            List<Vector3> colorData = SamplePixels(ReferImage, SampleInterval);

            // 运行 k-means 聚类
            List<Vector3> clusterCenters = KMeans(colorData, TargetColors, MaxIterations, ConvergenceThreshold);

            // 将聚类中心转换为颜色
            List<Color> rawColors = new List<Color>();
            foreach (Vector3 center in clusterCenters)
            {
                rawColors.Add(new Color(center.x, center.y, center.z));
            }

            // 筛选颜色，排除相近的颜色
            List<Color> filteredColors = FilterColors(rawColors, ColorSimilarityThreshold);

            List<Color> finalcols = new List<Color>();

            #region  分离冷暖色调
            List<Color> Warm = new List<Color>();
            List<Color> Cool = new List<Color>();
            foreach (Color color in filteredColors)
            {
                if (IsWarmColor(color))
                {
                    Warm.Add(color);
                }
                else
                {
                    Cool.Add(color);
                }
            }
            #endregion

            #region 合并冷暖色调
            for (int i = 0; i < Warm.Count; i++)
            {
                finalcols.Add(Warm[i]);
            }

            for (int i = 0; i < Cool.Count; i++)
            {
                finalcols.Add(Cool[i]);
            }
            #endregion


            for (int i = 0; i < finalcols.Count; i++)
            {
                xHud_Tool_ColorCaptureNode s_color = new xHud_Tool_ColorCaptureNode();
                s_color.color_theme = finalcols[i];
                s_color.color_complementary = Complementary_Creator(finalcols[i]);
                s_color.color_approximate = Approximate_Creator(finalcols[i], ApproximateOffset);
                s_color.color_analogous = Approximate_Creator(finalcols[i], Analogousffset);
                s_color.color_advancedgray = AdvancedGray_Creator(finalcols[i]);
                ColorCaptureNode.Add(s_color);
            }

        }

        List<Vector3> SamplePixels(Texture2D texture, int interval)
        {
            List<Vector3> sampledColors = new List<Vector3>();
            Color[] pixels = texture.GetPixels();

            for (int y = 0; y < texture.height; y += interval)
            {
                for (int x = 0; x < texture.width; x += interval)
                {
                    Color pixel = pixels[y * texture.width + x];
                    sampledColors.Add(new Vector3(pixel.r, pixel.g, pixel.b));
                }
            }

            return sampledColors;
        }

        List<Vector3> KMeans(List<Vector3> data, int k, int maxIterations, float threshold)
        {
            // 随机初始化聚类中心
            List<Vector3> centers = new List<Vector3>();
            for (int i = 0; i < k; i++)
            {
                centers.Add(data[Random.Range(0, data.Count)]);
            }

            bool converged = false;
            int iteration = 0;

            while (!converged && iteration < maxIterations)
            {
                converged = true;
                iteration++;

                // 创建聚类
                List<List<Vector3>> clusters = new List<List<Vector3>>();
                for (int i = 0; i < k; i++)
                {
                    clusters.Add(new List<Vector3>());
                }

                // 将数据点分配到最近的聚类中心
                foreach (Vector3 point in data)
                {
                    int closestCluster = 0;
                    float minDistance = Vector3.Distance(point, centers[0]);
                    for (int i = 1; i < k; i++)
                    {
                        float distance = Vector3.Distance(point, centers[i]);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestCluster = i;
                        }
                    }
                    clusters[closestCluster].Add(point);
                }

                // 更新聚类中心
                for (int i = 0; i < k; i++)
                {
                    Vector3 oldCenter = centers[i];
                    Vector3 newCenter = Vector3.zero;
                    foreach (Vector3 point in clusters[i])
                    {
                        newCenter += point;
                    }
                    newCenter /= clusters[i].Count;

                    if (Vector3.Distance(oldCenter, newCenter) > threshold)
                    {
                        converged = false;
                    }
                    centers[i] = newCenter;
                }
            }

            return centers;
        }

        List<Color> FilterColors(List<Color> colors, float similarityThreshold)
        {
            List<Color> filteredColors = new List<Color>();

            foreach (Color color in colors)
            {
                bool isSimilar = false;
                foreach (Color existingColor in filteredColors)
                {
                    if (ColorDistance(color, existingColor) < similarityThreshold)
                    {
                        isSimilar = true;
                        break;
                    }
                }

                if (!isSimilar)
                {
                    filteredColors.Add(color);
                }
            }

            return filteredColors;
        }

        bool IsWarmColor(Color color)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            return h <= 0.167f || h >= 0.833f;
        }

        float ColorDistance(Color color1, Color color2)
        {
            return Vector3.Distance(new Vector3(color1.r, color1.g, color1.b), new Vector3(color2.r, color2.g, color2.b));
        }

        /// <summary>
        /// 互补色计算
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        Color Complementary_Creator(Color color)
        {
            // 方法1：使用RGB取反计算互补色
            Color col = new Color(1 - color.r, 1 - color.g, 1 - color.b);

            // 方法2：使用HSV旋转色调计算互补色（可选）
            // float h, s, v;
            // 颜色_Color.RGBToHSV(color, out h, out s, out v);
            // h = (h + 0.5f) % 1.0f; // 旋转180度
            // col = 颜色_Color.HSVToRGB(h, s, v);

            return col;
        }

        /// <summary>
        /// 近似色计算
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        Color Approximate_Creator(Color color, float hueOffset)
        {
            // 将颜色从RGB转换为HSV
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);

            // 计算新的色调值
            float newHue = (h + hueOffset / 360.0f) % 1.0f;
            if (newHue < 0) newHue += 1.0f; // 确保色调值在0到1之间

            // 将新的HSV值转换回RGB格式
            return Color.HSVToRGB(newHue, s, v);
        }

        /// <summary>
        /// 高级灰色计算
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        Color AdvancedGray_Creator(Color originalColor)
        {
            // 将颜色从RGB转换为HSV
            float h, s, v;
            Color.RGBToHSV(originalColor, out h, out s, out v);

            // 调整饱和度和亮度
            s = Random.Range(0f, 0.5f * GrayOffset);
            v = Random.Range(0.5f * GrayBright, 1f * GrayBright);

            // 将新的HSV值转换回RGB格式
            return Color.HSVToRGB(h, s, v);
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

            string state = Editor_xHudGUI.Open(xHudDialogType.帮助, "ColorCaptureTool颜色提取工具通知", "导出为套系色卡库", $"是否要为当前图片<color={hexcol}>提取的套系色调</color>创建套装色板？选定文件夹后即会在该文件夹中创建<color={hexcol}>整套色板</color>，包含：<color={hexcol}>主题色</color> Theme、<color={hexcol}>互补色</color> Complementary、<color={hexcol}>近似色</color> Approximate、<color={hexcol}>类似色</color> Analogous、<color={hexcol}>高级灰</color> AdvanceGray", "创建", "暂不", 0);

            if (state == "创建")
            {
                //创建颜色库 - Theme
                xHud_Library_Colors lib_color_theme = ScriptableObject.CreateInstance<xHud_Library_Colors>();
                lib_color_theme.LibraryName = $"{ReferImage.name}_ColorsLibrary_Theme";

                //创建颜色库 - Complementary
                xHud_Library_Colors lib_color_complementary = ScriptableObject.CreateInstance<xHud_Library_Colors>();
                lib_color_complementary.LibraryName = $"{ReferImage.name}_ColorsLibrary_Complementary";

                //创建颜色库 - Approximate
                xHud_Library_Colors lib_color_approximate = ScriptableObject.CreateInstance<xHud_Library_Colors>();
                lib_color_approximate.LibraryName = $"{ReferImage.name}_ColorsLibrary_Approximate";

                //创建颜色库 - Analogous
                xHud_Library_Colors lib_color_analogous = ScriptableObject.CreateInstance<xHud_Library_Colors>();
                lib_color_analogous.LibraryName = $"{ReferImage.name}_ColorsLibrary_Analogous";

                //创建颜色库 - AdvanceGray
                xHud_Library_Colors lib_color_advanceGray = ScriptableObject.CreateInstance<xHud_Library_Colors>();
                lib_color_advanceGray.LibraryName = $"{ReferImage.name}_ColorsLibrary_AdvanceGray";

                List<Color> colors = new List<Color>();

                //加入颜色到各个类型的色板中
                for (int i = 0; i < ColorCaptureNode.Count; i++)
                {
                    xHud_Tool_ColorCaptureNode node = ColorCaptureNode[i];
                    lib_color_theme.ColorsLibrary_AddColor("Theme_" + i, node.color_theme, "-");
                    lib_color_complementary.ColorsLibrary_AddColor("Complementary_" + i, node.color_complementary, "-");
                    lib_color_approximate.ColorsLibrary_AddColor("Approximate_" + i, node.color_approximate, "-");
                    lib_color_analogous.ColorsLibrary_AddColor("Analogous_" + i, node.color_analogous, "-");
                    lib_color_advanceGray.ColorsLibrary_AddColor("AdvanceGray_" + i, node.color_advancedgray, "-");
                }

                string path = EditorUtility.OpenFolderPanel("", Application.dataPath, "");
                string path_folder = path.Substring(Application.dataPath.Length - 6);

                string path_theme = path_folder + "/theme.asset";
                string path_complementary = path_folder + "/complementary.asset";
                string path_approximate = path_folder + "/approximate.asset";
                string path_analogous = path_folder + "/analogous.asset";
                string path_advanceGray = path_folder + "/advanceGray.asset";

                AssetDatabase.CreateAsset(lib_color_theme, path_theme);
                AssetDatabase.CreateAsset(lib_color_complementary, path_complementary);
                AssetDatabase.CreateAsset(lib_color_approximate, path_approximate);
                AssetDatabase.CreateAsset(lib_color_analogous, path_analogous);
                AssetDatabase.CreateAsset(lib_color_advanceGray, path_advanceGray);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                return;
            }
        }
        #endregion
    }
}