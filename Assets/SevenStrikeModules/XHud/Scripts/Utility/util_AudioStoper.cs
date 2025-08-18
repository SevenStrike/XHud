namespace SevenStrikeModules.XHud.Utilitys
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [ExecuteInEditMode]
    public class util_AudioStoper : MonoBehaviour
    {
        public AudioSource AudioSource;

        /// <summary>
        /// 设置播放器
        /// </summary>
        /// <param tweenName="au"></param>
        public void SetAudioSource(AudioSource au)
        {
            AudioSource = au;
        }

        private void Update()
        {
            if (AudioSource != null)
            {
                if (!AudioSource.isPlaying)
                {
                    transform.localPosition = Vector3.zero;
                    transform.localEulerAngles = Vector3.zero;
                    transform.localScale = Vector3.one;

                    Invoke("DestroyAudioObject", 0.3f);
                }
            }
        }

        private void DestroyAudioObject()
        {
            DestroyImmediate(gameObject, true);
        }
    }
}