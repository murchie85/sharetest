```c#
string appendedCerts = "";
var parts = (string.IsNullOrOrEmpty(uploadedCerts) ? certAlias : $"{uploadedCerts},{certAlias}").Split(',');
var uniqueParts = new List<string>();
foreach (var part in parts)
{
    var trimmedPart = part.Trim();
    if (!string.IsNullOrWhiteSpace(trimmedPart) && !uniqueParts.Contains(trimmedPart))
        uniqueParts.Add(trimmedPart);
}
appendedCerts = string.Join(",", uniqueParts.ToArray());
```