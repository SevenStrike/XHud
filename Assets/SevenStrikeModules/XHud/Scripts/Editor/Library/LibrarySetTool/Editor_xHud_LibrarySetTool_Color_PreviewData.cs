namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(xHud_LibrarySetTool_Color_PreviewData))]
    public class Editor_xHud_LibrarySetTool_Color_PreviewData : Editor
    {
        private XHud.xHud_LibrarySetTool_Color_PreviewData BaseScript;
        private Texture2D collect_r;
        private Texture2D collect_p;
        private Texture2D clear_r;
        private Texture2D clear_p;

        private void OnEnable()
        {
            BaseScript = (XHud.xHud_LibrarySetTool_Color_PreviewData)target;
            collect_r = Editor_xHudGUI.GetIcon("Icons_Hud_Library_Color_Setter/collect_r");
            collect_p = Editor_xHudGUI.GetIcon("Icons_Hud_Library_Color_Setter/collect_p");
            clear_r = Editor_xHudGUI.GetIcon("Icons_Hud_Library_Color_Setter/clear_r");
            clear_p = Editor_xHudGUI.GetIcon("Icons_Hud_Library_Color_Setter/clear_p");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Editor_xHudGUI.Gui_Layout_Banner(Editor_xHudGUI.GetIcon("Icons_Hud_Library_Color_Setter/preview_data"), HudFilled.实体, HudColor.深空灰, "色卡编辑器预览视觉", Color.white);

            #region 快捷功能
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "收集序列帧", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_FlexSpace();
            if (Editor_xHudGUI.Gui_Layout_Button(14, "扫描序列帧", collect_r, collect_p))
            {
                if (BaseScript.Textures != null)
                    BaseScript.Textures.Clear();

                if (BaseScript.Textures == null)
                    BaseScript.Textures = new List<Texture2D>();

                string path = AssetDatabase.GetAssetPath(BaseScript);
                path = Path.GetDirectoryName(path);

                string[] v = Directory.GetFiles($"{path}/{BaseScript.name}", "*.png");
                for (int i = 0; i < v.Length; i++)
                {
                    BaseScript.Textures.Add(AssetDatabase.LoadAssetAtPath<Texture2D>($"{path}/{BaseScript.name}/{i}.png"));
                }

                EditorUtility.SetDirty(BaseScript);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            Editor_xHudGUI.Gui_Layout_Space(50);
            if (Editor_xHudGUI.Gui_Layout_Button(14, "清空序列帧", clear_r, clear_p))
            {
                if (BaseScript.Textures != null)
                    BaseScript.Textures.Clear();
            }
            Editor_xHudGUI.Gui_Layout_FlexSpace();
            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}