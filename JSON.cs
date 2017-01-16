using System;
using Newtonsoft.Json;

namespace AServer
{
    class JSON
    {
        public JSON(object Convert)
        {
            string output = JsonConvert.SerializeObject(Convert);
            Console.WriteLine(output);
        }

        ~JSON()
        {
            ((IDisposable)this).Dispose();
        }
    }
}
