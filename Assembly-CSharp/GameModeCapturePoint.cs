using System;
using Facepunch;
using ProtoBuf;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200050F RID: 1295
public class GameModeCapturePoint : global::BaseEntity
{
	// Token: 0x04002157 RID: 8535
	public CapturePointTrigger captureTrigger;

	// Token: 0x04002158 RID: 8536
	public float timeToCapture = 3f;

	// Token: 0x04002159 RID: 8537
	public int scorePerSecond = 1;

	// Token: 0x0400215A RID: 8538
	public string scoreName = "score";

	// Token: 0x0400215B RID: 8539
	private float captureFraction;

	// Token: 0x0400215C RID: 8540
	private int captureTeam = -1;

	// Token: 0x0400215D RID: 8541
	private int capturingTeam = -1;

	// Token: 0x0400215E RID: 8542
	public EntityRef capturingPlayer;

	// Token: 0x0400215F RID: 8543
	public EntityRef capturedPlayer;

	// Token: 0x04002160 RID: 8544
	public const global::BaseEntity.Flags Flag_Contested = global::BaseEntity.Flags.Busy;

	// Token: 0x04002161 RID: 8545
	public RustText capturePointText;

	// Token: 0x04002162 RID: 8546
	public RustText captureOwnerName;

	// Token: 0x04002163 RID: 8547
	public Image captureProgressImage;

	// Token: 0x04002164 RID: 8548
	public GameObjectRef progressBeepEffect;

	// Token: 0x04002165 RID: 8549
	public GameObjectRef progressCompleteEffect;

	// Token: 0x04002166 RID: 8550
	public Transform computerPoint;

	// Token: 0x04002167 RID: 8551
	private float nextBeepTime;

	// Token: 0x0600295A RID: 10586 RVA: 0x0002A535 File Offset: 0x00028735
	public bool IsContested()
	{
		return base.HasFlag(global::BaseEntity.Flags.Busy);
	}

