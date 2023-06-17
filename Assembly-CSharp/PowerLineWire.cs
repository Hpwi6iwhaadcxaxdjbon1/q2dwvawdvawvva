using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000682 RID: 1666
[AddComponentMenu("Procedural/Mega Wire")]
public class PowerLineWire : MonoBehaviour
{
	// Token: 0x0400272F RID: 10031
	public List<Transform> poles = new List<Transform>();

	// Token: 0x04002730 RID: 10032
	public List<PowerLineWireConnectionDef> connections = new List<PowerLineWireConnectionDef>();

	// Token: 0x04002731 RID: 10033
	public List<PowerLineWireSpan> spans = new List<PowerLineWireSpan>();

	// Token: 0x06002FBA RID: 12218 RVA: 0x0011EC98 File Offset: 0x0011CE98
	public void Copy(PowerLineWire from, PowerLineWireConnectionHelper helper)
	{
		this.connections.Clear();
		if (helper)
		{
			for (int i = 0; i < helper.connections.Count; i++)
			{
				this.connections.Add(new PowerLineWireConnectionDef(helper.connections[i]));
			}
			return;
		}
		for (int j = 0; j < from.connections.Count; j++)
		{
			this.connections.Add(new PowerLineWireConnectionDef(from.connections[j]));
		}
	}

	// Token: 0x06002FBB RID: 12219 RVA: 0x0011ED20 File Offset: 0x0011CF20
	public static PowerLineWire Create(PowerLineWire wire, List<GameObject> objs, GameObjectRef wirePrefab, string name, PowerLineWire copyfrom, float wiresize, float str)
	{
		if (objs != null && objs.Count > 1)
		{
			GameObject gameObject;
			if (wire == null)
			{
				gameObject = new GameObject();
				gameObject.name = name;
				wire = gameObject.AddComponent<PowerLineWire>();
			}
			else
			{
				gameObject = wire.gameObject;
			}
			wire.poles.Clear();
			wire.spans.Clear();
			wire.connections.Clear();
			wire.poles.Add(objs[0].transform);
			for (int i = 0; i < objs.Count - 1; i++)
			{
				PowerLineWireSpan powerLineWireSpan = new GameObject
				{
					name = name + " Span Mesh " + i,
					transform = 
					{
						parent = gameObject.transform
					}
				}.AddComponent<PowerLineWireSpan>();
				powerLineWireSpan.wirePrefab = wirePrefab;
				powerLineWireSpan.start = objs[i].transform;
				powerLineWireSpan.end = objs[i + 1].transform;
				wire.spans.Add(powerLineWireSpan);
				wire.poles.Add(objs[i + 1].transform);
			}
			PowerLineWireConnectionHelper component = objs[0].GetComponent<PowerLineWireConnectionHelper>();
			if (copyfrom)
			{
				wire.Copy(copyfrom, component);
			}
			else if (component)
			{
				wire.Copy(wire, component);
			}
			else
			{
				PowerLineWireConnectionDef item = new PowerLineWireConnectionDef();
				wire.connections.Add(item);
			}
			if (wiresize != 1f)
			{
				for (int j = 0; j < wire.connections.Count; j++)
				{
					wire.connections[j].radius *= wiresize;
				}
			}
			wire.Init();
		}
		return wire;
	}

	// Token: 0x06002FBC RID: 12220 RVA: 0x0011EECC File Offset: 0x0011D0CC
	public void Init()
	{
		for (int i = 0; i < this.spans.Count; i++)
		{
			PowerLineWireSpan powerLineWireSpan = this.spans[i];
			powerLineWireSpan.connections.Clear();
			for (int j = 0; j < this.connections.Count; j++)
			{
				PowerLineWireConnection powerLineWireConnection = new PowerLineWireConnection
				{
					start = powerLineWireSpan.start,
					end = powerLineWireSpan.end,
					inOffset = this.connections[j].inOffset,
					outOffset = this.connections[j].outOffset,
					radius = this.connections[j].radius
				};
				PowerLineWireConnectionHelper component = powerLineWireSpan.start.GetComponent<PowerLineWireConnectionHelper>();
				PowerLineWireConnectionHelper component2 = powerLineWireSpan.end.GetComponent<PowerLineWireConnectionHelper>();
				powerLineWireConnection.inOffset = component2.connections[j].inOffset;
				powerLineWireConnection.outOffset = component.connections[j].outOffset;
				if (!component.connections[j].hidden && !component2.connections[j].hidden)
				{
					powerLineWireSpan.connections.Add(powerLineWireConnection);
				}
			}
			powerLineWireSpan.Init(this);
		}
	}
}
