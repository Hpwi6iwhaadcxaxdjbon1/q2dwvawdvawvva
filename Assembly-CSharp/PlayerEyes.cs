using System;
using UnityEngine;

// Token: 0x02000445 RID: 1093
public class PlayerEyes : EntityComponent<BasePlayer>
{
	// Token: 0x04001CAB RID: 7339
	public static readonly Vector3 EyeOffset = new Vector3(0f, 1.5f, 0f);

	// Token: 0x04001CAC RID: 7340
	public static readonly Vector3 DuckOffset = new Vector3(0f, -0.6f, 0f);

	// Token: 0x04001CAD RID: 7341
	public static readonly Vector3 CrawlOffset = new Vector3(0f, -1.15f, 0.175f);

	// Token: 0x04001CAE RID: 7342
	public Vector3 thirdPersonSleepingOffset = new Vector3(0.43f, 1.25f, 0.7f);

	// Token: 0x04001CAF RID: 7343
	public LazyAimProperties defaultLazyAim;

	// Token: 0x04001CB0 RID: 7344
	private Vector3 viewOffset = Vector3.zero;

	// Token: 0x170002FF RID: 767
	// (get) Token: 0x06002481 RID: 9345 RVA: 0x000E82B0 File Offset: 0x000E64B0
	public Vector3 worldMountedPosition
	{
		get
		{
			if (base.baseEntity && base.baseEntity.isMounted)
			{
				Vector3 vector = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity, this.GetLookRotation());
				if (vector != Vector3.zero)
				{
					return vector;
				}
			}
			return this.worldStandingPosition;
		}
	}

	// Token: 0x17000300 RID: 768
	// (get) Token: 0x06002482 RID: 9346 RVA: 0x000E8309 File Offset: 0x000E6509
	public Vector3 worldStandingPosition
	{
		get
		{
			return base.transform.position + PlayerEyes.EyeOffset;
		}
	}

	// Token: 0x17000301 RID: 769
	// (get) Token: 0x06002483 RID: 9347 RVA: 0x000E8320 File Offset: 0x000E6520
	public Vector3 worldCrouchedPosition
	{
		get
		{
			return this.worldStandingPosition + PlayerEyes.DuckOffset;
		}
	}

	// Token: 0x17000302 RID: 770
	// (get) Token: 0x06002484 RID: 9348 RVA: 0x000E8332 File Offset: 0x000E6532
	public Vector3 worldCrawlingPosition
	{
		get
		{
			return this.worldStandingPosition + PlayerEyes.CrawlOffset;
		}
	}

	// Token: 0x17000303 RID: 771
	// (get) Token: 0x06002485 RID: 9349 RVA: 0x000E8344 File Offset: 0x000E6544
	public Vector3 position
	{
		get
		{
			if (!base.baseEntity || !base.baseEntity.isMounted)
			{
				return base.transform.position + base.transform.rotation * (PlayerEyes.EyeOffset + this.viewOffset) + this.BodyLeanOffset;
			}
			Vector3 vector = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity, this.GetLookRotation());
			if (vector != Vector3.zero)
			{
				return vector;
			}
			return base.transform.position + base.transform.up * (PlayerEyes.EyeOffset.y + this.viewOffset.y) + this.BodyLeanOffset;
		}
	}

	// Token: 0x17000304 RID: 772
	// (get) Token: 0x06002486 RID: 9350 RVA: 0x0002BE49 File Offset: 0x0002A049
	private Vector3 BodyLeanOffset
	{
		get
		{
			return Vector3.zero;
		}
	}

	// Token: 0x17000305 RID: 773
	// (get) Token: 0x06002487 RID: 9351 RVA: 0x000E8414 File Offset: 0x000E6614
	public Vector3 center
	{
		get
		{
			if (base.baseEntity && base.baseEntity.isMounted)
			{
				Vector3 vector = base.baseEntity.GetMounted().EyeCenterForPlayer(base.baseEntity, this.GetLookRotation());
				if (vector != Vector3.zero)
				{
					return vector;
				}
			}
			return base.transform.position + base.transform.up * (PlayerEyes.EyeOffset.y + PlayerEyes.DuckOffset.y);
		}
	}

	// Token: 0x17000306 RID: 774
	// (get) Token: 0x06002488 RID: 9352 RVA: 0x000E849C File Offset: 0x000E669C
	public Vector3 offset
	{
		get
		{
			return base.transform.up * (PlayerEyes.EyeOffset.y + this.viewOffset.y);
		}
	}

	// Token: 0x17000307 RID: 775
	// (get) Token: 0x06002489 RID: 9353 RVA: 0x000E84C4 File Offset: 0x000E66C4
	// (set) Token: 0x0600248A RID: 9354 RVA: 0x000E84D7 File Offset: 0x000E66D7
	public Quaternion rotation
	{
		get
		{
			return this.parentRotation * this.bodyRotation;
		}
		set
		{
			this.bodyRotation = Quaternion.Inverse(this.parentRotation) * value;
		}
	}

	// Token: 0x17000308 RID: 776
	// (get) Token: 0x0600248B RID: 9355 RVA: 0x000E84F0 File Offset: 0x000E66F0
	// (set) Token: 0x0600248C RID: 9356 RVA: 0x000E84F8 File Offset: 0x000E66F8
	public Quaternion bodyRotation { get; set; }

	// Token: 0x17000309 RID: 777
	// (get) Token: 0x0600248D RID: 9357 RVA: 0x000E8504 File Offset: 0x000E6704
	public Quaternion parentRotation
	{
		get
		{
			if (base.baseEntity.isMounted || !(base.transform.parent != null))
			{
				return Quaternion.identity;
			}
			return Quaternion.Euler(0f, base.transform.parent.rotation.eulerAngles.y, 0f);
		}
	}

	// Token: 0x0600248E RID: 9358 RVA: 0x000E8564 File Offset: 0x000E6764
	public void NetworkUpdate(Quaternion rot)
	{
		if (base.baseEntity.IsCrawling())
		{
			this.viewOffset = PlayerEyes.CrawlOffset;
		}
		else if (base.baseEntity.IsDucked())
		{
			this.viewOffset = PlayerEyes.DuckOffset;
		}
		else
		{
			this.viewOffset = Vector3.zero;
		}
		this.bodyRotation = rot;
	}

	// Token: 0x0600248F RID: 9359 RVA: 0x000E85B8 File Offset: 0x000E67B8
	public Vector3 MovementForward()
	{
		return Quaternion.Euler(new Vector3(0f, this.rotation.eulerAngles.y, 0f)) * Vector3.forward;
	}

	// Token: 0x06002490 RID: 9360 RVA: 0x000E85F8 File Offset: 0x000E67F8
	public Vector3 MovementRight()
	{
		return Quaternion.Euler(new Vector3(0f, this.rotation.eulerAngles.y, 0f)) * Vector3.right;
	}

	// Token: 0x06002491 RID: 9361 RVA: 0x000E8636 File Offset: 0x000E6836
	public Ray BodyRay()
	{
		return new Ray(this.position, this.BodyForward());
	}

	// Token: 0x06002492 RID: 9362 RVA: 0x000E8649 File Offset: 0x000E6849
	public Vector3 BodyForward()
	{
		return this.rotation * Vector3.forward;
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x000E865B File Offset: 0x000E685B
	public Vector3 BodyRight()
	{
		return this.rotation * Vector3.right;
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x000E866D File Offset: 0x000E686D
	public Vector3 BodyUp()
	{
		return this.rotation * Vector3.up;
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x000E867F File Offset: 0x000E687F
	public Ray HeadRay()
	{
		return new Ray(this.position, this.HeadForward());
	}

	// Token: 0x06002496 RID: 9366 RVA: 0x000E8692 File Offset: 0x000E6892
	public Vector3 HeadForward()
	{
		return this.GetLookRotation() * Vector3.forward;
	}

	// Token: 0x06002497 RID: 9367 RVA: 0x000E86A4 File Offset: 0x000E68A4
	public Vector3 HeadRight()
	{
		return this.GetLookRotation() * Vector3.right;
	}

	// Token: 0x06002498 RID: 9368 RVA: 0x000E86B6 File Offset: 0x000E68B6
	public Vector3 HeadUp()
	{
		return this.GetLookRotation() * Vector3.up;
	}

	// Token: 0x06002499 RID: 9369 RVA: 0x000E86C8 File Offset: 0x000E68C8
	public Quaternion GetLookRotation()
	{
		return this.rotation;
	}

	// Token: 0x0600249A RID: 9370 RVA: 0x000E86C8 File Offset: 0x000E68C8
	public Quaternion GetAimRotation()
	{
		return this.rotation;
	}
}
