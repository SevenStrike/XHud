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
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_BlurMask), true)]
    public class Editor_XHud_BlurMask : Editor
    {
        private XHud_BlurMask BaseScript;

        private SerializedProperty
            BlurMask,
            BlurMaskAlpha,
            BlurMaskRaycastAlphaThreshold,
            BlurMaskRaycastEnabled,
            BlurMaskColor,
            BlurMaskTexture,
            UniversalFeature_Blur_Intensity,
            IsInitialized;

        private bool OriginalDisplay;
        private Texture2D icon_main, status;

        #region 批量化操作
        private XHud_BlurMask[] SelectedObjects;

        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_BlurMask[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_BlurMask)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_BlurMask[targets.Length];
                SelectedObjects[0] = (XHud_BlurMask)target;
            }
        }

        private bool Targets_Selected()
        {
            if (SelectedObjects == null)
                return false;
            if (SelectedObjects.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        void OnEnable()
        {
            BaseScript = (XHud_BlurMask)target;

            BlurMask = serializedObject.FindProperty("BlurMask");
            BlurMaskAlpha = serializedObject.FindProperty("BlurMaskAlpha");
            BlurMaskRaycastAlphaThreshold = serializedObject.FindProperty("BlurMaskRaycastAlphaThreshold");
            BlurMaskRaycastEnabled = serializedObject.FindProperty("BlurMaskRaycastEnabled");
            BlurMaskColor = serializedObject.FindProperty("BlurMaskColor");
            BlurMaskTexture = serializedObject.FindProperty("BlurMaskTexture");
            UniversalFeature_Blur_Intensity = serializedObject.FindProperty("UniversalFeature_Blur_Intensity");
            IsInitialized = serializedObject.FindProperty("IsInitialized");

            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_SceneDefocusDevice/icon_main");
            status = Editor_XHud_GUI.GetIcon("Icons_XHud_SceneDefocusDevice/status");

            Targets_Get();

            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }

        private void OnUndoRedoPerformed()
        {
            // 重新同步序列化属性
            serializedObject.Update();

            xHud_EditorUpdate_BlurMask();

            // 重绘 Inspector
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 场景散焦器", Color.white);

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", XHud_Dashboard.Theme_Primary);

            Editor_XHud_GUI.Gui_Layout_Space(5);
            EditorGUI.BeginChangeCheck();

            #region 射线遮挡阈值状态     
            Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), BlurMaskRaycastEnabled.boolValue ? "射线阻挡" : "射线穿透", 12, BlurMaskRaycastEnabled.boolValue ? Color.red : XHud_Dashboard.Theme_Primary, status, 12, new Vector2(0, 4), false);
            #endregion

            Editor_XHud_GUI.SetEnabled(true);
            if (!IsInitialized.boolValue)
                Editor_XHud_GUI.SetEnabled(false);

            Editor_XHud_GUI.Gui_Layout_Property_Field("散焦面", BlurMask, 120);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("散焦强度", UniversalFeature_Blur_Intensity, 120);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("散焦透明度", BlurMaskAlpha, 120);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("散焦交互遮挡阈值", BlurMaskRaycastAlphaThreshold, 120);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("散焦交互遮挡", BlurMaskRaycastEnabled, 120);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("散焦面着色", BlurMaskColor, 120);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("散焦面纹理", BlurMaskTexture, 120);

            if (!Application.isPlaying)
            {
                Editor_XHud_GUI.SetEnabled(true);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                if (!IsInitialized.boolValue)
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button("创建散焦面", "", HudFilled.实体, HudColor.深空灰, Color.white, 30, new RectOffset(), new Vector2(0, 0)))
                    {
                        InitializeBlurMask();
                        IsInitialized.boolValue = true;
                    }
                }
                else
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button("移除散焦面", "", HudFilled.实体, HudColor.魅力红, Color.black, 30, new RectOffset(), new Vector2(0, 0)))
                    {
                        RemoveBlurMask();
                        IsInitialized.boolValue = false;
                    }
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                xHud_EditorUpdate_BlurMask();
            }
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();

            #region 源脚本
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 原始变量
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            OriginalDisplay = EditorGUILayout.Foldout(OriginalDisplay, "变量/属性", true);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            if (OriginalDisplay)
                DrawDefaultInspector();
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        private void xHud_EditorUpdate_BlurMask()
        {
            BaseScript.hm_BlurMaskUpdate();
            BaseScript.hm_UniversalFeature_Blur_FastTo_ForEditor(UniversalFeature_Blur_Intensity.floatValue);
            EditorUtility.SetDirty(BaseScript.UniversalFeature_Blur);
        }

        private void InitializeBlurMask()
        {
            #region 检查是否已经创建了散焦面物体
            bool exist = false;
            Canvas hudCanvas_Screen = XHud_Dashboard.HudManagerGet().HudCanvas_Screen;
            for (int i = 0; i < hudCanvas_Screen.transform.childCount; i++)
            {
                Transform obj = hudCanvas_Screen.transform.GetChild(i);
                Image img = obj.GetComponent<Image>();
                if (img != null)
                {
                    if (img.material != null)
                    {
                        if (img.material.shader.name == "TintedBlurUI")
                        {
                            exist = true;
                            break;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }
            if (exist)
                return;
            #endregion

            #region 散焦面创建
            GameObject obj_BlurMask = new GameObject();
            obj_BlurMask.layer = LayerMask.NameToLayer("XHud");
            UnityEngine.RectTransform m_BlurMask = obj_BlurMask.AddComponent<UnityEngine.RectTransform>();
            Image BlurMask_img = obj_BlurMask.AddComponent<Image>();
            BlurMask_img.color = Color.black;
            BlurMask_img.material = AssetDatabase.LoadAssetAtPath<Material>($"{XHud_Dashboard.Get_Path_XHUD_THIRDPLUGIN_Path()}Universal-Blur/Materials/UniversalBlur.mat");

            m_BlurMask.name = "BlurMask";
            m_BlurMask.SetParent(XHud_Dashboard.HudManagerGet().HudCanvas_Screen.transform);
            m_BlurMask.localPosition = Vector3.zero;
            m_BlurMask.localScale = Vector3.one;
            m_BlurMask.anchorMin = new Vector2(0, 0);
            m_BlurMask.anchorMax = new Vector2(1, 1);
            m_BlurMask.sizeDelta = new Vector2(0, 0);
            BlurMask.objectReferenceValue = BlurMask_img;
            BlurMask.serializedObject.ApplyModifiedProperties();
            #endregion
        }

        private void RemoveBlurMask()
        {
            if (BlurMask.objectReferenceValue == null)
                return;

            Image img_blurmask = (Image)BlurMask.objectReferenceValue;
            DestroyImmediate(img_blurmask.gameObject, true);
            BlurMask.objectReferenceValue = null;
            BlurMask.serializedObject.ApplyModifiedProperties();
        }
    }
}