namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using UnityEngine;

    public partial class XHud_Manager : MonoBehaviour
    {
        [Tooltip("音效池")]
        /// <summary>
        /// 音效池
        /// </summary>
        public AudioPlayer[] Pool_Sounder;
        [Tooltip("音效池预加载数量")]
        /// <summary>
        /// 音效池预加载数量
        /// </summary>
        public int SounderPoolCount;
        [Range(0, 100)]
        [Tooltip("影响所有UI的音效音量")]
        /// <summary>
        /// 音效音量
        /// </summary>        
        public float Volume = 100;
        [Tooltip("勾选后所有音效都会静音")]
        /// <summary>
        /// 音效静音
        /// </summary>
        public bool VolumeMute;

        /// <summary>
        /// 初始化音效播放器池
        /// 预创建指定数量的 AudioSource 组件，用于播放 UI 音效
        /// 
        /// 工作原理：
        /// 1. 创建 Pool_Sound 根节点作为音效播放器容器
        /// 2. 根据 SounderPoolCount 创建指定数量的 AudioSource 实例
        /// 3. 配置每个 AudioSource 的基本参数（最小/最大距离、不自动播放）
        /// 4. 将 AudioSource 存入 Pool_Sounder 数组供后续使用
        /// 
        /// 为什么需要音效池？
        /// - UI 音效通常很短促（点击、悬停、开关切换等）
        /// - 频繁创建 AudioSource 会产生大量 GC 和性能开销
        /// - 通过对象池复用 AudioSource，避免重复创建销毁
        /// 
        /// 音效池的使用流程：
        /// 1. 初始化：创建 N 个 AudioSource（N = SounderPoolCount）
        /// 2. 播放时：从池中查找空闲的 AudioSource（!isPlaying）
        /// 3. 设置音频剪辑、音量、音高 → 播放
        /// 4. 播放完成后，AudioSource 自动变为空闲状态，可被复用
        /// 
        /// 配置说明：
        /// - minDistance/maxDistance 设置为 0.1/0.2，确保 UI 音效不受 3D 空间影响
        /// - playOnAwake = false，避免未经授权自动播放
        /// </summary>
        /// <param name="count">预创建的 AudioSource 数量（SounderPoolCount）</param>
        private void hm_LibrarySounds_Initialize(int count)
        {
            if (count == 0)
                return;

            Pool_Sounder = new AudioPlayer[count];

            GameObject PoolRoot = new GameObject();
            PoolRoot.name = "Pool_Sound";
            PoolRoot.transform.SetParent(transform);
            PoolRoot.transform.localPosition = Vector3.zero;
            PoolRoot.transform.localEulerAngles = Vector3.zero;
            PoolRoot.transform.localScale = Vector3.one;

            for (int i = 0; i < Pool_Sounder.Length; i++)
            {
                AudioPlayer ap = new AudioPlayer();
                GameObject obj = new GameObject();
                obj.name = "Sounder_" + i;
                obj.transform.SetParent(PoolRoot.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localEulerAngles = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                ap.Player = obj.AddComponent<AudioSource>();
                ap.Player.minDistance = 0.1f;
                ap.Player.maxDistance = 0.2f;
                ap.Player.playOnAwake = false;
                Pool_Sounder[i] = ap;
            }
        }
        /// <summary>
        /// 取得一个音效播放器
        /// </summary>
        /// <returns></returns>
        public AudioSource hm_LibrarySounds_GetSounder()
        {
            AudioSource aus = null;
            if (Pool_Sounder == null && Pool_Sounder.Length <= 0)
            {
                Debug.Log("您未指定音效池的数量！因此无法播放声音！请前往HudManager中配置音效池的数量！");
                return null;
            }
            for (int i = 0; i < Pool_Sounder.Length; i++)
            {
                if (!Pool_Sounder[i].Player.isPlaying)
                {
                    aus = Pool_Sounder[i].Player;
                    break;
                }
            }
            return aus;
        }
        /// <summary>
        /// 根据音效名字取得其在音效库中的索引号
        /// </summary>
        /// <returns></returns>
        public int hm_LibrarySounds_GetSoundIndex(string name)
        {
            int index = -1;
            for (int i = 0; i < Hud_Sounds.SoundLibrary.Count; i++)
            {
                if (Hud_Sounds.SoundLibrary[i].Name == name)
                {
                    index = i;
                }
            }
            if (index == -1)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("xHud Manager管理器消息", "无法在音效库里找到目标名称的索引号！", HudMsgState.警告);
                return 0;
            }
            else
            {
                return index;
            }
        }
        /// <summary>
        /// 根据音效索引号取得其在音效库中的名称
        /// </summary>
        /// <returns></returns>
        public string hm_LibrarySounds_GetSoundName(int index)
        {
            if (Hud_Sounds.SoundLibrary_IndexIsValid(index))
            {
                return Hud_Sounds.SoundLibrary[index].Name;
            }
            else
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("xHud Manager管理器消息", "无法在音效库里找到目标索引号的音效名称！", HudMsgState.警告);
                return "";
            }
        }
        /// <summary>
        /// 音效播放器池状态更新
        /// 同步音效池中每个 AudioSource 的播放状态到对应的 AudioPlayer 记录中
        /// 
        /// 工作原理：
        /// 1. 检查音效播放器池（Pool_Sounder）是否已初始化且不为空
        /// 2. 遍历池中的所有 AudioPlayer
        /// 3. 将每个 AudioSource 当前的播放状态（isPlaying）同步到 AudioPlayer.IsPlaying 字段
        /// 
        /// 为什么需要同步播放状态？
        /// - 外部系统可能需要查询某个音效是否正在播放
        /// - 音效池管理需要知道哪些 AudioSource 空闲，以便复用
        /// - 调试时查看音效播放状态
        /// 
        /// 音效池复用逻辑：
        /// 1. 播放音效时，遍历 Pool_Sounder 找到 IsPlaying = false 的 AudioPlayer
        /// 2. 设置音频剪辑、音量、音高并播放
        /// 3. 播放过程中 IsPlaying 为 true，不会被其他音效复用
        /// 4. 播放完成后 IsPlaying 自动变为 false，可被下一个音效复用
        /// 
        /// 使用场景：
        /// - 为 hm_LibrarySounds_GetSounder() 提供准确的空闲状态信息
        /// - 调试时查看哪些音效正在播放
        /// - 音效池的监控和管理
        /// 
        /// 调用频率：
        /// - 每帧在 XHud_Manager.Update() 中调用
        /// - 开销极小，仅遍历数组并读取 isPlaying 属性
        /// 
        /// 性能考虑：
        /// - 音效池大小（SounderPoolCount）通常不会太大（10-30 个）
        /// - 每帧遍历开销可以忽略不计
        /// 
        /// 注意事项：
        /// - 如果音效池未初始化（Pool_Sounder 为 null 或长度为 0），方法直接返回
        /// - 此方法仅同步状态，不创建或销毁 AudioSource
        /// </summary>
        public void hm_LibrarySounds_Update()
        {
            if (Pool_Sounder == null || Pool_Sounder.Length <= 0)
                return;
            for (int i = 0; i < Pool_Sounder.Length; i++)
            {
                Pool_Sounder[i].IsPlaying = Pool_Sounder[i].Player.isPlaying;
            }
        }
        /// <summary>
        /// 播放器池清理
        /// </summary>
        public void hm_LibrarySounds_Clean()
        {
            for (int i = 0; i < Pool_Sounder.Length; i++)
            {
                if (Pool_Sounder[i].Player.clip != null)
                {
                    Pool_Sounder[i].Player.clip = null;
                }
                Pool_Sounder[i].Player.transform.localPosition = Vector3.zero;
                Pool_Sounder[i].Player.transform.localEulerAngles = Vector3.zero;
                Pool_Sounder[i].Player.transform.localScale = Vector3.one;
            }
        }
    }
}