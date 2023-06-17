using System;

// Token: 0x0200018F RID: 399
public class SnowMachine : FogMachine
{
	// Token: 0x040010CF RID: 4303
	public AdaptMeshToTerrain snowMesh;

	// Token: 0x040010D0 RID: 4304
	public TriggerTemperature tempTrigger;

	// Token: 0x060017E4 RID: 6116 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool MotionModeEnabled()
	{
		return false;
	}

	// Token: 0x060017E5 RID: 6117 RVA: 0x000B4169 File Offset: 0x000B2369
	public override void EnableFogField()
	{
		base.EnableFogField();
		this.tempTrigger.gameObject.SetActive(true);
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x000B4182 File Offset: 0x000B2382
	public override void FinishFogging()
	{
		base.FinishFogging();
		this.tempTrigger.gameObject.SetActive(false);
	}
}
