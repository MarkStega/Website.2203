namespace Website.Lib;

using System.Threading.Tasks;

public interface ITeamsNotificationService
{
    Task SendNotification(ContactMessage message);
    Task SendNotification(RecruitmentEnquiry message);
    Task SendNotification(RealEstateInvestorEnquiry message);
    Task SendNotification(VentureCapitalEnquiry message);
}
