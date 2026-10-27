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
        public bool use_custom_cursor;
        /// <summary>
        /// 鼠标锚点
        /// </summary>
        public RectTransform cursor_rect;
        /// <summary>
        /// 鼠标透明度组件
        /// </summary>
        public CanvasGroup cursor_canvas_group;
        /// <summary>
        /// 鼠标图像组件
        /// </summary>
        public Image cursor_imager;

        [Range(0, 1)]
        /// <summary>
        /// 鼠标透明度
        /// </summary>
        public float cursor_opacity = 1;

        Vector2 calaculate_pos;

        public MouseType mouse_click_type = MouseType.左键;

        /// <summary>
        /// 当前正在使用的样式
        /// </summary>
        public MouseStyles current_cursor_style;
        /// <summary>
        /// 用于重置的样式
        /// </summary>
        private MouseStyles OriginalMouseStyle;

        /// <summary>
        /// 光标尺寸
        /// </summary>
        public float cursor_size = 20;

        /// <summary>
        /// 光标颜色
        /// </summary>
        public Color cursor_color = Color.white;
        public float cursor_color_smooth = 1;

        /// <summary>
        /// 鼠标样式列表
        /// </summary>
        public List<MouseStyles> mouse_styles;

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

        public string index_mouse_type = "左键";

        public float cursor_imager_size_smooth = 1;

        public string use_lerp_cursor_size = "差值模式";

        private XTween_Interface tween_cursor_size;
        public float cursor_size_tween_duration = 1;
        public EaseMode cursor_size_tween_ease;

        private XTween_Interface tween_cursor_opacity;
        public float cursor_opacity_tween_duration = 1;
        public EaseMode cursor_opacity_tween_ease;

        #region 折叠
        public bool
            fold_based = true,
            fold_options = true,
            fold_styles = true,
            fold_params = true;

        public void GroupFold(bool state)
        {
            fold_based = state;
            fold_options = state;
            fold_styles = state;
            fold_params = state;
        }

        public void GroupFold(bool based = true, bool options = true, bool styles = true, bool param = true)
        {
            fold_based = based;
            fold_options = options;
            fold_styles = styles;
            fold_params = param;
        }
        #endregion

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
                cursor_imager = img;
                cursor_rect = rect;
            }
            else
            {
                cursor_imager = transform.GetChild(0).GetComponent<Image>();
            }

            cursor_rect = GetComponent<RectTransform>();

            cursor_canvas_group = GetComponent<CanvasGroup>();
            if (cursor_canvas_group == null)
            {
                cursor_canvas_group = gameObject.AddComponent<CanvasGroup>();
            }

            if (current_cursor_style != null)
            {
                OriginalMouseStyle = current_cursor_style;
            }

            cursor_imager.rectTransform.sizeDelta = Vector2.one * cursor_size;
            cursor_imager.color = cursor_color;
        }

        void Update()
        {
            mc_CustomCursorUpdate();
        }

        /// <summary>
        /// 是否开启自定义光标
        /// </summary>
        /// <param name="treeState">开启状态</param>
        public void mc_SetCustomCursorEnabled(bool state)
        {
            use_custom_cursor = state;
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
            switch (mouse_click_type)
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
            if (!use_custom_cursor)
            {
                Cursor.visible = true;
                if (cursor_imager != null)
                    cursor_imager.enabled = false;
                return;
            }
            else
            {
                Cursor.visible = false;
                if (cursor_imager != null)
                    cursor_imager.enabled = true;
            }

            ///---同步样式到鼠标位置
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(XHud_Manager.instance.cv_screen.transform as RectTransform, Input.mousePosition, XHud_Manager.instance.camera_ui, out calaculate_pos))
            {

                cursor_rect.anchoredPosition = calaculate_pos;
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

            if (cursor_canvas_group != null)
            {
                ///---鼠标样式透明度
                cursor_canvas_group.alpha = cursor_opacity;
            }

            if (use_lerp_cursor_size == "差值模式")
                cursor_imager.rectTransform.sizeDelta = Vector2.Lerp(cursor_imager.rectTransform.sizeDelta, Vector2.one * cursor_size, Time.deltaTime * cursor_imager_size_smooth);
            else
                cursor_imager.rectTransform.sizeDelta = Vector2.one * cursor_size;

            mc_SetCursorImager(current_cursor_style);

            cursor_imager.color = Color.Lerp(cursor_imager.color, cursor_color, Time.deltaTime * cursor_color_smooth);
        }

        /// <summary>
        /// 设置鼠标图像
        /// </summary>
        /// <param name="cursor"></param>
        public void mc_SetCursorImager(MouseStyles style)
        {
            if (cursor_imager == null)
                return;
            if (style == null)
                return;
            cursor_imager.sprite = style.Sprite;
            cursor_imager.rectTransform.pivot = style.Pivot;
        }

        /// <summary>
        /// 设置光标颜色
        /// </summary>
        /// <param name="color"></param>
        public void mc_SetCursorColor(Color color)
        {
            if (cursor_imager == null)
                return;
            cursor_color = color;
        }

        /// <summary>
        /// 设置鼠标图像尺寸
        /// </summary>
        /// <param name="cursor"></param>
        public XTween_Interface mc_SetCursorSize(float size)
        {
            if (use_lerp_cursor_size == "差值模式")
            {
                cursor_size = size;
            }
            else
            {
                if (tween_cursor_size != null && tween_cursor_size.IsActive)
                {
                    if (tween_cursor_size.IsPlaying)
                    {
                        tween_cursor_size.Rewind();
                    }
                }

                tween_cursor_size = XTween.To(() => cursor_size, x => cursor_size = x, size, cursor_size_tween_duration * XHud_Manager.instance.tween_global_duration).SetEase(cursor_size_tween_ease).SetAutoKill(true);
            }

            return tween_cursor_size;
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
            if (mouse_styles != null && mouse_styles.Count <= 0)
                return;
            for (int i = 0; i < mouse_styles.Count; i++)
            {
                if (mouse_styles[i].Name == stylename)
                {
                    current_cursor_style = mouse_styles[i];
                    if (act_on_changed != null)
                        act_on_changed(Input.mousePosition, stylename, mouse_styles[i]);
                    break;
                }
            }
        }

        /// <summary>
        /// 光标透明度设置
        /// </summary>
        /// <param name="opacity"></param>
        public XTween_Interface mc_CursorOpacitySet(float opacity)
        {
            if (tween_cursor_opacity != null && tween_cursor_opacity.IsActive)
            {
                if (tween_cursor_opacity.IsPlaying)
                {
                    tween_cursor_opacity.Rewind();
                }
            }

            tween_cursor_opacity = XTween.To(() => cursor_opacity, x => cursor_opacity = x, opacity, cursor_opacity_tween_duration).SetEase(cursor_opacity_tween_ease).SetAutoKill(true);

            return tween_cursor_opacity;
        }

        /// <summary>
        /// 光标透明度设置 - 快速
        /// </summary>
        /// <param name="opacity"></param>
        public void mc_CursorOpacityFastSet(float opacity)
        {
            cursor_opacity = opacity;
        }

        /// <summary>
        /// 锁定光标
        /// </summary>
        /// <param name="mode"></param>
        public void mc_LockCursor(CursorLockMode mode)
        {
            Cursor.lockState = mode;
        }
    }
}