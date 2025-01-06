namespace StudentTaskManagement.ViewModels
{
    public class _BaseViewModel
    {
        public string PageTitle { get; set; }

        public string ReturnUrl { get; set; }

        public string ActiveMenu { get; set; }

        //For Audit Log Usage
        public int? AuditReferenceId { get; set; }

    }
}
