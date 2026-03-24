namespace SevenStrikeModules.XHud.Utilitys
{
    using SevenStrikeModules.XHud.Enums;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    /// <summary>
    /// 常用功能
    /// </summary>
    public static class xHud_Utilitys
    {
        public static bool PrintMsgEnable = true;

        /// <summary>
        /// 优化Unity实例化出来的物体的(Clone)的标记名称
        /// </summary>
        /// <returns>返回不带(Clone)后缀的原物体名称.</returns>
        /// <param tweenName="RemoveString">要处理的字符串.</param>
        public static string Func_RemoveCloneSuffix(string RemoveString)
        {
            string str = RemoveString.Remove(RemoveString.Length - 7, RemoveString.Length - (RemoveString.Length - 7));
            return str;
        }

        /// <summary>
        /// 打印消息到控制台
        /// </summary>
        /// <param tweenName="Title">标题</param>
        /// <param tweenName="文字内容_Content">内容</param>
        /// <param tweenName="Mode">消息类型</param>
        public static HudMsgState Func_PrintInfo(string Title, string Content, HudMsgState Mode, GameObject xobject = null)
        {
            if (!PrintMsgEnable)
                return HudMsgState.未开启消息模块功能;
            switch (Mode)
            {
                case HudMsgState.确认:
                    Debug.Log("<color=#c4c4c4>" + Title + "： </color>" + "┠─<color=#c3e55c>" + Content + "</color>", xobject);
                    break;
                case HudMsgState.错误:
                    Debug.Log("<color=#c4c4c4>" + Title + "： </color>" + "┠─<color=#ff3f3f>" + Content + "</color>", xobject);
                    break;
                case HudMsgState.通知:
                    Debug.Log("<color=#c4c4c4>" + Title + "： </color>" + "┠─<color=#bebebe>" + Content + "</color>", xobject);
                    break;
                case HudMsgState.警告:
                    Debug.Log("<color=#c4c4c4>" + Title + "： </color>" + "┠─<color=#ffb80e>" + Content + "</color>", xobject);
                    break;
                case HudMsgState.设置:
                    Debug.Log("<color=#c4c4c4>" + Title + "： </color>" + "┠─<color=#5cc1e5>" + Content + "</color>", xobject);
                    break;
            }
            return Mode;
        }

        /// <summary>
        /// 移除物体名称中的小括号
        /// </summary>
        public static string Func_RemoveSymbol(Transform Object)
        {
            string ss = Object.name.Replace("（", "(").Replace("）", ")");
            ss = Regex.Replace(ss, @"\(.*\)", "");
            Object.name = ss;
            return ss;
        }

        /// <summary>
        /// 将所选的物体进行排序0-9   /   A-正向Z轴
        /// </summary>
        public static void Func_Sorting(GameObject[] Objects)
        {
            GameObject[] ss = Objects;

            List<GameObject> unsortedObjects = new List<GameObject>();
            for (int i = 0; i < ss.Length; i++)
            {
                unsortedObjects.Add(ss[i]);
            }
            List<GameObject> sortedObjects = new List<GameObject>();

            bool added = false;
            //Iterate over all objects in the unsorted
            for (int i = 0; i < unsortedObjects.Count; i++)
            {
                added = false;
                //Iterate over the objects in sorted for comparison.
                for (int j = 0; j < sortedObjects.Count; j++)
                {
                    //The condition, in this case, tweenName being less then
                    if (unsortedObjects[i].name.CompareTo(sortedObjects[j].name) < 0)
                    {
                        //Insert the object into list at point. Placing it before.
                        sortedObjects.Insert(j, unsortedObjects[i]);
                        added = true;
                        break;
                    }
                }
                //Add to back of list
                if (!added)
                    sortedObjects.Add(unsortedObjects[i]);
            }
            //if you want it as an array
            GameObject[] sortedObjectsArray = sortedObjects.ToArray();

            for (int i = 0; i < sortedObjectsArray.Length; i++)
            {
                sortedObjectsArray[i].transform.SetSiblingIndex(i);
            }
        }

        /// <summary>
        /// 获取Float数组中最小值
        /// </summary>
        /// <param tweenName="Values">源数组</param>
        /// <returns>返回数组中的最小值</returns>
        public static float Array_MinValue(float[] Values)
        {
            float minValue = Values.ToArray().Max();
            for (int i = 0; i < Values.Length; ++i)
            {
                if ((Values[i] > -9999.0f) && (minValue > Values[i]))
                {
                    minValue = Values[i];
                }
            }
            return minValue;
        }

        /// <summary>
        /// 获取Float数组中最大值
        /// </summary>
        /// <param tweenName="Values"></param>
        /// <returns>返回数组中的最大值</returns>
        public static float Array_MaxValue(float[] Values)
        {
            float maxValue = Values.ToArray().Max();

            return maxValue;
        }

        /// <summary>
        /// 拷贝字符串内容到系统剪贴板
        /// </summary>
        /// <param tweenName="str"></param>
        /// <returns></returns>
        public static string CopyToSystemBuffer(string str)
        {
            GUIUtility.systemCopyBuffer = str;
            return str;
        }

        /// <summary>
        /// 处理转义字符串
        /// </summary>
        /// <param tweenName="input"></param>
        /// <returns></returns>
        public static string ProcessEscapeSequences(string input)
        {
            return input.Replace("\\n", "\n");
        }

        #region 颜色亮度值
        /// <summary>
        /// 计算颜色的亮度极限
        /// </summary>
        /// <param tweenName="color">要计算亮度的颜色</param>
        /// <returns>返回False小于亮度中间值，返回True大于亮度中间值</returns>
        public static bool GetBrightnessLimite(Color color, float Threshold = 0.35f)
        {
            return (0.299f * color.r + 0.587f * color.g + 0.114f * color.b) > Threshold;
        }

        /// <summary>
        /// 计算颜色的亮度（灰度值）
        /// </summary>
        /// <param tweenName="color">要计算亮度的颜色</param>
        /// <returns>颜色的亮度，范围在 0 到 1 之间</returns>
        public static float GetBrightness(Color color)
        {
            return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
        }
        #endregion

        #region 字符串 -> 颜色
        /// <summary>
        /// 根据RGBA值转换到Color类型
        /// </summary>
        /// <param tweenName="R">填写 R - 红色值</param>
        /// <param tweenName="G">填写 G - 绿色值</param>
        /// <param tweenName="B">填写 B - 蓝色值</param>
        /// <param tweenName="A">填写 A - 透明度值</param>
        /// <param tweenName="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color</returns>
        public static Color Color_From_RGBA(float R, float G, float B, float A, bool ValueMode = true)
        {
            Color color = new Color();
            if (ValueMode)
            {
                color.r = R / 255f;
                color.g = G / 255f;
                color.b = B / 255f;
                color.a = A / 255f;
            }
            else
            {
                color.r = R;
                color.g = G;
                color.b = B;
                color.a = A;
            }
            return color;
        }
        /// <summary>
        /// 将分隔符为逗号的"r,g,b,a"的字符串转换为Color类型
        /// </summary>
        /// <param tweenName="ColorString">目标颜色格式字符串.</param>
        /// <param tweenName="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color.</returns>
        public static Color Color_From_String(string ColorString, bool ValueMode = true)
        {
            if (string.IsNullOrEmpty(ColorString))
                return Color.white;
            int Count = 0;
            string[] xx = ColorString.Split(new char[1] { ',' });

            float[] x = new float[xx.Length];

            foreach (string a in xx)
            {
                if (ValueMode)
                    x[Count] = float.Parse(a) / 255f;
                else
                    x[Count] = float.Parse(a);
                Count++;
                if (Count >= 4)
                {
                    Count = 0;
                }
            }
            Color color = new Color(x[0], x[1], x[2], x[3]);
            return color;
        }
        /// <summary>
        /// 将字符串数组转换为Color类型
        /// </summary>
        /// <param tweenName="ColorStringArray">颜色字符串数组，数组长度为4，既代表了RGBA</param>
        /// <param tweenName="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color.</returns>
        public static Color Color_From_StringArray(string[] ColorStringArray, bool ValueMode = true)
        {
            Color color = new Color();
            if (ValueMode)
            {
                color.r = float.Parse(ColorStringArray[0]) / 255f;
                color.g = float.Parse(ColorStringArray[1]) / 255f;
                color.b = float.Parse(ColorStringArray[2]) / 255f;
                color.a = float.Parse(ColorStringArray[3]);
            }
            else
            {
                color.r = float.Parse(ColorStringArray[0]);
                color.g = float.Parse(ColorStringArray[1]);
                color.b = float.Parse(ColorStringArray[2]);
                color.a = float.Parse(ColorStringArray[3]);
            }
            return color;
        }
        /// <summary>
        /// 将浮点数组转换为Color类型
        /// </summary>
        /// <param tweenName="ColorFloatArray">颜色浮点数组，数组长度为4，既代表了RGBA</param>
        /// <param tweenName="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color.</returns>
        public static Color Color_From_FloatArray(float[] ColorFloatArray, bool ValueMode = true)
        {
            Color color = new Color();
            if (ValueMode)
            {
                color.r = ColorFloatArray[0] / 255f;
                color.g = ColorFloatArray[1] / 255f;
                color.b = ColorFloatArray[2] / 255f;
                color.a = ColorFloatArray[3];
            }
            else
            {
                color.r = ColorFloatArray[0];
                color.g = ColorFloatArray[1];
                color.b = ColorFloatArray[2];
                color.a = ColorFloatArray[3];
            }
            return color;
        }
        /// <summary>
        /// 将整形数组转换为Color类型
        /// </summary>
        /// <param tweenName="ColorFloatArray">颜色浮点数组，数组长度为4，既代表了RGBA</param>
        /// <param tweenName="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color.</returns>
        public static Color Color_From_IntArray(int[] ColorFloatArray, bool ValueMode = true)
        {
            Color color = new Color();
            if (ValueMode)
            {
                color.r = (float)ColorFloatArray[0] / 255f;
                color.g = (float)ColorFloatArray[1] / 255f;
                color.b = (float)ColorFloatArray[2] / 255f;
                color.a = (float)ColorFloatArray[3];
            }
            else
            {
                color.r = (float)ColorFloatArray[0];
                color.g = (float)ColorFloatArray[1];
                color.b = (float)ColorFloatArray[2];
                color.a = (float)ColorFloatArray[3];
            }
            return color;
        }
        /// <summary>
        /// 将十六位进制颜色码字符串转换到Color类型
        /// </summary>
        /// <param tweenName="hex">填写需要转换成Color类型的十六位进制的颜色码字符串（请忽略颜色代码开头的 #）</param>
        /// <returns>此方法返回类型为 -> 颜色_Color</returns>
        public static Color Color_From_HexString(string hex)
        {
            Color nowColor = Color.black;

            bool HasPrefix = false;
            for (int i = 0; i < hex.Length; i++)
            {
                if (hex[i] == '#')
                    HasPrefix = true;
            }
            if (HasPrefix)
                ColorUtility.TryParseHtmlString(hex, out nowColor);
            else
                ColorUtility.TryParseHtmlString("#" + hex, out nowColor);
            return nowColor;
        }
        /// <summary>
        /// 将HSV颜色转换到Color类型
        /// </summary>
        /// <param tweenName="H">H</param>
        /// <param tweenName="S">S</param>
        /// <param tweenName="V">V</param>
        /// <returns>此方法返回类型为 -> 颜色_Color</returns>
        public static Color Color_From_HSV(float H, float S, float V)
        {
            //  将HSV值标准化到0-1范围内
            H = Mathf.Repeat(H, 1.0f);
            S = Mathf.Clamp01(S);
            V = Mathf.Clamp01(V);

            //  计算色相对应的角位置
            float C = V * S;  //  Chroma
            float HPrime = H * 6.0f;
            float X = C * (1.0f - Mathf.Abs(Mod(HPrime, 2.0f) - 1.0f));
            float m = V - C;
            float r, g, b;

            if (0 <= HPrime && HPrime < 1) { r = C; g = X; b = 0; }
            else if (1 <= HPrime && HPrime < 2) { r = X; g = C; b = 0; }
            else if (2 <= HPrime && HPrime < 3) { r = 0; g = C; b = X; }
            else if (3 <= HPrime && HPrime < 4) { r = 0; g = X; b = C; }
            else if (4 <= HPrime && HPrime < 5) { r = X; g = 0; b = C; }
            else if (5 <= HPrime && HPrime < 6) { r = C; g = 0; b = X; }
            else { r = 0; g = 0; b = 0; }

            //  转换为RGB值
            r = r + m;
            g = g + m;
            b = b + m;

            return new Color(r, g, b, 1.0f);
        }
        static float Mod(float dividend, float divisor)
        {
            return ((dividend % divisor) + divisor) % divisor;
        }
        #endregion

        #region 颜色 -> 字符串
        /// <summary>
        /// 将Color类型转换到十六进制颜色码字符串
        /// </summary>
        /// <param tweenName="color">填写需要转换成字符串格式的Color类型</param>
        /// <param tweenName="HasPrefixSymbol">True：前缀带有 # 号，False：无_None # 号前缀</param>
        /// <returns>此方法返回类型为 -> 字符串_String</returns>
        public static string Color_To_HexColor(Color color, bool HasPrefixSymbol = false)
        {
            if (HasPrefixSymbol)
                return "#" + ColorUtility.ToHtmlStringRGB(color);
            else
                return ColorUtility.ToHtmlStringRGB(color);
        }
        /// <summary>
        /// 将Color类型转换到字符串
        /// </summary>
        /// <param tweenName="color">填写需要转换成字符串格式的Color类型</param>
        /// <returns>此方法返回类型为 -> 字符串_String，格式为：R,G,B,A顺序排列的字符串</returns>
        public static string Color_To_String(Color color)
        {
            string _color = color.ToString();
            _color = _color.Remove(0, 5);
            _color = _color.Remove(_color.Length - 1, _color.Length - (_color.Length - 1)).Trim();
            return _color;
        }
        /// <summary>
        /// 将Color类型转换到自定义符号分隔的完整字符串
        /// </summary>
        /// <param tweenName="color">填写需要转换成字符串格式的Color类型</param>
        /// <param tweenName="Symbol">指定最后输出的字符串的分隔符号，例如： '|'   ','   '/'   '\'   '''   '-'   '.'   </param>
        /// <returns>此方法返回类型为 -> 字符串_String，格式为：R "自定义分隔符" G "自定义分隔符" B "自定义分隔符" A 顺序排列的字符串</returns>
        public static string Color_To_String(Color color, char Symbol = ',')
        {
            string _color = color.ToString();
            _color = _color.Remove(0, 5);
            _color = _color.Remove(_color.Length - 1, _color.Length - (_color.Length - 1)).Trim();
            string[] _splites = _color.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            string combine = "";
            for (int i = 0; i < _splites.Length; i++)
            {
                _splites[i] = _splites[i].Trim();
                if (i < _splites.Length - 1)
                    combine += _splites[i] + Symbol;
                else
                    combine += _splites[i];
            }
            return combine;
        }
        /// <summary>
        /// 将Color类型转换到字符串数组
        /// </summary>
        /// <param tweenName="color">填写需要转换成字符串数组格式的Color类型</param>
        /// <returns>此方法返回类型为 -> 字符串_String[]，格式为：字符串数组</returns>
        public static string[] Color_To_StringArray(Color color)
        {
            string _color = color.ToString();
            _color = _color.Remove(0, 5);
            _color = _color.Remove(_color.Length - 1, _color.Length - (_color.Length - 1)).Trim();
            string[] _SpliteColors = _color.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < _SpliteColors.Length; i++)
            {
                _SpliteColors[i] = _SpliteColors[i].Trim();
            }
            return _SpliteColors;
        }
        /// <summary>
        /// 将Color类型转换到浮点数组
        /// </summary>
        /// <param tweenName="color">填写需要转换成浮点数组格式的Color类型</param>
        /// <returns>此方法返回类型为 -> 浮点数_Float[]，格式为：浮点数组</returns>
        public static float[] Color_To_FloatArray(Color color)
        {
            string _color = color.ToString();
            _color = _color.Remove(0, 5);
            _color = _color.Remove(_color.Length - 1, _color.Length - (_color.Length - 1)).Trim();
            string[] _SpliteColors = _color.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            float[] fs = new float[_SpliteColors.Length];
            for (int i = 0; i < _SpliteColors.Length; i++)
            {
                _SpliteColors[i] = _SpliteColors[i].Trim();
                fs[i] = float.Parse(_SpliteColors[i]);
            }
            return fs;
        }
        #endregion

        #region 颜色 -> 调整
        /// <summary>
        /// 调整颜色的饱和度
        /// </summary>
        /// <param name="color">原始颜色</param>
        /// <param name="percent">饱和度百分比（0-1）</param>
        /// <returns>调整后的颜色</returns>
        public static Color Color_SetSaturation(Color color, float percent)
        {
            // 确保百分比在0到1之间
            percent = Mathf.Clamp01(percent);

            // 将RGB转换为HSV
            Color.RGBToHSV(color, out float h, out float s, out float v);

            // 调整饱和度
            s *= percent;

            // 将HSV转换回RGB
            return Color.HSVToRGB(h, s, v);
        }
        #endregion

        #region 字符串 -> 数组
        /// <summary>
        /// 将指定的带分隔符的字符串转换为字符串数组
        /// </summary>
        /// <param tweenName="字符串_String">需要转换的字符串</param>
        /// <param tweenName="Symbol">字符串中的分隔符</param>
        /// <returns>此方法返回类型为 -> 字符串_String[]字符串数组.</returns>
        public static string[] String_To_StringArray(string String, char Symbol = ',')
        {
            string[] Splite = String.Split(new char[1] { Symbol }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < Splite.Length; i++)
            {
                Splite[i] = Splite[i].Trim();
            }
            return Splite;
        }
        /// <summary>
        /// 将指定的带分隔符的字符串转换为字符串数组并赋值给指定字符串数组
        /// </summary>
        /// <param tweenName="StringArray">存储转换后的字符串数组</param>
        /// <param tweenName="字符串_String">需要转换的字符串</param>
        /// <param tweenName="Symbol">字符串中的分隔符</param>
        /// <returns></returns>
        public static void String_To_StringArray(string[] StringArray, string String, char Symbol = ',')
        {
            StringArray = String.Split(new char[1] { Symbol }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < StringArray.Length; i++)
            {
                StringArray[i] = StringArray[i].Trim();
            }
        }
        /// <summary>
        /// 将指定的带分隔符的字符串转换为浮点数组
        /// </summary>
        /// <param tweenName="字符串_String">需要转换的字符串</param>
        /// <param tweenName="Symbol">字符串中的分隔符</param>
        /// <returns>此方法返回类型为 -> 浮点数_Float[]浮点数组.</returns>
        public static float[] String_To_FloatArray(string String, char Symbol = ',')
        {
            string[] Splite = String.Split(new char[1] { Symbol }, System.StringSplitOptions.RemoveEmptyEntries);
            float[] ConValue = new float[Splite.Length];
            for (int i = 0; i < Splite.Length; i++)
            {
                Splite[i] = Splite[i].Trim();
                ConValue[i] = float.Parse(Splite[i]);
            }
            return ConValue;
        }
        /// <summary>
        /// 将指定的带分隔符的字符串转换为字符串数组并赋值给指定浮点数组
        /// </summary>
        /// <param tweenName="FloatArray">存储转换后的浮点数组</param>
        /// <param tweenName="字符串_String">需要转换的字符串</param>
        /// <param tweenName="Symbol">字符串中的分隔符</param>
        /// <returns></returns>
        public static void String_To_FloatArray(float[] FloatArray, string String, char Symbol = ',')
        {
            string[] StringArr = String.Split(new char[1] { Symbol }, System.StringSplitOptions.RemoveEmptyEntries);
            if (FloatArray == null || FloatArray.Length == 0)
                FloatArray = new float[StringArr.Length];
            for (int i = 0; i < FloatArray.Length; i++)
            {
                FloatArray[i] = float.Parse(StringArr[i].Trim());
            }
        }
        /// <summary>
        /// 将指定的带分隔符的字符串转换为整形数组
        /// </summary>
        /// <param tweenName="字符串_String">需要转换的字符串</param>
        /// <param tweenName="Symbol">字符串中的分隔符</param>
        /// <returns>此方法返回类型为 -> 整数_Int[]整形数组.</returns>
        public static int[] String_To_IntArray(string String, char Symbol = ',')
        {
            string[] Splite = String.Split(new char[1] { Symbol }, System.StringSplitOptions.RemoveEmptyEntries);
            int[] ConValue = new int[Splite.Length];
            for (int i = 0; i < Splite.Length; i++)
            {
                Splite[i] = Splite[i].Trim();
                ConValue[i] = int.Parse(Splite[i]);
            }
            return ConValue;
        }
        /// <summary>
        /// 将指定的带分隔符的字符串转换为字符串数组并赋值给指定整型数组
        /// </summary>
        /// <param tweenName="IntArray">存储转换后的整型数组</param>
        /// <param tweenName="字符串_String">需要转换的字符串</param>
        /// <param tweenName="Symbol">字符串中的分隔符</param>
        /// <returns></returns>
        public static void String_To_IntArray(int[] IntArray, string String, char Symbol = ',')
        {
            string[] StringArr = String.Split(new char[1] { Symbol }, System.StringSplitOptions.RemoveEmptyEntries);
            if (IntArray == null || IntArray.Length == 0)
                IntArray = new int[StringArr.Length];
            for (int i = 0; i < IntArray.Length; i++)
            {
                IntArray[i] = int.Parse(StringArr[i].Trim());
            }
        }
        #endregion

        #region 字符串 -> 向量 
        /// <summary>
        /// 将指定的带分隔符的字符串转换为Vector4向量
        /// </summary>
        /// <param tweenName="SourceVector">需要转换的字符串</param>
        /// <param tweenName="Symbol">字符串中的分隔符</param>
        /// <returns>此方法返回类型为 -> Vector4向量.</returns>
        public static Vector4 Vector4_From_String(string SourceVector, char Symbol = ',')
        {
            string[] Splite = SourceVector.Split(new char[1] { Symbol });
            Vector4 Combine = new Vector4(float.Parse(Splite[0]), float.Parse(Splite[1]), float.Parse(Splite[2]), float.Parse(Splite[3]));
            return Combine;
        }
        /// <summary>
        /// 将指定的带分隔符的字符串转换为Vector3向量
        /// </summary>
        /// <param tweenName="SourceVector">需要->换的向量字符串</param>
        /// <param tweenName="Symbol">字符串中的分隔符</param>
        /// <returns>此方法返回类型为 -> Vector3向量.</returns>
        public static Vector3 Vector3_From_String(string SourceVector, char Symbol = ',')
        {
            string[] Splite = SourceVector.Split(new char[1] { Symbol });
            Vector3 Combine = new Vector3(float.Parse(Splite[0]), float.Parse(Splite[1]), float.Parse(Splite[2]));
            return Combine;
        }
        /// <summary>
        /// 将指定的带分隔符的字符串转换为Vector2向量
        /// </summary>
        /// <param tweenName="SourceVector">需要转换的向量字符串</param>
        /// <param tweenName="Symbol">字符串中的分隔符</param>
        /// <returns>此方法返回类型为 -> Vector2向量.</returns>
        public static Vector2 Vector2_From_String(string SourceVector, char Symbol = ',')
        {
            string[] Splite = SourceVector.Split(new char[1] { Symbol });
            Vector2 Combine = new Vector2(float.Parse(Splite[0]), float.Parse(Splite[1]));
            return Combine;
        }
        #endregion

        #region 字符串数组 -> 向量 
        /// <summary>
        /// 将字符串数组转换为Vector4向量
        /// </summary>
        /// <param tweenName="StringArray">需要转换的字符串数组，请保证数组最大长度为4</param>
        /// <returns>此方法返回类型为 -> Vector4向量</returns>
        public static Vector4 Vector4_From_StringArray(string[] StringArray)
        {
            Vector4 Combine = new Vector4(float.Parse(StringArray[0]), float.Parse(StringArray[1]), float.Parse(StringArray[2]), float.Parse(StringArray[3]));
            return Combine;
        }
        /// <summary>
        /// 将字符串数组转换为Vector3向量
        /// </summary>
        /// <param tweenName="StringArray">需要转换的字符串数组，请保证数组最大长度为3</param>
        /// <returns>此方法返回类型为 -> Vector3向量</returns>
        public static Vector3 Vector3_From_StringArray(string[] StringArray)
        {
            Vector3 Combine = new Vector3(float.Parse(StringArray[0]), float.Parse(StringArray[1]), float.Parse(StringArray[2]));
            return Combine;
        }
        /// <summary>
        /// 将字符串数组转换为Vector2向量
        /// </summary>
        /// <param tweenName="StringArray">需要转换的字符串数组，请保证数组最大长度为2</param>
        /// <returns>此方法返回类型为 -> Vector2向量</returns>
        public static Vector2 Vector2_From_StringArray(string[] StringArray)
        {
            Vector2 Combine = new Vector2(float.Parse(StringArray[0]), float.Parse(StringArray[1]));
            return Combine;
        }
        #endregion

        #region 整型数组 -> 向量 
        /// <summary>
        /// 将整型数组转换为Vector4向量
        /// </summary>
        /// <param tweenName="IntArray">需要转换的整型数组，请保证数组最大长度为4</param>
        /// <returns>此方法返回类型为 -> Vector4向量</returns>
        public static Vector4 Vector4_From_IntArray(int[] IntArray)
        {
            Vector4 Combine = new Vector4((float)IntArray[0], (float)IntArray[1], (float)IntArray[2], (float)IntArray[3]);
            return Combine;
        }
        /// <summary>
        /// 将整型数组转换为Vector3向量
        /// </summary>
        /// <param tweenName="IntArray">需要转换的整型数组，请保证数组最大长度为3</param>
        /// <returns>此方法返回类型为 -> Vector3向量</returns>
        public static Vector3 Vector3_From_IntArray(int[] IntArray)
        {
            Vector3 Combine = new Vector3((float)IntArray[0], (float)IntArray[1], (float)IntArray[2]);
            return Combine;
        }
        /// <summary>
        /// 将整型数组转换为Vector2向量
        /// </summary>
        /// <param tweenName="IntArray">需要转换的整型数组，请保证数组最大长度为2</param>
        /// <returns>此方法返回类型为 -> Vector2向量</returns>
        public static Vector2 Vector2_From_IntArray(int[] IntArray)
        {
            Vector2 Combine = new Vector2((float)IntArray[0], (float)IntArray[1]);
            return Combine;
        }
        #endregion

        #region 浮点数组 -> 向量 
        /// <summary>
        /// 将浮点数组转换为Vector4向量
        /// </summary>
        /// <param tweenName="FloatArray">需要转换的浮点数组，请保证数组最大长度为4</param>
        /// <returns>此方法返回类型为 -> Vector4向量</returns>
        public static Vector4 Vector4_From_FloatArray(float[] FloatArray)
        {
            Vector4 Combine = new Vector4(FloatArray[0], FloatArray[1], FloatArray[2], FloatArray[3]);
            return Combine;
        }
        /// <summary>
        /// 将浮点数组转换为Vector3Int向量
        /// </summary>
        /// <param tweenName="FloatArray">需要转换的浮点数组，请保证数组最大长度为3</param>
        /// <returns>此方法返回类型为 -> Vector3Int向量</returns>
        public static Vector3 Vector3Int_From_IntArray(float[] FloatArray)
        {
            Vector3 Combine = new Vector3(FloatArray[0], FloatArray[1], FloatArray[2]);
            return Combine;
        }
        /// <summary>
        /// 将浮点数组转换为Vector2向量
        /// </summary>
        /// <param tweenName="FloatArray">需要转换的浮点数组，请保证数组最大长度为2</param>
        /// <returns>此方法返回类型为 -> Vector2向量</returns>
        public static Vector2 Vector2_From_FloatArray(float[] FloatArray)
        {
            Vector2 Combine = new Vector2(FloatArray[0], FloatArray[1]);
            return Combine;
        }
        #endregion

        #region 字符串 -> 布尔值
        /// <summary>
        /// 将字符串形式的布尔值解析到布尔类型
        /// </summary>
        /// <param tweenName="字符串_String">需要解析成布尔值的字符串，字符串内容只能是true、false、0、1、on、off</param>
        /// <returns>此方法返回类型为 -> Bool类型.</returns>
        public static bool String_To_Bool(string String)
        {
            bool Val = false;
            if (String == "true" || String == "1" || String == "on")
                Val = true;
            if (String == "false" || String == "0" || String == "off")
                Val = false;
            return Val;
        }
        /// <summary>
        /// 将字符串形式的多个布尔值解析转换成布尔数组
        /// </summary>
        /// <param tweenName="字符串_String">需要解析成布尔数组的字符串，字符串内容可以是 \n "true,true,false,true" \n "1,1,0,1" \n "on,on,off,on"</param>
        /// <param tweenName="Symbol">字符串中的分隔符</param>
        /// <returns>此方法返回类型为 -> Bool数组类型.</returns>
        public static bool[] String_To_BoolArray(string String, char Symbol = ',')
        {
            string[] Splite = String.Split(new char[1] { Symbol }, System.StringSplitOptions.RemoveEmptyEntries);
            bool[] Vals = new bool[Splite.Length];
            for (int i = 0; i < Splite.Length; i++)
            {
                Splite[i] = Splite[i].Trim();
                if (Splite[i] == "true" || Splite[i] == "1" || Splite[i] == "on")
                    Vals[i] = true;
                else if (Splite[i] == "false" || Splite[i] == "0" || Splite[i] == "off")
                    Vals[i] = false;
            }
            return Vals;
        }
        #endregion

        #region 向量 -> 字符串
        /// <summary>
        ///  将Vector3向量转换为 x,y,z 字符串格式（小括号可选）
        /// </summary>
        /// <param tweenName="SourceVector">需要转换的向量.</param>
        /// <param tweenName="IncludeBracket">转换后是否包含前后小括号，例：(x,y,z) </param>
        /// <returns>此方法返回类型为 ->  x,y,z 格式的字符串.</returns>
        public static string Vector3_To_String(Vector3 SourceVector, bool IncludeBracket = false)
        {
            string Combine = null;

            if (!IncludeBracket)
            {
                string ConData = SourceVector.ToString().Remove(0, 1).Trim();
                Combine = ConData.Remove(ConData.Length - 1, 1).Trim();
            }
            else
            {
                Combine = SourceVector.ToString();
            }
            return Combine;
        }
        /// <summary>
        ///  将Vector4向量转换为 x,y,z,w 字符串格式（小括号可选）
        /// </summary>
        /// <param tweenName="SourceVector">需要转换的向量.</param>
        /// <param tweenName="IncludeBracket">转换后是否包含前后小括号，例：(x,y,z) </param>
        /// <returns>此方法返回类型为 ->  x,y,z,w 格式的字符串.</returns>
        public static string Vector4_To_String(Vector4 SourceVector, bool IncludeBracket = false)
        {
            string Combine = null;

            if (!IncludeBracket)
            {
                string ConData = SourceVector.ToString().Remove(0, 1).Trim();
                Combine = ConData.Remove(ConData.Length - 1, 1).Trim();
            }
            else
            {
                Combine = SourceVector.ToString();
            }
            return Combine;
        }
        /// <summary>
        ///  将Vector2向量转换为 x,y 字符串格式（小括号可选）
        /// </summary>
        /// <param tweenName="SourceVector">需要转换的向量.</param>
        /// <param tweenName="IncludeBracket">转换后是否包含前后小括号，例：(x,y,z) </param>
        /// <returns>此方法返回类型为 ->  x,y 格式的字符串.</returns>
        public static string Vector2_To_String(Vector2 SourceVector, bool IncludeBracket = false)
        {
            string Combine = null;

            if (!IncludeBracket)
            {
                string ConData = SourceVector.ToString().Remove(0, 1).Trim();
                Combine = ConData.Remove(ConData.Length - 1, 1).Trim();
            }
            else
            {
                Combine = SourceVector.ToString();
            }
            return Combine;
        }
        #endregion

        #region 向量 -> 字符串数组
        /// <summary>
        ///  将Vector3向量转换为字符串数组
        /// </summary>
        /// <param tweenName="VectorValue">待转换的目标 三维向量_Vector3 </param>
        /// <returns>此方法返回类型为 ->String数组.</returns>
        public static string[] Vector3_To_StringArray(Vector3 VectorValue)
        {
            string[] Splite = new string[3];

            Splite[0] = VectorValue.x.ToString();
            Splite[1] = VectorValue.y.ToString();
            Splite[2] = VectorValue.z.ToString();

            return Splite;
        }
        /// <summary>
        ///  将Vector4向量转换为字符串数组
        /// </summary>
        /// <param tweenName="VectorValue">待转换的目标 四维向量_Vector4 </param>
        /// <returns>此方法返回类型为 -> String数组.</returns>
        public static string[] Vector4_To_StringArray(Vector4 VectorValue)
        {
            string[] Splite = new string[4];

            Splite[0] = VectorValue.x.ToString();
            Splite[1] = VectorValue.y.ToString();
            Splite[2] = VectorValue.z.ToString();
            Splite[3] = VectorValue.w.ToString();

            return Splite;
        }
        /// <summary>
        ///  将Vector2向量转换为字符串数组
        /// </summary>
        /// <param tweenName="VectorValue">待转换的目标 二维向量_Vector2 </param>
        /// <returns>此方法返回类型为 ->String数组.</returns>
        public static string[] Vector2_To_StringArray(Vector2 VectorValue)
        {
            string[] Splite = new string[2];

            Splite[0] = VectorValue.x.ToString();
            Splite[1] = VectorValue.y.ToString();

            return Splite;
        }
        #endregion

        #region 向量 -> 浮点数组
        /// <summary>
        ///  将Vector3向量转换为浮点数组格式
        /// </summary>
        /// <param tweenName="VectorValue">待转换的目标 三维向量_Vector3 </param>
        /// <returns>此方法返回类型为 -> Float浮点数组.</returns>
        public static float[] Vector3_To_FloatArray(Vector3 VectorValue)
        {
            float[] CutNum = new float[3];

            CutNum[0] = VectorValue.x;
            CutNum[1] = VectorValue.y;
            CutNum[2] = VectorValue.z;

            return CutNum;
        }
        /// <summary>
        ///  将Vector4向量转换为浮点数组格式
        /// </summary>
        /// <param tweenName="VectorValue">待转换的目标 四维向量_Vector4 </param>
        /// <returns>此方法返回类型为 -> Float浮点数组.</returns>
        public static float[] Vector4_To_FloatArray(Vector4 VectorValue)
        {
            float[] CutNum = new float[4];

            CutNum[0] = VectorValue.x;
            CutNum[1] = VectorValue.y;
            CutNum[2] = VectorValue.z;
            CutNum[3] = VectorValue.w;

            return CutNum;
        }
        /// <summary>
        ///  将Vector2向量转换为浮点数组格式
        /// </summary>
        /// <param tweenName="VectorValue">待转换的目标 二维向量_Vector2 </param>
        /// <returns>此方法返回类型为 -> Float浮点数组.</returns>
        public static float[] Vector2_To_FloatArray(Vector2 VectorValue)
        {
            float[] CutNum = new float[2];

            CutNum[0] = VectorValue.x;
            CutNum[1] = VectorValue.y;

            return CutNum;
        }
        #endregion

        #region 向量 -> 整型数组
        /// <summary>
        ///  将Vector3向量转换为整形数组格式
        /// </summary>
        /// <param tweenName="VectorValue">待转换的目标 三维向量_Vector3 </param>
        /// <returns>此方法返回类型为 -> Int整形数组.</returns>
        public static int[] Vector3_To_IntArray(Vector3 VectorValue)
        {
            int[] CutNum = new int[3];

            CutNum[0] = (int)VectorValue.x;
            CutNum[1] = (int)VectorValue.y;
            CutNum[2] = (int)VectorValue.z;

            return CutNum;
        }
        /// <summary>
        ///  将Vector4向量转换为整形数组格式
        /// </summary>
        /// <param tweenName="VectorValue">待转换的目标 四维向量_Vector4 </param>
        /// <returns>此方法返回类型为 -> Int整形数组.</returns>
        public static int[] Vector4_To_IntArray(Vector4 VectorValue)
        {
            int[] CutNum = new int[4];

            CutNum[0] = (int)VectorValue.x;
            CutNum[1] = (int)VectorValue.y;
            CutNum[2] = (int)VectorValue.z;
            CutNum[3] = (int)VectorValue.w;

            return CutNum;
        }
        /// <summary>
        ///  将Vector2向量转换为整形数组格式
        /// </summary>
        /// <param tweenName="VectorValue">待转换的目标 二维向量_Vector2 </param>
        /// <returns>此方法返回类型为 -> Int整形数组.</returns>
        public static int[] Vector2_To_IntArray(Vector2 VectorValue)
        {
            int[] CutNum = new int[2];

            CutNum[0] = (int)VectorValue.x;
            CutNum[1] = (int)VectorValue.y;

            return CutNum;
        }
        #endregion

        #region 数值保存到Prefs / ForEditor是适用于编辑器模式，而ForRuntime适用于游戏运行时

        ///--------------------编辑器模式
#if UNITY_EDITOR
        /// <summary>
        /// 检查数据关键字是否存在（编辑器模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        public static bool PlayerPrefs_KeyIsExist_ForEditor(string key)
        {
            return EditorPrefs.HasKey(key);
        }

        /// <summary>
        /// 存入数据（编辑器模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static void PlayerPrefs_SaveValue_ForEditor(string key, string value)
        {
            EditorPrefs.SetString(key, value);
        }

        /// <summary>
        /// 存入数据（编辑器模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static void PlayerPrefs_SaveValue_ForEditor(string key, int value)
        {
            EditorPrefs.SetInt(key, value);
        }

        /// <summary>
        /// 存入数据（编辑器模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static void PlayerPrefs_SaveValue_ForEditor(string key, float value)
        {
            EditorPrefs.SetFloat(key, value);
        }

        /// <summary>
        /// 存入数据（编辑器模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static void PlayerPrefs_SaveValue_ForEditor(string key, bool value)
        {
            EditorPrefs.SetBool(key, value);
        }

        /// <summary>
        /// 取出数据（编辑器模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static string PlayerPrefs_ReadValue_String_ForEditor(string key)
        {
            return EditorPrefs.GetString(key);
        }

        /// <summary>
        /// 取出数据（编辑器模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static int PlayerPrefs_ReadValue_Int_ForEditor(string key)
        {
            return EditorPrefs.GetInt(key);
        }

        /// <summary>
        /// 取出数据（编辑器模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static float PlayerPrefs_ReadValue_Float_ForEditor(string key)
        {
            return EditorPrefs.GetFloat(key);
        }

        /// <summary>
        /// 取出数据（编辑器模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static bool PlayerPrefs_ReadValue_Bool_ForEditor(string key)
        {
            return EditorPrefs.GetBool(key);
        }

        /// <summary>
        /// 清空数据（编辑器模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        public static void PlayerPrefs_DeleteValue_ForEditor(string key)
        {
            EditorPrefs.DeleteKey(key);
        }
#endif
        ///--------------------运行模式

        /// <summary>
        /// 检查数据关键字是否存在（运行模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        public static bool PlayerPrefs_KeyIsExist_ForRuntime(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        /// <summary>
        /// 存入数据（运行模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static void PlayerPrefs_SaveValue_ForRuntime(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        /// <summary>
        /// 存入数据（运行模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static void PlayerPrefs_SaveValue_ForRuntime(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        /// <summary>
        /// 存入数据（运行模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static void PlayerPrefs_SaveValue_ForRuntime(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        /// <summary>
        /// 取出数据（运行模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static string PlayerPrefs_ReadValue_String_ForRuntime(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        /// <summary>
        /// 取出数据（运行模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static int PlayerPrefs_ReadValue_Int_ForRuntime(string key)
        {
            return PlayerPrefs.GetInt(key);
        }

        /// <summary>
        /// 取出数据（运行模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        /// <param tweenName="value">值</param>
        public static float PlayerPrefs_ReadValue_Float_ForRuntime(string key)
        {
            return PlayerPrefs.GetFloat(key);
        }

        /// <summary>
        /// 清空数据（运行模式）
        /// </summary>
        /// <param tweenName="key">索引名称</param>
        public static void PlayerPrefs_DeleteValue_ForRuntime(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
        #endregion

        #region 方法运行计时器
        public static void MethodStopwatch(Action Methods = null)
        {
            // 创建一个Stopwatch实例
            Stopwatch stopwatch = new Stopwatch();

            // 开始计时
            stopwatch.Start();

            if (Methods != null)
                Methods.Invoke();

            // 停止计时
            stopwatch.Stop();

            // 输出方法的耗时
            Debug.Log($"方法耗时：{stopwatch.ElapsedMilliseconds} 毫秒");
        }

        #endregion
    }
}