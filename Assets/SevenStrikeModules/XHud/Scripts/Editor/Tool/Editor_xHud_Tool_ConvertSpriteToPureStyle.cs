namespace SevenStrikeModules.XHud.Utilitys
{
    using SevenStrikeModules.XHud.GuiLib;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [CustomEditor(typeof(Image))]
    public class Editor_XHud_Tool_ConvertSpriteToPureStyle : UnityEditor.UI.ImageEditor
    {
        // 在Image组件的上下文菜单中添加一个自定义选项
        [MenuItem("CONTEXT/Image/ConvertToPureSprite", priority = 100)]
        private static void ConvertPureSprite(MenuCommand command)
        {
            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud PSD Reconstruction消息", "RMS残留信息", "是否需要为当前Image中的Sprite创建纯净Sprite并指定？", "创建", "暂不", 1);
            if (res == "暂不")
            {
                return;
            }

            // 获取选中的Image组件
            Image selectedImage = (Image)command.context;

            if (selectedImage.sprite == null)
                return;

            Texture2D tex = selectedImage.sprite.texture;

            string path_tex = AssetDatabase.GetAssetPath(tex);
            // 获取源贴图的路径
            string directory = Path.GetDirectoryName(path_tex);
            // 获取文件名（不含扩展名）
            string fileName = Path.GetFileNameWithoutExtension(path_tex);
            // 新文件名
            string newFileName = $"{fileName}_Desaturated.png";
            // 新路径
            string newPath = Path.Combine(directory, newFileName);

            Debug.Log(newPath);

            // 处理贴图去饱和度
            Texture2D puretex = Desaturator(tex);

            // 将处理后的Texture2D保存
            SaveTexture(puretex, newPath);

            // 将PNG文件导入为Sprite资源
            ConvertSprite(newPath);

            selectedImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(newPath);
        }

        /// <summary>
        /// 去色处理并将不透明部分调整为纯白色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Texture2D Desaturator(Texture2D input)
        {
            EnsureTextureReadWrite(input, true);

            TextureFormat targetFormat = TextureFormat.RGBA32;
            // 创建一个新的Texture2D对象，大小和格式与输入纹理相同
            Texture2D output = new Texture2D(input.width, input.height, targetFormat, false);

            // 获取输入纹理的像素数据
            Color[] pixels = input.GetPixels();

            EnsureTextureReadWrite(input, false);

            // 遍历每个像素
            for (int i = 0; i < pixels.Length; i++)
            {
                Color pixel = pixels[i];
                float alpha = pixel.a; // 获取当前像素的透明度

                // 如果像素不透明（透明度_Alpha > 0），将其颜色值设置为纯白色
                if (alpha > 0)
                {
                    pixels[i] = new Color(1.0f, 1.0f, 1.0f, alpha); // 设置为纯白色并保留透明度
                }
                else
                {
                    pixels[i] = new Color(0, 0, 0, 0); // 透明部分保持不变
                }
            }

            // 将处理后的像素数据应用到输出纹理
            output.SetPixels(pixels);
            output.Apply();


            return output;
        }

        /// <summary>
        /// 使用AssetDatabase保存Texture2D为PNG文件
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="path"></param>
        public static void SaveTexture(Texture2D texture, string path)
        {
            // 将Texture2D转换为PNG格式的字节数组
            byte[] bytes = texture.EncodeToPNG();

            // 将字节数组保存为文件
            File.WriteAllBytes(path, bytes);

            // 刷新AssetDatabase以使新资源可见
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 将PNG文件导入为Sprite资源，并设置Sprite Mode为Single
        /// </summary>
        /// <param name="assetPath"></param>
        public static void ConvertSprite(string assetPath)
        {
            // 获取TextureImporter
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null)
            {
                // 设置Texture Type为Sprite (2D and UI)
                importer.textureType = TextureImporterType.Sprite;
                // 设置Sprite Mode为Single
                importer.spriteImportMode = SpriteImportMode.Single;
                // 设置其他相关属性（可选）
                importer.spritePixelsPerUnit = 100; // 根据需要调整
                importer.isReadable = true; // 如果需要从脚本访问纹理数据，需要设置为true

                // 重新导入纹理以应用设置
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }
            else
            {
                Debug.LogError("Failed to get TextureImporter for the asset.");
            }
        }

        /// <summary>
        /// 确保贴图的Read/Write属性正确设置。
        /// 如果贴图的Read/Write未开启，则开启；如果已开启，则关闭。
        /// </summary>
        /// <param name="texture">需要处理的Texture2D对象。</param>
        public static void EnsureTextureReadWrite(Texture2D texture, bool state)
        {
            if (texture == null)
            {
                //sp_DebugMode.LogError("Texture is not assigned.");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(texture);
            if (string.IsNullOrEmpty(assetPath))
            {
                //sp_DebugMode.LogError("Texture is not an asset in the project.");
                return;
            }

            // 获取TextureImporter
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                //sp_DebugMode.LogError("Failed to get TextureImporter for the asset.");
                return;
            }

            if (importer.isReadable == state)
                return;

            importer.isReadable = state; // 开启Read/Write

            // 重新导入贴图以应用更改
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            //sp_DebugMode.Log($"Texture Read/Write property updated for: {assetPath}");
        }
    }
}