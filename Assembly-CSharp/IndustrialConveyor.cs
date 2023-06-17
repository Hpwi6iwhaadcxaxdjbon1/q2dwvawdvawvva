﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using Newtonsoft.Json;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Token: 0x02000084 RID: 132
public class IndustrialConveyor : IndustrialEntity
{
	// Token: 0x04000800 RID: 2048
	public int MaxStackSizePerMove = 128;

	// Token: 0x04000801 RID: 2049
	public GameObjectRef FilterDialog;

	// Token: 0x04000802 RID: 2050
	private const float ScreenUpdateRange = 30f;

	// Token: 0x04000803 RID: 2051
	public const global::BaseEntity.Flags FilterPassFlag = global::BaseEntity.Flags.Reserved9;

	// Token: 0x04000804 RID: 2052
	public const global::BaseEntity.Flags FilterFailFlag = global::BaseEntity.Flags.Reserved10;

	// Token: 0x04000805 RID: 2053
	public const int MaxContainerDepth = 32;

	// Token: 0x04000806 RID: 2054
	public SoundDefinition transferItemSoundDef;

	// Token: 0x04000807 RID: 2055
	public SoundDefinition transferItemStartSoundDef;

	// Token: 0x04000808 RID: 2056
	private List<global::IndustrialConveyor.ItemFilter> filterItems = new List<global::IndustrialConveyor.ItemFilter>();

	// Token: 0x04000809 RID: 2057
	private global::IndustrialConveyor.ConveyorMode mode;

	// Token: 0x0400080A RID: 2058
	public const int MAX_FILTER_SIZE = 12;

	// Token: 0x0400080B RID: 2059
	public Image IconTransferImage;

	// Token: 0x0400080C RID: 2060
	private bool refreshInputOutputs;

	// Token: 0x0400080D RID: 2061
	private IIndustrialStorage workerOutput;

	// Token: 0x0400080E RID: 2062
	private Func<IIndustrialStorage, int, bool> filterFunc;

	// Token: 0x0400080F RID: 2063
	private List<global::IOEntity.ContainerInputOutput> splitOutputs = new List<global::IOEntity.ContainerInputOutput>();

	// Token: 0x04000810 RID: 2064
	private List<global::IOEntity.ContainerInputOutput> splitInputs = new List<global::IOEntity.ContainerInputOutput>();

	// Token: 0x04000811 RID: 2065
	private bool? lastFilterState;

	// Token: 0x04000812 RID: 2066
	private Stopwatch transferStopWatch = new Stopwatch();

	// Token: 0x04000813 RID: 2067
	private bool wasOnWhenPowerLost;

