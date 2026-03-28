namespace Stockify.Models
{
    public class PortfolioItem : Stock
    {
        private float ownedValue;
        private float quantityOwned;

        public float OwnedValue { get => ownedValue; set => ownedValue = value; }
        public float QuantityOwned { get => quantityOwned; set => quantityOwned = value; }
    }
}
