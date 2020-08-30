using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Splash
{
    public class Commands
    {
        [Command("add"), Description("Adds user to specified role.")]
        public async Task AddToRole(CommandContext ctx, [Description("What to add")] string type, [Description("Role the user wants to be added to")] string role)
        {
            switch (type)
            {
                case "role":
                    role = role.ToLower();

                    //Allowed roles
                    //TODO: handle these in a config file
                    if (role != "osu!" && role != "tournament" && role != "multi")
                    {
                        await ctx.Channel.SendMessageAsync($"Il ruolo {role} non ti può essere assegnato");
                        return;
                    }

                    //Tries to get specified role
                    //TODO: check if user already has the specified role. In that case, throw an error
                    for (int i = 0; i < ctx.Guild.Roles.Count; i++)
                    {
                        if (ctx.Guild.Roles[i].Name == role)
                        {
                            //TODO: specify in config file role/channel relationships
                            if ((role == "osu!" && ctx.Channel.Name == "welcome") ||
                                (role == "tournament" || role == "multi") && ctx.Channel.Name == "role-assignment")
                            {
                                await ctx.Guild.GrantRoleAsync(ctx.Member, ctx.Guild.Roles[i]);

                                //Deletes user's message upon completion
                                await ctx.Message.DeleteAsync($"Role {role} assigned.");
                            }
                            else
                            {
                                //TODO: output which role can be assigned in which channel
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
