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

    [CustomEditor(typeof(local_source))]
    public class Editor_local_source : Editor_XBinder_Source
    {
        #region 字段 - 序列化属性
        private SerializedProperty sp_use_absolute_path;
        private SerializedProperty sp_absolute_path;
        private SerializedProperty sp_json;
        #endregion

        #region 字段 - 选项文字
        /// <summary>
        /// 开关控件的"混合选项"文字
        /// </summary>
        string[] stroptions_absolute = new string[2] { "绝对路径", "指定文件" };
        #endregion       

        public override void OnEnable()
        {
            base.OnEnable();

            base_script = (local_source)target;

            #region 获取序列化字段
            sp_json = serializedObject.FindProperty(nameof(local_source.json));
            sp_use_absolute_path = serializedObject.FindProperty(nameof(local_source.use_absolute_path));
            sp_absolute_path = serializedObject.FindProperty(nameof(local_source.absolute_path));
            #endregion
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Draw_Params()
        {
            base.Draw_Params();

            if (sp_use_absolute_path.boolValue)
            {
                #region 目标数据绝对路径
                XGUI.layout_property_field(
                    title: "数据绝对路径",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_absolute_path,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
            else
            {
                #region 目标数据文件
                XGUI.layout_property_field(
                    title: "目标数据文件",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_json,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
        }

        public override void Draw_Options()
        {
            base.Draw_Options();

            #region 手动请求数据
            DrawToggle("使用绝对路径文件", sp_use_absolute_path, 120, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, stroptions_absolute, (b) => { });
            #endregion
        }

        public override void Draw_Extended()
        {
            base.Draw_Extended();
        }
    }
}