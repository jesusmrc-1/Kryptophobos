using System.Collections;
using TMPro;
using UnityEngine;

public class ShowObjective : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private float _timeToVanish;
    [SerializeField] private float _timeShowingObjective;

    //Se pone en awake la referencia porque en Start al ser llamada la funcion NewObjective desde UIManager, no le da tiempo a referenciar y da error null reference
    private void Awake()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _textMeshPro.text = "";
    }

    public void NewObjective()
    {
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        Color color = _textMeshPro.color;

        float timer = 0f;

        while (timer < _timeToVanish)
        {
            timer += Time.deltaTime;
            float t = timer / _timeToVanish;
            color.a = Mathf.Lerp(color.a, 1, t);
            _textMeshPro.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(_timeShowingObjective);

        StartCoroutine(HideText());
    }

    IEnumerator HideText()
    {
        Color color = _textMeshPro.color;

        float timer = 0f;

        while (timer < _timeToVanish)
        {
            timer += Time.deltaTime;
            float t = timer / _timeToVanish;
            color.a = Mathf.Lerp(color.a, 0, t);
            _textMeshPro.color = color;
            yield return null;
        }
    }
}
