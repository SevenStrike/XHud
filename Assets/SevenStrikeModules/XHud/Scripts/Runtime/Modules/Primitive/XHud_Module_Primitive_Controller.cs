namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using System;
    using UnityEditor.MemoryProfiler;
    using UnityEngine;
    using UnityEngine.UI;

    public class XHud_Module_Primitive_Controller : MonoBehaviour
    {
        #region 基础信息
        /// <summary>
        /// 图元编号
        /// </summary>
        [SerializeField] public int ID;
        /// <summary>
        /// 图元名称
        /// </summary>
        [SerializeField] public string Indicator;
        /// <summary>
        /// 初始化状态
        /// </summary>
        [SerializeField] public bool IsInitial;
        /// <summary>
        /// 图元类型（标识当前操作的组件类型）
        /// </summary>
        [SerializeField] public ModuleType ModuleType;
        #endregion

        #region 受控组件   
        /// <summary>
        /// 组件 - CanvasGroup
        /// </summary>
        [SerializeField] public CanvasGroup mod_CanvasGroup;
        /// <summary>
        /// 组件 - Rect
        /// </summary>
        [SerializeField] public RectTransform mod_Rect;
        /// <summary>
        /// 组件 - Tmp文字
        /// </summary>
        [SerializeField] public XHud_Module_TmpText mod_TmpText;
        /// <summary>
        /// 组件 - 文字
        /// </summary>
        [SerializeField] public XHud_Module_Text mod_Text;
        /// <summary>
        /// 组件 - 图片
        /// </summary>
        [SerializeField] public Image mod_Image;
        /// <summary>
        /// 组件 - Raw图片
        /// </summary>
        [SerializeField] public RawImage mod_RawImage;
        #endregion

        #region 状态开关
        /// <summary>
        /// 调试开关
        /// </summary>
        [SerializeField] public bool Debug;
        #endregion

        #region 图元模块
        /// <summary>
        /// 配色器
        /// </summary>
        [SerializeField] public XHud_Module_Primitive_Painting pt_Painting;
        /// <summary>
        /// 姿态基准
        /// </summary>
        [SerializeField] public XHud_Module_Primitive_Feature pt_Feature;
        #endregion

        void Awake()
        {
            // 获取被控组件
            GetControlComponent();
            // 获取被控组件基类类型
            RecognizeType();
        }

        void Start()
        {

        }

        void Update()
        {

        }

        /// <summary>
        /// 获取被控组件
        /// </summary>
        public void GetControlComponent()
        {
            if (mod_CanvasGroup == null)
                mod_CanvasGroup = GetComponent<CanvasGroup>();
            if (mod_Rect == null)
                mod_Rect = GetComponent<RectTransform>();
            if (mod_Image == null)
                mod_Image = GetComponent<Image>();
            if (mod_RawImage == null)
                mod_RawImage = GetComponent<RawImage>();
            if (mod_Text == null)
                mod_Text = GetComponent<XHud_Module_Text>();
            if (mod_TmpText == null)
                mod_TmpText = GetComponent<XHud_Module_TmpText>();
            //if (mod_HudButton == null)
            //    mod_HudButton = GetComponentInParent<XHud_Module_Button>();
        }
        /// <summary>
        /// 获取被控组件基类类型
        /// 判断Image、RawImage、Text、TmpText是否不为空
        /// 取其一个不为空的作为他们的Graphic基类传出
        /// </summary>
        /// <returns></returns>
        public Graphic RecognizeType()
        {
            Graphic gc = null;

            ///---------Image组件
            if (mod_Image != null)
            {
                gc = mod_Image;
                ModuleType = ModuleType.Image;
            }
            ///---------RawImage
            if (mod_RawImage != null)
            {
                gc = mod_RawImage;
                ModuleType = ModuleType.RawImage;
            }
            ///---------Text
            if (mod_Text != null)
            {
                gc = mod_Text;
                ModuleType = ModuleType.Text;
            }
            ///-}--------TmpText
            if (mod_TmpText != null)
            {
                gc = mod_TmpText;
                ModuleType = ModuleType.TmpText;
            }

            return gc;
        }

        /// <summary>
        /// 获取受控模组类型
        /// </summary>
        /// <returns></returns>
        public ModuleType GetModuleType()
        {
            return ModuleType;
        }

        #region 组件标识
        /// <summary>
        /// 获取动画器ID
        /// </summary>
        /// <returns></returns>
        public int GetID()
        {
            return ID;
        }
        /// <summary>
        /// 设置动画器ID
        /// </summary>
        /// <param tweenName="id"></param>
        public void SetID(int id)
        {
            ID = id;
        }
        /// <summary>
        /// 获取动画器标识名称
        /// </summary>
        /// <returns></returns>
        public string GetIndicator()
        {
            return Indicator;
        }
        /// <summary>
        /// 设置动画器标识名称
        /// </summary>
        /// <param tweenName="indicator"></param>
        public void SetIndicator(string indicator)
        {
            Indicator = indicator;
        }
        #endregion
    }
}