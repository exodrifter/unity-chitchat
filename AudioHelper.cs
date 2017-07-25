using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.ChitChat
{
	[AddComponentMenu("Chit Chat/Audio Helper")]
	public class AudioHelper : MonoBehaviour
	{
		private List<AudioSource> loops;

		public void Play(AudioClip clip, float volume, float pitch)
		{
			var source = GetNewSource();
			source.clip = clip;
			source.volume = volume;
			source.pitch = pitch;
			source.loop = false;

			source.Play();
			Destroy(source.gameObject, clip.length + 0.1f);
		}

		public void Loop(AudioClip clip, float volume, float pitch)
		{
			var source = GetNewSource();
			source.clip = clip;
			source.volume = volume;
			source.pitch = pitch;
			source.loop = true;

			loops = loops ?? new List<AudioSource>();
			loops.Add(source);

			source.Play();
		}

		public void Loop(AudioClip clip, float fadeInTime, float volume, float pitch)
		{
			var source = GetNewSource();
			source.clip = clip;
			source.volume = 0;
			source.pitch = pitch;
			source.loop = true;

			loops = loops ?? new List<AudioSource>();
			loops.Add(source);

			source.Play();

			StartCoroutine(TweenVolume(source, fadeInTime, volume));
		}

		public void StopLoops(float time)
		{
			StopAllCoroutines();

			if (loops != null)
			{
				foreach (var source in loops)
				{
					StartCoroutine(TweenVolume(source, time, 0));
				}
			}
		}

		private IEnumerator TweenVolume(AudioSource source, float time, float volume)
		{
			var a = source.volume;
			var b = volume;

			var elapsed = 0f;
			while(elapsed < time)
			{
				yield return new WaitForEndOfFrame();

				elapsed += Time.deltaTime;

				var t = elapsed / time;
				source.volume = Mathf.Lerp(a, b, t);
			}

			source.volume = b;
		}

		private AudioSource GetNewSource()
		{
			var go = new GameObject("Sound");
			go.transform.parent = transform;

			return go.AddComponent<AudioSource>();
		}
	}
}
