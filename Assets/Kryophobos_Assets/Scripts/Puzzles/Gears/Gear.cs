using UnityEngine;

public class Gear : MonoBehaviour
{
    private PuzzleObjectData _objectData;
    private bool _hasGearClicked;
    private bool _canGearFit = true;
    private float _speed = 1.0f;
    private float _despawnTime = 3f;
    private Collider _collider;

    public Vector3 MovingDir;

    [HideInInspector] public string TypeOfGear;
    //[HideInInspector] public int Index;
    [HideInInspector] public Vector3 NearestSocket;   //Guarda la información del hueco en el que se coloca al ser instanciada desde el script Puzzle_Gears.
    [HideInInspector] public GearMachine PuzzleGears;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _objectData = GetComponent<PuzzleObjectData>();
    }
    void Update()
    {
        bool isPuzzleSolved = PuzzleGears.GetPuzzleStatus();
        if (isPuzzleSolved) Destroy(gameObject);


        //PONER DE FORMA PUBLICA ENUM PARA SELECCIONAR LA DIRECCIÓN DE COLOCACIÓN


        //Animación de colocar el engranaje 
        if (!_hasGearClicked) transform.position -= MovingDir * _speed * Time.deltaTime;

        //Animación de quitar el engranaje
        else if (!_canGearFit) 
        {
            transform.position += transform.right * _speed * Time.deltaTime;
            _collider.enabled = false;   //Evitamos que si el engranaje no cabe, no poder hacer click en el cuando esta volviendo para evitar sumar un engranaje extra.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Si el engranaje toca en hueco, cambiamos el estado del engranaje.
        if (other.tag == "GearSocket")
        {
            _hasGearClicked = true;
        }

        //Si el engranaje toca cualquier otro y no esta colocado, se quita el engranaje y se devuelve a la cantidad que teniamos.
        else if((other.tag == "Big_Gear" || other.tag == "Medium_Gear" || other.tag == "Small_Gear" || other.tag == "Obstacle") && !_hasGearClicked && _canGearFit)
        {
            GearMachine puzzleGearsScript = GameObject.FindGameObjectWithTag("Puzzle_Gears").GetComponent<GearMachine>();

            if (puzzleGearsScript != null)
            {
                puzzleGearsScript.SetSocketObjectID(_objectData.SocketIndex, "Empty ID");
                puzzleGearsScript.SetSocketStatus(_objectData.SocketIndex, false);
                puzzleGearsScript.ReturnObject(_objectData.ID);

                _canGearFit = false;
                Destroy(gameObject, _despawnTime);
            }
        }
    }
}
