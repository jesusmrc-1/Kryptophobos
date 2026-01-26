using UnityEngine;

/*La palabra abstract significa:

No se puede usar directamente esa clase

Las clases hijas están obligadas a implementar la función

Si no lo hacen, Unity marca error

Esto evita olvidos.
 */

public abstract class PuzzleBase : MonoBehaviour
{
    public abstract void EnablePuzzle();
    public abstract void DisablePuzzle();
}
