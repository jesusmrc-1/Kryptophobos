using UnityEngine;

public class Puzzle : TriggerBase
{
    private Player _player;
    private PlayerInventory _inventory;
    private CameraManager _cameraManager;

    //POLIMORFISMO, los puzzles al heredar de PuzzleBase, se puede referenciar el script del puzle que nos interese aquí.
    [Tooltip("Arrastrar el GameObject que tenga el script del puzzle.")]
    [SerializeField] PuzzleBase puzzle;

    [Tooltip("La referencia de la camara que pertenece al puzle.")]
    [SerializeField] private GameObject _cam;

    [Tooltip("OPCIONAL: La referencia del Animator que pertenece al puzle.")]
    [SerializeField] private Animator _animator;

    [Tooltip("OPCIONAL: Desmarca esta casilla si quieres que el puzle no necesite objetos.")]
    [SerializeField] private bool _isItemRequired = true;
    [Tooltip("Si se necesita un objeto (Scriptable Object) para hacer el puzle, entonces referenciarlo aquí.")]
    [SerializeField] private ItemData _requiredItem;

    [Tooltip(".")]
    [SerializeField] private string _noItemRequiredText;

    [Tooltip(".")]
    [SerializeField] private string _itemRequiredText;

    [SerializeField] private bool _canInteract;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        _cameraManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();

        _cam.SetActive(false);

        if (_isItemRequired) DiegeticText = _itemRequiredText;
        else DiegeticText = _noItemRequiredText;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canInteract) return;
        if (!other.gameObject.CompareTag("Player")) return;
        _player.IsPlayerNearPuzzle = true;

        //Si se necesitan items, se comprueba que el jugador los tenga
        if (_isItemRequired) CheckRequiredItems();
        else _player.PlayerHasRequiredItems = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_canInteract) return;
        if (!other.gameObject.CompareTag("Player")) return;
        _player.IsPlayerNearPuzzle = false;

        _player.PlayerHasRequiredItems = false;
    }

    void CheckRequiredItems()
    {
        foreach (var item in _inventory.Items)
        {
            //Buscamos en el inventario comparando Scriptable Objects Item y RequiredItem, en caso de querer buscar mas objetos, hacer una lista con RequiredItems (Varios SO)
            if (item.Item == _requiredItem)
            {
                _player.PlayerHasRequiredItems = true;
                DiegeticText = _noItemRequiredText;
            }
        }
    }

    //Activar camara y animación opcional si hay en el puzle.
    public void EnablePuzzle()
    {
        puzzle.enabled = true;
        _cameraManager.FocusPuzzle(_cam);
        if (puzzle != null) puzzle.EnablePuzzle();
        _animator.SetTrigger("EnablePuzzle");
    }

    //Desactiva camara y animación opcional si hay en el puzle.
    public void DisablePuzzle()
    {
        puzzle.enabled = false;
        if (_cameraManager != null) _cameraManager.UnfocusPuzzle(_cam);
        if (puzzle != null) puzzle.DisablePuzzle();
        _animator.SetTrigger("DisablePuzzle");
    }

    public void Disable()
    {
        DisablePuzzle();
        _canInteract = false;

        if (_player != null)
        {
            _player.IsPlayerNearPuzzle = false;
            _player.IsPlayerDoingPuzzle = false;
            _player.PlayerHasRequiredItems = false;

            _player.ActionStateMachine.ChangeState(_player.NonActionState);
        }

        DiegeticText = string.Empty;
        _itemRequiredText = string.Empty;
        _noItemRequiredText = string.Empty;
    }

    public void RemoveInventoryItem()
    {
        //Borrar los engranajes
        if (_inventory != null)
        {
            ItemStack item = _inventory.Items.Find(itemStack => itemStack.Item == _requiredItem);
            _inventory.Items.Remove(item);
        }
    }

    public bool CanInteract()
    {
        return _canInteract;
    }
}