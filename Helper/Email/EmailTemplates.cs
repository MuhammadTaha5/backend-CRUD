namespace StudentManagement.Helper.Email
{
    public static class EmailTemplates
    {
        public static string PasswordReset(string fullName, string resetLink) => $"""
        <p>Hi {fullName},</p>
        <p>Click below to reset your password:</p>
        <a href='{resetLink}'>Reset Password</a>
        """;
        public static string ActivateAccount(string fullName, string confirmLink) => $"""
        <p>Hi {fullName},</p><p>Click below to set your password and activate your account:</p><a href='{confirmLink}'>Confirm Account</a>"
        """;
        
    }
}