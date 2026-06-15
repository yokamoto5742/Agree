using System.Net;
using System.Net.Mail;

namespace AgentlabUtilityLibrary;

public class SmtpClientShinseikai : SmtpClient
{
	private string host = "h180-147-243-159.vps.ablenet.jp";

	public SmtpClientShinseikai()
	{
		base.Host = host;
		base.Credentials = new NetworkCredential("shinryou", "q,A|)!jBnZ");
		base.Port = 587;
	}

	public SmtpClientShinseikai(string user, string passwd)
	{
		base.Host = host;
		base.Credentials = new NetworkCredential(user, passwd);
		base.Port = 587;
	}

	public SmtpClientShinseikai(string user, string passwd, int port)
	{
		base.Host = host;
		base.Credentials = new NetworkCredential(user, passwd);
		base.Port = port;
	}
}
