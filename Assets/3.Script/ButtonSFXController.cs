using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSFXController : MonoBehaviour
{
    [SerializeField] private int sfxIndex;    // ��ư�� �Ҵ��� SFX�� �ε���
    float volume = 1f;

    private void Start()
    {
        // ��ư Ŭ�� �� SFX ���
        GetComponent<Button>().onClick.AddListener(PlayButtonSFX);
    }

    // ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    private void PlayButtonSFX()
    {
        // SoundManager�� PlaySFX ȣ���Ͽ� �ش� SFX ���
        SoundManager.Instance.PlaySFX(sfxIndex, volume);
    }
}
