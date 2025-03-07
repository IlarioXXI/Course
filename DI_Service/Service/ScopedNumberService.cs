namespace DI_Service.Service
{
    public class ScopedNumberService : IScopedNumberService
    {
        private readonly int Number;

        readonly Random rdn = new Random();
        public ScopedNumberService()
        {
            Number = rdn.Next();
        }
        public int GetNumber()
        {
            return Number;
        }
    }
}
