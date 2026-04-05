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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
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

            Rect btn_rect_faster = new Rect(btn1_x, btn1_y, BUTTON_WIDTH, BUTTON_HEIGHT);
            if (Editor_XHud_GUI.Gui_Button(btn_rect_faster, null, null, false, "XHud 快速操作", "", Editor_XHud_GUI.GetColor(HudColor.深空灰), Color.white, HudFilled.实体))
            {
                GenericMenu menu = new GenericMenu();

                XHud_Manager mgr = XHud_Dashboard.HudManagerGet();
                if (mgr != null)
                {
                    for (int i = 0; i < mgr.Anchors_Layout_Screen.Count; i++)
                    {
                        Anchor_Layout anchor = mgr.Anchors_Layout_Screen[i];
                        menu.AddItem(new GUIContent($"S 选择锚点/{anchor.Name}（{anchor.Type}）"), false, () =>
                        {
                            Selection.activeGameObject = anchor.Anchor.gameObject;
                        });
                    }
                }

                menu.AddSeparator("");

                if (mgr.HudCanvas_Screen != null)
                {
                    menu.AddItem(new GUIContent("F 选中屏幕画布"), false, () =>
                    {
                        Selection.activeGameObject = mgr.HudCanvas_Screen.gameObject;
                    });
                }

                if (mgr.HudCanvas_Screen != null)
                {
                    menu.AddItem(new GUIContent("W 选中世界画布"), false, () =>
                    {
                        Selection.activeGameObject = mgr.HudCanvas_World.gameObject;
                    });
                }

                menu.AddItem(new GUIContent("E 选中所有元素"), false, () =>
                {
                    var modules = Object.FindObjectsByType<XHud_Module_Element>(
                        FindObjectsSortMode.None
                    );

                    List<GameObject> objs = new List<GameObject>();
                    for (int i = 0; i < modules.Length; i++)
                    {
                        objs.Add(modules[i].gameObject);
                    }

                    if (objs.Count > 0)
                    {
                        Selection.objects = objs.ToArray();
                    }
                });

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("A 刷新场景"), false, () =>
                {
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    Debug.Log("场景已刷新");
                });

                menu.AddItem(new GUIContent("C 清除控制台"), false, () =>
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