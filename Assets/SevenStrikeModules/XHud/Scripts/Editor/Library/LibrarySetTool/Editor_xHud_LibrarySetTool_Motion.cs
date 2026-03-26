namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
    using UnityEditor;
    using UnityEngine;

    public class Editor_XHud_LibrarySetTool_Motion : EditorWindow
    {
        private SerializedObject BaseObject;
        private SerializedProperty sp_LibName, sp_Description, sp_CreateArgs, sp_RecycleArgs, sp_OriginLibName, sp_motiontype, sp_OriginType;

        private HudElementMotionType Type;
        private Texture2D icon_libsetter_motion;

        private Texture2D icon_libsetter_motion_movement_r;
        private Texture2D icon_libsetter_motion_movement_p;
        private Texture2D icon_libsetter_motion_rotator_r;
        private Texture2D icon_libsetter_motion_rotator_p;
        private Texture2D icon_libsetter_motion_alpha_r;
        private Texture2D icon_libsetter_motion_alpha_p;
        private Texture2D icon_motionstoptim_refer;

        public bool Motiontype;

        public LibrarySetterMode LibrarySetterMode;

        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;

        [SerializeField]
        public string LibName;
        [SerializeField]
        public string DateTimes;
        [SerializeField]
        public string Description;
        [SerializeField]
        public string OriginLibName;
        [SerializeField]
        public HudElementMotionType OriginType;

        /// <summary>
        /// 按钮宽度
        /// </summary>
        private float ButtonWidth = 110;
        /// <summary>
        /// 按钮高度
        /// </summary>
        private float ButtonHeight = 25;
        /// <summary>
        /// 按钮间距
        /// </summary>
        private float ButtonDistance = 15;

        public string ButtonText_Ok;
        public string ButtonText_Cancel;

        private float IconSize = 35;

        Color SepLineColor = new Color(1, 1, 1, 0.15f);
        Color MessageColor = new Color(1, 1, 1, 0.62f);
        Color DateTimeColor = new Color(1, 1, 1, 0.42f);

        Rect Sepline_rect;
        Rect Title_rect;
        Rect Date_rect;
        Rect Icon_rect;

        public Motion_Creator CreateArgs;
        public Motion_Recycler RecycleArgs;

        public int ModifiedIndex;
        private XHud_Library_Motion Target_Hud_MotionLibrary;
        string Title;

        float prop_margin = 10;

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);
            sp_LibName = BaseObject.FindProperty("LibName");
            sp_Description = BaseObject.FindProperty("Description");
            sp_CreateArgs = BaseObject.FindProperty("CreateArgs");
            sp_RecycleArgs = BaseObject.FindProperty("RecycleArgs");
            sp_OriginLibName = BaseObject.FindProperty("OriginLibName");
            sp_motiontype = BaseObject.FindProperty("Motiontype");
            sp_OriginType = BaseObject.FindProperty("OriginType");

            icon_libsetter_motion = Editor_XHud_GUI.GetIcon("LibSetter/icon_libsetter_motion");

            icon_libsetter_motion_movement_r = Editor_XHud_GUI.GetIcon("LibSetter/icon_libsetter_motion_movement_r");
            icon_libsetter_motion_rotator_r = Editor_XHud_GUI.GetIcon("LibSetter/icon_libsetter_motion_rotator_r");
            icon_libsetter_motion_alpha_r = Editor_XHud_GUI.GetIcon("LibSetter/icon_libsetter_motion_alpha_r");
            icon_libsetter_motion_movement_p = Editor_XHud_GUI.GetIcon("LibSetter/icon_libsetter_motion_movement_p");
            icon_libsetter_motion_rotator_p = Editor_XHud_GUI.GetIcon("LibSetter/icon_libsetter_motion_rotator_p");
            icon_libsetter_motion_alpha_p = Editor_XHud_GUI.GetIcon("LibSetter/icon_libsetter_motion_alpha_p");
            icon_motionstoptim_refer = Editor_XHud_GUI.GetIcon("LibSetter/icon_motionstoptim_refer");

            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Dialog");

            Description = "动效参数说明内容";
            LibName = "动效名称";


            if (Type == HudElementMotionType.Creator)
                sp_motiontype.boolValue = false;
            else
                sp_motiontype.boolValue = true;

            sp_motiontype.serializedObject.ApplyModifiedProperties();

            MotionAnimateEndState maes = MotionAnimateEndState.以_透明度为准;
            SerializedProperty sp_MotionAnimateEndState;

            if (Type == HudElementMotionType.Creator)
            {
                sp_MotionAnimateEndState = sp_CreateArgs.FindPropertyRelative("MotionAnimateEndState");
            }
            else
            {
                sp_MotionAnimateEndState = sp_RecycleArgs.FindPropertyRelative("MotionAnimateEndState");
            }
            maes = (MotionAnimateEndState)sp_MotionAnimateEndState.enumValueIndex;
            switch (maes)
            {
                case MotionAnimateEndState.以_透明度为准:
                    mase_offset = new Vector2(120, 116);
                    break;
                case MotionAnimateEndState.以_移动为准:
                    mase_offset = new Vector2(320, 116);
                    break;
                case MotionAnimateEndState.以_旋转为准:
                    mase_offset = new Vector2(525, 116);
                    break;
            }
        }

        private void OnDestroy()
        {
            GetWindow<SceneView>().Focus();
        }

        Vector2 mase_offset;

        private void OnGUI()
        {
            BaseObject.Update();

            //动效动画结束基准状态
            SerializedProperty sp_MotionAnimateEndState_crc = sp_CreateArgs.FindPropertyRelative("MotionAnimateEndState");
            MotionAnimateEndState maes_crc = (MotionAnimateEndState)sp_MotionAnimateEndState_crc.enumValueIndex;

            SerializedProperty sp_MotionAnimateEndState_rec = sp_RecycleArgs.FindPropertyRelative("MotionAnimateEndState");
            MotionAnimateEndState maes_rec = (MotionAnimateEndState)sp_MotionAnimateEndState_rec.enumValueIndex;

            Rect rect = new Rect(0, 0, position.width, position.height);

            Icon_rect = new Rect(26, 15, 48, 48);

            Editor_XHud_GUI.Gui_Icon(Icon_rect, icon_libsetter_motion);

            Title_rect = new Rect(rect.x + 100, rect.y + 15, rect.width - 80, 30);
            Editor_XHud_GUI.Gui_Labelfield(Title_rect, Title, HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 20, Font_Bold, TextClipping.Ellipsis, true);

            Sepline_rect = new Rect(rect.x + 102, rect.y + 60, 200, 1);
            Editor_XHud_GUI.Gui_Box(Sepline_rect, SepLineColor);

            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(new Rect(rect.x + 26, rect.y + 80, rect.width - 45, rect.height), "以下为待入库的动效参数概览，您可以检查每项参数是否符合您的要求，每项参数均可手动校正调整！", HudFilled.无, HudColor.无, MessageColor, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true, Font_Light);

            DateTimes = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss:ff");
            Date_rect = new Rect(rect.x + 150, rect.y + 15, rect.width - 180, rect.height);
            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(Date_rect, DateTimes, HudFilled.无, HudColor.无, DateTimeColor, TextAnchor.UpperRight, new Vector2(0, 0), 13, true, Font_Light);

            #region 动效信息
            string colorhex = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);
            string typetext = Type == HudElementMotionType.Creator ? "生成" : "回收";
            float panel_verticle = 110;
            float panel_height = 270;
            float panel_center = (rect.width / 2);
            float panel_width = (rect.width / 3);
            float Panel_distance = 5;

            #region 面板定位
            Rect rect_movement = new Rect(panel_center - (panel_width + (panel_width / 2) + Panel_distance + 15), panel_verticle, panel_width, panel_height);
            //EditorGUI.DrawRect(rect_movement, 颜色_Color.gray * 0.4f);

            Rect rect_rotator = new Rect(panel_center - (panel_width / 2) - 20, panel_verticle, panel_width, panel_height);
            //EditorGUI.DrawRect(rect_rotator, 颜色_Color.gray * 0.4f);

            Rect rect_alpha = new Rect(panel_center + (panel_width / 2) + Panel_distance - 25, panel_verticle, panel_width, panel_height);
            //EditorGUI.DrawRect(rect_alpha, 颜色_Color.gray * 0.4f);
            #endregion

            float iconoffset = 30;

            #region 图标
            if (Editor_XHud_GUI.Gui_Button(new Rect(rect_movement.x + (rect_movement.width / 2) - (IconSize / 2) + iconoffset, rect_movement.y + 15, IconSize, IconSize), icon_libsetter_motion_movement_r, icon_libsetter_motion_movement_p, true, "", "", XHud_Dashboard.Theme_Primary))
            {
                if (Type == HudElementMotionType.Creator)
                {
                    sp_MotionAnimateEndState_crc.enumValueIndex = (int)MotionAnimateEndState.以_移动为准;
                }
                else
                {
                    sp_MotionAnimateEndState_rec.enumValueIndex = (int)MotionAnimateEndState.以_移动为准;
                }
                sp_MotionAnimateEndState_crc.serializedObject.ApplyModifiedProperties();
                sp_MotionAnimateEndState_rec.serializedObject.ApplyModifiedProperties();

                sp_CreateArgs.serializedObject.ApplyModifiedProperties();
                sp_RecycleArgs.serializedObject.ApplyModifiedProperties();
            }

            if (Editor_XHud_GUI.Gui_Button(new Rect(rect_rotator.x + (rect_rotator.width / 2) - (IconSize / 2) + iconoffset, rect_rotator.y + 15, IconSize, IconSize), icon_libsetter_motion_rotator_r, icon_libsetter_motion_rotator_p, true, "", "", XHud_Dashboard.Theme_Primary))
            {
                if (Type == HudElementMotionType.Creator)
                {
                    sp_MotionAnimateEndState_crc.enumValueIndex = (int)MotionAnimateEndState.以_旋转为准;
                }
                else
                {
                    sp_MotionAnimateEndState_rec.enumValueIndex = (int)MotionAnimateEndState.以_旋转为准;
                }
                sp_MotionAnimateEndState_crc.serializedObject.ApplyModifiedProperties();
                sp_MotionAnimateEndState_rec.serializedObject.ApplyModifiedProperties();

                sp_CreateArgs.serializedObject.ApplyModifiedProperties();
                sp_RecycleArgs.serializedObject.ApplyModifiedProperties();
            }

            if (Editor_XHud_GUI.Gui_Button(new Rect(rect_alpha.x + (rect_alpha.width / 2) - (IconSize / 2) + iconoffset, rect_alpha.y + 15, IconSize, IconSize), icon_libsetter_motion_alpha_r, icon_libsetter_motion_alpha_p, true, "", "", XHud_Dashboard.Theme_Primary))
            {
                if (Type == HudElementMotionType.Creator)
                {
                    sp_MotionAnimateEndState_crc.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                }
                else
                {
                    sp_MotionAnimateEndState_rec.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                }

                sp_MotionAnimateEndState_crc.serializedObject.ApplyModifiedProperties();
                sp_MotionAnimateEndState_rec.serializedObject.ApplyModifiedProperties();

                sp_CreateArgs.serializedObject.ApplyModifiedProperties();
                sp_RecycleArgs.serializedObject.ApplyModifiedProperties();
            }

            if (Type == HudElementMotionType.Creator)
            {
                switch (maes_crc)
                {
                    case MotionAnimateEndState.以_透明度为准:
                        mase_offset = new Vector2(525, 116);
                        break;
                    case MotionAnimateEndState.以_移动为准:
                        mase_offset = new Vector2(120, 116);
                        break;
                    case MotionAnimateEndState.以_旋转为准:
                        mase_offset = new Vector2(320, 116);
                        break;
                }
            }
            else
            {
                switch (maes_rec)
                {
                    case MotionAnimateEndState.以_透明度为准:
                        mase_offset = new Vector2(525, 116);
                        break;
                    case MotionAnimateEndState.以_移动为准:
                        mase_offset = new Vector2(120, 116);
                        break;
                    case MotionAnimateEndState.以_旋转为准:
                        mase_offset = new Vector2(320, 116);
                        break;
                }
            }

            Rect anc = new Rect(mase_offset.x, mase_offset.y, 12, 12);
            Editor_XHud_GUI.Gui_Icon(anc, icon_motionstoptim_refer);

            GUI.backgroundColor = Color.white;
            #endregion

            #region 参数
            float offset = 70;
            float margin = 25;
            float titlewidth = 45;

            if (Type == HudElementMotionType.Creator)
            {
                prop_margin = 0;
                PorpertyDrawer(sp_CreateArgs, "Movement.Movement", rect_movement, "方式", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "Movement.Distance", rect_movement, "距离", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "Movement.Duration", rect_movement, "耗时", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "Movement.Delay", rect_movement, "延迟", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "Movement.Curve", rect_movement, "曲线", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "Movement.Ease", rect_movement, "缓动", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "anchor", rect_movement, "锚点", offset, margin, titlewidth);

                prop_margin = 0;
                PorpertyDrawer(sp_CreateArgs, "Rotation.Rotation", rect_rotator, "方式", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "Rotation.Degree", rect_rotator, "角度", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "Rotation.Duration", rect_rotator, "耗时", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "Rotation.Delay", rect_rotator, "延迟", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "Rotation.Curve", rect_rotator, "曲线", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "Rotation.Ease", rect_rotator, "缓动", offset, margin, titlewidth);

                prop_margin = 0;
                PorpertyDrawer(sp_CreateArgs, "Alpha.Duration", rect_alpha, "耗时", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "Alpha.Delay", rect_alpha, "延迟", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "Alpha.Curve", rect_alpha, "曲线", offset, margin, titlewidth);
                PorpertyDrawer(sp_CreateArgs, "Alpha.Ease", rect_alpha, "缓动", offset, margin, titlewidth);
            }
            else
            {
                prop_margin = 0;
                PorpertyDrawer(sp_RecycleArgs, "Movement.Movement", rect_movement, "方式", offset, margin, titlewidth);
                PorpertyDrawer(sp_RecycleArgs, "Movement.Distance", rect_movement, "距离", offset, margin, titlewidth);
                PorpertyDrawer(sp_RecycleArgs, "Movement.Duration", rect_movement, "耗时", offset, margin, titlewidth);
                PorpertyDrawer(sp_RecycleArgs, "Movement.Delay", rect_movement, "延迟", offset, margin, titlewidth);
                PorpertyDrawer(sp_RecycleArgs, "Movement.Curve", rect_movement, "曲线", offset, margin, titlewidth);
                PorpertyDrawer(sp_RecycleArgs, "Movement.Ease", rect_movement, "缓动", offset, margin, titlewidth);

                prop_margin = 0;
                PorpertyDrawer(sp_RecycleArgs, "Rotation.Rotation", rect_rotator, "方式", offset, margin, titlewidth);
                PorpertyDrawer(sp_RecycleArgs, "Rotation.Degree", rect_rotator, "角度", offset, margin, titlewidth);
                PorpertyDrawer(sp_RecycleArgs, "Rotation.Duration", rect_rotator, "耗时", offset, margin, titlewidth);
                PorpertyDrawer(sp_RecycleArgs, "Rotation.Delay", rect_rotator, "延迟", offset, margin, titlewidth);
                PorpertyDrawer(sp_RecycleArgs, "Rotation.Curve", rect_rotator, "曲线", offset, margin, titlewidth);
                PorpertyDrawer(sp_RecycleArgs, "Rotation.Ease", rect_rotator, "缓动", offset, margin, titlewidth);

                prop_margin = 0;
                PorpertyDrawer(sp_RecycleArgs, "Alpha.Duration", rect_alpha, "耗时", offset, margin, titlewidth);
                PorpertyDrawer(sp_RecycleArgs, "Alpha.Delay", rect_alpha, "延迟", offset, margin, titlewidth);
                PorpertyDrawer(sp_RecycleArgs, "Alpha.Curve", rect_alpha, "曲线", offset, margin, titlewidth);
                PorpertyDrawer(sp_RecycleArgs, "Alpha.Ease", rect_alpha, "缓动", offset, margin, titlewidth);
            }


            string[] str_motiontype = new string[2] { "生成", "回收" };
            EditorGUI.BeginChangeCheck();
            sp_motiontype.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect.width - 160, rect.y + 300, 142, 50), false, str_motiontype, sp_motiontype.boolValue, HudFilled.无, HudColor.亮白, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, Color.black);
            sp_motiontype.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                if (sp_motiontype.boolValue)
                    Type = HudElementMotionType.Recycler;
                else
                    Type = HudElementMotionType.Creator;
            }

            #endregion

            Rect LibName_Rect = new Rect(rect.x + 25, rect.height - 175, (rect.width / 2) - 100, 70);
            Rect Description_Rect = new Rect((rect.width / 2) - 65, rect.height - 175, (rect.width / 2) + 45, 70);

            if (LibrarySetterMode != LibrarySetterMode.修改生成器项参数)
            {
                #region 入库名称
                Color LibName_color = Color.white;
                if (string.IsNullOrEmpty(sp_LibName.stringValue))
                {
                    LibName_color = Color.gray;
                    sp_LibName.stringValue = "动效名称";
                }
                else
                {
                    if (sp_LibName.stringValue == "动效名称")
                        LibName_color = Color.gray;
                    else
                        LibName_color = Color.white;
                }

                sp_LibName.stringValue = Editor_XHud_GUI.Gui_TextField(LibName_Rect, sp_LibName.stringValue, LibName_color, 12);
                sp_LibName.serializedObject.ApplyModifiedProperties();
                #endregion

                #region 说明文字
                Color Description_Color = Color.white;
                if (string.IsNullOrEmpty(sp_Description.stringValue))
                {
                    Description_Color = Color.gray;
                    sp_Description.stringValue = "动效参数说明内容";
                }
                else
                {
                    if (sp_Description.stringValue == "动效参数说明内容")
                        Description_Color = Color.gray;
                    else
                        Description_Color = Color.white;
                }

                sp_Description.stringValue = Editor_XHud_GUI.Gui_TextField(Description_Rect, sp_Description.stringValue, Description_Color, 12);
                sp_Description.serializedObject.ApplyModifiedProperties();

                #endregion
            }

            #endregion

            #region 动效类型

            if (LibrarySetterMode == LibrarySetterMode.修改生成器项参数)
            {
                if (Type == HudElementMotionType.Creator)
                {
                    Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.x + 15, rect.height - 100, 250, 20), "回收动效结束时机", sp_MotionAnimateEndState_crc, 10, 110);
                }
                else
                {
                    Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.x + 15, rect.height - 100, 250, 20), "生成动效结束时机", sp_MotionAnimateEndState_rec, 10, 110);
                }
            }
            else
            {
                if (Type == HudElementMotionType.Creator)
                {
                    Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.x + 15, rect.height - 96, 200, 20), "生成动效结束时机", sp_MotionAnimateEndState_crc, 10, 110);
                }
                else
                {
                    Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.x + 15, rect.height - 96, 200, 20), "回收动效结束时机", sp_MotionAnimateEndState_rec, 10, 110);
                }
            }

            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 25, rect.height - 70, 200, 15), $"动效类型：<color={colorhex}>{Type.ToString()}  ( {typetext} )</color>", HudFilled.无, HudColor.无, Color.white, false, Color.blue, TextAnchor.UpperLeft, Vector2.zero, 16, Font_Bold);

            if (Type == HudElementMotionType.Creator)
                Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 25, rect.height - 40, 200, 15), "该参数用于元素被生成时的进入动效", HudFilled.无, HudColor.无, Color.white * 0.7f, false, Color.blue, TextAnchor.UpperLeft, Vector2.zero, 12, Font_Light);
            else
                Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 25, rect.height - 40, 200, 15), "该参数用于元素被回收时的离开动效", HudFilled.无, HudColor.无, Color.white * 0.7f, false, Color.blue, TextAnchor.UpperLeft, Vector2.zero, 12, Font_Light);
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(476);

            Repaint();

            BaseObject.ApplyModifiedProperties();

            DialogType_Buttons();

            Event e = Event.current;

            // 检测点击事件
            if (e.type == EventType.MouseDown)
            {
                // 检查点击位置是否在窗口内
                if (!Description_Rect.Contains(e.mousePosition))
                {
                    GUI.FocusControl(null); // 取消所有控件的焦点
                    Repaint(); // 重新绘制窗口
                }

                // 检查点击位置是否在窗口内
                if (!LibName_Rect.Contains(e.mousePosition))
                {
                    GUI.FocusControl(null); // 取消所有控件的焦点
                    Repaint(); // 重新绘制窗口
                }

            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }

        }

        private void PorpertyDrawer(SerializedProperty prop, string propname, Rect rect, string Title, float offset, float margin, float TitleWidth)
        {
            SerializedProperty sp_prop = prop.FindPropertyRelative(propname);
            Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.x, rect.y + prop_margin + offset, rect.width, 20), Title, sp_prop, TitleWidth, rect.width - TitleWidth, 20);
            prop_margin += margin;
        }

        /// <summary>
        /// 发送到库
        /// </summary>
        private void SendToLibrary()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            if (sp_LibName.stringValue == "动效名称")
            {
                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud 动效库采集器消息", "未填写名称", "请为动效模版添加一个名称！", "明白");
                return;
            }

            string colorhex = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            bool exist = Target_Hud_MotionLibrary.ElementMotion_IsExist(sp_LibName.stringValue, Type);

            if (exist)
            {
                string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud 动效库采集器消息", "存在重复动效名称", $"名称为<color={colorhex}> {sp_LibName.stringValue} </color>的已经存在于动效库中！", "重命名", 1);
                if (res == "重命名")
                {
                    return;
                }
            }
            else
            {
                XHud.XHud_LibraryArg_Motion motion = new XHud.XHud_LibraryArg_Motion();
                motion.Name = sp_LibName.stringValue;
                motion.Crc = CreateArgs;
                motion.Rec = RecycleArgs;
                motion.Des = sp_Description.stringValue;
                motion.Mode = (int)Type;
                Target_Hud_MotionLibrary.ElementMotion_Add(motion);

                Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud 动效库采集器消息", "已添加到动效库", $"已将名称为<color={colorhex}> {sp_LibName.stringValue} </color>的动效参数添加到动效库中！", "明白");
                Close();
            }
        }

        /// <summary>
        /// 从库更新
        /// </summary>
        private void UpdateToLibrary()
        {
            string colorhex = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            XHud.XHud_LibraryArg_Motion motion = new XHud.XHud_LibraryArg_Motion(sp_LibName.stringValue, (int)Type, sp_Description.stringValue, CreateArgs, RecycleArgs);
            Target_Hud_MotionLibrary.ElementMotion_ReplaceMotion(ModifiedIndex, motion);

            Close();
        }

        /// <summary>
        /// 设置动效- 生成
        /// </summary>
        /// <param name="eles"></param>
        public void SetElementMotion(Motion_Creator creator)
        {
            Type = HudElementMotionType.Creator;
            Motiontype = false;
            CreateArgs = creator;
        }

        /// <summary>
        /// 设置动效 - 回收
        /// </summary>
        /// <param name="eles"></param>
        public void SetElementMotion(Motion_Recycler recycle)
        {
            Type = HudElementMotionType.Recycler;
            Motiontype = true;
            RecycleArgs = recycle;
        }

        /// <summary>
        /// 设置动效 - 全部
        /// </summary>
        public void SetElementMotion(Motion_Creator creator, Motion_Recycler recycle)
        {
            CreateArgs = creator;
            RecycleArgs = recycle;
        }

        /// <summary>
        /// 设置按钮文字
        /// </summary>
        /// <param name="ok"></param>
        /// <param name="cancel"></param>
        public void SetButtonText(string ok, string cancel)
        {
            ButtonText_Cancel = cancel;
            ButtonText_Ok = ok;
        }

        /// <summary>
        /// 设置按钮文字
        /// </summary>
        /// <param name="ok"></param>
        public void SetButtonText(string ok)
        {
            ButtonText_Ok = ok;
        }

        /// <summary>
        /// 设置模版基础信息（针对从库源修改更新使用）
        /// </summary>
        /// <param name="libname"></param>
        /// <param name="decription"></param>
        public void SetInfo(string libname, string decription)
        {
            LibName = libname;
            Description = decription;
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            Title = title;
        }

        /// <summary>
        /// 设置为添加到库模式还是修改库源参数模式
        /// </summary>
        /// <param name="mode"></param>
        public void SetLibrarySetterMode(LibrarySetterMode mode)
        {
            LibrarySetterMode = mode;
        }

        /// <summary>
        /// 控件按钮
        /// </summary>
        private void DialogType_Buttons()
        {
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();

            if (LibrarySetterMode != LibrarySetterMode.修改生成器项参数)
            {
                if (Editor_XHud_GUI.Gui_Layout_Button(ButtonText_Cancel, "", HudFilled.实体, HudColor.亮白, Color.black, 12, ButtonWidth, ButtonHeight, Font_Light, ButtonText_Cancel))
                {
                    Close();
                }
                Editor_XHud_GUI.Gui_Layout_Space(ButtonDistance);
            }
            GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
            if (Editor_XHud_GUI.Gui_Layout_Button(ButtonText_Ok, "", HudFilled.实体, HudColor.亮白, Color.black, 12, ButtonWidth, ButtonHeight, Font_Light, ButtonText_Ok))
            {
                if (LibrarySetterMode == LibrarySetterMode.添加到库)
                {
                    SendToLibrary();
                }
                else if (LibrarySetterMode == LibrarySetterMode.修改库源参数)
                {
                    UpdateToLibrary();
                }
                else if (LibrarySetterMode == LibrarySetterMode.修改生成器项参数)
                {
                    Close();
                }
                return;
            }
            GUI.backgroundColor = Color.white;

            Editor_XHud_GUI.Gui_Layout_Space(25);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
        }

        public void SetOriginLibName(string name)
        {
            OriginLibName = name;
        }

        public void SetOriginType(HudElementMotionType type)
        {
            OriginType = type;
        }

        public void SetTarget_Hud_MotionLibrary(XHud_Library_Motion lib)
        {
            Target_Hud_MotionLibrary = lib;
        }
    }
}
