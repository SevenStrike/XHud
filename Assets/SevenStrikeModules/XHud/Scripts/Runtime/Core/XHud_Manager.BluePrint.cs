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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
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
        public bool BluePrintMode;
        [Range(0, 1)]
        [Tooltip("蓝图视觉整体透明度")]
        /// <summary>
        /// 蓝图视觉透明度
        /// </summary>
        public float BluePrint_opacity = 1;
        [Tooltip("蓝图视觉总体淡化动画")]
        /// <summary>
        /// 蓝图视觉总体淡化动画
        /// </summary>
        private XTween_Interface BluePrint_opacityTweener;
        [Tooltip("蓝图视觉网格淡化动画")]
        /// <summary>
        /// 蓝图视觉网格淡化动画
        /// </summary>
        private XTween_Interface BluePrint_grid_opacityTweener;
        [Tooltip("蓝图视觉网格尺寸")]
        /// <summary>
        /// 蓝图视觉网格尺寸
        /// </summary>
        public float BluePrint_grid_size = 35;
        [Tooltip("蓝图视觉网格线宽度")]
        /// <summary>
        /// 蓝图视觉网格线宽度
        /// </summary>
        public float BluePrint_linewidth = 1;
        [Tooltip("蓝图视觉网格线颜色")]
        /// <summary>
        /// 蓝图视觉网格线颜色
        /// </summary>
        public Color BluePrint_grid_color = XHud_Utilitys.Color_From_RGBA(166, 166, 166, 26);
        [Tooltip("蓝图视觉背景颜色")]
        /// <summary>
        /// 蓝图视觉背景颜色
        /// </summary>
        public Color BluePrint_bg_color = XHud_Utilitys.Color_From_RGBA(55, 55, 55, 255);
        [Tooltip("蓝图视觉叠加颜色")]
        /// <summary>
        /// 蓝图视觉叠加颜色
        /// </summary>
        public Color BluePrint_bg_decal_color = XHud_Utilitys.Color_From_RGBA(0, 0, 0, 80);
        [Tooltip("蓝图视觉背景图像索引名称")]
        /// <summary>
        /// 蓝图视觉背景图像索引名称
        /// </summary>
        public string BluePrint_bg_name = "";
        [Range(0, 1)]
        [Tooltip("蓝图视觉网格线透明度")]
        /// <summary>
        /// 蓝图视觉网格线透明度
        /// </summary>
        public float BluePrint_grid_Opacity = 1;
        [Range(0, 1)]
        [Tooltip("蓝图视觉背景图像透明度")]
        /// <summary>
        /// 蓝图视觉背景图像透明度
        /// </summary>
        public float BluePrint_bg_mapOpacity = 1;
        [Tooltip("蓝图视觉背景图像平铺模式开关")]
        /// <summary>
        /// 蓝图视觉背景图像平铺模式开关
        /// </summary>
        public int BluePrint_bg_usetilling_index = 0;
        [Tooltip("蓝图视觉背景图像强制方形比例")]
        /// <summary>
        /// 蓝图视觉背景图像强制方形比例
        /// </summary>
        public int BluePrint_bg_usesquareratio_index = 0;
        [Range(0, 1)]
        [Tooltip("蓝图视觉背景透明度")]
        /// <summary>
        /// 蓝图视觉背景透明度
        /// </summary>
        public float BluePrint_bg_opacity = 1;
        [Tooltip("蓝图视觉背景图像平铺")]
        /// <summary>
        /// 蓝图视觉背景图像平铺
        /// </summary>
        public float BluePrint_bg_tilling = 1;
        [Tooltip("蓝图视觉大标题颜色")]
        /// <summary>
        /// 蓝图视觉大标题颜色
        /// </summary>
        public Color BluePrint_marktitle_color = Color.white;
        [Tooltip("蓝图视觉小标题颜色")]
        /// <summary>
        /// 蓝图视觉小标题颜色
        /// </summary>
        public Color BluePrint_marksubtitle_color = XHud_Dashboard.Theme_Primary;
        [Tooltip("蓝图视觉标题尺寸")]
        /// <summary>
        /// 蓝图视觉标题尺寸
        /// </summary>
        public float BluePrint_mark_size = 40;
        [Range(0, 1)]
        [Tooltip("蓝图视觉标题透明度")]
        /// <summary>
        /// 蓝图视觉标题透明度
        /// </summary>
        public float BluePrint_mark_opacity = 1;
        [Tooltip("蓝图视觉标题边距")]
        /// <summary>
        /// 蓝图视觉标题边距
        /// </summary>
        public Vector4 BluePrint_mark_margin = new Vector4(60, 60, 60, 60);
        [Tooltip("蓝图视觉标题间距")]
        /// <summary>
        /// 蓝图视觉标题间距
        /// </summary>
        public float BluePrint_mark_space = 1;
        [Tooltip("蓝图视觉透明度组件")]
        /// <summary>
        ///蓝图视觉透明度组件
        /// </summary>
        public CanvasGroup BluePrint_canvasgroup;
        [Tooltip("蓝图视觉根节点")]
        /// <summary>
        ///蓝图视觉根节点
        /// </summary>
        public RectTransform BluePrint_root;
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
        public string BluePrint_title_content = "XHUD CustomUI";
        [Tooltip("蓝图视觉小标题内容")]
        /// <summary>
        /// 蓝图视觉小标题内容
        /// </summary>
        public string BluePrint_subtitle_content = "powered by sevenstrike media";
        [Tooltip("蓝图视觉标题锚点")]
        /// <summary>
        /// 蓝图视觉标题锚点
        /// </summary>
        public IsolateVisualModeMarkAnchor BluePrint_MarkAnchors = IsolateVisualModeMarkAnchor.右下;
        [Tooltip("蓝图视觉背景动画速率")]
        /// <summary>
        /// 蓝图视觉背景动画速率
        /// </summary>
        public float BluePrint_AnimationDuration = 1;
        [Tooltip("蓝图视觉背景动画退场延迟")]
        /// <summary>
        /// 蓝图视觉背景动画退场延迟
        /// </summary>
        public float BluePrint_Bg_FadeAnimationDelay = 0;
        [Tooltip("蓝图视觉网格线动画速率")]
        /// <summary>
        /// 蓝图视觉网格线动画速率
        /// </summary>
        public float BluePrint_Grid_AnimationDuration = 1;
        [Tooltip("蓝图视觉背景淡化动画")]
        /// <summary>
        /// 蓝图视觉背景淡化动画
        /// </summary>
        private XTween_Interface BluePrint_BgFadeTweener;
        [Tooltip("蓝图视觉水印淡化动画")]
        /// <summary>
        /// 蓝图视觉水印淡化动画
        /// </summary>
        private XTween_Interface BluePrint_MarkFadeTweener;
        [Tooltip("蓝图网格状态")]
        /// <summary>
        /// 蓝图网格状态
        /// </summary>
        public bool BluePrint_Displayed = false;
        [Tooltip("蓝图视觉网格线动画起始时退场状态")]
        /// <summary>
        /// 蓝图视觉网格线动画起始时退场状态
        /// </summary>
        public bool BluePrint_OnStartHide = false;
        [Tooltip("蓝图视觉网格线动画缓动参数")]
        /// <summary>
        /// 蓝图视觉网格线动画缓动参数
        /// </summary>
        public EaseMode BluePrint_Grid_AnimationEase = EaseMode.OutQuart;
        [Tooltip("蓝图视觉背景动画缓动参数 - 入场")]
        /// <summary>
        /// 蓝图视觉背景动画缓动参数 - 入场
        /// </summary>
        public EaseMode BluePrint_Bg_AnimationEase_In = EaseMode.OutQuart;
        [Tooltip("蓝图视觉背景动画缓动参数 - 出场")]
        /// <summary>
        /// 蓝图视觉背景动画缓动参数 - 出场
        /// </summary>
        public EaseMode BluePrint_Bg_AnimationEase_Out = EaseMode.InQuart;
        [Tooltip("蓝图视觉网格长度百分比")]
        /// <summary>
        /// 蓝图视觉网格长度百分比
        /// </summary>
        public float BluePrint_Grid_LengthPercentage = 1;
        [Tooltip("蓝图视觉网格分级高差")]
        /// <summary>
        /// 蓝图视觉网格分级高差
        /// </summary>
        public float BluePrint_Grid_LevelHeight = 0;
        [Tooltip("网格动画结束百分比")]
        /// <summary>
        /// 网格动画结束百分比
        /// </summary>
        public float BluePrint_GridEnd = 1;
        [Tooltip("网格动画起始百分比")]
        /// <summary>
        /// 网格动画起始百分比
        /// </summary>
        public float BluePrint_GridStart;
        [Tooltip("蓝图视觉网格长度动画")]
        /// <summary>
        /// 蓝图模式网格长度动画
        /// </summary>
        private XTween_Interface BluePrint_GridLengthTweener;
        /// <summary>
        /// 控制网格生长动画是否执行
        /// true: 在蓝图入场时执行网格线从中心向外生长的动画效果
        /// false: 跳过网格生长动画，网格线直接以完整长度显示
        /// </summary>
        public bool Eft_Grid;
        /// <summary>
        /// 控制网格淡入淡出动画是否执行
        /// true: 在蓝图入场/退场时执行网格线的透明度渐变动画
        /// false: 跳过网格透明度动画，网格线直接显示或隐藏
        /// </summary>
        public bool Eft_GridFade;
        /// <summary>
        /// 控制背景层动画是否执行
        /// true: 在蓝图入场/退场时执行背景层的淡入淡出动画
        /// false: 跳过背景动画，背景直接显示或隐藏
        /// </summary>
        public bool Eft_Bg;
        /// <summary>
        /// 控制水印文字动画是否执行
        /// true: 在蓝图入场/退场时执行标题和副标题文字的淡入淡出动画
        /// false: 跳过水印动画，文字直接显示或隐藏
        /// </summary>
        public bool Eft_Mark;

        #region 调用
        /// <summary>
        /// 蓝图视觉透明度
        /// </summary>
        public void hm_BluePrint_OpacitySet(float opacity, float dur = 1, EaseMode ease = EaseMode.OutExpo, float delay = 0)
        {
            if (!BluePrintMode)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            hm_BluePrint_Fade(opacity, dur, ease, delay);

            if (opacity == 0)
            {
                if (Act_BluePrint_IsTransparency != null)
                    Act_BluePrint_IsTransparency();
            }
            else
            {
                if (Act_BluePrint_IsSolid != null)
                    Act_BluePrint_IsSolid();
            }

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图网格入场", HudMsgState.设置);
        }
        /// <summary>
        /// 蓝图入场
        /// </summary>
        /// <param name="eft_grid">影响网格</param>
        /// <param name="eft_bg">影响背景</param>
        /// <param name="eft_mark">影响水印</param>
        public void hm_BluePrint_In(bool eft_grid = true, bool eft_gridfade = true, bool eft_bg = true, bool eft_mark = true)
        {
            if (!BluePrintMode)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            if (BluePrint_Displayed)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图网格入场，无需重复入场！", HudMsgState.设置);
                return;
            }

            BluePrint_Displayed = true;

            hm_BluePrint_ResetTweeners();

            Eft_Grid = eft_grid;
            Eft_GridFade = eft_gridfade;
            Eft_Bg = eft_bg;
            Eft_Mark = eft_mark;

            if (Eft_GridFade)
                hm_BluePrint_Grid_Fade(1, BluePrint_AnimationDuration, BluePrint_Grid_AnimationEase);
            if (Eft_Grid)
                hm_BluePrint_Grid_Length(BluePrint_GridEnd, BluePrint_Grid_AnimationDuration, BluePrint_Grid_AnimationEase);
            if (Eft_Bg)
                hm_BluePrint_Bg_Fade(1, BluePrint_AnimationDuration, BluePrint_Bg_AnimationEase_In);
            if (Eft_Mark)
                hm_BluePrint_Mark_Fade(1, BluePrint_AnimationDuration, BluePrint_Bg_AnimationEase_In);

            if (Act_BluePrint_In != null)
                Act_BluePrint_In();

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图网格入场", HudMsgState.设置);
        }
        /// <summary>
        /// 蓝图入场
        /// </summary>
        /// <param name="eft_grid">影响网格</param>
        /// <param name="eft_bg">影响背景</param>
        /// <param name="eft_mark">影响水印</param>
        public void hm_BluePrint_In()
        {
            if (!BluePrintMode)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            if (BluePrint_Displayed)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图网格入场，无需重复入场！", HudMsgState.设置);
                return;
            }

            BluePrint_Displayed = true;

            hm_BluePrint_ResetTweeners();

            if (Eft_GridFade)
                hm_BluePrint_Grid_Fade(1, BluePrint_AnimationDuration, BluePrint_Grid_AnimationEase);
            if (Eft_Grid)
                hm_BluePrint_Grid_Length(BluePrint_GridEnd, BluePrint_Grid_AnimationDuration, BluePrint_Grid_AnimationEase);
            if (Eft_Bg)
                hm_BluePrint_Bg_Fade(1, BluePrint_AnimationDuration, BluePrint_Bg_AnimationEase_In);
            if (Eft_Mark)
                hm_BluePrint_Mark_Fade(1, BluePrint_AnimationDuration, BluePrint_Bg_AnimationEase_In);

            if (Act_BluePrint_In != null)
                Act_BluePrint_In();

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图网格入场", HudMsgState.设置);
        }
        /// <summary>
        /// 蓝图退场
        /// </summary>
        public void hm_BluePrint_Out()
        {
            if (!BluePrintMode)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            if (!BluePrint_Displayed)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图网格已经退场，无需重复退场！", HudMsgState.设置);
                return;
            }

            BluePrint_Displayed = false;

            hm_BluePrint_ResetTweeners();

            hm_BluePrint_Grid_Fade(0, BluePrint_AnimationDuration, BluePrint_Grid_AnimationEase);
            hm_BluePrint_Grid_Length(BluePrint_GridStart, BluePrint_AnimationDuration, BluePrint_Grid_AnimationEase);
            hm_BluePrint_Bg_Fade(-0.5f, BluePrint_AnimationDuration, BluePrint_Bg_AnimationEase_Out, BluePrint_Bg_FadeAnimationDelay);
            hm_BluePrint_Mark_Fade(-0.5f, BluePrint_AnimationDuration, BluePrint_Bg_AnimationEase_Out);

            if (Act_BluePrint_Out != null)
                Act_BluePrint_Out();

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图网格退场", HudMsgState.设置);
        }
        /// <summary>
        /// 蓝图动画清空重置
        /// </summary>
        public void hm_BluePrint_ResetTweeners()
        {
            if (!BluePrintMode)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            hm_BluePrint_StopTweener(BluePrint_BgFadeTweener);
            hm_BluePrint_StopTweener(BluePrint_grid_opacityTweener);
            hm_BluePrint_StopTweener(BluePrint_MarkFadeTweener);
            hm_BluePrint_StopTweener(BluePrint_GridLengthTweener);

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图网格清空并停止动画", HudMsgState.设置);
        }
        /// <summary>
        /// 停止动画器
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
            if (!BluePrintMode)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            BluePrint_Grid_LengthPercentage = 0;
            BluePrint_bg_opacity = 0;
            BluePrint_mark_opacity = 0;

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图网格快速到隐藏状态", HudMsgState.设置);
        }
        /// <summary>
        /// 将蓝图设为图层级的最底层
        /// </summary>
        public void hm_BluePrintRootFirstSibling()
        {
            if (BluePrint_root != null)
            {
                Transform trs = (Transform)BluePrint_root;
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
        private void hm_BluePrint_Fade(float val, float dur = 1, EaseMode ease = EaseMode.OutQuart, float delay = 0)
        {
            BluePrint_opacityTweener = XTween.To(() => BluePrint_opacity, opa => BluePrint_opacity = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay).OnComplete((d) =>
            {
                hm_BluePrint_StopTweener(BluePrint_opacityTweener);
            });
        }
        /// <summary>
        /// 蓝图背景淡化方式
        /// </summary>
        /// <param name="val">透明度_Alpha</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        private void hm_BluePrint_Bg_Fade(float val, float dur = 1, EaseMode ease = EaseMode.OutQuart, float delay = 0)
        {
            BluePrint_BgFadeTweener = XTween.To(() => BluePrint_bg_opacity, opa => BluePrint_bg_opacity = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay);
        }
        /// <summary>
        /// 水印淡化方式
        /// </summary>
        /// <param name="val">透明度_Alpha</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        private void hm_BluePrint_Mark_Fade(float val, float dur = 1, EaseMode ease = EaseMode.OutQuart, float delay = 0f)
        {
            BluePrint_MarkFadeTweener = XTween.To(() => BluePrint_mark_opacity, opa => BluePrint_mark_opacity = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay);
        }
        /// <summary>
        /// 网格生长方式
        /// </summary>
        /// <param name="val">透明度_Alpha</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        private void hm_BluePrint_Grid_Length(float val, float dur = 1, EaseMode ease = EaseMode.OutQuart, float delay = 0f)
        {
            BluePrint_GridLengthTweener = XTween.To(() => BluePrint_Grid_LengthPercentage, opa => BluePrint_Grid_LengthPercentage = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay).OnComplete((d) =>
            {
                hm_BluePrint_ResetTweeners();
            });
        }
        /// <summary>
        /// 网格淡化方式
        /// </summary>
        /// <param name="val">透明度_Alpha</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        private void hm_BluePrint_Grid_Fade(float val, float dur = 1, EaseMode ease = EaseMode.OutQuart, float delay = 0)
        {
            BluePrint_grid_opacityTweener = XTween.To(() => BluePrint_grid_Opacity, opa => BluePrint_grid_Opacity = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay).OnComplete((d) =>
            {
                hm_BluePrint_StopTweener(BluePrint_grid_opacityTweener);
            });
        }
        #endregion

        #region 操作预更新
        /// <summary>
        /// 蓝图创建网格线
        /// </summary>
        public void hm_BluePrint_Create(bool Hide = true, Material bluemat = null, TMP_FontAsset font_title = null, TMP_FontAsset font_subtitle = null)
        {
            if (!BluePrintMode)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            if (BluePrint_root != null)
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
            BluePrint_root = trs_root;

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
            img_bg.color = BluePrint_bg_color;
            img_bg.material = bluemat;
            BluePrint_mainbg = img_bg;
            #endregion

            #region 创建网格
            int h_count = (int)BluePrint_grid_size;
            float dis = ScreenRes.x / h_count;
            int v_count = (int)(ScreenRes.y / dis);

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
                trs_grid.sizeDelta = new Vector2(ScreenRes.x - (BluePrint_linewidth * 2), BluePrint_linewidth);

                trs_grid.anchoredPosition3D = new Vector3(BluePrint_linewidth, i * dis, 0);

                trs_grid.localEulerAngles = Vector3.zero;
                trs_grid.localScale = Vector3.one;
                Image img_grid = obj_isolateVisual_grid.AddComponent<Image>();
                img_grid.raycastTarget = false;
                img_grid.maskable = false;
                img_grid.color = BluePrint_grid_color;
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
                    trs_grid.sizeDelta = new Vector2(BluePrint_linewidth, ScreenRes.y);
                    trs_grid.anchoredPosition3D = new Vector3(i * dis, 0, 0);
                }
                else if (i == h_count)
                {
                    trs_grid.sizeDelta = new Vector2(BluePrint_linewidth, ScreenRes.y);
                    trs_grid.anchoredPosition3D = new Vector3(i * dis - (BluePrint_linewidth * 0.5f), 0, 0);
                }
                else
                {
                    trs_grid.sizeDelta = new Vector2(BluePrint_linewidth, ScreenRes.y - BluePrint_linewidth);
                    trs_grid.anchoredPosition3D = new Vector3(i * dis, BluePrint_linewidth, 0);
                }


                trs_grid.localEulerAngles = Vector3.zero;
                trs_grid.localScale = Vector3.one;
                Image img_grid = obj_isolateVisual_grid.AddComponent<Image>();
                img_grid.raycastTarget = false;
                img_grid.maskable = false;
                img_grid.color = BluePrint_grid_color;
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
            title.TextStyleInfo.gen_RayCastSet(false);
            title.TextStyleInfo.gen_Set_MaskableSet(false);
            title.TextStyleInfo.tmp_font = font_title;
            title.SyncGlobalFontSize = false;
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
            subtitle.TextStyleInfo.gen_RayCastSet(false);
            subtitle.TextStyleInfo.gen_Set_MaskableSet(false);
            subtitle.TextStyleInfo.tmp_font = font_subtitle;
            subtitle.SyncGlobalFontSize = false;
            BluePrint_subtitle_module = subtitle;

            #endregion

            BluePrint_bg_opacity = 1;
            BluePrint_Grid_LengthPercentage = BluePrint_GridEnd;
            BluePrint_mark_opacity = 1;

            hm_BluePrint_Update();

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "已创建蓝图网格", HudMsgState.设置);
        }
        /// <summary>
        /// 蓝图移除网格线
        /// </summary>
        public void hm_BluePrint_Remove(bool reset = true)
        {
            if (BluePrint_root == null)
                return;

            DestroyImmediate(BluePrint_root.gameObject, true);

            BluePrint_gridlines_H.Clear();
            BluePrint_gridlines_V.Clear();

            BluePrint_root = null;
            BluePrint_canvasgroup = null;
            BluePrint_mainbg = null;
            BluePrint_title_module = null;
            BluePrint_subtitle_module = null;
            BluePrint_Displayed = false;
            if (reset)
            {
                BluePrint_bg_opacity = 0;
                BluePrint_Grid_LengthPercentage = 0;
                BluePrint_mark_opacity = 0;
            }
            hm_BluePrint_ClearHidden();

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "已移除蓝图网格", HudMsgState.设置);
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
            if (!BluePrintMode)
                return;

            #region 整体透明度
            CanvasGroup img_cg = (CanvasGroup)BluePrint_canvasgroup;
            if (img_cg != null)
                img_cg.alpha = BluePrint_opacity;
            #endregion

            #region 主背景颜色
            if (BluePrint_mainbg != null)
            {
                float alp_bg = BluePrint_bg_color.a * BluePrint_bg_opacity;

                Color bgcolor = BluePrint_bg_color;
                bgcolor.a = alp_bg;

                BluePrint_mainbg.color = bgcolor;
                BluePrint_mainbg.material.SetVector("_Tiling", new Vector2(BluePrint_bg_tilling, BluePrint_bg_tilling));
                hm_BluePrint_SetBg_TilingMode(ConvertIntToBool(BluePrint_bg_usetilling_index));
                hm_BluePrint_SetBg_SquareRatio(ConvertIntToBool(BluePrint_bg_usesquareratio_index));
            }
            #endregion

            #region 背景叠加图片颜色
            if (BluePrint_mainbg != null)
            {
                BluePrint_mainbg.material.SetColor("_Color", BluePrint_bg_decal_color);
                BluePrint_mainbg.material.SetFloat("_BgMapOpacity", BluePrint_bg_mapOpacity);
            }
            #endregion

            #region 网格尺寸&颜色

            float alp = BluePrint_grid_color.a * BluePrint_grid_Opacity;

            Color gridcolor = BluePrint_grid_color;
            gridcolor.a = alp;

            if (BluePrint_gridlines_H.Count > 0)
            {
                for (int i = 0; i < BluePrint_gridlines_H.Count; i++)
                {
                    if (BluePrint_gridlines_H[i] == null)
                        continue;

                    BluePrint_gridlines_H[i].color = gridcolor;

                    float height = ScreenRes.y * BluePrint_Grid_LengthPercentage - (BluePrint_Grid_LevelHeight * i) * 0.55f;

                    BluePrint_gridlines_H[i].rectTransform.sizeDelta = new Vector2(BluePrint_linewidth, Mathf.Clamp(height, 0, ScreenRes.y));
                }
            }
            if (BluePrint_gridlines_V.Count > 0)
            {
                for (int i = 0; i < BluePrint_gridlines_V.Count; i++)
                {
                    if (BluePrint_gridlines_V[i] == null)
                        continue;

                    BluePrint_gridlines_V[i].color = gridcolor;

                    float height = ScreenRes.x * BluePrint_Grid_LengthPercentage - (BluePrint_Grid_LevelHeight * i);

                    BluePrint_gridlines_V[i].rectTransform.sizeDelta = new Vector2(Mathf.Clamp(height, 0, ScreenRes.x), BluePrint_linewidth);
                }
            }

            #endregion

            BluePrint_mark_opacity = Mathf.Clamp01(BluePrint_mark_opacity);

            #region 大标题
            if (BluePrint_title_module != null)
            {
                #region anchor
                switch (BluePrint_MarkAnchors)
                {
                    case IsolateVisualModeMarkAnchor.左上:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.顶部靠左);
                        break;
                    case IsolateVisualModeMarkAnchor.左下:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.底部靠左);
                        break;
                    case IsolateVisualModeMarkAnchor.右上:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.顶部靠右);
                        break;
                    case IsolateVisualModeMarkAnchor.右下:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.底部靠右);
                        break;
                    case IsolateVisualModeMarkAnchor.中心:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.中心);
                        break;
                    case IsolateVisualModeMarkAnchor.左:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.左);
                        break;
                    case IsolateVisualModeMarkAnchor.右:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.右);
                        break;
                    case IsolateVisualModeMarkAnchor.上:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.顶部);
                        break;
                    case IsolateVisualModeMarkAnchor.下:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.底部);
                        break;
                }
                #endregion

                #region margin
                Vector4 margin = BluePrint_mark_margin;

                if (BluePrint_MarkAnchors == IsolateVisualModeMarkAnchor.中心 || BluePrint_MarkAnchors == IsolateVisualModeMarkAnchor.左 || BluePrint_MarkAnchors == IsolateVisualModeMarkAnchor.右)
                    BluePrint_title_module.TextStyleInfo.tmp_Set_MarginSet(new Vector4(margin.x, margin.y, margin.z, margin.w * BluePrint_mark_space));
                else
                    BluePrint_title_module.TextStyleInfo.tmp_Set_MarginSet(BluePrint_mark_margin);
                #endregion

                #region content
                BluePrint_title_module.tmp_Set_Content(BluePrint_title_content);
                #endregion

                #region size
                BluePrint_title_module.TextStyleInfo.tmp_Set_FontSize(BluePrint_mark_size);
                #endregion

                #region color
                BluePrint_title_module.TextStyleInfo.tmp_Set_FontColor(BluePrint_marktitle_color);
                BluePrint_title_module.color = BluePrint_marktitle_color;
                #endregion

                #region opacity
                Color cc = BluePrint_title_module.color;
                cc.a *= BluePrint_mark_opacity;
                BluePrint_title_module.TextStyleInfo.tmp_Set_FontColor(cc);
                #endregion
            }
            #endregion

            #region 小标题
            if (BluePrint_subtitle_module != null)
            {
                #region anchor
                TmpContentAnchor x_anchor = TmpContentAnchor.中心;
                switch (BluePrint_MarkAnchors)
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
                BluePrint_subtitle_module.TextStyleInfo.tmp_Set_Alignment(x_anchor);
                #endregion

                #region margin
                Vector4 margin = BluePrint_mark_margin;
                Vector4 x_margin = new Vector4();
                switch (BluePrint_MarkAnchors)
                {
                    case IsolateVisualModeMarkAnchor.左上:
                        x_margin = new Vector4(margin.x, margin.y + 80 * BluePrint_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.左下:
                        x_margin = new Vector4(margin.x, margin.y, margin.z, margin.w + 80 * BluePrint_mark_space);
                        break;
                    case IsolateVisualModeMarkAnchor.右上:
                        x_margin = new Vector4(margin.x, margin.y + 80 * BluePrint_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.右下:
                        x_margin = new Vector4(margin.x, margin.y, margin.z, margin.w + 80 * BluePrint_mark_space);
                        break;
                    case IsolateVisualModeMarkAnchor.中心:
                        x_margin = new Vector4(margin.x, margin.y + 110 * BluePrint_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.左:
                        x_margin = new Vector4(margin.x, margin.y + 110 * BluePrint_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.右:
                        x_margin = new Vector4(margin.x, margin.y + 110 * BluePrint_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.上:
                        x_margin = new Vector4(margin.x, margin.y + 80 * BluePrint_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.下:
                        x_margin = new Vector4(margin.x, margin.y, margin.z, margin.w + 80 * BluePrint_mark_space);
                        break;
                }
                BluePrint_subtitle_module.TextStyleInfo.tmp_Set_MarginSet(x_margin);
                #endregion

                #region content
                BluePrint_subtitle_module.tmp_Set_Content(BluePrint_subtitle_content);
                #endregion

                #region size
                BluePrint_subtitle_module.TextStyleInfo.tmp_Set_FontSize(BluePrint_mark_size * 0.65f);
                #endregion

                #region color
                BluePrint_subtitle_module.TextStyleInfo.tmp_Set_FontColor(BluePrint_marksubtitle_color);
                BluePrint_subtitle_module.color = BluePrint_marksubtitle_color;
                #endregion

                #region opacity
                Color cc = BluePrint_subtitle_module.color;
                cc.a *= BluePrint_mark_opacity;
                BluePrint_subtitle_module.TextStyleInfo.tmp_Set_FontColor(cc);
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
            if (BluePrint_OnStartHide)
            {
                hm_BluePrint_Hide();
                BluePrint_Displayed = false;
            }
            else
            {
                BluePrint_Displayed = true;
            }
        }
    }
}