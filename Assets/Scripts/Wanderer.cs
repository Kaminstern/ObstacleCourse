using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Wanderer : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Rotating,
            Moving
        }
        private State state = State.Idle;

        [HideInInspector]
        public WanderRegion region;

        [Header("References")]
        public Transform trans;
        public Transform modelTrans;

        [Header("Stats")]
        public float movespeed = 18;

        [Tooltip("重定向前需要等待的最短时间。")]
        public float minRetargetInterval = 4.4f;
        [Tooltip("重定向前需要等待的最长时间。")]
        public float maxRetargetInterval = 6.2f;
        [Tooltip("在确定目标点后，开始移动前进行旋转所需要的时间。")]
        public float rotationTime = .6f;
        [Tooltip("旋转完成后，开始移动前需要等待的时间。")]
        public float postRotationWaitTime = .3f;

        private Vector3 currentTarget;  // 当前目标位置
        private Quaternion initialRotation; // 初始旋转
        private Quaternion targetRotation; // 目标旋转
        private float rotationStartTime;   // 开始旋转时间

        // Use this for initialization
        void Start()
        {
            Retarget();
        }

        // Update is called once per frame
        void Update()
        {
            Move();
        }

        void Retarget()
        {
            // 在区域内选择一个随机点作为当前目标点
            currentTarget = region.GetRandomPointWithin();
            // 标记初始旋转
            initialRotation = modelTrans.rotation;
            // 标记面向目标点所需要的旋转
            targetRotation = Quaternion.LookRotation((currentTarget - trans.position).normalized);
            // 开始旋转
            state = State.Rotating;
            rotationStartTime = Time.time;

            //在旋转结束后，等待postRotationWaitTime秒后开始移动
            Invoke("BeginMoving", rotationTime + postRotationWaitTime);
        }

        void BeginMoving()
        {
            // 确保模型的旋转与targetRotation一致
            modelTrans.rotation = targetRotation;

            // 将状态设置为Moving
            state = State.Moving;
        }

        private void Move()
        {
            if (state == State.Moving)
            {
                // 按deltaTime向目标点移动
                trans.position = Vector3.MoveTowards(trans.position, currentTarget, movespeed * Time.deltaTime);

                // 到达目标点后变为待机状态，然后调用下一个Retarget
                if (trans.position == currentTarget)
                {
                    state = State.Idle;
                    Invoke("Retarget", Random.Range(minRetargetInterval, maxRetargetInterval));
                }
            }
            else if (state == State.Rotating)
            {
                // 计算从旋转开始到现在所经过的时间，以秒为单位
                float timeSpentRotating = Time.time - rotationStartTime;

                //从initialRotation旋转到targetRotation
                modelTrans.rotation = Quaternion.Slerp(initialRotation, targetRotation, timeSpentRotating / rotationTime);
            }
        }
    }
}