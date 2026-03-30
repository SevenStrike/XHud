namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
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
        /// <summary>
        /// 转场图形组件
        /// </summary>
        public Image TransitionImage;
        /// <summary>
        /// 转场-材质
        /// </summary>
        public Material TransitionMat;
        /// <summary>
        /// 转场模式
        /// </summary>
        public TransitionMode Mode;
        /// <summary>
        /// 转场模式开关
        /// </summary>
        public bool ModeSwitch;
        /// <summary>
        /// 转场耗时
        /// </summary>
        public float TransitionTime = 0.035f;
        /// <summary>
        /// 转场状态开关
        /// </summary>
        public bool IsTransiting;
        /// <summary>
        /// 范围量程
        /// </summary>
        [Range(0, 1)]
        public float TransitionProgress;
        /// <summary>
        /// 转场效果名称
        /// </summary>
        public string TransitionName;
        /// <summary>
        /// 当前帧
        /// </summary>
        public int CurrentFrame;
        [SerializeField]
        /// <summary>
        /// 按键可用
        /// </summary>
        public bool UseKeyControl;
        [SerializeField]
        /// <summary>
        /// 水平翻转
        /// </summary>
        public bool Flip_Hor;
        [SerializeField]
        /// <summary>
        /// 垂直翻转
        /// </summary>
        public bool Flip_Ver;

        [SerializeField]
        public bool DebugState;
        public float TransitionAlpha = 1;
        public Color TransitionOverlayColor;

        [Range(0, 1)]
        /// <summary>
        /// 接近转场开始后的帧百分比
        /// </summary>
        public float LimiteFramePer_Start;
        [Range(0, 1)]
        /// <summary>
        /// 接近转场结束时的帧百分比
        /// </summary>
        public float LimiteFramePer_End;

        /// <summary>
        /// 转场状态接近结束开关
        /// </summary>
        public bool IsTransiting_WithEnd;
        /// <summary>
        /// 转场状态接近开始开关
        /// </summary>
        public bool IsTransiting_WithStart;

        /// <summary>
        /// 用于预览的转场节点
        /// </summary>
        public XHud_LibraryArg_Transition CurrentTransitionNode = new XHud_LibraryArg_Transition();

        public KeyCode TransitionPlayKey = KeyCode.T;
        public KeyCode TransitionFlip_H_Key = KeyCode.H;
        public KeyCode TransitionFlip_V_Key = KeyCode.V;
        public KeyCode TransitionMod_Key = KeyCode.M;

        /// <summary>
        /// 转场中
        /// </summary>
        public UnityAction<float, Texture2D> On_InTransition;

        /// <summary>
        /// 转场开始
        /// </summary>
        public UnityAction On_TansitingStart;

        /// <summary>
        /// 转场完成
        /// </summary>
        public UnityAction On_TansitingComplete;

        /// <summary>
        /// 接近首帧极限时
        /// </summary>
        public UnityAction On_LimiteStartFrame;

        /// <summary>
        /// 接近尾帧极限时
        /// </summary>
        public UnityAction On_LimiteEndFrame;

        void Start()
        {
            InitialMaterial();
        }

        private void InitialMaterial()
        {
            TransitionMat = new Material(TransitionMat);
            TransitionMat.name = "RuntimeTransitionMat";
            TransitionImage.material = TransitionMat;
        }


        void Update()
        {
            TransitionUpdate();

            if (UseKeyControl)
            {
                if (Input.GetKeyDown(TransitionPlayKey))
                {
                    TransitionPlay();
                }
                if (Input.GetKeyDown(TransitionFlip_H_Key))
                {
                    Transition_Set_Flip_H(!Flip_Hor);
                }
                if (Input.GetKeyDown(TransitionFlip_V_Key))
                {
                    Transition_Set_Flip_V(!Flip_Ver);
                }
                if (Input.GetKeyDown(TransitionMod_Key))
                {
                    Transition_Set_Mode(!ModeSwitch);
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
        /// <param tweenName="mode"></param>
        /// <param tweenName="name"></param>
        /// <param tweenName="time"></param>
        public void TransitionPlay_Custom(TransitionMode mode, string name, float time)
        {
            if (IsTransiting)
            {
                Debug.Log("正在转场中...请等待当前转场动作结束！");
                return;
            }

            XHud_LibraryArg_Transition node = XHud_Manager.Instance.Hud_TransitionLib.TransitionLibrary_Get(name);
            CurrentTransitionNode = node;
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
            if (IsTransiting)
            {
                Debug.Log("正在转场中...请等待当前转场动作结束！");
                return;
            }

            if (Mode == TransitionMode.入场)
            {
                Transition_Set_ChannelInvert(false);
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("转场器通知", "开始转场 -> 入场！", HudMsgState.通知);
                }
            }
            else
            {
                Transition_Set_ChannelInvert(true);
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("转场器通知", "开始转场 -> 出场！", HudMsgState.通知);
                }
            }

            StartCoroutine(TransitionPlayer(CurrentTransitionNode, TransitionTime));
        }

        /// <summary>
        /// 根据指定的转场模式进行转场
        /// </summary>
        /// <param tweenName="mode"></param>
        public void TransitionPlay(TransitionMode mode)
        {
            if (IsTransiting)
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

            StartCoroutine(TransitionPlayer(CurrentTransitionNode, TransitionTime));
        }

        /// <summary>
        /// 转场逻辑
        /// </summary>
        /// <param tweenName="node"></param>
        /// <param tweenName="time"></param>
        /// <returns></returns>
        IEnumerator TransitionPlayer(XHud_LibraryArg_Transition node, float time)
        {
            int frameLimite_start = (int)Mathf.Floor(CurrentTransitionNode.Frames.Count * LimiteFramePer_Start);
            int frameLimite_end = (int)Mathf.Floor(CurrentTransitionNode.Frames.Count * LimiteFramePer_End);

            IsTransiting = true;
            if (On_TansitingStart != null)
                On_TansitingStart();
            for (int i = 0; i < node.Frames.Count; i += CurrentTransitionNode.SkipFrame)
            {
                if (i > frameLimite_start && i < frameLimite_end)
                {
                    IsTransiting_WithStart = true;
                    if (On_LimiteStartFrame != null)
                        On_LimiteStartFrame();
                }
                if (i > frameLimite_end)
                {
                    IsTransiting_WithEnd = true;
                    if (On_LimiteEndFrame != null)
                        On_LimiteEndFrame();
                }
                if (On_InTransition != null)
                    On_InTransition(i, node.Frames[i]);
                Transition_Set_MaskTexture(node.Frames[i]);
                yield return new WaitForSecondsRealtime(time);
            }
            IsTransiting = false;
            IsTransiting_WithStart = false;
            IsTransiting_WithEnd = false;
            if (On_TansitingComplete != null)
                On_TansitingComplete();

        }

        /// <summary>
        /// 设置Mask图片
        /// </summary>
        /// <param tweenName="tex"></param>
        public void Transition_Set_MaskTexture(Texture2D tex)
        {
            TransitionMat.SetTexture("_Mask", tex);
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param tweenName="tex"></param>
        public void Transition_Set_BaseTexture(Texture2D tex)
        {
            TransitionMat.SetTexture("_Texture", tex);
        }

        /// <summary>
        /// 设置叠加颜色
        /// </summary>
        /// <param tweenName="col"></param>
        public void Transition_Set_OverlayColor(Color col)
        {
            TransitionOverlayColor = col;
        }

        /// <summary>
        /// 刷新叠加颜色
        /// </summary>
        public void Transition_Update_OverlayColor()
        {
            TransitionMat.SetColor("_Color", TransitionOverlayColor);
        }

        /// <summary>
        /// 设置转场透明度
        /// </summary>
        /// <param tweenName="val"></param>
        public void Transition_Set_Alpha(float val)
        {
            TransitionAlpha = val;
        }

        /// <summary>
        /// 刷新转场透明度
        /// </summary>
        /// <param tweenName="val"></param>
        public void Transition_Update_Alpha()
        {
            TransitionMat.SetFloat("_Alpha", TransitionAlpha);
        }

        /// <summary>
        /// 设置通道翻转
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void Transition_Set_ChannelInvert(bool state)
        {
            if (state)
                TransitionMat.SetFloat("_Invert", 1);
            else
                TransitionMat.SetFloat("_Invert", 0);

        }

        /// <summary>
        /// 设置画面镜像翻转
        /// </summary>
        /// <param tweenName="flip_h">水平</param>
        /// <param tweenName="flip_v">垂直</param>
        public void Transition_Set_Flip(bool flip_h, bool flip_v)
        {
            Flip_Hor = flip_h;
            Flip_Ver = flip_v;

            TransitionMat.SetInt("_Flip_Hor", Flip_Hor ? 1 : 0);
            TransitionMat.SetInt("_Flip_Ver", Flip_Ver ? 1 : 0);
        }

        /// <summary>
        /// 设置画面垂直镜像翻转
        /// </summary>
        /// <param tweenName="flip_toggle">垂直</param>
        public void Transition_Set_Flip_V(bool flip)
        {
            Flip_Ver = flip;
            TransitionMat.SetInt("_Flip_Ver", Flip_Ver ? 1 : 0);
        }

        /// <summary>
        /// 设置画面水平镜像翻转
        /// </summary>
        /// <param tweenName="flip_toggle">水平</param>
        public void Transition_Set_Flip_H(bool flip)
        {
            Flip_Hor = flip;
            TransitionMat.SetInt("_Flip_Hor", Flip_Hor ? 1 : 0);
        }

        /// <summary>
        /// 设置转场方式
        /// </summary>
        /// <param tweenName="treeState">如果为True就是出场，否则就是入场</param>
        public void Transition_Set_Mode(bool state)
        {
            ModeSwitch = state;
            if (ModeSwitch)
            {
                Mode = TransitionMode.出场;
            }
            else
            {
                Mode = TransitionMode.入场;
            }
        }
    }
}