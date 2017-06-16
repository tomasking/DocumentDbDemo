using System;

namespace DocumentDbDemo
{
    using System.Threading.Tasks;

    class Program
    {
        public static void Main(string[] args)
        {
            Start();
        }

        public static void Start()
        {
            using (var scope = IoC.CreateContainer().BeginLifetimeScope())
            {
                Console.WriteLine("Press any key to Exit");
                Console.ReadLine();
            }
        }
    }
}