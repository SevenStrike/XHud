/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
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
namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XTween;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public enum MouseType
    {
        左键,
        中键,
        右键
    }

    [System.Serializable]
    public class MouseStyles
    {
        public string Name;
        public Sprite Sprite;
        public Vector2 Pivot;
    }

    [RequireComponent(typeof(CanvasGroup))]
    public class XHud_CustomMouseCursor : MonoBehaviour
    {
        /// <summary>
        /// 自定义鼠标样式
        /// </summary>
        public bool UseCustomCursor;
        /// <summary>
        /// 鼠标锚点
        /// </summary>
        public RectTransform CursorRect;
        /// <summary>
        /// 鼠标透明度组件
        /// </summary>
        public CanvasGroup CursorCanvasGroup;
        /// <summary>
        /// 鼠标图像组件
        /// </summary>
        public Image CursorImager;

        [Range(0, 1)]
        /// <summary>
        /// 鼠标透明度
        /// </summary>
        public float CursorOpacity = 1;

        Vector2 CalaculatePos;

        public MouseType MouseClickType = MouseType.左键;

        /// <summary>
        /// 当前正在使用的样式
        /// </summary>
        public MouseStyles CurrentCursorStyle;
        /// <summary>
        /// 用于重置的样式
        /// </summary>
        private MouseStyles OriginalMouseStyle;

        /// <summary>
        /// 光标尺寸
        /// </summary>
        public float CursorSize = 20;

        /// <summary>
        /// 光标颜色
        /// </summary>
        public Color CursorColor = Color.white;
        public float CursorColorSmooth = 1;

        /// <summary>
        /// 鼠标样式列表
        /// </summary>
        public List<MouseStyles> MouseStyles;

        /// <summary>
        /// 鼠标按下时
        /// </summary>
        public UnityAction<Vector3> act_on_press;
        /// <summary>
        /// 鼠标抬起时
        /// </summary>
        public UnityAction<Vector3> act_on_release;
        /// <summary>
        /// 鼠标样式切换时
        /// </summary>
        public UnityAction<Vector3, string, MouseStyles> act_on_changed;

        public string IndexMouseType = "左键";

        public float CursorImagerSizeSmooth = 1;

        public string UseLerpCursorSize = "差值模式";

        private XTween_Interface Tween_CursorSize;
        public float CursorSize_TweenDuration = 1;
        public EaseMode CursorSize_TweenEase;

        private XTween_Interface Tween_CursorOpacity;
        public float CursorOpacity_TweenDuration = 1;
        public EaseMode CursorOpacity_TweenEase;

        void Start()
        {
            InitializeCursor();
        }

        /// <summary>
        /// 初始化光标
        /// </summary>
        private void InitializeCursor()
        {
            if (transform.childCount <= 0)
            {
                GameObject imager = new GameObject();
                imager.transform.SetParent(transform);
                imager.transform.localPosition = Vector3.zero;
                imager.transform.localEulerAngles = Vector3.zero;
                imager.transform.localScale = Vector3.one;
                imager.name = "CursorImager";
                imager.layer = LayerMask.NameToLayer("XHud");
                RectTransform rect = imager.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(10, 10);
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                Image img = imager.AddComponent<Image>();
                img.raycastTarget = false;
                CursorImager = img;
                CursorRect = rect;
            }
            else
            {
                CursorImager = transform.GetChild(0).GetComponent<Image>();
            }

            CursorRect = GetComponent<RectTransform>();

            CursorCanvasGroup = GetComponent<CanvasGroup>();
            if (CursorCanvasGroup == null)
            {
                CursorCanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            if (CurrentCursorStyle != null)
            {
                OriginalMouseStyle = CurrentCursorStyle;
            }

            CursorImager.rectTransform.sizeDelta = Vector2.one * CursorSize;
            CursorImager.color = CursorColor;
        }

        void Update()
        {
            mc_CustomCursorUpdate();
        }

        /// <summary>
        /// 是否开启自定义光标
        /// </summary>
        /// <param tweenName="treeState">开启状态</param>
        public void mc_SetCustomCursorEnabled(bool state)
        {
            UseCustomCursor = state;
            if (!state)
                Cursor.visible = true;
        }

        /// <summary>
        /// 获取按键类型
        /// </summary>
        /// <returns></returns>
        private int mc_GetMouseType()
        {
            int index = 0;
            switch (MouseClickType)
            {
                case MouseType.左键:
                    index = 0;
                    break;
                case MouseType.中键:
                    index = 2;
                    break;
                case MouseType.右键:
                    index = 1;
                    break;
            }
            return index;
        }

        /// <summary>
        /// 更新鼠标位置
        /// </summary>
        private void mc_CustomCursorUpdate()
        {
            if (!UseCustomCursor)
            {
                Cursor.visible = true;
                if (CursorImager != null)
                    CursorImager.enabled = false;
                return;
            }
            else
            {
                Cursor.visible = false;
                if (CursorImager != null)
                    CursorImager.enabled = true;
            }

            ///---同步样式到鼠标位置
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(XHud_Manager.Instance.HudCanvas_Screen.transform as RectTransform, Input.mousePosition, XHud_Manager.Instance.HudCamera, out CalaculatePos))
            {

                CursorRect.anchoredPosition = CalaculatePos;
            }

            ///---点击事件
            if (Input.GetMouseButtonDown(mc_GetMouseType()))
            {
                if (act_on_press != null)
                    act_on_press(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(mc_GetMouseType()))
            {
                if (act_on_release != null)
                    act_on_release(Input.mousePosition);
            }

            if (CursorCanvasGroup != null)
            {
                ///---鼠标样式透明度
                CursorCanvasGroup.alpha = CursorOpacity;
            }

            if (UseLerpCursorSize == "差值模式")
                CursorImager.rectTransform.sizeDelta = Vector2.Lerp(CursorImager.rectTransform.sizeDelta, Vector2.one * CursorSize, Time.deltaTime * CursorImagerSizeSmooth);
            else
                CursorImager.rectTransform.sizeDelta = Vector2.one * CursorSize;

            mc_SetCursorImager(CurrentCursorStyle);

            CursorImager.color = Color.Lerp(CursorImager.color, CursorColor, Time.deltaTime * CursorColorSmooth);
        }

        /// <summary>
        /// 设置鼠标图像
        /// </summary>
        /// <param tweenName="cursor"></param>
        public void mc_SetCursorImager(MouseStyles style)
        {
            if (CursorImager == null)
                return;
            if (style == null)
                return;
            CursorImager.sprite = style.Sprite;
            CursorImager.rectTransform.pivot = style.Pivot;
        }

        /// <summary>
        /// 设置光标颜色
        /// </summary>
        /// <param tweenName="color"></param>
        public void mc_SetCursorColor(Color color)
        {
            if (CursorImager == null)
                return;
            CursorColor = color;
        }

        /// <summary>
        /// 设置鼠标图像尺寸
        /// </summary>
        /// <param tweenName="cursor"></param>
        public void mc_SetCursorSize(float size)
        {
            if (UseLerpCursorSize == "差值模式")
                CursorSize = size;
            else
            {
                if (Tween_CursorSize != null && Tween_CursorSize.IsActive)
                {
                    if (Tween_CursorSize.IsPlaying)
                    {
                        Tween_CursorSize.Rewind();
                    }
                }

                Tween_CursorSize = XTween.To(() => CursorSize, x => CursorSize = x, size, CursorSize_TweenDuration * XHud_Manager.Instance.DurationMultiply).SetEase(CursorSize_TweenEase).SetAutoKill(true);
            }
        }

        /// <summary>
        /// 重置初始样式
        /// </summary>
        public void mc_ResetCursorImage()
        {
            mc_ChangeStyle(OriginalMouseStyle.Name);
        }

        /// <summary>
        /// 切换样式
        /// </summary>
        public void mc_ChangeStyle(string stylename)
        {
            if (MouseStyles != null && MouseStyles.Count <= 0)
                return;
            for (int i = 0; i < MouseStyles.Count; i++)
            {
                if (MouseStyles[i].Name == stylename)
                {
                    CurrentCursorStyle = MouseStyles[i];
                    if (act_on_changed != null)
                        act_on_changed(Input.mousePosition, stylename, MouseStyles[i]);
                    break;
                }
            }
        }

        /// <summary>
        /// 光标透明度设置
        /// </summary>
        /// <param tweenName="opacity"></param>
        public void mc_CursorOpacitySet(float opacity)
        {
            if (Tween_CursorOpacity != null && Tween_CursorOpacity.IsActive)
            {
                if (Tween_CursorOpacity.IsPlaying)
                {
                    Tween_CursorOpacity.Rewind();
                }
            }

            Tween_CursorOpacity = XTween.To(() => CursorOpacity, x => CursorOpacity = x, opacity, CursorOpacity_TweenDuration).SetEase(CursorOpacity_TweenEase).SetAutoKill(true);
        }

        /// <summary>
        /// 光标透明度设置 - 快速
        /// </summary>
        /// <param tweenName="opacity"></param>
        public void mc_CursorOpacityFastSet(float opacity)
        {
            CursorOpacity = opacity;
        }

        /// <summary>
        /// 锁定光标
        /// </summary>
        /// <param tweenName="mode"></param>
        public void mc_LockCursor(CursorLockMode mode)
        {
            Cursor.lockState = mode;
        }
    }
}