using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{

    // 当前场景的构建索引
    private int currentScene = 0;

    // 当前场景的Level View Camera
    private GameObject levelViewCamera;

    // 当前正在进行的场景加载操作
    private AsyncOperation currentLoadOperation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        IsLoading();
    }

    private void IsLoading()
    {
        // 如果当前加载操作已经完成
        if(currentLoadOperation != null && currentLoadOperation.isDone)
        {
            // 将currentLoadOperation设置为null
            currentLoadOperation = null;

            // 在场景中查找Level View Camera
            levelViewCamera = GameObject.Find("Level View Camera");

            // 如果未找到Level View Camera，则记录错误信息
            if(levelViewCamera == null)
            {
                Debug.LogError("No level view camera was found in the scene!");
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Label("OBSTACLE COURSE");
        // 如果当前场景不是main场景
        if(currentScene != 0)
        {
            GUILayout.Label($"Currently viewing Level {currentScene}");
            // 显示一个play按钮
            if (GUILayout.Button("PLAY"))
            {
                // 如果按钮被点击，则开始运行当前关卡
                PlayCurrentLevel();
            }
        }
        else
        {
            GUILayout.Label("Select a level to preview it.");
        }

        // 从构建索引为1的场景开始，循环便利剩余场景索引
        for(int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            // 显示一个带有Level [Level number] 文本的按钮
            if(GUILayout.Button($"Level {i}"))
            {
                // 如果点击按钮，并且目前没有等待场景加载
                if(currentLoadOperation == null)
                {
                    // 异步加载关卡
                    currentLoadOperation = SceneManager.LoadSceneAsync(i);
                    // 设置当前场景
                    currentScene = i;
                }
            }
        }
    }

    private void PlayCurrentLevel()
    {
        // 停用Level View Camera
        levelViewCamera.SetActive(false);

        // 尝试找到Player游戏对象
        var playerGobj = GameObject.Find("Player");

        // 如果没有找打player游戏对象，则记录错误信息
        if (playerGobj == null)
        {
            Debug.LogError("Coundl't find a Player in the level!");
        }
        else
        {
            // 拿取Player脚本并启用
            var playerScript = playerGobj.GetComponent<Player>();
            playerScript.enabled = true;
            // 通过脚本访问Main Camera并启用
            playerScript.cam.SetActive(true);
            // 销毁UI，等到main场景再次被加载时，这个附加了ui脚本的对象会再次出现
            Destroy(this.gameObject);
        }

           
    }
}
