namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public class Editor_XHud_LayoutSpawner_ValueBinder_SetTool : EditorWindow
    {
        #region 字段
        /// <summary>
        /// 目标绑定器组件
        /// </summary>
        private XHud_LayoutSpawner_ValueBinder targetBinder;
        /// <summary>
        /// 绑定的数据列表
        /// </summary>
        [SerializeField] public List<ElementBinder> valuebinders;
        /// <summary>
        /// 可重排序列表
        /// </summary>
        private ReorderableList reorderableList;
        /// <summary>
        /// 序列化对象
        /// </summary>
        private SerializedObject so;
        /// <summary>
        /// 滚动视图位置
        /// </summary>
        private Vector2 scrollPosition;
        /// <summary>
        /// 是否已初始化
        /// </summary>
        private bool isInitialized = false;
        /// <summary>
        /// 选中的列表项索引
        /// </summary>
        private int selectedIndex = -1;
        /// <summary>
        /// 折叠状态字典（用于存储每个绑定器的子项折叠状态）
        /// </summary>
        private Dictionary<int, bool> foldoutStates = new Dictionary<int, bool>();
        /// <summary>
        /// 是否需要刷新列表
        /// </summary>
        private bool needsRefresh = false;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;
        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 行高设定
        /// </summary>
        Font Font_Dialog;
        private bool isDraggingOver = false;
        #endregion

        #region 图标
        private Texture2D icon_add_r, icon_add_p, icon_clear_r, icon_clear_p, icon_refresh_r, icon_refresh_p, icon_save_r, icon_save_p, icon_import, icon_image, icon_rawimage, icon_text, icon_tmptext, icon_transform, icon_locate_r, icon_locate_p, icon_remove_r, icon_remove_p, icon_locate_small_r, icon_locate_small_p;
        #endregion

        #region 窗口生命周期
        private void OnEnable()
        {
            InitIcons();

            Font_Light = Editor_XHud_GUI.GetFont("sx_regular");
            Font_Bold = Editor_XHud_GUI.GetFont("sx_bold");
            Font_Dialog = Editor_XHud_GUI.GetFont("sx_regular");

            // 获取当前选中的目标组件
            TryFindTargetBinder();

            // 同步数据
            SyncDataFromTarget();

            // 初始化序列化对象
            so = new SerializedObject(this);

            // 初始化ReorderableList
            InitReorderableList();

            isInitialized = true;
        }

        private void OnFocus()
        {
            // 窗口获得焦点时重新同步数据
            if (targetBinder != null)
            {
                //SyncDataFromTarget();
                so?.Update();
                //InitReorderableList(); // 重新初始化列表以刷新数据
                Repaint();
            }
        }

        private void Update()
        {
            // 处理延迟刷新
            if (needsRefresh)
            {
                needsRefresh = false;
                InitReorderableList();
                Repaint();
            }
        }

        private void OnDestroy()
        {
            isInitialized = false;

            CrosshairTool.AimDisplay(false);
            CrosshairTool.ClearTarget();
        }

        /// <summary>
        /// 尝试查找目标绑定器组件
        /// </summary>
        private void TryFindTargetBinder()
        {
            if (Selection.activeGameObject != null)
            {
                targetBinder = Selection.activeGameObject.GetComponent<XHud_LayoutSpawner_ValueBinder>();
                if (targetBinder == null)
                {
                    targetBinder = FindFirstObjectByType<XHud_LayoutSpawner_ValueBinder>();
                }
            }

            if (targetBinder == null)
            {
                targetBinder = FindFirstObjectByType<XHud_LayoutSpawner_ValueBinder>();
            }
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化图标
        /// </summary>
        private void InitIcons()
        {
            icon_add_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_add_r");
            icon_add_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_add_p");
            icon_clear_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_clear_r");
            icon_clear_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_clear_p");
            icon_refresh_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_refresh_r");
            icon_refresh_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_refresh_p");
            icon_save_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_save_r");
            icon_save_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_save_p");
            icon_import = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_import");
            icon_image = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_image");
            icon_rawimage = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_rawimage");
            icon_text = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_text");
            icon_tmptext = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_tmptext");
            icon_transform = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_transform");
            icon_locate_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_locate_r");
            icon_locate_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_locate_p");
            icon_remove_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_remove_r");
            icon_remove_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_remove_p");
            icon_locate_small_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_locate_small_r");
            icon_locate_small_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder_SetTool/icon_locate_small_p");
        }
        /// <summary>
        /// 从目标组件同步数据
        /// </summary>
        private void SyncDataFromTarget()
        {
            if (targetBinder != null)
            {
                // 深拷贝数据
                if (targetBinder.valuebinders != null)
                {
                    valuebinders = new List<ElementBinder>();
                    foreach (var binder in targetBinder.valuebinders)
                    {
                        valuebinders.Add(CopyValueBinder(binder));
                    }
                }
                else
                {
                    valuebinders = new List<ElementBinder>();
                }
            }
            else
            {
                valuebinders = new List<ElementBinder>();
            }
        }
        /// <summary>
        /// 深拷贝valuebinder
        /// </summary>
        private ElementBinder CopyValueBinder(ElementBinder source)
        {
            ElementBinder dest = new ElementBinder();
            dest.ID = source.ID;
            dest.Name = source.Name;
            dest.IsExpanded = false;
            dest.IsEnabled = source.IsEnabled;

            if (source.binders != null)
            {
                dest.binders = new PrimitiveBinder[source.binders.Length];
                for (int i = 0; i < source.binders.Length; i++)
                {
                    dest.binders[i] = CopyValueBinderType(source.binders[i]);
                }
            }
            else
            {
                dest.binders = new PrimitiveBinder[0];
            }

            return dest;
        }
        /// <summary>
        /// 深拷贝valuebindertype
        /// </summary>
        private PrimitiveBinder CopyValueBinderType(PrimitiveBinder source)
        {
            PrimitiveBinder dest = new PrimitiveBinder();
            dest.ID = source.ID;
            dest.Name = source.Name;
            dest.val_string = source.val_string;
            dest.val_sprite = source.val_sprite;
            dest.val_texture = source.val_texture;
            dest.val_color = source.val_color;
            dest.type = source.type;
            return dest;
        }
        /// <summary>
        /// 初始化ReorderableList - 使用原生滚动，避免虚拟滚动问题
        /// </summary>
        private void InitReorderableList()
        {
            if (valuebinders == null)
                return;

            // ⭐ 初始化折叠状态字典，从 binder.IsFold 读取初始值
            for (int i = 0; i < valuebinders.Count; i++)
            {
                if (!foldoutStates.ContainsKey(i))
                {
                    foldoutStates[i] = valuebinders[i].IsExpanded;
                }
            }

            reorderableList = new ReorderableList(valuebinders, typeof(ElementBinder), true, true, true, true);

            // 绘制头部
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "", EditorStyles.boldLabel);
            };

            // 绘制元素
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                DrawElement(rect, index, isActive, isFocused);
            };

            // 元素高度回调 - 动态计算
            reorderableList.elementHeightCallback = (int index) =>
            {
                return GetElementHeight(index);
            };

            // 添加元素回调
            reorderableList.onAddCallback = (ReorderableList list) =>
            {
                AddBinder_Element();
            };

            // 移除元素回调
            reorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                if (list.index >= 0 && list.index < valuebinders.Count)
                {
                    EditorApplication.delayCall += () =>
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素数据绑定器通知", "移除元素绑定数据", $"确定要移除名称为 \"{valuebinders[list.index].Name}\" 的元素绑定数据吗？", "移除", "暂不", 0);
                        if (res == "暂不")
                            return;

                        RemoveBinder(list.index);
                    };
                }
            };

            // 选择改变回调
            reorderableList.onSelectCallback = (ReorderableList list) =>
            {
                selectedIndex = list.index;
            };

            // 拖拽回调
            reorderableList.onCanRemoveCallback = (ReorderableList list) =>
            {
                return list.index >= 0 && list.index < valuebinders.Count;
            };

            // 重新排序回调
            reorderableList.onReorderCallback = (ReorderableList list) =>
            {
                // 重新排序后需要更新折叠状态字典的键
                var oldFoldoutStates = new Dictionary<int, bool>(foldoutStates);
                foldoutStates.Clear();

                for (int i = 0; i < valuebinders.Count; i++)
                {
                    // 尝试找到原来对应索引的折叠状态
                    if (oldFoldoutStates.ContainsKey(i))
                    {
                        foldoutStates[i] = oldFoldoutStates[i];
                    }
                    else
                    {
                        foldoutStates[i] = true;
                    }
                }

                // 刷新列表
                EditorApplication.delayCall += () =>
                {
                    if (this != null)
                    {
                        InitReorderableList();
                        Repaint();
                    }
                };
            };
        }
        /// <summary>
        /// 获取元素高度（动态计算）
        /// </summary>
        private float GetElementHeight(int index)
        {
            if (index < 0 || index >= valuebinders.Count)
                return 30f;

            float baseHeight = 30f; // 头部高度（ID输入框+折叠按钮）

            // 检查折叠状态，如果展开则计算子项高度
            bool isExpanded = foldoutStates.ContainsKey(index) ? foldoutStates[index] : true;
            if (isExpanded)
            {
                ElementBinder binder = valuebinders[index];
                if (binder.binders != null && binder.binders.Length > 0)
                {
                    // 子项列表头高度
                    //baseHeight += 5f;

                    for (int i = 0; i < binder.binders.Length; i++)
                    {
                        baseHeight += GetBinderItemHeight();
                    }
                }
                else
                {
                    // 空状态提示高度
                    baseHeight += 44f;
                }

                // 添加按钮区域高度（带间距）
                baseHeight += 45f;
            }

            return baseHeight;
        }
        /// <summary>
        /// 获取绑定器子项高度（固定高度）
        /// </summary>
        private float GetBinderItemHeight()
        {
            return 165; //Name&Icon +  ID + 字符串 + Sprite/Texture 三行，每行22f
        }
        /// <summary>
        /// 绘制元素
        /// </summary>
        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= valuebinders.Count)
                return;

            ElementBinder binder = valuebinders[index];

            if (index != 0)
                // 绘制边框
                EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), new Color(0.5f, 0.5f, 0.5f, 0.2f));

            float currentY = rect.y + 4;

            // ========== 头部区域 ==========

            // 从字典获取折叠状态，如果字典中没有则从 binder 读取并同步到字典
            if (!foldoutStates.ContainsKey(index))
            {
                foldoutStates[index] = binder.IsExpanded;
            }
            binder.IsExpanded = foldoutStates[index];

            #region 绘制折叠箭头
            bool newExpanded = EditorGUI.Foldout(new Rect(rect.x + 6, currentY + 2, 16, 16), binder.IsExpanded, GUIContent.none);
            if (newExpanded != binder.IsExpanded)
            {
                binder.IsExpanded = newExpanded;                    // 修改副本
                foldoutStates[index] = newExpanded;             // 更新字典
                valuebinders[index] = binder;                   // 关键：将修改后的副本写回列表

                // 高度变化，刷新列表（使用延迟刷新避免递归）
                EditorApplication.delayCall += () =>
                {
                    if (this != null)
                    {
                        InitReorderableList();
                        Repaint();
                    }
                };
            }
            #endregion

            #region 是否启用绑定
            bool IsEnabled = EditorGUI.Toggle(new Rect(rect.x + 28, currentY + 2, 16, 16), binder.IsEnabled);
            if (IsEnabled != binder.IsEnabled)
            {
                binder.IsEnabled = IsEnabled;                    // 修改副本              
                valuebinders[index] = binder;                   // 关键：将修改后的副本写回列表               
            }
            #endregion

            #region ID标签
            GUIStyle boldLabel = new GUIStyle(EditorStyles.boldLabel);
            boldLabel.font = Font_Light;
            boldLabel.fontSize = 11;
            EditorGUI.LabelField(new Rect(rect.x + 58, currentY + 2, 30, 18), "ID", boldLabel);
            #endregion

            #region ID输入框
            GUIStyle style_textfield = new GUIStyle(EditorStyles.textField);
            style_textfield.font = Font_Light;
            style_textfield.fontSize = 12;
            style_textfield.fontStyle = FontStyle.Bold;
            style_textfield.padding = new RectOffset(style_textfield.padding.left + 5, style_textfield.padding.right + 5, style_textfield.padding.top, style_textfield.padding.bottom);
            string newID = EditorGUI.TextField(new Rect(rect.x + 80, currentY + 2, 60, 20), binder.ID, style_textfield);
            if (newID != binder.ID)
            {
                binder.ID = newID;
                valuebinders[index] = binder;
            }
            #endregion

            #region 定位
            if (Editor_XHud_GUI.Gui_Button(new Rect(rect.x + 160, currentY - 4, 40, 30), icon_locate_small_r, icon_locate_small_p, true, "", "", Color.white))
            {
                XHud_Module_Element target = targetBinder.layoutspawner.GetElement_With_ID(valuebinders[index].ID);

                if (target == null)
                    return;

                EditorGUIUtility.PingObject(target);
                Selection.activeObject = target;

                CrosshairTool.AimTo(target.RectTransform);
                return;
            }
            #endregion

            #region 元素Name
            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + (rect.width - 190), currentY + 2, 150, 18), binder.Name, HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleRight, 12, Font_Light);
            #endregion

            #region 绘制索引标签（右侧）
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 25, currentY + 3, 45, 18), $"# {index}", EditorStyles.miniLabel);
            #endregion

            currentY += 30;

            // 如果折叠，到此结束
            if (!binder.IsExpanded)
                return;

            #region 子项区域
            if (binder.binders != null && binder.binders.Length > 0)
            {
                // 绘制每个子项
                for (int i = 0; i < binder.binders.Length; i++)
                {
                    Rect itemRect = new Rect(rect.x + 15, currentY, rect.width - 25, GetBinderItemHeight());
                    DrawBinderItem(itemRect, index, i);
                    currentY += GetBinderItemHeight() + 2;
                }
            }
            else
            {
                // 提示没有子项
                string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
                Editor_XHud_GUI.Gui_Labelfield_WrapText(new Rect(rect.x + 10, currentY + 15, rect.width - 35, 40), "该元素下暂未绑定任何图元控制器", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.UpperLeft, Vector2.zero, 11, true, true, TextClipping.Ellipsis, true, Font_Light);

                currentY += 48;
            }
            #endregion

            #region 添加子项按钮
            Rect rect_addPrim = new Rect(rect.x + 5, currentY + 6, 90, 22);
            if (Editor_XHud_GUI.Gui_Button(rect_addPrim, null, null, false, "", "", Editor_XHud_GUI.GetColor(HudColor.深空灰), Color.white, HudFilled.实体))
            {
                AddBinder_Primitive(index);
            }
            GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
            rect_addPrim.Set(rect_addPrim.x + rect_addPrim.width / 2 - 6, rect_addPrim.y + rect_addPrim.height / 2 - 6, 12, 12);
            Editor_XHud_GUI.Gui_Icon(rect_addPrim, icon_add_r);
            GUI.backgroundColor = Color.white;
            #endregion
        }
        /// <summary>
        /// 根据类型获取图标
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Texture2D GetPrimtiveTypeIcon(ModuleType type)
        {
            Texture2D tex = null;
            switch (type)
            {
                case ModuleType.RectTransform:
                    tex = icon_transform;
                    break;
                case ModuleType.Text:
                    tex = icon_text;
                    break;
                case ModuleType.TmpText:
                    tex = icon_tmptext;
                    break;
                case ModuleType.Image:
                    tex = icon_image;
                    break;
                case ModuleType.RawImage:
                    tex = icon_rawimage;
                    break;
            }
            return tex;
        }
        /// <summary>
        /// 绘制绑定器子项
        /// </summary>
        private void DrawBinderItem(Rect rect, int parentIndex, int childIndex)
        {
            if (parentIndex >= valuebinders.Count)
                return;

            ElementBinder binder = valuebinders[parentIndex];
            if (childIndex >= binder.binders.Length)
                return;

            PrimitiveBinder binderItem = binder.binders[childIndex];

            #region  绘制背景和边框
            Color bgColor = new Color(0, 0, 0, 0.12f);
            Rect BgRect = new Rect(rect.x - 10, rect.y, rect.width + 20, rect.height);
            Editor_XHud_GUI.Gui_Box(BgRect, bgColor);
            #endregion

            float currentX = rect.x + 8;
            float currentY = rect.y + 8;

            #region 序号
            GUIStyle style_index = new GUIStyle(EditorStyles.label);
            style_index.fontSize = 11;
            style_index.normal.textColor = Color.white * 0.65f;
            EditorGUI.LabelField(new Rect(rect.x, currentY + 2, 25, 18), $"# {childIndex}", style_index);
            #endregion

            #region 图元类型图标 
            Texture2D icon = GetPrimtiveTypeIcon(binderItem.type);
            Editor_XHud_GUI.Gui_Icon(new Rect(rect.x + 35, currentY + 3, icon.width, icon.height), icon, XHud_Dashboard.Theme_Primary);
            #endregion

            #region 删除按钮（右上角）
            GUI.color = new Color(1f, 0.4f, 0.4f);
            if (Editor_XHud_GUI.Gui_Button(new Rect(rect.x + rect.width - 46, rect.y + 33, 50, 50), icon_remove_r, icon_remove_p, true, "", "", Color.white))
            {
                EditorApplication.delayCall += () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素数据绑定器通知", "移除图元绑定数据", $"确定要移除 \"{valuebinders[parentIndex].binders[childIndex].Name}\" 的图元绑定数据吗？", "移除", "暂不", 0);
                    if (res == "暂不")
                        return;

                    RemoveBinderSubItem(parentIndex, childIndex, false);
                };
                return;
            }
            GUI.color = Color.white;
            #endregion

            #region 定位图元按钮（右下角）
            GUI.color = XHud_Dashboard.Theme_Primary;
            if (Editor_XHud_GUI.Gui_Button(new Rect(rect.x + rect.width - 46, rect.y + rect.height - 60, 50, 50), icon_locate_r, icon_locate_p, true, "", "", Color.white))
            {
                // 开始瞄准
                XHud_Module_Element target = targetBinder.layoutspawner.GetElement_With_ID(XHudSpace.屏幕空间, binder.ID);

                if (target == null)
                    return;

                XHud_Module_Primitive_Controller prim = target.GetPrimitiveController_With_ID(binderItem.ID);

                EditorGUIUtility.PingObject(prim.gameObject);
                Selection.activeObject = prim.gameObject;

                CrosshairTool.AimTo(prim.mod_Rect);
                return;
            }

            GUI.color = Color.white;
            #endregion

            float labelWidth = 100;
            float fieldWidth = (rect.width - 85 - labelWidth);

            GUIStyle style_prop_name = new GUIStyle(EditorStyles.whiteLabel);
            style_prop_name.fontSize = 12;
            style_prop_name.font = Font_Light;

            GUIStyle style_prop_value = new GUIStyle(EditorStyles.textField);
            style_prop_value.fontSize = 12;
            style_prop_value.font = Font_Light;
            style_prop_value.fontStyle = FontStyle.Normal;

            #region 图元Name
            Editor_XHud_GUI.Gui_Labelfield(new Rect(currentX + 50, currentY, labelWidth, 18), binderItem.Name, HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.MiddleLeft, 12, Font_Light);
            #endregion

            currentY += 26;

            #region 图元ID 
            EditorGUI.LabelField(new Rect(currentX + 25, currentY + 2, labelWidth, 18), "ID", style_prop_name);
            string newID = EditorGUI.TextField(new Rect(currentX + 25 + labelWidth, currentY + 1, fieldWidth, 18), binderItem.ID, style_prop_value);
            if (newID != binderItem.ID)
            {
                binderItem.ID = newID;
                binder.binders[childIndex] = binderItem;
                valuebinders[parentIndex] = binder;
            }
            #endregion

            currentY += 26;

            #region 值：String
            EditorGUI.LabelField(new Rect(currentX + 25, currentY + 2, labelWidth, 18), "String:", style_prop_name);
            string newString = EditorGUI.TextField(new Rect(currentX + 25 + labelWidth, currentY, fieldWidth, 18), binderItem.val_string, style_prop_value);
            if (newString != binderItem.val_string)
            {
                binderItem.val_string = newString;
                binder.binders[childIndex] = binderItem;
                valuebinders[parentIndex] = binder;
            }
            #endregion

            currentY += 26;

            #region 值：Sprite
            EditorGUI.LabelField(new Rect(currentX + 25, currentY, labelWidth, 18), "Sprite:", style_prop_name);
            Sprite newSprite = (Sprite)EditorGUI.ObjectField(new Rect(currentX + 25 + labelWidth, currentY, fieldWidth, 18), binderItem.val_sprite, typeof(Sprite), false);
            if (newSprite != binderItem.val_sprite)
            {
                binderItem.val_sprite = newSprite;
                binder.binders[childIndex] = binderItem;
                valuebinders[parentIndex] = binder;
            }
            #endregion

            currentY += 26;

            #region 值：Texture            
            EditorGUI.LabelField(new Rect(currentX + 25, currentY, labelWidth, 18), "Texture:", style_prop_name);
            Texture2D newTexture = (Texture2D)EditorGUI.ObjectField(new Rect(currentX + 25 + labelWidth, currentY, fieldWidth, 18), binderItem.val_texture, typeof(Texture2D), false);
            if (newTexture != binderItem.val_texture)
            {
                binderItem.val_texture = newTexture;
                binder.binders[childIndex] = binderItem;
                valuebinders[parentIndex] = binder;
            }
            #endregion

            currentY += 26;

            #region 值：Color            
            EditorGUI.LabelField(new Rect(currentX + 25, currentY, labelWidth, 18), "Color:", style_prop_name);
            Color newColor = EditorGUI.ColorField(new Rect(currentX + 25 + labelWidth, currentY, fieldWidth, 18), binderItem.val_color);
            if (newColor != binderItem.val_color)
            {
                binderItem.val_color = newColor;
                binder.binders[childIndex] = binderItem;
                valuebinders[parentIndex] = binder;
            }
            #endregion
        }
        #endregion

        #region 数据操作
        /// <summary>
        /// 添加新的绑定器
        /// </summary>
        private void AddBinder_Element()
        {
            ElementBinder newBinder = new ElementBinder();
            newBinder.ID = $"xxxx";
            newBinder.IsExpanded = true;
            newBinder.binders = new PrimitiveBinder[0];
            valuebinders.Add(newBinder);

            // 展开新添加的项
            foldoutStates[valuebinders.Count - 1] = true;

            // 刷新列表
            InitReorderableList();
            Repaint();
        }
        /// <summary>
        /// 通过拖拽添加新的绑定器
        /// </summary>
        private ElementBinder AddBinder_Element(string id, string name)
        {
            ElementBinder newBinder = new ElementBinder();
            newBinder.ID = $"{id}";
            newBinder.Name = $"{name}";
            newBinder.IsExpanded = true;
            newBinder.binders = new PrimitiveBinder[0];
            valuebinders.Add(newBinder);

            // 展开新添加的项
            foldoutStates[valuebinders.Count - 1] = true;

            // 刷新列表
            InitReorderableList();
            Repaint();

            return newBinder;
        }
        /// <summary>
        /// 移除绑定器
        /// </summary>
        private void RemoveBinder(int index)
        {
            if (index >= 0 && index < valuebinders.Count)
            {
                Undo.RecordObject(this, "RemoveBinder");

                valuebinders.RemoveAt(index);

                // 清理折叠状态
                Dictionary<int, bool> newFoldoutStates = new Dictionary<int, bool>();
                foreach (var kvp in foldoutStates)
                {
                    if (kvp.Key < index)
                        newFoldoutStates[kvp.Key] = kvp.Value;
                    else if (kvp.Key > index)
                        newFoldoutStates[kvp.Key - 1] = kvp.Value;
                }
                foldoutStates = newFoldoutStates;

                if (selectedIndex == index)
                    selectedIndex = -1;
                else if (selectedIndex > index)
                    selectedIndex--;

                // 刷新列表
                InitReorderableList();
                Repaint();
            }
        }
        /// <summary>
        /// 添加绑定器子项
        /// </summary>
        private void AddBinder_Primitive(int parentIndex)
        {
            if (parentIndex >= valuebinders.Count)
                return;

            ElementBinder binder = valuebinders[parentIndex];

            // 创建新的子项
            PrimitiveBinder newItem = new PrimitiveBinder();
            newItem.ID = $"xxxx";
            newItem.val_string = "";
            newItem.val_sprite = null;
            newItem.val_texture = null;

            // 添加到数组
            List<PrimitiveBinder> list = new List<PrimitiveBinder>(binder.binders);
            list.Add(newItem);
            binder.binders = list.ToArray();
            valuebinders[parentIndex] = binder;

            needsRefresh = true; // 高度变化，需要刷新
            Repaint();
        }
        /// <summary>
        /// 移除绑定器子项
        /// </summary>
        private void RemoveBinderSubItem(int parentIndex, int childIndex, bool refresh = true)
        {
            if (parentIndex >= valuebinders.Count)
                return;

            ElementBinder binder = valuebinders[parentIndex];
            if (childIndex >= binder.binders.Length)
                return;

            Undo.RecordObject(this, "RemoveBinderSubItem");

            List<PrimitiveBinder> list = new List<PrimitiveBinder>(binder.binders);
            list.RemoveAt(childIndex);
            binder.binders = list.ToArray();
            valuebinders[parentIndex] = binder;

            needsRefresh = refresh; // 高度变化，需要刷新
            Repaint();
        }
        /// <summary>
        /// 清空所有绑定器
        /// </summary>
        private void ClearAllBinders()
        {
            if (valuebinders.Count == 0)
                return;

            valuebinders.Clear();
            foldoutStates.Clear();
            selectedIndex = -1;

            InitReorderableList();
            Repaint();
        }
        /// <summary>
        /// 保存到目标组件
        /// </summary>
        private void SaveToTarget()
        {
            if (targetBinder == null)
            {
                TryFindTargetBinder();

                if (targetBinder == null)
                {
                    EditorUtility.DisplayDialog("错误", "未找到目标XHud_LayoutSpawner_ValueBinder组件！请确保场景中有该组件。", "确定");
                    return;
                }
            }

            // 记录Undo操作
            Undo.RecordObject(targetBinder, "修改数据绑定器");

            // 深拷贝数据到目标组件
            targetBinder.valuebinders = new ElementBinder[valuebinders.Count];
            for (int i = 0; i < valuebinders.Count; i++)
            {
                ElementBinder e_binder = valuebinders[i];
                e_binder.IsExpanded = false;

                targetBinder.valuebinders[i] = CopyValueBinder(valuebinders[i]);
            }

            EditorUtility.SetDirty(targetBinder);

            // 触发编辑器重绘
            if (targetBinder.layoutspawner != null)
            {
                EditorUtility.SetDirty(targetBinder.layoutspawner);
            }

            EditorApplication.delayCall += () =>
            {
                Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 布局元素数据绑定器通知", "绑定关系保存成功", $"已成功保存 {valuebinders.Count} 个关系绑定！", "明白", 0);
            };
        }
        /// <summary>
        /// 刷新数据（从目标组件重新加载）
        /// </summary>
        private void RefreshFromTarget()
        {
            if (targetBinder != null)
            {
                EditorApplication.delayCall += () =>
                {
                    // 检查是否有未保存的更改
                    if (valuebinders.Count > 0)
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素数据绑定器通知", "刷新绑定数据", "确定要刷新数据绑定列表吗？刷新将丢失当前未保存的更改！", "刷新", "暂不", 0);
                        if (res == "暂不")
                            return;
                    }

                    SyncDataFromTarget();
                    foldoutStates.Clear();
                    selectedIndex = -1;

                    InitReorderableList();
                    Repaint();

                    Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素数据绑定器通知", "绑定数据刷新成功", "已从目标组件重新加载数据绑定器！", "明白", 0);
                };
            }
            else
            {
                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素数据绑定器通知", "绑定数据刷新失败", "未找到目标组件！", "明白", 0);
            }
        }
        #endregion

        #region GUI绘制
        private void OnGUI()
        {
            if (!isInitialized)
                return;

            so?.Update();

            // 绘制工具栏
            DrawToolbar();

            // 绘制列表区域 - 使用ReorderableList原生滚动
            DrawListArea();

            // 绘制底部状态栏
            DrawStatusBar();

            // 放入元素项
            DropElementItem();

            so?.ApplyModifiedProperties();
        }

        /// <summary>
        /// 拖入元素布局生成器元素项
        /// </summary>
        private void DropElementItem()
        {
            Rect rect_drop = new Rect(20, 20, position.width - 40, position.height - 40);

            if (Event.current.type == EventType.DragUpdated)
            {
                if (rect_drop.Contains(Event.current.mousePosition))
                {
                    if (!isDraggingOver)
                    {
                        //Debug.Log("拖入区域");
                        isDraggingOver = true;
                    }
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                }
                else
                {
                    if (isDraggingOver)
                    {
                        //Debug.Log("离开区域");
                        isDraggingOver = false;
                    }
                }
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                string data = ((string)DragAndDrop.GetGenericData("LayoutSpawnerBindData"));

                // 获取拖入的元素项
                XHud_LayoutSpawner_Item item = JsonUtility.FromJson<XHud_LayoutSpawner_Item>(data);

                Undo.RecordObject(this, "BindChanged");

                // 检查校验有效性
                for (int i = 0; i < valuebinders.Count; i++)
                {
                    var e_binder = valuebinders[i];

                    if (e_binder.ID != item.ID)
                        continue;

                    // 过滤掉无效的 binder
                    var validBinders = new List<PrimitiveBinder>();
                    foreach (var binder in e_binder.binders)
                    {
                        if (item.SpawnedElementNode.Element != null)
                        {
                            var con = item.SpawnedElementNode.Element.GetPrimitiveController_With_ID(binder.ID);
                            if (con != null)
                                validBinders.Add(binder);
                        }
                    }

                    e_binder.binders = validBinders.ToArray();
                }

                BindValue_Element(item);

                isDraggingOver = false;
                Repaint();
                Event.current.Use();
            }

            Rect rect_drop_visual = new Rect(0, 20, position.width, position.height - 20 - 25);
            Editor_XHud_GUI.Gui_Box(rect_drop_visual, isDraggingOver ? Color.black * 0.55f : Color.clear);

            if (isDraggingOver)
            {
                Rect rect_drop_visual_icon = new Rect(rect_drop_visual.x + rect_drop_visual.width / 2 - 25, rect_drop_visual.y + rect_drop_visual.height / 2 - 80, 50, 50);
                Editor_XHud_GUI.Gui_Icon(rect_drop_visual_icon, icon_import);

                Rect rect_drop_visual_text = new Rect(0, rect_drop_visual.y + rect_drop_visual.height / 2 - 30, rect_drop_visual.width, 50);
                Editor_XHud_GUI.Gui_Labelfield(rect_drop_visual_text, "添加到绑定列表", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, 16, Font_Bold);

                Rect rect_drop_visual_text_sub = new Rect(0, rect_drop_visual_text.y + rect_drop_visual_text.height / 2 + 10, rect_drop_visual_text.width, 50);
                Editor_XHud_GUI.Gui_Labelfield(rect_drop_visual_text_sub, "松开鼠标立即添加", HudFilled.无, HudColor.无, Color.white * 0.65f, TextAnchor.MiddleCenter, 12, Font_Light);
            }
        }
        /// <summary>
        /// 绘制工具栏
        /// </summary>
        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUIStyle toolbarStyle = new GUIStyle(EditorStyles.toolbarButton);
            toolbarStyle.font = Editor_XHud_GUI.GetFont("sx_regular");
            toolbarStyle.fontStyle = FontStyle.Normal;
            toolbarStyle.fontSize = 12;

            GUIContent content_savebind = new GUIContent("  保存绑定", icon_save_r);
            if (GUILayout.Button(content_savebind, toolbarStyle, GUILayout.Width(90)))
            {
                SaveToTarget();
            }
            GUIContent content_reload = new GUIContent("  刷新", icon_refresh_r);
            if (GUILayout.Button(content_reload, toolbarStyle, GUILayout.Width(60)))
            {
                RefreshFromTarget();
            }
            GUILayout.FlexibleSpace();

            GUIContent content_clear = new GUIContent("  清空全部", icon_clear_r);
            if (GUILayout.Button(content_clear, toolbarStyle, GUILayout.Width(80)))
            {
                EditorApplication.delayCall += () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素数据绑定器通知", "清空所有绑定数据", "确定要清空数据绑定列表吗？", "清空", "暂不", 0);
                    if (res == "暂不")
                        return;

                    ClearAllBinders();
                };
            }
            GUIContent content_add = new GUIContent("  添加一个绑定", icon_add_r);
            if (GUILayout.Button(content_add, toolbarStyle, GUILayout.Width(120)))
            {
                AddBinder_Element();
            }
            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 绘制列表区域 - 使用ReorderableList + 滚动区域
        /// </summary>
        private void DrawListArea()
        {
            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 10, "数据绑定器配置", XHud_Dashboard.Theme_Primary);

            Editor_XHud_GUI.Gui_Layout_Space(10);

            if (reorderableList != null)
            {
                if (valuebinders.Count == 0)
                {
                    // 显示空状态
                    Rect emptyRect = GUILayoutUtility.GetRect(0, 10);
                    emptyRect = new Rect(emptyRect.x + 10, emptyRect.y, emptyRect.width - 20, emptyRect.height);

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
                    Editor_XHud_GUI.Gui_Layout_Space(15);
                    string msg = $"请注意！暂无任何数据绑定器！请点击工具栏的<b><color={hexcol}> \"添加元素绑定\" </color></b>按钮添加新的<b><color={hexcol}>数据绑定器</color></b>。每个元素绑定器可以包含<b><color={hexcol}>多个图元绑定子项</color></b>，用于绑定元素下的多个图元控制器的数值";
                    Editor_XHud_GUI.Gui_Layout_TextArea_Wrap(msg, HudFilled.实体, HudColor.深空灰, Color.white * 0.8f, TextAnchor.UpperLeft, new RectOffset(10, 10, 10, 10), EditorGUIUtility.currentViewWidth, 11, Font_Dialog);
                    Editor_XHud_GUI.Gui_Layout_Space(15);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                    Editor_XHud_GUI.Gui_Layout_Space(15);
                }
                else
                {
                    // 计算列表所需的总高度
                    float listHeight = GetReorderableListTotalHeight();

                    // ⭐ 修改：滚动区域高度 = 窗口高度 - 预留空间（工具栏高度 + 状态栏高度 + 内边距）
                    // 工具栏约35px，状态栏约35px，上下内边距约30px，总计约100px
                    float reservedHeight = 105f;  // 可根据实际布局微调
                    float maxHeight = Mathf.Min(listHeight, position.height - reservedHeight);

                    // 确保最小高度
                    maxHeight = Mathf.Max(maxHeight, 150f);

                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(position.height - 120));
                    {
                        // 使用 ReorderableList 的原生布局方法
                        reorderableList.DoLayoutList();
                    }
                    EditorGUILayout.EndScrollView();
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
        }
        /// <summary>
        /// 计算 ReorderableList 的总高度
        /// </summary>
        private float GetReorderableListTotalHeight()
        {
            if (reorderableList == null || valuebinders == null || valuebinders.Count == 0)
                return 0f;

            float totalHeight = 0f;

            // 头部高度
            totalHeight += 22f;

            // 所有元素的高度
            for (int i = 0; i < valuebinders.Count; i++)
            {
                totalHeight += GetElementHeight(i);
            }

            // 底部留白
            totalHeight += 10f;

            return totalHeight;
        }
        /// <summary>
        /// 绘制底部状态栏
        /// </summary>
        private void DrawStatusBar()
        {
            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            // 目标组件信息
            if (targetBinder != null)
            {
                Editor_XHud_GUI.Gui_Layout_Labelfield($"目标组件: <color={hexcol}>  {targetBinder.gameObject.name}</color>", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, 12, Font_Light);
                //GUILayout.Label($"目标组件: <color={hexcol}>{targetBinder.gameObject.name}</color>", EditorStyles.boldLabel);
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_Labelfield($"目标组件: <color={hexcol}>未找到！</color>", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, 12, Font_Light);
            }

            GUILayout.Space(10);
            GUILayout.FlexibleSpace();

            // 统计信息
            int totalSubItems = 0;
            foreach (var binder in valuebinders)
            {
                if (binder.binders != null)
                    totalSubItems += binder.binders.Length;
            }
            Editor_XHud_GUI.Gui_Layout_Labelfield($"已绑定元素数据：<color={hexcol}>{valuebinders.Count}    </color>/    图元数据：<color={hexcol}>{totalSubItems}</color>", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleRight, 12, Font_Light);
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
        }
        #endregion

        #region 拖拽元素
        /// <summary>
        /// 将布局元素数据绑定到列表
        /// </summary>
        /// <param name="item"></param>
        public void BindValue_Element(XHud_LayoutSpawner_Item item)
        {
            XHud_Module_Element ele = item.SpawnedElementNode.Element;
            string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            // 用于覆盖原数据
            List<ElementBinder> BindList = new List<ElementBinder>();

            // 先检测是否已经绑定了目标元素项
            for (int i = 0; i < valuebinders.Count; i++)
            {
                ElementBinder ele_binder = valuebinders[i];
                BindList.Add(ele_binder);

                // 如果绑定了那么久完善未绑定的图元
                if (ele_binder.ID == item.ID)
                {
                    if (string.IsNullOrEmpty(ele_binder.Name) || ele_binder.Name != item.SpawnName)
                        ele_binder.Name = item.SpawnName;

                    if (ele != null)
                    {
                        ele_binder = BindValue_Primitives(ele_binder, ele);
                    }

                    ele_binder.IsExpanded = true;

                    valuebinders[i] = ele_binder;

                    // 关键：同步折叠状态字典
                    foldoutStates[i] = true;

                    // 刷新 UI
                    InitReorderableList();
                    Repaint();

                    // ⭐ 同步折叠状态字典
                    if (foldoutStates.ContainsKey(i))
                        foldoutStates[i] = true;
                    else
                        foldoutStates.Add(i, true);

                    if (ele_binder.binders.Length <= 0)
                    {
                        XGUI_Utilitys.Console("XHud - 布局元素数据绑定器通知", $"注意！ID为：<b><color={hex_col}> {item.ID} </b></color>的目标元素项已经绑定，<b><color={hex_col}>请勿重复绑定！</b></color>此操作已被跳过！", XGUIMsgState.通知);
                    }
                    else
                    {
                        XGUI_Utilitys.Console("XHud - 布局元素数据绑定器通知", $"注意！ID为：<b><color={hex_col}> {item.ID} </b></color>的目标元素项已经绑定，<b><color={hex_col}>因为您之前绑定过该元素ID</b></color>，但当时可能其下并没有任何图元！所以完善了对ID为：<b><color={hex_col}> {item.ID} </b></color>的元素其下的图元ID的绑定！</b></color>", XGUIMsgState.通知);
                    }

                    return;
                }
                else
                {
                    ele_binder.IsExpanded = false;

                    valuebinders[i] = ele_binder;
                }
            }

            // 能执行到此处则表示没有任何图元被绑定

            // 加入绑定列表
            ElementBinder x_ele_binder = AddBinder_Element(item.ID, item.SpawnName);

            // AddNewBinder 已经添加了项并设置了 foldoutStates，但需要确保展开
            int newIndex = valuebinders.Count - 1;
            if (foldoutStates.ContainsKey(newIndex))
                foldoutStates[newIndex] = true;
            else
                foldoutStates.Add(newIndex, true);

            // 如果元素未载入，只绑定元素项自身ID，后续不处理元素下的图元数据绑定，否则则跳过这一步判断
            if (item.SpawnedElementNode.Element == null)
            {
                XGUI_Utilitys.Console("XHud - 布局元素数据绑定器通知", $"注意！ID为：<b><color={hex_col}> {item.ID} </b></color>的目标元素项的元素并未加载！所以只能绑定元素项而不能绑定元素下的图元控制器的ID！", XGUIMsgState.警告);
                return;
            }

            if (ele)
            {
                // 处理元素下的图元数据绑定
                x_ele_binder = BindValue_Primitives(x_ele_binder, ele);
            }

            BindList.Add(x_ele_binder);
            valuebinders = BindList;
        }
        /// <summary>
        /// 绑定元素下所有图元数据
        /// </summary>
        /// <param name="ele_binder"></param>
        /// <param name="ele"></param>
        /// <returns></returns>
        private ElementBinder BindValue_Primitives(ElementBinder ele_binder, XHud_Module_Element ele)
        {
            if (ele.PrimitiveControllerNodes.Count <= 0)
                return ele_binder;

            #region 如果图元绑定器不为空，先储存现有的图元绑定器
            List<PrimitiveBinder> prim_binders = new List<PrimitiveBinder>();
            if (ele_binder.binders != null)
            {
                for (int i = 0; i < ele_binder.binders.Length; i++)
                {
                    prim_binders.Add(ele_binder.binders[i]);
                }
            }
            #endregion

            // 遍历元素下的所有图元控制器，如果在现有的绑定其中找不到则添加
            for (int i = 0; i < ele.PrimitiveControllerNodes.Count; i++)
            {
                XHud_Module_Primitive_Controller con = ele.PrimitiveControllerNodes[i].Controller;

                if (con == null)
                    continue;

                #region 判断是否已经存在？如果存在在判定是否名称已赋值？
                bool exist = false;
                for (int a = 0; a < prim_binders.Count; a++)
                {
                    if (prim_binders[a].ID == con.ID)
                    {
                        PrimitiveBinder binder = prim_binders[a];

                        exist = true;
                        // 如果图元绑定关系已存在，则根据情况更新绑定名称
                        if (prim_binders[a].Name != con.transform.name)
                        {
                            binder.Name = con.transform.name;
                        }

                        if (binder.type == ModuleType.Image)
                        {
                            if (con.pt_Painting != null &&
                                con.pt_Painting.SyncImageColor &&
                                !con.pt_Painting.SyncLibraryColor)
                            {
                                binder.val_color = con.pt_Painting.OriginalColor;
                            }
                            else
                            {
                                binder.val_color = con.mod_Image.color;
                            }
                        }
                        if (binder.type == ModuleType.RawImage)
                        {
                            if (con.pt_Painting != null &&
                                con.pt_Painting.SyncImageColor &&
                                !con.pt_Painting.SyncLibraryColor)
                            {
                                binder.val_color = con.pt_Painting.OriginalColor;
                            }
                            else
                            {
                                binder.val_color = con.mod_RawImage.color;
                            }
                        }
                        if (binder.type == ModuleType.Text)
                        {
                            if (con.pt_Painting != null &&
                                con.pt_Painting.SyncImageColor &&
                                !con.pt_Painting.SyncLibraryColor)
                            {
                                if (con.mod_Text.TextStyleInfo.SyncPrimitivePaintingColor)
                                    binder.val_color = con.pt_Painting.OriginalColor;
                            }
                            else
                                binder.val_color = con.mod_Text.TextStyleInfo.FontColor;
                        }
                        if (binder.type == ModuleType.TmpText)
                        {
                            if (con.pt_Painting != null &&
                                con.pt_Painting.SyncImageColor &&
                                !con.pt_Painting.SyncLibraryColor)
                            {
                                if (con.mod_TmpText.TextStyleInfo.SyncPrimitivePaintingColor)
                                    binder.val_color = con.pt_Painting.OriginalColor;
                            }
                            else
                                binder.val_color = con.mod_TmpText.TextStyleInfo.tmp_color;
                        }

                        prim_binders[a] = binder;

                        break;
                    }
                }
                #endregion

                // 如果不存在则加入列表，准备覆盖回原图元绑定列表
                if (!exist)
                {
                    PrimitiveBinder p_binder = new PrimitiveBinder();
                    p_binder.Name = con.transform.name;
                    p_binder.ID = con.ID;
                    p_binder.type = con.GetModuleType();

                    if (p_binder.type == ModuleType.Image)
                    {
                        if (con.pt_Painting != null &&
                            con.pt_Painting.SyncImageColor &&
                            !con.pt_Painting.SyncLibraryColor)
                        {
                            p_binder.val_color = con.pt_Painting.OriginalColor;
                        }
                        else
                        {
                            p_binder.val_color = con.mod_Image.color;
                        }
                    }
                    if (p_binder.type == ModuleType.RawImage)
                    {
                        if (con.pt_Painting != null &&
                            con.pt_Painting.SyncImageColor &&
                            !con.pt_Painting.SyncLibraryColor)
                        {
                            p_binder.val_color = con.pt_Painting.OriginalColor;
                        }
                        else
                        {
                            p_binder.val_color = con.mod_RawImage.color;
                        }
                    }
                    if (p_binder.type == ModuleType.Text)
                    {
                        if (con.pt_Painting != null &&
                            con.pt_Painting.SyncImageColor &&
                            !con.pt_Painting.SyncLibraryColor)
                        {
                            if (con.mod_Text.TextStyleInfo.SyncPrimitivePaintingColor)
                                p_binder.val_color = con.pt_Painting.OriginalColor;
                        }
                        else
                            p_binder.val_color = con.mod_Text.TextStyleInfo.FontColor;
                    }
                    if (p_binder.type == ModuleType.TmpText)
                    {
                        if (con.pt_Painting != null &&
                            con.pt_Painting.SyncImageColor &&
                            !con.pt_Painting.SyncLibraryColor)
                        {
                            if (con.mod_TmpText.TextStyleInfo.SyncPrimitivePaintingColor)
                                p_binder.val_color = con.pt_Painting.OriginalColor;
                        }
                        else
                            p_binder.val_color = con.mod_TmpText.TextStyleInfo.tmp_color;
                    }

                    prim_binders.Add(p_binder);
                }
            }
            ele_binder.binders = prim_binders.ToArray();

            return ele_binder;
        }
        #endregion             
    }
}