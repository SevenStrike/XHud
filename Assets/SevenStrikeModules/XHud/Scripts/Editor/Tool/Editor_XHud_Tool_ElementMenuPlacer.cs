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
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 元素放置器菜单工具窗口。
    /// <para/>
    /// <list type="bullet">
    /// <item><description>在 Project 视图右键菜单中添加 <c>Assets/XHud/PlaceToAnchor</c> 系列菜单。</description></item>
    /// <item><description>把选中的元素预制体实例化到管理器的指定锚点下（屏幕 / 世界）。</description></item>
    /// <item><description>提供自动识别（依 RMS 数据）与手动指定锚点两种放置方式。</description></item>
    /// </list>
    /// </summary>
    public class Editor_XHud_Tool_ElementMenuPlacer : EditorWindow
    {
        #region 工具 - 锚点放置
        /// <summary>
        /// 把选中元素预制体实例化到指定锚点下。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>校验管理器与锚点是否存在。</description></item>
        /// <item><description>遍历选中对象，仅实例化带 <c>XHud_Module_Element</c> 的预制体。</description></item>
        /// <item><description>按锚点类型应用 RectTransform 参数，并选中新创建的对象。</description></item>
        /// </list>
        /// </summary>
        /// <param name="anchor">目标锚点类型。</param>
        private static void SendObjects(XHudAnchor anchor)
        {
            XHud_Manager manager = XHud_Dashboard.HudManagerGet();
            if (manager == null)
            {
                XGUI_Utilitys.Console("XHud - 元素放置器通知", "未找到XHudManager管理器！", XGUIMsgState.错误);
                return;
            }

            // 获取目标锚点
            Anchor_Layout targetAnchor = null;
            for (int i = 0; i < manager.Anchors_Layout_Screen.Count; i++)
            {
                if (manager.Anchors_Layout_Screen[i].Type == anchor)
                {
                    targetAnchor = manager.Anchors_Layout_Screen[i];
                    break;
                }
            }

            if (targetAnchor == null)
            {
                XGUI_Utilitys.Console("XHud - 元素放置器通知", $"未找到锚点: {anchor}！", XGUIMsgState.错误);
                return;
            }

            // 获取所有选中的对象
            Object[] selectedObjects = Selection.objects;
            List<GameObject> createdObjects = new List<GameObject>();

            foreach (Object obj in selectedObjects)
            {
                // 检查是否为有效的预制体
                if (PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.NotAPrefab)
                    continue;

                GameObject prefab = obj as GameObject;
                if (prefab == null) continue;

                // 检查是否有 XHud_Module_Element 组件
                if (prefab.GetComponent<XHud_Module_Element>() == null) continue;

                // 实例化预制体
                GameObject objins = PrefabUtility.InstantiatePrefab(prefab, targetAnchor.Anchor.transform) as GameObject;
                if (objins == null) continue;

                Undo.RegisterCreatedObjectUndo(objins, "Place XHud Element");

                // 设置 RectTransform 属性
                RectTransform rect = objins.GetComponent<RectTransform>();
                ApplyAnchorSettings(rect, anchor);

                createdObjects.Add(objins);
            }

            // 选中所有新创建的对象
            if (createdObjects.Count > 0)
            {
                Selection.objects = createdObjects.ToArray();
                XGUI_Utilitys.Console("XHud - 元素放置器通知", $"成功放置 {createdObjects.Count} 个元素到 {anchor}", XGUIMsgState.确认);
            }
            else
            {
                XGUI_Utilitys.Console("XHud - 元素放置器通知", "没有有效的元素被放置！", XGUIMsgState.警告);
            }
        }
        /// <summary>
        /// 根据锚点类型应用 RectTransform 设置。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>底层：拉伸全屏。</description></item>
        /// <item><description>上/下/左/右/中心/顶层：贴边或居中锚点。</description></item>
        /// <item><description>左上/左下/右上/右下：角点锚点。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rect">目标 RectTransform。</param>
        /// <param name="anchor">锚点类型。</param>
        private static void ApplyAnchorSettings(RectTransform rect, XHudAnchor anchor)
        {
            switch (anchor)
            {
                case XHudAnchor.底层:
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(1f, 1f);
                    rect.offsetMin = new Vector2(0, 0);
                    rect.offsetMax = new Vector2(0, 0);
                    rect.anchoredPosition = Vector2.zero;
                    break;
                case XHudAnchor.上:
                    rect.pivot = new Vector2(0.5f, 1f);
                    rect.anchorMin = new Vector2(0.5f, 1f);
                    rect.anchorMax = new Vector2(0.5f, 1f);
                    rect.anchoredPosition = Vector2.zero;
                    break;
                case XHudAnchor.下:
                    rect.pivot = new Vector2(0.5f, 0f);
                    rect.anchorMin = new Vector2(0.5f, 0f);
                    rect.anchorMax = new Vector2(0.5f, 0f);
                    rect.anchoredPosition = Vector2.zero;
                    break;
                case XHudAnchor.左:
                    rect.pivot = new Vector2(0f, 0.5f);
                    rect.anchorMin = new Vector2(0f, 0.5f);
                    rect.anchorMax = new Vector2(0f, 0.5f);
                    rect.anchoredPosition = Vector2.zero;
                    break;
                case XHudAnchor.右:
                    rect.pivot = new Vector2(1f, 0.5f);
                    rect.anchorMin = new Vector2(1f, 0.5f);
                    rect.anchorMax = new Vector2(1f, 0.5f);
                    rect.anchoredPosition = Vector2.zero;
                    break;
                case XHudAnchor.中心:
                case XHudAnchor.顶层:
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.anchorMin = new Vector2(0.5f, 0.5f);
                    rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.anchoredPosition = Vector2.zero;
                    break;
                case XHudAnchor.左上:
                    rect.pivot = new Vector2(0f, 1f);
                    rect.anchorMin = new Vector2(0f, 1f);
                    rect.anchorMax = new Vector2(0f, 1f);
                    rect.anchoredPosition = Vector2.zero;
                    break;
                case XHudAnchor.左下:
                    rect.pivot = new Vector2(0f, 0f);
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(0f, 0f);
                    rect.anchoredPosition = Vector2.zero;
                    break;
                case XHudAnchor.右上:
                    rect.pivot = new Vector2(1f, 1f);
                    rect.anchorMin = new Vector2(1f, 1f);
                    rect.anchorMax = new Vector2(1f, 1f);
                    rect.anchoredPosition = Vector2.zero;
                    break;
                case XHudAnchor.右下:
                    rect.pivot = new Vector2(1f, 0f);
                    rect.anchorMin = new Vector2(1f, 0f);
                    rect.anchorMax = new Vector2(1f, 0f);
                    rect.anchoredPosition = Vector2.zero;
                    break;
            }
        }
        /// <summary>
        /// 把选中元素预制体实例化到世界锚点下。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅实例化带 <c>XHud_Module_Element</c> 的预制体。</description></item>
        /// <item><description>统一设置为居中锚点。</description></item>
        /// <item><description>选中所有新创建的对象。</description></item>
        /// </list>
        /// </summary>
        private static void SendObjects_World()
        {
            XHud_Manager manager = XHud_Dashboard.HudManagerGet();
            if (manager == null)
            {
                XGUI_Utilitys.Console("XHud - 元素放置器通知", "未找到XHudManager管理器！", XGUIMsgState.错误);
                return;
            }

            Object[] selectedObjects = Selection.objects;
            List<GameObject> createdObjects = new List<GameObject>();

            foreach (Object obj in selectedObjects)
            {
                if (PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.NotAPrefab)
                    continue;

                GameObject prefab = obj as GameObject;
                if (prefab == null) continue;
                if (prefab.GetComponent<XHud_Module_Element>() == null) continue;

                GameObject objins = PrefabUtility.InstantiatePrefab(prefab, manager.HudCanvas_WorldAnchor) as GameObject;
                if (objins == null) continue;

                Undo.RegisterCreatedObjectUndo(objins, "Place XHud Element");

                RectTransform rect = objins.GetComponent<RectTransform>();
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;

                createdObjects.Add(objins);
            }

            if (createdObjects.Count > 0)
            {
                Selection.objects = createdObjects.ToArray();
                XGUI_Utilitys.Console("XHud - 元素放置器通知", $"成功放置 {createdObjects.Count} 个元素到世界锚点", XGUIMsgState.确认);
            }
        }
        /// <summary>
        /// 校验当前选中对象是否为可放置的元素。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>选中对象必须为 GameObject 且挂载 <c>XHud_Module_Element</c>。</description></item>
        /// <item><description>供所有 <c>Validate_PlaceToAnchor_*</c> 方法统一调用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可放置返回 <c>true</c>。</returns>
        private static bool Validate_Selectable()
        {
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject == null)
                return false;
            return selectedObject.GetComponent<XHud_Module_Element>() != null;
        }
        #endregion

        #region 菜单 - 自动识别
        /// <summary>
        /// 自动识别放置菜单的可用性校验。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在选中对象含 <c>XHud_Module_Element</c> 时可用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可用返回 <c>true</c>。</returns>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /SendAuto(自动识别)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_Auto()
        {
            return Validate_Selectable();
        }
        /// <summary>
        /// 自动识别放置。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>读取元素的当前 RMS 布局数据并应用锚点/枢轴/位置/欧拉角/缩放。</description></item>
        /// <item><description>选中所有新创建的对象。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /SendAuto(自动识别)", priority = 500)]
        private static void PlaceToAnchor_Auto()
        {
            XHud_Manager manager = XHud_Dashboard.HudManagerGet();

            Object[] obj = Selection.objects;
            List<Object> CreatedObjs = new List<Object>();
            for (int i = 0; i < obj.Length; i++)
            {
                GameObject obj_ele = PrefabUtility.InstantiatePrefab(obj[i], null) as GameObject;
                Undo.RegisterCreatedObjectUndo(obj_ele, "CreateObjects");
                XHud_Module_Element ele = obj_ele.GetComponent<XHud_Module_Element>();
                Element_RMS_LayoutData rms = ele.elelemt_RMS_Get(manager.hm_RMS_GetCurrentSolution());
                ele.RectTransform.SetParent(manager.hm_ScreenElement_GetAnchored_RectTransform(rms.Anchor));
                ele.RectTransform.pivot = rms.Pivot;
                ele.RectTransform.anchorMin = rms.AnchorMin;
                ele.RectTransform.anchorMax = rms.AnchorMax;
                ele.RectTransform.anchoredPosition3D = rms.Position;
                ele.RectTransform.localEulerAngles = rms.Euler;
                ele.RectTransform.localScale = rms.Scale;

                CreatedObjs.Add(obj_ele);
            }
            Selection.objects = CreatedObjs.ToArray();
        }
        #endregion

        #region 菜单 - 顶层
        /// <summary>
        /// 顶层放置菜单的可用性校验。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在选中对象含 <c>XHud_Module_Element</c> 时可用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可用返回 <c>true</c>。</returns>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Top(顶层)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_T()
        {
            return Validate_Selectable();
        }
        /// <summary>
        /// 把选中对象放置到顶层锚点。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>调用 <c>SendObjects</c> 并传入 <c>XHudAnchor.顶层</c>。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Top(顶层)", priority = 500)]
        private static void PlaceToAnchor_T()
        {
            SendObjects(XHudAnchor.顶层);
        }
        #endregion

        #region 菜单 - 底层
        /// <summary>
        /// 底层放置菜单的可用性校验。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在选中对象含 <c>XHud_Module_Element</c> 时可用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可用返回 <c>true</c>。</returns>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Back(底层)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_B()
        {
            return Validate_Selectable();
        }
        /// <summary>
        /// 把选中对象放置到底层锚点。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>调用 <c>SendObjects</c> 并传入 <c>XHudAnchor.底层</c>。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Back(底层)", priority = 500)]
        private static void PlaceToAnchor_B()
        {
            SendObjects(XHudAnchor.底层);
        }
        #endregion

        #region 菜单 - 中心
        /// <summary>
        /// 中心放置菜单的可用性校验。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在选中对象含 <c>XHud_Module_Element</c> 时可用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可用返回 <c>true</c>。</returns>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Center(中心)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_C()
        {
            return Validate_Selectable();
        }
        /// <summary>
        /// 把选中对象放置到中心锚点。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>调用 <c>SendObjects</c> 并传入 <c>XHudAnchor.中心</c>。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Center(中心)", priority = 500)]
        private static void PlaceToAnchor_C()
        {
            SendObjects(XHudAnchor.中心);
        }
        #endregion

        #region 菜单 - 上
        /// <summary>
        /// 上锚点放置菜单的可用性校验。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在选中对象含 <c>XHud_Module_Element</c> 时可用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可用返回 <c>true</c>。</returns>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Up(上)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_U()
        {
            return Validate_Selectable();
        }
        /// <summary>
        /// 把选中对象放置到上锚点。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>调用 <c>SendObjects</c> 并传入 <c>XHudAnchor.上</c>。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Up(上)", priority = 500)]
        private static void PlaceToAnchor_U()
        {
            SendObjects(XHudAnchor.上);
        }
        #endregion

        #region 菜单 - 下
        /// <summary>
        /// 下锚点放置菜单的可用性校验。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在选中对象含 <c>XHud_Module_Element</c> 时可用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可用返回 <c>true</c>。</returns>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Down(下)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_D()
        {
            return Validate_Selectable();
        }
        /// <summary>
        /// 把选中对象放置到下锚点。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>调用 <c>SendObjects</c> 并传入 <c>XHudAnchor.下</c>。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Down(下)", priority = 500)]
        private static void PlaceToAnchor_D()
        {
            SendObjects(XHudAnchor.下);
        }
        #endregion

        #region 菜单 - 左
        /// <summary>
        /// 左锚点放置菜单的可用性校验。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在选中对象含 <c>XHud_Module_Element</c> 时可用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可用返回 <c>true</c>。</returns>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Left(左)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_L()
        {
            return Validate_Selectable();
        }
        /// <summary>
        /// 把选中对象放置到左锚点。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>调用 <c>SendObjects</c> 并传入 <c>XHudAnchor.左</c>。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Left(左)", priority = 500)]
        private static void PlaceToAnchor_L()
        {
            SendObjects(XHudAnchor.左);
        }
        #endregion

        #region 菜单 - 右
        /// <summary>
        /// 右锚点放置菜单的可用性校验。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在选中对象含 <c>XHud_Module_Element</c> 时可用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可用返回 <c>true</c>。</returns>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Right(右)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_R()
        {
            return Validate_Selectable();
        }
        /// <summary>
        /// 把选中对象放置到右锚点。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>调用 <c>SendObjects</c> 并传入 <c>XHudAnchor.右</c>。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Right(右)", priority = 500)]
        private static void PlaceToAnchor_R()
        {
            SendObjects(XHudAnchor.右);
        }
        #endregion

        #region 菜单 - 左上
        /// <summary>
        /// 左上锚点放置菜单的可用性校验。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在选中对象含 <c>XHud_Module_Element</c> 时可用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可用返回 <c>true</c>。</returns>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /LeftUp(左上)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_LU()
        {
            return Validate_Selectable();
        }
        /// <summary>
        /// 把选中对象放置到左上锚点。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>调用 <c>SendObjects</c> 并传入 <c>XHudAnchor.左上</c>。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /LeftUp(左上)", priority = 500)]
        private static void PlaceToAnchor_LU()
        {
            SendObjects(XHudAnchor.左上);
        }
        #endregion

        #region 菜单 - 左下
        /// <summary>
        /// 左下锚点放置菜单的可用性校验。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在选中对象含 <c>XHud_Module_Element</c> 时可用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可用返回 <c>true</c>。</returns>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /LeftDown(左下)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_LD()
        {
            return Validate_Selectable();
        }
        /// <summary>
        /// 把选中对象放置到左下锚点。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>调用 <c>SendObjects</c> 并传入 <c>XHudAnchor.左下</c>。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /LeftDown(左下)", priority = 500)]
        private static void PlaceToAnchor_LD()
        {
            SendObjects(XHudAnchor.左下);
        }
        #endregion

        #region 菜单 - 右上
        /// <summary>
        /// 右上锚点放置菜单的可用性校验。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在选中对象含 <c>XHud_Module_Element</c> 时可用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可用返回 <c>true</c>。</returns>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /RightUp(右上)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_RU()
        {
            return Validate_Selectable();
        }
        /// <summary>
        /// 把选中对象放置到右上锚点。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>调用 <c>SendObjects</c> 并传入 <c>XHudAnchor.右上</c>。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /RightUp(右上)", priority = 500)]
        private static void PlaceToAnchor_RU()
        {
            SendObjects(XHudAnchor.右上);
        }
        #endregion

        #region 菜单 - 右下
        /// <summary>
        /// 右下锚点放置菜单的可用性校验。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在选中对象含 <c>XHud_Module_Element</c> 时可用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可用返回 <c>true</c>。</returns>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /RightDown(右下)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_RD()
        {
            return Validate_Selectable();
        }
        /// <summary>
        /// 把选中对象放置到右下锚点。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>调用 <c>SendObjects</c> 并传入 <c>XHudAnchor.右下</c>。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /RightDown(右下)", priority = 500)]
        private static void PlaceToAnchor_RD()
        {
            SendObjects(XHudAnchor.右下);
        }
        #endregion

        #region 菜单 - 世界
        /// <summary>
        /// 世界锚点放置菜单的可用性校验。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>仅在选中对象含 <c>XHud_Module_Element</c> 时可用。</description></item>
        /// </list>
        /// </summary>
        /// <returns>可用返回 <c>true</c>。</returns>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /World(世界)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_World()
        {
            return Validate_Selectable();
        }
        /// <summary>
        /// 把选中对象放置到世界锚点。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>调用 <c>SendObjects_World</c>。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /World(世界)", priority = 500)]
        private static void PlaceToAnchor_World()
        {
            SendObjects_World();
        }
        #endregion
    }
}