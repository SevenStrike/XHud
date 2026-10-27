/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
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
    using SevenStrikeModules.XTween;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [System.Serializable]
    public struct XHudElementsStatistic
    {
        /// <summary>
        /// 数量数量 - ents
        /// </summary>
        public int count_elements;
        /// <summary>
        /// 数量 - sounders
        /// </summary>
        public int count_sounders;
        /// <summary>
        /// 数量 - primitives
        /// </summary>
        public int count_primitives;
        /// <summary>
        /// 数量 - sliders
        /// </summary>
        public int count_sliders;
        /// <summary>
        /// 数量 - toggles
        /// </summary>
        public int count_toggles;
        /// <summary>
        /// 数量 - progresses
        /// </summary>
        public int count_progresses;
        /// <summary>
        /// 数量 - buttons
        /// </summary>
        public int count_buttons;
        /// <summary>
        /// 数量 - options
        /// </summary>
        public int count_options;
        /// <summary>
        /// 数量 - texts
        /// </summary>
        public int count_texts;
        /// <summary>
        /// 数量 - tmptexts
        /// </summary>
        public int count_tmptexts;
    }

    /// <summary>
    /// 元素节点
    /// </summary>
    [System.Serializable]
    public class XHudElementNode
    {
        /// <summary>
        /// 标识名称
        /// </summary>
        public string Indicator;
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName;
        /// <summary>
        /// 模块ID编号
        /// </summary>
        public int ID;
        /// <summary>
        /// Hud元素
        /// </summary>
        public XHud_Module_Element Element;
        /// <summary>
        /// 动画状态
        /// </summary>
        public bool IsAnimating;

        // ---------------------------------------------------链式方法
        /// <summary>
        /// 设置位置 - 根据类型
        /// </summary>
        public XHudElementNode SetPosition_BySpace(XHudSpace space, Vector3 pos, bool AutoIn = false, Motion_Creator creator = null)
        {
            switch (space)
            {
                case XHudSpace.屏幕空间:
                    Element.element_PositionSet(pos);
                    break;
                case XHudSpace.世界空间:
                    Element.element_WorldPositionSet(pos);
                    break;
            }

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置位置 - 根据类型
        /// </summary>
        public XHudElementNode SetPosition_BySpace(XHudSpace space, Transform transform, bool AutoIn = false, Motion_Creator creator = null)
        {
            switch (space)
            {
                case XHudSpace.屏幕空间:
                    Element.element_PositionSet(transform.GetComponent<RectTransform>().anchoredPosition3D);
                    break;
                case XHudSpace.世界空间:
                    Element.element_WorldPositionSet(transform.position);
                    break;
            }

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置旋转 - 根据类型
        /// </summary>
        public XHudElementNode SetRotation_BySpace(XHudSpace space, Vector3 rot, bool AutoIn = false, Motion_Creator creator = null)
        {
            switch (space)
            {
                case XHudSpace.屏幕空间:
                    Element.element_RotationSet(rot);
                    break;
                case XHudSpace.世界空间:
                    Element.element_WorldRotationSet(Quaternion.Euler(rot));
                    break;
            }

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置位置 - 屏幕
        /// </summary>
        public XHudElementNode SetPosition_Screen(Vector3 pos, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.element_PositionSet(pos);

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置位置 - 屏幕
        /// </summary>
        public XHudElementNode SetPosition_Screen(RectTransform rect, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.element_PositionSet(rect.anchoredPosition3D);

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置位置 - 世界
        /// </summary>
        public XHudElementNode SetPosition_World(Vector3 pos, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.element_WorldPositionSet(pos);

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置位置 - 世界
        /// </summary>
        public XHudElementNode SetPosition_World(Transform trs, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.element_WorldPositionSet(trs.position);

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置旋转 - 屏幕
        /// </summary>
        public XHudElementNode SetRotation_Screen(Vector3 rot, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.element_RotationSet(rot);

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置旋转 - 世界
        /// </summary>
        public XHudElementNode SetRotation_World(Quaternion rot, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.element_WorldRotationSet(rot);

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置位置偏移
        /// </summary>
        public XHudElementNode SetOffset(Vector3 offset, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.element_PositionOffset(offset);

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置位置偏移 (x, y, z)
        /// </summary>
        public XHudElementNode SetOffset(float x, float y, float z, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.element_PositionOffset(new Vector3(x, y, z));

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置缩放
        /// </summary>
        public XHudElementNode SetScale(Vector3 scale, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.element_ScaleSet(scale);

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置缩放 (x, y, z)
        /// </summary>
        public XHudElementNode SetScale(float x, float y, float z, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.element_ScaleSet(new Vector3(x, y, z));

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置尺寸
        /// </summary>
        public XHudElementNode SetSize(Vector2 size, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.element_SizeSet(size);

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置尺寸 (width, height)
        /// </summary>
        public XHudElementNode SetSize(float width, float height, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.element_SizeSet(new Vector2(width, height));

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置透明度
        /// </summary>
        public XHudElementNode SetAlpha(float alpha, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.element_AlphaSet(alpha);

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 启用 RMS 布局适配
        /// </summary>
        public XHudElementNode SetRMS(bool enabled = true, string rmsName = "", bool AutoIn = false, Motion_Creator creator = null)
        {
            if (!enabled)
                return this;

            Element.element_RMS_Mode_Enabled(enabled);
            if (!string.IsNullOrEmpty(rmsName))
                Element.rms_name = rmsName;

            if (enabled && XHud_Manager.instance.rms_enabled)
            {
                Anchor_Layout anchor_struct = null;

                // 如果RMS名称为空，则使用当前的RMS指定的布局锚点
                if (string.IsNullOrEmpty(rmsName))
                    anchor_struct = XHud_Manager.instance.hm_ScreenElement_Get_RMS_Anchored(Element, XHud_Manager.instance.rms_current_solution);
                else
                    anchor_struct = XHud_Manager.instance.hm_ScreenElement_Get_RMS_Anchored(Element, rmsName);

                // 如果RMS名称为空，那么就参考 XHud 管理器当前选中的方案作为元素的RMS方案
                if (string.IsNullOrEmpty(rmsName))
                    XHud_Manager.instance.hm_ScreenElement_Initialize_By_RMS(Element, XHud_Manager.instance.rms_current_solution);
                else
                    XHud_Manager.instance.hm_ScreenElement_Initialize_By_RMS(Element, rmsName);
            }

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置并放置到锚点下
        /// </summary>
        public XHudElementNode SetAnchored_Screen(XHudAnchor anchor, string anchor_indicator, bool AutoIn = false, Motion_Creator creator = null)
        {
            Anchor_Layout anchor_struct = null;

            // 获取的匹配的锚点结构参数
            anchor_struct = XHud_Manager.instance.hm_ScreenElement_Matched_AnchoredType(anchor);
            // 初始化元素
            XHud_Manager.instance.hm_ScreenElement_Initialize_By_MotionArgs(Element, anchor_struct.Type);
            // 标记为已创建
            Element.create_state = XHudElementCreateState.Created;
            // 设置缩放归一化
            Element.element_ScaleSet(Vector3.one);
            // 尺寸归一化
            Element.element_SizeSet(Vector2.one * 100);
            // 将元素放置到目标屏幕锚点物体下，并设置标识名称和标记名称
            XHudElementNode node = XHud_Manager.instance.hm_ScreenElement_Send_To_AnchoredList(anchor_struct, Element, string.IsNullOrEmpty(Element.indicator) ? Element.gameObject.name : Element.indicator, anchor_indicator);

            // 自动播放入场状态标记
            if (AutoIn)
                node.Element_In(creator);
            return node;
        }
        /// <summary>
        /// 设置并放置到锚点下
        /// </summary>
        public XHudElementNode SetAnchored_Screen(XHudAnchor anchor, string anchor_indicator, Vector2 size, Vector3 scale, bool AutoIn = false, Motion_Creator creator = null)
        {
            Anchor_Layout anchor_struct = null;

            // 获取的匹配的锚点结构参数
            anchor_struct = XHud_Manager.instance.hm_ScreenElement_Matched_AnchoredType(anchor);
            // 初始化元素
            XHud_Manager.instance.hm_ScreenElement_Initialize_By_MotionArgs(Element, anchor_struct.Type);
            // 标记为已创建
            Element.create_state = XHudElementCreateState.Created;
            // 设置缩放归一化
            Element.element_ScaleSet(scale);
            // 尺寸归一化
            Element.element_SizeSet(size);
            // 将元素放置到目标屏幕锚点物体下
            XHudElementNode node = XHud_Manager.instance.hm_ScreenElement_Send_To_AnchoredList(anchor_struct, Element, string.IsNullOrEmpty(Element.indicator) ? Element.gameObject.name : Element.indicator, anchor_indicator);

            // 自动播放入场状态标记
            if (AutoIn)
                node.Element_In(creator);

            return node;
        }
        /// <summary>
        /// 设置并放置到世界下
        /// </summary>
        public XHudElementNode SetAnchored_World(string indicator, bool AutoIn = false, Motion_Creator creator = null)
        {
            // 初始化元素
            XHud_Manager.instance.hm_WorldElement_Initialize_For_World(Element);
            // 标记为已创建
            Element.create_state = XHudElementCreateState.Created;
            // 设置缩放归一化
            Element.element_ScaleSet(Vector3.one);
            // 尺寸归一化
            Element.element_SizeSet(Vector2.one * 100);
            // 将元素放置到目标屏幕锚点物体下
            XHudElementNode node = XHud_Manager.instance.hm_WorldElement_Send_To_AnchoredList(Element, string.IsNullOrEmpty(Element.indicator) ? Element.gameObject.name : Element.indicator, indicator);

            // 自动播放入场状态标记
            if (AutoIn)
                node.Element_In(creator);

            return node;
        }
        /// <summary>
        /// 设置并放置到世界下
        /// </summary>
        public XHudElementNode SetAnchored_World(string indicator, Vector2 size, Vector3 scale, bool AutoIn = false, Motion_Creator creator = null)
        {
            // 初始化元素
            XHud_Manager.instance.hm_WorldElement_Initialize_For_World(Element);
            // 标记为已创建
            Element.create_state = XHudElementCreateState.Created;
            // 设置缩放归一化
            Element.element_ScaleSet(scale);
            // 尺寸归一化
            Element.element_SizeSet(size);
            // 将元素放置到目标屏幕锚点物体下
            XHudElementNode node = XHud_Manager.instance.hm_WorldElement_Send_To_AnchoredList(Element, string.IsNullOrEmpty(Element.indicator) ? Element.gameObject.name : Element.indicator, indicator);

            // 自动播放入场状态标记
            if (AutoIn)
                node.Element_In(creator);

            return node;
        }
        /// <summary>
        /// 设置入场动画开始回调
        /// </summary>
        public XHudElementNode On_In_Start(UnityAction<XHud_Module_Element> action, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.act_on_element_in_start += action;

            // 自动播放入场状态标记
            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置入场动画进度回调
        /// </summary>
        public XHudElementNode On_In_Progress(UnityAction<float> action, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.act_on_element_in_progress += action;

            // 自动播放入场状态标记
            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置入场动画结束回调
        /// </summary>
        public XHudElementNode On_In_End(UnityAction<XHud_Module_Element> action, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.act_on_element_in_end += action;

            // 自动播放入场状态标记
            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置出场动画开始回调
        /// </summary>
        public XHudElementNode On_Out_Start(UnityAction<XHud_Module_Element> action, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.act_on_element_out_start += action;

            // 自动播放入场状态标记
            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置出场动画进度回调
        /// </summary>
        public XHudElementNode On_Out_Progress(UnityAction<float> action, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.act_on_element_out_progress += action;

            // 自动播放入场状态标记
            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置出场动画结束回调
        /// </summary>
        public XHudElementNode On_Out_End(UnityAction<XHud_Module_Element> action, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.act_on_element_out_end += action;

            // 自动播放入场状态标记
            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 手动播放入场动画（当 autoin = false 时使用）
        /// </summary>
        public XHudElementNode Element_In(Motion_Creator args = null)
        {
            Element.Element_In(args ?? XHud_Manager.instance.global_motion_create_args);
            return this;
        }
        /// <summary>
        /// 手动播放出场动画
        /// </summary>
        public XHudElementNode Element_Out(Motion_Recycler args = null)
        {
            Element.Element_Out(args ?? XHud_Manager.instance.global_motion_recycle_args);
            return this;
        }

        // -----------------------------------------获取自身

        /// <summary>
        /// 获取元素本身（方便直接使用）
        /// </summary>
        public XHud_Module_Element GetElement()
        {
            return Element;
        }

        /// <summary>
        /// 获取元素节点ID
        /// </summary>
        public int GetElementID()
        {
            return ID;
        }
    }

    public partial class XHud_Manager : MonoBehaviour
    {
        /// <summary>
        /// 元素库 - 配置化（Editor期间）
        /// </summary>
        [SerializeField]
        public List<XHud_Library_Element> ele_library_elements = new List<XHud_Library_Element>();
        /// <summary>
        /// 默认参数 - 元素生成
        /// </summary>
        [SerializeField]
        public Motion_Creator global_motion_create_args = new Motion_Creator
        {
            // 创建HudManager 的时候初始化动效参数
            anchor = XHudAnchor.中心,
            Movement = new MotionNode_Movement()
            {
                Movement = HudMotion_Movement.S_从下至上,
                Distance = 100,
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic,
            },
            Rotation = new MotionNode_Rotation()
            {
                Rotation = HudMotion_Rotation.A_无旋转,
                Degree = 0,
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic,
            },
            Alpha = new MotionNode_Alpha()
            {
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic,
            }
        };
        /// <summary>
        /// 默认参数 - 元素回收
        /// </summary>
        [SerializeField]
        public Motion_Recycler global_motion_recycle_args = new Motion_Recycler
        {
            // 创建HudManager 的时候初始化动效参数
            MotionAnimateEndState = MotionAnimateEndState.以_透明度为准,
            Movement = new MotionNode_Movement()
            {
                Movement = HudMotion_Movement.D_从上至下,
                Distance = 100,
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic
            },
            Rotation = new MotionNode_Rotation()
            {
                Rotation = HudMotion_Rotation.A_无旋转,
                Degree = 0,
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic
            },
            Alpha = new MotionNode_Alpha()
            {
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic
            }
        };
        /// <summary>
        /// 用于编辑器内元素生成模板下拉菜单的选择索引名称
        /// </summary>
        [SerializeField]
        public string ele_global_motion_create_library_index_name;
        /// <summary>
        /// 用于编辑器内元素回收模板下拉菜单的选择索引名称
        /// </summary>
        [SerializeField]
        public string ele_global_motion_recycle_library_index_name;
        /// <summary>
        /// 元素屏幕拖放放置器的开关标记字段
        /// </summary>
        [SerializeField]
        public bool ele_visual_placer = false;

        #region 元素库相关操作
        /// <summary>
        /// 获取所有元素库
        /// </summary>
        /// <returns></returns>
        public XHud_Library_Element[] hm_ElementLibrary_GetArray()
        {
            return ele_library_elements.ToArray();
        }
        /// <summary>
        /// 获取所有元素库
        /// </summary>
        /// <returns></returns>
        public List<XHud_Library_Element> hm_ElementLibrary_GetList()
        {
            return ele_library_elements;
        }
        /// <summary>
        /// 获取所有元素库
        /// </summary>
        /// <returns></returns>
        public string[] hm_ElementLibrary_GetAllLibraryNames()
        {
            string[] names = new string[ele_library_elements.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = ele_library_elements[i].library_name;
            }
            return names;
        }
        /// <summary>
        /// 获取目标元素库
        /// </summary>
        /// <returns></returns>
        public XHud_Library_Element hm_ElementLibrary_GetTargetLibrary(string name)
        {
            XHud_Library_Element lib = null;

            string[] lib_names = hm_ElementLibrary_GetAllLibraryNames();
            for (int i = 0; i < lib_names.Length; i++)
            {
                if (name == lib_names[i])
                {
                    lib = ele_library_elements[i];
                }
            }
            return lib;
        }
        /// <summary>
        /// 获取首位元素库
        /// </summary>
        /// <param name="LibName">目标元素库</param>
        public XHud_Library_Element hm_ElementLibrary_GetFirstLibrary()
        {
            XHud_Library_Element res_lib = null;
            if (ele_library_elements != null && ele_library_elements.Count > 0)
                res_lib = ele_library_elements[0];
            return res_lib;
        }
        /// <summary>
        /// 元素库状态更新
        /// 遍历所有元素库，更新每个预生成元素的使用状态、使用计数和回收计数
        /// 
        /// 工作原理：
        /// 1. 检查元素库是否已初始化且不为空
        /// 2. 遍历所有元素库（Hud_ElementLibrarys）
        /// 3. 遍历每个元素库中的所有预制体项（ElementLibrary）
        /// 4. 遍历每个预制体项中的预生成元素列表（PreloadElements）
        /// 5. 更新每个元素的使用状态（Using）
        /// 6. 重新计算该预制体项的使用数量（UsedCount）和回收数量（RecycledCount）
        /// 
        /// 状态定义：
        /// - Using: 元素当前是否正在被使用（CreateState == Created）
        /// - UsedCount: 当前正在使用的元素数量
        /// - RecycledCount: 当前在池中空闲的元素数量
        /// 
        /// 更新时机：
        /// - 每帧在 XHud_Manager.Update() 中调用
        /// - 确保元素使用状态的实时性
        /// 
        /// 使用场景：
        /// - 元素库监控：实时查看元素使用情况
        /// - 调试：快速定位哪些元素正在使用
        /// - 性能优化：发现某个预制体频繁耗尽，考虑增加初始化数量
        /// - 资源管理：确保元素回收后正确标记为空闲
        /// 
        /// 性能考虑：
        /// - 遍历所有库、所有项、所有预生成元素
        /// - 如果元素库较大（多个库，每个库多个预制体，每个预制体多个实例），每帧遍历可能有性能压力
        /// - 建议在实际项目中根据元素数量评估是否需要每帧调用
        /// 
        /// 注意事项：
        /// - 此方法仅更新统计数据，不改变元素的实际使用状态
        /// - 确保 PreloadElements 列表中的元素引用有效
        /// </summary>
        private void hm_ElementLibrary_UpdateStates()
        {
            if (ele_library_elements == null || ele_library_elements.Count <= 0)
                return;

            #region 更新元素是否正在在被使用的状态
            for (int i = 0; i < ele_library_elements.Count; i++)
            {
                XHud_Library_Element lib = ele_library_elements[i];
                for (int s = 0; s < lib.library_elements.Count; s++)
                {
                    XHud_LibraryArg_Element_Item item = lib.library_elements[s];
                    if (item.preload_elements != null && item.preload_elements.Count > 0)
                    {
                        for (int v = 0; v < item.preload_elements.Count; v++)
                        {
                            item.preload_elements[v].UpdateState();
                        }
                        item.GetUsedCount();
                        item.GetRecycledCount();
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// 初始化元素库（对象池系统）
        /// 根据配置的预制体列表，预先生成指定数量的 UI 元素实例，放入对象池待用
        /// 
        /// 工作原理：
        /// 1. 创建 Pool_Elements 根节点，作为所有元素库的容器
        /// 2. 遍历所有元素库（XHud_Library_Element）
        /// 3. 为每个库创建独立的目录节点
        /// 4. 遍历库中的每个预制体，实例化指定数量（InitializeCount）的元素副本
        /// 5. 将生成的元素放入 PreloadElements 列表，初始状态设为 Recycled（已回收）
        /// 6. 禁用生成的元素，等待后续从池中取出使用
        /// 
        /// 为什么需要对象池？
        /// - UI 元素频繁创建销毁会造成 GC 压力，导致卡顿
        /// - 对象池通过复用机制，大幅减少内存分配和回收
        /// - 预先生成（预热）可以避免首次使用时产生卡顿
        /// 
        /// 数据结构：
        /// Pool_Elements/                          # 根节点
        ///   ├── Library -> (MainUI)/              # 主 UI 库目录
        ///   │     ├── Category - 按钮/             # 按钮预制体目录
        ///   │     │     ├── Button_Clone_0         # 已回收的按钮实例
        ///   │     │     ├── Button_Clone_1         # 已回收的按钮实例
        ///   │     │     └── Button_Clone_2         # 已回收的按钮实例
        ///   │     └── Category - 弹窗/             # 弹窗预制体目录
        ///   │           ├── Popup_Clone_0
        ///   │           └── Popup_Clone_1
        ///   └── Library -> (CommonUI)/             # 通用 UI 库目录
        ///         └── Category - 提示框/
        ///               └── Toast_Clone_0
        /// 
        /// 使用流程：
        /// 1. 初始化：预生成元素 → 放入池中（状态：Recycled）
        /// 2. 使用时：从池中取出（Spawn）→ 激活并播放动画（状态：Created）
        /// 3. 回收时：播放出场动画 → 放回池中（状态：Recycled）
        /// </summary>
        private void hm_ElementLibrary_Initialize()
        {
            if (ele_library_elements.Count <= 0)
                return;
            ///---创建元素库根目录
            GameObject PoolRoot = new GameObject();
            PoolRoot.name = "Pool_Elements";
            PoolRoot.layer = LayerMask.NameToLayer("XHud");
            PoolRoot.transform.SetParent(transform);
            PoolRoot.transform.localPosition = Vector3.zero;
            PoolRoot.transform.localEulerAngles = Vector3.zero;
            PoolRoot.transform.localScale = Vector3.one;

            hm_ElementLibrary_Clean();

            for (int v = 0; v < ele_library_elements.Count; v++)
            {
                XHud_Library_Element Lib = ele_library_elements[v];

                ///---创建库容器
                GameObject lib_obj = new GameObject();
                lib_obj.name = "Library -> ( " + Lib.library_name + " )";
                lib_obj.layer = LayerMask.NameToLayer("XHud");
                lib_obj.transform.SetParent(PoolRoot.transform);
                lib_obj.transform.localPosition = Vector3.zero;
                lib_obj.transform.localEulerAngles = Vector3.zero;
                lib_obj.transform.localScale = Vector3.one;

                Lib.library_root = lib_obj.transform;

                for (int i = 0; i < Lib.library_elements.Count; i++)
                {
                    XHud_LibraryArg_Element_Item item = Lib.library_elements[i];

                    if (item.target == null)
                    {
                        XGUI_Utilitys.Console("XHud - 管理器通知", $"库元素 \" {item.name} \" 的目标预制体 \"已丢失\"，预生成元素过程将  \"跳过\"  此元素！", XGUIMsgState.错误);
                        continue;
                    }

                    GameObject root = new GameObject();
                    root.name = "Category - " + item.target.name;
                    root.layer = LayerMask.NameToLayer("XHud");
                    root.transform.SetParent(lib_obj.transform);
                    root.transform.localPosition = Vector3.zero;
                    root.transform.localEulerAngles = Vector3.zero;
                    root.transform.localScale = Vector3.one;
                    item.root = root.transform;

                    for (int s = 0; s < item.initialize_count; s++)
                    {
                        XHud_Module_Element element = Instantiate(item.target, Vector3.zero, Quaternion.identity, item.root);
                        ///---保存原始名称
                        element.original_name = element.transform.name.Substring(0, element.transform.name.Length - 7);
                        ///---改名
                        element.transform.name = element.original_name + "_Clone_" + s;
                        ///---赋值源库名
                        element.origin_poolname = Lib.library_name;

                        element.GroupFold(
                            param: true,
                            preview: true,
                            state: false,
                            option: true,
                            rms: true,
                            list: true,
                            statistic: true,
                            component: true);

                        XHud_LibraryArg_Element_Info pw = new XHud_LibraryArg_Element_Info();
                        pw.element = element;
                        pw.element.create_state = XHudElementCreateState.Recycled;
                        pw.element.element_Reset();
                        pw.element.gameObject.SetActive(false);
                        item.preload_elements.Add(pw);

                    }
                }
            }
            if (act_elements_library_instantiated != null)
                act_elements_library_instantiated();
        }
        /// <summary>
        /// 回收一个元素到元素库
        /// </summary>
        /// <param name="Element">目标元素</param>
        public void hm_ElementLibrary_Despawn(XHud_Module_Element Element)
        {
            for (int i = 0; i < ele_library_elements.Count; i++)
            {
                XHud_Library_Element lib = ele_library_elements[i];

                for (int m = 0; m < lib.library_elements.Count; m++)
                {
                    XHud_LibraryArg_Element_Item item = lib.library_elements[m];

                    if (Element.original_name == item.name)
                    {
                        Element.transform.SetParent(item.root);
                        Element.element_Reset();
                        Element.gameObject.SetActive(false);
                    }
                }
            }
        }
        /// <summary>
        /// 回收所有元素库的所有元素
        /// </summary>
        /// <returns></returns>
        public void hm_ElementLibrary_DespawnAll()
        {
            for (int i = 0; i < ele_library_elements.Count; i++)
            {
                ele_library_elements[i].ElementLibrary_RecycleAll();
            }
        }
        /// <summary>
        /// 获取元素库的数量
        /// </summary>
        /// <returns></returns>
        public int hm_ElementLibrary_GetCount()
        {
            return ele_library_elements.Count;
        }
        /// <summary>
        /// 判断元素库是否存在有效
        /// </summary>
        /// <returns></returns>
        public bool hm_ElementLibrary_IsExist(string name)
        {
            string[] names = hm_ElementLibrary_GetAllLibraryNames();
            bool exist = false;

            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == name)
                {
                    exist = true;
                    break;
                }
            }

            return exist;
        }
        /// <summary>
        /// 清理预生成的元素队列列表，因为在Unity编辑器中如果对ScriptableObject临时赋值会被保留下来，因此需要清理队列和使用痕迹
        /// </summary>
        public void hm_ElementLibrary_Clean()
        {
            for (int v = 0; v < ele_library_elements.Count; v++)
            {
                XHud_Library_Element lib = ele_library_elements[v];
                #region 清理预生成元素及其使用信息痕迹，主要是为了弥补ScriptableObject在运行时也能被赋值
                lib.library_root = null;
                for (int i = 0; i < lib.library_elements.Count; i++)
                {
                    lib.library_elements[i].root = null;
                    lib.library_elements[i].used_count = 0;
                    lib.library_elements[i].recycled_count = 0;
                    lib.library_elements[i].next_index = 0;
                    lib.library_elements[i].root = null;

                    if (lib.library_elements[i].preload_elements == null)
                        lib.library_elements[i].preload_elements = new List<XHud_LibraryArg_Element_Info>();

                    lib.library_elements[i].preload_elements.Clear();
                }
                #endregion
            }
        }
        /// <summary>
        /// 从元素库中获取并激活指定元素
        /// </summary>
        /// <param name="ElementName">元素标识名称</param>
        /// <param name="LibraryName">可选：指定库名称，为null时遍历所有库</param>
        /// <returns>返回元素，未找到返回null</returns>
        public XHud_Module_Element hm_ElementLibrary_Spawn(string ElementName, string LibraryName = null)
        {
            XHud_Module_Element element = null;

            for (int i = 0; i < ele_library_elements.Count; i++)
            {
                // 如果指定了库名且不匹配，跳过
                if (LibraryName != null && LibraryName != ele_library_elements[i].library_name)
                    continue;

                XHud_Library_Element lib = ele_library_elements[i];

                for (int k = 0; k < lib.library_elements.Count; k++)
                {
                    XHud_LibraryArg_Element_Item item = lib.library_elements[k];

                    if (item.target.name == ElementName)
                    {
                        int index = item.next_index;

                        if (item.preload_elements != null && item.preload_elements.Count > 0)
                        {
                            if (item.preload_elements[index] != null)
                            {
                                element = item.preload_elements[index].element;
                                element.gameObject.SetActive(true);
                                // 更新索引
                                if (item.next_index >= item.initialize_count - 1)
                                    item.next_index = 0;
                                else
                                    item.next_index++;
                            }

                            if (act_spawn_element != null)
                                act_spawn_element(element);
                        }
                        return element; // 找到后直接返回
                    }
                }

                // 如果指定了库名，说明已经查找了指定库，无论是否找到都退出循环
                if (LibraryName != null)
                    break;
            }

            return element;
        }
        /// <summary>
        /// 保留原方法作为重载，保持接口兼容性
        /// </summary>
        /// <param name="LibraryName"></param>
        /// <param name="ModuleName"></param>
        /// <returns></returns>
        private XHud_Module_Element hm_ElementLibrary_GetElement(string LibraryName, string ModuleName)
        {
            return hm_ElementLibrary_Spawn(ModuleName, LibraryName);
        }

        #region 创建元素库
        /// <summary>
        /// 创建元素库的项
        /// </summary>
        /// <param name="Element">目标元素</param>
        /// <param name="InitialCount">初始化数量</param>
        /// <returns></returns>
        public XHud_LibraryArg_Element_Item hm_ElementLibrary_CreateItem(XHud_Module_Element Element, int InitialCount)
        {
            XHud_LibraryArg_Element_Item item = new XHud_LibraryArg_Element_Item();
            item.target = Element;
            item.name = string.IsNullOrEmpty(Element.transform.name) ? "NewElement" : Element.transform.name;
            item.initialize_count = InitialCount;

            if (item.preload_elements == null)
                item.preload_elements = new List<XHud_LibraryArg_Element_Info>();

            return item;
        }
        /// <summary>
        /// 动态创建元素库
        /// </summary>
        /// <param name="LibraryName">元素库名称</param>
        /// <param name="Items">元素库项数组</param>
        /// <returns>返回元素库</returns>
        public XHud_Library_Element hm_ElementLibrary_AddItem(string LibraryName, XHud_LibraryArg_Element_Item[] Items)
        {
            XHud_Library_Element lib = ScriptableObject.CreateInstance<XHud_Library_Element>();
            lib.name = "RunTimeLib";
            lib.library_name = LibraryName;
            for (int i = 0; i < Items.Length; i++)
            {
                if (lib.library_elements == null)
                    lib.library_elements = new List<XHud_LibraryArg_Element_Item>();

                lib.library_elements.Add(Items[i]);
            }
            return lib;
        }
        #endregion

        #region 元素库实例化 / 与销毁
        /// <summary>
        /// 实例化目标元素库
        /// </summary>
        /// <param name="Library">目标元素库</param>
        public void hm_ElementLibrary_Initialize(XHud_Library_Element Library)
        {
            GameObject PoolRoot = transform.Find("Pool_Elements").gameObject;
            if (PoolRoot == null)
            {
                ///---创建元素库根目录
                PoolRoot = new GameObject();
                PoolRoot.name = "Pool_Elements";
                PoolRoot.transform.SetParent(transform);
                PoolRoot.transform.localPosition = Vector3.zero;
                PoolRoot.transform.localEulerAngles = Vector3.zero;
                PoolRoot.transform.localScale = Vector3.one;
            }

            ///---创建库容器
            GameObject lib_obj = new GameObject();
            lib_obj.name = "Category -> ( " + Library.library_name + " )";
            lib_obj.transform.SetParent(PoolRoot.transform);
            lib_obj.transform.localPosition = Vector3.zero;
            lib_obj.transform.localEulerAngles = Vector3.zero;
            lib_obj.transform.localScale = Vector3.one;
            Library.library_root = lib_obj.transform;

            List<XHud_LibraryArg_Element_Item> items = Library.library_elements;

            for (int i = 0; i < items.Count; i++)
            {
                GameObject root = new GameObject();
                root.name = "Case - " + items[i].target.name;
                root.transform.SetParent(lib_obj.transform);
                root.transform.localPosition = Vector3.zero;
                root.transform.localEulerAngles = Vector3.zero;
                root.transform.localScale = Vector3.one;
                items[i].root = root.transform;

                for (int s = 0; s < items[i].initialize_count; s++)
                {
                    XHud_Module_Element element = Instantiate(items[i].target, Vector3.zero, Quaternion.identity, items[i].root);
                    ///---保存原始名称
                    element.original_name = element.transform.name.Substring(0, element.transform.name.Length - 7);
                    ///---改名
                    element.transform.name = element.original_name + "_Clone_" + s;

                    XHud_LibraryArg_Element_Info pw = new XHud_LibraryArg_Element_Info();
                    pw.element = element;
                    pw.element.create_state = XHudElementCreateState.Recycled;
                    pw.element.element_Reset();
                    pw.element.gameObject.SetActive(false);

                    if (items[i].preload_elements == null)
                        items[i].preload_elements = new List<XHud_LibraryArg_Element_Info>();

                    items[i].preload_elements.Add(pw);
                }
            }

            ele_library_elements.Add(Library);

            if (act_elements_library_instantiated != null)
                act_elements_library_instantiated();
        }
        /// <summary>
        /// 删除已经初始化的元素库
        /// </summary>
        /// <param name="LibName">目标元素库</param>
        public void hm_ElementLibrary_Destroyed(string LibName)
        {
            for (int i = 0; i < ele_library_elements.Count; i++)
            {
                XHud_Library_Element lib = ele_library_elements[i];
                if (lib.library_name == LibName)
                {
                    lib.ElementLibrary_RecycleAll();
                    DestroyImmediate(lib.library_root.gameObject, true);
                    ele_library_elements.RemoveAt(i);
                }
            }
        }
        #endregion
        #endregion

        #region 元素辅助类
        /// <summary>
        /// XHud - 管理器通知 - 设置父物体
        /// </summary>
        /// <param name="element">目标元素</param>
        /// <param name="anchor">锚点父物体</param>
        public void hm_Element_SetParentAnchored(XHud_Module_Element element, RectTransform anchor)
        {
            element.rect_transform.SetParent(anchor);
        }
        /// <summary>
        /// 检测是否存在已生成的Hud元素
        /// </summary>
        /// <returns>返回True则当前XHud - 管理器通知中已生成了Hud元素反之则说明已清空</returns>
        public bool hm_HasElements()
        {
            bool state = false;
            for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
            {
                if (cv_anchors_layout_screen[i].HudElementInfos != null && cv_anchors_layout_screen[i].HudElementInfos.Count > 0)
                {
                    state = true;
                }
            }
            return state;
        }
        /// <summary>
        /// 更新所有已生成 Hud 元素的动画状态
        /// 遍历屏幕空间锚点列表中的所有元素，同步其动画状态到元素节点信息中
        /// 
        /// 工作原理：
        /// 1. 遍历屏幕空间的所有锚点布局（Anchors_Layout_Screen）
        /// 2. 遍历每个锚点下的所有 Hud 元素（HudElementInfos）
        /// 3. 将每个元素当前的动画状态（Animating）同步到对应的节点信息中（IsAnimating）
        /// 
        /// 为什么需要这个方法？
        /// - 外部系统可能需要查询某个 UI 元素是否正在播放动画
        /// - 通过 HudElementNode.IsAnimating 可以快速获取状态，无需直接访问元素
        /// - 提供统一的动画状态查询接口，便于 UI 状态机管理
        /// 
        /// 同步的状态：
        /// - 元素的 Animating 属性（true/false）
        ///   true: 元素正在播放入场/出场动画
        ///   false: 元素处于静止状态
        /// 
        /// 使用场景：
        /// - 判断是否可以与 UI 元素交互（动画播放时通常禁用交互）
        /// - 等待所有动画完成后执行后续逻辑
        /// - UI 状态机中需要知道当前动画状态
        /// - 调试时查看哪些元素正在播放动画
        /// 
        /// 调用频率：
        /// - 每帧在 XHud_Manager.Update() 中调用
        /// - 开销较小，遍历所有已生成的 UI 元素
        /// 
        /// 性能考虑：
        /// - 如果场景中 UI 元素数量较多（100+），每帧遍历可能有轻微开销
        /// - 可以考虑仅在状态变化时触发事件，而不是每帧遍历
        /// 
        /// 注意事项：
        /// - 此方法仅同步屏幕空间的 UI 元素（世界空间元素未包含）
        /// - 确保元素节点引用有效，避免空引用异常
        /// </summary>
        public void hm_Element_UpdateAnimating()
        {
            for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
            {
                if (cv_anchors_layout_screen[i].HudElementInfos != null)
                {
                    for (int s = 0; s < cv_anchors_layout_screen[i].HudElementInfos.Count; s++)
                    {
                        if (cv_anchors_layout_screen[i].HudElementInfos[s] != null)
                        {
                            cv_anchors_layout_screen[i].HudElementInfos[s].IsAnimating = cv_anchors_layout_screen[i].HudElementInfos[s].Element.tweening;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 元素和组件统计，获取场景中现有的各种组件的集合数量
        /// </summary>
        /// <returns></returns>
        public XHudElementsStatistic hm_Element_GetStatistic()
        {
            XHudElementsStatistic statistic = new XHudElementsStatistic();

            //统计屏幕UI组件数
            for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
            {
                Anchor_Layout lay = cv_anchors_layout_screen[i];
                for (int s = 0; s < lay.HudElementInfos.Count; s++)
                {
                    statistic.count_elements++;

                    XHudElementNode node = lay.HudElementInfos[s];
                    statistic.count_primitives += node.Element.primitive_controller_nodes.Count;
                    statistic.count_sounders += node.Element.sounder_nodes.Count;
                    statistic.count_buttons += node.Element.button_nodes.Count;
                    statistic.count_options += node.Element.option_nodes.Count;
                    statistic.count_texts += node.Element.text_nodes.Count;
                    statistic.count_tmptexts += node.Element.tmptext_nodes.Count;
                    statistic.count_sliders += node.Element.slider_nodes.Count;
                    statistic.count_progresses += node.Element.progress_nodes.Count;
                    statistic.count_toggles += node.Element.toggle_nodes.Count;
                }
            }

            //统计世界UI组件数
            for (int i = 0; i < cv_anchors_layout_world.Count; i++)
            {
                statistic.count_elements++;

                XHudElementNode node = cv_anchors_layout_world[i];
                statistic.count_primitives += node.Element.primitive_controller_nodes.Count;
                statistic.count_sounders += node.Element.sounder_nodes.Count;
                statistic.count_buttons += node.Element.button_nodes.Count;
                statistic.count_options += node.Element.option_nodes.Count;
                statistic.count_texts += node.Element.text_nodes.Count;
                statistic.count_tmptexts += node.Element.tmptext_nodes.Count;
                statistic.count_sliders += node.Element.slider_nodes.Count;
                statistic.count_progresses += node.Element.progress_nodes.Count;
                statistic.count_toggles += node.Element.toggle_nodes.Count;
            }

            return statistic;
        }
        /// <summary>
        /// 元素和组件统计，获取场景中现有的各种组件的集合数量
        /// </summary>
        /// <returns></returns>
        public XHudElementsStatistic hm_Element_GetStatistic(XHudSpace space)
        {
            XHudElementsStatistic statistic = new XHudElementsStatistic();

            if (space == XHudSpace.屏幕空间)
            {
                //统计屏幕UI组件数
                for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
                {
                    Anchor_Layout lay = cv_anchors_layout_screen[i];
                    for (int s = 0; s < lay.HudElementInfos.Count; s++)
                    {
                        statistic.count_elements++;

                        XHudElementNode node = lay.HudElementInfos[s];
                        statistic.count_primitives += node.Element.primitive_controller_nodes.Count;
                        statistic.count_sounders += node.Element.sounder_nodes.Count;
                        statistic.count_buttons += node.Element.button_nodes.Count;
                        statistic.count_options += node.Element.option_nodes.Count;
                        statistic.count_texts += node.Element.text_nodes.Count;
                        statistic.count_tmptexts += node.Element.tmptext_nodes.Count;
                        statistic.count_sliders += node.Element.slider_nodes.Count;
                        statistic.count_progresses += node.Element.progress_nodes.Count;
                        statistic.count_toggles += node.Element.toggle_nodes.Count;
                    }
                }
            }
            else
            {
                //统计世界UI组件数
                for (int i = 0; i < cv_anchors_layout_world.Count; i++)
                {
                    statistic.count_elements++;

                    XHudElementNode node = cv_anchors_layout_world[i];
                    statistic.count_primitives += node.Element.primitive_controller_nodes.Count;
                    statistic.count_sounders += node.Element.sounder_nodes.Count;
                    statistic.count_buttons += node.Element.button_nodes.Count;
                    statistic.count_options += node.Element.option_nodes.Count;
                    statistic.count_texts += node.Element.text_nodes.Count;
                    statistic.count_tmptexts += node.Element.tmptext_nodes.Count;
                    statistic.count_sliders += node.Element.slider_nodes.Count;
                    statistic.count_progresses += node.Element.progress_nodes.Count;
                    statistic.count_toggles += node.Element.toggle_nodes.Count;
                }
            }
            return statistic;
        }
        #endregion

        //---------------------------------- 屏幕

        #region 创建辅助方法 - 屏幕元素
        /// <summary>
        /// 根据锚点类型获取锚点根物体
        /// </summary>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public RectTransform hm_ScreenElement_GetAnchored_RectTransform(XHudAnchor anchor)
        {
            RectTransform rect = null;
            for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
            {
                if (cv_anchors_layout_screen[i].Type == anchor)
                {
                    rect = cv_anchors_layout_screen[i].Anchor;
                    break;
                }
            }
            return rect;
        }
        /// <summary>
        /// XHud - 管理器通知 - 获取目标元素身上的分辨率匹配方案的标识名称的信息
        /// </summary>
        /// <param name="element">目标元素</param>
        /// <param name="solutionName">分辨率匹配方案的标识名称</param>
        /// <returns></returns>
        public Anchor_Layout hm_ScreenElement_Get_RMS_Anchored(XHud_Module_Element element, string solutionName)
        {
            Anchor_Layout layout = null;
            for (int i = 0; i < element.rms_layout_datas.Count; i++)
            {
                if (element.rms_layout_datas[i].layout_name == solutionName)
                {
                    layout = hm_ScreenElement_Matched_AnchoredType(element.rms_layout_datas[i].anchor);
                }
            }
            return layout;
        }
        /// <summary>
        /// XHud - 管理器通知 - 匹配锚点类型
        /// </summary>
        /// <param name="type">锚点类型</param>
        /// <returns>返回一个锚点布局</returns>
        public Anchor_Layout hm_ScreenElement_Matched_AnchoredType(XHudAnchor type)
        {
            for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
            {
                if (cv_anchors_layout_screen[i].Type == type)
                {
                    return cv_anchors_layout_screen[i];
                }
            }
            return null;
        }
        /// <summary>
        /// XHud - 管理器通知 - 创建的元素的初始化设置 - 屏幕模式 (依据元素自身设计布局信息)
        /// </summary>
        /// <param name="element">目标元素</param>
        /// <param name="alpha">透明度_Alpha</param>
        /// <param name="offset">位置偏移</param>
        /// <param name="rms_name">RMS 方案名称</param>
        /// <param name="DontCreateID">是否为自身生成随机ID</param>
        public void hm_ScreenElement_Initialize_By_RMS(XHud_Module_Element element, string rms_name, bool DontCreateID = false)
        {
            bool IsExist = false;
            for (int i = 0; i < element.rms_layout_datas.Count; i++)
            {
                if (element.rms_layout_datas[i].layout_name == rms_name)
                {
                    Element_RMS_LayoutData info = element.rms_layout_datas[i];
                    element.rect_transform.SetParent(hm_ScreenElement_GetAnchored_RectTransform(info.anchor));
                    element.element_AnchorRangeSet(info.anchor_min, info.anchor_max);
                    element.element_PivotSet(info.pivot);
                    element.element_PositionSet(info.position);
                    element.element_RotationSet(info.euler);
                    element.element_ScaleSet(info.scale);

                    if (!DontCreateID)
                        element.ID = element.element_CreateID(hm_ScreenElement_CollectAllElements());
                    if (use_debug)
                        XGUI_Utilitys.Console("XHud - 管理器通知", "已将元素生成到指定设计布局！", XGUIMsgState.通知);
                    IsExist = true;
                    break;
                }
                else
                {
                    continue;
                }
            }
            if (!IsExist)
            {
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", "未找到指定标识名称的设计布局，请检查该元素是否有记录设计布局信息！", XGUIMsgState.错误);
            }
        }
        /// <summary>
        /// XHud - 管理器通知 - 创建的元素的初始化设置 - 屏幕模式
        /// </summary>
        /// <param name="element"></param>
        /// <param name="anchor"></param>
        public void hm_ScreenElement_Initialize_By_MotionArgs(XHud_Module_Element element, XHudAnchor anchor)
        {
            // 放置到目标锚点下
            element.rect_transform.SetParent(hm_ScreenElement_GetAnchored_RectTransform(anchor));
            // 位置归零
            element.element_PositionResetZero();
            // 旋转归零
            element.element_RotationResetZero();
            // 为元素分配ID
            element.ID = element.element_CreateID(hm_ScreenElement_CollectAllElements());
        }
        /// <summary>
        /// XHud - 管理器通知 - 预存储到锚点列表 - 屏幕
        /// </summary>
        /// <param name="anchor_struct">锚点根节点</param>
        /// <param name="element">元素</param>
        /// <param name="module_name">模块名称</param>
        /// <param name="indicator_name">标识名称</param>
        /// <returns></returns>
        public XHudElementNode hm_ScreenElement_Send_To_AnchoredList(Anchor_Layout anchor_struct, XHud_Module_Element element, string module_name, string indicator_name)
        {
            if (anchor_struct.HudElementInfos == null)
                anchor_struct.HudElementInfos = new List<XHudElementNode>();

            XHudElementNode item = new XHudElementNode();
            item.ModuleName = module_name;
            if (string.IsNullOrEmpty(indicator_name))
            {
                item.Indicator = "indicator_" + module_name;
            }
            item.Indicator = indicator_name;
            item.ID = element.ID;
            item.Element = element;

            anchor_struct.HudElementInfos.Add(item);
            return item;
        }
        /// <summary>
        /// XHud - 管理器通知 - 收集所有生成的HudElementItem元素 - 屏幕
        /// </summary>
        /// <returns></returns>
        private XHudElementNode[] hm_ScreenElement_CollectAllElements()
        {
            List<XHudElementNode> items = new List<XHudElementNode>();

            for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
            {
                for (int s = 0; s < cv_anchors_layout_screen[i].HudElementInfos.Count; s++)
                {
                    items.Add(cv_anchors_layout_screen[i].HudElementInfos[s]);
                }
            }
            return items.ToArray();
        }
        /// <summary>
        /// XHud - 管理器通知 - 清理已存在的Element项 - 屏幕
        /// </summary>
        /// <param name="element">目标元素</param>
        public void hm_ScreenElement_Remove_From_ScreenAnchored(XHud_Module_Element element)
        {
            for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
            {
                for (int s = 0; s < cv_anchors_layout_screen[i].HudElementInfos.Count; s++)
                {
                    if (element.ID == cv_anchors_layout_screen[i].HudElementInfos[s].ID && element.indicator == cv_anchors_layout_screen[i].HudElementInfos[s].Indicator)
                    {
                        cv_anchors_layout_screen[i].HudElementInfos.RemoveAt(s);
                    }
                }
            }
        }
        #endregion

        #region 屏幕空间 - 生成元素 - 高效链式方法
        /// <summary>
        /// XHud - 管理器通知 - 创建一个XHud元素 - 屏幕空间
        /// </summary>
        /// <param name="element">目标 element 预制体</param>
        /// <returns>返回一个XHudElement节点元素体</returns>
        public XHudElementNode hm_ScreenElement_Create(XHud_Module_Element element)
        {
            // 目标元素
            if (element == null)
            {
                XGUI_Utilitys.Console("XHud - 元素生成通知", "您指定的元素为空！", XGUIMsgState.警告);
                return null;
            }

            // 标记元素生成来源为  -  实例化源，便于回收&销毁时的识别操作
            element.element_CreatedSourceTypeSet(XHudElementCreatedSourceType.Instantiate);

            // 从屏幕元素列表中清理目标元素
            hm_ScreenElement_Remove_From_ScreenAnchored(element);

            // 将生成的元素存入节点，便于后续链式调用
            XHudElementNode node = new XHudElementNode();
            node.Element = element;

            // 返回已生成的元素
            return node;
        }
        /// <summary>
        /// XHud - 管理器通知 - 创建一个XHud元素 - 屏幕空间
        /// </summary>
        /// <param name="libname">目标 element 预制体</param>
        /// <param name="modulename">目标 element 预制体</param>
        /// <returns>返回一个XHudElement节点元素体</returns>
        public XHudElementNode hm_ScreenElement_Create(string libname, string modulename)
        {
            // 从元素库中取出元素
            XHud_Module_Element element = hm_ElementLibrary_GetElement(libname, modulename);

            // 目标元素
            if (element == null)
            {
                XGUI_Utilitys.Console("XHud - 元素生成通知", "您指定的元素为空！", XGUIMsgState.警告);
                return null;
            }

            // 标记元素生成来源为  -  实例化源，便于回收&销毁时的识别操作
            element.element_CreatedSourceTypeSet(XHudElementCreatedSourceType.Library);

            // 从屏幕元素列表中清理目标元素
            hm_ScreenElement_Remove_From_ScreenAnchored(element);

            // 重要操作：清理元素！清理元素！清理元素！
            element.element_Reset(false, element.clear_events_with_spawn, element.clear_actions_with_spawn);

            // 将生成的元素存入节点，便于后续链式调用
            XHudElementNode node = new XHudElementNode();
            node.Element = element;

            // 返回已生成的元素
            return node;
        }
        #endregion

        //---------------------------------- 世界

        #region 创建辅助方法 - 世界元素
        /// <summary>
        /// XHud - 管理器通知 - 创建的元素的初始化设置 - 世界模式
        /// </summary>
        /// <param name="element">目标元素</param>
        /// <param name="size">尺寸</param>
        /// <param name="alpha">透明度_Alpha</param>
        /// <param name="offset">位置偏移</param>
        /// <param name="position">位置_Position</param>
        /// <param name="rotation">旋转_Rotation</param>
        public void hm_WorldElement_Initialize_For_World(XHud_Module_Element element)
        {
            element.rect_transform.SetParent(cv_hudcanvas_world_anchor);
            element.element_PositionResetZero();
            element.element_RotationResetZero();
            element.ID = element.element_CreateID(hm_WorldElement_CollectAllElements());
        }
        /// <summary>
        /// XHud - 管理器通知 - 预存储到锚点列表 - 世界
        /// </summary>
        /// <param name="anchor_struct">锚点根节点</param>
        /// <param name="element">元素</param>
        /// <param name="module_name">模块名称</param>
        /// <param name="indicator_name">标识名称</param>
        /// <returns></returns>
        public XHudElementNode hm_WorldElement_Send_To_AnchoredList(XHud_Module_Element element, string module_name, string indicator_name)
        {
            XHudElementNode item = new XHudElementNode();
            item.ModuleName = module_name;
            item.Indicator = indicator_name;
            item.ID = element.ID;
            item.Element = element;

            cv_anchors_layout_world.Add(item);

            return item;
        }
        /// <summary>
        /// XHud - 管理器通知 - 收集所有生成的HudElementItem元素 - 世界
        /// </summary>
        /// <param name="structs"></param>
        /// <returns></returns>
        private XHudElementNode[] hm_WorldElement_CollectAllElements()
        {
            List<XHudElementNode> items = new List<XHudElementNode>();

            for (int i = 0; i < cv_anchors_layout_world.Count; i++)
            {
                items.Add(cv_anchors_layout_world[i]);
            }
            return items.ToArray();
        }
        /// <summary>
        /// XHud - 管理器通知 - 清理已存在的Element项 - 世界
        /// </summary>
        /// <param name="element">目标元素</param>
        public void hm_WorldElement_Remove_From_WorldAnchored(XHud_Module_Element element)
        {
            for (int i = 0; i < cv_anchors_layout_world.Count; i++)
            {
                if (element.ID == cv_anchors_layout_world[i].ID && element.indicator == cv_anchors_layout_world[i].Indicator)
                {
                    cv_anchors_layout_world.RemoveAt(i);
                }
            }
        }
        #endregion

        #region 世界空间 - 生成元素 - 高效链式方法
        /// <summary>
        /// XHud - 管理器通知 - 创建一个XHud元素 - 世界空间
        /// </summary>
        /// <param name="element">目标 element 预制体</param>
        /// <returns>返回一个XHudElement节点元素体</returns>
        public XHudElementNode hm_WorldElement_Create(XHud_Module_Element element)
        {
            if (!cv_support_world_ui)
            {
                XGUI_Utilitys.Console("XHud - 元素生成通知", "你想要生成世界UI，但是XHud管理器并未开启世界模式支持！请检查该开关是否为开启状态！", XGUIMsgState.警告);
                return null;
            }

            // 目标元素
            if (element == null)
            {
                XGUI_Utilitys.Console("XHud - 元素生成通知", "您指定的元素为空！！", XGUIMsgState.警告);
                return null;
            }

            // 标记元素生成来源为  -  实例化源，便于回收&销毁时的识别操作
            element.element_CreatedSourceTypeSet(XHudElementCreatedSourceType.Instantiate);

            // 从屏幕元素列表中清理目标元素
            hm_WorldElement_Remove_From_WorldAnchored(element);

            // 将生成的元素存入节点，便于后续链式调用
            XHudElementNode node = new XHudElementNode();
            node.Element = element;

            // 返回已生成的元素
            return node;
        }
        /// <summary>
        /// XHud - 管理器通知 - 创建一个XHud元素 - 世界空间
        /// </summary>
        /// <param name="libname">目标 element 预制体</param>
        /// <param name="modulename">目标标识名称</param>    
        /// <returns>返回一个XHudElement节点元素体</returns>
        public XHudElementNode hm_WorldElement_Create(string libname, string modulename)
        {
            if (!cv_support_world_ui)
            {
                XGUI_Utilitys.Console("XHud - 元素生成通知", "你想要生成世界UI，但是XHud管理器并未开启世界模式支持！请检查该开关是否为开启状态！", XGUIMsgState.警告);
                return null;
            }

            // 从元素库中取出元素
            XHud_Module_Element element = hm_ElementLibrary_GetElement(libname, modulename);
            // 目标元素
            if (element == null)
            {
                XGUI_Utilitys.Console("XHud - 元素生成通知", "您指定的元素为空！！", XGUIMsgState.警告);
                return null;
            }

            // 标记元素生成来源为  -  实例化源，便于回收&销毁时的识别操作
            element.element_CreatedSourceTypeSet(XHudElementCreatedSourceType.Library);

            // 从屏幕元素列表中清理目标元素
            hm_WorldElement_Remove_From_WorldAnchored(element);

            // 重要操作：清理元素！清理元素！清理元素！
            element.element_Reset(false, element.clear_events_with_spawn, element.clear_actions_with_spawn);

            // 将生成的元素存入节点，便于后续链式调用
            XHudElementNode node = new XHudElementNode();
            node.Element = element;

            // 返回已生成的元素
            return node;
        }
        #endregion

        //---------------------------------- 回收

        #region 回收元素
        /// <summary>
        /// XHud - 管理器通知 - 清空所有生成的Hud元素
        /// </summary>
        /// <param name="args">回收参数</param>
        /// <param name="action_out_start">委托 - 回收时</param>
        /// <param name="action_out_progress">委托 - 回收中</param>
        /// <param name="action_out_end">委托 - 回收后</param>
        public void hm_HudElement_RecycleAll(Motion_Recycler args, UnityAction<XHud_Module_Element> action_out_start = null, UnityAction<float> action_out_progress = null, UnityAction<XHud_Module_Element> action_out_end = null)
        {
            // 设置默认回收参数
            if (args == null)
                args = global_motion_recycle_args;

            string x_indicator = "";
            int x_id = 0;

            // 回收所有屏幕元素
            for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
            {
                for (int s = 0; s < cv_anchors_layout_screen[i].HudElementInfos.Count; s++)
                {
                    XHudElementNode node = cv_anchors_layout_screen[i].HudElementInfos[s];
                    x_indicator = node.Indicator;
                    x_id = node.ID;

                    /*释放音效器对元素的In_Start  /  In_End  /  Out_Start  的委托，
                     * 如果不执行这步会导致 Element 的 act_on_element_out_start 委托总是被占用一个，
                     * 虽然回收后会清空
                     * -----------------------
                     * 原因：音效器会在元素从生出来的时候自动注册：
                     *   if (Element != null)
                     *    {
                     *        Element.act_on_element_in_start += ele_act_on_element_in_start;
                     *        Element.act_on_element_in_end += ele_act_on_element_in_end;
                     *        Element.act_on_element_out_start += ele_act_on_element_out_start;
                     *        if (DebugState)
                     *        {
                     *            XGUI_Utilitys.Func_PrintInfo("XHud - 音效器通知", "已注册 - 元素 - 事件", XGUIMsgState.通知);
                     *        }
                     *    }
                     */
                    // 但是会导致元素退出时无法播放音效了
                    //node.Element.element_SounderUnRegisterAction();

                    // 因为音效器的委托关系就不能再判断，否则如果元素上的音效器选定时机为元素退出时，就会不是空的
                    //if (node.Element.act_on_element_out_start == null)
                    node.Element.act_on_element_out_start += action_out_start;

                    //if (node.Element.act_on_element_out_progress == null)
                    node.Element.act_on_element_out_progress += action_out_progress;

                    //if (node.Element.act_on_element_out_end == null)
                    node.Element.act_on_element_out_end += action_out_end;

                    // 执行回收
                    if (args == null)
                        args = global_motion_recycle_args;
                    node.Element.Element_Out(args);

                    // 调试信息 - 成功
                    if (use_debug)
                        XGUI_Utilitys.Console("XHud - 管理器通知", $"已{(node.Element.created_source_type == XHudElementCreatedSourceType.Library ? "回收" : "销毁")}元素：" + x_indicator + " / " + x_id, XGUIMsgState.通知);
                }
            }

            // 回收所有世界元素
            for (int i = 0; i < cv_anchors_layout_world.Count; i++)
            {
                XHudElementNode node = cv_anchors_layout_world[i];
                x_indicator = node.Indicator;
                x_id = node.ID;

                /*释放音效器对元素的In_Start  /  In_End  /  Out_Start  的委托，
                 * 如果不执行这步会导致 Element 的 act_on_element_out_start 委托总是被占用一个，
                 * 虽然回收后会清空
                 * -----------------------
                 * 原因：音效器会在元素从生出来的时候自动注册：
                 *   if (Element != null)
                 *    {
                 *        Element.act_on_element_in_start += ele_act_on_element_in_start;
                 *        Element.act_on_element_in_end += ele_act_on_element_in_end;
                 *        Element.act_on_element_out_start += ele_act_on_element_out_start;
                 *        if (DebugState)
                 *        {
                 *            XGUI_Utilitys.Func_PrintInfo("XHud - 音效器通知", "已注册 - 元素 - 事件", XGUIMsgState.通知);
                 *        }
                 *    }
                 */
                // 但是会导致元素退出时无法播放音效了
                //node.Element.element_SounderUnRegisterAction();

                // 因为音效器的委托关系就不能再判断，否则如果元素上的音效器选定时机为元素退出时，就会不是空的
                //if (node.Element.act_on_element_out_start == null)
                node.Element.act_on_element_out_start += action_out_start;

                //if (node.Element.act_on_element_out_progress == null)
                node.Element.act_on_element_out_progress += action_out_progress;

                //if (node.Element.act_on_element_out_end == null)
                node.Element.act_on_element_out_end += action_out_end;

                // 执行回收
                if (args == null)
                    args = global_motion_recycle_args;
                node.Element.Element_Out(args);

                // 调试信息 - 成功
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", $"已{(node.Element.created_source_type == XHudElementCreatedSourceType.Library ? "回收" : "销毁")}元素：" + x_indicator + " / " + x_id, XGUIMsgState.通知);
            }
        }

        /// <summary>
        /// XHud - 管理器通知 - 通过元素回收
        /// </summary>
        /// <param name="element">目标元素</param>
        /// <param name="args">回收参数</param>
        /// <param name="action_out_start">委托 - 回收时</param>
        /// <param name="action_out_progress">委托 - 回收中</param>
        /// <param name="action_out_end">委托 - 回收后</param>
        public void hm_HudElement_RecycleAt(XHud_Module_Element element, Motion_Recycler args, UnityAction<XHud_Module_Element> action_out_start = null, UnityAction<float> action_out_progress = null, UnityAction<XHud_Module_Element> action_out_end = null)
        {
            bool finded = false;
            int x_id = 0;
            string x_indicator = "";
            XHudElementNode targetNode = null;

            // 在屏幕元素中查找
            for (int i = 0; i < cv_anchors_layout_screen.Count && !finded; i++)
            {
                for (int s = 0; s < cv_anchors_layout_screen[i].HudElementInfos.Count && !finded; s++)
                {
                    if (element.ID == cv_anchors_layout_screen[i].HudElementInfos[s].ID)
                    {
                        x_id = cv_anchors_layout_screen[i].HudElementInfos[s].ID;
                        x_indicator = cv_anchors_layout_screen[i].HudElementInfos[s].Indicator;
                        targetNode = cv_anchors_layout_screen[i].HudElementInfos[s];
                        finded = true;
                    }
                }
            }

            // 在世界元素中查找（仅在屏幕元素中未找到时）
            if (!finded)
            {
                for (int i = 0; i < cv_anchors_layout_world.Count && !finded; i++)
                {
                    if (element.ID == cv_anchors_layout_world[i].ID)
                    {
                        x_id = cv_anchors_layout_world[i].ID;
                        x_indicator = cv_anchors_layout_world[i].Indicator;
                        targetNode = cv_anchors_layout_world[i];
                        finded = true;
                    }
                }
            }

            // 执行回收逻辑
            if (finded)
            {
                /*释放音效器对元素的In_Start  /  In_End  /  Out_Start  的委托，
                 * 如果不执行这步会导致 Element 的 act_on_element_out_start 委托总是被占用一个，
                 * 虽然回收后会清空
                 * -----------------------
                 * 原因：音效器会在元素从生出来的时候自动注册：
                 *   if (Element != null)
                 *    {
                 *        Element.act_on_element_in_start += ele_act_on_element_in_start;
                 *        Element.act_on_element_in_end += ele_act_on_element_in_end;
                 *        Element.act_on_element_out_start += ele_act_on_element_out_start;
                 *        if (DebugState)
                 *        {
                 *            XGUI_Utilitys.Func_PrintInfo("XHud - 音效器通知", "已注册 - 元素 - 事件", XGUIMsgState.通知);
                 *        }
                 *    }
                 */
                // 但是会导致元素退出时无法播放音效了
                //targetNode.Element.element_SounderUnRegisterAction();

                // 因为音效器的委托关系就不能再判断，否则如果元素上的音效器选定时机为元素退出时，就会不是空的
                //if (targetNode.Element.act_on_element_out_start == null)
                targetNode.Element.act_on_element_out_start += action_out_start;

                //if (targetNode.Element.act_on_element_out_progress == null)
                targetNode.Element.act_on_element_out_progress += action_out_progress;

                //if (targetNode.Element.act_on_element_out_end == null)
                targetNode.Element.act_on_element_out_end += action_out_end;

                // 执行回收
                if (args == null)
                    args = global_motion_recycle_args;
                targetNode.Element.Element_Out(args);

                // 调试信息 - 成功
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", $"已{(element.created_source_type == XHudElementCreatedSourceType.Library ? "回收" : "销毁")}元素：" + x_indicator + " / " + x_id, XGUIMsgState.通知);
            }
            else
            {
                // 调试信息 - 未找到
                if (use_debug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", "XHud元素：" + $"{(string.IsNullOrEmpty(element.indicator) ? element.gameObject.name : element.indicator)}" + $" 不存在！未找到要{(element.created_source_type == XHudElementCreatedSourceType.Library ? "回收" : "销毁")}的目标元素！", XGUIMsgState.通知);
            }
        }

        /// <summary>
        /// XHud - 管理器通知 - 通过ID回收
        /// </summary>
        /// <param name="id">目标元素ID</param>
        /// <param name="args">回收参数</param>
        /// <param name="action_out_start">委托 - 回收时</param>
        /// <param name="action_out_progress">委托 - 回收中</param>
        /// <param name="action_out_end">委托 - 回收后</param>
        public void hm_HudElement_RecycleAt(int id, Motion_Recycler args = null, UnityAction<XHud_Module_Element> action_out_start = null, UnityAction<float> action_out_progress = null, UnityAction<XHud_Module_Element> action_out_end = null)
        {
            XHud_Module_Element element = null;

            // 在屏幕元素中查找
            for (int i = 0; i < cv_anchors_layout_screen.Count && element == null; i++)
            {
                for (int s = 0; s < cv_anchors_layout_screen[i].HudElementInfos.Count && element == null; s++)
                {
                    if (id == cv_anchors_layout_screen[i].HudElementInfos[s].ID)
                    {
                        element = cv_anchors_layout_screen[i].HudElementInfos[s].Element;
                    }
                }
            }

            // 在世界元素中查找
            for (int i = 0; i < cv_anchors_layout_world.Count && element == null; i++)
            {
                if (id == cv_anchors_layout_world[i].ID)
                {
                    element = cv_anchors_layout_world[i].Element;
                }
            }

            // 找到则调用元素回收方法，否则输出调试信息
            if (element != null)
            {
                hm_HudElement_RecycleAt(element, args, action_out_start, action_out_progress, action_out_end);
            }
            else if (use_debug)
            {
                XGUI_Utilitys.Console("XHud - 管理器通知", $"XHud元素ID：{id} 不存在！", XGUIMsgState.通知);
            }
        }
        #endregion

        #region 获取
        /// <summary>
        /// XHud - 管理器通知 - 根据目标ID从锚点列表中获取生成的Hud元素
        /// </summary>
        /// <param name="id">目标ID的元素</param>
        /// <returns>根据目标ID获取的元素</returns>
        public XHud_Module_Element hm_HudElement_Get(int id, XHudSpace space)
        {
            XHud_Module_Element element = null;
            if (space == XHudSpace.屏幕空间)
            {
                for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
                {
                    for (int s = 0; s < cv_anchors_layout_screen[i].HudElementInfos.Count; s++)
                    {
                        if (id == cv_anchors_layout_screen[i].HudElementInfos[s].ID)
                        {
                            element = cv_anchors_layout_screen[i].HudElementInfos[s].Element;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < cv_anchors_layout_world.Count; i++)
                {
                    if (id == cv_anchors_layout_world[i].Element.ID)
                    {
                        element = cv_anchors_layout_world[i].Element;
                    }
                }
            }
            return element;
        }
        /// <summary>
        ///  XHud - 管理器通知 - 根据目标名称从锚点列表中获取生成的Hud元素
        /// </summary>
        /// <param name="name"></param>
        /// <returns>根据目标名称获取的元素</returns>
        public XHud_Module_Element hm_HudElement_Get(string name, XHudSpace space)
        {
            XHud_Module_Element element = null;
            if (space == XHudSpace.屏幕空间)
            {
                for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
                {
                    for (int s = 0; s < cv_anchors_layout_screen[i].HudElementInfos.Count; s++)
                    {
                        if (name == cv_anchors_layout_screen[i].HudElementInfos[s].Indicator)
                        {
                            element = cv_anchors_layout_screen[i].HudElementInfos[s].Element;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < cv_anchors_layout_world.Count; i++)
                {
                    if (name == cv_anchors_layout_world[i].Element.indicator)
                    {
                        element = cv_anchors_layout_world[i].Element;
                    }
                }
            }
            return element;
        }
        /// <summary>
        ///  XHud - 管理器通知 - 从锚点列表中获取所有已生成的Hud元素
        /// </summary>
        /// <returns>所有已生成到锚点里的Hud元素</returns>
        public XHud_Module_Element[] hm_HudElement_GetAll(XHudSpace space)
        {
            List<XHud_Module_Element> list = new List<XHud_Module_Element>();
            if (space == XHudSpace.屏幕空间)
            {
                for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
                {
                    for (int s = 0; s < cv_anchors_layout_screen[i].HudElementInfos.Count; s++)
                    {
                        list.Add(cv_anchors_layout_screen[i].HudElementInfos[s].Element);
                    }
                }
            }
            else
            {
                for (int i = 0; i < cv_anchors_layout_world.Count; i++)
                {
                    list.Add(cv_anchors_layout_world[i].Element);
                }
            }
            return list.ToArray();
        }
        /// <summary>
        /// XHud - 管理器通知 - 获取已生成到锚点里的总Element数量
        /// </summary>
        /// <returns></returns>
        public int hm_HudElement_TotalCount(XHudSpace space)
        {
            int x = 0;
            if (space == XHudSpace.屏幕空间)
            {
                for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
                {
                    for (int s = 0; s < cv_anchors_layout_screen[i].HudElementInfos.Count; s++)
                    {
                        if (cv_anchors_layout_screen[i].HudElementInfos[s].Element != null)
                            x++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < cv_anchors_layout_world.Count; i++)
                {
                    if (cv_anchors_layout_world[i].Element != null)
                        x++;
                }
            }
            return x;
        }
        /// <summary>
        /// 主要在编辑器模式下用于获取所有锚点下的物体进行集控
        /// </summary>
        /// <returns></returns>
        public GameObject[] hm_AnchorChildsGet(XHudSpace space)
        {
            List<GameObject> list = new List<GameObject>();

            XHud_Manager mgr = FindFirstObjectByType<XHud_Manager>();

            if (space == XHudSpace.屏幕空间)
            {
                for (int i = 0; i < mgr.cv_hudcanvas_screen_anchor.childCount; i++)
                {
                    Transform trans = mgr.cv_hudcanvas_screen_anchor.GetChild(i);
                    if (trans.childCount > 0)
                    {
                        for (int s = 0; s < trans.childCount; s++)
                        {
                            GameObject obj = trans.GetChild(s).gameObject;
                            if (obj.hideFlags != HideFlags.HideInHierarchy)
                                list.Add(obj);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < mgr.cv_hudcanvas_world_anchor.childCount; i++)
                {
                    Transform trans = mgr.cv_hudcanvas_world_anchor.GetChild(i);
                    if (trans.childCount > 0)
                    {
                        for (int s = 0; s < trans.childCount; s++)
                        {
                            GameObject obj = trans.GetChild(s).gameObject;
                            if (obj.hideFlags != HideFlags.HideInHierarchy)
                                list.Add(obj);
                        }
                    }
                }
            }
            return list.ToArray();
        }
        /// <summary>
        /// 主要在编辑器模式下用于获取所有锚点下的物体进行集控
        /// </summary>
        /// <returns></returns>
        public XHud_Module_Element[] hm_AnchorElementsGet()
        {
            XHud_Manager mgr = FindFirstObjectByType<XHud_Manager>();
            XHud_Module_Element[] trans = mgr.cv_hudcanvas_screen_anchor.GetComponentsInChildren<XHud_Module_Element>();
            return trans;
        }
        public RectTransform hm_GetAnchorRoot(XHudSpace Space)
        {
            if (Space == XHudSpace.屏幕空间)
                return cv_hudcanvas_screen_anchor;
            else
                return cv_hudcanvas_world_anchor;
        }
        #endregion
    }
}