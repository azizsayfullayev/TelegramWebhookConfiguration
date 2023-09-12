﻿using System.Text;
using Telegram.Bot.Types;

namespace FastApiWebhook.Services.UserServices
{
    public interface IUserService
    {
        Task EchoUser(Message message);
    }
}
