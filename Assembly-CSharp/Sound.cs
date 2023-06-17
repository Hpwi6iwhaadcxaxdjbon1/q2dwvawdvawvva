using System;
using UnityEngine;

// Token: 0x0200023C RID: 572
public class Sound : MonoBehaviour, IClientComponent
{
	// Token: 0x04001485 RID: 5253
	public static float volumeExponent = Mathf.Log(Mathf.Sqrt(10f), 2f);

	// Token: 0x04001486 RID: 5254
	public SoundDefinition definition;

	// Token: 0x04001487 RID: 5255
	public SoundModifier[] modifiers;

	// Token: 0x04001488 RID: 5256
	public SoundSource soundSource;

	// Token: 0x04001489 RID: 5257
	public AudioSource[] audioSources = new AudioSource[2];

	// Token: 0x0400148A RID: 5258
	[SerializeField]
	private SoundFade _fade;

	// Token: 0x0400148B RID: 5259
	[SerializeField]
	private SoundModulation _modulation;

	// Token: 0x0400148C RID: 5260
	[SerializeField]
	private SoundOcclusion _occlusion;

	// Token: 0x1700025C RID: 604
	// (get) Token: 0x06001C08 RID: 7176 RVA: 0x000C4023 File Offset: 0x000C2223
	public SoundFade fade
	{
		get
		{
			return this._fade;
		}
	}

	// Token: 0x1700025D RID: 605
	// (get) Token: 0x06001C09 RID: 7177 RVA: 0x000C402B File Offset: 0x000C222B
	public SoundModulation modulation
	{
		get
		{
			return this._modulation;
		}
	}

	// Token: 0x1700025E RID: 606
	// (get) Token: 0x06001C0A RID: 7178 RVA: 0x000C4033 File Offset: 0x000C2233
	public SoundOcclusion occlusion
	{
		get
		{
			return this._occlusion;
		}
	}
}
