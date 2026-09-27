using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // 引用
    [Header("References")]
    public Transform trans;             // 逻辑物理根节点
    public Transform modelTrans;        // 视觉表现子节点
    public CharacterController characterController;

    // 移动
    [Header("Movement")]
    [Tooltip("每秒移动的单位数，最高速度。")]
    public float movespeed = 24;

    [Tooltip("达到最高速度所需的时间，以秒为单位。")]
    public float timeToMaxSpeed = .26f;

    private float VeloctiyGainPerSecond { get { return movespeed / timeToMaxSpeed; } }

    [Tooltip("从最高速度到静止所需的时间，以秒未单位。")]
    public float timeToLoseMaxSpeed = .2f;

    private float VerlocityLossPerSpeed { get { return movespeed / timeToLoseMaxSpeed; } }

    [Tooltip("尝试向与当前行进方向相反的方向移动时的动量乘数（如向左移动时试图向右移动）")]
    public float reverseMomentumMultiplier = 2.2f;

    private Vector3 movementVelocity = Vector3.zero;

    // 死亡与重生
    [Header("Death and Respawning")]
    [Tooltip("玩家死亡后等待多久才会重生")]
    public float respawnWaitTime = 2f;

    private bool dead = false;

    private Vector3 spawnPoint;

    private Quaternion spawnRotation;

    // 冲刺
    [Header("Dash")]
    [Tooltip("执行冲刺时移动的总距离")]
    public float dashDistance = 17;
    [Tooltip("执行冲刺所需的时间，以秒为单位")]
    public float dashTime = .15f;

    [Tooltip("冲刺完成后到再次执行冲刺之前的时间间隔，以秒为单位")]
    public float dashCooldown = 1.8f;

    private bool CanDashNow
    {
        get
        {
            return Time.time > dashBeginTime + dashTime + dashCooldown;
        }
    }

    private bool IsDashing
    {
        get
        {
            return Time.time < dashBeginTime + dashTime;
        }
    }
    private Vector3 dashDirection;
    private float dashBeginTime = Mathf.NegativeInfinity;

    public GameObject cam;

    private bool paused = false;


    private void Start()
    {
        spawnPoint = trans.position;
        spawnRotation = modelTrans.rotation;
    }

    private void Update()
    {
        if (!paused)
        {
            Movement();
            Dashing();
            if (Input.GetKeyDown(KeyCode.T))
            {
                Die();
            }
        }
        Pausing();
    }

    private void OnGUI()
    {
        if (paused)
        {
            float boxWidth = Screen.width * .4f;
            float boxHeight = Screen.height * .4f;

            GUILayout.BeginArea(new Rect(
                (Screen.width * .5f) - (boxWidth * .5f),
                (Screen.height * .5f) - (boxHeight * .5f),
                boxWidth,
                boxHeight));

            if(GUILayout.Button("RESUME GAME", GUILayout.Height(boxHeight * .5f)))
            {
                paused = false;
                Time.timeScale = 1;
            }

            if(GUILayout.Button("RETURN TO MAIN MENU", GUILayout.Height(boxHeight * .5f)))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(0);
            }

            GUILayout.EndArea();
        }
    }


    public void Respawn()
    {
        dead = false;
        trans.position = spawnPoint;
        enabled = true;
        characterController.enabled = true;
        modelTrans.gameObject.SetActive(true);
        modelTrans.rotation = spawnRotation;
    }

    private void Pausing()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 切换暂停状态
            paused = !paused;
            // 如果是暂停状态，将timeScale设置为0
            if (paused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    public void Die()
    {
        if (!dead)
        {
            dead = true;
            Invoke("Respawn", respawnWaitTime);
            movementVelocity = Vector3.zero;
            enabled = false;           // 每个组件类型都有的实例变量
            characterController.enabled = false;
            modelTrans.gameObject.SetActive(false);

            dashBeginTime = Mathf.NegativeInfinity;
        }
    }

    private void Movement()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (movementVelocity.z >= 0)
            {
                movementVelocity.z = Mathf.Min(movespeed, movementVelocity.z + VeloctiyGainPerSecond * Time.deltaTime);
            }
            else
            {
                movementVelocity.z = Mathf.Min(0, movementVelocity.z + VeloctiyGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
            }
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (movementVelocity.z > 0)
            {
                movementVelocity.z = Mathf.Max(0, movementVelocity.z - VeloctiyGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
            }
            else
            {
                movementVelocity.z = Mathf.Max(-movespeed, movementVelocity.z - VeloctiyGainPerSecond * Time.deltaTime);
            }
        }
        else
        {
            if (movementVelocity.z > 0)
            {
                movementVelocity.z = Mathf.Max(0, movementVelocity.z - VerlocityLossPerSpeed * Time.deltaTime);
            }
            else if (movementVelocity.z < 0)
            {
                movementVelocity.z = Mathf.Max(0, movementVelocity.z + VerlocityLossPerSpeed * Time.deltaTime);
            }
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (movementVelocity.x >= 0)
            {
                movementVelocity.x = Mathf.Min(movespeed, movementVelocity.x + VeloctiyGainPerSecond * Time.deltaTime);
            }
            else
            {
                movementVelocity.x = Mathf.Min(0, movementVelocity.x + VeloctiyGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
            }
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (movementVelocity.x > 0)
            {
                movementVelocity.x = Mathf.Max(0, movementVelocity.x - VeloctiyGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
            }
            else
            {
                movementVelocity.x = Mathf.Max(-movespeed, movementVelocity.x - VeloctiyGainPerSecond * Time.deltaTime);
            }
        }
        else
        {
            if (movementVelocity.x > 0)
            {
                movementVelocity.x = Mathf.Max(0, movementVelocity.x - VerlocityLossPerSpeed * Time.deltaTime);
            }
            else if (movementVelocity.x < 0)
            {
                movementVelocity.x = Mathf.Max(0, movementVelocity.x + VerlocityLossPerSpeed * Time.deltaTime);
            }
        }


    }

    private void Dashing()
    {
        // 当前没有处于冲刺状态
        if (!IsDashing)
        {
            // 如果玩家在任一方向上移动
            if (movementVelocity.x != 0 || movementVelocity.z != 0)
            {
                // 应用移动速度
                characterController.Move(movementVelocity * Time.deltaTime);
                // 使用Model Holder面朝最后移动的方向
                modelTrans.rotation = Quaternion.Slerp(modelTrans.rotation, Quaternion.LookRotation(movementVelocity), .18f);
            }
            // 冲刺不在冷却中，并且按下了shift
            if (CanDashNow && Input.GetKey(KeyCode.LeftShift))
            {
                // 找出玩家按下的移动键
                Vector3 movementDir = Vector3.zero;

                // 如果按住W或者上箭头键，将z设置为1；
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    movementDir.z = 1;
                }
                // 如果按住S或者下箭头键，将z设置为-1
                else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    movementDir.z = -1;
                }
                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    movementDir.x = -1;
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    movementDir.x = 1;
                }

                // 如果有按键被按下，则需要执行冲刺
                if (movementDir.z != 0 || movementDir.x != 0)
                {
                    dashDirection = movementDir;
                    dashBeginTime = Time.time;
                    movementVelocity = dashDirection * movespeed;
                    modelTrans.forward = dashDirection;
                }
            }
        }
        else    // 当前已经处于冲刺状态
        {
            characterController.Move(dashDirection.normalized * (dashDistance / dashTime) * Time.deltaTime);
        }
    }
}
