namespace DI_Service.Service
{
    public class TransientNumberService : ITransientNumberService
    {
        readonly Random rdn = new Random();
        private readonly int Number;
        public TransientNumberService()
        {
            Number = rdn.Next();
        }
        public int GetNumber()
        {
            return Number;
        }
    }
}
