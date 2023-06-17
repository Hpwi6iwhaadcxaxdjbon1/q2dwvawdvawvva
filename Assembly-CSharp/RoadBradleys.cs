using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004E2 RID: 1250
public class RoadBradleys : TriggeredEvent
{
	// Token: 0x040020A1 RID: 8353
	public List<BradleyAPC> spawnedAPCs = new List<BradleyAPC>();

	// Token: 0x06002871 RID: 10353 RVA: 0x000F9CD5 File Offset: 0x000F7ED5
	public int GetNumBradleys()
	{
		this.CleanList();
		return this.spawnedAPCs.Count;
	}

	// Token: 0x06002872 RID: 10354 RVA: 0x000F9CE8 File Offset: 0x000F7EE8
	public int GetDesiredNumber()
	{
		return Mathf.CeilToInt(World.Size / 1000f) * 2;
	}

	// Token: 0x06002873 RID: 10355 RVA: 0x000F9D00 File Offset: 0x000F7F00
	private void CleanList()
	{
		for (int i = this.spawnedAPCs.Count - 1; i >= 0; i--)
		{
			if (this.spawnedAPCs[i] == null)
			{
				this.spawnedAPCs.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002874 RID: 10356 RVA: 0x000F9D48 File Offset: 0x000F7F48
	private void RunEvent()
	{
		int numBradleys = this.GetNumBradleys();
		int num = this.GetDesiredNumber() - numBradleys;
		if (num <= 0)
		{
			return;
		}
		if (TerrainMeta.Path == null || TerrainMeta.Path.Roads.Count == 0)
		{
			return;
		}
		Debug.Log("Spawning :" + num + "Bradleys");
		for (int i = 0; i < num; i++)
		{
			Vector3 vector = Vector3.zero;
			PathList pathList = TerrainMeta.Path.Roads[UnityEngine.Random.Range(0, TerrainMeta.Path.Roads.Count)];
			vector = pathList.Path.Points[UnityEngine.Random.Range(0, pathList.Path.Points.Length)];
			BradleyAPC bradleyAPC = BradleyAPC.SpawnRoadDrivingBradley(vector, Quaternion.identity);
			if (bradleyAPC)
			{
				this.spawnedAPCs.Add(bradleyAPC);
			}
			else
			{
				Debug.Log("Failed to spawn bradley at: " + vector);
			}
		}
	}
}
