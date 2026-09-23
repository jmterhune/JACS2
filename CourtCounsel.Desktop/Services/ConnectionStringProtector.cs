using System.Security.Cryptography;
using System.Text;

namespace CourtCounsel.Desktop.Services;

// Encrypts the connection string at rest so it doesn't sit as plain text
// in settings.json (readable in Notepad, grep, a screen-share, etc.).
//
// This deliberately does NOT use Windows DPAPI: DPAPI keys are tied to the
// specific user account or machine that encrypted the data, so a settings
// file produced by a domain admin on their machine would fail to decrypt
// on every end-user's machine it gets deployed to — which defeats "install
// with the settings already set." Instead this uses a fixed, embedded
// AES-256-GCM key, which is portable across every machine running this
// build of the app.
//
// Threat model this actually addresses: a user opening settings.json out
// of curiosity, or the file being copied/emailed/screenshotted incidentally,
// doesn't hand over the SQL login in plain text. It does NOT protect against
// a determined attacker who decompiles this app — the key is recoverable
// from the binary. Treat the SQL login itself (least-privilege, scoped to
// what this app needs) as the real security boundary, not this encryption.
public static class ConnectionStringProtector
{
    // Any fixed passphrase works here — changing it invalidates every
    // previously-encrypted settings file, so treat it as part of the app's
    // deployed identity, not a rotatable secret.
    private const string Passphrase = "CourtCounselDesktop.v1.settings-protection";

    private static byte[] DeriveKey() => SHA256.HashData(Encoding.UTF8.GetBytes(Passphrase));

    public static string Protect(string? plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return "";

        var key = DeriveKey();
        var nonce = RandomNumberGenerator.GetBytes(AesGcm.NonceByteSizes.MaxSize);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = new byte[plainBytes.Length];
        var tag = new byte[AesGcm.TagByteSizes.MaxSize];

        using var aes = new AesGcm(key, tag.Length);
        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

        // Pack nonce + tag + ciphertext into one base64 blob.
        var packed = new byte[nonce.Length + tag.Length + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, packed, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, packed, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherBytes, 0, packed, nonce.Length + tag.Length, cipherBytes.Length);
        return Convert.ToBase64String(packed);
    }

    public static string Unprotect(string? protectedText)
    {
        if (string.IsNullOrEmpty(protectedText)) return "";

        try
        {
            var packed = Convert.FromBase64String(protectedText);
            var nonceSize = AesGcm.NonceByteSizes.MaxSize;
            var tagSize = AesGcm.TagByteSizes.MaxSize;

            var nonce = packed[..nonceSize];
            var tag = packed[nonceSize..(nonceSize + tagSize)];
            var cipherBytes = packed[(nonceSize + tagSize)..];
            var plainBytes = new byte[cipherBytes.Length];

            using var aes = new AesGcm(DeriveKey(), tagSize);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch
        {
            // Not encrypted (e.g. an older plain-text settings.json) or
            // corrupt — fall back to treating it as plain text rather than
            // losing the value outright.
            return protectedText;
        }
    }
}
