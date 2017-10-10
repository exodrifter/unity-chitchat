using Exodrifter.Anchor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Exodrifter.Rumor.Util;

namespace Exodrifter.ChitChat
{
	[AddComponentMenu("Chit Chat/Dialog Helper")]
	public class DialogHelper : MonoBehaviour
	{
		[SerializeField]
		private Pool pool;

		public void Show(LazyDictionary<object, string> dialog)
		{
			pool.DespawnAll();

			foreach (var key in dialog.Keys)
			{
				var i = pool.Spawn<DialogItem>();
				i.NameTag = key == null ? "" : key.ToString();
				i.Dialog = dialog[key];
			}
		}
	}
}
