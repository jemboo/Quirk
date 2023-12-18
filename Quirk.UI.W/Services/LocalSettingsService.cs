using Microsoft.Extensions.Options;

using Quirk.UI.W.Contracts.Services;
using Quirk.UI.W.Core.Contracts.Services;
using Quirk.UI.W.Core.Helpers;
using Quirk.UI.W.Helpers;
using Quirk.UI.W.Models;

using Windows.ApplicationModel;
using Windows.Storage;

namespace Quirk.UI.W.Services;

public class LocalSettingsService : ILocalSettingsService
{
    private const string _defaultApplicationDataFolder = "Quirk.UI.W/ApplicationData";
    private const string _defaultLocalSettingsFile = "LocalSettings.json";

    //private readonly IFileService _fileService;
    private readonly Quirk.Core.IFileUtils _fileService;
    private readonly LocalSettingsOptions _options;

    private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string _applicationDataFolder;
    private readonly string _localsettingsFile;

    private IDictionary<string, object> _settings;

    private bool _isInitialized;

    public LocalSettingsService(Quirk.Core.IFileUtils fileService, IOptions<LocalSettingsOptions> options)
    {
        _fileService = fileService;
        _options = options.Value;

        _applicationDataFolder = Path.Combine(_localApplicationData, _options.ApplicationDataFolder ?? _defaultApplicationDataFolder);
        _localsettingsFile = _options.LocalSettingsFile ?? _defaultLocalSettingsFile;

        _settings = new Dictionary<string, object>();
    }

    private async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            _settings = await Task.Run(
                () =>
                {
                    var res = _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, _localsettingsFile);
                    if (res.IsOk)
                    {
                        return res.ResultValue;
                    }
                    else
                    {
                        return new Dictionary<string, object>();
                    }
                }
             );

            _isInitialized = true;
        }
    }

    public async Task<T?> ReadSettingAsync<T>(string key)
    {
        if (RuntimeHelper.IsMSIX)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }
        }
        else
        {
            await InitializeAsync();

            if (_settings != null && _settings.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }
        }

        return default;
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Json.StringifyAsync(value);
        }
        else
        {
            await InitializeAsync();

            _settings[key] = await Json.StringifyAsync(value);

            await Task.Run(() => _fileService.Save(_applicationDataFolder, _localsettingsFile, _settings));
        }
    }
}
