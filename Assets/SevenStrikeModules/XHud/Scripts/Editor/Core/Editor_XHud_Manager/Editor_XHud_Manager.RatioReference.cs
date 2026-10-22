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
    using UnityEngine;
    using UnityEngine.UI;

    public partial class Editor_XHud_Manager : Editor
    {
        #region 绘制 - 比例图例
        /// <summary>
        /// 绘制「比例图例」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制使用图例比例参考、使用局部模式两个开关。</description></item>
        /// <item><description>绘制物理屏幕可视尺寸、图例着色字段。</description></item>
        /// <item><description>绘制参考物整体/局部的尺寸与图例字段。</description></item>
        /// <item><description>绘制比例参考图形的尺寸、公差字段与创建按钮。</description></item>
        /// </list>
        /// </summary>
        private void Draw_RatioReference()
        {
            #region 使用图例比例参考
            DrawToggle("使用图例比例参考", UseRatioReference, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                UseRatioReference.boolValue = b;
                UseRatioReference.serializedObject.ApplyModifiedProperties();

                if (UseRatioReference.boolValue)
                {
                    if (BaseScript.Res_Part.sprite == null && BaseScript.Res_Full.sprite == null)
                    {
                        UseRatioReference.boolValue = false;
                        UseRatioReference.serializedObject.ApplyModifiedProperties();
                        return;
                    }

                    //如果已经存在就不用创建
                    if (sp_Reference_Image.objectReferenceValue != null)
                        return;

                    Sprite spr = BaseScript.Res_Part.sprite;
                    if (!RatioReferenceIsPart.boolValue)
                        spr = BaseScript.Res_Full.sprite;

                    xHud_RatioReference_Create(spr);
                }
                else
                {
                    xHud_RatioReference_Destroy();
                }
            });
            #endregion

            #region 使用局部比例
            DrawToggle("使用局部模式", RatioReferenceIsPart, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                xHud_RatioReference_Switch();
                xHud_RatioReference_Update();
            });
            #endregion

            #region 物理屏幕可视尺寸
            XGUI.layout_property_field(
                title: "物理屏幕可视尺寸",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 200,
                //status_icon: "icon_field_status",
                //status_icon_color: CameraOrthographicSize.floatValue != 0 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.red,
                prop: sp_PhysicsScreenSize,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            XGUI.layout_space(5);

            XGUI.ChangedCheck_Start();

            #region 图例着色
            XGUI.layout_colorfield(
                prop: sp_Reference_Image_Color,
                title: "图例着色",
                title_width: 80,
                //title_color: Color.white,
                //icon: "icon_field_status",
                //icon_color: Color.red,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(sp_Reference_Image_Color.colorValue)}",
                state_value_color: sp_Reference_Image_Color.colorValue);
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 5, 15));

            #region 参考物尺寸 - 整体
            XGUI.layout_property_field(
                title: "参考物尺寸 - 整体",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 140,
                //status_icon: "icon_field_status",
                //status_icon_color: CameraOrthographicSize.floatValue != 0 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.red,
                prop: sp_Res_Full.FindPropertyRelative("size"),
                prop_margin: new RectOffset(0, 0, 5, 0));

            XGUI.layout_space(5);

            XGUI.layout_property_field(
                title: "参考物图例 - 整体",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 140,
                //status_icon: "icon_field_status",
                //status_icon_color: CameraOrthographicSize.floatValue != 0 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.red,
                prop: sp_Res_Full.FindPropertyRelative("sprite"),
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 10));

            #region 参考物尺寸 - 局部
            XGUI.layout_property_field(
                title: "参考物尺寸 - 局部",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 140,
                //status_icon: "icon_field_status",
                //status_icon_color: CameraOrthographicSize.floatValue != 0 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.red,
                prop: sp_Res_Part.FindPropertyRelative("size"),
                prop_margin: new RectOffset(0, 0, 5, 0));

            XGUI.layout_space(5);

            XGUI.layout_property_field(
                title: "参考物图例 - 局部",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 140,
                //status_icon: "icon_field_status",
                //status_icon_color: CameraOrthographicSize.floatValue != 0 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.red,
                prop: sp_Res_Part.FindPropertyRelative("sprite"),
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            if (XGUI.ChangedCheck_End())
            {
                xHud_RatioReference_Update();
                xHud_RatioReference_Switch();
            }

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 10));

            #region 比例参考图形
            XGUI.layout_property_field(
                title: "比例参考图形 - 尺寸",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 140,
                prop: sp_ReferShape_RatioSize,
                prop_margin: new RectOffset(0, 0, 10, 10));

            XGUI.layout_property_field(
                title: "比例参考图形 - 公差",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 140,
                prop: sp_ReferShape_RatioTolerance,
                prop_margin: new RectOffset(0, 0, 10, 10));
            #endregion

            if (XGUI.layout_button(
                text: "创建比例参考图形",
                tooltip: "",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(XHud_Dashboard.themes_get_color(ThemeColorType.primary)) ? Color.black : Color.white,
                press_fill: XGUIGroup.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.M,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 10),
                padding: new RectOffset(0, 0, 0, 0),
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                xHud_CreateReferenceShape(sp_ReferShape_RatioSize.vector2Value.x, sp_ReferShape_RatioSize.vector2Value.y, sp_ReferShape_RatioTolerance.vector2Value.x, sp_ReferShape_RatioTolerance.vector2Value.y);
                return;
            }
        }
        #endregion

        #region 工具 - 比例参考计算
        /// <summary>
        /// 比例参考标识物体 - 计算。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>根据物理屏幕尺寸与参考物尺寸换算出屏幕像素尺寸。</description></item>
        /// <item><description>将结果应用到 <c>Reference_Image</c> 的 <c>sizeDelta</c>。</description></item>
        /// </list>
        /// </summary>
        public void xHud_RatioReference_Calculate()
        {
            Vector2 res = ScreenRes.vector2Value;
            Vector2 screen_size = sp_PhysicsScreenSize.vector2Value;
            Vector2 refer_size = RatioReferenceIsPart.boolValue ? BaseScript.Res_Part.size : BaseScript.Res_Full.size;

            if (refer_size == Vector2.zero)
                return;

            float pw = refer_size.x / screen_size.x;
            float ph = refer_size.y / screen_size.y;

            float nw = pw * res.x;
            float nh = ph * res.y;

            BaseScript.Reference_Image.rectTransform.sizeDelta = new Vector2(nw, nh);
        }
        /// <summary>
        /// 比例参考标识物体 - 创建。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>创建 <c>ReferRatioMark</c> 物体并挂载 <c>Image</c>。</description></item>
        /// <item><description>设置锚点、着色与原始尺寸，并注册撤销。</description></item>
        /// </list>
        /// </summary>
        /// <param name="spr">参考图例精灵。</param>
        public void xHud_RatioReference_Create(Sprite spr)
        {
            Undo.SetCurrentGroupName("Create Image Operation");
            GameObject ref_hum_ima_obj = new GameObject();
            ref_hum_ima_obj.name = "ReferRatioMark";
            Image ref_hum_ima = ref_hum_ima_obj.AddComponent<Image>();
            ref_hum_ima.raycastTarget = false;
            ref_hum_ima.maskable = false;
            ref_hum_ima.sprite = spr;

            ref_hum_ima.rectTransform.SetParent(BaseScript.HudCanvas_Screen.transform);
            ref_hum_ima.rectTransform.SetAsLastSibling();

            ref_hum_ima.rectTransform.anchorMin = new Vector2(0.5f, 0);
            ref_hum_ima.rectTransform.anchorMax = new Vector2(0.5f, 0);
            ref_hum_ima.rectTransform.pivot = new Vector2(0.5f, 0);

            ref_hum_ima.rectTransform.localScale = Vector3.one;
            ref_hum_ima.rectTransform.localPosition = Vector3.zero;
            ref_hum_ima.rectTransform.localEulerAngles = Vector3.zero;
            ref_hum_ima.rectTransform.anchoredPosition3D = Vector3.zero;

            ref_hum_ima.color = sp_Reference_Image_Color.colorValue;

            ref_hum_ima.SetNativeSize();

            sp_Reference_Image.objectReferenceValue = ref_hum_ima;
            sp_Reference_Image.serializedObject.ApplyModifiedProperties();


            // 记录撤销操作
            Undo.RegisterCreatedObjectUndo(ref_hum_ima_obj, "Create Image");
            Undo.RecordObject(sp_Reference_Image.serializedObject.targetObject, "Update SerializedProperty");
            xHud_RatioReference_Update();
        }
        /// <summary>
        /// 比例参考标识物体 - 销毁。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>销毁对应的 Image 物体并清空序列化引用。</description></item>
        /// </list>
        /// </summary>
        public void xHud_RatioReference_Destroy()
        {
            if (sp_Reference_Image.objectReferenceValue == null)
                return;

            Image img = sp_Reference_Image.objectReferenceValue as Image;

            DestroyImmediate(img.gameObject, true);
            sp_Reference_Image.objectReferenceValue = null;
            sp_Reference_Image.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 切换参考类型。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>根据局部/整体模式切换参考图例的 Sprite。</description></item>
        /// </list>
        /// </summary>
        public void xHud_RatioReference_Switch()
        {
            if (sp_Reference_Image.objectReferenceValue != null)
            {
                Sprite spr = BaseScript.Res_Part.sprite;
                if (!RatioReferenceIsPart.boolValue)
                    spr = BaseScript.Res_Full.sprite;

                if (spr == null)
                    return;

                Image img = sp_Reference_Image.objectReferenceValue as Image;

                img.sprite = spr;
            }
        }
        /// <summary>
        /// 比例参考标识物体 - 刷新。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>同步着色与位置，并重新计算尺寸。</description></item>
        /// </list>
        /// </summary>
        public void xHud_RatioReference_Update()
        {
            if (!UseRatioReference.boolValue)
                return;
            if (sp_Reference_Image.objectReferenceValue != null)
            {
                BaseScript.Reference_Image.color = sp_Reference_Image_Color.colorValue;
                BaseScript.Reference_Image.rectTransform.anchoredPosition3D = Vector3.zero;
                xHud_RatioReference_Calculate();
            }
        }
        #endregion

        #region 工具 - 生成比例参考标记
        /// <summary>
        /// 生成比例参考标记物体。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>按物理尺寸换算为像素尺寸并创建 <c>XHud_Module_Element</c> 物体。</description></item>
        /// <item><description>创建子 Image 作为参考图形，创建子 Text 显示尺寸文字。</description></item>
        /// </list>
        /// </summary>
        /// <param name="width">参考物宽度（厘米）。</param>
        /// <param name="height">参考物高度（厘米）。</param>
        /// <param name="tolerance_w">宽度公差（厘米）。</param>
        /// <param name="tolerance_h">高度公差（厘米）。</param>
        private void xHud_CreateReferenceShape(float width, float height, float tolerance_w = 0, float tolerance_h = 0)
        {
            float o_w = width;
            float o_h = height;

            width += tolerance_w;
            height += tolerance_h;

            // 屏幕宽度（厘米）
            float screenWidthCm = sp_PhysicsScreenSize.vector2Value.x;
            // 屏幕高度（厘米）
            float screenHeightCm = sp_PhysicsScreenSize.vector2Value.y;

            // 获取屏幕分辨率
            float screenWidthPx = ScreenRes.vector2Value.x;
            float screenHeightPx = ScreenRes.vector2Value.y;

            // 计算目标宽度对应的像素值
            float imageWidthPx = (width / screenWidthCm) * screenWidthPx;

            // 计算目标高度对应的像素值
            float imageHeightPx = (height / screenHeightCm) * screenHeightPx;

            // 校准宽度或高度，确保比例一致
            float widthToHeightRatio = width / height;
            float actualWidthToHeightRatio = imageWidthPx / imageHeightPx;

            if (actualWidthToHeightRatio != widthToHeightRatio)
            {
                // 如果比例不一致，调整高度以匹配宽度
                imageHeightPx = imageWidthPx / widthToHeightRatio;
            }

            // 创建 Element 对象
            GameObject obj = new GameObject();
            Undo.RegisterCreatedObjectUndo(obj, $"CreateRealWorldReferShape - {o_w} cm x {o_h} cm");
            obj.name = $"RealWorldReferShape - {o_w} cm x {o_h} cm";
            obj.layer = LayerMask.NameToLayer("XHud");
            XHud_Module_Element obj_ele = obj.AddComponent<XHud_Module_Element>();
            obj_ele.Indicator = "RealWorldReferShape";
            UnityEngine.RectTransform obj_ele_rect = obj.GetComponent<UnityEngine.RectTransform>();
            obj_ele_rect.transform.SetParent(BaseScript.hm_ScreenElement_GetAnchored_RectTransform(XHudAnchor.顶层));
            obj_ele_rect.anchoredPosition3D = Vector3.zero;
            obj_ele_rect.localEulerAngles = Vector3.zero;
            obj_ele_rect.localScale = Vector3.one;

            // 创建 Image组件 对象
            GameObject image = new GameObject($"ReferShape");
            Image img = image.AddComponent<Image>();
            img.raycastTarget = false;
            img.maskable = false;
            img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/ReferShapeBG.png");
            img.type = Image.Type.Tiled;
            img.color = Color.white * 0.48f;
            img.pixelsPerUnitMultiplier = 1;
            img.rectTransform.SetParent(obj.transform);
            img.rectTransform.localScale = Vector3.one;
            img.rectTransform.localEulerAngles = Vector3.zero;
            img.rectTransform.localPosition = Vector3.zero;
            img.rectTransform.anchoredPosition3D = Vector3.zero;
            img.rectTransform.sizeDelta = new Vector2(imageWidthPx, imageHeightPx);

            GameObject textobj = new GameObject();
            textobj.transform.SetParent(obj.transform);
            textobj.name = "ValueText";
            textobj.transform.localScale = Vector3.one;
            textobj.transform.localEulerAngles = Vector3.zero;
            textobj.transform.localPosition = Vector3.zero;
            textobj.layer = LayerMask.NameToLayer("XHud");

            Font font = AssetDatabase.LoadAssetAtPath<Font>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Fonts/Text/sx_light.otf");

            UnityEngine.RectTransform rect = textobj.AddComponent<UnityEngine.RectTransform>();
            rect.sizeDelta = new Vector2(120, 30);
            rect.anchorMin = Vector2.one * 0.5f;
            rect.anchorMax = Vector2.one * 0.5f;
            rect.pivot = Vector2.one * 0.5f;
            rect.anchoredPosition3D = new Vector3(0, rect.anchoredPosition3D.y + img.rectTransform.sizeDelta.y / 2 + 30, 0);

            XHud_Module_Text hud_text = textobj.AddComponent<XHud_Module_Text>();
            hud_text.TextStyleInfo.txt_Set_Alignment(ContentAnchor.中心);
            hud_text.alignment = TextAnchor.MiddleCenter;
            hud_text.text = $"{o_w} cm x {o_h} cm";
            hud_text.TextStyleInfo.txt_Set_FontSize(18);
            hud_text.TextStyleInfo.txt_Set_FontColor(Color.white);
            hud_text.TextStyleInfo.txt_Set_Font(font);
            hud_text.font = font;
            hud_text.TextStyleInfo.txt_Set_FontStyle(FontStyle.Normal);
            hud_text.TextStyleInfo.txt_Set_Overflow(HorizontalWrapMode.Overflow);
            hud_text.TextStyleInfo.txt_Set_Overflow(HorizontalWrapMode.Overflow);
            hud_text.TextStyleInfo.gen_RayCastSet(false);

            Selection.activeObject = obj;
        }
        #endregion
    }
}