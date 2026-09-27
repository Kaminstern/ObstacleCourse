using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("References")]
    public Transform spawnPoint;
    public GameObject projectilePrefab;

    [Header("Stats")]
    [Tooltip("投射物的发射间隔，单位为秒")]
    public float fireRate = 1;

    private float lastFireTime = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 如果当前游戏时间大于上次发射时间和速度
        if (Time.time >= lastFireTime + fireRate)
        {
            // 生成一个投射物并记录当前时间
            lastFireTime = Time.time; ;
            Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