	// Token: 0x0600295B RID: 10587 RVA: 0x000FD85B File Offset: 0x000FBA5B
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.AssignPoints), 0f, 1f);
	}

	// Token: 0x0600295C RID: 10588 RVA: 0x000FD87F File Offset: 0x000FBA7F
	public void Update()
	{
		if (base.isClient)
		{
			return;
		}
		this.UpdateCaptureAmount();
	}

	// Token: 0x0600295D RID: 10589 RVA: 0x000FD890 File Offset: 0x000FBA90
	public void AssignPoints()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode == null)
		{
			return;
		}
		if (!activeGameMode.IsMatchActive())
		{
			return;
		}
		if (activeGameMode.IsTeamGame())
		{
			if (this.captureTeam != -1 && this.captureFraction == 1f)
			{
				activeGameMode.ModifyTeamScore(this.captureTeam, this.scorePerSecond);
				return;
			}
		}
		else if (this.capturedPlayer.IsValid(true))
		{
			activeGameMode.ModifyPlayerGameScore(this.capturedPlayer.Get(true).GetComponent<global::BasePlayer>(), "score", this.scorePerSecond);
		}
	}

	// Token: 0x0600295E RID: 10590 RVA: 0x000FD918 File Offset: 0x000FBB18
	public void DoCaptureEffect()
	{
		Effect.server.Run(this.progressCompleteEffect.resourcePath, this.computerPoint.position, default(Vector3), null, false);
	}

	// Token: 0x0600295F RID: 10591 RVA: 0x000FD94C File Offset: 0x000FBB4C
	public void DoProgressEffect()
	{
		if (Time.time < this.nextBeepTime)
		{
			return;
		}
		Effect.server.Run(this.progressBeepEffect.resourcePath, this.computerPoint.position, default(Vector3), null, false);
		this.nextBeepTime = Time.time + 0.5f;
	}

	// Token: 0x06002960 RID: 10592 RVA: 0x000FD9A0 File Offset: 0x000FBBA0
	public void UpdateCaptureAmount()
	{
		if (base.isClient)
		{
			return;
		}
		float num = this.captureFraction;
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode == null)
		{
			return;
		}
		if (this.captureTrigger.entityContents == null)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, false, false, false);
			return;
		}
		if (!activeGameMode.IsMatchActive())
		{
			return;
		}
		if (activeGameMode.IsTeamGame())
		{
			int[] array = new int[activeGameMode.GetNumTeams()];
			foreach (global::BaseEntity baseEntity in this.captureTrigger.entityContents)
			{
				if (!(baseEntity == null) && !baseEntity.isClient)
				{
					global::BasePlayer component = baseEntity.GetComponent<global::BasePlayer>();
					if (!(component == null) && component.IsAlive() && !component.IsNpc && component.gamemodeteam != -1)
					{
						array[component.gamemodeteam]++;
					}
				}
			}
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] > 0)
				{
					num2++;
				}
			}
			if (num2 < 2)
			{
				int num3 = -1;
				int num4 = 0;
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] > num4)
					{
						num4 = array[j];
						num3 = j;
					}
				}
				if (this.captureTeam == -1 && this.captureFraction == 0f)
				{
					this.capturingTeam = num3;
				}
				if (this.captureFraction > 0f && num3 != this.captureTeam && num3 != this.capturingTeam)
				{
					this.captureFraction = Mathf.Clamp01(this.captureFraction - Time.deltaTime / this.timeToCapture);
					if (this.captureFraction == 0f)
					{
						this.captureTeam = -1;
					}
				}
				else if (this.captureTeam == -1 && this.captureFraction < 1f && this.capturingTeam == num3)
				{
					this.DoProgressEffect();
					this.captureFraction = Mathf.Clamp01(this.captureFraction + Time.deltaTime / this.timeToCapture);
					if (this.captureFraction == 1f)
					{
						this.DoCaptureEffect();
						this.captureTeam = num3;
					}
				}
			}
			base.SetFlag(global::BaseEntity.Flags.Busy, num2 > 1, false, true);
		}
		else
		{
			if (!this.capturingPlayer.IsValid(true) && !this.capturedPlayer.IsValid(true))
			{
				this.captureFraction = 0f;
			}
			if (this.captureTrigger.entityContents.Count == 0)
			{
				this.capturingPlayer.Set(null);
			}
			if (this.captureTrigger.entityContents.Count == 1)
			{
				foreach (global::BaseEntity baseEntity2 in this.captureTrigger.entityContents)
				{
					global::BasePlayer component2 = baseEntity2.GetComponent<global::BasePlayer>();
					if (!(component2 == null))
					{
						if (!this.capturedPlayer.IsValid(true) && this.captureFraction == 0f)
						{
							this.capturingPlayer.Set(component2);
						}
						if (this.captureFraction > 0f && component2 != this.capturedPlayer.Get(true) && component2 != this.capturingPlayer.Get(true))
						{
							this.captureFraction = Mathf.Clamp01(this.captureFraction - Time.deltaTime / this.timeToCapture);
							if (this.captureFraction == 0f)
							{
								this.capturedPlayer.Set(null);
								break;
							}
							break;
						}
						else
						{
							if (this.capturedPlayer.Get(true) || this.captureFraction >= 1f || !(this.capturingPlayer.Get(true) == component2))
							{
								break;
							}
							this.DoProgressEffect();
							this.captureFraction = Mathf.Clamp01(this.captureFraction + Time.deltaTime / this.timeToCapture);
							if (this.captureFraction == 1f)
							{
								this.DoCaptureEffect();
								this.capturedPlayer.Set(component2);
								break;
							}
							break;
						}
					}
				}
			}
			base.SetFlag(global::BaseEntity.Flags.Busy, this.captureTrigger.entityContents.Count > 1, false, true);
		}
		if (num != this.captureFraction)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06002961 RID: 10593 RVA: 0x000FDDF4 File Offset: 0x000FBFF4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericFloat1 = this.captureFraction;
		info.msg.ioEntity.genericInt1 = this.captureTeam;
		info.msg.ioEntity.genericInt2 = this.capturingTeam;
		info.msg.ioEntity.genericEntRef1 = this.capturedPlayer.uid;
		info.msg.ioEntity.genericEntRef2 = this.capturingPlayer.uid;
	}
}
