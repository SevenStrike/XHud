namespace SevenStrikeModules.XHud
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    [System.Serializable]
    /// <summary>
    /// Hud转场信息
    /// </summary>
    public class XHud_LibraryArg_Transition
    {
        public string Name = "新转场";
        public string Description = "描述转场效果的文字";
        public int TotalFramesCount;
        public int LastFrameIndex;
        public int SkipFrame = 1;
        public Vector2Int Res;
        public List<Texture2D> Frames = new List<Texture2D>();

        public void CopyData(XHud_LibraryArg_Transition node)
        {
            Name = node.Name;
            Description = node.Description;
            TotalFramesCount = node.TotalFramesCount;
            LastFrameIndex = node.LastFrameIndex;
            SkipFrame = node.SkipFrame;
            Res = node.Res;
            for (int i = 0; i < node.Frames.Count; i++)
            {
                Frames.Add(node.Frames[i]);
            }
        }
    }

    [CreateAssetMenu(fileName = "XHud_Library_Transition", menuName = "XHud/CreateAssets (创建Hud资源库)/Library-Transition (转场库)", order = 0)]
    public class XHud_Library_Transition : ScriptableObject
    {
        public string LibraryName = "NewTransitionLibrary";
        public ScaleMode PreviewScaleMode = ScaleMode.ScaleToFit;
        public float PreviewSpeed = 1;
        public List<XHud_LibraryArg_Transition> TransitionLibrary = new List<XHud_LibraryArg_Transition>();


        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 75;

        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 8;

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
        public Vector2 TransitionInfoList_Original_Scroller;

        private void OnEnable()
        {
            itemHeight = 75;
            visibleItemCount = 8;
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
        /// <param tweenName="name"></param>
        /// <param tweenName="texs"></param>
        /// <param tweenName="skipframe"></param>
        public void TransitionLibrary_Add(string name, Texture2D[] texs, int skipframe)
        {
            XHud_LibraryArg_Transition node = new XHud_LibraryArg_Transition();
            List<Texture2D> texlist = new List<Texture2D>();
            for (int i = 0; i < texs.Length; i++)
            {
                texlist.Add(texs[i]);
            }
            node.Frames = texlist;
            node.Name = name;
            node.SkipFrame = skipframe;
            TransitionLibrary.Add(node);
        }

        /// <summary>
        /// 清空转场资源
        /// </summary>
        public void TransitionLibrary_Clear()
        {
            TransitionLibrary.Clear();
        }

        /// <summary>
        /// 移除一个转场资源
        /// </summary>
        /// <param tweenName="name"></param>
        public void TransitionLibrary_Remove(string name)
        {
            for (int i = 0; i < TransitionLibrary.Count; i++)
            {
                if (TransitionLibrary[i].Name == name)
                {
                    TransitionLibrary.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 获取指定名称的转场资源名称
        /// </summary>
        /// <param tweenName="name"></param>
        /// <returns></returns>
        public XHud_LibraryArg_Transition TransitionLibrary_Get(string name)
        {
            XHud_LibraryArg_Transition node = new XHud_LibraryArg_Transition();
            for (int i = 0; i < TransitionLibrary.Count; i++)
            {
                if (TransitionLibrary[i].Name == name)
                {
                    node = TransitionLibrary[i];
                }
            }
            return node;
        }

        /// <summary>
        /// 通过指定的转场资源名称获取索引号
        /// </summary>
        /// <param tweenName="name"></param>
        /// <returns></returns>
        public int TransitionLibrary_GetIndexWithName(string name)
        {
            int index = 0;
            for (int i = 0; i < TransitionLibrary.Count; i++)
            {
                if (TransitionLibrary[i].Name == name)
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
            string[] names = new string[TransitionLibrary.Count];
            for (int i = 0; i < TransitionLibrary.Count; i++)
            {
                names[i] = TransitionLibrary[i].Name;
            }
            return names;
        }

        /// <summary>
        /// 判断转场库是否为空
        /// </summary>
        /// <returns></returns>
        public bool TransitionLibrary_IsEmpty()
        {
            return TransitionLibrary.Count == 0 ? true : false;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 定位
        /// </summary>
        /// <param tweenName="name"></param>
        public void TransitionLibrary_Location(string name)
        {
            int index = TransitionLibrary_GetIndexWithName(name);
            SelectedIndex = index;
            LocationSelectedIndex = index;
            Highlight = name;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < TransitionLibrary.Count; i++)
            {
                if (TransitionLibrary[i].Name == name)
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
            Vector2 newscroll = TransitionInfoList_Original_Scroller;
            newscroll.y = scrollval;
            TransitionInfoList_Original_Scroller = newscroll;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 查找
        /// </summary>
        /// <param tweenName="name"></param>
        public void TransitionLibrary_Location_Find(string name)
        {
            int index = TransitionLibrary_GetIndexWithName(name);
            SelectedIndex = index;
            LocationSelectedIndex = index;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < TransitionLibrary.Count; i++)
            {
                if (TransitionLibrary[i].Name == name)
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
            Vector2 newscroll = TransitionInfoList_Original_Scroller;
            newscroll.y = scrollval;
            TransitionInfoList_Original_Scroller = newscroll;
        }

        /// <summary>
        /// 替换目标转场
        /// </summary>
        /// <param tweenName="index">替换的转场索引号</param>
        /// <param tweenName="node">替换转场</param>
        public void TransitionsLibrary_Replace(int index, XHud_LibraryArg_Transition node)
        {
            TransitionLibrary[index] = node;
        }
    }
}