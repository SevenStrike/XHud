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
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Element : Editor
    {
        /// <summary>
        /// 预览元素身上挂载的所有音效器的音效（单选路径）。
        /// <para/>
        /// 【参数来源】SerializedProperty（serializedObject 即当前 target）。
        /// <para/>
        /// 本方法只负责"筛选出要预览的 Sounder"，参数计算交给 <see cref="Preview_ElementSounder_Item"/>。
        /// </summary>
        /// <param name="Timings">匹配时机</param>
        /// <param name="sp_sounds">音效节点列表对象</param>
        private void Preview_ElementSounder_OnSerialize(string Timings, SerializedProperty sp_sounds)
        {
            if (HudManager == null || HudManager.Hud_Sounds == null) return;

            for (int i = 0; i < sp_sounds.arraySize; i++)
            {
                SerializedProperty sp_soundnode = sp_sounds.GetArrayElementAtIndex(i);
                SerializedProperty sp_sounder = sp_soundnode.FindPropertyRelative("Sounder");
                if (sp_sounder.objectReferenceValue == null) continue;

                XHud_Element_Sounder sounder = sp_sounder.objectReferenceValue as XHud_Element_Sounder;
                if (sounder == null) continue;
                if (sounder.Timings != Timings) continue;

                Preview_ElementSounder_Item(sounder);
            }
        }
        /// <summary>
        /// 预览元素身上挂载的所有音效器的音效（多选路径）。
        /// <para/>
        /// 【参数来源】List&lt;ElementNode_Sound&gt;，遍历每个 SelectedObjects[i] 各取各的。
        /// </summary>
        /// <param name="Timings">匹配时机</param>
        /// <param name="soundnodes">音效节点列表对象</param>
        private void Preview_ElementSounder_OnList(string Timings, List<ElementNode_Sound> soundnodes)
        {
            if (HudManager == null || HudManager.Hud_Sounds == null) return;

            for (int i = 0; i < soundnodes.Count; i++)
            {
                ElementNode_Sound node = soundnodes[i];
                if (node == null || node.Sounder == null) continue;
                if (node.Sounder.Timings != Timings) continue;

                Preview_ElementSounder_Item(node.Sounder);
            }
        }
        /// <summary>
        /// 把单个 Sounder 的预览音效"算好参数"后交给 helper 入队。
        /// <para/>
        /// 【参数来源】
        /// <list type="bullet">
        /// <item><description>clip：从音效库按 SoundName 取；取不到则跳过。</description></item>
        /// <item><description>delay：sounder.DelayTime 原值（不乘任何倍增系数）。</description></item>
        /// <item><description>volume：sounder.Volume × HudManager.Volume（对齐运行时 DelayPlay 算法）。</description></item>
        /// <item><description>mute：HudManager.VolumeMute（对齐运行时）。</description></item>
        /// <item><description>pitch：sounder.UseRandomPitch 为 true 时在 [Pitch_Min, Pitch_Max] 随机，否则 1。</description></item>
        /// </list>
        /// </summary>
        private void Preview_ElementSounder_Item(XHud_Element_Sounder sounder)
        {
            if (sounder == null) return;
            if (string.IsNullOrEmpty(sounder.SoundName)) return;

            AudioClip clip = HudManager.Hud_Sounds.SoundLibrary_GetSound(sounder.SoundName);
            if (clip == null) return;

            // ── 算好参数 ──
            float delay = Mathf.Max(0f, sounder.DelayTime);

            float masterVol = (HudManager != null) ? HudManager.Volume * 0.01f : 1f;
            bool masterMute = (HudManager != null) && HudManager.VolumeMute;
            float volume = masterVol * Mathf.Clamp01(sounder.Volume);
            bool mute = masterMute;

            bool useRandomPitch = sounder.UseRandomPitch;
            float pMin = sounder.Pitch_Min;
            float pMax = sounder.Pitch_Max;

            XGUI.EditorPlayer_Sound.Enqueue(
                clip, delay, volume, mute, useRandomPitch, pMin, pMax);
        }
        /// <summary>
        /// 预览子图元动画器上匹配指定时机的音效。
        /// <para/>
        /// 【延迟公式】sod.Delay × HudManager.DurationMultiply × globalDuration + nodeDelay
        /// 与子图元动画的延迟公式保持一致（动画延迟 = tween.Delay + node.DelayTime）。
        /// </summary>
        private void Preview_PrimitiveTween_Sound(XHud_Module_Primitive_Tween tweener, string timing, float globalDuration, float nodeDelay)
        {
            if (tweener == null) return;
            if (tweener.MutePlay) return;
            if (EditorUtility.audioMasterMute) return;
            if (string.IsNullOrEmpty(timing)) return;

            for (int s = 0; s < tweener.PrimitiveTweenSounds.Count; s++)
            {
                TweenSound sod = tweener.PrimitiveTweenSounds[s];
                if (sod == null) continue;
                if (sod.Mute) continue;
                if (sod.Timing != timing) continue;

                AudioClip clip = Resolve_PrimitiveSoundClip(sod);
                if (clip == null) continue;

                // ── 延迟：与子图元动画对齐 ──
                float mul = 1f;
                if (HudManager != null && HudManager.DurationMultiply > 0f)
                    mul = HudManager.DurationMultiply;
                mul *= globalDuration;

                float delay = Mathf.Max(0f, sod.Delay * mul + nodeDelay);

                // ── 音量 / 音高 / 静音 ──
                float masterVol = (HudManager != null) ? HudManager.Volume * 0.01f : 1f;
                bool masterMute = (HudManager != null) && HudManager.VolumeMute;
                float volume = masterVol * Mathf.Clamp01(sod.Volume);
                bool mute = masterMute;

                float pMin = Mathf.Min(sod.MinPitch, sod.MaxPitch);
                float pMax = Mathf.Max(sod.MinPitch, sod.MaxPitch);
                bool useRandomPitch = !Mathf.Approximately(pMin, 1f) || !Mathf.Approximately(pMax, 1f);

                XGUI.EditorPlayer_Sound.Enqueue(
                    clip, delay, volume, mute, useRandomPitch, pMin, pMax);
            }
        }
        /// <summary>
        /// 从音效库安全取出一条 AudioClip（与图元侧 ResolvePreviewClip 逻辑一致）。
        /// </summary>
        private AudioClip Resolve_PrimitiveSoundClip(TweenSound sod)
        {
            if (sod == null || sod.Sound == null) return null;

            if (HudManager != null && HudManager.Hud_Sounds != null)
            {
                string soundName = sod.Sound.name;
                if (!string.IsNullOrEmpty(soundName))
                {
                    AudioClip clip = HudManager.Hud_Sounds.SoundLibrary_GetSound(soundName);
                    if (clip != null) return clip;
                }
            }
            return sod.Sound;
        }
    }
}