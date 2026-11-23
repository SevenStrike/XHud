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
    using System;
    using System.Collections.Generic;

    #region 解析类实体
    /// <summary>
    /// 网络绑定数据实体（矩阵循环压力释放）
    /// </summary>
    [Serializable]
    public class web_Matrix_Loop_Pressure_Relief_Entity
    {
        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool success;
        /// <summary>
        /// 响应状态码
        /// </summary>
        public int code;
        /// <summary>
        /// 响应消息
        /// </summary>
        public string message;
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<web_Matrix_Loop_Pressure_Relief_Data> data;
        /// <summary>
        /// 令牌对象
        /// </summary>
        public object token;
        /// <summary>
        /// 英文标题
        /// </summary>
        public string title_en;
        /// <summary>
        /// 标题
        /// </summary>
        public string title;
        /// <summary>
        /// 副标题
        /// </summary>
        public string sub;
        /// <summary>
        /// 模块图标
        /// </summary>
        public string mx_icon;
        /// <summary>
        /// 模块标题
        /// </summary>
        public string mx_title;
        /// <summary>
        /// 模块上级文本
        /// </summary>
        public string mx_uper;
        /// <summary>
        /// 模块副文本
        /// </summary>
        public string mx_sub;
        /// <summary>
        /// 模块底部文本
        /// </summary>
        public string mx_footer;
        /// <summary>
        /// 中心图标
        /// </summary>
        public string center_icon;
        /// <summary>
        /// 风扇标题
        /// </summary>
        public string fan_title;
        /// <summary>
        /// 速度值
        /// </summary>
        public float speed;
        /// <summary>
        /// 电压值
        /// </summary>
        public string voltage;
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<web_Matrix_Loop_Pressure_Relief_Events> events;
    }
    /// <summary>
    /// 网络绑定数据明细项
    /// </summary>
    [Serializable]
    public class web_Matrix_Loop_Pressure_Relief_Data
    {
        public string stellarCycle;
        public string peakFlux;
        public string energyDrain;
        public string gridZone;
        public string deviceName;
        public string sectorName;
        public float fluxValue;
        public int thresholdCap;
        public string fluxLinkName;
        public int loadRate;
        public int currentFlux;
    }
    [Serializable]
    public class web_Matrix_Loop_Pressure_Relief_Events
    {
        public string eventDate;
        public string archiveLog;
        public string eventYear;
    }
    #endregion

    public class web_source : XBinder_Source
    {
        #region 字段
        /// <summary>
        /// Json 网络数据加载器
        /// </summary>
        public web_json_loader_post loader;
        #endregion

        #region Unity 生命周期方法
        /// <summary>
        /// 初始化绑定容器
        /// </summary>
        public override void Awake()
        {
            identifier = loader.r_name;
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
        /// 启用时订阅数据获取事件并发起请求
        /// </summary>
        public override void OnEnable()
        {
            loader.on_fetched_data += on_requested;
            base.OnEnable();
        }
        /// <summary>
        /// 禁用时取消订阅数据获取事件
        /// </summary>
        public override void OnDisable()
        {
            loader.on_fetched_data -= on_requested;
            base.OnDisable();
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
        /// 发起网络数据请求
        /// </summary>
        public override void request()
        {
            base.request();

            /// 调用网络数据加载器发起 HTTP 请求
            /// loader 是  <see cref="web_json_loader_post"/> 类型的成员变量，负责与后端接口通信。
            /// Request() 方法会向服务器发送请求，请求完成后通过 on_fetched_data 事件回调返回 Json 字符串。
            /// 该事件已在 OnEnable() 中通过 loader.on_fetched_data += <see cref="on_requested"/>; 完成订阅，
            /// 因此请求返回的数据会自动交给 <see cref="on_requested"/> 方法进行解析与绑定。
            loader.Request();
        }
        /// <summary>
        /// 数据获取回调，解析数据并写入绑定容器
        /// </summary>
        /// <param name="json">原始 Json 数据字符串</param>
        private void on_requested(string json)
        {
            bind(json);
        }
        #endregion

        #region 绑定数据
        public override void bind(string json)
        {
            // 将原始 Json 字符串反序列化为 web_Matrix_Loop_Pressure_Relief_Entity 实体对象
            // resolve<T>() 是基类 XBinder_Binder 提供的泛型解析方法，内部使用 JsonUtility.FromJson<T>() 实现反序列化
            // 若 Json 为空、解析失败或结果为空，resolve 会输出错误日志并返回 default（即 null），
            // 此时下方所有 entity.xxx 访问将抛出 NullReferenceException，调用方需保证数据来源可靠
            web_Matrix_Loop_Pressure_Relief_Entity entity = resolve<web_Matrix_Loop_Pressure_Relief_Entity>(json);

            // 以下 set_string / set_float 调用将实体字段逐一写入绑定容器 category
            // 键名（第一个参数）是 UI 层订阅时使用的标识符，需与 UI 侧读取时保持一致
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

            category.set_string("title_en", entity.title_en);
            category.set_string("title", entity.title);
            category.set_string("sub", entity.sub);
            category.set_string("mx_icon", entity.mx_icon);
            category.set_string("mx_title", entity.mx_title);
            category.set_string("mx_uper", entity.mx_uper);
            category.set_string("mx_sub", entity.mx_sub);
            category.set_string("mx_footer", entity.mx_footer);
            category.set_string("center_icon", entity.center_icon);
            category.set_string("fan_title", entity.fan_title);
            category.set_float("speed", entity.speed);
            category.set_string("voltage", entity.voltage);
            category.set_object("data", entity.data);
        }
        #endregion
    }
}