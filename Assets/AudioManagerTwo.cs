using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioManagerTwo : MonoBehaviour
{

    public static AudioManagerTwo instance;

    private GameObject SoundHolder;
    private GameObject TempSound;
    [SerializeField] AudioMixerGroup sfxSound;
   [SerializeField] private AudioSource audioSource;


    [SerializeField] SoundClip soundClip;
    [SerializeField] private List<SoundClip> CurrentSoundData;
    //[SerializeField] private List<AudioSource> CurrentSounds;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        //CurrentSounds = new List<AudioSource>();

    }
    private void Start()
    {
        //SoundHolder = new GameObject("SoundHolder");
       // audioSource = SoundHolder.AddComponent<AudioSource>();
        

        TempSound = new GameObject();
       // TempSound.AddComponent<AudioSource>();
        audioSource = TempSound.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = sfxSound;
    }
    //this will pick the sound that needs to be played
    public void PlaySoundOnce()
    {
        audioSource.PlayOneShot(soundClip.sfxAudio, 1.0f);
       
    }
    //this is for playing sounds for abilites or animations location 
    public void PlayClipAtPoint()
    {
        AudioSource.PlayClipAtPoint(soundClip.sfxAudio, new Vector3(0,0,0));
    }
    //needs to pick the sound from a list 

    //Plays a specific sound at a specfic spot and sets the volume to the 
    public void PlaySound(SoundClip Sound, Vector3 target)
    {
       // AudioSource sound = Instantiate(TempSound, target, Quaternion.identity, SoundHolder.transform).GetComponent<AudioSource>();
        AudioSource sound = Instantiate(TempSound, target, Quaternion.identity, TempSound.transform).GetComponent<AudioSource>();
        sound.clip = Sound.sfxAudio;
        sound.outputAudioMixerGroup = sfxSound;
        //If the sound doesn't loop then it should be destroyed aftera  while
        if (!Sound.DoesLoop)
        {

            StartCoroutine(RunSound(Sound.sfxAudio.length, sound, Sound));
        }
         else
        {
            //CurrentSounds.Add(sound);
            CurrentSoundData.Add(Sound);
        }


sound.Play();
    }
    IEnumerator RunSound(float seconds, AudioSource sound, SoundClip soundData)
    {
       // CurrentSounds.Add(sound);
        CurrentSoundData.Add(soundData);
        yield return new WaitForSeconds(seconds);
        //CurrentSounds.Remove(sound);
        CurrentSoundData.Remove(soundData);
        Destroy(sound.gameObject);
    }
}
