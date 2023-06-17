using System;
using UnityEngine;

// Token: 0x020003A3 RID: 931
public class PreloadedCassetteContent : ScriptableObject
{
	// Token: 0x040019C1 RID: 6593
	public SoundDefinition[] ShortTapeContent;

	// Token: 0x040019C2 RID: 6594
	public SoundDefinition[] MediumTapeContent;

	// Token: 0x040019C3 RID: 6595
	public SoundDefinition[] LongTapeContent;

	// Token: 0x060020CB RID: 8395 RVA: 0x000D7F90 File Offset: 0x000D6190
	public SoundDefinition GetSoundContent(int index, PreloadedCassetteContent.PreloadType type)
	{
		switch (type)
		{
		case PreloadedCassetteContent.PreloadType.Short:
			return this.GetDefinition(index, this.ShortTapeContent);
		case PreloadedCassetteContent.PreloadType.Medium:
			return this.GetDefinition(index, this.MediumTapeContent);
		case PreloadedCassetteContent.PreloadType.Long:
			return this.GetDefinition(index, this.LongTapeContent);
		default:
			return null;
		}
	}

	// Token: 0x060020CC RID: 8396 RVA: 0x000D7FDC File Offset: 0x000D61DC
	private SoundDefinition GetDefinition(int index, SoundDefinition[] array)
	{
		index = Mathf.Clamp(index, 0, array.Length);
		return array[index];
	}

	// Token: 0x060020CD RID: 8397 RVA: 0x000D7FF0 File Offset: 0x000D61F0
	public SoundDefinition GetSoundContent(uint id)
	{
		foreach (SoundDefinition soundDefinition in this.ShortTapeContent)
		{
			if (StringPool.Get(soundDefinition.name) == id)
			{
				return soundDefinition;
			}
		}
		foreach (SoundDefinition soundDefinition2 in this.MediumTapeContent)
		{
			if (StringPool.Get(soundDefinition2.name) == id)
			{
				return soundDefinition2;
			}
		}
		foreach (SoundDefinition soundDefinition3 in this.LongTapeContent)
		{
			if (StringPool.Get(soundDefinition3.name) == id)
			{
				return soundDefinition3;
			}
		}
		return null;
	}

	// Token: 0x02000CB6 RID: 3254
	public enum PreloadType
	{
		// Token: 0x04004493 RID: 17555
		Short,
		// Token: 0x04004494 RID: 17556
		Medium,
		// Token: 0x04004495 RID: 17557
		Long
	}
}
