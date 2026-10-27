/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGUI - Unity Editor界面可视化组件工具
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class Editor_XGUI_IconBrowser_Builtin : EditorWindow
    {
        #region 字段
        private List<string> m_allIcons = null;
        private List<string> m_filteredIcons = new List<string>();
        private List<List<string>> m_iconRows = new List<List<string>>();

        // 升级缓存版本，强制重扫
        private const string CacheKey = "XHud_BuiltinIconsCache_v3";

        private string m_searchText = string.Empty;
        private Vector2 m_scrollPos;

        private List<float> m_cachedItemHeights;
        private bool m_heightsDirty = true;

        private const int ColumnCount = 3;
        private const float ItemHeight = 34f;
        private const float ListTopReserved = 190;
        #endregion

        #region 菜单
        [MenuItem("Tools/XGUI/UnityBuiltinIconsGet Unity内建图标合集获取器")]
        private static void Init()
        {
            EditorWindow.GetWindow<Editor_XGUI_IconBrowser_Builtin>("Unity内建图标合集获取器")
                .minSize = new Vector2(500, 400);
        }
        #endregion

        #region 生命周期
        private void OnEnable()
        {
            LoadOrCollectIcons();
            RefreshFilter();
        }

        private void OnGUI()
        {
            float w = XGUI.GetCurrentWindowWidth();
            TextClipping clipping = XGUI.TryEllipsisClipping();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(15);
                using (new EditorGUILayout.VerticalScope())
                {
                    DrawHeader(w, clipping);
                    DrawToolbar(w, clipping);
                    DrawIconGrid(w, clipping);
                }
                GUILayout.Space(15);
            }
        }
        #endregion

        #region 绘制
        private void DrawHeader(float w, TextClipping clipping)
        {
            XGUI.layout_space(10);
            XGUI.layout_banner(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 32,
                icon: XGUI.GetBuiltInIcon("d_RawImage Icon"),
                icon_color: XGUI_Utilitys.HexString_To_Color("#3BFD9A"),
                title_text: "XHud  -  Unity 内建图标合集",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Bold,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping,
                bg_margin: new RectOffset(0, 0, 5, 5));
        }

        private void DrawToolbar(float w, TextClipping clipping)
        {
            XGUI.layout_space(20);
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                title: "搜索图标",
                title_text_color: XGUI_Utilitys.HexString_To_Color("#3BFD9A"),
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.black * 0.3f,
                margin: new RectOffset(0, 0, 5, 0),
                padding: new RectOffset(10, 10, 15, 15),
                absolute_margin: true,
                absolute_padding: true);

            EditorGUI.BeginChangeCheck();
            m_searchText = XGUI.layout_inputfield(
                prop: m_searchText,
                text_wrap: false,
                field_fontsize: XGUIFontSize.M,
                field_text_offset: Vector2.zero,
                field_height: 20,
                field_padding: new RectOffset(0, 0, 0, 0),
                field_margin: new RectOffset(0, 0, 5, 0),
                field_text_color: Color.white,
                title_width: 0,
                field_text_font: XGUI.GetFont("xg-medium"),
                field_text_style: FontStyle.Normal,
                field_text_anchor: TextAnchor.MiddleLeft);
            if (EditorGUI.EndChangeCheck())
                RefreshFilter();

            XGUI.layout_space(6);

            if (XGUI.layout_button(
                text: "重新扫描",
                tooltip: "清空缓存并重新扫描内建图标",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white,
                button_text_color: Color.black,
                press_fill: XGUIGroup.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.M,
                layout_width: 80,
                layout_min_width: 60,
                height: XGUI.GetSingleLineHeight(),
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                EditorPrefs.DeleteKey(CacheKey);
                CollectAndCacheIcons();
                RefreshFilter();
            }

            XGUI.layout_flexspace();

            string countText = $"显示 {m_filteredIcons.Count} / 共 {m_allIcons?.Count ?? 0}";
            XGUI.layout_label(
                text: countText,
                size: XGUIFontSize.M,
                text_color: Color.white * 0.8f,
                anchor: TextAnchor.MiddleRight);

            XGUI.layout_group_end(XGUIContainerType.Horizontal);
        }

        private void DrawIconGrid(float w, TextClipping clipping)
        {
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.black * 0.3f,
                title: "图标列表",
                title_size: XGUIFontSize.M,
                title_text_color: XGUI_Utilitys.HexString_To_Color("#3BFD9A"),
                title_clipping: clipping,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(0, 0, 20, 0),
                padding: new RectOffset(10, 10, 15, 15));

            if (m_iconRows.Count == 0)
            {
                XGUI.layout_label(
                    text: "没有匹配的图标。",
                    size: XGUIFontSize.M,
                    text_color: Color.gray,
                    anchor: TextAnchor.MiddleCenter,
                    margin: new RectOffset(0, 0, 10, 10));
            }
            else
            {
                float viewHeight = Mathf.Max(ItemHeight * 3, position.height - ListTopReserved);
                Rect viewRect = XGUI.GetControlRect(false, viewHeight);

                m_scrollPos = XGUI.gui_scrollview<List<string>>(
                    rect: viewRect,
                    scroll: m_scrollPos,
                    list: m_iconRows,
                    cachedItemHeights: ref m_cachedItemHeights,
                    heightsDirty: ref m_heightsDirty,
                    fixheight: ItemHeight,
                    onscroller: (rect, row, rowIndex) =>
                    {
                        float cellW = rect.width / ColumnCount;
                        for (int c = 0; c < row.Count; c++)
                        {
                            Rect cell = new Rect(
                                rect.x + c * cellW,
                                rect.y,
                                cellW - 2,
                                rect.height - 2);
                            DrawIconCell(cell, row[c], clipping);
                        }
                    });
            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);
        }

        private void DrawIconCell(Rect cell, string iconName, TextClipping clipping)
        {
            const float btnSize = 26f;
            float labelX = cell.x + btnSize + 6;

            Rect btnRect = new Rect(cell.x + 2, cell.y + (cell.height - btnSize) * 0.5f, btnSize, btnSize);
            Texture2D tex = EditorGUIUtility.IconContent(iconName).image as Texture2D;

            if (XGUI.gui_button(
                rect: btnRect,
                tooltip: iconName,
                tex_release: tex,
                tex_press: tex,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                width: btnSize,
                height: btnSize))
            {
                GUIUtility.systemCopyBuffer = iconName;
                ShowNotification(new GUIContent($"图标名称：{iconName} 已拷贝到剪贴板！"), 1f);
            }

            Rect labelRect = new Rect(labelX, cell.y, cell.xMax - labelX - 4, cell.height);
            XGUI.gui_label(
                rect: labelRect,
                text: new GUIContent(iconName),
                text_color: Color.white * 0.9f,
                size: XGUIFontSize.M,
                anchor: TextAnchor.MiddleLeft,
                clipping: clipping,
                font: XGUI.GetFont("xg-medium"));
        }
        #endregion

        #region 数据
        private void LoadOrCollectIcons()
        {
            string cached = EditorPrefs.GetString(CacheKey, string.Empty);
            if (!string.IsNullOrEmpty(cached))
            {
                m_allIcons = cached.Split('\n').Where(s => !string.IsNullOrEmpty(s)).ToList();
                return;
            }
            CollectAndCacheIcons();
        }

        private void CollectAndCacheIcons()
        {
            try
            {
                EditorUtility.DisplayProgressBar("扫描图标", "正在收集 Unity 内置图标...", 0.5f);
                m_allIcons = CollectAllBuiltinIcons();
                EditorPrefs.SetString(CacheKey, string.Join("\n", m_allIcons));
                m_heightsDirty = true;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void RefreshFilter()
        {
            m_filteredIcons.Clear();
            if (m_allIcons != null)
            {
                if (string.IsNullOrEmpty(m_searchText))
                {
                    m_filteredIcons.AddRange(m_allIcons);
                }
                else
                {
                    string kw = m_searchText.Trim().ToLowerInvariant();
                    foreach (var icon in m_allIcons)
                    {
                        if (icon.ToLowerInvariant().Contains(kw))
                            m_filteredIcons.Add(icon);
                    }
                }
            }

            m_iconRows.Clear();
            for (int i = 0; i < m_filteredIcons.Count; i += ColumnCount)
            {
                var row = new List<string>(ColumnCount);
                for (int c = 0; c < ColumnCount && i + c < m_filteredIcons.Count; c++)
                    row.Add(m_filteredIcons[i + c]);
                m_iconRows.Add(row);
            }

            m_heightsDirty = true;
            Repaint();
        }

        private static List<string> CollectAllBuiltinIcons()
        {
            var candidates = new HashSet<string>();

            // 1. 扫描持久化资源
            var allObjects = Resources.FindObjectsOfTypeAll<UnityEngine.Object>();
            foreach (var obj in allObjects)
            {
                if (obj == null || string.IsNullOrEmpty(obj.name)) continue;
                if (obj is Texture)
                {
                    candidates.Add(obj.name);
                    const string suffix = " Icon";
                    if (obj.name.EndsWith(suffix))
                        candidates.Add(obj.name.Substring(0, obj.name.Length - suffix.Length));
                }
            }

            // 2. 内置组件名补充
            foreach (var typeName in GetBuiltinComponentNames())
            {
                candidates.Add($"{typeName} Icon");
                if (typeName.Contains("Collider"))
                {
                    candidates.Add("Collider Icon");
                    candidates.Add("Collider2D Icon");
                }
            }

            // 3. 反射扫描所有 MonoBehaviour / UnityEngine 类
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in allAssemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type == null || type.IsAbstract || type.IsGenericType) continue;

                        // 过滤掉编译器生成的类 (包含 < > 或 + 号，这是引发异常的重灾区)
                        if (type.Name.Contains("<") || type.Name.Contains(">") || type.Name.Contains("+")) continue;

                        if (typeof(MonoBehaviour).IsAssignableFrom(type) ||
                            typeof(ScriptableObject).IsAssignableFrom(type) ||
                            type.Namespace != null && type.Namespace.StartsWith("UnityEngine"))
                        {
                            candidates.Add($"{type.Name} Icon");
                        }
                    }
                }
                catch { /* 忽略个别程序集异常 */ }
            }

            // 4. 常见特殊名称
            string[] extra = {
                "_Popup", "_Help", "SettingsIcon", "Search Icon", "Toolbar Plus", "Toolbar Minus",
                "animationvisibilitytoggleon", "animationvisibilitytoggleoff",
                "console.infoicon", "console.warnicon", "console.erroricon",
                "cs Script Icon", "js Script Icon", "dll Script Icon",
                "Folder Icon", "FolderEmpty Icon", "FolderFavorite Icon",
                "Prefab Icon", "PrefabNormal Icon", "PrefabModel Icon",
                "GameObject Icon", "Camera Icon", "Light Icon", "AudioSource Icon",
            };
            foreach (var n in extra) candidates.Add(n);

            // 5. 验证并过滤
            var result = new List<string>();
            foreach (var name in candidates)
            {
                if (IsIconContentValid(name))
                    result.Add(name);
            }
            result.Sort();
            return result;
        }

        private static HashSet<string> GetBuiltinComponentNames()
        {
            return new HashSet<string>
            {
                "CanvasRenderer", "Canvas", "CanvasGroup", "RectTransform", "Transform",
                "MeshRenderer", "SkinnedMeshRenderer", "SpriteRenderer", "ParticleSystemRenderer",
                "LineRenderer", "TrailRenderer", "SpriteMask",
                "BoxCollider", "SphereCollider", "CapsuleCollider", "MeshCollider", "TerrainCollider",
                "WheelCollider", "Cloth", "CharacterController", "CharacterJoint", "FixedJoint",
                "HingeJoint", "SpringJoint", "ConfigurableJoint", "ConstantForce",
                "BoxCollider2D", "CircleCollider2D", "CapsuleCollider2D", "PolygonCollider2D",
                "EdgeCollider2D", "CompositeCollider2D", "TilemapCollider2D",
                "Rigidbody", "Rigidbody2D",
                "Text", "Image", "RawImage", "Button", "Toggle", "Slider", "Scrollbar", "Dropdown",
                "InputField", "ScrollRect", "Mask", "RectMask2D", "LayoutElement",
                "HorizontalLayoutGroup", "VerticalLayoutGroup", "GridLayoutGroup",
                "Animator", "Animation", "AudioListener", "AudioSource", "AudioReverbZone",
                "ParticleSystem", "VisualEffect",
                "Light", "Camera", "ReflectionProbe", "Terrain", "NavMeshAgent", "NavMeshObstacle"
            };
        }

        /// <summary>
        /// 安全验证图标名称（防御非法字符崩溃、异常抛出、日志缺失）。
        /// </summary>
        private static bool IsIconContentValid(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;

            // 基础路径非法字符拦截
            if (name.IndexOfAny(new char[] { '<', '>', ':', '"', '/', '\\', '|', '?', '*' }) >= 0)
                return false;

            bool originalLogEnabled = Debug.unityLogger.logEnabled;
            Debug.unityLogger.logEnabled = false;

            try
            {
                var content = EditorGUIUtility.IconContent(name);
                return content != null && content.image != null;
            }
            catch
            {
                // 吞噬底层抛出的 ArgumentException（Illegal characters in path 等）
                return false;
            }
            finally
            {
                // 确保日志无论如何都会被恢复
                Debug.unityLogger.logEnabled = originalLogEnabled;
            }
        }
        #endregion
    }
}