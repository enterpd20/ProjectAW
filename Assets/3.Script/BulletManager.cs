using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance { get; private set; } // 인스턴스 저장 및 접근 모두 이 프로퍼티에서 처리

    // BulletType에 해당하는 프리팹 배열 (인스펙터에서 연결)
    public GameObject[] bulletPrefabs;

    private List<GameObject> bulletPool = new List<GameObject>();

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple BulletManager instances found! Destroying duplicate.");
            Destroy(gameObject); // 중복된 인스턴스가 있을 경우 파괴
        }
    }

    public GameObject GetPooledBullet(Gear gear, TeamManager ownerTeamManager, Transform targetTransform, Vector3 minBounds, Vector3 maxBounds)
    {
        BulletController.BulletType bulletType = BulletController.DetermineBulletType(gear);

        foreach (var bullet in bulletPool)
        {
            BulletController bulletController = bullet.GetComponent<BulletController>();
            if (!bullet.activeInHierarchy && bulletController != null && bulletController.bulletType == bulletType)
            {
                bulletController.InitializeBullet(gear, ownerTeamManager, targetTransform, minBounds, maxBounds); // 초기화
                return bullet;
            }
        }

        // 비활성화된 포탄이 없으면 새로운 포탄을 생성
        return CreateNewBullet(bulletType, gear, ownerTeamManager, targetTransform, minBounds, maxBounds);
    }

    // 새로운 포탄을 생성하는 함수
    private GameObject CreateNewBullet(BulletController.BulletType bulletType, Gear gear, TeamManager ownerTeamManager, Transform targetTransform, Vector3 minBounds, Vector3 maxBounds)
    {
        int bulletIndex = (int)bulletType;

        // bulletPrefabs 배열 내에 해당 타입의 포탄이 있는지 확인
        if (bulletIndex >= 0 && bulletIndex < bulletPrefabs.Length)
        {
            // 새로운 포탄 생성 및 초기화
            GameObject newBullet = Instantiate(bulletPrefabs[bulletIndex]);
            BulletController bulletController = newBullet.GetComponent<BulletController>();
            bulletController.bulletType = bulletType;
            bulletController.InitializeBullet(gear, ownerTeamManager, targetTransform, minBounds, maxBounds); // 초기화

            // 풀에 새로 생성한 포탄을 추가
            bulletPool.Add(newBullet);
            return newBullet;
        }

        Debug.LogError($"No bullet prefab found for type: {bulletType}");
        return null;
    }
}
