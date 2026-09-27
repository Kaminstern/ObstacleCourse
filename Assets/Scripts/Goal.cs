using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 如果other位于Player层
        if (other.gameObject.layer == 3)
        {
            SceneManager.LoadScene("main");
        }
    }
}
