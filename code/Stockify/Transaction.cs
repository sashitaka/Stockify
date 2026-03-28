namespace Stockify
{
    public class Transaction
    {
        private string date;
        private float montant;
        private string type; //buy,sell
        private string userID;
        private string stock;

        public Transaction(string date, float montant, string type, string userID, string stock)
        {
            this.date = date;
            this.montant = montant;
            this.type = type;
            this.userID = userID;
            this.stock = stock;
        }

        public string Date { get => date; set => date = value; }
        public float Montant { get => montant; set => montant = value; }
        public string Type { get => type; set => type = value; }
        public string UserID { get => userID; set => userID = value; }
        public string Stock { get => stock; set => stock = value; }
    }
}
