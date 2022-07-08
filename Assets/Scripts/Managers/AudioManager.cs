using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    music,
    ambient,
    soundFX,
    character,
    enemy,
}
/// <summary>
/// Only one of this type should exist to manage a levels volumes, Needs to eventually carry over settings from level to level for sound
/// </summary>
/// Adding Music would be done through a music manager which sends its sound tracks to this audio manager to play
/// The NavMeshScript will have to be setup to when the creature reaches their destination they should stop the sound effect for the sound
/// 
/// 
/// Setting up any type of sound script would be played through this script by sending it the Sound data.
///     The AudioController of the object is the middleman which is used to play what is needed.
///     
/// How the sound data should be retrieved to play for any type of audiocontroller
///     AudioManager(Instance)<-AudioController(PlayerAudioController)<-ComplexCreatureAudioData(DeathSound)<-SoundData
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private GameObject SoundHolder;
    private List<AudioSource> CurrentSounds;
    private GameObject TempSound;

    [SerializeField][Range(0f, 1f)] private float musicScale; //Music sounds
    [SerializeField][Range(0f, 1f)] private float ambientScale; // Ambient sounds
    [SerializeField][Range(0f, 1f)] private float soundFXScale; // Sound FX ex. abilities
    [SerializeField][Range(0f, 1f)] private float characterScale; //Character sounds ex. voicelines
    [SerializeField][Range(0f, 1f)] private float enemyScale; //Enemy Sounds ex. voicelines
    
    // Start is called before the first frame update
    void Awake()
    {
        if(instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        CurrentSounds = new List<AudioSource>();
    }
    private void Start()
    {
        SoundHolder = new GameObject("SoundHolder");

        TempSound = new GameObject();
        TempSound.AddComponent<AudioSource>();
    }
    //Used for when UI Sliders are adjusted in menu to change the games sound volume for those types
    public void AdjustVolume(SoundType type,float value)
    {
        switch (type)
        {
            case SoundType.music:
                musicScale = value;
                break;
            case SoundType.ambient:
                ambientScale = value;
                break;
            case SoundType.soundFX:
                soundFXScale = value;
                break;
            case SoundType.character:
                characterScale = value;
                break;
            case SoundType.enemy:
                enemyScale = value;
                break;
        }
    }
    //Plays a specific sound at a specfic spot and sets the volume to the 
    public void PlaySound(SoundData Sound, Vector3 target)
    {
        AudioSource sound = Instantiate(TempSound, target, Quaternion.identity, SoundHolder.transform).GetComponent<AudioSource>();
        sound.clip = Sound.audio;

        //Sets the volume to the audio type
        sound.volume = CalculateVolume(Sound);
        //If the sound doesn't loop then it should be destroyed aftera  while
        if (!Sound.DoesLoop)
        {
            StartCoroutine(RunSound(Sound.audio.length, sound));
        }

        sound.Play();
    }
    //Plays a sound that plays all across the game, ex music, ambient
    public void PlaySound(SoundData Sound)
    {
        AudioSource sound = Instantiate(TempSound, SoundHolder.transform).GetComponent<AudioSource>();
        sound.clip = Sound.audio;
        sound.loop = Sound.DoesLoop;

        //Sets the volume to the audio type
        sound.volume = CalculateVolume(Sound);

        if (!Sound.DoesLoop)
        {
            StartCoroutine(RunSound(Sound.audio.length, sound));
        }

        sound.Play();
    }
    //Returns the sound of the volume played and adjusts it by the scale of the sound
    float CalculateVolume(SoundData Sound)
    {
        //Sets the volume to the audio type
        switch (Sound.Type)
        {
            case SoundType.music:
                return Sound.Volume * musicScale;
            case SoundType.ambient:
                return Sound.Volume * ambientScale;
            case SoundType.soundFX:
                return Sound.Volume * soundFXScale;
            case SoundType.character:
                return Sound.Volume * characterScale;
            case SoundType.enemy:
                return Sound.Volume * enemyScale;
            default:
                return 1f;
        }
        //If the sound doesn't loop then it should be destroyed aftera  while
        
    }
    IEnumerator RunSound(float seconds, AudioSource sound)
    {
        CurrentSounds.Add(sound);
        yield return new WaitForSeconds(seconds);
        CurrentSounds.Remove(sound);

        Destroy(sound.gameObject);
    }
}
