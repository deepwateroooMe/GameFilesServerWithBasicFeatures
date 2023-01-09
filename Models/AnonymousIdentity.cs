using System.Security.Principal;

namespace MyServer.Models
{
    class AnonymousIdentity : IIdentity
    {
        public string Name => "Anonymouse";

        public string AuthenticationType => "";

        public bool IsAuthenticated => false;
    }
}
