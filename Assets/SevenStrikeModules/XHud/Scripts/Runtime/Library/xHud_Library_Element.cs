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
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 预制体信息结构
    /// </summary>
    [System.Serializable]
    public class XHud_LibraryArg_Element_Info
    {
        /// <summary>
        /// 是否正在使用
        /// </summary>
        public bool is_using;
        /// <summary>
        /// 预生成的元素
        /// </summary>
        public XHud_Module_Element element;

        public XHud_LibraryArg_Element_Info()
        {

        }

        /// <summary>
        /// 更新使用状态
        /// </summary>
        public void UpdateState()
        {
            if (element == null)
                return;

            if (element.create_state == XHudElementCreateState.Recycled)
            {
                is_using = false;
            }
            else
            {
                is_using = true;
            }
        }

        public void Copy(XHud_LibraryArg_Element_Info ele)
        {
            is_using = ele.is_using;
            element = ele.element;
        }
    }

    /// <summary>
    /// 预制体列表
    /// </summary>
    [System.Serializable]
    public class XHud_LibraryArg_Element_Item
    {
        /// <summary>
        /// 标识
        /// </summary>
        public string name;
        /// <summary>
        /// 需要预生成的数量
        /// </summary>
        public int initialize_count;
        /// <summary>
        /// 已使用的数量
        /// </summary>
        public int used_count;
        /// <summary>
        /// 已回收的数量
        /// </summary>
        public int recycled_count;
        /// <summary>
        /// 序号
        /// </summary>
        public int next_index;
        /// <summary>
        /// 根节点
        /// </summary>
        public Transform root;
        /// <summary>
        /// 元素
        /// </summary>
        public XHud_Module_Element target;
        [SerializeField]
        /// <summary>
        /// 预生成的元素信息列表
        /// </summary>
        public List<XHud_LibraryArg_Element_Info> preload_elements = new List<XHud_LibraryArg_Element_Info>();

        public XHud_LibraryArg_Element_Item()
        {

        }

        public XHud_LibraryArg_Element_Item(int initializeCount, XHud_Module_Element target)
        {
            name = target.name;
            initialize_count = initializeCount;
            used_count = 0;
            recycled_count = 0;
            next_index = 0;
            root = null;
            this.target = target;
            preload_elements = new List<XHud_LibraryArg_Element_Info>();
        }



        /// <summary>
        /// 更新已使用的数量
        /// </summary>
        public void GetUsedCount()
        {
            int count = 0;
            for (int i = 0; i < preload_elements.Count; i++)
            {
                if (preload_elements[i].is_using)
                {
                    count++;
                }
            }
            used_count = count;
        }

        /// <summary>
        /// 更新未使用的数量
        /// </summary>
        public void GetRecycledCount()
        {
            int count = 0;
            for (int i = 0; i < preload_elements.Count; i++)
            {
                if (!preload_elements[i].is_using)
                {
                    count++;
                }
            }
            recycled_count = count;
        }

        public void Copy(XHud_LibraryArg_Element_Item item)
        {
            name = item.name;
            initialize_count = item.initialize_count;
            used_count = item.used_count;
            recycled_count = item.recycled_count;
            next_index = item.next_index;
            root = item.root;
            target = item.target;
            preload_elements = item.preload_elements;
        }
    }

    [CreateAssetMenu(fileName = "XHud_Library_Element", menuName = "XHud/CreateAssets (创建Hud资源库)/Library-Element (元素库)", order = 0)]
    public class XHud_Library_Element : ScriptableObject
    {
        #region 字段 - 库基础信息
        /// <summary>
        /// 库名称
        /// </summary>
        public string library_name = "NewElementsLibrary";
        /// <summary>
        /// 库根节点
        /// </summary>
        public Transform library_root;
        /// <summary>
        /// 元素库数据列表
        /// </summary>
        public List<XHud_LibraryArg_Element_Item> library_elements = new List<XHud_LibraryArg_Element_Item>();
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
        public float item_height = 50;
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visible_item_count = 9;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 element_info_list_original_scroller;
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
            item_height = 50;
            visible_item_count = 9;
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
        /// 替换元素
        /// </summary>
        /// <param name="index"></param>
        /// <param name="paramObject"></param>
        public void ElementsLibrary_ReplaceElement(XHud_Module_Element paramObject)
        {
            for (int i = 0; i < library_elements.Count; i++)
            {
                if (library_elements[i].name == paramObject.transform.name)
                {
                    library_elements[i].target = paramObject;
                    library_elements[i].name = paramObject.transform.name;
                }
            }
        }

        /// <summary>
        /// 替换元素
        /// </summary>
        /// <param name="index"></param>
        /// <param name="paramObject"></param>
        public void ElementsLibrary_ReplaceElement(int index, XHud_LibraryArg_Element_Item item)
        {
            library_elements[index] = item;
        }

        /// <summary>
        /// 增加一个元素项
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="count">数量</param>
        public void ElementsLibrary_Add(XHud_Module_Element element, int count)
        {
            XHud_LibraryArg_Element_Item item = new XHud_LibraryArg_Element_Item();
            item.target = element;
            item.initialize_count = count;
            item.name = element.transform.name;
            library_elements.Add(item);
        }

        /// <summary>
        /// 增加一个元素项
        /// </summary>
        /// <param name="item">元素项</param>
        /// <param name="count">数量</param>
        public void ElementsLibrary_Add(XHud_LibraryArg_Element_Item item)
        {
            library_elements.Add(item);
        }

        /// <summary>
        /// 移除指定索引的元素
        /// </summary>
        /// <param name="index">索引</param>
        public void ElementsLibrary_Remove(int index)
        {
            library_elements.RemoveAt(index);
        }

        /// <summary>
        /// 修改库名称
        /// </summary>
        /// <param name="name">目标名称</param>
        public void ElementLibrary_ChangeLibraryName(string name)
        {
            library_name = name;
        }

        /// <summary>
        /// 获取所有元素名称
        /// </summary>
        /// <returns>元素名称列表</returns>
        public string[] ElementsLibrary_GetAllElementsNames()
        {
            string[] names = new string[library_elements.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = library_elements[i].name;
            }

            return names;
        }

        /// <summary>
        /// 获取所有元素数量
        /// </summary>
        /// <returns>所有元素数量</returns>
        public int ElementsLibrary_GetElementsCount()
        {
            return library_elements.Count;
        }

        /// <summary>
        /// 获取所有正在使用的元素数量
        /// </summary>
        /// <returns>所有正在使用的元素数量</returns>
        public int ElementsLibrary_GetElementsUsedCount()
        {
            int count = 0;

            for (int i = 0; i < library_elements.Count; i++)
            {
                count += library_elements[i].used_count;
            }

            return count;
        }

        /// <summary>
        /// 获取所有初始化的元素数量
        /// </summary>
        /// <returns>所有初始化的元素数量</returns>
        public int ElementsLibrary_GetElementsInitiatedCount()
        {
            int count = 0;

            for (int i = 0; i < library_elements.Count; i++)
            {
                count += library_elements[i].initialize_count;
            }

            return count;
        }

        /// <summary>
        /// 获取所有已回收的元素数量
        /// </summary>
        /// <returns>所有已回收的元素数量</returns>
        public int ElementsLibrary_GetElementsRecycledCount()
        {
            int count = 0;

            for (int i = 0; i < library_elements.Count; i++)
            {
                count += library_elements[i].recycled_count;
            }

            return count;
        }

        /// <summary>
        /// 获取指定标识名称的元素
        /// </summary>
        /// <param name="name">指定的标识名称</param>
        /// <returns>元素</returns>
        public XHud_Module_Element ElementsLibrary_GetTargetElement(string name)
        {
            XHud_Module_Element ele = null;
            for (int i = 0; i < library_elements.Count; i++)
            {
                if (library_elements[i].name == name)
                    ele = library_elements[i].target;
            }

            return ele;
        }

        /// <summary>
        /// 获取指定名称的元素项
        /// </summary>
        /// <param name="name">目标名称</param>
        /// <returns>元素</returns>
        public XHud_LibraryArg_Element_Item ElementsLibrary_GetTargetLibraryItem(string name)
        {
            XHud_LibraryArg_Element_Item item = null;
            for (int i = 0; i < library_elements.Count; i++)
            {
                if (library_elements[i].name == name)
                    item = library_elements[i];
            }

            return item;
        }

        /// <summary>
        /// 获取指定标识名称的元素的初始化数量
        /// </summary>
        /// <param name="Indicator">指定的标识名称</param>
        /// <param name="Count">初始化数量</param>
        public void ElementsLibrary_ChangeElementInitializeCount(string Indicator, int Count)
        {
            for (int i = 0; i < library_elements.Count; i++)
            {
                if (library_elements[i].name == Indicator)
                    library_elements[i].initialize_count = Count;
            }
        }

        /// <summary>
        /// 回收所有元素
        /// </summary>
        public void ElementLibrary_RecycleAll()
        {
            for (int i = 0; i < library_elements.Count; i++)
            {
                XHud_LibraryArg_Element_Item item = library_elements[i];

                for (int s = 0; s < item.preload_elements.Count; s++)
                {
                    XHud_Module_Element ele = item.preload_elements[s].element;

                    if (ele.create_state == XHudElementCreateState.Recycled)
                    {
                        continue;
                    }
                    else
                    {
                        ele.element_Reset();
                        ele.transform.SetParent(library_elements[i].root);
                        ele.gameObject.SetActive(false);
                    }
                }
                item.next_index = 0;
            }
        }

        /// <summary>
        /// 元素库是否是空的
        /// </summary>
        /// <returns></returns>
        public bool ElementLibrary_IsEmpty()
        {
            if (library_elements.Count > 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 已存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ElementLibrary_IsExist(string name)
        {
            bool isexist = false;
            for (int i = 0; i < library_elements.Count; i++)
            {
                if (library_elements[i].name == name)
                {
                    isexist = true;
                    break;
                }
            }
            return isexist;
        }

        /// <summary>
        /// 根据名称获取索引号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int ElementLibrary_GetIndexWithName(string name)
        {
            int isexist = 0;
            for (int i = 0; i < library_elements.Count; i++)
            {
                if (library_elements[i].name == name)
                {
                    isexist = i;
                    break;
                }
            }
            return isexist;
        }

        /// <summary>
        /// 元素库ID是否是有效的
        /// </summary>
        /// <returns></returns>
        public bool ElementLibrary_IndexIsValid(int Index)
        {
            if (Index >= 0 && Index < library_elements.Count)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 定位
        /// </summary>
        /// <param name="name"></param>
        public void ElementLibrary_Location(string name)
        {
            int index = ElementLibrary_GetIndexWithName(name);
            selected_index = index;
            location_selected_index = index;
            highlight = name;

            //计算列表滚动值
            float scrollval = item_height;
            for (int i = 0; i < library_elements.Count; i++)
            {
                if (library_elements[i].name == name)
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
            Vector2 newscroll = element_info_list_original_scroller;
            newscroll.y = scrollval;
            element_info_list_original_scroller = newscroll;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 查找
        /// </summary>
        /// <param name="name"></param>
        public void ElementLibrary_Location_Find(string name)
        {
            int index = ElementLibrary_GetIndexWithName(name);
            selected_index = index;
            location_selected_index = index;

            //计算列表滚动值
            float scrollval = item_height;
            for (int i = 0; i < library_elements.Count; i++)
            {
                if (library_elements[i].name == name)
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
            Vector2 newscroll = element_info_list_original_scroller;
            newscroll.y = scrollval;
            element_info_list_original_scroller = newscroll;
        }
    }
}