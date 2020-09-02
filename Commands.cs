using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace Splash
{
    public class Commands
    {
        [Command("add"), Description("Adds user to specified role.")]
        public async Task AddToRole(CommandContext ctx, [Description("What to add")] string type, [Description("Item to add")] string item)
        {
            switch (type)
            {
                case "role":
                    item = item.ToLower();

                    //Allowed roles
                    //TODO: handle these in a config file
                    if (item != "osu!" && item != "tournament" && item != "multi")
                    {
                        await ctx.Channel.SendMessageAsync($"Il ruolo {item} non ti può essere assegnato");
                        return;
                    }

                    //Tries to get specified role
                    //TODO: check if user already has the specified role. In that case, throw an error
                    for (int i = 0; i < ctx.Guild.Roles.Count; i++)
                    {
                        if (ctx.Guild.Roles[i].Name == item)
                        {
                            //TODO: specify in config file role/channel relationships
                            if ((item == "osu!" && ctx.Channel.Name == "welcome") ||
                                (item == "tournament" || item == "multi") && ctx.Channel.Name == "role-assignment")
                            {
                                await ctx.Guild.GrantRoleAsync(ctx.Member, ctx.Guild.Roles[i]);

                                //Deletes user's message upon completion
                                await ctx.Message.DeleteAsync($"Role {item} assigned.");
                            }
                            else
                            {
                                //TODO: output which role can be assigned in which channel
                            }
                        }
                    }
                    break;

                case "stream":
                case "twitch":

                    if (ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.Administrator) &&
                        ctx.Channel.Name == "twitch")
                    {
                        if (item != string.Empty)
                        {
                            if (ConfigManager.SetNewStreamMonitor(item, ctx.Guild.Id, ctx.Channel.Id))
                            {
                                await ctx.Message.DeleteAsync();
                            }
                            else
                            {
                                await ctx.Channel.SendMessageAsync($"Impossibile aggiungere lo stream {item} (forse è già stato aggiunto?)");
                            }                            
                        }
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
