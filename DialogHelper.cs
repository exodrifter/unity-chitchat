using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Exodrifter.ChitChat
{
	[AddComponentMenu("Chit Chat/Dialog Helper")]
	public class DialogHelper : MonoBehaviour
	{
		[SerializeField]
		private Text text;

		private void Show(string text)
		{
			this.text.text = text;
		}
	}
}
