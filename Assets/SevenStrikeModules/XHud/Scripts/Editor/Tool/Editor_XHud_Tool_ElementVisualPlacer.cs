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
    // ==================== 外部控制开关 ====================

    private static bool isEnabled = true;  // 默认启用

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

    // =================================================

    private static List<Rect> gridRects = new List<Rect>();
    private static bool isDraggingTarget = false;
    private static GameObject[] draggedPrefabs = null;
    private static string targetComponentName = "XHud_Module_Element";
    private static XHud_Manager mgr;
    // 当前鼠标悬停的格子索引，-1 表示没有悬停
    private static int hoverGridIndex = -1;

    // 九宫格颜色数组（9个格子独立颜色）
    private static Color[] Colors_Grid_UnHighlight = new Color[]
    {
        // 格子0 - 左上 (LU)
        Color.black*0.4f,
        // 格子1 - 上 (U)
        Color.black*0.4f,
        // 格子2 - 右上 (RU)
        Color.black*0.4f,
        // 格子3 - 左 (L)
        Color.black*0.4f,
        // 格子4 - 中心 (C)
        Color.black*0.4f,
        // 格子5 - 右 (R)
        Color.black*0.4f,
        // 格子6 - 左下 (LD)
        Color.black*0.4f,
        // 格子7 - 下 (D)
        Color.black*0.4f,
        // 格子8 - 右下 (RD)
        Color.black*0.4f
    };
    private static Color Color_Grid_Highlight = XHud_Dashboard.Theme_Primary * 0.5f;

    private static Color Color_Edge_Highlight = Color.white * 0.6f;
    private static Color Color_Edge_UnHighlight = Color.black;

    private static Color Color_Label_Highlight = Color.white;

    /// <summary>
    /// 字体 - 粗体
    /// </summary>
    private static Font Font_Bold;
    /// <summary>
    /// 字体 - 细体
    /// </summary>
    private static Font Font_Light;

    static Editor_XHud_Tool_ElementVisualPlacer()
    {
        SceneView.duringSceneGui += OnSceneGUI;

        Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
        Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Light");
    }

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

    // ==================== 拖拽放置处理方法 ====================

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

    // ==================== 辅助方法 ====================

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

    private static int GetGridIndex(Vector2 mousePos)
    {
        for (int i = 0; i < gridRects.Count; i++)
        {
            if (gridRects[i].Contains(mousePos))
                return i;
        }
        return -1;
    }

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
    private static Font GetBoldFont()
    {
        if (Font_Bold == null)
            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
        return Font_Bold;
    }

    private static Font GetLightFont()
    {
        if (Font_Light == null)
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Light");
        return Font_Light;
    }
}