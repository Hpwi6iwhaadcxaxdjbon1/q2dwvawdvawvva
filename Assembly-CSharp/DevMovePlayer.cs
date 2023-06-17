using System;
using UnityEngine;

// Token: 0x0200031E RID: 798
public class DevMovePlayer : BaseMonoBehaviour
{
	// Token: 0x040017E4 RID: 6116
	public BasePlayer player;

	// Token: 0x040017E5 RID: 6117
	public Transform[] Waypoints;

	// Token: 0x040017E6 RID: 6118
	public bool moveRandomly;

	// Token: 0x040017E7 RID: 6119
	public Vector3 destination = Vector3.zero;

	// Token: 0x040017E8 RID: 6120
	public Vector3 lookPoint = Vector3.zero;

	// Token: 0x040017E9 RID: 6121
	private int waypointIndex;

	// Token: 0x040017EA RID: 6122
	private float randRun;

	// Token: 0x06001ED1 RID: 7889 RVA: 0x000D1D58 File Offset: 0x000CFF58
	public void Awake()
	{
		this.randRun = UnityEngine.Random.Range(5f, 10f);
		this.player = base.GetComponent<BasePlayer>();
		if (this.Waypoints.Length != 0)
		{
			this.destination = this.Waypoints[0].position;
		}
		else
		{
			this.destination = base.transform.position;
		}
		if (this.player.isClient)
		{
			return;
		}
		if (this.player.eyes == null)
		{
			this.player.eyes = this.player.GetComponent<PlayerEyes>();
		}
		base.Invoke(new Action(this.LateSpawn), 1f);
	}

	// Token: 0x06001ED2 RID: 7890 RVA: 0x000D1E04 File Offset: 0x000D0004
	public void LateSpawn()
	{
		Item item = ItemManager.CreateByName("rifle.semiauto", 1, 0UL);
		this.player.inventory.GiveItem(item, this.player.inventory.containerBelt, false);
		this.player.UpdateActiveItem(item.uid);
		this.player.health = 100f;
	}

	// Token: 0x06001ED3 RID: 7891 RVA: 0x000D1E63 File Offset: 0x000D0063
	public void SetWaypoints(Transform[] wps)
	{
		this.Waypoints = wps;
		this.destination = wps[0].position;
	}

	// Token: 0x06001ED4 RID: 7892 RVA: 0x000D1E7C File Offset: 0x000D007C
	public void Update()
	{
		if (this.player.isClient)
		{
			return;
		}
		if (!this.player.IsAlive() || this.player.IsWounded())
		{
			return;
		}
		if (Vector3.Distance(this.destination, base.transform.position) < 0.25f)
		{
			if (this.moveRandomly)
			{
				this.waypointIndex = UnityEngine.Random.Range(0, this.Waypoints.Length);
			}
			else
			{
				this.waypointIndex++;
			}
			if (this.waypointIndex >= this.Waypoints.Length)
			{
				this.waypointIndex = 0;
			}
		}
		if (this.Waypoints.Length <= this.waypointIndex)
		{
			return;
		}
		this.destination = this.Waypoints[this.waypointIndex].position;
		Vector3 normalized = (this.destination - base.transform.position).normalized;
		float running = Mathf.Sin(Time.time + this.randRun);
		float speed = this.player.GetSpeed(running, 0f, 0f);
		Vector3 position = base.transform.position;
		float range = 1f;
		LayerMask mask = 1537286401;
		RaycastHit raycastHit;
		if (TransformUtil.GetGroundInfo(base.transform.position + normalized * speed * Time.deltaTime, out raycastHit, range, mask, this.player.transform))
		{
			position = raycastHit.point;
		}
		base.transform.position = position;
		Vector3 normalized2 = (new Vector3(this.destination.x, 0f, this.destination.z) - new Vector3(this.player.transform.position.x, 0f, this.player.transform.position.z)).normalized;
		this.player.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}
}
