using System;
using UnityEngine;

// Token: 0x0200023A RID: 570
public class ShoutcastStreamer : MonoBehaviour, IClientComponent
{
	// Token: 0x04001474 RID: 5236
	public string Host = "http://listen.57fm.com:80/rcxmas";

	// Token: 0x04001475 RID: 5237
	public AudioSource Source;
}
