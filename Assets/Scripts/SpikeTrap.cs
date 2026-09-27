using System.Threading;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    // Spike完全升起和降下的具体高度
    private const float SpikeHeight = 4f;
    private const float LoweredSpikeHeight = .3f;

    private enum State
    {
        Lowered,        // 尖刺完全降下
        Lowering,       // 尖刺正在缩短
        Raising,        // 尖刺正在伸长
        Raised          // 尖刺完全升起
    }
    private State state = State.Lowered;

    [Header("References")]
    [Tooltip("对所有Spike的父对象的引用")]
    public Transform spikeHolder;
    public GameObject hitboxGameObject;
    public GameObject colliderGameObject;

    [Header("Stats")]
    [Tooltip("尖刺完全降下到再次升起之前的等待时间，以秒为单位")]
    public float interval = 2f;

    [Tooltip("尖刺完全升起到开始下降之前的等待时间，以秒为单位")]
    public float raiseWaitTime = .3f;

    [Tooltip("尖刺完全降下所需的时间，以秒为单位")]
    public float lowerTime = .6f;

    [Tooltip("尖刺完全升起所需的时间，以秒为单位")]
    public float raiseTime = .08f;

    private float lastSwitchTime = Mathf.NegativeInfinity;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 默认情况下，尖刺处于完全降下的状态
        // 在游戏开始后，经过interval秒开始升起尖刺
        Invoke("StartRaising", interval);
    }

    // Update is called once per frame
    void Update()
    {
        upAndDown();
    }

    private void StartRaising()
    {
        lastSwitchTime = Time.time;
        state = State.Raising;

        // 通过激活hitbox来使尖刺变得致命
        hitboxGameObject.SetActive(true);
    }

    private void StartLowering()
    {
        lastSwitchTime = Time.time;
        state = State.Lowering;
    }

    private void upAndDown()
    {
        if(state == State.Lowering)
        {
            // 获取Spike的父对象的局部缩放
            Vector3 scale = spikeHolder.localScale;
            // 通过Lerp方法计算Y轴缩放，从最大高度过渡到最小高度
            scale.y = Mathf.Lerp(SpikeHeight, LoweredSpikeHeight, (Time.time - lastSwitchTime) / lowerTime);

            // 将更新后的缩放应用到Spike的父对象上
            spikeHolder.localScale = scale;

            // 如果尖刺已经完全落下
            if(scale.y == LoweredSpikeHeight)
            {
                // 更新状态，并在interval秒后调用StartRaising方法
                Invoke("StartRaising", interval);
                state = State.Lowered;
                colliderGameObject.SetActive(false);
            }
        }
        else if(state == State.Raising)
        {
            // 获取Spike的父对象的局部缩放
            Vector3 scale = spikeHolder.localScale;
            // 通过Lerp方法计算Y轴缩放，从最小高度过渡到最大高度
            scale.y = Mathf.Lerp(LoweredSpikeHeight, SpikeHeight, (Time.time - lastSwitchTime) / raiseTime);

            // 将更新后的缩放应用到Spike的父对象上
            spikeHolder.localScale = scale;

            // 如果尖刺已经完全升起
            if (scale.y == SpikeHeight)
            {
                // 更新状态，并在raiseWaitTime秒后调用StartLowering方法
                Invoke("StartLowering", raiseWaitTime);
                state = State.Raised;

                // 激活collider以阻挡玩家
                colliderGameObject.SetActive(true);
                // 停用hitbox以避免击杀玩家
                hitboxGameObject.SetActive(false);
            }
        }
    }
}
