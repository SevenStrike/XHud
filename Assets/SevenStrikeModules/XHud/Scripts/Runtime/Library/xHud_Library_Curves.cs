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
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;

    [System.Serializable]
    /// <summary>
    /// Hud曲线信息
    /// </summary>
    public class xHud_LibraryArg_Curve
    {
        public string name;
        public AnimationCurve curve;

        public xHud_LibraryArg_Curve()
        {

        }

        /// <summary>
        /// 新增曲线信息
        /// </summary>
        /// <param name="name">新增名称</param>
        /// <param name="curve">新增曲线</param>
        public xHud_LibraryArg_Curve(string name = null, AnimationCurve curve = null)
        {
            this.name = name;
            this.curve = curve;
        }
    }

    [CreateAssetMenu(fileName = "XHud_Library_Curves", menuName = "XHud/CreateAssets (创建Hud资源库)/Library-Curve (曲线库)", order = 0)]
    public class XHud_Library_Curves : ScriptableObject
    {
        #region 字段 - 库基础信息
        /// <summary>
        /// 库名称
        /// </summary>
        public string library_name = "NewCurveLibrary";
        /// <summary>
        /// 曲线库数据列表
        /// </summary>
        public List<xHud_LibraryArg_Curve> library_curves = new List<xHud_LibraryArg_Curve>();
        #endregion

        #region 字段 - 事件回调
        /// <summary>
        /// 曲线变化时触发的事件
        /// </summary>
        public UnityAction act_on_curve_changed;
        /// <summary>
        /// 曲线被设置时触发的事件（名称 + 曲线）
        /// </summary>
        public UnityAction<string, AnimationCurve> act_on_curve_set;
        /// <summary>
        /// 曲线被添加时触发的事件（名称 + 曲线）
        /// </summary>
        public UnityAction<string, AnimationCurve> act_on_curve_added;
        /// <summary>
        /// 曲线被移除时触发的事件
        /// </summary>
        public UnityAction act_on_curve_removed;
        #endregion

        #region 字段 - 列表查询与筛选
        /// <summary>
        /// 名称项查找（精确匹配）
        /// </summary>
        public string find = "";
        /// <summary>
        /// 筛选高亮名称
        /// </summary>
        public string highlight;
        #endregion

        #region 字段 - 列表布局
        /// <summary>
        /// Editor 列表项高度
        /// </summary>
        public float item_height = 30;
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visible_item_count = 15;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 sound_infolist_original_scroller;
        #endregion

        #region 字段 - 选中与定位
        /// <summary>
        /// 定位选中项索引号（-1 表示无）
        /// </summary>
        public int location_selected_index = -1;
        /// <summary>
        /// 当前选中的索引号
        /// </summary>
        public int selected_index = 0;
        #endregion

        private void OnEnable()
        {
            item_height = 30;
            visible_item_count = 15;
        }

        // 确保内部名称与文件名一致
        private void OnValidate()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(name))
            {
                string path = AssetDatabase.GetAssetPath(this);
                if (!string.IsNullOrEmpty(path))
                {
                    name = Path.GetFileNameWithoutExtension(path);
                }
            }
