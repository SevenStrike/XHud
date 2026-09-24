/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGUI - Unity Editor界面可视化组件工具
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
namespace SevenStrikeModules.XGUI.Runtime
{
    /// <summary>
    /// 对话框按钮模式
    /// </summary>
    public enum XGUIDialogButtonMode
    {
        /// <summary>
        /// 一个按钮
        /// </summary>
        单个按钮 = 0,
        /// <summary>
        /// 两个按钮
        /// </summary>
        两个按钮 = 1,
        /// <summary>
        /// 三个按钮
        /// </summary>
        三个按钮 = 2,
        /// <summary>
        /// 四个按钮
        /// </summary>
        四个按钮 = 3,
        /// <summary>
        /// 五个按钮
        /// </summary>
        五个按钮 = 4,
        /// <summary>
        /// 动画预设对话框
        /// </summary>
        参数提交 = 5
    }
    /// <summary>
    /// 对话框类型
    /// </summary>
    public enum XGUIDialogType
    {
        /// <summary>
        /// 通知
        /// </summary>
        通知 = 0,
        /// <summary>
        /// 确认
        /// </summary>
        确认 = 1,
        /// <summary>
        /// 修改
        /// </summary>
        修改 = 2,
        /// <summary>
        /// 警告
        /// </summary>
        警告 = 3,
        /// <summary>
        /// 错误
        /// </summary>
        错误 = 4,
        /// <summary>
        /// 帮助
        /// </summary>
        帮助 = 5
    }
    /// <summary>
    /// 通知消息类型
    /// 定义消息弹窗的样式类别
    /// </summary>
    public enum XGUIMsgState
    {
        通知 = 0,
        确认 = 1,
        警告 = 2,
        错误 = 3,
        设置 = 4,
        未开启消息模块功能 = 5
    }
    /// <summary>
    /// 帮助提示消息类型
    /// 定义XGUIHelpbox消息的图标样式类别
    /// </summary>
    public enum XGUIHelboxState
    {
        通知 = 0,
        警告 = 1,
        错误 = 2
    }
    /// <summary>
    /// 颜色样式
    /// </summary>
    public enum XGUIColor
    {
        /// <summary>
        /// 颜色值：232323
        /// </summary>
        深空灰 = 0,
        /// <summary>
        /// 颜色值：909090
        /// </summary>
        阴影灰 = 1,
        /// <summary>
        /// 颜色值：cecece
        /// </summary>
        亮白 = 2,
        /// <summary>
        /// 颜色值：a8cc3e
        /// </summary>
        柠檬绿 = 3,
        /// <summary>
        ///  颜色值：56b2f4
        /// </summary>
        工业蓝 = 4,
        /// <summary>
        /// 颜色值：ffc230
        /// </summary>
        警示黄 = 5,
        /// <summary>
        /// 颜色值：d977b7
        /// </summary>
        玫瑰粉 = 6,
        /// <summary>
        /// 颜色值：a86bcd
        /// </summary>
        神秘紫 = 7,
        /// <summary>
        /// 颜色值：f45656
        /// </summary>
        魅力红 = 8,
        /// <summary>
        /// 颜色值：72804b
        /// </summary>
        灰绿 = 9,
        /// <summary>
        /// 颜色值：ff8021
        /// </summary>
        亮橘红 = 10,
        /// <summary>
        /// 颜色值：747474
        /// </summary>
        枪灰 = 11,
        /// <summary>
        /// 颜色值：ffd86b
        /// </summary>
        亮金色 = 12,
        /// <summary>
        /// 颜色值：642a2a
        /// </summary>
        沉暗红 = 13,
        /// <summary>
        /// 颜色值：5d6a82
        /// </summary>
        烟灰蓝 = 14,
        /// <summary>
        /// 颜色值：359882
        /// </summary>
        健康绿 = 15,
        /// <summary>
        /// 颜色值：414141
        /// </summary>
        浅灰 = 16,
        无 = 17
    }
    /// <summary>
    /// 填充类型
    /// </summary>
    public enum XGUIFilled
    {
        无 = 0,
        实体 = 1,
        边框 = 2,
        缺口边框 = 3,
        纯色边框 = 4,
        缺口纯色边框 = 5,
        透明 = 6
    }
    /// <summary>
    /// 开关背景类型
    /// </summary>
    public enum XGUIToggleStyle
    {
        实体 = 0,
        嵌入 = 1,
        边框 = 2,
    }
    /// <summary>
    /// 卡片背景类型
    /// </summary>
    public enum XGUICardStyle
    {
        实体 = 0,
        边框 = 1,
    }
    /// <summary>
    /// 开关控制柄类型
    /// </summary>
    public enum XGUIToggleHandlerState
    {
        关 = 0,
        开 = 1,
    }
    /// <summary>
    /// 界面容器布局方式
    /// </summary>
    public enum XGUIContainerType
    {
        /// <summary>
        /// 垂直布局
        /// </summary>
        Vertical = 0,
        /// <summary>
        /// 水平布局
        /// </summary>
        Horizontal = 1
    }
    /// <summary>
    /// 字体大小规范
    /// </summary>
    public enum XGUIFontSize
    {
        /// <summary>
        /// 最小
        /// </summary>
        XS = 0,
        /// <summary>
        /// 小
        /// </summary>
        S = 1,
        /// <summary>
        /// 中
        /// </summary>
        M = 2,
        /// <summary>
        /// 标准
        /// </summary>
        B = 3,
        /// <summary>
        /// 标准
        /// </summary>
        BX = 4,
        /// <summary>
        /// 大
        /// </summary>
        L = 5,
        /// <summary>
        /// 最大
        /// </summary>
        XL = 6,
        /// <summary>
        /// 极大
        /// </summary>
        XXL = 7,
        /// <summary>
        /// 超大
        /// </summary>
        XXXL = 8,
    }
    /// <summary>
    /// 图标对齐
    /// </summary>
    public enum XGUIIconAlignment
    {
        /// <summary>
        /// 图标默认对齐
        /// </summary>
        默认 = 0,
        /// <summary>
        /// 图标对齐到左边
        /// </summary>
        左 = 1,
        /// <summary>
        /// 图标对齐到中心
        /// </summary>
        中心 = 2,
        /// <summary>
        /// 图标对齐到右边
        /// </summary>
        右 = 3,
    }
    /// <summary>
    /// 路径选择器的类型。
    /// </summary>
    public enum XGUIPathType
    {
        /// <summary>选择文件</summary>
        文件,
        /// <summary>选择文件夹</summary>
        文件夹,
        /// <summary>选择文件或文件夹</summary>
        文件或文件夹,
        /// <summary>保存文件（弹出保存对话框）</summary>
        保存文件
    }
    /// <summary>
    /// 分割线方向。
    /// </summary>
    public enum XGUISeplineDir
    {
        /// <summary>垂直分割（竖线）</summary>
        垂直,
        /// <summary>水平分割（横线）</summary>
        水平,
    }
}