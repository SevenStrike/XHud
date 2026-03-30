namespace SevenStrikeModules.XHud
{
    using UnityEngine;
    using UnityEngine.Events;

    public partial class XHud_Manager : MonoBehaviour
    {
        [Tooltip("光标样式")]
        /// <summary>
        /// 光标样式
        /// </summary>
        public XHud_CustomMouseCursor Hud_MouseCursor;
        /// <summary>
        /// 使用自定义鼠标样式
        /// </summary>
        public bool CustomCursor;

        /// <summary>
        /// 光标 - 尺寸_Size
        /// </summary>
        /// <param name="size">尺寸_Size</param>
        public void hm_Cursor_SizeSet(float size)
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_SetCursorSize(size);
        }
        /// <summary>
        /// 光标 - 启用
        /// </summary>
        public void hm_Cursor_Enabled()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_SetCustomCursorEnabled(true);
        }
        /// <summary>
        /// 光标 - 禁用
        /// </summary>
        public void hm_Cursor_Disabled()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_SetCustomCursorEnabled(false);
        }
        /// <summary>
        /// 光标 - 显示
        /// </summary>
        public void hm_Cursor_Display()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_CursorOpacitySet(1);
        }
        /// <summary>
        /// 光标 - 隐藏
        /// </summary>
        public void hm_Cursor_Hide()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_CursorOpacitySet(0);
        }
        /// <summary>
        /// 光标 - 快速显示
        /// </summary>
        public void hm_Cursor_FastDisplay()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_CursorOpacityFastSet(1);
        }
        /// <summary>
        /// 光标 - 快速隐藏
        /// </summary>
        public void hm_Cursor_FastHide()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_CursorOpacityFastSet(0);
        }
        /// <summary>
        /// 光标 - 解锁
        /// </summary>
        public void hm_Cursor_UnLock()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_LockCursor(CursorLockMode.None);
        }
        /// <summary>
        /// 光标 - 锁定
        /// </summary>
        public void hm_Cursor_Locked()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_LockCursor(CursorLockMode.Locked);
        }
        /// <summary>
        /// 光标 - 改变样式
        /// </summary>
        /// <param name="style">目标样式</param>
        public void hm_Cursor_ChangeStyle(string style)
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_ChangeStyle(style);
        }
        /// <summary>
        /// 光标 - 重置样式
        /// </summary>
        public void hm_Cursor_ResetStyle()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_ResetCursorImage();
        }
        /// <summary>
        /// 光标 - 设置颜色
        /// </summary>
        /// <param name="color">目标颜色</param>
        public void hm_Cursor_SetColor(Color color)
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_SetCursorColor(color);
        }
    }
}