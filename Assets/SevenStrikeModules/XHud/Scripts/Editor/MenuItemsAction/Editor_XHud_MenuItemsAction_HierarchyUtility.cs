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
 */namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XHud.Utilitys;
    using UnityEditor;
    using UnityEngine;

    public class Editor_XHud_MenuItemsAction_HierarchyUtility : EditorWindow
    {
        [MenuItem("GameObject/XHud/Utilitys (实用工具)/ChildCountGet (获取子物体数量)", priority = -1)]
        private static void util_GetChildCount()
        {
            GameObject seleobject = Selection.activeGameObject;
            string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 实用工具消息", "获取子物体数量", $"当前物体的子物体数量为： <color={hexcol}> {seleobject.transform.childCount} </color>", "明白", 0);

        }
        [MenuItem("GameObject/XHud/Utilitys (实用工具)/SelectedAllChild (选中所有子物体)", priority = -10)]
        private static void util_SelectedAllChild()
        {
            GameObject seleobject = Selection.activeGameObject;
            GameObject[] arr = new GameObject[seleobject.transform.childCount];
            for (int i = 0; i < arr.Length; i++)
            {
                GameObject obj = seleobject.transform.GetChild(i).gameObject;
                if (obj.activeSelf)
                    arr[i] = obj;
            }
            Selection.objects = arr;
        }
    }
}