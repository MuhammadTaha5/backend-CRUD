namespace StudentManagement.Helper.Email
{
    public static class EmailTemplates
    {
        /// <summary>
        /// provides a proper link to display in body of email using username, resetlink
        /// </summary>
        /// <param name="fullName">username whom to send email</param>
        /// <param name="resetLink">link of url with userid and token appended</param>
        /// <returns></returns>
        public static string PasswordReset(string fullName, string resetLink) => $"""
        <p>Hi {fullName},</p>
        <p>Click below to reset your password:</p>
        <a href='{resetLink}'>Reset Password</a>
        """;
        /// <summary>
        /// Activate account 
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="confirmLink"></param>
        /// <returns></returns>
        public static string ActivateAccount(string fullName, string confirmLink) => $"""
        <p>Hi {fullName},</p><p>Click below to confirm your email and activate your account:</p><a href='{confirmLink}'>Confirm Account</a>"
        """;
        
    }
}