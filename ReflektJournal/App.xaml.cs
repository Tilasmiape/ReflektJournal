using ReflektJournal.Data;

namespace ReflektJournal;

public partial class App : Application
{
    public App(ReflektDatabase db)
    {
        InitializeComponent();
        _ = db.InitializeAsync();
        MainPage = new MainPage();
    }
}