using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class HitchTrough : StorageContainer
{
	// Token: 0x04000E74 RID: 3700
	public HitchTrough.HitchSpot[] hitchSpots;

	// Token: 0x04000E75 RID: 3701
	public float caloriesToDecaySeconds = 36f;

	// Token: 0x0600160E RID: 5646 RVA: 0x000ACA00 File Offset: 0x000AAC00
	public global::Item GetFoodItem()
	{
		foreach (global::Item item in base.inventory.itemList)
		{
			if (item.info.category == ItemCategory.Food && item.info.GetComponent<ItemModConsumable>())
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x0600160F RID: 5647 RVA: 0x000ACA78 File Offset: 0x000AAC78
	public bool ValidHitchPosition(Vector3 pos)
	{
		return this.GetClosest(pos, false, 1f) != null;
	}

	// Token: 0x06001610 RID: 5648 RVA: 0x000ACA8C File Offset: 0x000AAC8C
	public bool HasSpace()
	{
		HitchTrough.HitchSpot[] array = this.hitchSpots;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsOccupied(true))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001611 RID: 5649 RVA: 0x000ACABC File Offset: 0x000AACBC
	public HitchTrough.HitchSpot GetClosest(Vector3 testPos, bool includeOccupied = false, float maxRadius = -1f)
	{
		float num = 10000f;
		HitchTrough.HitchSpot result = null;
		for (int i = 0; i < this.hitchSpots.Length; i++)
		{
			float num2 = Vector3.Distance(testPos, this.hitchSpots[i].spot.position);
			if (num2 < num && (maxRadius == -1f || num2 <= maxRadius) && (includeOccupied || !this.hitchSpots[i].IsOccupied(true)))
			{
				num = num2;
				result = this.hitchSpots[i];
			}
		}
		return result;
	}

	// Token: 0x06001612 RID: 5650 RVA: 0x000ACB2C File Offset: 0x000AAD2C
	public void Unhitch(RidableHorse horse)
	{
		foreach (HitchTrough.HitchSpot hitchSpot in this.hitchSpots)
		{
			if (hitchSpot.GetHorse(base.isServer) == horse)
			{
				hitchSpot.SetOccupiedBy(null);
				horse.SetHitch(null);
			}
		}
	}

	// Token: 0x06001613 RID: 5651 RVA: 0x000ACB74 File Offset: 0x000AAD74
	public int NumHitched()
	{
		int num = 0;
		HitchTrough.HitchSpot[] array = this.hitchSpots;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsOccupied(true))
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001614 RID: 5652 RVA: 0x000ACBA8 File Offset: 0x000AADA8
	public bool AttemptToHitch(RidableHorse horse, HitchTrough.HitchSpot hitch = null)
	{
		if (horse == null)
		{
			return false;
		}
		if (hitch == null)
		{
			hitch = this.GetClosest(horse.transform.position, false, -1f);
		}
		if (hitch != null)
		{
			hitch.SetOccupiedBy(horse);
			horse.SetHitch(this);
			horse.transform.SetPositionAndRotation(hitch.spot.position, hitch.spot.rotation);
			horse.DismountAllPlayers();
			return true;
		}
		return false;
	}

	// Token: 0x06001615 RID: 5653 RVA: 0x000ACC18 File Offset: 0x000AAE18
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericEntRef1 = this.hitchSpots[0].horse.uid;
		info.msg.ioEntity.genericEntRef2 = this.hitchSpots[1].horse.uid;
	}

	// Token: 0x06001616 RID: 5654 RVA: 0x000ACC80 File Offset: 0x000AAE80
	public override void PostServerLoad()
	{
		foreach (HitchTrough.HitchSpot hitchSpot in this.hitchSpots)
		{
			this.AttemptToHitch(hitchSpot.GetHorse(true), hitchSpot);
		}
	}

	// Token: 0x06001617 RID: 5655 RVA: 0x000ACCB8 File Offset: 0x000AAEB8
	public void UnhitchAll()
	{
		HitchTrough.HitchSpot[] array = this.hitchSpots;
		for (int i = 0; i < array.Length; i++)
		{
			RidableHorse horse = array[i].GetHorse(true);
			if (horse)
			{
				this.Unhitch(horse);
			}
		}
	}

	// Token: 0x06001618 RID: 5656 RVA: 0x000ACCF3 File Offset: 0x000AAEF3
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			this.UnhitchAll();
		}
		base.DestroyShared();
	}

	// Token: 0x06001619 RID: 5657 RVA: 0x000ACD09 File Offset: 0x000AAF09
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
	}

	// Token: 0x0600161A RID: 5658 RVA: 0x000ACD14 File Offset: 0x000AAF14
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.hitchSpots[0].horse.uid = info.msg.ioEntity.genericEntRef1;
			this.hitchSpots[1].horse.uid = info.msg.ioEntity.genericEntRef2;
		}
	}

	// Token: 0x02000C21 RID: 3105
	[Serializable]
	public class HitchSpot
	{
		// Token: 0x0400422D RID: 16941
		public HitchTrough owner;

		// Token: 0x0400422E RID: 16942
		public Transform spot;

		// Token: 0x0400422F RID: 16943
		public EntityRef horse;

		// Token: 0x06004E07 RID: 19975 RVA: 0x001A1E1D File Offset: 0x001A001D
		public RidableHorse GetHorse(bool isServer = true)
		{
			return this.horse.Get(isServer) as RidableHorse;
		}

		// Token: 0x06004E08 RID: 19976 RVA: 0x001A1E30 File Offset: 0x001A0030
		public bool IsOccupied(bool isServer = true)
		{
			return this.horse.IsValid(isServer);
		}

		// Token: 0x06004E09 RID: 19977 RVA: 0x001A1E3E File Offset: 0x001A003E
		public void SetOccupiedBy(RidableHorse newHorse)
		{
			this.horse.Set(newHorse);
		}
	}
}
