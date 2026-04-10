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
    using TMPro;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;
    using FontStyles = TMPro.FontStyles;

    public static class Editor_XHud_MenuItemsAction_ModulesCreator
    {
        #region 创建管理器
        [MenuItem("GameObject/XHud/Manager (管理器)", priority = 0, validate = true)]
        private static bool Validate_Create_HudElement()
        {
            bool valid = false;

            if (Transform.FindFirstObjectByType<XHud_Manager>() == null)
                valid = true;

            return valid;
        }

        [MenuItem("GameObject/XHud/Manager (管理器)", priority = 0)]
        private static void Create_HudElement()
        {
            XHud_Manager manager = Transform.FindFirstObjectByType<XHud_Manager>();
            if (manager != null)
            {
                EditorUtility.DisplayDialog("警告", "场景中已经存在HudManager,请勿重复创建!", "明白");
                EditorGUIUtility.PingObject(manager);
                return;
            }
            GameObject obj = new GameObject();
            obj.transform.SetParent(Selection.activeTransform);
            obj.name = "XHud";
            obj.transform.localScale = Vector3.one;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localPosition = Vector3.zero;
            obj.AddComponent<XHud_Manager>();
            Selection.activeTransform = obj.transform;
        }
        #endregion

        #region 模组创建 - 元素
        [MenuItem("GameObject/XHud/Element (元素)", priority = 50, validate = true)]
        private static bool Validate_Create_HudElementModule()
        {
            bool valid = false;
            if (Selection.activeGameObject != null)
            {
                // 获取当前选中的对象
                GameObject selectedObject = Selection.activeGameObject;
                RectTransform rectTransform = selectedObject.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    if (!selectedObject.GetComponent<XHud_Module_Element>())
                        if (selectedObject.GetComponentInParent<Canvas>())
                        {
                            if (selectedObject.name.Contains("Anchor_"))
                            {
                                valid = true;
                            }
                            else if (selectedObject.transform.parent.name == "World" && selectedObject.name == "Anchors")
                            {
                                valid = true;
                            }
                        }
                }
            }
            return valid;
        }

        [MenuItem("GameObject/XHud/Element (元素)", priority = 50)]
        private static void Create_HudElementModule()
        {
            Transform actobj = Selection.activeTransform;
            if (actobj != null)
            {
                GameObject obj = new GameObject();
                Undo.RegisterCreatedObjectUndo(obj, "CreateElementObject");
                obj.name = "NewElement";
                obj.layer = LayerMask.NameToLayer("XHud");
                XHud_Module_Element obj_ele = obj.AddComponent<XHud_Module_Element>();
                obj_ele.Indicator = "NewElement";
                RectTransform obj_ele_rect = obj.AddComponent<RectTransform>();
                obj_ele_rect.transform.SetParent(actobj);
                obj_ele_rect.anchoredPosition3D = Vector3.zero;
                obj_ele_rect.localEulerAngles = Vector3.zero;
                obj_ele_rect.localScale = Vector3.one;

                Selection.activeTransform = obj.transform;
            }
        }
        #endregion

        #region 模组创建 - 图元控制器
        [MenuItem("GameObject/XHud/PrimitiveController (图元控制器)", priority = 50, validate = true)]
        private static bool Validate_Create_HudPrimitiveController()
        {
            bool valid = false;
            if (Selection.activeGameObject != null)
            {
                // 获取当前选中的对象
                GameObject selectedObject = Selection.activeGameObject;
                RectTransform rectTransform = selectedObject.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    if (selectedObject.GetComponentInParent<XHud_Module_Element>())
                        valid = true;
                }
            }
            return valid;
        }

        [MenuItem("GameObject/XHud/PrimitiveController (图元控制器)", priority = 50)]
        private static void Create_HudPrimitiveController()
        {
            Transform actobj = Selection.activeTransform;
            if (actobj != null)
            {
                GameObject obj = new GameObject();
                Undo.RegisterCreatedObjectUndo(obj, "CreatePrimitiveController");
                obj.layer = LayerMask.NameToLayer("XHud");
                RectTransform obj_ele_rect = obj.AddComponent<RectTransform>();
                obj_ele_rect.transform.SetParent(actobj);
                obj_ele_rect.anchoredPosition3D = Vector3.zero;
                obj_ele_rect.localEulerAngles = Vector3.zero;
                obj_ele_rect.localScale = Vector3.one;

                string res = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 创建模组消息", "创建图元控制器", "您需要为创建的 PrimitiveController 图元控制器指定一个基础类型！", "暂不", "RawImage", "Image", "Text", "TmpText", 2);

                if (res == "暂不")
                    return;

                if (res == "Image")
                {
                    obj.AddComponent<Image>();
                    obj.name = "PrimitiveController ( Image )";
                    obj_ele_rect.sizeDelta = new Vector2(100, 100);
                }
                else if (res == "RawImage")
                {
                    obj.AddComponent<RawImage>();
                    obj.name = "PrimitiveController ( RawImage )";
                    obj_ele_rect.sizeDelta = new Vector2(100, 100);
                }
                else if (res == "Text")
                {
                    //创建Text
                    XHud_Module_Text value = Mc_AddText(obj, "Text", Vector3.zero, Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light.ttf", new Vector2(120, 30), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), ContentAnchor.中心, TextAnchor.MiddleCenter, "XHud Text", 18, Color.white, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, false);
                    value.Indicator = "Text";
                    obj.name = "PrimitiveController ( Text )";
                }
                else if (res == "TmpText")
                {
                    //创建TmpText
                    XHud_Module_TmpText value = Mc_AddTmpText(obj, "TmpText", Vector3.zero, Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light SDF", new Vector2(120, 30), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), TmpContentAnchor.中心, TextAlignmentOptions.Center, "XHud Text", 18, Color.white, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, false);
                    value.Indicator = "TmpText";
                    obj.name = "PrimitiveController ( TmpText )";
                }

                XHud_Module_Primitive_Controller obj_con = obj.AddComponent<XHud_Module_Primitive_Controller>();
                obj_con.Indicator = "NewPrimitiveController";
                Selection.activeTransform = obj.transform;
            }
        }
        #endregion

        #region 模组创建 - 文字模块
        [MenuItem("GameObject/XHud/Text (文字模块)", priority = 50, validate = true)]
        private static bool Validate_Create_HudText()
        {
            bool valid = false;
            if (Selection.activeGameObject != null)
            {
                // 获取当前选中的对象
                GameObject selectedObject = Selection.activeGameObject;
                RectTransform rectTransform = selectedObject.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    if (selectedObject.GetComponentInParent<XHud_Module_Element>())
                        valid = true;
                }
            }
            return valid;
        }

        [MenuItem("GameObject/XHud/Text (文字模块)", priority = 50)]
        private static void Create_HudText()
        {
            Transform actobj = Selection.activeTransform;
            if (actobj != null)
            {
                //创建Text
                XHud_Module_Text value = Mc_AddText(actobj, "Text", Vector3.zero, Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light.ttf", new Vector2(120, 30), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), ContentAnchor.中心, TextAnchor.MiddleCenter, "XHud Text", 18, Color.white, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, false);
                value.Indicator = "Text";

                string res = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 创建模组消息", "创建图元控制器", "是否要为创建的Text创建一个图元控制器？", "暂不", "添加", 0);
                if (res == "添加")
                {
                    // 初始化图元控制器
                    XHud_Module_Primitive_Controller con = value.gameObject.AddComponent<XHud_Module_Primitive_Controller>();
                    // 初始化图元控制器
                    con.InitialComponents_For_Editor();
                    // 初始配色同步
                    con.pt_Painting.OriginalColor = value.TextStyleInfo.tmp_color;
                }

                Selection.activeTransform = value.transform;
            }
        }
        #endregion

        #region 模组创建 - Tmp文字模块
        [MenuItem("GameObject/XHud/TmpText (Tmp文字模块)", priority = 50, validate = true)]
        private static bool Validate_Create_HudTmpText()
        {
            bool valid = false;
            if (Selection.activeGameObject != null)
            {
                // 获取当前选中的对象
                GameObject selectedObject = Selection.activeGameObject;
                RectTransform rectTransform = selectedObject.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    if (selectedObject.GetComponentInParent<XHud_Module_Element>())
                        valid = true;
                }
            }
            return valid;
        }

        [MenuItem("GameObject/XHud/TmpText (Tmp文字模块)", priority = 50)]
        private static void Create_HudTmpText()
        {
            Transform actobj = Selection.activeTransform;
            if (actobj != null)
            {
                //创建TmpText
                XHud_Module_TmpText value = Mc_AddTmpText(actobj, "TmpText", Vector3.zero, Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light SDF", new Vector2(120, 30), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), TmpContentAnchor.中心, TextAlignmentOptions.Center, "XHud Text", 18, Color.white, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, false);
                value.Indicator = "TmpText";

                string res = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 创建模组消息", "创建图元控制器", "是否要为创建的TmpText创建一个图元控制器？", "暂不", "添加", 0);
                if (res == "添加")
                {
                    // 初始化图元控制器
                    XHud_Module_Primitive_Controller con = value.gameObject.AddComponent<XHud_Module_Primitive_Controller>();
                    // 初始化图元控制器
                    con.InitialComponents_For_Editor();
                    // 初始配色同步
                    con.pt_Painting.OriginalColor = value.TextStyleInfo.tmp_color;
                }

                Selection.activeTransform = value.transform;
            }
        }
        #endregion

        #region 模组创建 - 按钮
        [MenuItem("GameObject/XHud/Module（模组）/Button (按钮)", priority = 1000, validate = true)]
        private static bool Validate_Create_HudButton()
        {
            bool valid = false;

            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject != null)
            {
                if (!selectedObject.GetComponentInParent<XHud_Module_Element>() && selectedObject.name.Contains("Anchor_"))
                    valid = true;
            }

            return valid;
        }

        [MenuItem("GameObject/XHud/Module（模组）/Button (按钮)", priority = 1000)]
        private static void Create_HudButton()
        {
            string res = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 创建模组消息", "选择按钮风格", "您想创建什么样风格的按钮？", "文字", "图标", "图标 & 文字", 0);

            #region 创建Hud元素
            GameObject obj = Mc_CreateObject("Element(Button)", "XHud", Vector3.zero, Vector3.zero, Vector3.one, Selection.activeTransform);
            XHud_Module_Element btn_ele = Mc_AddHudElement(obj);
            if (res == "图标")
                btn_ele.RectTransform = Mc_AddRectTransform(obj, new Vector2(45, 45));
            else if (res == "文字")
                btn_ele.RectTransform = Mc_AddRectTransform(obj, new Vector2(128, 45));
            else if (res == "图标 & 文字")
                btn_ele.RectTransform = Mc_AddRectTransform(obj, new Vector2(160, 45));
            #endregion

            #region 创建Button根物体
            GameObject btn_root = Mc_CreateObject("Button", "XHud", Vector3.zero, Vector3.zero, Vector3.one, btn_ele.RectTransform);
            RectTransform btn_root_rect = Mc_AddRectTransform(btn_root, new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(0, 0));
            #endregion

            #region 创建Button
            XHud_Module_Button btn = Mc_AddHudButton(btn_root, "Button", "XHud-Button");
            ColorBlock colors = btn.colors;
            colors.pressedColor = XHud_Dashboard.Theme_Primary;
            btn.colors = colors;
            #endregion

            if (res == "图标")
            {
                #region 创建Button - 图标
                GameObject btn_icon = Mc_CreateObject("Icon", "XHud", Vector3.zero, Vector3.zero, Vector3.one, btn_root_rect);
                Sprite btn_icon_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Icon_Objective.png");
                Image btn_img_icon = Mc_AddImage(btn_icon, Color.white, new Vector2(35, 35), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), true);
                btn_img_icon.sprite = btn_icon_sprite;
                btn.IconImage = btn_img_icon;
                #endregion

                btn.IconColorSyncFade = true;
            }
            else
            {
                string res_bg = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 创建模组消息", "选择背景类型", "您希望使用那种背景方式作为按钮背景使用？", "有背景", "无背景", 1);

                if (res_bg == "有背景")
                {
                    #region 创建Button - 背景
                    GameObject btn_bg = Mc_CreateObject("Bg", "XHud", Vector3.zero, Vector3.zero, Vector3.one, btn_root_rect);
                    Sprite btn_bg_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/BtnRect_Wire_CrossSlice.png");
                    Image btn_img_bg = Mc_AddImage(btn_bg, Color.white, new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(0, 0), true);
                    btn_img_bg.sprite = btn_bg_sprite;
                    btn_img_bg.type = Image.Type.Sliced;
                    btn.BgImage = btn_img_bg;
                    btn.BgColorSyncFade = true;
                    #endregion
                }

                string res_text = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 创建模组消息", "选择文字类型", "您希望使用那种文字组件类型作为按钮文字使用？", "Text", "TmpText", 1);

                if (res == "文字")
                {
                    if (res_text == "Text")
                    {
                        //创建Slider - 按钮文字
                        XHud_Module_Text btn_text = Mc_AddText(btn_root_rect, "Text", Vector3.zero, Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light.ttf", new Vector2(128, 35), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), ContentAnchor.中心, TextAnchor.MiddleCenter, btn.ButtonName, 13, Color.white, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, true);
                        btn.ButtonText = btn_text;
                    }
                    else if (res_text == "TmpText")
                    {
                        //创建Slider - 按钮文字
                        XHud_Module_TmpText btn_tmptext = Mc_AddTmpText(btn_root_rect, "Text", Vector3.zero, Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light SDF", new Vector2(128, 35), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), TmpContentAnchor.中心, TextAlignmentOptions.Center, $"{btn.ButtonName}", 13, Color.white, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, true);
                        btn.ButtonTmpText = btn_tmptext;
                    }
                    btn.TextColorSyncFade = true;
                }
                else if (res == "图标 & 文字")
                {
                    #region 创建Button - 图标
                    GameObject btn_icon = Mc_CreateObject("Icon", "XHud", new Vector3(23, 0, 0), Vector3.zero, Vector3.one, btn_root_rect);
                    Sprite btn_icon_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Icon_Objective.png");
                    Image btn_img_icon = Mc_AddImage(btn_icon, Color.white, new Vector2(16, 16), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), true);
                    btn_img_icon.sprite = btn_icon_sprite;
                    btn.IconImage = btn_img_icon;
                    #endregion

                    if (res_text == "Text")
                    {
                        //创建Slider - 按钮文字
                        XHud_Module_Text btn_text = Mc_AddText(btn_root_rect, "Text", new Vector3(-56, 0, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light.ttf", new Vector2(80, 25), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), ContentAnchor.右, TextAnchor.MiddleRight, btn.ButtonName, 13, Color.white, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, true);
                        btn.ButtonText = btn_text;
                    }
                    else if (res_text == "TmpText")
                    {
                        //创建Slider - 按钮文字
                        XHud_Module_TmpText btn_tmptext = Mc_AddTmpText(btn_root_rect, "Text", new Vector3(-56, 0, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light SDF", new Vector2(80, 25), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), TmpContentAnchor.右, TextAlignmentOptions.Right, $"{btn.ButtonName}", 13, Color.white, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, true);
                        btn.ButtonTmpText = btn_tmptext;
                    }

                    btn.TextColorSyncFade = true;
                    btn.IconColorSyncFade = true;
                }
            }
            SceneView.RepaintAll();
            Selection.activeTransform = obj.transform;
        }
        #endregion

        #region 模组创建 - 进度条
        [MenuItem("GameObject/XHud/Module（模组）/Progress (进度条)", priority = 1000, validate = true)]
        private static bool Validate_Create_HudProgress()
        {
            bool valid = false;

            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject != null)
            {
                if (selectedObject.GetComponentInParent<XHud_Manager>() && selectedObject.name.Contains("Anchor_"))
                    valid = true;
            }

            return valid;
        }

        [MenuItem("GameObject/XHud/Module（模组）/Progress (进度条)", priority = 1000)]
        private static void Create_HudProgress()
        {
            string res = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 创建模组消息", "选择进度条类型", "您希望进度条的标题、副标题以及数值文字使用哪种文字组件类型？", "Text", "TmpText", 1);

            #region 创建Hud元素
            GameObject obj = Mc_CreateObject("Element(Progress)", "XHud", Vector3.zero, Vector3.zero, Vector3.one, Selection.activeTransform);
            XHud_Module_Element pro_ele = Mc_AddHudElement(obj);
            pro_ele.RectTransform = Mc_AddRectTransform(obj, new Vector2(300, 70));
            #endregion

            #region 创建Progress根物体
            GameObject pro_root = Mc_CreateObject("Progress", "XHud", Vector3.zero, Vector3.zero, Vector3.one, pro_ele.RectTransform);
            RectTransform pro_root_rect = Mc_AddRectTransform(pro_root, new Vector2(300, 70));
            #endregion

            #region 创建Progress
            XHud_Module_Progress pro = Mc_AddHudProgress(pro_root, "Progress", 0.0f, "%", "XHud-Progress", "Progress Subtitle", 1);
            #endregion

            #region 文字组件创建
            if (res == "Text")
            {
                //创建Slider - 数值
                XHud_Module_Text value = Mc_AddText(pro_root_rect, "Percentage", new Vector3(-35, 28, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light.ttf", new Vector2(70, 25), new Vector2(1, 0), new Vector2(1, 0), new Vector2(0.5f, 0.5f), new Vector2(0, 0), ContentAnchor.右, TextAnchor.MiddleRight, $"{pro.ProgressValue.ToString("F1")} %", 12, Color.white, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, false);

                //创建Slider - 标题
                XHud_Module_Text title = Mc_AddText(pro_root_rect, "Title", new Vector3(115, -12.5f, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Bold.ttf", new Vector2(130, 25), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), ContentAnchor.左, TextAnchor.MiddleLeft, pro.con_title, 18, XHud_Dashboard.Theme_Primary, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, false);

                //创建Slider - 副标题
                XHud_Module_Text subtitle = Mc_AddText(pro_root_rect, "Subtitle", new Vector3(115, -38.5f, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light.ttf", new Vector2(130, 25), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), ContentAnchor.左, TextAnchor.MiddleLeft, pro.con_subtitle, 12, Color.white * 0.65f, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, false);

                pro.pro_Text_Percent = value;
                pro.pro_Text_Title = title;
                pro.pro_Text_Subtitle = subtitle;
            }
            else
            {
                //创建Slider - 数值
                XHud_Module_TmpText value = Mc_AddTmpText(pro_root_rect, "Percentage", new Vector3(-35, 28, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light SDF", new Vector2(70, 25), new Vector2(1, 0), new Vector2(1, 0), new Vector2(0.5f, 0.5f), new Vector2(0, 0), TmpContentAnchor.右, TextAlignmentOptions.Right, $"{pro.ProgressValue.ToString("F1")} %", 12, Color.white, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, false);

                //创建Slider - 标题
                XHud_Module_TmpText title = Mc_AddTmpText(pro_root_rect, "Title", new Vector3(115, -12.5f, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Bold SDF", new Vector2(130, 25), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), TmpContentAnchor.左, TextAlignmentOptions.Left, $"{pro.con_title}", 18, XHud_Dashboard.Theme_Primary, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, false);

                //创建Slider - 副标题
                XHud_Module_TmpText subtitle = Mc_AddTmpText(pro_root_rect, "Subtitle", new Vector3(115, -38.5f, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light SDF", new Vector2(130, 25), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), TmpContentAnchor.左, TextAlignmentOptions.Left, $"{pro.con_subtitle}", 12, Color.white * 0.65f, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, false);

                pro.pro_TmpText_Percent = value;
                pro.pro_TmpText_Title = title;
                pro.pro_TmpText_Subtitle = subtitle;
            }
            #endregion

            #region 创建Progress - 图标
            GameObject pro_icon = Mc_CreateObject("Icon", "XHud", new Vector3(15, -17.3f, 0), Vector3.zero, Vector3.one, pro_root_rect);
            Sprite pro_icon_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Icon_XHud.png");
            Image pro_img_icon = Mc_AddImage(pro_icon, Color.white, new Vector2(30, 30), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), false);
            pro_img_icon.sprite = pro_icon_sprite;
            #endregion

            #region 创建Progress - 轨道根物体
            GameObject pro_structure = Mc_CreateObject("Structure", "XHud", Vector3.zero, Vector3.zero, Vector3.one, pro_root_rect);
            RectTransform pro_structure_rect = Mc_AddRectTransform(pro_structure, new Vector2(0, 10), new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0.5f), new Vector2(5, 0), new Vector2(0, 0));
            pro_structure_rect.sizeDelta = new Vector2(pro_structure_rect.sizeDelta.x, 10);
            pro_structure_rect.offsetMin = new Vector2(5, pro_structure_rect.offsetMin.y);
            pro_structure_rect.offsetMax = new Vector2(-5, pro_structure_rect.offsetMax.y);
            #endregion

            #region 创建Progress - 轨道区域 - 背景
            GameObject pro_structure_bg = Mc_CreateObject("Bg", "XHud", Vector3.zero, Vector3.zero, Vector3.one, pro_structure_rect);
            Image pro_img_bg = Mc_AddImage(pro_structure_bg, Color.gray, new Vector2(0, 1), new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f), false);
            #endregion

            #region 创建Progress - 轨道区域 - 前景
            GameObject pro_structure_fg = Mc_CreateObject("Fg", "XHud", Vector3.zero, Vector3.zero, Vector3.one, pro_structure_rect);
            Image pro_img_fg = Mc_AddImage(pro_structure_fg, XHud_Dashboard.Theme_Primary, new Vector2(0, 1), new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f), false);
            #endregion

            #region 创建Progress - 进度Mark
            GameObject pro_mark = Mc_CreateObject("Mark", "XHud", Vector3.zero, Vector3.zero, Vector3.one, pro_structure_fg.transform);
            Sprite pro_mark_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Progress_Indicator.png");
            Image pro_mark_icon = Mc_AddImage(pro_mark, Color.white, new Vector2(10, 4), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f), false);
            pro_mark_icon.sprite = pro_mark_sprite;
            #endregion

            pro.OrbitWrapper = pro_structure_rect;
            pro.pro_Bg = pro_img_bg;
            pro.pro_Fore = pro_img_fg;
            pro.pro_Icon = pro_img_icon;
            pro.pro_Handle = pro_mark_icon;

            SceneView.RepaintAll();
            Selection.activeTransform = obj.transform;
        }
        #endregion

        #region 模组创建 - 滑动条
        [MenuItem("GameObject/XHud/Module（模组）/Slider (滑动条)", priority = 1000, validate = true)]
        private static bool Validate_Create_HudSlider()
        {
            bool valid = false;

            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject != null)
            {
                if (selectedObject.GetComponentInParent<XHud_Manager>() && selectedObject.name.Contains("Anchor_"))
                    valid = true;
            }

            return valid;
        }

        [MenuItem("GameObject/XHud/Module（模组）/Slider (滑动条)", priority = 1000)]
        private static void Create_HudSlider()
        {
            string res = Editor_XHud_GUI.Open(XHud_DialogType.通知, "XHud - 创建模组消息", "选择滑动条类型", "您希望滑动条的标题、副标题以及数值文字使用哪种文字组件类型？", "Text", "TmpText", 1);

            #region 创建Hud元素
            GameObject obj = Mc_CreateObject("Element(Slider)", "XHud", Vector3.zero, Vector3.zero, Vector3.one, Selection.activeTransform);
            XHud_Module_Element sli_ele = Mc_AddHudElement(obj);
            sli_ele.RectTransform = Mc_AddRectTransform(obj, new Vector2(300, 70));
            #endregion

            #region 创建Slider根物体
            GameObject sli_root = Mc_CreateObject("Slider", "XHud", Vector3.zero, Vector3.zero, Vector3.one, sli_ele.RectTransform);
            RectTransform sli_root_rect = Mc_AddRectTransform(sli_root, new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(0, 0));

            #endregion

            #region 创建Slider
            XHud_Module_Slider sli = Mc_AddHudSlider(sli_root, "Slider", 0, 100, 0.0f, "%", "XHud-Slider", "Slider Subtitle", 1);
            #endregion

            #region 文字组件创建
            if (res == "Text")
            {
                //创建Slider - 数值
                XHud_Module_Text value = Mc_AddText(sli_root_rect, "Percentage", Vector3.zero, Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light.ttf", new Vector2(50, 25), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0.5f), new Vector2(0, 2), ContentAnchor.右, TextAnchor.MiddleRight, $"{sli.value.ToString("F1")} %", 12, Color.white, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, false);

                //创建Slider - 标题
                XHud_Module_Text title = Mc_AddText(sli_root_rect, "Title", new Vector3(115, -12.5f, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Bold.ttf", new Vector2(130, 25), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), ContentAnchor.左, TextAnchor.MiddleLeft, sli.con_title, 18, XHud_Dashboard.Theme_Primary, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, false);

                //创建Slider - 副标题
                XHud_Module_Text subtitle = Mc_AddText(sli_root_rect, "Subtitle", new Vector3(115, -38.5f, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light.ttf", new Vector2(130, 25), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), ContentAnchor.左, TextAnchor.MiddleLeft, sli.con_subtitle, 12, Color.white * 0.65f, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, false);

                sli.sli_Text_Percent = value;
                sli.sli_Text_Title = title;
                sli.sli_Text_Subtitle = subtitle;
            }
            else
            {
                //创建Slider - 数值
                XHud_Module_TmpText value = Mc_AddTmpText(sli_root_rect, "Percentage", Vector3.zero, Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light SDF", new Vector2(50, 25), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0.5f), new Vector2(0, 2), TmpContentAnchor.右, TextAlignmentOptions.Right, $"{sli.value.ToString("F1")} %", 12, Color.white, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, false);

                //创建Slider - 标题
                XHud_Module_TmpText title = Mc_AddTmpText(sli_root_rect, "Title", new Vector3(115, -12.5f, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Bold SDF", new Vector2(130, 25), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), TmpContentAnchor.左, TextAlignmentOptions.Left, $"{sli.con_title}", 18, XHud_Dashboard.Theme_Primary, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, false);

                //创建Slider - 副标题
                XHud_Module_TmpText subtitle = Mc_AddTmpText(sli_root_rect, "Subtitle", new Vector3(115, -38.5f, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light SDF", new Vector2(130, 25), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), TmpContentAnchor.左, TextAlignmentOptions.Left, $"{sli.con_subtitle}", 12, Color.white * 0.65f, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, false);

                sli.sli_TmpText_Percent = value;
                sli.sli_TmpText_Title = title;
                sli.sli_TmpText_Subtitle = subtitle;
            }
            #endregion

            #region 创建Slider - 图标
            GameObject sli_icon = Mc_CreateObject("Icon", "XHud", new Vector3(15, -17.3f, 0), Vector3.zero, Vector3.one, sli_root_rect);
            Sprite sli_icon_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Icon_XHud.png");
            Image sli_img_icon = Mc_AddImage(sli_icon, Color.white, new Vector2(30, 30), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), false);
            sli_img_icon.sprite = sli_icon_sprite;
            #endregion

            #region 创建Slider - 滑动区域根物体
            GameObject sli_structure = Mc_CreateObject("Structure", "XHud", Vector3.zero, Vector3.zero, Vector3.one, sli.RectTransform);
            #endregion

            #region 绘制滑动条轨道根物体
            RectTransform sli_structure_rect = Mc_AddRectTransform(sli_structure, new Vector2(-90, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(90, 0), new Vector2(-90, 0));
            #endregion

            #region 创建Slider - 滑动区域 - 背景
            GameObject sli_structure_bg = Mc_CreateObject("Bg", "XHud", Vector3.zero, Vector3.zero, Vector3.one, sli_structure_rect);
            Image sli_img_bg = Mc_AddImage(sli_structure_bg, Color.gray, new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(0, 0), true);
            #endregion

            #region 创建Slider - 滑动区域 - 前景
            GameObject sli_structure_fg = Mc_CreateObject("Fg", "XHud", Vector3.zero, Vector3.zero, Vector3.one, sli_structure_rect);
            Image sli_img_fg = Mc_AddImage(sli_structure_fg, XHud_Dashboard.Theme_Primary, new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(0, 0), true);
            #endregion

            #region 创建Slider - 控制柄限制器
            GameObject sli_handle_root = Mc_CreateObject("HandleRect", "XHud", Vector3.zero, Vector3.zero, Vector3.one, sli_structure_rect);
            RectTransform sli_handle_root_rect = Mc_AddRectTransform(sli_handle_root, new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(15, 0), new Vector2(-15, 0));
            #endregion

            #region 创建Slider - 控制柄
            GameObject sli_handle = Mc_CreateObject("Handle", "XHud", Vector3.zero, Vector3.zero, Vector3.one, sli_handle_root_rect);
            RectTransform sli_handle_rect = Mc_AddRectTransform(sli_handle, new Vector2(1, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, -5), new Vector2(-30, 5));

            sli_handle_rect.sizeDelta = new Vector2(30, sli_handle_rect.sizeDelta.y);
            sli_handle_rect.anchoredPosition = new Vector2(0, sli_handle_rect.anchoredPosition.y);

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Slider_Handle.png");
            Image sli_img_handle = Mc_AddImage(sli_handle, Color.white, 1.5f, sprite, Image.Type.Sliced, true);
            #endregion

            #region 赋值
            sli.sli_Bg = sli_img_bg;
            sli.sli_Fore = sli_img_fg;
            sli.sli_Icon = sli_img_icon;
            sli.handleRect = sli_img_handle.rectTransform;
            sli.fillRect = sli_img_fg.rectTransform;
            sli.sli_Handle = sli_img_handle;
            sli.targetGraphic = sli_img_handle;
            #endregion

            SceneView.RepaintAll();
            Selection.activeTransform = obj.transform;
        }
        #endregion

        #region 模组创建 - 开关

        [MenuItem("GameObject/XHud/Module（模组）/Toggle (开关)", priority = 1000, validate = true)]
        private static bool Validate_Create_HudToggle()
        {
            bool valid = false;

            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject != null)
            {
                if (selectedObject.GetComponentInParent<XHud_Manager>() && selectedObject.name.Contains("Anchor_"))
                    valid = true;
            }

            return valid;
        }

        [MenuItem("GameObject/XHud/Module（模组）/Toggle (开关)", priority = 1000)]
        private static void Create_HudToggle()
        {
            string res = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 创建模组消息", "选择开关风格", "您想创建什么样风格的开关？", "文字", "图标", "图标 & 文字", "纯净", 2);

            #region 创建Hud元素
            GameObject obj = Mc_CreateObject("Element(Toggle)", "XHud", Vector3.zero, Vector3.zero, Vector3.one, Selection.activeTransform);
            XHud_Module_Element btn_ele = Mc_AddHudElement(obj);
            if (res == "图标")
                btn_ele.RectTransform = Mc_AddRectTransform(obj, new Vector2(80, 35));
            else if (res == "文字")
                btn_ele.RectTransform = Mc_AddRectTransform(obj, new Vector2(150, 35));
            else if (res == "图标 & 文字")
                btn_ele.RectTransform = Mc_AddRectTransform(obj, new Vector2(185, 35));
            else if (res == "纯净")
                btn_ele.RectTransform = Mc_AddRectTransform(obj, new Vector2(65, 35));
            #endregion

            #region 创建Toggle根物体
            GameObject tog_root = Mc_CreateObject("Toggle", "XHud", Vector3.zero, Vector3.zero, Vector3.one, btn_ele.RectTransform);
            RectTransform tog_root_rect = Mc_AddRectTransform(tog_root, new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(0, 0));
            #endregion

            #region 创建Toggle
            XHud_Module_Toggle tog = Mc_AddHudToggle(tog_root, "Toggle", "XHud-Toggle");
            ColorBlock colors = tog.colors;
            colors.pressedColor = XHud_Dashboard.Theme_Primary;
            tog.colors = colors;
            tog.BgCanToggle = true;
            #endregion

            #region 设置Toggle 颜色
            tog.Tog_Color_Bg_Unchecked = new Color(0.282353f, 0.282353f, 0.282353f, 1);
            tog.Tog_Color_Bg_Checked = XHud_Dashboard.Theme_Primary;
            tog.Tog_Color_Handle_Unchecked = new Color(0.8392157f, 0.8392157f, 0.8392157f, 1);
            tog.Tog_Color_Handle_Checked = new Color(0.2156863f, 0.2156863f, 0.2156863f, 1);
            #endregion

            if (res == "图标")
            {
                #region 创建Button - 图标
                GameObject tog_icon = Mc_CreateObject("Icon", "XHud", new Vector3(8, 0, 0), Vector3.zero, Vector3.one, tog_root_rect);
                Sprite tog_icon_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Icon_Objective.png");
                Image tog_img_icon = Mc_AddImage(tog_icon, Color.white, new Vector2(16, 16), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), false);
                tog_img_icon.sprite = tog_icon_sprite;
                #endregion

                tog.TitleCanToggle = false;
            }
            else if (res == "文字" || res == "图标 & 文字")
            {
                string res_text = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 创建模组消息", "选择文字类型", "您希望使用那种文字组件类型作为开关文字使用？", "Text", "TmpText", 1);

                if (res == "文字")
                {
                    if (res_text == "Text")
                    {
                        //创建Slider - 按钮文字
                        XHud_Module_Text tog_text = Mc_AddText(tog_root_rect, "Title", new Vector3(50, 0, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Bold.ttf", new Vector2(100, 30), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), ContentAnchor.左, TextAnchor.MiddleLeft, tog.ToggleName, 13, Color.white, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, false);
                        tog.ToggleText = tog_text;
                    }
                    else if (res_text == "TmpText")
                    {
                        //创建Slider - 按钮文字
                        XHud_Module_TmpText tog_tmptext = Mc_AddTmpText(tog_root_rect, "Title", new Vector3(50, 0, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Bold SDF", new Vector2(100, 30), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), TmpContentAnchor.左, TextAlignmentOptions.Left, $"{tog.ToggleName}", 13, Color.white, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, false);
                        tog.ToggleTmpText = tog_tmptext;
                    }
                }
                else if (res == "图标 & 文字")
                {
                    #region 创建Button - 图标
                    GameObject tog_icon = Mc_CreateObject("Icon", "XHud", new Vector3(8, 0, 0), Vector3.zero, Vector3.one, tog_root_rect);
                    Sprite tog_icon_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Icon_Objective.png");
                    Image tog_img_icon = Mc_AddImage(tog_icon, Color.white, new Vector2(16, 16), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), false);
                    tog_img_icon.sprite = tog_icon_sprite;
                    #endregion

                    if (res_text == "Text")
                    {
                        //创建Slider - 按钮文字
                        XHud_Module_Text tog_text = Mc_AddText(tog_root_rect, "Title", new Vector3(82, 0, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Bold.ttf", new Vector2(100, 30), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), ContentAnchor.左, TextAnchor.MiddleLeft, tog.ToggleName, 13, Color.white, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, false);
                        tog.ToggleText = tog_text;
                    }
                    else if (res_text == "TmpText")
                    {
                        //创建Slider - 按钮文字
                        XHud_Module_TmpText tog_tmptext = Mc_AddTmpText(tog_root_rect, "Title", new Vector3(82, 0, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Bold SDF", new Vector2(100, 30), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), TmpContentAnchor.左, TextAlignmentOptions.Left, $"{tog.ToggleName}", 13, Color.white, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, false);
                        tog.ToggleTmpText = tog_tmptext;
                    }
                }
                tog.TitleCanToggle = true;
            }

            #region 创建Toggle - Bg
            GameObject tog_bg = Mc_CreateObject("Bg", "XHud", Vector3.zero, Vector3.zero, Vector3.one, tog_root_rect);
            Sprite tog_bg_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Toggle_Bg_False.png");
            Image tog_img_bg = Mc_AddImage(tog_bg, tog.Tog_Color_Bg_Unchecked, new Vector2(44, 23), res == "纯净" ? new Vector2(0.5f, 0.5f) : new Vector2(1, 0.5f), res == "纯净" ? new Vector2(0.5f, 0.5f) : new Vector2(1, 0.5f), res == "纯净" ? new Vector2(0.5f, 0.5f) : new Vector2(1, 0.5f), true);
            tog_img_bg.sprite = tog_bg_sprite;
            tog.Tog_Bg = tog_img_bg;
            #endregion

            #region 创建Toggle - Handle
            GameObject tog_handle = Mc_CreateObject("Handle", "XHud", new Vector3(-11, 0, 0), Vector3.zero, Vector3.one, tog_img_bg.rectTransform);
            Sprite tog_handle_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Toggle_Handle_False.png");
            Image tog_img_handle = Mc_AddImage(tog_handle, tog.Tog_Color_Handle_Unchecked, new Vector2(18, 18), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), true);
            tog_img_handle.sprite = tog_handle_sprite;
            tog.Tog_Handle = tog_img_handle;
            #endregion

            tog.HandlePosRange = new Vector2(-11, 11);

            SceneView.RepaintAll();
            Selection.activeTransform = obj.transform;
        }
        #endregion

        #region 模组创建 - 选项

        [MenuItem("GameObject/XHud/Module（模组）/Option (选项器)", priority = 1000, validate = true)]
        private static bool Validate_Create_HudOption()
        {
            bool valid = false;

            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject != null)
            {
                if (selectedObject.GetComponentInParent<XHud_Manager>() && selectedObject.name.Contains("Anchor_"))
                    valid = true;
            }

            return valid;
        }

        [MenuItem("GameObject/XHud/Module（模组）/Option (选项器)", priority = 1000)]
        private static void Create_HudOption()
        {
            string res_type = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 创建模组消息", "选择选项器风格", "您想创建什么样风格的选项器？", "文字", "图标", "图标 & 文字", 0);

            #region 创建Hud元素
            GameObject obj = Mc_CreateObject("Element(Option)", "XHud", Vector3.zero, Vector3.zero, Vector3.one, Selection.activeTransform);
            XHud_Module_Element opt_ele = Mc_AddHudElement(obj);
            opt_ele.RectTransform = Mc_AddRectTransform(obj, new Vector2(420, 75));
            #endregion

            #region 创建Option根物体
            GameObject opt_root = Mc_CreateObject("Option", "XHud", Vector3.zero, Vector3.zero, Vector3.one, opt_ele.RectTransform);
            RectTransform opt_root_rect = Mc_AddRectTransform(opt_root, new Vector2(420, 75), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(0, 0));
            #endregion

            #region 创建Option
            XHud_Module_Option option = Mc_AddHudOption(opt_root, "Option");
            option.RepeatTweenPlay = true;
            option.UseBlinked = true;
            option.UseEaseMotion = true;
            #endregion

            #region 创建主图标
            GameObject opt_bg = Mc_CreateObject("Icon", "XHud", new Vector3(15, -15), Vector3.zero, Vector3.one, opt_root_rect);
            Sprite opt_bg_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Icon_XHud.png");
            Image opt_img_bg = Mc_AddImage(opt_bg, Color.white, new Vector2(30, 30), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), false);
            opt_img_bg.sprite = opt_bg_sprite;
            #endregion

            #region 创建标题
            string res = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 创建模组消息", "选择标题文字类型", "您希望选项器的标题以及选项按钮标题文字使用哪种文字组件类型？", "Text", "TmpText", 1);
            if (res == "Text")
            {
                //创建Slider - 标题
                XHud_Module_Text title = Mc_AddText(opt_root_rect, "Title", new Vector3(115, -12.5f, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Bold.ttf", new Vector2(130, 25), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), ContentAnchor.左, TextAnchor.MiddleLeft, "XHud-Option", 18, Color.white, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, false);
            }
            else
            {
                //创建Slider - 标题
                XHud_Module_TmpText title = Mc_AddTmpText(opt_root_rect, "Title", new Vector3(115, -12.5f, 0), Vector3.zero, Vector3.one, "XHud", "SevenBlack-Bold SDF", new Vector2(130, 25), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), TmpContentAnchor.左, TextAlignmentOptions.Left, "XHud-Option", 18, Color.white, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, false);
            }
            #endregion

            #region 创建 Selector 光标
            GameObject opt_selector = Mc_CreateObject("Selector", "XHud", new Vector3(-85, -22.5f), Vector3.zero, Vector3.one, opt_root_rect);
            Sprite opt_selector_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Opt_Selector2.png");
            Image opt_img_selector = Mc_AddImage(opt_selector, XHud_Dashboard.Theme_Primary, new Vector2(31, 31), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), false);
            opt_img_selector.sprite = opt_selector_sprite;
            option.SelectorMark = opt_img_selector.rectTransform;
            #endregion

            #region 创建Option 选项按钮根物体
            GameObject opt_groups = Mc_CreateObject("Groups", "XHud", Vector3.zero, Vector3.zero, Vector3.one, opt_root_rect);
            RectTransform opt_groups_rect = Mc_AddRectTransform(opt_groups, new Vector2(0, 30), new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            option.OptionRoot = opt_groups_rect;
            #endregion

            #region 创建Option - Button

            string[] ButtonText = new string[] { "A", "B", "C" };
            for (int i = 0; i < ButtonText.Length; i++)
            {
                #region 创建Button根物体
                GameObject btn_root = Mc_CreateObject("OptionButton_" + ButtonText[i], "XHud", new Vector3(85, -15), Vector3.zero, Vector3.one, opt_groups_rect);
                RectTransform btn_root_rect = Mc_AddRectTransform(btn_root, new Vector2(30, 30), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(0, 0));
                #endregion

                #region 创建Button
                XHud_Module_Button btn = Mc_AddHudButton(btn_root, ButtonText[i], "Item");
                ColorBlock colors = btn.colors;
                colors.pressedColor = XHud_Dashboard.Theme_Primary;
                btn.colors = colors;
                btn.BgColorSyncFade = true;
                btn.IsOptionButton = true;
                btn.ButtonActionTiming = "鼠标点击";
                #endregion

                #region 创建Button Bg
                GameObject opt_btn_bg = Mc_CreateObject("Bg", "XHud", Vector3.zero, Vector3.zero, Vector3.one, btn_root_rect);
                Sprite opt_btn_bg_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Opt_Circle.png");
                Image opt_img_btn_bg = Mc_AddImage(opt_btn_bg, Color.white, new Vector2(18, 18), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), true);
                opt_img_btn_bg.sprite = opt_btn_bg_sprite;

                btn.BgImage = opt_img_btn_bg;
                #endregion

                if (res_type == "图标")
                {
                    #region 创建选项图标
                    GameObject opt_item_icon = Mc_CreateObject("Icon", "XHud", new Vector3(-30, -9), Vector3.zero, Vector3.one, opt_img_btn_bg.rectTransform);
                    Sprite opt_item_icon_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Icon_Objective.png");
                    Image opt_img_item_icon = Mc_AddImage(opt_item_icon, Color.white, new Vector2(16, 16), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), true);
                    opt_img_item_icon.sprite = opt_item_icon_sprite;
                    btn.IconImage = opt_img_item_icon;
                    #endregion

                    btn.IconColorSyncFade = true;
                    btn.TextColorSyncFade = false;
                }
                else if (res_type == "图标 & 文字" || res_type == "文字")
                {
                    if (res_type == "图标 & 文字")
                    {
                        #region 创建选项图标
                        GameObject opt_item_icon = Mc_CreateObject("Icon", "XHud", new Vector3(-88, -8), Vector3.zero, Vector3.one, opt_img_btn_bg.rectTransform);
                        Sprite opt_item_icon_sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Sprites/Others/Icon_Objective.png");
                        Image opt_img_item_icon = Mc_AddImage(opt_item_icon, Color.white, new Vector2(16, 16), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), true);
                        opt_img_item_icon.sprite = opt_item_icon_sprite;
                        btn.IconImage = opt_img_item_icon;
                        #endregion

                        btn.IconColorSyncFade = true;
                        btn.TextColorSyncFade = true;
                    }
                    else
                    {
                        btn.IconColorSyncFade = false;
                        btn.TextColorSyncFade = true;
                    }

                    #region 创建Button Title
                    if (res == "Text")
                    {
                        //创建Slider - 标题
                        XHud_Module_Text title = Mc_AddText(opt_img_btn_bg.rectTransform, "Title", Vector3.zero, Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light.ttf", new Vector2(80, 35), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0, 0), ContentAnchor.中心, TextAnchor.MiddleCenter, "Item " + ButtonText[i], 13, Color.white, FontStyle.Normal, HorizontalWrapMode.Overflow, VerticalWrapMode.Overflow, true);

                        btn.ButtonText = title;
                    }
                    else
                    {
                        //创建Slider - 标题
                        XHud_Module_TmpText title = Mc_AddTmpText(opt_img_btn_bg.rectTransform, "Title", Vector3.zero, Vector3.zero, Vector3.one, "XHud", "SevenBlack-Light SDF", new Vector2(80, 35), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0, 0), TmpContentAnchor.中心, TextAlignmentOptions.Center, "Item " + ButtonText[i], 13, Color.white, FontStyles.Normal, TextOverflowModes.Overflow, TextWrappingModes.NoWrap, true);

                        btn.ButtonTmpText = title;
                    }
                    #endregion

                }

                #region 添加到选项按钮列表中
                ElementNode_OptionButton node = new ElementNode_OptionButton();
                node.Button = btn;
                node.Indicator = ButtonText[i];
                node.IsOptional = btn.IsOptionButton;
                if (option.OptionButtonNodes == null)
                    option.OptionButtonNodes = new List<ElementNode_OptionButton>();
                option.OptionButtonNodes.Add(node);
                #endregion
            }

            #region 增加水平自适应布局组件
            HorizontalLayoutGroup layout = opt_groups.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleRight;
            #endregion

            #endregion

            SceneView.RepaintAll();
            Selection.activeTransform = obj.transform;
        }

        #endregion

        #region 辅助创建

        /// <summary>
        /// 创建一个物体
        /// </summary>
        /// <param name="name"></param>
        /// <param name="layer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static GameObject Mc_CreateObject(string name, string layer, Vector3 pos, Vector3 ang, Vector3 scale, Transform parent)
        {
            GameObject obj = new GameObject();
            obj.transform.SetParent(parent);
            obj.name = name;
            obj.transform.localScale = scale;
            obj.transform.localEulerAngles = ang;
            obj.transform.localPosition = pos;
            obj.layer = LayerMask.NameToLayer(layer);
            return obj;
        }

        /// <summary>
        /// 添加RectTransform
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static RectTransform Mc_AddRectTransform(GameObject obj, Vector2 size)
        {
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            return rect;
        }

        /// <summary>
        /// 添加RectTransform
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static RectTransform Mc_AddRectTransform(GameObject obj, Vector2 size, Vector2 anchor_min, Vector2 anchor_max, Vector2 pivot, Vector2 offset_min, Vector2 offset_max)
        {
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchor_min;
            rect.anchorMax = anchor_max;
            rect.pivot = pivot;
            rect.offsetMin = offset_min;
            rect.offsetMax = offset_max;
            rect.sizeDelta = size;
            return rect;
        }

        /// <summary>
        /// 添加RectTransform
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static RectTransform Mc_AddRectTransform(GameObject obj, Vector2 anchor_min, Vector2 anchor_max, Vector2 pivot, Vector2 offset_min, Vector2 offset_max)
        {
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchor_min;
            rect.anchorMax = anchor_max;
            rect.pivot = pivot;
            rect.offsetMin = offset_min;
            rect.offsetMax = offset_max;
            return rect;
        }

        /// <summary>
        /// 添加Image
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="anchor_min"></param>
        /// <param name="anchor_max"></param>
        /// <param name="pivot"></param>
        /// <param name="offset_min"></param>
        /// <param name="offset_max"></param>
        /// <param name="raycast"></param>
        /// <returns></returns>
        private static Image Mc_AddImage(GameObject obj, Color color, Vector2 anchor_min, Vector2 anchor_max, Vector2 pivot, Vector2 offset_min, Vector2 offset_max, bool raycast)
        {
            Image img = obj.AddComponent<Image>();
            img.raycastTarget = raycast;
            img.color = color;
            RectTransform img_rect = img.rectTransform;
            img_rect.pivot = pivot;
            img_rect.anchorMin = anchor_min;
            img_rect.anchorMax = anchor_max;
            img_rect.offsetMin = offset_min;
            img_rect.offsetMax = offset_max;

            return img;
        }

        /// <summary>
        /// 添加Image
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="anchor_min"></param>
        /// <param name="anchor_max"></param>
        /// <param name="pivot"></param>
        /// <param name="raycast"></param>
        /// <returns></returns>
        private static Image Mc_AddImage(GameObject obj, Color color, Vector2 size, Vector2 anchor_min, Vector2 anchor_max, Vector2 pivot, bool raycast)
        {
            Image img = obj.AddComponent<Image>();
            img.raycastTarget = raycast;
            img.color = color;
            RectTransform img_rect = img.rectTransform;
            img_rect.sizeDelta = size;
            img_rect.pivot = pivot;
            img_rect.anchorMin = anchor_min;
            img_rect.anchorMax = anchor_max;

            return img;
        }

        /// <summary>
        /// 添加Image
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="color"></param>
        /// <param name="pixelPer"></param>
        /// <param name="sprite"></param>
        /// <param name="type"></param>
        /// <param name="raycast"></param>
        /// <returns></returns>
        private static Image Mc_AddImage(GameObject obj, Color color, float pixelPer, Sprite sprite, Image.Type type, bool raycast)
        {
            Image img = obj.AddComponent<Image>();
            img.raycastTarget = raycast;
            img.color = color;
            img.pixelsPerUnitMultiplier = pixelPer;
            img.sprite = sprite;
            img.type = type;
            img.raycastTarget = raycast;
            return img;
        }

        /// <summary>
        /// 添加HudElement
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static XHud_Module_Element Mc_AddHudElement(GameObject obj)
        {
            return obj.AddComponent<XHud_Module_Element>();
        }

        /// <summary>
        /// 添加滑动条
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="indicator"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <param name="title"></param>
        /// <param name="subtitle"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        private static XHud_Module_Slider Mc_AddHudSlider(GameObject obj, string indicator, float min, float max, float value, string unit, string title, string subtitle, int precision)
        {
            XHud_Module_Slider slider = obj.AddComponent<XHud_Module_Slider>();
            slider.Indicator = indicator;
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = value;
            slider.sli_Unit = unit;
            slider.con_title = title;
            slider.con_subtitle = subtitle;
            slider.sli_Precision = precision;

            return slider;
        }

        /// <summary>
        /// 添加进度条
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="indicator"></param>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <param name="title"></param>
        /// <param name="subtitle"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        private static XHud_Module_Progress Mc_AddHudProgress(GameObject obj, string indicator, float value, string unit, string title, string subtitle, int precision)
        {
            XHud_Module_Progress slider = obj.AddComponent<XHud_Module_Progress>();
            slider.Indicator = indicator;
            slider.ProgressValue = value;
            slider.ProgressUnit = unit;
            slider.con_title = title;
            slider.con_subtitle = subtitle;
            slider.ProgressPrecision = precision;

            return slider;
        }

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="indicator"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static XHud_Module_Button Mc_AddHudButton(GameObject obj, string indicator, string name)
        {
            XHud_Module_Button button = obj.AddComponent<XHud_Module_Button>();
            button.Indicator = indicator;
            button.ButtonName = name;
            return button;
        }

        /// <summary>
        /// 添加选项器
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="indicator"></param>
        /// <returns></returns>
        private static XHud_Module_Option Mc_AddHudOption(GameObject obj, string indicator)
        {
            XHud_Module_Option button = obj.AddComponent<XHud_Module_Option>();
            button.Indicator = indicator;
            return button;
        }

        /// <summary>
        /// 添加开关
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="indicator"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static XHud_Module_Toggle Mc_AddHudToggle(GameObject obj, string indicator, string name)
        {
            XHud_Module_Toggle tog = obj.AddComponent<XHud_Module_Toggle>();
            tog.Indicator = indicator;
            tog.ToggleName = name;
            return tog;
        }

        /// <summary>
        /// 添加文字 - Text
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        /// <param name="ang"></param>
        /// <param name="scale"></param>
        /// <param name="layer"></param>
        /// <param name="font_name"></param>
        /// <param name="text_size"></param>
        /// <param name="anchor_min"></param>
        /// <param name="anchor_max"></param>
        /// <param name="pivot"></param>
        /// <param name="anchor_pos"></param>
        /// <param name="anchor_content"></param>
        /// <param name="anchor_text"></param>
        /// <param name="text"></param>
        /// <param name="font_size"></param>
        /// <param name="font_color"></param>
        /// <param name="style"></param>
        /// <param name="h_mode"></param>
        /// <param name="v_mode"></param>
        /// <param name="raycast"></param>
        /// <returns></returns>
        private static XHud_Module_Text Mc_AddText(Transform parent, string name, Vector3 pos, Vector3 ang, Vector3 scale, string layer, string font_name, Vector2 text_size, Vector2 anchor_min, Vector2 anchor_max, Vector2 pivot, Vector2 anchor_pos, ContentAnchor anchor_content, TextAnchor anchor_text, string text, int font_size, Color font_color, FontStyle style, HorizontalWrapMode h_mode, VerticalWrapMode v_mode, bool raycast)
        {
            GameObject obj = new GameObject();
            obj.transform.SetParent(parent);
            obj.name = name;
            obj.transform.localScale = scale;
            obj.transform.localEulerAngles = ang;
            obj.transform.localPosition = pos;
            obj.layer = LayerMask.NameToLayer(layer);

            Font font = AssetDatabase.LoadAssetAtPath<Font>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Fonts/Text/{font_name}");

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.sizeDelta = text_size;
            rect.anchorMin = anchor_min;
            rect.anchorMax = anchor_max;
            rect.pivot = pivot;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + anchor_pos.x, rect.anchoredPosition.y + anchor_pos.y);

            XHud_Module_Text hud_text = obj.AddComponent<XHud_Module_Text>();
            hud_text.TextStyleInfo.txt_Set_Alignment(anchor_content);
            hud_text.alignment = anchor_text;
            hud_text.text = text;
            hud_text.TextStyleInfo.txt_Set_FontSize(font_size);
            hud_text.TextStyleInfo.txt_Set_FontColor(font_color);
            hud_text.TextStyleInfo.txt_Set_Font(font);
            hud_text.font = font;
            hud_text.TextStyleInfo.txt_Set_FontStyle(style);
            hud_text.TextStyleInfo.txt_Set_Overflow(h_mode);
            hud_text.TextStyleInfo.txt_Set_Overflow(v_mode);
            hud_text.TextStyleInfo.gen_RayCastSet(raycast);

            return hud_text;
        }

        /// <summary>
        /// 添加文字到现有物体上 - Text
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        /// <param name="ang"></param>
        /// <param name="scale"></param>
        /// <param name="layer"></param>
        /// <param name="font_name"></param>
        /// <param name="text_size"></param>
        /// <param name="anchor_min"></param>
        /// <param name="anchor_max"></param>
        /// <param name="pivot"></param>
        /// <param name="anchor_pos"></param>
        /// <param name="anchor_content"></param>
        /// <param name="anchor_text"></param>
        /// <param name="text"></param>
        /// <param name="font_size"></param>
        /// <param name="font_color"></param>
        /// <param name="style"></param>
        /// <param name="h_mode"></param>
        /// <param name="v_mode"></param>
        /// <param name="raycast"></param>
        /// <returns></returns>
        private static XHud_Module_Text Mc_AddText(GameObject parent, string name, Vector3 pos, Vector3 ang, Vector3 scale, string layer, string font_name, Vector2 text_size, Vector2 anchor_min, Vector2 anchor_max, Vector2 pivot, Vector2 anchor_pos, ContentAnchor anchor_content, TextAnchor anchor_text, string text, int font_size, Color font_color, FontStyle style, HorizontalWrapMode h_mode, VerticalWrapMode v_mode, bool raycast)
        {
            Font font = AssetDatabase.LoadAssetAtPath<Font>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Fonts/Text/{font_name}");

            RectTransform rect = parent.GetComponent<RectTransform>();
            if (rect == null)
                rect = parent.AddComponent<RectTransform>();
            rect.sizeDelta = text_size;
            rect.anchorMin = anchor_min;
            rect.anchorMax = anchor_max;
            rect.pivot = pivot;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + anchor_pos.x, rect.anchoredPosition.y + anchor_pos.y);

            XHud_Module_Text hud_text = parent.AddComponent<XHud_Module_Text>();
            hud_text.TextStyleInfo.txt_Set_Alignment(anchor_content);
            hud_text.alignment = anchor_text;
            hud_text.text = text;
            hud_text.TextStyleInfo.txt_Set_FontSize(font_size);
            hud_text.TextStyleInfo.txt_Set_FontColor(font_color);
            hud_text.TextStyleInfo.txt_Set_Font(font);
            hud_text.font = font;
            hud_text.TextStyleInfo.txt_Set_FontStyle(style);
            hud_text.TextStyleInfo.txt_Set_Overflow(h_mode);
            hud_text.TextStyleInfo.txt_Set_Overflow(v_mode);
            hud_text.TextStyleInfo.gen_RayCastSet(raycast);

            return hud_text;
        }

        /// <summary>
        /// 添加文字 - TmpText
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        /// <param name="ang"></param>
        /// <param name="scale"></param>
        /// <param name="layer"></param>
        /// <param name="font_name"></param>
        /// <param name="text_size"></param>
        /// <param name="anchor_min"></param>
        /// <param name="anchor_max"></param>
        /// <param name="pivot"></param>
        /// <param name="anchor_pos"></param>
        /// <param name="anchor_content"></param>
        /// <param name="anchor_text"></param>
        /// <param name="text"></param>
        /// <param name="font_size"></param>
        /// <param name="font_color"></param>
        /// <param name="style"></param>
        /// <param name="overflow"></param>
        /// <param name="wrap"></param>
        /// <param name="raycast"></param>
        /// <returns></returns>
        private static XHud_Module_TmpText Mc_AddTmpText(Transform parent, string name, Vector3 pos, Vector3 ang, Vector3 scale, string layer, string font_name, Vector2 text_size, Vector2 anchor_min, Vector2 anchor_max, Vector2 pivot, Vector2 anchor_pos, TmpContentAnchor anchor_content, TextAlignmentOptions anchor_text, string text, int font_size, Color font_color, FontStyles style, TextOverflowModes overflow, TextWrappingModes wrap
            , bool raycast)
        {
            GameObject obj = new GameObject();
            obj.transform.SetParent(parent);
            obj.name = name;
            obj.transform.localScale = scale;
            obj.transform.localEulerAngles = ang;
            obj.transform.localPosition = pos;
            obj.layer = LayerMask.NameToLayer(layer);

            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Fonts/Tmp/{font_name}.asset");

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.sizeDelta = text_size;
            rect.anchorMin = anchor_min;
            rect.anchorMax = anchor_max;
            rect.pivot = pivot;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + anchor_pos.x, rect.anchoredPosition.y + anchor_pos.y);

            XHud_Module_TmpText hud_tmptext = obj.AddComponent<XHud_Module_TmpText>();
            hud_tmptext.TextStyleInfo.tmp_Set_Alignment(anchor_content);
            hud_tmptext.alignment = anchor_text;
            hud_tmptext.text = text;
            hud_tmptext.TextStyleInfo.tmp_Set_FontSize(font_size);
            hud_tmptext.fontSize = font_size;
            hud_tmptext.TextStyleInfo.tmp_Set_FontColor(font_color);
            hud_tmptext.TextStyleInfo.tmp_Set_FontAsset(font);
            hud_tmptext.font = font;
            hud_tmptext.TextStyleInfo.tmp_Set_FontStyle(style);
            hud_tmptext.TextStyleInfo.tmp_Set_Overflow(overflow);
            hud_tmptext.TextStyleInfo.tmp_Set_WordWrappingMode(wrap);
            hud_tmptext.textWrappingMode = TextWrappingModes.NoWrap;
            hud_tmptext.TextStyleInfo.gen_RayCastSet(raycast);

            return hud_tmptext;
        }

        /// <summary>
        /// 添加文字到现有物体上 - TmpText
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        /// <param name="ang"></param>
        /// <param name="scale"></param>
        /// <param name="layer"></param>
        /// <param name="font_name"></param>
        /// <param name="text_size"></param>
        /// <param name="anchor_min"></param>
        /// <param name="anchor_max"></param>
        /// <param name="pivot"></param>
        /// <param name="anchor_pos"></param>
        /// <param name="anchor_content"></param>
        /// <param name="anchor_text"></param>
        /// <param name="text"></param>
        /// <param name="font_size"></param>
        /// <param name="font_color"></param>
        /// <param name="style"></param>
        /// <param name="overflow"></param>
        /// <param name="wrap"></param>
        /// <param name="raycast"></param>
        /// <returns></returns>
        private static XHud_Module_TmpText Mc_AddTmpText(GameObject parent, string name, Vector3 pos, Vector3 ang, Vector3 scale, string layer, string font_name, Vector2 text_size, Vector2 anchor_min, Vector2 anchor_max, Vector2 pivot, Vector2 anchor_pos, TmpContentAnchor anchor_content, TextAlignmentOptions anchor_text, string text, int font_size, Color font_color, FontStyles style, TextOverflowModes overflow, TextWrappingModes wrap
            , bool raycast)
        {
            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Fonts/Tmp/{font_name}.asset");

            RectTransform rect = parent.GetComponent<RectTransform>();
            if (rect == null)
                rect = parent.AddComponent<RectTransform>(); rect.sizeDelta = text_size;
            rect.anchorMin = anchor_min;
            rect.anchorMax = anchor_max;
            rect.pivot = pivot;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + anchor_pos.x, rect.anchoredPosition.y + anchor_pos.y);

            XHud_Module_TmpText hud_tmptext = parent.AddComponent<XHud_Module_TmpText>();
            hud_tmptext.TextStyleInfo.tmp_Set_Alignment(anchor_content);
            hud_tmptext.alignment = anchor_text;
            hud_tmptext.text = text;
            hud_tmptext.TextStyleInfo.tmp_Set_FontSize(font_size);
            hud_tmptext.fontSize = font_size;
            hud_tmptext.TextStyleInfo.tmp_Set_FontColor(font_color);
            hud_tmptext.TextStyleInfo.tmp_Set_FontAsset(font);
            hud_tmptext.font = font;
            hud_tmptext.TextStyleInfo.tmp_Set_FontStyle(style);
            hud_tmptext.TextStyleInfo.tmp_Set_Overflow(overflow);
            hud_tmptext.TextStyleInfo.tmp_Set_WordWrappingMode(wrap);
            hud_tmptext.textWrappingMode = TextWrappingModes.NoWrap;
            hud_tmptext.TextStyleInfo.gen_RayCastSet(raycast);

            return hud_tmptext;
        }
        #endregion
    }
}