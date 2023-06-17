using System;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008BC RID: 2236
public class TextEntryCookie : MonoBehaviour
{
	// Token: 0x17000466 RID: 1126
	// (get) Token: 0x06003736 RID: 14134 RVA: 0x0014CC18 File Offset: 0x0014AE18
	public InputField control
	{
		get
		{
			return base.GetComponent<InputField>();
		}
	}

	// Token: 0x06003737 RID: 14135 RVA: 0x0014CC20 File Offset: 0x0014AE20
	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString("TextEntryCookie_" + base.name);
		if (!string.IsNullOrEmpty(@string))
		{
			this.control.text = @string;
		}
		this.control.onValueChanged.Invoke(this.control.text);
	}

	// Token: 0x06003738 RID: 14136 RVA: 0x0014CC72 File Offset: 0x0014AE72
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		PlayerPrefs.SetString("TextEntryCookie_" + base.name, this.control.text);
	}
}
