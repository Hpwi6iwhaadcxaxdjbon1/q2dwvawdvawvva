using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E2 RID: 2018
public class FPSText : MonoBehaviour
{
	// Token: 0x04002D29 RID: 11561
	public Text text;

	// Token: 0x04002D2A RID: 11562
	private Stopwatch fpsTimer = Stopwatch.StartNew();

	// Token: 0x06003567 RID: 13671 RVA: 0x00146C3C File Offset: 0x00144E3C
	protected void Update()
	{
		if (this.fpsTimer.Elapsed.TotalSeconds < 0.5)
		{
			return;
		}
		this.text.enabled = true;
		this.fpsTimer.Reset();
		this.fpsTimer.Start();
		string text = Performance.current.frameRate + " FPS";
		this.text.text = text;
	}
}
