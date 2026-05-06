using System.Net;

namespace Velocity.Service.Networking;

public interface IAddressSelector
{
    IReadOnlyList<IPAddress> Select(IReadOnlyList<IPAddress> addresses, AddressFamilyPreference preference);
}