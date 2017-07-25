using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Exodrifter.ChitChat
{
	[AddComponentMenu("Chit Chat/Choice Helper")]
	public class ChoiceHelper : MonoBehaviour
	{
		[SerializeField]
		private GameObject prefab;

		private List<GameObject> pool = new List<GameObject>();

		public void Show(ChitChat chat, List<string> choices)
		{
			foreach (var item in pool)
			{
				Destroy(item);
			}

			for (var i = 0; i < choices.Count; ++i)
			{
				var choice = choices[i];
				var index = i;

				var go = Instantiate(prefab);
				go.GetComponentInChildren<Text>().text = choice;
				go.GetComponentInChildren<Button>().onClick.AddListener(
					() => { chat.Choose(index); }
				);

				go.transform.parent = prefab.transform.parent;
				go.name = "Choice " + i;
				go.SetActive(true);
			}
		}
	}
}
