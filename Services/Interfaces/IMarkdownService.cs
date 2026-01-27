using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflektJournal.Services.Interfaces
{
    public interface IMarkdownService
    {
        string ToHtml(string? markdown);
        string ToPlainText(string? markdown);
    }
}
