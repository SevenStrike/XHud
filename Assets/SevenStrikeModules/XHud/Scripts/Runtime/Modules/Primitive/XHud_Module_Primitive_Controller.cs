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
    using UnityEditor;
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

        #region 成员 - 特殊组件
        /// <summary>
        /// 组件 - 按钮
        /// </summary>
        [SerializeField] public XHud_Module_Button mod_HudButton;
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
        /// <summary>
        /// 动画
        /// </summary>
        [SerializeField] public XHud_Module_Primitive_Tween pt_Tween;
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
            if (mod_HudButton == null)
                mod_HudButton = GetComponentInParent<XHud_Module_Button>();
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
        /// 获取ID
        /// </summary>
        /// <returns></returns>
        public int GetID()
        {
            return ID;
        }
        /// <summary>
        /// 设置D
        /// </summary>
        /// <param name="id"></param>
        public void SetID(int id)
        {
            ID = id;
        }
        /// <summary>
        /// 获取标识名称
        /// </summary>
        /// <returns></returns>
        public string GetIndicator()
        {
            return Indicator;
        }
        /// <summary>
        /// 设置标识名称
        /// </summary>
        /// <param name="indicator"></param>
        public void SetIndicator(string indicator)
        {
            Indicator = indicator;
        }
        #endregion

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器内初始化图元控制器
        /// </summary>
        /// <returns></returns>
        public XHud_Module_Primitive_Controller InitialComponents_For_Editor()
        {
            Undo.AddComponent(gameObject, typeof(CanvasGroup));
            Undo.AddComponent(gameObject, typeof(XHud_Module_Primitive_Painting_Synchronizer));
            XHud_Module_Primitive_Painting comp_painting = (XHud_Module_Primitive_Painting)Undo.AddComponent(gameObject, typeof(XHud_Module_Primitive_Painting));
            comp_painting.FindController();
            XHud_Module_Primitive_Feature comp_feature = (XHud_Module_Primitive_Feature)Undo.AddComponent(gameObject, typeof(XHud_Module_Primitive_Feature));
            comp_feature.FindController();
            XHud_Module_Primitive_Tween comp_tween = (XHud_Module_Primitive_Tween)Undo.AddComponent(gameObject, typeof(XHud_Module_Primitive_Tween));
            comp_tween.FindController();

            IsInitial = true;

            return this;
        }

        /// <summary>
        /// 编辑器内清理已经初始化的图元控制器
        /// </summary>
        /// <returns></returns>
        public void ClearComponents_For_Editor()
        {
            IsInitial = false;
            if (gameObject.GetComponent<CanvasGroup>() != null)
                Undo.DestroyObjectImmediate(gameObject.GetComponent<CanvasGroup>());
            if (gameObject.GetComponent<XHud_Module_Primitive_Painting_Synchronizer>() != null)
                Undo.DestroyObjectImmediate(gameObject.GetComponent<XHud_Module_Primitive_Painting_Synchronizer>());
            if (gameObject.GetComponent<XHud_Module_Primitive_Painting>() != null)
                Undo.DestroyObjectImmediate(gameObject.GetComponent<XHud_Module_Primitive_Painting>());
            if (gameObject.GetComponent<XHud_Module_Primitive_Feature>() != null)
                Undo.DestroyObjectImmediate(gameObject.GetComponent<XHud_Module_Primitive_Feature>());
            if (gameObject.GetComponent<XHud_Module_Primitive_Tween>() != null)
                Undo.DestroyObjectImmediate(gameObject.GetComponent<XHud_Module_Primitive_Tween>());
        }
#endif
    }
}