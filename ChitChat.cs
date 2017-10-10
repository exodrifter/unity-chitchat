using System;
using System.Collections;
using System.Collections.Generic;
using Exodrifter.Anchor;
using Exodrifter.Rumor.Lang;
using Exodrifter.Rumor.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exodrifter.ChitChat
{
	[AddComponentMenu("Chit Chat/Chit Chat")]
	public class ChitChat : MonoBehaviour
	{
		public DialogHelper Dialog
		{
			get { return dialog; }
		}
		[SerializeField]
		private DialogHelper dialog;

		public ChoiceHelper Choice
		{
			get { return choice; }
		}
		[SerializeField]
		private ChoiceHelper choice;

		public AudioHelper Audio
		{
			get { return audio; }
		}
		[SerializeField]
		private new AudioHelper audio;

		public Rumor.Engine.Rumor Rumor
		{
			get { return rumor; }
		}
		private Rumor.Engine.Rumor rumor;

		public Font DialogFont { get; set; }
		public Font ChoiceFont { get; set; }

		private bool exiting;

		void Update()
		{
			if (Util.IsNull(rumor))
			{
				choice.Show(this, new List<string>());
				dialog.Show(new LazyDictionary<object, string>());
				return;
			}

			if (rumor.Finished)
			{
				choice.Show(this, new List<string>());
				dialog.Show(new LazyDictionary<object, string>());
				return;
			}

			choice.Show(this, rumor.Choosing ? rumor.State.Choices : new List<string>());

			if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
			{
				rumor.Advance();
			}

			rumor.Update(Time.deltaTime);

			dialog.Show(rumor.State.Dialog);
		}

		#region Rumor

		public void Run()
		{
			if (!Util.IsNull(rumor))
			{
				rumor.Finish();
				StopAllCoroutines();
			}

			StartCoroutine(rumor.Run());
		}

		private void Init(string script, Rumor.Engine.Scope scope)
		{
			var compiler = new RumorCompiler();
			var nodes = compiler.Compile(script);

			rumor = new Rumor.Engine.Rumor(nodes, scope);

			rumor.Bind<string>("hit", Hit);
			rumor.Bind<string>("loop", Loop);
			rumor.Bind<string, float>("loop", Loop);
			rumor.Bind<string, float, float, float>("loop", Loop);
			rumor.Bind<string>("stop", Stop);
			rumor.Bind<string, float>("stop", Stop);
			rumor.Bind<string>("play", Play);
			rumor.Bind<string, float, float>("play", Play);
		}

		private void Hit(string filename)
		{
			var volume = UnityEngine.Random.Range(0.8f, 1f);
			var pitch = UnityEngine.Random.Range(0.8f, 1.2f);
			audio.Play(filename, volume, pitch);
		}

		private void Loop(string filename)
		{
			audio.Loop(filename, 0, 1, 1);
		}

		private void Loop(string filename, float fadein)
		{
			audio.Loop(filename, fadein, 1, 1);
		}

		private void Loop
			(string filename, float fadein, float volume, float pitch)
		{
			audio.Loop(filename, fadein, volume, pitch);
		}

		private void Stop(string filename)
		{
			audio.Stop(filename, 0);
		}

		private void Stop(string filename, float fadeOut)
		{
			audio.Stop(filename, fadeOut);
		}

		private void Play(string filename)
		{
			audio.Play(filename, 1, 1);
		}

		private void Play(string filename, float volume, float pitch)
		{
			audio.Play(filename, volume, pitch);
		}

		internal void Choose(int index)
		{
			rumor.Choose(index);
		}

		#endregion

		#region Static

		public static ChitChat Initialize(string script, Rumor.Engine.Scope scope = null)
		{
			var go = GameObject.FindGameObjectWithTag("ChitChat");
			if (go == null || go.Equals(null))
			{
				throw new InvalidOperationException(
					"No GameObject in scene with the tag ChitChat."
				);
			}

			var chitchat = go.GetComponent<ChitChat>();
			if (chitchat == null || chitchat.Equals(null))
			{
				throw new InvalidOperationException(
					"GameObject is missing ChitChat component."
				);
			}

			chitchat.Init(script, scope);
			return chitchat;
		}

		#endregion
	}
}
