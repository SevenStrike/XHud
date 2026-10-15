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
    /// 十字线定位工具 - Editor 静态版本。
    /// <para/>
    /// <list type="bullet">
    /// <item><description>用于在 SceneView 中显示十字线，支持动态瞄准动画。</description></item>
    /// <item><description>通过静态构造函数订阅 <c>SceneView.duringSceneGui</c>，全程自动绘制。</description></item>
    /// </list>
    /// </summary>
    public static class Editor_XHud_Tool_Crosshair
    {
        #region 字段 - 显示参数
        /// <summary>
        /// 是否启用十字线显示。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认启用。</description></item>
        /// </list>
        /// </summary>
        private static bool _isDisplayEnabled = true;
        /// <summary>
        /// 十字线及中心点的颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认绿色。</description></item>
        /// </list>
        /// </summary>
        private static Color _aimColor = Color.green;
        /// <summary>
        /// 十字线线条粗细（像素）。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认 1 像素。</description></item>
        /// </list>
        /// </summary>
        private static float _lineThickness = 1f;
        /// <summary>
        /// 中心点尺寸（像素）。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认 20 像素。</description></item>
        /// </list>
        /// </summary>
        private static float _dotSize = 20f;
        #endregion

        #region 字段 - 瞄准状态
        /// <summary>
        /// 当前十字线的归一化位置。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>取值范围 0-1，原点在左下角。</description></item>
        /// </list>
        /// </summary>
        private static Vector2 _currentNormalizedPosition = new Vector2(0.5f, 0.5f);
        /// <summary>
        /// 目标十字线的归一化位置。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>取值范围 0-1，原点在左下角。</description></item>
        /// </list>
        /// </summary>
        private static Vector2 _targetNormalizedPosition = new Vector2(0.5f, 0.5f);
        /// <summary>
        /// 当前瞄准的 UI 目标。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>为 <c>null</c> 时表示使用归一化位置瞄准。</description></item>
        /// </list>
        /// </summary>
        private static RectTransform _targetRectTransform;
        #endregion

        #region 字段 - 缓存
        /// <summary>
        /// 缓存的 SceneView 相机。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnSceneGUI</c> 中自动更新。</description></item>
        /// </list>
        /// </summary>
        private static Camera _sceneViewCamera;
        /// <summary>
        /// 上一次的 SceneView 区域。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>用于计算归一化位置。</description></item>
        /// </list>
        /// </summary>
        private static Rect _lastSceneViewRect;
        #endregion

        #region 生命周期
        /// <summary>
        /// 静态构造函数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>订阅 <c>SceneView.duringSceneGui</c>，确保十字线全程自动绘制。</description></item>
        /// </list>
        /// </summary>
        static Editor_XHud_Tool_Crosshair()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }
        #endregion

        #region 公共API - 基础设置
        /// <summary>
        /// 设置十字线颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>设置后立即触发 SceneView 重绘。</description></item>
        /// </list>
        /// </summary>
        /// <param name="color">目标颜色。</param>
        public static void AimColor(Color color)
        {
            _aimColor = color;
            RepaintSceneView();
        }
        /// <summary>
        /// 设置显示状态。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>设置后立即触发 SceneView 重绘。</description></item>
        /// </list>
        /// </summary>
        /// <param name="state">为 <c>true</c> 时显示，为 <c>false</c> 时隐藏。</param>
        public static void AimDisplay(bool state)
        {
            _isDisplayEnabled = state;
            RepaintSceneView();
        }
        #endregion

        #region 公共API - 瞄准目标
        /// <summary>
        /// 对准 UI 元素。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>自动开启显示并计算目标归一化位置。</description></item>
        /// <item><description>将 SceneView 聚焦到目标。</description></item>
        /// <item><description>目标为 <c>null</c> 时清除目标。</description></item>
        /// </list>
        /// </summary>
        /// <param name="target">目标 RectTransform。</param>
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
        /// 设置屏幕位置。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>参数为归一化坐标，0-1 范围，左下角为原点。</description></item>
        /// <item><description>立即跳转到目标位置（无动画）。</description></item>
        /// </list>
        /// </summary>
        /// <param name="normalizedPosition">归一化坐标。</param>
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
        /// 清除当前目标。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>带动画回到屏幕中心。</description></item>
        /// </list>
        /// </summary>
        public static void ClearTarget()
        {
            _targetRectTransform = null;

            // 带动画回到屏幕中心
            AimToScreenPosition(new Vector2(0, 0));
        }
        /// <summary>
        /// 立即跳转到目标位置。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>无动画，直接设置当前位置为目标位置。</description></item>
        /// </list>
        /// </summary>
        public static void JumpToTarget()
        {
            _currentNormalizedPosition = _targetNormalizedPosition;
            RepaintSceneView();
        }
        #endregion

        #region 工具 - 位置计算
        /// <summary>
        /// 更新目标位置并执行动画。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>尝试获取目标的归一化位置。</description></item>
        /// <item><description>位置差异大于阈值时跳转到目标位置。</description></item>
        /// </list>
        /// </summary>
        private static void UpdateTargetPositionAndAnimate()
        {
            if (!TryGetTargetNormalizedPosition(out Vector2 targetPos))
                return;

            _targetNormalizedPosition = targetPos;

            if (Vector2.Distance(_currentNormalizedPosition, _targetNormalizedPosition) < 0.001f)
                return;

            JumpToTarget();
        }
        /// <summary>
        /// 尝试获取目标归一化位置。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>缓存 SceneView 相机引用。</description></item>
        /// <item><description>目标为空时返回 <c>false</c>。</description></item>
        /// </list>
        /// </summary>
        /// <param name="position">输出归一化位置。</param>
        /// <returns>获取成功返回 <c>true</c>。</returns>
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
        /// <summary>
        /// 世界坐标转归一化位置。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>相机为空或点在相机后方时返回屏幕中心。</description></item>
        /// <item><description>结果自动做 0-1 钳制。</description></item>
        /// </list>
        /// </summary>
        /// <param name="worldPosition">世界坐标。</param>
        /// <returns>归一化位置。</returns>
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
        /// <summary>
        /// RectTransform 转归一化位置。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>取四个世界角点的中心点进行换算。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rectTransform">目标 RectTransform。</param>
        /// <returns>归一化位置。</returns>
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

        #region 工具 - 动画预览
        /// <summary>
        /// 动画预览 - 播放。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>配置预览器为「杀死后自动清空列表」「杀死前倒退属性」「按耗时自动杀死」。</description></item>
        /// <item><description>追加并播放传入的动画。</description></item>
        /// </list>
        /// </summary>
        /// <param name="tweens">动画数组。</param>
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
        /// 动画预览 - 杀死。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>执行预览器杀死动作。</description></item>
        /// <item><description>取消自动杀死委托的订阅。</description></item>
        /// </list>
        /// </summary>
        private static void XTween_Preview_Kill()
        {
            // 预览器执行动作：杀死动画
            Editor_XTween_Previewer.Kill(true, true, null);

            // 当动画预览器为根据动画耗时自动杀死的情况下
            Editor_XTween_Previewer.act_on_editor_autokill -= XTween_OnAutoKillPreview;
        }
        /// <summary>
        /// 动画预览 - 自动杀死的委托。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>取消自动杀死委托的订阅。</description></item>
        /// </list>
        /// </summary>
        private static void XTween_OnAutoKillPreview()
        {
            // 清空预览动画杀死后的委托事件
            Editor_XTween_Previewer.act_on_editor_autokill -= XTween_OnAutoKillPreview;
        }
        #endregion

        #region 绘制 - SceneView
        /// <summary>
        /// SceneView 绘制回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>更新相机与 SceneView 区域缓存。</description></item>
        /// <item><description>存在目标时同步目标归一化位置。</description></item>
        /// <item><description>调用十字线绘制。</description></item>
        /// </list>
        /// </summary>
        /// <param name="sceneView">当前 SceneView。</param>
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
        /// <summary>
        /// 绘制十字线。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>按当前归一化位置换算为屏幕坐标。</description></item>
        /// <item><description>绘制水平线、垂直线与中心点。</description></item>
        /// </list>
        /// </summary>
        /// <param name="sceneView">当前 SceneView。</param>
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

        #region 工具 - 辅助
        /// <summary>
        /// 触发 SceneView 重绘。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在存在活动 SceneView 时执行。</description></item>
        /// </list>
        /// </summary>
        private static void RepaintSceneView()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.Repaint();
            }
        }
        /// <summary>
        /// 获取当前十字线的屏幕位置。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>返回归一化坐标 0-1。</description></item>
        /// </list>
        /// </summary>
        /// <returns>归一化位置。</returns>
        public static Vector2 GetCurrentPosition()
        {
            return _currentNormalizedPosition;
        }
        /// <summary>
        /// 聚焦到 UI 物体。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>取目标四个世界角点构建包围盒。</description></item>
        /// <item><description>调用 <c>SceneView.Frame</c> 聚焦。</description></item>
        /// </list>
        /// </summary>
        /// <param name="target">目标 RectTransform。</param>
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