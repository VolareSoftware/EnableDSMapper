using System;
using System.Linq;
using Microsoft.Web.Administration;

internal static class EnableDSMapper
{
    // Original: https://techcommunity.microsoft.com/t5/iis-support-blog/the-complete-list-of-changes-to-make-to-activate-client/ba-p/826391

    private const string SiteToChange = "SharePoint - 80";

    private static void Main()
    {
        using var serverManager = new ServerManager();
        var config = serverManager.GetApplicationHostConfiguration();
        var clientCertificateMappingAuthenticationSection = config.GetSection("system.webServer/security/authentication/clientCertificateMappingAuthentication", "Default Web Site");
        clientCertificateMappingAuthenticationSection["enabled"] = true;

        var accessSection = config.GetSection("system.webServer/security/access", "Default Web Site");
        accessSection["sslFlags"] = @"Ssl, SslNegotiateCert";

        // Iterate through all the sites on the sever get the site named 'Default Web Site’
        var selectedSite = serverManager.Sites.FirstOrDefault(s => 
            s.Name.Equals(SiteToChange, StringComparison.OrdinalIgnoreCase));

        // Iterate through the bindings of the site and attempt to retrieve an https binding
        var sslBinding = selectedSite?.Bindings.FirstOrDefault(b => 
            b.Protocol.Equals("https", StringComparison.OrdinalIgnoreCase));

        if (sslBinding != null)
        {
            // Enable the DS Mapper
            sslBinding.UseDsMapper = true;
        }

        serverManager.CommitChanges();
    }
}