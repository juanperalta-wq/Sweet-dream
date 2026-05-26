using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{


    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {

    }
    public void PlayAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();

        //audioSource.clip.length

        //->Corrutinas
        //->Invoke
        Invoke(nameof(ReturnToPool), audioSource.clip.length);
    }
    public void ReturnToPool()
    {
        //->comunicarme con mi music pool
        //-> regresar a casa :c 
        audioSource.clip = null;
        MusicPool.OnFinishAudio?.Invoke(this);
    }
}