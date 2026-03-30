namespace SevenStrikeModules.XHud
{
    using UnityEngine;

    public class XHud_Module_Primitive_Painting : MonoBehaviour
    {
        /// <summary>
        /// 图元控制器
        /// </summary>
        public XHud_Module_Primitive_Controller controller;

        #region 配色
        /// <summary>
        /// 原始色
        /// </summary>
        [SerializeField] public Color OriginalColor = Color.white;
        /// <summary>
        /// 配色名称（从颜色库中读取）
        /// </summary>
        [SerializeField] public string ColoriseName;
        #endregion

        #region 状态开关
        /// <summary>
        /// 调试开关
        /// </summary>
        [SerializeField] public bool Debug;
        /// <summary>
        /// 同步库颜色
        /// </summary>
        [SerializeField] public bool SyncLibraryColor;
        #endregion


        void Awake()
        {

        }

        void Start()
        {
            ColorSync();
        }

        void Update()
        {
            ColorSync();
        }

        #region 颜色设置
        /// <summary>
        /// 设置受控组件颜色
        /// </summary>
        /// <param name="color"></param>
        public void UpdateColor(Color color)
        {
            ////如果Animator存在于上级按钮物体下
            //if (mod_HudButton != null)
            //{
            //    //--如果 - 此Animator类型为TmpText并作为Button下挂的组件且按钮控制  文字变色  则不同步颜色
            //    if (mod_HudButton.TextColorSyncFade && mod_HudButton.ButtonTmpText != null && mod_HudButton.ButtonTmpText == mod_TmpText)
            //        return;
            //    //--如果 - 此Animator类型为Text并作为Button下挂的组件且按钮控制  文字变色  则不同步颜色
            //    if (mod_HudButton.TextColorSyncFade && mod_HudButton.ButtonText != null && mod_HudButton.ButtonText == mod_Text)
            //        return;
            //    //--如果 - 此Animator类型为Image并作为Button下挂的组件且按钮控制  图标变色  则不同步颜色
            //    if (mod_HudButton.IconColorSyncFade && mod_HudButton.IconImage == mod_Image)
            //        return;
            //    //--如果 - 此Animator类型为Image并作为Button下挂的组件且按钮控制  背景变色  则不同步颜色
            //    if (mod_HudButton.BgColorSyncFade && mod_HudButton.BgImage == mod_Image)
            //        return;
            //}

            //--如果 - 图像组件存在则使用color覆盖图像组件的颜色
            if (controller.mod_Image != null)
            {
                controller.mod_Image.color = color;
            }
            //--否则 - 如果 - 文字组件存在则使用color覆盖文字组件的颜色
            else if (controller.mod_Text != null)
            {
                if (!controller.mod_Text.TextStyleInfo.SyncAnimatorColor)
                    return;
                if (controller.mod_Text.StyleLibSynching)
                    if (controller.mod_Text.TextStyleInfo.LibStyle_Effect_color)
                        return;
                controller.mod_Text.TextStyleInfo.txt_Set_FontColor(color);
            }
            //--否则 - 如果 - 文字组件存在则使用color覆盖文字组件的颜色
            else if (controller.mod_TmpText != null)
            {
                if (!controller.mod_TmpText.TextStyleInfo.SyncAnimatorColor)
                    return;
                if (controller.mod_TmpText.StyleLibSynching)
                    if (controller.mod_TmpText.TextStyleInfo.LibStyle_Effect_color)
                        return;
                controller.mod_TmpText.TextStyleInfo.tmp_Set_FontColor(color);
            }
            //--否则 - 如果 - 原始图像组件存在则使用color覆盖原始图像组件的颜色
            else if (controller.mod_RawImage != null)
            {
                controller.mod_RawImage.color = color;
            }

            OriginalColor = color;
        }
        /// <summary>
        /// 根据色卡库匹配名同步/直接设置颜色
        /// </summary>
        public void ColorSync()
        {
            //--如果开启同步颜色库颜色则将组件的颜色值覆盖为库中选中的颜色，否则使用原始色
            if (SyncLibraryColor)
            {
                //--如果颜色库存在则设置组件颜色为指定名称的库中的颜色
                if (XHud_Manager.Instance.Hud_Colors != null)
                {
                    Color cc = XHud_Manager.Instance.Hud_Colors.ColorsLibrary_GetColor(ColoriseName);
                    UpdateColor(cc);
                }
            }
            else
            {
                //--设置组件颜色为原始色
                UpdateColor(OriginalColor);
            }
        }
        /// <summary>
        /// 设置目标色卡名称
        /// </summary>
        /// <param name="name"></param>
        public void SetColoriseName(string name)
        {
            ColoriseName = name;
        }
        /// <summary>
        /// 设置原始色
        /// </summary>
        /// <param name="name"></param>
        public void SetOriginalColor(Color color)
        {
            OriginalColor = color;
        }
        #endregion

        /// <summary>
        /// 在自身寻找图元控制器
        /// </summary>
        public void FindController()
        {
            controller = transform.GetComponent<XHud_Module_Primitive_Controller>();
            if (controller != null)
            {
                controller.pt_Painting = this;
            }
        }
    }
}