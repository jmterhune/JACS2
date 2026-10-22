namespace CourtCounsel.Desktop.Services;

public enum SuffixMode
{
    None,       // case type isn't CF — no suffix segment
    Numeric4,   // Sarasota / Venice CF cases — 4-digit numeric suffix
    Letters2    // DeSoto / Manatee CF cases — 2-letter suffix
}

// Reimplements the case-number composite widget from EditHistory.ascx.cs:
// County-Year-Type-Sequence[-Suffix], e.g. "S-2016-CF-004034" or
// "S-2023-CF-006436-0000". County letters: D=DeSoto, M=Manatee,
// S=Sarasota, V=Venice.
public static class CaseNumberHelper
{
    public static SuffixMode GetSuffixMode(string countyLetter, string caseTypeCode)
    {
        if (!string.Equals(caseTypeCode, "CF", StringComparison.OrdinalIgnoreCase))
            return SuffixMode.None;

        return countyLetter?.ToUpperInvariant() switch
        {
            "S" or "V" => SuffixMode.Numeric4,
            "D" or "M" => SuffixMode.Letters2,
            _ => SuffixMode.None
        };
    }

    public static string PadSequence(string sequence)
    {
        var digitsOnly = new string((sequence ?? "").Where(char.IsDigit).ToArray());
        return digitsOnly.PadLeft(6, '0');
    }

    public static string Build(string countyLetter, string caseYear, string caseTypeCode, string sequence, string? suffix)
    {
        var padded = PadSequence(sequence);
        var typeCode = (caseTypeCode ?? "").Trim().ToUpperInvariant();
        var result = $"{countyLetter}-{caseYear}-{typeCode}-{padded}";
        if (!string.IsNullOrWhiteSpace(suffix))
            result += $"-{suffix.Trim().ToUpperInvariant()}";
        return result;
    }

    public record CaseNumberParts(string CountyLetter, string CaseYear, string CaseTypeCode, string Sequence, string Suffix);

    public static CaseNumberParts Parse(string? caseNumber)
    {
        if (string.IsNullOrWhiteSpace(caseNumber))
            return new CaseNumberParts("", "", "", "", "");

        var parts = caseNumber.Split('-');
        return new CaseNumberParts(
            parts.Length > 0 ? parts[0] : "",
            parts.Length > 1 ? parts[1] : "",
            parts.Length > 2 ? parts[2] : "",
            parts.Length > 3 ? parts[3] : "",
            parts.Length > 4 ? parts[4] : "");
    }
}
