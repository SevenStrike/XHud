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
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public partial class Editor_XHud_Manager : Editor
    {
        #region 绘制 - 音效池
        /// <summary>
        /// 绘制「音效池」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制音效池能力说明标签。</description></item>
        /// <item><description>绘制预加载数量字段。</description></item>
        /// <item><description>绘制可重排的音效池列表。</description></item>
        /// </list>
        /// </summary>
        private void Draw_SoundsPool()
        {
            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            string msg = $"音效池支持提供<b><color={hexcol}> 回环队列池化 音效 / 音乐 / 声音播放 </color></b>的能力，如果您的项目<b><color={hexcol}> 不涉及UI音效 </color></b>则可忽略该提示！并保持<b><color={hexcol}> 预加载数量为0 </color></b>";

            XGUI.layout_label(
                text: msg,
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                size: XGUIFontSize.S,
                anchor: TextAnchor.MiddleLeft,
                text_color: Color.white * 0.85f,
                offset: new Vector2(0, 0),
                padding: new RectOffset(10, 10, 10, 10),
                margin: new RectOffset(0, 0, 6, 0),
                clipping: TextClipping.Clip,
                wrap: true,
                font: XGUI.GetFont("xg-regular"),
                font_style: FontStyle.Normal);

            XGUI.layout_property_field(
             title: "预加载数量",
             title_size: XGUIFontSize.M,
             title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
             title_width: 100,
             prop: sp_pool_sounder_count,
             //status_icon: "icon_field_status",
             //status_icon_color: Lib_TextStyleLibrary.objectReferenceValue != null ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.black,
             prop_margin: new RectOffset(0, 0, 10, 10));

            list_soundpools.DoLayoutList();
        }
        #endregion

        #region 绘制 - 音效池列表
        /// <summary>
        /// 音效池列表 - 初始化。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>构造 <c>List_SoundsPool</c> 并绑定全部具名回调。</description></item>
        /// </list>
        /// </summary>
        private void Init_SoundsPoolList()
        {
            list_soundpools = new ReorderableList(serializedObject, sp_pool_sounder)
            {
                draggable = !Application.isPlaying,
                displayAdd = !Application.isPlaying,
                displayRemove = !Application.isPlaying,
                drawHeaderCallback = SoundsPool_DrawHeader,
                drawElementCallback = SoundsPool_DrawElement,
                elementHeightCallback = SoundsPool_ElementHeight,
                onAddCallback = SoundsPool_OnAdd,
                onRemoveCallback = SoundsPool_OnRemove,
                onSelectCallback = SoundsPool_OnSelect
            };
        }
        /// <summary>
        /// 音效池列表 - 绘制表头。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在列表顶部绘制「音效池列表」标签。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rect">绘制区域。</param>
        private void SoundsPool_DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "音效池列表");
        }
        /// <summary>
        /// 音效池列表 - 绘制元素。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制播放状态图标。</description></item>
        /// <item><description>绘制 Player 对象引用字段。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rect">绘制区域。</param>
        /// <param name="index">元素索引。</param>
        /// <param name="isActive">是否激活。</param>
        /// <param name="isFocused">是否聚焦。</param>
        private void SoundsPool_DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty sp_Soundlib = sp_pool_sounder.GetArrayElementAtIndex(index);
            SerializedProperty player = sp_Soundlib.FindPropertyRelative(nameof(AudioPlayer.Player));
            SerializedProperty isplaying = sp_Soundlib.FindPropertyRelative(nameof(AudioPlayer.IsPlaying));

            XGUI.gui_icon(
                   rect: new Rect(rect.x + 5, rect.y + 6, 14, 14),
                   icon: soundplaying,
                   color: isplaying.boolValue ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray);

            XGUI.gui_property_field(
                rect: new Rect(rect.x + 28, rect.y + 5, rect.width - 25, 20),
                title: new GUIContent(player.name),
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 80,
                prop: player);
        }
        /// <summary>
        /// 音效池列表 - 元素高度。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>返回 1.5 倍行高。</description></item>
        /// </list>
        /// </summary>
        /// <param name="index">元素索引。</param>
        /// <returns>元素高度。</returns>
        private float SoundsPool_ElementHeight(int index)
        {
            return 1.5f * line_height;
        }
        /// <summary>
        /// 音效池列表 - 添加回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在列表末尾插入一个空元素。</description></item>
        /// </list>
        /// </summary>
        /// <param name="list">列表实例。</param>
        private void SoundsPool_OnAdd(ReorderableList list)
        {
            sp_pool_sounder.InsertArrayElementAtIndex(list.count);
        }
        /// <summary>
        /// 音效池列表 - 移除回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>删除当前选中索引处的元素。</description></item>
        /// </list>
        /// </summary>
        /// <param name="list">列表实例。</param>
        private void SoundsPool_OnRemove(ReorderableList list)
        {
            sp_pool_sounder.DeleteArrayElementAtIndex(list.index);
        }
        /// <summary>
        /// 音效池列表 - 选中回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>当前无额外处理。</description></item>
        /// </list>
        /// </summary>
        /// <param name="list">列表实例。</param>
        private void SoundsPool_OnSelect(ReorderableList list)
        {

        }
        #endregion
    }
}