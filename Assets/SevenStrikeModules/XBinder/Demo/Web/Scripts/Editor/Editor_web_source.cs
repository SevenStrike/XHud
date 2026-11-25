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
namespace SevenStrikeModules.XBinder.Example.Editor
{
    using SevenStrikeModules.XBinder.Editor;
    using SevenStrikeModules.XBinder.Example;
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(web_source))]
    public class Editor_web_source : Editor_XBinder_Source
    {
        #region 字段 - 组件 / 列表
        public web_source websource_script;
        #endregion

        #region 字段 - 序列化属性
        private SerializedProperty sp_loader;
        #endregion

        #region 字段 - 选项文字
        /// <summary>
        /// 开关控件的"混合选项"文字
        /// </summary>
        string[] stroptions_absolute = new string[2] { "绝对路径", "指定文件" };
        #endregion

        #region 字段 - 批量化操作
        /// <summary>
        /// 当前 Inspector 选中的所有目标组件缓存。
        /// </summary>
        private web_source[] selected_objects;
        #endregion

        #region 工具 - 批量化操作
        /// <summary>
        /// 缓存当前 Inspector 选中的所有目标组件到 <see cref="selected_objects"/>。
        /// <para/>
        /// 多选时逐个转换，单选时只填充一个元素。
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                selected_objects = new web_source[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (web_source)t;
                }
            }
            else
            {
                selected_objects = new web_source[targets.Length];
                selected_objects[0] = (web_source)target;
            }
        }
        /// <summary>
        /// 判断当前是否处于多选状态。
        /// </summary>
        /// <returns>true 表示选中了多个组件</returns>
        private bool Targets_Selected()
        {
            if (selected_objects == null)
                return false;
            if (selected_objects.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        public override void OnEnable()
        {
            base.OnEnable();

            websource_script = (web_source)target;
            base_script = websource_script;

            #region 获取序列化字段
            sp_loader = serializedObject.FindProperty(nameof(web_source.loader));
            #endregion

            Targets_Get();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Draw_Params()
        {
            base.Draw_Params();

            #region 网络数据源获取器
            XGUI.layout_property_field(
                title: "网络数据源获取器",
                title_size: XGUIFontSize.M,
                title_hover_color: XBinder_Dashboard.theme_primary,
                title_width: 120,
                prop: sp_loader,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion
        }
    }
}