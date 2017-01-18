using System;
using Newtonsoft.Json;

namespace AServer
{
    class JSON
    {
        public string output;

        public JSON(object Convert)
        {
            output = JsonConvert.SerializeObject(Convert);
            //Console.WriteLine(output);
        }

        ~JSON()
        {
            ((IDisposable)this).Dispose();
        }
    }
}
