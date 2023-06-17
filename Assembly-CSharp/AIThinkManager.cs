using System;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public class AIThinkManager : BaseMonoBehaviour, IServerComponent
{
	// Token: 0x0400129C RID: 4764
	public static ListHashSet<IThinker> _processQueue = new ListHashSet<IThinker>(8);

	// Token: 0x0400129D RID: 4765
	public static ListHashSet<IThinker> _removalQueue = new ListHashSet<IThinker>(8);

	// Token: 0x0400129E RID: 4766
	public static ListHashSet<IThinker> _animalProcessQueue = new ListHashSet<IThinker>(8);

	// Token: 0x0400129F RID: 4767
	public static ListHashSet<IThinker> _animalremovalQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040012A0 RID: 4768
	public static ListHashSet<IThinker> _petProcessQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040012A1 RID: 4769
	public static ListHashSet<IThinker> _petRemovalQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040012A2 RID: 4770
	[ServerVar]
	[Help("How many miliseconds to budget for processing AI entities per server frame")]
	public static float framebudgetms = 2.5f;

	// Token: 0x040012A3 RID: 4771
	[ServerVar]
	[Help("How many miliseconds to budget for processing animal AI entities per server frame")]
	public static float animalframebudgetms = 2.5f;

	// Token: 0x040012A4 RID: 4772
	[ServerVar]
	[Help("How many miliseconds to budget for processing pet AI entities per server frame")]
	public static float petframebudgetms = 1f;

	// Token: 0x040012A5 RID: 4773
	private static int lastIndex = 0;

	// Token: 0x040012A6 RID: 4774
	private static int lastAnimalIndex = 0;

	// Token: 0x040012A7 RID: 4775
	private static int lastPetIndex;

	// Token: 0x06001A01 RID: 6657 RVA: 0x000BCE98 File Offset: 0x000BB098
	public static void ProcessQueue(AIThinkManager.QueueType queueType)
	{
		if (queueType != AIThinkManager.QueueType.Human)
		{
		}
		if (queueType == AIThinkManager.QueueType.Human)
		{
			AIThinkManager.DoRemoval(AIThinkManager._removalQueue, AIThinkManager._processQueue);
			AIInformationZone.BudgetedTick();
		}
		else if (queueType == AIThinkManager.QueueType.Pets)
		{
			AIThinkManager.DoRemoval(AIThinkManager._petRemovalQueue, AIThinkManager._petProcessQueue);
		}
		else
		{
			AIThinkManager.DoRemoval(AIThinkManager._animalremovalQueue, AIThinkManager._animalProcessQueue);
		}
		if (queueType == AIThinkManager.QueueType.Human)
		{
			AIThinkManager.DoProcessing(AIThinkManager._processQueue, AIThinkManager.framebudgetms / 1000f, ref AIThinkManager.lastIndex);
			return;
		}
		if (queueType == AIThinkManager.QueueType.Pets)
		{
			AIThinkManager.DoProcessing(AIThinkManager._petProcessQueue, AIThinkManager.petframebudgetms / 1000f, ref AIThinkManager.lastPetIndex);
			return;
		}
		AIThinkManager.DoProcessing(AIThinkManager._animalProcessQueue, AIThinkManager.animalframebudgetms / 1000f, ref AIThinkManager.lastAnimalIndex);
	}

	// Token: 0x06001A02 RID: 6658 RVA: 0x000BCF44 File Offset: 0x000BB144
	private static void DoRemoval(ListHashSet<IThinker> removal, ListHashSet<IThinker> process)
	{
		if (removal.Count > 0)
		{
			foreach (IThinker val in removal)
			{
				process.Remove(val);
			}
			removal.Clear();
		}
	}

	// Token: 0x06001A03 RID: 6659 RVA: 0x000BCFA4 File Offset: 0x000BB1A4
	private static void DoProcessing(ListHashSet<IThinker> process, float budgetSeconds, ref int last)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (last < process.Count && Time.realtimeSinceStartup < realtimeSinceStartup + budgetSeconds)
		{
			IThinker thinker = process[last];
			if (thinker != null)
			{
				try
				{
					thinker.TryThink();
				}
				catch (Exception message)
				{
					Debug.LogWarning(message);
				}
			}
			last++;
		}
		if (last >= process.Count)
		{
			last = 0;
		}
	}

	// Token: 0x06001A04 RID: 6660 RVA: 0x000BD00C File Offset: 0x000BB20C
	public static void Add(IThinker toAdd)
	{
		AIThinkManager._processQueue.Add(toAdd);
	}

	// Token: 0x06001A05 RID: 6661 RVA: 0x000BD019 File Offset: 0x000BB219
	public static void Remove(IThinker toRemove)
	{
		AIThinkManager._removalQueue.Add(toRemove);
	}

	// Token: 0x06001A06 RID: 6662 RVA: 0x000BD026 File Offset: 0x000BB226
	public static void AddAnimal(IThinker toAdd)
	{
		AIThinkManager._animalProcessQueue.Add(toAdd);
	}

	// Token: 0x06001A07 RID: 6663 RVA: 0x000BD033 File Offset: 0x000BB233
	public static void RemoveAnimal(IThinker toRemove)
	{
		AIThinkManager._animalremovalQueue.Add(toRemove);
	}

	// Token: 0x06001A08 RID: 6664 RVA: 0x000BD040 File Offset: 0x000BB240
	public static void AddPet(IThinker toAdd)
	{
		AIThinkManager._petProcessQueue.Add(toAdd);
	}

	// Token: 0x06001A09 RID: 6665 RVA: 0x000BD04D File Offset: 0x000BB24D
	public static void RemovePet(IThinker toRemove)
	{
		AIThinkManager._petRemovalQueue.Add(toRemove);
	}

	// Token: 0x02000C4B RID: 3147
	public enum QueueType
	{
		// Token: 0x040042B4 RID: 17076
		Human,
		// Token: 0x040042B5 RID: 17077
		Animal,
		// Token: 0x040042B6 RID: 17078
		Pets
	}
}
