using System.Security.Principal;

namespace MyServer.Models
{
    public class AnonymousIdentity : IIdentity
    {
        public string Name => "Anonymouse";

        public string AuthenticationType => "";

        public bool IsAuthenticated => false;
    }
}
