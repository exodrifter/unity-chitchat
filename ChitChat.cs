using System.Collections;
using System.Collections.Generic;
using Exodrifter.Rumor.Lang;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exodrifter.ChitChat
{
	[AddComponentMenu("")]
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

		void Start()
		{
			StartCoroutine(rumor.Run());
		}

		void Update()
		{
			dialog.SetFont(DialogFont);
			choice.SetFont(ChoiceFont);

			if (rumor == null || rumor.Equals(null))
			{
				Exit();
				return;
			}

			if (rumor.Finished)
			{
				Exit();
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

			foreach (var key in rumor.State.Dialog.Keys)
			{
				var str = "";
				if (key is string && !string.IsNullOrEmpty(key as string))
				{
					str += string.Format("<b>{0}</b>\n", key);
				}
				dialog.Show(str + rumor.State.Dialog[key]);
			}
		}

		public void Exit()
		{
			if (!exiting)
			{
				exiting = true;
				StartCoroutine(ExitInternal());
			}
		}

		private IEnumerator ExitInternal()
		{
			try
			{
				Destroy(dialog.gameObject);
				Destroy(choice.gameObject);

				audio.Destroy(2);

				yield return new WaitForSeconds(2.1f);

			}
			finally
			{
				Destroy(gameObject);
			}
		}

		#region Rumor

		private void StartRumor(string script, Rumor.Engine.Scope scope)
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

			rumor.Scope.DefaultSpeaker = "Narrator";
		}

		private void Hit(string filename)
		{
			audio.Play(filename, Random.Range(0.8f, 1f), Random.Range(0.8f, 1.2f));
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

		public static ChitChat Start(string script, Rumor.Engine.Scope scope = null)
		{
			if (string.IsNullOrEmpty(script))
			{
				throw new System.ArgumentException("Script cannot be null or empty", "script");
			}
			scope = scope ?? new Rumor.Engine.Scope();

			var go = new GameObject("ChitChat");

			go.AddComponent<RectTransform>();
			var canvas = go.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			var canvasScaler = go.AddComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			canvasScaler.referenceResolution = new Vector2(800, 600);
			canvasScaler.matchWidthOrHeight = 0.5f;
			go.AddComponent<GraphicRaycaster>();
			var chitchat = go.AddComponent<ChitChat>();

			var dialogBox = new GameObject("Dialog Box");
			dialogBox.transform.parent = go.transform;
			dialogBox.transform.localScale = Vector3.one;
			var xform = dialogBox.AddComponent<RectTransform>();
			xform.pivot = new Vector2(0.5f, 0);
			xform.anchorMin = new Vector2(0, 0);
			xform.anchorMax = new Vector2(1, 0);
			xform.anchoredPosition = new Vector2(0, 0);
			xform.sizeDelta = new Vector2(0, 200);
			xform.localScale = Vector3.one;
			dialogBox.AddComponent<Image>();
			chitchat.dialog = dialogBox.AddComponent<DialogHelper>();

			var dialogText = new GameObject("Text");
			dialogText.transform.parent = dialogBox.transform;
			dialogText.transform.localScale = Vector3.one;
			xform = dialogText.AddComponent<RectTransform>();
			xform.pivot = new Vector2(0.5f, 0.5f);
			xform.anchorMin = new Vector2(0, 0);
			xform.anchorMax = new Vector2(1, 1);
			xform.anchoredPosition = new Vector2(0, 0);
			xform.sizeDelta = new Vector2(-40, -40);
			var text = dialogText.AddComponent<Text>();
			text.fontSize = 20;
			text.color = new Color32(50, 50, 50, 255);
			text.supportRichText = true;
			chitchat.dialog.Text = text;

			var choice = new GameObject("Choices");
			choice.transform.parent = go.transform;
			choice.transform.localScale = Vector3.one;
			xform = choice.AddComponent<RectTransform>();
			xform.pivot = new Vector2(0.5f, 0.5f);
			xform.anchorMin = new Vector2(0, 0);
			xform.anchorMax = new Vector2(1, 1);
			xform.anchoredPosition = new Vector2(0, 100);
			xform.sizeDelta = new Vector2(-400, -200);
			var vlayout = choice.AddComponent<VerticalLayoutGroup>();
			vlayout.padding = new RectOffset(0, 0, 50, 50);
			vlayout.spacing = 20;
			vlayout.childAlignment = TextAnchor.MiddleCenter;
			vlayout.childControlWidth = true;
			vlayout.childControlHeight = false;
			vlayout.childForceExpandWidth = true;
			vlayout.childForceExpandHeight = false;
			chitchat.choice = choice.AddComponent<ChoiceHelper>();

			var prefab = new GameObject("Choice Prefab");
			prefab.transform.parent = choice.transform;
			prefab.transform.localScale = Vector3.one;
			prefab.AddComponent<RectTransform>();
			prefab.AddComponent<Image>();
			prefab.AddComponent<Button>();
			var hlayout = prefab.AddComponent<HorizontalLayoutGroup>();
			hlayout.padding = new RectOffset(5, 5, 5, 5);
			hlayout.childAlignment = TextAnchor.MiddleCenter;
			hlayout.childControlWidth = true;
			hlayout.childControlHeight = true;
			hlayout.childForceExpandWidth = true;
			hlayout.childForceExpandHeight = true;
			var fitter = prefab.AddComponent<ContentSizeFitter>();
			fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
			fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			prefab.SetActive(false);
			chitchat.choice.Prefab = prefab;

			var choiceText = new GameObject("Choice Text");
			choiceText.transform.parent = prefab.transform;
			choiceText.transform.localScale = Vector3.one;
			choiceText.AddComponent<RectTransform>();
			text = choiceText.AddComponent<Text>();
			text.fontSize = 20;
			text.color = new Color32(50, 50, 50, 255);
			text.alignment = TextAnchor.MiddleCenter;
			text.horizontalOverflow = HorizontalWrapMode.Wrap;
			text.verticalOverflow = VerticalWrapMode.Truncate;
			text.supportRichText = true;

			var audio = new GameObject("Audio");
			audio.transform.parent = go.transform;
			audio.transform.localScale = Vector3.one;
			audio.transform.localPosition = Vector3.zero;
			chitchat.audio = audio.AddComponent<AudioHelper>();

			chitchat.StartRumor(script, scope);
			return chitchat;
		}

		#endregion
	}
}