#endif
        }

        /// <summary>
        /// 获取目标曲线
        /// </summary>
        /// <param name="name">目标曲线名称</param>
        /// <returns></returns>
        public AnimationCurve CurveLibrary_GetCurve(string name)
        {
            AnimationCurve curve = new AnimationCurve();
            for (int i = 0; i < library_curves.Count; i++)
            {
                if (library_curves[i].name == name)
                    curve = library_curves[i].curve;
            }
            return curve;
        }

        /// <summary>
        /// 获取目标曲线
        /// </summary>
        /// <param name="index">目标曲线序号</param>
        /// <returns></returns>
        public AnimationCurve CurveLibrary_GetCurve(int index)
        {
            AnimationCurve curve = new AnimationCurve();
            for (int i = 0; i < library_curves.Count; i++)
            {
                if (i == index)
                    curve = library_curves[i].curve;
            }
            return curve;
        }

        /// <summary>
        /// 设置目标曲线
        /// </summary>
        /// <param name="name"></param>
        /// <param name="curve"></param>
        public void CurveLibrary_SetCurve(string name, AnimationCurve curve)
        {
            for (int i = 0; i < library_curves.Count; i++)
            {
                if (library_curves[i].name == name)
                {
                    library_curves[i].curve = curve;
                    break;
                }
            }
            if (act_on_curve_set != null)
                act_on_curve_set(name, curve);
        }

        /// <summary>
        /// 新增目标曲线
        /// </summary>
        /// <param name="name">新增的曲线名称</param>
        /// <param name="curve">新增曲线</param>
        public void CurveLibrary_AddCurve(string name, AnimationCurve curve)
        {
            xHud_LibraryArg_Curve info = new xHud_LibraryArg_Curve(name, curve);
            library_curves.Add(info);
            if (act_on_curve_added != null)
                act_on_curve_added(name, curve);
        }

        /// <summary>
        /// 移除目标曲线
        /// </summary>
        /// <param name="name"></param>
        public void CurveLibrary_RemoveCurve(string name)
        {
            for (int i = 0; i < library_curves.Count; i++)
            {
                if (library_curves[i].name == name)
                    library_curves.RemoveAt(i);
            }
            if (act_on_curve_removed != null)
                act_on_curve_removed();
        }

        /// <summary>
        /// 获取曲线总数量
        /// </summary>
        /// <returns></returns>
        public int CurveLibrary_GetCurvesCount()
        {
            int count = library_curves.Count;

            return count;
        }

        /// <summary>
        /// 获取曲线名称清单
        /// </summary>
        /// <returns></returns>
        public string[] CurveLibrary_GetCurveNames()
        {
            string[] names = new string[library_curves.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = library_curves[i].name;
            }

            return names;
        }

        /// <summary>
        /// 获取目标曲线的索引号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int CurveLibrary_GetIndexWithName(string name)
        {
            int sw = 0;
            for (int i = 0; i < library_curves.Count; i++)
            {
                if (library_curves[i].name == name)
                {
                    sw = i;
                    break;
                }
            }
            return sw;
        }

        /// <summary>
        /// 获取曲线信息列表
        /// </summary>
        /// <returns></returns>
        public xHud_LibraryArg_Curve[] CurveLibrary_GetCurveInfos()
        {
            return library_curves.ToArray();
        }

        /// <summary>
        /// 曲线库是否是空的
        /// </summary>
        /// <returns></returns>
        public bool CurveLibrary_IsEmpty()
        {
            if (library_curves.Count > 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 曲线库ID是否是有效的
        /// </summary>
        /// <returns></returns>
        public bool CurvesLibrary_IndexIsValid(int Index)
        {
            if (Index >= 0 && Index < library_curves.Count)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 曲线库名称是否是有效的
        /// </summary>
        /// <returns></returns>
        public bool CurvesLibrary_NameIsValid(string name)
        {
            bool sw = false;
            for (int i = 0; i < library_curves.Count; i++)
            {
                if (library_curves[i].name == name)
                {
                    sw = true;
                }
            }

            return sw;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 定位
        /// </summary>
        /// <param name="name"></param>
        public void CurveLibrary_Location(string name)
        {
            int index = CurveLibrary_GetIndexWithName(name);
            location_selected_index = index;
            selected_index = index;
            highlight = name;

            //计算列表滚动值
            float scrollval = item_height;
            for (int i = 0; i < library_curves.Count; i++)
            {
                if (library_curves[i].name == name)
                {
                    if (i > visible_item_count / 2)
                        scrollval -= (visible_item_count / 2) * item_height;
                    else
                        scrollval -= item_height;
                    break;
                }
                else
                {
                    scrollval += item_height;
                }
            }
            Vector2 newscroll = sound_infolist_original_scroller;
            newscroll.y = scrollval;
            sound_infolist_original_scroller = newscroll;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 查找
        /// </summary>
        /// <param name="name"></param>
        public void CurveLibrary_Location_Find(string name)
        {
            int index = CurveLibrary_GetIndexWithName(name);
            location_selected_index = index;
            selected_index = index;

            //计算列表滚动值
            float scrollval = item_height;
            for (int i = 0; i < library_curves.Count; i++)
            {
                if (library_curves[i].name == name)
                {
                    if (i > visible_item_count / 2)
                        scrollval -= (visible_item_count / 2) * item_height;
                    else
                        scrollval -= item_height;
                    break;
                }
                else
                {
                    scrollval += item_height;
                }
            }
            Vector2 newscroll = sound_infolist_original_scroller;
            newscroll.y = scrollval;
            sound_infolist_original_scroller = newscroll;
        }
    }
}