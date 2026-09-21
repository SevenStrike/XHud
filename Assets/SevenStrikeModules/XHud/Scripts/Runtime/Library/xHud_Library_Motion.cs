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

    [System.Serializable]
    public class XHud_LibraryArg_Motion
    {
        /// <summary>
        /// 参数模版名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 参数模版模式
        /// 0=只有创建参数
        /// 1=只有回收参数
        /// </summary>
        public int Mode;
        /// <summary>
        /// 解释
        /// </summary>
        public string Des = "-";
        /// <summary>
        /// 参数模版 - 生成参数
        /// </summary>
        public Motion_Creator Crc = new Motion_Creator();
        /// <summary>
        /// 参数模版 - 回收参数
        /// </summary>
        public Motion_Recycler Rec = new Motion_Recycler();

        public XHud_LibraryArg_Motion()
        {

        }

        public XHud_LibraryArg_Motion(string name, int mode, string des, Motion_Creator crc, Motion_Recycler rec)
        {
            Name = name;
            Mode = mode;
            Des = des;
            Crc = crc;
            Rec = rec;
        }

        public XHud_LibraryArg_Motion(XHud_LibraryArg_Motion motion)
        {
            Name = motion.Name;
            Mode = motion.Mode;
            Des = motion.Des;

            // 实例化 CRC
            Motion_Creator crc = new Motion_Creator();
            crc.anchor = motion.Crc.anchor;
            crc.Movement = new MotionNode_Movement();
            crc.Movement.CopyData(motion.Crc.Movement);
            crc.Rotation = new MotionNode_Rotation();
            crc.Rotation.CopyData(motion.Crc.Rotation);
            crc.Alpha = new MotionNode_Alpha();
            crc.Alpha.CopyData(motion.Crc.Alpha);
            crc.MotionAnimateEndState = motion.Crc.MotionAnimateEndState;
            Crc = crc;

            // 实例化 REC
            Motion_Recycler rec = new Motion_Recycler();
            rec.Movement = new MotionNode_Movement();
            rec.Movement.CopyData(motion.Rec.Movement);
            rec.Rotation = new MotionNode_Rotation();
            rec.Rotation.CopyData(motion.Rec.Rotation);
            rec.Alpha = new MotionNode_Alpha();
            rec.Alpha.CopyData(motion.Rec.Alpha);
            rec.MotionAnimateEndState = motion.Rec.MotionAnimateEndState;
            Rec = rec;
        }
    }

    [CreateAssetMenu(fileName = "XHud_Library_ElementMotion", menuName = "XHud/CreateAssets (创建Hud资源库)/Library-ElementMotion (元素动效库)", order = 0)]
    public class XHud_Library_Motion : ScriptableObject
    {
        public string LibraryName = "NewMotionLibrary";

        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 80;

        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 6;

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
        public Vector2 ElementMotionList_Scroller;

        /// <summary>
        /// 元素参数模版列表
        /// </summary>
        public List<XHud_LibraryArg_Motion> ElementMotionList = new List<XHud_LibraryArg_Motion>();

        private void OnEnable()
        {
            itemHeight = 80;
            visibleItemCount = 6;
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

        public void ElementMotion_ReplaceMotion(int Index, XHud_LibraryArg_Motion motion)
        {
            ElementMotionList[Index] = motion;
        }

        /// <summary>
        /// 增加一套参数模版
        /// </summary>
        /// <param name="motion"></param>
        public void ElementMotion_Add(XHud_LibraryArg_Motion motion)
        {
            ElementMotionList.Add(motion);
        }

        /// <summary>
        /// 移除一套参数模版
        /// </summary>
        /// <param name="name"></param>
        public void ElementMotion_Remove(string name)
        {
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Name == name)
                    ElementMotionList.RemoveAt(i);
            }
        }

        /// <summary>
        /// 清空参数模版列表
        /// </summary>
        public void ElementMotion_RemoveAll()
        {
            ElementMotionList.Clear();
        }

        /// <summary>
        /// 判断目标名称是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ElementMotion_IsExist(string name)
        {
            bool sw = false;
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Name == name)
                {
                    sw = true;
                    break;
                }
            }
            if (sw)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断目标名称是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ElementMotion_IsExist(string name, HudElementMotionType type)
        {
            bool sw = false;
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Name == name && ElementMotionList[i].Mode == (int)type)
                {
                    sw = true;
                    break;
                }
            }
            if (sw)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取目标名称的参数模版
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XHud_LibraryArg_Motion ElementMotion_GetMotion(string name, HudElementMotionType type)
        {
            XHud_LibraryArg_Motion pi = null;
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Name == name && ElementMotionList[i].Mode == (int)type)
                {
                    pi = ElementMotionList[i];
                }
            }

            return pi;
        }

        /// <summary>
        /// 获取目标索引号的参数模版
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public XHud_LibraryArg_Motion ElementMotion_GetMotion(int index, HudElementMotionType type)
        {
            return ElementMotionList[index].Mode == (int)type ? ElementMotionList[index] : null;
        }

        /// <summary>
        /// 获取目标名称的参数模版索引号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int ElementMotion_GetIndexWithName(string name)
        {
            int pi = 0;
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Name == name)
                {
                    pi = i;
                }
            }

            return pi;
        }

        /// <summary>
        /// 获取目标索引号的参数模版名称
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string ElementMotion_GetNameWithIndex(int index)
        {
            return ElementMotionList[index].Name;
        }

        /// <summary>
        /// 获取参数模版的所有模版项
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XHud_LibraryArg_Motion[] ElementMotion_GetAllMotion()
        {
            return ElementMotionList.ToArray();
        }

        /// <summary>
        /// 获取参数模版的所有模版项名称
        /// </summary>
        /// <returns></returns>
        public string[] ElementMotion_GetAllMotionName()
        {
            string[] names = new string[ElementMotionList.Count];
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                names[i] = ElementMotionList[i].Name;
            }
            return names;
        }

        /// <summary>
        /// 获取参数模版的所有模版项名称
        /// </summary>
        /// <returns></returns>
        public string[] ElementMotion_GetAllName_With_Create()
        {
            List<string> names = new List<string>();
            names.Add("None");
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Mode == 0)
                    names.Add(ElementMotionList[i].Name);
            }
            return names.ToArray();
        }

        /// <summary>
        /// 获取参数模版的所有模版项名称
        /// </summary>
        /// <returns></returns>
        public string[] ElementMotion_GetAllName_With_Recycle()
        {
            List<string> names = new List<string>();
            names.Add("None");
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Mode == 1)
                    names.Add(ElementMotionList[i].Name);
            }
            return names.ToArray();
        }

        /// <summary>
        /// 获取参数模版中标为"生成类型"的所有参数集合体
        /// </summary>
        /// <returns></returns>
        public XHud_LibraryArg_Motion[] ElementMotion_GetAllMotion_With_Create()
        {
            List<XHud_LibraryArg_Motion> items = new List<XHud_LibraryArg_Motion>();
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Mode == 0)
                    items.Add(ElementMotionList[i]);
            }
            return items.ToArray();
        }

        /// <summary>
        /// 获取参数模版中标为"回收类型"的所有参数集合体
        /// </summary>
        /// <returns></returns>
        public XHud_LibraryArg_Motion[] ElementMotion_GetAllMotion_With_Recycle()
        {
            List<XHud_LibraryArg_Motion> items = new List<XHud_LibraryArg_Motion>();
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Mode == 1)
                    items.Add(ElementMotionList[i]);
            }
            return items.ToArray();
        }

        /// <summary>
        /// 获取参数模版中标为"生成类型"的所有参数
        /// </summary>
        /// <returns></returns>
        public Motion_Creator[] ElementMotion_GetElementCreator_At_Create()
        {
            List<Motion_Creator> items = new List<Motion_Creator>();
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Mode == 0)
                    items.Add(ElementMotionList[i].Crc);
            }
            return items.ToArray();
        }

        /// <summary>
        /// 获取参数模版中标为"生成类型"的目标名称参数
        /// </summary>
        /// <returns></returns>
        public Motion_Creator ElementMotion_GetElementCreator_At_Create(string name)
        {
            Motion_Creator items = new Motion_Creator();
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Mode == 0)
                {
                    if (ElementMotionList[i].Name == name)
                    {
                        items = ElementMotionList[i].Crc;
                    }
                }
            }
            return items;
        }

        /// <summary>
        /// 获取参数模版中标为"回收类型"的所有参数
        /// </summary>
        /// <returns></returns>
        public Motion_Recycler[] ElementMotion_GetElementCreator_At_Recycle()
        {
            List<Motion_Recycler> items = new List<Motion_Recycler>();
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Mode == 0)
                    items.Add(ElementMotionList[i].Rec);
            }
            return items.ToArray();
        }

        /// <summary>
        /// 获取参数模版中标为"回收类型"的目标名称参数
        /// </summary>
        /// <returns></returns>
        public Motion_Recycler ElementMotion_GetElementCreator_At_Recycle(string name)
        {
            Motion_Recycler items = new Motion_Recycler();
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Mode == 1)
                {
                    if (ElementMotionList[i].Name == name)
                    {
                        items = ElementMotionList[i].Rec;
                    }
                }
            }
            return items;
        }

        /// <summary>
        /// 参数库是否是空的
        /// </summary>
        /// <returns></returns>
        public bool ElementMotion_IsEmpty()
        {
            if (ElementMotionList.Count > 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 定位
        /// </summary>
        /// <param name="name"></param>
        public void ElementMotionLibrary_Location(string name)
        {
            int index = ElementMotion_GetIndexWithName(name);
            SelectedIndex = index;
            LocationSelectedIndex = index;
            Highlight = name;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Name == name)
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
            Vector2 newscroll = ElementMotionList_Scroller;
            newscroll.y = scrollval;
            ElementMotionList_Scroller = newscroll;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 查找
        /// </summary>
        /// <param name="name"></param>
        public void ElementMotionLibrary_Location_Find(string name)
        {
            int index = ElementMotion_GetIndexWithName(name);
            SelectedIndex = index;
            LocationSelectedIndex = index;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < ElementMotionList.Count; i++)
            {
                if (ElementMotionList[i].Name == name)
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
            Vector2 newscroll = ElementMotionList_Scroller;
            newscroll.y = scrollval;
            ElementMotionList_Scroller = newscroll;
        }
    }
}