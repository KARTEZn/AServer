using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
