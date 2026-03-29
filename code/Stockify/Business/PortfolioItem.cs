namespace Stockify.Models
{
    public class PortfolioItem : Stock
    {
        private float ownedValue;
        private float quantityOwned;
        private decimal currentPrice;

        public float OwnedValue { get => ownedValue; set => ownedValue = value; }
        public float QuantityOwned { get => quantityOwned; set => quantityOwned = value; }
        public decimal CurrentPrice { get => currentPrice; set => currentPrice = value; }
    }
}
