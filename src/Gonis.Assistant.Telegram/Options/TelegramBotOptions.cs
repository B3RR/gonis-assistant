using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gonis.Assistant.Telegram.Options
{
    public class TelegramBotOptions
    {
        public string Name { get; set; }

        public string Token { get; set; }
        public string ErrorsChatId { get; set; }
    }
}
