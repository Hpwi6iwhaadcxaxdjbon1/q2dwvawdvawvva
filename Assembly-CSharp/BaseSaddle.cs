using System;

// Token: 0x0200046B RID: 1131
public class BaseSaddle : BaseMountable
{
	// Token: 0x04001D89 RID: 7561
	public BaseRidableAnimal animal;

	// Token: 0x06002549 RID: 9545 RVA: 0x000EB6EA File Offset: 0x000E98EA
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (player != this._mounted)
		{
			return;
		}
		if (this.animal)
		{
			this.animal.RiderInput(inputState, player);
		}
	}

	// Token: 0x0600254A RID: 9546 RVA: 0x000EB715 File Offset: 0x000E9915
	public void SetAnimal(BaseRidableAnimal newAnimal)
	{
		this.animal = newAnimal;
	}
}
