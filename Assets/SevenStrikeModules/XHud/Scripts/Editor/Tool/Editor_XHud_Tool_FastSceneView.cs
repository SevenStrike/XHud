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
    using UnityEngine;

    /// <summary>
    /// SceneView 快速操作工具。
    /// <para/>
    /// <list type="bullet">
    /// <item><description>在 SceneView 右下角提供「XHud 快速操作」按钮。</description></item>
    /// <item><description>通过 <c>[InitializeOnLoad]</c> 自动订阅 <c>SceneView.duringSceneGui</c>。</description></item>
    /// <item><description>通过 <c>EditorApplication.playModeStateChanged</c> 在 Play/Edit 切换时重新同步状态。</description></item>
    /// <item><description>菜单项提供选中锚点/画布/元素、切换放置器、切换图元 ID 显示、刷新场景、清除控制台等快捷操作。</description></item>
    /// </list>
    /// </summary>
    [InitializeOnLoad]
    public class Editor_XHud_Tool_FastSceneView
    {
        #region 常量 - 按钮布局
        /// <summary>
        /// 按钮距离 SceneView 右边距（像素）。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认 20。</description></item>
        /// </list>
        /// </summary>
        private const int right_margin = 20;
        /// <summary>
        /// 按钮距离 SceneView 下边距（像素）。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认 45。</description></item>
        /// </list>
        /// </summary>
        private const int bottom_margin = 45;
        #endregion

        #region 图标
        private static Texture2D icon_mgr_r;
        private static Texture2D icon_mgr_p;
        #endregion

        private static bool pressed_inside = false;   // 记录按下时是否在范围内
        private static bool was_hover = false;        // 上一帧的 hover 状态，用于判断进出

        #region 生命周期
        /// <summary>
        /// 静态构造函数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>由 <c>[InitializeOnLoad]</c> 触发（域重载后执行一次）。</description></item>
        /// <item><description>订阅 <c>SceneView.duringSceneGui</c> 与 <c>EditorApplication.playModeStateChanged</c>。</description></item>
        /// <item><description>从 <c>XHud_Dashboard</c> 读取并恢复放置器与图元 ID 显示的开关状态。</description></item>
        /// </list>
        /// </summary>
        static Editor_XHud_Tool_FastSceneView()
        {
            // 先减后加，防止重复订阅（域重载 / Enter Play Mode Options 关闭域重载时可能导致多次订阅）
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            // 立即同步一次工具开关状态（无管理器时 SyncUtilitiesState 内部会自行守卫）
            SyncUtilitiesState();
        }

        /// <summary>
        /// Play/Edit 模式切换回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>进入/退出 Play 模式后延迟一帧，等待场景、管理器、Awake 全部就绪。</description></item>
        /// <item><description>无管理器时直接返回，按钮由 <c>OnSceneGUI</c> 的守卫自动隐藏。</description></item>
        /// <item><description>有管理器时重新同步工具开关状态并强制刷新 SceneView。</description></item>
        /// </list>
        /// </summary>
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // 延迟一帧，等场景 / 管理器 / Awake 全部就绪后再检测
            EditorApplication.delayCall += () =>
            {
                if (XHud_Dashboard.get_manager() == null)
                    return;

                // 图标可能因域重载丢失，懒加载
                if (icon_mgr_r == null || icon_mgr_p == null)
                    LoadIcon();

                SyncUtilitiesState();

                // 强制刷新 SceneView，让按钮立即出现
                SceneView.RepaintAll();
            };
        }

        /// <summary>
        /// 将 <c>XHud_Dashboard</c> 中持久化的工具开关状态同步给对应工具。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>元素视觉放置器（ElementVisualPlacer）。</description></item>
        /// <item><description>视觉瞄具（Activate_Aimer）。</description></item>
        /// </list>
        /// </summary>
        private static void SyncUtilitiesState()
        {
            Editor_XHud_Tool_SceneView_ElementVisualPlacer.SetEnabled(
                XHud_Dashboard.utilitys_get_util_screeen_element_visual_placer());
            Editor_XHud_Tool_SceneView_Activate_Aimer.SetEnabled(
                XHud_Dashboard.utilitys_get_util_screeen_aimer());
        }

        /// <summary>
        /// 加载 SceneView 按钮图标（幂等）。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>已加载时直接返回，避免重复读取资源。</description></item>
        /// <item><description>域重载 / Play 切换后静态字段可能为 null，由调用方按需触发。</description></item>
        /// </list>
        /// </summary>
        private static void LoadIcon()
        {
            if (icon_mgr_r != null && icon_mgr_p != null)
                return;

            icon_mgr_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/logo_sceneview_r");
            icon_mgr_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/logo_sceneview_p");
        }
        #endregion

        #region 绘制 - SceneView 按钮
        /// <summary>
        /// SceneView 的 GUI 事件处理函数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>每帧绘制前检测管理器，无管理器时不绘制按钮（避免 NRE）。</description></item>
        /// <item><description>在右下角绘制「XHud 快速操作」按钮。</description></item>
        /// <item><description>点击后弹出快捷菜单，提供选中锚点/画布/元素、切换工具、刷新场景、清除控制台等操作。</description></item>
        /// </list>
        /// </summary>
        /// <param name="sceneView">当前的 SceneView 窗口实例。</param>
        private static void OnSceneGUI(SceneView sceneView)
        {
            //  每次绘制前检测管理器：
            //   - Play 模式走单例 XHud_Manager.instance（O(1)）
            //   - Edit 模式走静态缓存（O(1)）
            //   - 无管理器时直接返回，不绘制按钮
            if (XHud_Dashboard.get_manager() == null)
                return;

            //  图标懒加载（域重载 / Play 切换后可能丢失）
            if (icon_mgr_r == null || icon_mgr_p == null)
                LoadIcon();

            // 图标加载失败也不绘制，避免 NRE
            if (icon_mgr_r == null || icon_mgr_p == null)
                return;

            Handles.BeginGUI();

            // 获取 SceneView 窗口的宽度和高度
            float viewWidth = sceneView.position.width;
            float viewHeight = sceneView.position.height;

            // 按钮的 X 坐标 = 窗口宽度 - 按钮宽度 - 右边距
            float btn_x = viewWidth - icon_mgr_r.width - right_margin;

            // 按钮的 Y 坐标 = 窗口高度 - 按钮高度 - 下边距
            float btn_mgr_y = viewHeight - icon_mgr_r.height - bottom_margin;

            Rect btn_rect_mgr = new Rect(btn_x, btn_mgr_y, icon_mgr_r.width, icon_mgr_r.height);

            Event e = Event.current;
            Vector2 mousePos = e.mousePosition;
            bool isHover = btn_rect_mgr.Contains(mousePos);

            // ---------- 进入 / 退出（通过状态变化判断，比 MouseEnter/Leave 可靠）----------
            if (isHover && !was_hover)
            {
                // 鼠标进入
                OnMouseEnter();
            }
            else if (!isHover && was_hover)
            {
                // 鼠标退出
                OnMouseLeave();
            }
            was_hover = isHover;

            // ---------- 按下 / 松开 / 点击 ----------
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (isHover && e.button == 0)
                    {
                        pressed_inside = true;
                        OnMouseDown();
                        e.Use(); // 只有按在按钮上才消费
                    }
                    break;

                case EventType.MouseUp:
                    if (e.button == 0 && pressed_inside)
                    {
                        // 只有"从按钮上按下过"才接管这次松开
                        if (isHover) OnClick();   // 按下+松开都在范围内 = 点击
                        OnMouseUp();
                        pressed_inside = false;
                        e.Use();
                    }
                    //  否则（_pressedInside == false）：完全不 Use，放行给 SceneView
                    break;

                case EventType.MouseDrag:
                    if (pressed_inside)
                    {
                        // 按下并拖动中
                    }
                    break;
            }

            XGUI.gui_icon(
                rect: btn_rect_mgr,
                color: isHover ? (pressed_inside ? Color.black * 0.7f : Color.white) : Color.white * 0.75f,
                icon: icon_mgr_r);

            XGUI.gui_label(
                rect: new Rect(btn_rect_mgr.x + 12, btn_rect_mgr.y - 5, 20, 20),
                text: new GUIContent("X"),
                text_color: isHover ? (pressed_inside ? Color.black * 0.7f : XHud_Dashboard.themes_get_color(ThemeColorType.primary)) : XHud_Dashboard.themes_get_color(ThemeColorType.primary) * 0.75f,
                size: XGUIFontSize.XL,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Bold,
                font: XGUI.GetFont("xg-bold"));

            Handles.EndGUI();
        }

        static void OnMouseEnter()
        {
            //Debug.Log("/* 鼠标进入 */");
        }
        static void OnMouseLeave()
        {
            //Debug.Log("/* 鼠标退出 */");
        }
        static void OnMouseDown()
        {
            //Debug.Log("/* 鼠标按下 */");
        }
        static void OnMouseUp()
        {
            //Debug.Log("/* 鼠标松开 */");
        }
        static void OnClick()
        {
            //Debug.Log("/* 点击生效 */");

            GenericMenu menu = new GenericMenu();

            // 获取当前视觉放置器 / 视觉瞄具 / 图元 ID 显示的启用状态
            bool isPlacerEnabled = Editor_XHud_Tool_SceneView_ElementVisualPlacer.GetEnabled();
            bool isAimEnabled = Editor_XHud_Tool_SceneView_Activate_Aimer.GetEnabled();
            bool isPrimitiveControllerIDDisplay = XHud_Dashboard.primitive_get_controller_displayid();

            XHud_Manager mgr = XHud_Dashboard.get_manager();
            if (mgr != null)
            {
                for (int i = 0; i < mgr.cv_anchors_layout_screen.Count; i++)
                {
                    Anchor_Layout anchor = mgr.cv_anchors_layout_screen[i];
                    menu.AddItem(new GUIContent($"S 选择锚点/{anchor.Name}（{anchor.Type}）"), false, () =>
                    {
                        Selection.activeGameObject = anchor.Anchor.gameObject;
                    });
                }
            }

            menu.AddSeparator("");

            if (mgr != null && mgr.cv_screen != null)
            {
                menu.AddItem(new GUIContent("F 选中屏幕画布"), false, () =>
                {
                    Selection.activeGameObject = mgr.cv_screen.gameObject;
                });
            }

            if (mgr != null && mgr.cv_hudcanvas_world != null)
            {
                menu.AddItem(new GUIContent("W 选中世界画布"), false, () =>
                {
                    Selection.activeGameObject = mgr.cv_hudcanvas_world.gameObject;
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

            // ========== 可勾选的菜单项：元素视觉放置器 ==========
            menu.AddItem(new GUIContent("T 元素视觉放置器"), isPlacerEnabled, () =>
            {
                bool newState = !isPlacerEnabled;
                Editor_XHud_Tool_SceneView_ElementVisualPlacer.SetEnabled(newState);
                XHud_Dashboard.utilitys_set_util_screeen_element_visual_placer(newState);
            });

            // ========== 可勾选的菜单项：视觉瞄具 ==========
            menu.AddItem(new GUIContent("A 视觉瞄具"), isAimEnabled, () =>
            {
                bool newState = !isAimEnabled;
                Editor_XHud_Tool_SceneView_Activate_Aimer.SetEnabled(newState);
                XHud_Dashboard.utilitys_set_util_screeen_aimer(newState);
            });

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("P 图元ID在场景中显示"), isPrimitiveControllerIDDisplay, () =>
            {
                bool newState = !isPrimitiveControllerIDDisplay;
                XHud_Dashboard.primitive_set_controller_displayid(newState);
            });

            menu.AddItem(new GUIContent("R 刷新场景"), false, () =>
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

            menu.AddItem(new GUIContent("X 快速选中管理器"), false, () =>
            {
                XHud_Manager manager = XHud_Dashboard.get_manager();

                if (manager != null)
                {
                    Selection.activeGameObject = manager.gameObject;
                }
                else
                {
                    EditorApplication.delayCall += () =>
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 快速视图工具消息",
                            title: "未找到管理器",
                            msg: $"抱歉未在场景列表中找到 <color=#{XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary))}>XHud Manager</color>！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    };
                }
            });

            menu.ShowAsContext();
        }
        #endregion
    }
}