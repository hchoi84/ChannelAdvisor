using ChannelAdvisor.Utilities;

namespace ChannelAdvisor.Models
{
  public interface IEmail
  {
    void EmailToken(GolfioUser golfioUser, string tokenLink, EmailType emailType);
  }
}