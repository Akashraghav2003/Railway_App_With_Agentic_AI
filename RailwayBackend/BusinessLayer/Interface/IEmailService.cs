using System;
using ModelLayer.Models;

namespace BusinessLayer.Interface;

public interface IEmailService
{
    void SendEmail(EmailModel emailModel);
}
