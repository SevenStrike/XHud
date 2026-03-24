namespace SevenStrikeModules.XHud
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ColorSetterPreviewData", menuName = "XHud/CreateColorSetterPreviewData(创建Hud色卡编辑器预览资源文件)", order = 0)]
    public class xHud_LibrarySetTool_Color_PreviewData : ScriptableObject
    {
        [SerializeField]
        public List<Texture2D> Textures = new List<Texture2D>();

        // 确保内部名称与文件名一致
        private void OnValidate()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(name))
            {
                string path = AssetDatabase.GetAssetPath(this);
                if (!string.IsNullOrEmpty(path))
                {
                    name = Path.GetFileNameWithoutExtension(path);
                }
            }
#endif
        }
    }
}