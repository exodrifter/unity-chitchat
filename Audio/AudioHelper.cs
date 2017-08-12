using UnityEngine;

namespace Exodrifter.ChitChat
{
	[AddComponentMenu("")]
	public class AudioHelper : MonoBehaviour
	{
		public void Play(string filename, float volume, float pitch)
		{
			var clip = GetClip(filename);

			var track = GetTrack(filename);
			track.clip = clip;
			track.volume = volume;
			track.pitch = pitch;
			track.loop = false;

			track.Play();
		}

		public void Loop
			(string filename, float fadeIn, float volume, float pitch)
		{
			var track = GetTrack(filename);
			track.pitch = pitch;
			track.loop = true;

			if (track.clip == null || track.clip.Equals(null))
			{
				track.volume = 0;
				track.clip = GetClip(filename);
			}

			track.Play();
			track.FadeIn(fadeIn, volume);
		}

		public void Stop(string filename, float fadeOut)
		{
			var track = GetTrack(filename);

			if (track.clip != null && !track.clip.Equals(null))
			{
				track.FadeOut(fadeOut);
			}
		}

		private AudioTrack GetTrack(string name)
		{
			var length = transform.childCount;
			for (int i = 0; i < length; ++i)
			{
				var child = transform.GetChild(i);

				if (child.name == name)
				{
					var track = child.GetComponent<AudioTrack>();
					if (track != null && !track.Equals(null))
					{
						return track;
					}
				}
			}

			var go = new GameObject(name);
			go.transform.parent = transform;
			return go.AddComponent<AudioTrack>();
		}

		private AudioClip GetClip(string filename)
		{
			return Resources.Load<AudioClip>(filename);
		}

		public void Destroy(float fadeOut)
		{
			var length = transform.childCount;
			for (int i = 0; i < length; ++i)
			{
				var child = transform.GetChild(i);

				var track = child.GetComponent<AudioTrack>();
				if (track != null && !track.Equals(null))
				{
					track.FadeOut(fadeOut);
				}
			}

			Destroy(gameObject, fadeOut + 0.1f);
		}
	}
}
