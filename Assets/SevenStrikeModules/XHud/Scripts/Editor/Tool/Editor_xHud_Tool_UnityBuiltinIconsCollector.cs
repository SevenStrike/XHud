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
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// Unity 内建图标合集获取器。
    /// <para/>
    /// <list type="bullet">
    /// <item><description>扫描当前 Unity 编辑器可访问的全部 Texture2D，筛选出可作为图标使用的项。</description></item>
    /// <item><description>以网格形式列出图标，点击图标按钮即把图标名称拷贝到系统剪贴板。</description></item>
    /// <item><description>基于 UI Toolkit 构建界面。</description></item>
    /// </list>
    /// </summary>
    public class Editor_XHud_Tool_UnityBuiltinIconsCollector : EditorWindow
    {
        #region 字段
        /// <summary>
        /// 收集到的有效图标名称列表。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>Awake</c> 中扫描填充。</description></item>
        /// </list>
        /// </summary>
        List<string> m_Icons = null;
        #endregion

        #region 菜单
        /// <summary>
        /// 打开内建图标合集获取器窗口。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>菜单路径：<c>Tools/XHud/UnityBuiltinIconsGet</c>。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Tools/XHud/UnityBuiltinIconsGet")]
        private static void Init()
        {
            EditorWindow.GetWindow<Editor_XHud_Tool_UnityBuiltinIconsCollector>("Unity内建图标合集获取器");
        }
        #endregion

        #region 生命周期
        /// <summary>
        /// 窗口唤醒时构建界面。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>扫描全部 Texture2D，筛选出可作为图标的项。</description></item>
        /// <item><description>构建 ScrollView 容器并按行布局图标按钮与名称标签。</description></item>
        /// <item><description>点击图标按钮即把图标名拷贝到剪贴板并提示。</description></item>
        /// </list>
        /// </summary>
        private void Awake()
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
        #endregion
    }
}