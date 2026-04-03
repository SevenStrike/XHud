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
    using SevenStrikeModules.XHud.Utilitys;
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

        private static Editor_XHud_Tool_GuideReference window;

        private Texture2D logo, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p;

        public GuideReferDataList GuideReferDataList;

        [SerializeField]
        private Texture2D[] ReferImages;

        Color SepLineColor = new Color(1, 1, 1, 0.15f);
        Color MessageColor = new Color(1, 1, 1, 0.62f);

        Rect Sepline_rect;
        Rect Title_rect;
        Rect Icon_rect;

        public string GuideType = "垂直对称";
        private int referIndex = 0;

        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);

            logo = Editor_XHud_GUI.GetIcon("Icons_XHud_GuideRefDescription/logo");

            left_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_GuideRefDescription/left_arrow_r");
            left_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_GuideRefDescription/left_arrow_p");
            right_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_GuideRefDescription/right_arrow_r");
            right_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_GuideRefDescription/right_arrow_p");

            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Dialog");

            // 读取配置表
            TextAsset value = (TextAsset)AssetDatabase.LoadAssetAtPath($"{XHud_Dashboard.Get_Path_XHUD_CONFIG_Path()}XHudGuideRefer.json", typeof(TextAsset));
            GuideReferDataList = JsonUtility.FromJson<GuideReferDataList>(value.text);
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
            string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            #region 抬头
            Rect rect = new Rect(0, 0, position.width, position.height);

            Icon_rect = new Rect(15, 15, 48, 48);

            Editor_XHud_GUI.Gui_Icon(Icon_rect, logo);

            Title_rect = new Rect(rect.x + 85, rect.y + 15, rect.width - 80, 30);
            Editor_XHud_GUI.Gui_Labelfield(Title_rect, $"XHud 构图参考说明书", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 20, Font_Bold);

            Sepline_rect = new Rect(rect.x + 85, rect.y + 60, 200, 1);
            Editor_XHud_GUI.Gui_Box(Sepline_rect, SepLineColor);

            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(new Rect(rect.x + 18, rect.y + 80, rect.width - 38, rect.height), "此窗口提供了各种构图参考线的具体解释，用户可根据每种构图线的释义来选择适合您的构图参考类型！", HudFilled.无, HudColor.无, MessageColor, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true, Font_Light);
            #endregion

            if (ReferImages == null)
            {
                ReferImages = new Texture2D[5];
                for (int i = 0; i < ReferImages.Length; i++)
                {
                    ReferImages[i] = AssetDatabase.LoadAssetAtPath<Texture2D>($"{XHud_Dashboard.Get_Path_XHUD_GUISTYLE_Path()}Icon/Icons_XHud_GuideRefDescription/ReferImages/{GuideType}/ReferImg_{i}.png");
                }
            }

            #region 参考图片 / 参数
            // 图标
            Editor_XHud_GUI.Gui_Icon(new Rect(rect.x + 18, rect.y + 130, 500, 500), ReferImages[referIndex]);

            #region 图片控件
            // 左翻页
            if (Editor_XHud_GUI.Gui_Button(new Rect(rect.x + 20, rect.y + 580, 32, 32), left_arrow_r, left_arrow_p, true, "", "", Color.white))
            {
                if (referIndex <= 0)
                    referIndex = ReferImages.Length - 1;
                else
                    referIndex--;
            }
            // 右翻页
            if (Editor_XHud_GUI.Gui_Button(new Rect(rect.x + 480, rect.y + 580, 32, 32), right_arrow_r, right_arrow_p, true, "", "", Color.white))
            {
                if (referIndex >= ReferImages.Length - 1)
                    referIndex = 0;
                else
                    referIndex++;
            }
            #endregion

            // 分割线
            Editor_XHud_GUI.Gui_Box(new Rect(rect.x + 545, rect.y + 150, 1, 350), Color.gray * 0.65f);

            // 构图参考说明 - 简要名称
            string abbr = "";
            // 构图参考说明 - 说明内容
            string des = "";

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

            // 构图参考说明 - 构图类型标题
            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 570, rect.y + 130, 448, 30), $"{GuideType}  <size=14>( <color={hexcol}>{abbr}</color> )</size>", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, 25, true, Font_Bold);

            // 构图参考说明 - 构图类型解释
            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 570, rect.y + 180, 345, 20), des, HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.UpperLeft, 13, Font_Light, true, TextClipping.Overflow);
            #endregion

            BaseObject.ApplyModifiedProperties();

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }
        }

        /// <summary>
        /// 设置参考说明类型
        /// </summary>
        /// <param name="type"></param>
        public void SetGuideType(string type)
        {
            GuideType = type;
        }
    }
}