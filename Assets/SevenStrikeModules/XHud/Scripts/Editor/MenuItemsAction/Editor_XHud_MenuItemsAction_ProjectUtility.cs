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
    using SevenStrikeModules.XHud.Enums;
    using TMPro;
    using UnityEditor;
    using UnityEngine;

    public class Editor_XHud_MenuItemsAction_ProjectUtility : EditorWindow
    {
        [MenuItem("Assets/XHud/ClearTmpFontAssetData (清理TMP字体资源图集缓存内容)", priority = 2000, validate = true)]
        private static bool ProjectUtility_Clear_Tmp_Font_Asset_Content_Validate()
        {
            // 获取当前选中的对象
            Object selectedObject = Selection.activeObject;

            // 检查选中的对象是否为 TMP_FontAsset 类型
            return selectedObject is TMP_FontAsset;
        }
        [MenuItem("Assets/XHud/ClearTmpFontAssetData (清理TMP字体资源图集缓存内容)", priority = 2000)]
        private static void ProjectUtility_Clear_Tmp_Font_Asset_Content()
        {
            Object FontAsset = Selection.activeObject;

            string res = XGUI.dialog(
                type: XGUIDialogType.警告,
                windowtitle: "XHud - TmpFontAssets清理器消息",
                title: "清理TmpFontAssetAtlas",
                msg: $"您确认要将 {FontAsset.name} 图集内容清空吗？清空后字体图集将保持纯净状态！此操作不可逆，请谨慎操作！",
                ok: "清空",
                cancel: "暂不",
                PrimaryIndex: 1,
                usemodal: true,
                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
            if (res == "暂不")
            {
                return;
            }
            else
            {
                try
                {
                    TMP_FontAsset sd = (TMP_FontAsset)FontAsset;

                    sd.ClearFontAssetData();
                }
                catch (System.Exception err)
                {
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - TmpFontAssets清理器消息",
                        title: "清理TmpFontAssetAtlas",
                        msg: $"您选中的物体 {FontAsset.name} 并非是TmpFontAsset类型！,详细信息： {err.Message}",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                }
            }
        }

        [MenuItem("Assets/XHud/AssignLibrary (设置为当前使用的资源库)", priority = 1000, validate = true)]
        private static bool ProjectUtility_Set_Asset_Library_Validate()
        {
            // 获取当前选中的对象
            Object selectedObject = Selection.activeObject;

            bool valid = false;

            if (selectedObject is XHud_Library_Colors)
                valid = true;
            if (selectedObject is XHud_Library_Curves)
                valid = true;
            if (selectedObject is XHud_Library_Element)
                valid = true;
            if (selectedObject is XHud_Library_Motion)
                valid = true;
            if (selectedObject is XHud_Library_Sounds)
                valid = true;
            if (selectedObject is XHud_Library_TextStyle)
                valid = true;
            if (selectedObject is XHud_Library_Transition)
                valid = true;

            // 检查选中的对象是否为 Material 类型
            return valid;
        }
        [MenuItem("Assets/XHud/AssignLibrary (设置为当前使用的资源库)", priority = 1000)]
        private static void ProjectUtility_Set_Asset_Library()
        {
            XHud_Manager manager = FindFirstObjectByType<XHud_Manager>();

            Object obj = Selection.activeObject;

            if (manager != null)
            {
                string type = obj.GetType().ToString();

                if (type == "SevenStrikeModules.XHud.Hud_SoundsLibrary")
                {
                    manager.library_sounds = obj as XHud_Library_Sounds;
                    XGUI_Utilitys.Console("XHud - 库通知", "已将名称为： " + manager.library_sounds.library_name + " 的音效库设为当前使用！", XGUIMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_TextStyleLibrary")
                {
                    manager.library_textstyles = obj as XHud_Library_TextStyle;
                    XGUI_Utilitys.Console("XHud - 库通知", "已将名称为： " + manager.library_textstyles.library_name + " 的字体库设为当前使用！", XGUIMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_TransitionLibrary")
                {
                    manager.library_transitions = obj as XHud_Library_Transition;
                    XGUI_Utilitys.Console("XHud - 库通知", "已将名称为： " + manager.library_transitions.library_name + " 的转场库设为当前使用！", XGUIMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_MotionLibrary")
                {
                    manager.library_motions = obj as XHud_Library_Motion;
                    XGUI_Utilitys.Console("XHud - 库通知", "已将名称为： " + manager.library_motions.name + " 的动效库设为当前使用！", XGUIMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_ElementLibrary")
                {
                    XHud_Library_Element lib = obj as XHud_Library_Element;
                    if (manager.ele_library_elements.Count > 0)
                    {
                        bool isexist = false;
                        for (int i = 0; i < manager.ele_library_elements.Count; i++)
                        {
                            if (manager.ele_library_elements[i].library_name == lib.library_name || manager.ele_library_elements[i].GetInstanceID() == lib.GetInstanceID())
                            {
                                isexist = true;
                            }
                        }
                        if (!isexist)
                        {
                            manager.ele_library_elements.Add(lib);
                            XGUI_Utilitys.Console("XHud - 库通知", "已将名称为： " + lib.library_name + " 的元素库添加到元素库列表中！", XGUIMsgState.通知);
                        }
                        else
                        {
                            XGUI_Utilitys.Console("XHud - 库通知", "元素库列表中已存在名称为： " + lib.library_name + " 的元素库！", XGUIMsgState.警告);
                        }
                    }
                    else
                    {
                        manager.ele_library_elements.Add(lib);
                        XGUI_Utilitys.Console("XHud - 库通知", "已将名称为： " + lib.library_name + " 的元素库添加到元素库列表中！", XGUIMsgState.通知);
                    }
                }
                else if (type == "SevenStrikeModules.XHud.Hud_CurvesLibrary")
                {
                    manager.library_curves = obj as XHud_Library_Curves;
                    XGUI_Utilitys.Console("XHud - 库通知", "已将名称为： " + manager.library_curves.library_name + " 的曲线库设为当前使用！", XGUIMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_ColorsLibrary")
                {
                    manager.library_colors = obj as XHud_Library_Colors;
                    XGUI_Utilitys.Console("XHud - 库通知", "已将名称为： " + manager.library_colors.library_name + " 的颜色库设为当前使用！", XGUIMsgState.通知);
                }
            }
        }

        [MenuItem("Assets/XHud/CreateConfigs (创建XHud配置文件)", priority = 0, validate = true)]
        private static bool ProjectUtility_Create_Configs_Validate()
        {
            return Selection.objects.Length != 0;
        }
        [MenuItem("Assets/XHud/CreateConfigs (创建XHud配置文件)", priority = 0)]
        private static void ProjectUtility_Create_Configs()
        {
            XHud_Dashboard.config_create();
            XHud_Dashboard.config_save();
        }
    }
}