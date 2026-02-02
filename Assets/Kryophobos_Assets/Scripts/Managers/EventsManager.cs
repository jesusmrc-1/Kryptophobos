using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EventsManager : MonoBehaviour
{
    [Header("❓ ¿Como funciona?")]
    //#pragma es una instrucción del compilador (compiler directive) que le dice a C# que cambie cómo procesa el código.
    //No es una función, no es una variable, no es código que se ejecute en tiempo de juego.
    //Es solo una orden especial que se lee antes de compilar.
#pragma warning disable 0414
    [SerializeField, TextArea(2, 6)]
    string info = "Este 'GameObject' contiene los eventos del juego, como pausar el juego o reanudarlo," +
        " los objetos que queramos que estén al tanto de cuando se llama a un evento, se pueden añadir pulsando" +
        " al simbolo +, luego buscas el 'GameObject' pulsando al simbolo del círculo con un punto en el centro" +
        " o lo arrastras al rectángulo abajo de 'Runtime Only', luego en el rectángulo de la derecha eliges que" +
        " función quieres que se llame por el evento.";
#pragma warning restore 0414

    [Tooltip("Este evento se llama cuando queremos que uno o varios 'GameObject' se detengan.")]
    public UnityEvent OnPause;

    [Tooltip("Este evento se llama cuando queremos que uno o varios 'GameObject' dejen de estar detenidos.")]
    public UnityEvent OnResume;

    public UnityEvent OnDoingPuzzle;

    public UnityEvent OnNotDoingPuzzle;

    public UnityEvent OnFocusingPOI;
    public UnityEvent OnUnfocusingPOI;

    public UnityEvent OnPlayerDeath;

    [SerializeField] private SlidingDoor _slidingDoorServerRoom;
    [SerializeField] private SlidingDoor _slidingDoorEntranceRoom;
    [SerializeField] private Door _serverRoomDoorFloatingText;

    [SerializeField] private Puzzle _triggerPuzzleElectronics;
    [SerializeField] private Puzzle _triggerPuzzleGears;

    [SerializeField] private GameObject _triggerEnemySpawner;

    [SerializeField] private GameObject _triggerEnemyFirstEncounter;
    [SerializeField] private GameObject _triggerEndingFirstLevel;
    [SerializeField] private BoxCollider _triggerEndingButton;

    [SerializeField] private float _fadeTime;

    [SerializeField] private GameObject[] _enemies;

    public void PauseGame()
    {
        Time.timeScale = 0f;

        //Esto es lo mismo que if (OnPause != null), para evitar Null reference
        OnPause?.Invoke();
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;

        //Esto es lo mismo que if (OnResume != null), para evitar Null reference
        OnResume?.Invoke();
    }

    public void DoingPuzzle()
    {
        Debug.Log("Class: " + this + " " + "Method: DoingPuzzle()");
        OnDoingPuzzle?.Invoke();
    }

    public void NotDoingPuzzle()
    {
        Debug.Log("Class: " + this + " " + "Method: NotDoingPuzzle()");
        OnNotDoingPuzzle?.Invoke();
    }

    public void PuzzleSolved(PuzzleBase puzzle)
    {
        switch (puzzle)
        {
            case PCB:
                //Cinematica, borrar trigger puzzle habilitar booleana interna.
                _triggerPuzzleElectronics.Disable();
                _slidingDoorServerRoom.UnlockDoor();
                Destroy(_serverRoomDoorFloatingText);
                _triggerEnemyFirstEncounter.SetActive(true);
                break;
            case GearMachine:
                //Cinematica, borrar trigger puzzle habilitar booleana interna.
                _triggerPuzzleGears.Disable();
                //_triggerPuzzleGears.RemoveInventoryItem();
                _triggerPuzzleGears.Invoke("RemoveInventoryItem", 2f);
                //_slidingDoorEntranceRoom.UnlockDoor();
                //_triggerEnemySpawner.SetActive(true);
                _triggerEndingFirstLevel.SetActive(true);
                _triggerEndingButton.enabled = true;
                
                foreach (var enemy in _enemies)
                {
                    if (enemy != null) enemy.SetActive(true);
                }

                break;
        }
    }

    public void PlayerDead()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        StartCoroutine(ManageScene(currentScene.name));

        //OnPlayerDeath?.Invoke(); //No se lee porque carga de nuevo la escena
    }

    public void FocusingPOI()
    {
        OnFocusingPOI?.Invoke();
    }

    public void UnfocusingPOI()
    {
        OnUnfocusingPOI?.Invoke();
    }

    public IEnumerator ManageScene(string sceneName)
    {
        Debug.Log("ManageScene " + sceneName);
        //Fade
        GameObject FadeGO = GameObject.FindGameObjectWithTag("Fade");
        if (FadeGO != null) 
        {
            Animator animator = FadeGO.GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetTrigger("FadeOut");
            }
        }

        yield return new WaitForSeconds(_fadeTime);

        SceneManager.LoadScene(sceneName);
    }

    //Añadir mas eventos..
}
