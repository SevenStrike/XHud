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
* ============================================================================
*/
namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XTween.Editor;
    using UnityEditor;
    using UnityEngine;
    using XTween;

    /// <summary>
    /// 十字线定位工具 - Editor静态版本
    /// 用于在SceneView中显示十字线，支持动态瞄准动画
    /// </summary>
    public static class CrosshairTool
    {
        #region 私有字段

        private static bool _isDisplayEnabled = true;
        private static Color _aimColor = Color.green;
        private static float _lineThickness = 1f;
        private static float _dotSize = 20f;

        // 目标位置（归一化坐标 0-1）
        private static Vector2 _currentNormalizedPosition = new Vector2(0.5f, 0.5f);
        private static Vector2 _targetNormalizedPosition = new Vector2(0.5f, 0.5f);

        private static RectTransform _targetRectTransform;

        // 缓存
        private static Camera _sceneViewCamera;
        private static Rect _lastSceneViewRect;

        // 静态构造函数，确保订阅事件
        static CrosshairTool()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        #endregion

        #region 公共API - 基础设置

        /// <summary>
        /// 设置十字线颜色
        /// </summary>
        public static void AimColor(Color color)
        {
            _aimColor = color;
            RepaintSceneView();
        }

        /// <summary>
        /// 设置显示状态
        /// </summary>
        public static void AimDisplay(bool state)
        {
            _isDisplayEnabled = state;
            RepaintSceneView();
        }

        #endregion

        #region 公共API - 瞄准目标

        /// <summary>
        /// 对准UI元素
        /// </summary>
        public static void AimTo(RectTransform target)
        {
            AimDisplay(true);

            if (target == null)
            {
                ClearTarget();
                return;
            }

            _targetRectTransform = target;

            UpdateTargetPositionAndAnimate();

            FocusUI(target);
        }

        /// <summary>
        /// 设置屏幕位置（0-1范围，左下角为原点）
        /// </summary>
        public static void AimToScreenPosition(Vector2 normalizedPosition)
        {
            _targetRectTransform = null;
            _targetNormalizedPosition = new Vector2(
                Mathf.Clamp01(normalizedPosition.x),
                Mathf.Clamp01(normalizedPosition.y)
            );

            JumpToTarget();
        }

        /// <summary>
        /// 清除当前目标（带动画回到中心）
        /// </summary>
        public static void ClearTarget()
        {
            _targetRectTransform = null;

            // 带动画回到屏幕中心
            AimToScreenPosition(new Vector2(0, 0));
        }

        /// <summary>
        /// 立即跳转到目标位置（无动画）
        /// </summary>
        public static void JumpToTarget()
        {
            _currentNormalizedPosition = _targetNormalizedPosition;
            RepaintSceneView();
        }

        #endregion

        #region 位置更新与动画

        private static void UpdateTargetPositionAndAnimate()
        {
            if (!TryGetTargetNormalizedPosition(out Vector2 targetPos))
                return;

            _targetNormalizedPosition = targetPos;

            if (Vector2.Distance(_currentNormalizedPosition, _targetNormalizedPosition) < 0.001f)
                return;

            JumpToTarget();
        }

        private static bool TryGetTargetNormalizedPosition(out Vector2 position)
        {
            position = Vector2.zero;

            if (_sceneViewCamera == null)
            {
                if (SceneView.lastActiveSceneView?.camera != null)
                    _sceneViewCamera = SceneView.lastActiveSceneView.camera;
                else
                    return false;
            }

            if (_targetRectTransform == null)
                return false;

            position = RectTransformToNormalizedPosition(_targetRectTransform);
            return true;
        }

        private static Vector2 WorldToNormalizedPosition(Vector3 worldPosition)
        {
            if (_sceneViewCamera == null)
                return new Vector2(0.5f, 0.5f);

            Vector3 screenPoint = _sceneViewCamera.WorldToScreenPoint(worldPosition);

            if (screenPoint.z <= 0)
                return new Vector2(0.5f, 0.5f);

            float width = _lastSceneViewRect.width > 0 ? _lastSceneViewRect.width : Screen.width;
            float height = _lastSceneViewRect.height > 0 ? _lastSceneViewRect.height : Screen.height;

            float normalizedX = Mathf.Clamp01(screenPoint.x / width);
            float normalizedY = Mathf.Clamp01(1f - (screenPoint.y / height));

            return new Vector2(normalizedX, normalizedY - 0.035f);
        }

        private static Vector2 RectTransformToNormalizedPosition(RectTransform rectTransform)
        {
            if (_sceneViewCamera == null)
                return new Vector2(0.5f, 0.5f);

            Vector3[] worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);
            Vector3 centerWorld = (worldCorners[0] + worldCorners[2]) * 0.5f;

            return WorldToNormalizedPosition(centerWorld);
        }

        #endregion

        #region 预览动画

        /// <summary>
        /// 动画预览 - 播放
        /// </summary>
        private static void XTween_Preview_Start(XTween_Interface[] tweens)
        {
            if (tweens == null || tweens.Length == 0)
                return;

            // 预览动画杀死后自动清空预览器的列表
            Editor_XTween_Previewer.AfterKillClear = true;
            // 预览动画杀死前将动画目标的属性倒退
            Editor_XTween_Previewer.BeforeKillRewind = true;
            // 根据动画耗时自动杀死
            Editor_XTween_Previewer.AutoKillWithDuration = true;

            // 预览动画杀死后的委托事件
            Editor_XTween_Previewer.act_on_editor_autokill += XTween_OnAutoKillPreview;

            for (int i = 0; i < tweens.Length; i++)
            {
                Editor_XTween_Previewer.Append(tweens[i]);
            }

            Editor_XTween_Previewer.Play(null);
        }

        /// <summary>
        /// 动画预览 - 杀死
        /// </summary>
        private static void XTween_Preview_Kill()
        {
            // 预览器执行动作：杀死动画
            Editor_XTween_Previewer.Kill(true, true, null);

            // 当动画预览器为根据动画耗时自动杀死的情况下
            Editor_XTween_Previewer.act_on_editor_autokill -= XTween_OnAutoKillPreview;
        }

        /// <summary>
        /// 动画预览 - 自动杀死的委托
        /// </summary>
        private static void XTween_OnAutoKillPreview()
        {
            // 清空预览动画杀死后的委托事件
            Editor_XTween_Previewer.act_on_editor_autokill -= XTween_OnAutoKillPreview;
        }

        #endregion

        #region SceneView绘制

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!_isDisplayEnabled) return;

            // 更新相机引用
            _sceneViewCamera = sceneView.camera;
            _lastSceneViewRect = sceneView.position;

            if (TryGetTargetNormalizedPosition(out Vector2 targetPos))
            {
                _targetNormalizedPosition = targetPos;
                _currentNormalizedPosition = targetPos;
            }

            DrawCrosshair(sceneView);
        }

        private static void DrawCrosshair(SceneView sceneView)
        {
            float width = sceneView.position.width;
            float height = sceneView.position.height;

            // 计算当前实际屏幕坐标
            float crossX = _currentNormalizedPosition.x * width;
            float crossY = _currentNormalizedPosition.y * height;

            // 边界检查
            if (crossX < 0 || crossX > width || crossY < 0 || crossY > height)
                return;

            Handles.BeginGUI();

            Color oldColor = GUI.color;
            GUI.color = _aimColor;

            // 绘制水平线
            GUI.DrawTexture(new Rect(0, crossY - _lineThickness / 2, width, _lineThickness),
                EditorGUIUtility.whiteTexture);

            // 绘制垂直线
            GUI.DrawTexture(new Rect(crossX - _lineThickness / 2, 0, _lineThickness, height),
                EditorGUIUtility.whiteTexture);

            // 绘制中心点
            GUI.color = new Color(_aimColor.r, _aimColor.g, _aimColor.b, 0.8f);
            GUI.DrawTexture(new Rect(crossX - _dotSize / 2, crossY - _dotSize / 2, _dotSize, _dotSize),
                EditorGUIUtility.whiteTexture);

            GUI.color = oldColor;
            Handles.EndGUI();
        }

        #endregion

        #region 辅助方法

        private static void RepaintSceneView()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.Repaint();
            }
        }

        /// <summary>
        /// 获取当前十字线的屏幕位置（归一化坐标 0-1）
        /// </summary>
        public static Vector2 GetCurrentPosition()
        {
            return _currentNormalizedPosition;
        }

        /// <summary>
        /// 聚焦到UI物体
        /// </summary>
        /// <param name="target"></param>
        public static void FocusUI(RectTransform target)
        {
            if (target == null) return;

            // 获取 RectTransform
            RectTransform rectTransform = target;

            // 计算世界空间的四个角点
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            // 计算 Bounds
            Bounds bounds = new Bounds(corners[0], Vector3.zero);
            for (int i = 1; i < corners.Length; i++)
            {
                bounds.Encapsulate(corners[i]);
            }

            // 聚焦到 SceneView
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.Frame(bounds, false);
            }
        }

        #endregion
    }
}