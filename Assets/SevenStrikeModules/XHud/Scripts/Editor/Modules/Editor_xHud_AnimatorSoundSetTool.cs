namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Random = UnityEngine.Random;

    [System.Serializable]
    public class AnimateSoundNode
    {
        public XHud_Module_Animator Animator;
        public int Index;
        public string Name;
        public TweenNodeType Type;
        public float MaxDuration;
        public List<TweenSound> TweenSounds = new List<TweenSound>();
    }

    public class Editor_XHud_AnimatorSoundSetTool : EditorWindow
    {
        #region 组件 / 列表
        /// <summary>
        /// 序列化物体
        /// </summary>
        private SerializedObject serializedObject;
        [SerializeField]
        public AnimateSoundNode AnimateSoundNode;
        private ReorderableList ListSounds;
        #endregion

        private float LineHeight = EditorGUIUtility.singleLineHeight;

        #region 序列化属性
        private SerializedProperty sp_AnimateSoundNodes;
        private SerializedProperty sp_TweenSounds;
        #endregion

        #region 选中的预览属性
        private string Selected_Name = "-";
        private string Selected_Hz = "- Hz";
        private string Selected_Length = "0.0 s";
        private string Selected_Steo = "-";
        #endregion

        private EditorCoroutine Coroutine_Preview;
        private Vector2 Scroll;
        public List<AudioSource> PreviewAudioList;

        private float Timer = 0f;
        private float Interval = 4f;

        #region 图标
        private Texture2D Logo_Add_Release, Logo_Add_Press, Logo_Play_Release, Logo_Play_Press, Logo_Apply_Release, Logo_Apply_Press, Icon_twn_sound_percent, Icon_twn_sound_volume, Icon_twn_sound_pitch, anim_type_move, anim_type_rotator, anim_type_scale, anim_type_color, anim_type_fade, anim_type_writter, anim_type_fill, anim_type_size, anim_type_custom_int, anim_type_custom_float, anim_type_custom_vector2, anim_type_custom_vector3, anim_type_custom_vector4, anim_type_custom_color;
        #endregion

        #region 字体
        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;
        #endregion

        private void OnDisable()
        {
            StopAllPreviewAudio();
        }

        private void Update()
        {
            SceneViewUpdate();
        }

        private void SceneViewUpdate()
        {
            Timer += Time.deltaTime;
            if (Timer >= Interval)
            {
                SceneView.RepaintAll();
                Timer = 0f;
            }
        }

        private void OnEnable()
        {
            serializedObject = new SerializedObject(this);

            sp_AnimateSoundNodes = serializedObject.FindProperty("AnimateSoundNode");
            sp_TweenSounds = sp_AnimateSoundNodes.FindPropertyRelative("TweenSounds");

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Dialog");

            #region 获取图标
            Logo_Add_Release = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/Logo_Add_Release");
            Logo_Add_Press = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/Logo_Add_Press");
            Logo_Play_Release = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/Logo_Play_Release");
            Logo_Play_Press = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/Logo_Play_Press");
            Logo_Apply_Release = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/Logo_Apply_Release");
            Logo_Apply_Press = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/Logo_Apply_Press");
            Icon_twn_sound_percent = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/Icon_twn_sound_percent");
            Icon_twn_sound_volume = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/Icon_twn_sound_volume");
            Icon_twn_sound_pitch = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/Icon_twn_sound_pitch");

            anim_type_move = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_move");
            anim_type_rotator = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_rotator");
            anim_type_scale = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_scale");
            anim_type_color = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_color");
            anim_type_fade = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_fade");
            anim_type_writter = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_writter");
            anim_type_fill = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_fill");
            anim_type_size = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_size");
            anim_type_custom_int = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_custom_int");
            anim_type_custom_float = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_custom_float");
            anim_type_custom_vector2 = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_custom_vector2");
            anim_type_custom_vector3 = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_custom_vector3");
            anim_type_custom_vector4 = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_custom_vector4");
            anim_type_custom_color = Editor_XHud_GUI.GetIcon("Icons_Hud_Animator_Sound_Setter/anim_type_custom_color");
            #endregion

            #region 动画列表
            ListSounds = new ReorderableList(serializedObject, sp_TweenSounds)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {

                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    #region Audioclip

                    SerializedProperty sp_clip = sp_TweenSounds.GetArrayElementAtIndex(index).FindPropertyRelative("Sound");
                    SerializedProperty sp_clippath = sp_TweenSounds.GetArrayElementAtIndex(index).FindPropertyRelative("Path");
                    EditorGUI.BeginChangeCheck();
                    Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.width - 130, rect.y + 2.5f, 150, 15), "", sp_clip, 0, 50);
                    if (EditorGUI.EndChangeCheck())
                    {
                        sp_clippath.stringValue = AssetDatabase.GetAssetPath(sp_clip.objectReferenceValue);
                        sp_clippath.serializedObject.ApplyModifiedProperties();
                    }

                    #endregion

                    #region 名称
                    GUI.color = Editor_XHud_GUI.GetColor(HudColor.亮白);
                    AudioClip clip = (AudioClip)sp_clip.objectReferenceValue;
                    string clipname = "";
                    if (clip != null)
                        clipname = clip.name;
                    else
                        clipname = "未指定";
                    Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 5, rect.y + 2, 100, 15), clipname, HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 12, true, TextClipping.Ellipsis);
                    GUI.color = Color.white;
                    #endregion

                    #region 触点
                    SerializedProperty sp_per = sp_TweenSounds.GetArrayElementAtIndex(index).FindPropertyRelative("Percentage");
                    float per = sp_per.floatValue * 100;
                    Editor_XHud_GUI.Gui_Icon(new Rect(rect.x + 5, rect.y + 25, 15, 15), Icon_twn_sound_percent);
                    Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 35, rect.y + 25, 60, 20), per.ToString("F2") + " %", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleRight, new Vector2(0, 0), 12);
                    sp_per.floatValue = EditorGUI.Slider(new Rect(rect.x + 120, rect.y + 25, rect.width - 120, 20), sp_per.floatValue, 0, 1);
                    sp_per.serializedObject.ApplyModifiedProperties();
                    #endregion

                    #region 音量
                    SerializedProperty sp_vol = sp_TweenSounds.GetArrayElementAtIndex(index).FindPropertyRelative("Volume");
                    float vol = sp_vol.floatValue * 100;
                    Editor_XHud_GUI.Gui_Icon(new Rect(rect.x + 5, rect.y + 55, 15, 15), Icon_twn_sound_volume);
                    Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 35, rect.y + 55, 60, 20), vol.ToString("F0") + " %", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleRight, new Vector2(0, 0), 12);
                    sp_vol.floatValue = EditorGUI.Slider(new Rect(rect.x + 120, rect.y + 55, rect.width - 120, 20), sp_vol.floatValue, 0, 1);
                    sp_vol.serializedObject.ApplyModifiedProperties();
                    #endregion

                    #region 音高
                    SerializedProperty sp_min_pitch = sp_TweenSounds.GetArrayElementAtIndex(index).FindPropertyRelative("MinPitch");
                    SerializedProperty sp_max_pitch = sp_TweenSounds.GetArrayElementAtIndex(index).FindPropertyRelative("MaxPitch");

                    float minpitch = sp_min_pitch.floatValue;
                    float maxpitch = sp_max_pitch.floatValue;

                    Editor_XHud_GUI.Gui_Icon(new Rect(rect.x + 5, rect.y + 85, 15, 15), Icon_twn_sound_pitch);
                    EditorGUI.MinMaxSlider(new Rect(rect.x + 55, rect.y + 85, rect.width - 135, 20), ref minpitch, ref maxpitch, -1, 2);

                    sp_min_pitch.floatValue = Editor_XHud_GUI.Gui_InputField_Float(new Rect(rect.width - 45, rect.y + 85, 30, 20), minpitch);
                    sp_max_pitch.floatValue = Editor_XHud_GUI.Gui_InputField_Float(new Rect(rect.width - 10, rect.y + 85, 30, 20), maxpitch);

                    sp_min_pitch.serializedObject.ApplyModifiedProperties();
                    sp_max_pitch.serializedObject.ApplyModifiedProperties();

                    #endregion
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    sp_TweenSounds.DeleteArrayElementAtIndex(list.index);
                    if (sp_TweenSounds.arraySize <= 0)
                    {
                        Selected_Name = null;
                        Selected_Length = null;
                        Selected_Steo = null;
                        Selected_Hz = null;
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_clip = sp_TweenSounds.GetArrayElementAtIndex(list.index).FindPropertyRelative("Sound");
                    SerializedProperty sp_vol = sp_TweenSounds.GetArrayElementAtIndex(list.index).FindPropertyRelative("Volume");
                    SerializedProperty sp_pit_min = sp_TweenSounds.GetArrayElementAtIndex(list.index).FindPropertyRelative("MinPitch");
                    SerializedProperty sp_pit_max = sp_TweenSounds.GetArrayElementAtIndex(list.index).FindPropertyRelative("MaxPitch");
                    AudioClip clip = (AudioClip)sp_clip.objectReferenceValue;
                    if (clip != null)
                    {
                        Selected_Name = clip.name;
                        Selected_Hz = clip.frequency.ToString() + "Hz";
                        Selected_Length = clip.length.ToString("F2") + " s";
                        if (clip.channels == 1)
                            Selected_Steo = "单声道";
                        else
                            Selected_Steo = "立体声";

                        CreatePreviewSound(clip, sp_vol.floatValue, sp_pit_min.floatValue, sp_pit_max.floatValue);
                    }
                },
                elementHeightCallback = index =>
                {
                    return LineHeight * 6.2f;
                }
            };
            #endregion
        }

        private void OnGUI()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            serializedObject.Update();

            #region 标题

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            Editor_XHud_GUI.Gui_Layout_Labelfield(string.IsNullOrEmpty(AnimateSoundNode.Name) ? "未命名动画" : AnimateSoundNode.Name, HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleRight, 18, Font_Bold);
            Editor_XHud_GUI.Gui_Layout_Space(20);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Rect rect_title = GUILayoutUtility.GetLastRect();
            Rect rect_subtitle = new Rect(rect_title.width - 120, rect_title.height + 10, 100, 15);
            Editor_XHud_GUI.Gui_Labelfield_Thin(rect_subtitle, AnimateSoundNode.Type.ToString(), HudFilled.无, HudColor.无, new Color(1, 1, 1, 0.4f), TextAnchor.MiddleRight, new Vector2(0, 0), 12, Font_Light);

            #endregion

            #region 图标
            Rect rect_icon = new Rect(position.size.x - 80, position.size.y - 80, 64, 64);
            GUI.color = new Color(1, 1, 1, 0.06f);
            Texture2D icon = null;
            switch (AnimateSoundNode.Type)
            {
                case TweenNodeType.位移:
                    icon = anim_type_move;
                    break;
                case TweenNodeType.旋转:
                    icon = anim_type_rotator;
                    break;
                case TweenNodeType.缩放:
                    icon = anim_type_scale;
                    break;
                case TweenNodeType.颜色:
                    icon = anim_type_color;
                    break;
                case TweenNodeType.淡化:
                    icon = anim_type_fade;
                    break;
                case TweenNodeType.打字机:
                    icon = anim_type_writter;
                    break;
                case TweenNodeType.图像填充:
                    icon = anim_type_fill;
                    break;
                case TweenNodeType.尺寸:
                    icon = anim_type_size;
                    break;
                case TweenNodeType.自定义整数:
                    icon = anim_type_custom_int;
                    break;
                case TweenNodeType.自定义浮点数:
                    icon = anim_type_custom_float;
                    break;
                case TweenNodeType.自定义2维向量:
                    icon = anim_type_custom_vector2;
                    break;
                case TweenNodeType.自定义3维向量:
                    icon = anim_type_custom_vector3;
                    break;
                case TweenNodeType.自定义4维向量:
                    icon = anim_type_custom_vector4;
                    break;
                case TweenNodeType.自定义颜色:
                    icon = anim_type_custom_color;
                    break;
            }
            Editor_XHud_GUI.Gui_Icon(rect_icon, icon);
            GUI.color = Color.white;
            #endregion

            #region 按钮
            Rect rect_btn_add = new Rect(rect_title.x + 15, rect_title.y + 12, 20, 20);
            Rect rect_btn_play = new Rect(rect_title.x + 60, rect_title.y + 12, 20, 20);
            Rect rect_btn_apply = new Rect(rect_title.x + 110, rect_title.y + 12, 20, 20);

            if (Editor_XHud_GUI.Gui_Button(rect_btn_add, Logo_Add_Release, Logo_Add_Press, true, "", "", Color.white))
            {
                int index = 0;
                if (sp_TweenSounds.arraySize <= 0)
                    index = 0;
                else
                    index = sp_TweenSounds.arraySize;

                sp_TweenSounds.InsertArrayElementAtIndex(index);

                SerializedProperty sp_soundinfo = sp_TweenSounds.GetArrayElementAtIndex(index);
                sp_soundinfo.FindPropertyRelative("Sound").objectReferenceValue = null;
                sp_soundinfo.FindPropertyRelative("Volume").floatValue = 1;
                sp_soundinfo.FindPropertyRelative("MaxPitch").floatValue = 1;
                sp_soundinfo.FindPropertyRelative("MinPitch").floatValue = 1;
                sp_TweenSounds.serializedObject.ApplyModifiedProperties();
            }

            if (Editor_XHud_GUI.Gui_Button(rect_btn_play, Logo_Play_Release, Logo_Play_Press, true, "", "", Color.white))
            {
                if (Application.isPlaying)
                {
                    Debug.Log("程序运行时无法预览音效！");
                    return;
                }
                for (int i = 0; i < AnimateSoundNode.TweenSounds.Count; i++)
                {
                    float x = AnimateSoundNode.TweenSounds[i].Percentage * (AnimateSoundNode.Animator.Animator_GlobalDuration * AnimateSoundNode.Animator.TweenNode_GetByIndex(AnimateSoundNode.Index).Duration);
                    PreviewSounds(AnimateSoundNode.TweenSounds[i].Sound, x, AnimateSoundNode.TweenSounds[i].Volume, AnimateSoundNode.TweenSounds[i].MinPitch, AnimateSoundNode.TweenSounds[i].MaxPitch);
                }
            }

            if (Editor_XHud_GUI.Gui_Button(rect_btn_apply, Logo_Apply_Release, Logo_Apply_Press, true, "", "", Color.white))
            {
                #region 获取动画器节点
                SerializedProperty sp_animator = sp_AnimateSoundNodes.FindPropertyRelative("Animator");
                SerializedProperty sp_index = sp_AnimateSoundNodes.FindPropertyRelative("Index");

                XHud_Module_Animator anim = (XHud_Module_Animator)sp_animator.objectReferenceValue;

                SerializedObject so_anim = new SerializedObject(anim);
                so_anim.Update();
                SerializedProperty sp_tweennodes = so_anim.FindProperty("AnimateTweenNodes");
                SerializedProperty sp_indexnode = sp_tweennodes.GetArrayElementAtIndex(sp_index.intValue);

                SerializedProperty sp_sounds = sp_indexnode.FindPropertyRelative("TweenSounds");

                sp_sounds.ClearArray();

                so_anim.ApplyModifiedProperties();
                #endregion

                for (int i = 0; i < sp_TweenSounds.arraySize; i++)
                {
                    SerializedProperty sp_sound = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("Sound");
                    SerializedProperty sp_percent = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("Percentage");
                    SerializedProperty sp_path = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("Path");
                    SerializedProperty sp_vol = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("Volume");
                    SerializedProperty sp_minpitch = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("MinPitch");
                    SerializedProperty sp_maxpitch = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("MaxPitch");

                    sp_sounds.InsertArrayElementAtIndex(i);
                    SerializedProperty sp_soundnode = sp_sounds.GetArrayElementAtIndex(i);
                    SerializedProperty sp_x_sound = sp_soundnode.FindPropertyRelative("Sound");
                    SerializedProperty sp_x_percent = sp_soundnode.FindPropertyRelative("Percentage");
                    SerializedProperty sp_x_path = sp_soundnode.FindPropertyRelative("Path");
                    SerializedProperty sp_x_vol = sp_soundnode.FindPropertyRelative("Volume");
                    SerializedProperty sp_x_min_pitch = sp_soundnode.FindPropertyRelative("MinPitch");
                    SerializedProperty sp_x_max_pitch = sp_soundnode.FindPropertyRelative("MaxPitch");

                    sp_x_sound.objectReferenceValue = sp_sound.objectReferenceValue;
                    sp_x_percent.floatValue = sp_percent.floatValue;
                    sp_x_path.stringValue = sp_path.stringValue;
                    sp_x_vol.floatValue = sp_vol.floatValue;
                    sp_x_min_pitch.floatValue = sp_minpitch.floatValue;
                    sp_x_max_pitch.floatValue = sp_maxpitch.floatValue;

                    sp_x_sound.serializedObject.ApplyModifiedProperties();
                    sp_x_percent.serializedObject.ApplyModifiedProperties();
                    sp_x_path.serializedObject.ApplyModifiedProperties();
                    sp_x_vol.serializedObject.ApplyModifiedProperties();
                    sp_x_min_pitch.serializedObject.ApplyModifiedProperties();
                    sp_x_max_pitch.serializedObject.ApplyModifiedProperties();
                }
            }
            #endregion

            #region 判断音效库中是否存在音效，如果没有则加入
            if (mgr.Hud_Sounds != null)
            {
                if (AnimateSoundNode.TweenSounds != null && AnimateSoundNode.TweenSounds.Count > 0)
                {
                    for (int i = 0; i < AnimateSoundNode.TweenSounds.Count; i++)
                    {
                        if (AnimateSoundNode.TweenSounds[i] != null)
                        {
                            if (AnimateSoundNode.TweenSounds[i].Sound != null)
                            {
                                AudioClip sod = AnimateSoundNode.TweenSounds[i].Sound;
                                int libcount = mgr.Hud_Sounds.SoundLibrary_GetSoundCount();

                                bool repeat = false;
                                for (int s = 0; s < libcount; s++)
                                {
                                    if (mgr.Hud_Sounds.SoundLibrary_GetSound(s).name == sod.name)
                                    {
                                        repeat = true;
                                        break;
                                    }
                                }
                                if (!repeat)
                                {
                                    mgr.Hud_Sounds.SoundLibrary_AddSound(sod.name, sod);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(25);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Scroll = EditorGUILayout.BeginScrollView(Scroll, GUILayout.Width(position.width - 20), GUILayout.ExpandWidth(true));
            ListSounds.DoLayoutList();
            EditorGUILayout.EndScrollView();
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(100);

            #region 信息

            Rect rect_info_name = new Rect(15, position.size.y - 80, position.size.x - 120, 22);
            Rect rect_info_hz = new Rect(15, position.size.y - 58, position.size.x, 22);
            Rect rect_info_channel = new Rect(15, position.size.y - 36, 70, 22);
            Rect rect_info_length = new Rect(70, position.size.y - 36, position.size.x, 22);

            Editor_XHud_GUI.Gui_Labelfield(rect_info_name, Selected_Name, HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, new Vector2(0, 0), 14, TextClipping.Ellipsis);
            Editor_XHud_GUI.Gui_Labelfield(rect_info_hz, Selected_Hz, HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, new Vector2(0, 0), 11);
            Editor_XHud_GUI.Gui_Labelfield(rect_info_channel, Selected_Steo, HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, new Vector2(0, 0), 11);
            Editor_XHud_GUI.Gui_Labelfield(rect_info_length, Selected_Length, HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, new Vector2(0, 0), 11);

            #endregion

            serializedObject.ApplyModifiedProperties();

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                this.Close();
            }
        }

        private void PreviewSounds(AudioClip clip, float time, float vol, float pitch_min, float pitch_max)
        {
            Coroutine_Preview = EditorCoroutineUtility.StartCoroutine(PreviewSound(clip, time, vol, pitch_min, pitch_max), this);
        }

        IEnumerator PreviewSound(AudioClip clip, float time, float vol, float pitch_min, float pitch_max)
        {
            yield return new EditorWaitForSeconds(time);

            if (PreviewAudioList == null)
                PreviewAudioList = new List<AudioSource>();
            PreviewAudioList.Add(CreatePreviewSound(clip, vol, pitch_min, pitch_max));
        }

        /// <summary>
        /// 预览声音
        /// </summary>
        /// <param name="clip"></param>
        public AudioSource CreatePreviewSound(AudioClip clip, float vol, float pit_min, float pit_max)
        {
            GameObject obj = new GameObject();
            obj.name = "SoundPreviewer-" + "[" + clip.length.ToString("F2") + " s]-" + "[" + clip.channels + " ch]-" + "[" + clip.frequency + " hz]";
            AudioSource au = obj.AddComponent<AudioSource>();
            au.clip = clip;
            au.volume = vol;
            au.pitch = Random.Range(pit_min, pit_max);
            au.Play();
            XHud_AudioStoper sp = au.gameObject.AddComponent<XHud_AudioStoper>();
            sp.AudioSource = au;
            return au;
        }

        private void StopAllPreviewAudio()
        {
            if (Coroutine_Preview != null)
            {
                EditorCoroutineUtility.StopCoroutine(Coroutine_Preview);
                Coroutine_Preview = null;
            }
            if (PreviewAudioList != null)
            {
                for (int i = 0; i < PreviewAudioList.Count; i++)
                {
                    if (PreviewAudioList[i] != null)
                    {
                        PreviewAudioList[i].Stop();
                        DestroyImmediate(PreviewAudioList[i].gameObject, true);
                        PreviewAudioList[i] = null;
                    }
                }
                PreviewAudioList.Clear();
            }

            SceneView.RepaintAll();
        }
    }
}
