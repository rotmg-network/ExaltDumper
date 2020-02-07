﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Il2CppDumper
{
    public class BinaryStream : IDisposable
    {
        public float version;
        public bool is32Bit;
        private Stream stream;
        private BinaryReader reader;
        private BinaryWriter writer;
        private MethodInfo readClass;
        private Dictionary<Type, MethodInfo> readClassCache = new Dictionary<Type, MethodInfo>();
        private Dictionary<FieldInfo, VersionAttribute> attributeCache = new Dictionary<FieldInfo, VersionAttribute>();

        public BinaryStream(Stream input)
        {
            stream = input;
            reader = new BinaryReader(stream, Encoding.UTF8, true);
            writer = new BinaryWriter(stream, Encoding.UTF8, true);
            readClass = GetType().GetMethod("ReadClass", Type.EmptyTypes);
        }

        public bool ReadBoolean() => reader.ReadBoolean();

        public byte ReadByte() => reader.ReadByte();

        public byte[] ReadBytes(int count) => reader.ReadBytes(count);

        public sbyte ReadSByte() => reader.ReadSByte();

        public short ReadInt16() => reader.ReadInt16();

        public ushort ReadUInt16() => reader.ReadUInt16();

        public int ReadInt32() => reader.ReadInt32();

        public uint ReadUInt32() => reader.ReadUInt32();

        public long ReadInt64() => reader.ReadInt64();

        public ulong ReadUInt64() => reader.ReadUInt64();

        public float ReadSingle() => reader.ReadSingle();

        public double ReadDouble() => reader.ReadDouble();

        public void Write(bool value) => writer.Write(value);

        public void Write(byte value) => writer.Write(value);

        public void Write(sbyte value) => writer.Write(value);

        public void Write(short value) => writer.Write(value);

        public void Write(ushort value) => writer.Write(value);

        public void Write(int value) => writer.Write(value);

        public void Write(uint value) => writer.Write(value);

        public void Write(long value) => writer.Write(value);

        public void Write(ulong value) => writer.Write(value);

        public void Write(float value) => writer.Write(value);

        public void Write(double value) => writer.Write(value);

        public ulong Position
        {
            get => (ulong)stream.Position;
            set => stream.Position = (long)value;
        }

        private object ReadPrimitive(Type type)
        {
            var typename = type.Name;
            switch (typename)
            {
                case "Int32":
                    return ReadInt32();
                case "UInt32":
                    return ReadUInt32();
                case "Int16":
                    return ReadInt16();
                case "UInt16":
                    return ReadUInt16();
                case "Byte":
                    return ReadByte();
                case "Int64" when is32Bit:
                    return (long)ReadInt32();
                case "Int64":
                    return ReadInt64();
                case "UInt64" when is32Bit:
                    return (ulong)ReadUInt32();
                case "UInt64":
                    return ReadUInt64();
                default:
                    return null;
            }
        }

        public T ReadClass<T>(ulong addr) where T : new()
        {
            Position = addr;
            return ReadClass<T>();
        }

        public T ReadClass<T>() where T : new()
        {
            var type = typeof(T);
            if (type.IsPrimitive)
            {
                return (T)ReadPrimitive(type);
            }
            else
            {
                var t = new T();
                foreach (var i in t.GetType().GetFields())
                {
                    if (!attributeCache.TryGetValue(i, out var versionAttribute))
                    {
                        if (Attribute.IsDefined(i, typeof(VersionAttribute)))
                        {
                            versionAttribute = (VersionAttribute)Attribute.GetCustomAttribute(i, typeof(VersionAttribute));
                            attributeCache.Add(i, versionAttribute);
                        }
                    }
                    if (versionAttribute != null)
                    {
                        if (version < versionAttribute.Min || version > versionAttribute.Max)
                            continue;
                    }
                    if (i.FieldType.IsPrimitive)
                    {
                        i.SetValue(t, ReadPrimitive(i.FieldType));
                    }
                    else
                    {
                        if (!readClassCache.TryGetValue(i.FieldType, out var methodInfo))
                        {
                            methodInfo = readClass.MakeGenericMethod(i.FieldType);
                            readClassCache.Add(i.FieldType, methodInfo);
                        }
                        var value = methodInfo.Invoke(this, null);
                        i.SetValue(t, value);
                        break;
                    }
                }
                return t;
            }
        }

        public T[] ReadClassArray<T>(long count) where T : new()
        {
            var t = new T[count];
            for (var i = 0; i < count; i++)
            {
                t[i] = ReadClass<T>();
            }
            return t;
        }

        public T[] ReadClassArray<T>(ulong addr, long count) where T : new()
        {
            Position = addr;
            return ReadClassArray<T>(count);
        }

        public string ReadStringToNull(ulong addr)
        {
            Position = addr;
            var bytes = new List<byte>();
            byte b;
            while ((b = ReadByte()) != 0)
                bytes.Add(b);
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                reader.Close();
                writer.Close();
                stream.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}