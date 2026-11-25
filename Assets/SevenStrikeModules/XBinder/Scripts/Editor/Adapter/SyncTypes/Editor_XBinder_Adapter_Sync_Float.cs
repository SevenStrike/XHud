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
namespace SevenStrikeModules.XBinder.Editor
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XBinder_Adapter_Sync_Float), true)]
    public class Editor_XBinder_Adapter_Sync_Float : Editor
    {
        #region 字段 - 组件 / 列表
        public XBinder_Adapter_Sync_Float base_script;
        #endregion

        #region 字段 - 序列化属性
        private SerializedProperty sp_identifier;
        private SerializedProperty sp_target_category;
        private SerializedProperty sp_target_key;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 面板主标题图标
        /// </summary>
        private Texture2D icon_main;
        #endregion

        #region 字段 - 批量化操作
        /// <summary>
        /// 当前 Inspector 选中的所有目标组件缓存。
        /// </summary>
        private XBinder_Adapter_Sync_Float[] selected_objects;
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
                selected_objects = new XBinder_Adapter_Sync_Float[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XBinder_Adapter_Sync_Float)t;
                }
            }
            else
            {
                selected_objects = new XBinder_Adapter_Sync_Float[targets.Length];
                selected_objects[0] = (XBinder_Adapter_Sync_Float)target;
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

        #region 生命周期
        protected virtual void OnEnable()
        {
            base_script = (XBinder_Adapter_Sync_Float)target;

            #region 获取序列化字段
            sp_identifier = serializedObject.FindProperty(nameof(XBinder_Adapter_Sync_Float.identifier));
            sp_target_category = serializedObject.FindProperty(nameof(XBinder_Adapter_Sync_Float.target_category));
            sp_target_key = serializedObject.FindProperty(nameof(XBinder_Adapter_Sync_Float.target_key));
            #endregion

            #region 图标获取
            icon_main = XGUI.GetCustomIcon($"{XBinder_Dashboard.get_path_xbinder_gui()}gui_adapter_Base/icon_main");
            #endregion

            Targets_Get();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string hexcol = XGUI_Utilitys.Color_To_HexString(Color.white, true);

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XBinder_Dashboard.theme_primary,
                title_text: string.IsNullOrEmpty(sp_identifier.stringValue) ? "XBinder  -  绑定适配器（Float）" : "XBinder - 绑定适配器（Float）-> ( " + sp_identifier.stringValue + " )",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping_ellipsis,
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 参数
            base_script.fold_param = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XBinder_Dashboard.theme_group,
                title: "参数",
                title_size: XGUIFontSize.M,
                title_text_color: XBinder_Dashboard.theme_primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(5, 5, 20, 15),
                foldout: base_script.fold_param);

            if (!base_script.fold_param)
            {
                Draw_Params();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 扩展面板
            Draw_Extended();
            #endregion

            #region 源脚本
            base_script.fold_based = XGUI.layout_group_start(
            type: XGUIContainerType.Vertical,
            bg_fill: XGUIGroup.缺口纯色边框,
            bg_color: XGUIColor.亮白,
            bg_color_gui: XBinder_Dashboard.theme_group,
            title: "源脚本",
            title_size: XGUIFontSize.M,
            title_text_color: XBinder_Dashboard.theme_primary,
            title_clipping: TextClipping.Clip,
            padding: new RectOffset(10, 10, 15, 15),
            foldout: base_script.fold_based);

            if (!base_script.fold_based)
            {
                DrawDefaultInspector();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 面板绘制   
        public virtual void Draw_Params()
        {
            #region 标识名称
            XGUI.layout_property_field(
            title: "标识名称",
            title_size: XGUIFontSize.M,
            title_hover_color: XBinder_Dashboard.theme_primary,
            title_width: 120,
            prop: sp_identifier,
            prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 数据源
            XGUI.layout_property_field(
                title: "数据源",
                title_size: XGUIFontSize.M,
                title_hover_color: XBinder_Dashboard.theme_primary,
                title_width: 120,
                prop: sp_target_category,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 键
            XGUI.layout_property_field(
                title: "键",
                title_size: XGUIFontSize.M,
                title_hover_color: XBinder_Dashboard.theme_primary,
                title_width: 120,
                prop: sp_target_key,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion
        }
        public virtual void Draw_Extended()
        {

        }
        #endregion
    }
}