using System.Security.Principal;

namespace MyServer.Models
{
    public class AnonymousUser : IPrincipal
    {
        public bool IsInRole(string role)
            {
                return false;
            }

        public IIdentity Identity => new AnonymousIdentity();
    }

}
