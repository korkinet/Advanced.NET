namespace RunApp
{
    internal class Service : IService
    {
        public int SomeIntValue { get; set; }
        public string SomeStringValue { get; set; }

        public Service()
        {
            SomeIntValue = 10;
            SomeStringValue = "2020";
        }
    }
}