using System;
using System.Diagnostics;
using Facepunch;
using Rust;
using TMPro;
using UnityEngine;

// Token: 0x020007E0 RID: 2016
public class ErrorText : MonoBehaviour
{
	// Token: 0x04002D16 RID: 11542
	public TextMeshProUGUI text;

	// Token: 0x04002D17 RID: 11543
	public int maxLength = 1024;

	// Token: 0x04002D18 RID: 11544
	private Stopwatch stopwatch;

	// Token: 0x06003561 RID: 13665 RVA: 0x00146AF0 File Offset: 0x00144CF0
	public void OnEnable()
	{
		Output.OnMessage += this.CaptureLog;
	}

	// Token: 0x06003562 RID: 13666 RVA: 0x00146B03 File Offset: 0x00144D03
	public void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		Output.OnMessage -= this.CaptureLog;
	}

	// Token: 0x06003563 RID: 13667 RVA: 0x00146B20 File Offset: 0x00144D20
	internal void CaptureLog(string error, string stacktrace, LogType type)
	{
		if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert)
		{
			return;
		}
		if (this.text == null)
		{
			return;
		}
		TextMeshProUGUI textMeshProUGUI = this.text;
		textMeshProUGUI.text = string.Concat(new string[]
		{
			textMeshProUGUI.text,
			error,
			"\n",
			stacktrace,
			"\n\n"
		});
		if (this.text.text.Length > this.maxLength)
		{
			this.text.text = this.text.text.Substring(this.text.text.Length - this.maxLength, this.maxLength);
		}
		this.stopwatch = Stopwatch.StartNew();
	}

	// Token: 0x06003564 RID: 13668 RVA: 0x00146BDC File Offset: 0x00144DDC
	protected void Update()
	{
		if (this.stopwatch != null && this.stopwatch.Elapsed.TotalSeconds > 30.0)
		{
			this.text.text = string.Empty;
			this.stopwatch = null;
		}
	}
}
