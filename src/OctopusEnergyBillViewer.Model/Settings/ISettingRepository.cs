namespace OctopusEnergyBillViewer.Model.Settings;

public interface ISettingRepository
{
    Task<Setting> LoadAsync();

    Task SaveAsync(Setting setting);
}
