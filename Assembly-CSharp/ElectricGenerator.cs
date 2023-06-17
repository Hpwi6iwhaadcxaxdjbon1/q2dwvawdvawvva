using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020004C8 RID: 1224
public class ElectricGenerator : global::IOEntity
{
	// Token: 0x0400203B RID: 8251
	public float electricAmount = 8f;

	// Token: 0x060027E9 RID: 10217 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x060027EA RID: 10218 RVA: 0x000F8213 File Offset: 0x000F6413
	public override int MaximalPowerOutput()
	{
		return Mathf.FloorToInt(this.electricAmount);
	}

	// Token: 0x060027EB RID: 10219 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x060027EC RID: 10220 RVA: 0x000F8220 File Offset: 0x000F6420
	public override int GetCurrentEnergy()
	{
		return (int)this.electricAmount;
	}

	// Token: 0x060027ED RID: 10221 RVA: 0x000F8229 File Offset: 0x000F6429
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return this.GetCurrentEnergy();
	}

	// Token: 0x060027EE RID: 10222 RVA: 0x000F8234 File Offset: 0x000F6434
	public override void UpdateOutputs()
	{
		this.currentEnergy = this.GetCurrentEnergy();
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				ioslot.connectedTo.Get(true).UpdateFromInput(this.currentEnergy, ioslot.connectedToSlot);
			}
		}
	}

	// Token: 0x060027EF RID: 10223 RVA: 0x00057F25 File Offset: 0x00056125
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x060027F0 RID: 10224 RVA: 0x000F8297 File Offset: 0x000F6497
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.Invoke(new Action(this.ForcePuzzleReset), 1f);
	}

	// Token: 0x060027F1 RID: 10225 RVA: 0x000F82B8 File Offset: 0x000F64B8
	private void ForcePuzzleReset()
	{
		global::PuzzleReset component = base.GetComponent<global::PuzzleReset>();
		if (component != null)
		{
			component.DoReset();
			component.ResetTimer();
		}
	}

	// Token: 0x060027F2 RID: 10226 RVA: 0x000F82E4 File Offset: 0x000F64E4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			global::PuzzleReset component = base.GetComponent<global::PuzzleReset>();
			if (component)
			{
				info.msg.puzzleReset = Pool.Get<ProtoBuf.PuzzleReset>();
				info.msg.puzzleReset.playerBlocksReset = component.playersBlockReset;
				if (component.playerDetectionOrigin != null)
				{
					info.msg.puzzleReset.playerDetectionOrigin = component.playerDetectionOrigin.position;
				}
				info.msg.puzzleReset.playerDetectionRadius = component.playerDetectionRadius;
				info.msg.puzzleReset.scaleWithServerPopulation = component.scaleWithServerPopulation;
				info.msg.puzzleReset.timeBetweenResets = component.timeBetweenResets;
			}
		}
	}

	// Token: 0x060027F3 RID: 10227 RVA: 0x000F83A8 File Offset: 0x000F65A8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.puzzleReset != null)
		{
			global::PuzzleReset component = base.GetComponent<global::PuzzleReset>();
			if (component != null)
			{
				component.playersBlockReset = info.msg.puzzleReset.playerBlocksReset;
				if (component.playerDetectionOrigin != null)
				{
					component.playerDetectionOrigin.position = info.msg.puzzleReset.playerDetectionOrigin;
				}
				component.playerDetectionRadius = info.msg.puzzleReset.playerDetectionRadius;
				component.scaleWithServerPopulation = info.msg.puzzleReset.scaleWithServerPopulation;
				component.timeBetweenResets = info.msg.puzzleReset.timeBetweenResets;
				component.ResetTimer();
			}
		}
	}
}
