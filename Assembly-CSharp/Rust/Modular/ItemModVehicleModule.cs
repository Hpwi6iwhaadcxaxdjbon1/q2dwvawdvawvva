using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B2F RID: 2863
	public class ItemModVehicleModule : ItemMod, VehicleModuleInformationPanel.IVehicleModuleInfo
	{
		// Token: 0x04003DEB RID: 15851
		public GameObjectRef entityPrefab;

		// Token: 0x04003DEC RID: 15852
		[Range(1f, 2f)]
		public int socketsTaken = 1;

		// Token: 0x04003DED RID: 15853
		public bool doNonUserSpawn;

		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x0600456E RID: 17774 RVA: 0x00196022 File Offset: 0x00194222
		public int SocketsTaken
		{
			get
			{
				return this.socketsTaken;
			}
		}

		// Token: 0x0600456F RID: 17775 RVA: 0x0019602C File Offset: 0x0019422C
		public BaseVehicleModule CreateModuleEntity(BaseEntity parent, Vector3 position, Quaternion rotation)
		{
			if (!this.entityPrefab.isValid)
			{
				Debug.LogError("Invalid entity prefab for module");
				return null;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, position, rotation, true);
			BaseVehicleModule baseVehicleModule = null;
			if (baseEntity != null)
			{
				if (parent != null)
				{
					baseEntity.SetParent(parent, true, false);
					baseEntity.canTriggerParent = false;
				}
				baseEntity.Spawn();
				baseVehicleModule = baseEntity.GetComponent<BaseVehicleModule>();
				if (this.doNonUserSpawn)
				{
					this.doNonUserSpawn = false;
					baseVehicleModule.NonUserSpawn();
				}
			}
			return baseVehicleModule;
		}
	}
}
