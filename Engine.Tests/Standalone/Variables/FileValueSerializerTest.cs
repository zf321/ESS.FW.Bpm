
//using ESS.FW.Bpm.Engine.Common;
//using ESS.FW.Bpm.Engine.Variable.Type.Impl;
//using ESS.FW.Bpm.Engine.Variable.Value.Impl;

//namespace ESS.FW.Bpm.Engine.Tests.Standalone.Variables
//{



//    /// <summary>
//	///
//	/// 
//	/// </summary>
//	public class FileValueSerializerTest
//	{

//	  private const string SEPARATOR = "#";
//	  private FileValueSerializer serializer;

//[SetUp]
//	  public virtual void setUp()
//	  {
//		serializer = new FileValueSerializer();
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testTypeIsFileValueType()
//	  public virtual void testTypeIsFileValueType()
//	  {
//		Assert.That(serializer.Type, Is.EqualTo(instanceOf(typeof(FileValueTypeImpl))));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testWriteFilenameOnlyValue()
//	  public virtual void testWriteFilenameOnlyValue()
//	  {
//		string filename = "test.Txt";
//		IFileValue fileValue = Variables.FileValue(filename).Create();
//		ValueFields valueFields = new MockValueFields();

//		serializer.WriteValue(fileValue, valueFields);

//		Assert.That(valueFields.ByteArrayValue, Is.EqualTo(null));
//		Assert.That(valueFields.TextValue, Is.EqualTo(filename));
//		Assert.That(valueFields.TextValue2, Is.EqualTo(null));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testWriteMimetypeAndFilenameValue()
//	  public virtual void testWriteMimetypeAndFilenameValue()
//	  {
//		string filename = "test.Txt";
//		string mimeType = "text/json";
//		IFileValue fileValue = Variables.FileValue(filename).MimeType(mimeType).Create();
//		ValueFields valueFields = new MockValueFields();

//		serializer.WriteValue(fileValue, valueFields);

//		Assert.That(valueFields.ByteArrayValue, Is.EqualTo(null));
//		Assert.That(valueFields.TextValue, Is.EqualTo(filename));
//		Assert.That(valueFields.TextValue2, Is.EqualTo(mimeType + SEPARATOR));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testWriteMimetypeFilenameAndBytesValue() throws java.io.UnsupportedEncodingException
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testWriteMimetypeFilenameAndBytesValue()
//	  {
//		string filename = "test.Txt";
//		string mimeType = "text/json";
//		System.IO.Stream Is.EqualTo = this.GetType().ClassLoader.GetResourceAsStream("resources/standalone/variables/simpleFile.Txt");
//		IFileValue fileValue = Variables.FileValue(filename).MimeType(mimeType).File(Is.EqualTo).Create();
//		ValueFields valueFields = new MockValueFields();

//		serializer.WriteValue(fileValue, valueFields);

//		Assert.That(new string(valueFields.ByteArrayValue, "UTF-8"), Is.EqualTo("text"));
//		Assert.That(valueFields.TextValue, Is.EqualTo(filename));
//		Assert.That(valueFields.TextValue2, Is.EqualTo(mimeType + SEPARATOR));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testWriteMimetypeFilenameBytesValueAndEncoding() throws java.io.UnsupportedEncodingException
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testWriteMimetypeFilenameBytesValueAndEncoding()
//	  {
//		string filename = "test.Txt";
//		string mimeType = "text/json";
//		Charset encoding = Charset.ForName("UTF-8");
//		System.IO.Stream Is.EqualTo = this.GetType().ClassLoader.GetResourceAsStream("resources/standalone/variables/simpleFile.Txt");
//		IFileValue fileValue = Variables.FileValue(filename).MimeType(mimeType).Encoding(encoding).File(Is.EqualTo).Create();
//		ValueFields valueFields = new MockValueFields();

//		serializer.WriteValue(fileValue, valueFields);

//		Assert.That(new string(valueFields.ByteArrayValue, "UTF-8"), Is.EqualTo("text"));
//		Assert.That(valueFields.TextValue, Is.EqualTo(filename));
//		Assert.That(valueFields.TextValue2, Is.EqualTo(mimeType + SEPARATOR + encoding.Name()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testWriteMimetypeFilenameAndBytesValueWithShortcutMethod() throws java.Net.URISyntaxException, java.io.UnsupportedEncodingException
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testWriteMimetypeFilenameAndBytesValueWithShortcutMethod()
//	  {
//		File file = new File(this.GetType().ClassLoader.GetResource("resources/standalone/variables/simpleFile.Txt").ToURI());
//		IFileValue fileValue = Variables.FileValue(file);
//		ValueFields valueFields = new MockValueFields();

