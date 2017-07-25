using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exodrifter.ChitChat
{
	[AddComponentMenu("Chit Chat/Chit Chat")]
	public class ChitChat : MonoBehaviour
	{
		[SerializeField]
		private DialogHelper dialog;
		[SerializeField]
		private ChoiceHelper choice;
		[SerializeField]
		private new AudioHelper audio;

		private Rumor.Engine.Rumor rumor;
		private bool exiting;

		private static ChitChat instance;
		private const string CHIT_CHAT_SCENE = "ChitChatScene";

		void Awake()
		{
			instance = this;
		}

		void OnDestroy()
		{
			instance = null;
		}

		void Update()
		{
			if (rumor == null || rumor.Equals(null))
			{
				if (!exiting)
				{
					StartCoroutine(Exit());
				}
				return;
			}

			if (rumor.Finished)
			{
				if (!exiting)
				{
					StartCoroutine(Exit());
				}

				return;
			}

			if (rumor.Choosing)
			{
				choice.Show(this, rumor.State.Choices);
			}

			if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
			{
				rumor.Advance();
			}

			rumor.Update(Time.deltaTime);
		}

		private IEnumerator Exit()
		{
			exiting = true;

			try
			{
				Destroy(dialog.gameObject);
				Destroy(choice.gameObject);

				audio.StopLoops(2);

				yield return new WaitForSeconds(2.1f);

				Destroy(this.gameObject);
			}
			finally
			{
				SceneManager.UnloadSceneAsync(CHIT_CHAT_SCENE);
			}
		}

		#region Rumor

		private void StartRumor(string script, Rumor.Engine.Scope scope)
		{
			var compiler = new Rumor.Lang.RumorCompiler();
			var nodes = compiler.Compile(script);

			rumor = new Rumor.Engine.Rumor(nodes, scope);

			rumor.Bind<string, float>("fade", Fade);
			rumor.Bind<string, float, float, float>("fade", Fade);
			rumor.Bind<string>("hit", Hit);
			rumor.Bind<string>("loop", Loop);
			rumor.Bind<string, float, float>("loop", Loop);
			rumor.Bind<string>("play", Play);
			rumor.Bind<string, float, float>("play", Play);

			rumor.Scope.DefaultSpeaker = "Narrator";

			StartCoroutine(rumor.Run());
		}

		private void Fade(string filename, float fadeInTime)
		{
			var clip = Resources.Load<AudioClip>(filename);
			audio.Loop(clip, fadeInTime, 1, 1);
		}

		private void Fade(string filename, float fadeInTime, float volume, float pitch)
		{
			var clip = Resources.Load<AudioClip>(filename);
			audio.Loop(clip, fadeInTime, volume, pitch);
		}

		private void Hit(string filename)
		{
			var clip = Resources.Load<AudioClip>(filename);
			audio.Play(clip, Random.Range(0.8f, 1f), Random.Range(0.8f, 1.2f));
		}

		private void Loop(string filename)
		{
			var clip = Resources.Load<AudioClip>(filename);
			audio.Loop(clip, 1, 1);
		}

		private void Loop(string filename, float volume, float pitch)
		{
			var clip = Resources.Load<AudioClip>(filename);
			audio.Loop(clip, volume, pitch);
		}

		private void Play(string filename)
		{
			var clip = Resources.Load<AudioClip>(filename);
			audio.Play(clip, 1, 1);
		}

		private void Play(string filename, float volume, float pitch)
		{
			var clip = Resources.Load<AudioClip>(filename);
			audio.Play(clip, volume, pitch);
		}

		internal void Choose(int index)
		{
			rumor.Choose(index);
		}

		#endregion

		#region Static

		static void Start(string script, Rumor.Engine.Scope scope)
		{
			if (instance == null || instance.Equals(null))
			{
				SceneManager.LoadScene(CHIT_CHAT_SCENE, LoadSceneMode.Additive);
				ChitChat.instance.StartRumor(script, scope);
			}
			else
			{
				throw new System.InvalidOperationException("Scene is already loaded!");
			}
		}

		#endregion
	}
}
