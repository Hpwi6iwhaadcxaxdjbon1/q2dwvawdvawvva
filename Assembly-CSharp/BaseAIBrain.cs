using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000036 RID: 54
public class BaseAIBrain : EntityComponent<global::BaseEntity>, IPet, IAISleepable, IAIDesign, IAIGroupable, IAIEventListener
{
	// Token: 0x040001E3 RID: 483
	public bool SendClientCurrentState;

	// Token: 0x040001E4 RID: 484
	public bool UseQueuedMovementUpdates;

	// Token: 0x040001E5 RID: 485
	public bool AllowedToSleep = true;

	// Token: 0x040001E6 RID: 486
	public AIDesignSO DefaultDesignSO;

	// Token: 0x040001E7 RID: 487
	public List<AIDesignSO> Designs = new List<AIDesignSO>();

	// Token: 0x040001E8 RID: 488
	public ProtoBuf.AIDesign InstanceSpecificDesign;

	// Token: 0x040001E9 RID: 489
	public float SenseRange = 10f;

	// Token: 0x040001EA RID: 490
	public float AttackRangeMultiplier = 1f;

	// Token: 0x040001EB RID: 491
	public float TargetLostRange = 40f;

	// Token: 0x040001EC RID: 492
	public float VisionCone = -0.8f;

	// Token: 0x040001ED RID: 493
	public bool CheckVisionCone;

	// Token: 0x040001EE RID: 494
	public bool CheckLOS;

	// Token: 0x040001EF RID: 495
	public bool IgnoreNonVisionSneakers = true;

	// Token: 0x040001F0 RID: 496
	public float IgnoreSneakersMaxDistance = 4f;

	// Token: 0x040001F1 RID: 497
	public float IgnoreNonVisionMaxDistance = 15f;

	// Token: 0x040001F2 RID: 498
	public float ListenRange;

	// Token: 0x040001F3 RID: 499
	public EntityType SenseTypes;

	// Token: 0x040001F4 RID: 500
	public bool HostileTargetsOnly;

	// Token: 0x040001F5 RID: 501
	public bool IgnoreSafeZonePlayers;

	// Token: 0x040001F6 RID: 502
	public int MaxGroupSize;

	// Token: 0x040001F7 RID: 503
	public float MemoryDuration = 10f;

	// Token: 0x040001F8 RID: 504
	public bool RefreshKnownLOS;

	// Token: 0x040001F9 RID: 505
	public bool CanBeBlinded = true;

	// Token: 0x040001FA RID: 506
	public float BlindDurationMultiplier = 1f;

	// Token: 0x040001FC RID: 508
	public AIState ClientCurrentState;

	// Token: 0x040001FD RID: 509
	public Vector3 mainInterestPoint;

	// Token: 0x04000202 RID: 514
	public bool UseAIDesign;

	// Token: 0x0400020A RID: 522
	public bool Pet;

	// Token: 0x0400020B RID: 523
	private List<IAIGroupable> groupMembers = new List<IAIGroupable>();

	// Token: 0x0400020C RID: 524
	[Header("Healing")]
	public bool CanUseHealingItems;

	// Token: 0x0400020D RID: 525
	public float HealChance = 0.5f;

	// Token: 0x0400020E RID: 526
	public float HealBelowHealthFraction = 0.5f;

	// Token: 0x0400020F RID: 527
	protected int loadedDesignIndex;

	// Token: 0x04000211 RID: 529
	private int currentStateContainerID = -1;

	// Token: 0x04000212 RID: 530
	private float lastMovementTickTime;

	// Token: 0x04000213 RID: 531
	private bool sleeping;

	// Token: 0x04000214 RID: 532
	private bool disabled;

	// Token: 0x04000215 RID: 533
	protected Dictionary<AIState, BaseAIBrain.BasicAIState> states;

	// Token: 0x04000216 RID: 534
	protected float thinkRate = 0.25f;

	// Token: 0x04000217 RID: 535
	protected float lastThinkTime;

	// Token: 0x04000218 RID: 536
	protected float unblindTime;

