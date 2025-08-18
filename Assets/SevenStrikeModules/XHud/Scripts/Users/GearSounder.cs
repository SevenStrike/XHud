namespace SevenStrikeModules.XHud.Example
{
    using SevenStrikeModules.XHud.Hud;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class GearSounder : MonoBehaviour
    {
        public Hud_Animator anim;
        public AudioClip Clip;
        public TweenNode node;

        public float interval = 2f;
        private float timer = 0f;

        public float Pitch_Min = 1;
        public float Pitch_Max = 1;
        public float Volume = 1;

        public List<AudioSource> SoundList = new List<AudioSource>();

        void Start()
        {

        }

        private void OnEnable()
        {
            node = anim.TweenNode_GetByIndicator("GearRotator");
            if (node != null)
            {
                node.Act_On_Quaternion_Changed += Act_On_Quaternion_Changed;
            }
        }

        private void OnDisable()
        {
            if (node != null)
            {
                node.Act_On_Quaternion_Changed -= Act_On_Quaternion_Changed;
            }
        }

        private void Act_On_Quaternion_Changed(Quaternion arg0)
        {
            if (!anim.IsAnimating())
                return;

            timer += Time.deltaTime;

            if (timer >= interval)
            {
                CreateSound();
                timer = 0f;
            }
        }

        private void CreateSound()
        {
            GameObject Sound = new GameObject();
            Sound.name = "GearSound";
            Sound.transform.SetParent(transform);
            AudioSource au = Sound.AddComponent<AudioSource>();
            au.pitch = Random.Range(Pitch_Min, Pitch_Max);
            au.volume = Volume;
            au.playOnAwake = false;
            au.clip = Clip;
            au.Play();

            SoundList.Add(au);
        }

        void Update()
        {
            if (SoundList != null && SoundList.Count > 0)
            {
                for (int i = 0; i < SoundList.Count; i++)
                {
                    if (!SoundList[i].isPlaying)
                    {
                        DestroyImmediate(SoundList[i].gameObject);
                        SoundList.RemoveAt(i);
                    }
                }
            }
        }
    }
}