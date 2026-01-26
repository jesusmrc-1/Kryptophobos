using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitOptions : MonoBehaviour
{
    [SerializeField] private GameObject buttonsContainerBackground;
    public void OnClickYes()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnClickNo() 
    {
        buttonsContainerBackground.SetActive(false);
    }
}
