using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000947 RID: 2375
public class PathFinder
{
	// Token: 0x04003360 RID: 13152
	private int[,] costmap;

	// Token: 0x04003361 RID: 13153
	private int[,] visited;

	// Token: 0x04003362 RID: 13154
	private PathFinder.Point[] neighbors;

	// Token: 0x04003363 RID: 13155
	private bool directional;

	// Token: 0x04003364 RID: 13156
	public PathFinder.Point PushPoint;

	// Token: 0x04003365 RID: 13157
	public int PushRadius;

	// Token: 0x04003366 RID: 13158
	public int PushDistance;

	// Token: 0x04003367 RID: 13159
	public int PushMultiplier;

	// Token: 0x04003368 RID: 13160
	private static PathFinder.Point[] mooreNeighbors = new PathFinder.Point[]
	{
		new PathFinder.Point(0, 1),
		new PathFinder.Point(-1, 0),
		new PathFinder.Point(1, 0),
		new PathFinder.Point(0, -1),
		new PathFinder.Point(-1, 1),
		new PathFinder.Point(1, 1),
		new PathFinder.Point(-1, -1),
		new PathFinder.Point(1, -1)
	};

	// Token: 0x04003369 RID: 13161
	private static PathFinder.Point[] neumannNeighbors = new PathFinder.Point[]
	{
		new PathFinder.Point(0, 1),
		new PathFinder.Point(-1, 0),
		new PathFinder.Point(1, 0),
		new PathFinder.Point(0, -1)
	};

	// Token: 0x060038F4 RID: 14580 RVA: 0x00153070 File Offset: 0x00151270
	public PathFinder(int[,] costmap, bool diagonals = true, bool directional = true)
	{
		this.costmap = costmap;
		this.neighbors = (diagonals ? PathFinder.mooreNeighbors : PathFinder.neumannNeighbors);
		this.directional = directional;
	}

	// Token: 0x060038F5 RID: 14581 RVA: 0x0015309B File Offset: 0x0015129B
	public int GetResolution(int index)
	{
		return this.costmap.GetLength(index);
	}

	// Token: 0x060038F6 RID: 14582 RVA: 0x001530A9 File Offset: 0x001512A9
	public PathFinder.Node FindPath(PathFinder.Point start, PathFinder.Point end, int depth = 2147483647)
	{
		return this.FindPathReversed(end, start, depth);
	}

