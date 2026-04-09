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
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Element : Editor
    {
        #region Preview - HudSounder
        private List<AudioSource> Preivew_HudSounder_SoundList = new List<AudioSource>();
        private List<EditorCoroutine> Preivew_HudSounder_CoroutineList_Stop = new List<EditorCoroutine>();
        #endregion

        /// <summary>
        ///  HudSounder 音效预览
        /// </summary>
        IEnumerator Preview_HudSounder_Play(float sp_vol, float sp_pitch_min, float sp_pitch_max, bool sp_userandom, AudioClip clip, float delay)
        {
            yield return new EditorWaitForSeconds(delay);
            Preivew_HudSounder_SoundList.Add(Preview_HudSounder_CreateSound(sp_vol, sp_pitch_min, sp_pitch_max, sp_userandom, clip));
            AudioSource au = Preivew_HudSounder_SoundList[Preivew_HudSounder_SoundList.Count - 1];
            while (true)
            {
                if (au != null && !au.isPlaying)
                {
                    break;
                }
                yield return null;
            }
            DestroyImmediate(au.gameObject, true);
        }
        /// <summary>
        /// 创建 HudSounder 预览指定声音
        /// </summary>
        /// <param name="sp_vol"></param>
        /// <param name="sp_pitch_min"></param>
        /// <param name="sp_pitch_max"></param>
        /// <param name="sp_userandom"></param>
        /// <param name="clip"></param>
        /// <returns></returns>
        public AudioSource Preview_HudSounder_CreateSound(float sp_vol, float sp_pitch_min, float sp_pitch_max, bool sp_userandom, AudioClip clip)
        {
            GameObject obj = new GameObject();
            obj.name = "HudSound_Previewer-" + "[" + clip.length.ToString("F2") + " s]-" + "[" + clip.channels + " ch]-" + "[" + clip.frequency + " hz]";
            AudioSource au = obj.AddComponent<AudioSource>();
            au.clip = clip;
            au.volume = sp_vol;
            if (sp_userandom)
            {
                au.pitch = Random.Range(sp_pitch_min, sp_pitch_max);
            }
            else
            {
                au.pitch = 1.0f;
            }
            au.Play();
            XHud_AudioStoper sp = au.gameObject.AddComponent<XHud_AudioStoper>();
            sp.SetAudioSource(au);
            return au;
        }
        /// <summary>
        ///  停止协程列表 - HudSounder 音效预览播放 / 停止播放并清空 HudSounder 预览列表与生成的音效物体
        /// </summary>
        private void Preview_HudSounder_Coroutine_Stop()
        {
            for (int i = 0; i < Preivew_HudSounder_CoroutineList_Stop.Count; i++)
            {
                if (Preivew_HudSounder_CoroutineList_Stop[i] != null)
                    EditorCoroutineUtility.StopCoroutine(Preivew_HudSounder_CoroutineList_Stop[i]);
            }
            Preivew_HudSounder_CoroutineList_Stop.Clear();

            if (Preivew_HudSounder_SoundList != null)
            {
                for (int i = 0; i < Preivew_HudSounder_SoundList.Count; i++)
                {
                    if (Preivew_HudSounder_SoundList[i] != null)
                    {
                        Preivew_HudSounder_SoundList[i].Stop();
                        DestroyImmediate(Preivew_HudSounder_SoundList[i].gameObject, true);
                        Preivew_HudSounder_SoundList[i] = null;
                    }
                }
                Preivew_HudSounder_SoundList.Clear();
            }

            SceneView.RepaintAll();
        }
    }
}