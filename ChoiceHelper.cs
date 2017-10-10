using Exodrifter.Anchor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Exodrifter.ChitChat
{
	[AddComponentMenu("Chit Chat/Choice Helper")]
	public class ChoiceHelper : MonoBehaviour
	{
		[SerializeField]
		private Pool pool;

		public void Show(ChitChat chat, List<string> choices)
		{
			pool.DespawnAll();

			for (var i = 0; i < choices.Count; ++i)
			{
				var choice = choices[i];
				var index = i;

				var go = pool.Spawn();
				go.GetComponentInChildren<Text>().text = choice;
				go.GetComponentInChildren<Button>().onClick.AddListener(
					() => { chat.Choose(index); }
				);

				go.name = "Choice " + i;
				go.SetActive(true);
			}
		}
	}
}
