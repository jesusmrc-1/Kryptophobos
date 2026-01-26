using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;

public class DiegeticText : MonoBehaviour
{
    [Header("⚙️ Configuración del Texto")]
    [Tooltip("El tiempo que tarda el texto en aparecer y desaparecer.")]
    [SerializeField] private float timeToVanish = 0.2f;

    [Tooltip("La velocidad de giro del texto.")]
    [SerializeField] private float rotateSpeed = 0.02f;

    private UIManager _UImanager;
    private TextMeshProUGUI _textMeshPro;

    private bool _textVisible;

    private void Start()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();

        //Cambia el Custom Render Queue para visualizar el texto por encima de los materiales con transparencia, se puede ver el valor en el inspector
        //si se cambia de modo Normal a Debug en los 3 puntos que estan al lado del candado arriba a la izquieda del todo.
        _textMeshPro.fontMaterial.renderQueue = 3001;

        //El texto empiza invisible.
        Color alpha = new Color (1,1,1,0);
        _textMeshPro.color = alpha;

        //REFERENCIAMOS EL TMP AL UI MANAGER
        //_UImanager = UIManager.Instance;
        //UIManager.Instance.DiegeticTMP = _textMeshPro;
    }

    private void Update()
    {
        FaceTextToMainCamera();
    }

    //Orientar el texto hacia la camara
    void FaceTextToMainCamera()
    {
        //Calcular la dirección en la que tiene que rotar el texto.
        Vector3 direction = transform.position - Camera.main.transform.position;
        direction.y = 0f;

        //Rotar el texto en la dirección calculada previamente
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion smoothedRotatio = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed);
        transform.rotation = smoothedRotatio;
    }

    public void CallCoroutine(string coroutine)
    {
        if (coroutine == "ShowText")
        {
            if (!_textVisible)
            {
                StopAllCoroutines();
                _textVisible = true;
                StartCoroutine(coroutine);
            }
        }
        else if (coroutine == "HideText")
        {
            if (_textVisible)
            {
                StopAllCoroutines();
                _textVisible = false;
                StartCoroutine(coroutine);
            }
        }
    }

    IEnumerator ShowText()
    {
        Color color = _textMeshPro.color;

        float timer = 0f;

        while (timer < timeToVanish)
        {
            timer += Time.deltaTime;
            float t = timer / timeToVanish;
            color.a = Mathf.Lerp(color.a, 1, t);
            _textMeshPro.color = color;
            yield return null;
        }
    }

    IEnumerator HideText()
    {
         Color color = _textMeshPro.color;

         float timer = 0f;

         while (timer < timeToVanish)
         {
             timer += Time.deltaTime;
             float t = timer / timeToVanish;
             color.a = Mathf.Lerp(color.a, 0, t);
             _textMeshPro.color = color;
             yield return null;
         }
     }
}
