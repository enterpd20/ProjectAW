using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_TEST : MonoBehaviour
{
    public Transform Enemy;

    private void Awake()
    {
        Enemy = GameObject.Find("Kasumi_META").transform;
    }
}
