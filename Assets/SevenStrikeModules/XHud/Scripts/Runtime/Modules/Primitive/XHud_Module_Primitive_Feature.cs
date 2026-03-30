namespace SevenStrikeModules.XHud
{
    using UnityEngine;

    [SerializeField]
    [System.Serializable]
    /// <summary>
    /// 图元的特性参数
    /// </summary>
    public class PrimitiveFeatures
    {
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// 角度
        /// </summary>
        public Vector3 Euler;
        /// <summary>
        /// 缩放
        /// </summary>
        public Vector3 Scale;
        /// <summary>
        /// 透明度
        /// </summary>
        public float Alpha;
        /// <summary>
        /// 颜色
        /// </summary>
        public Color Color;
        /// <summary>
        /// 尺寸
        /// </summary>
        public Vector2 Size;
        /// <summary>
        /// 填充
        /// </summary>
        public float Fill;
    }

    public class XHud_Module_Primitive_Feature : MonoBehaviour
    {
        /// <summary>
        /// 图元控制器
        /// </summary>
        public XHud_Module_Primitive_Controller controller;

        #region 图元特性参数
        /// <summary>
        /// 图元特性参数
        /// </summary>
        [SerializeField] public PrimitiveFeatures PrimitiveFeatures;
        /// <summary>
        /// 第一次点击的时候记录为首次使用动画器，并且会把所有动画器涉及的参数都收集，只有第一次使用才会起效
        /// </summary>
        [SerializeField] public bool FirstSaveFeatures;
        #endregion

        #region 状态开关
        /// <summary>
        /// 调试开关
        /// </summary>
        [SerializeField] public bool Debug;
        #endregion

        void Awake()
        {

        }

        void Start()
        {

        }

        void Update()
        {

        }

        /// <summary>
        /// 记录 PrimitiveFeature 特性参数
        /// </summary>
        public void PrimitiveFeature_Save()
        {
            // 保存特性：Rect
            if (controller.mod_Rect != null)
            {
                PrimitiveFeatures.Position = controller.mod_Rect.anchoredPosition3D;
                PrimitiveFeatures.Euler = controller.mod_Rect.localEulerAngles;
                PrimitiveFeatures.Scale = controller.mod_Rect.localScale;
                PrimitiveFeatures.Size = controller.mod_Rect.sizeDelta;
            }
            // 保存特性：CanvasGroup
            if (controller.mod_CanvasGroup != null)
            {
                PrimitiveFeatures.Alpha = controller.mod_CanvasGroup.alpha;
            }
            // 保存特性：Image
            if (controller.mod_Image != null)
            {
                PrimitiveFeatures.Color = controller.mod_Image.color;
                PrimitiveFeatures.Fill = controller.mod_Image.fillAmount;
            }
            // 保存特性：Text
            else if (controller.mod_Text != null)
            {
                PrimitiveFeatures.Color = controller.mod_Text.color;
            }
            // 保存特性：TmpText
            else if (controller.mod_TmpText != null)
            {
                PrimitiveFeatures.Color = controller.mod_TmpText.color;
            }
            // 保存特性：_RawImage
            else if (controller.mod_RawImage != null)
            {
                PrimitiveFeatures.Color = controller.mod_RawImage.color;
            }
        }
        /// <summary>
        /// 还原 PrimitiveFeature 特性参数
        /// </summary>
        public void PrimitiveFeature_Load()
        {
            // 读取特性：Rect
            if (controller.mod_Rect != null)
            {
                controller.mod_Rect.anchoredPosition3D = PrimitiveFeatures.Position;
                controller.mod_Rect.localEulerAngles = PrimitiveFeatures.Euler;
                controller.mod_Rect.localScale = PrimitiveFeatures.Scale;
                controller.mod_Rect.sizeDelta = PrimitiveFeatures.Size;
            }
            // 读取特性：CanvasGroup
            if (controller.mod_CanvasGroup != null)
            {
                controller.mod_CanvasGroup.alpha = PrimitiveFeatures.Alpha;
            }
            // 读取特性：Image
            if (controller.mod_Image != null)
            {
                controller.mod_Image.color = PrimitiveFeatures.Color;
                controller.mod_Image.fillAmount = PrimitiveFeatures.Fill;
            }
            // 读取特性：Text
            else if (controller.mod_Text != null)
            {
                controller.mod_Text.color = PrimitiveFeatures.Color;
            }
            // 读取特性：TmpText
            else if (controller.mod_TmpText != null)
            {
                controller.mod_TmpText.color = PrimitiveFeatures.Color;
            }
            // 读取特性：RawImage
            else if (controller.mod_RawImage != null)
            {
                controller.mod_RawImage.color = PrimitiveFeatures.Color;
            }
        }

        /// <summary>
        /// 在自身寻找图元控制器
        /// </summary>
        public void FindController()
        {
            controller = transform.GetComponent<XHud_Module_Primitive_Controller>();
            if (controller != null)
            {
                controller.pt_Feature = this;
            }
        }
    }
}