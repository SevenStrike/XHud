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
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Element : Editor
    {
        #region 生命周期 - 元素预览（进入 / 退出）
        /// <summary>
        /// 播放预览：元素 - 进入
        /// </summary>
        private void Preview_ElementTween_In_Play()
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

            if (Targets_Selected())
            {
                // 预览列表
                List<XTween_Interface> preview_twns = new List<XTween_Interface>();

                for (int i = 0; i < selected_objects.Length; i++)
                {
                    XHud_Module_Element ele = selected_objects[i];

                    ele.tweens_preivew_in_state = true;
                    ele.tweens_preivew_out_state = false;
                    //Debug.Log($"TweensPreivew_In：{SelectedObjects[i].TweensPreivew_In_State}");

                    // 创建元素自身基础三项动画
                    ele.ElementTweens_Creator(ele.motion_create_args, null, true, ele.create_preview_set_position, ele.create_preview_position);

                    #region 元素基础三项动画 - 加入预览列表
                    if (ele.tween_alpha != null)
                        preview_twns.Add(ele.tween_alpha);
                    if (ele.tween_move != null)
                        preview_twns.Add(ele.tween_move);
                    if (ele.tween_rotation != null)
                        preview_twns.Add(ele.tween_rotation);
                    #endregion

                    #region 元素子级中的图元动画 - 加入预览列表
                    if (ele.preview_include_primitives_tween)
                    {
                        for (int v = 0; v < ele.primitive_controller_nodes.Count; v++)
                        {
                            if (ele.primitive_controller_nodes[v].controller.pt_tween == null)
                                continue;

                            XTween_Interface[] primitive_tweens = Preview_PrimitiveTween_Collect(ele.primitive_controller_nodes[v].controller.pt_tween, "元素进入时", ele.primitives_tween_global_duration, selected_objects[i].primitive_controller_nodes[v].delay_time);

                            for (int g = 0; g < primitive_tweens.Length; g++)
                            {
                                preview_twns.Add(primitive_tweens[g]);
                            }

                            // ── 新增：子图元音效预览 ──
                            Preview_PrimitiveTween_Sound(
                                ele.primitive_controller_nodes[v].controller.pt_tween,
                                "元素进入时",
                                ele.primitives_tween_global_duration,
                                ele.primitive_controller_nodes[v].delay_time);
                        }
                    }
                    #endregion

                    // 元素音效器预览
                    Preview_ElementSounder_OnList("元素进入时", ele.sounder_nodes);
                }

                Preview_Start(preview_twns.ToArray());
            }
            else
            {
                sp_tweens_preivew_in_state.boolValue = true;
                sp_tweens_preivew_out_state.boolValue = false;
                //Debug.Log($"TweensPreivew_In：{TweensPreivew_In_State.boolValue}");

                sp_tweens_preivew_in_state.serializedObject.ApplyModifiedProperties();
                sp_tweens_preivew_out_state.serializedObject.ApplyModifiedProperties();

                // 创建元素自身基础三项动画
                base_script.ElementTweens_Creator(base_script.motion_create_args, null, true, sp_create_preview_set_position.boolValue, sp_create_preview_position.vector3Value);

                // 预览列表
                List<XTween_Interface> preview_twns = new List<XTween_Interface>();

                #region 元素基础三项动画 - 加入预览列表
                if (base_script.tween_alpha != null)
                    preview_twns.Add(base_script.tween_alpha);
                if (base_script.tween_move != null)
                    preview_twns.Add(base_script.tween_move);
                if (base_script.tween_rotation != null)
                    preview_twns.Add(base_script.tween_rotation);
                #endregion

                #region 元素子级中的图元动画 - 加入预览列表
                if (sp_preview_include_primitives_tween.boolValue)
                {
                    for (int i = 0; i < base_script.primitive_controller_nodes.Count; i++)
                    {
                        if (base_script.primitive_controller_nodes[i].controller.pt_tween == null)
                            continue;

                        XTween_Interface[] primitive_tweens = Preview_PrimitiveTween_Collect(base_script.primitive_controller_nodes[i].controller.pt_tween, "元素进入时", base_script.primitives_tween_global_duration, base_script.primitive_controller_nodes[i].delay_time);

                        for (int s = 0; s < primitive_tweens.Length; s++)
                        {
                            preview_twns.Add(primitive_tweens[s]);
                        }

                        // ── 新增：子图元音效预览 ──
                        Preview_PrimitiveTween_Sound(
                            base_script.primitive_controller_nodes[i].controller.pt_tween,
                            "元素进入时",
                            base_script.primitives_tween_global_duration,
                            base_script.primitive_controller_nodes[i].delay_time);
                    }
                }
                #endregion

                Preview_Start(preview_twns.ToArray());

                // 元素音效器预览
                Preview_ElementSounder_OnSerialize("元素进入时", sp_sounder_nodes);
            }
        }
        /// <summary>
        /// 停止预览：元素 - 进入
        /// </summary>
        private void Preview_ElementTween_In_Stop()
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

            if (Targets_Selected())
            {
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    selected_objects[i].tweens_preivew_in_state = false;
                    //Debug.Log($"TweensPreivew_In：{SelectedObjects[i].TweensPreivew_In_State}");
                    selected_objects[i].ElementTweens_Kill();
                }
            }
            else
            {
                sp_tweens_preivew_in_state.boolValue = false;
                //Debug.Log($"TweensPreivew_In：{TweensPreivew_In_State.boolValue}");
                sp_tweens_preivew_in_state.serializedObject.ApplyModifiedProperties();

                base_script.ElementTweens_Kill();
            }

            Preview_Kill();

            // 点停止预览立即静音（原实现只在 OnDisable 或播完自灭时停）
            XGUI.EditorPlayer_Sound.Stop();
        }
        /// <summary>
        /// 播放预览：元素 - 退出
        /// </summary>
        private void Preview_ElementTween_Out_Play()
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

            if (Targets_Selected())
            {
                // 预览列表
                List<XTween_Interface> preview_twns = new List<XTween_Interface>();

                for (int i = 0; i < selected_objects.Length; i++)
                {
                    XHud_Module_Element ele = selected_objects[i];

                    ele.tweens_preivew_in_state = false;
                    ele.tweens_preivew_out_state = true;
                    //Debug.Log($"TweensPreivew_Out：{SelectedObjects[i].TweensPreivew_Out_State}");
                    ele.ElementTweens_Recycler(ele.motion_recycle_args, null, true, ele.recycle_preview_set_position, ele.recycle_preview_position);

                    #region 元素基础三项动画 - 加入预览列表
                    if (ele.tween_alpha != null)
                        preview_twns.Add(ele.tween_alpha);
                    if (ele.tween_move != null)
                        preview_twns.Add(ele.tween_move);
                    if (ele.tween_rotation != null)
                        preview_twns.Add(ele.tween_rotation);
                    #endregion

                    #region 元素子级中的图元动画 - 加入预览列表
                    if (ele.preview_include_primitives_tween)
                    {
                        for (int v = 0; v < ele.primitive_controller_nodes.Count; v++)
                        {
                            if (ele.primitive_controller_nodes[v].controller.pt_tween == null)
                                continue;

                            XTween_Interface[] primitive_tweens = Preview_PrimitiveTween_Collect(ele.primitive_controller_nodes[v].controller.pt_tween, "元素退出时", ele.primitives_tween_global_duration, ele.primitive_controller_nodes[v].delay_time);

                            for (int g = 0; g < primitive_tweens.Length; g++)
                            {
                                preview_twns.Add(primitive_tweens[g]);
                            }

                            // ── 新增：子图元音效预览 ──
                            Preview_PrimitiveTween_Sound(
                                ele.primitive_controller_nodes[v].controller.pt_tween,
                                "元素退出时",
                                ele.primitives_tween_global_duration,
                                ele.primitive_controller_nodes[v].delay_time);
                        }
                    }
                    #endregion

                    Preview_ElementSounder_OnList("元素退出时", ele.sounder_nodes);
                }

                Preview_Start(preview_twns.ToArray());
            }
            else
            {
                sp_tweens_preivew_in_state.boolValue = false;
                sp_tweens_preivew_out_state.boolValue = true;
                //Debug.Log($"TweensPreivew_Out：{TweensPreivew_Out_State.boolValue}");

                sp_tweens_preivew_in_state.serializedObject.ApplyModifiedProperties();
                sp_tweens_preivew_out_state.serializedObject.ApplyModifiedProperties();

                base_script.ElementTweens_Recycler(base_script.motion_recycle_args, null, true, sp_recycle_preview_set_position.boolValue, sp_recycle_preview_position.vector3Value);

                // 预览列表
                List<XTween_Interface> preview_twns = new List<XTween_Interface>();

                #region 元素基础三项动画 - 加入预览列表
                if (base_script.tween_alpha != null)
                    preview_twns.Add(base_script.tween_alpha);
                if (base_script.tween_move != null)
                    preview_twns.Add(base_script.tween_move);
                if (base_script.tween_rotation != null)
                    preview_twns.Add(base_script.tween_rotation);
                #endregion

                #region 元素子级中的图元动画 - 加入预览列表
                if (sp_preview_include_primitives_tween.boolValue)
                {
                    for (int i = 0; i < base_script.primitive_controller_nodes.Count; i++)
                    {
                        if (base_script.primitive_controller_nodes[i].controller.pt_tween == null)
                            continue;

                        XTween_Interface[] primitive_tweens = Preview_PrimitiveTween_Collect(base_script.primitive_controller_nodes[i].controller.pt_tween, "元素退出时", base_script.primitives_tween_global_duration, base_script.primitive_controller_nodes[i].delay_time);

                        for (int s = 0; s < primitive_tweens.Length; s++)
                        {
                            preview_twns.Add(primitive_tweens[s]);
                        }

                        // ── 新增：子图元音效预览 ──
                        Preview_PrimitiveTween_Sound(
                            base_script.primitive_controller_nodes[i].controller.pt_tween,
                            "元素退出时",
                            base_script.primitives_tween_global_duration,
                            base_script.primitive_controller_nodes[i].delay_time);
                    }
                }
                #endregion

                Preview_Start(preview_twns.ToArray());
                Preview_ElementSounder_OnSerialize("元素退出时", sp_sounder_nodes);
            }
        }
        /// <summary>
        /// 停止预览：元素 - 退出
        /// </summary>
        private void Preview_ElementTween_Out_Stop()
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

            if (Targets_Selected())
            {
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    selected_objects[i].tweens_preivew_out_state = false;
                    //Debug.Log($"TweensPreivew_Out：{SelectedObjects[i].TweensPreivew_Out_State}");
                }
            }
            else
            {
                sp_tweens_preivew_out_state.boolValue = false;
                //Debug.Log($"TweensPreivew_Out：{TweensPreivew_Out_State.boolValue}");
                sp_tweens_preivew_out_state.serializedObject.ApplyModifiedProperties();
            }

            Preview_Kill();

            // 点停止预览立即静音（原实现只在 OnDisable 或播完自灭时停）
            XGUI.EditorPlayer_Sound.Stop();
        }
        #endregion

        #region 生命周期 - 预览器驱动（播放 / 杀死 / 自动杀死 / 倒退）
        /// <summary>
        /// 动画预览 - 播放
        /// </summary>
        /// <param name="tweens">传入需要预览的动画，但前提是动画已创建，如果是空的则会导致预览异常</param>
        public void Preview_Start(XTween_Interface[] tweens)
        {
            if (Application.isPlaying)
                return;

            // 如果预览动画播完后自动杀死
            if (sp_autokill_preview_tweens.boolValue)
            {
                // 预览动画杀死后自动清空预览器的列表
                Editor_XTween_Previewer.AfterKillClear = true;
                // 预览动画杀死前将动画目标的属性倒退
                Editor_XTween_Previewer.BeforeKillRewind = true;

                Editor_XTween_Previewer.AutoKillWithDuration = true;

                // 预览动画杀死后的委托事件
                Editor_XTween_Previewer.act_on_editor_autokill += Preview_OnAutoKill;
            }
            else
            {
                Editor_XTween_Previewer.AutoKillWithDuration = false;
                // 预览动画杀死后自动清空预览器的列表 - 状态根据元素脚本的开关
                Editor_XTween_Previewer.AfterKillClear = sp_clear_preview_tweens_with_kill.boolValue;
                // 预览动画杀死前将动画目标的属性倒退 - 状态根据元素脚本的开关
                Editor_XTween_Previewer.BeforeKillRewind = sp_rewind_preview_tweens_with_kill.boolValue;
            }

            for (int i = 0; i < tweens.Length; i++)
            {
                Editor_XTween_Previewer.Append(tweens[i]);
            }

            Editor_XTween_Previewer.Play(null);

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(
              state: true,
              x_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
              x_title: "Seven Strike Media",
              x_msg: "元素动效预览中...",
              x_anchor: XHudSceneActivateMarkAnchor.左下);
        }
        /// <summary>
        ///  动画预览 - 杀死
        /// </summary>
        private void Preview_Kill()
        {
            if (Application.isPlaying)
                return;

            // 预览开关状态复位
            Preview_State_ResetAll();

            // 预览器执行动作：杀死动画
            Editor_XTween_Previewer.Kill(sp_clear_preview_tweens_with_kill.boolValue, sp_rewind_preview_tweens_with_kill.boolValue, () =>
            {
                //BaseScript.CurrentTweener = null;
            });

            // 当动画预览器为根据动画耗时自动杀死的情况下
            if (sp_autokill_preview_tweens.boolValue)
                Editor_XTween_Previewer.act_on_editor_autokill -= Preview_OnAutoKill;

            if (Targets_Selected())
            {
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    XHud_Module_Element ele = selected_objects[i];

                    // 根据是否包含图元动画预览类型对图元动画进行特性复位
                    if (sp_preview_include_primitives_tween.boolValue)
                    {
                        for (int d = 0; d < selected_objects[i].primitive_controller_nodes.Count; d++)
                        {
                            PrimitiveControllerNode con = selected_objects[i].primitive_controller_nodes[d];
                            if (con.controller.pt_feature == null)
                                continue;
                            con.controller.pt_feature.PrimitiveFeature_Load();
                        }
                    }
                    // 杀死元素三项自身动画
                    ele.ElementTweens_Kill();
                    // 清空元素三项自身动画
                    ele.ElementTweens_Clear();
                }
            }
            else
            {
                // 根据是否包含图元动画预览类型对图元动画进行特性复位
                if (sp_preview_include_primitives_tween.boolValue)
                {
                    for (int i = 0; i < base_script.primitive_controller_nodes.Count; i++)
                    {
                        PrimitiveControllerNode con = base_script.primitive_controller_nodes[i];
                        if (con.controller.pt_feature != null)
                            con.controller.pt_feature.PrimitiveFeature_Load();
                    }
                }
                // 杀死元素三项自身动画
                base_script.ElementTweens_Kill();
                // 清空元素三项自身动画
                base_script.ElementTweens_Clear();
            }

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);
        }
        /// <summary>
        ///  动画预览 - 自动杀死的委托
        /// </summary>
        private void Preview_OnAutoKill()
        {
            // 预览开关状态复位
            Preview_State_ResetAll();

            if (Targets_Selected())
            {
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    XHud_Module_Element ele = selected_objects[i];

                    // 根据是否包含图元动画预览类型对图元动画进行特性复位
                    if (ele.preview_include_primitives_tween)
                    {
                        for (int d = 0; d < ele.primitive_controller_nodes.Count; d++)
                        {
                            PrimitiveControllerNode con = ele.primitive_controller_nodes[d];
                            if (con.controller.pt_feature == null)
                                continue;
                            con.controller.pt_feature.PrimitiveFeature_Load();
                        }
                    }

                    // 杀死元素三项自身动画
                    ele.ElementTweens_Kill();
                    // 清空元素三项自身动画
                    ele.ElementTweens_Clear();

                    ele.alpha = 1;
                }
            }
            else
            {
                // 根据是否包含图元动画预览类型对图元动画进行特性复位
                if (sp_preview_include_primitives_tween.boolValue)
                {
                    for (int i = 0; i < base_script.primitive_controller_nodes.Count; i++)
                    {
                        PrimitiveControllerNode con = base_script.primitive_controller_nodes[i];
                        if (con.controller.pt_feature == null)
                            continue;
                        con.controller.pt_feature.PrimitiveFeature_Load();
                    }
                }
                // 杀死元素三项自身动画
                base_script.ElementTweens_Kill();
                // 清空元素三项自身动画
                base_script.ElementTweens_Clear();
                base_script.alpha = 1;
            }

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);

            // 清空预览动画杀死后的委托事件
            Editor_XTween_Previewer.act_on_editor_autokill -= Preview_OnAutoKill;
        }
        /// <summary>
        /// 预览倒退重置
        /// </summary>
        private void Preview_Rewind()
        {
            Editor_XTween_Previewer.Rewind();
        }
        /// <summary>
        /// 预览开关状态复位
        /// </summary>
        private void Preview_State_ResetAll()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    selected_objects[i].tweens_preivew_in_state = false;
                    selected_objects[i].tweens_preivew_out_state = false;
                }
            }
            else
            {
                sp_tweens_preivew_in_state.boolValue = false;
                sp_tweens_preivew_out_state.boolValue = false;

                sp_tweens_preivew_in_state.serializedObject.ApplyModifiedProperties();
                sp_tweens_preivew_out_state.serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion

        #region 工具 - 图元控制器预览
        /// <summary>
        /// 创建收集图元动画器的动画节点列表所有动画
        /// </summary>
        /// <param name="tweener">目标图元动画器</param>
        /// <param name="tim">目标时机名称</param>
        /// <param name="dur">动画时长（会与全局倍增相乘）</param>
        /// <param name="delay">额外延迟（会叠加到每条动画的 Delay 上）</param>
        /// <returns>收集到的动画实例数组</returns>
        private XTween_Interface[] Preview_PrimitiveTween_Collect(XHud_Module_Primitive_Tween tweener, string tim, float dur, float delay)
        {
            List<XTween_Interface> tweens = new List<XTween_Interface>();
            for (int i = 0; i < tweener.primitive_tween_nodes.Count; i++)
            {
                if (!tweener.primitive_tween_nodes[i].enabled)
                    continue;
                if (tweener.primitive_tween_nodes[i].timings != tim)
                    continue;
                if (!tweener.Tween_CreateRuleValidCheck(tweener.primitive_tween_nodes[i]))
                    continue;

                XTween_Interface tween = tweener.Tween_Create(tweener.primitive_tween_nodes[i], tweener.global_duration * hud_manager.tween_global_duration * dur);

                if (tween != null)
                    tween.SetDelay(tween.Delay + delay);

                if (tween != null)
                    tweens.Add(tween);
            }

            return tweens.ToArray();
        }
        /// <summary>
        /// 预览单个图元控制器：动画 + 音效。
        /// <para/>
        /// 【触发场景】元素 Inspector 的"图元控制器列表"里点击某个条目时调用。
        /// <para/>
        /// 【时机】固定为 "元素进入时"，与 <see cref="Preview_PrimitiveTween_Collect"/> 里收集动画时使用的时机保持一致。
        /// <para/>
        /// 【先停后播】入口先调 <see cref="XGUI_Player_Sound.Stop()"/> 清掉上一次的预览音效，
        /// 再入队新的；避免连续点条目时旧音效残留叠加。
        /// </summary>
        private void Preview_PrimitiveTween_OnSelect(PrimitiveControllerNode node)
        {
            Preview_Kill();

            // 先掐断上一次的预览音效，避免连续点条目时旧音效残留
            XGUI.EditorPlayer_Sound.Stop();

            // 预览列表
            List<XTween_Interface> preview_twns = new List<XTween_Interface>();

            if (node.controller.pt_tween == null)
                return;

            XTween_Interface[] primitive_tweens = Preview_PrimitiveTween_Collect(node.controller.pt_tween, "元素进入时", base_script.primitives_tween_global_duration, node.delay_time);

            for (int s = 0; s < primitive_tweens.Length; s++)
            {
                preview_twns.Add(primitive_tweens[s]);
            }

            // ── 子图元音效预览（时机与上方动画收集一致，延迟与子图元动画对齐） ──
            Preview_PrimitiveTween_Sound(
                node.controller.pt_tween,
                "元素进入时",
                base_script.primitives_tween_global_duration,
                node.delay_time);

            Preview_Start(preview_twns.ToArray());
        }
        #endregion

        #region 工具 - 配置加载与参数重置
        /// <summary>
        /// 读取XHud元素的动画预览配置数据
        /// </summary>
        private void Preview_Config_Load()
        {
            #region 动画预览器参数状态获取
            // 自动杀死预览动画
            sp_autokill_preview_tweens.boolValue = XHud_Dashboard.element_get_preview_autokill();
            sp_autokill_preview_tweens.serializedObject.ApplyModifiedProperties();

            // 预览杀死前重置动画
            sp_rewind_preview_tweens_with_kill.boolValue = XHud_Dashboard.element_get_preview_rewind_before_kill();
            sp_rewind_preview_tweens_with_kill.serializedObject.ApplyModifiedProperties();

            // 预览杀死后清空预览列表
            sp_clear_preview_tweens_with_kill.boolValue = XHud_Dashboard.element_get_preview_clear_after_kill();
            sp_clear_preview_tweens_with_kill.serializedObject.ApplyModifiedProperties();
            #endregion
        }
        #endregion

        #region 工具 - 动效库设置器
        /// <summary>
        /// 动效库添加器
        /// </summary>
        public void Open_MotionLibrarySetter(HudElementMotionType Type)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 元素动效资源采集器");
            XGUI.CenterEditorWindow(new Vector2Int(348, Type == HudElementMotionType.Creator ? 850 : 780), window);

            // 将要存入元素库的物体信息发送至窗口
            switch (Type)
            {
                case HudElementMotionType.Recycler:
                    Motion_Recycler rec = new Motion_Recycler();
                    rec.Alpha = base_script.motion_recycle_args.Alpha;

                    rec.Movement = new MotionNode_Movement();
                    rec.Movement.CopyData(base_script.motion_recycle_args.Movement);

                    rec.Rotation = new MotionNode_Rotation();
                    rec.Rotation.CopyData(base_script.motion_recycle_args.Rotation);

                    rec.Alpha = new MotionNode_Alpha();
                    rec.Alpha.CopyData(base_script.motion_recycle_args.Alpha);

                    window.set_motion(rec);
                    break;
                case HudElementMotionType.Creator:
                    Motion_Creator crc = new Motion_Creator();
                    crc.anchor = base_script.motion_create_args.anchor;

                    crc.Alpha = base_script.motion_create_args.Alpha;

                    crc.Movement = new MotionNode_Movement();
                    crc.Movement.CopyData(base_script.motion_create_args.Movement);

                    crc.Rotation = new MotionNode_Rotation();
                    crc.Rotation.CopyData(base_script.motion_create_args.Rotation);

                    crc.Alpha = new MotionNode_Alpha();
                    crc.Alpha.CopyData(base_script.motion_create_args.Alpha);

                    window.set_motion(crc);
                    break;
            }
            window.set_motion_type(Type);
            window.set_library_setter_mode(LibrarySetterMode.添加到库);
            window.set_button_text("添加", "取消");
            window.set_title("元素动效资源采集器");
            window.set_target_motion_library(hud_manager.library_motions);
            //window.ShowModal();
            window.Show();
        }
        #endregion
    }
}