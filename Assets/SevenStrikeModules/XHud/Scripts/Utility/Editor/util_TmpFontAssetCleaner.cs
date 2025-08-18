namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.GuiLib;
    using TMPro;
    using UnityEditor;
    using UnityEngine;

    public class util_TmpFontAssetCleaner : EditorWindow
    {
        [MenuItem("Assets/XHud/ClearTmpFontAssetData (清理TMP字体资源图集缓存内容)", priority = 2000, validate = true)]
        private static bool ValidateClearTmpFontAssetContent()
        {
            // 获取当前选中的对象
            Object selectedObject = Selection.activeObject;

            // 检查选中的对象是否为 TMP_FontAsset 类型
            return selectedObject is TMP_FontAsset;
        }

        [MenuItem("Assets/XHud/ClearTmpFontAssetData (清理TMP字体资源图集缓存内容)", priority = 2000)]
        private static void ClearTmpFontAssetContent()
        {
            Object FontAsset = Selection.activeObject;

            string res = util_XHUDGUI.Open(XHudDialogType.警告, "XHud TmpFontAssets清理器消息", "清理TmpFontAssetAtlas", $"您确认要将 {FontAsset.name} 图集内容清空吗？清空后字体图集将保持纯净状态！此操作不可逆，请谨慎操作！", "清空", "暂不", 1);
            if (res == "暂不")
            {
                return;
            }
            else
            {
                try
                {
                    TMP_FontAsset sd = (TMP_FontAsset)FontAsset;

                    sd.ClearFontAssetData();
                }
                catch (System.Exception err)
                {
                    util_XHUDGUI.Open(XHudDialogType.警告, "XHud TmpFontAssets清理器消息", "清理TmpFontAssetAtlas", $"您选中的物体 {FontAsset.name} 并非是TmpFontAsset类型！,详细信息： {err.Message}", "明白");
                }
            }
        }
    }
}