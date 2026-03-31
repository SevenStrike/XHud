namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using UnityEngine;

    /// <summary>
    /// 锚点结构 - 布局构图节点
    /// </summary>
    [System.Serializable]
    public class AuxiliaryAnchor_Layout
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 锚点
        /// </summary>
        public RectTransform Anchor;
        /// <summary>
        /// 锚点类型
        /// </summary>
        public HudAnchors_CompGuide Type;
    }

    public partial class XHud_Manager : MonoBehaviour
    {
        [Tooltip("Hud画布-辅助构图锚点")]
        /// <summary>
        /// XHud画布-辅助构图锚点
        /// </summary>
        public RectTransform CompGuide_AnchorRoot;
        [SerializeField]
        /// <summary>
        /// 布局构图参考线锚点集合
        /// </summary>
        public AuxiliaryAnchor_Layout[] CompGuide_Anchors;
        [SerializeField]
        public string CompGuideMode = "水平对称";
        [SerializeField]
        private float GuideParam_CenterPointSize = 1f;
        /// <summary>
        /// 是否启用参考构图模式
        /// </summary>
        public bool UseCompGuide;

        #region 左右对称构图
        [SerializeField]
        [Range(-1, 1)]
        public float GuideParam_Mirror_LR_Offset;
        [SerializeField]
        public string GuideParam_Mirror_LR_GoldenMode = "自定义对称分割";
        #endregion

        #region 上下对称构图
        [SerializeField]
        [Range(-1, 1)]
        public float GuideParam_Mirror_UD_Offset;
        [SerializeField]
        public string GuideParam_Mirror_UD_GoldenMode = "自定义对称分割";
        #endregion

        #region 黄金螺旋构图      
        [SerializeField]
        public string GuideParam_Fibonacci_Mode = "右上";
        private Vector3 previousPoint;
        #endregion

        #region 对角线构图      
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_CornerLookat_Offset_H;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_CornerLookat_Offset_V;
        #endregion

        #region 三分线构图      
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Three_Offset_H = 0.5f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Three_Offset_V = 0.5f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Three_Offset_Coverage = 0.5f;
        #endregion

        #region 引导线构图
        [SerializeField]
        [Range(-1, 1)]
        private float GuideParam_GuideLine_BaseOffset = 0;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_GuideLine_BaseHeight = 0.5f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_GuideLine_Offset_Far;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_GuideLine_Offset_Near;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_GuideLine_Offset_NearHeight;
        #endregion

        #region 三角构图
        [SerializeField]
        [Range(-1, 1)]
        private float GuideParam_Triangle_TopOffset = 0f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Triangle_BaseHeight = 0.8f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Triangle_BottomHeight = 0.2f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Triangle_Offset_Left = 0.5f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Triangle_Offset_Right = 0.5f;
        #endregion

        #region 工字型
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_IShape_TopHeight = 0.8f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_IShape_BottomHeight = 0.2f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_IShape_Offset_Left = 0.5f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_IShape_Offset_Right = 0.5f;
        #endregion

        public Color GuideColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        public Color GuidePointColor = XHud_Dashboard.Theme_Primary;

        /// <summary>
        /// 绘制屏幕构图参考线
        /// </summary>
        private void hm_CompGuide_Draw()
        {
            switch (CompGuideMode)
            {
                case "水平对称":
                    hm_CompGuide_LeftRightMirror();
                    break;
                case "垂直对称":
                    hm_CompGuide_UpDownMirror();
                    break;
                case "黄金螺旋":
                    hm_CompGuide_GoldenSpiral();
                    break;
                case "对角线":
                    hm_CompGuide_DiagonalLine();
                    break;
                case "三分线":
                    hm_CompGuide_ThreeCut();
                    break;
                case "引导线":
                    hm_CompGuide_GuideLineRef();
                    break;
                case "三角":
                    hm_CompGuide_Triangle();
                    break;
                case "工字型":
                    hm_CompGuide_IShape();
                    break;
            }
        }

        /// <summary>
        /// 左右对称
        /// </summary>
        private void hm_CompGuide_LeftRightMirror()
        {
            if (!UseCompGuide)
                return;


            RectTransform rect_up = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.上);
            RectTransform rect_leftup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左上);
            RectTransform rect_down = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.下);
            RectTransform rect_leftdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左下);

            Vector3 start = Vector3.zero;
            Vector3 end = Vector3.zero;

            if (GuideParam_Mirror_LR_GoldenMode == "自定义对称分割")
            {
                start = rect_up.TransformPoint(rect_up.anchoredPosition3D + Vector3.right * GuideParam_Mirror_LR_Offset * ScreenRes.x / 2);
                end = rect_down.TransformPoint(rect_down.anchoredPosition3D + Vector3.right * GuideParam_Mirror_LR_Offset * ScreenRes.x / 2);
            }
            else if (GuideParam_Mirror_LR_GoldenMode == "靠右黄金比例")
            {
                start = rect_leftup.TransformPoint(rect_leftup.anchoredPosition3D + Vector3.right * ScreenRes.x / 1.618f);
                end = rect_leftdown.TransformPoint(rect_leftdown.anchoredPosition3D + Vector3.right * ScreenRes.x / 1.618f);
            }
            else if (GuideParam_Mirror_LR_GoldenMode == "靠左黄金比例")
            {
                float glod = ScreenRes.x / 1.618f;
                start = rect_leftup.TransformPoint(rect_leftup.anchoredPosition3D + Vector3.right * (ScreenRes.x - glod));
                end = rect_leftdown.TransformPoint(rect_leftdown.anchoredPosition3D + Vector3.right * (ScreenRes.x - glod));
            }

            Gizmos.color = GuidePointColor;
            Vector3 center = hm_CalculateMidpoint(start, end);
            Gizmos.DrawSphere(center, GuideParam_CenterPointSize * 0.005f);

            Gizmos.color = GuideColor;
            Gizmos.DrawLine(start, end);
            Gizmos.color = Color.white;
        }
        /// <summary>
        /// 上下对称
        /// </summary>
        private void hm_CompGuide_UpDownMirror()
        {
            if (!UseCompGuide)
                return;


            RectTransform rect_left = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左);
            RectTransform rect_leftup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左上);
            RectTransform rect_right = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右);
            RectTransform rect_rightup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右上);

            Vector3 start = Vector3.zero;
            Vector3 end = Vector3.zero;

            if (GuideParam_Mirror_UD_GoldenMode == "自定义对称分割")
            {
                start = rect_left.TransformPoint(rect_left.anchoredPosition3D + Vector3.up * GuideParam_Mirror_UD_Offset * ScreenRes.y / 2);
                end = rect_right.TransformPoint(rect_right.anchoredPosition3D + Vector3.up * GuideParam_Mirror_UD_Offset * ScreenRes.y / 2);
            }
            else if (GuideParam_Mirror_UD_GoldenMode == "靠上黄金比例")
            {
                float glod = ScreenRes.y / 1.618f;
                start = rect_leftup.TransformPoint(rect_leftup.anchoredPosition3D - Vector3.up * (ScreenRes.y - glod));
                end = rect_rightup.TransformPoint(rect_rightup.anchoredPosition3D - Vector3.up * (ScreenRes.y - glod));
            }
            else if (GuideParam_Mirror_UD_GoldenMode == "靠下黄金比例")
            {
                start = rect_leftup.TransformPoint(rect_leftup.anchoredPosition3D - Vector3.up * ScreenRes.y / 1.618f);
                end = rect_rightup.TransformPoint(rect_rightup.anchoredPosition3D - Vector3.up * ScreenRes.y / 1.618f);
            }

            Gizmos.color = GuidePointColor;
            Vector3 center = hm_CalculateMidpoint(start, end);
            Gizmos.DrawSphere(center, GuideParam_CenterPointSize * 0.005f);

            Gizmos.color = GuideColor;
            Gizmos.DrawLine(start, end);
            Gizmos.color = Color.white;
        }
        /// <summary>
        /// 黄金螺旋
        /// </summary>
        private void hm_CompGuide_GoldenSpiral()
        {
            if (!UseCompGuide)
                return;

            float scr_w = ScreenRes.x;
            float scr_h = ScreenRes.y;

            float gold_w = scr_w / 1.618f;
            float gold_h = scr_h / 1.618f;

            if (GuideParam_Fibonacci_Mode == "右上")
            {
                RectTransform rect_leftup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左上);
                Vector3 start_pos = rect_leftup.anchoredPosition3D;

                #region 第0分割
                Vector3 pos_s_v0 = start_pos + (Vector3.right * gold_w);
                Vector3 start_v0 = rect_leftup.TransformPoint(pos_s_v0);

                Vector3 pos_e_v0 = pos_s_v0 + start_pos - (Vector3.up * scr_h);
                Vector3 end_v0 = rect_leftup.TransformPoint(pos_e_v0);

                Gizmos.color = GuideColor;
                Gizmos.DrawLine(start_v0, end_v0);

                Vector3 curve_calc_v0_p1 = start_pos - (Vector3.up * scr_h / 2);
                Vector3 curve_v0_p1 = rect_leftup.TransformPoint(curve_calc_v0_p1);

                Vector3 curve_calc_v0_p2 = pos_e_v0 - (Vector3.right * gold_w / 2);
                Vector3 curve_v0_p2 = rect_leftup.TransformPoint(curve_calc_v0_p2);

                hm_DrawBezierCurve(rect_leftup.position, curve_v0_p1, curve_v0_p2, end_v0);
                #endregion

                #region 第1分割
                float dis_v1_h = scr_h - gold_h;
                float dis_v1_w = scr_w - gold_w;

                Vector3 pos_s_v1 = pos_s_v0 - (start_pos + Vector3.up * dis_v1_h);
                Vector3 start_v1 = rect_leftup.TransformPoint(pos_s_v1);

                Vector3 pos_e_v1 = pos_s_v1 + (start_pos + (Vector3.right * dis_v1_w));
                Vector3 end_v1 = rect_leftup.TransformPoint(pos_e_v1);

                Gizmos.DrawLine(start_v1, end_v1);

                Vector3 curve_calc_v1_p1 = pos_s_v1 - (Vector3.up * (gold_h)) + (Vector3.right * dis_v1_w / 2);
                Vector3 curve_v1_p1 = rect_leftup.TransformPoint(curve_calc_v1_p1);

                Vector3 curve_calc_v1_p2 = pos_s_v1 - (Vector3.up * (gold_h / 2)) + (Vector3.right * dis_v1_w);
                Vector3 curve_v1_p2 = rect_leftup.TransformPoint(curve_calc_v1_p2);

                hm_DrawBezierCurve(end_v0, curve_v1_p1, curve_v1_p2, end_v1);
                #endregion

                #region 第2分割
                float dis_v2_h = dis_v1_h;
                float dis_v2_w = dis_v1_w - (dis_v1_w / 1.618f);

                Vector3 pos_s_v2 = pos_s_v0 + (start_pos + Vector3.right * dis_v2_w);
                Vector3 start_v2 = rect_leftup.TransformPoint(pos_s_v2);

                Vector3 pos_e_v2 = pos_s_v2 - (start_pos + Vector3.up * dis_v2_h);
                Vector3 end_v2 = rect_leftup.TransformPoint(pos_e_v2);

                Gizmos.DrawLine(start_v2, end_v2);

                Vector3 curve_calc_v2_p1 = pos_s_v2 - (Vector3.up * dis_v2_h / 2) + (Vector3.right * (dis_v1_w / 1.618f));
                Vector3 curve_v2_p1 = rect_leftup.TransformPoint(curve_calc_v2_p1);

                Vector3 curve_calc_v2_p2 = pos_s_v2 + (Vector3.right * (dis_v1_w / 1.618f) / 2);
                Vector3 curve_v2_p2 = rect_leftup.TransformPoint(curve_calc_v2_p2);

                hm_DrawBezierCurve(end_v1, curve_v2_p1, curve_v2_p2, start_v2);
                #endregion

                #region 第3分割
                float dis_v3_h = (dis_v2_h / 1.618f);
                float dis_v3_w = dis_v2_w;

                Vector3 pos_s_v3 = pos_s_v0 - (start_pos + Vector3.up * dis_v3_h);
                Vector3 start_v3 = rect_leftup.TransformPoint(pos_s_v3);

                Vector3 pos_e_v3 = pos_s_v3 + (start_pos + Vector3.right * dis_v3_w);
                Vector3 end_v3 = rect_leftup.TransformPoint(pos_e_v3);

                Gizmos.DrawLine(start_v3, end_v3);

                Vector3 curve_calc_v3_p1 = pos_s_v2 - (Vector3.right * (dis_v3_w / 2));
                Vector3 curve_v3_p1 = rect_leftup.TransformPoint(curve_calc_v3_p1);

                Vector3 curve_calc_v3_p2 = pos_e_v3 - (Vector3.right * dis_v3_w) + (Vector3.up * (dis_v3_h / 2));
                Vector3 curve_v3_p2 = rect_leftup.TransformPoint(curve_calc_v3_p2);

                hm_DrawBezierCurve(start_v2, curve_v3_p1, curve_v3_p2, start_v3);
                #endregion

                #region 第4分割
                float dis_v4_h = (dis_v3_h / 1.618f);
                float dis_v4_w = (dis_v3_w / 1.618f);

                Vector3 pos_s_v4 = pos_s_v3 + (start_pos + Vector3.right * dis_v4_w);
                Vector3 start_v4 = rect_leftup.TransformPoint(pos_s_v4);

                Vector3 pos_e_v4 = pos_s_v4 - (start_pos + Vector3.up * dis_v4_h);
                Vector3 end_v4 = rect_leftup.TransformPoint(pos_e_v4);

                Gizmos.DrawLine(start_v4, end_v4);

                Vector3 curve_calc_v4_p1 = pos_s_v3 - (Vector3.up * (dis_v4_h / 2));
                Vector3 curve_v4_p1 = rect_leftup.TransformPoint(curve_calc_v4_p1);

                Vector3 curve_calc_v4_p2 = pos_e_v4 - (Vector3.right * dis_v4_w / 2);
                Vector3 curve_v4_p2 = rect_leftup.TransformPoint(curve_calc_v4_p2);

                hm_DrawBezierCurve(start_v3, curve_v4_p1, curve_v4_p2, end_v4);
                #endregion

                #region 第5分割
                float dis_v5_h = dis_v4_h - (dis_v4_h / 1.618f);
                float dis_v5_w = dis_v4_w / 1.618f;

                Vector3 pos_s_v5 = pos_s_v4 - (start_pos + Vector3.up * dis_v5_h);
                Vector3 start_v5 = rect_leftup.TransformPoint(pos_s_v5);

                Vector3 pos_e_v5 = pos_s_v5 + (start_pos + Vector3.right * dis_v5_w);
                Vector3 end_v5 = rect_leftup.TransformPoint(pos_e_v5);

                Gizmos.DrawLine(start_v5, end_v5);

                Vector3 curve_calc_v5_p1 = pos_e_v4 + (Vector3.right * dis_v5_w / 2);
                Vector3 curve_v5_p1 = rect_leftup.TransformPoint(curve_calc_v5_p1);

                Vector3 curve_calc_v5_p2 = pos_e_v5 - (Vector3.up * ((dis_v4_h / 1.618f) / 2));
                Vector3 curve_v5_p2 = rect_leftup.TransformPoint(curve_calc_v5_p2);

                hm_DrawBezierCurve(end_v4, curve_v5_p1, curve_v5_p2, end_v5);
                #endregion

                #region 第6分割
                float dis_v6_h = dis_v5_h;
                float dis_v6_w = dis_v5_w - (dis_v5_w / 1.618f);

                Vector3 pos_s_v6 = pos_s_v5 + (start_pos + Vector3.right * dis_v6_w);
                Vector3 start_v6 = rect_leftup.TransformPoint(pos_s_v6);

                Vector3 pos_e_v6 = pos_s_v6 + (start_pos + Vector3.up * dis_v6_h);
                Vector3 end_v6 = rect_leftup.TransformPoint(pos_e_v6);

                Gizmos.DrawLine(start_v6, end_v6);

                Vector3 curve_calc_v6_p1 = pos_e_v5 + (Vector3.up * (dis_v6_h / 2));
                Vector3 curve_v6_p1 = rect_leftup.TransformPoint(curve_calc_v6_p1);

                Vector3 curve_calc_v6_p2 = pos_e_v6 + (Vector3.right * ((dis_v5_w / 1.618f) / 2));
                Vector3 curve_v6_p2 = rect_leftup.TransformPoint(curve_calc_v6_p2);

                hm_DrawBezierCurve(end_v5, curve_v6_p1, curve_v6_p2, end_v6);
                #endregion

                #region 第7分割
                float dis_v7_h = dis_v6_h - (dis_v6_h / 1.618f);
                float dis_v7_w = dis_v6_w;

                Vector3 pos_s_v7 = pos_s_v5 + (start_pos + Vector3.up * dis_v7_h);
                Vector3 start_v7 = rect_leftup.TransformPoint(pos_s_v7);

                Vector3 pos_e_v7 = pos_s_v7 + (start_pos + Vector3.right * dis_v7_w);
                Vector3 end_v7 = rect_leftup.TransformPoint(pos_e_v7);

                Gizmos.DrawLine(start_v7, end_v7);

                Vector3 curve_calc_v7_p1 = pos_e_v6 - (Vector3.right * (dis_v7_w / 2));
                Vector3 curve_v7_p1 = rect_leftup.TransformPoint(curve_calc_v7_p1);

                Vector3 curve_calc_v7_p2 = pos_s_v7 + (Vector3.up * ((dis_v6_h / 1.618f) / 2));
                Vector3 curve_v7_p2 = rect_leftup.TransformPoint(curve_calc_v7_p2);

                hm_DrawBezierCurve(end_v6, curve_v7_p1, curve_v7_p2, start_v7);
                #endregion

                #region 第8分割
                float dis_v8_h = dis_v7_h;
                float dis_v8_w = (dis_v7_w / 1.618f);

                Vector3 pos_s_v8 = pos_s_v7 + (start_pos + Vector3.right * dis_v8_w);
                Vector3 start_v8 = rect_leftup.TransformPoint(pos_s_v8);

                Vector3 pos_e_v8 = pos_s_v8 - (start_pos + Vector3.up * dis_v8_h);
                Vector3 end_v8 = rect_leftup.TransformPoint(pos_e_v8);

                Gizmos.DrawLine(start_v8, end_v8);

                Vector3 curve_calc_v8_p1 = pos_s_v7 - (Vector3.up * (dis_v8_h / 2));
                Vector3 curve_v8_p1 = rect_leftup.TransformPoint(curve_calc_v8_p1);

                Vector3 curve_calc_v8_p2 = pos_e_v8 - (Vector3.right * (dis_v8_w / 2));
                Vector3 curve_v8_p2 = rect_leftup.TransformPoint(curve_calc_v8_p2);

                hm_DrawBezierCurve(start_v7, curve_v8_p1, curve_v8_p2, end_v8);
                #endregion

                #region 第9分割
                float dis_v9_h = dis_v8_h - (dis_v8_h / 1.618f);
                float dis_v9_w = dis_v8_w / 1.618f;

                Vector3 pos_s_v9 = pos_s_v8 - (start_pos + Vector3.up * dis_v9_h);
                Vector3 start_v9 = rect_leftup.TransformPoint(pos_s_v9);

                Vector3 pos_e_v9 = pos_s_v9 + (start_pos + Vector3.right * dis_v9_w);
                Vector3 end_v9 = rect_leftup.TransformPoint(pos_e_v9);

                Gizmos.DrawLine(start_v9, end_v9);

                Vector3 curve_calc_v9_p1 = pos_e_v8 + (Vector3.right * (dis_v9_w / 2));
                Vector3 curve_v9_p1 = rect_leftup.TransformPoint(curve_calc_v9_p1);

                Vector3 curve_calc_v9_p2 = pos_e_v9 - (Vector3.up * (dis_v9_h / 2));
                Vector3 curve_v9_p2 = rect_leftup.TransformPoint(curve_calc_v9_p2);

                hm_DrawBezierCurve(end_v8, curve_v9_p1, curve_v9_p2, end_v9);
                #endregion
            }
            else if (GuideParam_Fibonacci_Mode == "右下")
            {
                RectTransform rect_leftdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左下);
                Vector3 start_pos = rect_leftdown.anchoredPosition3D;

                #region 第0分割
                Vector3 pos_s_v0 = start_pos + (Vector3.right * gold_w);
                Vector3 start_v0 = rect_leftdown.TransformPoint(pos_s_v0);

                Vector3 pos_e_v0 = pos_s_v0 + start_pos + (Vector3.up * scr_h);
                Vector3 end_v0 = rect_leftdown.TransformPoint(pos_e_v0);

                Gizmos.color = GuideColor;
                Gizmos.DrawLine(start_v0, end_v0);

                Vector3 curve_calc_v0_p1 = start_pos + (Vector3.up * scr_h / 2);
                Vector3 curve_v0_p1 = rect_leftdown.TransformPoint(curve_calc_v0_p1);

                Vector3 curve_calc_v0_p2 = pos_e_v0 - (Vector3.right * gold_w / 2);
                Vector3 curve_v0_p2 = rect_leftdown.TransformPoint(curve_calc_v0_p2);

                hm_DrawBezierCurve(rect_leftdown.position, curve_v0_p1, curve_v0_p2, end_v0);
                #endregion

                #region 第1分割
                float dis_v1_h = (scr_h - gold_h);
                float dis_v1_w = scr_w - gold_w;

                Vector3 pos_s_v1 = pos_s_v0 + (start_pos + Vector3.up * dis_v1_h);
                Vector3 start_v1 = rect_leftdown.TransformPoint(pos_s_v1);

                Vector3 pos_e_v1 = pos_s_v1 + (start_pos + (Vector3.right * dis_v1_w));
                Vector3 end_v1 = rect_leftdown.TransformPoint(pos_e_v1);

                Gizmos.DrawLine(start_v1, end_v1);

                Vector3 curve_calc_v1_p1 = pos_e_v0 + (Vector3.right * (dis_v1_w / 2));
                Vector3 curve_v1_p1 = rect_leftdown.TransformPoint(curve_calc_v1_p1);

                Vector3 curve_calc_v1_p2 = pos_e_v1 + (Vector3.up * (gold_h / 2));
                Vector3 curve_v1_p2 = rect_leftdown.TransformPoint(curve_calc_v1_p2);

                hm_DrawBezierCurve(end_v0, curve_v1_p1, curve_v1_p2, end_v1);
                #endregion

                #region 第2分割
                float dis_v2_h = dis_v1_h;
                float dis_v2_w = dis_v1_w - (dis_v1_w / 1.618f);

                Vector3 pos_s_v2 = pos_s_v0 + (start_pos + Vector3.right * dis_v2_w);
                Vector3 start_v2 = rect_leftdown.TransformPoint(pos_s_v2);

                Vector3 pos_e_v2 = pos_s_v2 + (start_pos + Vector3.up * dis_v2_h);
                Vector3 end_v2 = rect_leftdown.TransformPoint(pos_e_v2);

                Gizmos.DrawLine(start_v2, end_v2);

                Vector3 curve_calc_v2_p1 = pos_e_v1 - (Vector3.up * (dis_v2_h / 2));
                Vector3 curve_v2_p1 = rect_leftdown.TransformPoint(curve_calc_v2_p1);

                Vector3 curve_calc_v2_p2 = pos_s_v2 + (Vector3.right * ((dis_v1_w / 1.618f) / 2));
                Vector3 curve_v2_p2 = rect_leftdown.TransformPoint(curve_calc_v2_p2);

                hm_DrawBezierCurve(end_v1, curve_v2_p1, curve_v2_p2, start_v2);
                #endregion

                #region 第3分割
                float dis_v3_h = (dis_v2_h / 1.618f);
                float dis_v3_w = dis_v2_w;

                Vector3 pos_s_v3 = pos_s_v0 + (start_pos + Vector3.up * dis_v3_h);
                Vector3 start_v3 = rect_leftdown.TransformPoint(pos_s_v3);

                Vector3 pos_e_v3 = pos_s_v3 + (start_pos + Vector3.right * dis_v3_w);
                Vector3 end_v3 = rect_leftdown.TransformPoint(pos_e_v3);

                Gizmos.DrawLine(start_v3, end_v3);

                Vector3 curve_calc_v3_p1 = pos_s_v2 - (Vector3.right * (dis_v3_w / 2));
                Vector3 curve_v3_p1 = rect_leftdown.TransformPoint(curve_calc_v3_p1);

                Vector3 curve_calc_v3_p2 = pos_s_v3 - (Vector3.up * (dis_v3_h / 2));
                Vector3 curve_v3_p2 = rect_leftdown.TransformPoint(curve_calc_v3_p2);

                hm_DrawBezierCurve(start_v2, curve_v3_p1, curve_v3_p2, start_v3);
                #endregion

                #region 第4分割
                float dis_v4_h = (dis_v3_h / 1.618f);
                float dis_v4_w = (dis_v3_w / 1.618f);

                Vector3 pos_s_v4 = pos_s_v3 + (start_pos + Vector3.right * dis_v4_w);
                Vector3 start_v4 = rect_leftdown.TransformPoint(pos_s_v4);

                Vector3 pos_e_v4 = pos_s_v4 + (start_pos + Vector3.up * dis_v4_h);
                Vector3 end_v4 = rect_leftdown.TransformPoint(pos_e_v4);

                Gizmos.DrawLine(start_v4, end_v4);

                Vector3 curve_calc_v4_p1 = pos_s_v3 + (Vector3.up * (dis_v4_h / 2));
                Vector3 curve_v4_p1 = rect_leftdown.TransformPoint(curve_calc_v4_p1);

                Vector3 curve_calc_v4_p2 = pos_e_v4 - (Vector3.right * (dis_v4_w / 2));
                Vector3 curve_v4_p2 = rect_leftdown.TransformPoint(curve_calc_v4_p2);

                hm_DrawBezierCurve(start_v3, curve_v4_p1, curve_v4_p2, end_v4);
                #endregion

                #region 第5分割
                float dis_v5_h = dis_v4_h - (dis_v4_h / 1.618f);
                float dis_v5_w = dis_v4_w / 1.618f;

                Vector3 pos_s_v5 = pos_s_v4 + (start_pos + Vector3.up * dis_v5_h);
                Vector3 start_v5 = rect_leftdown.TransformPoint(pos_s_v5);

                Vector3 pos_e_v5 = pos_s_v5 + (start_pos + Vector3.right * dis_v5_w);
                Vector3 end_v5 = rect_leftdown.TransformPoint(pos_e_v5);

                Gizmos.DrawLine(start_v5, end_v5);

                Vector3 curve_calc_v5_p1 = pos_e_v4 + (Vector3.right * (dis_v5_w / 2));
                Vector3 curve_v5_p1 = rect_leftdown.TransformPoint(curve_calc_v5_p1);

                Vector3 curve_calc_v5_p2 = pos_e_v5 + (Vector3.up * ((dis_v4_h / 1.618f) / 2));
                Vector3 curve_v5_p2 = rect_leftdown.TransformPoint(curve_calc_v5_p2);

                hm_DrawBezierCurve(end_v4, curve_v5_p1, curve_v5_p2, end_v5);
                #endregion

                #region 第6分割
                float dis_v6_h = dis_v5_h;
                float dis_v6_w = dis_v5_w - (dis_v5_w / 1.618f);

                Vector3 pos_s_v6 = pos_s_v5 + (start_pos + Vector3.right * dis_v6_w);
                Vector3 start_v6 = rect_leftdown.TransformPoint(pos_s_v6);

                Vector3 pos_e_v6 = pos_s_v6 - (start_pos + Vector3.up * dis_v6_h);
                Vector3 end_v6 = rect_leftdown.TransformPoint(pos_e_v6);

                Gizmos.DrawLine(start_v6, end_v6);

                Vector3 curve_calc_v6_p1 = pos_e_v5 - (Vector3.up * (dis_v6_h / 2));
                Vector3 curve_v6_p1 = rect_leftdown.TransformPoint(curve_calc_v6_p1);

                Vector3 curve_calc_v6_p2 = pos_e_v6 + (Vector3.right * (dis_v5_w / 1.618f / 2));
                Vector3 curve_v6_p2 = rect_leftdown.TransformPoint(curve_calc_v6_p2);

                hm_DrawBezierCurve(end_v5, curve_v6_p1, curve_v6_p2, end_v6);
                #endregion

                #region 第7分割
                float dis_v7_h = dis_v6_h - (dis_v6_h / 1.618f);
                float dis_v7_w = dis_v6_w;

                Vector3 pos_s_v7 = pos_s_v5 - (start_pos + Vector3.up * dis_v7_h);
                Vector3 start_v7 = rect_leftdown.TransformPoint(pos_s_v7);

                Vector3 pos_e_v7 = pos_s_v7 + (start_pos + Vector3.right * dis_v7_w);
                Vector3 end_v7 = rect_leftdown.TransformPoint(pos_e_v7);

                Gizmos.DrawLine(start_v7, end_v7);

                Vector3 curve_calc_v7_p1 = pos_e_v6 - (Vector3.right * (dis_v7_w / 2));
                Vector3 curve_v7_p1 = rect_leftdown.TransformPoint(curve_calc_v7_p1);

                Vector3 curve_calc_v7_p2 = pos_s_v7 - (Vector3.up * ((dis_v6_h / 1.618f) / 2));
                Vector3 curve_v7_p2 = rect_leftdown.TransformPoint(curve_calc_v7_p2);

                hm_DrawBezierCurve(end_v6, curve_v7_p1, curve_v7_p2, start_v7);
                #endregion

                #region 第8分割
                float dis_v8_h = dis_v7_h;
                float dis_v8_w = (dis_v7_w / 1.618f);

                Vector3 pos_s_v8 = pos_s_v7 + (start_pos + Vector3.right * dis_v8_w);
                Vector3 start_v8 = rect_leftdown.TransformPoint(pos_s_v8);

                Vector3 pos_e_v8 = pos_s_v8 + (start_pos + Vector3.up * dis_v8_h);
                Vector3 end_v8 = rect_leftdown.TransformPoint(pos_e_v8);

                Gizmos.DrawLine(start_v8, end_v8);

                Vector3 curve_calc_v8_p1 = pos_s_v7 + (Vector3.up * (dis_v8_h / 2));
                Vector3 curve_v8_p1 = rect_leftdown.TransformPoint(curve_calc_v8_p1);

                Vector3 curve_calc_v8_p2 = pos_e_v8 - (Vector3.right * (dis_v8_w / 2));
                Vector3 curve_v8_p2 = rect_leftdown.TransformPoint(curve_calc_v8_p2);

                hm_DrawBezierCurve(start_v7, curve_v8_p1, curve_v8_p2, end_v8);
                #endregion

                #region 第9分割
                float dis_v9_h = dis_v8_h - (dis_v8_h / 1.618f);
                float dis_v9_w = dis_v8_w / 1.618f;

                Vector3 pos_s_v9 = pos_s_v8 + (start_pos + Vector3.up * dis_v9_h);
                Vector3 start_v9 = rect_leftdown.TransformPoint(pos_s_v9);

                Vector3 pos_e_v9 = pos_s_v9 + (start_pos + Vector3.right * dis_v9_w);
                Vector3 end_v9 = rect_leftdown.TransformPoint(pos_e_v9);

                Gizmos.DrawLine(start_v9, end_v9);

                Vector3 curve_calc_v9_p1 = pos_e_v8 + (Vector3.right * (dis_v9_w / 2));
                Vector3 curve_v9_p1 = rect_leftdown.TransformPoint(curve_calc_v9_p1);

                Vector3 curve_calc_v9_p2 = pos_e_v9 + (Vector3.up * ((dis_v8_h / 1.618f) / 2));
                Vector3 curve_v9_p2 = rect_leftdown.TransformPoint(curve_calc_v9_p2);

                hm_DrawBezierCurve(end_v8, curve_v9_p1, curve_v9_p2, end_v9);
                #endregion
            }
            else if (GuideParam_Fibonacci_Mode == "左下")
            {
                RectTransform rect_rightdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右下);
                Vector3 start_pos = rect_rightdown.anchoredPosition3D;

                #region 第0分割
                Vector3 pos_s_v0 = start_pos - (Vector3.right * gold_w);
                Vector3 start_v0 = rect_rightdown.TransformPoint(pos_s_v0);

                Vector3 pos_e_v0 = pos_s_v0 + start_pos + (Vector3.up * scr_h);
                Vector3 end_v0 = rect_rightdown.TransformPoint(pos_e_v0);

                Gizmos.color = GuideColor;
                Gizmos.DrawLine(start_v0, end_v0);

                Vector3 curve_calc_v0_p1 = start_pos + (Vector3.up * scr_h / 2);
                Vector3 curve_v0_p1 = rect_rightdown.TransformPoint(curve_calc_v0_p1);

                Vector3 curve_calc_v0_p2 = pos_e_v0 + (Vector3.right * gold_w / 2);
                Vector3 curve_v0_p2 = rect_rightdown.TransformPoint(curve_calc_v0_p2);

                hm_DrawBezierCurve(rect_rightdown.position, curve_v0_p1, curve_v0_p2, end_v0);
                #endregion

                #region 第1分割
                float dis_v1_h = (scr_h - gold_h);
                float dis_v1_w = scr_w - gold_w;

                Vector3 pos_s_v1 = pos_s_v0 + (start_pos + Vector3.up * dis_v1_h);
                Vector3 start_v1 = rect_rightdown.TransformPoint(pos_s_v1);

                Vector3 pos_e_v1 = pos_s_v1 - (start_pos + (Vector3.right * dis_v1_w));
                Vector3 end_v1 = rect_rightdown.TransformPoint(pos_e_v1);

                Gizmos.DrawLine(start_v1, end_v1);

                Vector3 curve_calc_v1_p1 = pos_e_v0 - (Vector3.right * (dis_v1_w / 2));
                Vector3 curve_v1_p1 = rect_rightdown.TransformPoint(curve_calc_v1_p1);

                Vector3 curve_calc_v1_p2 = pos_e_v1 + (Vector3.up * gold_h / 2);
                Vector3 curve_v1_p2 = rect_rightdown.TransformPoint(curve_calc_v1_p2);

                hm_DrawBezierCurve(end_v0, curve_v1_p1, curve_v1_p2, end_v1);
                #endregion

                #region 第2分割
                float dis_v2_h = dis_v1_h;
                float dis_v2_w = dis_v1_w - (dis_v1_w / 1.618f);

                Vector3 pos_s_v2 = pos_s_v0 - (start_pos + Vector3.right * dis_v2_w);
                Vector3 start_v2 = rect_rightdown.TransformPoint(pos_s_v2);

                Vector3 pos_e_v2 = pos_s_v2 + (start_pos + Vector3.up * dis_v2_h);
                Vector3 end_v2 = rect_rightdown.TransformPoint(pos_e_v2);

                Gizmos.DrawLine(start_v2, end_v2);

                Vector3 curve_calc_v2_p1 = pos_e_v1 - (Vector3.up * (dis_v2_h / 2));
                Vector3 curve_v2_p1 = rect_rightdown.TransformPoint(curve_calc_v2_p1);

                Vector3 curve_calc_v2_p2 = pos_s_v2 - (Vector3.right * ((dis_v1_w / 1.618f) / 2));
                Vector3 curve_v2_p2 = rect_rightdown.TransformPoint(curve_calc_v2_p2);

                hm_DrawBezierCurve(end_v1, curve_v2_p1, curve_v2_p2, start_v2);
                #endregion

                #region 第3分割
                float dis_v3_h = (dis_v2_h / 1.618f);
                float dis_v3_w = dis_v2_w;

                Vector3 pos_s_v3 = pos_s_v0 + (start_pos + Vector3.up * dis_v3_h);
                Vector3 start_v3 = rect_rightdown.TransformPoint(pos_s_v3);

                Vector3 pos_e_v3 = pos_s_v3 - (start_pos + Vector3.right * dis_v3_w);
                Vector3 end_v3 = rect_rightdown.TransformPoint(pos_e_v3);

                Gizmos.DrawLine(start_v3, end_v3);

                Vector3 curve_calc_v3_p1 = pos_s_v2 + (Vector3.right * (dis_v3_w / 2));
                Vector3 curve_v3_p1 = rect_rightdown.TransformPoint(curve_calc_v3_p1);

                Vector3 curve_calc_v3_p2 = pos_s_v3 - (Vector3.up * (dis_v3_h / 2));
                Vector3 curve_v3_p2 = rect_rightdown.TransformPoint(curve_calc_v3_p2);

                hm_DrawBezierCurve(start_v2, curve_v3_p1, curve_v3_p2, start_v3);
                #endregion

                #region 第4分割
                float dis_v4_h = (dis_v3_h / 1.618f);
                float dis_v4_w = (dis_v3_w / 1.618f);

                Vector3 pos_s_v4 = pos_s_v3 - (start_pos + Vector3.right * dis_v4_w);
                Vector3 start_v4 = rect_rightdown.TransformPoint(pos_s_v4);

                Vector3 pos_e_v4 = pos_s_v4 + (start_pos + Vector3.up * dis_v4_h);
                Vector3 end_v4 = rect_rightdown.TransformPoint(pos_e_v4);

                Gizmos.DrawLine(start_v4, end_v4);

                Vector3 curve_calc_v4_p1 = pos_s_v3 + (Vector3.up * (dis_v4_h / 2));
                Vector3 curve_v4_p1 = rect_rightdown.TransformPoint(curve_calc_v4_p1);

                Vector3 curve_calc_v4_p2 = pos_e_v4 + (Vector3.right * (dis_v4_w / 2));
                Vector3 curve_v4_p2 = rect_rightdown.TransformPoint(curve_calc_v4_p2);

                hm_DrawBezierCurve(start_v3, curve_v4_p1, curve_v4_p2, end_v4);
                #endregion

                #region 第5分割
                float dis_v5_h = dis_v4_h - (dis_v4_h / 1.618f);
                float dis_v5_w = dis_v4_w / 1.618f;

                Vector3 pos_s_v5 = pos_s_v4 + (start_pos + Vector3.up * dis_v5_h);
                Vector3 start_v5 = rect_rightdown.TransformPoint(pos_s_v5);

                Vector3 pos_e_v5 = pos_s_v5 - (start_pos + Vector3.right * dis_v5_w);
                Vector3 end_v5 = rect_rightdown.TransformPoint(pos_e_v5);

                Gizmos.DrawLine(start_v5, end_v5);

                Vector3 curve_calc_v5_p1 = pos_e_v4 - (Vector3.right * (dis_v5_w / 2));
                Vector3 curve_v5_p1 = rect_rightdown.TransformPoint(curve_calc_v5_p1);

                Vector3 curve_calc_v5_p2 = pos_e_v5 + (Vector3.up * ((dis_v4_h / 1.618f) / 2));
                Vector3 curve_v5_p2 = rect_rightdown.TransformPoint(curve_calc_v5_p2);

                hm_DrawBezierCurve(end_v4, curve_v5_p1, curve_v5_p2, end_v5);
                #endregion

                #region 第6分割
                float dis_v6_h = dis_v5_h;
                float dis_v6_w = dis_v5_w - (dis_v5_w / 1.618f);

                Vector3 pos_s_v6 = pos_s_v5 - (start_pos + Vector3.right * dis_v6_w);
                Vector3 start_v6 = rect_rightdown.TransformPoint(pos_s_v6);

                Vector3 pos_e_v6 = pos_s_v6 - (start_pos + Vector3.up * dis_v6_h);
                Vector3 end_v6 = rect_rightdown.TransformPoint(pos_e_v6);

                Gizmos.DrawLine(start_v6, end_v6);

                Vector3 curve_calc_v6_p1 = pos_e_v5 - (Vector3.up * (dis_v6_h / 2));
                Vector3 curve_v6_p1 = rect_rightdown.TransformPoint(curve_calc_v6_p1);

                Vector3 curve_calc_v6_p2 = pos_e_v6 - (Vector3.right * ((dis_v5_w / 1.618f) / 2));
                Vector3 curve_v6_p2 = rect_rightdown.TransformPoint(curve_calc_v6_p2);

                hm_DrawBezierCurve(end_v5, curve_v6_p1, curve_v6_p2, end_v6);
                #endregion

                #region 第7分割
                float dis_v7_h = dis_v6_h - (dis_v6_h / 1.618f);
                float dis_v7_w = dis_v6_w;

                Vector3 pos_s_v7 = pos_s_v5 - (start_pos + Vector3.up * dis_v7_h);
                Vector3 start_v7 = rect_rightdown.TransformPoint(pos_s_v7);

                Vector3 pos_e_v7 = pos_s_v7 - (start_pos + Vector3.right * dis_v7_w);
                Vector3 end_v7 = rect_rightdown.TransformPoint(pos_e_v7);

                Gizmos.DrawLine(start_v7, end_v7);

                Vector3 curve_calc_v7_p1 = pos_e_v6 + (Vector3.right * (dis_v7_w / 2));
                Vector3 curve_v7_p1 = rect_rightdown.TransformPoint(curve_calc_v7_p1);

                Vector3 curve_calc_v7_p2 = pos_s_v7 - (Vector3.up * ((dis_v6_h / 1.618f) / 2));
                Vector3 curve_v7_p2 = rect_rightdown.TransformPoint(curve_calc_v7_p2);

                hm_DrawBezierCurve(end_v6, curve_v7_p1, curve_v7_p2, start_v7);
                #endregion

                #region 第8分割
                float dis_v8_h = dis_v7_h;
                float dis_v8_w = (dis_v7_w / 1.618f);

                Vector3 pos_s_v8 = pos_s_v7 - (start_pos + Vector3.right * dis_v8_w);
                Vector3 start_v8 = rect_rightdown.TransformPoint(pos_s_v8);

                Vector3 pos_e_v8 = pos_s_v8 + (start_pos + Vector3.up * dis_v8_h);
                Vector3 end_v8 = rect_rightdown.TransformPoint(pos_e_v8);

                Gizmos.DrawLine(start_v8, end_v8);

                Vector3 curve_calc_v8_p1 = pos_s_v7 + (Vector3.up * (dis_v8_h / 2));
                Vector3 curve_v8_p1 = rect_rightdown.TransformPoint(curve_calc_v8_p1);

                Vector3 curve_calc_v8_p2 = pos_e_v8 + (Vector3.right * (dis_v8_w / 2));
                Vector3 curve_v8_p2 = rect_rightdown.TransformPoint(curve_calc_v8_p2);

                hm_DrawBezierCurve(start_v7, curve_v8_p1, curve_v8_p2, end_v8);
                #endregion

                #region 第9分割
                float dis_v9_h = dis_v8_h - (dis_v8_h / 1.618f);
                float dis_v9_w = dis_v8_w / 1.618f;

                Vector3 pos_s_v9 = pos_s_v8 + (start_pos + Vector3.up * dis_v9_h);
                Vector3 start_v9 = rect_rightdown.TransformPoint(pos_s_v9);

                Vector3 pos_e_v9 = pos_s_v9 - (start_pos + Vector3.right * dis_v9_w);
                Vector3 end_v9 = rect_rightdown.TransformPoint(pos_e_v9);

                Gizmos.DrawLine(start_v9, end_v9);

                Vector3 curve_calc_v9_p1 = pos_e_v8 - (Vector3.right * (dis_v9_w / 2));
                Vector3 curve_v9_p1 = rect_rightdown.TransformPoint(curve_calc_v9_p1);

                Vector3 curve_calc_v9_p2 = pos_e_v9 + (Vector3.up * ((dis_v8_h / 1.618f) / 2));
                Vector3 curve_v9_p2 = rect_rightdown.TransformPoint(curve_calc_v9_p2);

                hm_DrawBezierCurve(end_v8, curve_v9_p1, curve_v9_p2, end_v9);
                #endregion
            }
            else if (GuideParam_Fibonacci_Mode == "左上")
            {
                RectTransform rect_rightup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右上);
                Vector3 start_pos = rect_rightup.anchoredPosition3D;

                #region 第0分割
                Vector3 pos_s_v0 = start_pos - (Vector3.right * gold_w);
                Vector3 start_v0 = rect_rightup.TransformPoint(pos_s_v0);

                Vector3 pos_e_v0 = pos_s_v0 + start_pos - (Vector3.up * scr_h);
                Vector3 end_v0 = rect_rightup.TransformPoint(pos_e_v0);

                Gizmos.color = GuideColor;
                Gizmos.DrawLine(start_v0, end_v0);

                Vector3 curve_calc_v0_p1 = start_pos - (Vector3.up * scr_h / 2);
                Vector3 curve_v0_p1 = rect_rightup.TransformPoint(curve_calc_v0_p1);

                Vector3 curve_calc_v0_p2 = pos_e_v0 + (Vector3.right * gold_w / 2);
                Vector3 curve_v0_p2 = rect_rightup.TransformPoint(curve_calc_v0_p2);

                hm_DrawBezierCurve(rect_rightup.position, curve_v0_p1, curve_v0_p2, end_v0);
                #endregion

                #region 第1分割
                float dis_v1_h = scr_h - gold_h;
                float dis_v1_w = scr_w - gold_w;

                Vector3 pos_s_v1 = pos_s_v0 - (start_pos + Vector3.up * dis_v1_h);
                Vector3 start_v1 = rect_rightup.TransformPoint(pos_s_v1);

                Vector3 pos_e_v1 = pos_s_v1 - (start_pos + (Vector3.right * dis_v1_w));
                Vector3 end_v1 = rect_rightup.TransformPoint(pos_e_v1);

                Gizmos.DrawLine(start_v1, end_v1);

                Vector3 curve_calc_v1_p1 = pos_e_v0 - (Vector3.right * (dis_v1_w / 2));
                Vector3 curve_v1_p1 = rect_rightup.TransformPoint(curve_calc_v1_p1);

                Vector3 curve_calc_v1_p2 = pos_e_v1 - (Vector3.up * gold_h / 2);
                Vector3 curve_v1_p2 = rect_rightup.TransformPoint(curve_calc_v1_p2);

                hm_DrawBezierCurve(end_v0, curve_v1_p1, curve_v1_p2, end_v1);
                #endregion

                #region 第2分割
                float dis_v2_h = dis_v1_h;
                float dis_v2_w = dis_v1_w - (dis_v1_w / 1.618f);

                Vector3 pos_s_v2 = pos_s_v0 - (start_pos + Vector3.right * dis_v2_w);
                Vector3 start_v2 = rect_rightup.TransformPoint(pos_s_v2);

                Vector3 pos_e_v2 = pos_s_v2 - (start_pos + Vector3.up * dis_v2_h);
                Vector3 end_v2 = rect_rightup.TransformPoint(pos_e_v2);

                Gizmos.DrawLine(start_v2, end_v2);

                Vector3 curve_calc_v2_p1 = pos_e_v1 + (Vector3.up * (dis_v2_h / 2));
                Vector3 curve_v2_p1 = rect_rightup.TransformPoint(curve_calc_v2_p1);

                Vector3 curve_calc_v2_p2 = pos_s_v2 - (Vector3.right * ((dis_v1_w / 1.618f) / 2));
                Vector3 curve_v2_p2 = rect_rightup.TransformPoint(curve_calc_v2_p2);

                hm_DrawBezierCurve(end_v1, curve_v2_p1, curve_v2_p2, start_v2);
                #endregion

                #region 第3分割
                float dis_v3_h = (dis_v2_h / 1.618f);
                float dis_v3_w = dis_v2_w;

                Vector3 pos_s_v3 = pos_s_v0 - (start_pos + Vector3.up * dis_v3_h);
                Vector3 start_v3 = rect_rightup.TransformPoint(pos_s_v3);

                Vector3 pos_e_v3 = pos_s_v3 - (start_pos + Vector3.right * dis_v3_w);
                Vector3 end_v3 = rect_rightup.TransformPoint(pos_e_v3);

                Gizmos.DrawLine(start_v3, end_v3);

                Vector3 curve_calc_v3_p1 = pos_s_v2 + (Vector3.right * (dis_v3_w / 2));
                Vector3 curve_v3_p1 = rect_rightup.TransformPoint(curve_calc_v3_p1);

                Vector3 curve_calc_v3_p2 = pos_s_v3 + (Vector3.up * (dis_v3_h / 2));
                Vector3 curve_v3_p2 = rect_rightup.TransformPoint(curve_calc_v3_p2);

                hm_DrawBezierCurve(start_v2, curve_v3_p1, curve_v3_p2, start_v3);
                #endregion

                #region 第4分割
                float dis_v4_h = (dis_v3_h / 1.618f);
                float dis_v4_w = (dis_v3_w / 1.618f);

                Vector3 pos_s_v4 = pos_s_v3 - (start_pos + Vector3.right * dis_v4_w);
                Vector3 start_v4 = rect_rightup.TransformPoint(pos_s_v4);

                Vector3 pos_e_v4 = pos_s_v4 - (start_pos + Vector3.up * dis_v4_h);
                Vector3 end_v4 = rect_rightup.TransformPoint(pos_e_v4);

                Gizmos.DrawLine(start_v4, end_v4);

                Vector3 curve_calc_v4_p1 = pos_s_v3 - (Vector3.up * (dis_v4_h / 2));
                Vector3 curve_v4_p1 = rect_rightup.TransformPoint(curve_calc_v4_p1);

                Vector3 curve_calc_v4_p2 = pos_e_v4 + (Vector3.right * (dis_v4_w / 2));
                Vector3 curve_v4_p2 = rect_rightup.TransformPoint(curve_calc_v4_p2);

                hm_DrawBezierCurve(start_v3, curve_v4_p1, curve_v4_p2, end_v4);
                #endregion

                #region 第5分割
                float dis_v5_h = dis_v4_h - (dis_v4_h / 1.618f);
                float dis_v5_w = dis_v4_w / 1.618f;

                Vector3 pos_s_v5 = pos_s_v4 - (start_pos + Vector3.up * dis_v5_h);
                Vector3 start_v5 = rect_rightup.TransformPoint(pos_s_v5);

                Vector3 pos_e_v5 = pos_s_v5 - (start_pos + Vector3.right * dis_v5_w);
                Vector3 end_v5 = rect_rightup.TransformPoint(pos_e_v5);

                Gizmos.DrawLine(start_v5, end_v5);

                Vector3 curve_calc_v5_p1 = pos_e_v4 - (Vector3.right * (dis_v5_w / 2));
                Vector3 curve_v5_p1 = rect_rightup.TransformPoint(curve_calc_v5_p1);

                Vector3 curve_calc_v5_p2 = pos_e_v5 - (Vector3.up * ((dis_v4_h / 1.618f) / 2));
                Vector3 curve_v5_p2 = rect_rightup.TransformPoint(curve_calc_v5_p2);

                hm_DrawBezierCurve(end_v4, curve_v5_p1, curve_v5_p2, end_v5);
                #endregion

                #region 第6分割
                float dis_v6_h = dis_v5_h;
                float dis_v6_w = dis_v5_w - (dis_v5_w / 1.618f);

                Vector3 pos_s_v6 = pos_s_v5 - (start_pos + Vector3.right * dis_v6_w);
                Vector3 start_v6 = rect_rightup.TransformPoint(pos_s_v6);

                Vector3 pos_e_v6 = pos_s_v6 + (start_pos + Vector3.up * dis_v6_h);
                Vector3 end_v6 = rect_rightup.TransformPoint(pos_e_v6);

                Gizmos.DrawLine(start_v6, end_v6);

                Vector3 curve_calc_v6_p1 = pos_e_v5 + (Vector3.up * (dis_v6_h / 2));
                Vector3 curve_v6_p1 = rect_rightup.TransformPoint(curve_calc_v6_p1);

                Vector3 curve_calc_v6_p2 = pos_e_v6 - (Vector3.right * ((dis_v5_w / 1.618f) / 2));
                Vector3 curve_v6_p2 = rect_rightup.TransformPoint(curve_calc_v6_p2);

                hm_DrawBezierCurve(end_v5, curve_v6_p1, curve_v6_p2, end_v6);
                #endregion

                #region 第7分割
                float dis_v7_h = dis_v6_h - (dis_v6_h / 1.618f);
                float dis_v7_w = dis_v6_w;

                Vector3 pos_s_v7 = pos_s_v5 + (start_pos + Vector3.up * dis_v7_h);
                Vector3 start_v7 = rect_rightup.TransformPoint(pos_s_v7);

                Vector3 pos_e_v7 = pos_s_v7 - (start_pos + Vector3.right * dis_v7_w);
                Vector3 end_v7 = rect_rightup.TransformPoint(pos_e_v7);

                Gizmos.DrawLine(start_v7, end_v7);

                Vector3 curve_calc_v7_p1 = pos_e_v6 + (Vector3.right * (dis_v7_w / 2));
                Vector3 curve_v7_p1 = rect_rightup.TransformPoint(curve_calc_v7_p1);

                Vector3 curve_calc_v7_p2 = pos_s_v7 + (Vector3.up * ((dis_v6_h / 1.618f) / 2));
                Vector3 curve_v7_p2 = rect_rightup.TransformPoint(curve_calc_v7_p2);

                hm_DrawBezierCurve(end_v6, curve_v7_p1, curve_v7_p2, start_v7);
                #endregion

                #region 第8分割
                float dis_v8_h = dis_v7_h;
                float dis_v8_w = (dis_v7_w / 1.618f);

                Vector3 pos_s_v8 = pos_s_v7 - (start_pos + Vector3.right * dis_v8_w);
                Vector3 start_v8 = rect_rightup.TransformPoint(pos_s_v8);

                Vector3 pos_e_v8 = pos_s_v8 - (start_pos + Vector3.up * dis_v8_h);
                Vector3 end_v8 = rect_rightup.TransformPoint(pos_e_v8);

                Gizmos.DrawLine(start_v8, end_v8);

                Vector3 curve_calc_v8_p1 = pos_s_v7 - (Vector3.up * (dis_v8_h / 2));
                Vector3 curve_v8_p1 = rect_rightup.TransformPoint(curve_calc_v8_p1);

                Vector3 curve_calc_v8_p2 = pos_e_v8 + (Vector3.right * (dis_v8_w / 2));
                Vector3 curve_v8_p2 = rect_rightup.TransformPoint(curve_calc_v8_p2);

                hm_DrawBezierCurve(start_v7, curve_v8_p1, curve_v8_p2, end_v8);
                #endregion

                #region 第9分割
                float dis_v9_h = dis_v8_h - (dis_v8_h / 1.618f);
                float dis_v9_w = dis_v8_w / 1.618f;

                Vector3 pos_s_v9 = pos_s_v8 - (start_pos + Vector3.up * dis_v9_h);
                Vector3 start_v9 = rect_rightup.TransformPoint(pos_s_v9);

                Vector3 pos_e_v9 = pos_s_v9 - (start_pos + Vector3.right * dis_v9_w);
                Vector3 end_v9 = rect_rightup.TransformPoint(pos_e_v9);

                Gizmos.DrawLine(start_v9, end_v9);

                Vector3 curve_calc_v9_p1 = pos_e_v8 - (Vector3.right * (dis_v9_w / 2));
                Vector3 curve_v9_p1 = rect_rightup.TransformPoint(curve_calc_v9_p1);

                Vector3 curve_calc_v9_p2 = pos_e_v9 - (Vector3.up * ((dis_v8_h / 1.618f) / 2));
                Vector3 curve_v9_p2 = rect_rightup.TransformPoint(curve_calc_v9_p2);

                hm_DrawBezierCurve(end_v8, curve_v9_p1, curve_v9_p2, end_v9);
                #endregion
            }
            Gizmos.color = Color.white;
        }
        /// <summary>
        /// 对角线构图
        /// </summary>
        private void hm_CompGuide_DiagonalLine()
        {
            if (!UseCompGuide)
                return;

            float scr_w = ScreenRes.x;
            float scr_h = ScreenRes.y;

            RectTransform rect_leftup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左上);
            RectTransform rect_rightdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右下);

            Vector3 start_pos = rect_leftup.anchoredPosition3D;
            Vector3 end_pos = rect_rightdown.anchoredPosition3D;

            Vector3 calc_pos_start = start_pos + (Vector3.right * scr_w * GuideParam_CornerLookat_Offset_H) - (Vector3.up * scr_h * GuideParam_CornerLookat_Offset_V);
            Vector3 start = rect_leftup.TransformPoint(calc_pos_start);

            Vector3 calc_pos_end = end_pos - (Vector3.right * scr_w * GuideParam_CornerLookat_Offset_H) + (Vector3.up * scr_h * GuideParam_CornerLookat_Offset_V);
            Vector3 end = rect_rightdown.TransformPoint(calc_pos_end);

            Gizmos.color = GuidePointColor;
            Vector3 center = hm_CalculateMidpoint(start, end);
            Gizmos.DrawSphere(center, GuideParam_CenterPointSize * 0.005f);

            Gizmos.color = GuideColor;
            Gizmos.DrawLine(start, end);
            Gizmos.color = Color.white;
        }
        /// <summary>
        /// 三分线构图
        /// </summary>
        private void hm_CompGuide_ThreeCut()
        {
            if (!UseCompGuide)
                return;

            float scr_w = ScreenRes.x;
            float scr_h = ScreenRes.y;

            Gizmos.color = GuideColor;

            #region 左分线
            RectTransform rect_leftup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左上);
            RectTransform rect_leftdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左下);

            Vector3 L_start_pos = rect_leftup.anchoredPosition3D;
            Vector3 L_end_pos = rect_leftdown.anchoredPosition3D;

            Vector3 L_calc_pos_start = L_start_pos + (Vector3.right * scr_w * GuideParam_Three_Offset_H * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 L_start = rect_leftup.TransformPoint(L_calc_pos_start);

            Vector3 L_calc_pos_end = L_end_pos + (Vector3.right * scr_w * GuideParam_Three_Offset_H * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 L_end = rect_leftdown.TransformPoint(L_calc_pos_end);

            Gizmos.DrawLine(L_start, L_end);
            #endregion

            #region 右分线
            RectTransform rect_rightup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右上);
            RectTransform rect_rightdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右下);

            Vector3 R_start_pos = rect_rightup.anchoredPosition3D;
            Vector3 R_end_pos = rect_rightdown.anchoredPosition3D;

            Vector3 R_calc_pos_start = R_start_pos - (Vector3.right * scr_w * GuideParam_Three_Offset_H * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 R_start = rect_rightup.TransformPoint(R_calc_pos_start);

            Vector3 R_calc_pos_end = R_end_pos - (Vector3.right * scr_w * GuideParam_Three_Offset_H * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 R_end = rect_rightdown.TransformPoint(R_calc_pos_end);

            Gizmos.DrawLine(R_start, R_end);
            #endregion

            #region 上分线
            Vector3 T_start_pos = rect_leftup.anchoredPosition3D;
            Vector3 T_end_pos = rect_rightup.anchoredPosition3D;

            Vector3 T_calc_pos_start = T_start_pos - (Vector3.up * scr_h * GuideParam_Three_Offset_V * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 T_start = rect_leftup.TransformPoint(T_calc_pos_start);

            Vector3 T_calc_pos_end = T_end_pos - (Vector3.up * scr_h * GuideParam_Three_Offset_V * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 T_end = rect_rightup.TransformPoint(T_calc_pos_end);

            Gizmos.DrawLine(T_start, T_end);
            #endregion

            #region 下分线
            Vector3 D_start_pos = rect_leftdown.anchoredPosition3D;
            Vector3 D_end_pos = rect_rightdown.anchoredPosition3D;

            Vector3 D_calc_pos_start = D_start_pos + (Vector3.up * scr_h * GuideParam_Three_Offset_V * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 D_start = rect_leftdown.TransformPoint(D_calc_pos_start);

            Vector3 D_calc_pos_end = D_end_pos + (Vector3.up * scr_h * GuideParam_Three_Offset_V * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 D_end = rect_rightdown.TransformPoint(D_calc_pos_end);

            Gizmos.DrawLine(D_start, D_end);
            #endregion

            Gizmos.color = Color.white;
        }
        /// <summary>
        /// 引导线构图
        /// </summary>
        private void hm_CompGuide_GuideLineRef()
        {
            if (!UseCompGuide)
                return;

            float scr_w = ScreenRes.x;
            float scr_h = ScreenRes.y;

            Gizmos.color = GuideColor;

            RectTransform rect_down = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.下);
            RectTransform rect_leftdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左下);
            RectTransform rect_rightdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右下);

            #region 水平高度
            Vector3 Start_pos = rect_down.anchoredPosition3D;

            Vector3 BaseHeight_calc_pos_start = Start_pos + (Vector3.up * scr_h * GuideParam_GuideLine_BaseHeight) - (Vector3.right * (scr_w / 2));
            Vector3 BaseHeight_start = rect_down.TransformPoint(BaseHeight_calc_pos_start);

            Vector3 BaseHeight_calc_pos_end = Start_pos + (Vector3.up * scr_h * GuideParam_GuideLine_BaseHeight) + (Vector3.right * (scr_w / 2));
            Vector3 BaseHeight_end = rect_down.TransformPoint(BaseHeight_calc_pos_end);

            Gizmos.DrawLine(BaseHeight_start, BaseHeight_end);
            #endregion

            #region 左侧引导线
            Vector3 Start_pos_L = rect_leftdown.anchoredPosition3D;

            Vector3 Near_calc_pos_start_L = Start_pos_L + (Vector3.right * (scr_w / 2) * (GuideParam_GuideLine_Offset_Near)) + (Vector3.up * scr_h * GuideParam_GuideLine_Offset_NearHeight);
            Vector3 Near_start_L = rect_leftdown.TransformPoint(Near_calc_pos_start_L);

            Vector3 Far_calc_pos_end_L = Start_pos + (Vector3.up * scr_h * GuideParam_GuideLine_BaseHeight) - (Vector3.right * (scr_w / 2) * (GuideParam_GuideLine_Offset_Far) - ((Vector3.right * (scr_w / 2) * GuideParam_GuideLine_BaseOffset)));
            Vector3 Far_end_L = rect_down.TransformPoint(Far_calc_pos_end_L);

            Gizmos.DrawLine(Near_start_L, Far_end_L);
            #endregion

            #region 右侧引导线            
            Vector3 Start_pos_R = rect_rightdown.anchoredPosition3D;

            Vector3 Near_calc_pos_start_R = Start_pos_R - (Vector3.right * (scr_w / 2) * (GuideParam_GuideLine_Offset_Near)) + (Vector3.up * scr_h * GuideParam_GuideLine_Offset_NearHeight);
            Vector3 Near_start_R = rect_rightdown.TransformPoint(Near_calc_pos_start_R);

            Vector3 Far_calc_pos_end_R = Start_pos + (Vector3.up * scr_h * GuideParam_GuideLine_BaseHeight) + (Vector3.right * (scr_w / 2) * (GuideParam_GuideLine_Offset_Far) + ((Vector3.right * (scr_w / 2) * GuideParam_GuideLine_BaseOffset)));
            Vector3 Far_end_R = rect_down.TransformPoint(Far_calc_pos_end_R);

            Gizmos.DrawLine(Near_start_R, Far_end_R);
            #endregion

            Gizmos.color = GuidePointColor;
            Vector3 center = hm_CalculateMidpoint(BaseHeight_start, BaseHeight_end);
            Gizmos.DrawSphere(center, GuideParam_CenterPointSize * 0.005f);

            Gizmos.color = Color.white;
        }
        /// <summary>
        /// 三角构图
        /// </summary>
        private void hm_CompGuide_Triangle()
        {
            if (!UseCompGuide)
                return;

            float scr_w = ScreenRes.x;
            float scr_h = ScreenRes.y;


            RectTransform rect_down = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.下);
            RectTransform rect_leftdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左下);
            RectTransform rect_rightdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右下);

            #region 顶部高度
            Vector3 Start_pos = rect_down.anchoredPosition3D;

            Vector3 BaseHeight_calc_pos_start = Start_pos + (Vector3.up * scr_h * GuideParam_Triangle_BaseHeight) + (Vector3.right * (scr_w / 2) * GuideParam_Triangle_TopOffset);
            Vector3 BaseHeight_start = rect_down.TransformPoint(BaseHeight_calc_pos_start);

            Vector3 BaseHeight_calc_pos_end = Start_pos + (Vector3.up * scr_h * GuideParam_Triangle_BottomHeight);
            Vector3 BaseHeight_end = rect_down.TransformPoint(BaseHeight_calc_pos_end);
            #endregion

            Gizmos.color = GuideColor;

            #region 左侧角点线
            Vector3 Start_pos_L = rect_down.anchoredPosition3D;

            Vector3 Near_calc_pos_start_L = Start_pos_L - (Vector3.right * (scr_w / 2) * (GuideParam_Triangle_Offset_Left)) + BaseHeight_calc_pos_end;
            Vector3 Near_start_L = rect_down.TransformPoint(Near_calc_pos_start_L);

            Vector3 Far_calc_pos_end_L = BaseHeight_calc_pos_start;
            Vector3 Far_end_L = rect_down.TransformPoint(BaseHeight_calc_pos_start);

            Gizmos.DrawLine(Near_start_L, Far_end_L);
            #endregion

            #region 右侧角点线
            Vector3 Start_pos_R = rect_down.anchoredPosition3D;

            Vector3 Near_calc_pos_start_R = Start_pos_R + (Vector3.right * (scr_w / 2) * (GuideParam_Triangle_Offset_Right)) + BaseHeight_calc_pos_end;
            Vector3 Near_start_R = rect_down.TransformPoint(Near_calc_pos_start_R);

            Vector3 Far_calc_pos_end_R = BaseHeight_calc_pos_start;
            Vector3 Far_end_R = rect_down.TransformPoint(Far_calc_pos_end_R);

            Gizmos.DrawLine(Near_start_R, Far_end_R);
            #endregion

            Gizmos.DrawLine(Near_start_L, Near_start_R);

            Gizmos.color = GuidePointColor;
            Vector3 center = hm_CalculateCentroid(Near_start_L, Far_end_L, Near_start_R);
            Gizmos.DrawSphere(center, GuideParam_CenterPointSize * 0.005f);

            Gizmos.color = Color.white;
        }
        /// <summary>
        /// 工字型构图
        /// </summary>
        private void hm_CompGuide_IShape()
        {
            if (!UseCompGuide)
                return;

            float scr_w = ScreenRes.x;
            float scr_h = ScreenRes.y;


            RectTransform rect_down = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.下);
            RectTransform rect_leftdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左下);
            RectTransform rect_rightdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右下);

            Gizmos.color = GuideColor;

            #region 顶部
            Vector3 Start_pos = rect_down.anchoredPosition3D;

            Vector3 Isp_top_calc_pos_start = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_TopHeight) - (Vector3.right * (scr_w / 2));
            Vector3 Isp_top_start = rect_down.TransformPoint(Isp_top_calc_pos_start);

            Vector3 Isp_top_calc_pos_end = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_TopHeight) + (Vector3.right * (scr_w / 2));
            Vector3 Isp_top_end = rect_down.TransformPoint(Isp_top_calc_pos_end);

            Gizmos.DrawLine(Isp_top_start, Isp_top_end);
            #endregion

            #region 底部

            Vector3 Isp_bottom_calc_pos_start = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_BottomHeight) - (Vector3.right * (scr_w / 2));
            Vector3 Isp_bottom_start = rect_down.TransformPoint(Isp_bottom_calc_pos_start);

            Vector3 Isp_bottom_calc_pos_end = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_BottomHeight) + (Vector3.right * (scr_w / 2));
            Vector3 Isp_bottom_end = rect_down.TransformPoint(Isp_bottom_calc_pos_end);

            Gizmos.DrawLine(Isp_bottom_start, Isp_bottom_end);
            #endregion

            #region 左侧线
            Vector3 calc_pos_start_L = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_TopHeight) - (Vector3.right * (scr_w / 2) * GuideParam_IShape_Offset_Left);
            Vector3 start_L = rect_down.TransformPoint(calc_pos_start_L);

            Vector3 calc_pos_end_L = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_BottomHeight) - (Vector3.right * (scr_w / 2) * GuideParam_IShape_Offset_Left);
            Vector3 end_L = rect_down.TransformPoint(calc_pos_end_L);

            Gizmos.DrawLine(start_L, end_L);
            #endregion

            #region 右侧线
            Vector3 calc_pos_start_R = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_TopHeight) + (Vector3.right * (scr_w / 2) * GuideParam_IShape_Offset_Right);
            Vector3 start_R = rect_down.TransformPoint(calc_pos_start_R);

            Vector3 calc_pos_end_R = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_BottomHeight) + (Vector3.right * (scr_w / 2) * GuideParam_IShape_Offset_Right);
            Vector3 end_R = rect_down.TransformPoint(calc_pos_end_R);

            Gizmos.DrawLine(start_R, end_R);
            #endregion

            Gizmos.color = GuidePointColor;
            Vector3 center_top = hm_CalculateMidpoint(Isp_top_start, Isp_top_end);
            Gizmos.DrawSphere(center_top, GuideParam_CenterPointSize * 0.005f);

            Vector3 center_bottom = hm_CalculateMidpoint(Isp_bottom_start, Isp_bottom_end);
            Gizmos.DrawSphere(center_bottom, GuideParam_CenterPointSize * 0.005f);
            Gizmos.color = Color.white;
        }
        /// <summary>
        /// 计算三个点的中心点坐标
        /// </summary>
        /// <param name="pointA">第一个点的坐标</param>
        /// <param name="pointB">第二个点的坐标</param>
        /// <param name="pointC">第三个点的坐标</param>
        /// <returns>中心点的坐标</returns>
        public static Vector3 hm_CalculateCentroid(Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {
            // 计算三个点的平均值
            return (pointA + pointB + pointC) / 3f;
        }
        /// <summary>
        /// 计算两个点的中心点坐标
        /// </summary>
        /// <param name="pointA">第一个点的坐标</param>
        /// <param name="pointB">第二个点的坐标</param>
        /// <returns>中心点的坐标</returns>
        public static Vector3 hm_CalculateMidpoint(Vector3 pointA, Vector3 pointB)
        {
            // 计算两个点的平均值
            return (pointA + pointB) / 2f;
        }
        /// <summary>
        /// 曲线绘制
        /// </summary>
        /// <param name="start">开始点</param>
        /// <param name="control1">开始点贝塞尔</param>
        /// <param name="control2">结束点贝塞尔</param>
        /// <param name="end">结束点</param>
        private void hm_DrawBezierCurve(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end)
        {
            // 分割曲线的点数
            int segments = 30;

            // 绘制曲线
            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector3 point = hm_CalculateBezierPoint(t, start, control1, control2, end);

                if (i == 0)
                {
                    Gizmos.DrawLine(start, point);
                }
                else
                {
                    Gizmos.DrawLine(previousPoint, point);
                }

                previousPoint = point;
            }
        }
        /// <summary>
        /// 计算贝塞尔
        /// </summary>
        /// <param name="t"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        private Vector3 hm_CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            // 贝塞尔曲线公式
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0; // 第一项
            p += 3 * uu * t * p1; // 第二项
            p += 3 * u * tt * p2; // 第三项
            p += ttt * p3; // 第四项

            return p;
        }
        /// <summary>
        /// 获取布局构图参考锚点根物体
        /// </summary>
        /// <returns></returns>
        public RectTransform hm_Auxiliary_GetAnchorRoot()
        {
            return CompGuide_AnchorRoot;
        }
        /// <summary>
        /// 获取布局构图参考锚点
        /// </summary>
        /// <param name="anchortype"></param>
        /// <returns></returns>
        public RectTransform hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide anchortype)
        {
            RectTransform rect = null;
            for (int i = 0; i < CompGuide_Anchors.Length; i++)
            {
                if (CompGuide_Anchors[i].Type == anchortype)
                {
                    rect = CompGuide_Anchors[i].Anchor;
                    break;
                }
            }

            return rect;
        }
    }
}