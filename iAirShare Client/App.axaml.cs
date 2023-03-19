using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Skia;
using iAirShare_Client.ViewModels;
using iAirShare_Client.Views;
using SkiaSharp;

namespace iAirShare_Client;

public class CustomFontManagerImpl : IFontManagerImpl
{
    private readonly string[] _bcp47 =
        { CultureInfo.CurrentCulture.ThreeLetterISOLanguageName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName };

    private readonly Typeface[] _customTypefaces;
    private readonly string _defaultFamilyName;

    //Load font resources in the project, you can load multiple font resources
    private readonly Typeface _defaultTypeface = new("Microsoft Yahei");

    public CustomFontManagerImpl()
    {
        _customTypefaces = new[] { _defaultTypeface };
        _defaultFamilyName = _defaultTypeface.FontFamily.FamilyNames.PrimaryFamilyName;
    }

    public string GetDefaultFontFamilyName()
    {
        return _defaultFamilyName;
    }

    public IEnumerable<string> GetInstalledFontFamilyNames(bool checkForUpdates = false)
    {
        return _customTypefaces.Select(x => x.FontFamily.Name);
    }

    public bool TryMatchCharacter(int codepoint, FontStyle fontStyle, FontWeight fontWeight, FontFamily fontFamily,
        CultureInfo culture, out Typeface typeface)
    {
        foreach (var customTypeface in _customTypefaces)
        {
            if (customTypeface.GlyphTypeface.GetGlyph((uint)codepoint) == 0) continue;

            typeface = new Typeface(customTypeface.FontFamily.Name, fontStyle, fontWeight);

            return true;
        }

        var fallback = SKFontManager.Default.MatchCharacter(fontFamily.Name, (SKFontStyleWeight)fontWeight,
            SKFontStyleWidth.Normal, (SKFontStyleSlant)fontStyle, _bcp47, codepoint);

        typeface = new Typeface(fallback?.FamilyName ?? _defaultFamilyName, fontStyle, fontWeight);

        return true;
    }

    public IGlyphTypefaceImpl CreateGlyphTypeface(Typeface typeface)
    {
        SKTypeface skTypeface;

        switch (typeface.FontFamily.Name)
        {
            case FontFamily.DefaultFontFamilyName:
                skTypeface = SKTypeface.FromFamilyName(_defaultTypeface.FontFamily.Name);
                break;
            default:
                skTypeface = SKTypeface.FromFamilyName(typeface.FontFamily.Name,
                    (SKFontStyleWeight)typeface.Weight, SKFontStyleWidth.Normal, (SKFontStyleSlant)typeface.Style);
                break;
        }

        return new GlyphTypefaceImpl(skTypeface);
    }
}

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaLocator.CurrentMutable.Bind<IFontManagerImpl>()
            .ToConstant(new CustomFontManagerImpl());
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };

        base.OnFrameworkInitializationCompleted();
    }
}