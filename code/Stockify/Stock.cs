namespace Stockify
{
    public class Stock
    {
        private string nom;
        private float currentValue;

        public string Nom { get => nom; set => nom = value; }
        public float Value { get => currentValue; set => currentValue = value; }
    }
}
