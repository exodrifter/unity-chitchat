using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Exodrifter.ChitChat
{
	[AddComponentMenu("Chit Chat/Dialog Item")]
	public class DialogItem : MonoBehaviour
	{
		public string NameTag
		{
			get { return nameTag.text; }
			set { nameTag.text = value; }
		}
		[SerializeField]
		private Text nameTag;

		public string Dialog
		{
			get { return dialog.text; }
			set { dialog.text = value; }
		}
		[SerializeField]
		private Text dialog;
	}
}
