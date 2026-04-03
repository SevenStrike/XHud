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
    using UnityEditor;
    using UnityEngine;

    public class Editor_XHud_Tool_MaterialPropertieCleaner : EditorWindow
    {
        private Material m_selectedMaterial;
        private SerializedObject m_serializedObject;

        [MenuItem("Assets/XHud/MaterialPropertieClean (材质清理器)", priority = 2000, validate = true)]
        private static bool ValidateInit()
        {
            // 获取当前选中的对象
            Object selectedObject = Selection.activeObject;

            // 检查选中的对象是否为 Material 类型
            return selectedObject is Material;
        }

        [MenuItem("Assets/XHud/MaterialPropertieClean (材质清理器)", priority = 2000)]
        private static void Init()
        {
            GetWindow<Editor_XHud_Tool_MaterialPropertieCleaner>("Ref. Cleaner");
        }

        protected virtual void OnEnable()
        {
            GetSelectedMaterial();
        }

        protected virtual void OnSelectionChange()
        {
            GetSelectedMaterial();
        }

        protected virtual void OnProjectChange()
        {
            GetSelectedMaterial();
        }

        protected virtual void OnGUI()
        {
            EditorGUIUtility.labelWidth = 200f;

            if (m_selectedMaterial == null)
            {
                EditorGUILayout.LabelField("未选中材质!");
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("当前选中的材质:", m_selectedMaterial.name);
                EditorGUILayout.LabelField("正在使用的着色器:", m_selectedMaterial.shader.name);
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("材质着色器属性");

                m_serializedObject.Update();

                if (GUILayout.Button("快速移除废弃属性", GUILayout.Width(200f)))
                {
                    RemoveAllOldProperties();
                }

                RemoveAllOldProperties();

                EditorGUILayout.Space();

                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("属性：Textures");
                EditorGUI.indentLevel++;
                ProcessProperties("m_SavedProperties.m_TexEnvs");
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("属性：Floats");
                EditorGUI.indentLevel++;
                ProcessProperties("m_SavedProperties.m_Floats");
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("属性：Colors");
                EditorGUI.indentLevel++;
                ProcessProperties("m_SavedProperties.m_Colors");
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;
            }

            EditorGUIUtility.labelWidth = 0;
        }

        private void ProcessProperties(string path)
        {
            var properties = m_serializedObject.FindProperty(path);
            if (properties != null && properties.isArray)
            {
                for (int i = 0; i < properties.arraySize; i++)
                {
                    string propName = properties.GetArrayElementAtIndex(i).displayName;
                    bool exist = m_selectedMaterial.HasProperty(propName);

                    if (exist)
                    {
                        EditorGUILayout.LabelField(propName, "现役", "CN StatusInfo");
                    }
                    else
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField(propName, "已废弃", "CN StatusError");
                            if (GUILayout.Button("移除", GUILayout.Width(80f)))
                            {
                                properties.DeleteArrayElementAtIndex(i);
                                m_serializedObject.ApplyModifiedProperties();
                                GUIUtility.ExitGUI();
                            }
                        }

                    }
                }
            }
        }

        private void AutoProcessProperties(string path)
        {
            var properties = m_serializedObject.FindProperty(path);
            if (properties != null && properties.isArray)
            {
                for (int i = 0; i < properties.arraySize; i++)
                {
                    string propName = properties.GetArrayElementAtIndex(i).displayName;
                    bool exist = m_selectedMaterial.HasProperty(propName);

                    if (!exist)
                    {
                        properties.DeleteArrayElementAtIndex(i);
                    }
                    m_serializedObject.ApplyModifiedProperties();
                    m_serializedObject.Update();
                }
            }
        }

        private void GetSelectedMaterial()
        {
            m_selectedMaterial = Selection.activeObject as Material;
            if (m_selectedMaterial != null)
            {
                m_serializedObject = new SerializedObject(m_selectedMaterial);
            }

            Repaint();
        }

        private void RemoveAllOldProperties()
        {
            AutoProcessProperties("m_SavedProperties.m_TexEnvs");

            AutoProcessProperties("m_SavedProperties.m_Floats");

            AutoProcessProperties("m_SavedProperties.m_Colors");

            m_serializedObject.Update();
        }
    }
}