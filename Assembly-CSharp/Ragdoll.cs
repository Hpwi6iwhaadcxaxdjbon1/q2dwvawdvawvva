using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020002DE RID: 734
public class Ragdoll : BaseMonoBehaviour, IPrefabPreProcess
{
	// Token: 0x0400171E RID: 5918
	public Transform eyeTransform;

	// Token: 0x0400171F RID: 5919
	public Transform centerBone;

	// Token: 0x04001720 RID: 5920
	public Rigidbody primaryBody;

	// Token: 0x04001721 RID: 5921
	public PhysicMaterial physicMaterial;

	// Token: 0x04001722 RID: 5922
	public SpringJoint corpseJoint;

	// Token: 0x04001723 RID: 5923
	public Skeleton skeleton;

	// Token: 0x04001724 RID: 5924
	public Model model;

	// Token: 0x04001725 RID: 5925
	public List<Joint> joints = new List<Joint>();

	// Token: 0x04001726 RID: 5926
	public List<CharacterJoint> characterJoints = new List<CharacterJoint>();

	// Token: 0x04001727 RID: 5927
	public List<ConfigurableJoint> configurableJoints = new List<ConfigurableJoint>();

	// Token: 0x04001728 RID: 5928
	public List<Rigidbody> rigidbodies = new List<Rigidbody>();

	// Token: 0x04001729 RID: 5929
	public GameObject GibEffect;

	// Token: 0x06001DE2 RID: 7650 RVA: 0x000CC530 File Offset: 0x000CA730
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (!clientside)
		{
			return;
		}
		this.joints.Clear();
		this.characterJoints.Clear();
		this.configurableJoints.Clear();
		this.rigidbodies.Clear();
		base.GetComponentsInChildren<Joint>(true, this.joints);
		base.GetComponentsInChildren<CharacterJoint>(true, this.characterJoints);
		base.GetComponentsInChildren<ConfigurableJoint>(true, this.configurableJoints);
		base.GetComponentsInChildren<Rigidbody>(true, this.rigidbodies);
	}
}
