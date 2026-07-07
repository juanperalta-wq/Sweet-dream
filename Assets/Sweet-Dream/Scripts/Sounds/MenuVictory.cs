using Sirenix.OdinInspector;
using UnityEngine;

public class MenuVictory : MonoBehaviour
{
    [BoxGroup("Sound")]
    [Required]
    [SerializeField] private AudioClip menuMusic;
    private void Start()
    {
        MusicPlayer.Instance.Play(menuMusic);
    }
}
