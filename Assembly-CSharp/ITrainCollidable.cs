using System;

// Token: 0x020004AC RID: 1196
public interface ITrainCollidable
{
	// Token: 0x06002728 RID: 10024
	bool CustomCollision(TrainCar train, TriggerTrainCollisions trainTrigger);

	// Token: 0x06002729 RID: 10025
	bool EqualNetID(BaseNetworkable other);
}
