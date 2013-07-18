namespace UnityRxExample
{
    public sealed class Unit
    {
        public static readonly Unit Instance = new Unit();

        private Unit() { }
    }
}