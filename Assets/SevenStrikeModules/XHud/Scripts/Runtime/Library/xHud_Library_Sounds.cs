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
    /// Hud配色信息
    /// </summary>
    public class XHud_LibraryArg_Sound
    {
        public string Name;
        public string Format;
        public AudioClip Clip;
        public int Frequency;
        public int Channel;
        public float Length;

        public XHud_LibraryArg_Sound()
        {
        }

        /// <summary>
        /// 新增配色信息
        /// </summary>
        /// <param name="name">新增名称</param>
        /// <param name="clip">新增配色</param>
        public XHud_LibraryArg_Sound(string name, AudioClip clip, string format)
        {
            Name = name;
            Clip = clip;
            Frequency = clip.frequency;
            Channel = clip.channels;
            Length = clip.length;
            Format = format;
        }
    }

    [CreateAssetMenu(fileName = "XHud_Library_Sounds", menuName = "XHud/CreateAssets (创建Hud资源库)/Library-Sound (音效库)", order = 0)]
    public class XHud_Library_Sounds : ScriptableObject
    {
        /// <summary>
        /// 音效库名称
        /// </summary>
        public string LibraryName = "NewSoundLibrary";
        /// <summary>
        /// 随机音高开关
        /// </summary>
        public bool UseRandomPitch = false;
        /// <summary>
        ///  最小音高
        /// </summary>
        public float Pitch_Min = 1;
        /// <summary>
        ///  最大音高
        /// </summary>
        public float Pitch_Max = 1;


        /// <summary>
        /// 名称项查找（精确匹配）
        /// </summary>
        public string Find = "";

        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 50;

        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 8;

        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 SoundInfoList_Original_Scroller;


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

        public List<XHud_LibraryArg_Sound> SoundLibrary = new List<XHud_LibraryArg_Sound>();
        public UnityAction act_on_SoundChanged;
        public UnityAction<string, AudioClip> act_on_SoundAdded;
        public UnityAction act_on_SoundRemoved;

        private void OnEnable()
        {
            itemHeight = 50;
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
        /// 获取目标音效
        /// </summary>        
        /// <param name="name">目标音效名称</param>
        /// <returns></returns>
        public XHud_LibraryArg_Sound SoundLibrary_GetSoundInfo(string name)
        {
            XHud_LibraryArg_Sound info = null;
            for (int x = 0; x < SoundLibrary.Count; x++)
            {
                if (SoundLibrary[x].Name == name)
                {
                    info = SoundLibrary[x];
                }
            }
            return info;
        }

        /// <summary>
        /// 获取目标音效
        /// </summary>        
        /// <param name="name">目标音效名称</param>
        /// <returns></returns>
        public AudioClip SoundLibrary_GetSound(string name)
        {
            AudioClip color = null;
            for (int x = 0; x < SoundLibrary.Count; x++)
            {
                if (SoundLibrary[x].Name == name)
                {
                    color = SoundLibrary[x].Clip;
                }
            }
            return color;
        }

        /// <summary>
        /// 获取目标音效
        /// </summary>        
        /// <param name="index">目标音效序号</param>
        /// <returns></returns>
        public AudioClip SoundLibrary_GetSound(int index)
        {
            return SoundLibrary[index].Clip;
        }

        /// <summary>
        /// 获取目标音效参数
        /// </summary>        
        /// <param name="index">目标音效序号</param>
        /// <returns></returns>
        public XHud_LibraryArg_Sound SoundLibrary_GetSoundArg(int index)
        {
            return SoundLibrary[index];
        }

        /// <summary>
        /// 新增音效
        /// </summary>
        /// <param name="name">新增的配色名称</param>
        /// <param name="sound">新增配色</param>
        public void SoundLibrary_AddSound(string name, AudioClip sound, string format)
        {
            bool sw = false;
            for (int i = 0; i < SoundLibrary.Count; i++)
            {
                if (SoundLibrary[i].Name == name)
                {
                    sw = true;
                    break;
                }
            }
            if (!sw)
            {
                SoundLibrary.Add(new XHud_LibraryArg_Sound(name, sound, format));
            }
            if (act_on_SoundAdded != null)
                act_on_SoundAdded(name, sound);
        }

        /// <summary>
        /// 移除音效
        /// </summary>
        /// <param name="name">移除的音效名称</param>
        public void SoundLibrary_RemoveSound(string name)
        {
            for (int i = 0; i < SoundLibrary.Count; i++)
            {
                if (SoundLibrary[i].Name == name)
                {
                    SoundLibrary.RemoveAt(i);

                    break;
                }
            }
            if (act_on_SoundRemoved != null)
                act_on_SoundRemoved();
        }

        /// <summary>
        /// 获取音效总数量
        /// </summary>
        /// <returns></returns>
        public int SoundLibrary_GetSoundCount()
        {
            int count = SoundLibrary.Count;

            return count;
        }

        /// <summary>
        /// 获取音效总数量 - 根据赫兹
        /// </summary>
        /// <returns></returns>
        public int SoundLibrary_GetSoundCount_WithFrequency(int Frequency)
        {
            int count = 0;
            for (int i = 0; i < SoundLibrary.Count; i++)
            {
                if (SoundLibrary[i].Clip.frequency == Frequency)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取音效总数量 - 根据长度
        /// </summary>
        /// <returns></returns>
        public int SoundLibrary_GetSoundCount_WithLength(int Length)
        {
            int count = 0;
            for (int i = 0; i < SoundLibrary.Count; i++)
            {
                if (SoundLibrary[i].Clip.length == Length)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取音效总数量 - 根据采样
        /// </summary>
        /// <returns></returns>
        public int SoundLibrary_GetSoundCount_WithSample(int Sample)
        {
            int count = 0;
            for (int i = 0; i < SoundLibrary.Count; i++)
            {
                if (SoundLibrary[i].Clip.samples == Sample)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取音效总数量 - 根据通道
        /// </summary>
        /// <returns></returns>
        public int SoundLibrary_GetSoundCount_WithChannels(int Channels)
        {
            int count = 0;
            for (int i = 0; i < SoundLibrary.Count; i++)
            {
                if (SoundLibrary[i].Clip.channels == Channels)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取音效名称清单
        /// </summary>
        /// <returns></returns>
        public string[] SoundLibrary_GetSoundNames()
        {
            string[] names = new string[SoundLibrary.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = SoundLibrary[i].Name;
            }

            return names;
        }

        /// <summary>
        /// 获取音效列表
        /// </summary>
        /// <returns></returns>
        public XHud_LibraryArg_Sound[] SoundLibrary_GetSoundLibrary()
        {
            return SoundLibrary.ToArray();
        }

        /// <summary>
        /// 音效库是否是空的
        /// </summary>
        /// <returns></returns>
        public bool SoundLibrary_IsEmpty()
        {
            if (SoundLibrary.Count > 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 音效库ID是否是有效的
        /// </summary>
        /// <returns></returns>
        public bool SoundLibrary_IndexIsValid(int Index)
        {
            if (Index >= 0 && Index < SoundLibrary.Count)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取目标声音的索引号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int SoundLibrary_GetIndexWithName(string name)
        {
            int sw = 0;
            for (int i = 0; i < SoundLibrary.Count; i++)
            {
                if (SoundLibrary[i].Name == name)
                {
                    sw = i;
                    break;
                }
            }
            return sw;
        }

        /// <summary>
        /// 检查是否已存在该名称的声音
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool SoundLibrary_NameIsValid(string name)
        {
            bool sw = false;
            for (int i = 0; i < SoundLibrary.Count; i++)
            {
                if (SoundLibrary[i].Name == name)
                {
                    sw = true;
                    break;
                }
            }
            return sw;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 定位
        /// </summary>
        /// <param name="name"></param>
        public void SoundLibrary_Location(string name)
        {
            int index = SoundLibrary_GetIndexWithName(name);
            SelectedIndex = index;
            LocationSelectedIndex = index;
            Highlight = name;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < SoundLibrary.Count; i++)
            {
                if (SoundLibrary[i].Name == name)
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
        /// <param name="name"></param>
        public void SoundLibrary_Location_Find(string name)
        {
            int index = SoundLibrary_GetIndexWithName(name);
            SelectedIndex = index;
            LocationSelectedIndex = index;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < SoundLibrary.Count; i++)
            {
                if (SoundLibrary[i].Name == name)
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