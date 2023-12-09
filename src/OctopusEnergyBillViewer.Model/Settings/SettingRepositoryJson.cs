using System.Text.Json;

namespace OctopusEnergyBillViewer.Model.Settings;

public class SettingRepositoryJson : ISettingRepository
{
    public static string SettingJsonPath => Path.Combine(Paths.DataDir, "setting.json");

    private readonly JsonSerializerOptions options_ = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public async Task<Setting> LoadAsync()
    {
        if (File.Exists(SettingJsonPath))
        {
            using var fileStream = new FileStream(SettingJsonPath, FileMode.Open, FileAccess.Read);
            var data = await JsonSerializer.DeserializeAsync<SettingData>(fileStream, options_);
            if (data is not null)
            {
                return data.ToModel();
            }
        }
        var initSetting = new Setting(new(""), new(""), new(""), new(""), new(""));
        await SaveAsync(initSetting);
        return initSetting;
    }

    public async Task SaveAsync(Setting setting)
    {
        using var fileStram = new FileStream(SettingJsonPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        await JsonSerializer.SerializeAsync(fileStram, SettingData.FromModel(setting), options_);
    }
}
