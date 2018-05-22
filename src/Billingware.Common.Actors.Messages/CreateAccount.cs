namespace Billingware.Common.Actors.Messages
{
    public struct CreateAccount
    {
        public CreateAccount(string accountNumber, string @alias, string extra)
        {
            AccountNumber = accountNumber;
            Alias = alias;
            Extra = extra;
        }
        public string AccountNumber { get; }

        /// <summary>
        /// Another caption for the account
        /// </summary>
        public string Alias { get; }
        /// <summary>
        /// JSON field
        /// </summary>
        public string Extra { get; }
    }
}