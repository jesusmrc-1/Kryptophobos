using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FootstepSet", menuName = "Scriptable Objects/FootstepSet")]
public class FootstepSet : ScriptableObject
{
    public List<AudioClip> clips;
    public float volume = 1f;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Count == 0) return null;
        return clips[Random.Range(0, clips.Count)];
    }

    public float GetRandomPitch()
    {
        return Random.Range(pitchRange.x, pitchRange.y);
    }
}
