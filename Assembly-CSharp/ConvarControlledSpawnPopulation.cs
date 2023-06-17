using System;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000564 RID: 1380
[CreateAssetMenu(menuName = "Rust/Convar Controlled Spawn Population")]
public class ConvarControlledSpawnPopulation : SpawnPopulation
{
	// Token: 0x04002278 RID: 8824
	[Header("Convars")]
	public string PopulationConvar;

	// Token: 0x04002279 RID: 8825
	private ConsoleSystem.Command _command;

	// Token: 0x1700038E RID: 910
	// (get) Token: 0x06002A5F RID: 10847 RVA: 0x00102715 File Offset: 0x00100915
	protected ConsoleSystem.Command Command
	{
		get
		{
			if (this._command == null)
			{
				this._command = ConsoleSystem.Index.Server.Find(this.PopulationConvar);
				Assert.IsNotNull<ConsoleSystem.Command>(this._command, string.Format("{0} has missing convar {1}", this, this.PopulationConvar));
			}
			return this._command;
		}
	}

	// Token: 0x1700038F RID: 911
	// (get) Token: 0x06002A60 RID: 10848 RVA: 0x00102752 File Offset: 0x00100952
	public override float TargetDensity
	{
		get
		{
			return this.Command.AsFloat;
		}
	}
}
