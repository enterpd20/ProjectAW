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
        // ����� ���� ���� �ҷ��� �����̴��� ����
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // �����̴� ���� ����Ǹ� SoundManager�� ���� ���� �޼��� ȣ��
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
        // �����̴� ���� PlayerPrefs�� ����
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("BGMVolume", bgmSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save(); // PlayerPrefs ����
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
