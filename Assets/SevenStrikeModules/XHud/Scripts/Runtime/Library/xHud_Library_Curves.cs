namespace SevenStrikeModules.XHud
{
    using System.Collections;
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
        public string Name;
        public AnimationCurve Curve;

        public xHud_LibraryArg_Curve()
        {

        }

        /// <summary>
        /// 新增曲线信息
        /// </summary>
        /// <param tweenName="name">新增名称</param>
        /// <param tweenName="curve">新增曲线</param>
        public xHud_LibraryArg_Curve(string name = null, AnimationCurve curve = null)
        {
            Name = name;
            Curve = curve;
        }
    }

    [CreateAssetMenu(fileName = "XHud_Library_Curves", menuName = "XHud/CreateAssets (创建Hud资源库)/Library-Curve (曲线库)", order = 0)]
    public class xHud_Library_Curves : ScriptableObject
    {
        public string LibraryName = "NewCurveLibrary";
        public List<xHud_LibraryArg_Curve> CurveLibrary = new List<xHud_LibraryArg_Curve>();

        public UnityAction act_on_CurveChanged;
        public UnityAction<string, AnimationCurve> act_on_CurveSet;
        public UnityAction<string, AnimationCurve> act_on_CurveAdded;
        public UnityAction act_on_CurveRemoved;

        public bool UsePreviewAutoStop;
        public bool UsePreviewLooped;
        public float PreviewGridSize = 20;
        public float PreviewImageSize = 1;
        public float PreviewDuration = 1;
        public Color PreviewBGColor = new Color(0.1f, 0.1f, 0.1f);
        public Color PreviewGridColor = new Color(0.3f, 0.3f, 0.3f);
        public int ReferImgIndex;
        public int PreviewModeIndex;
        public int PreviewCurveIndex;
        public string PreviewCurveName;
        [SerializeField] public Texture2D SelectedReferImage;
        /// <summary>
        /// 当前选中的索引号
        /// </summary>
        public int SelectedIndex = 0;
        public string Highlight;
        /// <summary>
        /// 名称项查找（精确匹配）
        /// </summary>
        public string Find = "";
        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 26;
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 18;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 SoundInfoList_Original_Scroller;

        private void OnEnable()
        {
            itemHeight = 26;
            visibleItemCount = 18;
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
        /// <param tweenName="name">目标曲线名称</param>
        /// <returns></returns>
        public AnimationCurve CurveLibrary_GetCurve(string name)
        {
            AnimationCurve curve = new AnimationCurve();
            for (int i = 0; i < CurveLibrary.Count; i++)
            {
                if (CurveLibrary[i].Name == name)
                    curve = CurveLibrary[i].Curve;
            }
            return curve;
        }

        /// <summary>
        /// 获取目标曲线
        /// </summary>
        /// <param tweenName="index">目标曲线序号</param>
        /// <returns></returns>
        public AnimationCurve CurveLibrary_GetCurve(int index)
        {
            AnimationCurve curve = new AnimationCurve();
            for (int i = 0; i < CurveLibrary.Count; i++)
            {
                if (i == index)
                    curve = CurveLibrary[i].Curve;
            }
            return curve;
        }

        /// <summary>
        /// 设置目标曲线
        /// </summary>
        /// <param tweenName="name"></param>
        /// <param tweenName="curve"></param>
        public void CurveLibrary_SetCurve(string name, AnimationCurve curve)
        {
            for (int i = 0; i < CurveLibrary.Count; i++)
            {
                if (CurveLibrary[i].Name == name)
                {
                    CurveLibrary[i].Curve = curve;
                    break;
                }
            }
            if (act_on_CurveSet != null)
                act_on_CurveSet(name, curve);
        }

        /// <summary>
        /// 新增目标曲线
        /// </summary>
        /// <param tweenName="name">新增的曲线名称</param>
        /// <param tweenName="curve">新增曲线</param>
        public void CurveLibrary_AddCurve(string name, AnimationCurve curve)
        {
            xHud_LibraryArg_Curve info = new xHud_LibraryArg_Curve(name, curve);
            CurveLibrary.Add(info);
            if (act_on_CurveAdded != null)
                act_on_CurveAdded(name, curve);
        }

        /// <summary>
        /// 移除目标曲线
        /// </summary>
        /// <param tweenName="name"></param>
        public void CurveLibrary_RemoveCurve(string name)
        {
            for (int i = 0; i < CurveLibrary.Count; i++)
            {
                if (CurveLibrary[i].Name == name)
                    CurveLibrary.RemoveAt(i);
            }
            if (act_on_CurveRemoved != null)
                act_on_CurveRemoved();
        }

        /// <summary>
        /// 获取曲线总数量
        /// </summary>
        /// <returns></returns>
        public int CurveLibrary_GetCurvesCount()
        {
            int count = CurveLibrary.Count;

            return count;
        }

        /// <summary>
        /// 获取曲线名称清单
        /// </summary>
        /// <returns></returns>
        public string[] CurveLibrary_GetCurveNames()
        {
            string[] names = new string[CurveLibrary.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = CurveLibrary[i].Name;
            }

            return names;
        }

        /// <summary>
        /// 获取目标曲线的索引号
        /// </summary>
        /// <param tweenName="name"></param>
        /// <returns></returns>
        public int CurveLibrary_GetIndexWithName(string name)
        {
            int sw = 0;
            for (int i = 0; i < CurveLibrary.Count; i++)
            {
                if (CurveLibrary[i].Name == name)
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
            return CurveLibrary.ToArray();
        }

        /// <summary>
        /// 曲线库是否是空的
        /// </summary>
        /// <returns></returns>
        public bool CurveLibrary_IsEmpty()
        {
            if (CurveLibrary.Count > 0)
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
            if (Index >= 0 && Index < CurveLibrary.Count)
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
            for (int i = 0; i < CurveLibrary.Count; i++)
            {
                if (CurveLibrary[i].Name == name)
                {
                    sw = true;
                }
            }

            return sw;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 定位
        /// </summary>
        /// <param tweenName="name"></param>
        public void CurveLibrary_Location(string name)
        {
            int index = CurveLibrary_GetIndexWithName(name);
            SelectedIndex = index;
            Highlight = name;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < CurveLibrary.Count; i++)
            {
                if (CurveLibrary[i].Name == name)
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
            Vector2 newscroll = SoundInfoList_Original_Scroller;
            newscroll.y = scrollval;
            SoundInfoList_Original_Scroller = newscroll;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 查找
        /// </summary>
        /// <param tweenName="name"></param>
        public void CurveLibrary_Location_Find(string name)
        {
            int index = CurveLibrary_GetIndexWithName(name);
            SelectedIndex = index;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < CurveLibrary.Count; i++)
            {
                if (CurveLibrary[i].Name == name)
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
            Vector2 newscroll = SoundInfoList_Original_Scroller;
            newscroll.y = scrollval;
            SoundInfoList_Original_Scroller = newscroll;
        }
    }
}