using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance; // 싱글톤 패턴을 위한 static 인스턴스

    [SerializeField] private AudioSource bgmAudioSource;   // 배경 음악을 재생할 AudioSource
    [SerializeField] private AudioClip[] bgmClips;    // 각 씬에 맞는 배경 음악 클립들

    [SerializeField] private AudioSource sfxAudioSource;   // 배경 음악을 재생할 AudioSource
    [SerializeField] private AudioClip[] sfxClips;      // 버튼 클릭 시 사용할 SFX 배열

    [SerializeField] private AudioMixer audioMixer;

    private string currentScene; // 현재 씬 이름을 저장

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 새로운 인스턴스를 파괴
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 구독
        PlayBGM(SceneManager.GetActiveScene().name); // 첫 씬의 음악 재생
    }

    // 씬이 로드될 때마다 호출
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGM(scene.name);
    }

    // 씬에 맞는 음악 재생
    private void PlayBGM(string sceneName)
    {
        AudioClip clipToPlay = null;

        // 각 씬에 맞는 배경 음악 할당
        switch (sceneName)
        {
            case "00_Loading":
                clipToPlay = bgmClips[2]; // 03_SelectBattle
                break;
            case "00_Title":
                clipToPlay = bgmClips[0]; // 00_Title
                break;
            case "01_Lobby":
                clipToPlay = bgmClips[1]; // 01_Lobby
                break;
            case "02_Dock":
                clipToPlay = bgmClips[1]; // 01_Lobby
                break;
            case "02_DockDetail":
                clipToPlay = bgmClips[1]; // 01_Lobby
                break;
            case "02_Depot":
                clipToPlay = bgmClips[1]; // 01_Lobby
                break;
            case "02_Build":
                clipToPlay = bgmClips[1]; // 01_Lobby
                break;
            case "02_Lab":
                clipToPlay = bgmClips[1]; // 01_Lobby
                break;
            case "03_SelectBattle":
                clipToPlay = bgmClips[2]; // 03_SelectBattle
                break;
            case "03_SelectStage":
                clipToPlay = bgmClips[2]; // 03_SelectBattle
                break;
            case "04_Battle":
                // 04_Battle 씬에서는 3개의 음악을 랜덤하게 선택
                int randomIndex = Random.Range(3, 6); // 04_Battle01, 04_Battle02, 04_Battle03
                clipToPlay = bgmClips[randomIndex];
                break;
            default:
                clipToPlay = bgmClips[0]; // 기본 음악
                break;
        }

        // 현재 재생 중인 음악과 다른 경우에만 교체
        if (clipToPlay != null && clipToPlay != bgmAudioSource.clip)
        {
            bgmAudioSource.clip = clipToPlay;
            bgmAudioSource.Play();
        }
    }

    // 버튼에 맞는 SFX를 재생
    public void PlaySFX(int sfxIndex, float volume)
    {
        if (sfxIndex >= 0 && sfxIndex < sfxClips.Length)
        {
            sfxAudioSource.PlayOneShot(sfxClips[sfxIndex], volume);
        }
    }

    // 볼륨 조절
    public void SetVolume(string parameterName, float value)
    {
        if (value == 0)
        {
            audioMixer.SetFloat(parameterName, -80f);  // 볼륨을 최소로 (음소거)
        }
        else
        {
            audioMixer.SetFloat(parameterName, Mathf.Log10(value) * 20);  // 로그 스케일로 볼륨 조정
        }
    }

    public void OnMasterVolumeChange(float value)
    {
        SetVolume("MasterVolume", value);
    }

    public void OnBGMVolumeChange(float value)
    {
        SetVolume("BGMVolume", value);
    }

    public void OnSFXVolumeChange(float value)
    {
        SetVolume("SFXVolume", value);
    }
}
