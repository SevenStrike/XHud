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
namespace SevenStrikeModules.XBinder.Example
{
    using SevenStrikeModules.XGUI.Runtime;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    #region 解析类实体
    /// <summary>
    /// 本地绑定数据实体（系统状态）
    /// </summary>
    public class local_status_entity
    {
        public string title;
        public string sub;
        public int cooling;
        public string cpu;
        public string keyboard;
        public string memory;
        public string monitor;
        public string mouse;
        public string wifi;
        public int speed;
        public string voltage;
        public string fan_title;
        public Vector2 screen_resolution_v2;
        public Vector2 cursor_position_v2;
        public Vector2 panel_offset_v2;
        public Vector3 camera_position_v3;
        public Vector3 device_rotation_v3;
        public Vector3 gravity_vector_v3;
        public Vector4 rotation_v4;
        public Quaternion quaternion_v4;
        public Rect rect_value;
        public RectOffset rectoffset_value;
        public Vector4 color_tint_v4;
        public Vector4 uv_scale_offset_v4;
        public Color theme_primary_color;
        public Color status_led_color;
        public Color warning_overlay_color;
        public Color panel_background_color;
        public Color text_highlight_color;
        public float fan_rpm;
        public string fan_mode;
        public int fan_count;
        public float power_draw_wat;
        public float uptime_seconds;
        public bool is_charging;
        public string last_sync_time;
        public string firmware_version;
        public string device_id;
        public List<string> sensor_labels;
        public List<int> temperature_history;
    }
    #endregion

    public class local_source : XBinder_Source
    {
        #region 字段
        /// <summary>
        /// 本地 Json 文本资源
        /// </summary>
        public TextAsset json;
        /// <summary>
        /// 是否使用绝对路径（桌面路径）读取 Json
        /// </summary>
        public bool use_absolute_path = false;
        /// <summary>
        /// 绝对路径
        /// </summary>
        public string absolute_path;
        #endregion

        #region Unity 生命周期方法
        /// <summary>
        /// 初始化绑定容器
        /// </summary>
        public override void Awake()
        {
            base.Awake();
        }
        /// <summary>
        /// 启动时发起首次请求
        /// </summary>
        public override void Start()
        {
            base.Start();
        }
        /// <summary>
        /// 启用时发起请求
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
        }
        /// <summary>
        /// 销毁时清理绑定容器
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        /// <summary>
        /// 每帧更新，处理请求调度
        /// </summary>
        public override void Update()
        {
            base.Update();
        }
        #endregion

        #region 请求与数据解析方法
        /// <summary>
        /// 发起本地数据请求，读取并解析 Json 写入绑定容器
        /// </summary>
        public override void request()
        {
            base.request();

            string data = "";

            if (use_absolute_path)
            {
                // 获取当前用户的桌面路径
                string filePath = absolute_path;

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    data = json;
                }
                else
                {
                    XGUI_Utilitys.Console("XBinder 消息", $"找不到文件: {filePath}，已切换为成员变量指定的Json文件", XGUIMsgState.警告);
                    data = json.text;
                }
            }
            else // 解析获取的原始数据到数据源
            {
                data = json.text;
            }

            bind(data);
        }
        #endregion

        #region 绑定数据
        public override void bind(string json = "")
        {
            // 将原始 Json 字符串反序列化为 local_status_entity 实体对象
            // resolve<T>() 是基类提供的泛型解析方法，内部使用 JsonUtility.FromJson<T>() 实现
            // 若 Json 为空、格式错误或解析结果为空，resolve 会打印错误日志并返回 default（即 null）
            // 此时下方所有的 entity.xxx 访问将抛出 NullReferenceException，调用方需保证数据来源可靠
            local_status_entity entity = resolve<local_status_entity>(json);

            // 以下 set_float / set_string 调用将实体字段逐一写入绑定容器 category
            // 键名（第一个参数）是 UI 层订阅读取时的唯一标识符，须与 UI 侧读取的键名完全一致
            // 值（第二个参数）来自已解析的 entity，类型须匹配：字符串字段用 set_string，浮点字段用 set_float

            /*
             * ╭────────────┬─────────────┬────────┬────────────────────────────────────╮
             * │       参数位置                  参数名                      类型                                                 说明
             * ├────────────────────────────────────────────────────────────────────────
             * │       第 1 参数             key（键名）                string          绑定容器中的数据键，UI 层订阅读取的唯一标识符
             * ├────────────────────────────────────────────────────────────────────────
             * │       第 2 参数             value（值）                string               从解析实体取出的实际数据值（英文标题）
             * ╰────────────────────────────────────────────────────────────────────────╯
             */

            category.set_string("title", entity.title);
            category.set_string("sub", entity.sub);
            category.set_float("cooling", entity.cooling);
            category.set_string("cpu", entity.cpu);
            category.set_string("keyboard", entity.keyboard);
            category.set_string("memory", entity.memory);
            category.set_string("monitor", entity.monitor);
            category.set_string("mouse", entity.mouse);
            category.set_string("wifi", entity.wifi);
            category.set_float("speed", entity.speed);
            category.set_string("voltage", entity.voltage);
            category.set_string("fan_title", entity.fan_title);
            category.set_vector2("screen_resolution_v2", entity.screen_resolution_v2);
            category.set_vector2("cursor_position_v2", entity.cursor_position_v2);
            category.set_vector2("panel_offset_v2", entity.panel_offset_v2);
            category.set_vector3("camera_position_v3", entity.camera_position_v3);
            category.set_vector3("device_rotation_v3", entity.device_rotation_v3);
            category.set_vector3("gravity_vector_v3", entity.gravity_vector_v3);
            category.set_vector4("rotation_v4", entity.rotation_v4);
            category.set_quaternion("quaternion_v4", entity.quaternion_v4);
            category.set_rect("rect_value", entity.rect_value);
            category.set_rectoffset("rectoffset_value", entity.rectoffset_value);
            category.set_vector4("color_tint_v4", entity.color_tint_v4);
            category.set_vector4("uv_scale_offset_v4", entity.uv_scale_offset_v4);
            category.set_color("theme_primary_color", entity.theme_primary_color);
            category.set_color("status_led_color", entity.status_led_color);
            category.set_color("warning_overlay_color", entity.warning_overlay_color);
            category.set_color("panel_background_color", entity.panel_background_color);
            category.set_color("text_highlight_color", entity.text_highlight_color);
            category.set_float("fan_rpm", entity.fan_rpm);
            category.set_string("fan_mode", entity.fan_mode);
            category.set_int("fan_count", entity.fan_count);
            category.set_float("power_draw_wat", entity.power_draw_wat);
            category.set_float("uptime_seconds", entity.uptime_seconds);
            category.set_bool("is_charging", entity.is_charging);
            category.set_string("last_sync_time", entity.last_sync_time);
            category.set_string("firmware_version", entity.firmware_version);
            category.set_string("device_id", entity.device_id);
            category.set_object("sensor_labels", entity.sensor_labels);
            category.set_object("temperature_history", entity.temperature_history);
        }
        #endregion
    }
}