//		serializer.WriteValue(fileValue, valueFields);

//		Assert.That(new string(valueFields.ByteArrayValue, "UTF-8"), Is.EqualTo("text"));
//		Assert.That(valueFields.TextValue, Is.EqualTo("simpleFile.Txt"));
//		Assert.That(valueFields.TextValue2, Is.EqualTo("text/plain" + SEPARATOR));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test(expected = UnsupportedOperationException.class) public void testThrowsExceptionWhenConvertingUnknownUntypedValueToTypedValue()
//	  public virtual void testThrowsExceptionWhenConvertingUnknownUntypedValueToTypedValue()
//	  {
//		serializer.convertToTypedValue((UntypedValueImpl) Variables.UntypedValue(new object()));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testReadFileNameMimeTypeAndByteArray() throws java.io.IOException
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testReadFileNameMimeTypeAndByteArray()
//	  {
//		System.IO.Stream Is.EqualTo = this.GetType().ClassLoader.GetResourceAsStream("resources/standalone/variables/simpleFile.Txt");
//		byte[] data = new sbyte[Is.EqualTo.available()];
//		DataInputStream dataInputStream = new DataInputStream(Is.EqualTo);
//		dataInputStream.ReadFully(data);
//		dataInputStream.Close();
//		MockValueFields valueFields = new MockValueFields();
//		string filename = "file.Txt";
//		valueFields.TextValue = filename;
//		valueFields.ByteArrayValue = data;
//		string mimeType = "text/plain";
//		valueFields.TextValue2 = mimeType + SEPARATOR;

//		IFileValue fileValue = serializer.ReadValue(valueFields, true);

//		Assert.That(fileValue.Filename, Is.EqualTo(filename));
//		Assert.That(fileValue.MimeType, Is.EqualTo(mimeType));
//		checkStreamFromValue(fileValue, "text");
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testReadFileNameEncodingAndByteArray() throws java.io.IOException
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testReadFileNameEncodingAndByteArray()
//	  {
//		System.IO.Stream Is.EqualTo = this.GetType().ClassLoader.GetResourceAsStream("resources/standalone/variables/simpleFile.Txt");
//		byte[] data = new sbyte[Is.EqualTo.available()];
//		DataInputStream dataInputStream = new DataInputStream(Is.EqualTo);
//		dataInputStream.ReadFully(data);
//		dataInputStream.Close();
//		MockValueFields valueFields = new MockValueFields();
//		string filename = "file.Txt";
//		valueFields.TextValue = filename;
//		valueFields.ByteArrayValue = data;
//		string encoding = SEPARATOR + "UTF-8";
//		valueFields.TextValue2 = encoding;

//		IFileValue fileValue = serializer.ReadValue(valueFields, true);

//		Assert.That(fileValue.Filename, Is.EqualTo(filename));
//		Assert.That(fileValue.Encoding, Is.EqualTo("UTF-8"));
//		Assert.That(fileValue.EncodingAsCharset, Is.EqualTo(Charset.ForName("UTF-8")));
//		checkStreamFromValue(fileValue, "text");
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testReadFullValue() throws java.io.IOException
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testReadFullValue()
//	  {
//		System.IO.Stream Is.EqualTo = this.GetType().ClassLoader.GetResourceAsStream("resources/standalone/variables/simpleFile.Txt");
//		byte[] data = new sbyte[Is.EqualTo.available()];
//		DataInputStream dataInputStream = new DataInputStream(Is.EqualTo);
//		dataInputStream.ReadFully(data);
//		dataInputStream.Close();
//		MockValueFields valueFields = new MockValueFields();
//		string filename = "file.Txt";
//		valueFields.TextValue = filename;
//		valueFields.ByteArrayValue = data;
//		string mimeType = "text/plain";
//		string encoding = "UTF-16";
//		valueFields.TextValue2 = mimeType + SEPARATOR + encoding;

//		IFileValue fileValue = serializer.ReadValue(valueFields, true);

