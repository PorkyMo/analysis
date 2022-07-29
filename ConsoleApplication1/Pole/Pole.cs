using DataAnalyst.Base;

namespace DataAnalyst.Pole
{
    public class Pole
    {
        public Direction Direction;
        public PriceItem Item;

        public Pole()
        {
        }

        public Pole(Direction direction, PriceItem priceItem)
        {
            Direction = direction;
            Item = priceItem;
        }
    }
}
