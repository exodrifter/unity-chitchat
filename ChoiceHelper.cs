using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Exodrifter.ChitChat
{
	[AddComponentMenu("")]
	public class ChoiceHelper : MonoBehaviour
	{
		public GameObject Prefab
		{
			get { return prefab; }
			set { prefab = value; }
		}
		[SerializeField]
		private GameObject prefab;

		private List<GameObject> pool = new List<GameObject>();

		private Font font;

		public void SetFont(Font font)
		{
			this.font = font;
			foreach (var item in pool)
			{
				item.GetComponentInChildren<Text>().font = font;
			}
		}

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
				go.GetComponentInChildren<Text>().font = font;
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
