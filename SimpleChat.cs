using Qurre;
using Qurre.API.Events;
using Qurre.Events;
using System;

namespace SimpleChat
{
    public class SimpleChat : Plugin
    {
        public override string Developer => "ohayo!#5601";
        public override string Name => "SimpleChat";
        public override Version Version => new Version(1, 0, 1);
        public override int Priority => int.MinValue;
        public void registerCfgs()
        {
            bool simplechat_enable = Config.GetBool("simplechat_enable", true, "on or off SimpleChat?");
            int simplechat_message_time = Config.GetInt("simplechat_message_time", 5, "the time when the message is displayed on the players screen");
            int simplechat_min_symbols = Config.GetInt("simplechat_min_symbols", 1, "minimum number of characters in a message");
            int simplechat_max_symbols = Config.GetInt("simplechat_max_symbols", 50, "maximum number of characters in a message");
            String simplechat_command = Config.GetString("simplechat_command", "say", "command to send a message to the chat (you can write any command, but keep in mind that it will always start with [.])");
            String simplechat_hello_bc = Config.GetString("simplechat_hello_bc", "You can send messages to a text chat by writing\n[.say <Message>] in the console\n<color=#FF60A9>Have a nice chat!</color>", "welcome message of the plugin about at the beginning of the round");
            int simplechat_hello_bc_time = Config.GetInt("simplechat_hello_bc_time", 15, "time in seconds for simplechat_hello_bc");
            bool simplechat_hello_bc_enable = Config.GetBool("simplechat_hello_bc_enable", true, "on or off SimpleChat hello bc?");
            String simplechat_you_rip_error = Config.GetString("simplechat_you_rip_error", "You cannot use the chat while you are in the spectators", "a message if a person sends a message being a spectator");
            String simplechat_success_send = Config.GetString("simplechat_success_send", "The message was sent successfully!", "the message sent to the console when the message is successfully sent");
            String simplechat_null_args_error = Config.GetString("simplechat_null_args_error", "You haven't written anything", "error with an empty message");
            String simplechat_minmax_error = Config.GetString("simplechat_minmax_error", "Your message is more or less than the allowed value", "an error in which the length of the sent message is greater or less than the allowed value");
        }
        public override void Enable()
        {
            registerCfgs();

            if (Config.GetBool("simplechat_enable", true) == false)
            {
                Log.Error(" > the SimpleChat is disabled because you disabled it in the config");
                return;
            }

            Server.SendingConsole += onConsoleSend;
            Round.Start += OnRoundStart;

            Log.Info(" SimpleChat enabled :)");
            Log.Info(" version: 1.0.1");
            Log.Info(" dev: ohayo!#5601");
            Log.Info(" site: www.rootkovskiy.ovh");
        }
        public override void Disable()
        {
            Server.SendingConsole -= onConsoleSend;
            Round.Start -= OnRoundStart;

            Log.Info(" SimpleChat disabled :(");
            Log.Info(" version: 1.0.1");
            Log.Info(" dev: ohayo!#5601");
            Log.Info(" site: www.rootkovskiy.ovh");
        }
        public void OnRoundStart()
        {
            if(Config.GetBool("simplechat_hello_bc_enable", true) == true)
            {
                foreach (Qurre.API.Player players in Qurre.API.Player.List)
                {
                    players.Broadcast(Config.GetString("simplechat_hello_bc", "You can send messages to a text chat by writing\n[.say <Message>] in the console\n<color=#FF60A9>Have a nice chat!</color>"), (ushort)Config.GetInt("simplechat_hello_bc_time", 15));
                }
            }
        }
        public static void onConsoleSend(SendingConsoleEvent ev)
        {
            if (ev.Name == Config.GetString("simplechat_command", "say"))
            {
                ev.Allowed = false;
                if (ev.Player.Team != Team.RIP)
                {
                    if(ev.Args.Length == 0)
                    {
                        ev.Player.SendConsoleMessage(Config.GetString("simplechat_null_args_error", "You haven't written anything"), "red");
                        return;
                    }
                    if (!(ev.Args.Length >= Config.GetInt("simplechat_min_symbols", 1) || ev.Args.Length <= Config.GetInt("simplechat_max_symbols", 50)))
                    {
                        ev.Player.SendConsoleMessage(Config.GetString("simplechat_minmax_error", "Your message is more or less than the allowed value"), "red");
                        return;
                    }
                    var message = string.Join(" ", ev.Args);
                    foreach (Qurre.API.Player players in Qurre.API.Player.List)
                    {
                        if(players.Team != Team.RIP)
                        {
                            players.ShowHint($"<align=\"left\"><pos=10%><color=#FF60A9>{ev.Player.Nickname}</color>: {message}", Config.GetInt("simplechat_message_time", 5));
                        }
                        players.SendConsoleMessage($"<color=#FF60A9>{ev.Player.Nickname}</color>: {message}", "white");
                    }
                    ev.Player.SendConsoleMessage(Config.GetString("simplechat_success_send", "The message was sent successfully!"), "green");
                } else
                {
                    ev.Player.SendConsoleMessage(Config.GetString("simplechat_you_rip_error", "You cannot use the chat while you are in the spectators"), "red");
                }
            }
        }
    }
}
