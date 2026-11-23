namespace SevenStrikeModules.XBinder.Example.Editor
{
    using SevenStrikeModules.XBinder.Editor;
    using SevenStrikeModules.XBinder.Example;
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(local_source))]
    public class Editor_local_source : Editor_XBinder_Source
    {
        #region 字段 - 序列化属性
        private SerializedProperty sp_use_absolute_path;
        private SerializedProperty sp_absolute_path;
        private SerializedProperty sp_json;
        #endregion

        #region 字段 - 选项文字
        /// <summary>
        /// 开关控件的"混合选项"文字
        /// </summary>
        string[] stroptions_absolute = new string[2] { "绝对路径", "指定文件" };
        #endregion       

        public override void OnEnable()
        {
            base.OnEnable();

            base_script = (local_source)target;

            #region 获取序列化字段
            sp_json = serializedObject.FindProperty(nameof(local_source.json));
            sp_use_absolute_path = serializedObject.FindProperty(nameof(local_source.use_absolute_path));
            sp_absolute_path = serializedObject.FindProperty(nameof(local_source.absolute_path));
            #endregion
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Draw_Params()
        {
            base.Draw_Params();

            if (sp_use_absolute_path.boolValue)
            {
                #region 目标数据绝对路径
                XGUI.layout_property_field(
                    title: "数据绝对路径",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_absolute_path,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
            else
            {
                #region 目标数据文件
                XGUI.layout_property_field(
                    title: "目标数据文件",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_json,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
        }

        public override void Draw_Options()
        {
            base.Draw_Options();

            #region 手动请求数据
            DrawToggle("使用绝对路径文件", sp_use_absolute_path, 120, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, stroptions_absolute, (b) => { });
            #endregion
        }
    }
}