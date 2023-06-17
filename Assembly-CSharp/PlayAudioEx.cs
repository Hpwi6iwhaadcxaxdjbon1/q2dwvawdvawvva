using System;
using UnityEngine;

// Token: 0x020002D4 RID: 724
public class PlayAudioEx : MonoBehaviour
{
	// Token: 0x040016CD RID: 5837
	public float delay;

	// Token: 0x06001DAB RID: 7595 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x06001DAC RID: 7596 RVA: 0x000CB67C File Offset: 0x000C987C
	private void OnEnable()
	{
		AudioSource component = base.GetComponent<AudioSource>();
		if (component)
		{
			component.PlayDelayed(this.delay);
		}
	}
}
