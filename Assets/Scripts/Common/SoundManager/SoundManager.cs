#pragma warning disable 0414    // CS0414 警告の回避。変数をｐ利用していても利用していないという警告が出るため

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public enum BGM_TYPE {
    Title,
    Clear,
    Boss,
    Stage_0,
    Stage_1,
    Stage_2,
    Stage_3,

}


public enum SE_TYPE {
    Hit_1,
    Hit_2,
    Hit_3,
    Hit_4,
    Hit_5,
    Critical,
    Miss,
    Magic_1,
    TrapDisarm,
    Drop,
    OpenTreasure,
    Heal,
    MemoryFragments,
    Debuff,
    Buff,
    Shield,
    Parry,
    Reflect,
    Absorb,
}

public enum VOICE_TYPE {
    Attack_1,  // えい
    Attack_2,  // そこです
    Win,       // やりました
    Loose,     // 負けました
    Timeout,   // 目が回ります
    Damage_1,  // いた
    Damage_2,  // きゃあ
    EnterBoss, // 強そう
}

/// <summary>
/// 音管理クラス
/// </summary>
public class SoundManager : AbstractSingleton<SoundManager> {
	// 音量
	public SoundVolume volume = new SoundVolume();

    // === AudioSource ===
    // BGM
    [SerializeField]
	private AudioSource[] BGMsources = new AudioSource[2];
	// SE
	private AudioSource[] SEsources = new AudioSource[24];
	// 音声
	private AudioSource[] VoiceSources = new AudioSource[8];
	
	// === AudioClip ===
	// BGM
	public BGMDatas[] BGM;
    // SE
    public AudioClip[] SE;
	// 音声
	public AudioClip[] Voice;

    // SE、ボイス設定用AudioMixer
    [SerializeField]
    private AudioMixerGroup[] audioMixerGroups;  // 0 = Master。1 = BGM、2 = SE、3 = ボイス に設定することで、まとめて制御できる

    bool isXFading = false;

    int currentBgmIndex = 999;

    public float masterVolume;
    private readonly float linearToDecibelScalingFactor = 20.0f;  // 音量（linearVolume）をデシベル（dB）に変換するための係数


    [System.Serializable]
    public class BGMDatas {
        public AudioClip clip;
        public float loopTime;
        public float endTime;
    }


    /// <summary>
    /// ゲーム起動時の処理
    /// </summary>
    //public void EntryRun() {
    //    if (instance == null) {
    //        instance = this;
    //        DontDestroyOnLoad(this.gameObject);
    //        Init(ConstData.DEFAULT_MASTER_VOLUME);
    //    } else {
    //        Destroy(this.gameObject);
    //    }
    //    //Debug.Log("SoundManager Entry 終了");
    //}


    protected override void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Init(ConstData.DEFAULT_MASTER_VOLUME);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="newMasterVolume"></param>
    public void Init (float newMasterVolume) {
        //// BGM AudioSource  ->  SerializeField属性にてインスペクターで登録済
        //BGMsources[0] = gameObject.AddComponent<AudioSource>();
        //BGMsources[1] = gameObject.AddComponent<AudioSource>();

        // SE AudioSource
        for (int i = 0 ; i < SEsources.Length ; i++ ){
			SEsources[i] = gameObject.AddComponent<AudioSource>();
            SEsources[i].outputAudioMixerGroup = audioMixerGroups[2];
        }
		
		// 音声 AudioSource
		for(int i = 0 ; i < VoiceSources.Length ; i++ ){
			VoiceSources[i] = gameObject.AddComponent<AudioSource>();
            VoiceSources[i].outputAudioMixerGroup = audioMixerGroups[3];
        }
        // 初期音量設定
        SetMasterVolume(newMasterVolume);

        DOTween.Init();
        DebugLogger.Log("SoundManager Init");
    }

