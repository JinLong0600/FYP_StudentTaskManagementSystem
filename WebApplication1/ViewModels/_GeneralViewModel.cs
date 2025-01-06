namespace StudentTaskManagement.ViewModels
{
    public class _GeneralViewModel
    {
        public string Password { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string HashedPassword { get; set; }

        public bool IsPasswordSameHased { get; set; }
    }
}
