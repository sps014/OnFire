using OnFire.Auth;
using OnFire.Data;
using OnFire.Net;
using System;
using System.Threading.Tasks;

namespace OnFire
{
    class Program
    {
        static string baseURL = "";
        static  void Main(string[] args)
        {
            Test();
            WriteTest();
            Console.ReadKey();
        }
  
        class Detail
        {
            public string Class { get; set; } = "Y5";
            public string Name { get; set; } = "MNNMM8";
        }
    }
}