	// Token: 0x06000C63 RID: 3171 RVA: 0x0006B3F4 File Offset: 0x000695F4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("IndustrialConveyor.OnRpcMessage", 0))
		{
			if (rpc == 617569194U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					UnityEngine.Debug.Log("SV_RPCMessage: " + player + " - RPC_ChangeFilters ");
				}
				using (TimeWarning.New("RPC_ChangeFilters", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(617569194U, "RPC_ChangeFilters", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(617569194U, "RPC_ChangeFilters", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_ChangeFilters(msg2);
						}
					}
					catch (Exception exception)
					{
						UnityEngine.Debug.LogException(exception);
						player.Kick("RPC Error in RPC_ChangeFilters");
					}
				}
				return true;
			}
			if (rpc == 3731379386U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					UnityEngine.Debug.Log("SV_RPCMessage: " + player + " - Server_RequestUpToDateFilters ");
				}
				using (TimeWarning.New("Server_RequestUpToDateFilters", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3731379386U, "Server_RequestUpToDateFilters", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3731379386U, "Server_RequestUpToDateFilters", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RequestUpToDateFilters(msg3);
						}
					}
					catch (Exception exception2)
					{
						UnityEngine.Debug.LogException(exception2);
						player.Kick("RPC Error in Server_RequestUpToDateFilters");
					}
				}
				return true;
			}
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					UnityEngine.Debug.Log("SV_RPCMessage: " + player + " - SvSwitch ");
				}
				using (TimeWarning.New("SvSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4167839872U, "SvSwitch", this, player, 2UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4167839872U, "SvSwitch", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SvSwitch(msg4);
						}
					}
					catch (Exception exception3)
					{
						UnityEngine.Debug.LogException(exception3);
						player.Kick("RPC Error in SvSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000C64 RID: 3172 RVA: 0x0006B8A4 File Offset: 0x00069AA4
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = next.HasFlag(global::BaseEntity.Flags.On);
		if (old.HasFlag(global::BaseEntity.Flags.On) != flag && base.isServer)
		{
			float conveyorMoveFrequency = ConVar.Server.conveyorMoveFrequency;
			if (flag && conveyorMoveFrequency > 0f)
			{
				base.InvokeRandomized(new Action(this.ScheduleMove), conveyorMoveFrequency, conveyorMoveFrequency, conveyorMoveFrequency * 0.5f);
				return;
			}
			base.CancelInvoke(new Action(this.ScheduleMove));
		}
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x0006B926 File Offset: 0x00069B26
	private void ScheduleMove()
	{
		IndustrialEntity.Queue.Add(this);
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x0006B934 File Offset: 0x00069B34
	private global::Item GetItemToMove(IIndustrialStorage storage, out global::IndustrialConveyor.ItemFilter associatedFilter, int slot, global::ItemContainer targetContainer = null)
	{
		associatedFilter = default(global::IndustrialConveyor.ItemFilter);
		ValueTuple<global::IndustrialConveyor.ItemFilter, int> valueTuple = default(ValueTuple<global::IndustrialConveyor.ItemFilter, int>);
		if (storage == null || storage.Container == null)
		{
			return null;
		}
		if (storage.Container.IsEmpty())
		{
			return null;
		}
		Vector2i vector2i = storage.OutputSlotRange(slot);
		for (int i = vector2i.x; i <= vector2i.y; i++)
		{
			global::Item slot2 = storage.Container.GetSlot(i);
			valueTuple = default(ValueTuple<global::IndustrialConveyor.ItemFilter, int>);
			if (slot2 != null && (this.filterItems.Count == 0 || this.FilterHasItem(slot2, out valueTuple)))
			{
				associatedFilter = valueTuple.Item1;
				if (targetContainer == null || !(associatedFilter.TargetItem != null) || associatedFilter.MaxAmountInOutput <= 0 || targetContainer.GetTotalItemAmount(slot2, vector2i.x, vector2i.y) < associatedFilter.MaxAmountInOutput)
				{
					return slot2;
				}
			}
		}
		return null;
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x0006BA04 File Offset: 0x00069C04
	private bool FilterHasItem(global::Item item, [TupleElementNames(new string[]
	{
		"filter",
		"index"
	})] out ValueTuple<global::IndustrialConveyor.ItemFilter, int> filter)
	{
		filter = default(ValueTuple<global::IndustrialConveyor.ItemFilter, int>);
		for (int i = 0; i < this.filterItems.Count; i++)
		{
			global::IndustrialConveyor.ItemFilter itemFilter = this.filterItems[i];
			if (this.FilterMatches(itemFilter, item))
			{
				filter = new ValueTuple<global::IndustrialConveyor.ItemFilter, int>(itemFilter, i);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x0006BA58 File Offset: 0x00069C58
	private bool FilterMatches(global::IndustrialConveyor.ItemFilter filter, global::Item item)
	{
		if (item.IsBlueprint() && filter.IsBlueprint && item.blueprintTargetDef == filter.TargetItem)
		{
			return true;
		}
		if (filter.TargetItem == item.info && !filter.IsBlueprint)
		{
			return true;
		}
		if (filter.TargetItem != null && item.info.isRedirectOf == filter.TargetItem)
		{
			return true;
		}
		if (filter.TargetCategory != null)
		{
			ItemCategory category = item.info.category;
			ItemCategory? targetCategory = filter.TargetCategory;
			if (category == targetCategory.GetValueOrDefault() & targetCategory != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x0006BB08 File Offset: 0x00069D08
	private bool FilterContainerInput(IIndustrialStorage storage, int slot)
	{
		IIndustrialStorage industrialStorage = this.workerOutput;
		global::IndustrialConveyor.ItemFilter itemFilter;
		return this.GetItemToMove(storage, out itemFilter, slot, (industrialStorage != null) ? industrialStorage.Container : null) != null;
	}

	// Token: 0x06000C6A RID: 3178 RVA: 0x0006BB34 File Offset: 0x00069D34
	protected override void RunJob()
	{
		global::IndustrialConveyor.<>c__DisplayClass28_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		base.RunJob();
		if (ConVar.Server.conveyorMoveFrequency <= 0f)
		{
			return;
		}
		if (this.filterFunc == null)
		{
			this.filterFunc = new Func<IIndustrialStorage, int, bool>(this.FilterContainerInput);
		}
		if (this.refreshInputOutputs)
		{
			this.refreshInputOutputs = false;
			this.splitInputs.Clear();
			this.splitOutputs.Clear();
			base.FindContainerSource(this.splitInputs, 32, true, -1, 0);
			base.FindContainerSource(this.splitOutputs, 32, false, -1, this.MaxStackSizePerMove);
		}
		CS$<>8__locals1.hasItems = this.CheckIfAnyInputPassesFilters(this.splitInputs);
		if (this.lastFilterState != null)
		{
			bool hasItems = CS$<>8__locals1.hasItems;
			bool? flag = this.lastFilterState;
			if (hasItems == flag.GetValueOrDefault() & flag != null)
			{
				goto IL_D1;
			}
		}
		if (!CS$<>8__locals1.hasItems)
		{
			this.<RunJob>g__UpdateFilterPassthroughs|28_0(ref CS$<>8__locals1);
		}
		IL_D1:
		if (!CS$<>8__locals1.hasItems)
		{
			return;
		}
		this.transferStopWatch.Restart();
		global::IndustrialConveyor.<>c__DisplayClass28_1 CS$<>8__locals2;
		CS$<>8__locals2.transfer = Facepunch.Pool.Get<IndustrialConveyorTransfer>();
		try
		{
			bool flag2 = false;
			CS$<>8__locals2.transfer.ItemTransfers = Facepunch.Pool.GetList<IndustrialConveyorTransfer.ItemTransfer>();
			CS$<>8__locals2.transfer.inputEntities = Facepunch.Pool.GetList<NetworkableId>();
			CS$<>8__locals2.transfer.outputEntities = Facepunch.Pool.GetList<NetworkableId>();
			List<int> list = Facepunch.Pool.GetList<int>();
			int num = 0;
			int count = this.splitOutputs.Count;
			foreach (global::IOEntity.ContainerInputOutput containerInputOutput in this.splitOutputs)
			{
				this.workerOutput = containerInputOutput.Storage;
				foreach (global::IOEntity.ContainerInputOutput containerInputOutput2 in this.splitInputs)
				{
					int num2 = 0;
					IIndustrialStorage storage = containerInputOutput2.Storage;
					if (storage != null && containerInputOutput.Storage != null && !(containerInputOutput2.Storage.IndustrialEntity == containerInputOutput.Storage.IndustrialEntity))
					{
						global::ItemContainer container = storage.Container;
						global::ItemContainer container2 = containerInputOutput.Storage.Container;
						if (container != null && container2 != null && storage.Container != null && !storage.Container.IsEmpty())
						{
							ValueTuple<global::IndustrialConveyor.ItemFilter, int> valueTuple = default(ValueTuple<global::IndustrialConveyor.ItemFilter, int>);
							Vector2i vector2i = storage.OutputSlotRange(containerInputOutput2.SlotIndex);
							for (int i = vector2i.x; i <= vector2i.y; i++)
							{
								Vector2i vector2i2 = containerInputOutput.Storage.InputSlotRange(containerInputOutput.SlotIndex);
								global::Item slot = storage.Container.GetSlot(i);
								if (slot != null)
								{
									bool flag3 = true;
									if (this.filterItems.Count > 0)
									{
										if (this.mode == global::IndustrialConveyor.ConveyorMode.Any || this.mode == global::IndustrialConveyor.ConveyorMode.And)
										{
											flag3 = this.FilterHasItem(slot, out valueTuple);
										}
										if (this.mode == global::IndustrialConveyor.ConveyorMode.Not)
										{
											flag3 = !this.FilterHasItem(slot, out valueTuple);
										}
									}
									if (flag3)
									{
										bool flag4 = this.mode == global::IndustrialConveyor.ConveyorMode.And || this.mode == global::IndustrialConveyor.ConveyorMode.Any;
										if (flag4 && valueTuple.Item1.TargetItem != null && valueTuple.Item1.MaxAmountInOutput > 0 && containerInputOutput.Storage.Container.GetTotalItemAmount(slot, vector2i2.x, vector2i2.y) >= valueTuple.Item1.MaxAmountInOutput)
										{
											flag2 = true;
										}
										else
										{
											int num3 = (int)((float)Mathf.Min(this.MaxStackSizePerMove, slot.info.stackable) / (float)count);
											if (flag4 && valueTuple.Item1.MinAmountInInput > 0)
											{
												if (valueTuple.Item1.TargetItem != null && global::IndustrialConveyor.<RunJob>g__FilterMatchItem|28_1(valueTuple.Item1, slot))
												{
													int totalItemAmount = container.GetTotalItemAmount(slot, vector2i.x, vector2i.y);
													num3 = Mathf.Min(num3, totalItemAmount - valueTuple.Item1.MinAmountInInput);
												}
												else if (valueTuple.Item1.TargetCategory != null)
												{
													num3 = Mathf.Min(num3, container.GetTotalCategoryAmount(valueTuple.Item1.TargetCategory.Value, vector2i2.x, vector2i2.y) - valueTuple.Item1.MinAmountInInput);
												}
												if (num3 == 0)
												{
													goto IL_705;
												}
											}
											if (slot.amount == 1 || (num3 <= 0 && slot.amount > 0))
											{
												num3 = 1;
											}
											if (flag4 && valueTuple.Item1.BufferAmount > 0)
											{
												num3 = Mathf.Min(num3, valueTuple.Item1.BufferTransferRemaining);
											}
											if (flag4 && valueTuple.Item1.MaxAmountInOutput > 0)
											{
												if (valueTuple.Item1.TargetItem != null && global::IndustrialConveyor.<RunJob>g__FilterMatchItem|28_1(valueTuple.Item1, slot))
												{
													num3 = Mathf.Min(num3, valueTuple.Item1.MaxAmountInOutput - container2.GetTotalItemAmount(slot, vector2i2.x, vector2i2.y));
												}
												else if (valueTuple.Item1.TargetCategory != null)
												{
													num3 = Mathf.Min(num3, valueTuple.Item1.MaxAmountInOutput - container2.GetTotalCategoryAmount(valueTuple.Item1.TargetCategory.Value, vector2i2.x, vector2i2.y));
												}
												if ((float)num3 <= 0f)
												{
													flag2 = true;
												}
											}
											float num4 = (float)Mathf.Min(slot.amount, num3);
											if (num4 > 0f && num4 < 1f)
											{
												num4 = 1f;
											}
											num3 = (int)num4;
											if (num3 > 0)
											{
												global::Item item = null;
												int amount = slot.amount;
												if (slot.amount > num3)
												{
													item = slot.SplitItem(num3);
													amount = item.amount;
												}
												containerInputOutput.Storage.OnStorageItemTransferBegin();
												bool flag5 = false;
												global::Item nonFullStackWithinRange = container2.GetNonFullStackWithinRange(item ?? slot, vector2i2);
												if (nonFullStackWithinRange != null)
												{
													(item ?? slot).MoveToContainer(container2, nonFullStackWithinRange.position, true, false, null, false);
												}
												else
												{
													for (int j = vector2i2.x; j <= vector2i2.y; j++)
													{
														global::Item slot2 = container2.GetSlot(j);
														if ((slot2 == null || slot2.info == slot.info) && (item ?? slot).MoveToContainer(container2, j, true, false, null, false))
														{
															flag5 = true;
															break;
														}
													}
												}
												if (valueTuple.Item1.BufferTransferRemaining > 0)
												{
													global::IndustrialConveyor.ItemFilter item2 = valueTuple.Item1;
													item2.BufferTransferRemaining -= amount;
													this.filterItems[valueTuple.Item2] = item2;
												}
												if (!flag5 && item != null)
												{
													slot.amount += item.amount;
													slot.MarkDirty();
													item.Remove(0f);
													item = null;
												}
												if (flag5)
												{
													num2++;
													if (item != null)
													{
														global::IndustrialConveyor.<RunJob>g__AddTransfer|28_2(item.info.itemid, amount, containerInputOutput2.Storage.IndustrialEntity, containerInputOutput.Storage.IndustrialEntity, ref CS$<>8__locals2);
													}
													else
													{
														global::IndustrialConveyor.<RunJob>g__AddTransfer|28_2(slot.info.itemid, amount, containerInputOutput2.Storage.IndustrialEntity, containerInputOutput.Storage.IndustrialEntity, ref CS$<>8__locals2);
													}
												}
												else if (!list.Contains(num))
												{
													list.Add(num);
												}
												containerInputOutput.Storage.OnStorageItemTransferEnd();
												if (num2 >= ConVar.Server.maxItemStacksMovedPerTickIndustrial)
												{
													break;
												}
											}
										}
									}
								}
								IL_705:;
							}
						}
					}
				}
				num++;
			}
			if ((CS$<>8__locals2.transfer.ItemTransfers.Count == 0 & CS$<>8__locals1.hasItems) && flag2)
			{
				CS$<>8__locals1.hasItems = false;
			}
			if (this.lastFilterState != null)
			{
				bool hasItems2 = CS$<>8__locals1.hasItems;
				bool? flag = this.lastFilterState;
				if (hasItems2 == flag.GetValueOrDefault() & flag != null)
				{
					goto IL_7B2;
				}
			}
			this.<RunJob>g__UpdateFilterPassthroughs|28_0(ref CS$<>8__locals1);
			IL_7B2:
			Facepunch.Pool.FreeList<int>(ref list);
			if (CS$<>8__locals2.transfer.ItemTransfers.Count > 0)
			{
				base.ClientRPCEx<IndustrialConveyorTransfer>(new SendInfo(global::BaseNetworkable.GetConnectionsWithin(base.transform.position, 30f)), null, "ReceiveItemTransferDetails", CS$<>8__locals2.transfer);
			}
		}
		finally
		{
			if (CS$<>8__locals2.transfer != null)
			{
				((IDisposable)CS$<>8__locals2.transfer).Dispose();
			}
		}
	}

	// Token: 0x06000C6B RID: 3179 RVA: 0x0006C39C File Offset: 0x0006A59C
	protected override void OnIndustrialNetworkChanged()
	{
		base.OnIndustrialNetworkChanged();
		this.refreshInputOutputs = true;
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x0006C3AB File Offset: 0x0006A5AB
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.refreshInputOutputs = true;
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x0006C3BC File Offset: 0x0006A5BC
	private bool CheckIfAnyInputPassesFilters(List<global::IOEntity.ContainerInputOutput> inputs)
	{
		if (this.filterItems.Count == 0)
		{
			using (List<global::IOEntity.ContainerInputOutput>.Enumerator enumerator = inputs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::IOEntity.ContainerInputOutput containerInputOutput = enumerator.Current;
					global::IndustrialConveyor.ItemFilter itemFilter;
					if (this.GetItemToMove(containerInputOutput.Storage, out itemFilter, containerInputOutput.SlotIndex, null) != null)
					{
						return true;
					}
				}
				return false;
			}
		}
		int num = 0;
		int num2 = 0;
		if (this.mode == global::IndustrialConveyor.ConveyorMode.And)
		{
			using (List<global::IndustrialConveyor.ItemFilter>.Enumerator enumerator2 = this.filterItems.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.BufferTransferRemaining > 0)
					{
						num2++;
					}
				}
			}
		}
		for (int i = 0; i < this.filterItems.Count; i++)
		{
			global::IndustrialConveyor.ItemFilter itemFilter2 = this.filterItems[i];
			int num3 = 0;
			int num4 = 0;
			foreach (global::IOEntity.ContainerInputOutput containerInputOutput2 in inputs)
			{
				Vector2i vector2i = containerInputOutput2.Storage.OutputSlotRange(containerInputOutput2.SlotIndex);
				for (int j = vector2i.x; j <= vector2i.y; j++)
				{
					global::Item slot = containerInputOutput2.Storage.Container.GetSlot(j);
					if (slot != null)
					{
						bool flag = this.FilterMatches(itemFilter2, slot);
						if (this.mode == global::IndustrialConveyor.ConveyorMode.Not)
						{
							flag = !flag;
						}
						if (flag)
						{
							if (itemFilter2.BufferAmount > 0)
							{
								num3 += slot.amount;
								if (itemFilter2.BufferTransferRemaining > 0)
								{
									num++;
									break;
								}
								if (num3 >= itemFilter2.BufferAmount + itemFilter2.MinAmountInInput)
								{
									if (this.mode != global::IndustrialConveyor.ConveyorMode.And)
									{
										itemFilter2.BufferTransferRemaining = itemFilter2.BufferAmount;
										this.filterItems[i] = itemFilter2;
									}
									num++;
									break;
								}
							}
							if (itemFilter2.MinAmountInInput > 0)
							{
								num4 += slot.amount;
								if (num4 > itemFilter2.MinAmountInInput + itemFilter2.BufferAmount)
								{
									num++;
									break;
								}
							}
							if (itemFilter2.BufferAmount == 0 && itemFilter2.MinAmountInInput == 0)
							{
								num++;
								break;
							}
						}
					}
				}
				if ((this.mode == global::IndustrialConveyor.ConveyorMode.Any || this.mode == global::IndustrialConveyor.ConveyorMode.Not) && num > 0)
				{
					return true;
				}
				if (itemFilter2.MinAmountInInput > 0)
				{
					num4 = 0;
				}
			}
			if (itemFilter2.BufferTransferRemaining > 0 && num3 == 0)
			{
				itemFilter2.BufferTransferRemaining = 0;
				this.filterItems[i] = itemFilter2;
			}
		}
		if (this.mode == global::IndustrialConveyor.ConveyorMode.And && (num == this.filterItems.Count || num == num2))
		{
			if (num2 == 0)
			{
				for (int k = 0; k < this.filterItems.Count; k++)
				{
					global::IndustrialConveyor.ItemFilter itemFilter3 = this.filterItems[k];
					itemFilter3.BufferTransferRemaining = itemFilter3.BufferAmount;
					this.filterItems[k] = itemFilter3;
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x0006C708 File Offset: 0x0006A908
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.filterItems.Count == 0)
		{
			return;
		}
		info.msg.industrialConveyor = Facepunch.Pool.Get<ProtoBuf.IndustrialConveyor>();
		info.msg.industrialConveyor.filters = Facepunch.Pool.GetList<ProtoBuf.IndustrialConveyor.ItemFilter>();
		info.msg.industrialConveyor.conveyorMode = (int)this.mode;
		foreach (global::IndustrialConveyor.ItemFilter itemFilter in this.filterItems)
		{
			ProtoBuf.IndustrialConveyor.ItemFilter itemFilter2 = Facepunch.Pool.Get<ProtoBuf.IndustrialConveyor.ItemFilter>();
			itemFilter.CopyTo(itemFilter2);
			info.msg.industrialConveyor.filters.Add(itemFilter2);
		}
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x0006C7C8 File Offset: 0x0006A9C8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	private void RPC_ChangeFilters(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		this.mode = (global::IndustrialConveyor.ConveyorMode)msg.read.Int32();
		this.filterItems.Clear();
		ProtoBuf.IndustrialConveyor.ItemFilterList itemFilterList = ProtoBuf.IndustrialConveyor.ItemFilterList.Deserialize(msg.read);
		if (itemFilterList.filters == null)
		{
			return;
		}
		int num = Mathf.Min(itemFilterList.filters.Count, 24);
		int num2 = 0;
		while (num2 < num && this.filterItems.Count < 12)
		{
			global::IndustrialConveyor.ItemFilter itemFilter = new global::IndustrialConveyor.ItemFilter(itemFilterList.filters[num2]);
			if (itemFilter.TargetItem != null || itemFilter.TargetCategory != null)
			{
				this.filterItems.Add(itemFilter);
			}
			num2++;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x0006C892 File Offset: 0x0006AA92
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	private void SvSwitch(global::BaseEntity.RPCMessage msg)
	{
		this.SetSwitch(!base.IsOn());
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x0006C8A4 File Offset: 0x0006AAA4
	public virtual void SetSwitch(bool wantsOn)
	{
		if (wantsOn == base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, wantsOn, false, true);
		base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved10, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved9, false, false, true);
		if (!wantsOn)
		{
			this.lastFilterState = null;
		}
		this.ensureOutputsUpdated = true;
		base.Invoke(new Action(this.Unbusy), 0.5f);
		for (int i = 0; i < this.filterItems.Count; i++)
		{
			global::IndustrialConveyor.ItemFilter itemFilter = this.filterItems[i];
			if (itemFilter.BufferTransferRemaining > 0)
			{
				itemFilter.BufferTransferRemaining = 0;
				this.filterItems[i] = itemFilter;
			}
		}
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x0006282C File Offset: 0x00060A2C
	public void Unbusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x0006C96C File Offset: 0x0006AB6C
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (inputSlot == 1)
		{
			bool flag = inputAmount >= this.ConsumptionAmount() && inputAmount > 0;
			if (this.IsPowered() && base.IsOn() && !flag)
			{
				this.wasOnWhenPowerLost = true;
			}
			base.SetFlag(global::BaseEntity.Flags.Reserved8, flag, false, true);
			if (!flag)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved9, false, false, true);
				base.SetFlag(global::BaseEntity.Flags.Reserved10, false, false, true);
			}
			this.currentEnergy = inputAmount;
			this.ensureOutputsUpdated = true;
			if (inputAmount <= 0 && base.IsOn())
			{
				this.SetSwitch(false);
			}
			if (inputAmount > 0 && this.wasOnWhenPowerLost && !base.IsOn())
			{
				this.SetSwitch(true);
				this.wasOnWhenPowerLost = false;
			}
			this.MarkDirty();
		}
		if (inputSlot == 2 && !base.IsOn() && inputAmount > 0 && this.IsPowered())
		{
			this.SetSwitch(true);
		}
		if (inputSlot == 3 && base.IsOn() && inputAmount > 0)
		{
			this.SetSwitch(false);
		}
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x0006CA5F File Offset: 0x0006AC5F
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot == 2)
		{
			if (!base.HasFlag(global::BaseEntity.Flags.Reserved10))
			{
				return 0;
			}
			return 1;
		}
		else if (outputSlot == 3)
		{
			if (!base.HasFlag(global::BaseEntity.Flags.Reserved9))
			{
				return 0;
			}
			return 1;
		}
		else
		{
			if (outputSlot == 1)
			{
				return this.GetCurrentEnergy();
			}
			return 0;
		}
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x00007641 File Offset: 0x00005841
	public override bool ShouldDrainBattery(global::IOEntity battery)
	{
		return base.IsOn();
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x0006CA98 File Offset: 0x0006AC98
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	private void Server_RequestUpToDateFilters(global::BaseEntity.RPCMessage msg)
	{
		if (!base.IsOn())
		{
			return;
		}
		using (ProtoBuf.IndustrialConveyor.ItemFilterList itemFilterList = Facepunch.Pool.Get<ProtoBuf.IndustrialConveyor.ItemFilterList>())
		{
			itemFilterList.filters = Facepunch.Pool.GetList<ProtoBuf.IndustrialConveyor.ItemFilter>();
			foreach (global::IndustrialConveyor.ItemFilter itemFilter in this.filterItems)
			{
				ProtoBuf.IndustrialConveyor.ItemFilter itemFilter2 = Facepunch.Pool.Get<ProtoBuf.IndustrialConveyor.ItemFilter>();
				itemFilter.CopyTo(itemFilter2);
				itemFilterList.filters.Add(itemFilter2);
			}
			base.ClientRPCPlayer<ProtoBuf.IndustrialConveyor.ItemFilterList>(null, msg.player, "Client_ReceiveBufferInfo", itemFilterList);
		}
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x0006CB44 File Offset: 0x0006AD44
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.filterItems.Clear();
		ProtoBuf.IndustrialConveyor industrialConveyor = info.msg.industrialConveyor;
		if (((industrialConveyor != null) ? industrialConveyor.filters : null) != null)
		{
			this.mode = (global::IndustrialConveyor.ConveyorMode)info.msg.industrialConveyor.conveyorMode;
			foreach (ProtoBuf.IndustrialConveyor.ItemFilter from in info.msg.industrialConveyor.filters)
			{
				global::IndustrialConveyor.ItemFilter itemFilter = new global::IndustrialConveyor.ItemFilter(from);
				if (itemFilter.TargetItem != null || itemFilter.TargetCategory != null)
				{
					this.filterItems.Add(itemFilter);
				}
			}
		}
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x0006CC4C File Offset: 0x0006AE4C
	[CompilerGenerated]
	internal static bool <RunJob>g__FilterMatchItem|28_1(global::IndustrialConveyor.ItemFilter filter, global::Item item)
	{
		return filter.TargetItem != null && (filter.TargetItem == item.info || (item.IsBlueprint() == filter.IsBlueprint && filter.TargetItem == item.blueprintTargetDef));
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x0006CCA0 File Offset: 0x0006AEA0
	[CompilerGenerated]
	internal static void <RunJob>g__AddTransfer|28_2(int itemId, int amount, global::BaseEntity fromEntity, global::BaseEntity toEntity, ref global::IndustrialConveyor.<>c__DisplayClass28_1 A_4)
	{
		if (A_4.transfer == null || A_4.transfer.ItemTransfers == null)
		{
			return;
		}
		if (fromEntity != null && !A_4.transfer.inputEntities.Contains(fromEntity.net.ID))
		{
			A_4.transfer.inputEntities.Add(fromEntity.net.ID);
		}
		if (toEntity != null && !A_4.transfer.outputEntities.Contains(toEntity.net.ID))
		{
			A_4.transfer.outputEntities.Add(toEntity.net.ID);
		}
		for (int i = 0; i < A_4.transfer.ItemTransfers.Count; i++)
		{
			IndustrialConveyorTransfer.ItemTransfer itemTransfer = A_4.transfer.ItemTransfers[i];
			if (itemTransfer.itemId == itemId)
			{
				itemTransfer.amount += amount;
				A_4.transfer.ItemTransfers[i] = itemTransfer;
				return;
			}
		}
		IndustrialConveyorTransfer.ItemTransfer item = new IndustrialConveyorTransfer.ItemTransfer
		{
			itemId = itemId,
			amount = amount
		};
		A_4.transfer.ItemTransfers.Add(item);
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x0006CDD0 File Offset: 0x0006AFD0
	[CompilerGenerated]
	private void <RunJob>g__UpdateFilterPassthroughs|28_0(ref global::IndustrialConveyor.<>c__DisplayClass28_0 A_1)
	{
		this.lastFilterState = new bool?(A_1.hasItems);
		base.SetFlag(global::BaseEntity.Flags.Reserved9, A_1.hasItems, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved10, !A_1.hasItems, false, true);
		this.ensureOutputsUpdated = true;
		this.MarkDirty();
	}

	// Token: 0x02000BD3 RID: 3027
	public enum ConveyorMode
	{
		// Token: 0x040040F6 RID: 16630
		Any,
		// Token: 0x040040F7 RID: 16631
		And,
		// Token: 0x040040F8 RID: 16632
		Not
	}

	// Token: 0x02000BD4 RID: 3028
	public struct ItemFilter
	{
		// Token: 0x040040F9 RID: 16633
		[JsonIgnore]
		public ItemDefinition TargetItem;

		// Token: 0x040040FA RID: 16634
		public ItemCategory? TargetCategory;

		// Token: 0x040040FB RID: 16635
		public int MaxAmountInOutput;

		// Token: 0x040040FC RID: 16636
		public int BufferAmount;

		// Token: 0x040040FD RID: 16637
		public int MinAmountInInput;

		// Token: 0x040040FE RID: 16638
		public bool IsBlueprint;

		// Token: 0x040040FF RID: 16639
		public int BufferTransferRemaining;

		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x06004DA6 RID: 19878 RVA: 0x001A12A3 File Offset: 0x0019F4A3
		// (set) Token: 0x06004DA7 RID: 19879 RVA: 0x001A12C4 File Offset: 0x0019F4C4
		public string TargetItemName
		{
			get
			{
				if (!(this.TargetItem != null))
				{
					return string.Empty;
				}
				return this.TargetItem.shortname;
			}
			set
			{
				this.TargetItem = ItemManager.FindItemDefinition(value);
			}
		}

		// Token: 0x06004DA8 RID: 19880 RVA: 0x001A12D4 File Offset: 0x0019F4D4
		public void CopyTo(ProtoBuf.IndustrialConveyor.ItemFilter target)
		{
			if (this.TargetItem != null)
			{
				target.itemDef = this.TargetItem.itemid;
			}
			target.maxAmountInDestination = this.MaxAmountInOutput;
			if (this.TargetCategory != null)
			{
				target.itemCategory = (int)this.TargetCategory.Value;
			}
			else
			{
				target.itemCategory = -1;
			}
			target.isBlueprint = (this.IsBlueprint ? 1 : 0);
			target.bufferAmount = this.BufferAmount;
			target.retainMinimum = this.MinAmountInInput;
			target.bufferTransferRemaining = this.BufferTransferRemaining;
		}

		// Token: 0x06004DA9 RID: 19881 RVA: 0x001A136C File Offset: 0x0019F56C
		public ItemFilter(ProtoBuf.IndustrialConveyor.ItemFilter from)
		{
			this = new global::IndustrialConveyor.ItemFilter
			{
				TargetItem = ItemManager.FindItemDefinition(from.itemDef),
				MaxAmountInOutput = from.maxAmountInDestination
			};
			if (from.itemCategory >= 0)
			{
				this.TargetCategory = new ItemCategory?((ItemCategory)from.itemCategory);
			}
			else
			{
				this.TargetCategory = null;
			}
			this.IsBlueprint = (from.isBlueprint == 1);
			this.BufferAmount = from.bufferAmount;
			this.MinAmountInInput = from.retainMinimum;
			this.BufferTransferRemaining = from.bufferTransferRemaining;
		}
	}
}
