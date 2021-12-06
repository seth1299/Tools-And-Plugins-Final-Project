using System;

namespace Zork
{
    public class ConsoleOutputService :IOutputService
    {//11/17/21 refer to Video @ 48minutes.
        public void Clear() => Console.Clear();
       
        public void Write(string value) => Console.Write(value);

        public void Write(object value) => Write(value.ToString());

        public void WriteLine(object value) => WriteLine(value.ToString());
       
        public void WriteLine(string value) => Console.WriteLine(value);
        
    }
}
