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
    using SevenStrikeModules.XBinder;
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using UnityEngine;

    public class XBinder_Source : MonoBehaviour
    {
        #region 绑定配置字段
        /// <summary>
        /// 绑定容器标识符
        /// </summary>
        public string identifier = "";
        public bool category_created;
        /// <summary>
        /// 绑定数据分类容器
        /// </summary>
        public XBinder_Category category;
        /// <summary>
        /// 手动请求时触发的按键
        /// </summary>
        public KeyCode request_key = KeyCode.Space;
        /// <summary>
        /// 是否启用手动请求模式
        /// </summary>
        public bool manual_request;
        #endregion

        #region 隔帧刷新数据
        /// <summary>
        /// 请求计时器原始累积值
        /// </summary>
        private float request_timer_raw = 0f;
        /// <summary>
        /// 请求间隔时间（秒）
        /// </summary>
        public float request_interval = 1f; // 间隔 1 秒
        #endregion

        #region 折叠
        public bool
            fold_param = true,
            fold_category = true,
            fold_option = true,
            fold_state = true,
            fold_based = true;

        public void GroupFold(bool state)
        {
            fold_param = state;
            fold_state = state;
            fold_option = state;
            fold_category = state;
            fold_based = state;
        }

        public void GroupFold(bool param = true, bool state = true, bool option = true, bool based = true, bool category = true)
        {
            fold_param = param;
            fold_category = category;
            fold_state = state;
            fold_option = option;
            fold_based = based;
        }
        #endregion

        #region Unity 生命周期方法
        /// <summary>
        /// 初始化时新建绑定容器
        /// </summary>
        public virtual void Awake()
        {
            category_create();
        }
        /// <summary>
        /// 启动时发起首次请求
        /// </summary>
        public virtual void Start()
        {
            request();
        }
        /// <summary>
        /// 启用时发起请求
        /// </summary>
        public virtual void OnEnable()
        {
            request();
        }
        /// <summary>
        /// 禁用时回调（空实现，供子类扩展）
        /// </summary>
        public virtual void OnDisable()
        {

        }
        /// <summary>
        /// 销毁时从总线移除容器，杜绝内存残影
        /// </summary>
        public virtual void OnDestroy()
        {
            if (category_created)
                // 销毁时，把容器从总线拔除，彻底杜绝内存残影
                category_remove();
        }

        /// <summary>
        /// 每帧更新，根据模式处理手动或自动请求
        /// </summary>
        public virtual void Update()
        {
            if (manual_request)
            {
                if (Input.GetKeyDown(request_key))
                    request();
            }
            else
            {
                #region 隔帧刷新数据
                request_timer_raw += Time.deltaTime;
                if (request_timer_raw >= request_interval)
                {
                    request_timer_raw -= request_interval;
                    request();
                }
                #endregion
            }
        }
        #endregion

        #region 数据源操作
        public void category_create()
        {
            // 原来只判断 category_created，多加两个条件
            if (category_created && category != null && XBinder_Bus.categories.Contains(category))
                return;   // 真正有效时才跳过

            // 其余情况（含 Domain Reload 后 category 失效）都重新创建
            category = XBinder_Bus.create_category(identifier);
            category_created = true;
        }

        public void category_remove()
        {
            if (!category_created)
                return;

            XBinder_Bus.destroy_category(category);
            category_created = false;
        }
        #endregion

        #region 请求与解析方法
        /// <summary>
        /// 发起数据请求（虚方法，供子类重写）
        /// </summary>
        public virtual void request()
        {
            // 请在子类中自行实现：请求 Json 数据的具体实现代码块...
        }
        /// <summary>
        /// 将 Json 字符串解析为指定类型对象
        /// </summary>
        /// <typeparam name="T">目标解析类型</typeparam>
        /// <param name="json">待解析的 Json 字符串</param>
        /// <returns>解析后的对象，失败时返回默认值</returns>
        public T resolve<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                XGUI_Utilitys.Console("XBinder 消息", $"[Loader] Json 为空", XGUIMsgState.错误);
                return default;
            }

            T data;

            try
            {
                data = JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                XGUI_Utilitys.Console("XBinder 消息", $"[Loader] 解析失败: {e.Message}", XGUIMsgState.错误);
                return default;
            }

            if (data == null)
            {
                XGUI_Utilitys.Console("XBinder 消息", $"[Loader] 解析结果为空", XGUIMsgState.错误);
                return default;
            }

            return data;
        }
        #endregion

        #region 绑定数据名称
        public virtual void bind(string json = "")
        {
            // 请在子类中自行实现：绑定数据的具体实现代码块...
        }
        #endregion

        public void Editor_Recomplier_Rebuild()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[XBinder] 非运行模式禁止 Rebuild。");
                return;
            }

            category_create();
            request();
        }
    }
}