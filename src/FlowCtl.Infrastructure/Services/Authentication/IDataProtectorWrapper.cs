namespace FlowCtl.Infrastructure.Services.Authentication;

public interface IDataProtectorWrapper
{
    string Protect(string input);
    string Unprotect(string input);
}