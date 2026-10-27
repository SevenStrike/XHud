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

    [System.Serializable]
    /// <summary>
    /// 转场信息
    /// </summary>
    public class XHud_LibraryArg_Transition
    {
        public string name = "新转场";
        public string description = "描述转场效果的文字";
        public int total_frames_count;
        public int last_frame_index;
        public int skip_frame = 1;
        public Vector2Int res;
        public List<Texture2D> frames = new List<Texture2D>();

        public void CopyData(XHud_LibraryArg_Transition node)
        {
            name = node.name;
            description = node.description;
            total_frames_count = node.total_frames_count;
            last_frame_index = node.last_frame_index;
            skip_frame = node.skip_frame;
            res = node.res;
            for (int i = 0; i < node.frames.Count; i++)
            {
                frames.Add(node.frames[i]);
            }
        }
    }

    [CreateAssetMenu(fileName = "XHud_Library_Transition", menuName = "XHud/CreateAssets (创建Hud资源库)/Library-Transition (转场库)", order = 0)]
    public class XHud_Library_Transition : ScriptableObject
    {
        #region 字段 - 库基础信息
        /// <summary>
        /// 转场库名称
        /// </summary>
        public string library_name = "NewTransitionLibrary";
        /// <summary>
        /// 转场库数据列表
        /// </summary>
        public List<XHud_LibraryArg_Transition> library_transitions = new List<XHud_LibraryArg_Transition>();
        #endregion

        #region 字段 - 预览设置
        /// <summary>
        /// 预览缩放模式
        /// </summary>
        public ScaleMode preview_scale_mode = ScaleMode.ScaleToFit;
        #endregion

        #region 字段 - 列表查询与筛选
        /// <summary>
        /// 名称项查找（精确匹配）
        /// </summary>
        public string find = "";
        /// <summary>
        /// Editor 筛选名称
        /// </summary>
        public string highlight;
        #endregion

        #region 字段 - 列表布局
        /// <summary>
        /// Editor 列表项高度
        /// </summary>
        public float item_height = 75;
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visible_item_count = 6;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 transition_info_list_original_scroller;
        #endregion

        #region 字段 - 选中与定位
        /// <summary>
        /// 选中项索引号
        /// </summary>
        public int selected_index;
        /// <summary>
        /// 定位选中项索引号（-1 表示无）
        /// </summary>
        public int location_selected_index = -1;
        #endregion

        private void OnEnable()
        {
            item_height = 75;
            visible_item_count = 6;
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
        /// 添加一个转场资源
        /// </summary>
        /// <param name="name"></param>
        /// <param name="texs"></param>
        /// <param name="skipframe"></param>
        public void TransitionLibrary_Add(string name, Texture2D[] texs, int skipframe)
        {
            XHud_LibraryArg_Transition node = new XHud_LibraryArg_Transition();
            List<Texture2D> texlist = new List<Texture2D>();
            for (int i = 0; i < texs.Length; i++)
            {
                texlist.Add(texs[i]);
            }
            node.frames = texlist;
            node.name = name;
            node.skip_frame = skipframe;
            library_transitions.Add(node);
        }

        /// <summary>
        /// 清空转场资源
        /// </summary>
        public void TransitionLibrary_Clear()
        {
            library_transitions.Clear();
        }

        /// <summary>
        /// 移除一个转场资源
        /// </summary>
        /// <param name="name"></param>
        public void TransitionLibrary_Remove(string name)
        {
            for (int i = 0; i < library_transitions.Count; i++)
            {
                if (library_transitions[i].name == name)
                {
                    library_transitions.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 获取指定名称的转场资源名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XHud_LibraryArg_Transition TransitionLibrary_Get(string name)
        {
            XHud_LibraryArg_Transition node = new XHud_LibraryArg_Transition();
            for (int i = 0; i < library_transitions.Count; i++)
            {
                if (library_transitions[i].name == name)
                {
                    node = library_transitions[i];
                }
            }
            return node;
        }

        /// <summary>
        /// 通过指定的转场资源名称获取索引号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int TransitionLibrary_GetIndexWithName(string name)
        {
            int index = 0;
            for (int i = 0; i < library_transitions.Count; i++)
            {
                if (library_transitions[i].name == name)
                {
                    index = i;
                }
            }
            return index;
        }

        /// <summary>
        /// 获取转场库的所有资源名称
        /// </summary>
        /// <returns></returns>
        public string[] TransitionLibrary_GetAllNames()
        {
            string[] names = new string[library_transitions.Count];
            for (int i = 0; i < library_transitions.Count; i++)
            {
                names[i] = library_transitions[i].name;
            }
            return names;
        }

        /// <summary>
        /// 判断转场库是否为空
        /// </summary>
        /// <returns></returns>
        public bool TransitionLibrary_IsEmpty()
        {
            return library_transitions.Count == 0 ? true : false;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 定位
        /// </summary>
        /// <param name="name"></param>
        public void TransitionLibrary_Location(string name)
        {
            int index = TransitionLibrary_GetIndexWithName(name);
            selected_index = index;
            location_selected_index = index;
            highlight = name;

            //计算列表滚动值
            float scrollval = item_height;
            for (int i = 0; i < library_transitions.Count; i++)
            {
                if (library_transitions[i].name == name)
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
            Vector2 newscroll = transition_info_list_original_scroller;
            newscroll.y = scrollval;
            transition_info_list_original_scroller = newscroll;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 查找
        /// </summary>
        /// <param name="name"></param>
        public void TransitionLibrary_Location_Find(string name)
        {
            int index = TransitionLibrary_GetIndexWithName(name);
            selected_index = index;
            location_selected_index = index;

            //计算列表滚动值
            float scrollval = item_height;
            for (int i = 0; i < library_transitions.Count; i++)
            {
                if (library_transitions[i].name == name)
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
            Vector2 newscroll = transition_info_list_original_scroller;
            newscroll.y = scrollval;
            transition_info_list_original_scroller = newscroll;
        }

        /// <summary>
        /// 替换目标转场
        /// </summary>
        /// <param name="index">替换的转场索引号</param>
        /// <param name="node">替换转场</param>
        public void TransitionsLibrary_Replace(int index, XHud_LibraryArg_Transition node)
        {
            library_transitions[index] = node;
        }
    }
}