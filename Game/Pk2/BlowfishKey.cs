using System;
using System.IO;
using System.Text;

namespace Silkroad.Pk2
{
    public class BlowfishKey
    {
        public static byte[] Generate(string key)
        {
            byte[] baseKey = new byte[] { 0x03, 0xF8, 0xE4, 0x44, 0x88, 0x99, 0x3F, 0x64, 0xFE, 0x35 };
            // Their key modification algorithm for the final blowfish key
            var bfKey = new byte[key.Length];
            for (byte x = 0; x < key.Length; ++x)
            {
                bfKey[x] = (byte)(key[x] ^ baseKey[x]);
            }
            return bfKey;
        }

        public static byte[] Generate(byte[] key)
        {
            byte[] baseKey = new byte[] { 0x03, 0xF8, 0xE4, 0x44, 0x88, 0x99, 0x3F, 0x64, 0xFE, 0x35 };
            // Their key modification algorithm for the final blowfish key
            var bfKey = new byte[key.Length];
            for (byte x = 0; x < key.Length; ++x)
            {
                bfKey[x] = (byte)(key[x] ^ baseKey[x]);
            }
            return bfKey;
        }

        public static byte[] Find(string clientpath)
        {
            FileStream file = new FileStream(Path.Combine(clientpath, "Silkroad.exe"), FileMode.Open);
            
            byte[] FileArray = StreamToArray(file);
            byte[] stringPattern = Encoding.ASCII.GetBytes("*.pk2");
            uint position = FindStringPattern(stringPattern, FileArray, 0x400000, 0x68, 1) + 262;
            file.Seek(position, SeekOrigin.Begin);
            byte[] keyAddress = new byte[4];
            file.Read(keyAddress, 0, 4);
            file.Seek(BitConverter.ToInt32(keyAddress, 0) - 0x400000, SeekOrigin.Begin);
            byte[] key = new byte[6];
            file.Read(key, 0, 6);
            return Generate(Encoding.ASCII.GetString(key));
        }

        public static byte[] StreamToArray(Stream sourceStream)
        {
            return new BinaryReader(sourceStream).ReadBytes((int)sourceStream.Length);
        }

        private static uint FindStringPattern(byte[] StringByteArray, byte[] FileArray, uint BaseAddress, byte StringWorker, uint Result)
        {
            uint MyPosition = 0;
            byte[] StringWorkerAddress = { StringWorker, 0x00, 0x00, 0x00, 0x00 };
            byte[] StringAddress = new byte[4];
            StringAddress = BitConverter.GetBytes(BaseAddress + FindPattern(StringByteArray, FileArray, 1));
            StringWorkerAddress[1] = StringAddress[0];
            StringWorkerAddress[2] = StringAddress[1];
            StringWorkerAddress[3] = StringAddress[2];
            StringWorkerAddress[4] = StringAddress[3];
            MyPosition = BaseAddress + FindPattern(StringWorkerAddress, FileArray, Result);
            return MyPosition - BaseAddress;
        }

        private static uint FindPattern(byte[] Pattern, byte[] FileByteArray, uint Result)
        {
            uint MyPosition = 0;
            uint ResultCounter = 0;
            for (uint PositionFileByteArray = 0; PositionFileByteArray < FileByteArray.Length - Pattern.Length; PositionFileByteArray++)
            {
                bool found = true;
                for (uint PositionPattern = 0; PositionPattern < Pattern.Length; PositionPattern++)
                {
                    if (FileByteArray[PositionFileByteArray + PositionPattern] != Pattern[PositionPattern])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    ResultCounter += 1;
                    if (Result == ResultCounter)
                    {
                        MyPosition = PositionFileByteArray;
                        break;
                    }
                }
            }
            return MyPosition;
        }
    }
}