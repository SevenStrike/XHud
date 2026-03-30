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
        public bool Using;
        /// <summary>
        /// 预生成的元素
        /// </summary>
        public XHud_Module_Element HudElement;

        public XHud_LibraryArg_Element_Info()
        {

        }

        /// <summary>
        /// 更新使用状态
        /// </summary>
        public void UpdateState()
        {
            if (HudElement == null)
                return;

            if (HudElement.CreateState == HudElementCreateState.Recycled)
            {
                Using = false;
            }
            else
            {
                Using = true;
            }
        }

        public void Copy(XHud_LibraryArg_Element_Info ele)
        {
            Using = ele.Using;
            HudElement = ele.HudElement;
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
        public string Name;
        /// <summary>
        /// 需要预生成的数量
        /// </summary>
        public int InitializeCount;
        /// <summary>
        /// 已使用的数量
        /// </summary>
        public int UsedCount;
        /// <summary>
        /// 已回收的数量
        /// </summary>
        public int RecycledCount;
        /// <summary>
        /// 序号
        /// </summary>
        public int NextIndex;
        /// <summary>
        /// 根节点
        /// </summary>
        public Transform Root;
        /// <summary>
        /// 元素
        /// </summary>
        public XHud_Module_Element Target;
        [SerializeField]
        /// <summary>
        /// 预生成的元素信息列表
        /// </summary>
        public List<XHud_LibraryArg_Element_Info> PreloadElements = new List<XHud_LibraryArg_Element_Info>();

        public XHud_LibraryArg_Element_Item()
        {

        }

        public XHud_LibraryArg_Element_Item(int initializeCount, XHud_Module_Element target)
        {
            Name = target.name;
            InitializeCount = initializeCount;
            UsedCount = 0;
            RecycledCount = 0;
            NextIndex = 0;
            Root = null;
            Target = target;
            PreloadElements = new List<XHud_LibraryArg_Element_Info>();
        }



        /// <summary>
        /// 更新已使用的数量
        /// </summary>
        public void GetUsedCount()
        {
            int count = 0;
            for (int i = 0; i < PreloadElements.Count; i++)
            {
                if (PreloadElements[i].Using)
                {
                    count++;
                }
            }
            UsedCount = count;
        }

        /// <summary>
        /// 更新未使用的数量
        /// </summary>
        public void GetRecycledCount()
        {
            int count = 0;
            for (int i = 0; i < PreloadElements.Count; i++)
            {
                if (!PreloadElements[i].Using)
                {
                    count++;
                }
            }
            RecycledCount = count;
        }

        public void Copy(XHud_LibraryArg_Element_Item item)
        {
            Name = item.Name;
            InitializeCount = item.InitializeCount;
            UsedCount = item.UsedCount;
            RecycledCount = item.RecycledCount;
            NextIndex = item.NextIndex;
            Root = item.Root;
            Target = item.Target;
            PreloadElements = item.PreloadElements;
        }
    }

    [CreateAssetMenu(fileName = "XHud_Library_Element", menuName = "XHud/CreateAssets (创建Hud资源库)/Library-Element (元素库)", order = 0)]
    public class XHud_Library_Element : ScriptableObject
    {
        public string LibraryName = "NewElementsLibrary";
        public string LibraryDescription = "NewDescription";

        public Transform LibraryRoot;
        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 50;
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 10;
        /// <summary>
        /// 名称项查找（精确匹配）
        /// </summary>
        public string Find = "";
        /// <summary>
        /// Editor筛选名称
        /// </summary>
        public string Highlight;
        /// <summary>
        /// 选中项索引号
        /// </summary>
        public int SelectedIndex;
        /// <summary>
        /// 定位选中项索引号
        /// </summary>
        public int LocationSelectedIndex = -1;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 ElementInfoList_Original_Scroller;

        public List<XHud_LibraryArg_Element_Item> ElementLibrary = new List<XHud_LibraryArg_Element_Item>();

        private void OnEnable()
        {
            itemHeight = 50;
            visibleItemCount = 10;
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
        /// <param tweenName="index"></param>
        /// <param tweenName="paramObject"></param>
        public void ElementsLibrary_ReplaceElement(XHud_Module_Element paramObject)
        {
            for (int i = 0; i < ElementLibrary.Count; i++)
            {
                if (ElementLibrary[i].Name == paramObject.transform.name)
                {
                    ElementLibrary[i].Target = paramObject;
                    ElementLibrary[i].Name = paramObject.transform.name;
                }
            }
        }

        /// <summary>
        /// 替换元素
        /// </summary>
        /// <param tweenName="index"></param>
        /// <param tweenName="paramObject"></param>
        public void ElementsLibrary_ReplaceElement(int index, XHud_LibraryArg_Element_Item item)
        {
            ElementLibrary[index] = item;
        }

        /// <summary>
        /// 增加一个元素项
        /// </summary>
        /// <param tweenName="element">元素</param>
        /// <param tweenName="count">数量</param>
        public void ElementsLibrary_Add(XHud_Module_Element element, int count)
        {
            XHud_LibraryArg_Element_Item item = new XHud_LibraryArg_Element_Item();
            item.Target = element;
            item.InitializeCount = count;
            item.Name = element.transform.name;
            ElementLibrary.Add(item);
        }

        /// <summary>
        /// 增加一个元素项
        /// </summary>
        /// <param tweenName="item">元素项</param>
        /// <param tweenName="count">数量</param>
        public void ElementsLibrary_Add(XHud_LibraryArg_Element_Item item)
        {
            ElementLibrary.Add(item);
        }

        /// <summary>
        /// 移除指定索引的元素
        /// </summary>
        /// <param tweenName="index">索引</param>
        public void ElementsLibrary_Remove(int index)
        {
            ElementLibrary.RemoveAt(index);
        }

        /// <summary>
        /// 修改库名称
        /// </summary>
        /// <param tweenName="name">目标名称</param>
        public void ElementLibrary_ChangeLibraryName(string name)
        {
            LibraryName = name;
        }

        /// <summary>
        /// 获取所有元素名称
        /// </summary>
        /// <returns>元素名称列表</returns>
        public string[] ElementsLibrary_GetAllElementsNames()
        {
            string[] names = new string[ElementLibrary.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = ElementLibrary[i].Name;
            }

            return names;
        }

        /// <summary>
        /// 获取所有元素数量
        /// </summary>
        /// <returns>所有元素数量</returns>
        public int ElementsLibrary_GetElementsCount()
        {
            return ElementLibrary.Count;
        }

        /// <summary>
        /// 获取所有正在使用的元素数量
        /// </summary>
        /// <returns>所有正在使用的元素数量</returns>
        public int ElementsLibrary_GetElementsUsedCount()
        {
            int count = 0;

            for (int i = 0; i < ElementLibrary.Count; i++)
            {
                count += ElementLibrary[i].UsedCount;
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

            for (int i = 0; i < ElementLibrary.Count; i++)
            {
                count += ElementLibrary[i].InitializeCount;
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

            for (int i = 0; i < ElementLibrary.Count; i++)
            {
                count += ElementLibrary[i].RecycledCount;
            }

            return count;
        }

        /// <summary>
        /// 获取指定标识名称的元素
        /// </summary>
        /// <param tweenName="name">指定的标识名称</param>
        /// <returns>元素</returns>
        public XHud_Module_Element ElementsLibrary_GetTargetElement(string name)
        {
            XHud_Module_Element ele = null;
            for (int i = 0; i < ElementLibrary.Count; i++)
            {
                if (ElementLibrary[i].Name == name)
                    ele = ElementLibrary[i].Target;
            }

            return ele;
        }

        /// <summary>
        /// 获取指定名称的元素项
        /// </summary>
        /// <param tweenName="name">目标名称</param>
        /// <returns>元素</returns>
        public XHud_LibraryArg_Element_Item ElementsLibrary_GetTargetLibraryItem(string name)
        {
            XHud_LibraryArg_Element_Item item = null;
            for (int i = 0; i < ElementLibrary.Count; i++)
            {
                if (ElementLibrary[i].Name == name)
                    item = ElementLibrary[i];
            }

            return item;
        }

        /// <summary>
        /// 获取指定标识名称的元素的初始化数量
        /// </summary>
        /// <param tweenName="Indicator">指定的标识名称</param>
        /// <param tweenName="Count">初始化数量</param>
        public void ElementsLibrary_ChangeElementInitializeCount(string Indicator, int Count)
        {
            for (int i = 0; i < ElementLibrary.Count; i++)
            {
                if (ElementLibrary[i].Name == Indicator)
                    ElementLibrary[i].InitializeCount = Count;
            }
        }

        /// <summary>
        /// 回收所有元素
        /// </summary>
        public void ElementLibrary_RecycleAll()
        {
            for (int i = 0; i < ElementLibrary.Count; i++)
            {
                XHud_LibraryArg_Element_Item item = ElementLibrary[i];

                for (int s = 0; s < item.PreloadElements.Count; s++)
                {
                    XHud_Module_Element ele = item.PreloadElements[s].HudElement;

                    if (ele.CreateState == HudElementCreateState.Recycled)
                    {
                        continue;
                    }
                    else
                    {
                        ele.element_Reset();
                        ele.transform.SetParent(ElementLibrary[i].Root);
                        ele.gameObject.SetActive(false);
                    }
                }
                item.NextIndex = 0;
            }
        }

        /// <summary>
        /// 元素库是否是空的
        /// </summary>
        /// <returns></returns>
        public bool ElementLibrary_IsEmpty()
        {
            if (ElementLibrary.Count > 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 已存在
        /// </summary>
        /// <param tweenName="name"></param>
        /// <returns></returns>
        public bool ElementLibrary_IsExist(string name)
        {
            bool isexist = false;
            for (int i = 0; i < ElementLibrary.Count; i++)
            {
                if (ElementLibrary[i].Name == name)
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
        /// <param tweenName="name"></param>
        /// <returns></returns>
        public int ElementLibrary_GetIndexWithName(string name)
        {
            int isexist = 0;
            for (int i = 0; i < ElementLibrary.Count; i++)
            {
                if (ElementLibrary[i].Name == name)
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
            if (Index >= 0 && Index < ElementLibrary.Count)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 定位
        /// </summary>
        /// <param tweenName="name"></param>
        public void ElementLibrary_Location(string name)
        {
            int index = ElementLibrary_GetIndexWithName(name);
            SelectedIndex = index;
            LocationSelectedIndex = index;
            Highlight = name;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < ElementLibrary.Count; i++)
            {
                if (ElementLibrary[i].Name == name)
                {
                    if (i > visibleItemCount / 2)
                        scrollval -= (visibleItemCount / 2) * itemHeight;
                    else
                        scrollval -= itemHeight;
                    break;
                }
                else
                {
                    scrollval += itemHeight;
                }
            }
            Vector2 newscroll = ElementInfoList_Original_Scroller;
            newscroll.y = scrollval;
            ElementInfoList_Original_Scroller = newscroll;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 查找
        /// </summary>
        /// <param tweenName="name"></param>
        public void ElementLibrary_Location_Find(string name)
        {
            int index = ElementLibrary_GetIndexWithName(name);
            SelectedIndex = index;
            LocationSelectedIndex = index;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < ElementLibrary.Count; i++)
            {
                if (ElementLibrary[i].Name == name)
                {
                    if (i > visibleItemCount / 2)
                        scrollval -= (visibleItemCount / 2) * itemHeight;
                    else
                        scrollval -= itemHeight;
                    break;
                }
                else
                {
                    scrollval += itemHeight;
                }
            }
            Vector2 newscroll = ElementInfoList_Original_Scroller;
            newscroll.y = scrollval;
            ElementInfoList_Original_Scroller = newscroll;
        }
    }
}