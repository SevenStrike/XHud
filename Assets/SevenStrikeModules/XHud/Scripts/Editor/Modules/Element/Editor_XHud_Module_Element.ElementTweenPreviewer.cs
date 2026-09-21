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
        //------------------------------------------------------------------------------------
        /// <summary>
        /// 播放预览：元素 - 进入
        /// </summary>
        private void ElementTweens_Preview_In_Play()
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
                      themecolor: XHud_Dashboard.Theme_Primary);
                return;
            }

            if (Targets_Selected())
            {
                // 预览列表
                List<XTween_Interface> preview_twns = new List<XTween_Interface>();

                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Element ele = SelectedObjects[i];

                    ele.TweensPreivew_In_State = true;
                    ele.TweensPreivew_Out_State = false;
                    //Debug.Log($"TweensPreivew_In：{SelectedObjects[i].TweensPreivew_In_State}");

                    // 创建元素自身基础三项动画
                    ele.ElementTweens_Creator(ele.CreateArgs, null, true, ele.Crc_Preview_SetPosition, ele.Crc_Preview_Position);

                    #region 元素基础三项动画 - 加入预览列表
                    if (ele.Tween_Alpha != null)
                        preview_twns.Add(ele.Tween_Alpha);
                    if (ele.Tween_Move != null)
                        preview_twns.Add(ele.Tween_Move);
                    if (ele.Tween_Rotation != null)
                        preview_twns.Add(ele.Tween_Rotation);
                    #endregion

                    #region 元素子级中的图元动画 - 加入预览列表
                    if (ele.PreviewIncludePrimitivesTween)
                    {
                        for (int v = 0; v < ele.PrimitiveControllerNodes.Count; v++)
                        {
                            if (ele.PrimitiveControllerNodes[v].Controller.pt_Tween == null)
                                continue;

                            XTween_Interface[] primitive_tweens = Preview_PrimitiveTweens_Collected(ele.PrimitiveControllerNodes[v].Controller.pt_Tween, "元素进入时", ele.PrimitivesTweenGlobalDuration, SelectedObjects[i].PrimitiveControllerNodes[v].DelayTime);

                            for (int g = 0; g < primitive_tweens.Length; g++)
                            {
                                preview_twns.Add(primitive_tweens[g]);
                            }
                        }
                    }
                    #endregion
                    Preview_XHudSounds("元素进入时", ele.SounderNodes);
                }

                XTween_Preview_Start(preview_twns.ToArray());
            }
            else
            {
                TweensPreivew_In_State.boolValue = true;
                TweensPreivew_Out_State.boolValue = false;
                //Debug.Log($"TweensPreivew_In：{TweensPreivew_In_State.boolValue}");

                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
                TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();

                // 创建元素自身基础三项动画
                BaseScript.ElementTweens_Creator(BaseScript.CreateArgs, null, true, Crc_Preview_SetPosition.boolValue, Crc_Preview_Position.vector3Value);

                // 预览列表
                List<XTween_Interface> preview_twns = new List<XTween_Interface>();

                #region 元素基础三项动画 - 加入预览列表
                if (BaseScript.Tween_Alpha != null)
                    preview_twns.Add(BaseScript.Tween_Alpha);
                if (BaseScript.Tween_Move != null)
                    preview_twns.Add(BaseScript.Tween_Move);
                if (BaseScript.Tween_Rotation != null)
                    preview_twns.Add(BaseScript.Tween_Rotation);
                #endregion

                #region 元素子级中的图元动画 - 加入预览列表
                if (PreviewIncludePrimitivesTween.boolValue)
                {
                    for (int i = 0; i < BaseScript.PrimitiveControllerNodes.Count; i++)
                    {
                        if (BaseScript.PrimitiveControllerNodes[i].Controller.pt_Tween == null)
                            continue;

                        XTween_Interface[] primitive_tweens = Preview_PrimitiveTweens_Collected(BaseScript.PrimitiveControllerNodes[i].Controller.pt_Tween, "元素进入时", BaseScript.PrimitivesTweenGlobalDuration, BaseScript.PrimitiveControllerNodes[i].DelayTime);

                        for (int s = 0; s < primitive_tweens.Length; s++)
                        {
                            preview_twns.Add(primitive_tweens[s]);
                        }
                    }
                }
                #endregion

                XTween_Preview_Start(preview_twns.ToArray());
                Preview_XHudSounds("元素进入时", SounderNodes);
            }
        }
        /// <summary>
        /// 停止预览：元素 - 进入
        /// </summary>
        private void ElementTweens_Preview_In_Stop()
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
                     themecolor: XHud_Dashboard.Theme_Primary);
                return;
            }

            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SelectedObjects[i].TweensPreivew_In_State = false;
                    //Debug.Log($"TweensPreivew_In：{SelectedObjects[i].TweensPreivew_In_State}");
                    SelectedObjects[i].ElementTweens_Kill();
                }
            }
            else
            {
                TweensPreivew_In_State.boolValue = false;
                //Debug.Log($"TweensPreivew_In：{TweensPreivew_In_State.boolValue}");
                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();

                BaseScript.ElementTweens_Kill();
            }

            XTween_Preview_Kill();
        }
        /// <summary>
        /// 播放预览：元素 - 退出
        /// </summary>
        private void ElementTweens_Preview_Out_Play()
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
                     themecolor: XHud_Dashboard.Theme_Primary);
                return;
            }

            if (Targets_Selected())
            {
                // 预览列表
                List<XTween_Interface> preview_twns = new List<XTween_Interface>();

                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Element ele = SelectedObjects[i];

                    ele.TweensPreivew_In_State = false;
                    ele.TweensPreivew_Out_State = true;
                    //Debug.Log($"TweensPreivew_Out：{SelectedObjects[i].TweensPreivew_Out_State}");
                    ele.ElementTweens_Recycler(ele.RecycleArgs, null, true, ele.Rec_Preview_SetPosition, ele.Rec_Preview_Position);

                    #region 元素基础三项动画 - 加入预览列表
                    if (ele.Tween_Alpha != null)
                        preview_twns.Add(ele.Tween_Alpha);
                    if (ele.Tween_Move != null)
                        preview_twns.Add(ele.Tween_Move);
                    if (ele.Tween_Rotation != null)
                        preview_twns.Add(ele.Tween_Rotation);
                    #endregion

                    #region 元素子级中的图元动画 - 加入预览列表
                    if (ele.PreviewIncludePrimitivesTween)
                    {
                        for (int v = 0; v < ele.PrimitiveControllerNodes.Count; v++)
                        {
                            if (BaseScript.PrimitiveControllerNodes[v].Controller.pt_Tween == null)
                                continue;

                            XTween_Interface[] primitive_tweens = Preview_PrimitiveTweens_Collected(ele.PrimitiveControllerNodes[v].Controller.pt_Tween, "元素退出时", ele.PrimitivesTweenGlobalDuration, ele.PrimitiveControllerNodes[v].DelayTime);

                            for (int g = 0; g < primitive_tweens.Length; g++)
                            {
                                preview_twns.Add(primitive_tweens[g]);
                            }
                        }
                    }
                    #endregion
                    Preview_XHudSounds("元素退出时", ele.SounderNodes);
                }

                XTween_Preview_Start(preview_twns.ToArray());
            }
            else
            {
                TweensPreivew_In_State.boolValue = false;
                TweensPreivew_Out_State.boolValue = true;
                //Debug.Log($"TweensPreivew_Out：{TweensPreivew_Out_State.boolValue}");

                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
                TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();

                BaseScript.ElementTweens_Recycler(BaseScript.RecycleArgs, null, true, Rec_Preview_SetPosition.boolValue, Rec_Preview_Position.vector3Value);

                // 预览列表
                List<XTween_Interface> preview_twns = new List<XTween_Interface>();

                #region 元素基础三项动画 - 加入预览列表
                if (BaseScript.Tween_Alpha != null)
                    preview_twns.Add(BaseScript.Tween_Alpha);
                if (BaseScript.Tween_Move != null)
                    preview_twns.Add(BaseScript.Tween_Move);
                if (BaseScript.Tween_Rotation != null)
                    preview_twns.Add(BaseScript.Tween_Rotation);
                #endregion

                #region 元素子级中的图元动画 - 加入预览列表
                if (PreviewIncludePrimitivesTween.boolValue)
                {
                    for (int i = 0; i < BaseScript.PrimitiveControllerNodes.Count; i++)
                    {
                        if (BaseScript.PrimitiveControllerNodes[i].Controller.pt_Tween == null)
                            continue;

                        XTween_Interface[] primitive_tweens = Preview_PrimitiveTweens_Collected(BaseScript.PrimitiveControllerNodes[i].Controller.pt_Tween, "元素退出时", BaseScript.PrimitivesTweenGlobalDuration, BaseScript.PrimitiveControllerNodes[i].DelayTime);

                        for (int s = 0; s < primitive_tweens.Length; s++)
                        {
                            preview_twns.Add(primitive_tweens[s]);
                        }
                    }
                }
                #endregion

                XTween_Preview_Start(preview_twns.ToArray());
                Preview_XHudSounds("元素退出时", SounderNodes);
            }
        }
        /// <summary>
        /// 停止预览：元素 - 进入
        /// </summary>
        private void ElementTweens_Preview_Out_Stop()
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
                     themecolor: XHud_Dashboard.Theme_Primary);
                return;
            }

            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SelectedObjects[i].TweensPreivew_Out_State = false;
                    //Debug.Log($"TweensPreivew_Out：{SelectedObjects[i].TweensPreivew_Out_State}");
                }
            }
            else
            {
                TweensPreivew_Out_State.boolValue = false;
                //Debug.Log($"TweensPreivew_Out：{TweensPreivew_Out_State.boolValue}");
                TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
            }

            XTween_Preview_Kill();
        }
        //------------------------------------------------------------------------------------

        /// <summary>
        /// 创建收集图元动画器的动画节点列表所有动画
        /// </summary>
        /// <param name="tweener"></param>
        /// <returns></returns>
        private XTween_Interface[] Preview_PrimitiveTweens_Collected(XHud_Module_Primitive_Tween tweener, string tim, float dur, float delay)
        {
            List<XTween_Interface> tweens = new List<XTween_Interface>();
            for (int i = 0; i < tweener.PrimitiveTweenNodes.Count; i++)
            {
                if (!tweener.PrimitiveTweenNodes[i].Enabled)
                    continue;
                if (tweener.PrimitiveTweenNodes[i].Timings != tim)
                    continue;
                XTween_Interface tween = tweener.Tween_Create(tweener.PrimitiveTweenNodes[i], tweener.GlobalDuration * HudManager.DurationMultiply * dur);

                tween.SetDelay(tween.Delay + delay);

                if (tween != null)
                    tweens.Add(tween);
            }

            return tweens.ToArray();
        }

        private void Preview_PrimitiveTween(PrimitiveControllerNode node)
        {
            XTween_Preview_Kill();

            // 预览列表
            List<XTween_Interface> preview_twns = new List<XTween_Interface>();

            if (node.Controller.pt_Tween == null)
                return;

            XTween_Interface[] primitive_tweens = Preview_PrimitiveTweens_Collected(node.Controller.pt_Tween, "元素进入时", BaseScript.PrimitivesTweenGlobalDuration, node.DelayTime);

            for (int s = 0; s < primitive_tweens.Length; s++)
            {
                preview_twns.Add(primitive_tweens[s]);
            }

            XTween_Preview_Start(preview_twns.ToArray());
        }
        //------------------------------------------------------------------------------------
        /// <summary>
        /// 预览开关状态复位
        /// </summary>
        private void StopAllPreviewState()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SelectedObjects[i].TweensPreivew_In_State = false;
                    SelectedObjects[i].TweensPreivew_Out_State = false;
                }
            }
            else
            {
                TweensPreivew_In_State.boolValue = false;
                TweensPreivew_Out_State.boolValue = false;

                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
                TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
            }
        }

        //------------------------------------------------------------------------------------

        /// <summary>
        /// 动画预览 - 播放
        /// </summary>
        /// <param name="tweens">传入需要预览的动画，但前提是动画已创建，如果是空的则会导致预览异常</param>
        public void XTween_Preview_Start(XTween_Interface[] tweens)
        {
            if (Application.isPlaying)
                return;

            // 如果预览动画播完后自动杀死
            if (AutoKillPreviewTweens.boolValue)
            {
                // 预览动画杀死后自动清空预览器的列表
                Editor_XTween_Previewer.AfterKillClear = true;
                // 预览动画杀死前将动画目标的属性倒退
                Editor_XTween_Previewer.BeforeKillRewind = true;

                Editor_XTween_Previewer.AutoKillWithDuration = true;

                // 预览动画杀死后的委托事件
                Editor_XTween_Previewer.act_on_editor_autokill += XTween_OnAutoKillPreview;
            }
            else
            {
                Editor_XTween_Previewer.AutoKillWithDuration = false;
                // 预览动画杀死后自动清空预览器的列表 - 状态根据元素脚本的开关
                Editor_XTween_Previewer.AfterKillClear = ClearPreviewTweensWithKill.boolValue;
                // 预览动画杀死前将动画目标的属性倒退 - 状态根据元素脚本的开关
                Editor_XTween_Previewer.BeforeKillRewind = RewindPreviewTweensWithKill.boolValue;
            }

            for (int i = 0; i < tweens.Length; i++)
            {
                Editor_XTween_Previewer.Append(tweens[i]);
            }

            Editor_XTween_Previewer.Play(null);

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(
              state: true,
              x_color: XHud_Dashboard.Theme_Primary,
              x_title: "Seven Strike Media",
              x_msg: "元素动效预览中...",
              x_anchor: XHudSceneActivateMarkAnchor.左下);
        }
        /// <summary>
        ///  动画预览 - 杀死
        /// </summary>
        private void XTween_Preview_Kill()
        {
            if (Application.isPlaying)
                return;

            // 预览开关状态复位
            StopAllPreviewState();

            // 预览器执行动作：杀死动画
            Editor_XTween_Previewer.Kill(ClearPreviewTweensWithKill.boolValue, RewindPreviewTweensWithKill.boolValue, () =>
            {
                //BaseScript.CurrentTweener = null;
            });

            // 当动画预览器为根据动画耗时自动杀死的情况下
            if (AutoKillPreviewTweens.boolValue)
                Editor_XTween_Previewer.act_on_editor_autokill -= XTween_OnAutoKillPreview;

            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Element ele = SelectedObjects[i];

                    // 根据是否包含图元动画预览类型对图元动画进行特性复位
                    if (PreviewIncludePrimitivesTween.boolValue)
                    {
                        for (int d = 0; d < SelectedObjects[i].PrimitiveControllerNodes.Count; d++)
                        {
                            PrimitiveControllerNode con = SelectedObjects[i].PrimitiveControllerNodes[d];
                            if (con.Controller.pt_Feature == null)
                                continue;
                            con.Controller.pt_Feature.PrimitiveFeature_Load();
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
                if (PreviewIncludePrimitivesTween.boolValue)
                {
                    for (int i = 0; i < BaseScript.PrimitiveControllerNodes.Count; i++)
                    {
                        PrimitiveControllerNode con = BaseScript.PrimitiveControllerNodes[i];
                        if (con.Controller.pt_Feature != null)
                            con.Controller.pt_Feature.PrimitiveFeature_Load();
                    }
                }
                // 杀死元素三项自身动画
                BaseScript.ElementTweens_Kill();
                // 清空元素三项自身动画
                BaseScript.ElementTweens_Clear();
            }

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);
        }
        /// <summary>
        ///  动画预览 - 自动杀死的委托
        /// </summary>
        private void XTween_OnAutoKillPreview()
        {
            // 预览开关状态复位
            StopAllPreviewState();

            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Element ele = SelectedObjects[i];

                    // 根据是否包含图元动画预览类型对图元动画进行特性复位
                    if (ele.PreviewIncludePrimitivesTween)
                    {
                        for (int d = 0; d < ele.PrimitiveControllerNodes.Count; d++)
                        {
                            PrimitiveControllerNode con = ele.PrimitiveControllerNodes[d];
                            if (con.Controller.pt_Feature == null)
                                continue;
                            con.Controller.pt_Feature.PrimitiveFeature_Load();
                        }
                    }

                    // 杀死元素三项自身动画
                    ele.ElementTweens_Kill();
                    // 清空元素三项自身动画
                    ele.ElementTweens_Clear();

                    ele.Alpha = 1;
                }
            }
            else
            {
                // 根据是否包含图元动画预览类型对图元动画进行特性复位
                if (PreviewIncludePrimitivesTween.boolValue)
                {
                    for (int i = 0; i < BaseScript.PrimitiveControllerNodes.Count; i++)
                    {
                        PrimitiveControllerNode con = BaseScript.PrimitiveControllerNodes[i];
                        if (con.Controller.pt_Feature == null)
                            continue;
                        con.Controller.pt_Feature.PrimitiveFeature_Load();
                    }
                }
                // 杀死元素三项自身动画
                BaseScript.ElementTweens_Kill();
                // 清空元素三项自身动画
                BaseScript.ElementTweens_Clear();
                BaseScript.Alpha = 1;
            }

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);

            // 清空预览动画杀死后的委托事件
            Editor_XTween_Previewer.act_on_editor_autokill -= XTween_OnAutoKillPreview;
        }
        /// <summary>
        /// 预览倒退重置
        /// </summary>
        private void XTween_Preview_Rewind()
        {
            Editor_XTween_Previewer.Rewind();
        }

        //------------------------------------------------------------------------------------

        /// <summary>
        /// 读取XHud元素的动画预览配置数据
        /// </summary>
        private void LoadXHudElementPreviewConfig()
        {
            #region 动画预览器参数状态获取
            XHud_Dashboard.LoadXHudElementPreviewConfig();

            // 自动杀死预览动画
            AutoKillPreviewTweens.boolValue = XHud_Dashboard.Get_PreviewOption_AutoKillPreviewTweens();
            AutoKillPreviewTweens.serializedObject.ApplyModifiedProperties();

            // 预览杀死前重置动画
            RewindPreviewTweensWithKill.boolValue = XHud_Dashboard.Get_PreviewOption_RewindPreviewTweensWithKill();
            RewindPreviewTweensWithKill.serializedObject.ApplyModifiedProperties();

            // 预览杀死后清空预览列表
            ClearPreviewTweensWithKill.boolValue = XHud_Dashboard.Get_PreviewOption_ClearPreviewTweensWithKill();
            ClearPreviewTweensWithKill.serializedObject.ApplyModifiedProperties();
            #endregion
        }
        /// <summary>
        /// 重置生成与回收的参数到默认
        /// </summary>
        private void ResetMotionParams(string state)
        {
            if (state == "c")
            {
                CreateArgs.FindPropertyRelative("anchor").enumValueIndex = (int)XHudAnchor.中心;
                CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.S_从下至上;
                CreateArgs.FindPropertyRelative("Movement.Distance").floatValue = 100;
                CreateArgs.FindPropertyRelative("Movement.Duration").floatValue = 1;
                CreateArgs.FindPropertyRelative("Movement.Delay").floatValue = 0;
                CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue = "";
                CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                CreateArgs.serializedObject.ApplyModifiedProperties();

                Crc_Lib_Name.stringValue = null;
                Crc_Lib_Name.serializedObject.ApplyModifiedProperties();
            }
            else if (state == "r")
            {
                RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.D_从上至下;
                RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue = 100;
                RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue = 1;
                RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue = "";
                RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                RecycleArgs.serializedObject.ApplyModifiedProperties();

                Rec_Lib_Name.stringValue = null;
                Rec_Lib_Name.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 动效库添加器
        /// </summary>
        public void OpenParameterSetter(HudElementMotionType Type)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 元素动效资源采集器");
            XGUI.CenterEditorWindow(new Vector2Int(348, Type == HudElementMotionType.Creator ? 850 : 780), window);

            // 将要存入元素库的物体信息发送至窗口
            switch (Type)
            {
                case HudElementMotionType.Recycler:
                    Motion_Recycler rec = new Motion_Recycler();
                    rec.Alpha = BaseScript.RecycleArgs.Alpha;

                    rec.Movement = new MotionNode_Movement();
                    rec.Movement.CopyData(BaseScript.RecycleArgs.Movement);

                    rec.Rotation = new MotionNode_Rotation();
                    rec.Rotation.CopyData(BaseScript.RecycleArgs.Rotation);

                    rec.Alpha = new MotionNode_Alpha();
                    rec.Alpha.CopyData(BaseScript.RecycleArgs.Alpha);

                    window.SetElementMotion(rec);
                    break;
                case HudElementMotionType.Creator:
                    Motion_Creator crc = new Motion_Creator();
                    crc.anchor = BaseScript.CreateArgs.anchor;

                    crc.Alpha = BaseScript.CreateArgs.Alpha;

                    crc.Movement = new MotionNode_Movement();
                    crc.Movement.CopyData(BaseScript.CreateArgs.Movement);

                    crc.Rotation = new MotionNode_Rotation();
                    crc.Rotation.CopyData(BaseScript.CreateArgs.Rotation);

                    crc.Alpha = new MotionNode_Alpha();
                    crc.Alpha.CopyData(BaseScript.CreateArgs.Alpha);

                    window.SetElementMotion(crc);
                    break;
            }
            window.SetElementMotionType(Type);
            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetButtonText("添加", "取消");
            window.SetTitle("元素动效资源采集器");
            window.SetTarget_Hud_MotionLibrary(HudManager.Hud_Motions);
            //window.ShowModal();
            window.Show();
        }
    }
}