using System.Transactions;

namespace Stockify.Business
{
    public class User
    {
        private int id;
        private string username;
        private int password;
        private string email;
        private string phone;
        private List<PortfolioItem> portfolioListe;
        private List<Transaction> transactionsListe;


        public User(int id, string username, int password, string email, string phone)
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.email = email;
            this.phone = phone;
            this.portfolioListe = new List<PortfolioItem>();
            this.transactionsListe = new List<Transaction>();
        }

        public int Id { get => id; set => id = value; }
        public string Username { get => username; set => username = value; }
        public int Password { get => password; set => password = value; }
        public string Email { get => email; set => email = value; }
        public string Phone { get => phone; set => phone = value; }
        public List<PortfolioItem> Portfolio { get => portfolioListe; set => portfolioListe = value; }


    }
}
