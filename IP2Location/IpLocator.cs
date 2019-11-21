using System.Diagnostics.Contracts;
using System.Net;

namespace SockPuppet.IP2Location
{
    public interface IpLocator
    {
        [Pure]
        Location Locate(IPAddress ip);
    }
}