/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGUI - Unity Editor界面可视化组件工具
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
namespace SevenStrikeModules.XGUI.Runtime
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;

    /// <summary>
    /// 性别枚举
    /// </summary>
    public enum Gender
    {
        Male,      // 男性
        Female,    // 女性
        Neutral    // 中性
    }

    #region 自定义类
    [System.Serializable]
    public class PlayerData
    {
        public string _name;
        public string _level;
        public string _weapon;
        public string _health;
        public string _region;
        public string _ammo;
        public string _cdpercent;
        public string _avatarpath;

        public PlayerData(string name, string level, string weapon, string health, string region, string ammo, string cdpercent, string avatarpath)
        {
            _name = name;
            _level = level;
            _weapon = weapon;
            _health = health;
            _region = region;
            _ammo = ammo;
            _cdpercent = cdpercent;
            _avatarpath = avatarpath;
        }
    }

    [System.Serializable]
    public class CardData
    {
        [SerializeField]
        public string _title;
        [SerializeField]
        public object _data;
        [SerializeField]
        public Color _bgcolor;
    }

    [System.Serializable]
    public class CardDataWrap
    {
        [SerializeField]
        public CardData[] _datas;
        [SerializeField]
        public int index;
    }

    public class NameGenerator
    {
        // 百家姓（常用姓氏，选取前100个）
        private static readonly string[] Surnames = new string[]
        {
        "赵", "钱", "孙", "李", "周", "吴", "郑", "王", "冯", "陈",
        "褚", "卫", "蒋", "沈", "韩", "杨", "朱", "秦", "尤", "许",
        "何", "吕", "施", "张", "孔", "曹", "严", "华", "金", "魏",
        "陶", "姜", "戚", "谢", "邹", "喻", "柏", "水", "窦", "章",
        "云", "苏", "潘", "葛", "奚", "范", "彭", "郎", "鲁", "韦",
        "昌", "马", "苗", "凤", "花", "方", "俞", "任", "袁", "柳",
        "酆", "鲍", "史", "唐", "费", "廉", "岑", "薛", "雷", "贺",
        "倪", "汤", "滕", "殷", "罗", "毕", "郝", "邬", "安", "常",
        "乐", "于", "时", "傅", "皮", "卞", "齐", "康", "伍", "余",
        "元", "卜", "顾", "孟", "平", "黄", "和", "穆", "萧", "尹"
        };

        // 男性名字常用字
        private static readonly string[] MaleGivenNames = new string[]
        {
        "伟", "强", "磊", "洋", "勇", "军", "杰", "涛", "明", "超",
        "辉", "刚", "毅", "俊", "峰", "飞", "浩", "然", "博", "文",
        "宇", "轩", "泽", "瑞", "晨", "曦", "鹏", "程", "万里", "志",
        "国", "安", "宏", "建", "海", "天", "龙", "云", "风", "林",
        "山", "川", "岳", "恒", "远", "达", "光", "华", "永", "胜"
        };

        // 女性名字常用字
        private static readonly string[] FemaleGivenNames = new string[]
        {
        "芳", "敏", "静", "丽", "婷", "雪", "莲", "梅", "兰", "竹",
        "菊", "萍", "波", "洁", "莹", "瑶", "琼", "琳", "玲", "莉",
        "娜", "莎", "欣", "怡", "悦", "慧", "智", "雅", "涵", "蕾",
        "蕊", "薇", "萱", "宁", "欣", "然", "彤", "妍", "媛", "璇",
        "琦", "瑶", "瑾", "瑜", "琪", "琳", "珊", "瑚", "珍珠", "如意"
        };

        // 中性名字用字（男女通用）
        private static readonly string[] NeutralGivenNames = new string[]
        {
        "一", "凡", "子", "小", "云", "天", "心", "文", "方", "日",
        "月", "水", "火", "玉", "生", "白", "立", "光", "同", "成",
        "曲", "羽", "辰", "初", "灵", "君", "妙", "彤", "书", "梦"
        };

        private static readonly System.Random random = new System.Random();

        /// <summary>
        /// 生成随机角色名称（默认性别随机）
        /// </summary>
        /// <returns>完整的角色名称</returns>
        public static string GenerateRandomName()
        {
            Gender gender = (Gender)random.Next(0, 3); // 0=男, 1=女, 2=中性
            return GenerateName(gender);
        }

        /// <summary>
        /// 生成指定性别的角色名称
        /// </summary>
        /// <param name="gender">性别</param>
        /// <returns>完整的角色名称</returns>
        public static string GenerateName(Gender gender)
        {
            string surname = GetRandomSurname();
            string givenName = GetRandomGivenName(gender);
            return surname + givenName;
        }

        /// <summary>
        /// 生成指定性别的角色名称（可指定名字长度）
        /// </summary>
        /// <param name="gender">性别</param>
        /// <param name="nameLength">名字长度：1或2</param>
        /// <returns>完整的角色名称</returns>
        public static string GenerateName(Gender gender, int nameLength)
        {
            if (nameLength < 1) nameLength = 1;
            if (nameLength > 2) nameLength = 2;

            string surname = GetRandomSurname();
            string givenName = "";

            for (int i = 0; i < nameLength; i++)
            {
                givenName += GetRandomGivenName(gender);
            }

            return surname + givenName;
        }

        /// <summary>
        /// 批量生成角色名称
        /// </summary>
        /// <param name="count">生成数量</param>
        /// <returns>名称列表</returns>
        public static List<string> GenerateNames(int count)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < count; i++)
            {
                names.Add(GenerateRandomName());
            }
            return names;
        }

        /// <summary>
        /// 批量生成指定性别的角色名称
        /// </summary>
        /// <param name="count">生成数量</param>
        /// <param name="gender">性别</param>
        /// <returns>名称列表</returns>
        public static List<string> GenerateNames(int count, Gender gender)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < count; i++)
            {
                names.Add(GenerateName(gender));
            }
            return names;
        }

        /// <summary>
        /// 获取随机姓氏
        /// </summary>
        private static string GetRandomSurname()
        {
            return Surnames[random.Next(Surnames.Length)];
        }

        /// <summary>
        /// 获取随机名字
        /// </summary>
        private static string GetRandomGivenName(Gender gender)
        {
            switch (gender)
            {
                case Gender.Male:
                    return MaleGivenNames[random.Next(MaleGivenNames.Length)];
                case Gender.Female:
                    return FemaleGivenNames[random.Next(FemaleGivenNames.Length)];
                case Gender.Neutral:
                default:
                    return NeutralGivenNames[random.Next(NeutralGivenNames.Length)];
            }
        }

        /// <summary>
        /// 检查姓氏是否在百家姓中
        /// </summary>
        public static bool IsValidSurname(string surname)
        {
            return Array.Exists(Surnames, s => s == surname);
        }

        /// <summary>
        /// 获取所有姓氏
        /// </summary>
        public static string[] GetAllSurnames()
        {
            return (string[])Surnames.Clone();
        }

        /// <summary>
        /// 获取指定性别的名字用字
        /// </summary>
        public static string[] GetGivenNames(Gender gender)
        {
            switch (gender)
            {
                case Gender.Male:
                    return (string[])MaleGivenNames.Clone();
                case Gender.Female:
                    return (string[])FemaleGivenNames.Clone();
                default:
                    return (string[])NeutralGivenNames.Clone();
            }
        }
    }
    #endregion

    [CreateAssetMenu(fileName = "XGUISamples", menuName = "XGUI/CreateSamples")]
    public class XGUI_Samples : ScriptableObject
    {
        #region 主题色
        public Color ThemeColor_Primary = XGUI_Utilitys.HexString_To_Color("#3BFD9A");
        public Color ThemeColor_Secondary = XGUI_Utilitys.HexString_To_Color("#FF9358");
        public Color ThemeColor_Group = Color.black;
        #endregion

        #region 按钮状态
        public bool playing;
        public bool recycled;
        #endregion

        #region 图片
        public string Hardware = "极限配置主机";
        public string Stones = "RutilatedQuartz";
        #endregion

        #region 状态
        public string Types;
        public string EaseMode;
        #endregion

        #region 参数
        public string param_string = "";
        public float param_float = 6.27f;
        public int param_int = 90;
        public Color param_color = Color.white;
        public Vector2 param_vector2 = new Vector2(1, 1);
        public Vector3 param_vector3 = new Vector3(10, 25, 5);
        public Vector4 param_vector4 = new Vector4(120, 3.5f, 12.8f, 70);
        public RectOffset param_rectoffset;
        public GameObject param_gameobject;
        public AnimationCurve param_curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        #endregion

        #region 滑动条
        public float param_slider_min = 20;
        public float param_slider_max = 100;
        public float param_slider_hz = 6000;
        public float param_slider_volume;
        public float param_slider_distance;
        public float param_slider_manual;
        public xgui_minmax_value param_slider_manual_val;
        public float param_slider_manual_min = 30;
        public float param_slider_manual_max = 85;
        #endregion

        #region 开关
        public bool state_toggle;
        public bool state_toggle_A = true;
        public bool state_toggle_B = true;
        public bool state_toggle_C = false;
        public bool state_toggle_D = true;
        public bool state_toggle_E = false;
        #endregion

        #region 输入框值
        public string field_string = "默认文本";
        public float field_float = 1.0f;
        public int field_int = 10;
        public Color field_color = Color.white;
        public AnimationCurve field_curve = AnimationCurve.Linear(0, 0, 1, 1);
        public Vector2 field_vector2 = Vector2.one;
        public Vector3 field_vector3 = Vector3.zero;
        public Vector4 field_vector4 = Vector4.zero;
        public Quaternion field_quaternion = Quaternion.identity;
        #endregion

        #region 曲线
        public AnimationCurve gui_curve_A = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve gui_curve_B = AnimationCurve.Linear(0, 0, 1, 1);
        #endregion

        #region 工具栏
        public int toolbar_index = 0;
        public int toolbar_manual_index = 0;
        #endregion

        #region 卡片数据
        [SerializeField]
        public CardDataWrap CardDataWrap_Edge;
        [SerializeField]
        public CardDataWrap CardDataWrap_Solid;
        [SerializeField]
        public CardDataWrap CardDataWrap_Solid_Manual;
        #endregion

        #region 折叠
        public bool fold_param_layout;
        public bool fold_param_gui;
        public bool fold_tweenstate_gui;
        public bool fold_pops_layout;
        public bool fold_sliders_layout;
        public bool fold_sliders_gui;
        public bool fold_inputfield_layout;
        public bool fold_inputfield_Manual;
        public bool fold_state_layout;
        public bool fold_state_gui;
        public bool fold_toolbar_layout;
        public bool fold_pathselector_layout;
        public bool fold_pathselector_manual;
        public bool fold_toolbar_gui;
        public bool fold_color_gui;
        public bool fold_color_gui_layout;
        public bool fold_curve_gui;
        public bool fold_toggle_layout;
        public bool fold_listview;
        public bool fold_card;
        public bool fold_card_manual;
        #endregion

        #region 列表
        public List<PlayerData> PlayerDatas;

        /// <summary>
        /// 生成示例数据（用于按钮调用）
        /// </summary>
        /// <param name="row"></param>
        public void CreateSampleData()
        {
            if (PlayerDatas == null)
                PlayerDatas = new List<PlayerData>();
            PlayerDatas.Clear();

            string[] weapons = new string[] { "MP7", "Vector ", "SG552", "SMG-45 ", "野牛 (Bizon)", "AWM ", "R93", "AK-12 ", "M4A1", "K416 ", "SCAR-H" };
            string[] regions = new string[] { "7号电站", "航天研究中心", "核设施厂", "工业废料区" };

            for (int i = 0; i < 15; i++)
            {
                PlayerDatas.Add(
                    new PlayerData(
                    name: $"{NameGenerator.GenerateRandomName()}",
                    level: Random.Range(0, 100).ToString("D0"),
                    weapon: weapons[Random.Range(0, weapons.Length)],
                    health: Random.Range(0f, 100f).ToString("F2"),
                    region: regions[Random.Range(0, regions.Length)],
                    ammo: Random.Range(0, 100).ToString("D2"),
                    cdpercent: Random.Range(0f, 100f).ToString("F2"),
                    avatarpath: $"Avatars/avatar_{i}")
                    );
            }
        }
        #endregion

        #region 路径
        public string _outputPath;
        public string _configPath;
        public string _filePath;
        public string _filePath_manual;
        public string _configPath_manual;
        #endregion
    }
}
