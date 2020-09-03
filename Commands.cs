using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using System.Runtime.InteropServices;
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
                    //TODO: handle these in a config file?
                    if (item != "member" && item != "tournaments" && item != "multi")
                    {
                        Bot.Log($"{ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} tried to get role {item} without permission", LogLevel: Bot.LogLevel.Warning);
                        return;
                    }

                    //Tries to get specified role
                    for (int i = 0; i < ctx.Guild.Roles.Count; i++)
                    {
                        if (ctx.Guild.Roles[i].Name.ToLower() == item)
                        {
                            //TODO: specify in config file role/channel relationships
                            if ((item == "osu!" && ctx.Channel.Name == "welcome") ||
                                (item == "tournaments" || item == "multi") && ctx.Channel.Name == "role-assignment")
                            {
                                Bot.Log($"Granting role {item} to {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator}");
                                await ctx.Guild.GrantRoleAsync(ctx.Member, ctx.Guild.Roles[i]);
                                await ctx.Message.DeleteAsync();
                            }
                            else
                            {
                                Bot.Log($"{ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} tried to get role {item} in {ctx.Message.Channel.Name}", Bot.LogLevel.Warning);
                            }
                        }
                    }

                    break;

                case "stream":
                case "twitch":

                    if (ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.Administrator) &&
                        ctx.Channel.Name == "twitch")
                    {
                        //if (item != string.Empty)
                        //{
                        //    if (ConfigManager.SetNewStreamMonitor(item, ctx.Guild.Id, ctx.Channel.Id))
                        //    {
                        //        await ctx.Message.DeleteAsync();
                        //    }
                        //    else
                        //    {
                        //        await ctx.Channel.SendMessageAsync($"Impossibile aggiungere lo stream {item} (forse è già stato aggiunto?)");
                        //    }
                        //}
                    }

                    break;
                default:
                    break;
            }
        }

        [Command("remove"), Description("Removes user from specified role.")]
        public async Task RemoveRole(CommandContext ctx, [Description("What to remove")] string type, [Description("Item to remove")] string item)
        {
            switch (type)
            {
                case "role":

                    item = item.ToLower();

                    if (ctx.Member.Roles.Where(r => r.Name.ToLower() == item).Count() > 0)
                    {
                        Bot.Log($"Found and removing {item} role from {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator}");
                        await ctx.Member.RevokeRoleAsync(ctx.Member.Roles.Where(r => r.Name.ToLower() == item).First());
                    }
                    else
                    {
                        Bot.Log($"No role {item} belonging to {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator}", LogLevel: Bot.LogLevel.Warning);
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
