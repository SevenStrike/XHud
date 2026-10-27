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
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XTween;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public partial class XHud_Manager : MonoBehaviour
    {
        /// <summary>
        /// 独立可视化背景
        /// </summary>
        public bool bp_mode;
        [Range(0, 1)]
        [Tooltip("蓝图视觉整体透明度")]
        /// <summary>
        /// 蓝图视觉透明度
        /// </summary>
        public float bp_opacity = 1;
        [Tooltip("蓝图视觉总体淡化动画")]
        /// <summary>
        /// 蓝图视觉总体淡化动画
        /// </summary>
        private XTween_Interface bp_opacity_tweener;
        [Tooltip("蓝图视觉网格淡化动画")]
        /// <summary>
        /// 蓝图视觉网格淡化动画
        /// </summary>
        private XTween_Interface bp_grid_opacity_tweener;
        [Tooltip("蓝图视觉网格尺寸")]
        /// <summary>
        /// 蓝图视觉网格尺寸
        /// </summary>
        public float bp_grid_size = 35;
        [Tooltip("蓝图视觉网格线宽度")]
        /// <summary>
        /// 蓝图视觉网格线宽度
        /// </summary>
        public float bp_linewidth = 1;
        [Tooltip("蓝图视觉网格线颜色")]
        /// <summary>
        /// 蓝图视觉网格线颜色
        /// </summary>
        public Color bp_grid_color = XGUI_Utilitys.RGBA_To_Color(166, 166, 166, 26);
        [Tooltip("蓝图视觉背景颜色")]
        /// <summary>
        /// 蓝图视觉背景颜色
        /// </summary>
        public Color bp_bg_color = XGUI_Utilitys.RGBA_To_Color(55, 55, 55, 255);
        [Tooltip("蓝图视觉叠加颜色")]
        /// <summary>
        /// 蓝图视觉叠加颜色
        /// </summary>
        public Color bp_bg_decal_color = XGUI_Utilitys.RGBA_To_Color(0, 0, 0, 80);
        [Tooltip("蓝图视觉背景图像索引名称")]
        /// <summary>
        /// 蓝图视觉背景图像索引名称
        /// </summary>
        public string bp_bg_name = "";
        [Range(0, 1)]
        [Tooltip("蓝图视觉网格线透明度")]
        /// <summary>
        /// 蓝图视觉网格线透明度
        /// </summary>
        public float bp_grid_opacity = 1;
        [Range(0, 1)]
        [Tooltip("蓝图视觉背景图像透明度")]
        /// <summary>
        /// 蓝图视觉背景图像透明度
        /// </summary>
        public float bp_bg_map_opacity = 1;
        [Tooltip("蓝图视觉背景图像平铺模式开关")]
        /// <summary>
        /// 蓝图视觉背景图像平铺模式开关
        /// </summary>
        public int bp_bg_usetilling_index = 0;
        [Tooltip("蓝图视觉背景图像强制方形比例")]
        /// <summary>
        /// 蓝图视觉背景图像强制方形比例
        /// </summary>
        public int bp_bg_usesquareratio_index = 0;
        [Range(0, 1)]
        [Tooltip("蓝图视觉背景透明度")]
        /// <summary>
        /// 蓝图视觉背景透明度
        /// </summary>
        public float bp_bg_opacity = 1;
        [Tooltip("蓝图视觉背景图像平铺")]
        /// <summary>
        /// 蓝图视觉背景图像平铺
        /// </summary>
        public float bp_bg_tilling = 1;
        [Tooltip("蓝图视觉大标题颜色")]
        /// <summary>
        /// 蓝图视觉大标题颜色
        /// </summary>
        public Color bp_marktitle_color = Color.white;
        [Tooltip("蓝图视觉小标题颜色")]
        /// <summary>
        /// 蓝图视觉小标题颜色
        /// </summary>
        public Color bp_marksubtitle_color = XHud_Dashboard.themes_get_color(ThemeColorType.primary);
        [Tooltip("蓝图视觉标题尺寸")]
        /// <summary>
        /// 蓝图视觉标题尺寸
        /// </summary>
        public float bp_mark_size = 40;
        [Range(0, 1)]
        [Tooltip("蓝图视觉标题透明度")]
        /// <summary>
        /// 蓝图视觉标题透明度
        /// </summary>
        public float bp_mark_opacity = 1;
        [Tooltip("蓝图视觉标题边距")]
        /// <summary>
        /// 蓝图视觉标题边距
        /// </summary>
        public Vector4 bp_mark_margin = new Vector4(60, 60, 60, 60);
        [Tooltip("蓝图视觉标题间距")]
        /// <summary>
        /// 蓝图视觉标题间距
        /// </summary>
        public float bp_mark_space = 1;
        [Tooltip("蓝图视觉透明度组件")]
        /// <summary>
        ///蓝图视觉透明度组件
        /// </summary>
        public CanvasGroup BluePrint_canvasgroup;
        [Tooltip("蓝图视觉根节点")]
        /// <summary>
        ///蓝图视觉根节点
        /// </summary>
        public RectTransform bp_root;
        [Tooltip("蓝图视觉背景组件")]
        /// <summary>
        ///蓝图视觉背景组件
        /// </summary>
        public Image BluePrint_mainbg;
        [Tooltip("蓝图视觉网格线数组H")]
        /// <summary>
        /// 蓝图视觉网格线数组V
        /// </summary>
        public List<Image> BluePrint_gridlines_H;
        [Tooltip("蓝图视觉网格线数组V")]
        /// <summary>
        /// 蓝图视觉网格线数组V
        /// </summary>
        public List<Image> BluePrint_gridlines_V;
        [Tooltip("蓝图视觉大标题组件")]
        /// <summary>
        /// 蓝图视觉大标题组件
        /// </summary>
        public XHud_Module_TmpText BluePrint_title_module;
        [Tooltip("蓝图视觉小标题组件")]
        /// <summary>
        /// 蓝图视觉小标题组件
        /// </summary>
        public XHud_Module_TmpText BluePrint_subtitle_module;
        [Tooltip("蓝图视觉大标题内容")]
        /// <summary>
        /// 蓝图视觉大标题内容
        /// </summary>
        public string bp_title_content = "XHUD CustomUI";
        [Tooltip("蓝图视觉小标题内容")]
        /// <summary>
        /// 蓝图视觉小标题内容
        /// </summary>
        public string bp_subtitle_content = "powered by sevenstrike media";
        [Tooltip("蓝图视觉标题锚点")]
        /// <summary>
        /// 蓝图视觉标题锚点
        /// </summary>
        public IsolateVisualModeMarkAnchor bp_mark_anchors = IsolateVisualModeMarkAnchor.右下;
        [Tooltip("蓝图视觉背景动画速率")]
        /// <summary>
        /// 蓝图视觉背景动画速率
        /// </summary>
        public float bp_animation_duration = 1;
        [Tooltip("蓝图视觉背景动画退场延迟")]
        /// <summary>
        /// 蓝图视觉背景动画退场延迟
        /// </summary>
        public float bp_bg_fade_animation_delay = 0;
        [Tooltip("蓝图视觉网格线动画速率")]
        /// <summary>
        /// 蓝图视觉网格线动画速率
        /// </summary>
        public float bp_grid_animation_duration = 1;
        [Tooltip("蓝图视觉背景淡化动画")]
        /// <summary>
        /// 蓝图视觉背景淡化动画
        /// </summary>
        private XTween_Interface bp_bg_fade_tweener;
        [Tooltip("蓝图视觉水印淡化动画")]
        /// <summary>
        /// 蓝图视觉水印淡化动画
        /// </summary>
        private XTween_Interface bp_mark_fade_tweener;
        [Tooltip("蓝图网格状态")]
        /// <summary>
        /// 蓝图网格状态
        /// </summary>
        public bool bp_displayed = false;
        [Tooltip("蓝图视觉网格线动画起始时退场状态")]
        /// <summary>
        /// 蓝图视觉网格线动画起始时退场状态
        /// </summary>
        public bool bp_on_start_hide = false;
        [Tooltip("蓝图视觉网格线动画缓动参数")]
        /// <summary>
        /// 蓝图视觉网格线动画缓动参数
        /// </summary>
        public EaseMode bp_grid_animation_ease = EaseMode.OutQuart;
        [Tooltip("蓝图视觉背景动画缓动参数 - 入场")]
        /// <summary>
        /// 蓝图视觉背景动画缓动参数 - 入场
        /// </summary>
        public EaseMode bp_bg_animation_ease_in = EaseMode.OutQuart;
        [Tooltip("蓝图视觉背景动画缓动参数 - 出场")]
        /// <summary>
        /// 蓝图视觉背景动画缓动参数 - 出场
        /// </summary>
        public EaseMode bp_bg_animation_ease_out = EaseMode.InQuart;
        [Tooltip("蓝图视觉网格长度百分比")]
        /// <summary>
        /// 蓝图视觉网格长度百分比
        /// </summary>
        public float bp_grid_length_percentage = 1;
        [Tooltip("蓝图视觉网格分级高差")]
        /// <summary>
        /// 蓝图视觉网格分级高差
        /// </summary>
        public float bp_grid_level_height = 0;
        [Tooltip("网格动画结束百分比")]
        /// <summary>
        /// 网格动画结束百分比
        /// </summary>
        public float bp_grid_end = 1;
        [Tooltip("网格动画起始百分比")]
        /// <summary>
        /// 网格动画起始百分比
        /// </summary>
        public float bp_grid_start;
        [Tooltip("蓝图视觉网格长度动画")]
        /// <summary>
        /// 蓝图模式网格长度动画
        /// </summary>
        private XTween_Interface bp_grid_length_tweener;
        /// <summary>
        /// 控制网格生长动画是否执行
        /// true: 在蓝图入场时执行网格线从中心向外生长的动画效果
        /// false: 跳过网格生长动画，网格线直接以完整长度显示
        /// </summary>
        public bool bp_eft_grid;
        /// <summary>
        /// 控制网格淡入淡出动画是否执行
        /// true: 在蓝图入场/退场时执行网格线的透明度渐变动画
        /// false: 跳过网格透明度动画，网格线直接显示或隐藏
        /// </summary>
        public bool bp_eft_grid_fade;
        /// <summary>
        /// 控制背景层动画是否执行
        /// true: 在蓝图入场/退场时执行背景层的淡入淡出动画
        /// false: 跳过背景动画，背景直接显示或隐藏
        /// </summary>
        public bool bp_eft_bg;
        /// <summary>
        /// 控制水印文字动画是否执行
        /// true: 在蓝图入场/退场时执行标题和副标题文字的淡入淡出动画
        /// false: 跳过水印动画，文字直接显示或隐藏
        /// </summary>
        public bool bp_eft_mark;

        #region 调用
        /// <summary>
        /// 蓝图视觉透明度
        /// </summary>
        public XTween_Interface hm_BluePrint_OpacitySet(float opacity, float dur = 1, EaseMode ease = EaseMode.OutExpo, float delay = 0)
        {
            if (!bp_mode)
            {
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图模式已关闭！", XGUIMsgState.设置);
                return null;
            }

            XTween_Interface twn = hm_BluePrint_Fade(opacity, dur, ease, delay);

            if (opacity == 0)
            {
                if (act_blueprint_is_transparency != null)
                    act_blueprint_is_transparency();
            }
            else
            {
                if (act_blueprint_is_solid != null)
                    act_blueprint_is_solid();
            }

            if (use_debug)
                XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图网格入场", XGUIMsgState.设置);

            return twn;
        }
        /// <summary>
        /// 蓝图入场
        /// </summary>
        /// <param name="eft_grid">影响网格</param>
        /// <param name="eft_bg">影响背景</param>
        /// <param name="eft_mark">影响水印</param>
        public XTween_Interface hm_BluePrint_In(bool eft_grid = true, bool eft_gridfade = true, bool eft_bg = true, bool eft_mark = true)
        {
            if (!bp_mode)
            {
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图模式已关闭！", XGUIMsgState.设置);
                return null;
            }

            if (bp_displayed)
            {
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图网格入场，无需重复入场！", XGUIMsgState.设置);
                return null;
            }

            bp_displayed = true;

            hm_BluePrint_ResetTweeners();

            bp_eft_grid = eft_grid;
            bp_eft_grid_fade = eft_gridfade;
            bp_eft_bg = eft_bg;
            bp_eft_mark = eft_mark;

            XTween_Interface twn = null;

            if (bp_eft_grid_fade)
                twn = hm_BluePrint_Grid_Fade(1, bp_animation_duration, bp_grid_animation_ease);
            if (bp_eft_grid)
                twn = hm_BluePrint_Grid_Length(bp_grid_end, bp_grid_animation_duration, bp_grid_animation_ease);
            if (bp_eft_bg)
                twn = hm_BluePrint_Bg_Fade(1, bp_animation_duration, bp_bg_animation_ease_in);
            if (bp_eft_mark)
                twn = hm_BluePrint_Mark_Fade(1, bp_animation_duration, bp_bg_animation_ease_in);

            if (act_blueprint_in != null)
                act_blueprint_in();

            if (use_debug)
                XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图网格入场", XGUIMsgState.设置);

            return twn;
        }
        /// <summary>
        /// 蓝图入场
        /// </summary>
        /// <param name="eft_grid">影响网格</param>
        /// <param name="eft_bg">影响背景</param>
        /// <param name="eft_mark">影响水印</param>
        public XTween_Interface hm_BluePrint_In()
        {
            if (!bp_mode)
            {
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图模式已关闭！", XGUIMsgState.设置);
                return null;
            }

            if (bp_displayed)
            {
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图网格入场，无需重复入场！", XGUIMsgState.设置);
                return null;
            }

            bp_displayed = true;

            hm_BluePrint_ResetTweeners();

            XTween_Interface twn = null;

            if (bp_eft_grid_fade)
                twn = hm_BluePrint_Grid_Fade(1, bp_animation_duration, bp_grid_animation_ease);
            if (bp_eft_grid)
                twn = hm_BluePrint_Grid_Length(bp_grid_end, bp_grid_animation_duration, bp_grid_animation_ease);
            if (bp_eft_bg)
                twn = hm_BluePrint_Bg_Fade(1, bp_animation_duration, bp_bg_animation_ease_in);
            if (bp_eft_mark)
                twn = hm_BluePrint_Mark_Fade(1, bp_animation_duration, bp_bg_animation_ease_in);

            if (act_blueprint_in != null)
                act_blueprint_in();

            if (use_debug)
                XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图网格入场", XGUIMsgState.设置);

            return twn;
        }
        /// <summary>
        /// 蓝图退场
        /// </summary>
        public XTween_Interface hm_BluePrint_Out()
        {
            if (!bp_mode)
            {
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图模式已关闭！", XGUIMsgState.设置);
                return null;
            }

            if (!bp_displayed)
            {
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图网格已经退场，无需重复退场！", XGUIMsgState.设置);
                return null;
            }

            bp_displayed = false;

            hm_BluePrint_ResetTweeners();

            XTween_Interface twn = null;

            if (bp_eft_grid_fade)
                twn = hm_BluePrint_Grid_Fade(0, bp_animation_duration, bp_grid_animation_ease);
            if (bp_eft_grid)
                twn = hm_BluePrint_Grid_Length(bp_grid_start, bp_animation_duration, bp_grid_animation_ease);
            if (bp_eft_bg)
                twn = hm_BluePrint_Bg_Fade(-0.5f, bp_animation_duration, bp_bg_animation_ease_out, bp_bg_fade_animation_delay);
            if (bp_eft_mark)
                twn = hm_BluePrint_Mark_Fade(-0.5f, bp_animation_duration, bp_bg_animation_ease_out);

            if (act_blueprint_out != null)
                act_blueprint_out();

            if (use_debug)
                XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图网格退场", XGUIMsgState.设置);

            return twn;
        }
        /// <summary>
        /// 蓝图动画清空重置
        /// </summary>
        public void hm_BluePrint_ResetTweeners()
        {
            if (!bp_mode)
            {
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图模式已关闭！", XGUIMsgState.设置);
                return;
            }

            hm_BluePrint_StopTweener(bp_bg_fade_tweener);
            hm_BluePrint_StopTweener(bp_grid_opacity_tweener);
            hm_BluePrint_StopTweener(bp_mark_fade_tweener);
            hm_BluePrint_StopTweener(bp_grid_length_tweener);

            if (use_debug)
                XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图网格清空并停止动画", XGUIMsgState.设置);
        }
        /// <summary>
        /// 停止蓝图图元动画器
        /// </summary>
        /// <param name="twn"></param>
        private void hm_BluePrint_StopTweener(XTween_Interface twn)
        {
            twn.Kill();
            twn.Rewind();
        }
        /// <summary>
        /// 蓝图动画快速到起始隐藏状态
        /// </summary>
        public void hm_BluePrint_Hide()
        {
            if (!bp_mode)
            {
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图模式已关闭！", XGUIMsgState.设置);
                return;
            }

            bp_grid_length_percentage = 0;
            bp_bg_opacity = 0;
            bp_mark_opacity = 0;

            if (use_debug)
                XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图网格快速到隐藏状态", XGUIMsgState.设置);
        }
        /// <summary>
        /// 将蓝图设为图层级的最底层
        /// </summary>
        public void hm_BluePrintRootFirstSibling()
        {
            if (bp_root != null)
            {
                Transform trs = (Transform)bp_root;
                trs.SetAsFirstSibling();
            }
        }
        #endregion

        #region 动画
        /// <summary>
        /// 蓝图淡化方式
        /// </summary>
        /// <param name="val">透明度_Alpha</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        private XTween_Interface hm_BluePrint_Fade(float val, float dur = 1, EaseMode ease = EaseMode.OutQuart, float delay = 0)
        {
            bp_opacity_tweener = XTween.To(() => bp_opacity, opa => bp_opacity = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay).OnComplete((d) =>
            {
                hm_BluePrint_StopTweener(bp_opacity_tweener);
            });

            return bp_opacity_tweener;
        }
        /// <summary>
        /// 蓝图背景淡化方式
        /// </summary>
        /// <param name="val">透明度_Alpha</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        private XTween_Interface hm_BluePrint_Bg_Fade(float val, float dur = 1, EaseMode ease = EaseMode.OutQuart, float delay = 0)
        {
            bp_bg_fade_tweener = XTween.To(() => bp_bg_opacity, opa => bp_bg_opacity = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay);

            return bp_bg_fade_tweener;
        }
        /// <summary>
        /// 水印淡化方式
        /// </summary>
        /// <param name="val">透明度_Alpha</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        private XTween_Interface hm_BluePrint_Mark_Fade(float val, float dur = 1, EaseMode ease = EaseMode.OutQuart, float delay = 0f)
        {
            bp_mark_fade_tweener = XTween.To(() => bp_mark_opacity, opa => bp_mark_opacity = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay);

            return bp_mark_fade_tweener;
        }
        /// <summary>
        /// 网格生长方式
        /// </summary>
        /// <param name="val">透明度_Alpha</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        private XTween_Interface hm_BluePrint_Grid_Length(float val, float dur = 1, EaseMode ease = EaseMode.OutQuart, float delay = 0f)
        {
            bp_grid_length_tweener = XTween.To(() => bp_grid_length_percentage, opa => bp_grid_length_percentage = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay).OnComplete((d) =>
            {
                hm_BluePrint_ResetTweeners();
            });

            return bp_grid_length_tweener;
        }
        /// <summary>
        /// 网格淡化方式
        /// </summary>
        /// <param name="val">透明度_Alpha</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        private XTween_Interface hm_BluePrint_Grid_Fade(float val, float dur = 1, EaseMode ease = EaseMode.OutQuart, float delay = 0)
        {
            bp_grid_opacity_tweener = XTween.To(() => bp_grid_opacity, opa => bp_grid_opacity = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay).OnComplete((d) =>
            {
                hm_BluePrint_StopTweener(bp_grid_opacity_tweener);
            });

            return bp_grid_opacity_tweener;
        }
        #endregion

        #region 操作预更新
        /// <summary>
        /// 蓝图创建网格线
        /// </summary>
        public void hm_BluePrint_Create(bool Hide = true, Material bluemat = null, TMP_FontAsset font_title = null, TMP_FontAsset font_subtitle = null)
        {
            if (!bp_mode)
            {
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", "蓝图模式已关闭！", XGUIMsgState.设置);
                return;
            }

            if (bp_root != null)
                return;

            #region 创建根物体
            GameObject obj_isolateVisual_root = new GameObject();
            obj_isolateVisual_root.name = "IsolateVisual";
            obj_isolateVisual_root.layer = LayerMask.NameToLayer("XHud");
            RectTransform trs_root = obj_isolateVisual_root.AddComponent<RectTransform>();
            CanvasGroup cg = obj_isolateVisual_root.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
            cg.interactable = false;
            BluePrint_canvasgroup = cg;

            trs_root.SetParent(hm_ScreenElement_GetAnchored_RectTransform(XHudAnchor.底层));
            trs_root.anchorMin = new Vector2(0, 0);
            trs_root.anchorMax = new Vector2(1, 1);
            trs_root.pivot = new Vector2(0.5f, 0.5f);
            trs_root.sizeDelta = new Vector2(0, 0);
            trs_root.anchoredPosition3D = Vector3.zero;
            trs_root.localEulerAngles = Vector3.zero;
            trs_root.localScale = Vector3.one;
            bp_root = trs_root;

            #endregion

            #region 创建底色背景
            GameObject obj_isolateVisual_bg = new GameObject();
            obj_isolateVisual_bg.name = "bg";
            obj_isolateVisual_bg.layer = LayerMask.NameToLayer("XHud");
            RectTransform trs_bg = obj_isolateVisual_bg.AddComponent<RectTransform>();
            trs_bg.SetParent(trs_root);
            trs_bg.anchorMin = new Vector2(0, 0);
            trs_bg.anchorMax = new Vector2(1, 1);
            trs_bg.pivot = new Vector2(0.5f, 0.5f);
            trs_bg.sizeDelta = new Vector2(0, 0);
            trs_bg.anchoredPosition3D = Vector3.zero;
            trs_bg.localEulerAngles = Vector3.zero;
            trs_bg.localScale = Vector3.one;
            Image img_bg = obj_isolateVisual_bg.AddComponent<Image>();
            img_bg.raycastTarget = false;
            img_bg.maskable = false;
            img_bg.color = bp_bg_color;
            img_bg.material = bluemat;
            BluePrint_mainbg = img_bg;
            #endregion

            #region 创建网格
            int h_count = (int)bp_grid_size;
            float dis = cv_screen_res.x / h_count;
            int v_count = (int)(cv_screen_res.y / dis);

            #region 垂直平铺
            for (int i = 0; i < v_count + 1; i++)
            {
                GameObject obj_isolateVisual_grid = new GameObject();
                obj_isolateVisual_grid.name = "grid_v_" + i;
                obj_isolateVisual_grid.layer = LayerMask.NameToLayer("XHud");
                RectTransform trs_grid = obj_isolateVisual_grid.AddComponent<RectTransform>();
                trs_grid.SetParent(trs_root);

                if (i == 0)
                {
                    trs_grid.pivot = new Vector2(0f, 0f);
                }
                else if (i == v_count + 1)
                {
                    trs_grid.pivot = new Vector2(0f, 0f);
                }
                else
                {
                    trs_grid.pivot = new Vector2(0f, 0.5f);
                }
                trs_grid.anchorMin = new Vector2(0, 0);
                trs_grid.anchorMax = new Vector2(0, 0);
                trs_grid.sizeDelta = new Vector2(cv_screen_res.x - (bp_linewidth * 2), bp_linewidth);

                trs_grid.anchoredPosition3D = new Vector3(bp_linewidth, i * dis, 0);

                trs_grid.localEulerAngles = Vector3.zero;
                trs_grid.localScale = Vector3.one;
                Image img_grid = obj_isolateVisual_grid.AddComponent<Image>();
                img_grid.raycastTarget = false;
                img_grid.maskable = false;
                img_grid.color = bp_grid_color;
                BluePrint_gridlines_V.Add(img_grid);
            }
            #endregion

            #region 水平平铺
            for (int i = 0; i < h_count + 1; i++)
            {
                GameObject obj_isolateVisual_grid = new GameObject();
                obj_isolateVisual_grid.name = "grid_h_" + i;
                obj_isolateVisual_grid.layer = LayerMask.NameToLayer("XHud");
                RectTransform trs_grid = obj_isolateVisual_grid.AddComponent<RectTransform>();
                trs_grid.SetParent(trs_root);

                if (i == 0)
                {
                    trs_grid.pivot = new Vector2(0f, 0f);
                }
                else if (i == h_count + 1)
                {
                    trs_grid.pivot = new Vector2(1f, 0f);
                }
                else
                {
                    trs_grid.pivot = new Vector2(0.5f, 0f);
                }
                trs_grid.anchorMin = new Vector2(0, 0);
                trs_grid.anchorMax = new Vector2(0, 0);

                if (i == 0)
                {
                    trs_grid.sizeDelta = new Vector2(bp_linewidth, cv_screen_res.y);
                    trs_grid.anchoredPosition3D = new Vector3(i * dis, 0, 0);
                }
                else if (i == h_count)
                {
                    trs_grid.sizeDelta = new Vector2(bp_linewidth, cv_screen_res.y);
                    trs_grid.anchoredPosition3D = new Vector3(i * dis - (bp_linewidth * 0.5f), 0, 0);
                }
                else
                {
                    trs_grid.sizeDelta = new Vector2(bp_linewidth, cv_screen_res.y - bp_linewidth);
                    trs_grid.anchoredPosition3D = new Vector3(i * dis, bp_linewidth, 0);
                }


                trs_grid.localEulerAngles = Vector3.zero;
                trs_grid.localScale = Vector3.one;
                Image img_grid = obj_isolateVisual_grid.AddComponent<Image>();
                img_grid.raycastTarget = false;
                img_grid.maskable = false;
                img_grid.color = bp_grid_color;
                BluePrint_gridlines_H.Add(img_grid);
            }
            #endregion

            #endregion

            #region 创建标签

            GameObject obj_isolateVisual_mark_title = new GameObject();
            obj_isolateVisual_mark_title.name = "title";
            obj_isolateVisual_mark_title.layer = LayerMask.NameToLayer("XHud");
            RectTransform trs_mark_title = obj_isolateVisual_mark_title.AddComponent<RectTransform>();
            trs_mark_title.SetParent(trs_root);
            trs_mark_title.anchorMin = new Vector2(0, 0);
            trs_mark_title.anchorMax = new Vector2(1, 1);
            trs_mark_title.pivot = new Vector2(0.5f, 0.5f);
            trs_mark_title.sizeDelta = new Vector2(0, 0);
            trs_mark_title.anchoredPosition3D = Vector3.zero;
            trs_mark_title.localEulerAngles = Vector3.zero;
            trs_mark_title.localScale = Vector3.one;
            XHud_Module_TmpText title = obj_isolateVisual_mark_title.AddComponent<XHud_Module_TmpText>();
            title.text_style_info.gen_RayCastSet(false);
            title.text_style_info.gen_Set_MaskableSet(false);
            title.text_style_info.tmp_font = font_title;
            title.sync_global_font_size = false;
            BluePrint_title_module = title;

            GameObject obj_isolateVisual_mark_subtitle = new GameObject();
            obj_isolateVisual_mark_subtitle.name = "subtitle";
            obj_isolateVisual_mark_subtitle.layer = LayerMask.NameToLayer("XHud");
            RectTransform trs_mark_subtitle = obj_isolateVisual_mark_subtitle.AddComponent<RectTransform>();
            trs_mark_subtitle.SetParent(trs_root);
            trs_mark_subtitle.anchorMin = new Vector2(0, 0);
            trs_mark_subtitle.anchorMax = new Vector2(1, 1);
            trs_mark_subtitle.pivot = new Vector2(0.5f, 0.5f);
            trs_mark_subtitle.sizeDelta = new Vector2(0, 0);
            trs_mark_subtitle.anchoredPosition3D = Vector3.zero;
            trs_mark_subtitle.localEulerAngles = Vector3.zero;
            trs_mark_subtitle.localScale = Vector3.one;
            XHud_Module_TmpText subtitle = obj_isolateVisual_mark_subtitle.AddComponent<XHud_Module_TmpText>();
            subtitle.text_style_info.gen_RayCastSet(false);
            subtitle.text_style_info.gen_Set_MaskableSet(false);
            subtitle.text_style_info.tmp_font = font_subtitle;
            subtitle.sync_global_font_size = false;
            BluePrint_subtitle_module = subtitle;

            #endregion

            bp_bg_opacity = 1;
            bp_grid_length_percentage = bp_grid_end;
            bp_mark_opacity = 1;

            hm_BluePrint_Update();

            if (use_debug)
                XGUI_Utilitys.Console("XHud - 管理器通知", "已创建蓝图网格", XGUIMsgState.设置);
        }
        /// <summary>
        /// 蓝图移除网格线
        /// </summary>
        public void hm_BluePrint_Remove(bool reset = true)
        {
            if (bp_root == null)
                return;

            DestroyImmediate(bp_root.gameObject, true);

            BluePrint_gridlines_H.Clear();
            BluePrint_gridlines_V.Clear();

            bp_root = null;
            BluePrint_canvasgroup = null;
            BluePrint_mainbg = null;
            BluePrint_title_module = null;
            BluePrint_subtitle_module = null;
            bp_displayed = false;
            if (reset)
            {
                bp_bg_opacity = 0;
                bp_grid_length_percentage = 0;
                bp_mark_opacity = 0;
            }
            hm_BluePrint_ClearHidden();

            if (use_debug)
                XGUI_Utilitys.Console("XHud - 管理器通知", "已移除蓝图网格", XGUIMsgState.设置);
        }
        private void hm_BluePrint_ClearHidden()
        {
            RectTransform rect_back = hm_ScreenElement_GetAnchored_RectTransform(XHudAnchor.底层);
            if (rect_back.childCount > 0)
            {
                for (int i = 0; i < rect_back.childCount; i++)
                {
                    //if (rect_back.GetChild(i).gameObject.hideFlags == HideFlags.HideInHierarchy)
                    DestroyImmediate(rect_back.GetChild(i).gameObject, true);
                }
            }
        }
        /// <summary>
        /// 设置背景图像
        /// </summary>
        /// <param name="tex"></param>
        public void hm_BluePrint_SetBgTexture(Texture2D tex)
        {
            if (BluePrint_mainbg != null)
                BluePrint_mainbg.material.SetTexture("_BgMap", tex);
        }
        /// <summary>
        /// 设置背景图像平铺模式
        /// </summary>
        /// <param name="treeState"></param>
        public void hm_BluePrint_SetBg_TilingMode(bool state)
        {
            if (BluePrint_mainbg != null)
            {
                if (state)
                {
                    BluePrint_mainbg.material.SetFloat("_UseTiling", 1);
                }
                else
                {
                    BluePrint_mainbg.material.SetFloat("_UseTiling", 0);
                }
            }
        }
        /// <summary>
        /// 设置背景图像强制方形比例
        /// </summary>
        /// <param name="treeState"></param>
        public void hm_BluePrint_SetBg_SquareRatio(bool state)
        {
            if (BluePrint_mainbg != null)
            {
                if (state)
                {
                    BluePrint_mainbg.material.SetFloat("_UseRatio", 1);
                }
                else
                {
                    BluePrint_mainbg.material.SetFloat("_UseRatio", 0);
                }
            }
        }
        /// <summary>
        /// 解析Int到Bool
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool ConvertIntToBool(int val)
        {
            return val != 0;
        }
        /// <summary>
        /// 蓝图视觉模式更新
        /// 负责同步蓝图视觉模式的所有视觉元素，包括整体透明度、背景、网格线、标题水印等
        /// 
        /// 工作原理：
        /// 1. 检查蓝图视觉模式是否开启（BluePrintMode）
        /// 2. 更新 CanvasGroup 的整体透明度（控制整个蓝图视觉的可见性）
        /// 3. 更新主背景的颜色、透明度、贴图、平铺模式等
        /// 4. 更新网格线的颜色、透明度、生长动画（长度百分比）
        /// 5. 根据锚点位置更新大标题和小标题的位置、边距、内容、字体、颜色、透明度
        /// 
        /// 什么是蓝图视觉模式（Blueprint Mode）？
        /// - UI 布局设计时的辅助视觉系统
        /// - 提供网格线、背景、水印等视觉参考元素
        /// - 帮助设计师进行 UI 对齐、测量和布局
        /// - 支持动画效果（网格生长、淡入淡出）
        /// 
        /// 蓝图视觉的组成部分：
        /// 1. CanvasGroup：控制整个蓝图视觉的整体透明度
        /// 2. 主背景（MainBg）：底色 + 可选的背景贴图
        /// 3. 网格线（GridLines）：水平/垂直网格线，支持生长动画
        /// 4. 标题水印（Title/Subtitle）：大标题和小标题，用于项目标识
        /// 
        /// 使用场景：
        /// - UI 布局设计阶段，辅助元素对齐和测量
        /// - 团队协作时，展示项目标识和版本信息
        /// - 开发调试时，快速定位 UI 元素位置
        /// 
        /// 调用频率：
        /// - 每帧在 XHud_Manager.Update() 中调用
        /// - 开销适中，主要涉及网格线位置和尺寸的重新计算
        /// 
        /// 注意事项：
        /// - 蓝图视觉模式仅用于开发/调试，正式发布时应关闭
        /// - 网格线的数量和密度会影响性能，建议根据实际需求调整
        /// - 所有蓝图视觉元素都是动态创建和更新的
        /// </summary>
        public void hm_BluePrint_Update()
        {
            if (!bp_mode)
                return;

            #region 整体透明度
            CanvasGroup img_cg = (CanvasGroup)BluePrint_canvasgroup;
            if (img_cg != null)
                img_cg.alpha = bp_opacity;
            #endregion

            #region 主背景颜色
            if (BluePrint_mainbg != null)
            {
                float alp_bg = bp_bg_color.a * bp_bg_opacity;

                Color bgcolor = bp_bg_color;
                bgcolor.a = alp_bg;

                BluePrint_mainbg.color = bgcolor;
                BluePrint_mainbg.material.SetVector("_Tiling", new Vector2(bp_bg_tilling, bp_bg_tilling));
                hm_BluePrint_SetBg_TilingMode(ConvertIntToBool(bp_bg_usetilling_index));
                hm_BluePrint_SetBg_SquareRatio(ConvertIntToBool(bp_bg_usesquareratio_index));
            }
            #endregion

            #region 背景叠加图片颜色
            if (BluePrint_mainbg != null)
            {
                BluePrint_mainbg.material.SetColor("_Color", bp_bg_decal_color);
                BluePrint_mainbg.material.SetFloat("_BgMapOpacity", bp_bg_map_opacity);
            }
            #endregion

            #region 网格尺寸&颜色

            float alp = bp_grid_color.a * bp_grid_opacity;

            Color gridcolor = bp_grid_color;
            gridcolor.a = alp;

            if (BluePrint_gridlines_H.Count > 0)
            {
                for (int i = 0; i < BluePrint_gridlines_H.Count; i++)
                {
                    if (BluePrint_gridlines_H[i] == null)
                        continue;

                    BluePrint_gridlines_H[i].color = gridcolor;

                    float height = cv_screen_res.y * bp_grid_length_percentage - (bp_grid_level_height * i) * 0.55f;

                    BluePrint_gridlines_H[i].rectTransform.sizeDelta = new Vector2(bp_linewidth, Mathf.Clamp(height, 0, cv_screen_res.y));
                }
            }
            if (BluePrint_gridlines_V.Count > 0)
            {
                for (int i = 0; i < BluePrint_gridlines_V.Count; i++)
                {
                    if (BluePrint_gridlines_V[i] == null)
                        continue;

                    BluePrint_gridlines_V[i].color = gridcolor;

                    float height = cv_screen_res.x * bp_grid_length_percentage - (bp_grid_level_height * i);

                    BluePrint_gridlines_V[i].rectTransform.sizeDelta = new Vector2(Mathf.Clamp(height, 0, cv_screen_res.x), bp_linewidth);
                }
            }

            #endregion

            bp_mark_opacity = Mathf.Clamp01(bp_mark_opacity);

            #region 大标题
            if (BluePrint_title_module != null)
            {
                #region anchor
                switch (bp_mark_anchors)
                {
                    case IsolateVisualModeMarkAnchor.左上:
                        BluePrint_title_module.text_style_info.tmp_Set_Alignment(TmpContentAnchor.顶部靠左);
                        break;
                    case IsolateVisualModeMarkAnchor.左下:
                        BluePrint_title_module.text_style_info.tmp_Set_Alignment(TmpContentAnchor.底部靠左);
                        break;
                    case IsolateVisualModeMarkAnchor.右上:
                        BluePrint_title_module.text_style_info.tmp_Set_Alignment(TmpContentAnchor.顶部靠右);
                        break;
                    case IsolateVisualModeMarkAnchor.右下:
                        BluePrint_title_module.text_style_info.tmp_Set_Alignment(TmpContentAnchor.底部靠右);
                        break;
                    case IsolateVisualModeMarkAnchor.中心:
                        BluePrint_title_module.text_style_info.tmp_Set_Alignment(TmpContentAnchor.中心);
                        break;
                    case IsolateVisualModeMarkAnchor.左:
                        BluePrint_title_module.text_style_info.tmp_Set_Alignment(TmpContentAnchor.左);
                        break;
                    case IsolateVisualModeMarkAnchor.右:
                        BluePrint_title_module.text_style_info.tmp_Set_Alignment(TmpContentAnchor.右);
                        break;
                    case IsolateVisualModeMarkAnchor.上:
                        BluePrint_title_module.text_style_info.tmp_Set_Alignment(TmpContentAnchor.顶部);
                        break;
                    case IsolateVisualModeMarkAnchor.下:
                        BluePrint_title_module.text_style_info.tmp_Set_Alignment(TmpContentAnchor.底部);
                        break;
                }
                #endregion

                #region margin
                Vector4 margin = bp_mark_margin;

                if (bp_mark_anchors == IsolateVisualModeMarkAnchor.中心 || bp_mark_anchors == IsolateVisualModeMarkAnchor.左 || bp_mark_anchors == IsolateVisualModeMarkAnchor.右)
                    BluePrint_title_module.text_style_info.tmp_Set_MarginSet(new Vector4(margin.x, margin.y, margin.z, margin.w * bp_mark_space));
                else
                    BluePrint_title_module.text_style_info.tmp_Set_MarginSet(bp_mark_margin);
                #endregion

                #region content
                BluePrint_title_module.tmp_Set_Content(bp_title_content);
                #endregion

                #region size
                BluePrint_title_module.text_style_info.tmp_Set_FontSize(bp_mark_size);
                #endregion

                #region color
                BluePrint_title_module.text_style_info.tmp_Set_FontColor(bp_marktitle_color);
                BluePrint_title_module.color = bp_marktitle_color;
                #endregion

                #region opacity
                Color cc = BluePrint_title_module.color;
                cc.a *= bp_mark_opacity;
                BluePrint_title_module.text_style_info.tmp_Set_FontColor(cc);
                #endregion
            }
            #endregion

            #region 小标题
            if (BluePrint_subtitle_module != null)
            {
                #region anchor
                TmpContentAnchor x_anchor = TmpContentAnchor.中心;
                switch (bp_mark_anchors)
                {
                    case IsolateVisualModeMarkAnchor.左上:
                        x_anchor = TmpContentAnchor.顶部靠左;
                        break;
                    case IsolateVisualModeMarkAnchor.左下:
                        x_anchor = TmpContentAnchor.底部靠左;
                        break;
                    case IsolateVisualModeMarkAnchor.右上:
                        x_anchor = TmpContentAnchor.顶部靠右;
                        break;
                    case IsolateVisualModeMarkAnchor.右下:
                        x_anchor = TmpContentAnchor.底部靠右;
                        break;
                    case IsolateVisualModeMarkAnchor.中心:
                        x_anchor = TmpContentAnchor.中心;
                        break;
                    case IsolateVisualModeMarkAnchor.左:
                        x_anchor = TmpContentAnchor.左;
                        break;
                    case IsolateVisualModeMarkAnchor.右:
                        x_anchor = TmpContentAnchor.右;
                        break;
                    case IsolateVisualModeMarkAnchor.上:
                        x_anchor = TmpContentAnchor.顶部;
                        break;
                    case IsolateVisualModeMarkAnchor.下:
                        x_anchor = TmpContentAnchor.底部;
                        break;
                }
                BluePrint_subtitle_module.text_style_info.tmp_Set_Alignment(x_anchor);
                #endregion

                #region margin
                Vector4 margin = bp_mark_margin;
                Vector4 x_margin = new Vector4();
                switch (bp_mark_anchors)
                {
                    case IsolateVisualModeMarkAnchor.左上:
                        x_margin = new Vector4(margin.x, margin.y + 80 * bp_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.左下:
                        x_margin = new Vector4(margin.x, margin.y, margin.z, margin.w + 80 * bp_mark_space);
                        break;
                    case IsolateVisualModeMarkAnchor.右上:
                        x_margin = new Vector4(margin.x, margin.y + 80 * bp_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.右下:
                        x_margin = new Vector4(margin.x, margin.y, margin.z, margin.w + 80 * bp_mark_space);
                        break;
                    case IsolateVisualModeMarkAnchor.中心:
                        x_margin = new Vector4(margin.x, margin.y + 110 * bp_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.左:
                        x_margin = new Vector4(margin.x, margin.y + 110 * bp_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.右:
                        x_margin = new Vector4(margin.x, margin.y + 110 * bp_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.上:
                        x_margin = new Vector4(margin.x, margin.y + 80 * bp_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.下:
                        x_margin = new Vector4(margin.x, margin.y, margin.z, margin.w + 80 * bp_mark_space);
                        break;
                }
                BluePrint_subtitle_module.text_style_info.tmp_Set_MarginSet(x_margin);
                #endregion

                #region content
                BluePrint_subtitle_module.tmp_Set_Content(bp_subtitle_content);
                #endregion

                #region size
                BluePrint_subtitle_module.text_style_info.tmp_Set_FontSize(bp_mark_size * 0.65f);
                #endregion

                #region color
                BluePrint_subtitle_module.text_style_info.tmp_Set_FontColor(bp_marksubtitle_color);
                BluePrint_subtitle_module.color = bp_marksubtitle_color;
                #endregion

                #region opacity
                Color cc = BluePrint_subtitle_module.color;
                cc.a *= bp_mark_opacity;
                BluePrint_subtitle_module.text_style_info.tmp_Set_FontColor(cc);
                #endregion
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// 蓝图视觉模式初始化
        /// 根据 BluePrint_OnStartHide 配置决定蓝图视觉在游戏启动时的显示状态
        /// 
        /// 蓝图视觉模式（Blueprint Mode）：
        /// 一个用于 UI 布局设计的辅助视觉系统，提供：
        /// - 网格线：辅助对齐和测量
        /// - 背景色：视觉参考
        /// - 标题/水印：版本或项目标识
        /// 
        /// 使用场景：
        /// - 开发阶段：蓝图视觉模式开启，方便 UI 布局调试
        /// - 正式发布：蓝图视觉模式关闭，不影响最终产品
        /// - 编辑器预览：可视化地调整 UI 元素位置
        /// </summary>
        public void hm_BluePrint_InitializeMode()
        {
            if (bp_on_start_hide)
            {
                hm_BluePrint_Hide();
                bp_displayed = false;
            }
            else
            {
                bp_displayed = true;
            }
        }
    }
}