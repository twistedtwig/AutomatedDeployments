
namespace IISBackup.ViewModels
{
    public enum IisApplicationType
    {
        WebSite,
        Application
    }

    public class IisApplicationViewModel
    {
        public IisApplicationType ApplicationType { get; set; }
        public string Name { get; set; }

    }
}
