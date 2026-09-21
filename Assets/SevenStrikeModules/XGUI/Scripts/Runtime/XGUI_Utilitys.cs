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
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using Debug = UnityEngine.Debug;
    using Random = UnityEngine.Random;

    /// <summary>
    /// 表示一个包含最小值和最大值的数值范围结构体。
    /// </summary>
    /// <remarks>
    /// 该结构体常用于补间动画（Tween）中定义属性的变化范围，例如：
    /// <code>
    /// var range = new xtween_minmax_value(0f, 100f);
    /// </code>
    /// </remarks>
    public struct xgui_minmax_value
    {
        /// <summary>
        /// 范围的最小值。
        /// </summary>
        public float min;

        /// <summary>
        /// 范围的最大值。
        /// </summary>
        public float max;

        /// <summary>
        /// 使用指定的最小值和最大值初始化 <see cref="xgui_minmax_value"/> 结构体的新实例。
        /// </summary>
        /// <param name="min">范围的最小值。</param>
        /// <param name="max">范围的最大值。</param>
        /// <exception cref="ArgumentException">当 min 大于 max 时抛出。</exception>
        public xgui_minmax_value(float min, float max)
        {
            // 可选：添加参数验证
            // if (min > max) throw new ArgumentException("min cannot be greater than max");
            this.min = min;
            this.max = max;
        }
    }

    public static class XGUI_Utilitys
    {
        public static bool PrintMsgEnable = true;

        #region To_Color
        /// <summary>
        /// 将分隔符为逗号的"r,g,b,a"的字符串转换为Color类型
        /// </summary>
        /// <param name="ColorString">目标颜色格式字符串.</param>
        /// <param name="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color.</returns>
        public static Color String_To_Color(string ColorString, bool ValueMode = true)
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
        /// 将十六位进制颜色码字符串转换到Color类型
        /// </summary>
        /// <param name="hex">填写需要转换成Color类型的十六位进制的颜色码字符串（请忽略颜色代码开头的 #）</param>
        /// <returns>此方法返回类型为 -> 颜色_Color</returns>
        public static Color HexString_To_Color(string hex)
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
        /// 根据RGBA值转换到Color类型
        /// </summary>
        /// <param name="R">填写 R - 红色值</param>
        /// <param name="G">填写 G - 绿色值</param>
        /// <param name="B">填写 B - 蓝色值</param>
        /// <param name="A">填写 A - 透明度值</param>
        /// <param name="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color</returns>
        public static Color RGBA_To_Color(float R, float G, float B, float A, bool ValueMode = true)
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
        /// 将字符串数组转换为Color类型
        /// </summary>
        /// <param name="ColorStringArray">颜色字符串数组，数组长度为4，既代表了RGBA</param>
        /// <param name="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color.</returns>
        public static Color StringArray_To_Color(string[] ColorStringArray, bool ValueMode = true)
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
        /// <param name="ColorFloatArray">颜色浮点数组，数组长度为4，既代表了RGBA</param>
        /// <param name="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color.</returns>
        public static Color FloatArray_To_Color(float[] ColorFloatArray, bool ValueMode = true)
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
        /// <param name="ColorFloatArray">颜色浮点数组，数组长度为4，既代表了RGBA</param>
        /// <param name="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color.</returns>
        public static Color IntArray_To_Color(int[] ColorFloatArray, bool ValueMode = true)
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
        /// 将HSV颜色转换到Color类型
        /// </summary>
        /// <param name="H">H</param>
        /// <param name="S">S</param>
        /// <param name="V">V</param>
        /// <returns>此方法返回类型为 -> 颜色_Color</returns>
        public static Color HSV_To_Color(float H, float S, float V)
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
        /// <summary>
        /// 对浮点数进行取模运算，结果始终为非负数。
        /// <para>与 C# 的 <c>%</c> 运算符不同，当被除数为负数时，本方法返回的结果仍保持与除数同号（通常为非负），
        /// 常用于颜色、角度等需要循环归一化的场景。</para>
        /// </summary>
        /// <param name="dividend">被除数（dividend），即需要取模的数值。</param>
        /// <param name="divisor">除数（divisor），即模数，不能为 0。</param>
        /// <returns>
        /// 返回 <paramref name="dividend"/> 对 <paramref name="divisor"/> 取模后的结果，
        /// 取值范围为 <c>[0, divisor)</c>（当 <paramref name="divisor"/> 为正数时）。
        /// </returns>
        /// <example>
        /// 示例：
        /// <code>
        /// Mod(7f, 3f)   // 返回 1
        /// Mod(-1f, 3f)  // 返回 2（而 -1f % 3f 返回 -1）
        /// Mod(3f, 3f)   // 返回 0
        /// </code>
        /// </example>
        static float Mod(float dividend, float divisor)
        {
            return ((dividend % divisor) + divisor) % divisor;
        }
        #endregion

        #region Color To
        /// <summary>
        /// 将Color类型转换到十六进制颜色码字符串
        /// </summary>
        /// <param name="color">填写需要转换成字符串格式的Color类型</param>
        /// <param name="HasPrefixSymbol">True：前缀带有 # 号，False：无_None # 号前缀</param>
        /// <returns>此方法返回类型为 -> 字符串_String</returns>
        public static string Color_To_HexString(Color color, bool HasPrefixSymbol = false)
        {
            if (HasPrefixSymbol)
                return "#" + ColorUtility.ToHtmlStringRGB(color);
            else
                return ColorUtility.ToHtmlStringRGB(color);
        }
        /// <summary>
        /// 将Color类型转换到字符串
        /// </summary>
        /// <param name="color">填写需要转换成字符串格式的Color类型</param>
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
        /// <param name="color">填写需要转换成字符串格式的Color类型</param>
        /// <param name="Symbol">指定最后输出的字符串的分隔符号，例如： '|'   ','   '/'   '\'   '''   '-'   '.'   </param>
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
        /// 将 Color 转换为长度为 4 的字符串数组，顺序为 R、G、B、A。
        /// </summary>
        /// <param name="color">要转换的颜色。</param>
        /// <returns>长度为 4 的数组，依次为 R、G、B、A 分量的字符串表示（不变文化格式）。</returns>
        /// <example>
        /// <code>
        /// string[] s = ColorToStringArray(new Color(0.25f, 0.5f, 0.75f, 1f));
        /// // s ≈ { "0.25", "0.5", "0.75", "1" }
        /// </code>
        /// </example>
        public static string[] Color_To_StringArray(Color color)
        {
            var ci = System.Globalization.CultureInfo.InvariantCulture;
            return new string[]
            {
                color.r.ToString(ci),
                color.g.ToString(ci),
                color.b.ToString(ci),
                color.a.ToString(ci),
            };
        }
        /// <summary>
        /// 将 Color 转换为长度为 4 的浮点数组，顺序为 R、G、B、A。
        /// </summary>
        /// <param name="color">要转换的颜色。</param>
        /// <returns>长度为 4 的数组，依次为 R、G、B、A 分量，取值范围 0~1。</returns>
        /// <example>
        /// <code>
        /// float[] rgba = ColorToFloatArray(new Color(0.25f, 0.5f, 0.75f, 1f));
        /// // rgba = { 0.25, 0.5, 0.75, 1 }
        /// </code>
        /// </example>
        public static float[] Color_To_FloatArray(Color color)
        {
            return new float[] { color.r, color.g, color.b, color.a };
        }
        /// <summary>
        /// 将 Color 转换为长度为 4 的整数数组，顺序为 R、G、B、A。
        /// 每个分量从 0~1 映射到 0~255。
        /// </summary>
        /// <param name="color">要转换的颜色。</param>
        /// <returns>长度为 4 的数组，依次为 R、G、B、A，取值范围 0~255。</returns>
        public static int[] Color_To_IntArray(Color color)
        {
            Color32 c32 = color;
            return new int[] { c32.r, c32.g, c32.b, c32.a };
        }
        #endregion

        #region ColorBrightness
        /// <summary>
        /// 计算颜色的亮度极限
        /// </summary>
        /// <param name="color">要计算亮度的颜色</param>
        /// <returns>返回False小于亮度中间值，返回True大于亮度中间值</returns>
        public static bool ColorBrightness_LimiteGet(Color color, float Threshold = 0.35f)
        {
            return (0.299f * color.r + 0.587f * color.g + 0.114f * color.b) > Threshold;
        }
        /// <summary>
        /// 计算颜色的亮度（灰度值）
        /// </summary>
        /// <param name="color">要计算亮度的颜色</param>
        /// <returns>颜色的亮度，范围在 0 到 1 之间</returns>
        public static float ColorBrightness_Get(Color color)
        {
            return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
        }
        #endregion

        #region Vector To
        /// <summary>
        ///  将Vector4向量转换为 x,y,z,w 字符串格式（小括号可选）
        /// </summary>
        /// <param name="SourceVector">需要转换的向量.</param>
        /// <param name="IncludeBracket">转换后是否包含前后小括号，例：(x,y,z) </param>
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
        ///  将Vector4向量转换为浮点数组格式
        /// </summary>
        /// <param name="VectorValue">待转换的目标 四维向量_Vector4 </param>
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
        ///  将Vector4向量转换为字符串数组
        /// </summary>
        /// <param name="VectorValue">待转换的目标 四维向量_Vector4 </param>
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
        ///  将Vector4向量转换为整形数组格式
        /// </summary>
        /// <param name="VectorValue">待转换的目标 四维向量_Vector4 </param>
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
        ///  将Vector3向量转换为 x,y,z 字符串格式（小括号可选）
        /// </summary>
        /// <param name="SourceVector">需要转换的向量.</param>
        /// <param name="IncludeBracket">转换后是否包含前后小括号，例：(x,y,z) </param>
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
        ///  将Vector3向量转换为字符串数组
        /// </summary>
        /// <param name="VectorValue">待转换的目标 三维向量_Vector3 </param>
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
        ///  将Vector3向量转换为浮点数组格式
        /// </summary>
        /// <param name="VectorValue">待转换的目标 三维向量_Vector3 </param>
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
        ///  将Vector3向量转换为整形数组格式
        /// </summary>
        /// <param name="VectorValue">待转换的目标 三维向量_Vector3 </param>
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
        ///  将Vector2向量转换为 x,y 字符串格式（小括号可选）
        /// </summary>
        /// <param name="SourceVector">需要转换的向量.</param>
        /// <param name="IncludeBracket">转换后是否包含前后小括号，例：(x,y,z) </param>
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
        /// <summary>
        ///  将Vector2向量转换为字符串数组
        /// </summary>
        /// <param name="VectorValue">待转换的目标 二维向量_Vector2 </param>
        /// <returns>此方法返回类型为 ->String数组.</returns>
        public static string[] Vector2_To_StringArray(Vector2 VectorValue)
        {
            string[] Splite = new string[2];

            Splite[0] = VectorValue.x.ToString();
            Splite[1] = VectorValue.y.ToString();

            return Splite;
        }
        /// <summary>
        ///  将Vector2向量转换为浮点数组格式
        /// </summary>
        /// <param name="VectorValue">待转换的目标 二维向量_Vector2 </param>
        /// <returns>此方法返回类型为 -> Float浮点数组.</returns>
        public static float[] Vector2_To_FloatArray(Vector2 VectorValue)
        {
            float[] CutNum = new float[2];

            CutNum[0] = VectorValue.x;
            CutNum[1] = VectorValue.y;

            return CutNum;
        }
        /// <summary>
        ///  将Vector2向量转换为整形数组格式
        /// </summary>
        /// <param name="VectorValue">待转换的目标 二维向量_Vector2 </param>
        /// <returns>此方法返回类型为 -> Int整形数组.</returns>
        public static int[] Vector2_To_IntArray(Vector2 VectorValue)
        {
            int[] CutNum = new int[2];

            CutNum[0] = (int)VectorValue.x;
            CutNum[1] = (int)VectorValue.y;

            return CutNum;
        }
        #endregion

        #region String To
        /// <summary>
        /// 将字符串形式的布尔值解析到布尔类型
        /// </summary>
        /// <param name="字符串_String">需要解析成布尔值的字符串，字符串内容只能是true、false、0、1、on、off</param>
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
        /// <param name="字符串_String">需要解析成布尔数组的字符串，字符串内容可以是 \n "true,true,false,true" \n "1,1,0,1" \n "on,on,off,on"</param>
        /// <param name="Symbol">字符串中的分隔符</param>
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

        /// <summary>
        /// 将指定的带分隔符的字符串转换为Vector4向量
        /// </summary>
        /// <param name="SourceVector">需要转换的字符串</param>
        /// <param name="Symbol">字符串中的分隔符</param>
        /// <returns>此方法返回类型为 -> Vector4向量.</returns>
        public static Vector4 String_To_Vector4(string SourceVector, char Symbol = ',')
        {
            string[] Splite = SourceVector.Split(new char[1] { Symbol });
            Vector4 Combine = new Vector4(float.Parse(Splite[0]), float.Parse(Splite[1]), float.Parse(Splite[2]), float.Parse(Splite[3]));
            return Combine;
        }
        /// <summary>
        /// 将指定的带分隔符的字符串转换为Vector3向量
        /// </summary>
        /// <param name="SourceVector">需要->换的向量字符串</param>
        /// <param name="Symbol">字符串中的分隔符</param>
        /// <returns>此方法返回类型为 -> Vector3向量.</returns>
        public static Vector3 String_To_Vector3(string SourceVector, char Symbol = ',')
        {
            string[] Splite = SourceVector.Split(new char[1] { Symbol });
            Vector3 Combine = new Vector3(float.Parse(Splite[0]), float.Parse(Splite[1]), float.Parse(Splite[2]));
            return Combine;
        }
        /// <summary>
        /// 将指定的带分隔符的字符串转换为Vector2向量
        /// </summary>
        /// <param name="SourceVector">需要转换的向量字符串</param>
        /// <param name="Symbol">字符串中的分隔符</param>
        /// <returns>此方法返回类型为 -> Vector2向量.</returns>
        public static Vector2 String_To_Vector2(string SourceVector, char Symbol = ',')
        {
            string[] Splite = SourceVector.Split(new char[1] { Symbol });
            Vector2 Combine = new Vector2(float.Parse(Splite[0]), float.Parse(Splite[1]));
            return Combine;
        }

        /// <summary>
        /// 将字符串数组转换为Vector4向量
        /// </summary>
        /// <param name="StringArray">需要转换的字符串数组，请保证数组最大长度为4</param>
        /// <returns>此方法返回类型为 -> Vector4向量</returns>
        public static Vector4 StringArray_To_Vector4(string[] StringArray)
        {
            Vector4 Combine = new Vector4(float.Parse(StringArray[0]), float.Parse(StringArray[1]), float.Parse(StringArray[2]), float.Parse(StringArray[3]));
            return Combine;
        }
        /// <summary>
        /// 将字符串数组转换为Vector3向量
        /// </summary>
        /// <param name="StringArray">需要转换的字符串数组，请保证数组最大长度为3</param>
        /// <returns>此方法返回类型为 -> Vector3向量</returns>
        public static Vector3 StringArray_To_Vector3(string[] StringArray)
        {
            Vector3 Combine = new Vector3(float.Parse(StringArray[0]), float.Parse(StringArray[1]), float.Parse(StringArray[2]));
            return Combine;
        }
        /// <summary>
        /// 将字符串数组转换为Vector2向量
        /// </summary>
        /// <param name="StringArray">需要转换的字符串数组，请保证数组最大长度为2</param>
        /// <returns>此方法返回类型为 -> Vector2向量</returns>
        public static Vector2 StringArray_To_Vector2(string[] StringArray)
        {
            Vector2 Combine = new Vector2(float.Parse(StringArray[0]), float.Parse(StringArray[1]));
            return Combine;
        }
        #endregion

        #region IntArray To
        /// <summary>
        /// 将整型数组转换为Vector4向量
        /// </summary>
        /// <param name="IntArray">需要转换的整型数组，请保证数组最大长度为4</param>
        /// <returns>此方法返回类型为 -> Vector4向量</returns>
        public static Vector4 IntArray_To_Vector4(int[] IntArray)
        {
            Vector4 Combine = new Vector4((float)IntArray[0], (float)IntArray[1], (float)IntArray[2], (float)IntArray[3]);
            return Combine;
        }
        /// <summary>
        /// 将整型数组转换为Vector3向量
        /// </summary>
        /// <param name="IntArray">需要转换的整型数组，请保证数组最大长度为3</param>
        /// <returns>此方法返回类型为 -> Vector3向量</returns>
        public static Vector3 IntArray_To_Vector3(int[] IntArray)
        {
            Vector3 Combine = new Vector3((float)IntArray[0], (float)IntArray[1], (float)IntArray[2]);
            return Combine;
        }
        /// <summary>
        /// 将整型数组转换为Vector2向量
        /// </summary>
        /// <param name="IntArray">需要转换的整型数组，请保证数组最大长度为2</param>
        /// <returns>此方法返回类型为 -> Vector2向量</returns>
        public static Vector2 IntArray_To_Vector2(int[] IntArray)
        {
            Vector2 Combine = new Vector2((float)IntArray[0], (float)IntArray[1]);
            return Combine;
        }
        #endregion

        #region FloatArray To
        /// <summary>
        /// 将浮点数组转换为Vector4向量
        /// </summary>
        /// <param name="FloatArray">需要转换的浮点数组，请保证数组最大长度为4</param>
        /// <returns>此方法返回类型为 -> Vector4向量</returns>
        public static Vector4 FloatArray_To_Vector4(float[] FloatArray)
        {
            Vector4 Combine = new Vector4(FloatArray[0], FloatArray[1], FloatArray[2], FloatArray[3]);
            return Combine;
        }
        /// <summary>
        /// 将浮点数组转换为Vector3Int向量
        /// </summary>
        /// <param name="FloatArray">需要转换的浮点数组，请保证数组最大长度为3</param>
        /// <returns>此方法返回类型为 -> Vector3Int向量</returns>
        public static Vector3 FloatArray_To_Vector3(float[] FloatArray)
        {
            Vector3 Combine = new Vector3(FloatArray[0], FloatArray[1], FloatArray[2]);
            return Combine;
        }
        /// <summary>
        /// 将浮点数组转换为Vector2向量
        /// </summary>
        /// <param name="FloatArray">需要转换的浮点数组，请保证数组最大长度为2</param>
        /// <returns>此方法返回类型为 -> Vector2向量</returns>
        public static Vector2 FloatArray_To_Vector2(float[] FloatArray)
        {
            Vector2 Combine = new Vector2(FloatArray[0], FloatArray[1]);
            return Combine;
        }
        #endregion

        #region ColorSet
        /// <summary>
        /// 生成完全随机的颜色
        /// </summary>
        /// <returns>RGB分量完全随机的颜色</returns>
        public static Color RandomRGB()
        {
            return new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            );
        }
        /// <summary>
        /// 生成鲜艳的颜色（高饱和、高亮度）
        /// </summary>
        /// <returns>高饱和度(0.8)、高亮度(0.9)的鲜艳颜色</returns>
        public static Color RandomVivid()
        {
            return Color.HSVToRGB(
                Random.Range(0f, 1f),
                0.8f,  // 高饱和
                0.9f   // 高亮度
            );
        }
        /// <summary>
        /// 生成柔和的颜色（低饱和、高亮度）
        /// </summary>
        /// <returns>饱和度0.2-0.4、亮度0.8-1.0的柔和颜色</returns>
        public static Color RandomPastel()
        {
            return Color.HSVToRGB(
                Random.Range(0f, 1f),
                Random.Range(0.2f, 0.4f),  // 低饱和
                Random.Range(0.8f, 1f)      // 高亮度
            );
        }
        /// <summary>
        /// 生成暗色调
        /// </summary>
        /// <returns>饱和度0.5-0.8、亮度0.2-0.4的暗色调颜色</returns>
        public static Color RandomDark()
        {
            return Color.HSVToRGB(
                Random.Range(0f, 1f),
                Random.Range(0.5f, 0.8f),
                Random.Range(0.2f, 0.4f)   // 低亮度
            );
        }
        /// <summary>
        /// 调整颜色的饱和度
        /// </summary>
        /// <param name="color">原始颜色</param>
        /// <param name="saturation">目标饱和度，0-1之间</param>
        /// <returns>调整后的颜色</returns>
        public static Color SetSaturation(Color color, float saturation)
        {
            Color.RGBToHSV(color, out float h, out float _, out float v);
            saturation = Mathf.Clamp01(saturation);
            return Color.HSVToRGB(h, saturation, v);
        }
        /// <summary>
        /// 按比例降低饱和度
        /// </summary>
        public static Color ReduceSaturation(Color color, float reductionFactor)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            s = Mathf.Clamp01(s * reductionFactor);
            return Color.HSVToRGB(h, s, v);
        }
        #endregion

        #region Utilitys
        /// <summary>
        /// 清理 GameObject 名称的后缀（末尾的空格、括号及括号内数字）
        /// 例如: "Cube (1)" -> "Cube", "Player  (2)" -> "Player", "Enemy  " -> "Enemy"
        /// </summary>
        public static string GameObject_SuffixName_Cleaner(GameObject obj)
        {
            if (obj == null) return string.Empty;

            string name = obj.name;

            // 去除末尾空格
            name = name.TrimEnd();

            // 去除末尾的括号及括号内数字，例如 " (123)", "(45)"
            name = Regex.Replace(name, @"\s*\(\d+\)$", "");

            return name;
        }
        /// <summary>
        /// 生成一个不与目标列表中ID重复的ID字符串
        /// </summary>
        /// <param name="existingIds">已有的ID列表（用于检查重复）</param>
        /// <param name="length">ID长度，默认为4位</param>
        /// <returns>不重复的ID字符串（包含小写字母、大写字母和数字）</returns>
        public static string GenerateUniqueId(string[] existingIds, int length = 4)
        {
            // 忽略 i  l  j  1  这几个容易导致视觉观测错误的字符
            const string chars = "abcdefghkmnopqrstuvwxyz023456789";
            string newId;
            int maxAttempts = 100; // 最大尝试次数，防止无限循环

            do
            {
                // 生成随机ID
                char[] stringChars = new char[length];
                for (int i = 0; i < length; i++)
                {
                    stringChars[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
                }
                newId = new string(stringChars);

                maxAttempts--;
                if (maxAttempts <= 0)
                {
                    // 如果尝试次数过多，增加ID长度以确保唯一性
                    Debug.LogWarning($"无法生成唯一ID，已尝试多次。请检查ID列表是否过大或增加ID长度。");
                    return GenerateUniqueId(existingIds, length + 1);
                }

            } while (existingIds != null && existingIds.Contains(newId));

            return newId;
        }
        /// <summary>
        /// 去除字符串左右小括号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ParenthesesCleaner(string str)
        {
            if (str.StartsWith("(") && str.EndsWith(")"))
            {
                return str.Substring(1, str.Length - 2);
            }
            return str;
        }
        /// <summary>
        /// 拷贝字符串内容到系统剪贴板
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CopyToSystemBuffer(string str)
        {
            GUIUtility.systemCopyBuffer = str;
            return str;
        }
        /// <summary>
        /// 处理转义字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Unescape(string input)
        {
            return input.Replace("\\n", "\n");
        }
        /// <summary>
        /// 将所选的物体进行排序0-9   /   A-正向Z轴
        /// </summary>
        public static void Sorting(GameObject[] Objects)
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
                    //The condition, in this case, name being less then
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
        /// 优化Unity实例化出来的物体的(Clone)的标记名称
        /// </summary>
        /// <returns>返回不带(Clone)后缀的原物体名称.</returns>
        /// <param name="RemoveString">要处理的字符串.</param>
        public static string RemoveCloneSuffix(string RemoveString)
        {
            string str = RemoveString.Remove(RemoveString.Length - 7, RemoveString.Length - (RemoveString.Length - 7));
            return str;
        }
        #endregion

        #region Float Fillter
        /// <summary>
        /// 获取 Float 数组中的最小值。
        /// </summary>
        /// <param name="values">源数组，不能为 null 或空。</param>
        /// <returns>数组中的最小值。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 null。</exception>
        /// <exception cref="ArgumentException"><paramref name="values"/> 为空数组。</exception>
        public static float MinValue(float[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length == 0) throw new ArgumentException("数组不能为空。", nameof(values));

            float min = float.MaxValue;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < min) min = values[i];
            }
            return min;
        }
        /// <summary>
        /// 获取 Float 数组中的最大值。
        /// </summary>
        /// <param name="values">源数组，不能为 null 或空。</param>
        /// <returns>数组中的最大值。</returns>
        public static float MaxValue(float[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length == 0) throw new ArgumentException("数组不能为空。", nameof(values));

            float max = float.MinValue;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > max) max = values[i];
            }
            return max;
        }
        /// <summary>
        /// 获取 Float 数组中的算术平均值。
        /// </summary>
        /// <param name="values">源数组，不能为 null 或空。</param>
        /// <returns>数组所有元素的算术平均值。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 null。</exception>
        /// <exception cref="ArgumentException"><paramref name="values"/> 为空数组。</exception>
        public static float Average(float[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length == 0) throw new ArgumentException("数组不能为空。", nameof(values));

            double sum = 0.0;                     // 用 double 累加，避免 float 精度丢失
            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i];
            }
            return (float)(sum / values.Length);
        }
        /// <summary>
        /// 获取 Float 数组中所有负数值。
        /// </summary>
        /// <param name="values">源数组，不能为 null。</param>
        /// <returns>由所有负数组成的数组；若无负数则返回空数组。</returns>
        public static float[] Negative(float[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            var result = new List<float>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < 0f) result.Add(values[i]);
            }
            return result.ToArray();
        }
        #endregion

        #region Int Fillter
        /// <summary>
        /// 获取 Int 数组中的最小值。
        /// </summary>
        /// <param name="values">源数组，不能为 null 或空。</param>
        /// <returns>数组中的最小值。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 null。</exception>
        /// <exception cref="ArgumentException"><paramref name="values"/> 为空数组。</exception>
        public static int MinValue(int[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length == 0) throw new ArgumentException("数组不能为空。", nameof(values));

            int min = int.MaxValue;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < min) min = values[i];
            }
            return min;
        }
        /// <summary>
        /// 获取 Int 数组中的最大值。
        /// </summary>
        /// <param name="values">源数组，不能为 null 或空。</param>
        /// <returns>数组中的最大值。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 null。</exception>
        /// <exception cref="ArgumentException"><paramref name="values"/> 为空数组。</exception>
        public static int MaxValue(int[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length == 0) throw new ArgumentException("数组不能为空。", nameof(values));

            int max = int.MinValue;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > max) max = values[i];
            }
            return max;
        }
        /// <summary>
        /// 获取 Int 数组中的算术平均值。
        /// </summary>
        /// <param name="values">源数组，不能为 null 或空。</param>
        /// <returns>数组所有元素的算术平均值。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 null。</exception>
        /// <exception cref="ArgumentException"><paramref name="values"/> 为空数组。</exception>
        public static double Average(int[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length == 0) throw new ArgumentException("数组不能为空。", nameof(values));

            long sum = 0L;                        // 用 long 累加，避免 int 溢出
            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i];
            }
            return (double)sum / values.Length;   // 返回 double，保留小数
        }
        /// <summary>
        /// 获取 Int 数组中所有负数值。
        /// </summary>
        /// <param name="values">源数组，不能为 null。</param>
        /// <returns>由所有负数组成的数组；若无负数则返回空数组。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 null。</exception>
        public static int[] Negative(int[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            var result = new List<int>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < 0) result.Add(values[i]);
            }
            return result.ToArray();
        }
        /// <summary>
        /// 获取 Int 数组中所有奇数值。
        /// </summary>
        /// <param name="values">源数组，不能为 null。</param>
        /// <returns>由所有奇数组成的数组；若无奇数则返回空数组。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 null。</exception>
        public static int[] Odd(int[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            var result = new List<int>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] % 2 != 0) result.Add(values[i]);
            }
            return result.ToArray();
        }
        /// <summary>
        /// 获取 Int 数组中所有偶数值。
        /// </summary>
        /// <param name="values">源数组，不能为 null。</param>
        /// <returns>由所有偶数组成的数组；若无偶数则返回空数组。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 null。</exception>
        public static int[] Even(int[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            var result = new List<int>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] % 2 == 0) result.Add(values[i]);
            }
            return result.ToArray();
        }
        /// <summary>
        /// 获取 Int 数组中的众数（出现次数最多的元素）。
        /// <para>若存在多个出现次数相同的元素，返回其中<b>最先达到该次数</b>的那个。</para>
        /// </summary>
        /// <param name="values">源数组，不能为 null 或空。</param>
        /// <returns>数组中出现次数最多的元素。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 null。</exception>
        /// <exception cref="ArgumentException"><paramref name="values"/> 为空数组。</exception>
        /// <example>
        /// 示例：
        /// <code>
        /// Mode(new int[] { 1, 2, 2, 3, 3, 3 })  // 返回 3
        /// Mode(new int[] { 1, 1, 2, 2 })         // 返回 1（并列时取最先达到的）
        /// Mode(new int[] { 5 })                  // 返回 5
        /// </code>
        /// </example>
        public static int Mode(int[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length == 0) throw new ArgumentException("数组不能为空。", nameof(values));

            var freq = new Dictionary<int, int>();
            int best = values[0], bestCount = 0;
            for (int i = 0; i < values.Length; i++)
            {
                freq.TryGetValue(values[i], out int c);
                c++;
                freq[values[i]] = c;
                if (c > bestCount) { bestCount = c; best = values[i]; }
            }
            return best;
        }
        /// <summary>
        /// 在 Int 数组中查找指定值第一次出现的索引。
        /// </summary>
        /// <param name="values">源数组，不能为 null（可为空数组）。</param>
        /// <param name="target">要查找的目标值。</param>
        /// <returns>
        /// 目标值第一次出现的从 0 开始的索引；
        /// 若数组中不存在该值，返回 -1。
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> 为 null。</exception>
        /// <example>
        /// 示例：
        /// <code>
        /// IndexOf(new int[] { 10, 20, 30, 20 }, 20)  // 返回 1
        /// IndexOf(new int[] { 10, 20, 30, 20 }, 99)  // 返回 -1
        /// IndexOf(new int[0], 1)                      // 返回 -1（空数组不报错）
        /// </code>
        /// </example>
        public static int IndexOf(int[] values, int target)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            for (int i = 0; i < values.Length; i++)
                if (values[i] == target) return i;
            return -1;   // 惯例：找不到返回 -1
        }
        #endregion

        #region Message
        /// <summary>
        /// 打印消息到控制台
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="mode">消息类型</param>
        public static XGUIMsgState Console(string title, string content, XGUIMsgState mode, GameObject xobject = null)
        {
            if (!PrintMsgEnable)
                return XGUIMsgState.未开启消息模块功能;

            switch (mode)
            {
                case XGUIMsgState.确认:
                    Debug.Log("<color=#c4c4c4>" + title + "： </color>" + "┠─<color=#c3e55c>" + content + "</color>", xobject);
                    break;
                case XGUIMsgState.错误:
                    Debug.Log("<color=#c4c4c4>" + title + "： </color>" + "┠─<color=#f05f5f>" + content + "</color>", xobject);
                    break;
                case XGUIMsgState.通知:
                    Debug.Log("<color=#c4c4c4>" + title + "： </color>" + "┠─<color=#bebebe>" + content + "</color>", xobject);
                    break;
                case XGUIMsgState.警告:
                    Debug.Log("<color=#c4c4c4>" + title + "： </color>" + "┠─<color=#ffb80e>" + content + "</color>", xobject);
                    break;
                case XGUIMsgState.设置:
                    Debug.Log("<color=#c4c4c4>" + title + "： </color>" + "┠─<color=#5cc1e5>" + content + "</color>", xobject);
                    break;
            }
            return mode;
        }
        #endregion

        #region Path
        /// <summary>
        /// 将路径转换为相对于项目的路径（如果路径在项目目录内）。
        /// </summary>
        /// <param name="path">完整路径。</param>
        /// <returns>相对路径或原路径。</returns>
        public static string ToRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            string dataPath = Application.dataPath;
            // 确保路径使用正斜杠以便比较
            string normalizedPath = path.Replace('\\', '/');
            string normalizedDataPath = dataPath.Replace('\\', '/');

            if (normalizedPath.StartsWith(normalizedDataPath))
            {
                return "Assets" + normalizedPath.Substring(normalizedDataPath.Length);
            }

            return path;
        }
        /// <summary>
        /// 将相对路径转换为绝对路径。
        /// </summary>
        /// <param name="path">相对路径（如 "Assets/...").</param>
        /// <returns>绝对路径。</returns>
        public static string ToAbsolutePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            if (path.StartsWith("Assets/") || path.StartsWith("Assets\\"))
            {
                string dataPath = Application.dataPath.Replace('\\', '/');
                return dataPath + "/" + path.Substring(7);
            }

            return path;
        }
        /// <summary>
        /// 验证路径是否存在（文件或文件夹）。
        /// </summary>
        /// <param name="path">要验证的路径。</param>
        /// <returns>如果路径存在则返回 <c>true</c>。</returns>
        public static bool PathExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return System.IO.File.Exists(path) || System.IO.Directory.Exists(path);
        }
        /// <summary>
        /// 检查路径是否在项目目录内。
        /// </summary>
        /// <param name="path">要检查的路径。</param>
        /// <returns>如果在项目目录内则返回 <c>true</c>。</returns>
        public static bool IsPathInProject(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            string dataPath = Application.dataPath.Replace('\\', '/');
            string normalizedPath = path.Replace('\\', '/');

            return normalizedPath.StartsWith(dataPath);
        }
        #endregion

        #region StopWatch
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

        #region RuntimeData
        /// <summary>
        /// 数据是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool x_Runtime_Data_Has_String(string key)
        {
            if (PlayerPrefs.HasKey(key))
                return true;
            else return false;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static float x_Runtime_Data_Get_With_Float(string key)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetFloat(key);
            else return 0;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string x_Runtime_Data_Get_With_String(string key)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetString(key);
            else return null;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int x_Runtime_Data_Get_With_Int(string key)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetInt(key);
            else return 0;
        }
        /// <summary>
        /// 存入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void x_Runtime_Data_Set_With_Float(string key, float data)
        {
            PlayerPrefs.SetFloat(key, data);
        }
        /// <summary>
        /// 存入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void x_Runtime_Data_Set_With_String(string key, string data)
        {
            PlayerPrefs.SetString(key, data);
        }
        /// <summary>
        /// 存入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void x_Runtime_Data_Set_With_Int(string key, int data)
        {
            PlayerPrefs.SetInt(key, data);
        }
        /// <summary>
        /// 清空数据
        /// </summary>
        /// <param name="key"></param>
        public static void x_Runtime_Data_Clear(string key)
        {
            if (PlayerPrefs.HasKey(key))
                PlayerPrefs.DeleteKey(key);
        }
        #endregion

        #region Excute
        /// <summary>
        /// 打开网址
        /// </summary>
        /// <param name="url"></param>
        public static void OpenURL(string url)
        {
            // 使用Process.Start打开URL
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        #endregion

        #region Web

        #endregion

        #region DataBase

        #endregion
    }
}