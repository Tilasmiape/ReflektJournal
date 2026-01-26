using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflektJournal.Models
{
    public sealed record ToastMessage(
    ToastType Type,
    string Text,
    int DurationMs = 3200);
}
