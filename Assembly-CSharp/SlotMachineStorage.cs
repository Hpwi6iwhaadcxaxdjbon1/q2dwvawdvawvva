using System;
using Network;

// Token: 0x020000CE RID: 206
public class SlotMachineStorage : StorageContainer
{
	// Token: 0x04000B86 RID: 2950
	public int Amount;

	// Token: 0x06001270 RID: 4720 RVA: 0x000955BC File Offset: 0x000937BC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SlotMachineStorage.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x000955FC File Offset: 0x000937FC
	public bool IsPlayerValid(BasePlayer player)
	{
		return player.isMounted && !(player.GetMounted() != base.GetParentEntity());
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x0009561C File Offset: 0x0009381C
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		return this.IsPlayerValid(player) && base.PlayerOpenLoot(player, panelToOpen, true);
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x00095634 File Offset: 0x00093834
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		Item slot = base.inventory.GetSlot(0);
		this.UpdateAmount((slot != null) ? slot.amount : 0);
	}

	// Token: 0x06001274 RID: 4724 RVA: 0x00095666 File Offset: 0x00093866
	public void UpdateAmount(int amount)
	{
		if (this.Amount == amount)
		{
			return;
		}
		this.Amount = amount;
		(base.GetParentEntity() as SlotMachine).OnBettingScrapUpdated(amount);
		base.ClientRPC<int>(null, "RPC_UpdateAmount", this.Amount);
	}

	// Token: 0x06001275 RID: 4725 RVA: 0x0009569C File Offset: 0x0009389C
	public override bool CanBeLooted(BasePlayer player)
	{
		return this.IsPlayerValid(player) && base.CanBeLooted(player);
	}
}
