
namespace IISBackup.ViewModels
{
    public class ApplicationOverVewViewModel : ViewModelBase
    {
        public string Path { get; set; }       
        public string AppPoolName { get; set; }
        public string EnabledProtocols { get; set; }
        public string ParentWebsite { get; set; }

        public bool IsSelected { get; set; }
        
        protected override void RefreshPageData() { }
    }
}
