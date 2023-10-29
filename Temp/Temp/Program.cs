
namespace Temp
{
    internal class Program
    {
        public int Num;
        static void Main(string[] args)
        {
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(i);
            }

}
        string ExtendableString(params string[] Values)
        {
            // -x-x-x-
            string FinalString = string.Join("-", Values);
            return FinalString;
        }
    }
}