namespace Catalog.ServiceDefaults;

using Microsoft.AspNetCore.Builder;

public static class SecurityHeaderDefinitions
{
    public static HeaderPolicyCollection AddDefaultApiSecurityHeaders()
    {
        var policies = new HeaderPolicyCollection();

        policies.AddFrameOptionsDeny();
        policies.AddContentTypeOptionsNoSniff();
        policies.AddStrictTransportSecurityMaxAge();
        policies.RemoveServerHeader();

        policies.AddContentSecurityPolicy(p =>
        {
            p.AddDefaultSrc().None();
            p.AddFrameAncestors().None();
        });

        policies.AddReferrerPolicyNoReferrer();
        policies.AddPermissionsPolicy(p =>
        {
            p.AddAccelerometer().None();
            p.AddAutoplay().None();
            p.AddBluetooth().None();
            p.AddFullscreen().None();
            p.AddGeolocation().None();
            p.AddGyroscope().None();
            p.AddHid().None();
            p.AddMagnetometer().None();
            p.AddCamera().None();
            p.AddMicrophone().None();
            p.AddMidi().None();
            p.AddPayment().None();
            p.AddPictureInPicture().None();
            p.AddPublickeyCredentialsGet().None();
            p.AddScreenWakeLock().None();
            p.AddSyncXHR().None();
            p.AddUsb().None();
            p.AddWebShare().None();
            p.AddXrSpatialTracking().None();
        });
        return policies;
    }
}