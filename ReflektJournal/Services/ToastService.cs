using ReflektJournal.Models;
using ReflektJournal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflektJournal.Services
{
    public sealed class ToastService : IToastService
    {
        public event Action<ToastMessage>? OnShow;

        public void Success(string message, int durationMs = 3200)
            => OnShow?.Invoke(new ToastMessage(ToastType.Success, message, durationMs));

        public void Error(string message, int durationMs = 3600)
            => OnShow?.Invoke(new ToastMessage(ToastType.Error, message, durationMs));

        public void Info(string message, int durationMs = 3200)
            => OnShow?.Invoke(new ToastMessage(ToastType.Info, message, durationMs));
    }
}
