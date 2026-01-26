using UnityEngine;

public class LightManager: MonoBehaviour
{
    public GameObject light;


    void Start()
    {
        turnOffLIght();
    }

    public void turnOnLIght() {
        light.SetActive(true);
    }

    public void turnOffLIght()
    {
        light.SetActive(false);
    }

}
