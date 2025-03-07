namespace DI_Service.Service
{
    public class SingletonNumberService : ISingletonNumberService
    {
         readonly Random rdn = new Random();

        private readonly int Number;
        public int GetNumber()
        {
            return Number;
        }
        public SingletonNumberService()
        {
            Number = rdn.Next();
        }
    }
}