	// Token: 0x060038F7 RID: 14583 RVA: 0x001530B4 File Offset: 0x001512B4
	private PathFinder.Node FindPathReversed(PathFinder.Point start, PathFinder.Point end, int depth = 2147483647)
	{
		if (this.visited == null)
		{
			this.visited = new int[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		int num = 0;
		int num2 = this.costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = this.costmap.GetLength(1) - 1;
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = default(IntrusiveMinHeap<PathFinder.Node>);
		int num5 = this.Cost(start);
		int heuristic = this.Heuristic(start, end);
		intrusiveMinHeap.Add(new PathFinder.Node(start, num5, heuristic, null));
		this.visited[start.x, start.y] = num5;
		while (!intrusiveMinHeap.Empty && depth-- > 0)
		{
			PathFinder.Node node = intrusiveMinHeap.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < this.neighbors.Length; i++)
			{
				PathFinder.Point point = node.point + this.neighbors[i];
				if (point.x >= num && point.x <= num2 && point.y >= num3 && point.y <= num4)
				{
					int num6 = this.Cost(point, node);
					if (num6 != 2147483647)
					{
						int num7 = this.visited[point.x, point.y];
						if (num7 == 0 || num6 < num7)
						{
							int cost = node.cost + num6;
							int heuristic2 = this.Heuristic(point, end);
							intrusiveMinHeap.Add(new PathFinder.Node(point, cost, heuristic2, node));
							this.visited[point.x, point.y] = num6;
						}
					}
					else
					{
						this.visited[point.x, point.y] = -1;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x060038F8 RID: 14584 RVA: 0x001532A5 File Offset: 0x001514A5
	public PathFinder.Node FindPathDirected(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		if (startList.Count == 0 || endList.Count == 0)
		{
			return null;
		}
		return this.FindPathReversed(endList, startList, depth);
	}

	// Token: 0x060038F9 RID: 14585 RVA: 0x001532C2 File Offset: 0x001514C2
	public PathFinder.Node FindPathUndirected(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		if (startList.Count == 0 || endList.Count == 0)
		{
			return null;
		}
		if (startList.Count > endList.Count)
		{
			return this.FindPathReversed(endList, startList, depth);
		}
		return this.FindPathReversed(startList, endList, depth);
	}

	// Token: 0x060038FA RID: 14586 RVA: 0x001532F8 File Offset: 0x001514F8
	private PathFinder.Node FindPathReversed(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		if (this.visited == null)
		{
			this.visited = new int[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		int num = 0;
		int num2 = this.costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = this.costmap.GetLength(1) - 1;
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = default(IntrusiveMinHeap<PathFinder.Node>);
		using (List<PathFinder.Point>.Enumerator enumerator = startList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PathFinder.Point point = enumerator.Current;
				int num5 = this.Cost(point);
				int heuristic = this.Heuristic(point, endList);
				intrusiveMinHeap.Add(new PathFinder.Node(point, num5, heuristic, null));
				this.visited[point.x, point.y] = num5;
			}
			goto IL_1FD;
		}
		IL_E0:
		PathFinder.Node node = intrusiveMinHeap.Pop();
		if (node.heuristic == 0)
		{
			return node;
		}
		for (int i = 0; i < this.neighbors.Length; i++)
		{
			PathFinder.Point point2 = node.point + this.neighbors[i];
			if (point2.x >= num && point2.x <= num2 && point2.y >= num3 && point2.y <= num4)
			{
				int num6 = this.Cost(point2, node);
				if (num6 != 2147483647)
				{
					int num7 = this.visited[point2.x, point2.y];
					if (num7 == 0 || num6 < num7)
					{
						int cost = node.cost + num6;
						int heuristic2 = this.Heuristic(point2, endList);
						intrusiveMinHeap.Add(new PathFinder.Node(point2, cost, heuristic2, node));
						this.visited[point2.x, point2.y] = num6;
					}
				}
				else
				{
					this.visited[point2.x, point2.y] = -1;
				}
			}
		}
		IL_1FD:
		if (intrusiveMinHeap.Empty || depth-- <= 0)
		{
			return null;
		}
		goto IL_E0;
	}

	// Token: 0x060038FB RID: 14587 RVA: 0x00153528 File Offset: 0x00151728
	public PathFinder.Node FindClosestWalkable(PathFinder.Point start, int depth = 2147483647)
	{
		if (this.visited == null)
		{
			this.visited = new int[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		int num = 0;
		int num2 = this.costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = this.costmap.GetLength(1) - 1;
		if (start.x < num)
		{
			return null;
		}
		if (start.x > num2)
		{
			return null;
		}
		if (start.y < num3)
		{
			return null;
		}
		if (start.y > num4)
		{
			return null;
		}
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = default(IntrusiveMinHeap<PathFinder.Node>);
		int num5 = 1;
		int heuristic = this.Heuristic(start);
		intrusiveMinHeap.Add(new PathFinder.Node(start, num5, heuristic, null));
		this.visited[start.x, start.y] = num5;
		while (!intrusiveMinHeap.Empty && depth-- > 0)
		{
			PathFinder.Node node = intrusiveMinHeap.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < this.neighbors.Length; i++)
			{
				PathFinder.Point point = node.point + this.neighbors[i];
				if (point.x >= num && point.x <= num2 && point.y >= num3 && point.y <= num4)
				{
					int num6 = 1;
					if (this.visited[point.x, point.y] == 0)
					{
						int cost = node.cost + num6;
						int heuristic2 = this.Heuristic(point);
						intrusiveMinHeap.Add(new PathFinder.Node(point, cost, heuristic2, node));
						this.visited[point.x, point.y] = num6;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x060038FC RID: 14588 RVA: 0x001536FC File Offset: 0x001518FC
	public bool IsWalkable(PathFinder.Point point)
	{
		return this.costmap[point.x, point.y] != int.MaxValue;
	}

	// Token: 0x060038FD RID: 14589 RVA: 0x00153720 File Offset: 0x00151920
	public bool IsWalkableWithNeighbours(PathFinder.Point point)
	{
		if (this.costmap[point.x, point.y] == 2147483647)
		{
			return false;
		}
		for (int i = 0; i < this.neighbors.Length; i++)
		{
			PathFinder.Point point2 = point + this.neighbors[i];
			if (this.costmap[point2.x, point2.y] == 2147483647)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060038FE RID: 14590 RVA: 0x00153794 File Offset: 0x00151994
	public PathFinder.Node Reverse(PathFinder.Node start)
	{
		PathFinder.Node node = null;
		PathFinder.Node next = null;
		for (PathFinder.Node node2 = start; node2 != null; node2 = node2.next)
		{
			if (node != null)
			{
				node.next = next;
			}
			next = node;
			node = node2;
		}
		if (node != null)
		{
			node.next = next;
		}
		return node;
	}

	// Token: 0x060038FF RID: 14591 RVA: 0x001537CC File Offset: 0x001519CC
	public PathFinder.Node FindEnd(PathFinder.Node start)
	{
		for (PathFinder.Node node = start; node != null; node = node.next)
		{
			if (node.next == null)
			{
				return node;
			}
		}
		return start;
	}

	// Token: 0x06003900 RID: 14592 RVA: 0x001537F4 File Offset: 0x001519F4
	public int Cost(PathFinder.Point a)
	{
		int num = this.costmap[a.x, a.y];
		int num2 = 0;
		if (num != 2147483647 && this.PushMultiplier > 0)
		{
			int num3 = Mathf.Max(0, this.Heuristic(a, this.PushPoint) - this.PushRadius * this.PushRadius);
			int num4 = Mathf.Max(0, this.PushDistance * this.PushDistance - num3);
			num2 = this.PushMultiplier * num4;
		}
		return num + num2;
	}

	// Token: 0x06003901 RID: 14593 RVA: 0x00153870 File Offset: 0x00151A70
	public int Cost(PathFinder.Point a, PathFinder.Node neighbour)
	{
		int num = this.Cost(a);
		int num2 = 0;
		if (num != 2147483647 && this.directional && neighbour != null && neighbour.next != null && this.Heuristic(a, neighbour.next.point) <= 2)
		{
			num2 = 10000;
		}
		return num + num2;
	}

	// Token: 0x06003902 RID: 14594 RVA: 0x001538BE File Offset: 0x00151ABE
	public int Heuristic(PathFinder.Point a)
	{
		if (this.costmap[a.x, a.y] != 2147483647)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x06003903 RID: 14595 RVA: 0x001538E4 File Offset: 0x00151AE4
	public int Heuristic(PathFinder.Point a, PathFinder.Point b)
	{
		int num = a.x - b.x;
		int num2 = a.y - b.y;
		return num * num + num2 * num2;
	}

	// Token: 0x06003904 RID: 14596 RVA: 0x00153914 File Offset: 0x00151B14
	public int Heuristic(PathFinder.Point a, List<PathFinder.Point> b)
	{
		int num = int.MaxValue;
		for (int i = 0; i < b.Count; i++)
		{
			num = Mathf.Min(num, this.Heuristic(a, b[i]));
		}
		return num;
	}

	// Token: 0x06003905 RID: 14597 RVA: 0x00153950 File Offset: 0x00151B50
	public float Distance(PathFinder.Point a, PathFinder.Point b)
	{
		int num = a.x - b.x;
		int num2 = a.y - b.y;
		return Mathf.Sqrt((float)(num * num + num2 * num2));
	}

	// Token: 0x02000EBE RID: 3774
	public struct Point : IEquatable<PathFinder.Point>
	{
		// Token: 0x04004CCD RID: 19661
		public int x;

		// Token: 0x04004CCE RID: 19662
		public int y;

		// Token: 0x06005333 RID: 21299 RVA: 0x001B1E54 File Offset: 0x001B0054
		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x06005334 RID: 21300 RVA: 0x001B1E64 File Offset: 0x001B0064
		public static PathFinder.Point operator +(PathFinder.Point a, PathFinder.Point b)
		{
			return new PathFinder.Point(a.x + b.x, a.y + b.y);
		}

		// Token: 0x06005335 RID: 21301 RVA: 0x001B1E85 File Offset: 0x001B0085
		public static PathFinder.Point operator -(PathFinder.Point a, PathFinder.Point b)
		{
			return new PathFinder.Point(a.x - b.x, a.y - b.y);
		}

		// Token: 0x06005336 RID: 21302 RVA: 0x001B1EA6 File Offset: 0x001B00A6
		public static PathFinder.Point operator *(PathFinder.Point p, int i)
		{
			return new PathFinder.Point(p.x * i, p.y * i);
		}

		// Token: 0x06005337 RID: 21303 RVA: 0x001B1EBD File Offset: 0x001B00BD
		public static PathFinder.Point operator /(PathFinder.Point p, int i)
		{
			return new PathFinder.Point(p.x / i, p.y / i);
		}

		// Token: 0x06005338 RID: 21304 RVA: 0x001B1ED4 File Offset: 0x001B00D4
		public static bool operator ==(PathFinder.Point a, PathFinder.Point b)
		{
			return a.Equals(b);
		}

		// Token: 0x06005339 RID: 21305 RVA: 0x001B1EDE File Offset: 0x001B00DE
		public static bool operator !=(PathFinder.Point a, PathFinder.Point b)
		{
			return !a.Equals(b);
		}

		// Token: 0x0600533A RID: 21306 RVA: 0x001B1EEB File Offset: 0x001B00EB
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode();
		}

		// Token: 0x0600533B RID: 21307 RVA: 0x001B1F04 File Offset: 0x001B0104
		public override bool Equals(object other)
		{
			return other is PathFinder.Point && this.Equals((PathFinder.Point)other);
		}

		// Token: 0x0600533C RID: 21308 RVA: 0x001B1F1C File Offset: 0x001B011C
		public bool Equals(PathFinder.Point other)
		{
			return this.x == other.x && this.y == other.y;
		}
	}

	// Token: 0x02000EBF RID: 3775
	public class Node : IMinHeapNode<PathFinder.Node>, ILinkedListNode<PathFinder.Node>
	{
		// Token: 0x04004CCF RID: 19663
		public PathFinder.Point point;

		// Token: 0x04004CD0 RID: 19664
		public int cost;

		// Token: 0x04004CD1 RID: 19665
		public int heuristic;

		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x0600533D RID: 21309 RVA: 0x001B1F3C File Offset: 0x001B013C
		// (set) Token: 0x0600533E RID: 21310 RVA: 0x001B1F44 File Offset: 0x001B0144
		public PathFinder.Node next { get; set; }

		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x0600533F RID: 21311 RVA: 0x001B1F4D File Offset: 0x001B014D
		// (set) Token: 0x06005340 RID: 21312 RVA: 0x001B1F55 File Offset: 0x001B0155
		public PathFinder.Node child { get; set; }

		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x06005341 RID: 21313 RVA: 0x001B1F5E File Offset: 0x001B015E
		public int order
		{
			get
			{
				return this.cost + this.heuristic;
			}
		}

		// Token: 0x06005342 RID: 21314 RVA: 0x001B1F6D File Offset: 0x001B016D
		public Node(PathFinder.Point point, int cost, int heuristic, PathFinder.Node next = null)
		{
			this.point = point;
			this.cost = cost;
			this.heuristic = heuristic;
			this.next = next;
		}
	}
}
