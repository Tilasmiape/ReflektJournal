using ReflektJournal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflektJournal.Services.Interfaces
{
    public interface IToastService
    {
        event Action<ToastMessage>? OnShow;

        void Success(string message, int durationMs = 3200);
        void Error(string message, int durationMs = 3600);
        void Info(string message, int durationMs = 3200);
    }
}
