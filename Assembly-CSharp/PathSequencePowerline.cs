using System;
using System.Collections.Generic;

// Token: 0x02000681 RID: 1665
public class PathSequencePowerline : PathSequence
{
	// Token: 0x0400272D RID: 10029
	public PathSequencePowerline.SequenceRule Rule;

	// Token: 0x0400272E RID: 10030
	private const int RegularPowerlineSpacing = 2;

	// Token: 0x06002FB6 RID: 12214 RVA: 0x0011EB4C File Offset: 0x0011CD4C
	public override void ApplySequenceReplacement(List<Prefab> sequence, ref Prefab replacement, Prefab[] possibleReplacements, int pathLength, int pathIndex)
	{
		bool flag = false;
		if (this.Rule == PathSequencePowerline.SequenceRule.Powerline)
		{
			if (pathLength >= 3)
			{
				flag = (sequence.Count == 0 || pathIndex == pathLength - 1);
				if (!flag)
				{
					flag = (this.GetIndexCountToRule(sequence, PathSequencePowerline.SequenceRule.PowerlinePlatform) >= 2);
				}
			}
		}
		else if (this.Rule == PathSequencePowerline.SequenceRule.PowerlinePlatform)
		{
			flag = (pathLength < 3);
			if (!flag)
			{
				int indexCountToRule = this.GetIndexCountToRule(sequence, PathSequencePowerline.SequenceRule.PowerlinePlatform);
				flag = (indexCountToRule < 2 && indexCountToRule != sequence.Count && pathIndex < pathLength - 1);
			}
		}
		if (flag)
		{
			Prefab prefabOfType = this.GetPrefabOfType(possibleReplacements, (this.Rule == PathSequencePowerline.SequenceRule.PowerlinePlatform) ? PathSequencePowerline.SequenceRule.Powerline : PathSequencePowerline.SequenceRule.PowerlinePlatform);
			if (prefabOfType != null)
			{
				replacement = prefabOfType;
			}
		}
	}

	// Token: 0x06002FB7 RID: 12215 RVA: 0x0011EBE4 File Offset: 0x0011CDE4
	private Prefab GetPrefabOfType(Prefab[] options, PathSequencePowerline.SequenceRule ruleToFind)
	{
		for (int i = 0; i < options.Length; i++)
		{
			PathSequencePowerline pathSequencePowerline = options[i].Attribute.Find<PathSequence>(options[i].ID) as PathSequencePowerline;
			if (pathSequencePowerline == null || pathSequencePowerline.Rule == ruleToFind)
			{
				return options[i];
			}
		}
		return null;
	}

	// Token: 0x06002FB8 RID: 12216 RVA: 0x0011EC34 File Offset: 0x0011CE34
	private int GetIndexCountToRule(List<Prefab> sequence, PathSequencePowerline.SequenceRule rule)
	{
		int num = 0;
		for (int i = sequence.Count - 1; i >= 0; i--)
		{
			PathSequencePowerline pathSequencePowerline = sequence[i].Attribute.Find<PathSequence>(sequence[i].ID) as PathSequencePowerline;
			if (pathSequencePowerline != null)
			{
				if (pathSequencePowerline.Rule == rule)
				{
					break;
				}
				num++;
			}
		}
		return num;
	}

	// Token: 0x02000DB7 RID: 3511
	public enum SequenceRule
	{
		// Token: 0x040048C3 RID: 18627
		PowerlinePlatform,
		// Token: 0x040048C4 RID: 18628
		Powerline
	}
}
