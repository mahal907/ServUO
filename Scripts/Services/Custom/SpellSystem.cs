using Server.Commands;
using Server.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services.Custom
{
    public class SpellSystem
    {
        public static void Initialize()
        {
            CommandSystem.Register("Fuck", AccessLevel.Player, e =>
            {
                Console.WriteLine("[+] " + e.ArgString);

                if (!int.TryParse(e.ArgString, out int spellID))
                {
                    e.Mobile.SendMessage("[-] wrong magic spell id.");
                    return;
                }

                Spell spell = SpellRegistry.NewSpell(spellID, e.Mobile, null);

                if (spell != null)
                {
                    if (!spell.Cast())
                    {
                        e.Mobile.SendMessage("[-] 启动法术{0}失败.", spell.Name);
                        return;
                    }
                }
            });
        }
    }
}
