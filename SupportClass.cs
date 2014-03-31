using System;
public class SupportClass
{
	public static void WriteStackTrace(System.Exception throwable, System.IO.TextWriter stream)
	{
		stream.Write(throwable.StackTrace);
		stream.Flush();
	}

	/*******************************/
	public static object PutElement(System.Collections.Hashtable hashTable, object key, object newValue)
	{
		System.Object element = hashTable[key];
		hashTable[key] = newValue;
		return element;
	}

}
