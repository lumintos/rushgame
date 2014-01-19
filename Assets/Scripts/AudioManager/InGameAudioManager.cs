using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This AudioManager class is used to manage the audio and play the corresponding audios when needed
public class InGameAudioManager : Singleton<InGameAudioManager> {

	// List of clip audio to play when necessary 
	private List<ClipInfo> m_activeAudio;

	// set the indices of the animation audio
	private int _runAudioIdx, _jumpAudioIdx, _doubleJumpAudioIdx, _winAudioIdx, _loseAudioIdx, _getStoneAudioIdx;

	//sfx for animation of the player
	public AudioClip[] animationAudio;


	// This ClipInfo class is only accessed by the Audio class (nesting class)
	class ClipInfo
	{
		//ClipInfo used to maintain default audio source info
		public AudioSource source { get; set; }
		public float defaultVolume { get; set; }
	}

	void Awake() {
		Debug.Log("AudioManager Initializing");
		try {
			transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform;
			//transform.localPosition = new Vector3(0, 0, 0);
			m_activeAudio = new List<ClipInfo>();
		} catch {
			Debug.Log("Unable to find main camera to put audiomanager");
		}
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
}

