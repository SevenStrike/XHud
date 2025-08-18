namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;
    using static SevenStrikeModules.XHud.Hud.Hud_CameraCapture;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Hud_CameraCapture))]
    public class Editor_Hud_CameraCapture : Editor
    {
        #region 组件 / 列表
        private Hud_CameraCapture BaseScript;
        #endregion

        private bool BasicVars;

        #region 序列化属性
        private SerializedProperty
            TargetCamera,
            x_sizemode,
            x_mode,
            x_type,
            x_bgtype,
            Pixels,
            x_path,
            x_name,
            x_UICamera_bgcolor,
            x_UICamera_bgcolor_Original;
        #endregion

        #region 图标
        private Texture2D
            icon_main,
            opt_0_r,
            opt_0_p,
            opt_1_r,
            opt_1_p,
            opt_2_r,
            opt_2_p;
        #endregion

        #region 选项文字
        string[] str_x_mode = new string[2] { "场景相机", "UI相机" };
        string[] str_x_type = new string[3] { "JPG", "PNG", "TGA" };
        string[] str_x_size = new string[3] { "相机尺寸", "屏幕分辨率", "固定尺寸" };
        string[] str_x_bgtype = new string[3] { "天空盒", "颜色", "透明" };
        #endregion

        #region 批量化操作
        private Hud_CameraCapture[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new Hud_CameraCapture[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (Hud_CameraCapture)t;
                }
            }
            else
            {
                SelectedObjects = new Hud_CameraCapture[targets.Length];
                SelectedObjects[0] = (Hud_CameraCapture)target;
            }
        }

        private bool IsMultiSelected()
        {
            if (SelectedObjects == null)
                return false;
            if (SelectedObjects.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        private void OnEnable()
        {
            BaseScript = (Hud_CameraCapture)target;

            GetAllTargets();

            #region 获取变量
            TargetCamera = serializedObject.FindProperty("TargetCamera");
            x_sizemode = serializedObject.FindProperty("x_sizemode");
            x_mode = serializedObject.FindProperty("x_mode");
            x_type = serializedObject.FindProperty("x_type");
            Pixels = serializedObject.FindProperty("Pixels");
            x_path = serializedObject.FindProperty("x_path");
            x_bgtype = serializedObject.FindProperty("x_bgtype");
            x_name = serializedObject.FindProperty("x_name");
            x_UICamera_bgcolor = serializedObject.FindProperty("x_UICamera_bgcolor");
            x_UICamera_bgcolor_Original = serializedObject.FindProperty("x_UICamera_bgcolor_Original");
            #endregion

            #region 获取图标
            icon_main = util_XHUDGUI.GetIcon("Icons_Hud_CameraCapture/icon_main");
            opt_0_r = util_XHUDGUI.GetIcon("Icons_Hud_CameraCapture/opt_0_r");
            opt_0_p = util_XHUDGUI.GetIcon("Icons_Hud_CameraCapture/opt_0_p");
            opt_1_r = util_XHUDGUI.GetIcon("Icons_Hud_CameraCapture/opt_1_r");
            opt_1_p = util_XHUDGUI.GetIcon("Icons_Hud_CameraCapture/opt_1_p");
            opt_2_r = util_XHUDGUI.GetIcon("Icons_Hud_CameraCapture/opt_2_r");
            opt_2_p = util_XHUDGUI.GetIcon("Icons_Hud_CameraCapture/opt_2_p");
            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region 标题
            util_XHUDGUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "Hud - 相机截图捕捉", Color.white);
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);

            #region 相机指定
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "相机", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(5);

            #region 截图相机
            util_XHUDGUI.Gui_Layout_Property_Field("截图相机", TargetCamera);
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);

            if (x_mode.enumValueIndex == (int)CaptureCameraType.场景相机)
                EditorGUILayout.HelpBox("注意！如果您开启了PostProcessing模式则场景背景则会丢失透明通道！", MessageType.Info);

            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(5);

            #region 相机类型            
            util_XHUDGUI.Gui_Layout_Popup<int, Hud_CameraCapture>("相机类型", str_x_mode, ref x_mode, HudFilled.实体, 120, 22, SelectedObjects);
            #endregion

            #region 截图格式            
            util_XHUDGUI.Gui_Layout_Popup<int, Hud_CameraCapture>("截图格式", str_x_type, ref x_type, HudFilled.实体, 120, 22, SelectedObjects);
            #endregion

            #region 截图背景
            if (x_mode.enumValueIndex == (int)CaptureCameraType.场景相机)
                util_XHUDGUI.Gui_Layout_Popup<int, Hud_CameraCapture>("截图背景", str_x_bgtype, ref x_bgtype, HudFilled.实体, 120, 22, SelectedObjects);
            #endregion

            #region 截图尺寸            
            util_XHUDGUI.Gui_Layout_Popup<int, Hud_CameraCapture>("截图尺寸", str_x_size, ref x_sizemode, HudFilled.实体, 120, 22, SelectedObjects);
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 参数
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(5);
            #region 固定尺寸
            if (x_sizemode.intValue == 2)
            {
                util_XHUDGUI.Gui_Layout_Property_Field("固定尺寸", Pixels);
            }
            #endregion
            util_XHUDGUI.Gui_Layout_Space(5);
            #region UI截图背景颜色
            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            util_XHUDGUI.Gui_Layout_Space(10);
            if (util_XHUDGUI.Gui_Layout_Button(30, "", opt_0_r, opt_0_p))
            {
                Color colorValue = x_UICamera_bgcolor.colorValue;
                colorValue.a = 0;
                x_UICamera_bgcolor.colorValue = colorValue;
            }
            util_XHUDGUI.Gui_Layout_FlexSpace();
            if (util_XHUDGUI.Gui_Layout_Button(30, "", opt_1_r, opt_1_p))
            {
                Color colorValue = x_UICamera_bgcolor.colorValue;
                colorValue.a = 0.5f;
                x_UICamera_bgcolor.colorValue = colorValue;
            }
            util_XHUDGUI.Gui_Layout_FlexSpace();
            if (util_XHUDGUI.Gui_Layout_Button(30, "", opt_2_r, opt_2_p))
            {
                Color colorValue = x_UICamera_bgcolor.colorValue;
                colorValue.a = 1;
                x_UICamera_bgcolor.colorValue = colorValue;
            }
            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Horizontal_End();

            util_XHUDGUI.Gui_Layout_Space(5);

            util_XHUDGUI.Gui_Layout_Property_Field("截图背景色", x_UICamera_bgcolor);

            #endregion
            util_XHUDGUI.Gui_Layout_Space(5);
            #region 截图路径
            util_XHUDGUI.Gui_Layout_Property_Field("截图路径", x_path);
            #endregion
            util_XHUDGUI.Gui_Layout_Space(5);
            #region 文件名
            util_XHUDGUI.Gui_Layout_Property_Field("文件名", x_name);
            #endregion
            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 立即截图
            util_XHUDGUI.Gui_Layout_Space(5);
            if (util_XHUDGUI.Gui_Layout_Button("立即截图", "", HudFilled.实体, HudColor.深空灰, Color.white, 35, new RectOffset(), new Vector2(0, 0)))
            {
                if (TargetCamera.objectReferenceValue == null)
                {
                    util_XHUDGUI.Open(XHudDialogType.警告, "CameraCapture消息", "相机缺失", "您是否忘了指定目标相机了？", "明白");
                    return;
                }

                Camera cam = (Camera)TargetCamera.objectReferenceValue;

                UniversalAdditionalCameraData universal = cam.GetComponent<UniversalAdditionalCameraData>();
                if (universal.renderType == CameraRenderType.Base && x_mode.enumValueIndex == (int)CaptureCameraType.UI相机)
                {
                    string res = util_XHUDGUI.Open(XHudDialogType.警告, "CameraCapture消息", "类型匹配错误", "目标相机和您选择的要进行截图的相机类型选项不匹配！目标相机模式为 \"Base\" 基础态，但是您选择的 \"相机类型\" 是 \"UI相机\"，这就是矛盾的地方...", "暂不", "快速修正", 1);
                    if (res == "快速修正")
                    {
                        x_mode.enumValueIndex = (int)CaptureCameraType.场景相机;
                        x_mode.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        return;
                    }
                }
                if (universal.renderType == CameraRenderType.Overlay && x_mode.enumValueIndex == (int)CaptureCameraType.场景相机)
                {
                    string res = util_XHUDGUI.Open(XHudDialogType.警告, "CameraCapture消息", "类型匹配错误", "目标相机和您选择的要进行截图的相机类型选项不匹配！目标相机模式为 \"Overlay\" 叠加态，但是您选择的 \"相机类型\" 是 \"场景相机\"，这就是矛盾的地方...", "暂不", "快速修正", 1);
                    if (res == "快速修正")
                    {
                        x_mode.enumValueIndex = (int)CaptureCameraType.UI相机;
                        x_mode.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        return;
                    }
                }

                BaseScript.CaptureNow();

                util_XHUDGUI.Open(XHudDialogType.通知, "CameraCapture消息", "截图捕捉完成", $"已将 {str_x_mode[x_mode.intValue]} 的画面数据按照 {str_x_size[x_sizemode.intValue]} 模式保存格式为 {str_x_type[x_type.intValue]} 图片到路径： {x_path.stringValue}", "明白");
            }
            #endregion           

            #region 源脚本
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(5);
            #region 原始变量
            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            util_XHUDGUI.Gui_Layout_Space(10);
            BasicVars = EditorGUILayout.Foldout(BasicVars, "变量/属性", true);
            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Horizontal_End();
            if (BasicVars)
                DrawDefaultInspector();
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}