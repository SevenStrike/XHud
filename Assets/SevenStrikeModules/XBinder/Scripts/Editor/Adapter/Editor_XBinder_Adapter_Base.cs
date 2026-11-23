namespace SevenStrikeModules.XBinder.Editor
{
    using UnityEditor;
    using UnityEngine;

    public abstract class Editor_XBinder_Adapter_Base : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty sp_name = serializedObject.FindProperty("target_category");
            SerializedProperty sp_key = serializedObject.FindProperty("target_key");

            EditorGUILayout.LabelField("数据源配置", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(sp_name, new GUIContent("数据容器名"));
            EditorGUILayout.PropertyField(sp_key, new GUIContent("数据键名"));

            EditorGUILayout.Space(10);

            // 画子类自己的字段
            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }
    }
}