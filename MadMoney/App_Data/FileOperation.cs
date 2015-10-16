using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace MadMoney
{
    public class FileOperation
    {
        public static void WriteToAPKFile(APKStaticStore[] list)
        {
            var serializer = new BinaryFormatter();
            using (var stream = File.OpenWrite("D:\\APKTree.txt"))
            {
                serializer.Serialize(stream, list);
            }
        }
        public static APKStaticStore[] ReadAPKFile()
        {
            APKStaticStore[] list = null;
            var serializer = new BinaryFormatter();
            using (var stream = File.OpenRead("D:\\APKTree.txt"))
            {
                try
                {
                    list = (APKStaticStore[])serializer.Deserialize(stream);
                }
                catch (Exception){return list;}
            }
            return list;
        }
    }
}