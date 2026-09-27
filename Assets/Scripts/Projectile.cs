using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    public Transform trans;

    [Header("Stats")]
    [Tooltip("投射物每秒将向前移动多少个单位")]
    public float speed = 34;

    [Tooltip("投射物的最大攻击范围")]
    public float range = 70;


    private Vector3 spawnPoint; // 记录投射物生成时所处位置

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint = trans.position;
    }

    // Update is called once per frame
    void Update()
    {
        // 沿着局部z轴（向前）移动投射物
        trans.Translate(0, 0, speed * Time.deltaTime, Space.Self);

        // 如果投射物已经达到或超过最大攻击范围，则销毁
        if(Vector3.Distance(trans.position, spawnPoint) >= range)
        {
            Destroy(gameObject);
        }
    }
}
