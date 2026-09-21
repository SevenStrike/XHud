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
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
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
        private Texture2D logo;

        private bool fold_based;

        private void OnEnable()
        {
            BaseScript = (XHud_LibrarySetTool_Color_PreviewData)target;

            collect_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_color_setter/collect_r");
            collect_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_color_setter/collect_p");
            clear_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_color_setter/clear_r");
            clear_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_color_setter/clear_p");
            logo = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_color_setter/preview_data");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: logo,
                icon_color: XHud_Dashboard.Theme_Primary,
                title_text: "XHUD  -  色卡编辑器预览视觉包",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: TextClipping.Ellipsis,
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            #region 快捷功能          
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "快捷功能",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(15, 15, 20, 15));

            #region 收集序列帧
            if (XGUI.layout_button(
                tooltip: "收集序列帧",
                tex_release: collect_r,
                tex_press: collect_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
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
            #endregion

            GUILayout.FlexibleSpace();

            #region 清空序列帧
            if (XGUI.layout_button(
                tooltip: "清空序列帧",
                tex_release: clear_r,
                tex_press: clear_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                if (BaseScript.Textures != null)
                    BaseScript.Textures.Clear();
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 源脚本
            fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: fold_based);

            if (fold_based)
            {
                DrawDefaultInspector();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}