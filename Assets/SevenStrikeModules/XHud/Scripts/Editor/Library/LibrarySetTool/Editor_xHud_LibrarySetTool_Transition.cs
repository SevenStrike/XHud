namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class Editor_XHud_LibrarySetTool_Transition : EditorWindow
    {
        private SerializedObject BaseObject;
        private SerializedProperty
            sp_TransitionNode,
            sp_TransitionName,
            sp_TransitionDescription,
            sp_Preview_Texture_Bg,
            sp_Preview_Color,
            sp_BgGrid_Tilling_multiply,
            sp_BgGrid_Color_A,
            sp_BgGrid_Color_B,
            sp_itemHeight,
            sp_visibleItemCount,
            sp_SelectedIndex,
            sp_TransitionInfoList_Scroller;

        private ReorderableList TrasitionInfoList;

        private Texture2D icon_libsetter_transition;

        private Texture2D leftarr_r;
        private Texture2D leftarr_p;
        private Texture2D rightarr_r;
        private Texture2D rightarr_p;
        private Texture2D stop_r;
        private Texture2D stop_p;
        private Texture2D play_r;
        private Texture2D play_p;

        private int PreviewData_Index;
        private bool IsPreviewing;
        private bool StoppedResetMode = true;
        private EditorCoroutine PreviewCoroutine;
        private float PreviewDataDuration = 0.035f;
        /// <summary>
        /// 反色通道
        /// </summary>
        private bool InvertChannel = false;

        private Material Preview_Material;
        [SerializeField]
        private Texture2D Preview_Texture_Bg;
        [SerializeField]
        private Color Preview_Color = Color.white;
        [SerializeField]
#pragma warning disable CS0414
        private float BgGrid_Tilling_multiply = 14;
#pragma warning restore CS0414
        [SerializeField]
        private float BgGrid_Tilling_x = 14;
        [SerializeField]
        private float BgGrid_Tilling_y = 14;
        [SerializeField]
        private Color BgGrid_Color_A = Color.white * 0.509f;
        [SerializeField]
        private Color BgGrid_Color_B = Color.white * 0.376f;

        [SerializeField]
        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 25;
        [SerializeField]
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 19;
        [SerializeField]
        /// <summary>
        /// 选中项索引号
        /// </summary>
        public int SelectedIndex;
        [SerializeField]
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 TransitionInfoList_Scroller;

        public int ModifiedIndex;

        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;

        [SerializeField]
        public string TransitionName;
        [SerializeField]
        public string DateTimes;
        [SerializeField]
        public string TransitionDescription;

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

        [SerializeField]
        public string ButtonText_Ok;
        [SerializeField]
        public string ButtonText_Cancel;

        [SerializeField]
        private XHud_LibraryArg_Transition TransitionNode;
        [SerializeField]
        private XHud_LibraryArg_Transition OriginTransitionNode;

        private Color SepLineColor = new Color(1, 1, 1, 0.15f);
        private Color MessageColor = new Color(1, 1, 1, 0.62f);
        private Color DateTimeColor = new Color(1, 1, 1, 0.42f);

        private Rect draw_rect;
        private Rect dragarea;
        private XHud_Library_Transition Target_Hud_TransitionLibrary;
        private string Title;

        private void OnDisable()
        {
            Editor_XHud_GUI.EditorData_Set_With_String("XED_Transition_Bg_Grid_ColorA", XHud_Utilitys.Color_To_String(sp_BgGrid_Color_A.colorValue));
            Editor_XHud_GUI.EditorData_Set_With_String("XED_Transition_Bg_Grid_ColorB", XHud_Utilitys.Color_To_String(sp_BgGrid_Color_B.colorValue));
            Editor_XHud_GUI.EditorData_Set_With_String("XED_Transition_Bg_Grid_Tilling", sp_BgGrid_Tilling_multiply.floatValue.ToString("F2"));
            Editor_XHud_GUI.EditorData_Set_With_String("XED_Transition_Bg_Path", AssetDatabase.GetAssetPath(sp_Preview_Texture_Bg.objectReferenceValue));
        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);

            sp_TransitionNode = BaseObject.FindProperty("TransitionNode");
            sp_TransitionName = BaseObject.FindProperty("TransitionName");
            sp_TransitionDescription = BaseObject.FindProperty("TransitionDescription");

            sp_Preview_Texture_Bg = BaseObject.FindProperty("Preview_Texture_Bg");
            sp_BgGrid_Tilling_multiply = BaseObject.FindProperty("BgGrid_Tilling_multiply");
            sp_BgGrid_Color_A = BaseObject.FindProperty("BgGrid_Color_A");
            sp_BgGrid_Color_B = BaseObject.FindProperty("BgGrid_Color_B");
            sp_Preview_Color = BaseObject.FindProperty("Preview_Color");

            sp_itemHeight = BaseObject.FindProperty("itemHeight");
            sp_visibleItemCount = BaseObject.FindProperty("visibleItemCount");
            sp_SelectedIndex = BaseObject.FindProperty("SelectedIndex");
            sp_TransitionInfoList_Scroller = BaseObject.FindProperty("TransitionInfoList_Scroller");

            InitializePreviewMaterial();

            LoadPreviewTexture();

            if (Editor_XHud_GUI.EditorData_Has_String("XED_Transition_Bg_Grid_ColorA"))
                sp_BgGrid_Color_A.colorValue = XHud_Utilitys.Color_From_String(Editor_XHud_GUI.EditorData_Get_With_String("XED_Transition_Bg_Grid_ColorA"), false);
            else
                sp_BgGrid_Color_A.colorValue = Color.white * 0.509f;

            if (Editor_XHud_GUI.EditorData_Has_String("XED_Transition_Bg_Grid_ColorB"))
                sp_BgGrid_Color_B.colorValue = XHud_Utilitys.Color_From_String(Editor_XHud_GUI.EditorData_Get_With_String("XED_Transition_Bg_Grid_ColorB"), false);
            else
                sp_BgGrid_Color_B.colorValue = Color.white * 0.376f;

            if (Editor_XHud_GUI.EditorData_Has_String("XED_Transition_Bg_Grid_Tilling"))
                sp_BgGrid_Tilling_multiply.floatValue = float.Parse(Editor_XHud_GUI.EditorData_Get_With_String("XED_Transition_Bg_Grid_Tilling"));
            else
                sp_BgGrid_Tilling_multiply.floatValue = 5;

            sp_BgGrid_Color_A.serializedObject.ApplyModifiedProperties();
            sp_BgGrid_Color_B.serializedObject.ApplyModifiedProperties();
            sp_BgGrid_Tilling_multiply.serializedObject.ApplyModifiedProperties();

            icon_libsetter_transition = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Transition_Setter/logo");

            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Dialog");

            leftarr_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Transition_Setter/left_arrow_p");
            leftarr_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Transition_Setter/left_arrow_r");
            rightarr_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Transition_Setter/right_arrow_p");
            rightarr_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Transition_Setter/right_arrow_r");

            stop_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Transition_Setter/prw_stop_r");
            stop_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Transition_Setter/prw_stop_p");
            play_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Transition_Setter/prw_play_r");
            play_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Transition_Setter/prw_play_p");

            TransitionDescription = "转场说明内容";
            TransitionName = "转场名称";

            sp_itemHeight.floatValue = 25;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 19;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            #region ReorderableList - TransitionLibrary
            TrasitionInfoList = new ReorderableList(BaseObject, sp_TransitionNode.FindPropertyRelative("Frames"), true, true, true, true);
            TrasitionInfoList.drawElementCallback = TransitionInfoList_Original_DrawElementCallback;
            #endregion
        }

        private void OnDestroy()
        {
            GetWindow<SceneView>().Focus();
        }

        #region TransitionInfoList_Original      

        Rect drawelement_rect;
        Rect scrollview_rect;
        Rect item_rect;
        Color SelectedBg = new Color(0, 0, 0, 0.2f);

        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void TransitionInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_TransitionNode;
            SerializedProperty sp_texlist = prop.FindPropertyRelative("Frames");
            SerializedProperty sp_tex = sp_texlist.GetArrayElementAtIndex(index);

            Texture2D tex = sp_tex.objectReferenceValue as Texture2D;

            #region 序号
            drawelement_rect.Set(rect.x + 15, rect.y + 3, 30, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, index.ToString("D2"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);
            #endregion

            #region 名称
            drawelement_rect.Set(rect.x + 40, rect.y, rect.width - 80, rect.height);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, tex.name, HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, Vector2.zero, 12, TextClipping.Ellipsis);
            #endregion

            #region 贴图
            drawelement_rect.Set(rect.width - 50, rect.y + rect.height * (0.3f / 2), rect.height * 0.7f, rect.height * 0.7f);
            Editor_XHud_GUI.Gui_Icon(drawelement_rect, tex);
            #endregion           
        }

        /// <summary>
        /// 绘制列表
        /// </summary>
        private void DrawTransitionInfoList_Original(Rect rect)
        {
            // 绘制滚动视图
            scrollview_rect.Set(rect.x + 10, rect.y + 18, rect.width - 20, sp_visibleItemCount.intValue * sp_itemHeight.floatValue);
            sp_TransitionInfoList_Scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_TransitionInfoList_Scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, TrasitionInfoList.count * sp_itemHeight.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_TransitionInfoList_Scroller.vector2Value.y / sp_itemHeight.floatValue);
            int endIndex = Mathf.CeilToInt((sp_TransitionInfoList_Scroller.vector2Value.y + scrollview_rect.height) / sp_itemHeight.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < TrasitionInfoList.count; i++)
            {
                SerializedProperty sp_texlist = sp_TransitionNode.FindPropertyRelative("Frames");
                SerializedProperty sp_tex = sp_texlist.GetArrayElementAtIndex(i);

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                if (sp_SelectedIndex.intValue == i)
                {
                    // 高亮标记表示选中
                    item_rect.Set(1, i * sp_itemHeight.floatValue + 10, 5, 5);
                    EditorGUI.DrawRect(item_rect, XHud_Dashboard.Theme_Primary);
                    // 高亮背景表示选中
                    item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);
                    EditorGUI.DrawRect(item_rect, SelectedBg);
                }

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                TrasitionInfoList.drawElementCallback.Invoke(item_rect, i, i == TrasitionInfoList.index, true);

                // 检测鼠标是否在当前元素区域内
                Event e = Event.current;
                if (item_rect.Contains(e.mousePosition))
                {
                    #region 切换序列帧索引
                    if (e.type == EventType.KeyDown)
                    {
                        if (e.keyCode == KeyCode.UpArrow)
                        {
                            StopPreviewUpdate();
                            IsPreviewing = false;

                            if (sp_SelectedIndex.intValue <= 0)
                            {
                                sp_SelectedIndex.intValue = 0;
                            }
                            else

                            {
                                sp_SelectedIndex.intValue--;
                            }
                            PreviewData_Index = sp_SelectedIndex.intValue;
                            CalculateListScroller();
                        }
                        if (e.keyCode == KeyCode.DownArrow)
                        {
                            StopPreviewUpdate();
                            IsPreviewing = false;
                            if (sp_SelectedIndex.intValue >= TransitionNode.Frames.Count - 1)
                            {
                                sp_SelectedIndex.intValue = TransitionNode.Frames.Count - 1;
                            }
                            else

                            {
                                sp_SelectedIndex.intValue++;
                            }
                            PreviewData_Index = sp_SelectedIndex.intValue;
                            CalculateListScroller();
                        }
                        e.Use();
                    }
                    #endregion

                    #region 选中序列帧索引
                    if (e.type == EventType.MouseDown)
                    {
                        // 更新选中项
                        sp_SelectedIndex.intValue = i;

                        PreviewData_Index = sp_SelectedIndex.intValue;

                        // 标记界面需要更新
                        GUI.changed = true;
                        e.Use();
                    }
                    #endregion
                }

                // 绘制元素
                //EditorGUI.PropertyField(item_rect, sp_tex, GUIContent.none);
            }

            GUI.EndScrollView();
        }

        #endregion

        private void OnGUI()
        {
            BaseObject.Update();

            SerializedProperty sp_name = sp_TransitionNode.FindPropertyRelative("Name");
            SerializedProperty sp_des = sp_TransitionNode.FindPropertyRelative("Description");
            SerializedProperty sp_TotalFramesCount = sp_TransitionNode.FindPropertyRelative("TotalFramesCount");
            SerializedProperty sp_LastFrameIndex = sp_TransitionNode.FindPropertyRelative("LastFrameIndex");
            SerializedProperty sp_SkipFrame = sp_TransitionNode.FindPropertyRelative("SkipFrame");
            SerializedProperty sp_Res = sp_TransitionNode.FindPropertyRelative("Res");
            SerializedProperty sp_Frames = sp_TransitionNode.FindPropertyRelative("Frames");

            Rect rect = new Rect(0, 0, position.width, position.height);
            draw_rect.Set(0, 0, position.width, position.height);

            draw_rect.Set(26, 15, 48, 48);
            Editor_XHud_GUI.Gui_Icon(draw_rect, icon_libsetter_transition);

            draw_rect.Set(rect.x + 100, rect.y + 15, rect.width - 80, 30);
            Editor_XHud_GUI.Gui_Labelfield(draw_rect, Title, HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 20, Font_Bold);

            draw_rect.Set(rect.x + 102, rect.y + 60, 200, 1);
            Editor_XHud_GUI.Gui_Box(draw_rect, SepLineColor);

            draw_rect.Set(rect.x + 26, rect.y + 80, rect.width - 45, rect.height);
            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(draw_rect, "以下为待入库的转场资源参数与效果的预览，您可以检查即将入库的转场资源是否符合您的要求，同时您可以再次调整即将入库的转场资源！(请确保导入的序列图片颜色空间匹配您的渲染管线！)", HudFilled.无, HudColor.无, MessageColor, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true, Font_Light);

            DateTimes = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss:ff");
            draw_rect.Set(rect.x + 150, rect.y + 15, rect.width - 180, rect.height);
            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(draw_rect, DateTimes, HudFilled.无, HudColor.无, DateTimeColor, TextAnchor.UpperRight, new Vector2(0, 0), 13, true, Font_Light);

            #region 数值关联
            if (sp_Frames.arraySize > 0)
            {
                sp_Res.vector2IntValue = new Vector2Int(TransitionNode.Frames[0].width, TransitionNode.Frames[0].height);
                sp_TotalFramesCount.intValue = sp_Frames.arraySize;
                sp_LastFrameIndex.intValue = sp_Frames.arraySize - 1;
            }
            #endregion

            #region 颜色入库信息
            float Added_Input_Height = 80;
            #region 入库名称
            Color LibName_color = Color.white;
            if (string.IsNullOrEmpty(sp_TransitionName.stringValue))
            {
                LibName_color = Color.gray;
                sp_TransitionName.stringValue = "转场名称";
            }
            else
            {
                if (sp_TransitionName.stringValue == "转场名称")
                    LibName_color = Color.gray;
                else
                    LibName_color = Color.white;
            }

            draw_rect.Set(rect.width - 527, rect.height - 230 + Added_Input_Height, 230, 60);
            sp_TransitionName.stringValue = Editor_XHud_GUI.Gui_TextField(draw_rect, sp_TransitionName.stringValue, LibName_color, 12);
            sp_TransitionName.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 说明文字
            Color Description_Color = Color.white;
            if (string.IsNullOrEmpty(sp_TransitionDescription.stringValue))
            {
                Description_Color = Color.gray;
                sp_TransitionDescription.stringValue = "转场说明内容";
            }
            else
            {
                if (sp_TransitionDescription.stringValue == "转场说明内容")
                    Description_Color = Color.gray;
                else
                    Description_Color = Color.white;
            }
            draw_rect.Set(rect.width - 285, rect.height - 230 + Added_Input_Height, 260, 60);
            sp_TransitionDescription.stringValue = Editor_XHud_GUI.Gui_TextField(draw_rect, sp_TransitionDescription.stringValue, Description_Color, 12);
            sp_TransitionDescription.serializedObject.ApplyModifiedProperties();
            #endregion
            #endregion

            #region 转场帧信息
            float Added_Info_Height = 30;
            string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);
            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 275, rect.height - 100 + Added_Info_Height, 200, 15), $"总帧数：<color={hexcol}> {sp_TotalFramesCount.intValue} </color> 张", HudFilled.无, HudColor.无, Color.white, false, Color.blue, TextAnchor.UpperLeft, Vector2.zero, 15, Font_Bold);

            Editor_XHud_GUI.Gui_Labelfield_Thin(new Rect(rect.x + 275, rect.height - 75 + Added_Info_Height, 200, 15), $"尾帧 ：{sp_LastFrameIndex.intValue} 帧", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.UpperLeft, new Vector2(0, 0), 12, false, false, true);
            #endregion

            #region 预览

            Rect rect_preview_group = new Rect(rect.width - 530, rect.y + 140, 510, 380);
            Editor_XHud_GUI.Gui_Group(rect_preview_group, HudFilled.纯色边框, HudColor.亮白, "序列帧预览器", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);

            if (TransitionNode.Frames != null && TransitionNode.Frames.Count > 0)
            {
                float w = TransitionNode.Frames[0].width;
                float h = TransitionNode.Frames[0].height;

                Rect rect_preview = new Rect(rect.width - 530, rect.y + 140, 510, 298.75f);
                float margin = 10;

                #region 预览序列帧
                Rect rect_canvas = new Rect(rect_preview.x + margin, rect_preview.y + 20 + margin, rect_preview.width - 2 * margin, rect_preview.height - 2 * margin);

                #region 计算纹理的缩放比例
                float scaleX = rect_canvas.width / w;
                float scaleY = rect_canvas.height / h;

                // 选择合适的缩放比例，确保纹理等比缩放并且完全显示在目标矩形内
                float scale;
                if (w > h)
                {
                    scale = scaleX; // 宽度大于高度，以宽度为主
                }
                else
                {
                    scale = scaleY; // 高度大于宽度，以高度为主
                }

                // 计算纹理的绘制矩形
                float textureWidth = w * scale;
                float textureHeight = h * scale;

                // 计算纹理在目标矩形中的居中位置
                float x = rect_canvas.x + (rect_canvas.width - textureWidth) / 2;
                float y = rect_canvas.y + (rect_canvas.height - textureHeight) / 2;
                #endregion

                draw_rect.Set(x, y + 15, textureWidth, textureHeight);

                Editor_XHud_GUI.Gui_Box(draw_rect, Color.black * 0.1f);

                Preview_Material.SetColor("_Color", sp_Preview_Color.colorValue);
                Preview_Material.SetFloat("_Alpha", sp_Preview_Color.colorValue.a);

                BgGrid_Tilling_x = sp_BgGrid_Tilling_multiply.floatValue;
                BgGrid_Tilling_y = sp_BgGrid_Tilling_multiply.floatValue;

                Preview_Material.SetFloat("_Grid_Tilling_x", BgGrid_Tilling_x * textureWidth);
                Preview_Material.SetFloat("_Grid_Tilling_y", BgGrid_Tilling_y * textureHeight);
                Preview_Material.SetColor("_Grid_Color_A", sp_BgGrid_Color_A.colorValue);
                Preview_Material.SetColor("_Grid_Color_B", sp_BgGrid_Color_B.colorValue);
                Preview_Material.SetTexture("_Texture", Preview_Texture_Bg);
                Preview_Material.SetTexture("_Mask", TransitionNode.Frames[PreviewData_Index]);
                Preview_Material.SetInt("_Invert", InvertChannel ? 1 : 0);

                #region 绘制预览图片
                GL.PushMatrix();
                {
                    // Correct coordinate system (正向Y轴-up)
                    GL.LoadPixelMatrix(0, position.width, position.height, 0);

                    Preview_Material.SetPass(0);
                    GL.Begin(GL.QUADS);
                    {
                        // Corrected vertex order for 正向Y轴-up
                        GL.TexCoord2(0, 1); GL.Vertex3(draw_rect.x, draw_rect.y, 0);
                        GL.TexCoord2(1, 1); GL.Vertex3(draw_rect.x + draw_rect.width, draw_rect.y, 0);
                        GL.TexCoord2(1, 0); GL.Vertex3(draw_rect.x + draw_rect.width, draw_rect.y + draw_rect.height, 0);
                        GL.TexCoord2(0, 0); GL.Vertex3(draw_rect.x, draw_rect.y + draw_rect.height, 0);
                    }
                    GL.End();
                }
                GL.PopMatrix();
                #endregion
                #endregion

                #region 预览控件

                float PreviewActiveX_added_height = 15;

                #region 颜色
                draw_rect.Set(rect_preview.x + rect_preview.width - 50, rect_preview_group.y + 3 + margin, 40, 20);
                sp_Preview_Color.colorValue = Editor_XHud_GUI.Gui_ColorField(draw_rect, sp_Preview_Color.colorValue);
                #endregion

                #region Grid A颜色
                draw_rect.Set(rect_preview.x + rect_preview.width - 100, rect_preview_group.y + 3 + margin, 40, 20);
                sp_BgGrid_Color_A.colorValue = Editor_XHud_GUI.Gui_ColorField(draw_rect, sp_BgGrid_Color_A.colorValue);
                #endregion

                #region Grid B颜色
                draw_rect.Set(rect_preview.x + rect_preview.width - 150, rect_preview_group.y + 3 + margin, 40, 20);
                sp_BgGrid_Color_B.colorValue = Editor_XHud_GUI.Gui_ColorField(draw_rect, sp_BgGrid_Color_B.colorValue);
                #endregion

                #region 背景平铺
                draw_rect.Set(rect_preview.x + rect_preview.width - 235, rect_preview_group.y + 3 + margin, 65, 20);
                Editor_XHud_GUI.Gui_Property_Field(draw_rect, "平铺", sp_BgGrid_Tilling_multiply, 0, 30);
                #endregion

                #region 跳帧
                draw_rect.Set(rect_preview.x + rect_preview.width - 310, rect_preview_group.y + 3 + margin, 65, 20);
                Editor_XHud_GUI.Gui_Property_Field(draw_rect, "跳帧", sp_SkipFrame, 0, 30);
                #endregion

                #region 转场图片
                draw_rect.Set(rect_preview.x + rect_preview.width - 495, rect_preview_group.y + 3 + margin, 170, 20);
                Editor_XHud_GUI.Gui_Property_Field(draw_rect, "预览图", sp_Preview_Texture_Bg, 0, 40);
                #endregion

                #region 播放停止模式
                string stopmode = "自动复位";
                if (!StoppedResetMode)
                {
                    stopmode = "自动复位";
                }
                else
                {
                    stopmode = "手动复位";
                }
                draw_rect.Set(rect_preview.x + rect_preview.width - 210, rect_preview.y + 316 + margin + PreviewActiveX_added_height, 40, 20);
                if (Editor_XHud_GUI.Gui_Button(draw_rect, null, null, TextAnchor.MiddleCenter, false, stopmode, "", Color.clear, XHud_Dashboard.Theme_Primary, HudFilled.透明, 12))
                {
                    StoppedResetMode = !StoppedResetMode;
                }
                #endregion

                #region 进度条
                draw_rect.Set(rect_preview.x + 15, rect_preview.y + 316 + margin + PreviewActiveX_added_height, rect_preview.width - 245, 18);
                EditorGUI.BeginChangeCheck();
                PreviewData_Index = Editor_XHud_GUI.Gui_Slider(draw_rect, "进度", PreviewData_Index, 0, TransitionNode.Frames.Count - 1);
                if (EditorGUI.EndChangeCheck())
                {
                    StopPreviewUpdate();
                    IsPreviewing = false;

                    sp_SelectedIndex.intValue = PreviewData_Index;

                    CalculateListScroller();
                }
                #endregion

                #region 反色模式
                string inverted = "正常";
                if (!InvertChannel)
                {
                    inverted = "正常";
                }
                else
                {
                    inverted = "反色";
                }
                draw_rect.Set(rect_preview.x + rect_preview.width - 155, rect_preview.y + 316 + margin + PreviewActiveX_added_height, 40, 20);
                if (Editor_XHud_GUI.Gui_Button(draw_rect, null, null, TextAnchor.MiddleCenter, false, inverted, "", Color.clear, XHud_Dashboard.Theme_Primary, HudFilled.透明, 12))
                {
                    InvertChannel = !InvertChannel;
                    StopPreviewUpdate();
                    IsPreviewing = false;

                    PreviewData_Index = 0;
                }
                #endregion

                #region 播放
                draw_rect.Set(rect_preview.x + rect_preview.width - 98, rect_preview.y + 319 + margin + PreviewActiveX_added_height, 14, 14);
                if (!IsPreviewing && Editor_XHud_GUI.Gui_Button(draw_rect, play_r, play_p, true, "", "", Color.white))
                {
                    IsPreviewing = true;
                    StartPreviewUpdate();
                }
                #endregion

                #region 停止
                draw_rect.Set(rect_preview.x + rect_preview.width - 98, rect_preview.y + 319 + margin + PreviewActiveX_added_height, 14, 14);
                if (IsPreviewing && Editor_XHud_GUI.Gui_Button(draw_rect, stop_r, stop_p, true, "", "", Color.white))
                {
                    IsPreviewing = false;
                    StopPreviewUpdate();
                    PreviewData_Index = 0;
                }
                #endregion

                #region 左一帧
                draw_rect.Set(rect_preview.x + rect_preview.width - 60, rect_preview.y + 319 + margin + PreviewActiveX_added_height, 14, 14);
                if (Editor_XHud_GUI.Gui_Button(draw_rect, leftarr_r, leftarr_p, true, "", "", Color.white))
                {
                    FrameMoveBackward();
                }
                #endregion

                #region 右一帧
                draw_rect.Set(rect_preview.x + rect_preview.width - 25, rect_preview.y + 319 + margin + PreviewActiveX_added_height, 14, 14);
                if (Editor_XHud_GUI.Gui_Button(draw_rect, rightarr_r, rightarr_p, true, "", "", Color.white))
                {
                    FrameMoveForward();
                }
                #endregion

                #region 帧显示
                draw_rect.Set(rect_preview.x + rect_preview.width - 70, rect_preview.y + rect_preview.height - 8, 50, 20);
                GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
                Editor_XHud_GUI.Gui_Labelfield_Thin(draw_rect, $"{PreviewData_Index} / {sp_TotalFramesCount.intValue}", HudFilled.实体, HudColor.亮白, Color.black, TextAnchor.MiddleCenter, new Vector2(-2, -1), 10, false, false, true);
                GUI.backgroundColor = Color.white;
                #endregion
                #endregion
            }
            else
            {
                draw_rect.Set(rect.width - 530, rect.y + 330, 510, 20);
                Editor_XHud_GUI.Gui_Labelfield(draw_rect, "请先添加序列帧图像", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleCenter, Vector2.zero, 12, Font_Light);
            }

            GUI.backgroundColor = Color.white;
            #endregion

            #region 序列帧列表
            Rect rect_transitionlist_group = new Rect(rect.x + 20, rect.y + 140, 245, 518);
            Editor_XHud_GUI.Gui_Group(rect_transitionlist_group, HudFilled.纯色边框, HudColor.亮白, "序列帧列表", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
            DrawTransitionInfoList_Original(rect_transitionlist_group);
            #endregion

            #region 拖放序列帧
            dragarea.Set(rect_transitionlist_group.x, rect_transitionlist_group.y + 10, rect_transitionlist_group.width, rect_transitionlist_group.height);

            Event e = Event.current;
            if (dragarea.Contains(e.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (e.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    Object[] dropobjs = DragAndDrop.objectReferences;
                    TransitionNode.Frames.Clear();
                    foreach (var item in dropobjs)
                    {
                        if (item is Texture2D == false)
                            continue;

                        Texture2D tex = (Texture2D)item;

                        if (TransitionNode.Frames == null)
                            TransitionNode.Frames = new List<Texture2D>();

                        TransitionNode.Frames.Add(tex);
                    }

                    e.Use();
                    PreviewData_Index = 0;
                    SelectedIndex = 0;
                }
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(631);

            DialogType_Buttons();

            #region 检测点击事件以及左右帧进
            if (e.type == EventType.MouseDown)
            {
                draw_rect.Set(rect.width - 280, rect.height - 175, (rect.width / 2) - 55, 60);
                // 检查点击位置是否在窗口内
                if (!draw_rect.Contains(e.mousePosition))
                {
                    GUI.FocusControl(null); // 取消所有控件的焦点
                    Repaint(); // 重新绘制窗口
                }
                draw_rect.Set(rect.width - 280, rect.height - 230, (rect.width / 2) - 55, 50);
                // 检查点击位置是否在窗口内
                if (!draw_rect.Contains(e.mousePosition))
                {
                    GUI.FocusControl(null); // 取消所有控件的焦点
                    Repaint(); // 重新绘制窗口
                }
            }
            if (e.type == EventType.KeyDown)
            {
                if (rect_preview_group.Contains(e.mousePosition))
                {
                    if (e.keyCode == KeyCode.RightArrow)
                    {
                        FrameMoveForward();
                        CalculateListScroller();
                    }
                    if (e.keyCode == KeyCode.LeftArrow)
                    {
                        FrameMoveBackward();
                        CalculateListScroller();
                    }
                }
                if (e.keyCode == KeyCode.Escape)
                {
                    Close();
                }
                e.Use();
            }
            #endregion

            Repaint();

            if (BaseObject.targetObject != null)
                BaseObject.ApplyModifiedProperties();
        }

        private void FrameMoveForward()
        {
            StopPreviewUpdate();
            IsPreviewing = false;

            if (PreviewData_Index >= TransitionNode.Frames.Count - 1)
            {
                PreviewData_Index = TransitionNode.Frames.Count - 1;
            }
            else

            {
                PreviewData_Index++;
            }

            sp_SelectedIndex.intValue = PreviewData_Index;
        }

        private void FrameMoveBackward()
        {
            StopPreviewUpdate();
            IsPreviewing = false;

            if (PreviewData_Index <= 0)
            {
                PreviewData_Index = 0;
            }
            else

            {
                PreviewData_Index--;
            }

            sp_SelectedIndex.intValue = PreviewData_Index;
        }

        #region 辅助
        private void UpdateToLibrary()
        {
            string colorhex = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            XHud_LibraryArg_Transition info = new XHud_LibraryArg_Transition();
            info.CopyData(TransitionNode);
            info.Name = sp_TransitionName.stringValue;
            info.Description = sp_TransitionDescription.stringValue;
            Target_Hud_TransitionLibrary.TransitionsLibrary_Replace(ModifiedIndex, info);

            Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 转场资源修改器消息", "更新完成", $"转场资源已更新完成！", "明白");

            Close();
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
        /// 设置模版基础信息（针对从库源修改更新使用）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="decription"></param>
        /// <param name="color"></param>
        public void SetInfo(string name, string decription)
        {
            sp_TransitionName.stringValue = name;
            sp_TransitionDescription.stringValue = decription;

            sp_TransitionName.serializedObject.ApplyModifiedProperties();
            sp_TransitionDescription.serializedObject.ApplyModifiedProperties();

            PreviewData_Index = 0;
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
        /// 控件按钮
        /// </summary>
        private void DialogType_Buttons()
        {
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();

            if (Editor_XHud_GUI.Gui_Layout_Button(ButtonText_Cancel, "", HudFilled.实体, HudColor.亮白, Color.black, 12, ButtonWidth, ButtonHeight, Font_Light, ButtonText_Cancel))
            {
                Close();
            }
            Editor_XHud_GUI.Gui_Layout_Space(ButtonDistance);
            GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
            if (Editor_XHud_GUI.Gui_Layout_Button(ButtonText_Ok, "", HudFilled.实体, HudColor.亮白, XHud_Utilitys.GetBrightnessLimite(XHud_Dashboard.Theme_Primary) ? Color.black : Color.white, 12, ButtonWidth, ButtonHeight, Font_Light, ButtonText_Ok))
            {
                UpdateToLibrary();
                return;
            }
            GUI.backgroundColor = Color.white;

            Editor_XHud_GUI.Gui_Layout_Space(25);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
        }

        public void Set_TransitionNode(XHud_LibraryArg_Transition node)
        {
            TransitionNode.CopyData(node);
        }

        public void Set_Target_Hud_TransitionLibrary(XHud_Library_Transition lib)
        {
            Target_Hud_TransitionLibrary = lib;
        }

        public void Set_OriginTransitionNode(XHud_LibraryArg_Transition node)
        {
            OriginTransitionNode.CopyData(node);
        }

        /// <summary>
        /// 开始预览
        /// </summary>
        public void StartPreviewUpdate()
        {
            PreviewCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(PreviewUpdater());
        }

        /// <summary>
        /// 停止预览
        /// </summary>
        public void StopPreviewUpdate()
        {
            if (PreviewCoroutine == null)
                return;
            EditorCoroutineUtility.StopCoroutine(PreviewCoroutine);
        }

        IEnumerator PreviewUpdater()
        {
            var waitForOneSecond = new EditorWaitForSeconds(PreviewDataDuration);
            while (true)
            {
                if (PreviewData_Index >= TransitionNode.Frames.Count - 1 - TransitionNode.SkipFrame)
                {
                    if (!StoppedResetMode)
                    {
                        IsPreviewing = false;
                        PreviewData_Index = 0;
                    }
                    else
                    {
                        PreviewData_Index = TransitionNode.Frames.Count - 1;
                    }
                    StopPreviewUpdate();
                    break;
                }
                else
                {
                    PreviewData_Index += TransitionNode.SkipFrame;
                }
                SelectedIndex = PreviewData_Index;

                yield return waitForOneSecond;
            }
        }

        /// <summary>
        /// 初始化预览转场材质
        /// </summary>
        private void InitializePreviewMaterial()
        {
            Shader shader = Shader.Find("XHud/EditorTransition");
            Preview_Material = new Material(shader);
        }

        /// <summary>
        /// 初始化预览背景图
        /// </summary>
        private void LoadPreviewTexture()
        {
            string path = Editor_XHud_GUI.EditorData_Get_With_String("XED_Transition_Bg_Path");
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
                sp_Preview_Texture_Bg.objectReferenceValue = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Transition_Setter/defaultbg");
            else
                sp_Preview_Texture_Bg.objectReferenceValue = tex;
        }

        private void CalculateListScroller()
        {
            if (SelectedIndex >= visibleItemCount)
            {
                sp_TransitionInfoList_Scroller.vector2Value = new Vector2(sp_TransitionInfoList_Scroller.vector2Value.x, (SelectedIndex) * itemHeight);
            }
            else
                sp_TransitionInfoList_Scroller.vector2Value = Vector2.zero;
        }
        #endregion
    }
}