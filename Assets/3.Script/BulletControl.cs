using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public float damage;

    public void InitializeBullet(Gear gear)
    {
        damage = gear.stats.DMG;
        // 다른 속성도 필요하면 초기화하기
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 적에게 데미지를 주는 로직 작성
    }
}
