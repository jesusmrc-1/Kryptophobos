using System.Collections;
using TMPro;
using UnityEngine;

public class WalkieTalkieCall : MonoBehaviour
{
    public Animator Animator;
    [SerializeField] private AudioSource TextBeep;

    public float DelayBeforeEndCall;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void Call()
    {
        TMP.text = "";
        Animator.SetTrigger("Call");
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(DelayBeforeEndCall);

        EndCall();
    }

    public void EndCall()
    {
        TMP.text = "";
        Animator.SetTrigger("EndCall");
    }

    #region Letra a letra
    public TextMeshProUGUI TMP;
    public string TextToShow;
    public float TextSpeed = 0.05f;

    IEnumerator ShowText()
    {
        TMP.text = "";

        foreach (char letra in TextToShow)
        {
            TMP.text += letra;
            if (TextBeep != null) TextBeep.Play();
            yield return new WaitForSeconds(TextSpeed);
        }
    }
    #endregion
}
