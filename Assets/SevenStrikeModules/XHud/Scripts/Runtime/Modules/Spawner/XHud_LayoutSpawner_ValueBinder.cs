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
namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using System;
    using UnityEngine;

    [Serializable]
    public struct ElementBinder
    {
        [SerializeField] public string Name;
        [SerializeField] public string ID;
        [SerializeField] public bool IsExpanded;
        [SerializeField] public bool IsEnabled;
        public PrimitiveBinder[] binders;
    }

    [Serializable]
    public struct PrimitiveBinder
    {
        [SerializeField] public string Name;
        [SerializeField] public string ID;
        [SerializeField] public string val_string;
        [SerializeField] public Sprite val_sprite;
        [SerializeField] public Texture2D val_texture;
        [SerializeField] public Color val_color;
        [SerializeField] public ModuleType type;
    }

    public class XHud_LayoutSpawner_ValueBinder : MonoBehaviour
    {
        [SerializeField] public XHud_LayoutSpawner layoutspawner;
        [SerializeField] public ElementBinder[] valuebinders;
        [SerializeField] public bool UseDebug;

        void OnEnable()
        {
            if (layoutspawner == null)
                layoutspawner = GetComponent<XHud_LayoutSpawner>();

            Action_Register();
        }

        void OnDisable()
        {
            Action_Unregister();
        }

        /// <summary>
        /// 注册布局生成器的委托动作
        /// </summary>
        private void Action_Register()
        {
            layoutspawner.act_on_spawn_with_id += on_spawn_with_id;
            layoutspawner.act_on_despawn_with_id += on_despawn_with_id;
        }
        /// <summary>
        /// 注销布局生成器的委托动作
        /// </summary>
        private void Action_Unregister()
        {
            layoutspawner.act_on_spawn_with_id -= on_spawn_with_id;
            layoutspawner.act_on_despawn_with_id -= on_despawn_with_id;
        }

        public virtual void on_spawn_with_id(XHudSpace space, XHudElementNode elementnode, string ID)
        {
            for (int i = 0; i < valuebinders.Length; i++)
            {
                ElementBinder valuebinder = valuebinders[i];

                // 如果绑定器的目标ID和当前生成的元素项ID匹配
                if (ID == valuebinder.ID)
                {
                    if (!valuebinder.IsEnabled)
                        continue;

                    // 遍历绑定器中的图元控制器绑定器列表来和元素下的所有图元控制器匹配ID
                    for (int w = 0; w < valuebinder.binders.Length; w++)
                    {
                        PrimitiveBinder binder = valuebinder.binders[w];
                        ModifiyValue(elementnode.Element, binder);
                    }
                }
            }

            if (UseDebug)
            {
                string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

                XGUI_Utilitys.Console("XHud - 布局元素数据绑定器消息", $"已生成元素： \n目标空间：<b><color={hex_col}> {space.ToString()} </color></b>\n名称：<b><color={hex_col}> {elementnode.Element.name} </color></b>\n生成项ID：<b><color={hex_col}> {ID} </color></b>\n", XGUIMsgState.通知);
            }
        }

        public virtual void on_despawn_with_id(XHudSpace space, XHud_Module_Element element, string ID)
        {
            if (UseDebug)
            {
                string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

                XGUI_Utilitys.Console("XHud - 布局元素数据绑定器消息", $"已回收元素： \n目标空间：<b><color={hex_col}> {space.ToString()} </color></b>\n名称：<b><color={hex_col}> {element.name} </color></b>\n生成项ID：<b><color={hex_col}> {ID} </color></b>\n", XGUIMsgState.通知);
            }
        }

        void Update()
        {

        }

        public void ModifiyValue(XHud_Module_Element element, PrimitiveBinder binder)
        {
            if (element == null)
                return;
            XHud_Module_Primitive_Controller con = element.GetPrimitiveController_With_ID(binder.ID);
            if (con == null)
                return;
            // 如果图元控制器的ID和图元绑定器ID匹配，则根据图元控制器的类型修改内容
            if (binder.ID == con.ID)
            {
                ModuleType type = con.GetModuleType();
                switch (type)
                {
                    case ModuleType.RectTransform:
                        break;
                    case ModuleType.Text:
                        if (!string.IsNullOrEmpty(binder.val_string))
                            con.mod_Text.txt_Set_Content(binder.val_string);
                        // 当图元配色接管 / 当图元配色不同步色卡库 / 当文字颜色被接管
                        if (con.pt_Painting != null &&
                            con.pt_Painting.SyncImageColor &&
                            !con.pt_Painting.SyncLibraryColor &&
                            con.mod_Text.TextStyleInfo.SyncPrimitivePaintingColor)
                        {
                            con.pt_Painting.OriginalColor = binder.val_color;
                        }
                        else
                        {
                            con.mod_Text.TextStyleInfo.FontColor = binder.val_color;
                        }
                        break;
                    case ModuleType.TmpText:
                        if (!string.IsNullOrEmpty(binder.val_string))
                            con.mod_TmpText.tmp_Set_Content(binder.val_string);
                        // 当图元配色接管 / 当图元配色不同步色卡库 / 当文字颜色被接管
                        if (con.pt_Painting != null &&
                          con.pt_Painting.SyncImageColor &&
                          !con.pt_Painting.SyncLibraryColor &&
                          con.mod_TmpText.TextStyleInfo.SyncPrimitivePaintingColor)
                        {
                            con.pt_Painting.OriginalColor = binder.val_color;
                        }
                        else
                        {
                            con.mod_TmpText.TextStyleInfo.tmp_color = binder.val_color;
                        }
                        break;
                    case ModuleType.Image:
                        if (binder.val_string != null)
                            con.mod_Image.sprite = binder.val_sprite;
                        // 当图元配色接管 / 当图元配色不同步色卡库
                        if (con.pt_Painting != null &&
                          con.pt_Painting.SyncImageColor &&
                          !con.pt_Painting.SyncLibraryColor)
                        {
                            con.pt_Painting.OriginalColor = binder.val_color;
                        }
                        else
                        {
                            con.mod_Image.color = binder.val_color;
                        }
                        break;
                    case ModuleType.RawImage:
                        if (binder.val_texture != null)
                            con.mod_RawImage.texture = binder.val_texture;
                        // 当图元配色接管 / 当图元配色不同步色卡库
                        if (con.pt_Painting != null &&
                            con.pt_Painting.SyncImageColor &&
                            !con.pt_Painting.SyncLibraryColor)
                        {
                            con.pt_Painting.OriginalColor = binder.val_color;
                        }
                        else
                        {
                            con.mod_RawImage.color = binder.val_color;
                        }
                        break;
                }
            }
        }
    }
}