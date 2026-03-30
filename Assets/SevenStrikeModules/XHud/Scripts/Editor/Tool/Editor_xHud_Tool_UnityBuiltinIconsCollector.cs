namespace SevenStrikeModules.XHud.Utilitys
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class Editor_XHud_Tool_UnityBuiltinIconsCollector : EditorWindow
    {

        [MenuItem(("Tools/XHud/UnityBuiltinIconsGet"))]
        static void Init()
        {
            EditorWindow.GetWindow<Editor_XHud_Tool_UnityBuiltinIconsCollector>("Unity内建图标合集获取器");
        }

        Vector2 m_Scroll;
        List<string> m_Icons = null;

        void Awake()
        {
            m_Icons = new List<string>();
            Texture2D[] t = Resources.FindObjectsOfTypeAll<Texture2D>();

            foreach (Texture2D x in t)
            {
                Debug.unityLogger.logEnabled = false;
                GUIContent gc = EditorGUIUtility.IconContent(x.name);
                Debug.unityLogger.logEnabled = true;
                if (gc != null && gc.image != null)
                {
                    m_Icons.Add(x.name);
                }
            }

            var scroll = new ScrollView(ScrollViewMode.Vertical);
            rootVisualElement.Add(scroll);
            scroll.StretchToParentSize();

            var container = scroll.Q<VisualElement>("unity-content-container");
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexWrap = Wrap.Wrap;

            foreach (var icon in m_Icons)
            {
                var viewEle = new VisualElement();
                viewEle.style.width = 200;
                viewEle.style.flexDirection = FlexDirection.Row;

                var btn = new Button();
                btn.style.height = 30;
                btn.style.width = 30;
                btn.clicked += () =>
                {
                    GUIUtility.systemCopyBuffer = icon;
                    ShowNotification(new GUIContent("图标名称：" + icon + " 已拷贝到剪切板！"), 1f);
                };

                var iconObj = EditorGUIUtility.IconContent(icon);
                btn.style.backgroundImage = new StyleBackground(iconObj.image as Texture2D);

                var label = new Label(icon);
                label.style.width = new StyleLength(StyleKeyword.Auto);
                label.style.height = 30;
                viewEle.Add(btn);
                viewEle.Add(label);
                scroll.Add(viewEle);
            }
        }
    }
}