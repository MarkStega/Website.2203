namespace Website.Lib;

using System;
using System.Threading.Tasks;

public interface ITeamsNotificationService
{
    Task SendNotification(IMessage message);
}
