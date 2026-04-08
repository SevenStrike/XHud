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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XTween;
    using SevenStrikeModules.XTween.Editor;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Element : Editor
    {
        /// <summary>
        /// 收集元素的基础三项动画（前提是如果动画不为空则收集）
        /// </summary>
        /// <returns></returns>
        private XTween_Interface[] GetElementTweens()
        {
            // 收集元素三项基础动画给预览器
            List<XTween_Interface> twns = new List<XTween_Interface>();
            if (BaseScript.Tween_Alpha != null)
                twns.Add(BaseScript.Tween_Alpha);
            if (BaseScript.Tween_Move != null)
                twns.Add(BaseScript.Tween_Move);
            if (BaseScript.Tween_Rotation != null)
                twns.Add(BaseScript.Tween_Rotation);
            return twns.ToArray();
        }

        private XTween_Interface[] GetTargetsElementTweens()
        {
            // 收集所有批量元素三项基础动画给预览器
            List<XTween_Interface> twns = new List<XTween_Interface>();

            for (int i = 0; i < SelectedObjects.Length; i++)
            {
                XHud_Module_Element ele = SelectedObjects[i];
                if (ele.Tween_Alpha != null)
                    twns.Add(ele.Tween_Alpha);
                if (ele.Tween_Move != null)
                    twns.Add(ele.Tween_Move);
                if (ele.Tween_Rotation != null)
                    twns.Add(ele.Tween_Rotation);
            }
            return twns.ToArray();
        }

        /// <summary>
        /// 播放预览：元素 - 进入
        /// </summary>
        private void ElementTweens_Preview_In_Play()
        {
            if (Application.isPlaying)
            {
                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                return;
            }

            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SelectedObjects[i].TweensPreivew_In_State = true;
                    SelectedObjects[i].TweensPreivew_Out_State = false;
                    SelectedObjects[i].PrimitivePreivew_State = false;
                    //Debug.Log($"TweensPreivew_In：{SelectedObjects[i].TweensPreivew_In_State}");

                    SelectedObjects[i].ElementTweens_Creator(SelectedObjects[i].CreateArgs, null, true);
                    Preview_Start(GetTargetsElementTweens());
                }
            }
            else
            {
                TweensPreivew_In_State.boolValue = true;
                TweensPreivew_Out_State.boolValue = false;
                PrimitivePreivew_State.boolValue = false;
                //Debug.Log($"TweensPreivew_In：{TweensPreivew_In_State.boolValue}");

                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
                TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
                PrimitivePreivew_State.serializedObject.ApplyModifiedProperties();

                BaseScript.ElementTweens_Creator(BaseScript.CreateArgs, null, true);
                Preview_Start(GetElementTweens());
            }
        }
        /// <summary>
        /// 停止预览：元素 - 进入
        /// </summary>
        private void ElementTweens_Preview_In_Stop()
        {
            if (Application.isPlaying)
            {
                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                return;
            }

            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SelectedObjects[i].TweensPreivew_In_State = false;
                    //Debug.Log($"TweensPreivew_In：{SelectedObjects[i].TweensPreivew_In_State}");
                    SelectedObjects[i].KillElementTweens();
                }
            }
            else
            {
                TweensPreivew_In_State.boolValue = false;
                //Debug.Log($"TweensPreivew_In：{TweensPreivew_In_State.boolValue}");
                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();

                BaseScript.KillElementTweens();
            }

            Preview_Kill();
        }
        /// <summary>
        /// 播放预览：元素 - 退出
        /// </summary>
        private void ElementTweens_Preview_Out_Play()
        {
            if (Application.isPlaying)
            {
                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                return;
            }

            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SelectedObjects[i].TweensPreivew_In_State = false;
                    SelectedObjects[i].TweensPreivew_Out_State = true;
                    SelectedObjects[i].PrimitivePreivew_State = false;
                    //Debug.Log($"TweensPreivew_Out：{SelectedObjects[i].TweensPreivew_Out_State}");

                    SelectedObjects[i].ElementTweens_Recycler(SelectedObjects[i].RecycleArgs, null, true);
                    Preview_Start(GetTargetsElementTweens());
                }
            }
            else
            {
                TweensPreivew_In_State.boolValue = false;
                TweensPreivew_Out_State.boolValue = true;
                PrimitivePreivew_State.boolValue = false;
                //Debug.Log($"TweensPreivew_Out：{TweensPreivew_Out_State.boolValue}");

                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
                TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
                PrimitivePreivew_State.serializedObject.ApplyModifiedProperties();

                BaseScript.ElementTweens_Recycler(BaseScript.RecycleArgs, null, true);
                Preview_Start(GetElementTweens());
            }
        }
        /// <summary>
        /// 停止预览：元素 - 进入
        /// </summary>
        private void ElementTweens_Preview_Out_Stop()
        {
            if (Application.isPlaying)
            {
                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
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

            Preview_Kill();
        }
        /// <summary>
        /// 播放预览：元素 - 图元
        /// </summary>
        private void PrimitiveTweens_Preview_Play()
        {
            if (Application.isPlaying)
            {
                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                return;
            }

            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SelectedObjects[i].TweensPreivew_In_State = false;
                    SelectedObjects[i].TweensPreivew_Out_State = false;
                    SelectedObjects[i].PrimitivePreivew_State = true;
                    //Debug.Log($"PrimitivePreivew：{SelectedObjects[i].PrimitivePreivew_State}");
                }
            }
            else
            {
                TweensPreivew_In_State.boolValue = false;
                TweensPreivew_Out_State.boolValue = false;
                PrimitivePreivew_State.boolValue = true;
                //Debug.Log($"PrimitivePreivew：{PrimitivePreivew_State.boolValue}");

                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
                TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
                PrimitivePreivew_State.serializedObject.ApplyModifiedProperties();
            }

            // 收集子级下的所有图元Tweens
            //PreviewStart(GetPrimitiveTweens());
        }
        /// <summary>
        /// 停止预览：元素 - 图元
        /// </summary>
        private void PrimitiveTweens_Preview_Stop()
        {
            if (Application.isPlaying)
            {
                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                return;
            }

            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SelectedObjects[i].PrimitivePreivew_State = false;
                    //Debug.Log($"PrimitivePreivew：{SelectedObjects[i].PrimitivePreivew_State}");
                }
            }
            else
            {
                PrimitivePreivew_State.boolValue = false;
                //Debug.Log($"PrimitivePreivew：{PrimitivePreivew_State.boolValue}");
                PrimitivePreivew_State.serializedObject.ApplyModifiedProperties();
            }

            Preview_Kill();
        }
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
                    SelectedObjects[i].PrimitivePreivew_State = false;
                }
            }
            else
            {
                TweensPreivew_In_State.boolValue = false;
                TweensPreivew_Out_State.boolValue = false;
                PrimitivePreivew_State.boolValue = false;

                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
                TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
                PrimitivePreivew_State.serializedObject.ApplyModifiedProperties();
            }
        }

        //------------------------------------------------------------------------------------

        /// <summary>
        /// 动画预览 - 播放
        /// </summary>
        /// <param name="tweens">传入需要预览的动画，但前提是动画已创建，如果是空的则会导致预览异常</param>
        public void Preview_Start(XTween_Interface[] tweens)
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
                Editor_XTween_Previewer.act_on_editor_autokill += OnAutoKillPreview;
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
        }
        /// <summary>
        ///  动画预览 - 杀死
        /// </summary>
        private void Preview_Kill()
        {
            if (Application.isPlaying)
                return;

            // 预览开关状态复位
            StopAllPreviewState();

            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Element ele = SelectedObjects[i];
                    // 根据预览类型清空 & 杀死
                    if (!ele.PrimitivePreivew_State)
                    {
                        // 杀死元素三项自身动画
                        ele.KillElementTweens();
                        // 清空元素三项自身动画
                        ele.ClearElementTweens();
                    }
                    else
                    {
                        // 杀死子级所有图元动画
                        //BaseScript.KillPrimitiveTweens();
                        // 清空子级所有图元动画
                        //BaseScript.ClearPrimitiveTweens();
                    }
                }
            }
            else
            {
                // 根据预览类型清空 & 杀死
                if (!PrimitivePreivew_State.boolValue)
                {
                    // 杀死元素三项自身动画
                    BaseScript.KillElementTweens();
                    // 清空元素三项自身动画
                    BaseScript.ClearElementTweens();
                }
                else
                {
                    // 杀死子级所有图元动画
                    //BaseScript.KillPrimitiveTweens();
                    // 清空子级所有图元动画
                    //BaseScript.ClearPrimitiveTweens();
                }
            }


            // 预览器执行动作：杀死动画
            Editor_XTween_Previewer.Kill(ClearPreviewTweensWithKill.boolValue, RewindPreviewTweensWithKill.boolValue, () =>
            {
                //BaseScript.CurrentTweener = null;
            });

            // 当动画预览器为根据动画耗时自动杀死的情况下
            if (AutoKillPreviewTweens.boolValue)
                Editor_XTween_Previewer.act_on_editor_autokill -= OnAutoKillPreview;
        }
        /// <summary>
        ///  动画预览 - 倒退
        /// </summary>
        /// <summary>
        /// 杀死预览动画后的操作逻辑
        /// </summary>
        private void OnAutoKillPreview()
        {
            // 预览开关状态复位
            StopAllPreviewState();

            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Element ele = SelectedObjects[i];
                    if (!ele.PrimitivePreivew_State)
                    {
                        // 杀死元素三项自身动画
                        ele.KillElementTweens();
                        // 清空元素三项自身动画
                        ele.ClearElementTweens();
                    }
                    else
                    {
                        // 杀死子级所有图元动画
                        //BaseScript.KillPrimitiveTweens();
                        // 清空子级所有图元动画
                        //BaseScript.ClearPrimitiveTweens();
                    }
                }
            }
            else
            {
                // 根据预览类型清空 & 杀死
                if (!PrimitivePreivew_State.boolValue)
                {
                    // 杀死元素三项自身动画
                    BaseScript.KillElementTweens();
                    // 清空元素三项自身动画
                    BaseScript.ClearElementTweens();
                }
                else
                {
                    // 杀死子级所有图元动画
                    //BaseScript.KillPrimitiveTweens();
                    // 清空子级所有图元动画
                    //BaseScript.ClearPrimitiveTweens();
                }
            }

            // 清空预览动画杀死后的委托事件
            Editor_XTween_Previewer.act_on_editor_autokill -= OnAutoKillPreview;
        }
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
        private void Preview_Rewind()
        {
            Editor_XTween_Previewer.Rewind();
        }

        //------------------------------------------------------------------------------------

        /// <summary>
        /// 重置生成与回收的参数到默认
        /// </summary>
        private void ResetMotionParams(string state)
        {
            if (state == "CreateArgs")
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
            }
            else if (state == "RecycleArgs")
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
            }
        }
        /// <summary>
        /// 动效库添加器
        /// </summary>
        public void OpenParameterSetter(HudElementMotionType Type)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud 动效库采集器");
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(600, 530), window);

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
            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetButtonText("添加", "取消");
            window.SetTitle("XHud 动效库采集器");
            window.SetTarget_Hud_MotionLibrary(XHud_Dashboard.HudManagerGet().Hud_Motions);
            //window.ShowModal();
            window.Show();
        }
    }
}