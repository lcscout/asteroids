using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
	#region Fields
	public static AudioManager Instance { get; private set; }

	[Header("Settings")]
	[SerializeField] private bool dontDestroyOnLoad = true;
	[SerializeField] private int numberOfTracks = 4;
	[Header("References")]
	[SerializeField] private Button soundButton;
	[SerializeField] private GameObject soundHigh;
	[SerializeField] private GameObject soundLow;
	[SerializeField] private GameObject soundOff;
	[Header("Debug")]
	[SerializeField] private int soundOption = 0;
	[SerializeField] private AudioSource[] tracks;

	private const string SoundOptionKey = "SoundOption";
	#endregion

	#region Unity Methods
	private void Awake()
	{
		InitializeSingleton();

		tracks = new AudioSource[numberOfTracks];
		for (int i = 0; i < numberOfTracks; i++)
		{
			AudioSource track = gameObject.AddComponent<AudioSource>();
			tracks[i] = track;
		}

		soundOption = PlayerPrefs.GetInt(SoundOptionKey, 0);
		SetSoundOption();

		soundButton.onClick.AddListener(ChangeSoundOption);
	}

	private void OnDestroy()
	{
		soundButton.onClick.RemoveListener(ChangeSoundOption);
	}
	#endregion

	#region Public Methods
	public static void PlaySound(string audioName, int trackIndex)
	{
		if (Instance.tracks.Length <= 0)
		{
			Debug.LogError("No tracks available");
			return;
		}

		if (trackIndex >= Instance.tracks.Length)
		{
			Debug.LogError("Track index of '" + trackIndex + "' not valid.");
			return;
		}

		AudioClip sfxClip = Resources.Load<AudioClip>("Sounds/" + audioName);
		AudioSource audioSource = Instance.tracks[trackIndex];

		if (sfxClip != null)
		{
			audioSource.clip = sfxClip;
			audioSource.Play();
		}
		else
		{
			Debug.LogError("Sound '" + audioName + "' not found.");
		}
	}
	#endregion

	#region Private Methods
	private void InitializeSingleton()
	{
		if (Instance == null)
		{
			Instance = this;

			if (dontDestroyOnLoad)
				DontDestroyOnLoad(gameObject);
		}
		else if (Instance != this)
		{
			Destroy(Instance.gameObject);
			Instance = this;

			if (dontDestroyOnLoad)
				DontDestroyOnLoad(gameObject);
		}
	}

	private void ChangeSoundOption()
	{
		soundOption++;
		if (soundOption > 2)
			soundOption = 0;

		PlayerPrefs.SetInt(SoundOptionKey, soundOption);

		SetSoundOption();
	}

	private void SetSoundOption()
	{
		void DisableAll()
		{
			soundHigh.SetActive(false);
			soundLow.SetActive(false);
			soundOff.SetActive(false);
		}

		switch (soundOption)
		{
			case 0:
				DisableAll();
				soundHigh.SetActive(true);

				foreach (var track in tracks)
					track.volume = 1;

				break;
			case 1:
				DisableAll();
				soundLow.SetActive(true);

				foreach (var track in tracks)
					track.volume = .5f;

				break;
			case 2:
				DisableAll();
				soundOff.SetActive(true);

				foreach (var track in tracks)
					track.volume = 0;

				break;
		}
	}
	#endregion
}
