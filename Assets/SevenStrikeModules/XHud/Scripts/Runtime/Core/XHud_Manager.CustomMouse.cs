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