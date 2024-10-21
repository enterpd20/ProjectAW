using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSFXController : MonoBehaviour
{
    [SerializeField] private int sfxIndex;    // 버튼에 할당할 SFX의 인덱스
    float volume = 1f;

    private void Start()
    {
        // 버튼 클릭 시 SFX 재생
        GetComponent<Button>().onClick.AddListener(PlayButtonSFX);
    }

    // 버튼 클릭 시 호출되는 함수
    private void PlayButtonSFX()
    {
        // SoundManager의 PlaySFX 호출하여 해당 SFX 재생
        SoundManager.Instance.PlaySFX(sfxIndex, volume);
    }
}
