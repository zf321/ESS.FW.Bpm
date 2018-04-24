//using System;
//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Common;
//using NUnit.Framework;
//using Newtonsoft.Json.Linq;

//namespace ESS.FW.Bpm.Engine.Tests.Standalone.Util
//{
    
//    /// <summary>
//	/// 
//	/// </summary>
//	public class JsonUtilTest
//	{

////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test @SuppressWarnings("unchecked") public void testJsonObjectToMap()
//	  public virtual void testJsonObjectToMap()
//	  {
//		Assert.IsNull(jsonObjectAsMap(null));

//		JObject jsonObject = new JObject();

//		IDictionary<string, object> map = jsonObjectAsMap(jsonObject);
//		Assert.True(map.Count == 0);

//		jsonObject.Put("boolean", true);
//		jsonObject.Put("int", 12);
//		jsonObject.Put("double", 11.1);
//		jsonObject.Put("long", 13l);
//		jsonObject.Put("string", "test");
//		jsonObject.Put("list", Collections.singletonList("test"));
//		jsonObject.Put("map", Collections.singletonMap("test", "test"));
//		jsonObject.Put("date", new DateTime());
//		jsonObject.Put("null", JSONObject.NULL);

//		map = jsonObjectAsMap(jsonObject);
//		Assert.AreEqual(9, map.Count);

//		Assert.AreEqual(true, map["boolean"]);
//		Assert.AreEqual(12, map["int"]);
//		Assert.AreEqual(11.1, map["double"]);
//		Assert.AreEqual(13l, map["long"]);
//		Assert.AreEqual("test", map["string"]);

//		IList<object> embeddedList = (IList<object>) map["list"];
//		Assert.AreEqual(1, embeddedList.Count);
//		Assert.AreEqual("test", embeddedList[0]);

//		IDictionary<string, object> embeddedMap = (IDictionary<string, object>) map["map"];
//		Assert.AreEqual(1, embeddedMap.Count);
//		Assert.True(embeddedMap.ContainsKey("test"));
//		Assert.AreEqual("test", embeddedMap["test"]);

//		DateTime embeddedDate = (DateTime) map["date"];
//		Assert.AreEqual(new DateTime(), embeddedDate);

//		Assert.True(map.ContainsKey("null"));
//		Assert.IsNull(map["null"]);
//	  }

////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test @SuppressWarnings("unchecked") public void testJsonArrayToList()
//	  public virtual void testJsonArrayToList()
//	  {
//		Assert.IsNull(jsonArrayAsList(null));

//		JSONArray jsonArray = new JSONArray();

//		IList<object> list = jsonArrayAsList(jsonArray);
//		Assert.True(list.Count == 0);

//		jsonArray.Put(true);
//		jsonArray.Put(12);
//		jsonArray.Put(11.1);
//		jsonArray.Put(13l);
//		jsonArray.Put("test");
//		jsonArray.Put(Collections.singletonList("test"));
//		jsonArray.Put(Collections.singletonMap("test", "test"));
//		jsonArray.Put(new DateTime());
//		jsonArray.Put(JSONObject.NULL);

//		list = jsonArrayAsList(jsonArray);
//		Assert.AreEqual(9, list.Count);

//		Assert.AreEqual(true, list[0]);
//		Assert.AreEqual(12, list[1]);
//		Assert.AreEqual(11.1, list[2]);
//		Assert.AreEqual(13l, list[3]);
//		Assert.AreEqual("test", list[4]);

//		IList<object> embeddedList = (IList<object>) list[5];
//		Assert.AreEqual(1, embeddedList.Count);
//		Assert.AreEqual("test", embeddedList[0]);

//		IDictionary<string, object> embeddedMap = (IDictionary<string, object>) list[6];
//		Assert.AreEqual(1, embeddedMap.Count);
//		Assert.True(embeddedMap.ContainsKey("test"));
//		Assert.AreEqual("test", embeddedMap["test"]);

//		DateTime embeddedDate = (DateTime) list[7];
//		Assert.AreEqual(new DateTime(), embeddedDate);

//		object null_ = list[8];
//		Assert.IsNull(null_);
//	  }

////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testJsonObjectNullRetained()
//	  public virtual void testJsonObjectNullRetained()
//	  {
//		string json = "{\"key\":null}";

//		JObject @object = new JObject(json);
//		Assert.True(@object.Property("key"));

//		IDictionary<string, object> map = jsonObjectAsMap(@object);
//		Assert.True(map.ContainsKey("key"));
//		Assert.IsNull(map["key"]);
//	  }

////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testJsonArrayNullRetained()
//	  public virtual void testJsonArrayNullRetained()
//	  {
//		string json = "[null]";

//		JArray array = new JArray(json);
//		Assert.AreEqual(1, array.length());

//		IList<object> list = jsonArrayAsList(array);
//		Assert.AreEqual(1, list.Count);
//		Assert.IsNull(list[0]);
//	  }

//	}

//}