using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance; // �̱��� ������ ���� static �ν��Ͻ�

    [SerializeField] private AudioSource bgmAudioSource;   // ��� ������ ����� AudioSource
    [SerializeField] private AudioClip[] bgmClips;    // �� ���� �´� ��� ���� Ŭ����

    [SerializeField] private AudioSource sfxAudioSource;   // ��� ������ ����� AudioSource
    [SerializeField] private AudioClip[] sfxClips;      // ��ư Ŭ�� �� ����� SFX �迭

    [SerializeField] private AudioMixer audioMixer;

    private string currentScene; // ���� �� �̸��� ����

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� ����Ǿ �ı����� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� ���ο� �ν��Ͻ��� �ı�
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �̺�Ʈ ����
        PlayBGM(SceneManager.GetActiveScene().name); // ù ���� ���� ���
    }

    // ���� �ε�� ������ ȣ��
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGM(scene.name);
    }

    // ���� �´� ���� ���
    private void PlayBGM(string sceneName)
    {
        AudioClip clipToPlay = null;

        // �� ���� �´� ��� ���� �Ҵ�
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
                // 04_Battle �������� 3���� ������ �����ϰ� ����
                int randomIndex = Random.Range(3, 6); // 04_Battle01, 04_Battle02, 04_Battle03
                clipToPlay = bgmClips[randomIndex];
                break;
            default:
                clipToPlay = bgmClips[0]; // �⺻ ����
                break;
        }

        // ���� ��� ���� ���ǰ� �ٸ� ��쿡�� ��ü
        if (clipToPlay != null && clipToPlay != bgmAudioSource.clip)
        {
            bgmAudioSource.clip = clipToPlay;
            bgmAudioSource.Play();
        }
    }

    // ��ư�� �´� SFX�� ���
    public void PlaySFX(int sfxIndex, float volume)
    {
        if (sfxIndex >= 0 && sfxIndex < sfxClips.Length)
        {
            sfxAudioSource.PlayOneShot(sfxClips[sfxIndex], volume);
        }
    }

    // ���� ����
    public void SetVolume(string parameterName, float value)
    {
        if (value == 0)
        {
            audioMixer.SetFloat(parameterName, -80f);  // ������ �ּҷ� (���Ұ�)
        }
        else
        {
            audioMixer.SetFloat(parameterName, Mathf.Log10(value) * 20);  // �α� �����Ϸ� ���� ����
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
