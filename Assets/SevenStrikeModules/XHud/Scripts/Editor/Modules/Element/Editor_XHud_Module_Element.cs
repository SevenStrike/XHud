/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XHud - Unity UGUI 高级管理架构插件
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
namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XTween;
    using SevenStrikeModules.XTween.Editor;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.UI;
    using Color = UnityEngine.Color;
    using Object = UnityEngine.Object;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Element), true)]
    public partial class Editor_XHud_Module_Element : Editor
    {
        #region 字段 - 组件 / 列表
        /// <summary>
        /// 当前编辑的元素组件。
        /// <para/>
        /// 单选模式下使用；多选时通过 <see cref="selected_objects"/> 访问全部目标。
        /// </summary>
        private XHud_Module_Element base_script;
        /// <summary>
        /// XHud 管理器实例。
        /// <para/>
        /// 用于访问：
        /// <list type="bullet">
        /// <item><description>音效库 / 动效库 / 曲线库；</description></item>
        /// <item><description>RMS 分辨率节点；</description></item>
        /// <item><description>全局音量、倍增系数等。</description></item>
        /// </list>
        /// </summary>
        private XHud_Manager hud_manager;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 编辑器序列化字段 ID 属性
        /// </summary>
        private SerializedProperty sp_editor_serializefield_id;
        /// <summary>
        /// 画布组组件属性
        /// </summary>
        private SerializedProperty sp_canvasgroup;
        /// <summary>
        /// 音效节点列表属性
        /// </summary>
        private SerializedProperty sp_sounder_nodes;
        /// <summary>
        /// 图元控制器节点列表属性
        /// </summary>
        private SerializedProperty sp_primitive_controller_nodes;
        /// <summary>
        /// 选项节点列表属性
        /// </summary>
        private SerializedProperty sp_option_nodes;
        /// <summary>
        /// 按钮节点列表属性
        /// </summary>
        private SerializedProperty sp_button_nodes;
        /// <summary>
        /// 文本折叠状态属性
        /// </summary>
        private SerializedProperty sp_text_is_fold;
        /// <summary>
        /// TMP 文本折叠状态属性
        /// </summary>
        private SerializedProperty sp_tmptext_is_fold;
        /// <summary>
        /// 滑动条节点列表属性
        /// </summary>
        private SerializedProperty sp_slider_nodes;
        /// <summary>
        /// 文本节点列表属性
        /// </summary>
        private SerializedProperty sp_text_nodes;
        /// <summary>
        /// TMP 文本节点列表属性
        /// </summary>
        private SerializedProperty sp_tmptext_nodes;
        /// <summary>
        /// 调试开关属性
        /// </summary>
        private SerializedProperty sp_debug_state;
        /// <summary>
        /// 进度条节点列表属性
        /// </summary>
        private SerializedProperty sp_progress_nodes;
        /// <summary>
        /// 开关节点列表属性
        /// </summary>
        private SerializedProperty sp_toggle_nodes;
        /// <summary>
        /// 标识属性
        /// </summary>
        private SerializedProperty sp_indicator;
        /// <summary>
        /// 图元动效最大时长属性
        /// </summary>
        private SerializedProperty sp_primitives_tween_max_duration;
        /// <summary>
        /// RectTransform 组件属性
        /// </summary>
        private SerializedProperty sp_rect_transform;
        /// <summary>
        /// 触发动作属性
        /// </summary>
        private SerializedProperty sp_trigger_action;
        /// <summary>
        /// 物体追踪器属性
        /// </summary>
        private SerializedProperty sp_object_tracker;
        /// <summary>
        /// 生成状态属性
        /// </summary>
        private SerializedProperty sp_create_state;
        /// <summary>
        /// 动画状态属性
        /// </summary>
        private SerializedProperty sp_animate_state;
        /// <summary>
        /// 图元动效全局时长属性
        /// </summary>
        private SerializedProperty sp_primitives_tween_global_duration;
        /// <summary>
        /// 原始对象池名称属性
        /// </summary>
        private SerializedProperty sp_origin_poolname;
        /// <summary>
        /// 图元折叠状态属性
        /// </summary>
        private SerializedProperty sp_primitives_is_fold;
        /// <summary>
        /// 按钮折叠状态属性
        /// </summary>
        private SerializedProperty sp_button_is_fold;
        /// <summary>
        /// 选项折叠状态属性
        /// </summary>
        private SerializedProperty sp_option_is_fold;
        /// <summary>
        /// 滑动条折叠状态属性
        /// </summary>
        private SerializedProperty sp_slider_is_fold;
        /// <summary>
        /// 音效折叠状态属性
        /// </summary>
        private SerializedProperty sp_sounder_is_fold;
        /// <summary>
        /// 进度条折叠状态属性
        /// </summary>
        private SerializedProperty sp_progress_is_fold;
        /// <summary>
        /// 开关折叠状态属性
        /// </summary>
        private SerializedProperty sp_toggle_is_fold;
        /// <summary>
        /// 事件折叠状态属性
        /// </summary>
        private SerializedProperty sp_event_is_fold;
        /// <summary>
        /// 图元动效自动播放属性
        /// </summary>
        private SerializedProperty sp_autoplay_primitives_tween;
        /// <summary>
        /// 预览动效自动杀死属性
        /// </summary>
        private SerializedProperty sp_autokill_preview_tweens;
        /// <summary>
        /// 透明度属性
        /// </summary>
        private SerializedProperty sp_alpha;
        /// <summary>
        /// RMS 启用属性
        /// </summary>
        private SerializedProperty sp_rms_enabled;
        /// <summary>
        /// RMS 布局数据属性
        /// </summary>
        private SerializedProperty sp_rms_layout_datas;
        /// <summary>
        /// RMS 名称属性
        /// </summary>
        private SerializedProperty sp_rms_name;
        /// <summary>
        /// 当前轴心属性
        /// </summary>
        private SerializedProperty sp_current_pivot;
        /// <summary>
        /// 原始名称属性
        /// </summary>
        private SerializedProperty sp_original_name;
        /// <summary>
        /// 预览入场状态属性
        /// </summary>
        private SerializedProperty sp_tweens_preivew_in_state;
        /// <summary>
        /// 预览出场状态属性
        /// </summary>
        private SerializedProperty sp_tweens_preivew_out_state;
        /// <summary>
        /// 杀死时重置预览动效属性
        /// </summary>
        private SerializedProperty sp_rewind_preview_tweens_with_kill;
        /// <summary>
        /// 杀死时清空预览动效属性
        /// </summary>
        private SerializedProperty sp_clear_preview_tweens_with_kill;
        /// <summary>
        /// 生成动效参数属性
        /// </summary>
        private SerializedProperty sp_motion_create_args;
        /// <summary>
        /// 回收动效参数属性
        /// </summary>
        private SerializedProperty sp_motion_recycle_args;
        /// <summary>
        /// 生成动效库索引名称属性
        /// </summary>
        private SerializedProperty sp_motion_create_library_index_name;
        /// <summary>
        /// 回收动效库索引名称属性
        /// </summary>
        private SerializedProperty sp_motion_recycle_library_index_name;
        /// <summary>
        /// 生成预览位置属性
        /// </summary>
        private SerializedProperty sp_create_preview_position;
        /// <summary>
        /// 回收预览位置属性
        /// </summary>
        private SerializedProperty sp_recycle_preview_position;
        /// <summary>
        /// 生成预览设置位置属性
        /// </summary>
        private SerializedProperty sp_create_preview_set_position;
        /// <summary>
        /// 回收预览设置位置属性
        /// </summary>
        private SerializedProperty sp_recycle_preview_set_position;
        /// <summary>
        /// 预览是否包含图元动效属性
        /// </summary>
        private SerializedProperty sp_preview_include_primitives_tween;
        /// <summary>
        /// 生成时清空动作属性
        /// </summary>
        private SerializedProperty sp_clear_actions_with_spawn;
        /// <summary>
        /// 生成时清空事件属性
        /// </summary>
        private SerializedProperty sp_clear_events_with_spawn;
        /// <summary>
        /// 高亮器属性
        /// </summary>
        private SerializedProperty sp_highlighter;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 面板主标题图标
        /// </summary>
        private Texture2D icon_main;
        /// <summary>
        /// 声音模块图标
        /// </summary>
        private Texture2D icon_sound;
        /// <summary>
        /// 动画模块图标
        /// </summary>
        private Texture2D icon_anim;
        /// <summary>
        /// 按钮模块图标
        /// </summary>
        private Texture2D icon_button;
        /// <summary>
        /// 选项模块图标
        /// </summary>
        private Texture2D icon_option;
        /// <summary>
        /// 滑动条模块图标
        /// </summary>
        private Texture2D icon_slider;
        /// <summary>
        /// 进度条模块图标
        /// </summary>
        private Texture2D icon_progress;
        /// <summary>
        /// 开关模块图标
        /// </summary>
        private Texture2D icon_toggle;
        /// <summary>
        /// 文本模块图标
        /// </summary>
        private Texture2D icon_text;
        /// <summary>
        /// TMP 文本模块图标
        /// </summary>
        private Texture2D icon_tmptext;
        /// <summary>
        /// 扫描按钮常态图标
        /// </summary>
        private Texture2D icon_scan_r;
        /// <summary>
        /// 扫描按钮按下图标
        /// </summary>
        private Texture2D icon_scan_p;
        /// <summary>
        /// RMS 录制按钮常态图标
        /// </summary>
        private Texture2D icon_record_rms_r;
        /// <summary>
        /// RMS 录制按钮按下图标
        /// </summary>
        private Texture2D icon_record_rms_p;
        /// <summary>
        /// 重置锚点位置按钮常态图标
        /// </summary>
        private Texture2D resetanchorpos_r;
        /// <summary>
        /// 重置锚点位置按钮按下图标
        /// </summary>
        private Texture2D resetanchorpos_p;
        /// <summary>
        /// 定位按钮常态图标
        /// </summary>
        private Texture2D locate_r;
        /// <summary>
        /// 定位按钮按下图标
        /// </summary>
        private Texture2D locate_p;
        /// <summary>
        /// 左箭头按钮常态图标
        /// </summary>
        private Texture2D left_arrow_r;
        /// <summary>
        /// 左箭头按钮按下图标
        /// </summary>
        private Texture2D left_arrow_p;
        /// <summary>
        /// 右箭头按钮常态图标
        /// </summary>
        private Texture2D right_arrow_r;
        /// <summary>
        /// 右箭头按钮按下图标
        /// </summary>
        private Texture2D right_arrow_p;
        /// <summary>
        /// 预览播放（外）按钮常态图标
        /// </summary>
        private Texture2D prw_play_out_r;
        /// <summary>
        /// 预览播放（外）按钮按下图标
        /// </summary>
        private Texture2D prw_play_out_p;
        /// <summary>
        /// 预览播放（内）按钮常态图标
        /// </summary>
        private Texture2D prw_play_in_r;
        /// <summary>
        /// 预览播放（内）按钮按下图标
        /// </summary>
        private Texture2D prw_play_in_p;
        /// <summary>
        /// 保存按钮常态图标
        /// </summary>
        private Texture2D save_r;
        /// <summary>
        /// 保存按钮按下图标
        /// </summary>
        private Texture2D save_p;
        /// <summary>
        /// 重置按钮常态图标
        /// </summary>
        private Texture2D reset_r;
        /// <summary>
        /// 重置按钮按下图标
        /// </summary>
        private Texture2D reset_p;
        /// <summary>
        /// 重建 ID 按钮常态图标
        /// </summary>
        private Texture2D icon_recreateid_r;
        /// <summary>
        /// 重建 ID 按钮按下图标
        /// </summary>
        private Texture2D icon_recreateid_p;
        /// <summary>
        /// 编辑按钮常态图标
        /// </summary>
        private Texture2D edit_r;
        /// <summary>
        /// 编辑按钮按下图标
        /// </summary>
        private Texture2D edit_p;
        #endregion

        #region 字段 - 选项文字
        /// <summary>
        /// 开关控件的"混合选项"文字。
        /// <para/>
        /// 包含：关闭 / 开启、不包含 / 包含、关闭 / 调试、手动 / 自动。
        /// </summary>
        string[] stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_include = new string[2] { "不包含", "包含" }, stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_auto = new string[2] { "手动", "自动" };
        #endregion

        #region 字段 - 批量模式查看索引
        /// <summary>
        /// 多选时当前查看的组件索引。
        /// <para/>
        /// 状态 / RMS / 统计 等面板共用。
        /// </summary>
        private int element_statu_index;
        #endregion

        #region 字段 - 批量化操作
        /// <summary>
        /// 当前 Inspector 选中的所有目标组件缓存。
        /// </summary>
        private XHud_Module_Element[] selected_objects;
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
                selected_objects = new XHud_Module_Element[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XHud_Module_Element)t;
                }
            }
            else
            {
                selected_objects = new XHud_Module_Element[targets.Length];
                selected_objects[0] = (XHud_Module_Element)target;
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
        /// <summary>
        /// Unity 启用回调。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>获取 HudManager 单例；</description></item>
        /// <item><description>缓存目标组件；</description></item>
        /// <item><description>缓存序列化属性与组件引用；</description></item>
        /// <item><description>清理无效子组件节点；</description></item>
        /// <item><description>加载所有图标资源；</description></item>
        /// <item><description>构造所有 ReorderableList；</description></item>
        /// <item><description>处理 RMS 残留信息；</description></item>
        /// <item><description>缓存所有选中组件；</description></item>
        /// <item><description>加载动画预览配置。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            hud_manager = XHud_Dashboard.get_manager();

            base_script = (XHud_Module_Element)target;

            #region 获取序列化字段
            #region SounderNodes
            sp_sounder_nodes = serializedObject.FindProperty(nameof(XHud_Module_Element.sounder_nodes));
            sp_sounder_is_fold = serializedObject.FindProperty(nameof(XHud_Module_Element.sounder_is_fold));
            #endregion

            # region ButtonNodes
            sp_button_nodes = serializedObject.FindProperty(nameof(XHud_Module_Element.button_nodes));
            sp_button_is_fold = serializedObject.FindProperty(nameof(XHud_Module_Element.button_is_fold));
            #endregion

            # region OptionNodes
            sp_option_nodes = serializedObject.FindProperty(nameof(XHud_Module_Element.option_nodes));
            sp_option_is_fold = serializedObject.FindProperty(nameof(XHud_Module_Element.option_is_fold));
            #endregion

            # region SliderNodes
            sp_slider_nodes = serializedObject.FindProperty(nameof(XHud_Module_Element.slider_nodes));
            sp_slider_is_fold = serializedObject.FindProperty(nameof(XHud_Module_Element.slider_is_fold));
            #endregion

            # region ProgressNodes
            sp_progress_nodes = serializedObject.FindProperty(nameof(XHud_Module_Element.progress_nodes));
            sp_progress_is_fold = serializedObject.FindProperty(nameof(XHud_Module_Element.progress_is_fold));
            #endregion

            # region TextNodes
            sp_text_nodes = serializedObject.FindProperty(nameof(XHud_Module_Element.text_nodes));
            sp_text_is_fold = serializedObject.FindProperty(nameof(XHud_Module_Element.text_is_fold));
            #endregion

            # region TmpTextNodes
            sp_tmptext_nodes = serializedObject.FindProperty(nameof(XHud_Module_Element.tmptext_nodes));
            sp_tmptext_is_fold = serializedObject.FindProperty(nameof(XHud_Module_Element.tmptext_is_fold));
            #endregion

            # region ToggleNodes
            sp_toggle_nodes = serializedObject.FindProperty(nameof(XHud_Module_Element.toggle_nodes));
            sp_toggle_is_fold = serializedObject.FindProperty(nameof(XHud_Module_Element.toggle_is_fold));
            #endregion

            #region Based
            sp_debug_state = serializedObject.FindProperty(nameof(XHud_Module_Element.debug_state));
            sp_editor_serializefield_id = serializedObject.FindProperty(XHud_Module_Element.editor_serializefield_id);
            sp_indicator = serializedObject.FindProperty(nameof(XHud_Module_Element.indicator));
            sp_original_name = serializedObject.FindProperty(nameof(XHud_Module_Element.original_name));
            sp_origin_poolname = serializedObject.FindProperty(nameof(XHud_Module_Element.origin_poolname));
            sp_rect_transform = serializedObject.FindProperty(nameof(XHud_Module_Element.rect_transform));
            sp_current_pivot = serializedObject.FindProperty(nameof(XHud_Module_Element.current_pivot));
            sp_create_state = serializedObject.FindProperty(nameof(XHud_Module_Element.create_state));
            sp_animate_state = serializedObject.FindProperty(nameof(XHud_Module_Element.animate_state));
            #endregion

            #region Alpha
            sp_canvasgroup = serializedObject.FindProperty(nameof(XHud_Module_Element.canvasgroup));
            sp_alpha = serializedObject.FindProperty(nameof(XHud_Module_Element.alpha));
            #endregion

            #region TriggerAction
            sp_trigger_action = serializedObject.FindProperty(nameof(XHud_Module_Element.trigger_action));
            #endregion

            #region ObjectTrack
            sp_object_tracker = serializedObject.FindProperty(nameof(XHud_Module_Element.object_tracker));
            #endregion

            #region ElementTween
            sp_autokill_preview_tweens = serializedObject.FindProperty(nameof(XHud_Module_Element.autokill_preview_tweens));
            sp_tweens_preivew_in_state = serializedObject.FindProperty(nameof(XHud_Module_Element.tweens_preivew_in_state));
            sp_tweens_preivew_out_state = serializedObject.FindProperty(nameof(XHud_Module_Element.tweens_preivew_out_state));
            sp_rewind_preview_tweens_with_kill = serializedObject.FindProperty(nameof(XHud_Module_Element.rewind_preview_tweens_with_kill));
            sp_clear_preview_tweens_with_kill = serializedObject.FindProperty(nameof(XHud_Module_Element.clear_preview_tweens_with_kill));
            sp_create_preview_position = serializedObject.FindProperty(nameof(XHud_Module_Element.create_preview_position));
            sp_recycle_preview_position = serializedObject.FindProperty(nameof(XHud_Module_Element.recycle_preview_position));
            sp_preview_include_primitives_tween = serializedObject.FindProperty(nameof(XHud_Module_Element.preview_include_primitives_tween));
            sp_create_preview_set_position = serializedObject.FindProperty(nameof(XHud_Module_Element.create_preview_set_position));
            sp_recycle_preview_set_position = serializedObject.FindProperty(nameof(XHud_Module_Element.recycle_preview_set_position));
            sp_motion_create_library_index_name = serializedObject.FindProperty(nameof(XHud_Module_Element.motion_create_library_index_name));
            sp_motion_recycle_library_index_name = serializedObject.FindProperty(nameof(XHud_Module_Element.motion_recycle_library_index_name));
            sp_motion_create_args = serializedObject.FindProperty(nameof(XHud_Module_Element.motion_create_args));
            sp_motion_recycle_args = serializedObject.FindProperty(nameof(XHud_Module_Element.motion_recycle_args));
            #endregion

            #region Events
            sp_event_is_fold = serializedObject.FindProperty(nameof(XHud_Module_Element.event_is_fold));
            sp_clear_events_with_spawn = serializedObject.FindProperty(nameof(XHud_Module_Element.clear_events_with_spawn));
            #endregion

            #region RMS
            sp_rms_enabled = serializedObject.FindProperty(nameof(XHud_Module_Element.rms_enabled));
            sp_rms_layout_datas = serializedObject.FindProperty(nameof(XHud_Module_Element.rms_layout_datas));
            sp_rms_name = serializedObject.FindProperty(nameof(XHud_Module_Element.rms_name));
            #endregion

            #region PrimitiveTweens
            sp_autoplay_primitives_tween = serializedObject.FindProperty(nameof(XHud_Module_Element.autoplay_primitives_tween));
            sp_primitives_tween_max_duration = serializedObject.FindProperty(nameof(XHud_Module_Element.primitives_tween_max_duration));
            sp_primitives_tween_global_duration = serializedObject.FindProperty(nameof(XHud_Module_Element.primitives_tween_global_duration));
            #endregion

            #region PrimitiveController
            sp_primitives_is_fold = serializedObject.FindProperty(nameof(XHud_Module_Element.primitives_is_fold));
            sp_primitive_controller_nodes = serializedObject.FindProperty(nameof(XHud_Module_Element.primitive_controller_nodes));
            #endregion

            #region Actions
            sp_clear_actions_with_spawn = serializedObject.FindProperty(nameof(XHud_Module_Element.clear_actions_with_spawn));
            #endregion

            #region Highlighter
            sp_highlighter = serializedObject.FindProperty(nameof(XHud_Module_Element.highlighter));
            #endregion
            #endregion

            // 获取组件
            GetComponents();

            Vector2 ButtonSize = new Vector2(14, 14);

            ClearEmptyNodes();

            #region 图标获取
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_main");
            icon_sound = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_sound");
            icon_button = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_button");
            icon_option = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_option");
            icon_slider = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_slider");
            icon_progress = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_progress");
            icon_toggle = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_toggle");
            icon_text = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_text");
            icon_tmptext = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_tmptext");
            icon_anim = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_anim");
            icon_scan_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_scan_r");
            icon_scan_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_scan_p");
            icon_record_rms_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_record_rms_r");
            icon_record_rms_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_record_rms_p");
            resetanchorpos_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/resetanchorpos_r");
            resetanchorpos_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/resetanchorpos_p");
            locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/locate_r");
            locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/locate_p");
            left_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/left_arrow_r");
            left_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/left_arrow_p");
            right_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/right_arrow_r");
            right_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/right_arrow_p");
            prw_play_out_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/prw_play_out_r");
            prw_play_out_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/prw_play_out_p");
            prw_play_in_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/prw_play_in_p");
            prw_play_in_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/prw_play_in_r");
            save_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/save_r");
            save_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/save_p");
            reset_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/reset_r");
            reset_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/reset_p");
            icon_recreateid_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_recreateid_r");
            icon_recreateid_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_recreateid_p");
            edit_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/edit_r");
            edit_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/edit_p");
            #endregion

            // 控件列表绘制
            ReorderableList_Draw_Sounder();
            ReorderableList_Draw_PrimitiveController();
            ReorderableList_Draw_Button();
            ReorderableList_Draw_Option();
            ReorderableList_Draw_Slider();
            ReorderableList_Draw_Progress();
            ReorderableList_Draw_Toggle();
            ReorderableList_Draw_Text();
            ReorderableList_Draw_TmpText();

            // RMS 残留信息清理
            if (hud_manager != null)
            {
                if (hud_manager.hm_RMS_GetResolutionNodes().Length <= 0 && sp_rms_layout_datas.arraySize > 0)
                {
                    EditorApplication.delayCall += () =>
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素消息",
                            title: "RMS残留信息",
                            msg: $"发现残留匹配分辨率信息列表，是否需要清空？请谨慎此操作！",
                            ok: "清空",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        if (res == "清空")
                        {
                            sp_rms_layout_datas.ClearArray();
                            sp_rms_layout_datas.serializedObject.ApplyModifiedProperties();
                        }
                    };
                }
            }

            Targets_Get();

            Preview_Config_Load();
        }
        /// <summary>
        /// Unity 禁用回调。
        /// <para/>
        /// 编辑器非播放模式下：
        /// <list type="bullet">
        /// <item><description>停止预览动画与音效；</description></item>
        /// <item><description>关闭场景视图激活标记。</description></item>
        /// </list>
        /// </summary>
        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                if (base_script != null)
                {
                    RMS_Redraw();
                    XGUI.EditorPlayer_Sound.Stop();
                    Preview_Kill();
                }
            }

            //AllListFoldState(true);

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);
        }
        /// <summary>
        /// 绘制 Inspector 主入口。
        /// <para/>
        /// 按自上而下的顺序绘制所有面板：
        /// 标题 → 快捷功能 → 预览 → 参数 → 选项 → 状态 → RMS 布局信息 → 子组件 → 统计 → 组件 → 源脚本。
        /// <para/>
        /// 右键菜单在最后统一处理。
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_text: string.IsNullOrEmpty(sp_indicator.stringValue) ? "XHud  -  元素" : "XHud - 元素 -> ( " + sp_indicator.stringValue + " )",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping_ellipsis,
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 快捷功能          
            XGUI.layout_group_start(
               type: XGUIContainerType.Horizontal,
               bg_fill: XGUIGroup.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
               title: "快捷功能",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(15, 15, 20, 15));

            #region 扫描子组件
            if (XGUI.layout_button(
                tooltip: "扫描子组件",
                tex_release: icon_scan_r,
                tex_press: icon_scan_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                if (Application.isPlaying)
                {
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 元素消息",
                        title: "扫描子元素",
                        msg: $"程序正在运行，无法在运行期间执行此功能！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    return;
                }

                GetPrimitivesTween();
                GetAllText();
                GetAllTmpText();
                GetAllSounder();
                GetAllButtons();
                GetAllOptions();
                GetAllSliders();
                GetAllProgress();
                GetAllToggle();
                GetModulesResult();
                return;
            }
            #endregion

            XGUI.layout_flexspace();

            #region 记录RMS布局信息
            if (sp_rms_enabled.boolValue)
            {
                if (XGUI.layout_button(
                tooltip: "记录RMS布局信息",
                tex_release: icon_record_rms_r,
                tex_press: icon_record_rms_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
                {
                    if (Application.isPlaying)
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素消息",
                            title: "记录RMS布局信息",
                            msg: $"程序正在运行，无法在运行期间执行此功能！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                        return;
                    }

                    RMS_Record();

                    return;
                }
                XGUI.layout_flexspace();
            }
            #endregion

            #region 元素重置到锚点初始位置
            if (XGUI.layout_button(
                tooltip: "元素重置到锚点初始位置",
                tex_release: resetanchorpos_r,
                tex_press: resetanchorpos_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                if (Application.isPlaying)
                {
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 元素消息",
                        title: "锚点初始化",
                        msg: $"程序正在运行，无法在运行期间执行此功能！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                    return;
                }

                RestoreToAnchorPosition();

                return;
            }
            #endregion

            #region 刷新所有图元控制的ID
            if (sp_primitive_controller_nodes.arraySize > 0)
            {
                XGUI.layout_flexspace();

                if (XGUI.layout_button(
                    tooltip: "刷新所有图元控制的ID",
                    tex_release: icon_recreateid_r,
                    tex_press: icon_recreateid_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: 14,
                    height: 14))
                {
                    if (Targets_Selected())
                    {
                        List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();

                        for (int i = 0; i < selected_objects.Length; i++)
                        {
                            XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();
                            data.Title = string.IsNullOrEmpty(selected_objects[i].indicator) ? selected_objects[i].name : selected_objects[i].indicator;
                            data.SubTitle = "即将更新所有子级图元控制器的ID";
                            data.Message = "";
                            Datas.Add(data);
                        }

                        EditorApplication.delayCall += () =>
                        {
                            string res_x = XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 元素消息",
                                title: "批量元素子级控制器ID刷新",
                                msg: $"是否需要批量刷新所有元素的子级的控制器的ID吗？",
                                ok: "刷新",
                                cancel: "暂不",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                            if (res_x == "刷新")
                            {
                                for (int i = 0; i < selected_objects.Length; i++)
                                {
                                    XHud_Module_Element ele = selected_objects[i];

                                    // 收集每个元素下所有子级图元控制器的 ID
                                    List<string> list = new List<string>();
                                    for (int k = 0; k < ele.primitive_controller_nodes.Count; k++)
                                    {
                                        list.Add(ele.primitive_controller_nodes[k].controller.id);
                                    }

                                    for (int c = 0; c < ele.primitive_controller_nodes.Count; c++)
                                    {
                                        Undo.RecordObject(ele.primitive_controller_nodes[c].controller, "刷新控制器ID");
                                        XHud_Module_Primitive_Controller con = ele.primitive_controller_nodes[c].controller;
                                        // 根据收集的 list 来不重复创建 ID 给每个子级图元控制器
                                        con.id = XGUI_Utilitys.GenerateUniqueId(list.ToArray());
                                    }
                                }
                            }
                        };
                    }
                    else
                    {
                        EditorApplication.delayCall += () =>
                        {
                            string res_x = XGUI.dialog(
                               type: XGUIDialogType.警告,
                               windowtitle: "XHud - 元素消息",
                               title: "所有图元控制器ID刷新",
                               msg: $"是否需要批量刷新所有图元控制器的ID吗？",
                               ok: "刷新",
                               cancel: "暂不",
                               PrimaryIndex: 0,
                               usemodal: true,
                               themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            if (res_x == "刷新")
                            {
                                List<string> list = new List<string>();
                                for (int i = 0; i < base_script.primitive_controller_nodes.Count; i++)
                                {
                                    XHud_Module_Primitive_Controller con = base_script.primitive_controller_nodes[i].controller;
                                    list.Add(con.id);
                                }

                                for (int i = 0; i < base_script.primitive_controller_nodes.Count; i++)
                                {
                                    Undo.RecordObject(base_script.primitive_controller_nodes[i].controller, "批量刷新ID");
                                    XHud_Module_Primitive_Controller con = base_script.primitive_controller_nodes[i].controller;
                                    // 记录每个对象的状态
                                    con.id = XGUI_Utilitys.GenerateUniqueId(list.ToArray());
                                }
                            }
                        };
                    }
                }
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 预览          
            base_script.fold_preview = XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIGroup.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
               title: "预览",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(5, 5, 20, 15),
               foldout: base_script.fold_preview);

            if (!base_script.fold_preview)
            {
                #region 控制按钮
                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    bg_fill: XGUIGroup.透明,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    absolute_margin: true,
                    absolute_padding: true,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(15, 15, 5, 10));

                #region 生成
                XGUI.SetEnabled(!sp_tweens_preivew_out_state.boolValue);
                if (XGUI.layout_button(
                    tooltip: !sp_tweens_preivew_in_state.boolValue ? "元素 - 进入 - 播放" : "元素 - 进入 - 停止",
                    tex_release: !sp_tweens_preivew_in_state.boolValue ? prw_play_in_r : prw_play_in_p,
                    tex_press: !sp_tweens_preivew_in_state.boolValue ? prw_play_in_p : prw_play_in_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: 14,
                    height: 14))
                {
                    if (!sp_tweens_preivew_in_state.boolValue)
                        Preview_ElementTween_In_Play();
                    else
                        Preview_ElementTween_In_Stop();
                }
                #endregion

                XGUI.layout_flexspace();

                #region 回收
                XGUI.SetEnabled(!sp_tweens_preivew_in_state.boolValue);
                if (XGUI.layout_button(
                tooltip: !sp_tweens_preivew_out_state.boolValue ? "元素 - 退出 - 播放" : "元素 - 退出 - 停止",
                tex_release: !sp_tweens_preivew_out_state.boolValue ? prw_play_out_r : prw_play_out_p,
                tex_press: !sp_tweens_preivew_out_state.boolValue ? prw_play_out_p : prw_play_out_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
                {
                    if (!sp_tweens_preivew_out_state.boolValue)
                        Preview_ElementTween_Out_Play();
                    else
                        Preview_ElementTween_Out_Stop();
                }
                XGUI.SetEnabled(true);
                #endregion

                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                sp_tweens_preivew_in_state.serializedObject.ApplyModifiedProperties();
                sp_tweens_preivew_out_state.serializedObject.ApplyModifiedProperties();
                #endregion

                if (!sp_tweens_preivew_in_state.boolValue && !sp_tweens_preivew_out_state.boolValue)
                {
                    XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                            margin: new RectOffset(15, 15, 5, 25));

                    #region 选项          
                    XGUI.layout_group_start(
                        type: XGUIContainerType.Vertical,
                        bg_fill: XGUIGroup.缺口纯色边框,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                        title: "选项",
                        title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_size: XGUIFontSize.M,
                        title_clipping: TextClipping.Clip,
                        title_offset: new Vector2(10, 0),
                        absolute_margin: true,
                        absolute_padding: true,
                        margin: new RectOffset(0, 0, 20, 15),
                        padding: new RectOffset(15, 0, 20, 20));

                    DrawToggle("包含子级图元", sp_preview_include_primitives_tween, 140, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_include, (b) => { });
                    DrawToggle("指定位置 - 生成", sp_create_preview_set_position, 140, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                    DrawToggle("指定位置 - 回收", sp_recycle_preview_set_position, 140, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });

                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                    #endregion

                    #region 参数
                    if (sp_create_preview_set_position.boolValue || sp_recycle_preview_set_position.boolValue)
                    {
                        XGUI.layout_group_start(
                            type: XGUIContainerType.Vertical,
                            bg_fill: XGUIGroup.缺口纯色边框,
                            bg_color: XGUIColor.亮白,
                            bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                            title: "参数",
                            title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            title_size: XGUIFontSize.M,
                            title_clipping: TextClipping.Clip,
                            title_offset: new Vector2(10, 0),
                            absolute_margin: true,
                            absolute_padding: true,
                            margin: new RectOffset(0, 0, 10, 15),
                            padding: new RectOffset(15, 10, 20, 20));

                        #region 目标位置
                        if (sp_create_preview_set_position.boolValue)
                        {
                            XGUI.layout_property_field(
                                title: "目标位置",
                                title_size: XGUIFontSize.M,
                                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                title_width: 90,
                                prop: sp_create_preview_position,
                                prop_margin: new RectOffset(0, 0, 5, 0));
                        }
                        #endregion

                        #region 目标位置
                        if (sp_recycle_preview_set_position.boolValue)
                        {
                            XGUI.layout_property_field(
                                title: "目标位置",
                                title_size: XGUIFontSize.M,
                                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                title_width: 90,
                                prop: sp_recycle_preview_position,
                                prop_margin: new RectOffset(0, 0, 5, 0));
                        }
                        #endregion

                        XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                    }
                    #endregion

                    #region 动效
                    XGUI.layout_group_start(
                         type: XGUIContainerType.Vertical,
                            bg_fill: XGUIGroup.缺口纯色边框,
                            bg_color: XGUIColor.亮白,
                            bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                            title: "动效",
                            title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            title_size: XGUIFontSize.M,
                            title_clipping: TextClipping.Clip,
                            title_offset: new Vector2(10, 0),
                            absolute_margin: true,
                            absolute_padding: true,
                            margin: new RectOffset(0, 0, 10, 15),
                            padding: new RectOffset(15, 10, 20, 20));

                    #region 生成
                    #region 动效模板
                    if (hud_manager.library_motions != null)
                    {
                        //确保动效库不是空的
                        if (hud_manager.library_motions.motion_list == null || hud_manager.library_motions.motion_list.Count <= 0)
                        {
                            XGUI.layout_label(
                                text: "未在动效库中发现任何动效资源，请先为其添加动效资源！",
                                bg_fill: XGUIGroup.实体,
                                bg_color: XGUIColor.深空灰,
                                size: XGUIFontSize.S,
                                anchor: TextAnchor.MiddleLeft,
                                text_color: Color.white * 0.85f,
                                offset: new Vector2(0, 0),
                                padding: new RectOffset(10, 10, 10, 10),
                                margin: new RectOffset(0, 0, 6, 0),
                                clipping: TextClipping.Clip,
                                wrap: true,
                                font_style: FontStyle.Normal);
                        }
                        else
                        {
                            #region 模板下拉菜单
                            string[] motnames = hud_manager.library_motions.ElementMotion_GetAllName_With_Create();
                            XGUI.layout_string_popup(
                                title: XGUI.CurrentWindowWidthThreshold(">", 240) ? "模板 - 生成" : null,
                                title_width: 120,
                                title_size: XGUIFontSize.M,
                                title_anchor: TextAnchor.MiddleLeft,
                                prop: sp_motion_create_library_index_name,
                                options: motnames,
                                opt_text_size: XGUIFontSize.M,
                                opt_text_color: Color.black,
                                opt_text_padding: new RectOffset(10, 10, 0, 0),
                                opt_anchor: TextAnchor.MiddleCenter,
                                opt_font_style: FontStyle.Normal,
                                opt_bg_fill: XGUIGroup.实体,
                                opt_bg_color: XGUIColor.亮白,
                                opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                icon_arrow_color: Color.black,
                                margin: new RectOffset(0, 0, 5, 5),
                                padding: new RectOffset(5, 5, 0, 0),
                                title_margin: new RectOffset(0, 0, 0, 0),
                                act_on_changed: (value) =>
                                {
                                    sp_motion_create_library_index_name.stringValue = value;
                                    sp_motion_create_library_index_name.serializedObject.ApplyModifiedProperties();

                                    Motion_Creator crc = hud_manager.library_motions.ElementMotion_GetElementCreator_At_Create(sp_motion_create_library_index_name.stringValue);
                                    base_script.motion_create_args = crc.Clone();
                                });
                            #endregion
                        }
                    }
                    else
                    {
                        XGUI.layout_label(
                           text: "XHud 管理器中未指定动效库，请先配置动效库！",
                           bg_fill: XGUIGroup.实体,
                           bg_color: XGUIColor.深空灰,
                           size: XGUIFontSize.S,
                           anchor: TextAnchor.MiddleLeft,
                           text_color: Color.white * 0.85f,
                           offset: new Vector2(0, 0),
                           padding: new RectOffset(10, 10, 10, 10),
                           margin: new RectOffset(0, 0, 6, 0),
                           clipping: TextClipping.Clip,
                           wrap: true,
                           font_style: FontStyle.Normal);
                    }
                    #endregion

                    XGUI.layout_group_start(
                        type: XGUIContainerType.Horizontal,
                        title_clipping: TextClipping.Clip,
                        absolute_padding: true,
                        absolute_margin: true,
                        margin: new RectOffset(0, 0, 12, 0),
                        padding: new RectOffset(5, 10, 0, 0));

                    #region 保存
                    XGUI.SetEnabled(hud_manager.library_motions != null);
                    if (XGUI.layout_button(
                        tooltip: "保存",
                        tex_release: save_r,
                        tex_press: save_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: save_r.width,
                        height: save_r.height))
                    {
                        motion_save(HudElementMotionType.Creator);
                        return;
                    }
                    #endregion

                    XGUI.layout_flexspace();

                    #region 定位
                    XGUI.SetEnabled(hud_manager.library_motions != null);
                    if (XGUI.layout_button(
                      tooltip: "定位",
                      tex_release: locate_r,
                      tex_press: locate_p,
                      tex_gui_color: Color.white,
                      border: new RectOffset(0, 0, 0, 0),
                      width: locate_r.width,
                      height: locate_r.height))
                    {
                        if (!hud_manager.library_motions.ElementMotion_IsExist(sp_motion_create_library_index_name.stringValue))
                            return;
                        Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                        hud_manager.library_motions.ElementMotionLibrary_Location(sp_motion_create_library_index_name.stringValue);
                        return;
                    }
                    XGUI.SetEnabled(true);
                    #endregion

                    XGUI.layout_flexspace();

                    #region 修改
                    if (XGUI.layout_button(
                      tooltip: "修改",
                      tex_release: edit_r,
                      tex_press: edit_p,
                      tex_gui_color: Color.white,
                      border: new RectOffset(0, 0, 0, 0),
                      width: edit_r.width,
                      height: edit_r.height))
                    {
                        motion_edit(new XHud_Arg_MotionGroup(base_script.motion_create_args));
                        return;
                    }
                    #endregion

                    XGUI.layout_flexspace();

                    #region 重置
                    if (XGUI.layout_button(
                      tooltip: "重置",
                      tex_release: reset_r,
                      tex_press: reset_p,
                      tex_gui_color: Color.white,
                      border: new RectOffset(0, 0, 0, 0),
                      width: reset_r.width,
                      height: reset_r.height))
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素生成器消息",
                            title: "重置动效参数",
                            msg: "确定要将动效参数重置吗？您将丢失当前的动效参数！",
                            ok: "重置",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            on_selected: (d) =>
                            {
                                if (d == "重置")
                                {
                                    sp_motion_create_library_index_name.stringValue = null;
                                    sp_motion_create_library_index_name.serializedObject.ApplyModifiedProperties();
                                    base_script.motion_create_args.Reset();
                                }
                            });
                        return;
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                    #endregion

                    XGUI.layout_seperator(
                        thickness: 1,
                        color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                        margin: new RectOffset(15, 15, 15, 15));

                    #region 回收
                    #region 动效模板
                    if (hud_manager.library_motions != null)
                    {
                        //确保动效库不是空的
                        if (hud_manager.library_motions.motion_list == null || hud_manager.library_motions.motion_list.Count <= 0)
                        {
                            XGUI.layout_label(
                                text: "未在动效库中发现任何动效资源，请先为其添加动效资源！",
                                bg_fill: XGUIGroup.实体,
                                bg_color: XGUIColor.深空灰,
                                size: XGUIFontSize.S,
                                anchor: TextAnchor.MiddleLeft,
                                text_color: Color.white * 0.85f,
                                offset: new Vector2(0, 0),
                                padding: new RectOffset(10, 10, 10, 10),
                                margin: new RectOffset(0, 0, 6, 0),
                                clipping: TextClipping.Clip,
                                wrap: true,
                                font_style: FontStyle.Normal);
                        }
                        else
                        {
                            #region 模板下拉菜单
                            string[] motnames = hud_manager.library_motions.ElementMotion_GetAllName_With_Recycle();
                            XGUI.layout_string_popup(
                                title: XGUI.CurrentWindowWidthThreshold(">", 240) ? "模板 - 回收" : null,
                                title_width: 120,
                                title_size: XGUIFontSize.M,
                                title_anchor: TextAnchor.MiddleLeft,
                                prop: sp_motion_recycle_library_index_name,
                                options: motnames,
                                opt_text_size: XGUIFontSize.M,
                                opt_text_color: Color.black,
                                opt_text_padding: new RectOffset(10, 10, 0, 0),
                                opt_anchor: TextAnchor.MiddleCenter,
                                opt_font_style: FontStyle.Normal,
                                opt_bg_fill: XGUIGroup.实体,
                                opt_bg_color: XGUIColor.亮白,
                                opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                icon_arrow_color: Color.black,
                                margin: new RectOffset(0, 0, 5, 5),
                                padding: new RectOffset(5, 5, 0, 0),
                                title_margin: new RectOffset(0, 0, 0, 0),
                                act_on_changed: (value) =>
                                {
                                    sp_motion_recycle_library_index_name.stringValue = value;
                                    sp_motion_recycle_library_index_name.serializedObject.ApplyModifiedProperties();

                                    Motion_Recycler crc = hud_manager.library_motions.ElementMotion_GetElementCreator_At_Recycle(sp_motion_recycle_library_index_name.stringValue);
                                    base_script.motion_recycle_args = crc.Clone();
                                });
                            #endregion
                        }
                    }
                    else
                    {
                        XGUI.layout_label(
                           text: "XHud 管理器中未指定动效库，请先配置动效库！",
                           bg_fill: XGUIGroup.实体,
                           bg_color: XGUIColor.深空灰,
                           size: XGUIFontSize.S,
                           anchor: TextAnchor.MiddleLeft,
                           text_color: Color.white * 0.85f,
                           offset: new Vector2(0, 0),
                           padding: new RectOffset(10, 10, 10, 10),
                           margin: new RectOffset(0, 0, 6, 0),
                           clipping: TextClipping.Clip,
                           wrap: true,
                           font_style: FontStyle.Normal);
                    }
                    #endregion

                    XGUI.layout_group_start(
                        type: XGUIContainerType.Horizontal,
                        title_clipping: TextClipping.Clip,
                        absolute_padding: true,
                        absolute_margin: true,
                        margin: new RectOffset(0, 0, 12, 0),
                        padding: new RectOffset(5, 10, 0, 0));

                    #region 保存
                    XGUI.SetEnabled(hud_manager.library_motions != null);
                    if (XGUI.layout_button(
                        tooltip: "保存",
                        tex_release: save_r,
                        tex_press: save_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: save_r.width,
                        height: save_r.height))
                    {
                        motion_save(HudElementMotionType.Recycler);
                        return;
                    }
                    #endregion

                    XGUI.layout_flexspace();

                    #region 定位
                    XGUI.SetEnabled(hud_manager.library_motions != null);
                    if (XGUI.layout_button(
                      tooltip: "定位",
                      tex_release: locate_r,
                      tex_press: locate_p,
                      tex_gui_color: Color.white,
                      border: new RectOffset(0, 0, 0, 0),
                      width: locate_r.width,
                      height: locate_r.height))
                    {
                        if (!hud_manager.library_motions.ElementMotion_IsExist(sp_motion_recycle_library_index_name.stringValue))
                            return;
                        Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                        hud_manager.library_motions.ElementMotionLibrary_Location(sp_motion_recycle_library_index_name.stringValue);
                        return;
                    }
                    XGUI.SetEnabled(true);
                    #endregion

                    XGUI.layout_flexspace();

                    #region 修改
                    if (XGUI.layout_button(
                      tooltip: "修改",
                      tex_release: edit_r,
                      tex_press: edit_p,
                      tex_gui_color: Color.white,
                      border: new RectOffset(0, 0, 0, 0),
                      width: edit_r.width,
                      height: edit_r.height))
                    {
                        motion_edit(new XHud_Arg_MotionGroup(base_script.motion_recycle_args));
                        return;
                    }
                    #endregion

                    XGUI.layout_flexspace();

                    #region 重置
                    if (XGUI.layout_button(
                      tooltip: "重置",
                      tex_release: reset_r,
                      tex_press: reset_p,
                      tex_gui_color: Color.white,
                      border: new RectOffset(0, 0, 0, 0),
                      width: reset_r.width,
                      height: reset_r.height))
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素生成器消息",
                            title: "重置动效参数",
                            msg: "确定要将动效参数重置吗？您将丢失当前的动效参数！",
                            ok: "重置",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            on_selected: (d) =>
                            {
                                if (d == "重置")
                                {
                                    sp_motion_recycle_library_index_name.stringValue = null;
                                    sp_motion_recycle_library_index_name.serializedObject.ApplyModifiedProperties();
                                    base_script.motion_recycle_args.Reset();
                                }
                            });
                        return;
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                    #endregion

                    #region 添加运行时预览组件
                    if (XGUI.layout_button(
                        text: "添加运行时预览组件",
                        tooltip: "",
                        bg_fill: XGUIGroup.实体,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(XHud_Dashboard.themes_get_color(ThemeColorType.primary)) ? Color.black : Color.white,
                        press_fill: XGUIGroup.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.white,
                        font_size: XGUIFontSize.M,
                        height: 30,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        XHud_Element_Preview xHud_Element_Preview = base_script.GetComponent<XHud_Element_Preview>();
                        if (xHud_Element_Preview == null)
                        {
                            xHud_Element_Preview = Undo.AddComponent<XHud_Element_Preview>(base_script.gameObject);
                            EditorApplication.delayCall += () =>
                            {
                                // 收集原始姿态数据
                                xHud_Element_Preview.OriginalDataCollect();

                                xHud_Element_Preview.motion_create_args.Movement.Movement = HudMotion_Movement.S_从下至上;
                                xHud_Element_Preview.motion_create_args.Movement.Distance = 100;
                                xHud_Element_Preview.motion_recycle_args.Movement.Movement = HudMotion_Movement.D_从上至下;
                                xHud_Element_Preview.motion_recycle_args.Movement.Distance = 100;
                            };
                        }
                    }
                    #endregion
                }
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 参数          
            if (!sp_tweens_preivew_in_state.boolValue && !sp_tweens_preivew_out_state.boolValue)
            {
                base_script.fold_param = XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIGroup.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
               title: "参数",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(5, 5, 20, 15),
               foldout: base_script.fold_param);

                if (!base_script.fold_param)
                {
                    #region 标识名称
                    XGUI.ChangedCheck_Start();
                    XGUI.layout_property_field(
                        title: "标识名称",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_indicator,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    if (XGUI.ChangedCheck_End())
                    {
                        if (Targets_Selected())
                        {
                            for (int i = 0; i < selected_objects.Length; i++)
                            {
                                SerializedObject so_ele = new SerializedObject(selected_objects[i]);
                                SerializedProperty sp_indicator = so_ele.FindProperty(nameof(XHud_Module_Element.indicator));
                                so_ele.Update();
                                sp_indicator.stringValue = this.sp_indicator.stringValue;
                                sp_indicator.serializedObject.ApplyModifiedProperties();
                                so_ele.ApplyModifiedProperties();
                            }
                        }
                    }
                    #endregion

                    #region 透明度
                    XGUI.ChangedCheck_Start();
                    XGUI.layout_property_field(
                        title: "透明度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_alpha,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    if (XGUI.ChangedCheck_End())
                    {
                        ChangeAlpha();
                    }
                    #endregion

                    #region 速率倍增
                    XGUI.layout_property_field(
                        title: "速率倍增",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_primitives_tween_global_duration,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 当前锚点
                    XGUI.SetEnabled(false);
                    XGUI.layout_property_field(
                        title: "当前锚点",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_current_pivot,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    XGUI.SetEnabled(true);
                    #endregion
                }
                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 选项
            if (!sp_tweens_preivew_in_state.boolValue && !sp_tweens_preivew_out_state.boolValue)
            {
                base_script.fold_option = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "选项",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_option);

                if (!base_script.fold_option)
                {
                    #region 状态调试
                    DrawToggle("状态调试", sp_debug_state, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_debug, (b) => { });
                    #endregion

                    #region 预览自动停止
                    DrawToggle("预览自动停止", sp_autokill_preview_tweens, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) =>
                    {
                        sp_autokill_preview_tweens.boolValue = b;
                        sp_autokill_preview_tweens.serializedObject.ApplyModifiedProperties();


                        XHud_Dashboard.element_set_preview_autokill(sp_autokill_preview_tweens.boolValue);
                        Editor_XTween_Previewer.AutoKillWithDuration = sp_autokill_preview_tweens.boolValue;

                        // 如果为自动杀死动画则会强制开启：杀死前重置动画 / 杀死后清空预览列表
                        if (sp_autokill_preview_tweens.boolValue)
                        {
                            // 强制开启：杀死前重置动画 / 杀死后清空预览列表
                            sp_rewind_preview_tweens_with_kill.boolValue = true;
                            sp_rewind_preview_tweens_with_kill.serializedObject.ApplyModifiedProperties();
                            sp_clear_preview_tweens_with_kill.boolValue = true;
                            sp_clear_preview_tweens_with_kill.serializedObject.ApplyModifiedProperties();

                            // 保存配置数据：杀死前重置动画
                            XHud_Dashboard.element_set_preview_rewind_before_kill(sp_rewind_preview_tweens_with_kill.boolValue);

                            // 预览器设置：杀死预览动画前重置动画
                            Editor_XTween_Previewer.BeforeKillRewind = sp_rewind_preview_tweens_with_kill.boolValue;

                            // 保存配置数据：杀死后清空预览列表
                            XHud_Dashboard.element_set_preview_clear_after_kill(sp_clear_preview_tweens_with_kill.boolValue);

                            // 预览器设置：杀死后清空预览列表
                            Editor_XTween_Previewer.AfterKillClear = sp_clear_preview_tweens_with_kill.boolValue;
                        }

                        // 保存预览自动停止开关状态
                        sp_autokill_preview_tweens.serializedObject.ApplyModifiedProperties();
                    });
                    #endregion

                    #region 预览杀死前先重置
                    if (!sp_autokill_preview_tweens.boolValue)
                    {
                        DrawToggle("预览杀死前先重置", sp_rewind_preview_tweens_with_kill, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, new string[2] { "禁用", "启用" }, (b) =>
                    {
                        sp_rewind_preview_tweens_with_kill.boolValue = b;
                        sp_rewind_preview_tweens_with_kill.serializedObject.ApplyModifiedProperties();

                        // 保存配置数据：杀死前重置动画
                        XHud_Dashboard.element_set_preview_rewind_before_kill(sp_rewind_preview_tweens_with_kill.boolValue);

                        // 预览器设置：杀死预览动画前重置动画
                        Editor_XTween_Previewer.BeforeKillRewind = sp_rewind_preview_tweens_with_kill.boolValue;
                    });
                    }
                    #endregion

                    #region 预览杀死后清空预览器列表
                    if (!sp_autokill_preview_tweens.boolValue)
                    {
                        DrawToggle("预览杀死后清空预览器列表", sp_clear_preview_tweens_with_kill, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, new string[2] { "禁用", "启用" }, (b) =>
                    {
                        sp_clear_preview_tweens_with_kill.boolValue = b;
                        sp_clear_preview_tweens_with_kill.serializedObject.ApplyModifiedProperties();

                        // 保存配置数据：杀死后清空预览列表
                        XHud_Dashboard.element_set_preview_clear_after_kill(sp_clear_preview_tweens_with_kill.boolValue);

                        // 预览器设置：杀死后清空预览列表
                        Editor_XTween_Previewer.AfterKillClear = sp_clear_preview_tweens_with_kill.boolValue;
                    });
                    }
                    #endregion

                    XGUI.layout_seperator(
                        thickness: 1,
                        color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                        margin: new RectOffset(15, 15, 15, 15));

                    #region 图元动画器自动播放
                    DrawToggle("图元动画器自动播放", sp_autoplay_primitives_tween, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_auto, (b) => { });
                    #endregion

                    #region 使用设计布局
                    DrawToggle("使用设计布局", sp_rms_enabled, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                    #endregion

                    #region 说明
                    if (sp_rms_enabled.boolValue)
                    {
                        string msg = "此元素在生成时会优先使用自身记录的设计布局信息来进行定位和位置尺寸的匹配（但前提是在您制作好此元素时需要手动点击\"记录布局信息\"按钮）";

                        XGUI.layout_label(
                            text: msg,
                            bg_fill: XGUIGroup.实体,
                            bg_color: XGUIColor.深空灰,
                            size: XGUIFontSize.S,
                            anchor: TextAnchor.MiddleLeft,
                            text_color: Color.white * 0.85f,
                            offset: new Vector2(0, 0),
                            padding: new RectOffset(10, 10, 10, 10),
                            margin: new RectOffset(0, 0, 6, 0),
                            clipping: clipping_ellipsis,
                            wrap: true,
                            font: XGUI.GetFont("xg-regular"),
                            font_style: FontStyle.Normal);
                    }
                    #endregion

                    XGUI.layout_seperator(
                        thickness: 1,
                        color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                        margin: new RectOffset(15, 15, 15, 15));

                    #region 从库中生成时清空委托
                    DrawToggle("从库中生成时清空委托", sp_clear_actions_with_spawn, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                    #endregion

                    #region 从库中生成时清空事件
                    DrawToggle("从库中生成时清空事件", sp_clear_events_with_spawn, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                    #endregion
                }
                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 状态
            if (!sp_tweens_preivew_in_state.boolValue && !sp_tweens_preivew_out_state.boolValue)
            {
                string statu_title = "状态";
                if (Targets_Selected())
                    statu_title = "状态 - ( 批量模式 )";

                base_script.fold_state = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: statu_title,
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_state);

                if (!base_script.fold_state)
                {
                    if (!Targets_Selected())
                    {
                        #region 使用状态
                        XGUI.layout_state_displayer_text(
                            title: "使用状态",
                            title_size: XGUIFontSize.M,
                            subtitle: (XHudElementCreateState)sp_create_state.enumValueIndex == XHudElementCreateState.Created ? "已被生成" : "已被回收",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: (XHudElementCreateState)sp_create_state.enumValueIndex == XHudElementCreateState.Created ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray,
                            margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 动画相关
                        if (sp_primitive_controller_nodes != null && sp_primitive_controller_nodes.arraySize > 0)
                        {
                            #region 动画状态
                            XGUI.layout_state_displayer_text(
                              title: "动画状态",
                              title_size: XGUIFontSize.M,
                              subtitle: (XHudElementAnimateState)sp_animate_state.enumValueIndex == XHudElementAnimateState.Animating ? "动画中" : "静止状态",
                              subtitle_size: XGUIFontSize.M,
                              subtitle_color: sp_animate_state.enumValueIndex == 1 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray,
                              margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 最大耗时（速率倍增）
                            XGUI.layout_state_displayer_text(
                            title: "最大耗时<color=#909090>（速率倍增）</color>",
                            title_size: XGUIFontSize.M,
                            subtitle: sp_primitives_tween_max_duration.floatValue.ToString() + " 秒",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: (XHudElementCreateState)sp_create_state.enumValueIndex == XHudElementCreateState.Created ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.white * 0.75f,
                            margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 最大耗时（XHud 倍增）
                            XGUI.layout_state_displayer_text(
                            title: "最大耗时<color=#909090>（XHud 倍增）</color>",
                            title_size: XGUIFontSize.M,
                            subtitle: (hud_manager.tween_global_duration * sp_primitives_tween_max_duration.floatValue).ToString() + " 秒",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: (XHudElementCreateState)sp_create_state.enumValueIndex == XHudElementCreateState.Created ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.white * 0.75f,
                            margin: new RectOffset(5, 5, 0, 5));
                        }
                        #endregion

                        #endregion

                        #region 物体跟踪状态
                        if (sp_object_tracker.objectReferenceValue != null)
                        {
                            XHud_ObjectTracker tracker = (XHud_ObjectTracker)sp_object_tracker.objectReferenceValue;
                            if (tracker.self_object != null && tracker.target_object != null)
                            {
                                XGUI.layout_state_displayer_text(
                                    title: "物体跟踪状态",
                                    title_size: XGUIFontSize.M,
                                    subtitle: tracker.self_object.name + "  >  " + tracker.target_object.name,
                                    subtitle_size: XGUIFontSize.M,
                                    subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    margin: new RectOffset(5, 5, 0, 5));
                            }
                        }
                        #endregion

                        #region 元素库源
                        if (Application.isPlaying)
                        {
                            bool lib_exist = false;

                            if (!string.IsNullOrEmpty(sp_origin_poolname.stringValue))
                            {
                                //遍历元素库集合找到目标元素库
                                foreach (XHud_Library_Element lib in hud_manager.ele_library_elements)
                                {
                                    if (lib.library_name == sp_origin_poolname.stringValue)
                                    {
                                        if (lib.ElementLibrary_IsExist(sp_original_name.stringValue))
                                        {
                                            lib_exist = true;
                                            break;
                                        }
                                        break;
                                    }
                                }
                            }

                            if (lib_exist)
                            {
                                XGUI.layout_state_displayer_btn(
                                    title: "元素库源",
                                    title_size: XGUIFontSize.M,
                                    btn_bg: XGUIGroup.实体,
                                    btn_color: XGUIColor.亮白,
                                    btn_text: sp_origin_poolname.stringValue,
                                    btn_tooltip: "定位",
                                    btn_text_size: XGUIFontSize.M,
                                    btn_layout_width: 120,
                                    btn_gui_color: Color.white,
                                    padding: new RectOffset(0, 0, 0, 0),
                                    callback_clicked: () =>
                                    {
                                        if (!lib_exist)
                                        {
                                            XGUI.dialog(
                                                type: XGUIDialogType.警告,
                                                windowtitle: "XHud - 元素消息",
                                                title: "定位元素",
                                                msg: $"此元素并非由元素库生成，无法为其进行定位！",
                                                ok: "明白",
                                                PrimaryIndex: 0,
                                                usemodal: true,
                                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                                            return;
                                        }
                                        //打开目标元素库
                                        Editor_XHud_MenuItemsAction_OpenLibrary.open_target_elements(sp_origin_poolname.stringValue).ElementLibrary_Location_Find(sp_original_name.stringValue);
                                    });
                            }
                            else
                            {
                                XGUI.layout_state_displayer_text(
                                    title: "元素库源",
                                    title_size: XGUIFontSize.M,
                                    subtitle: "非元素库资源",
                                    subtitle_size: XGUIFontSize.M,
                                    subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    margin: new RectOffset(5, 5, 0, 5));
                            }
                        }
                        else
                        {
                            //编辑器模式下，通过遍历所有元素库来找到当前这个元素在哪个库里
                        }
                        #endregion
                    }
                    else
                    {
                        #region 控制区
                        XGUI.layout_group_start(
                            type: XGUIContainerType.Horizontal,
                            bg_fill: XGUIGroup.透明,
                            bg_color: XGUIColor.亮白,
                            bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                            absolute_margin: true,
                            absolute_padding: true,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(15, 15, 5, 10));

                        #region 标题按钮
                        if (XGUI.layout_button(
                            text: $"{selected_objects[element_statu_index].name} ( {selected_objects[element_statu_index].indicator} )",
                            tooltip: "",
                            bg_fill: XGUIGroup.透明,
                            button_text_color: Color.gray,
                            press_fill: XGUIGroup.透明,
                            press_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            font_size: XGUIFontSize.M,
                            anchor: TextAnchor.MiddleLeft,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0),
                            layout_width: 150,
                            button_text_font: XGUI.GetFont("xg-medium")))
                        {
                            EditorGUIUtility.PingObject(selected_objects[element_statu_index]);
                        }
                        #endregion

                        XGUI.layout_flexspace();

                        #region 上一个
                        if (XGUI.layout_button(
                            tooltip: "上一个",
                            tex_release: left_arrow_r,
                            tex_press: left_arrow_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: 12,
                            height: 12))
                        {
                            if (element_statu_index <= 0)
                            {
                                element_statu_index = selected_objects.Length - 1;
                            }
                            else
                            {
                                element_statu_index--;
                            }
                            EditorGUIUtility.PingObject(selected_objects[element_statu_index]);
                        }
                        #endregion

                        XGUI.layout_space(20);

                        #region 下一个
                        if (XGUI.layout_button(
                            tooltip: "下一个",
                            tex_release: right_arrow_r,
                            tex_press: right_arrow_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: 12,
                            height: 12))
                        {
                            if (element_statu_index >= selected_objects.Length - 1)
                            {
                                element_statu_index = 0;
                            }
                            else
                            {
                                element_statu_index++;
                            }
                            EditorGUIUtility.PingObject(selected_objects[element_statu_index]);
                        }
                        #endregion

                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                        #endregion

                        XGUI.layout_seperator(
                                thickness: 1,
                                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                                margin: new RectOffset(15, 15, 5, 25));

                        #region 使用状态
                        XGUI.layout_state_displayer_text(
                            title: "使用状态",
                            title_size: XGUIFontSize.M,
                            subtitle: selected_objects[element_statu_index].create_state == XHudElementCreateState.Created ? "已被生成" : "已被回收",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: selected_objects[element_statu_index].create_state == XHudElementCreateState.Created ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray,
                            margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 动画相关
                        if (sp_primitive_controller_nodes != null && sp_primitive_controller_nodes.arraySize > 0)
                        {
                            #region 动画状态
                            XGUI.layout_state_displayer_text(
                                title: "动画状态",
                                title_size: XGUIFontSize.M,
                                subtitle: selected_objects[element_statu_index].animate_state == XHudElementAnimateState.Animating ? "动画中" : "静止状态",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: (int)selected_objects[element_statu_index].animate_state == 1 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray,
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 最大耗时（速率倍增）
                            XGUI.layout_state_displayer_text(
                                title: "最大耗时<color=#909090>（速率倍增）</color>",
                                title_size: XGUIFontSize.M,
                                subtitle: selected_objects[element_statu_index].primitives_tween_max_duration.ToString() + " 秒",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: (XHudElementCreateState)sp_create_state.enumValueIndex == XHudElementCreateState.Created ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.white * 0.75f,
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 最大耗时（XHud 倍增）
                            XGUI.layout_state_displayer_text(
                                title: "最大耗时<color=#909090>（XHud 倍增）</color>",
                                title_size: XGUIFontSize.M,
                                subtitle: (hud_manager.tween_global_duration * selected_objects[element_statu_index].primitives_tween_max_duration).ToString() + " 秒",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: (XHudElementCreateState)selected_objects[element_statu_index].create_state == XHudElementCreateState.Created ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.white * 0.75f,
                                margin: new RectOffset(5, 5, 0, 5));
                        }
                        #endregion

                        #endregion

                        #region 物体跟踪状态
                        if (sp_object_tracker.objectReferenceValue != null)
                        {
                            XHud_ObjectTracker tracker = (XHud_ObjectTracker)selected_objects[element_statu_index].object_tracker;
                            if (tracker.self_object != null && tracker.target_object != null)
                            {
                                XGUI.layout_state_displayer_text(
                                    title: "物体跟踪状态",
                                    title_size: XGUIFontSize.M,
                                    subtitle: tracker.self_object.name + "  >  " + tracker.target_object.name,
                                    subtitle_size: XGUIFontSize.M,
                                    subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    margin: new RectOffset(5, 5, 0, 5));
                            }
                        }
                        #endregion

                        #region 元素库源
                        if (Application.isPlaying)
                        {
                            bool lib_exist = false;

                            if (!string.IsNullOrEmpty(selected_objects[element_statu_index].origin_poolname))
                            {
                                //遍历元素库集合找到目标元素库
                                foreach (XHud_Library_Element lib in hud_manager.ele_library_elements)
                                {
                                    if (lib.library_name == selected_objects[element_statu_index].origin_poolname)
                                    {
                                        if (lib.ElementLibrary_IsExist(selected_objects[element_statu_index].original_name))
                                        {
                                            lib_exist = true;
                                            break;
                                        }
                                        break;
                                    }
                                }
                            }

                            if (lib_exist)
                            {
                                XGUI.layout_state_displayer_btn(
                                    title: "元素库源",
                                    title_size: XGUIFontSize.M,
                                    btn_bg: XGUIGroup.实体,
                                    btn_color: XGUIColor.亮白,
                                    btn_text: selected_objects[element_statu_index].origin_poolname,
                                    btn_tooltip: "定位",
                                    btn_text_size: XGUIFontSize.M,
                                    btn_layout_width: 120,
                                    btn_gui_color: Color.white,
                                    padding: new RectOffset(0, 0, 0, 0),
                                    callback_clicked: () =>
                                    {
                                        if (!lib_exist)
                                        {
                                            XGUI.dialog(
                                                type: XGUIDialogType.警告,
                                                windowtitle: "XHud - 元素消息",
                                                title: "定位元素",
                                                msg: $"此元素并非由元素库生成，无法为其进行定位！",
                                                ok: "明白",
                                                PrimaryIndex: 0,
                                                usemodal: true,
                                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                                            return;
                                        }
                                        //打开目标元素库
                                        Editor_XHud_MenuItemsAction_OpenLibrary.open_target_elements(selected_objects[element_statu_index].origin_poolname).ElementLibrary_Location_Find(selected_objects[element_statu_index].original_name);
                                    });
                            }
                            else
                            {
                                XGUI.layout_state_displayer_text(
                                    title: "元素库源",
                                    title_size: XGUIFontSize.M,
                                    subtitle: "非元素库资源",
                                    subtitle_size: XGUIFontSize.M,
                                    subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    margin: new RectOffset(5, 5, 0, 5));
                            }
                        }
                        else
                        {
                            //编辑器模式下，通过遍历所有元素库来找到当前这个元素在哪个库里
                        }
                        #endregion
                    }
                }
                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region RMS 布局信息
            if (!sp_tweens_preivew_in_state.boolValue && !sp_tweens_preivew_out_state.boolValue)
            {
                string rms_lay_title = "RMS 布局信息";
                if (Targets_Selected())
                    rms_lay_title = "RMS 布局信息 - ( 批量模式 )";

                base_script.fold_rms = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: rms_lay_title,
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_rms);

                if (!base_script.fold_rms)
                {
                    if (hud_manager != null)
                    {
                        string[] nodesName = hud_manager.hm_RMS_GetResolutionNodeNames();
                        if (nodesName.Length > 0)
                        {
                            XGUI.layout_helpbox(
                                    state: XGUIHelboxState.通知,
                                    title_text: "RMS方案列表仅用于切换和预览，在取消选择元素后会自动回到Hud管理器当前选中的RMS方案",
                                    title_size: XGUIFontSize.M,
                                    title_style: FontStyle.Normal,
                                    wrap: true,
                                    title_color: Color.white * 0.75f);

                            sp_rms_name.stringValue = XGUI.layout_string_popup(
                                title: "R M S 方案",
                                title_width: 120,
                                title_color: Color.white,
                                title_size: XGUIFontSize.M,
                                title_anchor: TextAnchor.MiddleLeft,
                                prop: sp_rms_name,
                                options: nodesName,
                                opt_text_size: XGUIFontSize.M,
                                opt_text_color: Color.black,
                                opt_text_padding: new RectOffset(10, 10, 0, 0),
                                opt_anchor: TextAnchor.MiddleLeft,
                                opt_font_style: FontStyle.Normal,
                                opt_bg_fill: XGUIGroup.实体,
                                opt_bg_color: XGUIColor.亮白,
                                opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(0, 0, 5, 5),
                                padding: new RectOffset(5, 5, 0, 0),
                                title_margin: new RectOffset(0, 0, 0, 0),
                                icon_arrow_color: Color.black,
                                act_on_changed: (v) =>
                                {
                                    sp_rms_name.stringValue = v;
                                    sp_rms_name.serializedObject.ApplyModifiedProperties();

                                    if (!Application.isPlaying)
                                    {
                                        if (Targets_Selected())
                                        {
                                            for (int i = 0; i < selected_objects.Length; i++)
                                            {
                                                if (!string.IsNullOrEmpty(selected_objects[i].gameObject.scene.name))
                                                    RMS_Preview(selected_objects[i], selected_objects[i].alpha, Vector3.zero, selected_objects[i].rms_name, true);
                                            }
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(base_script.gameObject.scene.name))
                                                RMS_Preview(base_script, sp_alpha.floatValue, Vector3.zero, sp_rms_name.stringValue, true);
                                        }
                                    }
                                });

                            #region RMS方案信息查看
                            if (Targets_Selected())
                            {
                                #region 控制区
                                XGUI.layout_group_start(
                                    type: XGUIContainerType.Horizontal,
                                    bg_fill: XGUIGroup.透明,
                                    bg_color: XGUIColor.亮白,
                                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                                    absolute_margin: true,
                                    absolute_padding: true,
                                    margin: new RectOffset(0, 0, 0, 0),
                                    padding: new RectOffset(15, 15, 5, 10));

                                #region 标题按钮
                                if (XGUI.layout_button(
                                    text: $"{selected_objects[element_statu_index].name} ( {selected_objects[element_statu_index].indicator} )",
                                    tooltip: "",
                                    bg_fill: XGUIGroup.透明,
                                    button_text_color: Color.gray,
                                    press_fill: XGUIGroup.透明,
                                    press_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    font_size: XGUIFontSize.M,
                                    anchor: TextAnchor.MiddleLeft,
                                    margin: new RectOffset(0, 0, 0, 0),
                                    padding: new RectOffset(0, 0, 0, 0),
                                    layout_width: 150,
                                    button_text_font: XGUI.GetFont("xg-medium")))
                                {
                                    EditorGUIUtility.PingObject(selected_objects[element_statu_index]);
                                }
                                #endregion

                                XGUI.layout_flexspace();

                                #region 上一个
                                if (XGUI.layout_button(
                                    tooltip: "上一个",
                                    tex_release: left_arrow_r,
                                    tex_press: left_arrow_p,
                                    tex_gui_color: Color.white,
                                    border: new RectOffset(0, 0, 0, 0),
                                    width: 12,
                                    height: 12))
                                {
                                    if (element_statu_index <= 0)
                                    {
                                        element_statu_index = selected_objects.Length - 1;
                                    }
                                    else
                                    {
                                        element_statu_index--;
                                    }
                                    EditorGUIUtility.PingObject(selected_objects[element_statu_index]);
                                }
                                #endregion

                                XGUI.layout_space(20);

                                #region 下一个
                                if (XGUI.layout_button(
                                    tooltip: "下一个",
                                    tex_release: right_arrow_r,
                                    tex_press: right_arrow_p,
                                    tex_gui_color: Color.white,
                                    border: new RectOffset(0, 0, 0, 0),
                                    width: 12,
                                    height: 12))
                                {
                                    if (element_statu_index >= selected_objects.Length - 1)
                                    {
                                        element_statu_index = 0;
                                    }
                                    else
                                    {
                                        element_statu_index++;
                                    }
                                    EditorGUIUtility.PingObject(selected_objects[element_statu_index]);
                                }
                                #endregion

                                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                                #endregion

                                XGUI.layout_seperator(
                                        thickness: 1,
                                        color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                                        margin: new RectOffset(15, 15, 5, 25));

                                if (selected_objects[element_statu_index].rms_layout_datas.Count <= 0)
                                {
                                    XGUI.layout_label(
                                        text: "暂无对应方案的布局设计数据",
                                        size: XGUIFontSize.S,
                                        anchor: TextAnchor.MiddleCenter,
                                        text_color: Color.white * 0.85f,
                                        offset: new Vector2(0, 0),
                                        padding: new RectOffset(10, 10, 10, 10),
                                        margin: new RectOffset(0, 0, 6, 0),
                                        clipping: clipping_ellipsis,
                                        wrap: true,
                                        font: XGUI.GetFont("xg-regular"),
                                        font_style: FontStyle.Normal);
                                }
                                else
                                {
                                    bool exist_lay = false;
                                    for (int s = 0; s < selected_objects[element_statu_index].rms_layout_datas.Count; s++)
                                    {
                                        Element_RMS_LayoutData dat = selected_objects[element_statu_index].rms_layout_datas[s];

                                        if (dat.layout_name == selected_objects[element_statu_index].rms_name)
                                        {
                                            exist_lay = true;

                                            #region 锚点
                                            XGUI.layout_state_displayer_text(
                                                title: "锚点",
                                                title_size: XGUIFontSize.M,
                                                subtitle: dat.anchor.ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion

                                            #region 最小锚点
                                            XGUI.layout_state_displayer_text(
                                                title: "最小锚点",
                                                title_size: XGUIFontSize.M,
                                                subtitle: dat.anchor_min.ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion

                                            #region 最大锚点
                                            XGUI.layout_state_displayer_text(
                                                title: "最大锚点",
                                                title_size: XGUIFontSize.M,
                                                subtitle: dat.anchor_max.ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion

                                            #region 位置
                                            XGUI.layout_state_displayer_text(
                                                title: "位置",
                                                title_size: XGUIFontSize.M,
                                                subtitle: dat.position.ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion

                                            #region 角度
                                            XGUI.layout_state_displayer_text(
                                                title: "角度",
                                                title_size: XGUIFontSize.M,
                                                subtitle: dat.euler.ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion

                                            #region 缩放
                                            XGUI.layout_state_displayer_text(
                                                title: "缩放",
                                                title_size: XGUIFontSize.M,
                                                subtitle: dat.scale.ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion

                                            #region 轴心
                                            XGUI.layout_state_displayer_text(
                                                title: "轴心",
                                                title_size: XGUIFontSize.M,
                                                subtitle: dat.pivot.ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion
                                        }
                                    }

                                    if (!exist_lay)
                                    {
                                        XGUI.layout_label(
                                            text: $"暂无对应 {selected_objects[element_statu_index].rms_name} 方案的布局设计数据",
                                            size: XGUIFontSize.S,
                                            anchor: TextAnchor.MiddleCenter,
                                            text_color: Color.white * 0.85f,
                                            offset: new Vector2(0, 0),
                                            padding: new RectOffset(10, 10, 10, 10),
                                            margin: new RectOffset(0, 0, 6, 0),
                                            clipping: clipping_ellipsis,
                                            wrap: true,
                                            font: XGUI.GetFont("xg-regular"),
                                            font_style: FontStyle.Normal);
                                    }
                                }
                            }
                            else
                            {
                                if (sp_rms_layout_datas.arraySize <= 0)
                                {
                                    XGUI.layout_label(
                                        text: "暂无对应方案的布局设计数据",
                                        size: XGUIFontSize.S,
                                        anchor: TextAnchor.MiddleCenter,
                                        text_color: Color.white * 0.85f,
                                        offset: new Vector2(0, 0),
                                        padding: new RectOffset(10, 10, 10, 10),
                                        margin: new RectOffset(0, 0, 6, 0),
                                        clipping: clipping_ellipsis,
                                        wrap: true,
                                        font: XGUI.GetFont("xg-regular"),
                                        font_style: FontStyle.Normal);
                                }
                                else
                                {
                                    for (int i = 0; i < sp_rms_layout_datas.arraySize; i++)
                                    {
                                        SerializedProperty sp_item = sp_rms_layout_datas.GetArrayElementAtIndex(i);
                                        SerializedProperty sp_item_Name = sp_item.FindPropertyRelative(nameof(Element_RMS_LayoutData.layout_name));

                                        if (sp_item_Name.stringValue == sp_rms_name.stringValue)
                                        {
                                            SerializedProperty sp_item_Anchor = sp_item.FindPropertyRelative(nameof(Element_RMS_LayoutData.anchor));
                                            SerializedProperty sp_item_Position = sp_item.FindPropertyRelative(nameof(Element_RMS_LayoutData.position));
                                            SerializedProperty sp_item_Euler = sp_item.FindPropertyRelative(nameof(Element_RMS_LayoutData.euler));
                                            SerializedProperty sp_item_Scale = sp_item.FindPropertyRelative(nameof(Element_RMS_LayoutData.scale));
                                            SerializedProperty sp_item_AnchorMin = sp_item.FindPropertyRelative(nameof(Element_RMS_LayoutData.anchor_min));
                                            SerializedProperty sp_item_AnchorMax = sp_item.FindPropertyRelative(nameof(Element_RMS_LayoutData.anchor_min));
                                            SerializedProperty sp_item_Pivot = sp_item.FindPropertyRelative(nameof(Element_RMS_LayoutData.pivot));

                                            #region 锚点
                                            XGUI.layout_state_displayer_text(
                                                title: "锚点",
                                                title_size: XGUIFontSize.M,
                                                subtitle: ((XHudAnchor)sp_item_Anchor.enumValueIndex).ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion

                                            #region 最小锚点
                                            XGUI.layout_state_displayer_text(
                                                title: "最小锚点",
                                                title_size: XGUIFontSize.M,
                                                subtitle: sp_item_AnchorMin.vector2Value.ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion

                                            #region 最大锚点
                                            XGUI.layout_state_displayer_text(
                                                title: "最大锚点",
                                                title_size: XGUIFontSize.M,
                                                subtitle: sp_item_AnchorMax.vector2Value.ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion

                                            #region 位置
                                            XGUI.layout_state_displayer_text(
                                                title: "位置",
                                                title_size: XGUIFontSize.M,
                                                subtitle: sp_item_Position.vector3Value.ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion

                                            #region 角度
                                            XGUI.layout_state_displayer_text(
                                                title: "角度",
                                                title_size: XGUIFontSize.M,
                                                subtitle: sp_item_Euler.vector3Value.ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion

                                            #region 缩放
                                            XGUI.layout_state_displayer_text(
                                                title: "缩放",
                                                title_size: XGUIFontSize.M,
                                                subtitle: sp_item_Scale.vector3Value.ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion

                                            #region 轴心
                                            XGUI.layout_state_displayer_text(
                                                title: "轴心",
                                                title_size: XGUIFontSize.M,
                                                subtitle: sp_item_Pivot.vector2Value.ToString(),
                                                subtitle_size: XGUIFontSize.M,
                                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                                margin: new RectOffset(5, 5, 0, 5));
                                            #endregion

                                            #region 控制按钮
                                            XGUI.layout_group_start(
                                                type: XGUIContainerType.Horizontal,
                                                bg_fill: XGUIGroup.透明,
                                                bg_color: XGUIColor.亮白,
                                                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                                                absolute_margin: true,
                                                absolute_padding: true,
                                                margin: new RectOffset(0, 0, 0, 0),
                                                padding: new RectOffset(0, 0, 5, 10));

                                            #region 清空所有方案
                                            if (sp_rms_layout_datas.arraySize > 0)
                                            {
                                                if (XGUI.layout_button(
                                                text: "清空所有方案",
                                                tooltip: "清空所有分辨率匹配方案列表",
                                                bg_fill: XGUIGroup.实体,
                                                bg_color: XGUIColor.亮白,
                                                bg_color_gui: Color.red * 0.75f,
                                                button_text_color: Color.white,
                                                press_fill: XGUIGroup.实体,
                                                press_color: XGUIColor.深空灰,
                                                press_text_color: Color.white,
                                                font_size: XGUIFontSize.M,
                                                anchor: TextAnchor.MiddleCenter,
                                                margin: new RectOffset(0, 5, 0, 0),
                                                padding: new RectOffset(0, 0, 0, 0),
                                                //width: ButtonWidth,
                                                height: 20,
                                                button_text_font: XGUI.GetFont("xg-medium")))
                                                {
                                                    string res = XGUI.dialog(
                                                        type: XGUIDialogType.警告,
                                                        windowtitle: "XHud - 元素消息",
                                                        title: "RMS方案操作",
                                                        msg: $"如果清空匹配分辨率信息列表，会导致您之前为不同分辨率记录的坐标信息全部清空，请谨慎此操作！",
                                                        ok: "清空",
                                                        cancel: "暂不",
                                                        PrimaryIndex: 0,
                                                        usemodal: true,
                                                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                                                    if (res == "清空")
                                                    {
                                                        sp_rms_layout_datas.ClearArray();
                                                        sp_rms_layout_datas.serializedObject.ApplyModifiedProperties();
                                                    }
                                                    return;
                                                }
                                            }
                                            #endregion

                                            //XGUI.layout_flexspace();

                                            #region 移除当前方案
                                            if (RMS_IsExist())
                                            {
                                                if (XGUI.layout_button(
                                                    text: "移除当前方案",
                                                    tooltip: "移除当前选择的分辨率匹配方案",
                                                    bg_fill: XGUIGroup.实体,
                                                    bg_color: XGUIColor.亮白,
                                                    bg_color_gui: Color.white,
                                                    button_text_color: Color.black,
                                                    press_fill: XGUIGroup.实体,
                                                    press_color: XGUIColor.深空灰,
                                                    press_text_color: Color.white,
                                                    font_size: XGUIFontSize.M,
                                                    anchor: TextAnchor.MiddleCenter,
                                                    margin: new RectOffset(5, 0, 0, 0),
                                                    padding: new RectOffset(0, 0, 0, 0),
                                                    //width: ButtonWidth,
                                                    height: 20,
                                                    button_text_font: XGUI.GetFont("xg-medium")))
                                                {
                                                    string res = XGUI.dialog(
                                                        type: XGUIDialogType.警告,
                                                        windowtitle: "XHud - 元素消息",
                                                        title: "RMS方案操作",
                                                        msg: $"此操作会导致您为当前分辨率匹配的方案会被移除，请谨慎此操作！",
                                                        ok: "清空",
                                                        cancel: "暂不",
                                                        PrimaryIndex: 0,
                                                        usemodal: true,
                                                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                                                    if (res == "暂不")
                                                        return;
                                                    for (int s = 0; s < sp_rms_layout_datas.arraySize; s++)
                                                    {
                                                        SerializedProperty sp_Name = sp_rms_layout_datas.GetArrayElementAtIndex(s).FindPropertyRelative(nameof(Element_RMS_LayoutData.layout_name));
                                                        if (sp_Name.stringValue == sp_rms_name.stringValue)
                                                        {
                                                            sp_rms_layout_datas.DeleteArrayElementAtIndex(s);
                                                            sp_rms_layout_datas.serializedObject.ApplyModifiedProperties();
                                                            break;
                                                        }
                                                    }
                                                    return;
                                                }
                                            }
                                            #endregion

                                            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                                            sp_tweens_preivew_in_state.serializedObject.ApplyModifiedProperties();
                                            sp_tweens_preivew_out_state.serializedObject.ApplyModifiedProperties();
                                            #endregion
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            XGUI.layout_label(
                                text: "暂未在管理器中配置 R M S 列表",
                                //bg_fill: XGUIFilled.无,
                                //bg_color: XGUIColor.深空灰,
                                size: XGUIFontSize.S,
                                anchor: TextAnchor.MiddleCenter,
                                text_color: Color.white * 0.85f,
                                offset: new Vector2(0, 0),
                                padding: new RectOffset(10, 10, 10, 10),
                                margin: new RectOffset(0, 0, 6, 0),
                                clipping: clipping_ellipsis,
                                wrap: true,
                                font: XGUI.GetFont("xg-regular"),
                                font_style: FontStyle.Normal);
                        }
                    }
                    else
                    {
                        XGUI.layout_label(
                               text: "暂未发现 XHud 管理器",
                               //bg_fill: XGUIFilled.无,
                               //bg_color: XGUIColor.深空灰,
                               size: XGUIFontSize.S,
                               anchor: TextAnchor.MiddleCenter,
                               text_color: Color.white * 0.85f,
                               offset: new Vector2(0, 0),
                               padding: new RectOffset(10, 10, 10, 10),
                               margin: new RectOffset(0, 0, 6, 0),
                               clipping: clipping_ellipsis,
                               wrap: true,
                               font: XGUI.GetFont("xg-regular"),
                               font_style: FontStyle.Normal);
                    }
                }
                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 子组件
            if (!sp_tweens_preivew_in_state.boolValue && !sp_tweens_preivew_out_state.boolValue)
            {
                base_script.fold_list = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "子组件",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_list);

                if (!base_script.fold_list)
                {
                    #region 列表
                    if (Targets_Selected())
                    {
                        XGUI.layout_label(
                            text: "列表不支持批量操作",
                            size: XGUIFontSize.S,
                            anchor: TextAnchor.MiddleCenter,
                            text_color: Color.white * 0.85f,
                            offset: new Vector2(0, 0),
                            padding: new RectOffset(10, 10, 10, 10),
                            margin: new RectOffset(0, 0, 6, 0),
                            clipping: clipping_ellipsis,
                            wrap: true,
                            font: XGUI.GetFont("xg-regular"),
                            font_style: FontStyle.Normal);
                    }
                    else
                    {
                        if (sp_sounder_nodes.arraySize == 0 && sp_primitive_controller_nodes.arraySize == 0 && sp_button_nodes.arraySize == 0 && sp_text_nodes.arraySize == 0 && sp_tmptext_nodes.arraySize == 0 && sp_option_nodes.arraySize == 0 && sp_slider_nodes.arraySize == 0 && sp_progress_nodes.arraySize == 0 && sp_toggle_nodes.arraySize == 0)
                        {
                            XGUI.layout_label(
                                text: "暂无有效子组件列表",
                                size: XGUIFontSize.S,
                                anchor: TextAnchor.MiddleCenter,
                                text_color: Color.white * 0.85f,
                                offset: new Vector2(0, 0),
                                padding: new RectOffset(10, 10, 10, 10),
                                margin: new RectOffset(0, 0, 6, 0),
                                clipping: clipping_ellipsis,
                                wrap: true,
                                font: XGUI.GetFont("xg-regular"),
                                font_style: FontStyle.Normal);
                        }
                        else
                        {
                            XGUI.layout_space(20);

                            #region 图元控制器
                            if (sp_primitive_controller_nodes.arraySize > 0)
                            {
                                sp_primitives_is_fold.boolValue = XGUI.layout_group_start(
                                    type: XGUIContainerType.Vertical,
                                    bg_fill: XGUIGroup.缺口纯色边框,
                                    bg_color: XGUIColor.亮白,
                                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                                    title: "图元控制器",
                                    title_size: XGUIFontSize.M,
                                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    title_clipping: TextClipping.Clip,
                                    title_offset: new Vector2(10, 0),
                                    padding: new RectOffset(10, 10, 15, 15),
                                    foldout: sp_primitives_is_fold.boolValue);

                                if (!sp_primitives_is_fold.boolValue)
                                    primitives_list.DoLayoutList();

                                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                            }
                            #endregion

                            #region 文字
                            if (sp_text_nodes.arraySize > 0)
                            {
                                sp_text_is_fold.boolValue = XGUI.layout_group_start(
                                    type: XGUIContainerType.Vertical,
                                    bg_fill: XGUIGroup.缺口纯色边框,
                                    bg_color: XGUIColor.亮白,
                                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                                    title: "文字",
                                    title_size: XGUIFontSize.M,
                                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    title_clipping: TextClipping.Clip,
                                    title_offset: new Vector2(10, 0),
                                    padding: new RectOffset(10, 10, 15, 15),
                                    foldout: sp_text_is_fold.boolValue);

                                if (!sp_text_is_fold.boolValue)
                                    text_list.DoLayoutList();

                                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                            }
                            #endregion

                            #region Tmp 文字
                            if (sp_tmptext_nodes.arraySize > 0)
                            {
                                sp_tmptext_is_fold.boolValue = XGUI.layout_group_start(
                                    type: XGUIContainerType.Vertical,
                                    bg_fill: XGUIGroup.缺口纯色边框,
                                    bg_color: XGUIColor.亮白,
                                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                                    title: "Tmp 文字",
                                    title_size: XGUIFontSize.M,
                                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    title_clipping: TextClipping.Clip,
                                    title_offset: new Vector2(10, 0),
                                    padding: new RectOffset(10, 10, 15, 15),
                                    foldout: sp_tmptext_is_fold.boolValue);

                                if (!sp_tmptext_is_fold.boolValue)
                                    tmp_text_list.DoLayoutList();

                                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                            }
                            #endregion

                            #region 按钮
                            if (sp_button_nodes.arraySize > 0)
                            {
                                sp_button_is_fold.boolValue = XGUI.layout_group_start(
                                    type: XGUIContainerType.Vertical,
                                    bg_fill: XGUIGroup.缺口纯色边框,
                                    bg_color: XGUIColor.亮白,
                                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                                    title: "按钮",
                                    title_size: XGUIFontSize.M,
                                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    title_clipping: TextClipping.Clip,
                                    title_offset: new Vector2(10, 0),
                                    padding: new RectOffset(10, 10, 15, 15),
                                    foldout: sp_button_is_fold.boolValue);

                                if (!sp_button_is_fold.boolValue)
                                    button_list.DoLayoutList();

                                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                            }
                            #endregion

                            #region 选项
                            if (sp_option_nodes.arraySize > 0)
                            {
                                sp_option_is_fold.boolValue = XGUI.layout_group_start(
                                    type: XGUIContainerType.Vertical,
                                    bg_fill: XGUIGroup.缺口纯色边框,
                                    bg_color: XGUIColor.亮白,
                                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                                    title: "选项",
                                    title_size: XGUIFontSize.M,
                                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    title_clipping: TextClipping.Clip,
                                    title_offset: new Vector2(10, 0),
                                    padding: new RectOffset(10, 10, 15, 15),
                                    foldout: sp_option_is_fold.boolValue);

                                if (!sp_option_is_fold.boolValue)
                                    option_list.DoLayoutList();

                                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                            }
                            #endregion

                            #region 滑动条
                            if (sp_slider_nodes.arraySize > 0)
                            {
                                sp_slider_is_fold.boolValue = XGUI.layout_group_start(
                                    type: XGUIContainerType.Vertical,
                                    bg_fill: XGUIGroup.缺口纯色边框,
                                    bg_color: XGUIColor.亮白,
                                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                                    title: "滑动条",
                                    title_size: XGUIFontSize.M,
                                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    title_clipping: TextClipping.Clip,
                                    title_offset: new Vector2(10, 0),
                                    padding: new RectOffset(10, 10, 15, 15),
                                    foldout: sp_slider_is_fold.boolValue);

                                if (!sp_slider_is_fold.boolValue)
                                    slider_list.DoLayoutList();

                                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                            }
                            #endregion

                            #region 进度条
                            if (sp_progress_nodes.arraySize > 0)
                            {
                                sp_progress_is_fold.boolValue = XGUI.layout_group_start(
                                    type: XGUIContainerType.Vertical,
                                    bg_fill: XGUIGroup.缺口纯色边框,
                                    bg_color: XGUIColor.亮白,
                                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                                    title: "进度条",
                                    title_size: XGUIFontSize.M,
                                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    title_clipping: TextClipping.Clip,
                                    title_offset: new Vector2(10, 0),
                                    padding: new RectOffset(10, 10, 15, 15),
                                    foldout: sp_progress_is_fold.boolValue);

                                if (!sp_progress_is_fold.boolValue)
                                    progress_list.DoLayoutList();

                                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                            }
                            #endregion

                            #region 开关
                            if (sp_toggle_nodes.arraySize > 0)
                            {
                                sp_toggle_is_fold.boolValue = XGUI.layout_group_start(
                                    type: XGUIContainerType.Vertical,
                                    bg_fill: XGUIGroup.缺口纯色边框,
                                    bg_color: XGUIColor.亮白,
                                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                                    title: "开关",
                                    title_size: XGUIFontSize.M,
                                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    title_clipping: TextClipping.Clip,
                                    title_offset: new Vector2(10, 0),
                                    padding: new RectOffset(10, 10, 15, 15),
                                    foldout: sp_toggle_is_fold.boolValue);

                                if (!sp_toggle_is_fold.boolValue)
                                    toggle_list.DoLayoutList();

                                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                            }
                            #endregion

                            #region 音效器
                            if (sp_sounder_nodes.arraySize > 0)
                            {
                                sp_sounder_is_fold.boolValue = XGUI.layout_group_start(
                                    type: XGUIContainerType.Vertical,
                                    bg_fill: XGUIGroup.缺口纯色边框,
                                    bg_color: XGUIColor.亮白,
                                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                                    title: "音效器",
                                    title_size: XGUIFontSize.M,
                                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    title_clipping: TextClipping.Clip,
                                    title_offset: new Vector2(10, 0),
                                    padding: new RectOffset(10, 10, 15, 15),
                                    foldout: sp_sounder_is_fold.boolValue);

                                if (!sp_sounder_is_fold.boolValue)
                                    sounder_list.DoLayoutList();

                                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                            }
                            #endregion

                        }
                    }
                    #endregion
                }
                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 统计
            if (!sp_tweens_preivew_in_state.boolValue && !sp_tweens_preivew_out_state.boolValue)
            {
                string statu_statistic = "统计";
                if (Targets_Selected())
                    statu_statistic = "统计 - ( 批量模式 )";

                base_script.fold_statistic = XGUI.layout_group_start(
                  type: XGUIContainerType.Vertical,
                  bg_fill: XGUIGroup.缺口纯色边框,
                  bg_color: XGUIColor.亮白,
                  bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                  title: statu_statistic,
                  title_size: XGUIFontSize.M,
                  title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                  title_clipping: TextClipping.Clip,
                  padding: new RectOffset(10, 10, 15, 15),
                  foldout: base_script.fold_statistic);

                if (!base_script.fold_statistic)
                {
                    if (!Targets_Selected())
                    {
                        if (sp_sounder_nodes.arraySize == 0 && sp_primitive_controller_nodes.arraySize == 0 && sp_button_nodes.arraySize == 0 && sp_text_nodes.arraySize == 0 && sp_tmptext_nodes.arraySize == 0 && sp_option_nodes.arraySize == 0 && sp_slider_nodes.arraySize == 0 && sp_progress_nodes.arraySize == 0 && sp_toggle_nodes.arraySize == 0)
                        {
                            XGUI.layout_label(
                                text: "暂无有效子组件列表",
                                size: XGUIFontSize.S,
                                anchor: TextAnchor.MiddleCenter,
                                text_color: Color.white * 0.85f,
                                offset: new Vector2(0, 0),
                                padding: new RectOffset(10, 10, 10, 10),
                                margin: new RectOffset(0, 0, 6, 0),
                                clipping: clipping_ellipsis,
                                wrap: true,
                                font: XGUI.GetFont("xg-regular"),
                                font_style: FontStyle.Normal);
                        }

                        #region 音效器
                        if (sp_sounder_nodes.arraySize > 0)
                            XGUI.layout_state_displayer_text(
                                title: "音效器",
                                title_size: XGUIFontSize.M,
                                subtitle: sp_sounder_nodes.arraySize.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 图元控制器
                        if (sp_primitive_controller_nodes.arraySize > 0)
                            XGUI.layout_state_displayer_text(
                                title: "图元控制器",
                                title_size: XGUIFontSize.M,
                                subtitle: sp_primitive_controller_nodes.arraySize.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 按钮
                        if (sp_button_nodes.arraySize > 0)
                            XGUI.layout_state_displayer_text(
                                title: "按钮",
                                title_size: XGUIFontSize.M,
                                subtitle: sp_button_nodes.arraySize.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 选项
                        if (sp_option_nodes.arraySize > 0)
                            XGUI.layout_state_displayer_text(
                                title: "选项",
                                title_size: XGUIFontSize.M,
                                subtitle: sp_option_nodes.arraySize.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 滑动条
                        if (sp_slider_nodes.arraySize > 0)
                            XGUI.layout_state_displayer_text(
                                title: "滑动条",
                                title_size: XGUIFontSize.M,
                                subtitle: sp_slider_nodes.arraySize.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 进度条
                        if (sp_progress_nodes.arraySize > 0)
                            XGUI.layout_state_displayer_text(
                                title: "进度条",
                                title_size: XGUIFontSize.M,
                                subtitle: sp_progress_nodes.arraySize.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 开关
                        if (sp_toggle_nodes.arraySize > 0)
                            XGUI.layout_state_displayer_text(
                                title: "开关",
                                title_size: XGUIFontSize.M,
                                subtitle: sp_toggle_nodes.arraySize.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 文字
                        if (sp_text_nodes.arraySize > 0)
                            XGUI.layout_state_displayer_text(
                                title: "文字",
                                title_size: XGUIFontSize.M,
                                subtitle: sp_text_nodes.arraySize.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region Tmp文字
                        if (sp_tmptext_nodes.arraySize > 0)
                            XGUI.layout_state_displayer_text(
                                title: "Tmp文字",
                                title_size: XGUIFontSize.M,
                                subtitle: sp_tmptext_nodes.arraySize.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion
                    }
                    else
                    {
                        #region 批量控件
                        #region 控制区
                        XGUI.layout_group_start(
                            type: XGUIContainerType.Horizontal,
                            bg_fill: XGUIGroup.透明,
                            bg_color: XGUIColor.亮白,
                            bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                            absolute_margin: true,
                            absolute_padding: true,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(15, 15, 5, 10));

                        #region 标题按钮
                        if (XGUI.layout_button(
                            text: $"{selected_objects[element_statu_index].name} ( {selected_objects[element_statu_index].indicator} )",
                            tooltip: "",
                            bg_fill: XGUIGroup.透明,
                            button_text_color: Color.gray,
                            press_fill: XGUIGroup.透明,
                            press_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            font_size: XGUIFontSize.M,
                            anchor: TextAnchor.MiddleLeft,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0),
                            layout_width: 150,
                            button_text_font: XGUI.GetFont("xg-medium")))
                        {
                            EditorGUIUtility.PingObject(selected_objects[element_statu_index]);
                        }
                        #endregion

                        XGUI.layout_flexspace();

                        #region 上一个
                        if (XGUI.layout_button(
                            tooltip: "上一个",
                            tex_release: left_arrow_r,
                            tex_press: left_arrow_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: 12,
                            height: 12))
                        {
                            if (element_statu_index <= 0)
                            {
                                element_statu_index = selected_objects.Length - 1;
                            }
                            else
                            {
                                element_statu_index--;
                            }
                            EditorGUIUtility.PingObject(selected_objects[element_statu_index]);
                        }
                        #endregion

                        XGUI.layout_space(20);

                        #region 下一个
                        if (XGUI.layout_button(
                            tooltip: "下一个",
                            tex_release: right_arrow_r,
                            tex_press: right_arrow_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: 12,
                            height: 12))
                        {
                            if (element_statu_index >= selected_objects.Length - 1)
                            {
                                element_statu_index = 0;
                            }
                            else
                            {
                                element_statu_index++;
                            }
                            EditorGUIUtility.PingObject(selected_objects[element_statu_index]);
                        }
                        #endregion

                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                        #endregion

                        XGUI.layout_seperator(
                                thickness: 1,
                                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                                margin: new RectOffset(15, 15, 5, 25));

                        XHud_Module_Element ele = selected_objects[element_statu_index];

                        if (ele.sounder_nodes.Count == 0 && ele.primitive_controller_nodes.Count == 0 && ele.button_nodes.Count == 0 && ele.text_nodes.Count == 0 && ele.tmptext_nodes.Count == 0 && ele.option_nodes.Count == 0 && ele.slider_nodes.Count == 0 && ele.progress_nodes.Count == 0 && ele.toggle_nodes.Count == 0)
                        {
                            XGUI.layout_label(
                                text: "暂无有效子组件列表",
                                size: XGUIFontSize.S,
                                anchor: TextAnchor.MiddleCenter,
                                text_color: Color.white * 0.85f,
                                offset: new Vector2(0, 0),
                                padding: new RectOffset(10, 10, 10, 10),
                                margin: new RectOffset(0, 0, 6, 0),
                                clipping: clipping_ellipsis,
                                wrap: true,
                                font: XGUI.GetFont("xg-regular"),
                                font_style: FontStyle.Normal);
                        }

                        #region 音效器
                        if (ele.sounder_nodes.Count > 0)
                            XGUI.layout_state_displayer_text(
                                title: "音效器",
                                title_size: XGUIFontSize.M,
                                subtitle: ele.sounder_nodes.Count.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 图元控制器
                        if (ele.primitive_controller_nodes.Count > 0)
                            XGUI.layout_state_displayer_text(
                                title: "图元控制器",
                                title_size: XGUIFontSize.M,
                                subtitle: ele.primitive_controller_nodes.Count.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 按钮
                        if (ele.button_nodes.Count > 0)
                            XGUI.layout_state_displayer_text(
                                title: "按钮",
                                title_size: XGUIFontSize.M,
                                subtitle: ele.button_nodes.Count.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 选项
                        if (ele.option_nodes.Count > 0)
                            XGUI.layout_state_displayer_text(
                                title: "选项",
                                title_size: XGUIFontSize.M,
                                subtitle: ele.option_nodes.Count.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 滑动条
                        if (ele.slider_nodes.Count > 0)
                            XGUI.layout_state_displayer_text(
                                title: "滑动条",
                                title_size: XGUIFontSize.M,
                                subtitle: ele.slider_nodes.Count.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 进度条
                        if (ele.progress_nodes.Count > 0)
                            XGUI.layout_state_displayer_text(
                                title: "进度条",
                                title_size: XGUIFontSize.M,
                                subtitle: ele.progress_nodes.Count.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 开关
                        if (ele.toggle_nodes.Count > 0)
                            XGUI.layout_state_displayer_text(
                                title: "开关",
                                title_size: XGUIFontSize.M,
                                subtitle: ele.toggle_nodes.Count.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 文字
                        if (ele.text_nodes.Count > 0)
                            XGUI.layout_state_displayer_text(
                                title: "文字",
                                title_size: XGUIFontSize.M,
                                subtitle: ele.text_nodes.Count.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region Tmp文字
                        if (ele.tmptext_nodes.Count > 0)
                            XGUI.layout_state_displayer_text(
                                title: "Tmp文字",
                                title_size: XGUIFontSize.M,
                                subtitle: ele.tmptext_nodes.Count.ToString() + " 个",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                        #endregion
                        #endregion
                    }
                }
                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 组件
            if (!sp_tweens_preivew_in_state.boolValue && !sp_tweens_preivew_out_state.boolValue)
            {
                base_script.fold_component = XGUI.layout_group_start(
type: XGUIContainerType.Vertical,
bg_fill: XGUIGroup.缺口纯色边框,
bg_color: XGUIColor.亮白,
bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
title: "组件",
title_size: XGUIFontSize.M,
title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
title_clipping: TextClipping.Clip,
padding: new RectOffset(10, 10, 15, 15),
foldout: base_script.fold_component);

                if (!base_script.fold_component)
                {
                    #region 基础图像组件
                    XGUI.layout_property_field(
                        title: "基础图像组件",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        status_icon: "icon_field_status",
                        status_icon_color: sp_canvasgroup.objectReferenceValue == null ? Color.gray : XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        prop: sp_canvasgroup,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 变换组件
                    XGUI.layout_property_field(
                        title: "变换组件",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        status_icon: "icon_field_status",
                        status_icon_color: sp_rect_transform.objectReferenceValue == null ? Color.gray : XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        prop: sp_rect_transform,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 触发器
                    XGUI.layout_property_field(
                        title: "触发器",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        status_icon: "icon_field_status",
                        status_icon_color: sp_trigger_action.objectReferenceValue == null ? Color.gray : XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        prop: sp_trigger_action,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 追踪器
                    XGUI.layout_property_field(
                        title: "追踪器",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        status_icon: "icon_field_status",
                        status_icon_color: sp_object_tracker.objectReferenceValue == null ? Color.gray : XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        prop: sp_object_tracker,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion
                }
                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 检测事件
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("组件"));
                menu.AddItem(new GUIContent("X (扫描)"), false, () =>
                {
                    if (Application.isPlaying)
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素消息",
                            title: "扫描元素",
                            msg: $"程序正在运行，无法在运行期间执行此功能！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                        return;
                    }
                    GetPrimitivesTween();
                    GetAllText();
                    GetAllTmpText();
                    GetAllSounder();
                    GetAllButtons();
                    GetAllOptions();
                    GetAllSliders();
                    GetAllProgress();
                    GetAllToggle();
                    GetModulesResult();
                    return;
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("元素RMS布局信息"));
                if (sp_rms_enabled.boolValue)
                {
                    menu.AddItem(new GUIContent("D (记录)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 元素消息",
                                title: "记录RMS方案信息",
                                msg: $"程序正在运行，无法在运行期间执行此功能！",
                                ok: "明白",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                            return;
                        }

                        RMS_Record();
                    });
                    menu.AddItem(new GUIContent("C (清空)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 元素消息",
                                title: "清空RMS方案信息",
                                msg: $"程序正在运行，无法在运行期间执行此功能！",
                                ok: "明白",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                            return;
                        }

                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素消息",
                            title: "RMS方案操作",
                            msg: $"如果清空匹配分辨率信息列表，会导致您之前为不同分辨率记录的坐标信息全部清空，请谨慎此操作！",
                            ok: "清空",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        if (res == "清空")
                        {
                            sp_rms_layout_datas.ClearArray();
                            sp_rms_layout_datas.serializedObject.ApplyModifiedProperties();
                        }
                        return;
                    });
                }
                menu.AddDisabledItem(new GUIContent("坐标"));
                menu.AddItem(new GUIContent("R (回到锚点)"), false, () =>
                {
                    if (Application.isPlaying)
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素消息",
                            title: "锚点初始化",
                            msg: $"程序正在运行，无法在运行期间执行此功能！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                        return;
                    }
                    RestoreToAnchorPosition();
                });
                menu.AddDisabledItem(new GUIContent("动效快速操作"));
                menu.AddItem(new GUIContent("E (复制预览动效)"), false, () =>
                {
                    string res = XGUI.dialog(
                            type: XGUIDialogType.修改,
                            windowtitle: "XHud - 元素消息",
                            title: "复制预览动效",
                            msg: $"请选择动效参数复制模式！",
                            ok: "取消",
                            cancel: "生成",
                            alt: "回收",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    if (res == "取消")
                        return;

                    MotionNode_Movement M = null;
                    MotionNode_Rotation R = null;
                    MotionNode_Alpha A = null;
                    string json = "";

                    switch (res)
                    {
                        case "生成"://生成
                            Motion_Creator crc = new Motion_Creator();
                            crc.anchor = (XHudAnchor)sp_motion_create_args.FindPropertyRelative("anchor").enumValueIndex;

                            M = new MotionNode_Movement();
                            M.Movement = (HudMotion_Movement)sp_motion_create_args.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            M.Distance = sp_motion_create_args.FindPropertyRelative("Movement.Distance").floatValue;
                            M.Duration = sp_motion_create_args.FindPropertyRelative("Movement.Duration").floatValue;
                            M.Delay = sp_motion_create_args.FindPropertyRelative("Movement.Delay").floatValue;
                            M.Ease = (EaseMode)sp_motion_create_args.FindPropertyRelative("Movement.Ease").enumValueIndex;
                            M.Curve = sp_motion_create_args.FindPropertyRelative("Movement.Curve").animationCurveValue;
                            M.CurveName = sp_motion_create_args.FindPropertyRelative("Movement.CurveName").stringValue;

                            R = new MotionNode_Rotation();
                            R.Rotation = (HudMotion_Rotation)sp_motion_create_args.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            R.Degree = sp_motion_create_args.FindPropertyRelative("Rotation.Degree").floatValue;
                            R.Duration = sp_motion_create_args.FindPropertyRelative("Rotation.Duration").floatValue;
                            R.Delay = sp_motion_create_args.FindPropertyRelative("Rotation.Delay").floatValue;
                            R.Ease = (EaseMode)sp_motion_create_args.FindPropertyRelative("Rotation.Ease").enumValueIndex;
                            R.Curve = sp_motion_create_args.FindPropertyRelative("Rotation.Curve").animationCurveValue;
                            R.CurveName = sp_motion_create_args.FindPropertyRelative("Rotation.CurveName").stringValue;

                            A = new MotionNode_Alpha();
                            A.Duration = sp_motion_create_args.FindPropertyRelative("Alpha.Duration").floatValue;
                            A.Curve = sp_motion_create_args.FindPropertyRelative("Alpha.Curve").animationCurveValue;
                            A.CurveName = sp_motion_create_args.FindPropertyRelative("Alpha.CurveName").stringValue;
                            A.Ease = (EaseMode)sp_motion_create_args.FindPropertyRelative("Alpha.Ease").enumValueIndex;
                            A.Delay = sp_motion_create_args.FindPropertyRelative("Alpha.Delay").floatValue;

                            crc.Movement = M;
                            crc.Rotation = R;
                            crc.Alpha = A;

                            json = JsonUtility.ToJson(crc);
                            XGUI.x_Editor_Data_Set_With_String("xData_MotionArgs", json);
                            break;
                        case "回收"://回收
                            Motion_Recycler rec = new Motion_Recycler();

                            M = new MotionNode_Movement();
                            M.Movement = (HudMotion_Movement)sp_motion_recycle_args.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            M.Distance = sp_motion_recycle_args.FindPropertyRelative("Movement.Distance").floatValue;
                            M.Duration = sp_motion_recycle_args.FindPropertyRelative("Movement.Duration").floatValue;
                            M.Delay = sp_motion_recycle_args.FindPropertyRelative("Movement.Delay").floatValue;
                            M.Ease = (EaseMode)sp_motion_recycle_args.FindPropertyRelative("Movement.Ease").enumValueIndex;
                            M.Curve = sp_motion_recycle_args.FindPropertyRelative("Movement.Curve").animationCurveValue;
                            M.CurveName = sp_motion_recycle_args.FindPropertyRelative("Movement.CurveName").stringValue;

                            R = new MotionNode_Rotation();
                            R.Rotation = (HudMotion_Rotation)sp_motion_recycle_args.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            R.Degree = sp_motion_recycle_args.FindPropertyRelative("Rotation.Degree").floatValue;
                            R.Duration = sp_motion_recycle_args.FindPropertyRelative("Rotation.Duration").floatValue;
                            R.Delay = sp_motion_recycle_args.FindPropertyRelative("Rotation.Delay").floatValue;
                            R.Ease = (EaseMode)sp_motion_recycle_args.FindPropertyRelative("Rotation.Ease").enumValueIndex;
                            R.Curve = sp_motion_recycle_args.FindPropertyRelative("Rotation.Curve").animationCurveValue;
                            R.CurveName = sp_motion_recycle_args.FindPropertyRelative("Rotation.CurveName").stringValue;

                            A = new MotionNode_Alpha();
                            A.Duration = sp_motion_recycle_args.FindPropertyRelative("Alpha.Duration").floatValue;
                            A.Curve = sp_motion_recycle_args.FindPropertyRelative("Alpha.Curve").animationCurveValue;
                            A.CurveName = sp_motion_recycle_args.FindPropertyRelative("Alpha.CurveName").stringValue;
                            A.Ease = (EaseMode)sp_motion_recycle_args.FindPropertyRelative("Alpha.Ease").enumValueIndex;
                            A.Delay = sp_motion_recycle_args.FindPropertyRelative("Alpha.Delay").floatValue;

                            rec.Movement = M;
                            rec.Rotation = R;
                            rec.Alpha = A;

                            json = JsonUtility.ToJson(rec);
                            XGUI.x_Editor_Data_Set_With_String("xData_MotionArgs", json);
                            break;
                    }

                    string mode = "";

                    if (res == "生成")
                        mode = "生成动效参数";
                    else if (res == "回收")
                        mode = "回收动效参数";

                    XGUI.dialog(
                           type: XGUIDialogType.修改,
                           windowtitle: "XHud - 元素消息",
                           title: "复制预览动效",
                           msg: $"已复制 \" {mode} \" 参数到 xData！",
                           ok: "明白",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                });
                menu.AddItem(new GUIContent("R (粘贴预览动效)"), false, () =>
                {
                    string buffer = XGUI.x_Editor_Data_Get_With_String("xData_MotionArgs");
                    if (buffer.Contains("anchor"))//粘贴生成参数
                    {
                        string res = XGUI.dialog(
                           type: XGUIDialogType.修改,
                           windowtitle: "XHud - 元素消息",
                           title: "粘贴预览动效",
                           msg: $"检测到动效参数类型为： \"生成动效\"，确定要使用这个参数吗？",
                           ok: "确定",
                           cancel: "暂不",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        if (res == "暂不")
                            return;

                        Motion_Creator crc = JsonUtility.FromJson<Motion_Creator>(buffer);

                        sp_motion_create_args.FindPropertyRelative("anchor").enumValueIndex = (int)crc.anchor;

                        sp_motion_create_args.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)crc.Movement.Movement;
                        sp_motion_create_args.FindPropertyRelative("Movement.Distance").floatValue = crc.Movement.Distance;
                        sp_motion_create_args.FindPropertyRelative("Movement.Duration").floatValue = crc.Movement.Duration;
                        sp_motion_create_args.FindPropertyRelative("Movement.Delay").floatValue = crc.Movement.Delay;
                        sp_motion_create_args.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)crc.Movement.Ease;
                        sp_motion_create_args.FindPropertyRelative("Movement.Curve").animationCurveValue = crc.Movement.Curve;
                        sp_motion_create_args.FindPropertyRelative("Movement.CurveName").stringValue = crc.Movement.CurveName;

                        sp_motion_create_args.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)crc.Rotation.Rotation;
                        sp_motion_create_args.FindPropertyRelative("Rotation.Degree").floatValue = crc.Rotation.Degree;
                        sp_motion_create_args.FindPropertyRelative("Rotation.Duration").floatValue = crc.Rotation.Duration;
                        sp_motion_create_args.FindPropertyRelative("Rotation.Delay").floatValue = crc.Rotation.Delay;
                        sp_motion_create_args.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)crc.Rotation.Ease;
                        sp_motion_create_args.FindPropertyRelative("Rotation.Curve").animationCurveValue = crc.Rotation.Curve;
                        sp_motion_create_args.FindPropertyRelative("Rotation.CurveName").stringValue = crc.Rotation.CurveName;

                        sp_motion_create_args.FindPropertyRelative("Alpha.Duration").floatValue = crc.Alpha.Duration;
                        sp_motion_create_args.FindPropertyRelative("Alpha.Curve").animationCurveValue = crc.Alpha.Curve;
                        sp_motion_create_args.FindPropertyRelative("Alpha.CurveName").stringValue = crc.Alpha.CurveName;
                        sp_motion_create_args.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)crc.Alpha.Ease;
                        sp_motion_create_args.FindPropertyRelative("Alpha.Delay").floatValue = crc.Alpha.Delay;

                        sp_motion_create_args.serializedObject.ApplyModifiedProperties();

                        XGUI.dialog(
                           type: XGUIDialogType.确认,
                           windowtitle: "XHud - 元素消息",
                           title: "粘贴预览动效",
                           msg: $"已更新 \"生成\" 动效参数！",
                           ok: "明白",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                    else//粘贴回收参数
                    {
                        string res = XGUI.dialog(
                           type: XGUIDialogType.修改,
                           windowtitle: "XHud - 元素消息",
                           title: "粘贴预览动效",
                           msg: $"检测到动效参数类型为： \"回收动效\"，确定要使用这个参数吗？",
                           ok: "确定",
                           cancel: "暂不",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                        if (res == "暂不")
                            return;

                        Motion_Recycler rec = JsonUtility.FromJson<Motion_Recycler>(buffer);

                        sp_motion_recycle_args.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)rec.Movement.Movement;
                        sp_motion_recycle_args.FindPropertyRelative("Movement.Distance").floatValue = rec.Movement.Distance;
                        sp_motion_recycle_args.FindPropertyRelative("Movement.Duration").floatValue = rec.Movement.Duration;
                        sp_motion_recycle_args.FindPropertyRelative("Movement.Delay").floatValue = rec.Movement.Delay;
                        sp_motion_recycle_args.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)rec.Movement.Ease;
                        sp_motion_recycle_args.FindPropertyRelative("Movement.Curve").animationCurveValue = rec.Movement.Curve;
                        sp_motion_recycle_args.FindPropertyRelative("Movement.CurveName").stringValue = rec.Movement.CurveName;

                        sp_motion_recycle_args.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)rec.Rotation.Rotation;
                        sp_motion_recycle_args.FindPropertyRelative("Rotation.Degree").floatValue = rec.Rotation.Degree;
                        sp_motion_recycle_args.FindPropertyRelative("Rotation.Duration").floatValue = rec.Rotation.Duration;
                        sp_motion_recycle_args.FindPropertyRelative("Rotation.Delay").floatValue = rec.Rotation.Delay;
                        sp_motion_recycle_args.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)rec.Rotation.Ease;
                        sp_motion_recycle_args.FindPropertyRelative("Rotation.Curve").animationCurveValue = rec.Rotation.Curve;
                        sp_motion_recycle_args.FindPropertyRelative("Rotation.CurveName").stringValue = rec.Rotation.CurveName;

                        sp_motion_recycle_args.FindPropertyRelative("Alpha.Duration").floatValue = rec.Alpha.Duration;
                        sp_motion_recycle_args.FindPropertyRelative("Alpha.Curve").animationCurveValue = rec.Alpha.Curve;
                        sp_motion_recycle_args.FindPropertyRelative("Alpha.CurveName").stringValue = rec.Alpha.CurveName;
                        sp_motion_recycle_args.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)rec.Alpha.Ease;
                        sp_motion_recycle_args.FindPropertyRelative("Alpha.Delay").floatValue = rec.Alpha.Delay;

                        sp_motion_recycle_args.serializedObject.ApplyModifiedProperties();

                        XGUI.dialog(
                           type: XGUIDialogType.确认,
                           windowtitle: "XHud - 元素消息",
                           title: "粘贴预览动效",
                           msg: $"已更新 \"回收\" 动效参数！",
                           ok: "明白",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("F (折叠组件列表)"), false, () =>
                {
                    AllListFoldState(true);
                });
                menu.AddItem(new GUIContent("V (展开组件列表)"), false, () =>
                {
                    AllListFoldState(false);
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Q (折叠编组)"), false, () =>
                {
                    base_script.GroupFold(true);
                });
                menu.AddItem(new GUIContent("W (展开编组)"), false, () =>
                {
                    base_script.GroupFold(false);
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("生成预览"));
                if (!sp_tweens_preivew_out_state.boolValue)
                {
                    if (!sp_tweens_preivew_in_state.boolValue)
                    {
                        menu.AddItem(new GUIContent("S (生成预览 - 开始)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "预览动画",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                                return;
                            }
                            Preview_ElementTween_In_Play();
                        });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("S (生成预览 - 停止)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "停止预览",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                                return;
                            }
                            Preview_ElementTween_In_Stop();
                        });
                    }
                }

                if (!sp_tweens_preivew_in_state.boolValue)
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("回收预览"));
                    if (!sp_tweens_preivew_out_state.boolValue)
                    {
                        menu.AddItem(new GUIContent("D (回收预览 - 开始)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "预览动画",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                                return;
                            }
                            Preview_ElementTween_Out_Play();
                        });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("D (回收预览 - 停止)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "停止预览",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                                return;
                            }
                            Preview_ElementTween_Out_Stop();
                        });
                    }
                }

                if (PrefabUtility.IsAnyPrefabInstanceRoot(base_script.gameObject))
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("预制体"));
                    menu.AddItem(new GUIContent("A (应用)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "预制体应用",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                            return;
                        }
                        if (Targets_Selected())
                        {
                            for (int i = 0; i < selected_objects.Length; i++)
                            {
                                XHud_Module_Element ele = selected_objects[i];
                                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(ele.gameObject);
                                PrefabUtility.SaveAsPrefabAssetAndConnect(ele.gameObject, path, InteractionMode.AutomatedAction);
                            }
                        }
                        else
                        {
                            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(base_script.gameObject);
                            PrefabUtility.SaveAsPrefabAssetAndConnect(base_script.gameObject, path, InteractionMode.AutomatedAction);
                        }
                    });
                    menu.AddItem(new GUIContent("W (恢复)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "预制体恢复",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                            return;
                        }
                        if (Targets_Selected())
                        {
                            for (int i = 0; i < selected_objects.Length; i++)
                            {
                                XHud_Module_Element ele = selected_objects[i];
                                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(ele.gameObject);
                                PrefabUtility.RevertPrefabInstance(ele.gameObject, InteractionMode.AutomatedAction);
                            }
                        }
                        else
                        {
                            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(base_script.gameObject);
                            PrefabUtility.RevertPrefabInstance(base_script.gameObject, InteractionMode.AutomatedAction);
                        }
                    });
                    menu.AddItem(new GUIContent("Q (定位)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "预制体定位",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                            return;
                        }
                        if (Targets_Selected())
                        {
                            for (int i = 0; i < selected_objects.Length; i++)
                            {
                                XHud_Module_Element ele = selected_objects[i];
                                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(ele.gameObject);
                                Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                                EditorGUIUtility.PingObject(obj);
                            }
                        }
                        else
                        {
                            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(base_script.gameObject);
                            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                            EditorGUIUtility.PingObject(obj);
                        }
                    });
                }
                menu.ShowAsContext(); // 在鼠标位置显示右键菜单
            }
            #endregion

            CheckModulesValid();

            #region 源脚本
            if (!sp_tweens_preivew_in_state.boolValue && !sp_tweens_preivew_out_state.boolValue)
            {
                base_script.fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_based);

                if (!base_script.fold_based)
                {
                    DrawDefaultInspector();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 预览时警告面板
            if (sp_tweens_preivew_in_state.boolValue || sp_tweens_preivew_out_state.boolValue)
            {
                XGUI.layout_group_start(
                   type: XGUIContainerType.Vertical,
                   bg_fill: XGUIGroup.缺口纯色边框,
                   bg_color: XGUIColor.亮白,
                   bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                   title: "预览状态",
                   title_size: XGUIFontSize.M,
                   title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                   title_clipping: TextClipping.Clip,
                   padding: new RectOffset(10, 10, 15, 15));

                XGUI.layout_label(
                  text: "正在预览动画 >>>",
                  size: XGUIFontSize.S,
                  anchor: TextAnchor.MiddleCenter,
                  text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                  offset: new Vector2(0, 0),
                  padding: new RectOffset(0, 0, 0, 0),
                  margin: new RectOffset(0, 0, 6, 0),
                  clipping: TextClipping.Clip);

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            serializedObject.ApplyModifiedProperties();

            if (!sp_tweens_preivew_in_state.boolValue && !sp_tweens_preivew_out_state.boolValue)
                PrimitiveTweens_MaxDuration_Calculate();

        }
        #endregion

        #region 工具 - 组件缓存
        /// <summary>
        /// 缓存元素自身的基础组件引用。
        /// <para/>
        /// 覆盖：
        /// <list type="bullet">
        /// <item><description>RectTransform；</description></item>
        /// <item><description>CanvasGroup；</description></item>
        /// <item><description>Highlighter（Image）；</description></item>
        /// <item><description>TriggerAction；</description></item>
        /// <item><description>ObjectTracker。</description></item>
        /// </list>
        /// <para/>
        /// 仅在引用为空时填充，避免覆盖用户手动指定的引用。
        /// </summary>
        private void GetComponents()
        {
            if (sp_rect_transform.objectReferenceValue == null)
            {
                sp_rect_transform.objectReferenceValue = base_script.GetComponent<RectTransform>();
                sp_rect_transform.serializedObject.ApplyModifiedProperties();
            }
            if (sp_canvasgroup.objectReferenceValue == null)
            {
                sp_canvasgroup.objectReferenceValue = base_script.GetComponent<CanvasGroup>();
                sp_canvasgroup.serializedObject.ApplyModifiedProperties();
            }
            if (sp_highlighter.objectReferenceValue == null)
            {
                sp_highlighter.objectReferenceValue = base_script.GetComponent<Image>();
                sp_highlighter.serializedObject.ApplyModifiedProperties();
            }

            if (base_script.highlighter != null)
            {
                base_script.highlighter.type = Image.Type.Sliced;
                base_script.highlighter.color = new Color(1, 1, 1, 0);
            }

            if (base_script.highlighter.sprite == null)
            {
                base_script.highlighter.type = Image.Type.Sliced;
                base_script.highlighter.sprite = (Sprite)AssetDatabase.LoadAssetAtPath($"{XHud_Dashboard.Get_Path_XHUD_SPRITES_Path()}Others/Rect_SolidBlur.png", typeof(Sprite));
            }

            if (sp_trigger_action.objectReferenceValue == null)
            {
                sp_trigger_action.objectReferenceValue = base_script.GetComponent<XHud_Element_TriggerAction>();
                sp_trigger_action.serializedObject.ApplyModifiedProperties();
            }
            if (sp_object_tracker.objectReferenceValue == null)
            {
                sp_object_tracker.objectReferenceValue = base_script.GetComponent<XHud_ObjectTracker>();
                sp_object_tracker.serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion

        #region 工具 - 模块校验
        /// <summary>
        /// 校验所有子组件列表，清理已失效（引用为 null）的节点条目。
        /// <para/>
        /// 覆盖：
        /// <list type="bullet">
        /// <item><description>Button；</description></item>
        /// <item><description>Toggle；</description></item>
        /// <item><description>Progress；</description></item>
        /// <item><description>Slider；</description></item>
        /// <item><description>Option；</description></item>
        /// <item><description>Sounder；</description></item>
        /// <item><description>PrimitiveController。</description></item>
        /// </list>
        /// </summary>
        private void CheckModulesValid()
        {
            #region Button
            for (int i = 0; i < sp_button_nodes.arraySize; i++)
            {
                SerializedProperty sp_root = sp_button_nodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative(nameof(ElementNode_Button.button));
                    XHud_Module_Button con = (XHud_Module_Button)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        sp_button_nodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion

            #region Toggle
            for (int i = 0; i < sp_toggle_nodes.arraySize; i++)
            {
                SerializedProperty sp_root = sp_toggle_nodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative(nameof(ElementNode_Toggle.toggle));
                    XHud_Module_Toggle con = (XHud_Module_Toggle)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        sp_toggle_nodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion

            #region Progress
            for (int i = 0; i < sp_progress_nodes.arraySize; i++)
            {
                SerializedProperty sp_root = sp_progress_nodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative(nameof(ElementNode_Progress.progress));
                    XHud_Module_Progress con = (XHud_Module_Progress)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        sp_progress_nodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion

            #region Slider
            for (int i = 0; i < sp_slider_nodes.arraySize; i++)
            {
                SerializedProperty sp_root = sp_slider_nodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative(nameof(ElementNode_Slider.slider));
                    XHud_Module_Slider con = (XHud_Module_Slider)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        sp_slider_nodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion

            #region Option
            for (int i = 0; i < sp_option_nodes.arraySize; i++)
            {
                SerializedProperty sp_root = sp_option_nodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative(nameof(ElementNode_Option.option));
                    XHud_Module_Option con = (XHud_Module_Option)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        sp_option_nodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion

            #region Sounder
            for (int i = 0; i < sp_sounder_nodes.arraySize; i++)
            {
                SerializedProperty sp_root = sp_sounder_nodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative(nameof(ElementNode_Sound.sounder));
                    XHud_Element_Sounder con = (XHud_Element_Sounder)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        sp_sounder_nodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion

            #region PrimitiveControllers
            for (int i = 0; i < sp_primitive_controller_nodes.arraySize; i++)
            {
                SerializedProperty sp_node = sp_primitive_controller_nodes.GetArrayElementAtIndex(i);
                if (sp_node != null)
                {
                    SerializedProperty sp_node_con = sp_node.FindPropertyRelative(nameof(PrimitiveControllerNode.controller));
                    XHud_Module_Primitive_Controller sp_con = (XHud_Module_Primitive_Controller)sp_node_con.objectReferenceValue;
                    if (sp_con == null)
                    {
                        sp_primitive_controller_nodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// 扫描完成后弹窗显示所有收集到的子组件列表。
        /// <para/>
        /// 多选时逐个元素分别展示，单选时只展示当前元素。
        /// </summary>
        private void GetModulesResult()
        {
            List<XGUIDialogListDatas> Datas = new List<XGUIDialogListDatas>();
            if (Targets_Selected())
            {
                for (int s = 0; s < selected_objects.Length; s++)
                {
                    bool existcomp = false;

                    XHud_Module_Element ele = selected_objects[s];
                    //-扫描 - 图元控制器 
                    for (int i = 0; i < ele.primitive_controller_nodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{selected_objects[s].name} ( {selected_objects[s].indicator} )";
                        dataitem.SubTitle = "已收集 / 图元控制器";
                        dataitem.Message = $"{ele.primitive_controller_nodes[i].controller.name} ( {ele.primitive_controller_nodes[i].controller.indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 按钮
                    for (int i = 0; i < ele.button_nodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{selected_objects[s].name} ( {selected_objects[s].indicator} )";
                        dataitem.SubTitle = "已收集 / 按钮";
                        dataitem.Message = $"{ele.button_nodes[i].button.name} ( {ele.button_nodes[i].button.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 选项器
                    for (int i = 0; i < ele.option_nodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{selected_objects[s].name} ( {selected_objects[s].indicator} )";
                        dataitem.SubTitle = "已收集 / 选项";
                        dataitem.Message = $"{ele.option_nodes[i].option.name} ( {ele.option_nodes[i].option.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 滑动条
                    for (int i = 0; i < ele.slider_nodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{selected_objects[s].name} ( {selected_objects[s].indicator} )";
                        dataitem.SubTitle = "已收集 / 滑动条";
                        dataitem.Message = $"{ele.slider_nodes[i].slider.name} ( {ele.slider_nodes[i].slider.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 进度条
                    for (int i = 0; i < ele.progress_nodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{selected_objects[s].name} ( {selected_objects[s].indicator} )";
                        dataitem.SubTitle = "已收集 / 进度条";
                        dataitem.Message = $"{ele.progress_nodes[i].progress.name} ( {ele.progress_nodes[i].progress.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 开关
                    for (int i = 0; i < ele.toggle_nodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{selected_objects[s].name} ( {selected_objects[s].indicator} )";
                        dataitem.SubTitle = "已收集 / 开关";
                        dataitem.Message = $"{ele.toggle_nodes[i].toggle.name} ( {ele.toggle_nodes[i].toggle.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 声音
                    for (int i = 0; i < ele.sounder_nodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{selected_objects[s].name} ( {selected_objects[s].indicator} )";
                        dataitem.SubTitle = "已收集 / 音效器";
                        dataitem.Message = $"{ele.sounder_nodes[i].sounder.name} ( {ele.sounder_nodes[i].sounder.indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - Text文字
                    for (int i = 0; i < ele.text_nodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{selected_objects[s].name} ( {selected_objects[s].indicator} )";
                        dataitem.SubTitle = "已收集 / Text文字";
                        dataitem.Message = $"{ele.text_nodes[i].text.name} ( {ele.text_nodes[i].text.indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - Tmp文字
                    for (int i = 0; i < ele.tmptext_nodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{selected_objects[s].name} ( {selected_objects[s].indicator} )";
                        dataitem.SubTitle = "已收集 / Text文字";
                        dataitem.Message = $"{ele.tmptext_nodes[i].tmptext.name} ( {ele.tmptext_nodes[i].tmptext.indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }

                    if (!existcomp)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{selected_objects[s].name} ( {selected_objects[s].indicator} )";
                        dataitem.SubTitle = "";
                        dataitem.Message = "未发现任何存在的子组件";
                        Datas.Add(dataitem);
                    }
                }

                XGUI.dialog_listview(
                         datas: Datas.ToArray(),
                         type: XGUIDialogType.通知,
                         windowtitle: "XHud - 元素消息",
                         title: "批量扫描元素子组件",
                         msg: "以下是批量扫描到的所有元素子组件列表，请您检查核对：",
                         ok: "明白",
                         PrimaryIndex: 0,
                         usemodal: false,
                         themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
            }
            else
            {
                //-扫描 - 图元控制器
                for (int i = 0; i < base_script.primitive_controller_nodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{base_script.name} ({base_script.indicator} )";
                    dataitem.SubTitle = "已收集 / 图元控制器";
                    dataitem.Message = $"{base_script.primitive_controller_nodes[i].controller.name} ({base_script.primitive_controller_nodes[i].controller.indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 按钮
                for (int i = 0; i < base_script.button_nodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{base_script.name} ({base_script.indicator} )";
                    dataitem.SubTitle = "已收集 / 按钮";
                    dataitem.Message = $"{base_script.button_nodes[i].button.name} ({base_script.button_nodes[i].button.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 选项器
                for (int i = 0; i < base_script.option_nodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{base_script.name} ({base_script.indicator} )";
                    dataitem.SubTitle = "已收集 / 选项";
                    dataitem.Message = $"{base_script.option_nodes[i].option.name} ({base_script.option_nodes[i].option.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 滑动条
                for (int i = 0; i < base_script.slider_nodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{base_script.name} ({base_script.indicator} )";
                    dataitem.SubTitle = "已收集 / 滑动条";
                    dataitem.Message = $"{base_script.slider_nodes[i].slider.name} ({base_script.slider_nodes[i].slider.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 进度条
                for (int i = 0; i < base_script.progress_nodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{base_script.name} ({base_script.indicator} )";
                    dataitem.SubTitle = "已收集 / 进度条";
                    dataitem.Message = $"{base_script.progress_nodes[i].progress.name} ({base_script.progress_nodes[i].progress.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 开关
                for (int i = 0; i < base_script.toggle_nodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{base_script.name} ({base_script.indicator} )";
                    dataitem.SubTitle = "已收集 / 开关";
                    dataitem.Message = $"{base_script.toggle_nodes[i].toggle.name} ({base_script.toggle_nodes[i].toggle.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 声音
                for (int i = 0; i < base_script.sounder_nodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{base_script.name} ({base_script.indicator} )";
                    dataitem.SubTitle = "已收集 / 音效器";
                    dataitem.Message = $"{base_script.sounder_nodes[i].sounder.name} ({base_script.sounder_nodes[i].sounder.indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - Text文字
                for (int i = 0; i < base_script.text_nodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{base_script.name} ({base_script.indicator} )";
                    dataitem.SubTitle = "已收集 / Text文字";
                    dataitem.Message = $"{base_script.text_nodes[i].text.name} ({base_script.text_nodes[i].text.indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - Tmp文字
                for (int i = 0; i < base_script.tmptext_nodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{base_script.name} ({base_script.indicator} )";
                    dataitem.SubTitle = "已收集 / Text文字";
                    dataitem.Message = $"{base_script.tmptext_nodes[i].tmptext.name} ({base_script.tmptext_nodes[i].tmptext.indicator} )";
                    Datas.Add(dataitem);
                }

                if (base_script.primitive_controller_nodes.Count <= 0 && base_script.button_nodes.Count <= 0 && base_script.option_nodes.Count <= 0 && base_script.slider_nodes.Count <= 0 && base_script.progress_nodes.Count <= 0 && base_script.toggle_nodes.Count <= 0 && base_script.sounder_nodes.Count <= 0 && base_script.text_nodes.Count <= 0 && base_script.tmptext_nodes.Count <= 0)
                {
                    string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 元素消息",
                        title: "扫描元素子组件",
                        msg: $"未扫描到任何子元素，请您检查子物体中是否有物体包含以下组件： <color={hexcol}>XHud_PrimitiveController、XHud_Button、XHud_Progress、XHud_Slider、XHud_Option、XHudSound、XHud_Text、XHud_TmpText、XHudToggle</color>",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                }
                else
                {
                    XGUI.dialog_listview(
                        datas: Datas.ToArray(),
                        type: XGUIDialogType.通知,
                        windowtitle: "XHud - 元素消息",
                        title: "批量扫描元素子组件",
                        msg: "以下是扫描到的元素所有子组件列表，请您检查核对：",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: false,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                }
            }
        }
        #endregion

        #region 工具 - 元素状态
        /// <summary>
        /// 将元素重置到当前锚点的初始位置。
        /// <para/>
        /// 支持多选批量操作，并注册 Undo 以支持 Ctrl+Z。
        /// </summary>
        private void RestoreToAnchorPosition()
        {
            string res = XGUI.dialog(
                type: XGUIDialogType.警告,
                windowtitle: "XHud - 元素消息",
                title: "锚点初始化",
                msg: $"是否需要将元素回到当前所在的锚点初始位置？",
                ok: "确定",
                cancel: "暂不",
                PrimaryIndex: 0,
                usemodal: true,
                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

            if (res == "暂不")
            {
                return;
            }
            if (Targets_Selected())
            {
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    SerializedObject so = new SerializedObject(selected_objects[i]);
                    XHud_Module_Element ele = (XHud_Module_Element)so.targetObject;
                    Undo.RegisterCompleteObjectUndo(ele.transform, "MoveToAnchorPosition" + i);
                    ele.element_PositionResetZero();
                    so.Update();

                    so.ApplyModifiedProperties();
                }
            }
            else
            {
                Undo.RegisterCompleteObjectUndo(base_script.transform, "MoveToAnchorPosition");
                base_script.element_PositionResetZero();
            }
        }
        /// <summary>
        /// 修改元素透明度（写入 CanvasGroup.alpha）。
        /// <para/>
        /// 支持多选批量操作，并注册 Undo。
        /// </summary>
        private void ChangeAlpha()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(selected_objects[i]);
                    SerializedProperty sp_alpha = so_ele.FindProperty(nameof(XHud_Module_Element.alpha));
                    SerializedProperty sp_cgp = so_ele.FindProperty(nameof(XHud_Module_Element.canvasgroup));

                    CanvasGroup cgp = (CanvasGroup)sp_cgp.objectReferenceValue;

                    so_ele.Update();
                    sp_alpha.floatValue = this.sp_alpha.floatValue;
                    Undo.RecordObject(cgp, "ChangeAlpha");

                    cgp.alpha = this.sp_alpha.floatValue;

                    sp_cgp.serializedObject.ApplyModifiedProperties();
                    sp_alpha.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                CanvasGroup cgp = (CanvasGroup)sp_canvasgroup.objectReferenceValue;
                Undo.RecordObject(cgp, "ChangeAlpha");

                cgp.alpha = sp_alpha.floatValue;

                sp_canvasgroup.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 折叠 / 展开所有子组件列表。
        /// <para/>
        /// 支持多选批量操作。
        /// </summary>
        /// <param name="state">true = 折叠；false = 展开</param>
        private void AllListFoldState(bool state)
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    if (selected_objects[i] == null)
                        continue;
                    SerializedObject so_ele = new SerializedObject(selected_objects[i]);
                    SerializedProperty sp_primitives_is_fold = so_ele.FindProperty(nameof(XHud_Module_Element.primitives_is_fold));
                    SerializedProperty sp_button_is_fold = so_ele.FindProperty(nameof(XHud_Module_Element.button_is_fold));
                    SerializedProperty sp_progress_is_fold = so_ele.FindProperty(nameof(XHud_Module_Element.progress_is_fold));
                    SerializedProperty sp_toggle_is_fold = so_ele.FindProperty(nameof(XHud_Module_Element.toggle_is_fold));
                    SerializedProperty sp_text_is_fold = so_ele.FindProperty(nameof(XHud_Module_Element.text_is_fold));
                    SerializedProperty sp_tmptext_is_fold = so_ele.FindProperty(nameof(XHud_Module_Element.tmptext_is_fold));
                    SerializedProperty sp_option_is_fold = so_ele.FindProperty(nameof(XHud_Module_Element.option_is_fold));
                    SerializedProperty sp_slider_is_fold = so_ele.FindProperty(nameof(XHud_Module_Element.slider_is_fold));
                    SerializedProperty sp_event_is_fold = so_ele.FindProperty(nameof(XHud_Module_Element.event_is_fold));
                    SerializedProperty sp_sounder_is_fold = so_ele.FindProperty(nameof(XHud_Module_Element.sounder_is_fold));

                    so_ele.Update();

                    sp_primitives_is_fold.boolValue = state;
                    sp_button_is_fold.boolValue = state;
                    sp_event_is_fold.boolValue = state;
                    sp_slider_is_fold.boolValue = state;
                    sp_option_is_fold.boolValue = state;
                    sp_sounder_is_fold.boolValue = state;
                    sp_text_is_fold.boolValue = state;
                    sp_tmptext_is_fold.boolValue = state;
                    sp_progress_is_fold.boolValue = state;
                    sp_toggle_is_fold.boolValue = state;

                    sp_primitives_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_button_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_slider_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_option_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_event_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_sounder_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_text_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_tmptext_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_progress_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_toggle_is_fold.serializedObject.ApplyModifiedProperties();

                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                if (target != null)
                {
                    sp_primitives_is_fold.boolValue = state;
                    sp_button_is_fold.boolValue = state;
                    sp_option_is_fold.boolValue = state;
                    sp_slider_is_fold.boolValue = state;
                    sp_event_is_fold.boolValue = state;
                    sp_sounder_is_fold.boolValue = state;
                    sp_toggle_is_fold.boolValue = state;
                    sp_text_is_fold.boolValue = state;
                    sp_tmptext_is_fold.boolValue = state;
                    sp_progress_is_fold.boolValue = state;

                    sp_primitives_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_button_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_option_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_slider_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_event_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_sounder_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_toggle_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_text_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_tmptext_is_fold.serializedObject.ApplyModifiedProperties();
                    sp_progress_is_fold.serializedObject.ApplyModifiedProperties();

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
        #endregion

        #region 工具 - 图元动画器
        /// <summary>
        /// 刷新所有目标组件的动画耗时统计缓存。
        /// <para/>
        /// 单选时刷新当前元素，多选时遍历刷新所有选中元素。
        /// </summary>
        private void PrimitiveTweens_MaxDuration_Calculate()
        {
            if (!Targets_Selected())
            {
                sp_primitives_tween_max_duration.floatValue = PrimitiveTweens_MaxDuration_Get(base_script.primitive_controller_nodes, sp_primitives_tween_global_duration.floatValue);
                sp_primitives_tween_max_duration.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    selected_objects[i].primitives_tween_max_duration = PrimitiveTweens_MaxDuration_Get(selected_objects[i].primitive_controller_nodes, selected_objects[i].primitives_tween_global_duration);
                }
            }


        }
        /// <summary>
        /// 从所有子图元控制器的动画器中获取最大耗时。
        /// <para/>
        /// 耗时公式：
        /// <code>(每条动画的 MaxTimerWithGlobalDuration + 控制器延迟) 的最大值 × 全局倍增</code>
        /// </summary>
        /// <param name="list">图元控制器节点列表</param>
        /// <param name="globaldur">图元动画器的全局速率倍增</param>
        /// <returns>最大耗时（秒）</returns>
        public float PrimitiveTweens_MaxDuration_Get(List<PrimitiveControllerNode> list, float globaldur)
        {
            if (list.Count <= 0)
                return 0;
            float[] x_list = new float[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].controller == null)
                {
                    x_list[i] = 0;
                    continue;
                }
                if (list[i].controller.pt_tween != null)
                    list[i].controller.pt_tween.TweenNode_GetTimers();
                x_list[i] = (list[i].controller.pt_tween == null ? 0 : list[i].controller.pt_tween.max_timer_with_global_duration) + list[i].delay_time;
            }
            float v = XGUI_Utilitys.MaxValue(x_list);
            return v * globaldur;
        }
        /// <summary>
        /// 判断当前元素的所有子图元动画器中是否存在无限循环节点。
        /// <para/>
        /// 无限循环判定：<c>LoopCount == -1</c>。
        /// </summary>
        /// <returns>true 表示存在无限循环</returns>
        public bool PrimitiveTweens_HasLoopMode()
        {
            bool hasLoop = false;

            List<int> loops = new List<int>();
            for (int i = 0; i < sp_primitive_controller_nodes.arraySize; i++)
            {
                SerializedProperty sp_node_con = sp_primitive_controller_nodes.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(PrimitiveControllerNode.controller));
                XHud_Module_Primitive_Controller sp_con = (XHud_Module_Primitive_Controller)sp_node_con.objectReferenceValue;
                if (sp_con == null)
                {
                    continue;
                }
                if (sp_con == null && sp_con.pt_tween.primitive_tween_nodes == null || sp_con.pt_tween.primitive_tween_nodes.Count <= 0)
                {
                    continue;
                }
                else
                {
                    for (int s = 0; s < sp_con.pt_tween.primitive_tween_nodes.Count; s++)
                    {
                        loops.Add(sp_con.pt_tween.primitive_tween_nodes[s].loop_count);
                    }
                }
            }

            for (int i = 0; i < loops.Count; i++)
            {
                if (loops[i] == -1)
                {
                    hasLoop = true;
                    break;
                }
            }
            return hasLoop;
        }
        /// <summary>
        /// 判断指定动画节点列表中是否存在无限循环节点。
        /// <para/>
        /// 无限循环判定：<c>LoopCount == -1</c>。
        /// </summary>
        /// <param name="tweenlist">动画节点列表</param>
        /// <returns>true 表示存在无限循环</returns>
        public bool PrimitiveTweenNodes_HasLoopMode(List<TweenNode> tweenlist)
        {
            bool hasLoop = false;

            for (int s = 0; s < tweenlist.Count; s++)
            {
                if (tweenlist[s].loop_count == -1)
                {
                    hasLoop = true;
                    break;
                }
            }
            return hasLoop;
        }
        #endregion

        #region 工具 - 预制体状态
        /// <summary>
        /// 判断当前是否处于预制体独立场景模式（Prefab Stage）。
        /// </summary>
        /// <returns>true 表示在 Prefab Stage 中</returns>
        private bool IsInPrefabStageMode()
        {
            bool IsInPrefabStage = false;
            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage == null)
            {
                IsInPrefabStage = false;
            }
            else
            {
                if (stage.mode == PrefabStage.Mode.InIsolation)
                    IsInPrefabStage = true;
                else if (stage.mode == PrefabStage.Mode.InContext)
                    IsInPrefabStage = true;
            }

            return IsInPrefabStage;
        }
        /// <summary>
        /// 获取指定 GameObject 的预制体状态。
        /// <para/>
        /// 返回值：
        /// <list type="bullet">
        /// <item><description><c>InProject</c>：属于工程内的预制体资源；</description></item>
        /// <item><description><c>InScene</c>：属于场景中的预制体实例；</description></item>
        /// <item><description><c>InSceneNotPrefab</c>：场景中非预制体对象。</description></item>
        /// </list>
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        public ElementPrefabStatus GetPrefabStatus(GameObject gameObject)
        {
            if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                return ElementPrefabStatus.InProject;
            }
            else if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                return ElementPrefabStatus.InScene;
            }
            else
            {
                return ElementPrefabStatus.InSceneNotPrefab;
            }
        }
        #endregion

        #region 动效设置
        /// <summary>
        /// 打开元素动效资源采集器窗口，用于把当前生成或回收动效保存到动效库
        /// </summary>
        /// <param name="Type">
        /// 动效类型（生成动效或回收动效）
        /// </param>
        public void motion_save(HudElementMotionType Type)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 元素动效资源采集器");
            XGUI.CenterEditorWindow(new Vector2Int(348, Type == HudElementMotionType.Creator ? 870 : 800), window);

            switch (Type)
            {
                case HudElementMotionType.Creator:
                    window.set_motion(base_script.motion_create_args.Clone());
                    break;
                case HudElementMotionType.Recycler:
                    window.set_motion(base_script.motion_recycle_args.Clone());
                    break;
            }

            window.set_motion_type(Type);
            window.set_library_setter_mode(LibrarySetterMode.添加到库);
            window.set_button_text("添加", "取消");
            window.set_title("元素动效资源采集器");
            window.set_target_motion_library(XHud_Dashboard.get_manager().library_motions);
            //window.ShowModal();
            window.Show();
        }
        /// <summary>
        /// 打开动效资源修改器窗口，用于编辑当前生成或回收动效参数
        /// </summary>
        /// <param name="group">
        /// 待编辑的动效参数分组
        /// </param>
        public void motion_edit(XHud_Arg_MotionGroup group)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 动效资源修改器");
            XGUI.CenterEditorWindow(new Vector2Int(348, group.Type == HudElementMotionType.Creator ? 710 : 640), window);
            window.set_motion_type(group.Type);
            window.set_preedit_target_id(base_script.GetInstanceID().ToString());
            window.set_motion(group.Creator, group.Recycler);
            window.set_library_setter_mode(LibrarySetterMode.修改动效参数);
            window.set_button_text("更新");

            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);
            window.set_title($"<color={hexcol}>{(string.IsNullOrEmpty(sp_indicator.stringValue) ? base_script.gameObject.name : $"{base_script.gameObject.name} - {sp_indicator.stringValue}")} </color>");

            //window.ShowModal();
            window.Show();
        }
        #endregion

        #region 绘制控件
        /// <summary>
        /// 通用方法：绘制带标题的开关控件。
        /// <para/>
        /// 封装 <see cref="XGUI.layout_toggle"/>，统一标题宽度、字体、混合选项样式。
        /// </summary>
        private void DrawToggle(string title, SerializedProperty prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, string[] options = null, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(5, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: style,
                tog_padding: new RectOffset(0, 9, 0, 0),
                tog_margin: new RectOffset(0, 0, 0, 5),
                tog_mixed_options: options,
                tog_mixed_text_size: XGUIFontSize.M,
                tog_mixed_text_color: Color.black,
                tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                tog_mixed_font_style: FontStyle.Normal,
                tog_bg_off_color: color_bg_off,
                tog_bg_on_color: color_bg_on,
                tog_handler_off_color: color_off,
                tog_handler_on_color: color_on,
                tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                act_on_changed: act_on_changed);
        }
        #endregion
    }
}