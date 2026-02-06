using UnityEngine;
using UnityEngine.UI;

public class DamageFX : MonoBehaviour
{
    private Player _player;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        GameObject PlayerGO = GameObject.FindGameObjectWithTag("Player");
        if(PlayerGO != null)
        {
            _player = PlayerGO.GetComponent<Player>();
            if (_player != null)
            {
                _player.OnPlayerDamage += UpdateHealth;
            }
        }
    }

    private void UpdateHealth()
    {
        Debug.Log("UPDATE HEALTH");
        if (_image != null)
        {
            float alpha = 1f - (_player.CurrentHealth / _player.MaxHealth);
            //float alpha = Mathf.Pow(1f - (_player.CurrentHealth / _player.MaxHealth), 2f);
            //float alpha = Mathf.SmoothStep(0f, 1f, 1f - (_player.CurrentHealth / _player.MaxHealth));

            Color color = _image.color;
            color.a = alpha;
            _image.color = color;
        }
    }
}
