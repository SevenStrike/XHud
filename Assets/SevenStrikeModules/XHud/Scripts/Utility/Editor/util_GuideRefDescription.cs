namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Random = UnityEngine.Random;
    using RangeAttribute = UnityEngine.RangeAttribute;

    public class util_GuideRefDescription : EditorWindow
    {
        private SerializedObject BaseObject;

        private static util_GuideRefDescription window;

        private Texture2D logo, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p;

        [SerializeField]
        private Texture2D[] ReferImages;

        Color SepLineColor = new Color(1, 1, 1, 0.15f);
        Color MessageColor = new Color(1, 1, 1, 0.62f);

        Rect Sepline_rect;
        Rect Title_rect;
        Rect Icon_rect;

        public string GuideType = "垂直对称";
        private int referIndex = 0;

        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);

            logo = util_XHUDGUI.GetIcon("Icons_GuideRefDescription/logo");

            left_arrow_r = util_XHUDGUI.GetIcon("Icons_GuideRefDescription/left_arrow_r");
            left_arrow_p = util_XHUDGUI.GetIcon("Icons_GuideRefDescription/left_arrow_p");
            right_arrow_r = util_XHUDGUI.GetIcon("Icons_GuideRefDescription/right_arrow_r");
            right_arrow_p = util_XHUDGUI.GetIcon("Icons_GuideRefDescription/right_arrow_p");

            Font_Bold = util_XHUDGUI.GetFont("SS_Editor_Bold");
            Font_Light = util_XHUDGUI.GetFont("SS_Editor_Dialog");
        }

        private void OnDestroy()
        {
            GetWindow<SceneView>().Focus();
        }

        private void OnGUI()
        {
            Hud_Manager mgr = util_Dashboard.HudManagerGet();

            BaseObject.Update();

            string hexcol = util_Tools.Color_To_HexColor(util_Dashboard.Theme_Primary, true);

            #region 抬头
            Rect rect = new Rect(0, 0, position.width, position.height);

            Icon_rect = new Rect(15, 15, 48, 48);

            util_XHUDGUI.Gui_Icon(Icon_rect, logo);

            Title_rect = new Rect(rect.x + 85, rect.y + 15, rect.width - 80, 30);
            util_XHUDGUI.Gui_Labelfield(Title_rect, $"XHud 构图参考说明书", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 20, Font_Bold);

            Sepline_rect = new Rect(rect.x + 85, rect.y + 60, 200, 1);
            util_XHUDGUI.Gui_Box(Sepline_rect, SepLineColor);

            util_XHUDGUI.Gui_Labelfield_Thin_WrapClip(new Rect(rect.x + 18, rect.y + 80, rect.width - 38, rect.height), "此窗口提供了各种构图参考线的具体解释，用户可根据每种构图线的释义来选择适合您的构图参考类型！", HudFilled.无, HudColor.无, MessageColor, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true, Font_Light);
            #endregion

            if (ReferImages == null)
            {
                ReferImages = new Texture2D[5];
                for (int i = 0; i < ReferImages.Length; i++)
                {
                    ReferImages[i] = AssetDatabase.LoadAssetAtPath<Texture2D>($"{util_Dashboard.Get_GUIStyle_Path()}Icon/Icons_GuideRefDescription/ReferImages/{GuideType}/ReferImg_{i}.png");
                }
            }

            #region 参考图片 / 参数
            util_XHUDGUI.Gui_Icon(new Rect(rect.x + 18, rect.y + 130, 500, 500), ReferImages[referIndex]);

            #region 图片控件
            if (util_XHUDGUI.Gui_Button(new Rect(rect.x + 20, rect.y + 580, 32, 32), left_arrow_r, left_arrow_p, true, "", "", Color.white))
            {
                if (referIndex <= 0)
                    referIndex = ReferImages.Length - 1;
                else
                    referIndex--;
            }

            if (util_XHUDGUI.Gui_Button(new Rect(rect.x + 480, rect.y + 580, 32, 32), right_arrow_r, right_arrow_p, true, "", "", Color.white))
            {
                if (referIndex >= ReferImages.Length - 1)
                    referIndex = 0;
                else
                    referIndex++;
            }
            #endregion

            util_XHUDGUI.Gui_Box(new Rect(rect.x + 545, rect.y + 150, 1, 350), Color.gray * 0.65f);

            string abbr = "";
            string des = "";
            switch (GuideType)
            {
                case "水平对称":
                    abbr = "Horizontal Symmetry";
                    des = "水平对称构图是一种非常经典的构图方式，通过将画面元素以中心线为轴进行左右对称布局，营造出平衡、稳定、和谐的视觉效果。它适用于多种场景和领域，以下是一些具体的应用场景适用于多种场景和领域。它能够很好地突出主体元素，增强画面的平衡感和美感，同时也具有很强的视觉引导性和稳定性\n\n" +
                        "-  自然景观，引导观众的视线，增强画面的引导性和深度感\n\n" +
                        "-  广告海报，增强画面平衡感和吸引力，突出产品核心信息\n\n" +
                        "-  界面布局，增强网页的平衡感和专业感，提升用户体验\n\n" +
                        "-  室内空间，水平对称构图可以突出空间的规整和秩序感\n\n" +
                        "-  建筑造型，建筑具有对称结构，突出建筑规整性和对称美";
                    break;
                case "垂直对称":
                    abbr = "Vertical Symmetry";
                    des = "垂直对称构图是一种通过将画面元素以水平中心线为轴进行对称布局的构图方式，能够营造出平衡、稳定和和谐的视觉效果。它虽然不如左右对称构图常见，但在某些特定场景中同样具有独特的表现力\n\n" +
                        "-  自然景观，引导观众的视线，增强画面的引导性和深度感\n\n" +
                        "-  广告海报，增强画面平衡感和吸引力，突出产品核心信息\n\n" +
                        "-  界面布局，增强网页的平衡感和专业感，提升用户体验\n\n" +
                        "-  室内空间，垂直对称构图可以突出空间的规整和秩序感\n\n" +
                        "-  建筑造型，建筑具有对称结构，突出建筑规整性和对称美";
                    break;
                case "黄金螺旋":
                    abbr = " Fibonacci Golden Spiral";
                    des = "黄金螺旋线构图，也常被称为“黄金螺旋”或“费波那契螺旋”，是一种在艺术、建筑和设计中广泛使用的构图技巧。它基于黄金比例，这是一种在自然界和人类审美中普遍存在的数学比例。黄金螺旋线构图的特点包括：\n\n" +
                        "-  这个比例被认为是美学上最令人愉悦的比例之一\n\n" +
                        "-  艺术作品中使用黄金螺旋可以增加作品的自然和谐感\n\n" +
                        "-  动态引导增加画面深度和维度，观众视线在画面中流动\n\n" +
                        "-  螺旋线动态的创造出一种平衡感，在构图中实现和谐\n\n" +
                        "-  它常常被用来表达成长、进化和生命力等概念\n\n" +
                        "-  螺旋线终点放置画面兴趣点，自然吸引观众注意力";
                    break;
                case "对角线":
                    abbr = "Diagonal Composition";
                    des = "对角线构图是一种在摄影、绘画、设计等视觉艺术中常用的构图技巧，其主要特点是利用画面中的对角线来安排主要元素，从而引导观众的视线，增加画面的动态感和深度。对角线构图的特点包括：\n\n" +
                        "-  引导观众视线在画面的角落之间流动，增加观看流动性\n\n" +
                        "-  给人一种运动和方向感，使画面显得更加生动和有活力\n\n" +
                        "-  通过安排主体，使其在画面中突出，吸引观众的注意力\n\n" +
                        "-  在表现运动或冲突的场景时，增加画面紧张感或戏剧性，\n\n" +
                        "-  打破水平或垂直线条可能带来的单调感";
                    break;
                case "三分线":
                    abbr = "Rule Of Thirds";
                    des = "三分线构图（也称为三分法构图或黄金分割构图）是一种广泛使用的摄影和视觉艺术构图技巧。它基于将画面划分为九个相等部分的两条水平线和两条垂直线，这些线条在画面中形成四个交叉点。三分线构图的特点包括：\n\n" +
                        "-  将主要元素放在线条或交点上，创造出一种视觉上的平衡\n\n" +
                        "-  人眼自然地被这些交叉点吸引，更自然地吸引观众的注意\n\n" +
                        "-  沿着三分线安排前、中、背景元素，增强画面的深度感\n\n" +
                        "-  三分线可以简化构图，通过强调关键元素来减少视觉混乱\n\n" +
                        "-  在线条和交点上放置元素，可以创造出视觉上的节奏感";
                    break;
                case "引导线":
                    abbr = "Leading Lines Composition";
                    des = "引导线是绘画、绘图和设计中用来表现透视效果的一系列线条，它们有助于创造出三维空间的深度和距离感。这些线条从画面中的物体边缘延伸并汇聚于一个或多个灭点。透视灭点辅助线的特点包括：\n\n" +
                        "-  可以增强画面深度感，使二维图像看起来更加有空间感\n\n" +
                        "-  给画面带来方向性，指示运动或视线的方向，增加动态感\n\n" +
                        "-  帮助用户快速的找到立体空间定位\n\n" +
                        "-  根据引导线的多样性可以演变出多种立体视角的参考";
                    break;
                case "三角":
                    abbr = "Triangular Composition";
                    des = "三角构图是一种在摄影、绘画、设计等视觉艺术中常用的构图技巧，它利用三角形的形状来安排画面中的元素。以下是三角构图的特点：\n\n" +
                        "-  最稳定的几何形状之一，给画面带来稳定和坚实的感觉\n\n" +
                        "-  三个顶点可以自然地引导观众视线，突出画面关键元素\n\n" +
                        "-  创造出平衡感，通过三角的排列达到视觉上的平衡\n\n" +
                        "-  将主要元素放在三角形的内部，可以有效的突出主体\n\n" +
                        "-  可以用来强调故事的关键元素，增强画面的故事性";
                    break;
                case "工字型":
                    abbr = "H-Shape Composition";
                    des = "工字型构图（有时也称为“H型构图”）是一种利用类似于字母“H”的形状来安排画面元素的构图方法。这种构图方式在视觉上可以创造出一种平衡和对称的效果，以下是工字型构图的一些特点：\n\n" +
                        "-  通过在画面上下部分放置视觉重量相当的元素，创造出一种垂直方向上的平衡感\n\n" +
                        "-  这种构图方式往往具有对称性，可以是完全对称，也可以是近似对称，这有助于增强画面的稳定感\n\n" +
                        "-  工字型构图可以引导观众的视线沿着画面的垂直方向移动，从而突出画面的中心元素\n\n" +
                        "-  通过在工字型的中间横杠部分放置主要的主体或焦点，可以有效地强调画面的中心区域\n\n" +
                        "-  这种构图方式的结构清晰，容易让观众理解画面的布局和重点";
                    break;
            }

            util_XHUDGUI.Gui_Labelfield(new Rect(rect.x + 570, rect.y + 130, 448, 20), $"{GuideType}  <size=14>( <color={hexcol}>{abbr}</color> )</size>", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, 25, true, Font_Bold);

            util_XHUDGUI.Gui_Labelfield(new Rect(rect.x + 570, rect.y + 180, 345, 20), des, HudFilled.无, HudColor.无, Color.white * 0.75f, TextAnchor.UpperLeft, 13, Font_Light, true);
            #endregion

            BaseObject.ApplyModifiedProperties();

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }
        }

        public void SetGuideType(string type)
        {
            GuideType = type;
        }
    }
}