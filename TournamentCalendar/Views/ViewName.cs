namespace TournamentCalendar.Views;

public static class ViewName
{
    #region *** Shared views ***

    /// <summary>
    /// Shared views
    /// </summary>
    public static class Shared
    {
        /// <summary>
        /// ~/Views/Shared/_Layout.cshtml
        /// </summary>
        public const string _LayoutTournament = "~/Views/Shared/_Layout.cshtml";
    }

    #endregion

    #region *** ContentSynd views ***
    public static class ContentSynd
    {
        /// <summary>
        /// ~/Views/ContentSynd/CalendarListPartial.cshtml
        /// </summary>
        public const string CalendarListPartial = "~/Views/ContentSynd/CalendarListPartial.cshtml";

        /// <summary>
        /// ~/Views/ContentSynd/CalendarListPartialCss.cshtml
        /// </summary>
        public const string CalendarListPartialCss = "~/Views/ContentSynd/CalendarListPartialCss.cshtml";

        /// <summary>
        /// ~/Views/ContentSynd/CalendarListPartialJs.cshtml
        /// </summary>
        public const string CalendarListPartialJs = "~/Views/ContentSynd/CalendarListPartialJs.cshtml";
    }

    #endregion

    #region *** Calendar views ***

    /// <summary>
    /// Tournament calendar - ~/kalender
    /// </summary>
    public static class Calendar
    {
        /// <summary>
        /// ~/Views/Calendar/Overview.cshtml
        /// </summary>
        public const string Overview = "~/Views/Calendar/Overview.cshtml";

        /// <summary>
        /// ~/Views/Calendar/Show.cshtml
        /// </summary>
        public const string Show = "~/Views/Calendar/Show.cshtml";

        /// <summary>
        /// ~/Views/Calendar/Edit.cshtml
        /// </summary>
        public const string Edit = "~/Views/Calendar/Edit.cshtml";

        /// <summary>
        /// ~/Views/Calendar/Confirm.cshtml
        /// </summary>
        public const string Confirm = "~/Views/Calendar/Confirm.cshtml";

        /// <summary>
        /// ~/Views/Calendar/Approve.cshtml
        /// </summary>
        public const string Approve = "~/Views/Calendar/Approve.cshtml";

        /// <summary>
        /// ~/Views/Calendar/_ShowSingleEntry.cshtml
        /// </summary>
        public const string _ShowSingleEntry = "~/Views/Calendar/_ShowSingleEntry.cshtml";

        /// <summary>
        /// ~/Views/Calendar/Integrate.cshtml
        /// </summary>
        public const string Integrate = "~/Views/Calendar/Integrate.cshtml";
    }

    #endregion

    #region *** Collect views ***

    /// <summary>
    /// Tournament import - ~/import
    /// </summary>
    public static class Collect
    {
        /// <summary>
        /// ~/Views/Collect/Show.cshtml
        /// </summary>
        public const string Show = "~/Views/Collect/Show.cshtml";
    }

    #endregion

    #region *** InfoService views ***

    /// <summary>
    /// InfoService registration - ~/volley-news
    /// </summary>
    public static class InfoService
    {
        /// <summary>
        /// ~/Views/InfoService/Edit.cshtml
        /// </summary>
        public const string Edit = "~/Views/InfoService/Edit.cshtml";

        /// <summary>
        /// ~/Views/InfoService/Confirm.cshtml
        /// </summary>
        public const string Confirm = "~/Views/InfoService/Confirm.cshtml";

        /// <summary>
        /// ~/Views/InfoService/Approve.cshtml
        /// </summary>
        public const string Approve = "~/Views/InfoService/Approve.cshtml";

        /// <summary>
        /// ~/Views/InfoService/Unsubscribe.cshtml
        /// </summary>
        public const string Unsubscribe = "~/Views/InfoService/Unsubscribe.cshtml";
    }

    #endregion

    #region *** Organisation views ***

    /// <summary>
    /// Software - ~/software
    /// </summary>
    public static class Organization
    {
        /// <summary>
        /// ~/Views/Software/Evaluation.cshtml
        /// </summary>
        public const string Apps = "~/Views/Organization/Apps.cshtml";
    }

    #endregion

    #region *** Error views ***

    /// <summary>
    /// Error - ~/orga
    /// </summary>
    public static class Error
    {
        /// <summary>
        /// ~/Views/Error/Index.cshtml
        /// </summary>
        public const string Index = "~/Views/Error/Index.cshtml";
    }

    #endregion

    #region *** Info views ***

    /// <summary>
    /// Info - ~/info
    /// </summary>
    public static class Info
    {
        /// <summary>
        /// ~/Views/Info/LegalDetails.cshtml
        /// </summary>
        public const string LegalDetailsTournament = "~/Views/Info/LegalDetails.cshtml";

        /// <summary>
        /// ~/Views/Info/LegalDetails.cshtml
        /// </summary>
        public const string PrivacyPolicy = "~/Views/Info/PrivacyPolicy.cshtml";
    }

    #endregion

    #region *** Contact views ***

    /// <summary>
    /// Contact - ~/contact
    /// </summary>
    public static class Contact
    {
        /// <summary>
        /// ~/Views/Contact/Message.cshtml
        /// </summary>
        public const string Message = "~/Views/Contact/Message.cshtml";

        /// <summary>
        /// ~/Views/Contact/Message.cshtml
        /// </summary>
        public const string Confirm = "~/Views/Contact/Confirm.cshtml";
    }

    #endregion

    #region *** Authorization views ***

    /// <summary>
    /// Contact - ~/contact
    /// </summary>
    public static class Auth
    {
        /// <summary>
        /// ~/Views/Auth/SignIn.cshtml
        /// </summary>
        public const string SignIn = "~/Views/Auth/SignIn.cshtml";

        /// <summary>
        /// ~/Views/Auth/Denied.cshtml
        /// </summary>
        public const string Denied = "~/Views/Auth/Denied.cshtml";
    }

    #endregion

}
