using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorld
{
    enum Didier
    {
        Test1,
        // with comment
        Test2
    }

    /// <summary>
    /// A sweet class.
    /// </summary>
    class Program
    {
        private string _toto;

        private Didier a;

        public int Name { get; set; }

        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name=""args"">The arguments.</param>
        void Salut(string text)
        {
            var c = Didier.Test1;
            this.a = c;
            Console.WriteLine(text);
        }

        static void Main(string[] args)
        {
            var a = 1;
            a = a + 1;
            Console.WriteLine("Hello, World!");
        }
    }
}