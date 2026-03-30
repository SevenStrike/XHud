namespace SevenStrikeModules.XHud
{
    using UnityEditor;
    using UnityEngine;

    [ExecuteInEditMode]
#if UNITY_EDITOR
    [CanEditMultipleObjects]
#endif
    public class XHud_Module_Primitive_Painting_Synchronizer : MonoBehaviour
    {
        /// <summary>
        /// 图元配色器
        /// </summary>
        private XHud_Module_Primitive_Painting Painting;
        /// <summary>
        /// XHud 管理器
        /// </summary>
        private XHud_Manager mgr;

        private Vector3 ActivateScale;

        private void OnEnable()
        {
            if (Application.isPlaying)
                return;

            if (Painting == null)
                Painting = GetComponent<XHud_Module_Primitive_Painting>();

            if (mgr == null)
            {
                mgr = FindFirstObjectByType<XHud_Manager>();
            }

            if (mgr.Hud_Colors != null)
            {
                mgr.Hud_Colors.act_on_ColorChanged += act_on_ColorChanged;
            }
        }

        private void act_on_ColorChanged()
        {
            UpdateLibraryColor(); // 执行逻辑
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
                return;

            if (mgr.Hud_Colors != null)
                mgr.Hud_Colors.act_on_ColorChanged -= act_on_ColorChanged;
        }

        void Update()
        {
            if (Application.isPlaying)
                return;
            UpdateLibraryColor();
        }

        /// <summary>
        /// 同步刷新HudAnimator颜色信息
        /// </summary>
        private void UpdateLibraryColor()
        {
            if (Painting != null)
            {
                //--如果使用颜色库颜色则将组件的颜色值覆盖为库中选中的颜色，否则使用原始色
                if (Painting.SyncLibraryColor)
                {
                    if (mgr == null)
                    {
                        //--在场景中找到Hud管理器                
                        mgr = FindFirstObjectByType<XHud_Manager>();
                    }

                    //--检查色卡名称是否失效
                    if (!mgr.Hud_Colors.ColorsLibrary_IsExist(Painting.ColoriseName) || string.IsNullOrEmpty(Painting.ColoriseName))
                    {
                        Painting.ColoriseName = mgr.Hud_Colors.ColorsLibrary_GetColorName(0);
                    }

                    //--判断颜色库是否正确配置
                    if (mgr != null && mgr.Hud_Colors != null)
                    {
                        if (mgr.Hud_Colors.ColorLibrary.Count > 0)
                        {
                            Painting.UpdateColor(mgr.Hud_Colors.ColorsLibrary_GetColor(Painting.ColoriseName));
                        }
                    }
                }
                else
                {
                    Painting.UpdateColor(Painting.OriginalColor);
                }

                //做缩放并还原动作来激活颜色变化
                ActivateScale = Painting.transform.localScale;
                Painting.transform.localScale = Vector3.zero;
                Painting.transform.localScale = ActivateScale;
            }
            else
            {
                Painting = GetComponent<XHud_Module_Primitive_Painting>();
            }
        }
    }
}