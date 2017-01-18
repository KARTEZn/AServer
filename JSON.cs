using System;
using Newtonsoft.Json;

namespace AServer
{
    class JSON
    {
        public string output;

        public JSON(object Convert)
        {
            try
            {
                output = JsonConvert.SerializeObject(Convert);
            }
            catch(Exception _Exception)
                    {
                Console.WriteLine("Ошибка [JSON]: " + _Exception.Message);
            }
            //Console.WriteLine(output);
        }

        ~JSON()
        {
            ((IDisposable)this).Dispose();
        }
    }
}
