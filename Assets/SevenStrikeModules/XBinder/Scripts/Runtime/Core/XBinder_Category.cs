/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XBinder - Unity 高级数据绑定管理插件
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
namespace SevenStrikeModules.XBinder
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class XBinder_Category
    {
        #region 字段
        /// <summary>
        /// 给面板下拉菜单看的数据源名称
        /// </summary>
        public string display_name = "未命名数据源";
        #endregion

        #region 存储通道
        /// <summary>
        /// 存储所有 float 类型数据的字典
        /// </summary>
        private readonly Dictionary<string, float> _floats = new();
        /// <summary>
        /// 存储所有 int 类型数据的字典
        /// </summary>
        private readonly Dictionary<string, int> _ints = new();
        /// <summary>
        /// 存储所有 bool 类型数据的字典
        /// </summary>
        private readonly Dictionary<string, bool> _bools = new();
        /// <summary>
        /// 存储所有 string 类型数据的字典
        /// </summary>
        private readonly Dictionary<string, string> _strings = new();
        /// <summary>
        /// 存储所有 Color 类型数据的字典
        /// </summary>
        private readonly Dictionary<string, Color> _colors = new();
        /// <summary>
        /// 存储所有 Vector2 类型数据的字典
        /// </summary>
        private readonly Dictionary<string, Vector2> _vector2s = new();
        /// <summary>
        /// 存储所有 Vector3 类型数据的字典
        /// </summary>
        private readonly Dictionary<string, Vector3> _vector3s = new();
        /// <summary>
        /// 存储所有 Vector4 类型数据的字典
        /// </summary>
        private readonly Dictionary<string, Vector4> _vector4s = new();
        /// <summary>
        /// 存储所有 Quaternion 类型数据的字典
        /// </summary>
        private readonly Dictionary<string, Quaternion> _quaternions = new();
        /// <summary>
        /// 存储所有 Rect 类型数据的字典
        /// </summary>
        private readonly Dictionary<string, Rect> _rects = new();
        /// <summary>
        /// 存储所有 RectOffset 类型数据的字典
        /// </summary>
        private readonly Dictionary<string, RectOffset> _rectoffsets = new();
        /// <summary>
        /// 存储所有 Object 类型数据的字典
        /// </summary>
        private readonly Dictionary<string, object> _objects = new();
        #endregion

        #region 订阅通道
        /// <summary>
        /// float 类型数据的订阅者列表字典
        /// </summary>
        private readonly Dictionary<string, List<Action<float>>> _subs_float = new();
        /// <summary>
        /// int 类型数据的订阅者列表字典
        /// </summary>
        private readonly Dictionary<string, List<Action<int>>> _subs_int = new();
        /// <summary>
        /// bool 类型数据的订阅者列表字典
        /// </summary>
        private readonly Dictionary<string, List<Action<bool>>> _subs_bool = new();
        /// <summary>
        /// string 类型数据的订阅者列表字典
        /// </summary>
        private readonly Dictionary<string, List<Action<string>>> _subs_string = new();
        /// <summary>
        /// Color 类型数据的订阅者列表字典
        /// </summary>
        private readonly Dictionary<string, List<Action<Color>>> _subs_color = new();
        /// <summary>
        /// Vector2 类型数据的订阅者列表字典
        /// </summary>
        private readonly Dictionary<string, List<Action<Vector2>>> _subs_vector2 = new();
        /// <summary>
        /// Vector3 类型数据的订阅者列表字典
        /// </summary>
        private readonly Dictionary<string, List<Action<Vector3>>> _subs_vector3 = new();
        /// <summary>
        /// Vector4 类型数据的订阅者列表字典
        /// </summary>
        private readonly Dictionary<string, List<Action<Vector4>>> _subs_vector4 = new();
        /// <summary>
        /// Quaternion 类型数据的订阅者列表字典
        /// </summary>
        private readonly Dictionary<string, List<Action<Quaternion>>> _subs_quaternion = new();
        /// <summary>
        /// Rect 类型数据的订阅者列表字典
        /// </summary>
        private readonly Dictionary<string, List<Action<Rect>>> _subs_rect = new();
        /// <summary>
        /// RectPffset 类型数据的订阅者列表字典
        /// </summary>
        private readonly Dictionary<string, List<Action<RectOffset>>> _subs_rectoffset = new();
        /// <summary>
        /// Object 类型数据的订阅者列表字典
        /// </summary>
        private readonly Dictionary<string, List<Action<object>>> _subs_object = new();
        #endregion

        #region 赋值
        /// <summary>
        /// 设置 float 类型数据，并在值改变时通知订阅者
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="value">数据值</param>
        public void set_float(string key, float value)
        {
            if (_floats.TryGetValue(key, out float old) && Math.Abs(old - value) < 0.0001f) return;
            _floats[key] = value;
            if (_subs_float.TryGetValue(key, out var list))
                for (int i = list.Count - 1; i >= 0; i--) { if (i >= list.Count) continue; list[i]?.Invoke(value); }
        }
        /// <summary>
        /// 设置 int 类型数据，并在值改变时通知订阅者
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="value">数据值</param>
        public void set_int(string key, int value)
        {
            if (_ints.TryGetValue(key, out int old) && old == value) return;
            _ints[key] = value;
            if (_subs_int.TryGetValue(key, out var list))
                for (int i = list.Count - 1; i >= 0; i--) { if (i >= list.Count) continue; list[i]?.Invoke(value); }
        }
        /// <summary>
        /// 设置 bool 类型数据，并在值改变时通知订阅者
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="value">数据值</param>
        public void set_bool(string key, bool value)
        {
            if (_bools.TryGetValue(key, out bool old) && old == value) return;
            _bools[key] = value;
            if (_subs_bool.TryGetValue(key, out var list))
                for (int i = list.Count - 1; i >= 0; i--) { if (i >= list.Count) continue; list[i]?.Invoke(value); }
        }
        /// <summary>
        /// 设置 string 类型数据，并在值改变时通知订阅者
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="value">数据值</param>
        public void set_string(string key, string value)
        {
            if (_strings.TryGetValue(key, out string old) && old == value) return;
            _strings[key] = value;
            if (_subs_string.TryGetValue(key, out var list))
                for (int i = list.Count - 1; i >= 0; i--) { if (i >= list.Count) continue; list[i]?.Invoke(value); }
        }
        /// <summary>
        /// 设置 Color 类型数据，并在值改变时通知订阅者
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="value">数据值</param>
        public void set_color(string key, Color value)
        {
            if (_colors.TryGetValue(key, out Color old) && old == value) return;
            _colors[key] = value;
            if (_subs_color.TryGetValue(key, out var list))
                for (int i = list.Count - 1; i >= 0; i--) { if (i >= list.Count) continue; list[i]?.Invoke(value); }
        }
        /// <summary>
        /// 设置 Vector2 类型数据，并在值改变时通知订阅者
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="value">数据值</param>
        public void set_vector2(string key, Vector2 value)
        {
            if (_vector2s.TryGetValue(key, out Vector2 old) && old == value) return;
            _vector2s[key] = value;
            if (_subs_vector2.TryGetValue(key, out var list))
                for (int i = list.Count - 1; i >= 0; i--) { if (i >= list.Count) continue; list[i]?.Invoke(value); }
        }
        /// <summary>
        /// 设置 Vector3 类型数据，并在值改变时通知订阅者
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="value">数据值</param>
        public void set_vector3(string key, Vector3 value)
        {
            if (_vector3s.TryGetValue(key, out Vector3 old) && old == value) return;
            _vector3s[key] = value;
            if (_subs_vector3.TryGetValue(key, out var list))
                for (int i = list.Count - 1; i >= 0; i--) { if (i >= list.Count) continue; list[i]?.Invoke(value); }
        }
        /// <summary>
        /// 设置 Vector4 类型数据，并在值改变时通知订阅者
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="value">数据值</param>
        public void set_vector4(string key, Vector4 value)
        {
            if (_vector4s.TryGetValue(key, out Vector4 old) && old == value) return;
            _vector4s[key] = value;
            if (_subs_vector4.TryGetValue(key, out var list))
                for (int i = list.Count - 1; i >= 0; i--) { if (i >= list.Count) continue; list[i]?.Invoke(value); }
        }
        /// <summary>
        /// 设置 Quaternion 类型数据，并在值改变时通知订阅者
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="value">数据值</param>
        public void set_quaternion(string key, Quaternion value)
        {
            if (_quaternions.TryGetValue(key, out Quaternion old) && old == value) return;
            _quaternions[key] = value;
            if (_subs_quaternion.TryGetValue(key, out var list))
                for (int i = list.Count - 1; i >= 0; i--) { if (i >= list.Count) continue; list[i]?.Invoke(value); }
        }
        /// <summary>
        /// 设置 Rect 类型数据，并在值改变时通知订阅者
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="value">数据值</param>
        public void set_rect(string key, Rect value)
        {
            if (_rects.TryGetValue(key, out Rect old) && old == value) return;
            _rects[key] = value;
            if (_subs_rect.TryGetValue(key, out var list))
                for (int i = list.Count - 1; i >= 0; i--) { if (i >= list.Count) continue; list[i]?.Invoke(value); }
        }
        /// <summary>
        /// 设置 RectOffset 类型数据，并在值改变时通知订阅者
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="value">数据值</param>
        public void set_rectoffset(string key, RectOffset value)
        {
            if (_rectoffsets.TryGetValue(key, out RectOffset old) && old == value) return;
            _rectoffsets[key] = value;
            if (_subs_rectoffset.TryGetValue(key, out var list))
                for (int i = list.Count - 1; i >= 0; i--) { if (i >= list.Count) continue; list[i]?.Invoke(value); }
        }
        /// <summary>
        /// 设置 Object 类型数据，并在值改变时通知订阅者
        /// </summary>
        public void set_object(string key, object value)
        {
            if (_objects.TryGetValue(key, out var old) && old == value) return;
            _objects[key] = value;
            if (_subs_object.TryGetValue(key, out var list))
                for (int i = list.Count - 1; i >= 0; i--) { if (i >= list.Count) continue; list[i]?.Invoke(value); }
        }
        #endregion

        #region 获取
        /// <summary>
        /// 获取 float 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <returns>数据值，未找到则返回 0f</returns>
        public float get_float(string key) => _floats.TryGetValue(key, out var v) ? v : 0f;
        /// <summary>
        /// 获取 int 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <returns>数据值，未找到则返回 0</returns>
        public int get_int(string key) => _ints.TryGetValue(key, out var v) ? v : 0;
        /// <summary>
        /// 获取 bool 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <returns>数据值，未找到则返回 false</returns>
        public bool get_bool(string key) => _bools.TryGetValue(key, out var v) && v;
        /// <summary>
        /// 获取 string 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <returns>数据值，未找到则返回 null</returns>
        public string get_string(string key) => _strings.TryGetValue(key, out var v) ? v : null;
        /// <summary>
        /// 获取 Color 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <returns>数据值，未找到则返回 default</returns>
        public Color get_color(string key) => _colors.TryGetValue(key, out var v) ? v : default;
        /// <summary>
        /// 获取 Vector2 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <returns>数据值，未找到则返回 default</returns>
        public Vector2 get_vector2(string key) => _vector2s.TryGetValue(key, out var v) ? v : default;
        /// <summary>
        /// 获取 Vector3 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <returns>数据值，未找到则返回 default</returns>
        public Vector3 get_vector3(string key) => _vector3s.TryGetValue(key, out var v) ? v : default;
        /// <summary>
        /// 获取 Vector4 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <returns>数据值，未找到则返回 default</returns>
        public Vector4 get_vector4(string key) => _vector4s.TryGetValue(key, out var v) ? v : default;
        /// <summary>
        /// 获取 Quaternion 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <returns>数据值，未找到则返回 default</returns>
        public Quaternion get_quaternion(string key) => _quaternions.TryGetValue(key, out var v) ? v : default;
        /// <summary>
        /// 获取 Rect 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <returns>数据值，未找到则返回 default</returns>
        public Rect get_rect(string key) => _rects.TryGetValue(key, out var v) ? v : default;
        /// <summary>
        /// 获取 RectOffset 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <returns>数据值，未找到则返回 default</returns>
        public RectOffset get_rectoffset(string key) => _rectoffsets.TryGetValue(key, out var v) ? v : default;
        /// <summary>
        /// 获取 Object 类型数据
        /// </summary>
        public object get_object(string key) => _objects.TryGetValue(key, out var v) ? v : null;
        #endregion

        #region 订阅
        /// <summary>
        /// 订阅 float 类型数据变化
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">数据变化时的回调</param>
        public void subscribe_float(string key, Action<float> cb)
        {
            if (!_subs_float.TryGetValue(key, out var list)) { list = new List<Action<float>>(); _subs_float[key] = list; }
            list.Add(cb);
        }
        /// <summary>
        /// 订阅 int 类型数据变化
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">数据变化时的回调</param>
        public void subscribe_int(string key, Action<int> cb)
        {
            if (!_subs_int.TryGetValue(key, out var list)) { list = new List<Action<int>>(); _subs_int[key] = list; }
            list.Add(cb);
        }
        /// <summary>
        /// 订阅 bool 类型数据变化
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">数据变化时的回调</param>
        public void subscribe_bool(string key, Action<bool> cb)
        {
            if (!_subs_bool.TryGetValue(key, out var list)) { list = new List<Action<bool>>(); _subs_bool[key] = list; }
            list.Add(cb);
        }
        /// <summary>
        /// 订阅 string 类型数据变化
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">数据变化时的回调</param>
        public void subscribe_string(string key, Action<string> cb)
        {
            if (!_subs_string.TryGetValue(key, out var list)) { list = new List<Action<string>>(); _subs_string[key] = list; }
            list.Add(cb);
        }
        /// <summary>
        /// 订阅 Color 类型数据变化
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">数据变化时的回调</param>
        public void subscribe_color(string key, Action<Color> cb)
        {
            if (!_subs_color.TryGetValue(key, out var list)) { list = new List<Action<Color>>(); _subs_color[key] = list; }
            list.Add(cb);
        }
        /// <summary>
        /// 订阅 Vector2 类型数据变化
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">数据变化时的回调</param>
        public void subscribe_vector2(string key, Action<Vector2> cb)
        {
            if (!_subs_vector2.TryGetValue(key, out var list)) { list = new List<Action<Vector2>>(); _subs_vector2[key] = list; }
            list.Add(cb);
        }
        /// <summary>
        /// 订阅 Vector3 类型数据变化
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">数据变化时的回调</param>
        public void subscribe_vector3(string key, Action<Vector3> cb)
        {
            if (!_subs_vector3.TryGetValue(key, out var list)) { list = new List<Action<Vector3>>(); _subs_vector3[key] = list; }
            list.Add(cb);
        }
        /// <summary>
        /// 订阅 Vector4 类型数据变化
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">数据变化时的回调</param>
        public void subscribe_vector4(string key, Action<Vector4> cb)
        {
            if (!_subs_vector4.TryGetValue(key, out var list)) { list = new List<Action<Vector4>>(); _subs_vector4[key] = list; }
            list.Add(cb);
        }
        /// <summary>
        /// 订阅 Quaternion 类型数据变化
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">数据变化时的回调</param>
        public void subscribe_quaternion(string key, Action<Quaternion> cb)
        {
            if (!_subs_quaternion.TryGetValue(key, out var list)) { list = new List<Action<Quaternion>>(); _subs_quaternion[key] = list; }
            list.Add(cb);
        }
        /// <summary>
        /// 订阅 Rect 类型数据变化
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">数据变化时的回调</param>
        public void subscribe_rect(string key, Action<Rect> cb)
        {
            if (!_subs_rect.TryGetValue(key, out var list)) { list = new List<Action<Rect>>(); _subs_rect[key] = list; }
            list.Add(cb);
        }
        /// <summary>
        /// 订阅 RectOffset 类型数据变化
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">数据变化时的回调</param>
        public void subscribe_rectoffset(string key, Action<RectOffset> cb)
        {
            if (!_subs_rectoffset.TryGetValue(key, out var list)) { list = new List<Action<RectOffset>>(); _subs_rectoffset[key] = list; }
            list.Add(cb);
        }
        /// <summary>
        /// 订阅 Object 类型数据变化
        /// </summary>
        public void subscribe_object(string key, Action<object> cb)
        {
            if (!_subs_object.TryGetValue(key, out var list)) { list = new List<Action<object>>(); _subs_object[key] = list; }
            list.Add(cb);
        }
        #endregion

        #region 取消订阅
        /// <summary>
        /// 取消订阅 float 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">要移除的回调</param>
        public void unsubscribe_float(string key, Action<float> cb)
        {
            if (_subs_float.TryGetValue(key, out var list)) list.Remove(cb);
        }
        /// <summary>
        /// 取消订阅 int 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">要移除的回调</param>
        public void unsubscribe_int(string key, Action<int> cb)
        {
            if (_subs_int.TryGetValue(key, out var list)) list.Remove(cb);
        }
        /// <summary>
        /// 取消订阅 bool 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">要移除的回调</param>
        public void unsubscribe_bool(string key, Action<bool> cb)
        {
            if (_subs_bool.TryGetValue(key, out var list)) list.Remove(cb);
        }
        /// <summary>
        /// 取消订阅 string 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">要移除的回调</param>
        public void unsubscribe_string(string key, Action<string> cb)
        {
            if (_subs_string.TryGetValue(key, out var list)) list.Remove(cb);
        }
        /// <summary>
        /// 取消订阅 Color 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">要移除的回调</param>
        public void unsubscribe_color(string key, Action<Color> cb)
        {
            if (_subs_color.TryGetValue(key, out var list)) list.Remove(cb);
        }
        /// <summary>
        /// 取消订阅 Vector2 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">要移除的回调</param>
        public void unsubscribe_vector2(string key, Action<Vector2> cb)
        {
            if (_subs_vector2.TryGetValue(key, out var list)) list.Remove(cb);
        }
        /// <summary>
        /// 取消订阅 Vector3 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">要移除的回调</param>
        public void unsubscribe_vector3(string key, Action<Vector3> cb)
        {
            if (_subs_vector3.TryGetValue(key, out var list)) list.Remove(cb);
        }
        /// <summary>
        /// 取消订阅 Vector4 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">要移除的回调</param>
        public void unsubscribe_vector4(string key, Action<Vector4> cb)
        {
            if (_subs_vector4.TryGetValue(key, out var list)) list.Remove(cb);
        }
        /// <summary>
        /// 取消订阅 Quaternion 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">要移除的回调</param>
        public void unsubscribe_quaternion(string key, Action<Quaternion> cb)
        {
            if (_subs_quaternion.TryGetValue(key, out var list)) list.Remove(cb);
        }
        /// <summary>
        /// 取消订阅 Rect 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">要移除的回调</param>
        public void unsubscribe_rect(string key, Action<Rect> cb)
        {
            if (_subs_rect.TryGetValue(key, out var list)) list.Remove(cb);
        }
        /// <summary>
        /// 取消订阅 RectOffset 类型数据
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="cb">要移除的回调</param>
        public void unsubscribe_rectoffset(string key, Action<RectOffset> cb)
        {
            if (_subs_rectoffset.TryGetValue(key, out var list)) list.Remove(cb);
        }
        /// <summary>
        /// 取消订阅 Object 类型数据
        /// </summary>
        public void unsubscribe_object(string key, Action<object> cb)
        {
            if (_subs_object.TryGetValue(key, out var list)) list.Remove(cb);
        }
        #endregion

        #region Editor 只读视图（仅供编辑器面板使用，运行时不会调用）
#if UNITY_EDITOR
        /// <summary>
        /// 获取当前所有已存在的键（跨 11 种类型去重后的并集）
        /// </summary>
        /// <returns>键名集合</returns>
        public HashSet<string> get_all_keys()
        {
            var set = new HashSet<string>();
            foreach (var k in _floats.Keys) set.Add(k);
            foreach (var k in _ints.Keys) set.Add(k);
            foreach (var k in _bools.Keys) set.Add(k);
            foreach (var k in _strings.Keys) set.Add(k);
            foreach (var k in _colors.Keys) set.Add(k);
            foreach (var k in _vector2s.Keys) set.Add(k);
            foreach (var k in _vector3s.Keys) set.Add(k);
            foreach (var k in _vector4s.Keys) set.Add(k);
            foreach (var k in _quaternions.Keys) set.Add(k);
            foreach (var k in _rects.Keys) set.Add(k);
            foreach (var k in _rectoffsets.Keys) set.Add(k);
            foreach (var k in _objects.Keys) set.Add(k);
            return set;
        }

        /// <summary>
        /// 获取某个键所属的类型通道
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>类型枚举；若键不存在返回 None</returns>
        public XBinder_ValueType get_key_type(string key)
        {
            if (_floats.ContainsKey(key)) return XBinder_ValueType.Float;
            if (_ints.ContainsKey(key)) return XBinder_ValueType.Int;
            if (_bools.ContainsKey(key)) return XBinder_ValueType.Bool;
            if (_strings.ContainsKey(key)) return XBinder_ValueType.String;
            if (_colors.ContainsKey(key)) return XBinder_ValueType.Color;
            if (_vector2s.ContainsKey(key)) return XBinder_ValueType.Vector2;
            if (_vector3s.ContainsKey(key)) return XBinder_ValueType.Vector3;
            if (_vector4s.ContainsKey(key)) return XBinder_ValueType.Vector4;
            if (_quaternions.ContainsKey(key)) return XBinder_ValueType.Quaternion;
            if (_rects.ContainsKey(key)) return XBinder_ValueType.Rect;
            if (_rectoffsets.ContainsKey(key)) return XBinder_ValueType.RectOffset;
            if (_objects.ContainsKey(key)) return XBinder_ValueType.Object;
            return XBinder_ValueType.None;
        }

        /// <summary>
        /// 以 object 形式取任意键的值（仅 Editor 显示用，会有装箱）
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>值；未找到返回 null</returns>
        public object get_value_as_object(string key)
        {
            if (_floats.TryGetValue(key, out var f)) return f;
            if (_ints.TryGetValue(key, out var i)) return i;
            if (_bools.TryGetValue(key, out var b)) return b;
            if (_strings.TryGetValue(key, out var s)) return s;
            if (_colors.TryGetValue(key, out var c)) return c;
            if (_vector2s.TryGetValue(key, out var v2)) return v2;
            if (_vector3s.TryGetValue(key, out var v3)) return v3;
            if (_vector4s.TryGetValue(key, out var v4)) return v4;
            if (_quaternions.TryGetValue(key, out var q)) return q;
            if (_rects.TryGetValue(key, out var r)) return r;
            if (_rectoffsets.TryGetValue(key, out var ro)) return ro;
            if (_objects.TryGetValue(key, out var o)) return o;
            return null;
        }

        /// <summary>
        /// 获取某个键的订阅者总数（跨类型求和）
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>订阅者数量</returns>
        public int get_subscriber_count(string key)
        {
            int n = 0;
            if (_subs_float.TryGetValue(key, out var l1)) n += l1.Count;
            if (_subs_int.TryGetValue(key, out var l2)) n += l2.Count;
            if (_subs_bool.TryGetValue(key, out var l3)) n += l3.Count;
            if (_subs_string.TryGetValue(key, out var l4)) n += l4.Count;
            if (_subs_color.TryGetValue(key, out var l5)) n += l5.Count;
            if (_subs_vector2.TryGetValue(key, out var l6)) n += l6.Count;
            if (_subs_vector3.TryGetValue(key, out var l7)) n += l7.Count;
            if (_subs_vector4.TryGetValue(key, out var l8)) n += l8.Count;
            if (_subs_quaternion.TryGetValue(key, out var l9)) n += l9.Count;
            if (_subs_rect.TryGetValue(key, out var l10)) n += l10.Count;
            if (_subs_object.TryGetValue(key, out var l11)) n += l11.Count;
            if (_subs_rectoffset.TryGetValue(key, out var l12)) n += l12.Count;
            return n;
        }

        /// <summary>
        /// 获取当前通道的总数（各类型字典 Count 之和，跨类型同键会重复计数）
        /// </summary>
        /// <returns>通道条目总数</returns>
        public int get_key_count()
        {
            return _floats.Count + _ints.Count + _bools.Count + _strings.Count
                 + _colors.Count + _vector2s.Count + _vector3s.Count + _vector4s.Count
                 + _quaternions.Count + _rects.Count + _rectoffsets.Count + _objects.Count;
        }
#endif
        #endregion
    }
}