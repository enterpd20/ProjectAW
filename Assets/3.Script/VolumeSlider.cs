using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public RectTransform volumeSettingPanel;

    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // 저장된 볼륨 값을 불러와 슬라이더에 적용
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // 슬라이더 값이 변경되면 SoundManager의 볼륨 조정 메서드 호출
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    public void OnMasterVolumeChanged(float value)
    {
        SoundManager.Instance.SetVolume("MasterVolume", value);
    }

    public void OnBGMVolumeChanged(float value)
    {
        SoundManager.Instance.SetVolume("BGMVolume", value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance.SetVolume("SFXVolume", value);
    }

    public void SaveVolumeSettings()
    {
        // 슬라이더 값을 PlayerPrefs에 저장
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("BGMVolume", bgmSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save(); // PlayerPrefs 저장
        CloseVolumeSettingPanel();
    }

    public void OpenVolumeSettingPanel()
    {
        volumeSettingPanel.localScale = Vector3.one;
    }

    public void CloseVolumeSettingPanel()
    {
        volumeSettingPanel.localScale = Vector3.zero;
    }
}