	// Token: 0x060001DA RID: 474 RVA: 0x000258E8 File Offset: 0x00023AE8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseAIBrain.OnRpcMessage", 0))
		{
			if (rpc == 66191493U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestAIDesign ");
				}
				using (TimeWarning.New("RequestAIDesign", 0))
				{
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
							this.RequestAIDesign(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RequestAIDesign");
					}
				}
				return true;
			}
			if (rpc == 2122228512U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StopAIDesign ");
				}
				using (TimeWarning.New("StopAIDesign", 0))
				{
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
							this.StopAIDesign(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in StopAIDesign");
					}
				}
				return true;
			}
			if (rpc == 657290375U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SubmitAIDesign ");
				}
				using (TimeWarning.New("SubmitAIDesign", 0))
				{
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
							this.SubmitAIDesign(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in SubmitAIDesign");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060001DB RID: 475 RVA: 0x00025C54 File Offset: 0x00023E54
	public bool IsPet()
	{
		return this.Pet;
	}

	// Token: 0x060001DC RID: 476 RVA: 0x00025C5C File Offset: 0x00023E5C
	public void SetPetOwner(global::BasePlayer player)
	{
		global::BaseEntity baseEntity = this.GetBaseEntity();
		player.PetEntity = baseEntity;
		baseEntity.OwnerID = player.userID;
		BasePet.ActivePetByOwnerID[player.userID] = (baseEntity as BasePet);
	}

	// Token: 0x060001DD RID: 477 RVA: 0x00025C99 File Offset: 0x00023E99
	public bool IsOwnedBy(global::BasePlayer player)
	{
		return !(this.OwningPlayer == null) && !(player == null) && this != null && this.OwningPlayer == player;
	}

	// Token: 0x060001DE RID: 478 RVA: 0x00025CC8 File Offset: 0x00023EC8
	public bool IssuePetCommand(PetCommandType cmd, int param, Ray? ray)
	{
		if (ray != null)
		{
			int layerMask = 10551296;
			RaycastHit raycastHit;
			if (UnityEngine.Physics.Raycast(ray.Value, out raycastHit, 75f, layerMask))
			{
				this.Events.Memory.Position.Set(raycastHit.point, 6);
			}
			else
			{
				this.Events.Memory.Position.Set(base.transform.position, 6);
			}
		}
		switch (cmd)
		{
		case PetCommandType.LoadDesign:
			if (param < 0 || param >= this.Designs.Count)
			{
				return false;
			}
			this.LoadAIDesign(AIDesigns.GetByNameOrInstance(this.Designs[param].Filename, this.InstanceSpecificDesign), null, param);
			return true;
		case PetCommandType.SetState:
		{
			global::AIStateContainer stateContainerByID = this.AIDesign.GetStateContainerByID(param);
			return stateContainerByID != null && this.SwitchToState(stateContainerByID.State, param);
		}
		case PetCommandType.Destroy:
			this.GetBaseEntity().Kill(global::BaseNetworkable.DestroyMode.None);
			return true;
		default:
			return false;
		}
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x060001DF RID: 479 RVA: 0x00025DB8 File Offset: 0x00023FB8
	// (set) Token: 0x060001E0 RID: 480 RVA: 0x00025DC0 File Offset: 0x00023FC0
	public BaseAIBrain.BasicAIState CurrentState { get; private set; }

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x060001E1 RID: 481 RVA: 0x00025DC9 File Offset: 0x00023FC9
	// (set) Token: 0x060001E2 RID: 482 RVA: 0x00025DD1 File Offset: 0x00023FD1
	public AIThinkMode ThinkMode { get; protected set; } = AIThinkMode.Interval;

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x060001E3 RID: 483 RVA: 0x00025DDA File Offset: 0x00023FDA
	// (set) Token: 0x060001E4 RID: 484 RVA: 0x00025DE2 File Offset: 0x00023FE2
	public float Age { get; private set; }

	// Token: 0x060001E5 RID: 485 RVA: 0x00025DEB File Offset: 0x00023FEB
	public void ForceSetAge(float age)
	{
		this.Age = age;
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x060001E6 RID: 486 RVA: 0x00025DF4 File Offset: 0x00023FF4
	// (set) Token: 0x060001E7 RID: 487 RVA: 0x00025DFC File Offset: 0x00023FFC
	public AIBrainSenses Senses { get; private set; } = new AIBrainSenses();

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x060001E8 RID: 488 RVA: 0x00025E05 File Offset: 0x00024005
	// (set) Token: 0x060001E9 RID: 489 RVA: 0x00025E0D File Offset: 0x0002400D
	public BasePathFinder PathFinder { get; protected set; }

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060001EA RID: 490 RVA: 0x00025E16 File Offset: 0x00024016
	// (set) Token: 0x060001EB RID: 491 RVA: 0x00025E1E File Offset: 0x0002401E
	public AIEvents Events { get; private set; }

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060001EC RID: 492 RVA: 0x00025E27 File Offset: 0x00024027
	// (set) Token: 0x060001ED RID: 493 RVA: 0x00025E2F File Offset: 0x0002402F
	public global::AIDesign AIDesign { get; private set; }

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060001EE RID: 494 RVA: 0x00025E38 File Offset: 0x00024038
	// (set) Token: 0x060001EF RID: 495 RVA: 0x00025E40 File Offset: 0x00024040
	public global::BasePlayer DesigningPlayer { get; private set; }

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x060001F0 RID: 496 RVA: 0x00025E49 File Offset: 0x00024049
	// (set) Token: 0x060001F1 RID: 497 RVA: 0x00025E51 File Offset: 0x00024051
	public global::BasePlayer OwningPlayer { get; private set; }

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x060001F2 RID: 498 RVA: 0x00025E5A File Offset: 0x0002405A
	// (set) Token: 0x060001F3 RID: 499 RVA: 0x00025E62 File Offset: 0x00024062
	public bool IsGroupLeader { get; private set; }

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060001F4 RID: 500 RVA: 0x00025E6B File Offset: 0x0002406B
	// (set) Token: 0x060001F5 RID: 501 RVA: 0x00025E73 File Offset: 0x00024073
	public bool IsGrouped { get; private set; }

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060001F6 RID: 502 RVA: 0x00025E7C File Offset: 0x0002407C
	// (set) Token: 0x060001F7 RID: 503 RVA: 0x00025E84 File Offset: 0x00024084
	public IAIGroupable GroupLeader { get; private set; }

	// Token: 0x060001F8 RID: 504 RVA: 0x00025E8D File Offset: 0x0002408D
	public int LoadedDesignIndex()
	{
		return this.loadedDesignIndex;
	}

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x060001F9 RID: 505 RVA: 0x00025E95 File Offset: 0x00024095
	// (set) Token: 0x060001FA RID: 506 RVA: 0x00025E9D File Offset: 0x0002409D
	public BaseNavigator Navigator { get; private set; }

	// Token: 0x060001FB RID: 507 RVA: 0x00025EA6 File Offset: 0x000240A6
	public void SetEnabled(bool flag)
	{
		this.disabled = !flag;
	}

	// Token: 0x060001FC RID: 508 RVA: 0x00025EB2 File Offset: 0x000240B2
	bool IAIDesign.CanPlayerDesignAI(global::BasePlayer player)
	{
		return this.PlayerCanDesignAI(player);
	}

	// Token: 0x060001FD RID: 509 RVA: 0x00025EBB File Offset: 0x000240BB
	private bool PlayerCanDesignAI(global::BasePlayer player)
	{
		return AI.allowdesigning && !(player == null) && this.UseAIDesign && !(this.DesigningPlayer != null) && player.IsDeveloper;
	}

	// Token: 0x060001FE RID: 510 RVA: 0x00025EF8 File Offset: 0x000240F8
	[global::BaseEntity.RPC_Server]
	private void RequestAIDesign(global::BaseEntity.RPCMessage msg)
	{
		if (!this.UseAIDesign)
		{
			return;
		}
		if (msg.player == null)
		{
			return;
		}
		if (this.AIDesign == null)
		{
			return;
		}
		if (!this.PlayerCanDesignAI(msg.player))
		{
			return;
		}
		msg.player.designingAIEntity = this.GetBaseEntity();
		msg.player.ClientRPCPlayer<ProtoBuf.AIDesign>(null, msg.player, "StartDesigningAI", this.AIDesign.ToProto(this.currentStateContainerID));
		this.DesigningPlayer = msg.player;
		this.SetOwningPlayer(msg.player);
	}

	// Token: 0x060001FF RID: 511 RVA: 0x00025F88 File Offset: 0x00024188
	[global::BaseEntity.RPC_Server]
	private void SubmitAIDesign(global::BaseEntity.RPCMessage msg)
	{
		ProtoBuf.AIDesign aidesign = ProtoBuf.AIDesign.Deserialize(msg.read);
		if (!this.LoadAIDesign(aidesign, msg.player, this.loadedDesignIndex))
		{
			return;
		}
		this.SaveDesign();
		if (aidesign.scope == 2)
		{
			return;
		}
		global::BaseEntity baseEntity = this.GetBaseEntity();
		global::BaseEntity[] array = global::BaseEntity.Util.FindTargets(baseEntity.ShortPrefabName, false);
		if (array == null || array.Length == 0)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity2 in array)
		{
			if (!(baseEntity2 == null) && !(baseEntity2 == baseEntity))
			{
				EntityComponentBase[] components = baseEntity2.Components;
				if (components != null)
				{
					EntityComponentBase[] array3 = components;
					for (int j = 0; j < array3.Length; j++)
					{
						IAIDesign iaidesign;
						if ((iaidesign = (array3[j] as IAIDesign)) != null)
						{
							iaidesign.LoadAIDesign(aidesign, null);
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x06000200 RID: 512 RVA: 0x00026051 File Offset: 0x00024251
	void IAIDesign.StopDesigning()
	{
		this.ClearDesigningPlayer();
	}

	// Token: 0x06000201 RID: 513 RVA: 0x00026059 File Offset: 0x00024259
	void IAIDesign.LoadAIDesign(ProtoBuf.AIDesign design, global::BasePlayer player)
	{
		this.LoadAIDesign(design, player, this.loadedDesignIndex);
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0002606A File Offset: 0x0002426A
	public bool LoadDefaultAIDesign()
	{
		return this.loadedDesignIndex == 0 || this.LoadAIDesignAtIndex(0);
	}

	// Token: 0x06000203 RID: 515 RVA: 0x00026080 File Offset: 0x00024280
	public bool LoadAIDesignAtIndex(int index)
	{
		return this.Designs != null && index >= 0 && index < this.Designs.Count && this.LoadAIDesign(AIDesigns.GetByNameOrInstance(this.Designs[index].Filename, this.InstanceSpecificDesign), null, index);
	}

	// Token: 0x06000204 RID: 516 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnAIDesignLoadedAtIndex(int index)
	{
	}

	// Token: 0x06000205 RID: 517 RVA: 0x000260D0 File Offset: 0x000242D0
	protected bool LoadAIDesign(ProtoBuf.AIDesign design, global::BasePlayer player, int index)
	{
		if (design == null)
		{
			Debug.LogError(this.GetBaseEntity().gameObject.name + " failed to load AI design!");
			return false;
		}
		if (player != null)
		{
			AIDesignScope scope = (AIDesignScope)design.scope;
			if (scope == AIDesignScope.Default && !player.IsDeveloper)
			{
				return false;
			}
			if (scope == AIDesignScope.EntityServerWide && !player.IsDeveloper && !player.IsAdmin)
			{
				return false;
			}
		}
		if (this.AIDesign == null)
		{
			return false;
		}
		this.AIDesign.Load(design, base.baseEntity);
		global::AIStateContainer defaultStateContainer = this.AIDesign.GetDefaultStateContainer();
		if (defaultStateContainer != null)
		{
			this.SwitchToState(defaultStateContainer.State, defaultStateContainer.ID);
		}
		this.loadedDesignIndex = index;
		this.OnAIDesignLoadedAtIndex(this.loadedDesignIndex);
		return true;
	}

	// Token: 0x06000206 RID: 518 RVA: 0x00026188 File Offset: 0x00024388
	public void SaveDesign()
	{
		if (this.AIDesign == null)
		{
			return;
		}
		ProtoBuf.AIDesign aidesign = this.AIDesign.ToProto(this.currentStateContainerID);
		string text = "cfg/ai/";
		string text2 = this.Designs[this.loadedDesignIndex].Filename;
		switch (this.AIDesign.Scope)
		{
		case AIDesignScope.Default:
			text += text2;
			try
			{
				using (FileStream fileStream = File.Create(text))
				{
					ProtoBuf.AIDesign.Serialize(fileStream, aidesign);
				}
				AIDesigns.RefreshCache(text2, aidesign);
				return;
			}
			catch (Exception)
			{
				Debug.LogWarning("Error trying to save default AI Design: " + text);
				return;
			}
			break;
		case AIDesignScope.EntityServerWide:
			break;
		case AIDesignScope.EntityInstance:
			return;
		default:
			return;
		}
		text2 += "_custom";
		text += text2;
		try
		{
			using (FileStream fileStream2 = File.Create(text))
			{
				ProtoBuf.AIDesign.Serialize(fileStream2, aidesign);
			}
			AIDesigns.RefreshCache(text2, aidesign);
		}
		catch (Exception)
		{
			Debug.LogWarning("Error trying to save server-wide AI Design: " + text);
		}
	}

	// Token: 0x06000207 RID: 519 RVA: 0x000262B0 File Offset: 0x000244B0
	[global::BaseEntity.RPC_Server]
	private void StopAIDesign(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == this.DesigningPlayer)
		{
			this.ClearDesigningPlayer();
		}
	}

	// Token: 0x06000208 RID: 520 RVA: 0x000262CB File Offset: 0x000244CB
	private void ClearDesigningPlayer()
	{
		this.DesigningPlayer = null;
	}

	// Token: 0x06000209 RID: 521 RVA: 0x000262D4 File Offset: 0x000244D4
	public void SetOwningPlayer(global::BasePlayer owner)
	{
		this.OwningPlayer = owner;
		this.Events.Memory.Entity.Set(this.OwningPlayer, 5);
		if (this != null && ((IPet)this).IsPet())
		{
			((IPet)this).SetPetOwner(owner);
			owner.Pet = this;
		}
	}

	// Token: 0x0600020A RID: 522 RVA: 0x0002631F File Offset: 0x0002451F
	public virtual bool ShouldServerThink()
	{
		return this.ThinkMode == AIThinkMode.Interval && UnityEngine.Time.time > this.lastThinkTime + this.thinkRate;
	}

	// Token: 0x0600020B RID: 523 RVA: 0x00026344 File Offset: 0x00024544
	public virtual void DoThink()
	{
		float delta = UnityEngine.Time.time - this.lastThinkTime;
		this.Think(delta);
	}

	// Token: 0x0600020C RID: 524 RVA: 0x00026365 File Offset: 0x00024565
	public List<AIState> GetStateList()
	{
		return this.states.Keys.ToList<AIState>();
	}

	// Token: 0x0600020D RID: 525 RVA: 0x00026377 File Offset: 0x00024577
	public bool Blinded()
	{
		return UnityEngine.Time.time < this.unblindTime;
	}

	// Token: 0x0600020E RID: 526 RVA: 0x00026388 File Offset: 0x00024588
	public void SetBlinded(float duration)
	{
		if (!this.CanBeBlinded)
		{
			return;
		}
		if (this.Blinded())
		{
			return;
		}
		this.unblindTime = UnityEngine.Time.time + duration;
		if (this.HasState(AIState.Blinded) && this.AIDesign != null)
		{
			BaseAIBrain.BasicAIState basicAIState = this.states[AIState.Blinded];
			global::AIStateContainer firstStateContainerOfType = this.AIDesign.GetFirstStateContainerOfType(AIState.Blinded);
			if (basicAIState != null && firstStateContainerOfType != null)
			{
				this.SwitchToState(basicAIState, firstStateContainerOfType.ID);
			}
		}
	}

	// Token: 0x0600020F RID: 527 RVA: 0x000263F6 File Offset: 0x000245F6
	public void Start()
	{
		this.AddStates();
		this.InitializeAI();
	}

	// Token: 0x06000210 RID: 528 RVA: 0x00026404 File Offset: 0x00024604
	public virtual void AddStates()
	{
		this.states = new Dictionary<AIState, BaseAIBrain.BasicAIState>();
	}

	// Token: 0x06000211 RID: 529 RVA: 0x00026414 File Offset: 0x00024614
	public virtual void InitializeAI()
	{
		global::BaseEntity baseEntity = this.GetBaseEntity();
		baseEntity.HasBrain = true;
		this.Navigator = base.GetComponent<BaseNavigator>();
		if (this.UseAIDesign)
		{
			this.AIDesign = new global::AIDesign();
			this.AIDesign.SetAvailableStates(this.GetStateList());
			if (this.Events == null)
			{
				this.Events = new AIEvents();
			}
			bool senseFriendlies = this.MaxGroupSize > 0;
			this.Senses.Init(baseEntity, this, this.MemoryDuration, this.SenseRange, this.TargetLostRange, this.VisionCone, this.CheckVisionCone, this.CheckLOS, this.IgnoreNonVisionSneakers, this.ListenRange, this.HostileTargetsOnly, senseFriendlies, this.IgnoreSafeZonePlayers, this.SenseTypes, this.RefreshKnownLOS);
			if (this.DefaultDesignSO == null && this.Designs.Count == 0)
			{
				Debug.LogWarning("Brain on " + base.gameObject.name + " is trying to load a null AI design!");
				return;
			}
			this.Events.Memory.Position.Set(base.transform.position, 4);
			if (this.Designs.Count == 0)
			{
				this.Designs.Add(this.DefaultDesignSO);
			}
			this.loadedDesignIndex = 0;
			this.LoadAIDesign(AIDesigns.GetByNameOrInstance(this.Designs[this.loadedDesignIndex].Filename, this.InstanceSpecificDesign), null, this.loadedDesignIndex);
			AIInformationZone forPoint = AIInformationZone.GetForPoint(base.transform.position, false);
			if (forPoint != null)
			{
				forPoint.RegisterSleepableEntity(this);
			}
		}
		global::BaseEntity.Query.Server.AddBrain(baseEntity);
		this.StartMovementTick();
	}

	// Token: 0x06000212 RID: 530 RVA: 0x000265B8 File Offset: 0x000247B8
	public global::BaseEntity GetBrainBaseEntity()
	{
		return this.GetBaseEntity();
	}

	// Token: 0x06000213 RID: 531 RVA: 0x000265C0 File Offset: 0x000247C0
	public virtual void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		global::BaseEntity.Query.Server.RemoveBrain(this.GetBaseEntity());
		AIInformationZone aiinformationZone = null;
		HumanNPC humanNPC = this.GetBaseEntity() as HumanNPC;
		if (humanNPC != null)
		{
			aiinformationZone = humanNPC.VirtualInfoZone;
		}
		if (aiinformationZone == null)
		{
			aiinformationZone = AIInformationZone.GetForPoint(base.transform.position, true);
		}
		if (aiinformationZone != null)
		{
			aiinformationZone.UnregisterSleepableEntity(this);
		}
		this.LeaveGroup();
	}

	// Token: 0x06000214 RID: 532 RVA: 0x00026634 File Offset: 0x00024834
	private void StartMovementTick()
	{
		base.CancelInvoke(new Action(this.TickMovement));
		base.InvokeRandomized(new Action(this.TickMovement), 1f, 0.1f, 0.010000001f);
	}

	// Token: 0x06000215 RID: 533 RVA: 0x00026669 File Offset: 0x00024869
	private void StopMovementTick()
	{
		base.CancelInvoke(new Action(this.TickMovement));
	}

	// Token: 0x06000216 RID: 534 RVA: 0x00026680 File Offset: 0x00024880
	public void TickMovement()
	{
		if (BasePet.queuedMovementsAllowed && this.UseQueuedMovementUpdates && this.Navigator != null)
		{
			if (BasePet.onlyQueueBaseNavMovements && this.Navigator.CurrentNavigationType != BaseNavigator.NavigationType.Base)
			{
				this.DoMovementTick();
				return;
			}
			BasePet basePet = this.GetBaseEntity() as BasePet;
			if (basePet != null && !basePet.inQueue)
			{
				BasePet._movementProcessQueue.Enqueue(basePet);
				basePet.inQueue = true;
				return;
			}
		}
		else
		{
			this.DoMovementTick();
		}
	}

	// Token: 0x06000217 RID: 535 RVA: 0x000266FC File Offset: 0x000248FC
	public void DoMovementTick()
	{
		float delta = UnityEngine.Time.realtimeSinceStartup - this.lastMovementTickTime;
		this.lastMovementTickTime = UnityEngine.Time.realtimeSinceStartup;
		if (this.Navigator != null)
		{
			this.Navigator.Think(delta);
		}
	}

	// Token: 0x06000218 RID: 536 RVA: 0x0002673C File Offset: 0x0002493C
	public void AddState(BaseAIBrain.BasicAIState newState)
	{
		if (this.states.ContainsKey(newState.StateType))
		{
			Debug.LogWarning("Trying to add duplicate state: " + newState.StateType.ToString() + " to " + this.GetBaseEntity().PrefabName);
			return;
		}
		newState.brain = this;
		newState.Reset();
		this.states.Add(newState.StateType, newState);
	}

	// Token: 0x06000219 RID: 537 RVA: 0x000267AF File Offset: 0x000249AF
	public bool HasState(AIState state)
	{
		return this.states.ContainsKey(state);
	}

	// Token: 0x0600021A RID: 538 RVA: 0x000267BD File Offset: 0x000249BD
	protected bool SwitchToState(AIState newState, int stateContainerID = -1)
	{
		if (!this.HasState(newState))
		{
			return false;
		}
		bool flag = this.SwitchToState(this.states[newState], stateContainerID);
		if (flag)
		{
			this.OnStateChanged();
		}
		return flag;
	}

	// Token: 0x0600021B RID: 539 RVA: 0x000267E8 File Offset: 0x000249E8
	private bool SwitchToState(BaseAIBrain.BasicAIState newState, int stateContainerID = -1)
	{
		if (newState == null || !newState.CanEnter())
		{
			return false;
		}
		if (this.CurrentState != null)
		{
			if (!this.CurrentState.CanLeave())
			{
				return false;
			}
			if (this.CurrentState == newState && !this.UseAIDesign)
			{
				return false;
			}
			this.CurrentState.StateLeave(this, this.GetBaseEntity());
		}
		this.AddEvents(stateContainerID);
		this.CurrentState = newState;
		this.CurrentState.StateEnter(this, this.GetBaseEntity());
		this.currentStateContainerID = stateContainerID;
		return true;
	}

	// Token: 0x0600021C RID: 540 RVA: 0x00026868 File Offset: 0x00024A68
	protected virtual void OnStateChanged()
	{
		if (this.SendClientCurrentState)
		{
			global::BaseEntity baseEntity = this.GetBaseEntity();
			if (baseEntity != null)
			{
				baseEntity.ClientRPC<int>(null, "ClientChangeState", (int)((this.CurrentState != null) ? this.CurrentState.StateType : AIState.None));
			}
		}
	}

	// Token: 0x0600021D RID: 541 RVA: 0x000268AF File Offset: 0x00024AAF
	private void AddEvents(int stateContainerID)
	{
		if (!this.UseAIDesign)
		{
			return;
		}
		if (this.AIDesign == null)
		{
			return;
		}
		this.Events.Init(this, this.AIDesign.GetStateContainerByID(stateContainerID), base.baseEntity, this.Senses);
	}

	// Token: 0x0600021E RID: 542 RVA: 0x000268E8 File Offset: 0x00024AE8
	public virtual void Think(float delta)
	{
		if (!AI.think)
		{
			return;
		}
		this.lastThinkTime = UnityEngine.Time.time;
		if (this.sleeping || this.disabled)
		{
			return;
		}
		this.Age += delta;
		if (this.UseAIDesign)
		{
			this.Senses.Update();
			this.UpdateGroup();
		}
		if (this.CurrentState != null)
		{
			this.UpdateAgressionTimer(delta);
			StateStatus stateStatus = this.CurrentState.StateThink(delta, this, this.GetBaseEntity());
			if (this.Events != null)
			{
				this.Events.Tick(delta, stateStatus);
			}
		}
		if (!this.UseAIDesign && (this.CurrentState == null || this.CurrentState.CanLeave()))
		{
			float num = 0f;
			BaseAIBrain.BasicAIState basicAIState = null;
			foreach (BaseAIBrain.BasicAIState basicAIState2 in this.states.Values)
			{
				if (basicAIState2 != null && basicAIState2.CanEnter())
				{
					float weight = basicAIState2.GetWeight();
					if (weight > num)
					{
						num = weight;
						basicAIState = basicAIState2;
					}
				}
			}
			if (basicAIState != this.CurrentState)
			{
				this.SwitchToState(basicAIState, -1);
			}
		}
	}

	// Token: 0x0600021F RID: 543 RVA: 0x00026A18 File Offset: 0x00024C18
	private void UpdateAgressionTimer(float delta)
	{
		if (this.CurrentState == null)
		{
			this.Senses.TimeInAgressiveState = 0f;
			return;
		}
		if (this.CurrentState.AgrresiveState)
		{
			this.Senses.TimeInAgressiveState += delta;
			return;
		}
		this.Senses.TimeInAgressiveState = 0f;
	}

	// Token: 0x06000220 RID: 544 RVA: 0x00026A6F File Offset: 0x00024C6F
	bool IAISleepable.AllowedToSleep()
	{
		return this.AllowedToSleep;
	}

	// Token: 0x06000221 RID: 545 RVA: 0x00026A77 File Offset: 0x00024C77
	void IAISleepable.SleepAI()
	{
		if (this.sleeping)
		{
			return;
		}
		this.sleeping = true;
		if (this.Navigator != null)
		{
			this.Navigator.Pause();
		}
		this.StopMovementTick();
	}

	// Token: 0x06000222 RID: 546 RVA: 0x00026AA8 File Offset: 0x00024CA8
	void IAISleepable.WakeAI()
	{
		if (!this.sleeping)
		{
			return;
		}
		this.sleeping = false;
		if (this.Navigator != null)
		{
			this.Navigator.Resume();
		}
		this.StartMovementTick();
	}

	// Token: 0x06000223 RID: 547 RVA: 0x00026ADC File Offset: 0x00024CDC
	private void UpdateGroup()
	{
		if (!AI.groups)
		{
			return;
		}
		if (this.MaxGroupSize <= 0)
		{
			return;
		}
		if (!this.InGroup() && this.Senses.Memory.Friendlies.Count > 0)
		{
			IAIGroupable iaigroupable = null;
			foreach (global::BaseEntity baseEntity in this.Senses.Memory.Friendlies)
			{
				if (!(baseEntity == null))
				{
					IAIGroupable component = baseEntity.GetComponent<IAIGroupable>();
					if (component != null)
					{
						if (component.InGroup() && component.AddMember(this))
						{
							break;
						}
						if (iaigroupable == null && !component.InGroup())
						{
							iaigroupable = component;
						}
					}
				}
			}
			if (!this.InGroup() && iaigroupable != null)
			{
				this.AddMember(iaigroupable);
			}
		}
	}

	// Token: 0x06000224 RID: 548 RVA: 0x00026BB4 File Offset: 0x00024DB4
	public bool AddMember(IAIGroupable member)
	{
		if (this.InGroup() && !this.IsGroupLeader)
		{
			return this.GroupLeader.AddMember(member);
		}
		if (this.MaxGroupSize <= 0)
		{
			return false;
		}
		if (this.groupMembers.Contains(member))
		{
			return true;
		}
		if (this.groupMembers.Count + 1 >= this.MaxGroupSize)
		{
			return false;
		}
		this.groupMembers.Add(member);
		this.IsGrouped = true;
		this.IsGroupLeader = true;
		this.GroupLeader = this;
		global::BaseEntity baseEntity = this.GetBaseEntity();
		this.Events.Memory.Entity.Set(baseEntity, 6);
		member.JoinGroup(this, baseEntity);
		return true;
	}

	// Token: 0x06000225 RID: 549 RVA: 0x00026C58 File Offset: 0x00024E58
	public void JoinGroup(IAIGroupable leader, global::BaseEntity leaderEntity)
	{
		this.Events.Memory.Entity.Set(leaderEntity, 6);
		this.GroupLeader = leader;
		this.IsGroupLeader = false;
		this.IsGrouped = true;
	}

	// Token: 0x06000226 RID: 550 RVA: 0x00026C88 File Offset: 0x00024E88
	public void SetGroupRoamRootPosition(Vector3 rootPos)
	{
		if (this.IsGroupLeader)
		{
			foreach (IAIGroupable iaigroupable in this.groupMembers)
			{
				iaigroupable.SetGroupRoamRootPosition(rootPos);
			}
		}
		this.Events.Memory.Position.Set(rootPos, 5);
	}

	// Token: 0x06000227 RID: 551 RVA: 0x00026CF8 File Offset: 0x00024EF8
	public bool InGroup()
	{
		return this.IsGrouped;
	}

	// Token: 0x06000228 RID: 552 RVA: 0x00026D00 File Offset: 0x00024F00
	public void LeaveGroup()
	{
		if (!this.InGroup())
		{
			return;
		}
		if (!this.IsGroupLeader)
		{
			if (this.GroupLeader != null)
			{
				this.GroupLeader.RemoveMember(base.GetComponent<IAIGroupable>());
			}
			return;
		}
		if (this.groupMembers.Count == 0)
		{
			return;
		}
		IAIGroupable iaigroupable = this.groupMembers[0];
		if (iaigroupable == null)
		{
			return;
		}
		this.RemoveMember(iaigroupable);
		for (int i = this.groupMembers.Count - 1; i >= 0; i--)
		{
			IAIGroupable iaigroupable2 = this.groupMembers[i];
			if (iaigroupable2 != null && iaigroupable2 != iaigroupable)
			{
				this.RemoveMember(iaigroupable2);
				iaigroupable.AddMember(iaigroupable2);
			}
		}
		this.groupMembers.Clear();
	}

	// Token: 0x06000229 RID: 553 RVA: 0x00026DA4 File Offset: 0x00024FA4
	public void RemoveMember(IAIGroupable member)
	{
		if (member == null)
		{
			return;
		}
		if (!this.IsGroupLeader)
		{
			return;
		}
		if (!this.groupMembers.Contains(member))
		{
			return;
		}
		this.groupMembers.Remove(member);
		member.SetUngrouped();
		if (this.groupMembers.Count == 0)
		{
			this.SetUngrouped();
		}
	}

	// Token: 0x0600022A RID: 554 RVA: 0x00026DF3 File Offset: 0x00024FF3
	public void SetUngrouped()
	{
		this.IsGrouped = false;
		this.IsGroupLeader = false;
		this.GroupLeader = null;
	}

	// Token: 0x0600022B RID: 555 RVA: 0x00026E0A File Offset: 0x0002500A
	public override void LoadComponent(global::BaseNetworkable.LoadInfo info)
	{
		base.LoadComponent(info);
	}

	// Token: 0x0600022C RID: 556 RVA: 0x00026E14 File Offset: 0x00025014
	public override void SaveComponent(global::BaseNetworkable.SaveInfo info)
	{
		base.SaveComponent(info);
		if (this.SendClientCurrentState && this.CurrentState != null)
		{
			info.msg.brainComponent = Facepunch.Pool.Get<BrainComponent>();
			info.msg.brainComponent.currentState = (int)this.CurrentState.StateType;
		}
	}

	// Token: 0x0600022D RID: 557 RVA: 0x00026E63 File Offset: 0x00025063
	private void SendStateChangeEvent(int previousStateID, int newStateID, int sourceEventID)
	{
		if (this.DesigningPlayer != null)
		{
			this.DesigningPlayer.ClientRPCPlayer<int, int, int>(null, this.DesigningPlayer, "OnDebugAIEventTriggeredStateChange", previousStateID, newStateID, sourceEventID);
		}
	}

	// Token: 0x0600022E RID: 558 RVA: 0x00026E90 File Offset: 0x00025090
	public void EventTriggeredStateChange(int newStateContainerID, int sourceEventID)
	{
		if (this.AIDesign == null)
		{
			return;
		}
		if (newStateContainerID == -1)
		{
			return;
		}
		global::AIStateContainer stateContainerByID = this.AIDesign.GetStateContainerByID(newStateContainerID);
		int previousStateID = this.currentStateContainerID;
		this.SwitchToState(stateContainerByID.State, newStateContainerID);
		this.SendStateChangeEvent(previousStateID, this.currentStateContainerID, sourceEventID);
	}

	// Token: 0x02000B5C RID: 2908
	public class BaseAttackState : BaseAIBrain.BasicAIState
	{
		// Token: 0x04003ECA RID: 16074
		private IAIAttack attack;

		// Token: 0x06004CA1 RID: 19617 RVA: 0x0019EC12 File Offset: 0x0019CE12
		public BaseAttackState() : base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004CA2 RID: 19618 RVA: 0x0019EC24 File Offset: 0x0019CE24
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.attack = (entity as IAIAttack);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				Vector3 aimDirection = BaseAIBrain.BaseAttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (this.attack.CanAttack(baseEntity))
				{
					this.StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004CA3 RID: 19619 RVA: 0x0019ECD3 File Offset: 0x0019CED3
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.Stop();
			this.StopAttacking();
		}

		// Token: 0x06004CA4 RID: 19620 RVA: 0x0019ECF9 File Offset: 0x0019CEF9
		private void StopAttacking()
		{
			this.attack.StopAttacking();
		}

		// Token: 0x06004CA5 RID: 19621 RVA: 0x0019ED08 File Offset: 0x0019CF08
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (this.attack == null)
			{
				return StateStatus.Error;
			}
			if (baseEntity == null)
			{
				brain.Navigator.ClearFacingDirectionOverride();
				this.StopAttacking();
				return StateStatus.Finished;
			}
			if (brain.Senses.ignoreSafeZonePlayers)
			{
				global::BasePlayer basePlayer = baseEntity as global::BasePlayer;
				if (basePlayer != null && basePlayer.InSafeZone())
				{
					return StateStatus.Error;
				}
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, 0f))
			{
				return StateStatus.Error;
			}
			Vector3 aimDirection = BaseAIBrain.BaseAttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
			brain.Navigator.SetFacingDirectionOverride(aimDirection);
			if (this.attack.CanAttack(baseEntity))
			{
				this.StartAttacking(baseEntity);
			}
			else
			{
				this.StopAttacking();
			}
			return StateStatus.Running;
		}

		// Token: 0x06004CA6 RID: 19622 RVA: 0x0019EDFD File Offset: 0x0019CFFD
		private static Vector3 GetAimDirection(Vector3 from, Vector3 target)
		{
			return Vector3Ex.Direction2D(target, from);
		}

		// Token: 0x06004CA7 RID: 19623 RVA: 0x0019EE06 File Offset: 0x0019D006
		private void StartAttacking(global::BaseEntity entity)
		{
			this.attack.StartAttacking(entity);
		}
	}

	// Token: 0x02000B5D RID: 2909
	public class BaseBlindedState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CA8 RID: 19624 RVA: 0x0019EE15 File Offset: 0x0019D015
		public BaseBlindedState() : base(AIState.Blinded)
		{
		}
	}

	// Token: 0x02000B5E RID: 2910
	public class BaseChaseState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CA9 RID: 19625 RVA: 0x0019EE1F File Offset: 0x0019D01F
		public BaseChaseState() : base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004CAA RID: 19626 RVA: 0x0019EE30 File Offset: 0x0019D030
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004CAB RID: 19627 RVA: 0x0019EE91 File Offset: 0x0019D091
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004CAC RID: 19628 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004CAD RID: 19629 RVA: 0x0019EEB4 File Offset: 0x0019D0B4
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000B5F RID: 2911
	public class BaseCooldownState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CAE RID: 19630 RVA: 0x0019EF32 File Offset: 0x0019D132
		public BaseCooldownState() : base(AIState.Cooldown)
		{
		}
	}

	// Token: 0x02000B60 RID: 2912
	public class BaseDismountedState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CAF RID: 19631 RVA: 0x0019EF3C File Offset: 0x0019D13C
		public BaseDismountedState() : base(AIState.Dismounted)
		{
		}
	}

	// Token: 0x02000B61 RID: 2913
	public class BaseFleeState : BaseAIBrain.BasicAIState
	{
		// Token: 0x04003ECB RID: 16075
		private float nextInterval = 2f;

		// Token: 0x04003ECC RID: 16076
		private float stopFleeDistance;

		// Token: 0x06004CB0 RID: 19632 RVA: 0x0019EF46 File Offset: 0x0019D146
		public BaseFleeState() : base(AIState.Flee)
		{
		}

		// Token: 0x06004CB1 RID: 19633 RVA: 0x0019EF5C File Offset: 0x0019D15C
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				this.stopFleeDistance = UnityEngine.Random.Range(80f, 100f) + Mathf.Clamp(Vector3Ex.Distance2D(brain.Navigator.transform.position, baseEntity.transform.position), 0f, 50f);
			}
			this.FleeFrom(brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot), entity);
		}

		// Token: 0x06004CB2 RID: 19634 RVA: 0x0019F008 File Offset: 0x0019D208
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004CB3 RID: 19635 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004CB4 RID: 19636 RVA: 0x0019F018 File Offset: 0x0019D218
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				return StateStatus.Finished;
			}
			if (Vector3Ex.Distance2D(brain.Navigator.transform.position, baseEntity.transform.position) >= this.stopFleeDistance)
			{
				return StateStatus.Finished;
			}
			if ((brain.Navigator.UpdateIntervalElapsed(this.nextInterval) || !brain.Navigator.Moving) && !this.FleeFrom(baseEntity, entity))
			{
				return StateStatus.Error;
			}
			return StateStatus.Running;
		}

		// Token: 0x06004CB5 RID: 19637 RVA: 0x0019F0B8 File Offset: 0x0019D2B8
		private bool FleeFrom(global::BaseEntity fleeFromEntity, global::BaseEntity thisEntity)
		{
			if (thisEntity == null || fleeFromEntity == null)
			{
				return false;
			}
			this.nextInterval = UnityEngine.Random.Range(3f, 6f);
			Vector3 pos;
			if (!this.brain.PathFinder.GetBestFleePosition(this.brain.Navigator, this.brain.Senses, fleeFromEntity, this.brain.Events.Memory.Position.Get(4), 50f, 100f, out pos))
			{
				return false;
			}
			bool flag = this.brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			if (!flag)
			{
				this.Stop();
			}
			return flag;
		}
	}

	// Token: 0x02000B62 RID: 2914
	public class BaseFollowPathState : BaseAIBrain.BasicAIState
	{
		// Token: 0x04003ECD RID: 16077
		private AIMovePointPath path;

		// Token: 0x04003ECE RID: 16078
		private StateStatus status;

		// Token: 0x04003ECF RID: 16079
		private AIMovePoint currentTargetPoint;

		// Token: 0x04003ED0 RID: 16080
		private float currentWaitTime;

		// Token: 0x04003ED1 RID: 16081
		private AIMovePointPath.PathDirection pathDirection;

		// Token: 0x04003ED2 RID: 16082
		private int currentNodeIndex;

		// Token: 0x06004CB6 RID: 19638 RVA: 0x0019F165 File Offset: 0x0019D365
		public BaseFollowPathState() : base(AIState.FollowPath)
		{
		}

		// Token: 0x06004CB7 RID: 19639 RVA: 0x0019F170 File Offset: 0x0019D370
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			brain.Navigator.SetBrakingEnabled(false);
			this.path = brain.Navigator.Path;
			if (this.path == null)
			{
				AIInformationZone forPoint = AIInformationZone.GetForPoint(entity.ServerPosition, true);
				if (forPoint == null)
				{
					return;
				}
				this.path = forPoint.GetNearestPath(entity.ServerPosition);
				if (this.path == null)
				{
					return;
				}
			}
			this.currentNodeIndex = this.path.FindNearestPointIndex(entity.ServerPosition);
			this.currentTargetPoint = this.path.FindNearestPoint(entity.ServerPosition);
			if (this.currentTargetPoint == null)
			{
				return;
			}
			this.status = StateStatus.Running;
			this.currentWaitTime = 0f;
			brain.Navigator.SetDestination(this.currentTargetPoint.transform.position, BaseNavigator.NavigationSpeed.Slow, 0f, 0f);
		}

		// Token: 0x06004CB8 RID: 19640 RVA: 0x0019F265 File Offset: 0x0019D465
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.SetBrakingEnabled(true);
		}

		// Token: 0x06004CB9 RID: 19641 RVA: 0x0019F288 File Offset: 0x0019D488
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			if (!brain.Navigator.Moving)
			{
				if (this.currentWaitTime <= 0f && this.currentTargetPoint.HasLookAtPoints())
				{
					Transform randomLookAtPoint = this.currentTargetPoint.GetRandomLookAtPoint();
					if (randomLookAtPoint != null)
					{
						brain.Navigator.SetFacingDirectionOverride(Vector3Ex.Direction2D(randomLookAtPoint.transform.position, entity.ServerPosition));
					}
				}
				if (this.currentTargetPoint.WaitTime > 0f)
				{
					this.currentWaitTime += delta;
				}
				if (this.currentTargetPoint.WaitTime <= 0f || this.currentWaitTime >= this.currentTargetPoint.WaitTime)
				{
					brain.Navigator.ClearFacingDirectionOverride();
					this.currentWaitTime = 0f;
					int num = this.currentNodeIndex;
					this.currentNodeIndex = this.path.GetNextPointIndex(this.currentNodeIndex, ref this.pathDirection);
					this.currentTargetPoint = this.path.GetPointAtIndex(this.currentNodeIndex);
					if ((!(this.currentTargetPoint != null) || this.currentNodeIndex != num) && (this.currentTargetPoint == null || !brain.Navigator.SetDestination(this.currentTargetPoint.transform.position, BaseNavigator.NavigationSpeed.Slow, 0f, 0f)))
					{
						return StateStatus.Error;
					}
				}
			}
			else if (this.currentTargetPoint != null)
			{
				brain.Navigator.SetDestination(this.currentTargetPoint.transform.position, BaseNavigator.NavigationSpeed.Slow, 1f, 0f);
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000B63 RID: 2915
	public class BaseIdleState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CBA RID: 19642 RVA: 0x0019F42D File Offset: 0x0019D62D
		public BaseIdleState() : base(AIState.Idle)
		{
		}
	}

	// Token: 0x02000B64 RID: 2916
	public class BaseMountedState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CBB RID: 19643 RVA: 0x0019F436 File Offset: 0x0019D636
		public BaseMountedState() : base(AIState.Mounted)
		{
		}

		// Token: 0x06004CBC RID: 19644 RVA: 0x0019F43F File Offset: 0x0019D63F
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			brain.Navigator.Stop();
		}
	}

	// Token: 0x02000B65 RID: 2917
	public class BaseMoveTorwardsState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CBD RID: 19645 RVA: 0x0019F454 File Offset: 0x0019D654
		public BaseMoveTorwardsState() : base(AIState.MoveTowards)
		{
		}

		// Token: 0x06004CBE RID: 19646 RVA: 0x0019F45E File Offset: 0x0019D65E
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004CBF RID: 19647 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004CC0 RID: 19648 RVA: 0x0019F470 File Offset: 0x0019D670
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			this.FaceTarget();
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, brain.Navigator.MoveTowardsSpeed, 0.25f, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}

		// Token: 0x06004CC1 RID: 19649 RVA: 0x0019F500 File Offset: 0x0019D700
		private void FaceTarget()
		{
			if (!this.brain.Navigator.FaceMoveTowardsTarget)
			{
				return;
			}
			global::BaseEntity baseEntity = this.brain.Events.Memory.Entity.Get(this.brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.brain.Navigator.ClearFacingDirectionOverride();
				return;
			}
			if (Vector3.Distance(baseEntity.transform.position, this.brain.transform.position) <= 1.5f)
			{
				this.brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
		}
	}

	// Token: 0x02000B66 RID: 2918
	public class BaseNavigateHomeState : BaseAIBrain.BasicAIState
	{
		// Token: 0x04003ED3 RID: 16083
		private StateStatus status;

		// Token: 0x06004CC2 RID: 19650 RVA: 0x0019F59D File Offset: 0x0019D79D
		public BaseNavigateHomeState() : base(AIState.NavigateHome)
		{
		}

		// Token: 0x06004CC3 RID: 19651 RVA: 0x0019F5A8 File Offset: 0x0019D7A8
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			Vector3 pos = brain.Events.Memory.Position.Get(4);
			this.status = StateStatus.Running;
			if (!brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				this.status = StateStatus.Error;
			}
		}

		// Token: 0x06004CC4 RID: 19652 RVA: 0x0019F5FB File Offset: 0x0019D7FB
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004CC5 RID: 19653 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004CC6 RID: 19654 RVA: 0x0019F60B File Offset: 0x0019D80B
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000B67 RID: 2919
	public class BasePatrolState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CC7 RID: 19655 RVA: 0x0019F637 File Offset: 0x0019D837
		public BasePatrolState() : base(AIState.Patrol)
		{
		}
	}

	// Token: 0x02000B68 RID: 2920
	public class BaseRoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x04003ED4 RID: 16084
		private float nextRoamPositionTime = -1f;

		// Token: 0x04003ED5 RID: 16085
		private float lastDestinationTime;

		// Token: 0x06004CC8 RID: 19656 RVA: 0x0019F640 File Offset: 0x0019D840
		public BaseRoamState() : base(AIState.Roam)
		{
		}

		// Token: 0x06004CC9 RID: 19657 RVA: 0x00029CA8 File Offset: 0x00027EA8
		public override float GetWeight()
		{
			return 0f;
		}

		// Token: 0x06004CCA RID: 19658 RVA: 0x0019F654 File Offset: 0x0019D854
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.nextRoamPositionTime = -1f;
			this.lastDestinationTime = UnityEngine.Time.time;
		}

		// Token: 0x06004CCB RID: 19659 RVA: 0x0002BE49 File Offset: 0x0002A049
		public virtual Vector3 GetDestination()
		{
			return Vector3.zero;
		}

		// Token: 0x06004CCC RID: 19660 RVA: 0x0019F674 File Offset: 0x0019D874
		public virtual Vector3 GetForwardDirection()
		{
			return Vector3.forward;
		}

		// Token: 0x06004CCD RID: 19661 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void SetDestination(Vector3 destination)
		{
		}

		// Token: 0x06004CCE RID: 19662 RVA: 0x0019F67B File Offset: 0x0019D87B
		public override void DrawGizmos()
		{
			base.DrawGizmos();
			this.brain.PathFinder.DebugDraw();
		}

		// Token: 0x06004CCF RID: 19663 RVA: 0x0019F694 File Offset: 0x0019D894
		public virtual Vector3 GetRoamAnchorPosition()
		{
			if (this.brain.Navigator.MaxRoamDistanceFromHome > -1f)
			{
				return this.brain.Events.Memory.Position.Get(4);
			}
			return this.brain.GetBaseEntity().transform.position;
		}

		// Token: 0x06004CD0 RID: 19664 RVA: 0x0019F6EC File Offset: 0x0019D8EC
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			bool flag = UnityEngine.Time.time - this.lastDestinationTime > 25f;
			if ((Vector3.Distance(this.GetDestination(), entity.transform.position) < 2f || flag) && this.nextRoamPositionTime == -1f)
			{
				this.nextRoamPositionTime = UnityEngine.Time.time + UnityEngine.Random.Range(5f, 10f);
			}
			if (this.nextRoamPositionTime != -1f && UnityEngine.Time.time > this.nextRoamPositionTime)
			{
				AIMovePoint bestRoamPoint = brain.PathFinder.GetBestRoamPoint(this.GetRoamAnchorPosition(), entity.ServerPosition, this.GetForwardDirection(), brain.Navigator.MaxRoamDistanceFromHome, brain.Navigator.BestRoamPointMaxDistance);
				if (bestRoamPoint)
				{
					float num = Vector3.Distance(bestRoamPoint.transform.position, entity.transform.position) / 1.5f;
					bestRoamPoint.SetUsedBy(entity, num + 11f);
				}
				this.lastDestinationTime = UnityEngine.Time.time;
				Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
				insideUnitSphere.y = 0f;
				insideUnitSphere.Normalize();
				Vector3 destination = (bestRoamPoint == null) ? entity.transform.position : (bestRoamPoint.transform.position + insideUnitSphere * bestRoamPoint.radius);
				this.SetDestination(destination);
				this.nextRoamPositionTime = -1f;
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000B69 RID: 2921
	public class BaseSleepState : BaseAIBrain.BasicAIState
	{
		// Token: 0x04003ED6 RID: 16086
		private StateStatus status = StateStatus.Error;

		// Token: 0x06004CD1 RID: 19665 RVA: 0x0019F858 File Offset: 0x0019DA58
		public BaseSleepState() : base(AIState.Sleep)
		{
		}

		// Token: 0x06004CD2 RID: 19666 RVA: 0x0019F86C File Offset: 0x0019DA6C
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			IAISleep iaisleep;
			if ((iaisleep = (entity as IAISleep)) == null)
			{
				return;
			}
			iaisleep.StartSleeping();
			this.status = StateStatus.Running;
		}

		// Token: 0x06004CD3 RID: 19667 RVA: 0x0019F8A0 File Offset: 0x0019DAA0
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			IAISleep iaisleep;
			if ((iaisleep = (entity as IAISleep)) == null)
			{
				return;
			}
			iaisleep.StopSleeping();
		}

		// Token: 0x06004CD4 RID: 19668 RVA: 0x0019F8C6 File Offset: 0x0019DAC6
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			return this.status;
		}
	}

	// Token: 0x02000B6A RID: 2922
	public class BasicAIState
	{
		// Token: 0x04003ED8 RID: 16088
		public BaseAIBrain brain;

		// Token: 0x04003EDA RID: 16090
		protected float _lastStateExitTime;

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x06004CD5 RID: 19669 RVA: 0x0019F8D8 File Offset: 0x0019DAD8
		// (set) Token: 0x06004CD6 RID: 19670 RVA: 0x0019F8E0 File Offset: 0x0019DAE0
		public AIState StateType { get; private set; }

		// Token: 0x06004CD7 RID: 19671 RVA: 0x0019F8E9 File Offset: 0x0019DAE9
		public virtual void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			this.TimeInState = 0f;
		}

		// Token: 0x06004CD8 RID: 19672 RVA: 0x0019F8F6 File Offset: 0x0019DAF6
		public virtual StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			this.TimeInState += delta;
			return StateStatus.Running;
		}

		// Token: 0x06004CD9 RID: 19673 RVA: 0x0019F907 File Offset: 0x0019DB07
		public virtual void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			this.TimeInState = 0f;
			this._lastStateExitTime = UnityEngine.Time.time;
		}

		// Token: 0x06004CDA RID: 19674 RVA: 0x0000441C File Offset: 0x0000261C
		public virtual bool CanInterrupt()
		{
			return true;
		}

		// Token: 0x06004CDB RID: 19675 RVA: 0x0000441C File Offset: 0x0000261C
		public virtual bool CanEnter()
		{
			return true;
		}

		// Token: 0x06004CDC RID: 19676 RVA: 0x0019F91F File Offset: 0x0019DB1F
		public virtual bool CanLeave()
		{
			return this.CanInterrupt();
		}

		// Token: 0x06004CDD RID: 19677 RVA: 0x00029CA8 File Offset: 0x00027EA8
		public virtual float GetWeight()
		{
			return 0f;
		}

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x06004CDE RID: 19678 RVA: 0x0019F927 File Offset: 0x0019DB27
		// (set) Token: 0x06004CDF RID: 19679 RVA: 0x0019F92F File Offset: 0x0019DB2F
		public float TimeInState { get; private set; }

		// Token: 0x06004CE0 RID: 19680 RVA: 0x0019F938 File Offset: 0x0019DB38
		public float TimeSinceState()
		{
			return UnityEngine.Time.time - this._lastStateExitTime;
		}

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06004CE1 RID: 19681 RVA: 0x0019F946 File Offset: 0x0019DB46
		// (set) Token: 0x06004CE2 RID: 19682 RVA: 0x0019F94E File Offset: 0x0019DB4E
		public bool AgrresiveState { get; protected set; }

		// Token: 0x06004CE3 RID: 19683 RVA: 0x0019F957 File Offset: 0x0019DB57
		public BasicAIState(AIState state)
		{
			this.StateType = state;
		}

		// Token: 0x06004CE4 RID: 19684 RVA: 0x0019F8E9 File Offset: 0x0019DAE9
		public void Reset()
		{
			this.TimeInState = 0f;
		}

		// Token: 0x06004CE5 RID: 19685 RVA: 0x0019F966 File Offset: 0x0019DB66
		public bool IsInState()
		{
			return this.brain != null && this.brain.CurrentState != null && this.brain.CurrentState == this;
		}

		// Token: 0x06004CE6 RID: 19686 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void DrawGizmos()
		{
		}
	}
}
