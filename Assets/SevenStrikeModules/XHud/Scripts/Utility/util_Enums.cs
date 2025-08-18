namespace SevenStrikeModules.XHud.Enums
{
    /// <summary>
    /// 运动样式 - 位移
    /// </summary>
    public enum HudMotion_Movement
    {
        A_无运动 = 0,
        S_从下至上 = 1,
        D_从上至下 = 2,
        F_从左至右 = 3,
        G_从右至左 = 4,
        H_从左上至中心 = 5,
        J_从左下至中心 = 6,
        K_从右上至中心 = 7,
        L_从右下至中心 = 8,
        Z_从中心至左上 = 9,
        X_从中心至左下 = 10,
        C_从中心至右上 = 11,
        V_从中心至右下 = 12,
        B_从中心至左 = 13,
        N_从中心至右 = 14,
        M_从中心至上 = 15,
        Q_从中心至下 = 16,
        W_中心缩放 = 17,
        E_从前到中心 = 18,
        R_从中心到前 = 19,
        T_从后到中心 = 20,
        Y_从中心到后 = 21,
    }

    /// <summary>
    /// 运动样式 - 旋转_Rotation
    /// </summary>
    public enum HudMotion_Rotation
    {
        A_无旋转 = 0,
        B_顺向_水平 = 1,
        C_顺向_垂直 = 2,
        D_顺向_倾角 = 3,
        E_逆向_水平 = 4,
        F_逆向_垂直 = 5,
        G_逆向_倾角 = 6
    }

    /// <summary>
    /// 元素动效类分项
    /// </summary>
    public enum MotionAnimateEndState
    {
        /// <summary>
        /// 以_透明度动效_的完成为调用停止参考，当动效的透明度动画完成后才调用动作委托
        /// </summary>
        以_透明度为准 = 0,
        /// <summary>
        /// 以_移动动效_的完成为调用停止参考，当动效的移动动画完成后才调用动作委托
        /// </summary>
        以_移动为准 = 1,
        /// <summary>
        /// 以_旋转动效_的完成为调用停止参考，当动效的旋转动画完成后才调用动作委托
        /// </summary>
        以_旋转为准 = 2
    }

    /// <summary>
    /// 锚点
    /// </summary>
    public enum HudAnchor
    {
        底层 = 0,
        上 = 1,
        下 = 2,
        左 = 3,
        右 = 4,
        中心 = 5,
        左上 = 6,
        左下 = 7,
        右上 = 8,
        右下 = 9,
        顶层 = 10
    }

    /// <summary>
    /// 布局构图锚点
    /// </summary>
    public enum HudAnchors_CompGuide
    {
        上 = 0,
        下 = 1,
        左 = 2,
        右 = 3,
        中心 = 4,
        左上 = 5,
        左下 = 6,
        右上 = 7,
        右下 = 8
    }

    /// <summary>
    /// 边距方向
    /// </summary>
    public enum HudAnchorMargin
    {
        上 = 0,
        下 = 1,
        左 = 2,
        右 = 3,
    }

    /// <summary>
    /// 库设置器模式
    /// </summary>
    public enum LibrarySetterMode
    {
        添加到库,
        修改库源参数,
        修改生成器项参数,
    }

    /// <summary>
    /// 内容锚点
    /// </summary>
    public enum ContentAnchor
    {
        上 = 0,
        下 = 1,
        左 = 2,
        右 = 3,
        中心 = 4,
        左上 = 5,
        左下 = 6,
        右上 = 7,
        右下 = 8,
    }

    /// <summary>
    /// Tmp内容锚点
    /// </summary>
    public enum TmpContentAnchor
    {
        顶部 = 0,
        顶部靠左 = 1,
        顶部靠右 = 2,
        顶部填充 = 3,
        顶部均分 = 4,
        顶部基线 = 5,
        底部 = 6,
        底部靠左 = 7,
        底部靠右 = 8,
        底部左右填充 = 9,
        底部左右均分 = 10,
        底部基线 = 11,
        左 = 12,
        右 = 13,
        中心 = 14,
        中心填充 = 15,
        中心左右均分 = 16,
        基线 = 17,
        基线靠左 = 18,
        基线靠右 = 19,
        基线左右填充 = 20,
        基线左右均分 = 21,
        中线 = 22,
        中线靠左 = 23,
        中线靠右 = 24,
        中线左右填充 = 25,
        中线左右均分 = 26
    }

    /// <summary>
    /// 颜色样式
    /// </summary>
    public enum HudColor
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
    public enum HudFilled
    {
        实体 = 0,
        边框 = 1,
        纯色边框 = 2,
        无 = 3,
        透明 = 4
    }

    /// <summary>
    /// 开关状态
    /// </summary>
    public enum HudSwitcher
    {
        开启 = 0,
        关闭 = 1
    }

    /// <summary>
    /// 连接状态
    /// </summary>
    public enum HudConnectState
    {
        已连接 = 0,
        已断开 = 1
    }

    /// <summary>
    /// 通知消息类型
    /// </summary>
    public enum HudMsgState
    {
        通知 = 0,
        确认 = 1,
        警告 = 2,
        错误 = 3,
        设置 = 4,
        未开启消息模块功能 = 5
    }

    public enum HudPathLocation
    {
        系统临时目录 = 0,
        项目工程根目录 = 1,
        自定义 = 2
    }

    public enum HudSpace
    {
        屏幕空间 = 0,
        世界空间 = 1
    }

    public enum HudSpaceCoordinateMode
    {
        绝对坐标 = 0,
        参考物体坐标 = 1
    }

    public enum CanvasAnchor
    {
        CameraNear = 0,
        CameraFar = 1,
        Custom = 2
    }

    public enum Hud_ButtonAction
    {
        点击 = 0,
        按下 = 1,
        抬起 = 2,
        选中 = 3,
        取消选中 = 4,
        进入 = 5,
        离开 = 6,
        长按 = 7,
        待命 = 8
    }

    public enum ElementSpaceType
    {
        None = 0,
        Screen = 1,
        World = 2
    }

    public enum TweenNodeType
    {
        位移 = 0,
        旋转 = 1,
        缩放 = 2,
        颜色 = 3,
        淡化 = 4,
        打字机 = 5,
        图像填充 = 6,
        尺寸 = 7,
        自定义整数 = 8,
        自定义浮点数 = 9,
        自定义2维向量 = 10,
        自定义3维向量 = 11,
        自定义4维向量 = 12,
        自定义颜色 = 13
    }

    /// <summary>
    /// 动画器类型
    /// </summary>
    public enum ModuleType
    {
        RectTransform = 0,
        Text = 1,
        TmpText = 2,
        Image = 3,
        RawImage = 4
    }

    public enum IsolateVisualModeMarkAnchor
    {
        左上 = 0,
        左下 = 1,
        右上 = 2,
        右下 = 3,
        中心 = 4,
        左 = 5,
        右 = 6,
        上 = 7,
        下 = 8
    }

    /// <summary>
    /// 元素的生成状态
    /// </summary>
    public enum HudElementCreateState
    {
        Recycled,
        Created
    }

    /// <summary>
    /// 元素的动效类型
    /// </summary>
    public enum HudElementMotionType
    {
        Creator = 0,
        Recycler = 1
    }

    /// <summary>
    /// 元素的动画状态
    /// </summary>
    public enum HudElementAnimateState
    {
        Static,
        Animating
    }

    public enum GameViewSizeType
    {
        AspectRatio, FixedResolution
    }
}
