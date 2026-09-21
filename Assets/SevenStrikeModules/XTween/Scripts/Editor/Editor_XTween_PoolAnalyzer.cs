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
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    public struct PoolData
    {
        /// <summary>
        /// 动画类型总数
        /// </summary>
        public int Count;
        /// <summary>
        /// 动画类型预加载数
        /// </summary>
        public int Preloaded;
        /// <summary>
        /// 动画已使用总数
        /// </summary>
        public int InUseCount;
        /// <summary>
        /// 动画已使用百分比
        /// </summary>
        public float Percentage;
        /// <summary>
        /// 动画已使用百分比（平滑）
        /// </summary>
        public float Percentage_Smooth;

        /// <summary>
        /// 计算数量与百分比
        /// </summary>
        /// <param name="type"></param>
        public void CalculateData(Type type)
        {
            Count = XTween_Pool.Get_PoolCount(type);
            Preloaded = XTween_Pool.GetPreloadCount(type);
            InUseCount = XTween_Pool.Get_CreatedCount(type);
            Percentage = XTween_Pool.Get_UsagePercentage(type);
            Percentage_Smooth = Mathf.Lerp(Percentage_Smooth, Percentage, Time.unscaledDeltaTime * 6);
        }
    }

    public struct progress_value
    {
        public string title;
        public string sub_title;
        public float value;
        public float title_width;
        public float subtitle_width;
    }

    public class Editor_XTween_PoolAnalyzer : EditorWindow
    {
        private static Editor_XTween_PoolAnalyzer window;
        private StringBuilder stringBuilder;

        /// <summary>
        /// 图标
        /// </summary>
        private Texture2D
            logo_bg,
            liquid_bg,
            liquid_bg_pure,
            liquid_bg_scan,
            liquid_plug,
            liquid_metal_grid,
            liquid_dirty;

        PoolData PoolData_Int;
        PoolData PoolData_Float;
        PoolData PoolData_String;
        PoolData PoolData_Vector2;
        PoolData PoolData_Vector3;
        PoolData PoolData_Vector4;
        PoolData PoolData_Quaternion;
        PoolData PoolData_Color;

        public progress_value[] progress_values = new progress_value[8];

        [MenuItem("Assets/XTween/D 动画池分析仪（PoolManager)")]
        public static void Editor_Open_XTween_PoolAnalyzer()
        {
            window = (Editor_XTween_PoolAnalyzer)EditorWindow.GetWindow(typeof(Editor_XTween_PoolAnalyzer), false, "XTween 动画池分析仪", true);
            XGUI.CenterEditorWindow(new Vector2Int(353, 550), window);
            window.maxSize = window.minSize;
            window.Show();
        }

        private void OnEnable()
        {
            #region 图标获取
            logo_bg = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_pool/logo");
            liquid_bg_pure = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/pool/liquid_bg_pure");
            liquid_bg_scan = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/pool/liquid_bg_scan");
            liquid_plug = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/plug/liquid_plug_red");
            liquid_dirty = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/dirty/liquid_dirty");
            liquid_metal_grid = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/liquid_metal_grid");
            #endregion

            stringBuilder = new StringBuilder();
        }

        private void OnGUI()
        {
            #region 抬头
            Rect rect = new Rect(0, 0, position.width, position.height);

            // 图标
            Rect rect_icon = new Rect(15, 15, logo_bg.width, logo_bg.height);
            XGUI.gui_icon(
                rect: rect_icon,
                icon: logo_bg,
                padding: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                color: Color.white);

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            // 大标题
            Rect rect_title = new Rect(rect.x + 70, rect.y + 10, rect.width - 80, 30);
            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent("XTween 动画池分析仪"),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping,
                font: XGUI.GetFont("xg-heavy"));

            // 分割线
            Rect rect_seperate = new Rect(rect.x + 68, rect.y + 43, 200, 1);
            XGUI.gui_seperator(
                rect: rect_seperate,
                thickness: 1,
                color: Color.white * 0.45f,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            // 小标题
            Rect rect_subtitle = new Rect(rect.x + 18, rect.y + 45, rect.width, 30);
            XGUI.gui_label(
                rect: rect_subtitle,
                text: new GUIContent("此面板可监控并管理XTween动画池的使用状态以及参数！"),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                clipping: clipping);
            #endregion

            CollectPoolData();

            // 获取液晶屏的根锚点
            Rect rect_root = new Rect(0, 0, position.width, position.height);
            rect_root.Set(rect_root.x + 10, rect_root.y + 100, rect_root.width - 20, rect_root.height);
            Rect rect_liquid = rect_root;

            // 测试区域
            //XGUI.gui_box(rect_liquid, Color.red);

            Color liquid_bg_color = Color.white;

            #region 面板预览器
            if (Application.isPlaying)
            {
                if (XTween_Dashboard.XTweenConfig.Datas.LiquidScanStyle)
                    liquid_bg = liquid_bg_scan;
                else
                    liquid_bg = liquid_bg_pure;
                liquid_bg_color = XTween_Dashboard.XTweenConfig.Datas.LiquidColor_Playing;
            }
            else
            {
                liquid_bg_color = XTween_Dashboard.XTweenConfig.Datas.LiquidColor_Idle;

                if (XTween_Dashboard.XTweenConfig.Datas.LiquidScanStyle)
                    liquid_bg = liquid_bg_scan;
                else
                    liquid_bg = liquid_bg_pure;
            }

            if (!XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode)
            {
                #region 液晶 - 背景
                float liquid_bg_x = rect_liquid.x;
                float liquid_bg_y = rect_liquid.y;
                float liquid_bg_w = rect_liquid.width;
                float liquid_bg_h = liquid_bg.height;

                Rect rect_liquidbg = new Rect(liquid_bg_x, liquid_bg_y, liquid_bg_w, liquid_bg_h);

                // 测试区域
                //XGUI.gui_box(rect_liquidbg, Color.red);

                XGUI.gui_box(
                    rect: rect_liquidbg,
                    bg: liquid_bg,
                    bg_color_gui: liquid_bg_color,
                    border: new RectOffset(45, 45, 20, 20));
                #endregion

                #region 液晶 - 附加图形 - 肮脏层
                if (XTween_Dashboard.XTweenConfig.Datas.LiquidDirty)
                {
                    GUI.BeginGroup(rect_liquidbg);

                    float d_x = rect_liquid.width - liquid_dirty.width;
                    float d_y = 0;
                    float d_w = liquid_dirty.width;
                    float d_h = liquid_dirty.height;
                    Rect rect_dirty = new Rect(d_x, d_y, d_w, d_h);

                    XGUI.gui_box(
                        rect: rect_dirty,
                        bg: liquid_dirty);

                    GUI.EndGroup();

                    // 测试区域
                    //XGUI.gui_box(rect_dirty, Color.red * 0.5f);
                }
                #endregion

                #region 液晶 - 附加图形 - 接口
                float liquid_plug_x = rect_liquidbg.x + ((rect_liquidbg.width / 2)) - (liquid_plug.width / 2);
                float liquid_plug_y = rect_liquidbg.y + rect_liquidbg.height;
                float liquid_plug_w = liquid_plug.width;
                float liquid_plug_h = liquid_plug.height;
                Rect rect_plug = new Rect(liquid_plug_x, liquid_plug_y, liquid_plug_w, liquid_plug_h);

                XGUI.gui_box(
                    rect: rect_plug,
                    bg: liquid_plug);

                // 测试区域
                //XGUI.gui_box(rect_plug, Color.red);
                #endregion

                #region 液晶 - 附加图形 - 金属网格角
                float liquid_metal_grid_x = rect_liquidbg.x + rect_liquidbg.width - (liquid_metal_grid.width - 10);
                float liquid_metal_grid_y = rect_liquidbg.y + rect_liquidbg.height - 40;
                float liquid_metal_grid_w = liquid_metal_grid.width;
                float liquid_metal_grid_h = liquid_metal_grid.height;
                Rect rect_metal_grid = new Rect(liquid_metal_grid_x, liquid_metal_grid_y, liquid_metal_grid_w, liquid_metal_grid_h);

                XGUI.gui_box(
                    rect: rect_metal_grid,
                    bg: liquid_metal_grid);

                // 测试区域
                //XGUI.gui_box(rect_metal_grid, Color.red * 0.5f);
                #endregion

                float margin = 50;
                float width_max = rect_liquidbg.width - rect_liquidbg.x - 30;

                for (int i = 0; i < progress_values.Length; i++)
                {
                    progress_value dat = progress_values[i];
                    #region 进度条
                    Rect rect_progress_int = new Rect(rect_liquidbg.x + 20, rect_liquidbg.y + margin, width_max, 0);
                    XGUI.gui_progress(
                        rect: rect_progress_int,
                        title: dat.title,
                        title_width: dat.title_width,
                        subtitle_width: dat.subtitle_width,
                        title_size: XGUIFontSize.M,
                        title_offset: new Vector2(0, 0),
                        title_color: Color.black,
                        subtitle: dat.sub_title,
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
                        value: dat.value,
                        thickness: 2);
                    #endregion

                    margin += 45;
                }
            }
            else
            {
                float margin = 50;
                float width_max = rect_liquid.width - rect_liquid.x - 30;

                for (int i = 0; i < progress_values.Length; i++)
                {
                    progress_value dat = progress_values[i];
                    #region 进度条
                    Rect rect_progress_int = new Rect(rect_liquid.x + 20, rect_liquid.y + margin, width_max, 0);
                    XGUI.gui_progress(
                        rect: rect_progress_int,
                        title: dat.title,
                        title_width: dat.title_width,
                        subtitle_width: dat.subtitle_width,
                        title_size: XGUIFontSize.M,
                        title_offset: new Vector2(0, 0),
                        title_color: Color.white,
                        subtitle: dat.sub_title,
                        subtitle_size: XGUIFontSize.S,
                        subtitle_offset: new Vector2(0, 0),
                        subtitle_color: Color.white * 0.8f,
                         line_left_color: Color.white * 0.5f,
                         line_right_color: Color.white * 0.5f,
                         line_center_color: Color.white * 0.5f,
                        progress_fg_color: XTween_Dashboard.Theme_Primary,
                        progress_bg_color: Color.black * 0.12f,
                        indicator_color: Color.white,
                        icon_indicator: XGUI.GetBasedIcon("icon_mark_arrow_up"),
                        value: dat.value,
                        thickness: 2);
                    #endregion

                    margin += 45;
                }
            }
            GUI.backgroundColor = Color.white;
            #endregion
        }

        private string PoolDataVisual(PoolData data)
        {
            stringBuilder.Clear();
            stringBuilder.Append(data.InUseCount)
                .Append(" | ")
                .Append(data.Count)
                .Append(" / ")
                .Append(data.Preloaded)
                .Append(" | ")
                .Append(data.Percentage_Smooth.ToString("F4"))
                .Append(" %");
            return stringBuilder.ToString();
        }

        private void Update()
        {
            CalculatePoolData();
        }

        /// <summary>
        /// 计算每个类型的动画池的状态数据
        /// </summary>
        private void CalculatePoolData()
        {
            if (Application.isPlaying)
            {
                PoolData_Int.CalculateData(typeof(XTween_Specialized_Int));
                PoolData_Float.CalculateData(typeof(XTween_Specialized_Float));
                PoolData_String.CalculateData(typeof(XTween_Specialized_String));
                PoolData_Vector2.CalculateData(typeof(XTween_Specialized_Vector2));
                PoolData_Vector3.CalculateData(typeof(XTween_Specialized_Vector3));
                PoolData_Vector4.CalculateData(typeof(XTween_Specialized_Vector4));
                PoolData_Quaternion.CalculateData(typeof(XTween_Specialized_Quaternion));
                PoolData_Color.CalculateData(typeof(XTween_Specialized_Color));
            }
            Repaint();
        }

        /// <summary>
        /// 收集动画池数据
        /// </summary>
        private void CollectPoolData()
        {
            #region 数据获取
            progress_values[0].title = "整形 - Integer";
            progress_values[1].title = "浮点 - Float";
            progress_values[2].title = "字符串 - String";
            progress_values[3].title = "二维向量 - Vector 2";
            progress_values[4].title = "三维向量 - Vector 3";
            progress_values[5].title = "四维向量 - Vector 4";
            progress_values[6].title = "四元数 - Quaternion";
            progress_values[7].title = "颜色 - Color";

            if (Application.isPlaying)
            {
                progress_values[0].value = PoolData_Int.Percentage_Smooth;
                progress_values[1].value = PoolData_Float.Percentage_Smooth;
                progress_values[2].value = PoolData_String.Percentage_Smooth;
                progress_values[3].value = PoolData_Vector2.Percentage_Smooth;
                progress_values[4].value = PoolData_Vector3.Percentage_Smooth;
                progress_values[5].value = PoolData_Vector4.Percentage_Smooth;
                progress_values[6].value = PoolData_Quaternion.Percentage_Smooth;
                progress_values[7].value = PoolData_Color.Percentage_Smooth;
            }
            else
            {
                progress_values[0].value = 0;
                progress_values[1].value = 0;
                progress_values[2].value = 0;
                progress_values[3].value = 0;
                progress_values[4].value = 0;
                progress_values[5].value = 0;
                progress_values[6].value = 0;
                progress_values[7].value = 0;
            }

            progress_values[0].sub_title = Application.isPlaying ? PoolDataVisual(PoolData_Int) : "待预加载";
            progress_values[1].sub_title = Application.isPlaying ? PoolDataVisual(PoolData_Float) : "待预加载";
            progress_values[2].sub_title = Application.isPlaying ? PoolDataVisual(PoolData_String) : "待预加载";
            progress_values[3].sub_title = Application.isPlaying ? PoolDataVisual(PoolData_Vector2) : "待预加载";
            progress_values[4].sub_title = Application.isPlaying ? PoolDataVisual(PoolData_Vector3) : "待预加载";
            progress_values[5].sub_title = Application.isPlaying ? PoolDataVisual(PoolData_Vector4) : "待预加载";
            progress_values[6].sub_title = Application.isPlaying ? PoolDataVisual(PoolData_Quaternion) : "待预加载";
            progress_values[7].sub_title = Application.isPlaying ? PoolDataVisual(PoolData_Color) : "待预加载";

            progress_values[0].title_width = 125;
            progress_values[1].title_width = 125;
            progress_values[2].title_width = 125;
            progress_values[3].title_width = 125;
            progress_values[4].title_width = 125;
            progress_values[5].title_width = 125;
            progress_values[6].title_width = 125;
            progress_values[7].title_width = 125;

            progress_values[0].subtitle_width = 125;
            progress_values[1].subtitle_width = 125;
            progress_values[2].subtitle_width = 125;
            progress_values[3].subtitle_width = 125;
            progress_values[4].subtitle_width = 125;
            progress_values[5].subtitle_width = 125;
            progress_values[6].subtitle_width = 125;
            progress_values[7].subtitle_width = 125;

            #endregion
        }
    }
}