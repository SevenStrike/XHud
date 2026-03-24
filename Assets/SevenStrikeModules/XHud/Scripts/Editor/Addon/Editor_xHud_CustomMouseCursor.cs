namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.UI;

    [CustomEditor(typeof(xHud_CustomMouseCursor), true)]
    public class Editor_xHud_CustomMouseCursor : Editor
    {
        #region 组件 / 列表
        private xHud_CustomMouseCursor BaseScript;
        private ReorderableList StyleList;
        #endregion

        #region 序列化属性
        private SerializedProperty CursorRect, CursorCanvasGroup, CursorImager, UseCustomCursor, MouseStyles, CursorSize_TweenDuration, CursorSize_TweenEase, CursorImagerSizeSmooth, CursorOpacity_TweenDuration, CursorOpacity_TweenEase, CursorSize, CursorColor, CursorColorSmooth, CursorOpacity, IndexMouseType, UseLerpCursorSize, CurrentCursorStyle;
        #endregion

        private bool BasicVars;
        private float LineHeight;
        private bool IsMouseStyles;

        #region 图标
        private Texture2D icon_main;
        #endregion

        #region 批量化操作
        private xHud_CustomMouseCursor[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new xHud_CustomMouseCursor[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (xHud_CustomMouseCursor)t;
                }
            }
            else
            {
                SelectedObjects = new xHud_CustomMouseCursor[targets.Length];
                SelectedObjects[0] = (xHud_CustomMouseCursor)target;
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

        void OnEnable()
        {
            BaseScript = (xHud_CustomMouseCursor)target;

            #region 获取序列化属性
            CursorRect = serializedObject.FindProperty("CursorRect");
            CursorCanvasGroup = serializedObject.FindProperty("CursorCanvasGroup");
            UseCustomCursor = serializedObject.FindProperty("UseCustomCursor");
            CursorOpacity = serializedObject.FindProperty("CursorOpacity");
            CursorImager = serializedObject.FindProperty("CursorImager");
            MouseStyles = serializedObject.FindProperty("MouseStyles");
            CursorSize_TweenDuration = serializedObject.FindProperty("CursorSize_TweenDuration");
            CursorSize_TweenEase = serializedObject.FindProperty("CursorSize_TweenEase");
            CursorImagerSizeSmooth = serializedObject.FindProperty("CursorImagerSizeSmooth");
            CursorOpacity_TweenDuration = serializedObject.FindProperty("CursorOpacity_TweenDuration");
            CursorOpacity_TweenEase = serializedObject.FindProperty("CursorOpacity_TweenEase");
            CursorSize = serializedObject.FindProperty("CursorSize");
            CursorColor = serializedObject.FindProperty("CursorColor");
            CursorColorSmooth = serializedObject.FindProperty("CursorColorSmooth");
            CurrentCursorStyle = serializedObject.FindProperty("CurrentCursorStyle");
            IndexMouseType = serializedObject.FindProperty("IndexMouseType");
            UseLerpCursorSize = serializedObject.FindProperty("UseLerpCursorSize");
            #endregion

            #region 获取图标
            icon_main = Editor_xHudGUI.GetIcon("Icons_Hud_MouseCursor/icon_main");
            #endregion

            BaseScript.CursorRect = BaseScript.GetComponent<RectTransform>();
            BaseScript.CursorCanvasGroup = BaseScript.GetComponent<CanvasGroup>();

            if (BaseScript.transform.childCount <= 0)
            {
                GameObject imager = new GameObject();
                imager.transform.SetParent(BaseScript.transform);
                imager.transform.localPosition = Vector3.zero;
                imager.transform.localEulerAngles = Vector3.zero;
                imager.transform.localScale = Vector3.one;
                imager.name = "Imager";
                imager.layer = LayerMask.NameToLayer("XHud");
                RectTransform rect = imager.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(10, 10);
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                Image img = imager.AddComponent<Image>();
                img.raycastTarget = false;
                BaseScript.CursorImager = img;
            }
            else
            {
                BaseScript.CursorImager = BaseScript.transform.GetChild(0).GetComponent<Image>();
            }

            LineHeight = EditorGUIUtility.singleLineHeight;

            GetAllTargets();

            #region ReorderableList - MouseStyles
            StyleList = new ReorderableList(serializedObject, MouseStyles)
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "鼠标样式表");
                },

                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 2;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_name = MouseStyles.GetArrayElementAtIndex(index).FindPropertyRelative("Name");

                    SerializedProperty index_sprite = MouseStyles.GetArrayElementAtIndex(index).FindPropertyRelative("Sprite");
                    Sprite spr = (Sprite)index_sprite.objectReferenceValue;

                    SerializedProperty pivot = MouseStyles.GetArrayElementAtIndex(index).FindPropertyRelative("Pivot");

                    #region 光标名称
                    sp_name.stringValue = Editor_xHudGUI.Gui_TextField(new Rect(rect.width - (rect.width - 60), titleheight, 70, LineHeight), sp_name.stringValue);
                    sp_name.serializedObject.ApplyModifiedProperties();

                    Editor_xHudGUI.Gui_SerializePropertyWithObject(new Rect(rect.width - (rect.width - 57), titleheight + 30, 185, LineHeight), index_sprite);

                    if (spr != null)
                        sp_name.stringValue = spr.name;
                    index_sprite.serializedObject.ApplyModifiedProperties();
                    #endregion

                    #region 锚点位置
                    pivot.vector2Value = Editor_xHudGUI.Gui_InputField_Vector2(new Rect(rect.width - (rect.width - 140), titleheight + 2.5f, 100, LineHeight), pivot.vector2Value);
                    pivot.serializedObject.ApplyModifiedProperties();

                    SerializedProperty cur_sprite = CurrentCursorStyle.FindPropertyRelative("Sprite");
                    Sprite cur_spr = (Sprite)cur_sprite.objectReferenceValue;

                    if (cur_spr == spr)
                    {
                        Image img_cursor = (Image)CursorImager.objectReferenceValue;
                        img_cursor.rectTransform.pivot = pivot.vector2Value;
                        CursorImager.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    #region 预览光标
                    Texture2D tex = null;
                    Sprite icon_spr = (Sprite)index_sprite.objectReferenceValue;
                    if (icon_spr != null)
                    {
                        tex = icon_spr.texture;
                        Editor_xHudGUI.Gui_Icon(new Rect(rect.width + 30, titleheight + 15, 15, 15), tex);
                    }
                    #endregion
                },
                onAddCallback = (ReorderableList list) =>
                {
                    MouseStyles.InsertArrayElementAtIndex(list.count);

                    MouseStyles.GetArrayElementAtIndex(list.count - 1).FindPropertyRelative("Name").stringValue = "NewCursor";
                    MouseStyles.GetArrayElementAtIndex(list.count - 1).FindPropertyRelative("Pivot").vector2Value = new Vector2(0, 1);
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    MouseStyles.DeleteArrayElementAtIndex(list.index);
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_style = MouseStyles.GetArrayElementAtIndex(list.index);

                    MouseStyles style = new MouseStyles();

                    style.Name = sp_style.FindPropertyRelative("Name").stringValue;
                    style.Pivot = sp_style.FindPropertyRelative("Pivot").vector2Value;
                    style.Sprite = (Sprite)sp_style.FindPropertyRelative("Sprite").objectReferenceValue;

                    CurrentCursorStyle.FindPropertyRelative("Name").stringValue = style.Name;
                    CurrentCursorStyle.FindPropertyRelative("Pivot").vector2Value = style.Pivot;
                    CurrentCursorStyle.FindPropertyRelative("Sprite").objectReferenceValue = style.Sprite;

                    Image img = (Image)CursorImager.objectReferenceValue;
                    img.sprite = style.Sprite;
                    img.rectTransform.pivot = CurrentCursorStyle.FindPropertyRelative("Pivot").vector2Value;
                },
                elementHeightCallback = index =>
                {
                    return 3f * LineHeight;
                }
            };
            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Editor_xHudGUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "Hud - 鼠标样式", Color.white);

            #region 控制
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "控制", xHud_Dashboard.Theme_Primary);

            if (!Application.isPlaying)
            {
                CanvasGroup cg = (CanvasGroup)CursorCanvasGroup.objectReferenceValue;
                Image imgsursor = (Image)CursorImager.objectReferenceValue;
                if (cg != null)
                    cg.alpha = CursorOpacity.floatValue;
                CursorCanvasGroup.serializedObject.ApplyModifiedProperties();

                if (imgsursor != null)
                {
                    imgsursor.rectTransform.sizeDelta = Vector2.one * CursorSize.floatValue;
                }
                CursorImager.serializedObject.ApplyModifiedProperties();
            }

            if (UseCustomCursor.boolValue)
            {
                #region 当前选择的光标图标
                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(10);
                GUILayout.FlexibleSpace();

                SerializedProperty index_sprite = CurrentCursorStyle.FindPropertyRelative("Sprite");
                Sprite spr = (Sprite)index_sprite.objectReferenceValue;

                GUI.color = CursorColor.colorValue;
                if (spr != null)
                    Editor_xHudGUI.Gui_Layout_Icon(25, spr.texture, new Vector2(0, 0));
                GUI.color = Color.white;

                GUILayout.FlexibleSpace();
                Editor_xHudGUI.Gui_Layout_Space(10);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();
                #endregion
            }

            #region 光标位置
            if (Application.isPlaying)
            {
                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(10);
                Vector3 mpos = Input.mousePosition;
                Editor_xHudGUI.Gui_Layout_Labelfield("光标X轴：" + mpos.x, HudFilled.无, HudColor.无, Editor_xHudGUI.GetColor(HudColor.魅力红), TextAnchor.MiddleLeft);
                Editor_xHudGUI.Gui_Layout_Space(10);
                Editor_xHudGUI.Gui_Layout_Labelfield("光标Y轴：" + mpos.y, HudFilled.无, HudColor.无, Editor_xHudGUI.GetColor(HudColor.柠檬绿), TextAnchor.MiddleRight);
                Editor_xHudGUI.Gui_Layout_Space(10);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();
            }
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(5);

            Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_CustomMouseCursor>("自定义光标", new string[] { "禁用", "启用" }, ref UseCustomCursor, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            string[] actionlist = System.Enum.GetNames(typeof(MouseType));
            Editor_xHudGUI.Gui_Layout_Popup<string, xHud_CustomMouseCursor>("鼠标响应", actionlist, ref IndexMouseType, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
            {
                BaseScript.MouseClickType = (MouseType)System.Enum.Parse(typeof(MouseType), res);
            });

            string[] smoothsizelist = new string[2] { "缓动模式", "差值模式" };
            Editor_xHudGUI.Gui_Layout_Popup<string, xHud_CustomMouseCursor>("光标尺寸平滑方式", smoothsizelist, ref UseLerpCursorSize, HudFilled.实体, 120, 22, SelectedObjects);

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 参数
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(10);

            Image img_cursor = (Image)CursorImager.objectReferenceValue;

            if (UseCustomCursor.boolValue)
            {
                img_cursor.enabled = true;

                Editor_xHudGUI.Gui_Layout_Property_Field("焦点物体", CursorRect);

                Editor_xHudGUI.Gui_Layout_Space(5);

                Editor_xHudGUI.Gui_Layout_Property_Field("透明组件", CursorCanvasGroup);

                Editor_xHudGUI.Gui_Layout_Space(5);

                Editor_xHudGUI.Gui_Layout_Property_Field("光标组件", CursorImager);

                Editor_xHudGUI.Gui_Layout_Space(5);

                Editor_xHudGUI.Gui_Layout_Property_Field("尺寸", CursorSize);

                Editor_xHudGUI.Gui_Layout_Space(5);

                EditorGUI.BeginChangeCheck();
                Editor_xHudGUI.Gui_Layout_Property_Field("颜色", CursorColor);
                if (EditorGUI.EndChangeCheck())
                {
                    BaseScript.CursorImager.color = CursorColor.colorValue;
                }

                Editor_xHudGUI.Gui_Layout_Space(5);

                Editor_xHudGUI.Gui_Layout_Property_Field("颜色平滑系数", CursorColorSmooth, 90);

                Editor_xHudGUI.Gui_Layout_Space(5);

                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(5);
                CursorOpacity.floatValue = Editor_xHudGUI.Gui_Layout_Slider("透明度", CursorOpacity.floatValue, 0, 1);
                CursorOpacity.serializedObject.ApplyModifiedProperties();
                Editor_xHudGUI.Gui_Layout_Horizontal_End();

                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Property_Field("透明度缓动方式", CursorOpacity_TweenEase, 120);
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Property_Field("透明度变化耗时", CursorOpacity_TweenDuration, 120);


                if (UseLerpCursorSize.stringValue == "缓动模式")
                {
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Property_Field("光标尺寸缓动耗时", CursorSize_TweenDuration, 120);

                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Property_Field("光标尺寸缓动方式", CursorSize_TweenEase, 120);
                }
                else
                {
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Property_Field("光标尺寸差值速率", CursorImagerSizeSmooth, 120);
                }
            }
            else
            {
                if (img_cursor != null)
                    img_cursor.enabled = false;
            }

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 光标样式表
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "光标样式表", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(10);

            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_xHudGUI.Gui_Layout_Space(10);
            IsMouseStyles = EditorGUILayout.Foldout(IsMouseStyles, "光标样式表");
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();

            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_xHudGUI.Gui_Layout_Space(5);
            if (IsMouseStyles)
                StyleList.DoLayoutList();
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 源脚本
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(5);

            #region 原始变量
            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_xHudGUI.Gui_Layout_Space(10);
            BasicVars = EditorGUILayout.Foldout(BasicVars, "变量/属性", true);
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();
            if (BasicVars)
                DrawDefaultInspector();
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}