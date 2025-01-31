﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Silkroad.Materials
{
    internal class O2File
    {
        public List<O2FileElement> Elements = new(128);

        public O2File(int xsec, int ysec)
        {
            var buffer = Program.Map.GetFileBuffer($@"{ysec}\{xsec}.o2");
            if (buffer == null)
                return;

            using var reader = new BinaryReader(new MemoryStream(buffer));
            reader.BaseStream.Position += 12; //skip header
            for (int i = 0; i < 144; i++)
            {
                var count = reader.ReadInt16();
                for (int j = 0; j < count; j++)
                {
                    var obj = new O2FileElement
                    {
                        Index = reader.ReadInt32(),
                        Position = reader.ReadVector3(),

                        UnknownFlag1 = reader.ReadUInt16(),

                        Theta = reader.ReadSingle(),
                        Id = reader.ReadInt32(),

                        UnknownFlag2 = reader.ReadUInt16(),

                        RegionX = reader.ReadByte(),
                        RegionY = reader.ReadByte()
                    };
                    float num2;
                    for (num2 = obj.Theta; num2 < 0f; num2 += 6.2831855f)
                    {
                    }
                    while (num2 > 6.2831855f)
                    {
                        num2 -= 6.2831855f;
                    }

                    if (obj.Theta != num2)
                        Console.WriteLine("Theta error:" + obj.Theta + " " + num2);

                    obj.Theta = num2;

                    obj.Position.X += (obj.RegionX - xsec) * 1920;
                    obj.Position.Z += (obj.RegionY - ysec) * 1920;

                    Elements.Add(obj);
                }
            }
        }
    }
}