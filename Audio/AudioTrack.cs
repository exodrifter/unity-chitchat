using System.Collections;
using UnityEngine;

public class AudioTrack : MonoBehaviour
{
	#region Properties

	public AudioClip clip
	{
		get { return source.clip; }
		set { source.clip = value; }
	}

	public float volume
	{
		get { return source.volume; }
		set { source.volume = value; }
	}

	public float pitch
	{
		get { return source.pitch; }
		set { source.pitch = value; }
	}

	public bool loop
	{
		get { return source.loop; }
		set { source.loop = value; }
	}

	#endregion

	#region Monobehaviour

	private AudioSource source;

	private void Awake()
	{
		source = gameObject.AddComponent<AudioSource>();
	}

	#endregion

	public void Play()
	{
		source.Play();

		StopAllCoroutines();
		if (!source.loop)
		{
			StartCoroutine(Destroy(source.clip.length + 0.1f));
		}
	}

	public void FadeIn(float seconds)
	{
		StopAllCoroutines();
		StartCoroutine(FadeInternal(seconds, 1, false));
	}

	public void FadeOut(float seconds)
	{
		StopAllCoroutines();
		StartCoroutine(FadeInternal(seconds, 0, true));
	}

	private IEnumerator FadeInternal(float seconds, float b, bool destroy)
	{
		var a = source.volume;

		var elapsed = 0f;
		while (elapsed < seconds)
		{
			var t = elapsed / seconds;
			source.volume = Mathf.Lerp(a, b, t);

			yield return new WaitForEndOfFrame();

			elapsed += Time.deltaTime;
		}

		source.volume = b;

		yield return new WaitForEndOfFrame();

		if (destroy)
		{
			Destroy(gameObject);
		}
	}

	private IEnumerator Destroy(float seconds = 0)
	{
		if (seconds > 0)
		{
			yield return new WaitForSeconds(seconds);
		}

		Destroy(gameObject);
	}
}
