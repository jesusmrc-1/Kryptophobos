using System.Collections;
using UnityEngine;

public class Bright : MonoBehaviour
{
    [SerializeField] float brightTime;
    [SerializeField] float brightDelay;
    [SerializeField] Color normalColor;
    [SerializeField] Color targetColor;
    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        StartCoroutine(BrightEffect());
    }

    IEnumerator BrightEffect()
    {
        meshRenderer.material.EnableKeyword("_EMISSION");

        Color color = Color.white;

        float timer = 0f;

        float intensity = 0f;

        while (true)
        {
            while (timer < brightTime)
            {
                timer += Time.deltaTime;
                float t = timer / brightTime;

                color = Color.Lerp(normalColor,targetColor, t);

                //No funciona bien, hay que "refrescar Unity"? mientras sube el valor de la intensidad, si tocas alguna propiedad del material se ven cambios
                intensity = Mathf.Lerp(0f, 1000f, t);
                meshRenderer.material.SetFloat("_EmissiveIntensity", intensity);


                meshRenderer.material.color = color;

                yield return null;
            }

            timer = 0f;

            while (timer < brightTime)
            {
                timer += Time.deltaTime;
                float t = timer / brightTime;

                color = Color.Lerp(targetColor, normalColor, t);

                //""
                intensity = Mathf.Lerp(1000f, 0f, t);
                meshRenderer.material.SetFloat("_EmissiveIntensity", intensity);

                meshRenderer.material.color = color;

                yield return null;
            }

            timer = 0f;

            yield return new WaitForSeconds(brightDelay);
        }
    }
}
