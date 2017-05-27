using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>  
///   *音乐管理器  
///   *2017/5/24  
/// </summary>  
public class AudioSoundManager : MonoBehaviour
{
    private static AudioSource _audioSource = null;
    private static AudioSoundManager _instance = null;
    public static AudioSoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AudioSoundManager");
                _instance = go.AddComponent<AudioSoundManager>();
                DontDestroyOnLoad(go);

                GameObject obj = new GameObject("SoundClip");
                _audioSource = obj.AddComponent<AudioSource>();
                DontDestroyOnLoad(obj);
            }

            return _instance;
        }
    }
    private Dictionary<eResType, AudioClip> AudioDictionary = new Dictionary<eResType, AudioClip>();

    private AudioSource BGMAudioSource;
    private const string mOnOverClipName = "/Audio/Menu/MenuLoad.aif";
    private const string mOnFilledClipName = "/Audio/Menu/MenuSelect.aif";

    /// <summary>  
    /// 播放  
    /// </summary>  
    /// <param name="audioname"></param>  
    public void PlaySound(eResType type, float volume = 1)
    {
        if (type == eResType.None)
        {
            return;
        }
        AudioClip sound = null;
        if (AudioDictionary.TryGetValue(type, out sound))
        {
            this.PlayClip(sound, volume);
        }
        else
        {
            StartCoroutine(LoadAudioClip(type, () =>
            {
                if (AudioDictionary.TryGetValue(type, out sound))
                {
                    this.PlayClip(sound, volume);
                }
            }));
        }
    }

    /// <summary>  
    /// 暂停所有音效音乐  
    /// </summary>  
    public void SoundAllPause()
    {
        AudioSource[] allsource = FindObjectsOfType<AudioSource>();
        if (allsource != null && allsource.Length > 0)
        {
            for (int i = 0; i < allsource.Length; i++)
            {
                allsource[i].Pause();
            }
        }
    }

    /// <summary>  
    /// 停止特定的音效  
    /// </summary>  
    /// <param name="audioname"></param>  
    public void SoundStop(string audioname)
    {
        GameObject obj = this.transform.FindChild(audioname).gameObject;
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    #region 音效资源路径  
    public enum eResType
    {
        None,
        OnOverClip,
        OnFilledClip
    }

    /// <summary>  
    /// 下载音效  
    /// </summary>  
    /// <param name="aduioname"></param>  
    /// <param name="type"></param>  
    /// <returns></returns>  
    private string GetAudioClipPath(eResType type)
    {
        string path = string.Empty;
        string streamingAssetsPath = string.Empty;
#if UNITY_EDITOR
        streamingAssetsPath = "file://" + Application.streamingAssetsPath;
#elif UNITY_IPHONE
        streamingAssetsPath = Application.dataPath +"/Raw";
#elif UNITY_ANDROID
        streamingAssetsPath = "jar:file://" + Application.dataPath + "!/assets";
#endif
        switch (type)
        {
            case eResType.OnOverClip:
                //path = System.IO.Path.Combine(streamingAssetsPath, mOnOverClipName);
                path = streamingAssetsPath + mOnOverClipName;
                break;
            case eResType.OnFilledClip:
                //path = System.IO.Path.Combine(streamingAssetsPath, mOnFilledClipName);
                path = streamingAssetsPath + mOnFilledClipName;
                break;
            default:
                break;
        }
        return path;
    }

    private IEnumerator LoadAudioClip(eResType type, System.Action callback)
    {
        string path = GetAudioClipPath(type);
        Debuger.Log("[AudioSoundManager] LoadAudioClip:" + path);
        if (System.IO.File.Exists(path))
        {
            Debuger.LogError("file is exist!!!!!");
        }
        if (!string.IsNullOrEmpty(path) && !AudioDictionary.ContainsKey(type))
        {
            WWW www = new WWW(path);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                AudioClip clip = www.GetAudioClipCompressed(true, AudioType.AIFF);
                AudioDictionary.Add(type, clip);

                if (callback != null)
                {
                    callback();
                }

                www.Dispose();
                www = null;
            }
            else
            {
                Debuger.LogError(www.error);
            }
        }
    }
    #endregion

    #region 背景音乐  
    /// <summary>  
    /// 设置音量  
    /// </summary>  
    public void BGMSetVolume(float volume)
    {
        if (this.BGMAudioSource != null)
        {
            this.BGMAudioSource.volume = volume;
        }
    }

    /// <summary>  
    /// 暂停背景音乐  
    /// </summary>  
    public void BGMPause()
    {
        if (this.BGMAudioSource != null)
        {
            this.BGMAudioSource.Pause();
        }
    }

    /// <summary>  
    /// 停止背景音乐  
    /// </summary>  
    public void BGMStop()
    {
        if (this.BGMAudioSource != null && this.BGMAudioSource.gameObject)
        {
            Destroy(this.BGMAudioSource.gameObject);
            this.BGMAudioSource = null;
        }
    }

    /// <summary>  
    /// 重新播放  
    /// </summary>  
    public void BGMReplay()
    {
        if (this.BGMAudioSource != null)
        {
            this.BGMAudioSource.Play();
        }
    }

    /// <summary>  
    /// 背景音乐控制器  
    /// </summary>  
    /// <param name="audioClip"></param>  
    /// <param name="volume"></param>  
    /// <param name="isloop"></param>  
    /// <param name="name"></param>  
    private void PlayBGMAudioClip(AudioClip audioClip, float volume = 1f, bool isloop = false, string name = null)
    {
        if (audioClip == null)
        {
            return;
        }
        else
        {
            GameObject obj = new GameObject(name);
            obj.transform.parent = this.transform;
            AudioSource LoopClip = obj.AddComponent<AudioSource>();
            LoopClip.clip = audioClip;
            LoopClip.volume = volume;
            LoopClip.loop = true;
            LoopClip.pitch = 1f;
            LoopClip.Play();
            this.BGMAudioSource = LoopClip;
        }
    }

    /// <summary>  
    /// 播放一次的背景音乐  
    /// </summary>  
    /// <param name="audioClip"></param>  
    /// <param name="volume"></param>  
    /// <param name="name"></param>  
    private void PlayOnceBGMAudioClip(AudioClip audioClip, float volume = 1f, string name = null)
    {
        PlayBGMAudioClip(audioClip, volume, false, name == null ? "BGMSound" : name);
    }

    /// <summary>  
    /// 循环播放的背景音乐  
    /// </summary>  
    /// <param name="audioClip"></param>  
    /// <param name="volume"></param>  
    /// <param name="name"></param>  
    private void PlayLoopBGMAudioClip(AudioClip audioClip, float volume = 1f, string name = null)
    {
        PlayBGMAudioClip(audioClip, volume, true, name == null ? "LoopSound" : name);
    }

    #endregion

    #region  音效  
    /// <summary>  
    /// 播放音效  
    /// </summary>  
    /// <param name="audioClip"></param>  
    /// <param name="volume"></param>  
    /// <param name="name"></param>  
    private void PlayClip(AudioClip audioClip, float volume = 1f)
    {
        if (audioClip == null)
        {
            return;
        }
        else
        {
            //GameObject obj = new GameObject("SoundClip");
            //AudioSource source = obj.AddComponent<AudioSource>();
            //StartCoroutine(this.PlayClipEndDestroy(audioClip, obj));
            _audioSource.pitch = 1f;
            _audioSource.volume = volume;
            _audioSource.clip = audioClip;
            _audioSource.Play();
        }
    }

    /// <summary>  
    /// 播放完音效删除物体  
    /// </summary>  
    /// <param name="audioclip"></param>  
    /// <param name="soundobj"></param>  
    /// <returns></returns>  
    private IEnumerator PlayClipEndDestroy(AudioClip audioclip, GameObject soundobj)
    {
        if (soundobj == null || audioclip == null)
        {
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(audioclip.length * Time.timeScale);
            Destroy(soundobj);
        }
    }

    /// <summary>  
    ///   
    /// </summary>  
    /// <returns></returns>  
    private IEnumerator PlayClipEnd(AudioClip audioclip, System.Action callback)
    {
        if (audioclip != null)
        {
            yield return new WaitForSeconds(audioclip.length * Time.timeScale);
            if (callback != null)
            {
                callback();
            }
        }
        yield break;
    }
    #endregion
}