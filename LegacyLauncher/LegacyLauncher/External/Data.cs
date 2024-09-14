using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;


[Serializable]
public class Data
{
	public List<SerializableKeyValuePair> Values = new List<SerializableKeyValuePair>();
	public void Set(string key, object value, string description)
	{
		SerializableKeyValuePair stat = null;
		if (!Values.Exists(s => s.Key == key))
		{
			stat = new SerializableKeyValuePair(key, value);
			Values.Add(stat);
		}
		else
		{
			stat = Values.Find(s => s.Key == key);
			stat.Value = value;
		}


		if (description != null)
		{
			stat.Description = description;
		}
	}

	public void ForEach(Action<SerializableKeyValuePair> action)
	{
		Values.ForEach(action);
	}

	public void PreSave()
	{
		Values.ForEach(v => v.PreSave());
	}

	public void Set(string key, object value)
	{
		Set(key, value, null);
	}

	public object Get(string key)
	{
		SerializableKeyValuePair stat = null;
		stat = Values.Find(s => s.Key == key);
		if (stat is null)
		{
			return null;
		}

		stat.Deserialize();

		return stat.Value;
	}

	public object Get(string key, object defaultObject)
	{
		SerializableKeyValuePair stat = null;
		stat = Values.Find(s => s.Key == key);
		if (stat is null)
		{
			Set(key,defaultObject,null);
			return defaultObject;
		}

		stat.Deserialize();

		return stat.Value;
	}
}

/// <summary>
/// Поле пользовательской статистики. 
/// </summary>
[Serializable]
public class SerializableKeyValuePair
{
	[System.Xml.Serialization.XmlIgnore]
	public object Value;

	public string Key;
	public string ValueCode;

	public string ValueType;
	public string Description = "НЕТ ОПИСАНИЯ";

	private bool _deserialized = false;
	private bool _serialized = false;

	public SerializableKeyValuePair() { }

	public SerializableKeyValuePair(string key, object value, string description)
	{
		Key = key;
		Value = value;
		Description = description;
		_deserialized = true;
	}

	public Type GetValueType()
	{
		if (Value != null || ValueCode != null || ValueCode != "")
		{
			if (ValueType == null || ValueType == "")
			{
				ValueType = Value.GetType().FullName;
			}
			return Type.GetType(ValueType);
		}
		return null;
	}

	public SerializableKeyValuePair(string key, object value)
	{
		Key = key;
		Value = value;
		_deserialized = true;
	}

	public void Deserialize()
	{
		if (!_deserialized)
		{
			Logger.Log("Deserializing " + Key);
			try
			{
				XmlSerializer xs = new XmlSerializer(GetValueType());
				using (Stream stream = ValueCode.ToStream())
				{
					Logger.Log("Deserialized " + Key);
					StreamReader sr = new StreamReader(stream);
					Value = xs.Deserialize(sr);
					_deserialized = true;
					sr.Dispose();
				}
			}
			catch (Exception e)
			{
				Logger.Error("(DESERIALIZE) " + e.Message);
			}
		}
	}

	public void PreSave()
	{
		Serialize();
	}

	public void Serialize()
	{
		if (_deserialized)
		{
			ValueType = Value.GetType().FullName;
			try
			{
				XmlSerializer xs = new XmlSerializer(GetValueType());
				using (MemoryStream stream = new MemoryStream())
				{
					using (StreamWriter writer = new StreamWriter(stream))
					{
						xs.Serialize(writer, Value);
						ValueCode = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error("(SERIALIZE) " + e.Message);
			}
		}
	}
}
