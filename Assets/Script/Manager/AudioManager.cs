using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("오디오 소스")]
    public AudioSource sfxSource;
    public AudioSource bgmSource;

    [Header("사운드 목록")]
    public List<AudioClip> sfxClips;
    public List<AudioClip> bgmClips;

    private Dictionary<string, AudioClip> sfxDict;
    private Dictionary<string, AudioClip> bgmDict;

    void Awake()
    {
        // Singleton 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeAudioDictionaries()
    {
        sfxDict = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in sfxClips)
        {
            sfxDict[clip.name] = clip;
        }

        bgmDict = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in bgmClips)
        {
            bgmDict[clip.name] = clip;
        }
    }

    // SFX 재생
    public void PlaySFX(string clipName)
    {
        if (sfxDict.ContainsKey(clipName))
        {
            sfxSource.PlayOneShot(sfxDict[clipName]);
        }
        else
        {
            Debug.LogWarning($"SFX '{clipName}' not found!");
        }
    }

    // BGM 재생
    public void PlayBGM(string clipName, bool loop = true)
    {
        if (bgmDict.ContainsKey(clipName))
        {
            bgmSource.clip = bgmDict[clipName];
            bgmSource.loop = loop;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM '{clipName}' not found!");
        }
    }

    // BGM 정지
    public void StopBGM()
    {
        bgmSource.Stop();
    }
}
