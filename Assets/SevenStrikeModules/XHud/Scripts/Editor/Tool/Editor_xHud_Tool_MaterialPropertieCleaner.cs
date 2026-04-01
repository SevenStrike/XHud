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