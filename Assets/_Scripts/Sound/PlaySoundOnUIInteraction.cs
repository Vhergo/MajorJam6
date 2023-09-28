using UnityEngine;

public class PlaySoundOnUIInteraction : MonoBehaviour
{
    [SerializeField] private AudioClip selectSound;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip releaseSound;
    [SerializeField] private AudioClip exitSound;

    
    public void PlayOnSelect() {
        SoundManager.Instance.PlayUISound(selectSound);
    }

    public void PlayOnHover() {
        SoundManager.Instance.PlayUISound(hoverSound);
    }

    public void PlayOnRelease() {
        SoundManager.Instance.PlayUISound(releaseSound);
    }

    public void PlayOnExit() {
        SoundManager.Instance.PlayUISound(hoverSound);
    }
}
