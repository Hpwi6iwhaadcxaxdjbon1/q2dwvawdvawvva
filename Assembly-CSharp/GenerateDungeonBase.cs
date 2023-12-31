﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020006B7 RID: 1719
public class GenerateDungeonBase : ProceduralComponent
{
	// Token: 0x040027E7 RID: 10215
	public string EntranceFolder = string.Empty;

	// Token: 0x040027E8 RID: 10216
	public string LinkFolder = string.Empty;

	// Token: 0x040027E9 RID: 10217
	public string EndFolder = string.Empty;

	// Token: 0x040027EA RID: 10218
	public string TransitionFolder = string.Empty;

	// Token: 0x040027EB RID: 10219
	public InfrastructureType ConnectionType = InfrastructureType.UnderwaterLab;

	// Token: 0x040027EC RID: 10220
	private static Vector3 VolumeExtrudePositive = Vector3.one * 0.01f;

	// Token: 0x040027ED RID: 10221
	private static Vector3 VolumeExtrudeNegative = Vector3.one * -0.01f;

	// Token: 0x040027EE RID: 10222
	private const int MaxCount = 2147483647;

	// Token: 0x040027EF RID: 10223
	private const int MaxDepth = 3;

	// Token: 0x040027F0 RID: 10224
	private const int MaxFloor = 2;

	// Token: 0x040027F1 RID: 10225
	private List<GenerateDungeonBase.DungeonSegment> segmentsTotal = new List<GenerateDungeonBase.DungeonSegment>();

	// Token: 0x040027F2 RID: 10226
	private Quaternion[] horizontalRotations = new Quaternion[]
	{
		Quaternion.Euler(0f, 0f, 0f)
	};

