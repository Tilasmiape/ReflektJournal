using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using ReflektJournal.Services.Interfaces;

namespace ReflektJournal.Services;

public sealed class MarkdownService : IMarkdownService
{
    // NOTE: This is a safe, minimal renderer for your required features:
    // bold, italics, headings, lists, links.
    // It HTML-encodes first, then applies markdown formatting, so raw HTML isn't injected.

    public string ToHtml(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return "";

        // Encode first to avoid HTML injection
        var encoded = WebUtility.HtmlEncode(markdown);
        var lines = encoded.Replace("\r\n", "\n").Split('\n');

        var sb = new StringBuilder();
        var inUl = false;
        var inOl = false;

        void CloseListsIfOpen()
        {
            if (inUl) { sb.Append("</ul>"); inUl = false; }
            if (inOl) { sb.Append("</ol>"); inOl = false; }
        }

        foreach (var raw in lines)
        {
            var line = raw.TrimEnd();

            // Empty line -> close lists, add spacing
            if (string.IsNullOrWhiteSpace(line))
            {
                CloseListsIfOpen();
                sb.Append("<div class=\"md-spacer\"></div>");
                continue;
            }

            // Headings (#, ##, ###)
            if (line.StartsWith("### "))
            {
                CloseListsIfOpen();
                sb.Append("<h3>").Append(Inline(line[4..])).Append("</h3>");
                continue;
            }
            if (line.StartsWith("## "))
            {
                CloseListsIfOpen();
                sb.Append("<h2>").Append(Inline(line[3..])).Append("</h2>");
                continue;
            }
            if (line.StartsWith("# "))
            {
                CloseListsIfOpen();
                sb.Append("<h1>").Append(Inline(line[2..])).Append("</h1>");
                continue;
            }

            // Unordered list (- or *)
            if (Regex.IsMatch(line, @"^(-|\*)\s+"))
            {
                if (inOl) { sb.Append("</ol>"); inOl = false; }
                if (!inUl) { sb.Append("<ul>"); inUl = true; }

                var item = Regex.Replace(line, @"^(-|\*)\s+", "");
                sb.Append("<li>").Append(Inline(item)).Append("</li>");
                continue;
            }

            // Ordered list (1. 2. 3.)
            if (Regex.IsMatch(line, @"^\d+\.\s+"))
            {
                if (inUl) { sb.Append("</ul>"); inUl = false; }
                if (!inOl) { sb.Append("<ol>"); inOl = true; }

                var item = Regex.Replace(line, @"^\d+\.\s+", "");
                sb.Append("<li>").Append(Inline(item)).Append("</li>");
                continue;
            }

            // Normal paragraph
            CloseListsIfOpen();
            sb.Append("<p>").Append(Inline(line)).Append("</p>");
        }

        CloseListsIfOpen();
        return sb.ToString();
    }

    public string ToPlainText(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return "";

        // Remove common markdown tokens for word counting / previews
        var s = markdown;
        s = Regex.Replace(s, @"```[\s\S]*?```", " ");   // code blocks
        s = Regex.Replace(s, @"`.+?`", " ");           // inline code
        s = Regex.Replace(s, @"!\[.*?\]\(.*?\)", " "); // images
        s = Regex.Replace(s, @"\[([^\]]+)\]\([^)]+\)", "$1"); // links -> text
        s = Regex.Replace(s, @"[#>*_~\-]", " ");
        s = Regex.Replace(s, @"\s+", " ").Trim();
        return s;
    }

    private static string Inline(string s)
    {
        // Links: [text](url)
        s = Regex.Replace(s, @"\[(?<t>[^\]]+)\]\((?<u>[^)]+)\)", m =>
        {
            var text = m.Groups["t"].Value;
            var url = m.Groups["u"].Value;
            // still encoded; allow http(s) only
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return text;

            return $"<a href=\"{url}\" target=\"_blank\" rel=\"noopener noreferrer\">{text}</a>";
        });

        // Bold: **text**
        s = Regex.Replace(s, @"\*\*(.+?)\*\*", "<strong>$1</strong>");

        // Italic: *text*  (avoid matching ** already)
        s = Regex.Replace(s, @"(?<!\*)\*(?!\s)(.+?)(?<!\s)\*(?!\*)", "<em>$1</em>");

        return s;
    }
}
