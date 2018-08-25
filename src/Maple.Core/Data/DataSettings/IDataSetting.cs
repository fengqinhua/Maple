using System.Collections.Generic;

namespace Maple.Core.Data.DataSettings
{
    public interface IDataSetting
    {
        string DataConnectionString { get; }
        DataSouceType DataSouceType { get; }
        string Name { get; }
        IReadOnlyDictionary<string, string> RawDataSettings { get; }

        bool IsValid();
    }
}