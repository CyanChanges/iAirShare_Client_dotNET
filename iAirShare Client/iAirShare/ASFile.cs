namespace iAirShare_Client.iAirShare;

public enum ASFileType
{
    Null = 0x000,
    file = 0x00a,
    directory = 0x00b
}

public struct ASFile
{
    public class ASFileProperty
    {
        public string file_id;
        public string? mimetype { get; set; }
        public ulong? file_size { get; set; }
        public float? last_update { get; set; }
    }

    public string file_name { get; set; }
    public ASFileType file_type { get; set; }
    public ASFileProperty file_property { get; set; }
    public string? pointer;
}