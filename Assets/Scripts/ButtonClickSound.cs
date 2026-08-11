using UnityEngine;

public class ButtonClickSound : MonoBehaviour
{
    public void PlayClick()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PlayClick();
    }
}