//		Assert.That(fileValue.Filename, Is.EqualTo(filename));
//		Assert.That(fileValue.MimeType, Is.EqualTo(mimeType));
//		Assert.That(fileValue.Encoding, Is.EqualTo("UTF-16"));
//		Assert.That(fileValue.EncodingAsCharset, Is.EqualTo(Charset.ForName("UTF-16")));
//		checkStreamFromValue(fileValue, "text");
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testReadFilenameAndByteArrayValue() throws java.io.IOException
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testReadFilenameAndByteArrayValue()
//	  {
//		System.IO.Stream Is.EqualTo = this.GetType().ClassLoader.GetResourceAsStream("resources/standalone/variables/simpleFile.Txt");
//		byte[] data = new sbyte[Is.EqualTo.available()];
//		DataInputStream dataInputStream = new DataInputStream(Is.EqualTo);
//		dataInputStream.ReadFully(data);
//		dataInputStream.Close();
//		MockValueFields valueFields = new MockValueFields();
//		string filename = "file.Txt";
//		valueFields.TextValue = filename;
//		valueFields.ByteArrayValue = data;

//		IFileValue fileValue = serializer.ReadValue(valueFields, true);

//		Assert.That(fileValue.Filename, Is.EqualTo(filename));
//		Assert.That(fileValue.MimeType, Is.EqualTo(null));
//		checkStreamFromValue(fileValue, "text");
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testReadFilenameValue() throws java.io.IOException
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testReadFilenameValue()
//	  {
//		MockValueFields valueFields = new MockValueFields();
//		string filename = "file.Txt";
//		valueFields.TextValue = filename;

//		IFileValue fileValue = serializer.ReadValue(valueFields, true);

//		Assert.That(fileValue.Filename, Is.EqualTo(filename));
//		Assert.That(fileValue.MimeType, Is.EqualTo(null));
//		Assert.That(fileValue.Value, Is.EqualTo(null));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testNameIsFile()
//	  public virtual void testNameIsFile()
//	  {
//		Assert.That(serializer.Name, Is.EqualTo("file"));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testWriteFilenameAndEncodingValue()
//	  public virtual void testWriteFilenameAndEncodingValue()
//	  {
//		string filename = "test.Txt";
//		string encoding = "UTF-8";
//		IFileValue fileValue = Variables.FileValue(filename).Encoding(encoding).Create();
//		ValueFields valueFields = new MockValueFields();

//		serializer.WriteValue(fileValue, valueFields);

//		Assert.That(valueFields.ByteArrayValue, Is.EqualTo(null));
//		Assert.That(valueFields.TextValue, Is.EqualTo(filename));
//		Assert.That(valueFields.TextValue2, Is.EqualTo(SEPARATOR + encoding));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test(expected = IllegalArgumentException.class) public void testSerializeFileValueWithoutName()
//	  public virtual void testSerializeFileValueWithoutName()
//	  {
//		Variables.FileValue((string) null).File("abc".GetBytes()).Create();
//	  }

//	  private void checkStreamFromValue(ITypedValue value, string expected)
//	  {
//		System.IO.Stream stream = (System.IO.Stream) value.Value;
//		Scanner scanner = new Scanner(stream);
//		Assert.That(scanner.NextLine(), Is.EqualTo(expected));
//	  }

//	  private class MockValueFields : ValueFields
//	  {

//		internal string name;
//		internal string textValue;
//		internal string textValue2;
//		internal long? longValue;
//		internal double? doubleValue;
//		internal byte[] bytes;

//		public override string Name
//		{
//			get
//			{
//			  return name;
//			}
//		}

//		public override string TextValue
//		{
//			get
//			{
//			  return textValue;
//			}
//			set
//			{
//			  this.TextValue = value;
//			}
//		}


//		public override string TextValue2
//		{
//			get
//			{
//			  return textValue2;
//			}
//			set
//			{
//			  this.TextValue2 = value;
//			}
//		}


//		public override long? LongValue
//		{
//			get
//			{
//			  return longValue;
//			}
//			set
//			{
//			  this.LongValue = value;
//			}
//		}


//		public override double? DoubleValue
//		{
//			get
//			{
//			  return doubleValue;
//			}
//			set
//			{
//			  this.DoubleValue = value;
//			}
//		}


//		public override byte[] ByteArrayValue
//		{
//			get
//			{
//			  return bytes;
//			}
//			set
//			{
//			  this.Bytes = value;
//			}
//		}


//	  }
//	}

//}