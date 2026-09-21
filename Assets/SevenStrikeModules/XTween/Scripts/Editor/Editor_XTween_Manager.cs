/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XTween - Unity 高性能动画架构插件
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
namespace SevenStrikeModules.XTween.Editor
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public struct xTweenDatas
    {
        /// <summary>
        /// 数量
        /// </summary>
        public int Count;
        /// <summary>
        /// 百分比
        /// </summary>
        public float Percentage;
        /// <summary>
        /// 动画百分比
        /// </summary>
        public float Percentage_Smooth;

        /// <summary>
        /// 计算数量与百分比
        /// </summary>
        /// <param name="type"></param>
        public void CalculateData(int count, int total)
        {
            if (total <= 0) // 避免除零错误
            {
                Count = 0;
                Percentage = 0f;
                Percentage_Smooth = 0f;
                return;
            }

            Count = count;
            Percentage = (float)count / total;

            Percentage_Smooth = Mathf.Lerp(Percentage_Smooth, Percentage, Time.unscaledDeltaTime * 3);
        }
    }

    [CustomEditor(typeof(XTween_Manager))]
    public class Editor_XTween_Manager : Editor
    {
        private XTween_Manager BaseScript;

        /// <summary>
        /// 液晶背景
        /// </summary>
        private Texture2D
            icon_preview_r,
            icon_preview_p,
            icon_rewind_r,
            icon_rewind_p,
            icon_recycle_r,
            icon_recycle_p,
            liquid_bg_statistic,
            liquid_bg_statistic_pure,
            liquid_bg_statistic_scan,
            liquid_bg_list,
            liquid_bg_list_pure,
            liquid_bg_list_scan,
            liquid_plug_statistic,
            liquid_plug_list,
            liquid_metal_grid,
            liquid_dirty,
            ListIndex,
            logo_bg,
            logo;

        private Texture2D EasePicBg;

        private int Count_Tweens;

        private xTweenDatas TwnData_Playing;
        private xTweenDatas TwnData_Pausing;
        private xTweenDatas TwnData_Comleted;
        private xTweenDatas TwnData_HasLoop;

        private float currentWidth;

        private Vector2 TweenList_scrollPosition;
        //private List<XTween_Interface> TweenList_items = new List<XTween_Interface>();
        private List<float> TweenList_cachedItemHeights;
        private bool TweenList_heightsDirty = true;

        private void OnEnable()
        {
            BaseScript = (XTween_Manager)target;

            #region 图标获取
            logo_bg = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_manager/logo_bg");
            logo = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_manager/logo");
            icon_preview_r = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_manager/icon_preview_r");
            icon_preview_p = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_manager/icon_preview_p");
            icon_rewind_r = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_manager/icon_rewind_r");
            icon_rewind_p = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_manager/icon_rewind_p");
            icon_recycle_r = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_manager/icon_recycle_r");
            icon_recycle_p = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_manager/icon_recycle_p");
            liquid_bg_statistic_pure = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/manager/liquid_bg_statistic_pure");
            liquid_bg_statistic_scan = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/manager/liquid_bg_statistic_scan");
            liquid_bg_list_pure = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/manager/liquid_bg_list_pure");
            liquid_bg_list_scan = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/manager/liquid_bg_list_scan");
            liquid_plug_statistic = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/plug/liquid_plug_lime");
            liquid_plug_list = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/plug/liquid_plug_yellow");
            liquid_dirty = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/dirty/liquid_dirty");
            liquid_metal_grid = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/liquid_metal_grid");
            ListIndex = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_manager/icon_listindex");
            #endregion

            // 获取缓动曲线的背景
            EasePicBg = Get_Ease_Bg_Texture();

            // 获取所有缓动类型的名称 & 预存
            string[] names = Enum.GetNames(typeof(EaseMode));
            BaseScript.EasePics = new Texture2D[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                BaseScript.EasePics[i] = Get_Ease_Type_Texture(names[i]);
            }

            #region 测试数据
            /*
            string[] ease_names = Enum.GetNames(typeof(EaseMode));

            // 添加一些滚动列表测试数据
            for (int i = 0; i < 100; i++)
            {
                EaseMode ease = (EaseMode)Enum.Parse(typeof(EaseMode), ease_names[Random.Range(0, 31)]);

                EaseInfo easeInfo = new EaseInfo() { ease_arg = ease, ease_curve = AnimationCurve.Linear(0, 0, 1, 1), use_curve = Random.Range(0, 2) == 0 };

                XTween_Specialized_Vector3 twn = new XTween_Specialized_Vector3();
                twn.SetDuration(Random.Range(0.5f, 2))
                   .SetDelay(Random.Range(0.5f, 2))
                   .SetAutokill(Random.Range(0, 2) == 0)
                   .SetEndValue(new Vector3(Random.Range(0, 100f), Random.Range(0, 100f), Random.Range(0, 100f)))
                   .SetLoop(Random.Range(0, 3), XTween_LoopType.Restart)
                   .SetRelative(Random.Range(0, 2) == 0)
                   .SetLoopingDelay(Random.Range(0f, 2f))
                   .SetEase(easeInfo);
                TweenList_items.Add(twn);
            }
            */
            #endregion
        }

        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            currentWidth = XGUI.GetCurrentWindowWidth();

            #region Logo 背景
            float w_logo_bg = logo_bg.width;
            float h_logo_bg = logo_bg.height;
            float x_logo_bg = currentWidth / 2 - logo_bg.width / 2 + 5;
            float y_logo_bg = 30;
            Rect rect_logo_bg = new Rect(x_logo_bg, y_logo_bg, w_logo_bg, h_logo_bg);

            XGUI.gui_icon(
                rect: rect_logo_bg,
                icon: logo_bg,
                color: Color.white * 0.7f);
            #endregion

            #region Logo 主体
            float w_logo_main = logo.width;
            float h_logo_main = logo.height;
            float x_main = x_logo_bg + (w_logo_bg / 2) - (w_logo_main / 2);
            float y_main = y_logo_bg + (h_logo_bg / 2) - (h_logo_main / 2);
            Rect rect_logo_main = new Rect(x_main, y_main, w_logo_main, h_logo_main);

            XGUI.gui_icon(
                rect: rect_logo_main,
                icon: logo);
            #endregion

            XGUI.layout_space(160);

            #region 动画批量操作
            XGUI.layout_group_start(
             type: XGUIContainerType.Horizontal,
             bg_fill: XGUIFilled.缺口纯色边框,
             bg_color: XGUIColor.亮白,
             bg_color_gui: XTween_Dashboard.Theme_Group,
             title: "动画批量操作",
             title_size: XGUIFontSize.M,
             title_text_color: XTween_Dashboard.Theme_Primary,
             title_clipping: TextClipping.Clip,
             padding: new RectOffset(20, 20, 20, 20));

            #region 全部播放
            if (XGUI.layout_button(
                tooltip: "全部播放",
                tex_release: icon_preview_r,
                tex_press: icon_preview_p,
                tex_gui_color: Color.white,
                width: 15,
                height: 15))
            {
                if (Application.isPlaying)
                    BaseScript.Play_All();
                else
                {
                    XGUI_Utilitys.Console("XTween动画管理器消息", "应用未运行，只有在应用运行时期才可以使用此功能！", XGUIMsgState.警告);
                }
                return;
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 全部倒退
            if (XGUI.layout_button(
                tooltip: "全部倒退",
                tex_release: icon_rewind_r,
                tex_press: icon_rewind_p,
                tex_gui_color: Color.white,
                width: 15,
                height: 15))
            {
                if (Application.isPlaying)
                    BaseScript.Rewind_All();
                else
                {
                    XGUI_Utilitys.Console("XTween动画管理器消息", "应用未运行，只有在引用运行时期才可以使用此功能！", XGUIMsgState.警告);
                }
                return;
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 全部回收
            if (XGUI.layout_button(
                tooltip: "全部回收",
                tex_release: icon_recycle_r,
                tex_press: icon_recycle_p,
                tex_gui_color: Color.white,
                width: 15,
                height: 15))
            {
                if (Application.isPlaying)
                    XTween_Pool.ForceRecycleAll();
                else
                {
                    XGUI_Utilitys.Console("XTween动画管理器消息", "应用未运行，只有在引用运行时期才可以使用此功能！", XGUIMsgState.警告);
                }
                return;
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 判断窗口宽度阈值
            bool panel_extra_expand = XGUI.CurrentWindowWidthThreshold("<", 215);
            bool panel_expand = XGUI.CurrentWindowWidthThreshold("<", 150);
            #endregion

            #region 统计数据
            // 获取液晶屏的根锚点
            Rect liquid_root = XGUI.GetControlRect(false, 0);
            liquid_root.Set(liquid_root.x, liquid_root.y, liquid_root.width, 0);

            BaseScript.fold_statistic = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XTween_Dashboard.Theme_Group,
                title: "统计",
                title_size: XGUIFontSize.M,
                title_bg_fill: XGUIFilled.无,
                title_bg_color: XGUIColor.无,
                title_bg_color_gui: Color.white,
                title_text_color: XTween_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(
                    XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode ? 0 : 20,
                    XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode ? 0 : 20,
                    XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode ? 10 : 10,
                    (XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode ? (BaseScript.fold_statistic ? 20 : 195) : (BaseScript.fold_statistic ? 20 : 245))),
                foldout: BaseScript.fold_statistic);

            Color liquid_bg_color = Color.white;

            if (!BaseScript.fold_statistic)
            {
                #region 判断是否正在运行
                if (Application.isPlaying)
                {
                    #region 刷新数据            
                    Count_Tweens = BaseScript.Get_TweenCount_ActiveTween();
                    TwnData_Playing.CalculateData(BaseScript.Get_TweenCount_Playing(), Count_Tweens);
                    TwnData_Pausing.CalculateData(BaseScript.Get_TweenCount_Paused(), Count_Tweens);
                    TwnData_Comleted.CalculateData(BaseScript.Get_TweenCount_Completed(), Count_Tweens);
                    TwnData_HasLoop.CalculateData(BaseScript.Get_TweenCount_HasLoop(), Count_Tweens);
                    Repaint();
                    #endregion

                    // 液晶背景颜色
                    liquid_bg_color = XTween_Dashboard.LiquidColor_Playing;

                    // 判断是否是扫描线风格或是纯净风格
                    if (XTween_Dashboard.XTweenConfig.Datas.LiquidScanStyle)
                        liquid_bg_statistic = liquid_bg_statistic_scan;
                    else
                        liquid_bg_statistic = liquid_bg_statistic_pure;
                }
                else
                {
                    // 液晶背景颜色
                    liquid_bg_color = XTween_Dashboard.LiquidColor_Idle;

                    // 判断是否是扫描线风格或是纯净风格
                    if (XTween_Dashboard.XTweenConfig.Datas.LiquidScanStyle)
                        liquid_bg_statistic = liquid_bg_statistic_scan;
                    else
                        liquid_bg_statistic = liquid_bg_statistic_pure;
                }
                #endregion

                Rect rect_liquid = liquid_root;

                if (!XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode)
                {
                    // 测试区域
                    //XGUI.gui_box(new Rect(rect_liquid.x, rect_liquid.y, rect_liquid.width, 100), Color.red);

                    #region 液晶 - 背景
                    float liquid_bg_x = rect_liquid.x + 10;
                    float liquid_bg_y = rect_liquid.y + 38;
                    float liquid_bg_w = liquid_root.width - 20;
                    float liquid_bg_h = liquid_bg_statistic.height;

                    rect_liquid.Set(liquid_bg_x, liquid_bg_y, liquid_bg_w, liquid_bg_h);
                    XGUI.gui_box(
                        rect: rect_liquid,
                        bg: liquid_bg_statistic,
                        bg_color_gui: liquid_bg_color,
                        border: new RectOffset(45, 45, 20, 20));
                    #endregion

                    #region 液晶 - 附加图形 - 肮脏层
                    if (XTween_Dashboard.XTweenConfig.Datas.LiquidDirty)
                    {
                        float d_x = rect_liquid.width - liquid_dirty.width;
                        float d_y = 0;
                        float d_w = liquid_dirty.width;
                        float d_h = liquid_dirty.height;
                        Rect rect_dirty = new Rect(d_x, d_y, d_w, d_h);

                        GUI.BeginGroup(rect_liquid);
                        XGUI.gui_box(
                            rect: rect_dirty,
                            bg: liquid_dirty);
                        GUI.EndGroup();
                    }
                    #endregion

                    #region 液晶 - 附加图形 - 接口
                    if (!panel_extra_expand)
                    {
                        float liquid_plug_x = ((liquid_root.width / 2)) - (liquid_plug_statistic.width / 2);
                        float liquid_plug_y = 243;
                        float liquid_plug_w = liquid_plug_statistic.width;
                        float liquid_plug_h = liquid_plug_statistic.height;

                        rect_liquid.Set(liquid_root.x + liquid_plug_x, liquid_root.y + liquid_plug_y, liquid_plug_w, liquid_plug_h);
                        XGUI.gui_box(
                            rect: rect_liquid,
                            bg: liquid_plug_statistic);
                    }
                    #endregion

                    #region 液晶 - 附加图形 - 金属网格角
                    float liquid_metal_grid_x = liquid_root.x + liquid_root.width - liquid_metal_grid.width;
                    float liquid_metal_grid_y = liquid_root.y + 203;
                    float liquid_metal_grid_w = liquid_metal_grid.width;
                    float liquid_metal_grid_h = liquid_metal_grid.height;

                    rect_liquid.Set(liquid_metal_grid_x, liquid_metal_grid_y, liquid_metal_grid_w, liquid_metal_grid_h);
                    XGUI.gui_box(
                        rect: rect_liquid,
                        bg: liquid_metal_grid);
                    #endregion

                    float margin = 87;
                    float width_max = liquid_root.width - liquid_root.x - 25 - 10;

                    #region 进度条 - 播放中
                    rect_liquid.Set(liquid_root.x + 25, liquid_root.y + margin, width_max, 0);
                    XGUI.gui_progress(
                        rect: rect_liquid,
                        title: panel_expand ? "" : "播放中",
                        title_size: XGUIFontSize.M,
                        title_offset: new Vector2(0, 0),
                        title_color: Color.black,
                        subtitle: Application.isPlaying ? $"{TwnData_Playing.Count} / {Count_Tweens} ({(Count_Tweens == 0 ? 0 : TwnData_Playing.Percentage_Smooth.ToString("F2"))})" : "-",
                        subtitle_size: XGUIFontSize.S,
                        subtitle_offset: new Vector2(0, 0),
                        subtitle_color: Color.black,
                         line_left_color: Color.white * 0.5f,
                line_right_color: Color.white * 0.5f,
                line_center_color: Color.white * 0.5f,
                        progress_fg_color: Color.black,
                        progress_bg_color: Color.black * 0.12f,
                        indicator_color: Color.black,
                        icon_indicator: XGUI.GetBasedIcon("icon_mark_arrow_up"),
                        value: Count_Tweens == 0 ? 0 : TwnData_Playing.Percentage_Smooth,
                        thickness: 2);
                    #endregion

                    margin += 40;

                    #region 进度条 - 暂停中
                    rect_liquid.Set(liquid_root.x + 25, liquid_root.y + margin, width_max, 0);
                    XGUI.gui_progress(
                        rect: rect_liquid,
                        title: panel_expand ? "" : "暂停中",
                        title_size: XGUIFontSize.M,
                        title_offset: new Vector2(0, 0),
                        title_color: Color.black,
                        subtitle: Application.isPlaying ? $"{TwnData_Pausing.Count} / {Count_Tweens} ({(Count_Tweens == 0 ? 0 : TwnData_Pausing.Percentage_Smooth.ToString("F2"))})" : "-",
                        subtitle_size: XGUIFontSize.S,
                        subtitle_offset: new Vector2(0, 0),
                        subtitle_color: Color.black,
                        line_left_color: Color.white * 0.5f,
                        line_right_color: Color.white * 0.5f,
                        line_center_color: Color.white * 0.5f,
                        progress_fg_color: Color.black,
                        progress_bg_color: Color.black * 0.12f,
                        indicator_color: Color.black,
                        icon_indicator: XGUI.GetBasedIcon("icon_mark_arrow_up"),
                        value: Count_Tweens == 0 ? 0 : TwnData_Pausing.Percentage_Smooth,
                        thickness: 2);
                    #endregion

                    margin += 40;

                    #region 进度条 - 已完成
                    rect_liquid.Set(liquid_root.x + 25, liquid_root.y + margin, width_max, 0);
                    XGUI.gui_progress(
                        rect: rect_liquid,
                        title: panel_expand ? "" : "已完成",
                        title_size: XGUIFontSize.M,
                        title_offset: new Vector2(0, 0),
                        title_color: Color.black,
                        subtitle: Application.isPlaying ? $"{TwnData_Comleted.Count} / {Count_Tweens} ({(Count_Tweens == 0 ? 0 : TwnData_Comleted.Percentage_Smooth.ToString("F2"))})" : "-",
                        subtitle_size: XGUIFontSize.S,
                        subtitle_offset: new Vector2(0, 0),
                        subtitle_color: Color.black,
                        line_left_color: Color.white * 0.5f,
                        line_right_color: Color.white * 0.5f,
                        line_center_color: Color.white * 0.5f,
                        progress_fg_color: Color.black,
                        progress_bg_color: Color.black * 0.12f,
                        indicator_color: Color.black,
                        icon_indicator: XGUI.GetBasedIcon("icon_mark_arrow_up"),
                        value: Count_Tweens == 0 ? 0 : TwnData_Comleted.Percentage_Smooth,
                        thickness: 2);
                    #endregion

                    margin += 40;

                    #region 进度条 - 循环模式
                    rect_liquid.Set(liquid_root.x + 25, liquid_root.y + margin, width_max, 0);
                    XGUI.gui_progress(
                        rect: rect_liquid,
                        title: panel_expand ? "" : "循环模式",
                        title_size: XGUIFontSize.M,
                        title_offset: new Vector2(0, 0),
                        title_color: Color.black,
                        subtitle: Application.isPlaying ? $"{TwnData_HasLoop.Count} / {Count_Tweens} ({(Count_Tweens == 0 ? 0 : TwnData_HasLoop.Percentage_Smooth.ToString("F2"))})" : "-",
                        subtitle_size: XGUIFontSize.S,
                        subtitle_offset: new Vector2(0, 0),
                        subtitle_color: Color.black,
                        line_left_color: Color.white * 0.5f,
                        line_right_color: Color.white * 0.5f,
                        line_center_color: Color.white * 0.5f,
                        progress_fg_color: Color.black,
                        progress_bg_color: Color.black * 0.12f,
                        indicator_color: Color.black,
                        icon_indicator: XGUI.GetBasedIcon("icon_mark_arrow_up"),
                        value: Count_Tweens == 0 ? 0 : TwnData_HasLoop.Percentage_Smooth,
                        thickness: 2);
                    #endregion
                }
                else
                {
                    float margin = 60;
                    float width_max = liquid_root.width - liquid_root.x - 20;

                    #region 进度条 - 播放中
                    rect_liquid.Set(liquid_root.x + 20, liquid_root.y + margin, width_max, 0);
                    XGUI.gui_progress(
                        rect: rect_liquid,
                        title: panel_expand ? "" : "播放中",
                        title_size: XGUIFontSize.M,
                        title_offset: new Vector2(0, 0),
                        title_color: Color.white,
                        subtitle: Application.isPlaying ? $"{TwnData_Playing.Count} / {Count_Tweens} ({(Count_Tweens == 0 ? 0 : TwnData_Playing.Percentage_Smooth.ToString("F2"))})" : "-",
                        subtitle_size: XGUIFontSize.S,
                        subtitle_offset: new Vector2(0, 0),
                        subtitle_color: Color.white,
                        line_left_color: Color.white * 0.5f,
                        line_right_color: Color.white * 0.5f,
                        line_center_color: Color.white * 0.5f,
                        progress_fg_color: XTween_Dashboard.Theme_Primary,
                        progress_bg_color: Color.black * 0.12f,
                        indicator_color: Color.white,
                        icon_indicator: XGUI.GetBasedIcon("icon_mark_arrow_up"),
                        value: Count_Tweens == 0 ? 0 : TwnData_Playing.Percentage_Smooth,
                        thickness: 2);
                    #endregion

                    margin += 45;

                    #region 进度条 - 暂停中
                    rect_liquid.Set(liquid_root.x + 20, liquid_root.y + margin, width_max, 0);
                    XGUI.gui_progress(
                        rect: rect_liquid,
                        title: panel_expand ? "" : "暂停中",
                        title_size: XGUIFontSize.M,
                        title_offset: new Vector2(0, 0),
                        title_color: Color.white,
                        subtitle: Application.isPlaying ? $"{TwnData_Pausing.Count} / {Count_Tweens} ({(Count_Tweens == 0 ? 0 : TwnData_Pausing.Percentage_Smooth.ToString("F2"))})" : "-",
                        subtitle_size: XGUIFontSize.S,
                        subtitle_offset: new Vector2(0, 0),
                        subtitle_color: Color.white,
                        line_left_color: Color.white * 0.5f,
                        line_right_color: Color.white * 0.5f,
                        line_center_color: Color.white * 0.5f,
                        progress_fg_color: XTween_Dashboard.Theme_Primary,
                        progress_bg_color: Color.black * 0.12f,
                        indicator_color: Color.white,
                        icon_indicator: XGUI.GetBasedIcon("icon_mark_arrow_up"),
                        value: Count_Tweens == 0 ? 0 : TwnData_Pausing.Percentage_Smooth,
                        thickness: 2);
                    #endregion

                    margin += 45;

                    #region 进度条 - 已完成
                    rect_liquid.Set(liquid_root.x + 20, liquid_root.y + margin, width_max, 0);
                    XGUI.gui_progress(
                        rect: rect_liquid,
                        title: panel_expand ? "" : "已完成",
                        title_size: XGUIFontSize.M,
                        title_offset: new Vector2(0, 0),
                        title_color: Color.white,
                        subtitle: Application.isPlaying ? $"{TwnData_Comleted.Count} / {Count_Tweens} ({(Count_Tweens == 0 ? 0 : TwnData_Comleted.Percentage_Smooth.ToString("F2"))})" : "-",
                        subtitle_size: XGUIFontSize.S,
                        subtitle_offset: new Vector2(0, 0),
                        subtitle_color: Color.white,
                        line_left_color: Color.white * 0.5f,
                        line_right_color: Color.white * 0.5f,
                        line_center_color: Color.white * 0.5f,
                        progress_fg_color: XTween_Dashboard.Theme_Primary,
                        progress_bg_color: Color.black * 0.12f,
                        indicator_color: Color.white,
                        icon_indicator: XGUI.GetBasedIcon("icon_mark_arrow_up"),
                        value: Count_Tweens == 0 ? 0 : TwnData_Comleted.Percentage_Smooth,
                        thickness: 2);
                    #endregion

                    margin += 45;

                    #region 进度条 - 循环模式
                    rect_liquid.Set(liquid_root.x + 20, liquid_root.y + margin, width_max, 0);
                    XGUI.gui_progress(
                        rect: rect_liquid,
                        title: panel_expand ? "" : "循环模式",
                        title_size: XGUIFontSize.M,
                        title_offset: new Vector2(0, 0),
                        title_color: Color.white,
                        subtitle: Application.isPlaying ? $"{TwnData_HasLoop.Count} / {Count_Tweens} ({(Count_Tweens == 0 ? 0 : TwnData_HasLoop.Percentage_Smooth.ToString("F2"))})" : "-",
                        subtitle_size: XGUIFontSize.S,
                        subtitle_offset: new Vector2(0, 0),
                        subtitle_color: Color.white,
                        line_left_color: Color.white * 0.5f,
                        line_right_color: Color.white * 0.5f,
                        line_center_color: Color.white * 0.5f,
                        progress_fg_color: XTween_Dashboard.Theme_Primary,
                        progress_bg_color: Color.black * 0.12f,
                        indicator_color: Color.white,
                        icon_indicator: XGUI.GetBasedIcon("icon_mark_arrow_up"),
                        value: Count_Tweens == 0 ? 0 : TwnData_HasLoop.Percentage_Smooth,
                        thickness: 2);
                    #endregion
                }
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 活跃动画
            // 获取液晶屏的根锚点
            Rect list_root = XGUI.GetControlRect(false, 0);
            list_root.Set(list_root.x, list_root.y, list_root.width, 0);

            BaseScript.fold_tweenlist = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XTween_Dashboard.Theme_Group,
                title: "活跃动画",
                title_size: XGUIFontSize.M,
                title_text_color: XTween_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(
                    XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode ? 0 : 20,
                    XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode ? 0 : 20,
                    XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode ? 10 : 10,
                    (XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode ? (BaseScript.fold_tweenlist ? 20 : 195) : (BaseScript.fold_tweenlist ? 20 : 580))),
                foldout: BaseScript.fold_tweenlist);

            if (!BaseScript.fold_tweenlist)
            {
                if (Application.isPlaying)
                {
                    if (XTween_Dashboard.XTweenConfig.Datas.LiquidScanStyle)
                        liquid_bg_list = liquid_bg_list_scan;
                    else
                        liquid_bg_list = liquid_bg_list_pure;

                    // 液晶背景颜色
                    liquid_bg_color = XTween_Dashboard.LiquidColor_Playing;
                }
                else
                {
                    // 液晶背景颜色
                    liquid_bg_color = XTween_Dashboard.LiquidColor_Idle;

                    if (XTween_Dashboard.XTweenConfig.Datas.LiquidScanStyle)
                        liquid_bg_list = liquid_bg_list_scan;
                    else
                        liquid_bg_list = liquid_bg_list_pure;
                }

                Rect rect_list = list_root;
                bool shrink = currentWidth <= 248;
                List<XTween_Interface> active_tweens = BaseScript.Get_ActiveTweens();

#if UNITY_6000_0_OR_NEWER
                TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

                if (!XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode)
                {

                    #region 液晶 - 背景
                    float liquid_bg_x = rect_list.x + 10;
                    float liquid_bg_y = rect_list.y + 38;
                    float liquid_bg_w = liquid_root.width - 20;
                    float liquid_bg_h = liquid_bg_list.height;

                    rect_list.Set(liquid_bg_x, liquid_bg_y, liquid_bg_w, liquid_bg_h);
                    XGUI.gui_box(
                        rect: rect_list,
                        bg: liquid_bg_list,
                        bg_color_gui: liquid_bg_color,
                        border: new RectOffset(45, 45, 20, 20));
                    #endregion

                    #region 液晶 - 附加图形 - 肮脏层
                    if (XTween_Dashboard.XTweenConfig.Datas.LiquidDirty)
                    {
                        float d_x = rect_list.width - liquid_dirty.width;
                        float d_y = 0;
                        float d_w = liquid_dirty.width;
                        float d_h = liquid_dirty.height;
                        Rect rect_dirty = new Rect(d_x, d_y, d_w, d_h);

                        GUI.BeginGroup(rect_list);
                        XGUI.gui_box(
                            rect: rect_dirty,
                            bg: liquid_dirty);
                        GUI.EndGroup();
                    }
                    #endregion

                    #region 液晶 - 附加图形 - 接口
                    if (!panel_extra_expand)
                    {
                        float liquid_plug_x = ((list_root.width / 2)) - (liquid_plug_list.width / 2);
                        float liquid_plug_y = 578;
                        float liquid_plug_w = liquid_plug_list.width;
                        float liquid_plug_h = liquid_plug_list.height;

                        rect_list.Set(list_root.x + liquid_plug_x, list_root.y + liquid_plug_y, liquid_plug_w, liquid_plug_h);
                        XGUI.gui_box(
                            rect: rect_list,
                            bg: liquid_plug_list);
                    }
                    #endregion

                    #region 液晶 - 附加图形 - 金属网格角
                    float liquid_metal_grid_x = list_root.x + list_root.width - liquid_metal_grid.width;
                    float liquid_metal_grid_y = list_root.y + 539;
                    float liquid_metal_grid_w = liquid_metal_grid.width;
                    float liquid_metal_grid_h = liquid_metal_grid.height;

                    rect_list.Set(liquid_metal_grid_x, liquid_metal_grid_y, liquid_metal_grid_w, liquid_metal_grid_h);
                    XGUI.gui_box(
                        rect: rect_list,
                        bg: liquid_metal_grid);
                    #endregion

                    // 测试区域
                    //XGUI.gui_box(new Rect(list_root.x + 20, list_root.y + 55, list_root.width - 40, liquid_bg_list.height - 40), Color.red);

                    if (active_tweens.Count > 0)
                    {
                        TweenList_scrollPosition = XGUI.gui_scrollview<XTween_Interface>(
                            rect: new Rect(list_root.x + 20, list_root.y + 55, list_root.width - 40, liquid_bg_list.height - 40),
                            scroll: TweenList_scrollPosition,
                            list: active_tweens,
                            cachedItemHeights: ref TweenList_cachedItemHeights,
                           heightsDirty: ref TweenList_heightsDirty,
                           onscroller: (rect, type, index) =>
                           {
                               Rect rect_content = rect;

                               // 边框
                               rect_content.Set(rect.x, rect.y, rect.width, rect.height - 1);
                               XGUI.gui_box(
                                   rect: rect_content,
                                   bg: XGUI.GetFillTexture(XGUIFilled.纯色边框, XGUIColor.亮白),
                                   bg_color_gui: Color.black * 0.65f,
                                   offset: Vector2.zero,
                                   border: new RectOffset(10, 10, 10, 10),
                                   margin: new RectOffset(0, 0, 0, 0));

                               // 图标
                               rect_content.Set(rect.x + 10, rect.y + 8, ListIndex.width, ListIndex.height);
                               XGUI.gui_box(
                                   rect: rect_content,
                                   bg: ListIndex,
                                   bg_color_gui: Color.black * 0.8f,
                                   offset: Vector2.zero,
                                   margin: new RectOffset(0, 0, 0, 0));

                               // ID
                               rect_content.Set(rect.x + 15 + ListIndex.width + 5, rect.y + 6, rect.width - (10 + ListIndex.width + 10), XGUI.GetSingleLineHeight());
                               XGUI.gui_label(
                                   rect: rect_content,
                                   text: new GUIContent($"{type.ShortId}"),
                                   text_color: Color.black,
                                   size: XGUIFontSize.M,
                                   anchor: TextAnchor.MiddleLeft,
                                   font: XGUI.GetFont("xg-bold"),
                                   clipping: clipping);

                               // Args
                               rect_content.Set(rect.x + 10, rect.y + 25, rect.width - (shrink ? 20 : EasePicBg.width), XGUI.GetSingleLineHeight());
                               XGUI.gui_label(
                                  rect: rect_content,
                                  text: new GUIContent($"P: {type.CurrentLoopProgress.ToString("F2")}  |  E: {type.CurrentEasedProgress.ToString("F2")}  |  {(type.ElapsedTime * 1000).ToString("F2")} ms / {(type.Duration * 1000).ToString("F2")} ms  |  D: {type.Delay.ToString("F2")}s  |  R: {type.IsRelative}  |  L: {type.LoopCount}  |  C: {type.CurrentLoop} 次  |  {type.LoopType.ToString()}"),
                                  text_color: Color.black * 0.8f,
                                  size: XGUIFontSize.S,
                                  anchor: TextAnchor.MiddleLeft,
                                  clipping: clipping);

                               // 进度条 - 背景 - Ease
                               rect_content.Set(rect.x + 10, rect.y + 45, rect.width - 10 - (shrink ? 0 : EasePicBg.width) - (shrink ? 20 : 0), 2);
                               XGUI.gui_box(rect: rect_content,
                                   bg_color: Color.black * 0.2f);

                               // 进度条 - 前景 - Ease
                               rect_content.Set(rect.x + 10, rect.y + 45, (rect.width - 10 - (shrink ? 0 : EasePicBg.width) - (shrink ? 20 : 0)) * type.CurrentEasedProgress, 2);
                               XGUI.gui_box(rect: rect_content,
                                   bg_color: Color.black);

                               // 进度条 - 背景 - Loop
                               rect_content.Set(rect.x + 10, rect.y + 52, rect.width - 10 - (shrink ? 0 : EasePicBg.width) - (shrink ? 20 : 0), 2);
                               XGUI.gui_box(rect: rect_content,
                                   bg_color: Color.black * 0.2f);

                               // 进度条 - 前景 - Loop
                               rect_content.Set(rect.x + 10, rect.y + 52, (rect.width - 10 - (shrink ? 0 : EasePicBg.width) - (shrink ? 20 : 0)) * type.CurrentLoopProgress, 2);
                               XGUI.gui_box(rect: rect_content,
                                   bg_color: Color.black);

                               if (!shrink)
                               {
                                   // Ease Bg
                                   rect_content.Set(rect.x + (rect.width - (EasePicBg.width * 0.7f) - 10), rect.y + 16, EasePicBg.width * 0.7f, EasePicBg.height * 0.7f);
                                   XGUI.gui_box(
                                       rect: rect_content,
                                       bg: EasePicBg,
                                       bg_color_gui: Color.black * 0.8f,
                                       offset: Vector2.zero,
                                       margin: new RectOffset(0, 0, 0, 0));

                                   // Ease Graph
                                   if (type.UseCustomEaseCurve)
                                   {
                                       rect_content.Set(rect.x + (rect.width - (EasePicBg.width * 0.7f)), rect.y + 16, EasePicBg.width * 0.7f, EasePicBg.height * 0.7f);
                                       XGUI.gui_label(
                                           rect: rect_content,
                                           text: new GUIContent($"CustomCurve"),
                                           text_color: Color.black * 0.8f,
                                           size: XGUIFontSize.XS,
                                           anchor: TextAnchor.MiddleLeft,
                                           font: XGUI.GetFont("xg-heavy"),
                                           clipping: clipping);
                                   }
                                   else
                                   {
                                       Texture2D ease_graph = Get_Ease_Type_Texture(type.EaseMode);
                                       rect_content.Set(rect.x + (rect.width - (ease_graph.width * 0.7f) - 10), rect.y + 16, ease_graph.width * 0.7f, ease_graph.height * 0.7f);
                                       XGUI.gui_box(
                                           rect: rect_content,
                                           bg: ease_graph,
                                           bg_color_gui: Color.black * 0.8f,
                                           offset: Vector2.zero,
                                           margin: new RectOffset(0, 0, 0, 0));
                                   }
                               }
                           }
                           );
                    }
                    else
                    {
                        Rect rect_null = list_root;

                        // 测试区域
                        //XGUI.gui_box(new Rect(list_root.x, list_root.y + 38, list_root.width, liquid_bg_list.height), Color.red * 0.5f);

                        // 图标
                        Texture2D tex_icon = XGUI.GetBasedIcon("icon_warning");
                        rect_null.Set(list_root.x + (list_root.width / 2) - (tex_icon.width / 2), list_root.y + 20 + (liquid_bg_list.height / 2) - tex_icon.height, tex_icon.width, tex_icon.height);
                        XGUI.gui_box(
                            rect: rect_null,
                            bg: tex_icon,
                            bg_color_gui: Color.black * 0.7f,
                            offset: Vector2.zero,
                            margin: new RectOffset(0, 0, 0, 0));

                        // Tip
                        rect_null.Set(list_root.x, list_root.y + 60 + (liquid_bg_list.height / 2) - tex_icon.height, list_root.width, XGUI.GetSingleLineHeight());
                        XGUI.gui_label(
                            rect: rect_null,
                            text: new GUIContent("当 前 没 有 活 跃 的 动 画"),
                            text_color: Color.black * 0.5f,
                            size: XGUIFontSize.M,
                            anchor: TextAnchor.MiddleCenter,
                            clipping: clipping);
                    }
                }
                else
                {
                    XGUI.layout_space(345);

                    if (active_tweens.Count > 0)
                    {
                        TweenList_scrollPosition = XGUI.gui_scrollview<XTween_Interface>(
                            rect: new Rect(list_root.x + 20, list_root.y + 50, list_root.width - 40, liquid_bg_list.height - 40),
                            scroll: TweenList_scrollPosition,
                            list: active_tweens,
                            cachedItemHeights: ref TweenList_cachedItemHeights,
                           heightsDirty: ref TweenList_heightsDirty,
                           onscroller: (rect, type, index) =>
                           {
                               Rect rect_content = rect;

                               // 边框
                               rect_content.Set(rect.x, rect.y, rect.width, rect.height - 1);
                               XGUI.gui_box(
                                   rect: rect_content,
                                   bg: XGUI.GetFillTexture(XGUIFilled.纯色边框, XGUIColor.亮白),
                                   bg_color_gui: Color.white * 0.45f,
                                   offset: Vector2.zero,
                                   border: new RectOffset(10, 10, 10, 10),
                                   margin: new RectOffset(0, 0, 0, 0));

                               // 图标
                               rect_content.Set(rect.x + 10, rect.y + 8, ListIndex.width, ListIndex.height);
                               XGUI.gui_box(
                                   rect: rect_content,
                                   bg: ListIndex,
                                   bg_color_gui: XTween_Dashboard.Theme_Primary,
                                   offset: Vector2.zero,
                                   margin: new RectOffset(0, 0, 0, 0));

                               // ID
                               rect_content.Set(rect.x + 15 + ListIndex.width + 5, rect.y + 6, rect.width - (10 + ListIndex.width + 10), XGUI.GetSingleLineHeight());
                               XGUI.gui_label(
                                   rect: rect_content,
                                   text: new GUIContent($"{type.ShortId}"),
                                   text_color: Color.white,
                                   size: XGUIFontSize.M,
                                   anchor: TextAnchor.MiddleLeft,
                                   font: XGUI.GetFont("xg-bold"),
                                   clipping: clipping);

                               // Args
                               rect_content.Set(rect.x + 10, rect.y + 25, rect.width - (shrink ? 20 : EasePicBg.width), XGUI.GetSingleLineHeight());
                               XGUI.gui_label(
                                  rect: rect_content,
                                  text: new GUIContent($"P: {type.CurrentLoopProgress.ToString("F2")}  |  E: {type.CurrentEasedProgress.ToString("F2")}  |  {(type.ElapsedTime * 1000).ToString("F2")} ms / {(type.Duration * 1000).ToString("F2")} ms  |  D: {type.Delay.ToString("F2")}s  |  R: {type.IsRelative}  |  L: {type.LoopCount}  |  C: {type.CurrentLoop} 次  |  {type.LoopType.ToString()}"),
                                  text_color: Color.white * 0.8f,
                                  size: XGUIFontSize.S,
                                  anchor: TextAnchor.MiddleLeft,
                                  clipping: clipping);

                               // 进度条 - 背景 - Ease
                               rect_content.Set(rect.x + 10, rect.y + 45, rect.width - 10 - (shrink ? 0 : EasePicBg.width) - (shrink ? 20 : 0), 2);
                               XGUI.gui_box(rect: rect_content,
                                   bg_color: Color.white * 0.55f);

                               // 进度条 - 前景 - Ease
                               rect_content.Set(rect.x + 10, rect.y + 45, (rect.width - 10 - (shrink ? 0 : EasePicBg.width) - (shrink ? 20 : 0)) * type.CurrentEasedProgress, 2);
                               XGUI.gui_box(rect: rect_content,
                                   bg_color: Color.white * 0.85f);

                               // 进度条 - 背景 - Loop
                               rect_content.Set(rect.x + 10, rect.y + 52, rect.width - 10 - (shrink ? 0 : EasePicBg.width) - (shrink ? 20 : 0), 2);
                               XGUI.gui_box(rect: rect_content,
                                   bg_color: Color.white * 0.55f);

                               // 进度条 - 前景 - Loop
                               rect_content.Set(rect.x + 10, rect.y + 52, (rect.width - 10 - (shrink ? 0 : EasePicBg.width) - (shrink ? 20 : 0)) * type.CurrentLoopProgress, 2);
                               XGUI.gui_box(rect: rect_content,
                                   bg_color: Color.white * 0.85f);

                               if (!shrink)
                               {
                                   // Ease Bg
                                   rect_content.Set(rect.x + (rect.width - (EasePicBg.width * 0.7f) - 10), rect.y + 16, EasePicBg.width * 0.7f, EasePicBg.height * 0.7f);
                                   XGUI.gui_box(
                                       rect: rect_content,
                                       bg: EasePicBg,
                                       bg_color_gui: Color.white * 0.85f,
                                       offset: Vector2.zero,
                                       margin: new RectOffset(0, 0, 0, 0));

                                   // Ease Graph
                                   if (type.UseCustomEaseCurve)
                                   {
                                       rect_content.Set(rect.x + (rect.width - (EasePicBg.width * 0.7f)), rect.y + 16, EasePicBg.width * 0.7f, EasePicBg.height * 0.7f);
                                       XGUI.gui_label(
                                           rect: rect_content,
                                           text: new GUIContent($"CustomCurve"),
                                           text_color: Color.white * 0.8f,
                                           size: XGUIFontSize.XS,
                                           anchor: TextAnchor.MiddleLeft,
                                           font: XGUI.GetFont("xg-heavy"),
                                           clipping: clipping);
                                   }
                                   else
                                   {
                                       Texture2D ease_graph = Get_Ease_Type_Texture(type.EaseMode);
                                       rect_content.Set(rect.x + (rect.width - (ease_graph.width * 0.7f) - 10), rect.y + 16, ease_graph.width * 0.7f, ease_graph.height * 0.7f);
                                       XGUI.gui_box(
                                           rect: rect_content,
                                           bg: ease_graph,
                                           bg_color_gui: XTween_Dashboard.Theme_Primary,
                                           offset: Vector2.zero,
                                           margin: new RectOffset(0, 0, 0, 0));
                                   }
                               }
                           }
                           );
                    }
                    else
                    {
                        Rect rect_null = list_root;

                        // 测试区域
                        //XGUI.gui_box(new Rect(list_root.x, list_root.y + 38, list_root.width, liquid_bg_list.height), Color.red * 0.5f);

                        // 图标
                        Texture2D tex_icon = XGUI.GetBasedIcon("icon_warning");
                        rect_null.Set(list_root.x + (list_root.width / 2) - (tex_icon.width / 2), list_root.y + 258, tex_icon.width, tex_icon.height);
                        XGUI.gui_box(
                            rect: rect_null,
                            bg: tex_icon,
                            bg_color_gui: Color.white * 0.6f,
                            offset: Vector2.zero,
                            margin: new RectOffset(0, 0, 0, 0));

                        // Tip
                        rect_null.Set(list_root.x, list_root.y + 298, list_root.width, XGUI.GetSingleLineHeight());
                        XGUI.gui_label(
                            rect: rect_null,
                            text: new GUIContent("当 前 没 有 活 跃 的 动 画"),
                            text_color: Color.white * 0.65f,
                            size: XGUIFontSize.M,
                            anchor: TextAnchor.MiddleCenter,
                            clipping: clipping);
                    }
                }
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion           
        }

        #region 辅助
        private string Get_TweenState(XTween_Interface tween)
        {
            if (tween == null) return "null";
            if (tween.IsKilled) return "已杀死";
            if (tween.IsCompleted) return "已完成";
            if (tween.IsPaused) return "已暂停";
            return tween.IsPlaying ? "播放中" : "待命中";
        }
        private string Get_Tween_Progress(XTween_Interface tween)
        {
            if (tween == null) return "null";
            return $"{tween.CurrentLinearProgress * 100:F1}% (缓动参数: {tween.CurrentEasedProgress * 100:F1}%)";
        }
        #endregion

        #region GetEaseTextureAssets
        /// <summary>
        /// 获取缓动参数曲线图
        /// </summary>
        /// <param name="ease"></param>
        /// <returns></returns>
        private Texture2D Get_Ease_Type_Texture(EaseMode ease)
        {
            Texture2D ease_tex = null;
            for (int i = 0; i < BaseScript.EasePics.Length; i++)
            {
                if (BaseScript.EasePics[i].name == ease.ToString())
                {
                    ease_tex = BaseScript.EasePics[i];
                    break;
                }
            }
            return ease_tex;
        }
        private Texture2D Get_Ease_Type_Texture(string ease)
        {
            return XGUI.GetBasedIcon($"EaseGraph/{ease}");
        }
        /// <summary>
        /// 获取缓动参数曲线图背景
        /// </summary>        
        /// <returns></returns>
        private Texture2D Get_Ease_Bg_Texture()
        {
            return XGUI.GetBasedIcon($"EaseGraph/bg");
        }
        #endregion
    }
}