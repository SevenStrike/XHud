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
namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// 图片组件编辑器扩展工具
    /// 在 Image 组件的右键菜单中添加 "ConvertToPureSprite" 选项
    /// 用于将彩色图片转换为去饱和度的纯色精灵图（保留透明度）
    /// 
    /// 使用场景：
    /// - UI 设计时，需要将彩色图标转换为纯色版本（如白色图标）
    /// - 制作遮罩、轮廓、辉光效果时，需要纯色贴图
    /// - 快速生成图标的不同颜色变体
    /// - 处理导入的 PSD 文件，生成去色版本用于 UI
    /// 
    /// 工作流程：
    /// 1. 右键点击 Image 组件 → 选择 ConvertToPureSprite
    /// 2. 弹出确认对话框，询问是否创建纯净 Sprite
    /// 3. 获取当前 Image 的 Sprite 对应的原始纹理
    /// 4. 对纹理进行去色处理（非透明部分变为纯白色）
    /// 5. 保存处理后的纹理为 PNG 文件（原文件名_Desaturated.png）
    /// 6. 将 PNG 导入为 Sprite 资源
    /// 7. 自动将 Image 的 Sprite 替换为新生成的纯净 Sprite
    /// 
    /// 注意事项：
    /// - 此工具仅在编辑器中可用，不会影响运行时性能
    /// - 生成的新贴图会保存在原贴图相同目录下
    /// - 新贴图命名格式：原文件名_Desaturated.png
    /// </summary>
    [CustomEditor(typeof(Image))]
    public class Editor_XHud_Tool_ConvertSpriteToPureStyle : UnityEditor.UI.ImageEditor
    {
        /// <summary>
        /// 在 Image 组件的上下文菜单中添加自定义选项
        /// 菜单路径：CONTEXT/Image/ConvertToPureSprite
        /// priority = 100 控制菜单项的显示顺序（数值越大越靠下）
        /// </summary>
        [MenuItem("CONTEXT/Image/ConvertToPureSprite", priority = 100)]
        private static void ConvertPureSprite(MenuCommand command)
        {
            // ========== 1. 弹出确认对话框 ==========
            // 询问用户是否确认创建纯净 Sprite
            // 参数说明：对话框类型、标题、消息、确认按钮文本、取消按钮文本、默认选中的按钮索引
            string res = XGUI.dialog(
                type: XGUIDialogType.警告,
                windowtitle: "XHud - PSD Reconstruction消息",
                title: "去色化Sprite转换",
                msg: "是否需要为当前Image中的Sprite创建去色的Sprite并指定？",
                ok: "创建",
                cancel: "暂不",
                PrimaryIndex: 1,
                themecolor: XHud_Dashboard.Theme_Primary);

            // 如果用户选择"暂不"，则退出
            if (res == "暂不")
                return;

            // ========== 2. 获取选中的 Image 组件 ==========
            Image selectedImage = (Image)command.context;

            // 安全检查：如果当前 Image 没有关联 Sprite，直接返回
            if (selectedImage.sprite == null)
                return;

            // ========== 3. 获取原始贴图信息 ==========
            // 获取当前 Sprite 对应的纹理
            Texture2D tex = selectedImage.sprite.texture;

            // 获取贴图的资源路径（例如：Assets/Sprites/icon.png）
            string path_tex = AssetDatabase.GetAssetPath(tex);

            // 获取贴图所在的目录（例如：Assets/Sprites）
            string directory = Path.GetDirectoryName(path_tex);

            // 获取文件名（不含扩展名，例如：icon）
            string fileName = Path.GetFileNameWithoutExtension(path_tex);

            // 构造新文件名（例如：icon_Desaturated.png）
            string newFileName = $"{fileName}_Desaturated.png";

            // 构造新文件的完整路径（例如：Assets/Sprites/icon_Desaturated.png）
            string newPath = Path.Combine(directory, newFileName);

            Debug.Log(newPath);

            // ========== 4. 处理贴图（去饱和度） ==========
            // 将彩色贴图转换为纯色贴图（非透明部分变为纯白色）
            Texture2D puretex = Desaturator(tex);

            // ========== 5. 保存处理后的贴图为 PNG 文件 ==========
            SaveTexture(puretex, newPath);

            // ========== 6. 将 PNG 文件导入为 Sprite 资源 ==========
            ConvertSprite(newPath);

            // ========== 7. 将 Image 的 Sprite 替换为新生成的纯净 Sprite ==========
            selectedImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(newPath);
        }

        /// <summary>
        /// 去色处理：将贴图中所有不透明的像素转换为纯白色，同时保留透明度
        /// 透明像素保持完全透明
        /// 
        /// 处理逻辑：
        /// - 不透明像素（alpha > 0）：颜色设为纯白色 (1, 1, 1)，保留原始透明度
        /// - 透明像素（alpha = 0）：颜色设为完全透明 (0, 0, 0, 0)
        /// 
        /// 使用场景：
        /// - 将彩色图标转换为白色轮廓，用于 UI 着色或发光效果
        /// - 生成遮罩贴图
        /// - 创建纯色版本的精灵图
        /// 
        /// </summary>
        /// <param name="input">原始纹理</param>
        /// <returns>处理后的纯色纹理</returns>
        public static Texture2D Desaturator(Texture2D input)
        {
            // 确保贴图可读写（临时开启）
            EnsureTextureReadWrite(input, true);

            // 目标纹理格式：RGBA32（32位颜色，带 Alpha 通道）
            TextureFormat targetFormat = TextureFormat.RGBA32;

            // 创建新的 Texture2D 对象，大小和格式与输入纹理相同
            Texture2D output = new Texture2D(input.width, input.height, targetFormat, false);

            // 获取输入纹理的所有像素数据
            Color[] pixels = input.GetPixels();

            // 恢复贴图的 Read/Write 设置（因为之后不再需要像素读取）
            EnsureTextureReadWrite(input, false);

            // ========== 遍历每个像素进行处理 ==========
            for (int i = 0; i < pixels.Length; i++)
            {
                Color pixel = pixels[i];
                float alpha = pixel.a; // 获取当前像素的透明度

                // 如果像素不透明（透明度 > 0），将其颜色值设置为纯白色
                if (alpha > 0)
                {
                    // 设置为纯白色，保留原始透明度
                    pixels[i] = new Color(1.0f, 1.0f, 1.0f, alpha);
                }
                else
                {
                    // 透明部分设置为完全透明
                    pixels[i] = new Color(0, 0, 0, 0);
                }
            }

            // 将处理后的像素数据应用到输出纹理
            output.SetPixels(pixels);
            output.Apply();  // 应用更改

            return output;
        }

        /// <summary>
        /// 使用 AssetDatabase 将 Texture2D 保存为 PNG 文件
        /// </summary>
        /// <param name="texture">要保存的纹理</param>
        /// <param name="path">保存路径（相对于项目根目录）</param>
        public static void SaveTexture(Texture2D texture, string path)
        {
            // 将 Texture2D 编码为 PNG 格式的字节数组
            byte[] bytes = texture.EncodeToPNG();

            // 将字节数组写入文件
            File.WriteAllBytes(path, bytes);

            // 刷新 AssetDatabase，使新创建的资源在 Unity 编辑器中可见
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 将 PNG 文件导入为 Sprite 资源，并配置 Sprite 属性
        /// 
        /// 配置参数：
        /// - textureType: Sprite (2D and UI) - 设置为精灵图类型
        /// - spriteImportMode: Single - 单图模式（非图集）
        /// - spritePixelsPerUnit: 100 - 每单位像素数
        /// - isReadable: true - 允许从脚本访问纹理像素数据
        /// </summary>
        /// <param name="assetPath">PNG 文件的资源路径</param>
        public static void ConvertSprite(string assetPath)
        {
            // 获取 TextureImporter（纹理导入器）
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null)
            {
                // 设置纹理类型为 Sprite（2D 和 UI 使用）
                importer.textureType = TextureImporterType.Sprite;

                // 设置 Sprite 模式为 Single（单张图片，非图集）
                importer.spriteImportMode = SpriteImportMode.Single;

                // 设置每单位像素数（控制 Sprite 在场景中的大小）
                importer.spritePixelsPerUnit = 100;

                // 允许从脚本读取纹理像素数据（用于后续可能的处理）
                importer.isReadable = true;

                // 重新导入纹理以应用所有设置
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }
            else
            {
                Debug.LogError("Failed to get TextureImporter for the asset.");
            }
        }

        /// <summary>
        /// 确保贴图的 Read/Write 属性正确设置
        /// 
        /// 为什么需要这个功能？
        /// - 使用 GetPixels() 读取像素数据前，必须开启贴图的 Read/Write
        /// - 处理完成后可以关闭 Read/Write 以节省内存
        /// 
        /// </summary>
        /// <param name="texture">需要处理的 Texture2D 对象</param>
        /// <param name="state">目标状态（true: 开启, false: 关闭）</param>
        public static void EnsureTextureReadWrite(Texture2D texture, bool state)
        {
            if (texture == null)
            {
                return;
            }

            // 获取贴图的资源路径
            string assetPath = AssetDatabase.GetAssetPath(texture);
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            // 获取 TextureImporter
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            // 如果当前状态已经是目标状态，则无需操作
            if (importer.isReadable == state)
                return;

            // 设置 Read/Write 状态
            importer.isReadable = state;

            // 重新导入贴图以应用更改
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }
    }
}