    void Update () {
        // ミュート設定
        //BGMsources[0].mute = volume.Mute;
        //BGMsources[1].mute = volume.Mute;
        //      foreach(AudioSource source in SEsources ){
        //	source.mute = volume.Mute;
        //}
        //foreach(AudioSource source in VoiceSources ){
        //	source.mute = volume.Mute;
        //}

        // ボリューム設定
        //      if(!isXFading) {
        //          //BGMsources[0].volume = volume.BGM;
        //          //BGMsources[1].volume = volume.BGM;
        //      }
        //      foreach(AudioSource source in SEsources ){
        //	source.volume = volume.SE;
        //}
        //foreach(AudioSource source in VoiceSources ){
        //	source.volume = volume.Voice;
        //}

        // Loop処理
        if (currentBgmIndex != 999) {
            if (BGM[currentBgmIndex].loopTime > 0f) {
                if (!BGMsources[0].mute && BGMsources[0].isPlaying && BGMsources[0].clip != null) {
                    if (BGMsources[0].time >= BGM[currentBgmIndex].endTime) {
                        BGMsources[0].time = BGM[currentBgmIndex].loopTime;
                    }
                }
                if (!BGMsources[1].mute && BGMsources[1].isPlaying && BGMsources[1].clip != null) {
                    if (BGMsources[1].time >= BGM[currentBgmIndex].endTime) {
                        BGMsources[1].time = BGM[currentBgmIndex].loopTime;
                    }
                }
            }
        }
    }

    //***** BGM再生 *****
    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="bgmType"></param>
    /// <param name="loopFlg"></param>
    public void PlayBGM(BGM_TYPE bgmType, bool loopFlg = true){
        int index = (int)bgmType;
        currentBgmIndex = index;
        //if(PlayerPrefs.GetInt(Constant.BGM_FLG_NAME,1) == 1){
        if( 0 > index || BGM.Length <= index ){
				return;
			}
        // 同じBGMの場合は何もしない
        if(BGMsources[0].clip != null && BGMsources[0].clip == BGM[index].clip) {
            return;
        } else if(BGMsources[1].clip != null && BGMsources[1].clip == BGM[index].clip) {
            return;
        }
        // フェードでBGM開始
        if(BGMsources[0].clip == null && BGMsources[1].clip == null) {
            BGMsources[0].loop = loopFlg;
            BGMsources[0].clip = BGM[index].clip;
            BGMsources[0].Play();
            //BGMsources[0].DOFade(gameData.volumeBgm, XFADE_TIME);
        } else {
            // クロスフェード
            StartCoroutine(CrossfadeChangeBMG(index, loopFlg));
        }
    }

    /// <summary>
    /// クロスフェード再生
    /// </summary>
    /// <param name="index"></param>
    /// <param name="loopFlg"></param>
    /// <returns></returns>
    private IEnumerator CrossfadeChangeBMG(int index, bool loopFlg) {
        isXFading = true;
        if(BGMsources[0].clip != null) {
            // 0がなっていて、1を新しい曲としてPlay
            BGMsources[1].volume = 0;
            BGMsources[1].clip = BGM[index].clip;
            BGMsources[1].loop = loopFlg;
            BGMsources[1].Play();
            BGMsources[0].DOFade(0, ConstData.XFADE_TIME).SetEase(Ease.Linear).SetLink(gameObject);
            BGMsources[1].DOFade(1, ConstData.XFADE_TIME).SetEase(Ease.Linear).SetLink(gameObject);
            yield return new WaitForSeconds(ConstData.XFADE_TIME);
            BGMsources[0].Stop();
            BGMsources[0].clip = null;
        } else {
            // 1がなっていて、0を新しい曲としてPlay
            BGMsources[0].volume = 0;
            BGMsources[0].clip = BGM[index].clip;
            BGMsources[0].loop = loopFlg;
            BGMsources[0].Play();
            BGMsources[1].DOFade(0, ConstData.XFADE_TIME).SetEase(Ease.Linear).SetLink(gameObject);
            BGMsources[0].DOFade(1, ConstData.XFADE_TIME).SetEase(Ease.Linear).SetLink(gameObject);
            yield return new WaitForSeconds(ConstData.XFADE_TIME);
            BGMsources[1].Stop();
            BGMsources[1].clip = null;
        }
        isXFading = false;
    }

    /// <summary>
    /// BGM停止
    /// </summary>
    public void StopBGM(){
        BGMsources[0].Stop();
        BGMsources[1].Stop();
        //BGMsources[0].clip = null;
        //BGMsources[1].clip = null;
    }

