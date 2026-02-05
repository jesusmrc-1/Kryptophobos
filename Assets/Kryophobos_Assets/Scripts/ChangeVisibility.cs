using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeVisibility : MonoBehaviour
{
    [SerializeField] private bool _gameObjectCanBeDisabled;

    [SerializeField] private bool _beginDisabled;
    [SerializeField] private Renderer _renderMesh;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private Light _light;
    [SerializeField] private Image _image;

    private void Awake()
    {
        _renderMesh = GetComponent<Renderer>();
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _light = GetComponent<Light>();
        _image = GetComponent<Image>();

        if (_beginDisabled)
        {
            if (_renderMesh != null) _renderMesh.enabled = false;
            if (_textMeshPro != null) _textMeshPro.enabled = false;
            if (_light != null) _light.enabled = false;
            if (_image != null) _image.enabled = false;
        }
    }

    public void DisableRenderer()
    {
        if (_gameObjectCanBeDisabled)
        {
            gameObject.SetActive(false);
            return;
        }

        if (_renderMesh != null) _renderMesh.enabled = false;
        if (_textMeshPro != null) _textMeshPro.enabled = false;
        if (_light != null) _light.enabled = false;
        if (_image != null) _image.enabled = false;
    }

    public void EnableRenderer()
    {
        if (_gameObjectCanBeDisabled)
        {
            gameObject.SetActive(true);
            return;
        }

        if (_renderMesh != null) _renderMesh.enabled = true;
        if (_textMeshPro != null) _textMeshPro.enabled = true;
        if (_light != null) _light.enabled = true;
        if (_image != null) _image.enabled = true;
    }
}