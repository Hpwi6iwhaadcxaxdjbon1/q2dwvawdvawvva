using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000220 RID: 544
public class AmbienceEmitter : MonoBehaviour, IClientComponent, IComparable<AmbienceEmitter>
{
	// Token: 0x040013AA RID: 5034
	public AmbienceDefinitionList baseAmbience;

	// Token: 0x040013AB RID: 5035
	public AmbienceDefinitionList stings;

	// Token: 0x040013AC RID: 5036
	public bool isStatic = true;

	// Token: 0x040013AD RID: 5037
	public bool followCamera;

	// Token: 0x040013AE RID: 5038
	public bool isBaseEmitter;

	// Token: 0x040013AF RID: 5039
	public bool active;

	// Token: 0x040013B0 RID: 5040
	public float cameraDistanceSq = float.PositiveInfinity;

	// Token: 0x040013B1 RID: 5041
	public BoundingSphere boundingSphere;

	// Token: 0x040013B2 RID: 5042
	public float crossfadeTime = 2f;

	// Token: 0x040013B5 RID: 5045
	public Dictionary<AmbienceDefinition, float> nextStingTime = new Dictionary<AmbienceDefinition, float>();

	// Token: 0x040013B6 RID: 5046
	public float deactivateTime = float.PositiveInfinity;

	// Token: 0x040013B7 RID: 5047
	public bool playUnderwater = true;

	// Token: 0x040013B8 RID: 5048
	public bool playAbovewater = true;

	// Token: 0x17000255 RID: 597
	// (get) Token: 0x06001BB7 RID: 7095 RVA: 0x000C2F87 File Offset: 0x000C1187
	// (set) Token: 0x06001BB8 RID: 7096 RVA: 0x000C2F8F File Offset: 0x000C118F
	public TerrainTopology.Enum currentTopology { get; private set; }

	// Token: 0x17000256 RID: 598
	// (get) Token: 0x06001BB9 RID: 7097 RVA: 0x000C2F98 File Offset: 0x000C1198
	// (set) Token: 0x06001BBA RID: 7098 RVA: 0x000C2FA0 File Offset: 0x000C11A0
	public TerrainBiome.Enum currentBiome { get; private set; }

	// Token: 0x06001BBB RID: 7099 RVA: 0x000C2FA9 File Offset: 0x000C11A9
	public int CompareTo(AmbienceEmitter other)
	{
		return this.cameraDistanceSq.CompareTo(other.cameraDistanceSq);
	}
}
