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
    using UnityEditor;
    using UnityEngine;

    public class XHud_PerfomanceMonitor_Node : MonoBehaviour, xHud_MonitorData
    {
        #region 接口成员实现
        public string title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }
        public string value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public XHud_Module_TmpText tmp_title
        {
            get
            {
                return _tmp_title;
            }
            set
            {
                _tmp_title = value;
            }
        }
        /// <summary>
        /// 数值
        /// </summary>
        public XHud_Module_TmpText tmp_value
        {
            get
            {
                return _tmp_value;
            }
            set
            {
                _tmp_value = value;
            }
        }
        /// <summary>
        /// 标题颜色
        /// </summary>
        [SerializeField]
        public Color col_title
        {
            get
            {
                return _col_title;
            }
            set
            {
                _col_title = value;
            }
        }
        /// <summary>
        /// 数值颜色
        /// </summary>
        [SerializeField]
        public Color col_value
        {
            get
            {
                return _col_value;
            }
            set
            {
                _col_value = value;
            }
        }

        #endregion

        /// <summary>
        /// 标题内容
        /// </summary>
        [SerializeField] public string _title;
        /// <summary>
        /// 数值内容
        /// </summary>
        [SerializeField] public string _value;

        /// <summary>
        /// 标题
        /// </summary>
        [SerializeField] public XHud_Module_TmpText _tmp_title;
        /// <summary>
        /// 数值
        /// </summary>
        [SerializeField] public XHud_Module_TmpText _tmp_value;

        [SerializeField] public Color _col_title = Color.white;
        [SerializeField] public Color _col_value = XHud_Dashboard.Theme_Primary;

        [SerializeField] public Color _col_value_normal = XHud_Dashboard.Theme_Primary;
        [SerializeField] public Color _col_value_limited = new Color(1f, 0.3820755f, 0.3865624f, 1);
        /// <summary>
        /// Fps指标阈值
        /// </summary>
        [SerializeField] public int Threshold_FPS = 100;
        /// <summary>
        /// Cpu指标阈值
        /// </summary>
        [SerializeField] public float Threshold_CPU = 10f;

        private int fixedval_int
        {
            get { return _fixedval_int; }
            set
            {
                if (_fixedval_int != value)
                    _fixedval_int = value;
            }
        }
        private float fixedval_float
        {
            get { return _fixedval_float; }
            set
            {
                if (_fixedval_float != value)
                    _fixedval_float = value;
            }
        }
        private Vector2 fixedval_vector2
        {
            get { return _fixedval_vector2; }
            set
            {
                if (_fixedval_vector2 != value)
                    _fixedval_vector2 = value;
            }
        }

        [SerializeField] private int _fixedval_int;
        [SerializeField] private float _fixedval_float;
        [SerializeField] private Vector2 _fixedval_vector2;

        #region 接口方法实现
        public void Initialize()
        {
            tmp_title.tmp_Set_Content(title);
        }
        public void UpdateData()
        {
            switch (title)
            {
                case "FPS":
                    fixedval_int = Get_FPS();
                    if (fixedval_int < Threshold_FPS)
                        col_value = _col_value_limited;
                    else
                        col_value = _col_value_normal;

                    value = $"{fixedval_int.ToString("D0")} fps";
                    break;
                case "CpuTime":
                    fixedval_float = Get_CpuTime();
                    if (fixedval_float > Threshold_CPU)
                        col_value = _col_value_limited;
                    else
                        col_value = _col_value_normal;

                    value = $"{fixedval_float.ToString("F1")} ms";
                    break;
                case "ScreenSize":
                    fixedval_vector2 = Get_Resolution();
                    value = $"{fixedval_vector2.x} x {fixedval_vector2.y}";
                    break;
                case "Resolution":
                    fixedval_vector2 = Get_ScreenSize();
                    value = $"{fixedval_vector2.x} x {fixedval_vector2.y}";
                    break;
                case "FreshRate":
                    fixedval_int = Get_ScreenFreshRate();
                    value = $"{fixedval_int} hz";
                    break;
                case "Batches":
                    fixedval_int = Get_Batches();
                    value = fixedval_int == 0 ? "EditorOnly" : $"{fixedval_int}";
                    break;
                case "SetpassCalls":
                    fixedval_int = Get_SetpassCalls();
                    value = fixedval_int == 0 ? "EditorOnly" : $"{fixedval_int}";
                    break;
                case "Vertices":
                    fixedval_int = Get_Vertices();
                    value = fixedval_int == 0 ? "EditorOnly" : $"{fixedval_int}";
                    break;
                case "Triangles":
                    fixedval_int = Get_Triangles();
                    value = fixedval_int == 0 ? "EditorOnly" : $"{fixedval_int}";
                    break;
                case "DrawCalls":
                    fixedval_int = Get_DrawCalls();
                    value = fixedval_int == 0 ? "EditorOnly" : $"{fixedval_int}";
                    break;
            }

            tmp_value.tmp_Set_Content(value);
            tmp_title.TextStyleInfo.tmp_Set_FontColor(col_title);
            tmp_value.TextStyleInfo.tmp_Set_FontColor(col_value);
        }
        #endregion

        /// <summary>
        /// 获取屏幕尺寸
        /// </summary>
        /// <returns></returns>
        private Vector2 Get_ScreenSize()
        {
            return new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        }
        /// <summary>
        /// 获取分辨率
        /// </summary>
        /// <returns></returns>
        private Vector2 Get_Resolution()
        {
            return new Vector2(Screen.width, Screen.height);
        }
        /// <summary>
        /// 获取刷新率
        /// </summary>
        /// <returns></returns>
        private int Get_ScreenFreshRate()
        {
            return (int)Screen.currentResolution.refreshRateRatio.value;
        }
        /// <summary>
        /// 获取FPS
        /// </summary>
        /// <returns></returns>
        private int Get_FPS()
        {
            return (int)(1.0f / Time.unscaledDeltaTime);
        }
        /// <summary>
        /// 获取Cpu时间
        /// </summary>
        /// <returns></returns>
        private float Get_CpuTime()
        {
            return Time.unscaledDeltaTime * 1000f;
        }
        /// <summary>
        /// 批处理数
        /// </summary>
        /// <returns></returns>
        private int Get_Batches()
        {
#if UNITY_EDITOR
            return UnityStats.batches;
#else
return 0;
#endif
        }
        /// <summary>
        /// SetpassCalls
        /// </summary>
        /// <returns></returns>
        private int Get_SetpassCalls()
        {
#if UNITY_EDITOR
            return UnityStats.setPassCalls;
#else
return 0;
#endif
        }
        /// <summary>
        /// 顶点
        /// </summary>
        /// <returns></returns>
        private int Get_Vertices()
        {
#if UNITY_EDITOR
            return UnityStats.vertices;
#else
return 0;
#endif
        }
        /// <summary>
        /// 三角面
        /// </summary>
        /// <returns></returns>
        private int Get_Triangles()
        {
#if UNITY_EDITOR
            return UnityStats.triangles;
#else
return 0;
#endif
        }
        /// <summary>
        /// DrawCalls
        /// </summary>
        /// <returns></returns>
        private int Get_DrawCalls()
        {
#if UNITY_EDITOR
            return UnityStats.drawCalls;
#else
return 0;
#endif
        }
    }
}