	// Token: 0x040027F3 RID: 10227
	private Quaternion[] pillarRotations = new Quaternion[]
	{
		Quaternion.Euler(0f, 0f, 0f),
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, 180f, 0f),
		Quaternion.Euler(0f, 270f, 0f)
	};

	// Token: 0x040027F4 RID: 10228
	private Quaternion[] verticalRotations = new Quaternion[]
	{
		Quaternion.Euler(0f, 0f, 0f),
		Quaternion.Euler(0f, 45f, 0f),
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, 135f, 0f),
		Quaternion.Euler(0f, 180f, 0f),
		Quaternion.Euler(0f, 225f, 0f),
		Quaternion.Euler(0f, 270f, 0f),
		Quaternion.Euler(0f, 315f, 0f)
	};

	// Token: 0x1700040A RID: 1034
	// (get) Token: 0x0600316C RID: 12652 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600316D RID: 12653 RVA: 0x00127828 File Offset: 0x00125A28
	public override void Process(uint seed)
	{
		if (World.Cached)
		{
			TerrainMeta.Path.DungeonBaseRoot = HierarchyUtil.GetRoot("DungeonBase", true, false);
			return;
		}
		if (World.Networked)
		{
			World.Spawn("DungeonBase", null);
			TerrainMeta.Path.DungeonBaseRoot = HierarchyUtil.GetRoot("DungeonBase", true, false);
			return;
		}
		Prefab<DungeonBaseLink>[] array = Prefab.Load<DungeonBaseLink>("assets/bundled/prefabs/autospawn/" + this.EntranceFolder, null, null, true);
		if (array == null)
		{
			return;
		}
		Prefab<DungeonBaseLink>[] array2 = Prefab.Load<DungeonBaseLink>("assets/bundled/prefabs/autospawn/" + this.LinkFolder, null, null, true);
		if (array2 == null)
		{
			return;
		}
		Prefab<DungeonBaseLink>[] array3 = Prefab.Load<DungeonBaseLink>("assets/bundled/prefabs/autospawn/" + this.EndFolder, null, null, true);
		if (array3 == null)
		{
			return;
		}
		Prefab<DungeonBaseTransition>[] array4 = Prefab.Load<DungeonBaseTransition>("assets/bundled/prefabs/autospawn/" + this.TransitionFolder, null, null, true);
		if (array4 == null)
		{
			return;
		}
		foreach (DungeonBaseInfo dungeonBaseInfo in (TerrainMeta.Path ? TerrainMeta.Path.DungeonBaseEntrances : null))
		{
			TerrainPathConnect[] componentsInChildren = dungeonBaseInfo.GetComponentsInChildren<TerrainPathConnect>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				TerrainPathConnect terrainPathConnect = componentsInChildren[i];
				if (terrainPathConnect.Type == this.ConnectionType)
				{
					uint num = seed++;
					List<GenerateDungeonBase.DungeonSegment> list = new List<GenerateDungeonBase.DungeonSegment>();
					GenerateDungeonBase.DungeonSegment segmentStart = new GenerateDungeonBase.DungeonSegment();
					int num2 = 0;
					segmentStart.position = dungeonBaseInfo.transform.position;
					segmentStart.rotation = dungeonBaseInfo.transform.rotation;
					segmentStart.link = dungeonBaseInfo.GetComponentInChildren<DungeonBaseLink>();
					segmentStart.cost = 0;
					segmentStart.floor = 0;
					for (int j = 0; j < 25; j++)
					{
						List<GenerateDungeonBase.DungeonSegment> list2 = new List<GenerateDungeonBase.DungeonSegment>();
						list2.Add(segmentStart);
						this.PlaceSegments(ref num, int.MaxValue, 3, 2, true, false, list2, array2);
						int num3 = list2.Count((GenerateDungeonBase.DungeonSegment x) => x.link.MaxCountLocal != -1);
						if (num3 > num2 || (num3 == num2 && list2.Count > list.Count))
						{
							list = list2;
							num2 = num3;
						}
					}
					if (list.Count > 5)
					{
						list = (from x in list
						orderby (x.position - segmentStart.position).SqrMagnitude2D() descending
						select x).ToList<GenerateDungeonBase.DungeonSegment>();
						this.PlaceSegments(ref num, 1, 4, 2, true, false, list, array);
					}
					if (list.Count > 25)
					{
						GenerateDungeonBase.DungeonSegment segmentEnd = list[list.Count - 1];
						list = (from x in list
						orderby Mathf.Min((x.position - segmentStart.position).SqrMagnitude2D(), (x.position - segmentEnd.position).SqrMagnitude2D()) descending
						select x).ToList<GenerateDungeonBase.DungeonSegment>();
						this.PlaceSegments(ref num, 1, 5, 2, true, false, list, array);
					}
					bool flag = true;
					while (flag)
					{
						flag = false;
						for (int k = 0; k < list.Count; k++)
						{
							GenerateDungeonBase.DungeonSegment dungeonSegment = list[k];
							if (dungeonSegment.link.Cost <= 0 && !this.IsFullyOccupied(list, dungeonSegment))
							{
								list.RemoveAt(k--);
								flag = true;
							}
						}
					}
					this.PlaceSegments(ref num, int.MaxValue, int.MaxValue, 3, true, true, list, array3);
					this.PlaceTransitions(ref num, list, array4);
					this.segmentsTotal.AddRange(list);
				}
			}
		}
		foreach (GenerateDungeonBase.DungeonSegment dungeonSegment2 in this.segmentsTotal)
		{
			if (dungeonSegment2.prefab != null)
			{
				World.AddPrefab("DungeonBase", dungeonSegment2.prefab, dungeonSegment2.position, dungeonSegment2.rotation, Vector3.one);
			}
		}
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.DungeonBaseRoot = HierarchyUtil.GetRoot("DungeonBase", true, false);
		}
	}

	// Token: 0x0600316E RID: 12654 RVA: 0x00127C4C File Offset: 0x00125E4C
	private Quaternion[] GetRotationList(DungeonBaseSocketType type)
	{
		switch (type)
		{
		case DungeonBaseSocketType.Horizontal:
			return this.horizontalRotations;
		case DungeonBaseSocketType.Vertical:
			return this.verticalRotations;
		case DungeonBaseSocketType.Pillar:
			return this.pillarRotations;
		default:
			return null;
		}
	}

	// Token: 0x0600316F RID: 12655 RVA: 0x00127C78 File Offset: 0x00125E78
	private int GetSocketFloor(DungeonBaseSocketType type)
	{
		if (type != DungeonBaseSocketType.Vertical)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x06003170 RID: 12656 RVA: 0x00127C81 File Offset: 0x00125E81
	private bool IsFullyOccupied(List<GenerateDungeonBase.DungeonSegment> segments, GenerateDungeonBase.DungeonSegment segment)
	{
		return this.SocketMatches(segments, segment.link, segment.position, segment.rotation) == segment.link.Sockets.Count;
	}

	// Token: 0x06003171 RID: 12657 RVA: 0x00127CB0 File Offset: 0x00125EB0
	private bool NeighbourMatches(List<GenerateDungeonBase.DungeonSegment> segments, DungeonBaseTransition transition, Vector3 transitionPos, Quaternion transitionRot)
	{
		bool flag = false;
		bool flag2 = false;
		foreach (GenerateDungeonBase.DungeonSegment dungeonSegment in segments)
		{
			if (dungeonSegment.link == null)
			{
				if ((dungeonSegment.position - transitionPos).sqrMagnitude < 0.01f)
				{
					flag = false;
					flag2 = false;
				}
			}
			else
			{
				foreach (DungeonBaseSocket dungeonBaseSocket in dungeonSegment.link.Sockets)
				{
					if ((dungeonSegment.position + dungeonSegment.rotation * dungeonBaseSocket.transform.localPosition - transitionPos).sqrMagnitude < 0.01f)
					{
						if (!flag && dungeonSegment.link.Type == transition.Neighbour1)
						{
							flag = true;
						}
						else if (!flag2 && dungeonSegment.link.Type == transition.Neighbour2)
						{
							flag2 = true;
						}
					}
				}
			}
		}
		return flag && flag2;
	}

	// Token: 0x06003172 RID: 12658 RVA: 0x00127DE8 File Offset: 0x00125FE8
	private int SocketMatches(List<GenerateDungeonBase.DungeonSegment> segments, DungeonBaseLink link, Vector3 linkPos, Quaternion linkRot)
	{
		int num = 0;
		foreach (GenerateDungeonBase.DungeonSegment dungeonSegment in segments)
		{
			foreach (DungeonBaseSocket dungeonBaseSocket in dungeonSegment.link.Sockets)
			{
				Vector3 a = dungeonSegment.position + dungeonSegment.rotation * dungeonBaseSocket.transform.localPosition;
				foreach (DungeonBaseSocket dungeonBaseSocket2 in link.Sockets)
				{
					if (!(dungeonBaseSocket == dungeonBaseSocket2))
					{
						Vector3 b = linkPos + linkRot * dungeonBaseSocket2.transform.localPosition;
						if ((a - b).sqrMagnitude < 0.01f)
						{
							num++;
						}
					}
				}
			}
		}
		return num;
	}

	// Token: 0x06003173 RID: 12659 RVA: 0x00127F24 File Offset: 0x00126124
	private bool IsOccupied(List<GenerateDungeonBase.DungeonSegment> segments, DungeonBaseSocket socket, Vector3 socketPos, Quaternion socketRot)
	{
		foreach (GenerateDungeonBase.DungeonSegment dungeonSegment in segments)
		{
			foreach (DungeonBaseSocket dungeonBaseSocket in dungeonSegment.link.Sockets)
			{
				if (!(dungeonBaseSocket == socket) && (dungeonSegment.position + dungeonSegment.rotation * dungeonBaseSocket.transform.localPosition - socketPos).sqrMagnitude < 0.01f)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003174 RID: 12660 RVA: 0x00127FFC File Offset: 0x001261FC
	private int CountLocal(List<GenerateDungeonBase.DungeonSegment> segments, DungeonBaseLink link)
	{
		int num = 0;
		if (link == null)
		{
			return num;
		}
		foreach (GenerateDungeonBase.DungeonSegment dungeonSegment in segments)
		{
			if (!(dungeonSegment.link == null))
			{
				if (dungeonSegment.link == link)
				{
					num++;
				}
				else if (dungeonSegment.link.MaxCountIdentifier >= 0 && dungeonSegment.link.MaxCountIdentifier == link.MaxCountIdentifier)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x06003175 RID: 12661 RVA: 0x00128098 File Offset: 0x00126298
	private int CountGlobal(List<GenerateDungeonBase.DungeonSegment> segments, DungeonBaseLink link)
	{
		int num = 0;
		if (link == null)
		{
			return num;
		}
		foreach (GenerateDungeonBase.DungeonSegment dungeonSegment in segments)
		{
			if (!(dungeonSegment.link == null))
			{
				if (dungeonSegment.link == link)
				{
					num++;
				}
				else if (dungeonSegment.link.MaxCountIdentifier >= 0 && dungeonSegment.link.MaxCountIdentifier == link.MaxCountIdentifier)
				{
					num++;
				}
			}
		}
		foreach (GenerateDungeonBase.DungeonSegment dungeonSegment2 in this.segmentsTotal)
		{
			if (!(dungeonSegment2.link == null))
			{
				if (dungeonSegment2.link == link)
				{
					num++;
				}
				else if (dungeonSegment2.link.MaxCountIdentifier >= 0 && dungeonSegment2.link.MaxCountIdentifier == link.MaxCountIdentifier)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x06003176 RID: 12662 RVA: 0x001281B8 File Offset: 0x001263B8
	private bool IsBlocked(List<GenerateDungeonBase.DungeonSegment> segments, DungeonBaseLink link, Vector3 linkPos, Quaternion linkRot)
	{
		foreach (DungeonVolume dungeonVolume in link.Volumes)
		{
			OBB bounds = dungeonVolume.GetBounds(linkPos, linkRot, GenerateDungeonBase.VolumeExtrudeNegative);
			OBB bounds2 = dungeonVolume.GetBounds(linkPos, linkRot, GenerateDungeonBase.VolumeExtrudePositive);
			foreach (GenerateDungeonBase.DungeonSegment dungeonSegment in segments)
			{
				foreach (DungeonVolume dungeonVolume2 in dungeonSegment.link.Volumes)
				{
					OBB bounds3 = dungeonVolume2.GetBounds(dungeonSegment.position, dungeonSegment.rotation, GenerateDungeonBase.VolumeExtrudeNegative);
					if (bounds.Intersects(bounds3))
					{
						return true;
					}
				}
				foreach (DungeonBaseSocket dungeonBaseSocket in dungeonSegment.link.Sockets)
				{
					Vector3 vector = dungeonSegment.position + dungeonSegment.rotation * dungeonBaseSocket.transform.localPosition;
					if (bounds2.Contains(vector))
					{
						bool flag = false;
						foreach (DungeonBaseSocket dungeonBaseSocket2 in link.Sockets)
						{
							Vector3 b = linkPos + linkRot * dungeonBaseSocket2.transform.localPosition;
							if ((vector - b).sqrMagnitude < 0.01f)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							return true;
						}
					}
				}
			}
		}
		foreach (GenerateDungeonBase.DungeonSegment dungeonSegment2 in segments)
		{
			foreach (DungeonVolume dungeonVolume3 in dungeonSegment2.link.Volumes)
			{
				OBB bounds4 = dungeonVolume3.GetBounds(dungeonSegment2.position, dungeonSegment2.rotation, GenerateDungeonBase.VolumeExtrudePositive);
				foreach (DungeonBaseSocket dungeonBaseSocket3 in link.Sockets)
				{
					Vector3 vector2 = linkPos + linkRot * dungeonBaseSocket3.transform.localPosition;
					if (bounds4.Contains(vector2))
					{
						bool flag2 = false;
						foreach (DungeonBaseSocket dungeonBaseSocket4 in dungeonSegment2.link.Sockets)
						{
							if ((dungeonSegment2.position + dungeonSegment2.rotation * dungeonBaseSocket4.transform.localPosition - vector2).sqrMagnitude < 0.01f)
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06003177 RID: 12663 RVA: 0x001285D8 File Offset: 0x001267D8
	private void PlaceSegments(ref uint seed, int count, int budget, int floors, bool attachToFemale, bool attachToMale, List<GenerateDungeonBase.DungeonSegment> segments, Prefab<DungeonBaseLink>[] prefabs)
	{
		int num = 0;
		for (int i = 0; i < segments.Count; i++)
		{
			GenerateDungeonBase.DungeonSegment dungeonSegment = segments[i];
			if (dungeonSegment.cost < budget)
			{
				int num2 = SeedRandom.Range(ref seed, 0, dungeonSegment.link.Sockets.Count);
				for (int j = 0; j < dungeonSegment.link.Sockets.Count; j++)
				{
					DungeonBaseSocket dungeonBaseSocket = dungeonSegment.link.Sockets[(j + num2) % dungeonSegment.link.Sockets.Count];
					if ((dungeonBaseSocket.Female && attachToFemale) || (dungeonBaseSocket.Male && attachToMale))
					{
						Vector3 vector = dungeonSegment.position + dungeonSegment.rotation * dungeonBaseSocket.transform.localPosition;
						Quaternion quaternion = dungeonSegment.rotation * dungeonBaseSocket.transform.localRotation;
						if (!this.IsOccupied(segments, dungeonBaseSocket, vector, quaternion))
						{
							prefabs.Shuffle(ref seed);
							GenerateDungeonBase.DungeonSegment dungeonSegment2 = null;
							Quaternion[] rotationList = this.GetRotationList(dungeonBaseSocket.Type);
							foreach (Prefab<DungeonBaseLink> prefab in prefabs)
							{
								DungeonBaseLink component = prefab.Component;
								if (component.MaxCountLocal != 0 && component.MaxCountGlobal != 0 && (component.MaxFloor < 0 || dungeonSegment.floor <= component.MaxFloor))
								{
									int num3 = dungeonSegment.cost + component.Cost;
									if (num3 <= budget)
									{
										int num4 = dungeonSegment.floor + this.GetSocketFloor(dungeonBaseSocket.Type);
										if (num4 <= floors)
										{
											DungeonBaseSocket dungeonBaseSocket2 = null;
											Vector3 zero = Vector3.zero;
											Quaternion identity = Quaternion.identity;
											int score = 0;
											if (this.Place(ref seed, segments, dungeonBaseSocket, vector, quaternion, prefab, rotationList, out dungeonBaseSocket2, out zero, out identity, out score) && (component.MaxCountLocal <= 0 || this.CountLocal(segments, component) < component.MaxCountLocal) && (component.MaxCountGlobal <= 0 || this.CountGlobal(segments, component) < component.MaxCountGlobal))
											{
												GenerateDungeonBase.DungeonSegment dungeonSegment3 = new GenerateDungeonBase.DungeonSegment();
												dungeonSegment3.position = zero;
												dungeonSegment3.rotation = identity;
												dungeonSegment3.prefab = prefab;
												dungeonSegment3.link = component;
												dungeonSegment3.score = score;
												dungeonSegment3.cost = num3;
												dungeonSegment3.floor = num4;
												if (dungeonSegment2 == null || dungeonSegment2.score < dungeonSegment3.score)
												{
													dungeonSegment2 = dungeonSegment3;
												}
											}
										}
									}
								}
							}
							if (dungeonSegment2 != null)
							{
								segments.Add(dungeonSegment2);
								num++;
								if (num >= count)
								{
									return;
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06003178 RID: 12664 RVA: 0x0012887C File Offset: 0x00126A7C
	private void PlaceTransitions(ref uint seed, List<GenerateDungeonBase.DungeonSegment> segments, Prefab<DungeonBaseTransition>[] prefabs)
	{
		int count = segments.Count;
		for (int i = 0; i < count; i++)
		{
			GenerateDungeonBase.DungeonSegment dungeonSegment = segments[i];
			int num = SeedRandom.Range(ref seed, 0, dungeonSegment.link.Sockets.Count);
			for (int j = 0; j < dungeonSegment.link.Sockets.Count; j++)
			{
				DungeonBaseSocket dungeonBaseSocket = dungeonSegment.link.Sockets[(j + num) % dungeonSegment.link.Sockets.Count];
				Vector3 vector = dungeonSegment.position + dungeonSegment.rotation * dungeonBaseSocket.transform.localPosition;
				Quaternion quaternion = dungeonSegment.rotation * dungeonBaseSocket.transform.localRotation;
				prefabs.Shuffle(ref seed);
				foreach (Prefab<DungeonBaseTransition> prefab in prefabs)
				{
					if (dungeonBaseSocket.Type == prefab.Component.Type && this.NeighbourMatches(segments, prefab.Component, vector, quaternion))
					{
						segments.Add(new GenerateDungeonBase.DungeonSegment
						{
							position = vector,
							rotation = quaternion,
							prefab = prefab,
							link = null,
							score = 0,
							cost = 0,
							floor = 0
						});
						break;
					}
				}
			}
		}
	}

	// Token: 0x06003179 RID: 12665 RVA: 0x001289EC File Offset: 0x00126BEC
	private bool Place(ref uint seed, List<GenerateDungeonBase.DungeonSegment> segments, DungeonBaseSocket targetSocket, Vector3 targetPos, Quaternion targetRot, Prefab<DungeonBaseLink> prefab, Quaternion[] rotations, out DungeonBaseSocket linkSocket, out Vector3 linkPos, out Quaternion linkRot, out int linkScore)
	{
		linkSocket = null;
		linkPos = Vector3.one;
		linkRot = Quaternion.identity;
		linkScore = 0;
		DungeonBaseLink component = prefab.Component;
		int num = SeedRandom.Range(ref seed, 0, component.Sockets.Count);
		for (int i = 0; i < component.Sockets.Count; i++)
		{
			DungeonBaseSocket dungeonBaseSocket = component.Sockets[(i + num) % component.Sockets.Count];
			if (dungeonBaseSocket.Type == targetSocket.Type && ((dungeonBaseSocket.Male && targetSocket.Female) || (dungeonBaseSocket.Female && targetSocket.Male)))
			{
				rotations.Shuffle(ref seed);
				foreach (Quaternion lhs in rotations)
				{
					Quaternion quaternion = Quaternion.FromToRotation(-dungeonBaseSocket.transform.forward, targetRot * Vector3.forward);
					if (dungeonBaseSocket.Type != DungeonBaseSocketType.Vertical)
					{
						quaternion = QuaternionEx.LookRotationForcedUp(quaternion * Vector3.forward, Vector3.up);
					}
					Quaternion quaternion2 = lhs * quaternion;
					Vector3 vector = targetPos - quaternion2 * dungeonBaseSocket.transform.localPosition;
					if (!this.IsBlocked(segments, component, vector, quaternion2))
					{
						int num2 = this.SocketMatches(segments, component, vector, quaternion2);
						if (num2 > linkScore && prefab.CheckEnvironmentVolumesOutsideTerrain(vector, quaternion2, Vector3.one, EnvironmentType.UnderwaterLab, 1f))
						{
							linkSocket = dungeonBaseSocket;
							linkPos = vector;
							linkRot = quaternion2;
							linkScore = num2;
						}
					}
				}
			}
		}
		return linkScore > 0;
	}

	// Token: 0x0600317A RID: 12666 RVA: 0x00128B98 File Offset: 0x00126D98
	public static void SetupAI()
	{
		if (TerrainMeta.Path == null || TerrainMeta.Path.DungeonBaseEntrances == null)
		{
			return;
		}
		foreach (DungeonBaseInfo dungeonBaseInfo in TerrainMeta.Path.DungeonBaseEntrances)
		{
			if (!(dungeonBaseInfo == null))
			{
				List<AIInformationZone> list = new List<AIInformationZone>();
				int num = 0;
				AIInformationZone componentInChildren = dungeonBaseInfo.GetComponentInChildren<AIInformationZone>();
				if (componentInChildren != null)
				{
					list.Add(componentInChildren);
					num++;
				}
				foreach (GameObject gameObject in dungeonBaseInfo.GetComponent<DungeonBaseInfo>().Links)
				{
					AIInformationZone componentInChildren2 = gameObject.GetComponentInChildren<AIInformationZone>();
					if (!(componentInChildren2 == null))
					{
						list.Add(componentInChildren2);
						num++;
					}
				}
				GameObject gameObject2 = new GameObject("AIZ");
				gameObject2.transform.position = dungeonBaseInfo.gameObject.transform.position;
				AIInformationZone aiinformationZone = AIInformationZone.Merge(list, gameObject2);
				aiinformationZone.ShouldSleepAI = true;
				gameObject2.transform.SetParent(dungeonBaseInfo.gameObject.transform);
				GameObject gameObject3 = new GameObject("WakeTrigger");
				gameObject3.transform.position = gameObject2.transform.position + aiinformationZone.bounds.center;
				gameObject3.transform.localScale = aiinformationZone.bounds.extents + new Vector3(100f, 100f, 100f);
				gameObject3.AddComponent<BoxCollider>().isTrigger = true;
				gameObject3.layer = LayerMask.NameToLayer("Trigger");
				gameObject3.transform.SetParent(dungeonBaseInfo.gameObject.transform);
				TriggerWakeAIZ triggerWakeAIZ = gameObject3.AddComponent<TriggerWakeAIZ>();
				triggerWakeAIZ.interestLayers = LayerMask.GetMask(new string[]
				{
					"Player (Server)"
				});
				triggerWakeAIZ.Init(aiinformationZone);
			}
		}
	}

	// Token: 0x02000DDE RID: 3550
	private class DungeonSegment
	{
		// Token: 0x0400494E RID: 18766
		public Vector3 position;

		// Token: 0x0400494F RID: 18767
		public Quaternion rotation;

		// Token: 0x04004950 RID: 18768
		public Prefab prefab;

		// Token: 0x04004951 RID: 18769
		public DungeonBaseLink link;

		// Token: 0x04004952 RID: 18770
		public int score;

		// Token: 0x04004953 RID: 18771
		public int cost;

		// Token: 0x04004954 RID: 18772
		public int floor;
	}
}
