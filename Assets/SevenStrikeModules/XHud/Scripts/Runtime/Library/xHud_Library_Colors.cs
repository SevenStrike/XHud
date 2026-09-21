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
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;

    [System.Serializable]
    /// <summary>
    /// Hud配色信息
    /// </summary>
    public class xHud_LibraryArg_Color
    {
        public XHud_Module_Primitive_Painting Painting;
        public string Name;
        public Color Color;

        public xHud_LibraryArg_Color()
        {
        }

        /// <summary>
        /// 新增配色信息
        /// </summary>
        /// <param name="name">新增名称</param>
        /// <param name="color">新增配色</param>
        public xHud_LibraryArg_Color(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }

    public class TemplateColors
    {
        public string Name;
        public Color Color;
        public string Description;
    }

    [CreateAssetMenu(fileName = "XHud_Library_Colors", menuName = "XHud/CreateAssets (创建Hud资源库)/Library-Color (色卡库)", order = 0)]
    public class XHud_Library_Colors : ScriptableObject
    {
        public string LibraryName = "NewColorsLibrary";

        public List<xHud_LibraryArg_Color> ColorLibrary = new List<xHud_LibraryArg_Color>();

        public UnityAction act_on_ColorChanged;

        /// <summary>
        /// 名称项查找（精确匹配）
        /// </summary>
        public string Find = "";
        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 30;
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 15;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 ColorInfoList_Original_Scroller;
        /// <summary>
        /// Editor筛选名称
        /// </summary>
        public string Highlight;
        /// <summary>
        /// 定位选中项索引号
        /// </summary>
        public int LocationSelectedIndex = -1;
        /// <summary>
        /// 选中项索引号
        /// </summary>
        public int SelectedIndex;

        private void OnEnable()
        {
            itemHeight = 30;
            visibleItemCount = 15;
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

        #region 辅助
        /// <summary>
        /// 获取目标配色
        /// </summary>
        /// <param name="name">配色名称</param>
        /// <returns></returns>
        public Color ColorsLibrary_GetColor(string name)
        {
            Color color = new Color();
            for (int x = 0; x < ColorLibrary.Count; x++)
            {
                if (ColorLibrary[x].Name == name)
                {
                    color = ColorLibrary[x].Color;
                }
            }
            return color;
        }

        /// <summary>
        /// 根据Index获取目标配色名称
        /// </summary>
        /// <param name="index">配色Index</param>
        /// <returns></returns>
        public string ColorsLibrary_GetColorName(int index)
        {
            if (ColorLibrary.Count <= 0)
                return null;
            return ColorLibrary[index].Name;
        }

        /// <summary>
        /// 根据颜色名称获取目标配色的Index
        /// </summary>
        /// <param name="name">配色名称</param>
        /// <returns></returns>
        public int ColorsLibrary_GetColorIndex(string name)
        {
            int index = 0;
            for (int i = 0; i < ColorLibrary.Count; i++)
            {
                if (ColorLibrary[i].Name == name)
                    index = i;
            }
            return index;
        }

        /// <summary>
        /// 获取目标配色
        /// </summary>
        /// <param name="id">配色ID</param>
        /// <returns></returns>
        public Color ColorsLibrary_GetColor(int id)
        {
            Color color = new Color();
            for (int x = 0; x < ColorLibrary.Count; x++)
            {
                if (x == id)
                {
                    color = ColorLibrary[x].Color;
                    break;
                }
            }
            return color;
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="name"></param>
        /// <param name="color"></param>
        public void ColorsLibrary_SetColor(string name, Color color)
        {
            for (int x = 0; x < ColorLibrary.Count; x++)
            {
                if (ColorLibrary[x].Name == name)
                {
                    ColorLibrary[x].Color = color;
                    if (act_on_ColorChanged != null)
                        act_on_ColorChanged();
                }
            }
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="color"></param>
        public void ColorsLibrary_SetColor(int id, Color color)
        {
            ColorLibrary[id].Color = color;
            if (act_on_ColorChanged != null)
                act_on_ColorChanged();
        }

        /// <summary>
        /// 新增目标配色
        /// </summary>
        /// <param name="name">新增的配色名称</param>
        /// <param name="color">新增配色</param>
        public void ColorsLibrary_AddColor(string name, Color color)
        {
            bool sw = false;
            for (int i = 0; i < ColorLibrary.Count; i++)
            {
                if (ColorLibrary[i].Name == name)
                {
                    sw = true;
                    break;
                }
            }
            if (!sw)
                ColorLibrary.Add(new xHud_LibraryArg_Color(name, color));
        }

        /// <summary>
        /// 新增目标配色
        /// </summary>
        /// <param name="info">新增的配色信息</param>
        public void ColorsLibrary_AddColor(xHud_LibraryArg_Color info)
        {
            bool sw = false;
            for (int i = 0; i < ColorLibrary.Count; i++)
            {
                if (ColorLibrary[i].Name == name)
                {
                    sw = true;
                    break;
                }
            }
            if (!sw)
                ColorLibrary.Add(new xHud_LibraryArg_Color(info.Name, info.Color));
        }

        /// <summary>
        /// 替换目标配色
        /// </summary>
        /// <param name="name">替换的配色名称</param>
        /// <param name="color">替换配色</param>
        public void ColorsLibrary_ReplaceColor(string name, Color color)
        {
            for (int i = 0; i < ColorLibrary.Count; i++)
            {
                if (ColorLibrary[i].Name == name)
                {
                    ColorLibrary[i].Name = name;
                    ColorLibrary[i].Color = color;
                    break;
                }
            }
        }

        /// <summary>
        /// 替换目标配色
        /// </summary>
        /// <param name="name">替换的配色名称</param>
        /// <param name="color">替换配色</param>
        public void ColorsLibrary_ReplaceColorAndName(string originname, string name, Color color)
        {
            for (int i = 0; i < ColorLibrary.Count; i++)
            {
                if (ColorLibrary[i].Name == originname)
                {
                    ColorLibrary[i].Name = name;
                    ColorLibrary[i].Color = color;
                    break;
                }
            }
        }

        /// <summary>
        /// 替换目标配色
        /// </summary>
        /// <param name="name">替换的配色名称</param>
        /// <param name="color">替换配色</param>
        public void ColorsLibrary_ReplaceColorAndName(int index, string originname, string name, Color color)
        {
            ColorLibrary[index].Name = name;
            ColorLibrary[index].Color = color;
        }

        /// <summary>
        /// 获取配色总数量
        /// </summary>
        /// <returns></returns>
        public int ColorsLibrary_GetColorCount()
        {
            int count = ColorLibrary.Count;

            return count;
        }

        /// <summary>
        /// 获取配色名称清单
        /// </summary>
        /// <returns></returns>
        public string[] ColorsLibrary_GetColorNames()
        {
            string[] names = new string[ColorLibrary.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = ColorLibrary[i].Name;
            }

            return names;
        }

        /// <summary>
        /// 获取配色列表
        /// </summary>
        /// <returns></returns>
        public xHud_LibraryArg_Color[] ColorsLibrary_GetColorLibrary()
        {
            return ColorLibrary.ToArray();
        }

        /// <summary>
        /// 颜色库是否是空的
        /// </summary>
        /// <returns></returns>
        public bool ColorsLibrary_IsEmpty()
        {
            if (ColorLibrary.Count > 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 颜色库ID是否是有效的
        /// </summary>
        /// <returns></returns>
        public bool ColorsLibrary_IndexIsValid(int Index)
        {
            if (Index >= 0 && Index < ColorLibrary.Count)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 检查是否已存在该名称的颜色
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ColorsLibrary_IsExist(string name)
        {
            if (ColorLibrary.Count <= 0)
                return false;

            bool sw = false;
            for (int i = 0; i < ColorLibrary.Count; i++)
            {
                if (ColorLibrary[i].Name == name)
                {
                    sw = true;
                    break;
                }
            }
            return sw;
        }

        /// <summary>
        /// 检查是否已存在该名称的颜色
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int ColorsLibrary_CheckNameToIndex(string name)
        {
            int sw = 0;
            for (int i = 0; i < ColorLibrary.Count; i++)
            {
                if (ColorLibrary[i].Name == name)
                {
                    sw = i;
                    break;
                }
            }
            return sw;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 定位
        /// </summary>
        /// <param name="name"></param>
        public void ColorsLibrary_Location(string name)
        {
            int index = ColorsLibrary_GetColorIndex(name);
            SelectedIndex = index;
            LocationSelectedIndex = index;
            Highlight = name;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < ColorLibrary.Count; i++)
            {
                if (ColorLibrary[i].Name == name)
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
            Vector2 newscroll = ColorInfoList_Original_Scroller;
            newscroll.y = scrollval;
            ColorInfoList_Original_Scroller = newscroll;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 查找
        /// </summary>
        /// <param name="name"></param>
        public void ColorsLibrary_Location_Find(string name)
        {
            int index = ColorsLibrary_GetColorIndex(name);
            SelectedIndex = index;
            LocationSelectedIndex = index;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < ColorLibrary.Count; i++)
            {
                if (ColorLibrary[i].Name == name)
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
            Vector2 newscroll = ColorInfoList_Original_Scroller;
            newscroll.y = scrollval;
            ColorInfoList_Original_Scroller = newscroll;
        }
        #endregion

        #region 生成色卡
        /// <summary>
        /// 生成色卡 - 常规
        /// </summary>
        public void ColorsLibrary_CreateColors_BaseColor(string res_m)
        {
            if (res_m == "替换")
                ColorLibrary.Clear();

            #region 模拟颜色数据
            TemplateColors[] templateColors = new TemplateColors[20];

            string[] word_art = new string[20] { "玛瑙红(Agate Red)", "翡翠绿(Jade Green)", "夜幕蓝(Nightshade Blue)", "琥珀黄(Amber Yellow)", "玫瑰粉(Rose Pink)", "紫水晶紫(Amethyst Purple)", "钛白(Titanium White)", "炭黑(Charcoal Black)", "橄榄绿(Olive Green)", "珊瑚橙(Coral Orange)", "钻石蓝(Diamond Blue)", "茶色(Tea Brown)", "珍珠灰(Pearl Grey)", "翠鸟蓝(Kingfisher Blue)", "玫瑰金(Rose Gold)", "紫罗兰(Violet)", "雪松绿(Cedar Green)", "烟熏紫(Smoky Purple)", "钛金(Titanium Gold)", "雾霾蓝(Haze Blue)" };

            string[] text_art = new string[20] { "玛瑙红带有一种深沉而神秘的气质，常用于表现强烈的感情或重要时刻。", "翡翠绿带有高贵和清新的感觉，常用于表现自然之美或平静的氛围。", "夜幕蓝带有夜晚的宁静与深邃，常用于表现沉思或梦幻的场景。", "琥珀黄带有温暖和复古的感觉，常用于表现温馨或怀旧的氛围。", "瑰粉带有女性的柔美和甜蜜，常用于表现浪漫的爱情或温馨的场景。", "紫水晶紫带有高贵和神秘的气质，常用于表现深邃的情感或超脱的意境。", "钛白是一种纯净而明亮的白色，常用于表现简洁、高雅或神圣的氛围。", "炭黑带有一种质朴和深邃的感觉，常用于表现强烈对比或深沉的情感。", "橄榄绿有一种质朴和稳重的感觉，常用于表现自然景观或宁静的氛围。", "珊瑚橙带有海洋的清新感，常用于表现活力四溢的场景或温暖的情感。", "钻石蓝有一种璀璨和深邃的气质，常用于表现奢华或梦幻的场景。", "茶色带有温暖和质朴的感觉，常用于表现自然景观或温馨的氛围。", "珍珠灰带有一种柔和的光泽，常用于表现低调的奢华或平衡的氛围。", "翠鸟蓝有一种自然的活力，常用于表现生动的场景或清新的氛围。", "玫瑰金有一种柔和的光泽和温暖的色调，常用于表现高贵的氛围或浪漫的情感。", "紫罗兰有一种柔和而深邃的气质，常用于表现梦幻或优雅的场景。", "雪松绿有一种质朴的感觉，常用于表现自然景观或宁静的氛围。", "烟熏紫有一种朦胧的质感，常用于表现深邃的情感或梦幻的场景。", "钛金有一种明亮的光泽和温暖的色调，常用于表现现代艺术或奢华的氛围。", "雾霾蓝有一种柔和的质感，常用于表现梦幻或宁静的场景" };

            Color[] colors = new Color[20] { new Color(1.0f, 0.0f, 0.2f), new Color(0.0f, 0.5f, 0.2f), new Color(0.1f, 0.12f, 0.16f), new Color(1.0f, 0.75f, 0.0f), new Color(1.0f, 0.5f, 0.5f), new Color(0.6f, 0.4f, 0.8f), new Color(1.0f, 1.0f, 1.0f), new Color(0.0f, 0.0f, 0.0f), new Color(0.3f, 0.4f, 0.0f), new Color(1.0f, 0.5f, 0.3f), new Color(0.0f, 0.5f, 1.0f), new Color(0.5f, 0.4f, 0.3f), new Color(0.8f, 0.8f, 0.8f), new Color(0.0f, 0.7f, 0.9f), new Color(0.9f, 0.7f, 0.6f), new Color(0.9f, 0.5f, 0.9f), new Color(0.3f, 0.5f, 0.3f), new Color(0.4f, 0.3f, 0.5f), new Color(0.9f, 0.8f, 0.6f), new Color(0.6f, 0.7f, 0.8f), };
            #endregion

            for (int i = 0; i < templateColors.Length; i++)
            {
                templateColors[i] = new TemplateColors();
                templateColors[i].Name = word_art[i];
                templateColors[i].Color = colors[i];
                templateColors[i].Description = text_art[i];
                ColorLibrary.Add(new XHud.xHud_LibraryArg_Color(templateColors[i].Name, templateColors[i].Color));
            }

            if (act_on_ColorChanged != null)
                act_on_ColorChanged();
        }

        /// <summary>
        /// 生成色卡 - 高级灰
        /// </summary>
        public void ColorsLibrary_CreateColors_AdvancedGrayColor(string res_m)
        {
            if (res_m == "替换")
                ColorLibrary.Clear();

            #region 模拟颜色数据
            TemplateColors[] templateColors = new TemplateColors[76];

            string[] word_art = new string[76] { "晨雾玫瑰", "柔砂粉", "赤陶红", "轻纱粉", "梦幻粉", "玫瑰木", "覆盆子", "薰衣草雾", "珊瑚粉", "梅子紫", "珍珠白", "砖瓦红", "紫丁香", "天青蓝", "晨露蓝", "深海蓝", "薄雾紫", "浅水蓝", "湖光蓝", "银雾灰", "晴空蓝", "浪花蓝", "雾蓝", "宝石蓝", "云纱白", "浅湾蓝", "湖蓝", "石板灰", "薄暮紫", "午夜蓝", "碧空蓝", "沙米色", "奶油黄", "亚麻灰", "红土棕", "蜂蜜黄", "枫糖棕", "焦糖棕", "琥珀棕", "麦芽黄", "米灰", "陶橙", "稻壳黄", "陶土橙", "金秋黄", "碧波绿", "薄荷雾", "翡翠雾", "湖水绿", "嫩芽绿", "青柠雾", "晨露绿", "海藻绿", "苔原绿", "雾松绿", "溪水绿", "青瓷绿", "晨雾紫", "薰衣草灰", "紫藤雾", "雾蓝灰", "极光蓝", "淡紫纱", "晨雾蓝", "樱花紫", "梦幻紫", "紫藤纱", "晨露紫", "珍珠紫", "雾紫灰", "淡紫雾", "薄雾蓝", "霞光粉", "晨露紫", "晨雾绿", "暮光紫" };

            string[] colors = new string[76] { "E9D2CC", "E9D2CC", "D1807C", "EFE6E9", "FBCEEF", "C97D97", "CD5D85", "E3B8D4", "E2929D", "B14371", "F2E2E5", "A8585A", "C991AA", "ABC9EB", "AAC6D1", "316692", "D4D2E7", "7BAFC6", "8BA3C7", "D5D3E0", "8DB1E3", "7696C7", "B3C2D7", "5977BA", "DDE1EC", "7FA6C5", "4384A6", "BFC2C9", "7E87AE", "11507B", "7EB4DA", "D7D0BE", "FEE3C5", "CCBFAF", "A3776A", "EFCEAB", "C29484", "D3AB89", "C4967C", "DEBA99", "E5D8C8", "E0A784", "EBD8B7", "E0BCA6", "D2A46D", "ABC7BB", "7DA493", "B6CCB5", "517865", "D8ECE3", "94D4BC", "C7ECCD", "AFBF98", "7F9F6E", "CAEBE2", "7ABF91", "BBDAB9", "78B7A2", "698D52", "AECFC6", "778B76", "AFDBC2", "86CE9E", "778A70", "B7A6DD", "8B84AD", "9EA9DF", "DBAFD0", "D8BCE3", "8A73BB", "D4D2E7", "B782B0", "747DBC", "B8A8C2", "CEC9E8", "B99BBF" };
            #endregion

            for (int i = 0; i < templateColors.Length; i++)
            {
                templateColors[i] = new TemplateColors();
                templateColors[i].Name = word_art[i];
                templateColors[i].Color = XGUI_Utilitys.HexString_To_Color(colors[i]);
                templateColors[i].Description = colors[i];
                ColorLibrary.Add(new XHud.xHud_LibraryArg_Color(templateColors[i].Name, templateColors[i].Color));
            }

            if (act_on_ColorChanged != null)
                act_on_ColorChanged();
        }

        /// <summary>
        /// 生成色卡 - 冷暖交替
        /// </summary>
        public void ColorsLibrary_CreateColors_General(string res_m)
        {
            if (res_m == "替换")
                ColorLibrary.Clear();

            #region 模拟颜色数据
            TemplateColors[] templateColors = new TemplateColors[77];

            string[] word_art = new string[77] { "冰雾蓝", "纯白", "象牙白", "珍珠灰", "石板灰", "烟熏灰", "玫瑰灰", "银雾蓝", "钢灰", "云杉灰", "暗灰", "炭黑", "深灰", "夜黑", "宇宙黑", "铁灰蓝", "深海蓝", "暗夜蓝", "午夜蓝", "矿蓝", "天青蓝", "晴空蓝", "暗海军蓝", "亮钴蓝", "宝石蓝", "深靛蓝", "蔚蓝", "暗蓝", "银灰", "灰蓝", "暗钢蓝", "浅湖蓝", "深海蓝", "暗夜蓝", "极夜黑", "青灰", "苔藓绿", "橄榄灰", "暗森林绿", "灰绿", "薄荷灰", "松针绿", "暗苔绿", "湖水绿", "翡翠绿", "嫩草绿", "碧绿", "孔雀绿", "森林绿", "荧光绿", "柠檬黄绿", "淡黄绿", "奶油黄", "淡草绿", "卡其灰", "香草黄", "杏黄", "驼色", "沙棕", "灰棕", "深咖啡", "红棕", "浅棕", "金丝雀黄", "金黄", "柠檬黄", "橙黄", "淡金黄", "暗紫黑", "深紫红", "玫红", "淡玫红", "紫罗兰", "粉黛紫", "玫瑰紫", "番茄红", "烈焰红" };

            string[] colors = new string[77] { "F1F4F9", "FFFFFF", "F9F8E6", "EAE6DB", "B9B9B1", "B4ABA6", "DED4D3", "ACB0BB", "777E86", "8D989E", "696969", "323232", "333333", "191919", "121317", "555D6A", "333951", "3B3F4A", "293649", "4A5E83", "7898CB", "7EB8EA", "232E40", "5AB2FF", "2E5CEF", "1E3352", "036FED", "203962", "9EA2AB", "4F6271", "283043", "39A5CC", "0C5580", "192636", "131522", "4A6167", "567D7A", "616B63", "1E3137", "788B89", "94A094", "41543E", "292E28", "356F70", "5D9F83", "4AAB76", "1C9478", "02807F", "417F40", "64D346", "B8C84E", "D9E484", "FEFDA1", "E3EDB1", "A19C89", "F4E3AB", "F0BF84", "937F64", "C99661", "776F62", "41342C", "B05E2C", "C5A89A", "ECCC5F", "E1B030", "F7D708", "E38D1E", "F2E76B", "23212E", "3D1C37", "AB3764", "E56F9D", "743EC8", "AA98DB", "B480C8", "E14E46", "FF8336" };
            #endregion

            for (int i = 0; i < templateColors.Length; i++)
            {
                templateColors[i] = new TemplateColors();
                templateColors[i].Name = word_art[i];
                templateColors[i].Color = XGUI_Utilitys.HexString_To_Color(colors[i]);
                templateColors[i].Description = colors[i];
                ColorLibrary.Add(new XHud.xHud_LibraryArg_Color(templateColors[i].Name, templateColors[i].Color));
            }

            if (act_on_ColorChanged != null)
                act_on_ColorChanged();
        }
        #endregion
    }
}