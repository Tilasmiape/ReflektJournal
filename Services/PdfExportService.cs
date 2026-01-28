using System.Text;
using ReflektJournal.Services.Interfaces;
using Microsoft.Maui.ApplicationModel;

namespace ReflektJournal.Services;

public sealed class PdfExportService : IPdfExportService
{
    public async Task<string> ExportTimelineAsync(IReadOnlyList<IPdfExportService.ExportCard> cards)
    {
        var fileName = $"ReflektJournal_Timeline_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        var path = Path.Combine(FileSystem.CacheDirectory, fileName);

        var pdfBytes = SimplePdf.Build(cards);
        File.WriteAllBytes(path, pdfBytes);

        await Launcher.Default.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(path)
        });

        return path;
    }

    private static class SimplePdf
    {
        // A4 in points
        private const int W = 595;
        private const int H = 842;

        public static byte[] Build(IReadOnlyList<IPdfExportService.ExportCard> cards)
        {
            var lines = new List<string>
            {
                "ReflektJournal — Timeline Export",
                $"Exported: {DateTime.Now:f}",
                ""
            };

            foreach (var c in cards.OrderByDescending(x => x.DateIso))
            {
                lines.Add($"{c.DateIso} — {c.Title}");
                if (!string.IsNullOrWhiteSpace(c.Mood)) lines.Add($"Mood: {c.Mood}");
                if (c.Tags.Count > 0) lines.Add($"Tags: {string.Join(", ", c.Tags)}");
                lines.Add("");

                foreach (var l in Wrap(ToPlain(c.ContentMarkdown), 95))
                    lines.Add(l);

                lines.Add("");
                lines.Add(new string('-', 60));
                lines.Add("");
            }

            return WritePdfFromLines(lines);
        }

        private static string ToPlain(string md)
        {
            if (string.IsNullOrWhiteSpace(md)) return "";
            return md.Replace("\r", "").Trim();
        }

        private static IEnumerable<string> Wrap(string text, int width)
        {
            if (string.IsNullOrWhiteSpace(text))
                yield break;

            foreach (var paragraph in text.Split('\n'))
            {
                var p = paragraph.TrimEnd();
                if (p.Length == 0)
                {
                    yield return "";
                    continue;
                }

                var i = 0;
                while (i < p.Length)
                {
                    var take = Math.Min(width, p.Length - i);
                    var chunk = p.Substring(i, take);

                    // try break on space
                    if (i + take < p.Length)
                    {
                        var lastSpace = chunk.LastIndexOf(' ');
                        if (lastSpace > 20)
                        {
                            chunk = chunk[..lastSpace];
                            take = lastSpace + 1;
                        }
                    }

                    yield return chunk.TrimEnd();
                    i += take;
                }
            }
        }

        private static byte[] WritePdfFromLines(List<string> lines)
        {
            // Build pages
            var pageContents = new List<string>();
            var sb = new StringBuilder();

            int fontSize = 11;
            int leading = 14;
            int margin = 48;

            int x = margin;
            int y = H - margin;

            void NewPage()
            {
                if (sb.Length > 0)
                {
                    pageContents.Add(sb.ToString());
                    sb.Clear();
                }
                y = H - margin;
                sb.Append("BT\n");
                sb.Append($"/F1 {fontSize} Tf\n");
                sb.Append($"{x} {y} Td\n");
                sb.Append($"{leading} TL\n");
            }

            string Esc(string s) =>
                s.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");

            NewPage();

            foreach (var line in lines)
            {
                if (y <= margin + leading * 2)
                {
                    sb.Append("ET\n");
                    NewPage();
                }

                sb.Append($"({Esc(line)}) Tj\nT*\n");
                y -= leading;
            }

            sb.Append("ET\n");
            pageContents.Add(sb.ToString());

            // PDF objects
            var objects = new List<string>();

            // 1: Catalog
            objects.Add("<< /Type /Catalog /Pages 2 0 R >>");

            // 2: Pages (kids filled later)
            var kidsRefs = Enumerable.Range(0, pageContents.Count).Select(i => $"{3 + i * 2} 0 R").ToList();
            objects.Add($"<< /Type /Pages /Kids [ {string.Join(" ", kidsRefs)} ] /Count {pageContents.Count} >>");

            // Page + content objects per page
            for (int i = 0; i < pageContents.Count; i++)
            {
                int pageObj = 3 + i * 2;
                int contentObj = 4 + i * 2;

                objects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {W} {H}] /Resources << /Font << /F1 {3 + pageContents.Count * 2} 0 R >> >> /Contents {contentObj} 0 R >>");

                var content = pageContents[i];
                var bytes = Encoding.ASCII.GetBytes(content);
                objects.Add($"<< /Length {bytes.Length} >>\nstream\n{content}endstream");
            }

            // last: Font
            objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");

            // write file
            var outSb = new StringBuilder();
            outSb.Append("%PDF-1.4\n");

            var offsets = new List<int> { 0 };
            for (int i = 0; i < objects.Count; i++)
            {
                offsets.Add(outSb.Length);
                outSb.Append($"{i + 1} 0 obj\n{objects[i]}\nendobj\n");
            }

            var xrefPos = outSb.Length;
            outSb.Append($"xref\n0 {objects.Count + 1}\n");
            outSb.Append("0000000000 65535 f \n");
            for (int i = 1; i <= objects.Count; i++)
                outSb.Append($"{offsets[i]:D10} 00000 n \n");

            outSb.Append($"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xrefPos}\n%%EOF");

            return Encoding.ASCII.GetBytes(outSb.ToString());
        }
    }
}
