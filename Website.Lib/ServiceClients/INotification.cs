namespace Website.Lib;

using System.Threading.Tasks;

/// <summary>
/// A service that notifies the website owner of messages sent by website users.
/// </summary>
public interface INotification
{
    /// <summary>
    /// Sends a "contact us" message.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task Send(ContactMessage message);


    /// <summary>
    /// Sends a recruitment enquiry.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task Send(RecruitmentEnquiry message);


    /// <summary>
    /// Sends an enquiry from a client/real estate investor.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task Send(RealEstateInvestorEnquiry message);


    /// <summary>
    /// Sends an enquiry from a VC investor.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task Send(VentureCapitalEnquiry message);
}
