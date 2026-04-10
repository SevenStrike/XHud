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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
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
                Element.RMS_Name = rmsName;

            if (enabled && XHud_Manager.Instance.RMS_Enabled)
            {
                Anchor_Layout anchor_struct = null;

                // 如果RMS名称为空，则使用当前的RMS指定的布局锚点
                if (string.IsNullOrEmpty(rmsName))
                    anchor_struct = XHud_Manager.Instance.hm_ScreenElement_Get_RMS_Anchored(Element, XHud_Manager.Instance.RMS_CurrentSolution);
                else
                    anchor_struct = XHud_Manager.Instance.hm_ScreenElement_Get_RMS_Anchored(Element, rmsName);

                // 如果RMS名称为空，那么就参考 XHud 管理器当前选中的方案作为元素的RMS方案
                if (string.IsNullOrEmpty(rmsName))
                    XHud_Manager.Instance.hm_ScreenElement_Initialize_By_RMS(Element, XHud_Manager.Instance.RMS_CurrentSolution);
                else
                    XHud_Manager.Instance.hm_ScreenElement_Initialize_By_RMS(Element, rmsName);
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

            anchor_struct = XHud_Manager.Instance.hm_ScreenElement_Matched_AnchoredType(anchor);
            XHud_Manager.Instance.hm_ScreenElement_Initialize_By_MotionArgs(Element, anchor_struct.Type);

            Element.CreateState = XHudElementCreateState.Created;

            Element.element_ScaleSet(Vector3.one);
            Element.element_SizeSet(Vector2.one * 100);

            // 将元素放置到目标屏幕锚点物体下
            XHudElementNode node = XHud_Manager.Instance.hm_ScreenElement_Send_To_AnchoredList(anchor_struct, Element, string.IsNullOrEmpty(Element.Indicator) ? Element.gameObject.name : Element.Indicator, anchor_indicator);

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

            anchor_struct = XHud_Manager.Instance.hm_ScreenElement_Matched_AnchoredType(anchor);
            XHud_Manager.Instance.hm_ScreenElement_Initialize_By_MotionArgs(Element, anchor_struct.Type);

            Element.CreateState = XHudElementCreateState.Created;

            Element.element_ScaleSet(scale);
            Element.element_SizeSet(size);

            // 将元素放置到目标屏幕锚点物体下
            XHudElementNode node = XHud_Manager.Instance.hm_ScreenElement_Send_To_AnchoredList(anchor_struct, Element, string.IsNullOrEmpty(Element.Indicator) ? Element.gameObject.name : Element.Indicator, anchor_indicator);

            if (AutoIn)
                node.Element_In(creator);

            return node;
        }
        /// <summary>
        /// 设置并放置到世界下
        /// </summary>
        public XHudElementNode SetAnchored_World(string indicator, bool AutoIn = false, Motion_Creator creator = null)
        {
            XHud_Manager.Instance.hm_WorldElement_Initialize_For_World(Element);

            Element.CreateState = XHudElementCreateState.Created;

            Element.element_ScaleSet(Vector3.one);
            Element.element_SizeSet(Vector2.one * 100);

            // 将元素放置到目标屏幕锚点物体下
            XHudElementNode node = XHud_Manager.Instance.hm_WorldElement_Send_To_AnchoredList(Element, string.IsNullOrEmpty(Element.Indicator) ? Element.gameObject.name : Element.Indicator, indicator);

            if (AutoIn)
                node.Element_In(creator);

            return node;
        }
        /// <summary>
        /// 设置并放置到世界下
        /// </summary>
        public XHudElementNode SetAnchored_World(string indicator, Vector2 size, Vector3 scale, bool AutoIn = false, Motion_Creator creator = null)
        {
            XHud_Manager.Instance.hm_WorldElement_Initialize_For_World(Element);

            Element.CreateState = XHudElementCreateState.Created;

            Element.element_ScaleSet(scale);
            Element.element_SizeSet(size);

            // 将元素放置到目标屏幕锚点物体下
            XHudElementNode node = XHud_Manager.Instance.hm_WorldElement_Send_To_AnchoredList(Element, string.IsNullOrEmpty(Element.Indicator) ? Element.gameObject.name : Element.Indicator, indicator);

            if (AutoIn)
                node.Element_In(creator);

            return node;
        }
        /// <summary>
        /// 设置将子级中所有图元动画倒退
        /// </summary>
        public XHudElementNode SetPrimitivesTween(XHudPrimitiveTweenMode mode, bool AutoIn = false, Motion_Creator creator = null)
        {
            switch (mode)
            {
                case XHudPrimitiveTweenMode.倒退:
                    //Element.Animators_Rewind();
                    break;
                case XHudPrimitiveTweenMode.播放:
                    //Element.Animators_Play();
                    break;
                case XHudPrimitiveTweenMode.杀死:
                    //Element.Animators_Kill();
                    break;
                default:
                    break;
            }

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 设置入场动画开始回调
        /// </summary>
        public XHudElementNode On_In_Start(UnityAction<XHud_Module_Element> action, bool AutoIn = false, Motion_Creator creator = null)
        {
            Element.act_on_element_in_start += action;

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

            if (AutoIn)
                Element_In(creator);

            return this;
        }
        /// <summary>
        /// 手动播放入场动画（当 autoin = false 时使用）
        /// </summary>
        public XHudElementNode Element_In(Motion_Creator args = null)
        {
            Element.Element_In(args ?? XHud_Manager.Instance.CreateArgs_Default);
            return this;
        }
        /// <summary>
        /// 手动播放出场动画
        /// </summary>
        public XHudElementNode Element_Out(Motion_Recycler args = null)
        {
            Element.Element_Out(args ?? XHud_Manager.Instance.RecycleArgs_Default);
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
        [Tooltip("此元素库用于配置存放场景中需要调用的元素预制体")]
        /// <summary>
        /// 元素库 - 配置化（Editor期间）
        /// </summary>
        public List<XHud_Library_Element> Hud_ElementLibrarys = new List<XHud_Library_Element>();
        [Tooltip("此参数用于元素再生成时的默认动效效果")]
        /// <summary>
        /// 默认参数 - 元素生成
        /// </summary>
        public Motion_Creator CreateArgs_Default = new Motion_Creator
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
        [Tooltip("此参数用于元素再回收时的默认动效效果")]
        /// <summary>
        /// 默认参数 - 元素回收
        /// </summary>
        public Motion_Recycler RecycleArgs_Default = new Motion_Recycler
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
        public string Crc_Lib_Name;
        /// <summary>
        /// 用于编辑器内元素回收模板下拉菜单的选择索引名称
        /// </summary>
        public string Rec_Lib_Name;

        #region 元素库相关操作
        /// <summary>
        /// 获取所有元素库
        /// </summary>
        /// <returns></returns>
        public XHud_Library_Element[] hm_ElementLibrary_GetArray()
        {
            return Hud_ElementLibrarys.ToArray();
        }
        /// <summary>
        /// 获取所有元素库
        /// </summary>
        /// <returns></returns>
        public List<XHud_Library_Element> hm_ElementLibrary_GetList()
        {
            return Hud_ElementLibrarys;
        }
        /// <summary>
        /// 获取所有元素库
        /// </summary>
        /// <returns></returns>
        public string[] hm_ElementLibrary_GetAllLibraryNames()
        {
            string[] names = new string[Hud_ElementLibrarys.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = Hud_ElementLibrarys[i].LibraryName;
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
                    lib = Hud_ElementLibrarys[i];
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
            if (Hud_ElementLibrarys != null && Hud_ElementLibrarys.Count > 0)
                res_lib = Hud_ElementLibrarys[0];
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
            if (Hud_ElementLibrarys == null || Hud_ElementLibrarys.Count <= 0)
                return;

            #region 更新元素是否正在在被使用的状态
            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                XHud_Library_Element lib = Hud_ElementLibrarys[i];
                for (int s = 0; s < lib.ElementLibrary.Count; s++)
                {
                    XHud_LibraryArg_Element_Item item = lib.ElementLibrary[s];
                    if (item.PreloadElements != null && item.PreloadElements.Count > 0)
                    {
                        for (int v = 0; v < item.PreloadElements.Count; v++)
                        {
                            item.PreloadElements[v].UpdateState();
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
            if (Hud_ElementLibrarys.Count <= 0)
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

            for (int v = 0; v < Hud_ElementLibrarys.Count; v++)
            {
                XHud_Library_Element Lib = Hud_ElementLibrarys[v];

                ///---创建库容器
                GameObject lib_obj = new GameObject();
                lib_obj.name = "Library -> ( " + Lib.LibraryName + " )";
                lib_obj.layer = LayerMask.NameToLayer("XHud");
                lib_obj.transform.SetParent(PoolRoot.transform);
                lib_obj.transform.localPosition = Vector3.zero;
                lib_obj.transform.localEulerAngles = Vector3.zero;
                lib_obj.transform.localScale = Vector3.one;

                Lib.LibraryRoot = lib_obj.transform;

                for (int i = 0; i < Lib.ElementLibrary.Count; i++)
                {
                    XHud_LibraryArg_Element_Item item = Lib.ElementLibrary[i];

                    GameObject root = new GameObject();
                    root.name = "Category - " + item.Target.name;
                    root.layer = LayerMask.NameToLayer("XHud");
                    root.transform.SetParent(lib_obj.transform);
                    root.transform.localPosition = Vector3.zero;
                    root.transform.localEulerAngles = Vector3.zero;
                    root.transform.localScale = Vector3.one;
                    item.Root = root.transform;

                    for (int s = 0; s < item.InitializeCount; s++)
                    {
                        XHud_Module_Element element = Instantiate(item.Target, Vector3.zero, Quaternion.identity, item.Root);
                        ///---保存原始名称
                        element.OriginalName = element.transform.name.Substring(0, element.transform.name.Length - 7);
                        ///---改名
                        element.transform.name = element.OriginalName + "_Clone_" + s;
                        ///---赋值源库名
                        element.OriginPoolName = Lib.LibraryName;


                        XHud_LibraryArg_Element_Info pw = new XHud_LibraryArg_Element_Info();
                        pw.HudElement = element;
                        pw.HudElement.CreateState = XHudElementCreateState.Recycled;
                        pw.HudElement.element_Reset();
                        pw.HudElement.gameObject.SetActive(false);
                        item.PreloadElements.Add(pw);

                    }
                }
            }
            if (Act_ElementsLib_Instantiated != null)
                Act_ElementsLib_Instantiated();
        }

        /// <summary>
        /// 回收一个元素到元素库
        /// </summary>
        /// <param name="Element">目标元素</param>
        public void hm_ElementLibrary_Despawn(XHud_Module_Element Element)
        {
            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                XHud_Library_Element lib = Hud_ElementLibrarys[i];

                for (int m = 0; m < lib.ElementLibrary.Count; m++)
                {
                    XHud_LibraryArg_Element_Item item = lib.ElementLibrary[m];

                    if (Element.OriginalName == item.Name)
                    {
                        Element.transform.SetParent(item.Root);
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
            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                Hud_ElementLibrarys[i].ElementLibrary_RecycleAll();
            }
        }
        /// <summary>
        /// 获取元素库的数量
        /// </summary>
        /// <returns></returns>
        public int hm_ElementLibrary_GetCount()
        {
            return Hud_ElementLibrarys.Count;
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
            for (int v = 0; v < Hud_ElementLibrarys.Count; v++)
            {
                XHud_Library_Element lib = Hud_ElementLibrarys[v];
                #region 清理预生成元素及其使用信息痕迹，主要是为了弥补ScriptableObject在运行时也能被赋值
                lib.LibraryRoot = null;
                for (int i = 0; i < lib.ElementLibrary.Count; i++)
                {
                    lib.ElementLibrary[i].Root = null;
                    lib.ElementLibrary[i].UsedCount = 0;
                    lib.ElementLibrary[i].RecycledCount = 0;
                    lib.ElementLibrary[i].NextIndex = 0;
                    lib.ElementLibrary[i].Root = null;

                    if (lib.ElementLibrary[i].PreloadElements == null)
                        lib.ElementLibrary[i].PreloadElements = new List<XHud_LibraryArg_Element_Info>();

                    lib.ElementLibrary[i].PreloadElements.Clear();
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

            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                // 如果指定了库名且不匹配，跳过
                if (LibraryName != null && LibraryName != Hud_ElementLibrarys[i].LibraryName)
                    continue;

                XHud_Library_Element lib = Hud_ElementLibrarys[i];

                for (int k = 0; k < lib.ElementLibrary.Count; k++)
                {
                    XHud_LibraryArg_Element_Item item = lib.ElementLibrary[k];

                    if (item.Target.name == ElementName)
                    {
                        int index = item.NextIndex;

                        if (item.PreloadElements != null && item.PreloadElements.Count > 0)
                        {
                            if (item.PreloadElements[index] != null)
                            {
                                element = item.PreloadElements[index].HudElement;
                                element.gameObject.SetActive(true);

                                // 更新索引
                                if (item.NextIndex >= item.InitializeCount - 1)
                                    item.NextIndex = 0;
                                else
                                    item.NextIndex++;
                            }

                            if (Act_SpawnElement != null)
                                Act_SpawnElement(element);
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
            item.Target = Element;
            item.Name = string.IsNullOrEmpty(Element.transform.name) ? "NewElement" : Element.transform.name;
            item.InitializeCount = InitialCount;

            if (item.PreloadElements == null)
                item.PreloadElements = new List<XHud_LibraryArg_Element_Info>();

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
            lib.LibraryName = LibraryName;
            for (int i = 0; i < Items.Length; i++)
            {
                if (lib.ElementLibrary == null)
                    lib.ElementLibrary = new List<XHud_LibraryArg_Element_Item>();

                lib.ElementLibrary.Add(Items[i]);
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
            lib_obj.name = "Category -> ( " + Library.LibraryName + " )";
            lib_obj.transform.SetParent(PoolRoot.transform);
            lib_obj.transform.localPosition = Vector3.zero;
            lib_obj.transform.localEulerAngles = Vector3.zero;
            lib_obj.transform.localScale = Vector3.one;
            Library.LibraryRoot = lib_obj.transform;

            List<XHud_LibraryArg_Element_Item> items = Library.ElementLibrary;

            for (int i = 0; i < items.Count; i++)
            {
                GameObject root = new GameObject();
                root.name = "Case - " + items[i].Target.name;
                root.transform.SetParent(lib_obj.transform);
                root.transform.localPosition = Vector3.zero;
                root.transform.localEulerAngles = Vector3.zero;
                root.transform.localScale = Vector3.one;
                items[i].Root = root.transform;

                for (int s = 0; s < items[i].InitializeCount; s++)
                {
                    XHud_Module_Element element = Instantiate(items[i].Target, Vector3.zero, Quaternion.identity, items[i].Root);
                    ///---保存原始名称
                    element.OriginalName = element.transform.name.Substring(0, element.transform.name.Length - 7);
                    ///---改名
                    element.transform.name = element.OriginalName + "_Clone_" + s;

                    XHud_LibraryArg_Element_Info pw = new XHud_LibraryArg_Element_Info();
                    pw.HudElement = element;
                    pw.HudElement.CreateState = XHudElementCreateState.Recycled;
                    pw.HudElement.element_Reset();
                    pw.HudElement.gameObject.SetActive(false);

                    if (items[i].PreloadElements == null)
                        items[i].PreloadElements = new List<XHud_LibraryArg_Element_Info>();

                    items[i].PreloadElements.Add(pw);
                }
            }

            Hud_ElementLibrarys.Add(Library);

            if (Act_ElementsLib_Instantiated != null)
                Act_ElementsLib_Instantiated();
        }
        /// <summary>
        /// 删除已经初始化的元素库
        /// </summary>
        /// <param name="LibName">目标元素库</param>
        public void hm_ElementLibrary_Destroyed(string LibName)
        {
            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                XHud_Library_Element lib = Hud_ElementLibrarys[i];
                if (lib.LibraryName == LibName)
                {
                    lib.ElementLibrary_RecycleAll();
                    DestroyImmediate(lib.LibraryRoot.gameObject, true);
                    Hud_ElementLibrarys.RemoveAt(i);
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
            element.RectTransform.SetParent(anchor);
        }
        /// <summary>
        /// 检测是否存在已生成的Hud元素
        /// </summary>
        /// <returns>返回True则当前XHud - 管理器通知中已生成了Hud元素反之则说明已清空</returns>
        public bool hm_HasElements()
        {
            bool state = false;
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                if (Anchors_Layout_Screen[i].HudElementInfos != null && Anchors_Layout_Screen[i].HudElementInfos.Count > 0)
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
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                if (Anchors_Layout_Screen[i].HudElementInfos != null)
                {
                    for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                    {
                        if (Anchors_Layout_Screen[i].HudElementInfos[s] != null)
                        {
                            Anchors_Layout_Screen[i].HudElementInfos[s].IsAnimating = Anchors_Layout_Screen[i].HudElementInfos[s].Element.Animating;
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
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                Anchor_Layout lay = Anchors_Layout_Screen[i];
                for (int s = 0; s < lay.HudElementInfos.Count; s++)
                {
                    statistic.count_elements++;

                    XHudElementNode node = lay.HudElementInfos[s];
                    statistic.count_primitives += node.Element.PrimitiveControllerNodes.Count;
                    statistic.count_sounders += node.Element.SounderNodes.Count;
                    statistic.count_buttons += node.Element.ButtonNodes.Count;
                    statistic.count_options += node.Element.OptionNodes.Count;
                    statistic.count_texts += node.Element.TextNodes.Count;
                    statistic.count_tmptexts += node.Element.TmpTextNodes.Count;
                    statistic.count_sliders += node.Element.SliderNodes.Count;
                    statistic.count_progresses += node.Element.ProgressNodes.Count;
                    statistic.count_toggles += node.Element.ToggleNodes.Count;
                }
            }

            //统计世界UI组件数
            for (int i = 0; i < Anchors_Layout_World.Count; i++)
            {
                statistic.count_elements++;

                XHudElementNode node = Anchors_Layout_World[i];
                statistic.count_primitives += node.Element.PrimitiveControllerNodes.Count;
                statistic.count_sounders += node.Element.SounderNodes.Count;
                statistic.count_buttons += node.Element.ButtonNodes.Count;
                statistic.count_options += node.Element.OptionNodes.Count;
                statistic.count_texts += node.Element.TextNodes.Count;
                statistic.count_tmptexts += node.Element.TmpTextNodes.Count;
                statistic.count_sliders += node.Element.SliderNodes.Count;
                statistic.count_progresses += node.Element.ProgressNodes.Count;
                statistic.count_toggles += node.Element.ToggleNodes.Count;
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
                for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
                {
                    Anchor_Layout lay = Anchors_Layout_Screen[i];
                    for (int s = 0; s < lay.HudElementInfos.Count; s++)
                    {
                        statistic.count_elements++;

                        XHudElementNode node = lay.HudElementInfos[s];
                        statistic.count_primitives += node.Element.PrimitiveControllerNodes.Count;
                        statistic.count_sounders += node.Element.SounderNodes.Count;
                        statistic.count_buttons += node.Element.ButtonNodes.Count;
                        statistic.count_options += node.Element.OptionNodes.Count;
                        statistic.count_texts += node.Element.TextNodes.Count;
                        statistic.count_tmptexts += node.Element.TmpTextNodes.Count;
                        statistic.count_sliders += node.Element.SliderNodes.Count;
                        statistic.count_progresses += node.Element.ProgressNodes.Count;
                        statistic.count_toggles += node.Element.ToggleNodes.Count;
                    }
                }
            }
            else
            {
                //统计世界UI组件数
                for (int i = 0; i < Anchors_Layout_World.Count; i++)
                {
                    statistic.count_elements++;

                    XHudElementNode node = Anchors_Layout_World[i];
                    statistic.count_primitives += node.Element.PrimitiveControllerNodes.Count;
                    statistic.count_sounders += node.Element.SounderNodes.Count;
                    statistic.count_buttons += node.Element.ButtonNodes.Count;
                    statistic.count_options += node.Element.OptionNodes.Count;
                    statistic.count_texts += node.Element.TextNodes.Count;
                    statistic.count_tmptexts += node.Element.TmpTextNodes.Count;
                    statistic.count_sliders += node.Element.SliderNodes.Count;
                    statistic.count_progresses += node.Element.ProgressNodes.Count;
                    statistic.count_toggles += node.Element.ToggleNodes.Count;
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
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                if (Anchors_Layout_Screen[i].Type == anchor)
                {
                    rect = Anchors_Layout_Screen[i].Anchor;
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
            for (int i = 0; i < element.RMS_LayoutDatas.Count; i++)
            {
                if (element.RMS_LayoutDatas[i].LayoutName == solutionName)
                {
                    layout = hm_ScreenElement_Matched_AnchoredType(element.RMS_LayoutDatas[i].Anchor);
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
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                if (Anchors_Layout_Screen[i].Type == type)
                {
                    return Anchors_Layout_Screen[i];
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
            for (int i = 0; i < element.RMS_LayoutDatas.Count; i++)
            {
                if (element.RMS_LayoutDatas[i].LayoutName == rms_name)
                {
                    Element_RMS_LayoutData info = element.RMS_LayoutDatas[i];
                    element.RectTransform.SetParent(hm_ScreenElement_GetAnchored_RectTransform(info.Anchor));
                    element.element_AnchorRangeSet(info.AnchorMin, info.AnchorMax);
                    element.element_PivotSet(info.Pivot);
                    element.element_PositionSet(info.Position);
                    element.element_RotationSet(info.Euler);
                    element.element_ScaleSet(info.Scale);

                    if (!DontCreateID)
                        element.ID = element.element_CreateID(hm_ScreenElement_CollectAllElements());
                    if (UseDebug)
                        XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "已将元素生成到指定设计布局！", HudMsgState.通知);
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
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "未找到指定标识名称的设计布局，请检查该元素是否有记录设计布局信息！", HudMsgState.错误);
            }
        }
        /// <summary>
        /// XHud - 管理器通知 - 创建的元素的初始化设置 - 屏幕模式
        /// </summary>
        /// <param name="element">目标元素</param>
        /// <param name="anchor">锚点类型</param>
        /// <param name="alpha">透明度_Alpha</param>
        /// <param name="offset">位置偏移</param>
        /// <param name="scale">缩放</param>
        /// <param name="size">尺寸</param>XHud - PSD Reconstruction 通知
        public void hm_ScreenElement_Initialize_By_MotionArgs(XHud_Module_Element element, XHudAnchor anchor)
        {
            element.RectTransform.SetParent(hm_ScreenElement_GetAnchored_RectTransform(anchor));
            element.element_PositionResetZero();
            element.element_RotationResetZero();
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

            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    items.Add(Anchors_Layout_Screen[i].HudElementInfos[s]);
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
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    if (element.ID == Anchors_Layout_Screen[i].HudElementInfos[s].ID && element.Indicator == Anchors_Layout_Screen[i].HudElementInfos[s].Indicator)
                    {
                        Anchors_Layout_Screen[i].HudElementInfos.RemoveAt(s);
                    }
                }
            }
        }
        #endregion

        #region 屏幕空间 - 生成元素 - 传统多参数方式
        /// <summary>
        /// XHud - 管理器通知 - 创建一个XHud元素 - 屏幕空间
        /// </summary>
        /// <param name="libname">目标库名称</param>
        /// <param name="indicator">从库中取出后的自定义名称（仅为调用者自己理解的自定义名称）</param>
        /// <param name="modulename">预存入元素库的目标名称</param>
        /// <param name="offset">元素偏移</param>
        /// <param name="scale">元素缩放</param>
        /// <param name="size">元素尺寸</param>
        /// <param name="rms">RMS系统是否开启？</param>
        /// <param name="rms_name">RMS系统方案名称</param>
        /// <param name="args_creator">元素入场动画参数</param>
        /// <param name="action_in_start">元素入场开始委托</param>
        /// <param name="action_in_progress">元素入场进度委托</param>
        /// <param name="action_in_end">元素入场结束委托</param>        
        /// <param name="action_out_start">元素退场前委托</param>
        /// <param name="action_out_progress">元素退场进度委托</param>
        /// <param name="action_out_end">元素退场后委托</param>     
        /// <param name="autoin">此值是个非常关键的开关，如果你为一个元素编写了一个自定义控制的脚本绑定在它身上，并希望生成出来的时候由您自己决定何时播放动画，那么此值必须为False</param>
        /// <returns>返回一个XHudElement节点元素体</returns>
        public XHudElementNode hm_ScreenElement_Create(
            string libname, string indicator, string modulename, Motion_Creator args_creator, bool autoin = true,
            Vector3 offset = default(Vector3), Vector3 scale = default(Vector3), Vector2 size = default,
            bool rms = false, string rms_name = "",
            UnityAction<XHud_Module_Element> action_in_start = null, UnityAction<float> action_in_progress = null,
            UnityAction<XHud_Module_Element> action_in_end = null, UnityAction<XHud_Module_Element> action_out_start = null,
            UnityAction<float> action_out_progress = null, UnityAction<XHud_Module_Element> action_out_end = null)
        {
            // 从元素库中取出元素
            XHud_Module_Element element = hm_ElementLibrary_GetElement(libname, modulename);
            if (element == null)
            {
                XHud_Utilitys.Func_PrintInfo("XHud - 元素生成通知", "您从元素库获取的目标元素为空！请检查该元素在元素库中的状态！", HudMsgState.警告);
                return null;
            }

            // 标记元素生成来源为  -  元素库源，便于回收&销毁时的识别操作
            element.element_CreatedSourceTypeSet(XHudElementCreatedSourceType.Library);

            // 是否开启RMS模式
            element.element_RMS_Mode_Enabled(rms);

            // 从屏幕元素列表中清理目标元素
            hm_ScreenElement_Remove_From_ScreenAnchored(element);

            #region 初始化元素到对应的目标锚点下
            // 检查动效参数是否为空，如果是空的就使用管理器提供的默认动效参数
            if (args_creator == null)
                args_creator = CreateArgs_Default;

            XHudElementNode node = null;
            Anchor_Layout anchor_struct = null;

            // 如果元素启用了RMS匹配布局，则依据元素自身设计布局参数来布局元素实例
            if (element.RMS_Enabled && RMS_Enabled)
            {
                // 如果RMS名称为空，则使用当前的RMS指定的布局锚点
                if (string.IsNullOrEmpty(rms_name))
                    anchor_struct = hm_ScreenElement_Get_RMS_Anchored(element, RMS_CurrentSolution);
                else
                    anchor_struct = hm_ScreenElement_Get_RMS_Anchored(element, rms_name);

                // 如果RMS名称为空，那么就参考 XHud 管理器当前选中的方案作为元素的RMS方案
                if (string.IsNullOrEmpty(rms_name))
                    hm_ScreenElement_Initialize_By_RMS(element, RMS_CurrentSolution);
                else
                    hm_ScreenElement_Initialize_By_RMS(element, rms_name);
            }
            // 否则如果元素未启用RMS匹配布局，则根据动效参数指定的锚点来布局元素实例
            else
            {
                #region 依据构造参数
                anchor_struct = hm_ScreenElement_Matched_AnchoredType(args_creator.anchor);
                hm_ScreenElement_Initialize_By_MotionArgs(element, anchor_struct.Type);
                #endregion
            }

            // 将元素放置到目标屏幕锚点物体下
            node = hm_ScreenElement_Send_To_AnchoredList(anchor_struct, element, modulename, indicator);
            #endregion

            // 委托注册
            node.Element.act_on_element_in_start += action_in_start;
            node.Element.act_on_element_in_progress += action_in_progress;
            node.Element.act_on_element_in_end += action_in_end;
            node.Element.act_on_element_out_start += action_out_start;
            node.Element.act_on_element_out_progress += action_out_progress;
            node.Element.act_on_element_out_end += action_out_end;

            // 元素下的所有图元动画倒退
            node.Element.PrimitiveTween_Rewind();

            // 如果 autoin 开启，则表示生成元素后自动播放元素基础三项动画（Alpha、Movement、Rotation）
            if (autoin)
                node.Element.Element_In(args_creator);

            // 标记该元素为已被创建
            node.Element.CreateState = XHudElementCreateState.Created;

            // 返回已生成的元素
            return node;
        }
        /// <summary>
        /// XHud - 管理器通知 - 创建一个XHud元素 - 屏幕空间
        /// </summary>
        /// <param name="element">目标 element 预制体</param>
        /// <param name="indicator">从库中取出后的自定义名称（仅为调用者自己理解的自定义名称）</param>
        /// <param name="offset">元素偏移</param>
        /// <param name="scale">元素缩放</param>
        /// <param name="size">元素尺寸</param>
        /// <param name="rms">RMS系统是否开启？</param>
        /// <param name="rms_name">RMS系统方案名称</param>
        /// <param name="args_creator">元素入场动画参数</param>
        /// <param name="action_in_start">元素入场开始委托</param>
        /// <param name="action_in_progress">元素入场进度委托</param>
        /// <param name="action_in_end">元素入场结束委托</param>        
        /// <param name="action_out_start">元素退场前委托</param>
        /// <param name="action_out_progress">元素退场进度委托</param>
        /// <param name="action_out_end">元素退场后委托</param>     
        /// <param name="autoin">此值是个非常关键的开关，如果你为一个元素编写了一个自定义控制的脚本绑定在它身上，并希望生成出来的时候由您自己决定何时播放动画，那么此值必须为False</param>
        /// <returns>返回一个XHudElement节点元素体</returns>
        public XHudElementNode hm_ScreenElement_Create(
            XHud_Module_Element element, string indicator, Motion_Creator args_creator, bool autoin = true,
            Vector3 offset = default(Vector3), Vector3 scale = default(Vector3), Vector2 size = default,
            bool rms = false, string rms_name = "",
            UnityAction<XHud_Module_Element> action_in_start = null, UnityAction<float> action_in_progress = null,
            UnityAction<XHud_Module_Element> action_in_end = null, UnityAction<XHud_Module_Element> action_out_start = null,
            UnityAction<float> action_out_progress = null, UnityAction<XHud_Module_Element> action_out_end = null)
        {
            // 目标元素
            if (element == null)
            {
                XHud_Utilitys.Func_PrintInfo("XHud - 元素生成通知", "您指定的元素为空！", HudMsgState.警告);
                return null;
            }

            // 标记元素生成来源为  -  实例化源，便于回收&销毁时的识别操作
            element.element_CreatedSourceTypeSet(XHudElementCreatedSourceType.Instantiate);

            // 是否开启RMS模式
            element.element_RMS_Mode_Enabled(rms);

            // 从屏幕元素列表中清理目标元素
            hm_ScreenElement_Remove_From_ScreenAnchored(element);

            #region 初始化元素到对应的目标锚点下
            // 检查动效参数是否为空，如果是空的就使用管理器提供的默认动效参数
            if (args_creator == null)
                args_creator = CreateArgs_Default;

            XHudElementNode node = null;
            Anchor_Layout anchor_struct = null;

            // 如果元素启用了RMS匹配布局，则依据元素自身设计布局参数来布局元素实例
            if (element.RMS_Enabled && RMS_Enabled)
            {
                // 如果RMS名称为空，则使用当前的RMS指定的布局锚点
                if (string.IsNullOrEmpty(rms_name))
                    anchor_struct = hm_ScreenElement_Get_RMS_Anchored(element, RMS_CurrentSolution);
                else
                    anchor_struct = hm_ScreenElement_Get_RMS_Anchored(element, rms_name);

                // 如果RMS名称为空，那么就参考 XHud 管理器当前选中的方案作为元素的RMS方案
                if (string.IsNullOrEmpty(rms_name))
                    hm_ScreenElement_Initialize_By_RMS(element, RMS_CurrentSolution);
                else
                    hm_ScreenElement_Initialize_By_RMS(element, rms_name);
            }
            // 否则如果元素未启用RMS匹配布局，则根据动效参数指定的锚点来布局元素实例
            else
            {
                #region 依据构造参数
                anchor_struct = hm_ScreenElement_Matched_AnchoredType(args_creator.anchor);
                hm_ScreenElement_Initialize_By_MotionArgs(element, anchor_struct.Type);
                #endregion
            }

            // 将元素放置到目标屏幕锚点物体下
            node = hm_ScreenElement_Send_To_AnchoredList(anchor_struct, element, string.IsNullOrEmpty(element.Indicator) ? element.gameObject.name : element.Indicator, indicator);
            #endregion

            // 委托注册
            node.Element.act_on_element_in_start += action_in_start;
            node.Element.act_on_element_in_progress += action_in_progress;
            node.Element.act_on_element_in_end += action_in_end;
            node.Element.act_on_element_out_start += action_out_start;
            node.Element.act_on_element_out_progress += action_out_progress;
            node.Element.act_on_element_out_end += action_out_end;

            // 元素下的所有图元动画倒退
            node.Element.PrimitiveTween_Rewind();

            // 如果 autoin 开启，则表示生成元素后自动播放元素基础三项动画（Alpha、Movement、Rotation）
            if (autoin)
                node.Element.Element_In(args_creator);

            // 标记该元素为已被创建
            node.Element.CreateState = XHudElementCreateState.Created;

            // 返回已生成的元素
            return node;
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
                XHud_Utilitys.Func_PrintInfo("XHud - 元素生成通知", "您指定的元素为空！", HudMsgState.警告);
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
                XHud_Utilitys.Func_PrintInfo("XHud - 元素生成通知", "您指定的元素为空！", HudMsgState.警告);
                return null;
            }

            // 标记元素生成来源为  -  实例化源，便于回收&销毁时的识别操作
            element.element_CreatedSourceTypeSet(XHudElementCreatedSourceType.Library);

            // 从屏幕元素列表中清理目标元素
            hm_ScreenElement_Remove_From_ScreenAnchored(element);

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
            element.RectTransform.SetParent(HudCanvas_WorldAnchor);
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

            Anchors_Layout_World.Add(item);

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

            for (int i = 0; i < Anchors_Layout_World.Count; i++)
            {
                items.Add(Anchors_Layout_World[i]);
            }
            return items.ToArray();
        }
        /// <summary>
        /// XHud - 管理器通知 - 清理已存在的Element项 - 世界
        /// </summary>
        /// <param name="element">目标元素</param>
        public void hm_WorldElement_Remove_From_WorldAnchored(XHud_Module_Element element)
        {
            for (int i = 0; i < Anchors_Layout_World.Count; i++)
            {
                if (element.ID == Anchors_Layout_World[i].ID && element.Indicator == Anchors_Layout_World[i].Indicator)
                {
                    Anchors_Layout_World.RemoveAt(i);
                }
            }
        }
        #endregion

        #region 世界空间 - 生成元素 - 传统多参数方式
        /// <summary>
        /// XHud - 管理器通知 - 创建一个XHud元素 - 世界空间
        /// </summary>
        /// <param name="libname">目标元素库</param>
        /// <param name="indicator">目标标识名称</param>
        /// <param name="modulename">模块名称</param>
        /// <param name="size">锚点</param>
        /// <param name="position">位置_Position</param>
        /// <param name="rotation">旋转_Rotation</param>
        /// <param name="scale">缩放_Scale</param>
        /// <param name="offset">偏移</param>
        /// <param name="args_creator">元素动效参数 - 创建</param>
        /// <param name="action_in_start">委托-进入时</param>
        /// <param name="action_in_progress">委托-进入进度</param>
        /// <param name="action_in_end">委托-进入后</param>
        /// <param name="action_out_start">委托-退出时</param>
        /// <param name="action_out_progress">委托-退出进度</param>
        /// <param name="action_out_end">委托-退出后</param>
        /// <param name="autoin">元素自动执行ElementIn</param>
        /// <returns>返回一个XHudElement节点元素体</returns>
        public XHudElementNode hm_WorldElement_Create(
            string libname, string indicator, string modulename, Motion_Creator args_creator, bool autoin = true,
            Vector2 size = default(Vector2), Vector3 position = default(Vector3), Vector3 rotation = default(Vector3),
            Vector3 scale = default(Vector3), Vector3 offset = default(Vector3),
            UnityAction<XHud_Module_Element> action_in_start = null, UnityAction<float> action_in_progress = null,
            UnityAction<XHud_Module_Element> action_in_end = null, UnityAction<XHud_Module_Element> action_out_start = null,
            UnityAction<float> action_out_progress = null, UnityAction<XHud_Module_Element> action_out_end = null)
        {
            if (!SupportWorldUI)
            {
                XHud_Utilitys.Func_PrintInfo("XHud - 元素生成通知", "你想要生成世界UI，但是XHud管理器并未开启世界模式支持！请检查该开关是否为开启状态！", HudMsgState.警告);
                return null;
            }
            // 从元素库中取出元素
            XHud_Module_Element element = hm_ElementLibrary_GetElement(libname, modulename);
            if (element == null)
            {
                XHud_Utilitys.Func_PrintInfo("XHud - 元素生成通知", "您从元素库获取的目标元素为空！请检查该元素在元素库中的状态！", HudMsgState.警告);
                return null;
            }

            // 标记元素生成来源为  -  元素库源，便于回收&销毁时的识别操作
            element.element_CreatedSourceTypeSet(XHudElementCreatedSourceType.Library);

            // 从屏幕元素列表中清理目标元素
            hm_WorldElement_Remove_From_WorldAnchored(element);

            // 初始化元素到世界锚点下
            hm_WorldElement_Initialize_For_World(element);
            XHudElementNode node = hm_WorldElement_Send_To_AnchoredList(element, modulename, indicator);

            // 委托注册
            node.Element.act_on_element_in_start += action_in_start;
            node.Element.act_on_element_in_progress += action_in_progress;
            node.Element.act_on_element_in_end += action_in_end;
            node.Element.act_on_element_out_start += action_out_start;
            node.Element.act_on_element_out_progress += action_out_progress;
            node.Element.act_on_element_out_end += action_out_end;

            // 如果 autoin 开启，则表示生成元素后自动播放元素基础三项动画（Alpha、Movement、Rotation）
            if (autoin)
                node.Element.Element_In(args_creator);

            // 标记该元素为已被创建
            node.Element.CreateState = XHudElementCreateState.Created;

            // 返回已生成的元素
            return node;
        }
        /// <summary>
        /// XHud - 管理器通知 - 创建一个XHud元素 - 世界空间
        /// </summary>
        /// <param name="element">目标 element 预制体</param>
        /// <param name="indicator">目标标识名称</param>
        /// <param name="size">锚点</param>
        /// <param name="position">位置_Position</param>
        /// <param name="rotation">旋转_Rotation</param>
        /// <param name="scale">缩放_Scale</param>
        /// <param name="offset">偏移</param>
        /// <param name="args_creator">元素动效参数 - 创建</param>
        /// <param name="action_in_start">委托-进入时</param>
        /// <param name="action_in_progress">委托-进入进度</param>
        /// <param name="action_in_end">委托-进入后</param>
        /// <param name="action_out_start">委托-退出时</param>
        /// <param name="action_out_progress">委托-退出进度</param>
        /// <param name="action_out_end">委托-退出后</param>
        /// <param name="autoin">元素自动执行ElementIn</param>
        /// <returns>返回一个XHudElement节点元素体</returns>
        public XHudElementNode hm_WorldElement_Create(
            XHud_Module_Element element, string indicator, Motion_Creator args_creator, bool autoin = true,
            Vector2 size = default(Vector2), Vector3 position = default(Vector3), Vector3 rotation = default(Vector3),
            Vector3 scale = default(Vector3), Vector3 offset = default(Vector3),
            UnityAction<XHud_Module_Element> action_in_start = null, UnityAction<float> action_in_progress = null,
            UnityAction<XHud_Module_Element> action_in_end = null, UnityAction<XHud_Module_Element> action_out_start = null,
            UnityAction<float> action_out_progress = null, UnityAction<XHud_Module_Element> action_out_end = null)
        {
            if (!SupportWorldUI)
            {
                XHud_Utilitys.Func_PrintInfo("XHud - 元素生成通知", "你想要生成世界UI，但是XHud管理器并未开启世界模式支持！请检查该开关是否为开启状态！", HudMsgState.警告);
                return null;
            }

            // 目标元素
            if (element == null)
            {
                XHud_Utilitys.Func_PrintInfo("XHud - 元素生成通知", "您从元素库获取的目标元素为空！请检查该元素在元素库中的状态！", HudMsgState.警告);
                return null;
            }

            // 标记元素生成来源为  -  实例化源，便于回收&销毁时的识别操作
            element.element_CreatedSourceTypeSet(XHudElementCreatedSourceType.Instantiate);

            // 从屏幕元素列表中清理目标元素
            hm_WorldElement_Remove_From_WorldAnchored(element);

            // 初始化元素到世界锚点下
            hm_WorldElement_Initialize_For_World(element);
            XHudElementNode node = hm_WorldElement_Send_To_AnchoredList(element, string.IsNullOrEmpty(element.Indicator) ? element.gameObject.name : element.Indicator, indicator);

            // 委托注册
            node.Element.act_on_element_in_start += action_in_start;
            node.Element.act_on_element_in_progress += action_in_progress;
            node.Element.act_on_element_in_end += action_in_end;
            node.Element.act_on_element_out_start += action_out_start;
            node.Element.act_on_element_out_progress += action_out_progress;
            node.Element.act_on_element_out_end += action_out_end;

            // 如果 autoin 开启，则表示生成元素后自动播放元素基础三项动画（Alpha、Movement、Rotation）
            if (autoin)
                node.Element.Element_In(args_creator);

            // 标记该元素为已被创建
            node.Element.CreateState = XHudElementCreateState.Created;

            // 返回已生成的元素
            return node;
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
            if (!SupportWorldUI)
            {
                XHud_Utilitys.Func_PrintInfo("XHud - 元素生成通知", "你想要生成世界UI，但是XHud管理器并未开启世界模式支持！请检查该开关是否为开启状态！", HudMsgState.警告);
                return null;
            }

            // 目标元素
            if (element == null)
            {
                XHud_Utilitys.Func_PrintInfo("XHud - 元素生成通知", "您指定的元素为空！！", HudMsgState.警告);
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
            if (!SupportWorldUI)
            {
                XHud_Utilitys.Func_PrintInfo("XHud - 元素生成通知", "你想要生成世界UI，但是XHud管理器并未开启世界模式支持！请检查该开关是否为开启状态！", HudMsgState.警告);
                return null;
            }

            // 从元素库中取出元素
            XHud_Module_Element element = hm_ElementLibrary_GetElement(libname, modulename);
            // 目标元素
            if (element == null)
            {
                XHud_Utilitys.Func_PrintInfo("XHud - 元素生成通知", "您指定的元素为空！！", HudMsgState.警告);
                return null;
            }

            // 标记元素生成来源为  -  实例化源，便于回收&销毁时的识别操作
            element.element_CreatedSourceTypeSet(XHudElementCreatedSourceType.Library);

            // 从屏幕元素列表中清理目标元素
            hm_WorldElement_Remove_From_WorldAnchored(element);

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
                args = RecycleArgs_Default;

            string x_indicator = "";
            int x_id = 0;

            // 回收所有屏幕元素
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    XHudElementNode node = Anchors_Layout_Screen[i].HudElementInfos[s];
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
                     *            XHud_Utilitys.Func_PrintInfo("XHud - 音效器通知", "已注册 - 元素 - 事件", HudMsgState.通知);
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
                        args = RecycleArgs_Default;
                    node.Element.Element_Out(args);

                    // 调试信息 - 成功
                    if (UseDebug)
                        XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", $"已{(node.Element.CreatedSourceType == XHudElementCreatedSourceType.Library ? "回收" : "销毁")}元素：" + x_indicator + " / " + x_id, HudMsgState.通知);
                }
            }

            // 回收所有世界元素
            for (int i = 0; i < Anchors_Layout_World.Count; i++)
            {
                XHudElementNode node = Anchors_Layout_World[i];
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
                 *            XHud_Utilitys.Func_PrintInfo("XHud - 音效器通知", "已注册 - 元素 - 事件", HudMsgState.通知);
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
                    args = RecycleArgs_Default;
                node.Element.Element_Out(args);

                // 调试信息 - 成功
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", $"已{(node.Element.CreatedSourceType == XHudElementCreatedSourceType.Library ? "回收" : "销毁")}元素：" + x_indicator + " / " + x_id, HudMsgState.通知);
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
            for (int i = 0; i < Anchors_Layout_Screen.Count && !finded; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count && !finded; s++)
                {
                    if (element.ID == Anchors_Layout_Screen[i].HudElementInfos[s].ID)
                    {
                        x_id = Anchors_Layout_Screen[i].HudElementInfos[s].ID;
                        x_indicator = Anchors_Layout_Screen[i].HudElementInfos[s].Indicator;
                        targetNode = Anchors_Layout_Screen[i].HudElementInfos[s];
                        finded = true;
                    }
                }
            }

            // 在世界元素中查找（仅在屏幕元素中未找到时）
            if (!finded)
            {
                for (int i = 0; i < Anchors_Layout_World.Count && !finded; i++)
                {
                    if (element.ID == Anchors_Layout_World[i].ID)
                    {
                        x_id = Anchors_Layout_World[i].ID;
                        x_indicator = Anchors_Layout_World[i].Indicator;
                        targetNode = Anchors_Layout_World[i];
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
                 *            XHud_Utilitys.Func_PrintInfo("XHud - 音效器通知", "已注册 - 元素 - 事件", HudMsgState.通知);
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
                    args = RecycleArgs_Default;
                targetNode.Element.Element_Out(args);

                // 调试信息 - 成功
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", $"已{(element.CreatedSourceType == XHudElementCreatedSourceType.Library ? "回收" : "销毁")}元素：" + x_indicator + " / " + x_id, HudMsgState.通知);
            }
            else
            {
                // 调试信息 - 未找到
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "XHud元素：" + $"{(string.IsNullOrEmpty(element.Indicator) ? element.gameObject.name : element.Indicator)}" + $" 不存在！未找到要{(element.CreatedSourceType == XHudElementCreatedSourceType.Library ? "回收" : "销毁")}的目标元素！", HudMsgState.通知);
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
            for (int i = 0; i < Anchors_Layout_Screen.Count && element == null; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count && element == null; s++)
                {
                    if (id == Anchors_Layout_Screen[i].HudElementInfos[s].ID)
                    {
                        element = Anchors_Layout_Screen[i].HudElementInfos[s].Element;
                    }
                }
            }

            // 在世界元素中查找
            for (int i = 0; i < Anchors_Layout_World.Count && element == null; i++)
            {
                if (id == Anchors_Layout_World[i].ID)
                {
                    element = Anchors_Layout_World[i].Element;
                }
            }

            // 找到则调用元素回收方法，否则输出调试信息
            if (element != null)
            {
                hm_HudElement_RecycleAt(element, args, action_out_start, action_out_progress, action_out_end);
            }
            else if (UseDebug)
            {
                XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", $"XHud元素ID：{id} 不存在！", HudMsgState.通知);
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
                for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
                {
                    for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                    {
                        if (id == Anchors_Layout_Screen[i].HudElementInfos[s].ID)
                        {
                            element = Anchors_Layout_Screen[i].HudElementInfos[s].Element;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Anchors_Layout_World.Count; i++)
                {
                    if (id == Anchors_Layout_World[i].Element.ID)
                    {
                        element = Anchors_Layout_World[i].Element;
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
                for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
                {
                    for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                    {
                        if (name == Anchors_Layout_Screen[i].HudElementInfos[s].Indicator)
                        {
                            element = Anchors_Layout_Screen[i].HudElementInfos[s].Element;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Anchors_Layout_World.Count; i++)
                {
                    if (name == Anchors_Layout_World[i].Element.Indicator)
                    {
                        element = Anchors_Layout_World[i].Element;
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
                for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
                {
                    for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                    {
                        list.Add(Anchors_Layout_Screen[i].HudElementInfos[s].Element);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Anchors_Layout_World.Count; i++)
                {
                    list.Add(Anchors_Layout_World[i].Element);
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
                for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
                {
                    for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                    {
                        if (Anchors_Layout_Screen[i].HudElementInfos[s].Element != null)
                            x++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Anchors_Layout_World.Count; i++)
                {
                    if (Anchors_Layout_World[i].Element != null)
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
                for (int i = 0; i < mgr.HudCanvas_ScreenAnchor.childCount; i++)
                {
                    Transform trans = mgr.HudCanvas_ScreenAnchor.GetChild(i);
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
                for (int i = 0; i < mgr.HudCanvas_WorldAnchor.childCount; i++)
                {
                    Transform trans = mgr.HudCanvas_WorldAnchor.GetChild(i);
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
            XHud_Module_Element[] trans = mgr.HudCanvas_ScreenAnchor.GetComponentsInChildren<XHud_Module_Element>();
            return trans;
        }
        public RectTransform hm_GetAnchorRoot(XHudSpace Space)
        {
            if (Space == XHudSpace.屏幕空间)
                return HudCanvas_ScreenAnchor;
            else
                return HudCanvas_WorldAnchor;
        }
        #endregion
    }
}