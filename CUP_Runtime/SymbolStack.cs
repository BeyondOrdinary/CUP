using System;
using System.Collections;

namespace CUP.runtime
{
	/// <summary>
	/// Implements the stack pattern for the symbol stack used in
	/// evaluating the LALR parse tree.
	/// </summary>
	public class SymbolStack
	{
		private ArrayList _list = new ArrayList();

		public SymbolStack()
		{
		}

		public object this[int index]
		{
			get 
			{
				return(_list[index]);
			}
			set 
			{
				_list[index] = value;
			}
		}

		public object[] ToArray()
		{
			return(_list.ToArray());
		}

		public void Clear()
		{
			_list.Clear();
		}

		public bool IsEmpty
		{
			get 
			{
				return(_list.Count > 0);
			}
		}

		public int Count
		{
			get 
			{
				return(_list.Count);
			}
		}

		public object Pop()
		{
			object obj = Peek();
			_list.RemoveAt(_list.Count-1);
			return(obj);
		}

		public void Push(object obj)
		{
			_list.Add(obj);
		}

		public object Peek()
		{
			return(_list[_list.Count-1]);
		}

		public object Peek(int idx)
		{
			return(_list[idx]);
		}
	}
}
