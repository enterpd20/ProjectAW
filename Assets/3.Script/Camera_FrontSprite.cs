using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_FrontSprite : MonoBehaviour
{
    private string targetTag = "Player";        // �ش� �±׸� ���� ��� ������Ʈ�� �������

    private void LateUpdate()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);    // �ش� �±��� ��� ������Ʈ �迭��

        foreach(GameObject target in targets)       // �迭�� ��� ������Ʈ����
        {
            target.transform.forward = transform.forward;       // ī�޶� ���ϵ���
        }
    }
}
