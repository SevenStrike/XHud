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
    using System;
    using System.Buffers.Text;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public enum TransitionMode
    {
        入场 = 0,
        出场 = 1
    }

    public class XHud_TransitionController : MonoBehaviour
    {
        #region 字段 - 转场组件
        /// <summary>
        /// 转场图形组件
        /// </summary>
        public Image transition_image;
        /// <summary>
        /// 转场材质
        /// </summary>
        public Material transition_mat;
        #endregion

        #region 字段 - 转场模式与状态
        /// <summary>
        /// 转场模式
        /// </summary>
        public TransitionMode mode;
        /// <summary>
        /// 转场模式开关
        /// </summary>
        public bool mode_switch;
        /// <summary>
        /// 转场耗时
        /// </summary>
        public float transition_time = 0.035f;
        /// <summary>
        /// 转场状态开关
        /// </summary>
        public bool is_transiting;
        /// <summary>
        /// 转场状态接近结束开关
        /// </summary>
        public bool is_transiting_with_end;
        /// <summary>
        /// 转场状态接近开始开关
        /// </summary>
        public bool is_transiting_with_start;
        #endregion

        #region 字段 - 转场进度
        /// <summary>
        /// 范围量程（0 到 1）
        /// </summary>
        [Range(0, 1)]
        public float transition_progress;
        /// <summary>
        /// 当前帧
        /// </summary>
        public int current_frame;
        /// <summary>
        /// 接近转场开始后的帧百分比
        /// </summary>
        [Range(0, 1)]
        public float limite_frame_per_start;
        /// <summary>
        /// 接近转场结束时的帧百分比
        /// </summary>
        [Range(0, 1)]
        public float limite_frame_per_end;
        #endregion

        #region 字段 - 转场外观
        /// <summary>
        /// 转场效果名称
        /// </summary>
        public string transition_name;
        /// <summary>
        /// 转场透明度
        /// </summary>
        public float transition_alpha = 1;
        /// <summary>
        /// 转场叠加颜色
        /// </summary>
        public Color transition_overlay_color;
        #endregion

        #region 字段 - 翻转与按键控制
        /// <summary>
        /// 按键可用
        /// </summary>
        [SerializeField]
        public bool use_key_control;
        /// <summary>
        /// 水平翻转
        /// </summary>
        [SerializeField]
        public bool flip_hor;
        /// <summary>
        /// 垂直翻转
        /// </summary>
        [SerializeField]
        public bool flip_ver;
        #endregion

        #region 字段 - 调试
        /// <summary>
        /// 调试开关
        /// </summary>
        [SerializeField]
        public bool debug_state;
        #endregion

        #region 字段 - 预览与按键
        /// <summary>
        /// 用于预览的转场节点
        /// </summary>
        public XHud_LibraryArg_Transition current_transition_node = new XHud_LibraryArg_Transition();
        /// <summary>
        /// 转场播放按键
        /// </summary>
        public KeyCode transition_play_key = KeyCode.T;
        /// <summary>
        /// 水平翻转按键
        /// </summary>
        public KeyCode transition_flip_h_key = KeyCode.H;
        /// <summary>
        /// 垂直翻转按键
        /// </summary>
        public KeyCode transition_flip_v_key = KeyCode.V;
        /// <summary>
        /// 转场模式切换按键
        /// </summary>
        public KeyCode transition_mode_key = KeyCode.M;
        #endregion

        /// <summary>
        /// 转场中
        /// </summary>
        public UnityAction<float, Texture2D> on_InTransition;
        /// <summary>
        /// 转场开始
        /// </summary>
        public UnityAction on_TansitingStart;
        /// <summary>
        /// 转场完成
        /// </summary>
        public UnityAction on_TansitingComplete;
        /// <summary>
        /// 接近首帧极限时
        /// </summary>
        public UnityAction on_LimiteStartFrame;
        /// <summary>
        /// 接近尾帧极限时
        /// </summary>
        public UnityAction on_LimiteEndFrame;

        #region 折叠
        public bool fold_options = true;
        public bool fold_state = true;
        public bool fold_params = true;
        public bool fold_key = true;
        public bool fold_based = true;

        public void GroupFold(bool state)
        {
            fold_options = state;
            fold_state = state;
            fold_params = state;
            fold_key = state;
            fold_based = state;
        }

        public void GroupFold(bool options = true, bool state = true, bool param = true, bool key = true, bool based = true)
        {
            fold_options = options;
            fold_state = state;
            fold_params = param;
            fold_key = key;
            fold_based = based;
        }
        #endregion

        void Start()
        {
            InitialMaterial();
        }

        private void InitialMaterial()
        {
            transition_mat = new Material(transition_mat);
            transition_mat.name = "RuntimeTransitionMat";
            transition_image.material = transition_mat;
        }

        void Update()
        {
            TransitionUpdate();

            if (use_key_control)
            {
                if (Input.GetKeyDown(transition_play_key))
                {
                    TransitionPlay();
                }
                if (Input.GetKeyDown(transition_flip_h_key))
                {
                    Transition_Set_Flip_H(!flip_hor);
                }
                if (Input.GetKeyDown(transition_flip_v_key))
                {
                    Transition_Set_Flip_V(!flip_ver);
                }
                if (Input.GetKeyDown(transition_mode_key))
                {
                    Transition_Set_Mode(!mode_switch);
                }
            }
        }

        /// <summary>
        /// 转场更新
        /// </summary>
        private void TransitionUpdate()
        {
            Transition_Update_OverlayColor();
            Transition_Update_Alpha();
        }

        /// <summary>
        /// 播放自定义转场
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="name"></param>
        /// <param name="time"></param>
        public void TransitionPlay_Custom(TransitionMode mode, string name, float time)
        {
            if (is_transiting)
            {
                Debug.Log("正在转场中...请等待当前转场动作结束！");
                return;
            }

            XHud_LibraryArg_Transition node = XHud_Manager.instance.library_transitions.TransitionLibrary_Get(name);
            current_transition_node = node;
            if (mode == TransitionMode.入场)
            {
                Transition_Set_ChannelInvert(false);
            }
            else
            {
                Transition_Set_ChannelInvert(true);
            }
            StartCoroutine(TransitionPlayer(node, time));
        }

        /// <summary>
        /// 播放原生转场
        /// </summary>
        public void TransitionPlay()
        {
            if (is_transiting)
            {
                Debug.Log("正在转场中...请等待当前转场动作结束！");
                return;
            }

            if (mode == TransitionMode.入场)
            {
                Transition_Set_ChannelInvert(false);
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 转场器通知", "开始转场 -> 入场！", XGUIMsgState.通知);
                }
            }
            else
            {
                Transition_Set_ChannelInvert(true);
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 转场器通知", "开始转场 -> 出场！", XGUIMsgState.通知);
                }
            }

            StartCoroutine(TransitionPlayer(current_transition_node, transition_time));
        }

        /// <summary>
        /// 根据指定的转场模式进行转场
        /// </summary>
        /// <param name="mode"></param>
        public void TransitionPlay(TransitionMode mode)
        {
            if (is_transiting)
            {
                Debug.Log("正在转场中...请等待当前转场动作结束！");
                return;
            }

            if (mode == TransitionMode.入场)
            {
                Transition_Set_ChannelInvert(false);
                Debug.Log("开始转场 -> 入场");
            }
            else
            {
                Transition_Set_ChannelInvert(true);
                Debug.Log("开始转场 -> 出场");
            }

            StartCoroutine(TransitionPlayer(current_transition_node, transition_time));
        }

        /// <summary>
        /// 转场逻辑
        /// </summary>
        /// <param name="node"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        IEnumerator TransitionPlayer(XHud_LibraryArg_Transition node, float time)
        {
            int frameLimite_start = (int)Mathf.Floor(current_transition_node.frames.Count * limite_frame_per_start);
            int frameLimite_end = (int)Mathf.Floor(current_transition_node.frames.Count * limite_frame_per_end);

            is_transiting = true;
            if (on_TansitingStart != null)
                on_TansitingStart();
            for (int i = 0; i < node.frames.Count; i += current_transition_node.skip_frame)
            {
                if (i > frameLimite_start && i < frameLimite_end)
                {
                    is_transiting_with_start = true;
                    if (on_LimiteStartFrame != null)
                        on_LimiteStartFrame();
                }
                if (i > frameLimite_end)
                {
                    is_transiting_with_end = true;
                    if (on_LimiteEndFrame != null)
                        on_LimiteEndFrame();
                }
                if (on_InTransition != null)
                    on_InTransition(i, node.frames[i]);
                Transition_Set_MaskTexture(node.frames[i]);
                yield return new WaitForSecondsRealtime(time);
            }
            is_transiting = false;
            is_transiting_with_start = false;
            is_transiting_with_end = false;
            if (on_TansitingComplete != null)
                on_TansitingComplete();

        }

        /// <summary>
        /// 设置Mask图片
        /// </summary>
        /// <param name="tex"></param>
        public void Transition_Set_MaskTexture(Texture2D tex)
        {
            transition_mat.SetTexture("_Mask", tex);
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="tex"></param>
        public void Transition_Set_BaseTexture(Texture2D tex)
        {
            transition_mat.SetTexture("_Texture", tex);
        }

        /// <summary>
        /// 设置叠加颜色
        /// </summary>
        /// <param name="col"></param>
        public void Transition_Set_OverlayColor(Color col)
        {
            transition_overlay_color = col;
        }

        /// <summary>
        /// 刷新叠加颜色
        /// </summary>
        public void Transition_Update_OverlayColor()
        {
            transition_mat.SetColor("_Color", transition_overlay_color);
        }

        /// <summary>
        /// 设置转场透明度
        /// </summary>
        /// <param name="val"></param>
        public void Transition_Set_Alpha(float val)
        {
            transition_alpha = val;
        }

        /// <summary>
        /// 刷新转场透明度
        /// </summary>
        /// <param name="val"></param>
        public void Transition_Update_Alpha()
        {
            transition_mat.SetFloat("_Alpha", transition_alpha);
        }

        /// <summary>
        /// 设置通道翻转
        /// </summary>
        /// <param name="treeState"></param>
        public void Transition_Set_ChannelInvert(bool state)
        {
            if (state)
                transition_mat.SetFloat("_Invert", 1);
            else
                transition_mat.SetFloat("_Invert", 0);

        }

        /// <summary>
        /// 设置画面镜像翻转
        /// </summary>
        /// <param name="flip_h">水平</param>
        /// <param name="flip_v">垂直</param>
        public void Transition_Set_Flip(bool flip_h, bool flip_v)
        {
            flip_hor = flip_h;
            flip_ver = flip_v;

            transition_mat.SetInt("_Flip_Hor", flip_hor ? 1 : 0);
            transition_mat.SetInt("_Flip_Ver", flip_ver ? 1 : 0);
        }

        /// <summary>
        /// 设置画面垂直镜像翻转
        /// </summary>
        /// <param name="flip_toggle">垂直</param>
        public void Transition_Set_Flip_V(bool flip)
        {
            flip_ver = flip;
            transition_mat.SetInt("_Flip_Ver", flip_ver ? 1 : 0);
        }

        /// <summary>
        /// 设置画面水平镜像翻转
        /// </summary>
        /// <param name="flip_toggle">水平</param>
        public void Transition_Set_Flip_H(bool flip)
        {
            flip_hor = flip;
            transition_mat.SetInt("_Flip_Hor", flip_hor ? 1 : 0);
        }

        /// <summary>
        /// 设置转场方式
        /// </summary>
        /// <param name="treeState">如果为True就是出场，否则就是入场</param>
        public void Transition_Set_Mode(bool state)
        {
            mode_switch = state;
            if (mode_switch)
            {
                mode = TransitionMode.出场;
            }
            else
            {
                mode = TransitionMode.入场;
            }
        }
    }
}