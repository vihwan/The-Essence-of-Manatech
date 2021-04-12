using UnityEngine;

public enum Clip { Select, Swap, Clear };

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance;
	private AudioSource[] sfx;

	// Use this for initialization
	void Start()
	{
		instance = GetComponent<SoundManager>();
		sfx = GetComponents<AudioSource>();
	}

	public void PlaySFX(Clip audioClip)
	{
		sfx[(int)audioClip].Play();
	}
}
