namespace SevenStrikeModules.XHud.Utilitys
{
    using SevenStrikeModules.XHud;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public class Editor_XHud_Tool_FastSceneView
    {
        // 按钮尺寸常量
        private const int BUTTON_WIDTH = 120;
        private const int BUTTON_HEIGHT = 30;
        private const int BUTTON_SPACING = 5;  // 按钮之间的间距
        private const int RIGHT_MARGIN = 20;   // 右边距
        private const int BOTTOM_MARGIN = 40;  // 下边距

        static Editor_XHud_Tool_FastSceneView()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            Handles.BeginGUI();

            // ========== 计算右下角位置 ==========
            // 获取 SceneView 窗口的宽度和高度
            float viewWidth = sceneView.position.width;
            float viewHeight = sceneView.position.height;

            // 第一个按钮（XHud 管理器）的 X 坐标 = 窗口宽度 - 按钮宽度 - 右边距
            float btn1_x = viewWidth - BUTTON_WIDTH - RIGHT_MARGIN;
            // 第一个按钮的 Y 坐标 = 窗口高度 - 按钮高度 - 下边距
            float btn1_y = viewHeight - BUTTON_HEIGHT - BOTTOM_MARGIN;

            // 第二个按钮（XHud 快速操作）的 X 坐标 = 窗口宽度 - 按钮宽度 - 右边距
            float btn2_x = viewWidth - BUTTON_WIDTH - RIGHT_MARGIN;
            // 第二个按钮的 Y 坐标 = 第一个按钮 Y 坐标 - 按钮高度 - 按钮间距
            float btn2_y = btn1_y - BUTTON_HEIGHT - BUTTON_SPACING;

            // ========== 按钮1：XHud 管理器（底部右侧） ==========
            Rect btn_rect_mgr = new Rect(btn2_x, btn2_y, BUTTON_WIDTH, BUTTON_HEIGHT);
            if (Editor_XHud_GUI.Gui_Button(btn_rect_mgr, null, null, false, "XHud 管理器", "快速选中XHudManager物体", Editor_XHud_GUI.GetColor(HudColor.深空灰), Color.white, HudFilled.实体))
            {
                // 使用新的 API
                XHud_Manager[] managers = Object.FindObjectsByType<XHud_Manager>(
                    FindObjectsSortMode.None
                );

                if (managers.Length > 0)
                {
                    // 选中第一个管理器对应的物体
                    Selection.activeGameObject = managers[0].gameObject;
                    //sceneView.FrameSelected();
                }
                else
                {
                    EditorApplication.delayCall += () =>
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 快速视图工具消息", "未找到管理器", $"抱歉未在场景列表中找到 <color=#{XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary)}>XHud Manager</color>！", "明白", 0);
                    };
                }
            }

            // ========== 按钮2：XHud 快速操作（位于按钮1上方） ==========
            Rect btn_rect_faster = new Rect(btn1_x, btn1_y, BUTTON_WIDTH, BUTTON_HEIGHT);
            if (Editor_XHud_GUI.Gui_Button(btn_rect_faster, null, null, false, "XHud 快速操作", "", Editor_XHud_GUI.GetColor(HudColor.深空灰), Color.white, HudFilled.实体))
            {
                GenericMenu menu = new GenericMenu();

                // 添加带图标的菜单项
                menu.AddItem(new GUIContent("S 选中所有模块", EditorGUIUtility.IconContent("d_Rigidbody2D Icon").image), false, () =>
                {
                    //var modules = Object.FindObjectsByType<XHud_Manager>(
                    //    FindObjectsSortMode.None
                    //);

                    //if (modules.Length > 0)
                    //{
                    //    GameObject[] gameObjects = new GameObject[modules.Length];
                    //    for (int i = 0; i < modules.Length; i++)
                    //    {
                    //        gameObjects[i] = modules[i].gameObject;
                    //    }
                    //    Selection.objects = gameObjects;
                    //    Debug.Log($"已选中 {modules.Length} 个物体");
                    //}
                });

                menu.AddItem(new GUIContent("F 聚焦第一个模块", EditorGUIUtility.IconContent("d_Rigidbody2D Icon").image), false, () =>
                {
                    //var modules = Object.FindObjectsByType<XHud_Manager>(
                    //    FindObjectsSortMode.None
                    //);

                    //if (modules.Length > 0)
                    //{
                    //    Selection.activeGameObject = modules[0].gameObject;
                    //    SceneView.lastActiveSceneView.FrameSelected();
                    //}
                });

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("A 刷新场景", EditorGUIUtility.IconContent("d_Rigidbody2D Icon").image), false, () =>
                {
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    Debug.Log("场景已刷新");
                });

                menu.AddItem(new GUIContent("C 清除控制台", EditorGUIUtility.IconContent("d_Rigidbody2D Icon").image), false, () =>
                {
                    var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
                    var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    clearMethod.Invoke(null, null);
                });

                menu.ShowAsContext();
            }

            Handles.EndGUI();
        }
    }
}