    // ***** SE再生 *****
    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="seType"></param>
    public void PlaySE(SE_TYPE seType) {
        int index = (int)seType;
        //if(PlayerPrefs.GetInt(Constant.SE_FLG_NAME,1) == 1){
        if (0 > index || SE.Length <= index) {
            return;
        }

        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in SEsources) {
            if (false == source.isPlaying) {
                source.clip = SE[index];
                //volume.SE = gameData.volumeSe;
                source.Play();
                return;
            }
        }
        //}
    }

    /// <summary>
    /// SE停止
    /// </summary>
    public void StopSE(){
		// 全てのSE用のAudioSouceを停止する
		foreach(AudioSource source in SEsources){
			source.Stop();
			source.clip = null;
		}  
	}

    /// <summary>
    /// 指定した AudioGroup の音量変更
    /// Slider の値(0 - 1.0f)を AudioMixer のデジベルに変換して適用
    /// </summary>
    /// <param name="mixerGroupName"></param>
    /// <param name="linearVolume"></param>
    public void SetLinearVolumeToMixerGroup(string mixerGroupName, float linerVolume) {
        //float decibel = linearToDecibelScalingFactor * Mathf.Log10(linerVolume);
        float decibel = ConvertVolume2dB(linerVolume);

        if (float.IsNegativeInfinity(decibel)) {
            decibel = -96f;  // 無音は -80f ではなくて -96f にする
        }

        audioMixerGroups[0].audioMixer.SetFloat(mixerGroupName, decibel);

        //if (Mathf.Approximately(_currentVolume, _volume))
        //    return;

        //// AudioMixer.SetFloat で Exposed Parameter を設定する
        //_audioMixer.SetFloat("BGMVolume", _volume);
        //_currentVolume = _volume;

    }

    /// <summary>
    /// 指定した AudioGroup の音量を float で取得
    /// </summary>
    /// <param name="mixerGroupName"></param>
    /// <returns></returns>
    public float GetLinearVolumeFromMixerGroup(string mixerGroupName) {
        float decibel;

        // Master
        audioMixerGroups[0].audioMixer.GetFloat(mixerGroupName, out decibel);

        return Mathf.Pow(10f, decibel / linearToDecibelScalingFactor);
    }


    float ConvertVolume2dB(float volume) => Mathf.Clamp(20f * Mathf.Log10(Mathf.Clamp(volume, 0f, 1f)), -80f, 0f);



    // 使えなくはないが、上の方がよい
    //public float ConvertVolumeToDb(float sliderValue) {
    //    return Mathf.Clamp(Mathf.Log10(Mathf.Clamp(sliderValue, 0f, 1f)) * 20f, -80f, 0f);
    //}

    //public void SetMasterVolume(float sliderValue) {
    //    audioMixerGroups[0].audioMixer.SetFloat("Master", ConvertVolumeToDb(sliderValue));
    //}

    /// <summary>
    /// AudioMixerのボリューム設定
    /// </summary>
    /// <param name="vol"></param>
    //public void SetAudioMixerVolume(int index, float vol) {
    //    if (vol == 0) {
    //        audioMixerGroups[index].audioMixer.SetFloat("volumeSE", -80);
    //    } else {
    //        audioMixerGroups[index].audioMixer.SetFloat("volumeSE", 0);
    //    }
    //}

    /// <summary>
    /// ボイス再生
    /// </summary>
    /// <param name="voiceNo"></param>
    public void PlayVoice(VOICE_TYPE voiceNo) {
        int index = (int)voiceNo;

        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in VoiceSources) {
            if (false == source.isPlaying) {
                source.clip = Voice[index];
                volume.Voice = 1.0f;
                source.Play();
                return;
            }
        }
    }

    /// <summary>
    /// すべてのボイス停止
    /// </summary>
    public void StopVoice() {
        // 全てのSE用のAudioSouceを停止する
        foreach (AudioSource source in VoiceSources) {
            source.Stop();
            source.clip = null;
        }
    }
    /// <summary>
    /// マスターボリュームの設定値更新
    /// </summary>
    /// <param name="newVolume"></param>
    public void SetMasterVolume(float newVolume) {
        masterVolume = newVolume;
    }

    // ***** 音声再生 *****
    // 音声再生
    //public void PlayVoice(ENUM_VOICE voiceNo){
    //	int index = (int)voiceNo;
    //	if(PlayerPrefs.GetInt(Constant.VOICE_FLG_NAME,1) == 1){
    //		if( 0 > index || Voice.Length <= index ){
    //			return;
    //		}
    //		// 再生中で無いAudioSouceで鳴らす
    //		foreach(AudioSource source in VoiceSources){
    //			if( false == source.isPlaying ){
    //				source.clip = Voice[index];
    //				volume.Voice = gameData.volumeVoice;
    //				source.Play();
    //				return;
    //			}
    //		}
    //       }
    //   }

    //   // 音声停止
    //   public void StopVoice(){
    //	// 全ての音声用のAudioSouceを停止する
    //	foreach(AudioSource source in VoiceSources){
    //		source.Stop();
    //		source.clip = null;
    //	}  
    //}
}