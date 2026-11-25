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
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(web_adapter_multi))]
    public class Editor_web_adapter_multi : Editor_XBinder_Adapter_Multi
    {
        #region 字段 - 组件 / 列表
        private web_adapter_multi web_script;
        #endregion

        #region 字段 - 序列化属性     
        private SerializedProperty sp_text_title;
        private SerializedProperty sp_text_sub;
        private SerializedProperty sp_text_mx_title;
        private SerializedProperty sp_text_mx_sub;
        private SerializedProperty sp_text_mx_uper;
        private SerializedProperty sp_text_mx_footer;
        private SerializedProperty sp_text_fan_title;
        private SerializedProperty sp_text_fan_speed;
        private SerializedProperty sp_text_fan_voltage;
        private SerializedProperty sp_mx_titles;
        private SerializedProperty sp_mx_subs;
        private SerializedProperty sp_mx_values;
        private SerializedProperty sp_mx_seq;
        private SerializedProperty sp_rotator;
        private SerializedProperty sp_rotator_shadow;
        #endregion

        public override void OnEnable()
        {
            base.OnEnable();
            web_script = (web_adapter_multi)target;
            base_script = web_script;

            #region 获取序列化字段
            sp_text_title = serializedObject.FindProperty(nameof(web_adapter_multi.text_title));
            sp_text_sub = serializedObject.FindProperty(nameof(web_adapter_multi.text_sub));
            sp_text_mx_title = serializedObject.FindProperty(nameof(web_adapter_multi.text_mx_title));
            sp_text_mx_sub = serializedObject.FindProperty(nameof(web_adapter_multi.text_mx_sub));
            sp_text_mx_uper = serializedObject.FindProperty(nameof(web_adapter_multi.text_mx_uper));
            sp_text_mx_footer = serializedObject.FindProperty(nameof(web_adapter_multi.text_mx_footer));
            sp_text_fan_title = serializedObject.FindProperty(nameof(web_adapter_multi.text_fan_title));
            sp_text_fan_speed = serializedObject.FindProperty(nameof(web_adapter_multi.text_fan_speed));
            sp_text_fan_voltage = serializedObject.FindProperty(nameof(web_adapter_multi.text_fan_voltage));
            sp_mx_titles = serializedObject.FindProperty(nameof(web_adapter_multi.mx_titles));
            sp_mx_subs = serializedObject.FindProperty(nameof(web_adapter_multi.mx_subs));
            sp_mx_values = serializedObject.FindProperty(nameof(web_adapter_multi.mx_values));
            sp_mx_seq = serializedObject.FindProperty(nameof(web_adapter_multi.mx_seq));
            sp_rotator = serializedObject.FindProperty(nameof(web_adapter_multi.rotator));
            sp_rotator_shadow = serializedObject.FindProperty(nameof(web_adapter_multi.rotator_shadow));
            #endregion
        }

        public override void Draw_Params()
        {
            base.Draw_Params();
        }

        public override void Draw_Extended()
        {
            base.Draw_Extended();

            #region 组件
            web_script.fold_components = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XBinder_Dashboard.theme_group,
                title: "组件",
                title_size: XGUIFontSize.M,
                title_text_color: XBinder_Dashboard.theme_primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(5, 5, 20, 15),
                foldout: web_script.fold_components);

            if (!web_script.fold_components)
            {
                #region 标题文字
                XGUI.layout_property_field(
                    title: "标题文字",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_text_title,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 副标题文字
                XGUI.layout_property_field(
                    title: "副标题文字",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_text_sub,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 矩阵标题
                XGUI.layout_property_field(
                    title: "矩阵标题",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_text_mx_title,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 矩阵副标题
                XGUI.layout_property_field(
                    title: "矩阵副标题",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_text_mx_sub,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 矩阵上标文字
                XGUI.layout_property_field(
                    title: "矩阵上标文字",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_text_mx_uper,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 矩阵页脚文字
                XGUI.layout_property_field(
                    title: "矩阵页脚文字",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_text_mx_footer,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 风扇标题文字
                XGUI.layout_property_field(
                    title: "风扇标题文字",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_text_fan_title,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 风扇转速文字
                XGUI.layout_property_field(
                    title: "风扇转速文字",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_text_fan_speed,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 风扇电压文字
                XGUI.layout_property_field(
                    title: "风扇电压文字",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_text_fan_voltage,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 矩阵标题集
                XGUI.layout_property_field(
                    title: "矩阵标题集",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_mx_titles,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 矩阵副标题集
                XGUI.layout_property_field(
                    title: "矩阵副标题集",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_mx_subs,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 矩阵数值集
                XGUI.layout_property_field(
                    title: "矩阵数值集",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_mx_values,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 矩阵序列
                XGUI.layout_property_field(
                    title: "矩阵序列",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_mx_seq,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 旋转体
                XGUI.layout_property_field(
                    title: "旋转体",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_rotator,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 旋转体阴影
                XGUI.layout_property_field(
                    title: "旋转体阴影",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_rotator_shadow,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

        }
    }
}