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
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Manager : Editor
    {
        #region 主逻辑更新
        /// <summary>
        /// 刷新辅助视觉。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>运行时直接返回。</description></item>
        /// <item><description>按条件切换安全框结构显示。</description></item>
        /// <item><description>刷新布局、安全框与转场顶层视图。</description></item>
        /// </list>
        /// </summary>
        private void xHud_EditorUpdate_HelperVisual()
        {
            if (Application.isPlaying)
                return;

            if (SceneCamera.objectReferenceValue != null && ScreenRes.vector2Value != Vector2.zero && UseSafeFrame.boolValue)
                xHud_HiddenStructureDisplayer(SafeFrameStructureDisplayer.boolValue);

            BaseScript.hm_Layout_Update();

            BaseScript.hm_SafeFrameUpdate();

            BaseScript.hm_TransitionTopView();
        }
        /// <summary>
        /// 刷新画布与相机。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>将屏幕画布与相机、世界画布与场景相机绑定。</description></item>
        /// </list>
        /// </summary>
        private void xHud_EditorUpdate_Canvas_Camera()
        {
            if (Application.isPlaying)
                return;

            Canvas canvas_s = (Canvas)HudCanvas_Screen.objectReferenceValue;
            if (canvas_s != null)
            {
                canvas_s.renderMode = RenderMode.ScreenSpaceCamera;

                Camera cam = (Camera)HudCamera.objectReferenceValue;
                if (cam != null)
                {
                    canvas_s.worldCamera = cam;
                }
            }

            Canvas canvas_w = (Canvas)HudCanvas_World.objectReferenceValue;
            if (canvas_w != null)
            {
                canvas_w.renderMode = RenderMode.WorldSpace;

                Camera cam = (Camera)SceneCamera.objectReferenceValue;
                if (cam != null)
                {
                    canvas_w.worldCamera = cam;
                }
            }
        }
        /// <summary>
        /// 刷新相机参数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>同步剪切范围、正交模式、正交尺寸与透视视场角。</description></item>
        /// </list>
        /// </summary>
        private void xHud_EditorUpdate_CameraArgs()
        {
            BaseScript.hm_CameraCutterRange(BaseScript.CameraCutter_Near, BaseScript.CameraCutter_Far);
            BaseScript.hm_CameraOrthographicProjection(CameraOthograpicMode.boolValue);
            BaseScript.hm_CameraOrthographicSize(BaseScript.CameraOrthographicSize);
            BaseScript.hm_CameraPerspectiveFov(BaseScript.CameraFov);
        }
        /// <summary>
        /// 刷新掩码。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>调用管理器刷新掩码设置。</description></item>
        /// </list>
        /// </summary>
        private void xHud_EditorUpdate_Mask()
        {
            BaseScript.hm_MaskUpdate();
        }
        /// <summary>
        /// 刷新内容透明度。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>同步刷新屏幕与世界的透明度设置。</description></item>
        /// </list>
        /// </summary>
        private void xHud_EditorUpdate_ContentOpacity()
        {
            BaseScript.hm_Screen_ContentOpacity_Update();
            BaseScript.hm_World_ContentOpacity_Update();
        }
        #endregion

        #region 委托事件
        /// <summary>
        /// 层级变更事件回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>按刷新间隔降频执行辅助视觉与画布相机刷新。</description></item>
        /// </list>
        /// </summary>
        private void EditorApplication_EditorManagerUpdate()
        {
            // 用户操作后保持高频 0.8 秒，然后降回低频
            if (EditorApplication.timeSinceStartup - _lastUserActionTime > UPDATE_KEEPTIME)
            {
                UPDATE_INTERVAL = UPDATE_INTERVAL_Low;
            }

            if (EditorApplication.timeSinceStartup - _lastHierarchyUpdate < UPDATE_INTERVAL)
                return;
            _lastHierarchyUpdate = EditorApplication.timeSinceStartup;

            xHud_EditorUpdate_HelperVisual();
            xHud_EditorUpdate_Canvas_Camera();
        }
        #endregion

        #region 面板折叠
        /// <summary>
        /// 面板折叠键值设置。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>批量设置管理器所有面板的折叠状态。</description></item>
        /// <item><description>触发 Inspector 重绘。</description></item>
        /// </list>
        /// </summary>
        /// <param name="state">折叠状态。</param>
        private void xHud_GroupFolds(bool state)
        {
            BaseScript.fold_options = state;
            BaseScript.fold_canvasset = state;
            BaseScript.fold_camera = state;
            BaseScript.fold_ratiorefer = state;
            BaseScript.fold_compguid = state;
            BaseScript.fold_reslibs = state;
            BaseScript.fold_elementlibs = state;
            BaseScript.fold_soundslib = state;
            BaseScript.fold_masks = state;
            BaseScript.fold_global = state;
            BaseScript.fold_tool = state;
            BaseScript.fold_RMS = state;
            BaseScript.fold_layout = state;
            BaseScript.fold_visualsafe = state;
            BaseScript.fold_blueprint = state;
            BaseScript.fold_based = state;
            BaseScript.fold_motion_default = state;
            BaseScript.fold_themes = state;

            Repaint();
        }
        #endregion

        #region 频率控制
        /// <summary>
        /// 进入高频刷新模式。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>将刷新间隔设置为高频值并记录用户操作时间。</description></item>
        /// </list>
        /// </summary>
        private void SetHighFrequencyMode()
        {
            UPDATE_INTERVAL = UPDATE_INTERVAL_High;
            _lastUserActionTime = EditorApplication.timeSinceStartup;
        }
        #endregion
    }
}