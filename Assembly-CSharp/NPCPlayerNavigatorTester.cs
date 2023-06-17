using System;

// Token: 0x02000207 RID: 519
public class NPCPlayerNavigatorTester : BaseMonoBehaviour
{
	// Token: 0x04001327 RID: 4903
	public BasePathNode TargetNode;

	// Token: 0x04001328 RID: 4904
	private BasePathNode currentNode;

	// Token: 0x06001B36 RID: 6966 RVA: 0x000C0F24 File Offset: 0x000BF124
	private void Update()
	{
		if (this.TargetNode != this.currentNode)
		{
			base.GetComponent<BaseNavigator>().SetDestination(this.TargetNode.Path, this.TargetNode, 0.5f);
			this.currentNode = this.TargetNode;
		}
	}
}
