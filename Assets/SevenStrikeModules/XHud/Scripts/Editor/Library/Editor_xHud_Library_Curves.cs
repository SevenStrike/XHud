namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XTween;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [System.Serializable]
    public class CurveFrame
    {
        public float time;
        public float value;
        public float inTangent;
        public float inWeight;
        public float outTangent;
        public float outWeight;
        public WeightedMode weightedMode;
    }

    [System.Serializable]
    public class CurveNode
    {
        public string Name;
        public CurveFrame[] Frames;
    }

    [System.Serializable]
    public class CurveLib
    {
        public List<CurveNode> Nodes;
    }

    [CustomEditor(typeof(XHud_Library_Curves))]
    public class Editor_XHud_Library_Curves : Editor
    {
        #region 组件 / 列表
        private XHud_Library_Curves BaseScript;
        /// <summary>
        /// 曲线列表
        /// </summary>
        private ReorderableList CurveInfoList;
        #endregion

        #region 序列化属性
        private SerializedProperty sp_SelectedIndex, sp_CurveLibrary, sp_LibraryName, sp_PreviewBGColor, sp_ReferImgIndex, sp_PreviewGridColor, sp_PreviewGridSize, sp_PreviewImageSize, sp_UsePreviewLooped, sp_PreviewModeIndex, sp_PreviewCurveIndex, sp_PreviewCurveName, sp_PreviewDuration, sp_Highlight, sp_SoundInfoList_Original_Scroller, sp_itemHeight, sp_visibleItemCount, sp_Find, sp_UsePreviewAutoStop;
        #endregion

        /// <summary>
        /// 预览图片
        /// </summary>
        private Texture2D[] ReferImages;

        #region 图标
        private Texture2D import_p, import_r, export_p, export_r, clear_p, clear_r, create_p, create_r, delete_p, delete_r, play_p, play_r, leftarr_p, leftarr_r, rightarr_p, rightarr_r, stop_p, stop_r;
        #endregion

        #region 预览
        /// <summary>
        /// 动画预览 - 目标值
        /// </summary>
        private float TweenValue;
        /// <summary>
        /// 动画预览 - 目标角度
        /// </summary>
        private float TweenDeg;
        /// <summary>
        /// 动画预览 - 透明度_Alpha
        /// </summary>
        private float TweenAlpha;
        /// <summary>
        /// 动画预览 - 缩放值
        /// </summary>
        private float TweenScl;
        /// <summary>
        /// 动画预览 - 组件
        /// </summary>
        private XTween_Interface CurveTween;
        /// <summary>
        /// 动画预览 - 状态开关
        /// </summary>
        private bool TweenPlaying;
        #endregion

        /// <summary>
        /// 屏蔽颜色
        /// </summary>
        private Color blocked_col;
        /// <summary>
        /// 屏蔽颜色透明度
        /// </summary>
        private float blocked_alp = 0.45f;
        /// <summary>
        /// 预览标题文字
        /// </summary>
        public string PreviewHeader = "CurvePreview";

        private void OnEnable()
        {
            BaseScript = (XHud_Library_Curves)target;

            #region 获取序列化属性
            sp_CurveLibrary = serializedObject.FindProperty("CurveLibrary");
            sp_PreviewBGColor = serializedObject.FindProperty("PreviewBGColor");
            sp_ReferImgIndex = serializedObject.FindProperty("ReferImgIndex");
            sp_PreviewGridColor = serializedObject.FindProperty("PreviewGridColor");
            sp_PreviewGridSize = serializedObject.FindProperty("PreviewGridSize");
            sp_PreviewImageSize = serializedObject.FindProperty("PreviewImageSize");
            sp_UsePreviewLooped = serializedObject.FindProperty("UsePreviewLooped");
            sp_PreviewModeIndex = serializedObject.FindProperty("PreviewModeIndex");
            sp_PreviewCurveIndex = serializedObject.FindProperty("PreviewCurveIndex");
            sp_PreviewCurveName = serializedObject.FindProperty("PreviewCurveName");
            sp_PreviewDuration = serializedObject.FindProperty("PreviewDuration");
            sp_UsePreviewAutoStop = serializedObject.FindProperty("UsePreviewAutoStop");
            sp_LibraryName = serializedObject.FindProperty("LibraryName");
            sp_Highlight = serializedObject.FindProperty("Highlight");
            sp_SelectedIndex = serializedObject.FindProperty("SelectedIndex");
            sp_SoundInfoList_Original_Scroller = serializedObject.FindProperty("SoundInfoList_Original_Scroller");
            sp_itemHeight = serializedObject.FindProperty("itemHeight");
            sp_visibleItemCount = serializedObject.FindProperty("visibleItemCount");
            sp_Find = serializedObject.FindProperty("Find");
            #endregion

            #region 获取曲线动效预览图
            Texture2D tex = new Texture2D(512, 512, TextureFormat.ARGB32, true, true);
            if (BaseScript.SelectedReferImage == null)
            {
                BaseScript.SelectedReferImage = null;
                BaseScript.SelectedReferImage = tex;
            }
            ReferImages = LoadAllAssetsAtPathWithPattern<Texture2D>($"{XHud_Dashboard.Get_Path_XHUD_GUISTYLE_Path()}CurvePreviewImgs/", ".png").ToArray();
            sp_ReferImgIndex.intValue = 0;
            BaseScript.SelectedReferImage = ReferImages[sp_ReferImgIndex.intValue];
            #endregion

            #region 获取图标
            import_p = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/import_p");
            import_r = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/import_r");
            export_p = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/export_p");
            export_r = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/export_r");
            clear_p = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/clear_p");
            clear_r = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/clear_r");
            create_p = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/create_p");
            create_r = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/create_r");
            delete_p = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/delete_p");
            delete_r = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/delete_r");
            play_p = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/play_p");
            play_r = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/play_r");
            leftarr_p = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/leftarr_p");
            leftarr_r = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/leftarr_r");
            rightarr_p = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/rightarr_p");
            rightarr_r = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/rightarr_r");
            stop_p = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/stop_p");
            stop_r = Editor_XHud_GUI.GetIcon("Icons_XHud_CurveLibrary/stop_r");
            #endregion

            //设定列表项高度值
            sp_itemHeight.floatValue = 26;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 18;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            blocked_col = new Color(0, 0, 0, blocked_alp);

            sp_PreviewCurveIndex.intValue = 0;
            sp_PreviewCurveIndex.serializedObject.ApplyModifiedProperties();

            #region ReorderableList - CurveInfoList
            CurveInfoList = new ReorderableList(serializedObject, sp_CurveLibrary, true, true, true, true);
            CurveInfoList.drawElementCallback = CurveInfoList_Original_DrawElementCallback;
            #endregion
        }

        private void OnDisable()
        {
            sp_SoundInfoList_Original_Scroller.vector2Value = Vector2.zero;
            sp_SoundInfoList_Original_Scroller.serializedObject.ApplyModifiedProperties();

            sp_SelectedIndex.intValue = -1;
            sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

            sp_Find.stringValue = string.Empty;
            sp_Find.serializedObject.ApplyModifiedProperties();

            sp_Highlight.stringValue = string.Empty;
            sp_Highlight.serializedObject.ApplyModifiedProperties();
        }

        #region CurveInfoList_Original      

        Rect drawelement_rect;
        Rect scrollview_rect;
        Rect item_rect;
        Color SelectedBg = new Color(0, 0, 0, 0.2f);

        private void BlockGUI(string name)
        {
            if (!name.Contains(sp_Highlight.stringValue))
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
        }

        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void CurveInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_CurveLibrary.GetArrayElementAtIndex(index);

            SerializedProperty sp_name = prop.FindPropertyRelative("Name");
            SerializedProperty sp_curve = prop.FindPropertyRelative("Curve");

            drawelement_rect.Set(rect.x + 15, rect.y + 3, 30, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, index.ToString("D2"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);

            BlockGUI(sp_name.stringValue);

            drawelement_rect.Set(rect.x + 42, rect.y + 3, rect.width - 250, 20);
            sp_name.stringValue = Editor_XHud_GUI.Gui_InputField_String(drawelement_rect, sp_name.stringValue);
            sp_name.serializedObject.ApplyModifiedProperties();

            BlockGUI(sp_name.stringValue);

            EditorGUI.BeginChangeCheck();
            drawelement_rect.Set(rect.width - 120, rect.y + 1, 100, 20);
            sp_curve.animationCurveValue = Editor_XHud_GUI.Gui_CurveField(drawelement_rect, sp_curve.animationCurveValue);
            sp_curve.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                if (BaseScript.act_on_CurveChanged != null)
                    BaseScript.act_on_CurveChanged();
            }

            GUI.enabled = true;

            if (!sp_name.stringValue.Contains(sp_Highlight.stringValue))
            {
                drawelement_rect.Set(rect.x - 20, rect.y, rect.width + 20, rect.height);
                EditorGUI.DrawRect(drawelement_rect, blocked_col);
            }
        }

        /// <summary>
        /// 绘制列表
        /// </summary>
        private void DrawCurveInfoList_Original()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visibleItemCount.intValue * sp_itemHeight.floatValue);
            sp_SoundInfoList_Original_Scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_SoundInfoList_Original_Scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, CurveInfoList.count * sp_itemHeight.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_SoundInfoList_Original_Scroller.vector2Value.y / sp_itemHeight.floatValue);
            int endIndex = Mathf.CeilToInt((sp_SoundInfoList_Original_Scroller.vector2Value.y + scrollview_rect.height) / sp_itemHeight.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < CurveInfoList.count; i++)
            {
                SerializedProperty prop = sp_CurveLibrary.GetArrayElementAtIndex(i);
                SerializedProperty sp_name = prop.FindPropertyRelative("Name");

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                // 如果当前元素被选中，绘制选中效果
                if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                {
                    if (sp_SelectedIndex.intValue == i)
                    {
                        // 高亮标记表示选中
                        item_rect.Set(1, i * sp_itemHeight.floatValue + 10, 5, 5);
                        EditorGUI.DrawRect(item_rect, XHud_Dashboard.Theme_Primary);
                        // 高亮背景表示选中
                        item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width + 20, sp_itemHeight.floatValue);
                        EditorGUI.DrawRect(item_rect, SelectedBg);
                        sws = true;
                    }
                }

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                CurveInfoList.drawElementCallback.Invoke(item_rect, i, i == CurveInfoList.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                        {
                            Tween_Stop();
                            TweenValue = 0;

                            sp_PreviewCurveIndex.intValue = i;
                            sp_PreviewCurveIndex.serializedObject.ApplyModifiedProperties();

                            sp_PreviewCurveName.stringValue = sp_name.stringValue;
                            sp_PreviewCurveName.serializedObject.ApplyModifiedProperties();

                            // 更新选中项
                            sp_SelectedIndex.intValue = i;
                            sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

                            // 标记界面需要更新
                            GUI.changed = true;
                        }
                    }
                }

                // 绘制元素
                EditorGUI.PropertyField(item_rect, prop, GUIContent.none);
            }

            GUI.EndScrollView();
        }

        /// <summary>
        /// 列表移除
        /// </summary>
        /// <param name="list"></param>
        private void CurveInfoList_Original_Remove(ReorderableList list)
        {
            if (list.count <= 0)
                return;
            sp_CurveLibrary.DeleteArrayElementAtIndex(list.index);
            if (BaseScript.act_on_CurveRemoved != null)
                BaseScript.act_on_CurveRemoved();
        }

        /// <summary>
        /// 列表增加
        /// </summary>
        /// <param name="list"></param>
        private void CurveInfoList_Original_Add(ReorderableList list)
        {
            if (list.count <= 0)
            {
                if (BaseScript.CurveLibrary == null)
                    BaseScript.CurveLibrary = new List<xHud_LibraryArg_Curve>();
                BaseScript.CurveLibrary.Add(new XHud.xHud_LibraryArg_Curve("曲线" + BaseScript.CurveLibrary.Count, AnimationCurve.EaseInOut(0, 0, 1, 1)));
            }
            else
            {
                sp_CurveLibrary.InsertArrayElementAtIndex(list.index);
                SerializedProperty prop = sp_CurveLibrary.GetArrayElementAtIndex(list.index);
                SerializedProperty sp_name = prop.FindPropertyRelative("Name");
                sp_name.stringValue = $"新曲线_{list.index}";


                sp_CurveLibrary.serializedObject.ApplyModifiedProperties();
            }

            if (BaseScript.act_on_CurveAdded != null)
                BaseScript.act_on_CurveAdded(BaseScript.CurveLibrary[BaseScript.CurveLibrary.Count - 1].Name, BaseScript.CurveLibrary[BaseScript.CurveLibrary.Count - 1].Curve);
        }
        #endregion

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Editor_XHud_GUI.Gui_Layout_Banner(HudFilled.实体, HudColor.亮白, "XHud - 曲线库", Color.black);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Property_Field("曲线库名称", sp_LibraryName);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Property_Field("过滤（包含）", sp_Highlight);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Property_Field("查找（精确）", sp_Find);
            if (EditorGUI.EndChangeCheck())
            {
                if (!string.IsNullOrEmpty(sp_Find.stringValue))
                    BaseScript.CurveLibrary_Location_Find(sp_Find.stringValue);
                else
                {
                    sp_SelectedIndex.intValue = -1;
                    sp_SelectedIndex.serializedObject.ApplyModifiedProperties();
                }
            }
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Property_Field("底色", sp_PreviewBGColor);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Property_Field("网格色", sp_PreviewGridColor);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            if (string.IsNullOrEmpty(sp_Highlight.stringValue))
            {
                if (!Application.isPlaying)
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "读取曲线库", import_r, import_p, 4))
                    {
                        Curves_Import();
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "导出曲线库", export_r, export_p, 4))
                    {
                        Curves_Export();
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "清空曲线库", clear_r, clear_p, 4))
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 曲线库消息", "清空所有曲线", "是否清空所有曲线项？请注意！如果您的场景中或是预制体中的脚本用到了该曲线库中的曲线，清空后会导致动画器的曲线信息丢失，请谨慎操作！", "清空", "暂不", 1);
                        if (res == "暂不")
                            return;

                        sp_CurveLibrary.ClearArray();
                        sp_CurveLibrary.serializedObject.ApplyModifiedProperties();
                        return;
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "添加项", create_r, create_p, 4))
                    {
                        CurveInfoList_Original_Add(CurveInfoList);
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "删除项", delete_r, delete_p, 4))
                    {
                        CurveInfoList_Original_Remove(CurveInfoList);
                    }
                }
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_LabelfieldThin("当前为过滤筛选状态", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, new Vector2(0, 0), 12);
            }
            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "曲线列表", Color.white);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            DrawCurveInfoList_Original();

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 曲线库导出
        /// </summary>
        private void Curves_Export()
        {
            CurveLib lib = new CurveLib();
            lib.Nodes = new List<CurveNode>();

            for (int i = 0; i < BaseScript.CurveLibrary.Count; i++)
            {
                xHud_LibraryArg_Curve info = BaseScript.CurveLibrary[i];

                Keyframe[] frames = info.Curve.keys;

                CurveNode Node = new CurveNode();
                Node.Name = info.Name;
                Node.Frames = new CurveFrame[frames.Length];
                for (int s = 0; s < frames.Length; s++)
                {
                    CurveFrame cs = new CurveFrame();
                    cs.time = frames[s].time;
                    cs.value = frames[s].value;
                    cs.inTangent = frames[s].inTangent;
                    cs.inWeight = frames[s].inWeight;
                    cs.outTangent = frames[s].outTangent;
                    cs.outWeight = frames[s].outWeight;
                    cs.weightedMode = frames[s].weightedMode;
                    Node.Frames[s] = cs;
                }
                lib.Nodes.Add(Node);
            }

            string path = EditorUtility.SaveFilePanel("", Application.dataPath, "HudCurves", "json");

            if (string.IsNullOrEmpty(path))
                return;

            if (!string.IsNullOrEmpty(path))
            {
                FileInfo info = new FileInfo(path);
                StreamWriter sw = info.CreateText();

                string message = JsonUtility.ToJson(lib, true);

                sw.Write(message);
                sw.Close();
                sw.Dispose();
            }
        }

        /// <summary>
        /// 曲线库导入
        /// </summary>
        private void Curves_Import()
        {
            string path = "";
            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 曲线库消息", "读取曲线数据", "根据您的需要选择导入曲线数据的方式，如果是追加则会在当前曲线库的基础上后续叠加导入的曲线项，如果是替换则会完全替换当前曲线库的所有曲线项！", "追加", "替换", "暂不", 2);
            if (res == "暂不")
            {
                return;
            }

            switch (res)
            {
                case "追加":
                    path = EditorUtility.OpenFilePanel("读取曲线库数据文件", Application.dataPath, "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        string content = File.ReadAllText(path);

                        CurveLib lib = JsonUtility.FromJson<CurveLib>(content);

                        int size = sp_CurveLibrary.arraySize;

                        for (int i = 0; i < lib.Nodes.Count; i++)
                        {
                            string name = lib.Nodes[i].Name;

                            Keyframe[] frames = new Keyframe[lib.Nodes[i].Frames.Length];

                            for (int s = 0; s < frames.Length; s++)
                            {
                                frames[s] = new Keyframe(
                                    lib.Nodes[i].Frames[s].time,
                                    lib.Nodes[i].Frames[s].value,
                                    lib.Nodes[i].Frames[s].inTangent,
                                    lib.Nodes[i].Frames[s].outTangent,
                                    lib.Nodes[i].Frames[s].inWeight,
                                    lib.Nodes[i].Frames[s].outWeight);
                            }

                            AnimationCurve curve = new AnimationCurve(frames);

                            xHud_LibraryArg_Curve info = new xHud_LibraryArg_Curve(name, curve);

                            sp_CurveLibrary.InsertArrayElementAtIndex(i + size);
                            SerializedProperty sp_cur = sp_CurveLibrary.GetArrayElementAtIndex(i + size);
                            SerializedProperty sp_name = sp_cur.FindPropertyRelative("Name");
                            SerializedProperty sp_curve = sp_cur.FindPropertyRelative("Curve");

                            sp_name.stringValue = info.Name;
                            sp_curve.animationCurveValue = info.Curve;

                            sp_name.serializedObject.ApplyModifiedProperties();
                            sp_curve.serializedObject.ApplyModifiedProperties();
                            sp_cur.serializedObject.ApplyModifiedProperties();
                        }
                        sp_CurveLibrary.serializedObject.ApplyModifiedProperties();

                        if (BaseScript.act_on_CurveChanged != null)
                            BaseScript.act_on_CurveChanged();
                    }
                    break;
                case "替换":
                    path = EditorUtility.OpenFilePanel("读取曲线库数据文件", Application.dataPath, "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        string content = File.ReadAllText(path);
                        CurveLib lib = JsonUtility.FromJson<CurveLib>(content);

                        sp_CurveLibrary.ClearArray();

                        for (int i = 0; i < lib.Nodes.Count; i++)
                        {
                            string name = lib.Nodes[i].Name;

                            Keyframe[] frames = new Keyframe[lib.Nodes[i].Frames.Length];

                            for (int s = 0; s < frames.Length; s++)
                            {
                                frames[s] = new Keyframe(
                                    lib.Nodes[i].Frames[s].time,
                                    lib.Nodes[i].Frames[s].value,
                                    lib.Nodes[i].Frames[s].inTangent,
                                    lib.Nodes[i].Frames[s].outTangent,
                                    lib.Nodes[i].Frames[s].inWeight,
                                    lib.Nodes[i].Frames[s].outWeight);
                            }

                            AnimationCurve curve = new AnimationCurve(frames);

                            xHud_LibraryArg_Curve info = new xHud_LibraryArg_Curve(name, curve);

                            sp_CurveLibrary.InsertArrayElementAtIndex(i);
                            SerializedProperty sp_cur = sp_CurveLibrary.GetArrayElementAtIndex(i);
                            SerializedProperty sp_name = sp_cur.FindPropertyRelative("Name");
                            SerializedProperty sp_curve = sp_cur.FindPropertyRelative("Curve");

                            sp_name.stringValue = info.Name;
                            sp_curve.animationCurveValue = info.Curve;

                            sp_name.serializedObject.ApplyModifiedProperties();
                            sp_curve.serializedObject.ApplyModifiedProperties();
                            sp_cur.serializedObject.ApplyModifiedProperties();
                        }
                        sp_CurveLibrary.serializedObject.ApplyModifiedProperties();

                        if (BaseScript.act_on_CurveChanged != null)
                            BaseScript.act_on_CurveChanged();
                    }
                    break;
            }
        }

        private void Tween_Mover(Rect rect, float mul)
        {
            if (BaseScript.CurveLibrary.Count <= 0)
                return;

            float start = rect.x;
            float end = rect.width - (BaseScript.SelectedReferImage.width * mul);
            if (sp_UsePreviewLooped.boolValue)
                CurveTween = XTween.To(() => TweenValue, x => TweenValue = x, end, sp_PreviewDuration.floatValue).SetFrom(start).SetEase(sp_CurveLibrary.GetArrayElementAtIndex(sp_PreviewCurveIndex.intValue).FindPropertyRelative("Curve").animationCurveValue).SetAutoKill(true).SetLoop(-1, XTween_LoopType.Restart);
            else
                CurveTween = XTween.To(() => TweenValue, x => TweenValue = x, end, sp_PreviewDuration.floatValue).SetFrom(start).SetEase(sp_CurveLibrary.GetArrayElementAtIndex(sp_PreviewCurveIndex.intValue).FindPropertyRelative("Curve").animationCurveValue).SetAutoKill(true);
        }

        private void Tween_Rotater()
        {
            if (BaseScript.CurveLibrary.Count <= 0)
                return;

            float start = 0;
            float end = 360;
            if (sp_UsePreviewLooped.boolValue)
                CurveTween = XTween.To(() => TweenDeg, x => TweenDeg = x, end, sp_PreviewDuration.floatValue).SetFrom(start).SetEase(sp_CurveLibrary.GetArrayElementAtIndex(sp_PreviewCurveIndex.intValue).FindPropertyRelative("Curve").animationCurveValue).SetAutoKill(true).SetLoop(-1, XTween_LoopType.Restart);
            else
                CurveTween = XTween.To(() => TweenDeg, x => TweenDeg = x, end, sp_PreviewDuration.floatValue).SetFrom(start).SetEase(sp_CurveLibrary.GetArrayElementAtIndex(sp_PreviewCurveIndex.intValue).FindPropertyRelative("Curve").animationCurveValue).SetAutoKill(true);
        }

        private void Tween_Scaler(float maxsize)
        {
            if (BaseScript.CurveLibrary.Count <= 0)
                return;

            float start = 0;
            float end = maxsize;
            if (sp_UsePreviewLooped.boolValue)
                CurveTween = XTween.To(() => TweenScl, x => TweenScl = x, end, sp_PreviewDuration.floatValue).SetFrom(start).SetEase(sp_CurveLibrary.GetArrayElementAtIndex(sp_PreviewCurveIndex.intValue).FindPropertyRelative("Curve").animationCurveValue).SetAutoKill(true).SetLoop(-1, XTween_LoopType.Restart);
            else
                CurveTween = XTween.To(() => TweenScl, x => TweenScl = x, end, sp_PreviewDuration.floatValue).SetFrom(start).SetEase(sp_CurveLibrary.GetArrayElementAtIndex(sp_PreviewCurveIndex.intValue).FindPropertyRelative("Curve").animationCurveValue).SetAutoKill(true);
        }

        private void Tween_Alpha(float alpha)
        {
            if (BaseScript.CurveLibrary.Count <= 0)
                return;

            float start = 0;
            float end = alpha;
            if (sp_UsePreviewLooped.boolValue)
                CurveTween = XTween.To(() => TweenAlpha, x => TweenAlpha = x, end, sp_PreviewDuration.floatValue).SetFrom(start).SetEase(sp_CurveLibrary.GetArrayElementAtIndex(sp_PreviewCurveIndex.intValue).FindPropertyRelative("Curve").animationCurveValue).SetAutoKill(true).SetLoop(-1, XTween_LoopType.Restart);
            else
                CurveTween = XTween.To(() => TweenAlpha, x => TweenAlpha = x, end, sp_PreviewDuration.floatValue).SetFrom(start).SetEase(sp_CurveLibrary.GetArrayElementAtIndex(sp_PreviewCurveIndex.intValue).FindPropertyRelative("Curve").animationCurveValue).SetAutoKill(true);
        }

        private void Tween_Stop()
        {
            if (CurveTween != null)
                CurveTween.Rewind();
            if (CurveTween != null)
                CurveTween.Kill();
            if (CurveTween != null)
                CurveTween = null;
            TweenPlaying = false;
            //DOTweenEditorPreview.Stop();
        }

        /// <summary>
        /// 延时停止预览
        /// </summary>
        /// <param name="Time"></param>
        async void DelayPlay(float time)
        {
            float sec = time * 1000;
            await Task.Delay((int)sec);
            Tween_Stop();
        }

        /// <summary>
        /// 获取目标文件夹下的指定类型所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        List<T> LoadAllAssetsAtPathWithPattern<T>(string path, string pattern) where T : UnityEngine.Object
        {
            List<T> _out = new();

            string root_path = Directory.GetParent(Application.dataPath) + "/" + path;

            if (!Directory.Exists(root_path))
            {
                Debug.LogWarning("Path doesn't exist");
                return _out;
            }

            string[] fileEntries = Directory.GetFiles(root_path, $"*{pattern}");

            foreach (string FileName in fileEntries)
            {
                string[] filepath = FileName.Split(Directory.GetParent(Application.dataPath).FullName + "/");
                _out.Add(AssetDatabase.LoadAssetAtPath<T>(filepath[1]));
            }

            return _out;
        }
        #endregion

        #region 预览
        public override GUIContent GetPreviewTitle()
        {
            return new GUIContent(PreviewHeader);
        }

        public override void OnPreviewSettings()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("间距", GUILayout.Width(30));
            sp_PreviewGridSize.floatValue = Mathf.Clamp(EditorGUILayout.FloatField(sp_PreviewGridSize.floatValue, GUILayout.Width(25)), 10, 50);
            sp_PreviewGridSize.serializedObject.ApplyModifiedProperties();
            Editor_XHud_GUI.Gui_Layout_Space(8);
            EditorGUILayout.LabelField("耗时", GUILayout.Width(30));
            sp_PreviewDuration.floatValue = Mathf.Clamp(EditorGUILayout.FloatField(sp_PreviewDuration.floatValue, GUILayout.Width(25)), 0.1f, 10);
            sp_PreviewDuration.serializedObject.ApplyModifiedProperties();
            Editor_XHud_GUI.Gui_Layout_Space(8);
            EditorGUILayout.LabelField("尺寸", GUILayout.Width(30));
            sp_PreviewImageSize.floatValue = Mathf.Clamp(EditorGUILayout.FloatField(sp_PreviewImageSize.floatValue, GUILayout.Width(25)), 0.1f, 0.8f);
            sp_PreviewImageSize.serializedObject.ApplyModifiedProperties();
            Editor_XHud_GUI.Gui_Layout_Space(5);
            if (EditorGUI.EndChangeCheck())
            {
                Tween_Stop();
            }
        }

        bool sws;

        public override bool HasPreviewGUI()
        {
            return sws;
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            base.OnPreviewGUI(rect, background);
        }

        public override void OnInteractivePreviewGUI(Rect rect, GUIStyle background)
        {
            GUI.color = sp_PreviewBGColor.colorValue;
            Editor_XHud_GUI.Gui_Box(rect);

            GUI.color = sp_PreviewGridColor.colorValue;
            float Div = rect.width / 2;
            float DivCount = sp_PreviewGridSize.floatValue;

            for (int s = 0; s < Div; s++)
            {
                EditorGUI.DrawRect(new Rect(rect.x + s * DivCount, rect.y, 1, rect.height), new Color(0.5f, 0.5f, 0.5f, 1));
            }
            for (int s = 0; s < Div; s++)
            {
                EditorGUI.DrawRect(new Rect(rect.x, rect.y + s * DivCount, rect.width, 1), new Color(0.5f, 0.5f, 0.5f, 1));
            }

            GUI.color = Color.white;

            if (BaseScript.SelectedReferImage == null)
            {
                return;
            }

            float mul = (rect.height / BaseScript.SelectedReferImage.height) * sp_PreviewImageSize.floatValue;
            float up = ((rect.height + 38) / 2) - (((BaseScript.SelectedReferImage.height) * mul) / 2);

            float width = BaseScript.SelectedReferImage.width * mul;
            float height = BaseScript.SelectedReferImage.height * mul;

            if (!TweenPlaying)
            {
                switch (sp_PreviewModeIndex.intValue)
                {
                    case 0:
                        TweenValue = 0;
                        TweenScl = 1;
                        TweenAlpha = 1;
                        break;
                    case 1:
                        TweenValue = (rect.width / 2) - (width / 2);
                        TweenScl = 1;
                        TweenAlpha = 1;
                        break;
                    case 2:
                        TweenValue = (rect.width / 2) - (width / 2);
                        TweenScl = 1;
                        TweenAlpha = 1;
                        break;
                    case 3:
                        TweenValue = (rect.width / 2) - (width / 2);
                        TweenScl = 1;
                        TweenAlpha = 1;
                        break;
                }
            }

            #region 保存GUI转换矩阵
            Matrix4x4 matrix4X4 = GUI.matrix;
            GUIUtility.RotateAroundPivot(TweenDeg, new Vector2((rect.width / 2), up + height / 2));
            GUIUtility.ScaleAroundPivot(new Vector2(TweenScl, TweenScl), new Vector2((rect.width / 2), up + height / 2));
            GUI.color = new Color(1, 1, 1, TweenAlpha);
            Editor_XHud_GUI.Gui_Icon(new Rect(TweenValue, up, width, height), BaseScript.SelectedReferImage);
            GUI.matrix = matrix4X4;
            GUI.color = Color.white;
            #endregion

            #region 翻页切换
            if (Editor_XHud_GUI.Gui_Button(new Rect(rect.x + 15, (rect.height - 10), 14, 14), leftarr_r, leftarr_p, true, "", "", Color.white))
            {
                if (sp_ReferImgIndex.intValue == 0)
                    sp_ReferImgIndex.intValue = ReferImages.Length - 1;
                else
                    sp_ReferImgIndex.intValue -= 1;
                sp_ReferImgIndex.serializedObject.ApplyModifiedProperties();
                BaseScript.SelectedReferImage = ReferImages[sp_ReferImgIndex.intValue];

                Tween_Stop();
            }

            if (Editor_XHud_GUI.Gui_Button(new Rect(rect.width - 40, (rect.height - 10), 14, 14), rightarr_r, rightarr_p, true, "", "", Color.white))
            {
                if (sp_ReferImgIndex.intValue >= ReferImages.Length - 1)
                    sp_ReferImgIndex.intValue = 0;
                else
                    sp_ReferImgIndex.intValue += 1;
                sp_ReferImgIndex.serializedObject.ApplyModifiedProperties();
                BaseScript.SelectedReferImage = ReferImages[sp_ReferImgIndex.intValue];

                Tween_Stop();
            }
            #endregion

            #region 播放预览
            if (!TweenPlaying)
            {
                if (Editor_XHud_GUI.Gui_Button(new Rect((rect.width / 2) - 7, (rect.height - 10), 14, 14), play_r, play_p, true, "", "", Color.white))
                {
                    TweenPlaying = true;

                    switch (sp_PreviewModeIndex.intValue)
                    {
                        case 0:
                            Tween_Mover(rect, mul);
                            break;
                        case 1:
                            Tween_Rotater();
                            break;
                        case 2:
                            Tween_Scaler(1);
                            break;
                        case 3:
                            Tween_Alpha(TweenAlpha);
                            break;
                    }
                    //DOTweenEditorPreview.PrepareTweenForPreview(CurveTween);
                    //DOTweenEditorPreview.Start();

                    if (sp_UsePreviewAutoStop.boolValue)
                        DelayPlay(sp_PreviewDuration.floatValue);
                }
            }
            else
            {
                if (Editor_XHud_GUI.Gui_Button(new Rect((rect.width / 2) - 7, (rect.height - 10), 14, 14), stop_r, stop_p, true, "", "", Color.white))
                {
                    Tween_Stop();
                    TweenValue = 0;
                }
                Repaint();
            }
            #endregion

            #region 模式切换
            string[] options = new string[4] { "位移", "旋转", "缩放", "透明度" };
            EditorGUI.BeginChangeCheck();
            sp_PreviewModeIndex.intValue = Editor_XHud_GUI.Gui_Popup(new Rect((rect.width / 2) - 50, rect.y + 8, 100, 20), sp_PreviewModeIndex.intValue, options, HudFilled.实体, HudColor.警示黄, Color.black);
            sp_PreviewModeIndex.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                Tween_Stop();
            }
            #endregion

            #region 停止预览方式
            string autostop = "手动停止";
            if (sp_UsePreviewAutoStop.boolValue)
            {
                autostop = " 自动停止";
            }
            else
            {
                autostop = " 手动停止";
            }

            if (Editor_XHud_GUI.Gui_Button(new Rect(rect.x + 10, rect.y + 8, 60, 20), null, null, false, autostop, "", Color.clear, XHud_Dashboard.Theme_Primary, HudFilled.透明))
            {
                sp_UsePreviewAutoStop.boolValue = !sp_UsePreviewAutoStop.boolValue;
                sp_UsePreviewAutoStop.serializedObject.ApplyModifiedProperties();

                if (sp_UsePreviewAutoStop.boolValue)
                    sp_UsePreviewLooped.boolValue = false;
                sp_UsePreviewLooped.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 循环模式
            string cycle = "循环预览";
            if (sp_UsePreviewLooped.boolValue)
            {
                cycle = "循环预览";
            }
            else
            {
                cycle = "单次预览";
            }

            if (Editor_XHud_GUI.Gui_Button(new Rect(rect.width - 70, rect.y + 8, 60, 20), null, null, false, cycle, "", Color.clear, XHud_Dashboard.Theme_Primary, HudFilled.透明))
            {
                sp_UsePreviewLooped.boolValue = !sp_UsePreviewLooped.boolValue;
                if (sp_UsePreviewLooped.boolValue)
                    sp_UsePreviewAutoStop.boolValue = false;
                sp_UsePreviewLooped.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 拖放自定义贴图
            Event e = Event.current;

            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            if (e.type == EventType.DragPerform && e.button == 0)
            {
                if (rect.Contains(e.mousePosition))
                {
                    DragAndDrop.AcceptDrag();
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; ++i)
                    {
                        var o = DragAndDrop.objectReferences[i];

                        Texture2D tex = (Texture2D)o;
                        BaseScript.SelectedReferImage = tex;
                    }
                }
                e.Use();
            }
            #endregion
        }
        #endregion
    }
}