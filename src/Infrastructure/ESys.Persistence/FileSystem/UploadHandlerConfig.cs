namespace ESys.Persistence.FileSystem;

public class UploadHandlerConfig
{
    public int MaxSizeInMB { get; set; } = 10;
    public List<string> AcceptedExtensions { get; set; } = [];
    public string UploadRootDirectory { get; set; } = "ClientUploads";
}