using Microsoft.Extensions.Logging;
using System.Xml.Xsl;
using System.Xml;
using System.Xml.Linq;

namespace Lab2;
using models;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
          .UseMauiApp<App>()
          .ConfigureFonts(fonts =>
          {
              fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
              fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
          });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

public static class GlobalSettings
{
    public const string XmlFilePath = (@"C:\Users\User\source\repos\Lab2\courses.xml");

}

