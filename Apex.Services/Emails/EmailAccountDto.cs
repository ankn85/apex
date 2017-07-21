namespace Apex.Services.Emails
{
    public sealed class EmailAccountDto
    {
        public EmailAccountDto(
            int id,
            string email,
            string displayName,
            string host,
            int port,
            bool enableSsl,
            bool useDefaultCredentials,
            bool isDefaultEmailAccount)
            : this(id, email, displayName, host, port, null, null, enableSsl, useDefaultCredentials, isDefaultEmailAccount)
        {
        }

        public EmailAccountDto(
            int id,
            string email,
            string displayName,
            string host,
            int port,
            string userName,
            string password,
            bool enableSsl,
            bool useDefaultCredentials,
            bool isDefaultEmailAccount)
        {
            Id = id;
            Email = email;
            DisplayName = displayName;
            Host = host;
            Port = port;
            UserName = userName;
            Password = password;
            EnableSsl = enableSsl;
            UseDefaultCredentials = useDefaultCredentials;
            IsDefaultEmailAccount = isDefaultEmailAccount;
        }

        public int Id { get; }

        public string Email { get; }

        public string DisplayName { get; }

        public string Host { get; }

        public int Port { get; }

        public string UserName { get; }

        public string Password { get; }

        public bool EnableSsl { get; }

        public bool UseDefaultCredentials { get; }

        public bool IsDefaultEmailAccount { get; }
    }
}
