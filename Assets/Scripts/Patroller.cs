using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class Patroller : MonoBehaviour
{
    private const float rotationSlerpAmount = .68f;
    [Header("references")]
    public Transform trans;
    public Transform modelHolder;

    [Header("Stats")]
    public float movespeed = 30;

    private int currentPointIndex;      // 索引当前目标巡逻点在patrolPoint数组中的位置
    private Transform currentPoint;     // 指向当前巡逻点的Transform的引用


    // 巡逻点
    private Transform[] patrolPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitialPatrolPoints();
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        // 仅在有currentPoint的时候操作
        if (currentPoint != null)
        {
            // 将根游戏对象移动到currentPoint的位置
            trans.position = Vector3.MoveTowards(trans.position, currentPoint.position, movespeed * Time.deltaTime);

            // 如果已经到currentPoint的位置，则更换currentPoint
            if (trans.position == currentPoint.position)
            {
                SetCurrentPatrolPoint((currentPointIndex + 1) % patrolPoints.Length);
            }
            else // 如果没有到达，则将模型转至面向档期那currentPoint的方向
            {
                // 计算lookRotation
                Quaternion lookRotation = Quaternion.LookRotation((currentPoint.position - trans.position).normalized);

                // 平滑过度到lookRotation
                modelHolder.rotation = Quaternion.Slerp(modelHolder.rotation, lookRotation, rotationSlerpAmount);
            }
        }
    }


    private void SetCurrentPatrolPoint(int index)
    {
        currentPointIndex = index;
        currentPoint = patrolPoints[index];
    }

    private void InitialPatrolPoints()
    {
        // 获取未排序的巡逻点列表
        List<Transform> points = GetUnsortedPatrolPoints();

        // 只有存在知道一个巡逻点时才继续
        if (points.Count > 0)
        {
            // 准备patrolPoints数组
            patrolPoints = new Transform[points.Count];

            // 遍历所有巡逻点
            foreach (Transform point in points)
            {
                // 从名称中提取出巡逻点编号（拿到括号中的数字，不管是几位的）
                string indexSubstring = point.gameObject.name[14..^1];  // 等价于SubString(14,point.gameObject.name.Length - 15)

                // 在patrolPoints数组中设置引用
                patrolPoints[int.Parse(indexSubstring)] = point;

                // 解除巡逻点和Patroller的父子关系，以防它们随着Patroller移动
                point.SetParent(null);

                // 在“层级”窗口中隐藏巡逻点
                point.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            // 从数组中的第一个点开始巡逻
            SetCurrentPatrolPoint(0);
        }

    }

    private List<Transform> GetUnsortedPatrolPoints()
    {
        // 获取Patroller的每个子对象的Transform
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();

        // 声明一个用于存储Tansform的局部列表
        var points = new List<Transform>();

        foreach (Transform trans in children)
        {
            // 检查子对象的名称是否以“Patrol Point (”开头\
            if (trans.gameObject.name.StartsWith("Patrol Point ("))
            {
                // 是就添加到points中
                points.Add(trans);
            }
        }

        return points;
    }
}
