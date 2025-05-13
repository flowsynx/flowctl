using Microsoft.AspNetCore.DataProtection;

namespace FlowCtl.Infrastructure.Services.Authentication;

public class DataProtectorWrapper : IDataProtectorWrapper
{
    private readonly IDataProtector _protector;

    public DataProtectorWrapper(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("AuthenticationManager.Protector");
    }

    public string Protect(string input) => _protector.Protect(input);
    public string Unprotect(string input) => _protector.Unprotect(input);
}
