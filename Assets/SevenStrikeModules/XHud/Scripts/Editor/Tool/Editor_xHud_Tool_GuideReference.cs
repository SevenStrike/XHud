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
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public class GuideReferDataItem
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public string abbr;
        [SerializeField]
        public string des;
    }

    [Serializable]
    public class GuideReferDataList
    {
        [SerializeField]
        public List<GuideReferDataItem> GuideReferDatas;
    }

    public class Editor_XHud_Tool_GuideReference : EditorWindow
    {
        private SerializedObject BaseObject;

        private Texture2D logo, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p;

        public GuideReferDataList GuideReferDataList;

        [SerializeField]
        private Texture2D[] ReferImages;

        public string GuideType = "垂直对称";
        private int referIndex = 0;

        public float refer_img_overflow = 35;

        // 构图参考说明 - 简要名称
        string abbr = "";
        // 构图参考说明 - 说明内容
        string des = "";

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);

            logo = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_guide_description/logo");
            left_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_guide_description/left_arrow_r");
            left_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_guide_description/left_arrow_p");
            right_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_guide_description/right_arrow_r");
            right_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_guide_description/right_arrow_p");
        }
        private void LoadReferDes()
        {
            // 读取配置表
            TextAsset value = (TextAsset)AssetDatabase.LoadAssetAtPath($"{XHud_Dashboard.Get_Path_XHUD_CONFIG_Path()}XHudGuideRefer.json", typeof(TextAsset));
            GuideReferDataList = JsonUtility.FromJson<GuideReferDataList>(value.text);

            // 匹配参考说明内容
            for (int i = 0; i < GuideReferDataList.GuideReferDatas.Count; i++)
            {
                if (GuideReferDataList.GuideReferDatas[i].name == GuideType)
                {
                    abbr = GuideReferDataList.GuideReferDatas[i].abbr;
                    des = GuideReferDataList.GuideReferDatas[i].des;
                    break;
                }
            }
        }

        private void LoadReferImages(string type)
        {
            if (ReferImages == null)
            {
                ReferImages = new Texture2D[5];
                for (int i = 0; i < ReferImages.Length; i++)
                {
                    ReferImages[i] = AssetDatabase.LoadAssetAtPath<Texture2D>($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_guide_description/ReferImages/{type}/ReferImg_{i}.png");
                }
            }
        }

        private void OnDestroy()
        {
            GetWindow<SceneView>().Focus();
        }

        private void OnGUI()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            BaseObject.Update();

            // 关键文字主题色
            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            #region 抬头
            Rect rect = new Rect(0, 0, position.width, position.height);

            // 图标
            Rect rect_icon = new Rect(15, 15, logo.width, logo.height);
            XGUI.gui_icon(
                rect: rect_icon,
                icon: logo,
                padding: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                color: Color.white);

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            // 大标题
            Rect rect_title = new Rect(rect.x + 70, rect.y + 10, rect.width - 80, 30);
            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent("XHud 构图参考说明书"),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping,
                font: XGUI.GetFont("xg-heavy"));

            // 分割线
            Rect rect_seperate = new Rect(rect.x + 68, rect.y + 43, 200, 1);
            XGUI.gui_seperator(
                rect: rect_seperate,
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            // 小标题
            Rect rect_subtitle = new Rect(rect.x + 18, rect.y + 52, rect.width - 18, 30);
            XGUI.gui_label(
                rect: rect_subtitle,
                text: new GUIContent("此窗口提供了各种构图参考线的具体解释，用户可根据每种构图线的释义来选择适合的构图参考类型"),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                wrap: true,
                anchor: TextAnchor.UpperLeft,
                clipping: TextClipping.Overflow,
                padding: new RectOffset(0, 10, 0, 0));
            #endregion

            XGUI.layout_space(105);

            #region 参考图片 / 参数
            if (ReferImages == null)
                LoadReferImages(GuideType);

            Texture2D icon = ReferImages[referIndex];
            float ratio = (float)icon.width / icon.height;  // 注意要显式转换为float

            // 计算保持宽高比的高度
            float targetWidth = position.width - refer_img_overflow;
            float targetHeight = targetWidth / ratio;

            Rect rect_refer = XGUI.GetControlRect(false, targetHeight);
            rect_refer.Set(rect_refer.x + 15, rect_refer.y, targetWidth, targetHeight);

            // 图标
            XGUI.gui_icon(
                rect: rect_refer,
                icon: icon,
                color: Color.white);

            #region 图片控件
            // 左翻页
            if (XGUI.gui_button(
                rect: new Rect(rect_refer.x - 5, rect_refer.y + (rect_refer.height + 5), 32, 32),
                tooltip: "收藏的预设",
                tex_release: left_arrow_r,
                tex_press: left_arrow_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                focus_name: "prev_btn"))
            {
                if (referIndex <= 0)
                    referIndex = ReferImages.Length - 1;
                else
                    referIndex--;
            }

            // 右翻页
            if (XGUI.gui_button(
               rect: new Rect(rect_refer.x + (rect_refer.width - 25), rect_refer.y + (rect_refer.height + 5), 32, 32),
               tooltip: "收藏的预设",
               tex_release: right_arrow_r,
               tex_press: right_arrow_p,
               tex_gui_color: Color.white,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 0),
               focus_name: "prev_btn"))
            {
                if (referIndex >= ReferImages.Length - 1)
                    referIndex = 0;
                else
                    referIndex++;
            }
            #endregion

            // 构图参考说明 - 构图类型标题
            XGUI.gui_label(
                rect: new Rect(rect_refer.x, rect_refer.y + (rect_refer.height + 12), rect_refer.width, XGUI.GetSingleLineHeight()),
                text: new GUIContent($"{GuideType}    <size=11>( <color={hexcol}>{abbr}</color> )</size>"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleCenter,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(15, 15, 45, 10));

            #region 图片说明
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.无,
                bg_color: XGUIColor.无,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            XGUI.layout_label(
                text: des,
                size: XGUIFontSize.M,
                text_color: Color.white * 0.8f,
                margin: new RectOffset(15, 15, 10, 0),
                clipping: TextClipping.Overflow,
                wrap: true,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleLeft);

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            BaseObject.ApplyModifiedProperties();

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }

            return;
        }

        /// <summary>
        /// 设置参考说明类型
        /// </summary>
        /// <param name="type"></param>
        public void SetGuideType(string type)
        {
            GuideType = type;
            LoadReferDes();
        }
    }
}