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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using Random = UnityEngine.Random;

    /// <summary>
    /// 数据源类型枚举
    /// </summary>
    public enum ContainerType
    {
        None = 0,
        Text = 1,
        TmpText = 2,
        Image = 3,
        RawImage = 4
    }

    [System.Serializable]
    /// <summary>
    /// 数据源类型 - Text
    /// </summary>
    public class XHud_Container_Text
    {
        /// <summary>
        /// 文字
        /// </summary>
        public XHud_Module_Text Base;
        public void SetValue(string str)
        {
            if (Base == null)
                return;
            Base.txt_Set_Content(str);
        }
    }

    [System.Serializable]
    /// <summary>
    /// 数据源类型 - TmpText
    /// </summary>
    public class XHud_Container_TmpText
    {
        /// <summary>
        /// TMP文字
        /// </summary>
        public XHud_Module_TmpText Base;
        public void SetValue(string str)
        {
            if (Base == null)
                return;
            Base.tmp_Set_Content(str);
        }
    }

    [System.Serializable]
    /// <summary>
    /// 数据源类型 - Image组件
    /// </summary>
    public class XHud_Container_Image
    {
        /// <summary>
        /// 图形
        /// </summary>
        public Image Base;

        /// <summary>
        /// 设置图元
        /// </summary>
        /// <param tweenName="spr"></param>
        public void SetValue(Sprite spr)
        {
            if (Base == null)
                return;
            Base.sprite = spr;
        }

        /// <summary>
        /// 设置图元
        /// </summary>
        /// <param tweenName="tex"></param>
        public void SetValue(Texture2D tex)
        {
            if (Base == null)
                return;
            Base.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// 设置图元
        /// </summary>
        /// <param tweenName="tex"></param>
        /// <param tweenName="pivot"></param>
        public void SetValue(Texture2D tex, Vector2 pivot)
        {
            if (Base == null)
                return;
            Base.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot);
        }
    }

    [System.Serializable]
    /// <summary>
    /// 数据源类型 - RawImage
    /// </summary>
    public class XHud_Container_RawImage
    {
        /// <summary>
        /// Raw图形
        /// </summary>
        public RawImage Base;

        /// <summary>
        /// 设置图元
        /// </summary>
        /// <param tweenName="tex"></param>
        public void SetValue(Texture2D tex)
        {
            if (Base == null)
                return;
            Base.texture = tex;
        }
    }

    [System.Serializable]
    /// <summary>
    /// 数据源项
    /// </summary>
    public class XHud_ContainerItem
    {
        /// <summary>
        /// 类型
        /// </summary>
        public ContainerType Type;
        /// <summary>
        /// ID
        /// </summary>
        public int ID;
        /// <summary>
        /// 标识
        /// </summary>
        public string Indicator;
        /// <summary>
        /// 文字
        /// </summary>
        public XHud_Container_Text Text;
        /// <summary>
        /// TMP文字
        /// </summary>
        public XHud_Container_TmpText TmpText;
        /// <summary>
        /// 图形
        /// </summary>
        public XHud_Container_Image Image;
        /// <summary>
        /// Raw图形
        /// </summary>
        public XHud_Container_RawImage RawImage;
        /// <summary>
        /// 动画器播放延迟
        /// </summary>
        public float DelayTime;
        /// <summary>
        /// 动画器
        /// </summary>
        public XHud_Module_Animator Animator;
        public RectTransform Transform;

        /// <summary>
        /// 匹配容器项类型
        /// </summary>
        /// <typeparam tweenName="TArg">请根据：HudContainer_Text / HudContainer_TmpText / HudContainer_Image / HudContainer_RawImage / HudContainer_RectTransform 5项类型进行匹配</typeparam>
        /// <returns></returns>
        public T GetType<T>() where T : class
        {
            if (Text.Base != null)
                return Text as T;
            else if (TmpText.Base != null)
                return TmpText as T;
            else if (Image.Base != null)
                return Image as T;
            else if (RawImage.Base != null)
                return RawImage as T;
            else
            {
                return null;
            }
        }

        #region Text & TmpText
        /// <summary>
        /// 匹配容器项类型
        /// </summary>
        /// <typeparam tweenName="TArg">请根据：HudContainer_Text / HudContainer_TmpText 2项类型进行匹配</typeparam>
        /// <param tweenName="val">文字内容</param>
        /// <returns></returns>
        public T Set_Texts<T>(string val) where T : class
        {
            if (Text.Base != null)
            {
                Text.Base.txt_Set_Content(val);
                return Text as T;
            }
            else if (TmpText.Base != null)
            {
                TmpText.Base.tmp_Set_Content(val);
                return TmpText as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 匹配容器项类型
        /// </summary>
        /// <typeparam tweenName="TArg">请根据：HudContainer_Text / HudContainer_TmpText 2项类型进行匹配</typeparam>
        /// <param tweenName="val">文字内容</param>
        /// <param tweenName="col">颜色_Color</param>
        /// <returns></returns>
        public T Set_Texts<T>(string val, Color col) where T : class
        {
            if (Text.Base != null)
            {
                Text.Base.txt_Set_Content(val);
                Text.Base.TextStyleInfo.txt_Set_FontColor(col);
                return Text as T;
            }
            else if (TmpText.Base != null)
            {
                TmpText.Base.tmp_Set_Content(val);
                TmpText.Base.TextStyleInfo.tmp_Set_FontColor(col);
                return TmpText as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 匹配容器项类型
        /// </summary>
        /// <typeparam tweenName="TArg">请根据：HudContainer_Text / HudContainer_TmpText 2项类型进行匹配</typeparam>
        /// <param tweenName="val">文字内容</param>
        /// <param tweenName="anchor">文字锚点</param>
        /// <returns></returns>
        public T Set_Texts<T>(string val, ContentAnchor anchor) where T : class
        {
            if (Text.Base != null)
            {
                Text.Base.txt_Set_Content(val);
                Text.Base.TextStyleInfo.txt_Set_Alignment(anchor);
                return Text as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 匹配容器项类型
        /// </summary>
        /// <typeparam tweenName="TArg">请根据：HudContainer_Text / HudContainer_TmpText 2项类型进行匹配</typeparam>
        /// <param tweenName="val">文字内容</param>
        /// <param tweenName="col">颜色_Color</param>
        /// <param tweenName="anchor">文字锚点</param>
        /// <returns></returns>
        public T Set_Texts<T>(string val, Color col, ContentAnchor anchor) where T : class
        {
            if (Text.Base != null)
            {
                Text.Base.txt_Set_Content(val);
                Text.Base.TextStyleInfo.txt_Set_Alignment(anchor);
                Text.Base.TextStyleInfo.txt_Set_FontColor(col);
                return Text as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 匹配容器项类型
        /// </summary>
        /// <typeparam tweenName="TArg">请根据：HudContainer_Text / HudContainer_TmpText 2项类型进行匹配</typeparam>
        /// <param tweenName="val">文字内容</param>
        /// <param tweenName="anchor">文字锚点</param>
        /// <returns></returns>
        public T Set_Texts<T>(string val, TmpContentAnchor anchor) where T : class
        {
            if (TmpText.Base != null)
            {
                TmpText.Base.tmp_Set_Content(val);
                TmpText.Base.TextStyleInfo.tmp_Set_Alignment(anchor);
                return TmpText as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 匹配容器项类型
        /// </summary>
        /// <typeparam tweenName="TArg">请根据：HudContainer_Text / HudContainer_TmpText 2项类型进行匹配</typeparam>
        /// <param tweenName="val">文字内容</param>
        /// <param tweenName="col">颜色_Color</param>
        /// <param tweenName="anchor">文字锚点</param>
        /// <returns></returns>
        public T Set_Texts<T>(string val, Color col, TmpContentAnchor anchor) where T : class
        {
            if (TmpText.Base != null)
            {
                TmpText.Base.tmp_Set_Content(val);
                TmpText.Base.TextStyleInfo.tmp_Set_Alignment(anchor);
                TmpText.Base.TextStyleInfo.tmp_Set_FontColor(col);
                return TmpText as T;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Image
        /// <summary>
        /// 匹配容器项类型 - 预赋值Image
        /// </summary>
        /// <typeparam tweenName="TArg">请根据：HudContainer_Image 类型进行匹配</typeparam>
        /// <returns></returns>
        public T Set_Image<T>(Sprite val) where T : class
        {
            if (Image.Base != null)
            {
                Image.Base.sprite = val;
                return Image as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 匹配容器项类型 - 预赋值Image
        /// </summary>
        /// <typeparam tweenName="TArg">请根据：HudContainer_Image 类型进行匹配</typeparam>
        /// <returns></returns>
        public T Set_Image<T>(Sprite val, Vector2 pivot) where T : class
        {
            if (Image.Base != null)
            {
                Image.Base.sprite = val;
                Image.Base.rectTransform.pivot = pivot;
                return Image as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 匹配容器项类型 - 预赋值Image
        /// </summary>
        /// <typeparam tweenName="TArg">请根据：HudContainer_Image 类型进行匹配</typeparam>
        /// <returns></returns>
        public T Set_Image<T>(Texture2D val) where T : class
        {
            if (Image.Base != null)
            {
                Image.Base.sprite = Sprite.Create(val, new Rect(0, 0, val.width, val.height), new Vector2(0.5f, 0.5f));
                return Image as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 匹配容器项类型 - 预赋值Image
        /// </summary>
        /// <typeparam tweenName="TArg">请根据：HudContainer_Image 类型进行匹配</typeparam>
        /// <returns></returns>
        public T Set_Image<T>(Texture2D val, Vector2 pivot) where T : class
        {
            if (Image.Base != null)
            {
                Image.Base.sprite = Sprite.Create(val, new Rect(0, 0, val.width, val.height), pivot);
                return Image as T;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region RawImage
        /// <summary>
        /// 匹配容器项类型 - 预赋值RawImage
        /// </summary>
        /// <typeparam tweenName="TArg">请根据：HudContainer_RawImage 类型进行匹配</typeparam>
        /// <returns></returns>
        public T SetRawImage<T>(Texture2D val) where T : class
        {
            if (RawImage.Base != null)
            {
                RawImage.Base.texture = val;
                return RawImage as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 匹配容器项类型 - 预赋值RawImage
        /// </summary>
        /// <typeparam tweenName="TArg">请根据：HudContainer_RawImage 类型进行匹配</typeparam>
        /// <returns></returns>
        public T SetRawImage<T>(Texture2D val, Vector2 pivot) where T : class
        {
            if (RawImage.Base != null)
            {
                RawImage.Base.texture = val;
                RawImage.Base.rectTransform.pivot = pivot;
                return RawImage as T;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }

    public class XHud_Module_Container : MonoBehaviour
    {
        public string Indicator;
        public bool UseDebug;
        public List<XHud_ContainerItem> ContainerItems = new List<XHud_ContainerItem>();
        public float Animators_GlobalDuration = 1f;
        public float Animators_MaxDuration;
        public XHudElementAnimateState AnimateState = XHudElementAnimateState.Static;
        public bool IsEnabled;
        public bool EventIsFold;
        public bool AutoStopPreview = true;

        #region UnityAction
        /// <summary>
        /// 动作 - 获取所有容器总数
        /// </summary>
        public UnityAction<int> act_on_itemcount_get;
        /// <summary>
        /// 动作 - 获取一个容器项
        /// </summary>
        public UnityAction<XHud_ContainerItem> act_on_item_get;
        /// <summary>
        /// 动作 - 获取所有容器项
        /// </summary>
        public UnityAction<XHud_ContainerItem[]> act_on_items_get;
        /// <summary>
        /// 动作 - 放入一个容器项
        /// </summary>
        public UnityAction<XHud_ContainerItem> act_on_item_add;
        /// <summary>
        /// 动作 - 移除一个容器项
        /// </summary>
        public UnityAction<string, int> act_on_item_remove;
        /// <summary>
        /// 动作 - 移除所有容器项
        /// </summary>
        public UnityAction act_on_items_remove;
        /// <summary>
        /// 动作 - 容器动画播放（整体）
        /// </summary>
        public UnityAction<XHud_ContainerItem[]> act_on_animate_play_all;
        /// <summary>
        /// 动作 - 容器动画播放（每项）
        /// </summary>
        public UnityAction<XHud_ContainerItem, XHud_Module_Animator> act_on_animate_play_by_item;
        /// <summary>
        /// 动作 - 容器动画复位（整体）
        /// </summary>
        public UnityAction<XHud_ContainerItem[]> act_on_animate_rewind_all;
        /// <summary>
        /// 动作 - 容器动画复位（每项）
        /// </summary>
        public UnityAction<XHud_ContainerItem, XHud_Module_Animator> act_on_animate_rewind_by_item;
        /// <summary>
        /// 动作 - 容器动画就绪（整体）
        /// </summary>
        public UnityAction<XHud_ContainerItem[]> act_on_animate_ready_all;
        /// <summary>
        /// 动作 - 容器动画就绪（每项）
        /// </summary>
        public UnityAction<XHud_ContainerItem, XHud_Module_Animator> act_on_animate_ready_by_item;
        /// <summary>
        /// 动作 - 修改容器项数值
        /// </summary>
        public UnityAction<string, XHud_ContainerItem> act_on_item_changevalue;
        #endregion

        #region UnityEvent        
        /// <summary>
        /// 事件 - 获取所有容器总数
        /// </summary>
        public UnityEvent<int> eve_on_itemcount_get;
        /// <summary>
        /// 事件 - 获取一个容器项
        /// </summary>
        public UnityEvent<XHud_ContainerItem> eve_on_item_get;
        /// <summary>
        /// 事件 - 获取所有容器项
        /// </summary>
        public UnityEvent<XHud_ContainerItem[]> eve_on_items_get;
        /// <summary>
        /// 事件 - 放入一个容器项
        /// </summary>
        public UnityEvent<XHud_ContainerItem> eve_on_item_add;
        /// <summary>
        /// 事件 - 移除一个容器项
        /// </summary>
        public UnityEvent<string, int> eve_on_item_remove;
        /// <summary>
        /// 事件 - 移除所有容器项
        /// </summary>
        public UnityEvent eve_on_items_remove;
        /// <summary>
        /// 事件 - 容器动画播放（整体）
        /// </summary>
        public UnityEvent<XHud_ContainerItem[]> eve_on_animate_play_all;
        /// <summary>
        /// 事件 - 容器动画播放（每项）
        /// </summary>
        public UnityEvent<XHud_ContainerItem, XHud_Module_Animator> eve_on_animate_play_by_item;
        /// <summary>
        /// 事件 - 容器动画复位（整体）
        /// </summary>
        public UnityEvent<XHud_ContainerItem[]> eve_on_animate_rewind_all;
        /// <summary>
        /// 事件 - 容器动画复位（每项）
        /// </summary>
        public UnityEvent<XHud_ContainerItem, XHud_Module_Animator> eve_on_animate_rewind_by_item;
        /// <summary>
        /// 事件 - 容器动画就绪（整体）
        /// </summary>
        public UnityEvent<XHud_ContainerItem[]> eve_on_animate_ready_all;
        /// <summary>
        /// 事件 - 容器动画就绪（每项）
        /// </summary>
        public UnityEvent<XHud_ContainerItem, XHud_Module_Animator> eve_on_animate_ready_by_item;
        /// <summary>
        /// 事件 - 修改容器项数值
        /// </summary>
        public UnityEvent<string, XHud_ContainerItem> eve_on_item_changevalue;
        #endregion

        public string AnimatorPlayTiming = "无";

        private void Start()
        {

        }

        private void Update()
        {

        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        #region 获取

        /// <summary>
        /// 获取数据项
        /// </summary>
        /// <param tweenName="name"></param>
        /// <returns></returns>
        public XHud_ContainerItem Con_Get_Item(string name)
        {
            XHud_ContainerItem dat = new XHud_ContainerItem();

            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].Indicator == name)
                {
                    dat = ContainerItems[i];
                    if (act_on_item_get != null)
                        act_on_item_get(dat);
                    eve_on_item_get.Invoke(dat);
                }
            }
            return dat;
        }

        /// <summary>
        /// 获取数据项
        /// </summary>
        /// <param tweenName="id"></param>
        /// <returns></returns>
        public XHud_ContainerItem Con_Get_Item(int id)
        {
            XHud_ContainerItem dat = new XHud_ContainerItem();

            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].ID == id)
                {
                    dat = ContainerItems[i];
                    if (act_on_item_get != null)
                        act_on_item_get(dat);
                    eve_on_item_get.Invoke(dat);
                }
            }
            return dat;
        }

        /// <summary>
        /// 获取数据项
        /// </summary>
        /// <param tweenName="type"></param>
        /// <returns></returns>
        public XHud_ContainerItem Con_Get_Item(ContainerType type)
        {
            XHud_ContainerItem dat = new XHud_ContainerItem();

            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].Type == type)
                {
                    dat = ContainerItems[i];
                    if (act_on_item_get != null)
                        act_on_item_get(dat);
                    eve_on_item_get.Invoke(dat);
                }
            }
            return dat;
        }

        /// <summary>
        /// 获取指定类型的所有数据项
        /// </summary>
        /// <param tweenName="type"></param>
        /// <returns>返回指定类型的所有数据项</returns>
        public XHud_ContainerItem[] Con_Get_Items(ContainerType type)
        {
            List<XHud_ContainerItem> dats = new List<XHud_ContainerItem>();

            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].Type == type)
                {
                    dats.Add(ContainerItems[i]);
                }
            }

            if (act_on_items_get != null)
                act_on_items_get(dats.ToArray());
            eve_on_items_get.Invoke(dats.ToArray());
            return dats.ToArray();
        }

        /// <summary>
        /// 获取所有数据项
        /// </summary>
        /// <returns>返回所有数据项</returns>
        public XHud_ContainerItem[] Con_Get_Items()
        {
            if (act_on_items_get != null)
                act_on_items_get(ContainerItems.ToArray());
            eve_on_items_get.Invoke(ContainerItems.ToArray());
            return ContainerItems.ToArray();
        }

        /// <summary>
        /// 获取数据项总数
        /// </summary>
        /// <returns></returns>
        public int Con_Get_ItemsCount()
        {
            if (act_on_itemcount_get != null)
                act_on_itemcount_get(ContainerItems.Count);
            eve_on_itemcount_get.Invoke(ContainerItems.Count);
            return ContainerItems.Count;
        }

        #endregion

        #region 辅助

        /// <summary>
        /// 创建ID编号
        /// </summary>
        /// <returns></returns>
        public int Dv_CreateID()
        {
            List<int> ids = new List<int>();
            for (int i = 0; i < ContainerItems.Count; i++)
            {
                ids.Add(ContainerItems[i].ID);
            }

            int ran_id = Random.Range(1111, 9999);

            while (true)
            {
                if (ids.Contains(ran_id))
                {
                    ran_id = Random.Range(1111, 9999);
                }
                else
                {
                    return ran_id;
                }
            }
        }

        #endregion

        #region 创建项

        /// <summary>
        /// 创建容器项 - 容器类参数
        /// </summary>
        /// <param tweenName="item"></param>
        /// <returns></returns>
        public XHud_ContainerItem Con_Add_Item(XHud_ContainerItem item)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法新增容器项！", HudMsgState.警告);
                return null;
            }
            ///--------随机ID
            item.ID = Dv_CreateID();

            ///-------- 判断类型
            if (item.Text.Base != null)
            {
                item.Type = ContainerType.Text;
            }
            else if (item.TmpText.Base != null)
            {
                item.Type = ContainerType.TmpText;
            }
            else if (item.Image.Base != null)
            {
                item.Type = ContainerType.Image;
            }
            else if (item.RawImage.Base != null)
            {
                item.Type = ContainerType.RawImage;
            }
            else
            {
                item.Type = ContainerType.None;
            }

            ContainerItems.Add(item);

            if (act_on_item_add != null)
                act_on_item_add(item);
            eve_on_item_add.Invoke(item);

            return item;
        }

        /// <summary>
        /// 创建容器项 - Hud文字参数
        /// </summary>
        /// <param tweenName="text">Hud文字文字模块</param>
        /// <param tweenName="Indicator">标识名称</param>
        /// <returns></returns>
        public XHud_ContainerItem Con_Add_Item(XHud_Module_Text text, string Indicator)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法新增容器项！", HudMsgState.警告);
                return null;
            }
            XHud_ContainerItem item = new XHud_ContainerItem();
            XHud_Container_Text x_text = new XHud_Container_Text();
            x_text.Base = text;

            XHud_Module_Animator anim = text.GetComponent<XHud_Module_Animator>();
            if (anim != null)
                item.Animator = anim;

            item.Text = x_text;

            item.Indicator = Indicator;
            item.ID = Dv_CreateID();
            item.Type = ContainerType.Text;

            ContainerItems.Add(item);

            if (act_on_item_add != null)
                act_on_item_add(item);
            eve_on_item_add.Invoke(item);

            return item;
        }

        /// <summary>
        /// 创建容器项 - HudTmp文字参数
        /// </summary>
        /// <param tweenName="tmptext">HudTmp文字模块</param>
        /// <param tweenName="Indicator">标识名称</param>
        /// <returns></returns>
        public XHud_ContainerItem Con_Add_Item(XHud_Module_TmpText tmptext, string Indicator)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法新增容器项！", HudMsgState.警告);
                return null;
            }
            XHud_ContainerItem item = new XHud_ContainerItem();
            XHud_Container_TmpText x_text = new XHud_Container_TmpText();
            x_text.Base = tmptext;

            XHud_Module_Animator anim = tmptext.GetComponent<XHud_Module_Animator>();
            if (anim != null)
                item.Animator = anim;

            item.TmpText = x_text;

            item.Indicator = Indicator;
            item.ID = Dv_CreateID();
            item.Type = ContainerType.TmpText;

            ContainerItems.Add(item);

            if (act_on_item_add != null)
                act_on_item_add(item);
            eve_on_item_add.Invoke(item);

            return item;
        }

        /// <summary>
        /// 创建容器项 - Image参数
        /// </summary>
        /// <param tweenName="img">Image图像模块</param>
        /// <param tweenName="Indicator">标识名称</param>
        /// <returns></returns>
        public XHud_ContainerItem Con_Add_Item(Image img, string Indicator)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法新增容器项！", HudMsgState.警告);
                return null;
            }
            XHud_ContainerItem item = new XHud_ContainerItem();
            XHud_Container_Image x_img = new XHud_Container_Image();
            x_img.Base = img;

            XHud_Module_Animator anim = img.GetComponent<XHud_Module_Animator>();
            if (anim != null)
                item.Animator = anim;

            item.Image = x_img;

            item.Indicator = Indicator;
            item.ID = Dv_CreateID();
            item.Type = ContainerType.Image;

            ContainerItems.Add(item);

            if (act_on_item_add != null)
                act_on_item_add(item);
            eve_on_item_add.Invoke(item);
            return item;
        }

        /// <summary>
        /// 创建容器项 - RawImage参数
        /// </summary>
        /// <param tweenName="img">RawImage图像模块</param>
        /// <param tweenName="Indicator">标识名称</param>
        /// <returns></returns>
        public XHud_ContainerItem Con_Add_Item(RawImage img, string Indicator)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法新增容器项！", HudMsgState.警告);
                return null;
            }
            XHud_ContainerItem item = new XHud_ContainerItem();
            XHud_Container_RawImage x_img = new XHud_Container_RawImage();
            x_img.Base = img;

            XHud_Module_Animator anim = img.GetComponent<XHud_Module_Animator>();
            if (anim != null)
                item.Animator = anim;

            item.RawImage = x_img;

            item.Indicator = Indicator;
            item.ID = Dv_CreateID();
            item.Type = ContainerType.RawImage;

            ContainerItems.Add(item);

            if (act_on_item_add != null)
                act_on_item_add(item);
            eve_on_item_add.Invoke(item);
            return item;
        }
        #endregion

        #region 移除项

        /// <summary>
        /// 移除指定标识名称的容器项
        /// </summary>
        /// <param tweenName="Indicator"></param>
        public void Con_Remove_Item(string Indicator)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法移除容器项！", HudMsgState.警告);
                return;
            }
            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].Indicator == Indicator)
                {
                    ContainerItems.RemoveAt(i);

                    if (act_on_item_remove != null)
                        act_on_item_remove(ContainerItems[i].Indicator, ContainerItems[i].ID);
                    eve_on_item_remove.Invoke(ContainerItems[i].Indicator, ContainerItems[i].ID);
                    break;
                }
            }
        }

        /// <summary>
        /// 移除指定ID的容器项
        /// </summary>
        /// <param tweenName="id"></param>
        public void Con_Remove_Item(int id)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法移除容器项！", HudMsgState.警告);
                return;
            }
            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].ID == id)
                {
                    ContainerItems.RemoveAt(i);

                    if (act_on_item_remove != null)
                        act_on_item_remove(ContainerItems[i].Indicator, ContainerItems[i].ID);
                    eve_on_item_remove.Invoke(ContainerItems[i].Indicator, ContainerItems[i].ID);
                    break;
                }
            }
        }

        /// <summary>
        /// 移除所有的容器项
        /// </summary>
        public void Con_Remove_Item_All()
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法移除容器项！", HudMsgState.警告);
                return;
            }
            for (int i = 0; i < ContainerItems.Count; i++)
            {
                ContainerItems.RemoveAt(i);
            }
            if (act_on_items_remove != null)
                act_on_items_remove();
            eve_on_items_remove.Invoke();
            ContainerItems.Clear();
        }
        #endregion

        #region 修改项

        /// <summary>
        /// 修改目标标识名称的容器项数值
        /// </summary>
        /// <param tweenName="indicator">标识名称</param>
        /// <param tweenName="val">目标值</param>
        /// <param tweenName="playanimator">是否播放动画</param>
        /// <param tweenName="usedelay">播放延迟</param>
        public void Con_ChangeItemValue(string indicator, string val, bool playanimator = false, bool usedelay = false)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法修改数值！", HudMsgState.警告);
                return;
            }

            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].Indicator == indicator)
                {
                    XHud_Container_Text based_text = ContainerItems[i].GetType<XHud_Container_Text>();
                    XHud_Container_TmpText based_tmp = ContainerItems[i].GetType<XHud_Container_TmpText>();

                    if (based_text != null)
                    {
                        based_text.SetValue(val);

                        if (playanimator)
                            Con_Animators_PlayAt(indicator, usedelay);

                        if (act_on_item_changevalue != null)
                            act_on_item_changevalue(indicator, ContainerItems[i]);
                        eve_on_item_changevalue.Invoke(indicator, ContainerItems[i]);
                    }
                    else if (based_tmp != null)
                    {
                        based_tmp.SetValue(val);

                        if (playanimator)
                            Con_Animators_PlayAt(indicator, usedelay);

                        if (act_on_item_changevalue != null)
                            act_on_item_changevalue(indicator, ContainerItems[i]);
                        eve_on_item_changevalue.Invoke(indicator, ContainerItems[i]);
                    }

                    if (UseDebug)
                        XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "修改了标识为：" + indicator + "的容器项的值为:" + val + "！", HudMsgState.通知);
                }
            }
        }

        /// <summary>
        /// 修改目标标识名称的容器项数值
        /// </summary>
        /// <param tweenName="indicator">标识名称</param>
        /// <param tweenName="val">目标值</param>
        /// <param tweenName="playanimator">是否播放动画</param>
        /// <param tweenName="usedelay">播放延迟</param>
        public void Con_ChangeItemValue(string indicator, Sprite val, bool playanimator = false, bool usedelay = false)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法修改数值！", HudMsgState.警告);
                return;
            }

            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].Indicator == indicator)
                {
                    XHud_Container_Image based_image = ContainerItems[i].GetType<XHud_Container_Image>();

                    if (based_image != null)
                    {
                        based_image.SetValue(val);

                        if (playanimator)
                            Con_Animators_PlayAt(indicator, usedelay);

                        if (act_on_item_changevalue != null)
                            act_on_item_changevalue(indicator, ContainerItems[i]);
                        eve_on_item_changevalue.Invoke(indicator, ContainerItems[i]);
                    }

                    if (UseDebug)
                        XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "修改了标识为：" + indicator + "的容器项的值为:" + val + "！", HudMsgState.通知);
                }
            }
        }

        /// <summary>
        /// 修改目标标识名称的容器项数值
        /// </summary>
        /// <param tweenName="indicator">标识名称</param>
        /// <param tweenName="val">目标值</param>
        /// <param tweenName="playanimator">是否播放动画</param>
        /// <param tweenName="usedelay">播放延迟</param>
        public void Con_ChangeItemValue(string indicator, Texture2D val, bool playanimator = false, bool usedelay = false)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法修改数值！", HudMsgState.警告);
                return;
            }

            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].Indicator == indicator)
                {
                    XHud_Container_Image based_image = ContainerItems[i].GetType<XHud_Container_Image>();

                    if (based_image != null)
                    {
                        based_image.SetValue(val);

                        if (playanimator)
                            Con_Animators_PlayAt(indicator, usedelay);

                        if (act_on_item_changevalue != null)
                            act_on_item_changevalue(indicator, ContainerItems[i]);
                        eve_on_item_changevalue.Invoke(indicator, ContainerItems[i]);
                    }

                    if (UseDebug)
                        XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "修改了标识为：" + indicator + "的容器项的值为:" + val + "！", HudMsgState.通知);
                }
            }
        }

        /// <summary>
        /// 修改目标ID的容器项数值 - 字符串
        /// </summary>
        /// <param tweenName="id">标识名称</param>
        /// <param tweenName="val">目标值</param>
        /// <param tweenName="playanimator">是否播放动画</param>
        /// <param tweenName="usedelay">播放延迟</param>
        public void Con_ChangeItemValue(int id, string val, bool playanimator = false, bool usedelay = false)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法修改数值！", HudMsgState.警告);
                return;
            }

            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].ID == id)
                {
                    XHud_Container_Text based_text = ContainerItems[i].GetType<XHud_Container_Text>();
                    XHud_Container_TmpText based_tmp = ContainerItems[i].GetType<XHud_Container_TmpText>();

                    if (based_text != null)
                    {
                        based_text.SetValue(val);

                        if (playanimator)
                            Con_Animators_PlayAt(id, usedelay);

                        if (act_on_item_changevalue != null)
                            act_on_item_changevalue(id.ToString(), ContainerItems[i]);
                        eve_on_item_changevalue.Invoke(id.ToString(), ContainerItems[i]);
                    }
                    else if (based_tmp != null)
                    {
                        based_tmp.SetValue(val);

                        if (playanimator)
                            Con_Animators_PlayAt(id, usedelay);

                        if (act_on_item_changevalue != null)
                            act_on_item_changevalue(id.ToString(), ContainerItems[i]);
                        eve_on_item_changevalue.Invoke(id.ToString(), ContainerItems[i]);
                    }

                    if (UseDebug)
                        XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "修改了ID为：" + id + "的容器项的值为:" + val + "！", HudMsgState.通知);
                }
            }
        }

        /// <summary>
        /// 修改目标ID的容器项数值  - 精灵
        /// </summary>
        /// <param tweenName="id">标识名称</param>
        /// <param tweenName="val">目标值</param>
        /// <param tweenName="playanimator">是否播放动画</param>
        /// <param tweenName="usedelay">播放延迟</param>
        public void Con_ChangeItemValue(int id, Sprite val, bool playanimator = false, bool usedelay = false)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法修改数值！", HudMsgState.警告);
                return;
            }

            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].ID == id)
                {
                    XHud_Container_Image based_image = ContainerItems[i].GetType<XHud_Container_Image>();

                    if (based_image != null)
                    {
                        based_image.SetValue(val);

                        if (playanimator)
                            Con_Animators_PlayAt(id, usedelay);

                        if (act_on_item_changevalue != null)
                            act_on_item_changevalue(id.ToString(), ContainerItems[i]);
                        eve_on_item_changevalue.Invoke(id.ToString(), ContainerItems[i]);
                    }

                    if (UseDebug)
                        XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "修改了ID为：" + id + "的容器项的值为:" + val.name + "！", HudMsgState.通知);
                }
            }
        }

        /// <summary>
        /// 修改目标ID的容器项数值 - 图片
        /// </summary>
        /// <param tweenName="id">标识名称</param>
        /// <param tweenName="val">目标值</param>
        /// <param tweenName="playanimator">是否播放动画</param>
        /// <param tweenName="usedelay">播放延迟</param>
        public void Con_ChangeItemValue(int id, Texture2D val, bool playanimator = false, bool usedelay = false)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法修改数值！", HudMsgState.警告);
                return;
            }

            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].ID == id)
                {
                    XHud_Container_Image based_image = ContainerItems[i].GetType<XHud_Container_Image>();

                    if (based_image != null)
                    {
                        based_image.SetValue(val);

                        if (playanimator)
                            Con_Animators_PlayAt(id, usedelay);

                        if (act_on_item_changevalue != null)
                            act_on_item_changevalue(id.ToString(), ContainerItems[i]);
                        eve_on_item_changevalue.Invoke(id.ToString(), ContainerItems[i]);
                    }

                    if (UseDebug)
                        XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "修改了ID为：" + id + "的容器项的值为:" + val.name + "！", HudMsgState.通知);
                }
            }
        }

        /// <summary>
        /// 修改目标标识名称的容器项数值
        /// </summary>
        /// <param tweenName="indicator">标识名称</param>
        /// <param tweenName="val">目标值</param>
        /// <param tweenName="playanimator">是否播放动画</param>
        /// <param tweenName="usedelay">播放延迟</param>
        public void Con_ChangeItemValue_RawImage(string indicator, Texture2D val, bool playanimator = false, bool usedelay = false)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法修改数值！", HudMsgState.警告);
                return;
            }

            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].Indicator == indicator)
                {
                    XHud_Container_RawImage based_image = ContainerItems[i].GetType<XHud_Container_RawImage>();

                    if (based_image != null)
                    {
                        based_image.SetValue(val);

                        if (playanimator)
                            Con_Animators_PlayAt(indicator, usedelay);

                        if (act_on_item_changevalue != null)
                            act_on_item_changevalue(indicator, ContainerItems[i]);
                        eve_on_item_changevalue.Invoke(indicator, ContainerItems[i]);
                    }

                    if (UseDebug)
                        XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "修改了标识为：" + indicator + "的容器项的值为:" + val + "！", HudMsgState.通知);
                }
            }
        }

        /// <summary>
        /// 修改目标ID的容器项数值 - 图片
        /// </summary>
        /// <param tweenName="id">标识名称</param>
        /// <param tweenName="val">目标值</param>
        /// <param tweenName="playanimator">是否播放动画</param>
        /// <param tweenName="usedelay">播放延迟</param>
        public void Con_ChangeItemValue_RawImage(int id, Texture2D val, bool playanimator = false, bool usedelay = false)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法修改数值！", HudMsgState.警告);
                return;
            }

            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].ID == id)
                {
                    XHud_Container_RawImage based_image = ContainerItems[i].GetType<XHud_Container_RawImage>();

                    if (based_image != null)
                    {
                        based_image.SetValue(val);

                        if (playanimator)
                            Con_Animators_PlayAt(id, usedelay);

                        if (act_on_item_changevalue != null)
                            act_on_item_changevalue(id.ToString(), ContainerItems[i]);
                        eve_on_item_changevalue.Invoke(id.ToString(), ContainerItems[i]);
                    }

                    if (UseDebug)
                        XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "修改了ID为：" + id + "的容器项的值为:" + val.name + "！", HudMsgState.通知);
                }
            }
        }

        #endregion

        #region 动画器播放与倒退

        //------------Play

        /// <summary>
        /// 播放容器项的动画器动画
        /// </summary>
        /// <param tweenName="usedelay">播放延迟</param>
        public void Con_Animators_PlayAll(bool usedelay = true)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法播放动画！", HudMsgState.警告);
                return;
            }
            for (int i = 0; i < ContainerItems.Count; i++)
            {
                Con_Animators_PlayAt(ContainerItems[i].ID, usedelay);
            }
            if (act_on_animate_play_all != null)
                act_on_animate_play_all(ContainerItems.ToArray());
            eve_on_animate_play_all.Invoke(ContainerItems.ToArray());
            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "播放容器列表中的所有动画器动画！", HudMsgState.通知);
        }

        /// <summary>
        /// 播放容器项的动画器动画
        /// </summary>
        /// <param tweenName="indicator">标识名称</param>
        /// <param tweenName="usedelay">播放延迟</param>
        public void Con_Animators_PlayAt(string indicator, bool usedelay = true)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法播放动画！", HudMsgState.警告);
                return;
            }
            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].Indicator == indicator)
                {
                    XHud_Module_Animator anim = ContainerItems[i].Animator;
                    float Delaytime = ContainerItems[i].DelayTime;
                    if (!usedelay)
                        Delaytime = 0;
                    float Duration = Animators_GlobalDuration * anim.Animator_GlobalDuration;
                    if (anim != null)
                    {
                        anim.Play("容器调用", Delaytime, Duration);
                        if (act_on_animate_play_by_item != null)
                            act_on_animate_play_by_item(ContainerItems[i], anim);
                        eve_on_animate_play_by_item.Invoke(ContainerItems[i], anim);
                    }
                }
            }

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "播放容器列表中指定标识的项的动画器动画！", HudMsgState.通知);
        }

        /// <summary>
        /// 播放容器项的动画器动画
        /// </summary>
        /// <param tweenName="id">标识ID</param>
        /// <param tweenName="usedelay">播放延迟</param>
        public void Con_Animators_PlayAt(int id, bool usedelay = true)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法播放动画！", HudMsgState.警告);
                return;
            }
            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].ID == id)
                {
                    XHud_Module_Animator anim = ContainerItems[i].Animator;
                    float Delaytime = ContainerItems[i].DelayTime;
                    if (!usedelay)
                        Delaytime = 0;
                    float Duration = Animators_GlobalDuration * anim.Animator_GlobalDuration;
                    if (anim != null)
                    {
                        anim.Play("容器调用", Delaytime, Duration);
                        if (act_on_animate_play_by_item != null)
                            act_on_animate_play_by_item(ContainerItems[i], anim);
                        eve_on_animate_play_by_item.Invoke(ContainerItems[i], anim);
                    }
                }
            }

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "播放容器列表中指定ID的项的动画器动画！", HudMsgState.通知);
        }

        //------------Rewind

        /// <summary>
        /// 复位容器项中所有的动画器动画
        /// </summary>
        /// <returns></returns>
        public void Con_Animators_RewindAll()
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法复位动画！", HudMsgState.警告);
                return;
            }
            for (int i = 0; i < ContainerItems.Count; i++)
            {
                XHud_Module_Animator anim = ContainerItems[i].Animator;
                if (anim != null)
                    anim.RewindAllTweenNode();
            }

            if (act_on_animate_rewind_all != null)
                act_on_animate_rewind_all(ContainerItems.ToArray());
            eve_on_animate_rewind_all.Invoke(ContainerItems.ToArray());
            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "复位容器列表中的所有动画器动画！", HudMsgState.通知);
        }

        /// <summary>
        /// 复位目标标识名称的容器项中的所有的动画器动画
        /// </summary>
        /// <returns></returns>
        public void Con_Animators_RewindAt(string indicator)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法复位动画！", HudMsgState.警告);
                return;
            }
            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].Indicator == indicator)
                {
                    XHud_Module_Animator anim = ContainerItems[i].Animator;
                    if (anim != null)
                    {
                        anim.RewindAllTweenNode();
                        if (act_on_animate_rewind_by_item != null)
                            act_on_animate_rewind_by_item(ContainerItems[i], anim);
                        eve_on_animate_rewind_by_item.Invoke(ContainerItems[i], anim);
                    }
                }
            }

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "复位容器列表中指定标识的项的动画器动画！", HudMsgState.通知);
        }

        /// <summary>
        /// 复位目标ID的容器项中的所有的动画器动画
        /// </summary>
        /// <returns></returns>
        public void Con_Animators_RewindAt(int id)
        {
            if (!IsEnabled)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "因功能被禁用无法复位动画！", HudMsgState.警告);
                return;
            }
            for (int i = 0; i < ContainerItems.Count; i++)
            {
                if (ContainerItems[i].ID == id)
                {
                    XHud_Module_Animator anim = ContainerItems[i].Animator;
                    if (anim != null)
                    {
                        anim.RewindAllTweenNode();
                        if (act_on_animate_rewind_by_item != null)
                            act_on_animate_rewind_by_item(ContainerItems[i], anim);
                        eve_on_animate_rewind_by_item.Invoke(ContainerItems[i], anim);
                    }
                }
            }

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "复位容器列表中指定ID的项的动画器动画！", HudMsgState.通知);
        }

        #endregion

        #region 事件 & 委托

        /// <summary>
        /// 移除所有委托
        /// </summary>
        public void EventsClear()
        {
            act_on_itemcount_get = null;
            act_on_item_get = null;
            act_on_items_get = null;
            act_on_item_add = null;
            act_on_item_remove = null;
            act_on_items_remove = null;
            act_on_animate_play_all = null;
            act_on_animate_play_by_item = null;
            act_on_animate_rewind_all = null;
            act_on_animate_rewind_by_item = null;
            act_on_animate_ready_all = null;
            act_on_animate_ready_by_item = null;
            act_on_item_changevalue = null;

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "清空所有委托！", HudMsgState.通知);
        }

        /// <summary>
        /// 移除所有事件
        /// </summary>
        public void ActionsClear()
        {
            eve_on_itemcount_get.RemoveAllListeners();
            eve_on_item_get.RemoveAllListeners();
            eve_on_items_get.RemoveAllListeners();
            eve_on_item_add.RemoveAllListeners();
            eve_on_item_remove.RemoveAllListeners();
            eve_on_items_remove.RemoveAllListeners();
            eve_on_animate_play_all.RemoveAllListeners();
            eve_on_animate_play_by_item.RemoveAllListeners();
            eve_on_animate_rewind_all.RemoveAllListeners();
            eve_on_animate_rewind_by_item.RemoveAllListeners();
            eve_on_animate_ready_all.RemoveAllListeners();
            eve_on_animate_ready_by_item.RemoveAllListeners();
            eve_on_item_changevalue.RemoveAllListeners();

            if (UseDebug)
                XHud_Utilitys.Func_PrintInfo("XHud - 容器控件通知", "清空所有委托！", HudMsgState.通知);
        }

        #endregion
    }
}