using System;
using UnityEngine;

// Token: 0x02000367 RID: 871
public class AIMemoryBank<T>
{
	// Token: 0x0400191E RID: 6430
	private MemoryBankType type;

	// Token: 0x0400191F RID: 6431
	private T[] slots;

	// Token: 0x04001920 RID: 6432
	private float[] slotSetTimestamps;

	// Token: 0x04001921 RID: 6433
	private int slotCount;

	// Token: 0x06001F8C RID: 8076 RVA: 0x000D4FAE File Offset: 0x000D31AE
	public AIMemoryBank(MemoryBankType type, int slots)
	{
		this.Init(type, slots);
	}

	// Token: 0x06001F8D RID: 8077 RVA: 0x000D4FBE File Offset: 0x000D31BE
	public void Init(MemoryBankType type, int slots)
	{
		this.type = type;
		this.slotCount = slots;
		this.slots = new T[this.slotCount];
		this.slotSetTimestamps = new float[this.slotCount];
	}

	// Token: 0x06001F8E RID: 8078 RVA: 0x000D4FF0 File Offset: 0x000D31F0
	public void Set(T item, int index)
	{
		if (index < 0 || index >= this.slotCount)
		{
			return;
		}
		this.slots[index] = item;
		this.slotSetTimestamps[index] = Time.realtimeSinceStartup;
	}

	// Token: 0x06001F8F RID: 8079 RVA: 0x000D501C File Offset: 0x000D321C
	public T Get(int index)
	{
		if (index < 0 || index >= this.slotCount)
		{
			return default(T);
		}
		return this.slots[index];
	}

	// Token: 0x06001F90 RID: 8080 RVA: 0x000D504C File Offset: 0x000D324C
	public float GetTimeSinceSet(int index)
	{
		if (index < 0 || index >= this.slotCount)
		{
			return 0f;
		}
		return Time.realtimeSinceStartup - this.slotSetTimestamps[index];
	}

	// Token: 0x06001F91 RID: 8081 RVA: 0x000D5070 File Offset: 0x000D3270
	public void Remove(int index)
	{
		if (index < 0 || index >= this.slotCount)
		{
			return;
		}
		this.slots[index] = default(T);
	}

	// Token: 0x06001F92 RID: 8082 RVA: 0x000D50A0 File Offset: 0x000D32A0
	public void Clear()
	{
		for (int i = 0; i < 4; i++)
		{
			this.Remove(i);
		}
	}
}
