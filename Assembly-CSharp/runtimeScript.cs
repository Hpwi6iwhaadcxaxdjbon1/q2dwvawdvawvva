﻿using System;
using EasyRoads3Dv3;
using UnityEngine;

// Token: 0x02000989 RID: 2441
public class runtimeScript : MonoBehaviour
{
	// Token: 0x04003468 RID: 13416
	public ERRoadNetwork roadNetwork;

	// Token: 0x04003469 RID: 13417
	public ERRoad road;

	// Token: 0x0400346A RID: 13418
	public GameObject go;

	// Token: 0x0400346B RID: 13419
	public int currentElement;

	// Token: 0x0400346C RID: 13420
	public float distance;

	// Token: 0x0400346D RID: 13421
	public float speed = 5f;

	// Token: 0x06003A1A RID: 14874 RVA: 0x00157DB0 File Offset: 0x00155FB0
	private void Start()
	{
		Debug.Log("Please read the comments at the top of the runtime script (/Assets/EasyRoads3D/Scripts/runtimeScript) before using the runtime API!");
		this.roadNetwork = new ERRoadNetwork();
		ERRoadType erroadType = new ERRoadType();
		erroadType.roadWidth = 6f;
		erroadType.roadMaterial = (Resources.Load("Materials/roads/road material") as Material);
		erroadType.layer = 1;
		erroadType.tag = "Untagged";
		Vector3[] markers = new Vector3[]
		{
			new Vector3(200f, 5f, 200f),
			new Vector3(250f, 5f, 200f),
			new Vector3(250f, 5f, 250f),
			new Vector3(300f, 5f, 250f)
		};
		this.road = this.roadNetwork.CreateRoad("road 1", erroadType, markers);
		this.road.AddMarker(new Vector3(300f, 5f, 300f));
		this.road.InsertMarker(new Vector3(275f, 5f, 235f));
		this.road.DeleteMarker(2);
		this.roadNetwork.BuildRoadNetwork();
		this.go = GameObject.CreatePrimitive(PrimitiveType.Cube);
	}

	// Token: 0x06003A1B RID: 14875 RVA: 0x00157EF8 File Offset: 0x001560F8
	private void Update()
	{
		if (this.roadNetwork != null)
		{
			float num = Time.deltaTime * this.speed;
			this.distance += num;
			Vector3 position = this.road.GetPosition(this.distance, ref this.currentElement);
			position.y += 1f;
			this.go.transform.position = position;
			this.go.transform.forward = this.road.GetLookatSmooth(this.distance, this.currentElement);
		}
	}

	// Token: 0x06003A1C RID: 14876 RVA: 0x00157F88 File Offset: 0x00156188
	private void OnDestroy()
	{
		if (this.roadNetwork != null && this.roadNetwork.isInBuildMode)
		{
			this.roadNetwork.RestoreRoadNetwork();
			Debug.Log("Restore Road Network");
		}
	}
}
