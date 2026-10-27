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
    using SevenStrikeModules.XTween;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using TMPro;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem.UI;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;
    using UnityEngine.UI;

    public partial class Editor_XHud_Manager : Editor
    {
        #region 绘制 - 工具
        /// <summary>
        /// 绘制「工具」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>提供「截屏工具」按钮，创建 <c>XHud_CameraCapture</c>。</description></item>
        /// <item><description>提供「创建效能面板」按钮，构建完整的 <c>XHud_PerformanceMonitor</c> UI 结构。</description></item>
        /// </list>
        /// </summary>
        private void Draw_Tool()
        {
            #region 截屏工具
            if (Application.isPlaying)
                XGUI.SetEnabled(false);

            if (XGUI.layout_button(
                text: "截屏工具",
                tooltip: "",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_color_gui: Color.white,
                button_text_color: Color.white,
                press_fill: XGUIGroup.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.gray,
                font_size: XGUIFontSize.M,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                height: 25,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                XHud_CameraCapture cc = FindFirstObjectByType<XHud_CameraCapture>();
                if (cc == null)
                {
                    GameObject obj_cc = new GameObject();
                    obj_cc.name = "CameraCapture";
                    obj_cc.AddComponent<XHud_CameraCapture>();
                }
                else
                {
                    EditorApplication.delayCall += () =>
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 管理器消息",
                            title: "创建截屏工具",
                            msg: "场景中已存在CameraCapture！",
                            ok: "选中",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            on_selected: (d) =>
                            {
                                if (d == "选中")
                                    Selection.activeGameObject = cc.gameObject;
                                return;
                            });
                    };
                }
                return;
            }
            XGUI.SetEnabled(true);
            #endregion

            XGUI.layout_space(5);

            #region 创建效能面板
            if (XGUI.layout_button(
                text: "创建效能面板",
                tooltip: "",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_color_gui: Color.white,
                button_text_color: Color.white,
                press_fill: XGUIGroup.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.gray,
                font_size: XGUIFontSize.M,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                height: 25,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                XHud_PerformanceMonitor cc = FindFirstObjectByType<XHud_PerformanceMonitor>();
                if (cc == null)
                {
                    #region  创建根物体
                    GameObject obj_cc = new GameObject();
                    obj_cc.layer = LayerMask.NameToLayer("XHud");
                    obj_cc.name = "PerformanceMonitor";
                    RectTransform rect = obj_cc.AddComponent<RectTransform>();
                    rect.SetParent(base_script.cv_screen.transform);
                    rect.anchoredPosition3D = Vector3.zero;
                    rect.sizeDelta = new Vector2(240, 500);
                    rect.localEulerAngles = Vector3.zero;
                    rect.localScale = Vector3.one;
                    XHud_PerformanceMonitor monitor = obj_cc.AddComponent<XHud_PerformanceMonitor>();
                    monitor.update_interval = 0.2f;
                    monitor.rect_margin = 20;
                    monitor.scale_ratio = 1f;
                    #endregion

                    #region  创建标题
                    GameObject obj_Header_Root = new GameObject();
                    obj_Header_Root.layer = LayerMask.NameToLayer("XHud");
                    obj_Header_Root.name = "Header";
                    RectTransform rect_header_root = obj_Header_Root.AddComponent<RectTransform>();
                    rect_header_root.SetParent(rect);
                    rect_header_root.sizeDelta = new Vector2(0, 35);
                    rect_header_root.anchorMin = new Vector2(0, 1);
                    rect_header_root.anchorMax = new Vector2(1, 1);
                    rect_header_root.pivot = new Vector2(0.5f, 0.5f);
                    rect_header_root.anchoredPosition3D = Vector3.zero;
                    rect_header_root.localEulerAngles = Vector3.zero;
                    rect_header_root.localScale = Vector3.one;
                    #endregion

                    #region  创建标题背景
                    GameObject obj_Header_Bg = new GameObject();
                    obj_Header_Bg.layer = LayerMask.NameToLayer("XHud");
                    obj_Header_Bg.name = "Bg";
                    RectTransform rect_header_bg = obj_Header_Bg.AddComponent<RectTransform>();
                    Image img_header_bg = obj_Header_Bg.AddComponent<Image>();
                    rect_header_bg.SetParent(rect_header_root);
                    rect_header_bg.sizeDelta = new Vector2(0, 0);
                    rect_header_bg.anchorMin = new Vector2(0, 0);
                    rect_header_bg.anchorMax = new Vector2(1, 1);
                    rect_header_bg.pivot = new Vector2(0.5f, 0.5f);
                    rect_header_bg.anchoredPosition3D = Vector3.zero;
                    rect_header_bg.localEulerAngles = Vector3.zero;
                    rect_header_bg.localScale = Vector3.one;
                    img_header_bg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_SPRITES_Path()}Others/BtnRect_Pure.png");
                    img_header_bg.raycastTarget = false;
                    img_header_bg.color = XHud_Dashboard.themes_get_color(ThemeColorType.primary);
                    img_header_bg.type = Image.Type.Sliced;
                    #endregion

                    #region  创建标题图标
                    GameObject obj_Header_Icon = new GameObject();
                    obj_Header_Icon.layer = LayerMask.NameToLayer("XHud");
                    obj_Header_Icon.name = "Icon";
                    RectTransform rect_header_icon = obj_Header_Icon.AddComponent<RectTransform>();
                    Image img_header_icon = obj_Header_Icon.AddComponent<Image>();
                    rect_header_icon.SetParent(rect);
                    rect_header_icon.sizeDelta = new Vector2(20, 20);
                    rect_header_icon.anchorMin = new Vector2(0, 1);
                    rect_header_icon.anchorMax = new Vector2(0, 1);
                    rect_header_icon.pivot = new Vector2(0.5f, 0.5f);
                    rect_header_icon.anchoredPosition3D = Vector3.right * 40;
                    rect_header_icon.localEulerAngles = Vector3.zero;
                    rect_header_icon.localScale = Vector3.one;
                    img_header_icon.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_SPRITES_Path()}Others/Eye.png");
                    img_header_icon.raycastTarget = false;
                    img_header_icon.color = Color.black;
                    img_header_icon.type = Image.Type.Simple;
                    #endregion

                    #region  创建标题文字
                    GameObject obj_HeaderText = new GameObject();
                    obj_HeaderText.layer = LayerMask.NameToLayer("XHud");
                    obj_HeaderText.name = "Title";
                    RectTransform rect_header_title = obj_HeaderText.AddComponent<RectTransform>();
                    XHud_Module_TmpText txt_header_title = obj_HeaderText.AddComponent<XHud_Module_TmpText>();
                    rect_header_title.SetParent(rect_header_bg);
                    rect_header_title.sizeDelta = new Vector2(0, 30);
                    rect_header_title.anchorMin = new Vector2(0, 0.5f);
                    rect_header_title.anchorMax = new Vector2(1, 0.5f);
                    rect_header_title.pivot = new Vector2(0.5f, 0.5f);
                    rect_header_title.anchoredPosition3D = Vector3.zero;
                    rect_header_title.localEulerAngles = Vector3.zero;
                    rect_header_title.localScale = Vector3.one;
                    txt_header_title.text_style_info.tmp_Set_FontAsset(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Tmp/sx_bold SDF.asset"));
                    txt_header_title.text_style_info.gen_RayCastSet(false);
                    txt_header_title.raycastTarget = false;
                    txt_header_title.color = Color.black;
                    txt_header_title.fontSize = 16;
                    txt_header_title.alignment = TextAlignmentOptions.Left;
                    txt_header_title.textWrappingMode = TextWrappingModes.NoWrap;
                    txt_header_title.overflowMode = TextOverflowModes.Truncate;
                    txt_header_title.fontStyle = FontStyles.Bold;
                    txt_header_title.margin = new Vector4(75, 0, 15, 0);
                    txt_header_title.text_style_info.tmp_Set_FontColor(Color.black);
                    txt_header_title.text_style_info.tmp_Set_FontSize(16);
                    txt_header_title.text_style_info.tmp_Set_Alignment(TmpContentAnchor.左);
                    txt_header_title.text_style_info.tmp_Set_WordWrappingMode(TextWrappingModes.NoWrap);
                    txt_header_title.text_style_info.tmp_Set_Overflow(TextOverflowModes.Truncate);
                    txt_header_title.text_style_info.tmp_Set_FontStyle(FontStyles.Bold);
                    txt_header_title.text_style_info.tmp_Set_MarginSet(new Vector4(75, 0, 15, 0));
                    txt_header_title.tmp_Set_Content("DataMonitor");
                    #endregion
                    monitor.title = txt_header_title;
                    #region  创建列表
                    GameObject obj_Header_List = new GameObject();
                    obj_Header_List.layer = LayerMask.NameToLayer("XHud");
                    obj_Header_List.name = "List";
                    RectTransform rect_header_list = obj_Header_List.AddComponent<RectTransform>();
                    rect_header_list.SetParent(rect);
                    rect_header_list.sizeDelta = new Vector2(0, 0);
                    rect_header_list.anchorMin = new Vector2(0, 0);
                    rect_header_list.anchorMax = new Vector2(1, 1);
                    rect_header_list.pivot = new Vector2(0.5f, 0.5f);
                    rect_header_list.anchoredPosition3D = Vector3.zero;
                    rect_header_list.offsetMin = new Vector2(0, 0);
                    rect_header_list.offsetMax = new Vector2(0, -32);
                    rect_header_list.localEulerAngles = Vector3.zero;
                    rect_header_list.localScale = Vector3.one;
                    ScrollRect rect_header_scroll = obj_Header_List.AddComponent<ScrollRect>();
                    rect_header_scroll.vertical = true;
                    rect_header_scroll.horizontal = false;
                    #endregion
                    monitor.scroll_rect = rect_header_scroll;
                    #region  创建列表可视区域
                    GameObject obj_Header_ListViewport = new GameObject();
                    obj_Header_ListViewport.layer = LayerMask.NameToLayer("XHud");
                    obj_Header_ListViewport.name = "Viewport";
                    RectTransform rect_header_listviewport = obj_Header_ListViewport.AddComponent<RectTransform>();
                    rect_header_listviewport.SetParent(rect_header_list);
                    rect_header_listviewport.sizeDelta = new Vector2(0, 0);
                    rect_header_listviewport.anchorMin = new Vector2(0, 0);
                    rect_header_listviewport.anchorMax = new Vector2(1, 1);
                    rect_header_listviewport.pivot = new Vector2(0.5f, 0.5f);
                    rect_header_listviewport.anchoredPosition3D = Vector3.zero;
                    rect_header_listviewport.localEulerAngles = Vector3.zero;
                    rect_header_listviewport.localScale = Vector3.one;
                    Image img_header_viewport = obj_Header_ListViewport.AddComponent<Image>();
                    Mask img_header_viewport_mask = obj_Header_ListViewport.AddComponent<Mask>();
                    img_header_viewport_mask.showMaskGraphic = false;
                    RectMask2D img_header_viewport_maskrect = obj_Header_ListViewport.AddComponent<RectMask2D>();
                    img_header_viewport_maskrect.softness = new Vector2Int(0, 245);
                    img_header_viewport_maskrect.padding = new Vector4(0, 0, 0, -122);
                    #endregion

                    #region  创建列表内容区域
                    GameObject obj_Header_ListContent = new GameObject();
                    obj_Header_ListContent.layer = LayerMask.NameToLayer("XHud");
                    obj_Header_ListContent.name = "Content";
                    RectTransform rect_header_listcontent = obj_Header_ListContent.AddComponent<RectTransform>();
                    rect_header_listcontent.SetParent(rect_header_listviewport);
                    rect_header_listcontent.sizeDelta = new Vector2(0, 0);
                    rect_header_listcontent.anchorMin = new Vector2(0, 0);
                    rect_header_listcontent.anchorMax = new Vector2(1, 1);
                    rect_header_listcontent.pivot = new Vector2(0.5f, 1f);
                    rect_header_listcontent.anchoredPosition3D = Vector3.zero;
                    rect_header_listcontent.localEulerAngles = Vector3.zero;
                    rect_header_listcontent.localScale = Vector3.one;
                    VerticalLayoutGroup img_header_list_content_vgp = obj_Header_ListContent.AddComponent<VerticalLayoutGroup>();
                    img_header_list_content_vgp.spacing = 3.6f;
                    img_header_list_content_vgp.childAlignment = TextAnchor.UpperLeft;
                    img_header_list_content_vgp.childControlWidth = true;
                    img_header_list_content_vgp.childForceExpandWidth = true;
                    img_header_list_content_vgp.childForceExpandHeight = false;
                    ContentSizeFitter header_content_sizefilter = obj_Header_ListContent.AddComponent<ContentSizeFitter>();
                    header_content_sizefilter.verticalFit = ContentSizeFitter.FitMode.MinSize;

                    rect_header_scroll.content = rect_header_listcontent;
                    rect_header_scroll.viewport = rect_header_listviewport;
                    #endregion

                    #region 创建列表内容节点
                    string[] node_names = new string[10] { "FPS", "CpuTime", "ScreenSize", "Resolution", "FreshRate", "Batches", "SetpassCalls", "Vertices", "Triangles", "DrawCalls" };

                    for (int i = 0; i < node_names.Length; i++)
                    {
                        GameObject obj_Header_ListNode = new GameObject();
                        obj_Header_ListNode.layer = LayerMask.NameToLayer("XHud");
                        obj_Header_ListNode.name = $"Node_{node_names[i]}";
                        RectTransform rect_monitornode = obj_Header_ListNode.AddComponent<RectTransform>();
                        rect_monitornode.SetParent(rect_header_listcontent);
                        rect_monitornode.sizeDelta = new Vector2(0, 60);
                        rect_monitornode.anchorMin = new Vector2(0, 1);
                        rect_monitornode.anchorMax = new Vector2(0, 1);
                        rect_monitornode.pivot = new Vector2(0.5f, 0.5f);
                        rect_monitornode.anchoredPosition3D = Vector3.zero;
                        rect_monitornode.localEulerAngles = Vector3.zero;
                        rect_monitornode.localScale = Vector3.one;

                        Image monitornode_bg = obj_Header_ListNode.AddComponent<Image>();
                        monitornode_bg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_SPRITES_Path()}Others/BtnRect_3Dot.png");
                        monitornode_bg.color = new Color(0.099f, 0.099f, 0.099f, 0.5960785f);
                        monitornode_bg.type = Image.Type.Sliced;
                        XHud_PerfomanceMonitor_Node monitornode = obj_Header_ListNode.AddComponent<XHud_PerfomanceMonitor_Node>();
                        monitornode.title = node_names[i];
                        monitornode.value = "-";

                        #region  创建标题文字
                        GameObject obj_NodeText_Title = new GameObject();
                        obj_NodeText_Title.layer = LayerMask.NameToLayer("XHud");
                        obj_NodeText_Title.name = "Title";
                        RectTransform rect_node_title = obj_NodeText_Title.AddComponent<RectTransform>();
                        XHud_Module_TmpText txt_node_title = obj_NodeText_Title.AddComponent<XHud_Module_TmpText>();
                        rect_node_title.SetParent(rect_monitornode);

                        rect_node_title.anchorMin = new Vector2(0, 0);
                        rect_node_title.anchorMax = new Vector2(1, 1);
                        rect_node_title.pivot = new Vector2(0.5f, 0.5f);
                        rect_node_title.anchoredPosition3D = new Vector3(0, 0, 0);  // Y位置
                        rect_node_title.offsetMin = new Vector2(20, 30);  // Left:20px
                        rect_node_title.offsetMax = new Vector2(-25, -10); // Right:50px
                        rect_node_title.localEulerAngles = Vector3.zero;
                        rect_node_title.localScale = Vector3.one;
                        txt_node_title.text_style_info.tmp_Set_FontAsset(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Tmp/sx_bold SDF.asset"));
                        txt_node_title.text_style_info.gen_RayCastSet(false);
                        txt_node_title.text_style_info.gen_Set_MaskableSet(true);
                        txt_node_title.maskable = true;
                        txt_node_title.raycastTarget = false;
                        txt_node_title.color = monitornode.col_title;
                        txt_node_title.fontSize = 14;
                        txt_node_title.alignment = TextAlignmentOptions.Left;
                        txt_node_title.textWrappingMode = TextWrappingModes.NoWrap;
                        txt_node_title.overflowMode = TextOverflowModes.Ellipsis;
                        txt_node_title.fontStyle = FontStyles.Normal;
                        txt_node_title.margin = new Vector4(0, 0, 0, 0);
                        txt_node_title.text_style_info.tmp_Set_FontColor(monitornode.col_title);
                        txt_node_title.text_style_info.tmp_Set_FontSize(14);
                        txt_node_title.text_style_info.tmp_Set_Alignment(TmpContentAnchor.左);
                        txt_node_title.text_style_info.tmp_Set_WordWrappingMode(TextWrappingModes.NoWrap);
                        txt_node_title.text_style_info.tmp_Set_Overflow(TextOverflowModes.Ellipsis);
                        txt_node_title.text_style_info.tmp_Set_FontStyle(FontStyles.Normal);
                        txt_node_title.text_style_info.tmp_Set_MarginSet(Vector4.zero);
                        txt_node_title.tmp_Set_Content(node_names[i]);
                        #endregion

                        #region  创建数值文字
                        GameObject obj_NodeText_Value = new GameObject();
                        obj_NodeText_Value.layer = LayerMask.NameToLayer("XHud");
                        obj_NodeText_Value.name = "Value";
                        RectTransform rect_node_value = obj_NodeText_Value.AddComponent<RectTransform>();
                        XHud_Module_TmpText txt_node_value = obj_NodeText_Value.AddComponent<XHud_Module_TmpText>();
                        rect_node_value.SetParent(rect_monitornode);
                        rect_node_value.anchorMin = new Vector2(0, 0);
                        rect_node_value.anchorMax = new Vector2(1, 1);
                        rect_node_value.pivot = new Vector2(0.5f, 0.5f);
                        rect_node_value.anchoredPosition3D = new Vector3(0, 0, 0);  // Y位置
                        rect_node_value.offsetMin = new Vector2(50, 10);  // Left:20px
                        rect_node_value.offsetMax = new Vector2(-20, -30); // Right:50px
                        rect_node_value.localEulerAngles = Vector3.zero;
                        rect_node_value.localScale = Vector3.one;
                        txt_node_value.text_style_info.tmp_Set_FontAsset(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Tmp/sx_regular SDF.asset"));
                        txt_node_value.text_style_info.gen_RayCastSet(false);
                        txt_node_value.text_style_info.gen_Set_MaskableSet(true);
                        txt_node_value.maskable = true;
                        txt_node_value.raycastTarget = false;
                        txt_node_value.color = monitornode.col_value;
                        txt_node_value.fontSize = 13;
                        txt_node_value.alignment = TextAlignmentOptions.Right;
                        txt_node_value.textWrappingMode = TextWrappingModes.NoWrap;
                        txt_node_value.overflowMode = TextOverflowModes.Ellipsis;
                        txt_node_value.fontStyle = FontStyles.Normal;
                        txt_node_value.margin = new Vector4(0, 0, 0, 0);
                        txt_node_value.text_style_info.tmp_Set_FontColor(monitornode.col_value);
                        txt_node_value.text_style_info.tmp_Set_FontSize(13);
                        txt_node_value.text_style_info.tmp_Set_Alignment(TmpContentAnchor.右);
                        txt_node_value.text_style_info.tmp_Set_WordWrappingMode(TextWrappingModes.NoWrap);
                        txt_node_value.text_style_info.tmp_Set_Overflow(TextOverflowModes.Ellipsis);
                        txt_node_value.text_style_info.tmp_Set_FontStyle(FontStyles.Normal);
                        txt_node_value.text_style_info.tmp_Set_MarginSet(Vector4.zero);
                        txt_node_value.tmp_Set_Content("-");
                        #endregion

                        monitornode.tmp_title = txt_node_title;
                        monitornode.tmp_value = txt_node_value;
                    }

                    GameObject obj_Header_ListNode_Null = new GameObject();
                    obj_Header_ListNode_Null.layer = LayerMask.NameToLayer("XHud");
                    obj_Header_ListNode_Null.name = $"NullSpace";
                    RectTransform rect_monitornode_null = obj_Header_ListNode_Null.AddComponent<RectTransform>();
                    rect_monitornode_null.SetParent(rect_header_listcontent);
                    rect_monitornode_null.sizeDelta = new Vector2(0, 60);
                    rect_monitornode_null.anchorMin = new Vector2(0, 1);
                    rect_monitornode_null.anchorMax = new Vector2(0, 1);
                    rect_monitornode_null.pivot = new Vector2(0.5f, 0.5f);
                    rect_monitornode_null.anchoredPosition3D = Vector3.zero;
                    rect_monitornode_null.localEulerAngles = Vector3.zero;
                    rect_monitornode_null.localScale = Vector3.one;
                    #endregion

                    Selection.activeGameObject = obj_cc.gameObject;
                }
                else
                {
                    EditorApplication.delayCall += () =>
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 管理器消息",
                            title: "创建效能面板",
                            msg: "场景中已存在效能面板！无需重复添加！",
                            ok: "选中",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            on_selected: (d) =>
                            {
                                if (d == "选中")
                                    Selection.activeGameObject = cc.gameObject;
                                return;
                            });
                    };
                }
                return;
            }
            #endregion
        }
        #endregion

        #region 工具 - 结构初始化
        /// <summary>
        /// 初始化 HudManager 的结构。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>校验 URP 渲染管线资源与渲染器列表。</description></item>
        /// <item><description>清空并重建 Cam / Canvas / Anchors / Mask / EventSystem 等节点。</description></item>
        /// <item><description>初始化比例参考图形信息、XTween 管理器与默认动效参数。</description></item>
        /// </list>
        /// </summary>
        private void xHud_StructureInitialize()
        {
            RenderPipelineAsset currentPipelineAsset = GraphicsSettings.currentRenderPipeline;
            if (currentPipelineAsset == null)
            {
                XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 管理器消息",
                    title: "渲染管线异常",
                    msg: "请先指定URP渲染管线资源后再试！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                return;
            }

            string[] renderlist = null;

            // 使用反射获取 m_RendererDataList
            FieldInfo fieldInfo = typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])fieldInfo.GetValue(currentPipelineAsset);
                renderlist = new string[rendererDataList.Length];
                for (int i = 0; i < renderlist.Length; i++)
                {
                    renderlist[i] = rendererDataList[i].name;
                }
            }
            else
            {
                XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 管理器消息",
                    title: "渲染管线异常",
                    msg: "请确认当前渲染管线资源文件的渲染器列表的有效性！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
            }

            EditorApplication.delayCall += () =>
            {
                if (initialized.boolValue)
                {
                    string repeat = XGUI.dialog(
                       type: XGUIDialogType.警告,
                       windowtitle: "XHud - 管理器消息",
                       title: "初始化XHud管理器",
                       msg: "您已经初始化了该XHud管理器，是否需要对其重新进行初始化？（我们并不推荐您这样做）",
                       ok: "覆盖",
                       cancel: "暂不",
                       PrimaryIndex: 0,
                       usemodal: true,
                       themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                       on_selected: (d) =>
                       {
                       });

                    if (repeat == "暂不")
                    {
                        return;
                    }
                }

                string res = XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 管理器消息",
                    title: "初始化XHud管理器",
                    msg: "接下来会自动创建HudManager的所有子结构组成，在这之前会先清空所有现存的子物体，请确定是否继续该操作？",
                    ok: "初始化",
                    cancel: "暂不",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    on_selected: (d) =>
                    {
                        if (d == "初始化")
                        {
                            #region 重建Manager的结构

                            #region  清空并重置所有组件
                            sp_cv_anchors_layout_screen.ClearArray();
                            sp_cv_anchors_layout_screen.serializedObject.ApplyModifiedProperties();

                            foreach (Transform child in base_script.transform)
                            {
                                DestroyImmediate(child.gameObject, true);
                            }

                            GameObject[] objs = new GameObject[base_script.transform.childCount];
                            for (int i = 0; i < objs.Length; i++)
                            {
                                objs[i] = base_script.transform.GetChild(i).gameObject;
                                objs[i].layer = LayerMask.NameToLayer("XHud");
                            }
                            for (int i = 0; i < objs.Length; i++)
                            {
                                DestroyImmediate(objs[i], true);
                            }

                            base_script.cv_anchors_layout_screen = null;
                            base_script.sf_framelines = null;
                            base_script.sf_seperaters = null;
                            base_script.sf_center_marks = null;

                            base_script.gameObject.name = "XHud";
                            base_script.sf_safeframe = null;
                            base_script.xtween_manager = null;
                            #endregion

                            #region Cam
                            GameObject obj_cam = new GameObject();
                            obj_cam.layer = LayerMask.NameToLayer("XHud");
                            Camera cam = obj_cam.AddComponent<Camera>();
                            cam.name = "Cam";
                            var uac = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
                            uac.renderPostProcessing = false;
                            cam.cullingMask = (1 << LayerMask.NameToLayer("XHud"));
                            UniversalAdditionalCameraData toOverlayData = cam.GetUniversalAdditionalCameraData();
                            toOverlayData.renderType = CameraRenderType.Overlay;

                            ///---设置相机渲染器
                            for (int i = 0; i < renderlist.Length; i++)
                            {
                                if (renderlist[i] == "Render_XHud")
                                {
                                    toOverlayData.SetRenderer(i);
                                }
                            }
                            cam.orthographic = sp_camera_othograpic_mode.boolValue;
                            cam.clearFlags = CameraClearFlags.SolidColor;
                            cam.transform.SetParent(base_script.transform);
                            cam.transform.localPosition = Vector3.zero;
                            sp_camera_ui.objectReferenceValue = cam;
                            sp_camera_ui.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region Canvas-Screen
                            GameObject obj_cav = new GameObject();
                            obj_cav.layer = LayerMask.NameToLayer("XHud");
                            Canvas cav = obj_cav.AddComponent<Canvas>();
                            cav.renderMode = RenderMode.ScreenSpaceCamera;
                            cav.worldCamera = cam;
                            obj_cav.name = "Screen";
                            obj_cav.transform.SetParent(base_script.transform);
                            obj_cav.transform.localPosition = Vector3.zero;
                            CanvasScaler scaler = obj_cav.AddComponent<CanvasScaler>();
                            obj_cav.AddComponent<GraphicRaycaster>();
                            cav.renderMode = RenderMode.ScreenSpaceCamera;
                            sp_cv_scaler_mode_index.intValue = 1;
                            sp_cv_scaler_mode_index.serializedObject.ApplyModifiedProperties();
                            switch (sp_cv_scaler_mode_index.intValue)
                            {
                                case 0:
                                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                                    break;
                                case 1:
                                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                                    break;
                                case 2:
                                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPhysicalSize;
                                    break;
                            }
                            sp_cv_scaler.objectReferenceValue = scaler;
                            cav.pixelPerfect = sp_cv_use_perfect_pixel_update.boolValue;
                            sp_cv_screen.objectReferenceValue = cav;
                            sp_cv_screen.serializedObject.ApplyModifiedProperties();

                            sp_cv_hudcanvas_anchor_index.intValue = 1;
                            sp_cv_hudcanvas_anchor_index.serializedObject.ApplyModifiedProperties();
                            sp_cv_hudcanvas_anchor.enumValueIndex = (int)CanvasCameraAttachment.CameraFar;
                            sp_cv_hudcanvas_anchor.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region Anchors
                            GameObject obj_Anchors = new GameObject();
                            obj_Anchors.layer = LayerMask.NameToLayer("XHud");
                            UnityEngine.RectTransform Anchors = obj_Anchors.AddComponent<UnityEngine.RectTransform>();
                            CanvasGroup cavgroup = obj_Anchors.AddComponent<CanvasGroup>();
                            sp_op_hud_canvas_group_screen.objectReferenceValue = cavgroup;
                            sp_op_hud_canvas_group_screen.serializedObject.ApplyModifiedProperties();
                            cavgroup.alpha = 1;
                            Anchors.name = "Anchors";
                            Anchors.SetParent(cav.transform);
                            Anchors.localPosition = Vector3.zero;
                            Anchors.localScale = Vector3.one;
                            Anchors.anchorMin = new Vector2(0, 0);
                            Anchors.anchorMax = new Vector2(1, 1);
                            Anchors.sizeDelta = new Vector2(0, 0);

                            sp_cv_hudcanvas_screen_anchor.objectReferenceValue = Anchors;
                            sp_cv_hudcanvas_screen_anchor.serializedObject.ApplyModifiedProperties();

                            for (int i = 0; i < 11; i++)
                            {
                                GameObject obj_Layanchor = new GameObject();
                                obj_Layanchor.layer = LayerMask.NameToLayer("XHud");
                                UnityEngine.RectTransform layanchor = obj_Layanchor.AddComponent<UnityEngine.RectTransform>();
                                layanchor.SetParent(Anchors);
                                if (i == 0)
                                {
                                    layanchor.name = "Anchor_B";
                                    layanchor.anchorMin = new Vector2(0f, 0f);
                                    layanchor.anchorMax = new Vector2(1f, 1f);
                                    layanchor.pivot = new Vector2(0.5f, 0.5f);
                                    layanchor.sizeDelta = new Vector2(0, 0);
                                }
                                if (i == 1)
                                {
                                    layanchor.name = "Anchor_U";
                                    layanchor.anchorMin = new Vector2(0.5f, 1);
                                    layanchor.anchorMax = new Vector2(0.5f, 1);
                                    layanchor.pivot = new Vector2(0.5f, 1);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 2)
                                {
                                    layanchor.name = "Anchor_D";
                                    layanchor.anchorMin = new Vector2(0.5f, 0);
                                    layanchor.anchorMax = new Vector2(0.5f, 0);
                                    layanchor.pivot = new Vector2(0.5f, 0);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 3)
                                {
                                    layanchor.name = "Anchor_L";
                                    layanchor.anchorMin = new Vector2(0, 0.5f);
                                    layanchor.anchorMax = new Vector2(0, 0.5f);
                                    layanchor.pivot = new Vector2(0, 0.5f);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 4)
                                {
                                    layanchor.name = "Anchor_R";
                                    layanchor.anchorMin = new Vector2(1, 0.5f);
                                    layanchor.anchorMax = new Vector2(1, 0.5f);
                                    layanchor.pivot = new Vector2(1, 0.5f);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 5)
                                {
                                    layanchor.name = "Anchor_C";
                                    layanchor.anchorMin = new Vector2(0.5f, 0.5f);
                                    layanchor.anchorMax = new Vector2(0.5f, 0.5f);
                                    layanchor.pivot = new Vector2(0.5f, 0.5f);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 6)
                                {
                                    layanchor.name = "Anchor_L_U";
                                    layanchor.anchorMin = new Vector2(0, 1);
                                    layanchor.anchorMax = new Vector2(0, 1);
                                    layanchor.pivot = new Vector2(0, 1);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 7)
                                {
                                    layanchor.name = "Anchor_L_D";
                                    layanchor.anchorMin = new Vector2(0, 0);
                                    layanchor.anchorMax = new Vector2(0, 0);
                                    layanchor.pivot = new Vector2(0, 0);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 8)
                                {
                                    layanchor.name = "Anchor_R_U";
                                    layanchor.anchorMin = new Vector2(1, 1);
                                    layanchor.anchorMax = new Vector2(1, 1);
                                    layanchor.pivot = new Vector2(1, 1);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 9)
                                {
                                    layanchor.name = "Anchor_R_D";
                                    layanchor.anchorMin = new Vector2(1, 0);
                                    layanchor.anchorMax = new Vector2(1, 0);
                                    layanchor.pivot = new Vector2(1, 0);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 10)
                                {
                                    layanchor.name = "Anchor_T";
                                    layanchor.anchorMin = new Vector2(0.5f, 0.5f);
                                    layanchor.anchorMax = new Vector2(0.5f, 0.5f);
                                    layanchor.pivot = new Vector2(0.5f, 0.5f);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                layanchor.anchoredPosition3D = Vector3.zero;
                                layanchor.localScale = Vector3.one;
                            }
                            #endregion

                            #region ScreenMask
                            GameObject obj_Mask = new GameObject();
                            obj_Mask.layer = LayerMask.NameToLayer("XHud");
                            UnityEngine.RectTransform m_Mask = obj_Mask.AddComponent<UnityEngine.RectTransform>();
                            Image Mask_img = obj_Mask.AddComponent<Image>();
                            Mask_img.color = Color.clear;
                            m_Mask.name = "Mask";
                            m_Mask.SetParent(cav.transform);
                            m_Mask.localPosition = Vector3.zero;
                            m_Mask.localScale = Vector3.one;
                            m_Mask.anchorMin = new Vector2(0, 0);
                            m_Mask.anchorMax = new Vector2(1, 1);
                            m_Mask.sizeDelta = new Vector2(0, 0);
                            sp_mk_mask.objectReferenceValue = Mask_img;
                            sp_mk_mask.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region EventSystem
                            GameObject obj_esys = new GameObject();
                            obj_esys.layer = LayerMask.NameToLayer("XHud");
                            EventSystem esys = obj_esys.AddComponent<EventSystem>();
                            InputSystemUIInputModule slm = obj_esys.AddComponent<InputSystemUIInputModule>();
                            obj_esys.name = "Event";
                            obj_esys.transform.SetParent(base_script.transform);
                            obj_esys.transform.localPosition = Vector3.zero;
                            sp_hud_event_system.objectReferenceValue = esys;
                            sp_hud_input_system_ui_input_module.objectReferenceValue = slm;
                            sp_hud_event_system.serializedObject.ApplyModifiedProperties();
                            sp_hud_input_system_ui_input_module.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region 获取所有锚点结构 - 布局
                            if (sp_cv_anchors_layout_screen != null && sp_cv_hudcanvas_screen_anchor != null)
                            {
                                Transform anchorRoot = sp_cv_hudcanvas_screen_anchor.objectReferenceValue as Transform;
                                for (int i = 0; i < anchorRoot.childCount; i++)
                                {
                                    if (anchorRoot.GetChild(i).name.Contains("Anchor"))
                                    {
                                        UnityEngine.RectTransform obj = anchorRoot.GetChild(i).GetComponent<UnityEngine.RectTransform>();
                                        sp_cv_anchors_layout_screen.InsertArrayElementAtIndex(sp_cv_anchors_layout_screen.arraySize);
                                        sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Anchor").objectReferenceValue = obj;
                                        sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue = obj.name;
                                        string Type = anchorRoot.GetChild(i).name;
                                        switch (Type)
                                        {
                                            case "B":
                                                sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 0;
                                                break;
                                            case "U":
                                                sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 1;
                                                break;
                                            case "D":
                                                sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 2;
                                                break;
                                            case "L":
                                                sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 3;
                                                break;
                                            case "R":
                                                sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 4;
                                                break;
                                            case "C":
                                                sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 5;
                                                break;
                                            case "L_U":
                                                sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 6;
                                                break;
                                            case "L_D":
                                                sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 7;
                                                break;
                                            case "R_U":
                                                sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 8;
                                                break;
                                            case "R_D":
                                                sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 9;
                                                break;
                                            case "T":
                                                sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 10;
                                                break;
                                        }

                                        sp_cv_anchors_layout_screen.serializedObject.ApplyModifiedProperties();
                                    }
                                    else
                                        continue;
                                }
                            }

                            #endregion

                            #region 比例参考图形信息初始化
                            sp_hmr_physics_screen_size.vector2Value = new Vector2(59.6f, 33.7f);
                            sp_hmr_physics_screen_size.serializedObject.ApplyModifiedProperties();

                            SerializedProperty sp_res_full_sprite = sp_hmr_res_full.FindPropertyRelative("sprite");
                            SerializedProperty sp_res_full_size = sp_hmr_res_full.FindPropertyRelative("size");
                            sp_res_full_sprite.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/HumanRefer/Body_Solid.png");
                            sp_res_full_size.vector2Value = new Vector2(47, 172);
                            sp_hmr_res_full.serializedObject.ApplyModifiedProperties();

                            SerializedProperty sp_res_part_sprite = sp_hmr_res_part.FindPropertyRelative("sprite");
                            SerializedProperty sp_res_part_size = sp_hmr_res_part.FindPropertyRelative("size");
                            sp_res_part_sprite.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/HumanRefer/Head_Solid.png");
                            sp_res_part_size.vector2Value = new Vector2(19, 30);
                            sp_hmr_res_part.serializedObject.ApplyModifiedProperties();

                            sp_hmr_reference_image_color.colorValue = new Color(0.6f, 0.6f, 0.6f, 0.05f);
                            sp_hmr_reference_image_color.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region 创建XTween动画管理器
                            XTween_Manager xTween_Manager = FindFirstObjectByType<XTween_Manager>();
                            if (xTween_Manager != null)
                                base_script.xtween_manager = xTween_Manager;
                            else
                            {
                                GameObject xtween_obj = new GameObject();
                                xtween_obj.name = "XTween(Manager)";
                                xtween_obj.layer = LayerMask.NameToLayer("XHud");
                                xtween_obj.transform.SetParent(null);
                                xtween_obj.transform.localPosition = Vector3.zero;
                                xtween_obj.transform.localEulerAngles = Vector3.zero;
                                xtween_obj.transform.localScale = Vector3.one;
                                xtween_obj.transform.SetAsLastSibling();
                                base_script.xtween_manager = xtween_obj.AddComponent<XTween_Manager>();
                            }
                            #endregion

                            #region 初始化动效
                            // 元素生成动效
                            base_script.global_motion_create_args.Reset();
                            // 元素回收动效
                            base_script.global_motion_recycle_args.Reset();
                            #endregion

                            #endregion

                            initialized.boolValue = true;
                            initialized.serializedObject.ApplyModifiedProperties();
                            return;
                        }
                    });
            };
        }
        #endregion

        #region 工具 - 图层
        /// <summary>
        /// 确保图层存在。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>扫描 TagManager 中的图层列表，若目标图层不存在则寻找空槽创建。</description></item>
        /// </list>
        /// </summary>
        /// <param name="layerName">图层名称。</param>
        private void xHud_EnsureLayerExists(string layerName)
        {
            // 获取 TagManager 资源
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            // 检查图层是否已经存在
            bool layerExists = false;
            for (int i = 0; i < layersProp.arraySize; i++)
            {
                SerializedProperty layerProp = layersProp.GetArrayElementAtIndex(i);
                if (layerProp.stringValue == layerName)
                {
                    layerExists = true;
                    break;
                }
            }

            // 如果图层不存在，则创建它
            if (!layerExists)
            {
                for (int i = 8; i < layersProp.arraySize; i++) // 从索引 8 开始，因为前 8 个是 Unity 预留的
                {
                    SerializedProperty layerProp = layersProp.GetArrayElementAtIndex(i);
                    if (layerProp.stringValue == "")
                    {
                        layerProp.stringValue = layerName;
                        tagManager.ApplyModifiedProperties();
                        //sp_DebugMode.Log($"Layer '{layerName}' has been created.");
                        return;
                    }
                }

                //sp_DebugMode.LogError($"No empty layer slots available to create layer '{layerName}'.");
            }
            else
            {
                //sp_DebugMode.Log($"Layer '{layerName}' already exists.");
            }
        }
        #endregion

        #region 工具 - 贴图排序
        /// <summary>
        /// 公共方法，用于对 List&lt;Texture2D&gt; 进行排序。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>按贴图名称中的数字后缀升序排序。</description></item>
        /// </list>
        /// </summary>
        /// <param name="textures">待排序的贴图列表。</param>
        public static void xHud_SortTexturesByNameSuffix(List<Texture2D> textures)
        {
            // 使用Sort方法，并传入自定义的比较器
            textures.Sort((tex1, tex2) =>
            {
                // 获取贴图名称
                string name1 = tex1.name;
                string name2 = tex2.name;

                // 提取名称中的数字后缀
                int num1 = xHud_ExtractNumberFromName(name1);
                int num2 = xHud_ExtractNumberFromName(name2);

                // 按照数字后缀进行比较
                return num1.CompareTo(num2);
            });
        }
        /// <summary>
        /// 从贴图名称中提取数字后缀。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>假设名称格式为 <c>前缀_数字</c>。</description></item>
        /// <item><description>格式不匹配时抛出 <c>ArgumentException</c>。</description></item>
        /// </list>
        /// </summary>
        /// <param name="name">贴图名称。</param>
        /// <returns>数字后缀。</returns>
        /// <exception cref="ArgumentException">名称格式不符合预期时抛出。</exception>
        private static int xHud_ExtractNumberFromName(string name)
        {
            // 假设名称格式为 "Ink_数字"
            string[] parts = name.Split('_');

            // 检查是否符合格式
            if (parts.Length != 2 || !int.TryParse(parts[1], out int number))
            {
                throw new ArgumentException($"Invalid texture name format: {name}");
            }

            return number;
        }
        #endregion
    }
}