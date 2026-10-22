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
    using SevenStrikeModules.XHud;
    using SevenStrikeModules.XHud.Enums;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;

    /// <summary>
    /// 元素视觉放置器。
    /// <para/>
    /// <list type="bullet">
    /// <item><description>在 SceneView 中把元素预制体拖拽到九宫格锚点上进行放置。</description></item>
    /// <item><description>通过 <c>[InitializeOnLoad]</c> 自动订阅 <c>SceneView.duringSceneGui</c>。</description></item>
    /// <item><description>提供外部开关与格子颜色自定义接口。</description></item>
    /// </list>
    /// </summary>
    [InitializeOnLoad]
    public class Editor_XHud_Tool_SceneView_ElementVisualPlacer
    {
        #region 字段 - 状态
        /// <summary>
        /// 是否启动拖拽放置功能。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>为 <c>true</c> 时响应 SceneView 拖拽事件。</description></item>
        /// <item><description>可由 <c>SetEnabled</c> 动态切换。</description></item>
        /// </list>
        /// </summary>
        private static bool isEnabled = true;
        /// <summary>
        /// 标识是否正在拖拽一个有效的目标预制体。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在拖拽过程中为 <c>true</c>。</description></item>
        /// </list>
        /// </summary>
        private static bool isDraggingTarget = false;
        /// <summary>
        /// 当前拖拽的、包含目标组件的预制体数组。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>拖拽结束后清空。</description></item>
        /// </list>
        /// </summary>
        private static GameObject[] draggedPrefabs = null;
        /// <summary>
        /// 当前鼠标悬停的格子索引。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>取值范围 0-8，<c>-1</c> 表示未悬停在任何格子上。</description></item>
        /// </list>
        /// </summary>
        private static int hoverGridIndex = -1;
        #endregion

        #region 字段 - 映射
        /// <summary>
        /// 九宫格矩形区域列表。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引对应锚点位置：0-左上，1-上，2-右上，3-左，4-中心，5-右，6-左下，7-下，8-右下。</description></item>
        /// <item><description>由 <c>UpdateGridRects</c> 根据 SceneView 视口大小重建。</description></item>
        /// </list>
        /// </summary>
        private static List<Rect> gridRects = new List<Rect>();
        /// <summary>
        /// 目标组件名称，用于过滤拖拽对象。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>只有包含该组件的预制体才被视为有效的放置目标。</description></item>
        /// </list>
        /// </summary>
        private static string targetComponentName = "XHud_Module_Element";
        /// <summary>
        /// XHud 系统的主管理器引用。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>用于获取锚点布局等核心功能。</description></item>
        /// <item><description>首次使用时懒加载。</description></item>
        /// </list>
        /// </summary>
        private static XHud_Manager mgr;
        #endregion

        #region 字段 - 颜色配置
        /// <summary>
        /// 九宫格每个格子的非高亮（默认）背景色数组。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>长度 9，索引顺序与 <c>gridRects</c> 一致。</description></item>
        /// <item><description>每个格子初始为半透明黑色。</description></item>
        /// </list>
        /// </summary>
        private static Color[] Colors_Grid_UnHighlight = new Color[9]
        {
            Color.black * 0.4f, Color.black * 0.4f, Color.black * 0.4f,
            Color.black * 0.4f, Color.black * 0.4f, Color.black * 0.4f,
            Color.black * 0.4f, Color.black * 0.4f, Color.black * 0.4f,
        };
        /// <summary>
        /// 拖拽过程中，鼠标悬停格子的高亮背景色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>使用主色调并降低亮度，以达到半透明高亮效果。</description></item>
        /// </list>
        /// </summary>
        private static Color Color_Grid_Highlight = XHud_Dashboard.themes_get_color(ThemeColorType.primary) * 0.5f;
        /// <summary>
        /// 拖拽过程中，悬停格子的边框高亮颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认为半透明白色。</description></item>
        /// </list>
        /// </summary>
        private static Color Color_Edge_Highlight = Color.white * 0.6f;
        /// <summary>
        /// 非悬停状态下的格子边框颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认为纯黑。</description></item>
        /// </list>
        /// </summary>
        private static Color Color_Edge_UnHighlight = Color.black;
        /// <summary>
        /// 悬停状态下，格子标签文字的颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认为纯白。</description></item>
        /// </list>
        /// </summary>
        private static Color Color_Label_Highlight = Color.white;
        #endregion

        #region 生命周期
        /// <summary>
        /// 静态构造函数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>由 <c>[InitializeOnLoad]</c> 触发。</description></item>
        /// <item><description>订阅 <c>SceneView.duringSceneGui</c>。</description></item>
        /// </list>
        /// </summary>
        static Editor_XHud_Tool_SceneView_ElementVisualPlacer()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        #endregion

        #region 绘制 - 九宫格
        /// <summary>
        /// SceneView 的 GUI 事件处理函数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>更新九宫格区域划分与鼠标悬停索引。</description></item>
        /// <item><description>处理 DragUpdated / DragPerform / DragExited 事件。</description></item>
        /// <item><description>拖拽过程中绘制九宫格高亮界面。</description></item>
        /// </list>
        /// </summary>
        /// <param name="sceneView">当前的 SceneView 窗口实例。</param>
        private static void OnSceneGUI(SceneView sceneView)
        {
            if (mgr == null)
                mgr = XHud_Dashboard.get_manager();

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
        /// 更新九宫格矩形区域列表。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>根据当前 SceneView 的视口大小，将屏幕均匀划分为 3x3 的九宫格区域。</description></item>
        /// <item><description>每个格子的宽高分别为视口宽高的 1/3。</description></item>
        /// </list>
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
        /// 绘制锚点宫格。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制 9 个格子的背景、边框、主标签与缩写标签。</description></item>
        /// <item><description>悬停格子使用高亮颜色。</description></item>
        /// </list>
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
                XGUI.gui_label(
                    rect: label_rect,
                    text: new GUIContent(GetGridLabel(i)),
                    text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    offset: new Vector2(0, -1),
                    size: XGUIFontSize.BX,
                    anchor: TextAnchor.MiddleCenter,
                    padding: new RectOffset(0, 0, 0, 0),
                    font_style: FontStyle.Bold,
                    clipping: TextClipping.Clip);

                label_rect.Set(label_rect.x, label_rect.y + 40, label_rect.width, label_rect.height);
                XGUI.gui_label(
                    rect: label_rect,
                    text: new GUIContent(GetGridLabel_Abbr(i)),
                    text_color: Color.white * 0.9f,
                    offset: new Vector2(0, -1),
                    size: XGUIFontSize.B,
                    anchor: TextAnchor.MiddleCenter,
                    padding: new RectOffset(0, 0, 0, 0),
                    clipping: TextClipping.Clip);
            }

            Handles.EndGUI();
        }
        #endregion

        #region 工具 - 拖拽处理
        /// <summary>
        /// 批量处理拖入的元素预制体。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>输出提示并逐个调用 <c>ProcessDropedElement</c>。</description></item>
        /// </list>
        /// </summary>
        /// <param name="gridIndex">拖拽释放的格子索引。</param>
        /// <param name="prefabs">拖拽的预制体数组。</param>
        private static void ProcessDropedElements(int gridIndex, GameObject[] prefabs)
        {
            if (prefabs == null || prefabs.Length == 0) return;

            XGUI_Utilitys.Console("XHud - 元素视觉放置器", $"已将所选元素放置到屏幕锚点 \" {GetGridNameWithIndex(gridIndex)} \"，放置的数量：{prefabs.Length} 个", XGUIMsgState.确认);

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
        /// 处理单个拖入的元素预制体。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>按格子索引直接调用 <c>Placer</c> 并传入对应锚点。</description></item>
        /// <item><description>索引无效时输出错误提示。</description></item>
        /// </list>
        /// </summary>
        /// <param name="gridIndex">格子索引。</param>
        /// <param name="ele">被拖拽的元素。</param>
        private static void ProcessDropedElement(int gridIndex, XHud_Module_Element ele)
        {
            switch (gridIndex)
            {
                case 0: Placer(ele, XHudAnchor.左上); break;
                case 1: Placer(ele, XHudAnchor.上); break;
                case 2: Placer(ele, XHudAnchor.右上); break;
                case 3: Placer(ele, XHudAnchor.左); break;
                case 4: Placer(ele, XHudAnchor.中心); break;
                case 5: Placer(ele, XHudAnchor.右); break;
                case 6: Placer(ele, XHudAnchor.左下); break;
                case 7: Placer(ele, XHudAnchor.下); break;
                case 8: Placer(ele, XHudAnchor.右下); break;
                default: XGUI_Utilitys.Console("XHud - 元素视觉放置器", $"未知的锚点索引！", XGUIMsgState.错误); break;
            }
        }
        /// <summary>
        /// 实例化到锚点中。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>按锚点匹配实际锚点布局并实例化预制体。</description></item>
        /// <item><description>注册撤销并选中新对象。</description></item>
        /// </list>
        /// </summary>
        /// <param name="element">被拖拽的元素。</param>
        /// <param name="anchor">目标锚点。</param>
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
        /// 检查当前拖拽的对象中是否包含有效的目标预制体。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在拖拽未激活时执行，避免重复检测。</description></item>
        /// <item><description>符合条件时设置 <c>isDraggingTarget</c> 并缓存预制体数组。</description></item>
        /// </list>
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

                XGUI_Utilitys.Console("XHud - 元素视觉放置器", $"检测到目标预制体 {validPrefabs.Count} 个！", XGUIMsgState.设置);
            }
        }
        #endregion

        #region 工具 - 索引换算
        /// <summary>
        /// 根据鼠标位置获取其所在的九宫格索引。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>遍历 <c>gridRects</c> 判断鼠标所在格子。</description></item>
        /// </list>
        /// </summary>
        /// <param name="mousePos">鼠标在 SceneView 中的屏幕坐标位置。</param>
        /// <returns>格子索引 0-8；不在任何格子内时返回 -1。</returns>
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
        /// <para/>
        /// <list type="bullet">
        /// <item><description>把格子索引转换为 <c>XHudAnchor</c> 枚举对应名称。</description></item>
        /// <item><description>索引无效时默认返回「中心」。</description></item>
        /// </list>
        /// </summary>
        /// <param name="index">格子索引（0-8）。</param>
        /// <returns>锚点名称字符串。</returns>
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
        /// 获取宫格的名称。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>返回中文全称，如「左上角」「顶部」等。</description></item>
        /// </list>
        /// </summary>
        /// <param name="gridIndex">格子索引。</param>
        /// <returns>中文名称。</returns>
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
        /// 获取宫格的缩写名称。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>返回英文缩写，如「Left-U Corner」「Top」等。</description></item>
        /// </list>
        /// </summary>
        /// <param name="gridIndex">格子索引。</param>
        /// <returns>英文缩写名称。</returns>
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
        #endregion

        #region 工具 - 颜色
        /// <summary>
        /// 根据背景色自动获取对比文字颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>深色背景返回白色，浅色背景返回黑色。</description></item>
        /// </list>
        /// </summary>
        /// <param name="backgroundColor">背景色。</param>
        /// <returns>对比色。</returns>
        private static Color GetContrastColor(Color backgroundColor)
        {
            float brightness = backgroundColor.r * 0.299f + backgroundColor.g * 0.587f + backgroundColor.b * 0.114f;
            return brightness > 0.5f ? Color.black : Color.white;
        }
        /// <summary>
        /// 公开方法：设置指定格子的颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引越界时输出警告提示。</description></item>
        /// <item><description>设置后触发 SceneView 全量重绘。</description></item>
        /// </list>
        /// </summary>
        /// <param name="gridIndex">格子索引。</param>
        /// <param name="color">目标颜色。</param>
        public static void SetGridColor(int gridIndex, Color color)
        {
            if (gridIndex >= 0 && gridIndex < Colors_Grid_UnHighlight.Length)
            {
                Colors_Grid_UnHighlight[gridIndex] = color;
                SceneView.RepaintAll();
            }
            else
            {
                XGUI_Utilitys.Console("XHud - 元素视觉放置器", $"锚点区域索引 {gridIndex} 超出范围 (0-{Colors_Grid_UnHighlight.Length - 1})！", XGUIMsgState.警告);
            }
        }
        /// <summary>
        /// 设置所有格子颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>统一覆盖全部 9 个格子的默认背景色。</description></item>
        /// <item><description>设置后触发 SceneView 全量重绘。</description></item>
        /// </list>
        /// </summary>
        /// <param name="color">目标颜色。</param>
        public static void SetAllGridColors(Color color)
        {
            for (int i = 0; i < Colors_Grid_UnHighlight.Length; i++)
            {
                Colors_Grid_UnHighlight[i] = color;
            }
            SceneView.RepaintAll();
        }
        /// <summary>
        /// 获取当前颜色数组。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>返回内部数组的克隆，避免外部修改内部状态。</description></item>
        /// </list>
        /// </summary>
        /// <returns>颜色数组副本。</returns>
        public static Color[] GetGridColors()
        {
            return (Color[])Colors_Grid_UnHighlight.Clone();
        }
        #endregion

        #region 外部控制开关
        /// <summary>
        /// 设置拖拽功能是否启用。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>禁用时重置拖拽状态并触发 SceneView 重绘。</description></item>
        /// </list>
        /// </summary>
        /// <param name="enabled">true=启用, false=禁用。</param>
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
        /// 获取拖拽功能是否启用。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>直接返回内部开关状态。</description></item>
        /// </list>
        /// </summary>
        /// <returns>true=启用, false=禁用。</returns>
        public static bool GetEnabled()
        {
            return isEnabled;
        }
        /// <summary>
        /// 切换启用状态。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>内部调用 <c>SetEnabled(!isEnabled)</c>。</description></item>
        /// </list>
        /// </summary>
        public static void ToggleEnabled()
        {
            SetEnabled(!isEnabled);
        }
        #endregion

        [DidReloadScripts]
        public static void reload()
        {
            isEnabled = XHud_Dashboard.utilitys_get_util_screeen_element_visual_placer();
        }
    }
}