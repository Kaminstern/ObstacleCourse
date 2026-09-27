using Assets.Scripts;
using UnityEngine;


/// <summary>
/// 定义一个矩形区域并提供获取区域内随机点的方法，Wanderer游戏对象将在这个区域内活动
/// 所有wanderer对象都是其子对象，在start方法中找到这些子对象，向他们提供一个对自动WanderRegion的引用
/// Wanderer通过这个引用来获取新的移动目标点
/// </summary>
public class WanderRegion : MonoBehaviour
{
    [Header("References")]
    public Transform region;

    public Vector3 GetRandomPointWithin()
    {
        float x = transform.position.x + Random.Range(region.localScale.x * -.5f, region.localScale.x * .5f);
        float z = transform.position.z + Random.Range(region.localScale.z * -.5f, region.localScale.z * .5f);
        return new Vector3(x, 0, z);
    }

    private void Awake()
    {
        // 获取所有子对象中的Wanderer脚本
        var wanderers = gameObject.GetComponentsInChildren<Wanderer>();
        
        foreach (var wanderer in wanderers)
        {
            wanderer.region = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
