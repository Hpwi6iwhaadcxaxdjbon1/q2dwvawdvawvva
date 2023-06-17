using System;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public class ScientistNPC : HumanNPC, IAIMounted
{
	// Token: 0x040012B5 RID: 4789
	public GameObjectRef[] RadioChatterEffects;

	// Token: 0x040012B6 RID: 4790
	public GameObjectRef[] DeathEffects;

	// Token: 0x040012B7 RID: 4791
	public string deathStatName = "kill_scientist";

	// Token: 0x040012B8 RID: 4792
	public Vector2 IdleChatterRepeatRange = new Vector2(10f, 15f);

	// Token: 0x040012B9 RID: 4793
	public ScientistNPC.RadioChatterType radioChatterType;

	// Token: 0x040012BA RID: 4794
	protected float lastAlertedTime = -100f;

	// Token: 0x06001A4A RID: 6730 RVA: 0x000BE028 File Offset: 0x000BC228
	public void SetChatterType(ScientistNPC.RadioChatterType newType)
	{
		if (newType == this.radioChatterType)
		{
			return;
		}
		if (newType == ScientistNPC.RadioChatterType.Idle)
		{
			this.QueueRadioChatter();
			return;
		}
		base.CancelInvoke(new Action(this.PlayRadioChatter));
	}

	// Token: 0x06001A4B RID: 6731 RVA: 0x000BE051 File Offset: 0x000BC251
	public override void ServerInit()
	{
		base.ServerInit();
		this.SetChatterType(ScientistNPC.RadioChatterType.Idle);
		base.InvokeRandomized(new Action(this.IdleCheck), 0f, 20f, 1f);
	}

	// Token: 0x06001A4C RID: 6732 RVA: 0x000BE081 File Offset: 0x000BC281
	public void IdleCheck()
	{
		if (Time.time > this.lastAlertedTime + 20f)
		{
			this.SetChatterType(ScientistNPC.RadioChatterType.Idle);
		}
	}

	// Token: 0x06001A4D RID: 6733 RVA: 0x000BE09D File Offset: 0x000BC29D
	public void QueueRadioChatter()
	{
		if (!this.IsAlive() || base.IsDestroyed)
		{
			return;
		}
		base.Invoke(new Action(this.PlayRadioChatter), UnityEngine.Random.Range(this.IdleChatterRepeatRange.x, this.IdleChatterRepeatRange.y));
	}

	// Token: 0x06001A4E RID: 6734 RVA: 0x000BE0DD File Offset: 0x000BC2DD
	public override bool ShotTest(float targetDist)
	{
		bool result = base.ShotTest(targetDist);
		if (Time.time - this.lastGunShotTime < 5f)
		{
			this.Alert();
		}
		return result;
	}

	// Token: 0x06001A4F RID: 6735 RVA: 0x000BE0FF File Offset: 0x000BC2FF
	public void Alert()
	{
		this.lastAlertedTime = Time.time;
		this.SetChatterType(ScientistNPC.RadioChatterType.Alert);
	}

	// Token: 0x06001A50 RID: 6736 RVA: 0x000BE113 File Offset: 0x000BC313
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		this.Alert();
	}

	// Token: 0x06001A51 RID: 6737 RVA: 0x000BE124 File Offset: 0x000BC324
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		this.SetChatterType(ScientistNPC.RadioChatterType.NONE);
		if (this.DeathEffects.Length != 0)
		{
			Effect.server.Run(this.DeathEffects[UnityEngine.Random.Range(0, this.DeathEffects.Length)].resourcePath, this.ServerPosition, Vector3.up, null, false);
		}
		if (info != null && info.InitiatorPlayer != null && !info.InitiatorPlayer.IsNpc)
		{
			info.InitiatorPlayer.stats.Add(this.deathStatName, 1, (Stats)5);
		}
	}

	// Token: 0x06001A52 RID: 6738 RVA: 0x000BE1AC File Offset: 0x000BC3AC
	public void PlayRadioChatter()
	{
		if (this.RadioChatterEffects.Length == 0)
		{
			return;
		}
		if (base.IsDestroyed || base.transform == null)
		{
			base.CancelInvoke(new Action(this.PlayRadioChatter));
			return;
		}
		Effect.server.Run(this.RadioChatterEffects[UnityEngine.Random.Range(0, this.RadioChatterEffects.Length)].resourcePath, this, StringPool.Get("head"), Vector3.zero, Vector3.zero, null, false);
		this.QueueRadioChatter();
	}

	// Token: 0x06001A53 RID: 6739 RVA: 0x000BE228 File Offset: 0x000BC428
	public override void EquipWeapon(bool skipDeployDelay = false)
	{
		base.EquipWeapon(skipDeployDelay);
		HeldEntity heldEntity = base.GetHeldEntity();
		if (heldEntity != null)
		{
			Item item = heldEntity.GetItem();
			if (item != null && item.contents != null)
			{
				if (UnityEngine.Random.Range(0, 3) == 0)
				{
					Item item2 = ItemManager.CreateByName("weapon.mod.flashlight", 1, 0UL);
					if (!item2.MoveToContainer(item.contents, -1, true, false, null, true))
					{
						item2.Remove(0f);
						return;
					}
					this.lightsOn = false;
					base.InvokeRandomized(new Action(base.LightCheck), 0f, 30f, 5f);
					base.LightCheck();
					return;
				}
				else
				{
					Item item3 = ItemManager.CreateByName("weapon.mod.lasersight", 1, 0UL);
					if (!item3.MoveToContainer(item.contents, -1, true, false, null, true))
					{
						item3.Remove(0f);
					}
					base.LightToggle(true);
					this.lightsOn = true;
				}
			}
		}
	}

	// Token: 0x06001A54 RID: 6740 RVA: 0x000BE306 File Offset: 0x000BC506
	public bool IsMounted()
	{
		return base.isMounted;
	}

	// Token: 0x06001A55 RID: 6741 RVA: 0x000BE30E File Offset: 0x000BC50E
	protected override string OverrideCorpseName()
	{
		return "Scientist";
	}

	// Token: 0x02000C58 RID: 3160
	public enum RadioChatterType
	{
		// Token: 0x040042C6 RID: 17094
		NONE,
		// Token: 0x040042C7 RID: 17095
		Idle,
		// Token: 0x040042C8 RID: 17096
		Alert
	}
}
