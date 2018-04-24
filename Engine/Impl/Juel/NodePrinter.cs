using System.Collections.Generic;
using System.IO;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{


	/// <summary>
	/// Node pretty printer for debugging purposes.
	/// 
	/// 
	/// </summary>
	public class NodePrinter
	{
		private static bool IsLastSibling(INode node, INode parent)
		{
			if (parent != null)
			{
				return node == parent.GetChild(parent.Cardinality - 1);
			}
			return true;
		}

		private static void Dump(StreamWriter writer, INode node, Stack<INode> predecessors)
        {
            INode parent = null;
            if (predecessors.Count > 0)
			{
				foreach (INode predecessor in predecessors)
				{
					if (IsLastSibling(predecessor, parent))
					{
						writer.Write("   ");
					}
					else
					{
						writer.Write("|  ");
					}
					parent = predecessor;
				}
				writer.WriteLine("|");
			}
			parent = null;
			foreach (INode predecessor in predecessors)
			{
				if (IsLastSibling(predecessor, parent))
				{
					writer.Write("   ");
				}
				else
				{
					writer.Write("|  ");
				}
				parent = predecessor;
			}
			writer.Write("+- ");
			writer.WriteLine(node.ToString());

			predecessors.Push(node);
			for (int i = 0; i < node.Cardinality; i++)
			{
				Dump(writer, node.GetChild(i), predecessors);
			}
			predecessors.Pop();
		}

		public static void Dump(StreamWriter writer, INode node)
		{
			Dump(writer, node, new Stack<INode>());
		}
	}

}