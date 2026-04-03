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
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(XHud_LibrarySetTool_Color_PreviewData))]
    public class Editor_XHud_LibrarySetTool_Color_PreviewData : Editor
    {
        private XHud.XHud_LibrarySetTool_Color_PreviewData BaseScript;
        private Texture2D collect_r;
        private Texture2D collect_p;
        private Texture2D clear_r;
        private Texture2D clear_p;

        private void OnEnable()
        {
            BaseScript = (XHud.XHud_LibrarySetTool_Color_PreviewData)target;
            collect_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Color_Setter/collect_r");
            collect_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Color_Setter/collect_p");
            clear_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Color_Setter/clear_r");
            clear_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Color_Setter/clear_p");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Editor_XHud_GUI.Gui_Layout_Banner(Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Color_Setter/preview_data"), HudFilled.实体, HudColor.深空灰, "色卡编辑器预览视觉", Color.white);

            #region 快捷功能
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "收集序列帧", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            if (Editor_XHud_GUI.Gui_Layout_Button(14, "扫描序列帧", collect_r, collect_p))
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
            Editor_XHud_GUI.Gui_Layout_Space(50);
            if (Editor_XHud_GUI.Gui_Layout_Button(14, "清空序列帧", clear_r, clear_p))
            {
                if (BaseScript.Textures != null)
                    BaseScript.Textures.Clear();
            }
            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}