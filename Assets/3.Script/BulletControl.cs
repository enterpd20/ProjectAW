using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public float damage;

    public void InitializeBullet(Gear gear)
    {
        damage = gear.stats.DMG;
        // �ٸ� �Ӽ��� �ʿ��ϸ� �ʱ�ȭ�ϱ�
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ������ �������� �ִ� ���� �ۼ�
    }
}
