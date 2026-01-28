using ReflektJournal.Data;

namespace ReflektJournal;

public partial class App : Application
{
    private readonly ReflektDatabase _db;

    public App(ReflektDatabase db)
    {
        InitializeComponent();
        _db = db;

        // Initialize database asynchronously
        Task.Run(async () => await _db.InitializeAsync());
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new MainPage())
        {
            Title = "ReflektJournal"
        };
    }
}