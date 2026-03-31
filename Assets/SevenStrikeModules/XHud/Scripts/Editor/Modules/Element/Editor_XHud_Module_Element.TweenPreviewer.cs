namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XTween;
    using System.Collections.Generic;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Element : Editor
    {
        /// <summary>
        ///  动画预览列表
        /// </summary>
        private List<XTween_Interface> TweensPreivew_List = new List<XTween_Interface>();
        /// <summary>
        /// Editor协程 - 动画预览 - 开始
        /// </summary>
        private List<EditorCoroutine> Tweens_PreivewCoroutine_Play = new List<EditorCoroutine>();
        /// <summary>
        /// Editor协程 - 动画预览 - 停止
        /// </summary>
        private EditorCoroutine Tweens_PreivewCoroutine_Stop;

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
                    Debug.Log($"TweensPreivew_In：{SelectedObjects[i].TweensPreivew_In_State}");
                }
            }
            else
            {
                TweensPreivew_In_State.boolValue = true;
                TweensPreivew_Out_State.boolValue = false;
                PrimitivePreivew_State.boolValue = false;
                Debug.Log($"TweensPreivew_In：{TweensPreivew_In_State.boolValue}");

                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
                TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
                PrimitivePreivew_State.serializedObject.ApplyModifiedProperties();
            }
        }

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
                    Debug.Log($"TweensPreivew_In：{SelectedObjects[i].TweensPreivew_In_State}");
                }
            }
            else
            {
                TweensPreivew_In_State.boolValue = false;
                Debug.Log($"TweensPreivew_In：{TweensPreivew_In_State.boolValue}");
                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
            }
        }

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
                    Debug.Log($"TweensPreivew_Out：{SelectedObjects[i].TweensPreivew_Out_State}");
                }
            }
            else
            {
                TweensPreivew_In_State.boolValue = false;
                TweensPreivew_Out_State.boolValue = true;
                PrimitivePreivew_State.boolValue = false;
                Debug.Log($"TweensPreivew_Out：{TweensPreivew_Out_State.boolValue}");

                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
                TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
                PrimitivePreivew_State.serializedObject.ApplyModifiedProperties();
            }
        }

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
                    Debug.Log($"TweensPreivew_Out：{SelectedObjects[i].TweensPreivew_Out_State}");
                }
            }
            else
            {
                TweensPreivew_Out_State.boolValue = false;
                Debug.Log($"TweensPreivew_Out：{TweensPreivew_Out_State.boolValue}");
                TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
            }
        }

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
                    Debug.Log($"PrimitivePreivew：{SelectedObjects[i].PrimitivePreivew_State}");
                }
            }
            else
            {
                TweensPreivew_In_State.boolValue = false;
                TweensPreivew_Out_State.boolValue = false;
                PrimitivePreivew_State.boolValue = true;
                Debug.Log($"PrimitivePreivew：{PrimitivePreivew_State.boolValue}");

                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
                TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
                PrimitivePreivew_State.serializedObject.ApplyModifiedProperties();
            }
        }

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
                    Debug.Log($"PrimitivePreivew：{SelectedObjects[i].PrimitivePreivew_State}");
                }
            }
            else
            {
                PrimitivePreivew_State.boolValue = false;
                Debug.Log($"PrimitivePreivew：{PrimitivePreivew_State.boolValue}");
                PrimitivePreivew_State.serializedObject.ApplyModifiedProperties();
            }
        }

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
    }
}