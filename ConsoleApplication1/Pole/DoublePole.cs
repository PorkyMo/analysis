using DataAnalyst.Base;

namespace DataAnalyst.Pole
{
    public class DoublePole
    {
        public string Code { get; set; }
        public Pole Pole1 { get; set; }
        public Pole Pole2 { get; set; }
        public Period Period { get; set; }

        public DoublePole(string code, Period period, Pole pole1, Pole pole2)
        {
            if (pole1.Direction != pole2.Direction)
            {
                throw new System.Exception("poles not in same direction");
            }

            Code = code;
            Pole1 = pole1;
            Pole2 = pole2;
            Period = period;
        }
    }
}
