using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    [Header("🔗 References")]

    [SerializeField] private GameObject _flashlightSliderCharge;
    private Slider _flashlightSlider;

    [Header("👁️ Estado de la linterna")]
    [SerializeField] private bool _isOverCharged;
    [SerializeField] private bool _isFlashing;

    [Header("⚙️ Configuración del flash 💥")]
    [SerializeField] private float _flashTime = 0.1f;

    [SerializeField] private float _cooldown = 2f;
    [SerializeField] private float _flashIntensity = 40000f;

    [Header("⚙️ Configuración de la batería 🔋")]
    [SerializeField] private float _batteryCharge = 0f;
    [SerializeField] private float _maxBatteryCharge = 100f;
                     
    [SerializeField] private float _batteryOverCharge = 0f;
    [SerializeField] private float _maxBatteryOverCharge = 10f;
                     
    [SerializeField] private float _chargeRate = 25f;
    [SerializeField] private float _dischargeRate = 10f;

    [Header("⚙️ Configuración de la intensidad 💡")]
    [SerializeField] private float _lightIntensity = 0f;
    [SerializeField] private float _maxLightIntensity = 1500f;
    [SerializeField] private float _maxLightIntensityAtThisCharge = 75f;

    [Header("⚙️ Configuración de la temperatura 💡")]
    [SerializeField] private float _lightTemperature = 5500f;
    [SerializeField] private float _maxChargeLightTemperature = 5500f;
    [SerializeField] private float _maxOverchargedLightTemperature = 3200f;

    [Header("⚙️ Configuración del ángulo 💡")]
    [SerializeField] private float _minSpotAngle = 20f;
    [SerializeField] private float _maxSpotAngle = 80f;
    [SerializeField] private float _flashSpotAngle = 100f;

    private Light _lightBulb;
    private Player _player;
    private Slider _slider;
    private Animator _animator;
    private Animator _animatorCharacter;

    private bool _isGamePaused;

    [HideInInspector] public float ManualCharge;

    [SerializeField] GameObject _flash;


    private void Awake()
    {
        LayerMask ignoredMasks = IgnorePlayer | IgnoreEnemyVision | IgnoreRaycast | IgnoreInteractable;
        _ignoreLayerMasks = ~ignoredMasks;
    }

    void Start()
    {
        //charge.Enable();
        _animator = GetComponent<Animator>();

        if (transform.GetChild(0).GetComponent<Light>() == null)
        {
            Debug.LogError("Asegurate de que el primer hijo de este 'GameObject' es el que tiene el componente Light, si no es asi" +
                " cambia el numero de GetChild(NUMERO), por el correcto, va de '0' que es el primer hijo hasta el ultimo.");
        }

        _lightBulb = transform.GetChild(0).GetComponent<Light>();

        if (_flashlightSliderCharge == null) Debug.LogWarning("Falta referenciar en el inspector el GameObject slider a la linterna");
        else { _slider = _flashlightSliderCharge.GetComponent<Slider>(); }

        //gameObject.SetActive(false);


        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _animatorCharacter = _player.GetComponent<Animator>();

        if (_flashlightSliderCharge != null)
        {
            _flashlightSlider = _flashlightSliderCharge.GetComponent<Slider>();

            if (_flashlightSlider != null) _flashlightSlider.maxValue = _maxBatteryCharge + _maxBatteryOverCharge;
        }
    }

    void Update()
    {
        //Leer input
        //float ManualCharge = charge.ReadValue<float>();

        if (_slider != null) _slider.value = _batteryCharge;

        //if (_isFlashing) _animatorCharacter.SetBool("isCharging", false);

        //Si la linterna no esta en el estado de _isFlashing
        if (!_isFlashing)
        {
            //CARGA Y DESCARGA DE LA BATERIA

            if (_flashlightSlider != null)
            {
                
                if (_batteryCharge > 0.1f)
                {
                    _flashlightSliderCharge.SetActive(true);
                }
                else if (_batteryCharge < 0.1f)
                {
                    _flashlightSliderCharge.SetActive(false);
                }
            }

            if (_player != null) ManualCharge = _player.ChargeFlashlight.ReadValue<float>();

            //Si el jugador carga la linterna con la dinamo, la bateria se ira cargando
            if (ManualCharge == 1f && !_player.IsPlayerInteracting && !_player.IsPlayerDoingPuzzle && !_isGamePaused) ChargeBattery();

            //Si deja de hacerlo, la bateria se ira descargando
            else DischargeBattery();

            //SOBRECARGA Y DES-SOBRECARGA DE LA BATERIA

            //Si el jugador sigue cargando la linterna con la dinamo, sobrecargamos el valor de la bateria
            if (_isOverCharged && ManualCharge == 1f) OverchargeBattery();

            //Si deja de hacerlo, la sobrecarga ira bajando
            else if (_isOverCharged && ManualCharge == 0f) DisoverchargeBattery();

            //INTENSIDAD DE LA BOMBILLA SEGÚN CARGA

            Change_lightIntensity();

            //TEMPERATURA DE LA BOMBILLA SEGÚN CARGA

            Change_lightTemperature();

            //FLASHAZO POR SOBRECARGA

            //Si la bateria llega al tope de sobrecarga, meter flashazo y descargar por completo la carga.
            if (_batteryOverCharge >= _maxBatteryOverCharge) StartCoroutine(Flash());
        }
    }

    void ChargeBattery()
    {
        //Prender parametro de anim bool personaje
        //_animatorCharacter.SetBool("isCharging", true);

        //Prender parametro de anim bool linterna
        //_animator.SetBool("flashCharging", true);

        //Ralentizar al jugador
        //

        //Cargar la bateria poco a poco
        _batteryCharge += Time.deltaTime * _chargeRate;

        //Si la carga de la bateria es mayor a la "maxima carga" que puede tener, el estado de sobre cargada pasa a TRUE
        if (_batteryCharge > _maxBatteryCharge) _isOverCharged = true;

        //Si la carga de la bateria es menos a la "maxima carga" que puede tener, el estado de sobre cargada pasa a FALSE
        else { _isOverCharged = false; _batteryOverCharge = 0f; }
    }
    void DischargeBattery()
    {
        if (_isGamePaused) return;

        //Apagar parametro de anim bool personaje
        //_animatorCharacter.SetBool("isCharging", false);

        //Apagar parametro de anim bool linterna
        //_animator.SetBool("flashCharging", false);

        //Dejar de ralentizar al jugador
        //

        //Descargar la bateria
        if (_batteryCharge < 0f) _batteryCharge = 0f;
        else _batteryCharge -= Time.deltaTime * _dischargeRate;
    }

    void OverchargeBattery()
    {
        _batteryOverCharge += Time.deltaTime * _chargeRate;
    }
    void DisoverchargeBattery()
    {
        if (_batteryOverCharge < 0f) _batteryOverCharge = 0f;
        else _batteryOverCharge -= Time.deltaTime * _dischargeRate;
    }

    void Change_lightIntensity()
    {
        //t = 1 cuando la carga de la bateria llegue a tener la suficiente carga como para que la luz ya no brille mas.
        //Se divide la carga de la bateria / la carga a la cual la luz llega a su máxima intensidad
        float t = _batteryCharge / _maxLightIntensityAtThisCharge;

        //El valor float de Light Intensity del componente Light ira de 0 a _maxLightIntensity en t
        _lightIntensity = Mathf.Lerp(0, _maxLightIntensity, t);

        //Aplicamos el valor a intensity del componente de la luz
        _lightBulb.intensity = _lightIntensity;
    }
    void Change_lightTemperature()
    {
        //t = 1 cuando la sobrecarga de la bateria llegue a tener la máxima sobrecarga, la bombilla llegara a su temperatura máxima
        //Se divide la sobrecarga de la bateria / la sobrecarga maxima que puede llegar la bateria y así cambiar la temperatura de la bombilla
        float t = _batteryOverCharge / _maxBatteryOverCharge;

        //El valor float de Light Temperature del componente Light ira de _maxChargeLightTemperature a _maxOverchargedLightTemperature en t
        _lightTemperature = Mathf.Lerp(_maxChargeLightTemperature, _maxOverchargedLightTemperature, t);

        //El angulo del foco de luz se cerrara o abrire según t
        _lightBulb.spotAngle = Mathf.Lerp(_maxSpotAngle, _minSpotAngle, t);

        //Aplicamos el valor a colorTemperature del componente de la luz
        _lightBulb.colorTemperature = _lightTemperature;
    }

    IEnumerator Flash()
    {
        _isFlashing = true;
        _animatorCharacter.SetTrigger("FlashStun");
        FlashTargets();

        float timer = 0f;

        //Aumentamos el angulo, la temperatura y la intensidad de la luz.
        while (true)
        {
            timer += Time.deltaTime;
            float t = timer / _flashTime;
            _lightBulb.spotAngle = Mathf.Lerp(_maxSpotAngle, _flashSpotAngle, t);
            _lightBulb.colorTemperature = Mathf.Lerp(_maxOverchargedLightTemperature, 20000f, t);
            _lightBulb.intensity = _flashIntensity;
            if (timer >= _flashTime) break;
            yield return null;
        }

        //_flash.SetActive(true);

        timer = 0f;

        //Disminuimos el angulo, la temperatura y la intensidad de la luz.
        while (true)
        {
            timer += Time.deltaTime;
            float t = timer / _flashTime;
            _lightBulb.spotAngle = Mathf.Lerp(_flashSpotAngle, _maxSpotAngle, t);
            _lightBulb.colorTemperature = Mathf.Lerp(20000f, _maxOverchargedLightTemperature, t);
            _lightBulb.intensity = Mathf.Lerp(_flashIntensity, 0f,t);
            if (timer >= _flashTime) break;
            yield return null;
        }

        //_flash.SetActive(false);

        //Reiniciamos los valores de la luz y la bateria.
        _batteryCharge = 0f;
        _batteryOverCharge = 0f;
        _lightBulb.spotAngle = _maxSpotAngle;
        _lightBulb.colorTemperature = _lightTemperature;
        _lightBulb.intensity = 0;

        yield return new WaitForSeconds(_cooldown);

        _isFlashing = false;
        //_animatorCharacter.SetBool("flashStun", false);
    }

    public bool HasLanternFullyCharged()
    {
        return _isFlashing;
    }

    #region Flash Near Targets
    [SerializeField] private GameObject _area;
    [SerializeField] private GameObject _rayOrigin;
    [SerializeField] private float _flashRadius;
    [SerializeField] float _blindTime;

    [SerializeField] private LayerMask IgnorePlayer;
    [SerializeField] private LayerMask IgnoreEnemyVision;
    [SerializeField] private LayerMask IgnoreRaycast;
    [SerializeField] private LayerMask IgnoreInteractable;
    private int _ignoreLayerMasks;
    [SerializeField] private float _xOffset;

    private void FlashTargets()
    {
        Collider[] hits = Physics.OverlapSphere(
        _area.transform.position,
        _flashRadius,
        LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            Vector3 rayDirection = hit.transform.position - _rayOrigin.transform.position;
            rayDirection.x += _xOffset;

            float rayDistance = rayDirection.magnitude;
            rayDirection.Normalize();

            Debug.DrawRay(_rayOrigin.transform.position, rayDirection * rayDistance, Color.red, 5f);

            if (Physics.Raycast(
                _rayOrigin.transform.position,
                rayDirection,
                out RaycastHit hitInfo,
                rayDistance,
                _ignoreLayerMasks,
                QueryTriggerInteraction.Ignore
            ))
            {
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    Debug.LogWarning("Enemigo flasheado");
                    Enemy enemy = hit.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        if (!enemy.IsEnemyFlashed)
                        {
                            enemy.IsEnemyFlashed = true;
                            enemy.Attacking = false;
                            enemy.HoldingPosition = false;
                            enemy.IsPlayerInVisionRange = false;
                            enemy.PlayerSpotted = false;
                            enemy.PreparingAttack = false;
                            enemy.FollowingTarget = false;
                            enemy.StartSearching = false;
                            enemy.Animator.SetBool("IsTargetSpotted", true);
                            enemy.TargetLastKnownPosition = _player.gameObject.transform.position;
                            enemy.StartCoroutine(enemy.Blind(_blindTime));
                        }
                    }
                }
            }
        }
    }
    #endregion
}