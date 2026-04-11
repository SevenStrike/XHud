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
    using SevenStrikeModules.XHud;
    using SevenStrikeModules.XHud.Editor;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public class Editor_XHud_Tool_ElementVisualPlacer
    {
        /// <summary>
        /// 存储屏幕九宫格划分后的矩形区域列表，用于鼠标位置到锚点格子的映射。
        /// 索引对应锚点位置：0-左上，1-上，2-右上，3-左，4-中心，5-右，6-左下，7-下，8-右下。
        /// </summary>
        private static List<Rect> gridRects = new List<Rect>();

        /// <summary>
        /// 标识是否正在拖拽一个有效的目标预制体。
        /// </summary>
        private static bool isDraggingTarget = false;

        /// <summary>
        /// 当前拖拽的、包含目标组件（XHud_Module_Element）的预制体数组。
        /// </summary>
        private static GameObject[] draggedPrefabs = null;

        /// <summary>
        /// 目标组件名称，用于过滤拖拽对象。
        /// 只有包含该组件的预制体才被视为有效的放置目标。
        /// </summary>
        private static string targetComponentName = "XHud_Module_Element";

        /// <summary>
        /// XHud 系统的主管理器引用，用于获取锚点布局等核心功能。
        /// </summary>
        private static XHud_Manager mgr;

        /// <summary>
        /// 当前鼠标悬停的格子索引，-1 表示没有悬停在任何格子上。
        /// 该值用于在拖拽过程中高亮对应的锚点区域。
        /// </summary>
        private static int hoverGridIndex = -1;

        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        private static Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        private static Font Font_Light;

        #region 颜色配置

        /// <summary>
        /// 九宫格每个格子的非高亮（默认）背景色数组。
        /// 索引顺序与 gridRects 一致，每个格子初始为半透明黑色。
        /// </summary>
        private static Color[] Colors_Grid_UnHighlight = new Color[]
        {
    // 格子0 - 左上 (LU)
    Color.black * 0.4f,
    // 格子1 - 上 (U)
    Color.black * 0.4f,
    // 格子2 - 右上 (RU)
    Color.black * 0.4f,
    // 格子3 - 左 (L)
    Color.black * 0.4f,
    // 格子4 - 中心 (C)
    Color.black * 0.4f,
    // 格子5 - 右 (R)
    Color.black * 0.4f,
    // 格子6 - 左下 (LD)
    Color.black * 0.4f,
    // 格子7 - 下 (D)
    Color.black * 0.4f,
    // 格子8 - 右下 (RD)
    Color.black * 0.4f
        };

        /// <summary>
        /// 拖拽过程中，鼠标悬停格子的高亮背景色。
        /// 使用 XHud 仪表盘的主色调并降低饱和度/亮度，以达到半透明高亮效果。
        /// </summary>
        private static Color Color_Grid_Highlight = XHud_Dashboard.Theme_Primary * 0.5f;

        /// <summary>
        /// 拖拽过程中，悬停格子的边框高亮颜色。
        /// </summary>
        private static Color Color_Edge_Highlight = Color.white * 0.6f;

        /// <summary>
        /// 非悬停状态下的格子边框颜色。
        /// </summary>
        private static Color Color_Edge_UnHighlight = Color.black;

        /// <summary>
        /// 悬停状态下，格子标签文字的颜色。
        /// </summary>
        private static Color Color_Label_Highlight = Color.white;

        #endregion

        /// <summary>
        /// 静态初始化
        /// </summary>
        static Editor_XHud_Tool_ElementVisualPlacer()
        {
            SceneView.duringSceneGui += OnSceneGUI;

            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Light");
        }

        /// <summary>
        /// 更新九宫格矩形区域列表。
        /// 根据当前 SceneView 的视口大小，将屏幕均匀划分为 3x3 的九宫格区域，
        /// 每个格子的宽高分别为视口宽高的 1/3。
        /// 这些矩形区域用于后续的鼠标位置检测和高亮绘制。
        /// </summary>
        /// <param name="sceneView">当前的 SceneView 窗口，用于获取视口位置和尺寸。</param>
        private static void UpdateGridRects(SceneView sceneView)
        {
            gridRects.Clear();
            Rect viewRect = sceneView.position;
            float w = viewRect.width / 3;
            float h = viewRect.height / 3;

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    gridRects.Add(new Rect(col * w, row * h, w, h));
                }
            }
        }
        /// <summary>
        /// SceneView 的 GUI 事件处理函数。
        /// 在场景视图中处理拖拽放置 XHud 元素预制体的完整流程，包括：
        /// 1. 更新九宫格区域划分
        /// 2. 更新鼠标悬停的格子索引
        /// 3. 处理拖拽进入/更新事件（DragUpdated）
        /// 4. 处理拖拽释放事件（DragPerform）- 执行元素放置
        /// 5. 处理拖拽取消事件（DragExited）- 清理拖拽状态
        /// 6. 拖拽过程中绘制九宫格高亮界面
        /// </summary>
        /// <param name="sceneView">当前的 SceneView 窗口实例。</param>
        private static void OnSceneGUI(SceneView sceneView)
        {
            if (mgr == null)
                mgr = XHud_Dashboard.HudManagerGet();

            // 如果功能被禁用，不处理任何事件
            if (!isEnabled) return;

            Event e = Event.current;
            UpdateGridRects(sceneView);

            // 更新悬停格子索引（仅在拖拽目标时）
            if (isDraggingTarget)
            {
                int newHoverIndex = GetGridIndex(e.mousePosition);
                if (newHoverIndex != hoverGridIndex)
                {
                    hoverGridIndex = newHoverIndex;
                    sceneView.Repaint();
                }
            }
            else
            {
                hoverGridIndex = -1;
            }

            // 1. 拖拽进入/更新
            if (e.type == EventType.DragUpdated)
            {
                CheckAndSetDragTarget();

                if (isDraggingTarget)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    e.Use();
                    sceneView.Repaint();
                }
            }

            // 2. 拖拽释放
            if (e.type == EventType.DragPerform)
            {
                if (isDraggingTarget)
                {
                    Vector2 mousePos = e.mousePosition;
                    int gridIndex = GetGridIndex(mousePos);

                    if (gridIndex >= 0)
                    {
                        ProcessDropedElements(gridIndex, draggedPrefabs);
                    }

                    isDraggingTarget = false;
                    draggedPrefabs = null;
                    hoverGridIndex = -1;
                    e.Use();
                    sceneView.Repaint();
                }
            }

            // 3. 拖拽取消
            if (e.type == EventType.DragExited)
            {
                if (isDraggingTarget)
                {
                    isDraggingTarget = false;
                    draggedPrefabs = null;
                    hoverGridIndex = -1;
                    sceneView.Repaint();
                }
            }

            // 4. 绘制九宫格
            if (isDraggingTarget)
            {
                DrawGrid();
                sceneView.Repaint();
            }
        }
        /// <summary>
        /// 批量处理拖入的元素预制体
        /// </summary>
        /// <param name="gridIndex">拖拽释放的格子索引</param>
        /// <param name="prefabs">拖拽的预制体数组</param>
        private static void ProcessDropedElements(int gridIndex, GameObject[] prefabs)
        {
            if (prefabs == null || prefabs.Length == 0) return;

            XHud_Utilitys.Func_PrintInfo("XHud - 元素视觉放置器", $"已将所选元素放置到屏幕锚点 \" {GetGridNameWithIndex(gridIndex)} \"，放置的数量：{prefabs.Length} 个", HudMsgState.确认);

            foreach (GameObject prefab in prefabs)
            {
                if (prefab == null) continue;

                XHud_Module_Element ele = prefab.GetComponent<XHud_Module_Element>();
                if (ele != null)
                {
                    ProcessDropedElement(gridIndex, ele);
                }
            }
        }
        /// <summary>
        /// 处理单个拖入的元素预制体
        /// </summary>
        /// <param name="gridIndex"></param>
        /// <param name="ele"></param>
        private static void ProcessDropedElement(int gridIndex, XHud_Module_Element ele)
        {
            switch (gridIndex)
            {
                case 0: Dragged_To_LU(ele); break;
                case 1: Dragged_To_U(ele); break;
                case 2: Dragged_To_RU(ele); break;
                case 3: Dragged_To_L(ele); break;
                case 4: Dragged_To_C(ele); break;
                case 5: Dragged_To_R(ele); break;
                case 6: Dragged_To_LD(ele); break;
                case 7: Dragged_To_D(ele); break;
                case 8: Dragged_To_RD(ele); break;
                default: XHud_Utilitys.Func_PrintInfo("XHud - 元素视觉放置器", $"未知的锚点索引！", HudMsgState.错误); break;
            }
        }

        #region 拖拽放置处理方法
        /// <summary>
        /// 拖拽放置到左上角 (Left-Up)
        /// </summary>
        /// <param name="element">被拖拽的 XHud_Module_Element 元素</param>
        private static void Dragged_To_LU(XHud_Module_Element element)
        {
            //Debug.Log($"Dragged_To_LU - 预制体: {element?.name ?? "null"}");
            Placer(element, XHudAnchor.左上);
        }
        /// <summary>
        /// 拖拽放置到上 (Up)
        /// </summary>
        /// <param name="element">被拖拽的 XHud_Module_Element 元素</param>
        private static void Dragged_To_U(XHud_Module_Element element)
        {
            //Debug.Log($"Dragged_To_C - 预制体: {element?.name ?? "null"}");
            Placer(element, XHudAnchor.上);
        }
        /// <summary>
        /// 拖拽放置到右上角 (Right-Up)
        /// </summary>
        /// <param name="element">被拖拽的 XHud_Module_Element 元素</param>
        private static void Dragged_To_RU(XHud_Module_Element element)
        {
            //Debug.Log($"Dragged_To_RU - 预制体: {element?.name ?? "null"}");
            Placer(element, XHudAnchor.右上);
        }
        /// <summary>
        /// 拖拽放置到中心 (Center)
        /// </summary>
        /// <param name="element">被拖拽的 XHud_Module_Element 元素</param>
        private static void Dragged_To_C(XHud_Module_Element element)
        {
            //Debug.Log($"Dragged_To_C - 预制体: {element?.name ?? "null"}");
            Placer(element, XHudAnchor.中心);
        }
        /// <summary>
        /// 拖拽放置到左 (Left)
        /// </summary>
        /// <param name="element">被拖拽的 XHud_Module_Element 元素</param>
        private static void Dragged_To_L(XHud_Module_Element element)
        {
            //Debug.Log($"Dragged_To_L - 预制体: {element?.name ?? "null"}");
            Placer(element, XHudAnchor.左);
        }
        /// <summary>
        /// 拖拽放置到右 (Right)
        /// </summary>
        /// <param name="element">被拖拽的 XHud_Module_Element 元素</param>
        private static void Dragged_To_R(XHud_Module_Element element)
        {
            //Debug.Log($"Dragged_To_R - 预制体: {element?.name ?? "null"}");
            Placer(element, XHudAnchor.右);
        }
        /// <summary>
        /// 拖拽放置到左下角 (Left-Down)
        /// </summary>
        /// <param name="element">被拖拽的 XHud_Module_Element 元素</param>
        private static void Dragged_To_LD(XHud_Module_Element element)
        {
            //Debug.Log($"Dragged_To_LD - 预制体: {element?.name ?? "null"}");
            Placer(element, XHudAnchor.左下);
        }
        /// <summary>
        /// 拖拽放置到下 (Down)
        /// </summary>
        /// <param name="element">被拖拽的 XHud_Module_Element 元素</param>
        private static void Dragged_To_D(XHud_Module_Element element)
        {
            //Debug.Log($"Dragged_To_D - 预制体: {element?.name ?? "null"}");
            Placer(element, XHudAnchor.下);
        }
        /// <summary>
        /// 拖拽放置到右下角 (Right-Down)
        /// </summary>
        /// <param name="element">被拖拽的 XHud_Module_Element 元素</param>
        private static void Dragged_To_RD(XHud_Module_Element element)
        {
            //Debug.Log($"Dragged_To_RD - 预制体: {element?.name ?? "null"}");
            Placer(element, XHudAnchor.右下);
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 实例化到锚点中
        /// </summary>
        /// <param name="element"></param>
        /// <param name="anchor"></param>
        private static void Placer(XHud_Module_Element element, XHudAnchor anchor)
        {
            Anchor_Layout anchor_Layout = mgr.hm_ScreenElement_Matched_AnchoredType(anchor);

            GameObject obj = PrefabUtility.InstantiatePrefab(element.gameObject, anchor_Layout.Anchor) as GameObject;

            if (obj != null)
            {
                Undo.RegisterCreatedObjectUndo(obj, "Place XHud Element");
            }

            Selection.activeObject = obj;
        }
        /// <summary>
        /// 检查当前拖拽的对象中是否包含有效的目标预制体（包含 XHud_Module_Element 组件）。
        /// 如果存在有效预制体，则设置拖拽状态为 true，并存储这些预制体。
        /// 该方法仅在拖拽未激活时执行，避免重复检测。
        /// </summary>
        private static void CheckAndSetDragTarget()
        {
            if (isDraggingTarget) return;
            if (DragAndDrop.objectReferences.Length == 0) return;

            List<GameObject> validPrefabs = new List<GameObject>();

            foreach (Object obj in DragAndDrop.objectReferences)
            {
                if (PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.NotAPrefab)
                    continue;

                GameObject prefab = obj as GameObject;
                if (prefab == null) continue;

                if (prefab.GetComponent(targetComponentName) != null)
                {
                    validPrefabs.Add(prefab);
                }
            }

            if (validPrefabs.Count > 0)
            {
                isDraggingTarget = true;
                draggedPrefabs = validPrefabs.ToArray();

                XHud_Utilitys.Func_PrintInfo("XHud - 元素视觉放置器", $"检测到目标预制体 {validPrefabs.Count} 个！", HudMsgState.设置);
            }
        }
        /// <summary>
        /// 根据鼠标位置获取其所在的九宫格索引。
        /// 遍历预先生成的 gridRects 列表，判断鼠标位置属于哪个格子区域。
        /// </summary>
        /// <param name="mousePos">鼠标在 SceneView 中的屏幕坐标位置。</param>
        /// <returns>
        /// 返回鼠标所在格子的索引（0-8），如果鼠标不在任何格子范围内则返回 -1。
        /// 索引对应关系：0-左上，1-上，2-右上，3-左，4-中心，5-右，6-左下，7-下，8-右下。
        /// </returns>
        private static int GetGridIndex(Vector2 mousePos)
        {
            for (int i = 0; i < gridRects.Count; i++)
            {
                if (gridRects[i].Contains(mousePos))
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// 根据格子索引获取对应的锚点名称字符串。
        /// 将内部使用的格子索引转换为 XHudAnchor 枚举对应的可读名称。
        /// </summary>
        /// <param name="index">格子索引（0-8），对应九宫格中的九个锚点位置。</param>
        /// <returns>
        /// 返回锚点名称字符串（如 "左上"、"中心"、"右下" 等）。
        /// 如果索引无效，默认返回 "中心"。
        /// </returns>
        private static string GetGridNameWithIndex(int index)
        {
            XHudAnchor anchor = XHudAnchor.中心;

            switch (index)
            {
                case 0:
                    anchor = XHudAnchor.左上;
                    break;
                case 1:
                    anchor = XHudAnchor.上;
                    break;
                case 2:
                    anchor = XHudAnchor.右上;
                    break;
                case 3:
                    anchor = XHudAnchor.左;
                    break;
                case 4:
                    anchor = XHudAnchor.中心;
                    break;
                case 5:
                    anchor = XHudAnchor.右;
                    break;
                case 6:
                    anchor = XHudAnchor.左下;
                    break;
                case 7:
                    anchor = XHudAnchor.下;
                    break;
                case 8:
                    anchor = XHudAnchor.右下;
                    break;
            }

            return anchor.ToString();
        }
        /// <summary>
        /// 绘制锚点宫格
        /// </summary>
        private static void DrawGrid()
        {
            Handles.BeginGUI();

            for (int i = 0; i < gridRects.Count; i++)
            {
                Rect rect = gridRects[i];

                // 获取原始颜色
                Color originalColor = (i < Colors_Grid_UnHighlight.Length) ? Colors_Grid_UnHighlight[i] : new Color(0, 0.5f, 1, 0.3f);

                // 如果是当前悬停的格子，使用白色高亮（带一定透明度）
                Color drawColor = (i == hoverGridIndex) ? Color_Grid_Highlight : originalColor;

                EditorGUI.DrawRect(rect, drawColor);

                // 使用 Handles.DrawLine 绘制边框
                Vector3 topLeft = new Vector3(rect.x, rect.y, 0);
                Vector3 topRight = new Vector3(rect.x + rect.width, rect.y, 0);
                Vector3 bottomLeft = new Vector3(rect.x, rect.y + rect.height, 0);
                Vector3 bottomRight = new Vector3(rect.x + rect.width, rect.y + rect.height, 0);

                // 悬停时边框使用亮白色，否则使用半透明白色
                Handles.color = (i == hoverGridIndex) ? Color_Edge_Highlight : Color_Edge_UnHighlight;
                Handles.DrawLine(topLeft, topRight);     // 上边
                Handles.DrawLine(bottomLeft, bottomRight); // 下边
                Handles.DrawLine(topLeft, bottomLeft);   // 左边
                Handles.DrawLine(topRight, bottomRight); // 右边

                // 格子标签
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                // 悬停时标签文字使用亮色，否则使用对比色
                style.normal.textColor = (i == hoverGridIndex) ? Color_Label_Highlight : GetContrastColor(originalColor);
                style.fontStyle = FontStyle.Bold;

                Rect label_rect = new Rect(rect.x + rect.width / 2 - 60, rect.y + rect.height / 2 - 45, 120, 25);
                Editor_XHud_GUI.Gui_Labelfield(label_rect, GetGridLabel(i), HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleCenter, 14, GetBoldFont());
                label_rect.Set(label_rect.x, label_rect.y + 40, label_rect.width, label_rect.height);
                Editor_XHud_GUI.Gui_Labelfield(label_rect, GetGridLabel_Abbr(i), HudFilled.实体, HudColor.深空灰, Color.white * 0.9f, TextAnchor.MiddleCenter, 12, GetLightFont());
            }

            Handles.EndGUI();
        }
        /// <summary>
        /// 根据背景色自动获取对比文字颜色（深色背景用白字，浅色背景用黑字）
        /// </summary>
        /// <param name="backgroundColor"></param>
        /// <returns></returns>
        private static Color GetContrastColor(Color backgroundColor)
        {
            float brightness = backgroundColor.r * 0.299f + backgroundColor.g * 0.587f + backgroundColor.b * 0.114f;
            return brightness > 0.5f ? Color.black : Color.white;
        }
        /// <summary>
        /// 获取宫格的名称
        /// </summary>
        /// <param name="gridIndex"></param>
        /// <returns></returns>
        private static string GetGridLabel(int gridIndex)
        {
            switch (gridIndex)
            {
                case 0: return "左上角";
                case 1: return "顶部";
                case 2: return "右上角";
                case 3: return "左边";
                case 4: return "中心";
                case 5: return "右边";
                case 6: return "左下角";
                case 7: return "底部";
                case 8: return "右下角";
                default: return "";
            }
        }
        /// <summary>
        /// 获取宫格的缩写名称
        /// </summary>
        /// <param name="gridIndex"></param>
        /// <returns></returns>
        private static string GetGridLabel_Abbr(int gridIndex)
        {
            switch (gridIndex)
            {
                case 0: return "Left-U Corner";
                case 1: return "Top";
                case 2: return "Right-U Corner";
                case 3: return "Left";
                case 4: return "Center";
                case 5: return "Right";
                case 6: return "Left-D Corner";
                case 7: return "Bottom";
                case 8: return "Right-D Corner";
                default: return "";
            }
        }
        /// <summary>
        /// 公开方法：设置指定格子的颜色
        /// </summary>
        /// <param name="gridIndex"></param>
        /// <param name="color"></param>
        public static void SetGridColor(int gridIndex, Color color)
        {
            if (gridIndex >= 0 && gridIndex < Colors_Grid_UnHighlight.Length)
            {
                Colors_Grid_UnHighlight[gridIndex] = color;
                SceneView.RepaintAll();
            }
            else
            {
                XHud_Utilitys.Func_PrintInfo("XHud - 元素视觉放置器", $"锚点区域索引 {gridIndex} 超出范围 (0-{Colors_Grid_UnHighlight.Length - 1})！", HudMsgState.警告);
            }
        }
        /// <summary>
        /// 设置所有格子颜色
        /// </summary>
        /// <param name="color"></param>
        public static void SetAllGridColors(Color color)
        {
            for (int i = 0; i < Colors_Grid_UnHighlight.Length; i++)
            {
                Colors_Grid_UnHighlight[i] = color;
            }
            SceneView.RepaintAll();
        }
        /// <summary>
        /// 获取当前颜色数组 
        /// </summary>
        /// <returns></returns>
        public static Color[] GetGridColors()
        {
            return (Color[])Colors_Grid_UnHighlight.Clone();
        }
        /// <summary>
        /// 获取粗体字体资源。
        /// 如果字体尚未加载，则通过 Editor_XHud_GUI.GetFont 方法从资源中加载 "SS_Editor_Bold" 字体。
        /// </summary>
        /// <returns>返回粗体字体对象，用于 GUI 绘制中的标题或强调文本。</returns>
        private static Font GetBoldFont()
        {
            if (Font_Bold == null)
                Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            return Font_Bold;
        }
        /// <summary>
        /// 获取细体/常规字体资源。
        /// 如果字体尚未加载，则通过 Editor_XHud_GUI.GetFont 方法从资源中加载 "SS_Editor_Light" 字体。
        /// </summary>
        /// <returns>返回细体字体对象，用于 GUI 绘制中的辅助文本或说明文字。</returns>
        private static Font GetLightFont()
        {
            if (Font_Light == null)
                Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Light");
            return Font_Light;
        }
        #endregion

        #region 外部控制开关
        /// <summary>
        /// 是否启动拖拽放置功能
        /// </summary>
        private static bool isEnabled = true;
        /// <summary>
        /// 设置拖拽功能是否启用
        /// </summary>
        /// <param name="enabled">true=启用, false=禁用</param>
        public static void SetEnabled(bool enabled)
        {
            isEnabled = enabled;

            // 如果禁用功能，重置拖拽状态
            if (!isEnabled)
            {
                isDraggingTarget = false;
                draggedPrefabs = null;
                hoverGridIndex = -1;
                SceneView.RepaintAll();
            }

            //XHud_Utilitys.Func_PrintInfo("XHud - 元素视觉放置器", $"已{(isEnabled ? "启用" : "禁用")} ！", HudMsgState.确认);
        }
        /// <summary>
        /// 获取拖拽功能是否启用
        /// </summary>
        /// <returns>true=启用, false=禁用</returns>
        public static bool GetEnabled()
        {
            return isEnabled;
        }
        /// <summary>
        /// 切换启用状态
        /// </summary>
        public static void ToggleEnabled()
        {
            SetEnabled(!isEnabled);
        }
        #endregion
    }
}