using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour
{
    private void Update()
    {
        GameManager.Instance.ClearSavedData();
        SceneManager.LoadScene(0);
    }
}
