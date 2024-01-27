using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;

namespace Test
{
    public class DefectsDataTable:DataTable
    {
        public string Name;
        string path;
        IContainer container;
        JsonSerializer serializer= new JsonSerializer();

        public DefectsDataTable()
        {

        }

        public void addPath(string path)
        {
            this.path = Path.GetFullPath(path);
        }
    }
}
