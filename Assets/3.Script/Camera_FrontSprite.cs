using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_FrontSprite : MonoBehaviour
{
    private string targetTag = "Player";        // 해당 태그를 가진 모든 오브젝트들 대상지정

    private void LateUpdate()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);    // 해당 태그의 모든 오브젝트 배열로

        foreach(GameObject target in targets)       // 배열에 담긴 오브젝트들이
        {
            target.transform.forward = transform.forward;       // 카메라를 향하도록
        }
    }
}
