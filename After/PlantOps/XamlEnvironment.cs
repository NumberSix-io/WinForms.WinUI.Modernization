using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.XamlTypeInfo;

namespace PlantOps;

// A WinUI Application supplies metadata and resources, but never owns a window or
// starts a second message loop. WinForms remains the application host.
internal sealed class XamlEnvironment : Microsoft.UI.Xaml.Application, IXamlMetadataProvider
{
    private readonly IXamlMetadataProvider[] _providers =
    [new XamlControlsXamlMetaDataProvider(), new WinUI.PlantOps_WinUI_XamlTypeInfo.XamlMetaDataProvider()];

    public void InitializeResources()
    {
        RequestedTheme = ApplicationTheme.Light;
        Resources.MergedDictionaries.Add(new XamlControlsResources());
    }

    public IXamlType GetXamlType(Type type) => _providers.Select(p => p.GetXamlType(type)).FirstOrDefault(t => t is not null)!;
    public IXamlType GetXamlType(string name) => _providers.Select(p => p.GetXamlType(name)).FirstOrDefault(t => t is not null)!;
    public XmlnsDefinition[] GetXmlnsDefinitions() => _providers.SelectMany(p => p.GetXmlnsDefinitions()).ToArray();
}
