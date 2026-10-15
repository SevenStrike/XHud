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
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public partial class Editor_XHud_Manager : Editor
    {
        #region 绘制 - 元素库
        /// <summary>
        /// 绘制「元素库」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>库为空时显示错误帮助框。</description></item>
        /// <item><description>绘制可重排的元素库列表。</description></item>
        /// </list>
        /// </summary>
        private void Draw_ElementLibs()
        {
            if (Lib_ElementLibrarys.arraySize <= 0)
                XGUI.layout_helpbox(
                    state: XGUIHelboxState.错误,
                    title_text: "请至少指定一个元素库资源！",
                    title_size: XGUIFontSize.M,
                    title_style: FontStyle.Normal,
                    title_color: Color.white * 0.75f);

            List_ElementLib.DoLayoutList();
        }
        #endregion

        #region 绘制 - 元素库列表
        /// <summary>
        /// 元素库列表 - 初始化。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>构造 <c>List_ElementLib</c> 并绑定全部具名回调。</description></item>
        /// </list>
        /// </summary>
        private void Init_ElementLibList()
        {
            List_ElementLib = new ReorderableList(serializedObject, Lib_ElementLibrarys)
            {
                draggable = !Application.isPlaying,
                displayAdd = !Application.isPlaying,
                displayRemove = !Application.isPlaying,
                drawHeaderCallback = ElementLib_DrawHeader,
                drawElementCallback = ElementLib_DrawElement,
                elementHeightCallback = ElementLib_ElementHeight,
                onAddCallback = ElementLib_OnAdd,
                onRemoveCallback = ElementLib_OnRemove,
                onSelectCallback = ElementLib_OnSelect
            };
        }
        /// <summary>
        /// 元素库列表 - 绘制表头。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在列表顶部绘制「元素库列表」标签。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rect">绘制区域。</param>
        private void ElementLib_DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "元素库列表");
        }
        /// <summary>
        /// 元素库列表 - 绘制元素。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制元素库的对象引用字段。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rect">绘制区域。</param>
        /// <param name="index">元素索引。</param>
        /// <param name="isActive">是否激活。</param>
        /// <param name="isFocused">是否聚焦。</param>
        private void ElementLib_DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty sp_elementlib = Lib_ElementLibrarys.GetArrayElementAtIndex(index);

            XGUI.gui_property_field(
                rect: new Rect(rect.x, rect.y + 3, rect.width - 5, 20),
                title: null,
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: sp_elementlib);
        }
        /// <summary>
        /// 元素库列表 - 元素高度。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>返回 1.5 倍行高。</description></item>
        /// </list>
        /// </summary>
        /// <param name="index">元素索引。</param>
        /// <returns>元素高度。</returns>
        private float ElementLib_ElementHeight(int index)
        {
            return 1.5f * LineHeight;
        }
        /// <summary>
        /// 元素库列表 - 添加回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在列表末尾插入一个空元素。</description></item>
        /// </list>
        /// </summary>
        /// <param name="list">列表实例。</param>
        private void ElementLib_OnAdd(ReorderableList list)
        {
            Lib_ElementLibrarys.InsertArrayElementAtIndex(list.count);
        }
        /// <summary>
        /// 元素库列表 - 移除回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>删除当前选中索引处的元素。</description></item>
        /// </list>
        /// </summary>
        /// <param name="list">列表实例。</param>
        private void ElementLib_OnRemove(ReorderableList list)
        {
            Lib_ElementLibrarys.DeleteArrayElementAtIndex(list.index);
        }
        /// <summary>
        /// 元素库列表 - 选中回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>当前无额外处理。</description></item>
        /// </list>
        /// </summary>
        /// <param name="list">列表实例。</param>
        private void ElementLib_OnSelect(ReorderableList list)
        {

        }
        #endregion
    }
}