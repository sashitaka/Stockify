namespace Stockify.Business
{
    public class User
    {
        private int id;
        private string name;
        private string email;
        private string passwordHashed;
        private decimal balance;

        public User(int id, string name, string email, string passwordHashed, decimal balance)
        {
            this.id = id;
            this.name = name;
            this.email = email;
            this.passwordHashed = passwordHashed;
            this.balance = balance;
        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public string PasswordHashed { get => passwordHashed; set => passwordHashed = value; }
        public decimal Balance { get => balance; set => balance = value; }
    }
}
