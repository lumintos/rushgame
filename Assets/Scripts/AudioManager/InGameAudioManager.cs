// The code here is basically reference from: http://dirigiballers.blogspot.fr/2013/03/unity-c-audiomanager-tutorial-part-1.html
// You can go there for more information

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This AudioManager class is used to manage the audio and play the corresponding audios when needed
public class InGameAudioManager : Singleton<InGameAudioManager> {

	// List of clip audio to play when necessary 
	private List<ClipInfo> m_activeAudio;

	// set the indices of the animation audio
	private int _startGameIdx, _backgroundMusicIdx, _winSoundIdx, _loseSoundIdx;

	//sfx for animation of the player
	public AudioClip[] inGameAudio;

	// AudioSource for background music
	private AudioSource _backgroundAudioSource;

	// This ClipInfo class is only accessed by the Audio class (nesting class)
	class ClipInfo
	{
		//ClipInfo used to maintain default audio source info
		public AudioSource source { get; set; }
		public float defaultVolume { get; set; }
	}

	void Awake() {
		Debug.Log("IngameAudio Manager Initializing");
		try {
			transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform;
			//transform.localPosition = new Vector3(0, 0, 0);
			m_activeAudio = new List<ClipInfo>();
			_startGameIdx = 0;
			_backgroundMusicIdx = 1;
			_winSoundIdx = 2;
			_loseSoundIdx = 3;
		} catch {
			Debug.Log("Unable to find main camera to put audiomanager");
		}
	}

	
	// These functions play the corresponding sfx depending on the context
	// with the given position and volume
	public void PlayStartSfx(Vector3 soundOrigin, float volume)
	{
		AudioSource.PlayClipAtPoint(inGameAudio[_startGameIdx], soundOrigin, volume);
	}

	// The background should be looped throughout the game
	public void PlayBackground(Vector3 soundOrigin, float volume)
	{
		GameObject soundLoc = new GameObject("Audio: " + inGameAudio[_backgroundMusicIdx].name);
		soundLoc.transform.position = soundOrigin;
		//Create the source
		AudioSource source = soundLoc.AddComponent<AudioSource>();
		setSource(ref source, inGameAudio[_backgroundMusicIdx], volume);
		source.loop = true;
		source.Play();

		_backgroundAudioSource = source;

		// add this line to insert this sound to the m_activeAudio for stopping it when necessary
		m_activeAudio.Add(new ClipInfo{source = source, defaultVolume = volume});

		//AudioSource.PlayClipAtPoint(inGameAudio[_backgroundMusicIdx], soundOrigin, volume);
	}

	// Stop playing background music
	public void StopBackgroundMusic()
	{
		stopSound(_backgroundAudioSource);
	}


	public void PlayWinSfx(Vector3 soundOrigin, float volume)
	{	
		AudioSource.PlayClipAtPoint(inGameAudio[_winSoundIdx], soundOrigin, volume);
	}

	public void PlaySLoseSfx(Vector3 soundOrigin, float volume)
	{
		AudioSource.PlayClipAtPoint(inGameAudio[_loseSoundIdx], soundOrigin, volume);
	}



	// This function will play the audio clip
	// Usage: AudioManager.Instance.Play(parameters)
	public AudioSource Play(AudioClip clip, Vector3 soundOrigin, float volume) {
		//Create an empty game object
		GameObject soundLoc = new GameObject("Audio: " + clip.name);
		soundLoc.transform.position = soundOrigin;
		
		//Create the source
		AudioSource source = soundLoc.AddComponent<AudioSource>();
		setSource(ref source, clip, volume);
		source.Play();
		Destroy(soundLoc, clip.length);
		
		//Set the source as active
		m_activeAudio.Add(new ClipInfo{source = source, defaultVolume = volume});
		return source;
	}

	// This function sets the properties of the new audio source. 
	private void setSource(ref AudioSource source, AudioClip clip, float volume) {
		source.rolloffMode = AudioRolloffMode.Logarithmic;
		source.dopplerLevel = 0.2f;
		source.minDistance = 150;
		source.maxDistance = 1500;
		source.clip = clip;
		source.volume = volume;
	}

	// Play loop sound
	public AudioSource PlayLoop(AudioClip loop, Transform emitter, float volume) {
		//Create an empty game object
		GameObject movingSoundLoc = new GameObject("Audio: " + loop.name);
		movingSoundLoc.transform.position = emitter.position;
		movingSoundLoc.transform.parent = emitter;
		//Create the source
		AudioSource source = movingSoundLoc.AddComponent<AudioSource>();
		setSource(ref source, loop, volume);
		source.loop = true;
		source.Play();
		//Set the source as active
		m_activeAudio.Add(new ClipInfo{source = source, defaultVolume = volume});
		return source;
	}

	// Stop the looped sound
	public void stopSound(AudioSource toStop) {
		try {
			Destroy(m_activeAudio.Find(s => s.source == toStop).source.gameObject);
		} catch {
			Debug.Log("Error trying to stop audio source "+toStop);
		}
	}
}

