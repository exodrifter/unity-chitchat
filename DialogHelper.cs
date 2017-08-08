using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Exodrifter.ChitChat
{
	[AddComponentMenu("")]
	public class DialogHelper : MonoBehaviour
	{
		public Text Text
		{
			get { return text; }
			set { text = value; }
		}
		[SerializeField]
		private Text text;

		public void SetFont(Font font)
		{
			text.font = font;
		}

		public void Show(string text)
		{
			this.text.text = text;
		}
	}
}
