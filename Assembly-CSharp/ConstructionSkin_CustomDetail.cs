using System;
using UnityEngine;

// Token: 0x02000257 RID: 599
public class ConstructionSkin_CustomDetail : ConstructionSkin
{
	// Token: 0x0400150A RID: 5386
	public ConstructionSkin_ColourLookup ColourLookup;

	// Token: 0x06001C4C RID: 7244 RVA: 0x000C53E4 File Offset: 0x000C35E4
	public override uint GetStartingDetailColour(uint playerColourIndex)
	{
		if (playerColourIndex > 0U)
		{
			return (uint)Mathf.Clamp(playerColourIndex, 1f, (float)(this.ColourLookup.AllColours.Length + 1));
		}
		return (uint)UnityEngine.Random.Range(1, this.ColourLookup.AllColours.Length + 1);
	}
}
