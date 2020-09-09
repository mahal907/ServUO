using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Mysticism;
using Server.Spells.Spellweaving;
using Server.Targeting;
using System;

namespace Server.Mobiles
{
    /// <summary>
    /// 仆从 [add Servant
    /// This is a test creature
    /// You can set its value in game
    /// It die after 5 minutes, so your test server stay clean
    /// Create a macro to help your creation "[add Dummy 1 15 7 -1 0.5 2"
    /// 
    /// A iTeam of negative will set a faction at random
    /// 
    /// Say Kill if you want them to die
    /// 
    /// </summary>
    public class Servant : BaseCreature
    {
        [Constructable]
        public Servant()
            : base(AIType.AI_Melee, FightMode.Aggressor, 7, 7, 9, 9)
        {
            Body = 400 + Utility.Random(2);
            Hue = Utility.RandomSkinHue();
            this.RawStr = 200;
            this.RawDex = 200;
            this.RawInt = 200;

            Skills[SkillName.DetectHidden].Base = 100;
            Skills[SkillName.Swords].Base = 120;
            Skills[SkillName.Wrestling].Base = 120;
            Skills[SkillName.Anatomy].Base = 120;
            Skills[SkillName.Tactics].Base = 120;
            Skills[SkillName.Magery].Base = 120;
            Skills[SkillName.Meditation].Base = 120;
            Skills[SkillName.Imbuing].Base = 120;
            Skills[SkillName.Healing].Base = 120;
            Skills[SkillName.MagicResist].Base = 120;

            Team = Utility.Random(3);
            int iHue = 20 + Team * 40;
            int jHue = 25 + Team * 40;
            Utility.AssignRandomHair(this, iHue);


            Container pack = new Backpack();
            pack.Movable = false;
            AddItem(pack);


            LeatherGloves glv = new LeatherGloves();
            glv.Hue = iHue;
            //glv.LootType = LootType.Newbied;
            AddItem(glv);

            Item shroud = new HoodedShroudOfShadows();
            //shroud.LootType = LootType.Newbied;
            AddItem(shroud);

            Scimitar weapon = new Scimitar();

            weapon.Skill = SkillName.Wrestling;
            weapon.Hue = 38;
            //weapon.LootType = LootType.Newbied;
            AddItem(weapon);
            
            AddItem(new VirtualMountItem(this));


            // special functions
            {
            }
        }

        public Servant(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            if (e.Mobile == this.ControlMaster || e.Mobile.AccessLevel >= AccessLevel.GameMaster)
            {
                switch (e.Speech)
                {
                    case "bond":
                        this.Controlled = true;
                        this.SetControlMaster(e.Mobile);
                        this.IsBonded = true; 
                        break;
                    case "fire":
                        if (!new Spells.Seventh.FlameStrikeSpell(this, null).Cast())
                        {
                            Console.WriteLine("[-] 无法释放地狱火术");
                        }
                        break;
                    case "heal":
                        if (!new GreaterHealSpell(this, null).Cast())
                        {
                            Console.WriteLine("[-] 无法释放大治疗术");
                        }

                        break;

                }
            }
        }


        #region Pack and Equip
        public override bool AllowEquipFrom(Mobile from)
        {
            if (CheckAccess(this, from))
            {
                return true;
            }

            return base.AllowEquipFrom(from);
        }


        public bool CheckAccess(BaseCreature animal, Mobile from)
        {
            if (from == animal || from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (from.Alive && animal.Controlled && (from == animal.ControlMaster || from == animal.SummonMaster || animal.IsPetFriend(from)))
                return true;

            return false;
        }


        public override bool IsSnoop(Mobile from)
        {
            if (CheckAccess(this, from))
                return false;

            return base.IsSnoop(from);
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            Console.WriteLine("[+] OnDragDrop");

            if (CheckAccess(this, from))
            {
                AddToBackpack(item);
                return true;
            }

            return base.OnDragDrop(from, item);
        }

        public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
        {
            Console.WriteLine("[+] CheckNonLocalDrop");
            return CheckAccess(this, from);
        }

        public override bool CheckNonlocalLift(Mobile from, Item item)
        {
            Console.WriteLine("[+] CheckNonLocalLift");
            return CheckAccess(this, from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            Console.WriteLine("[+] OnDoubleClick");
            PackAnimal.TryPackOpen(this, from);
        }
#endregion

        private class VirtualMount : IMount
        {
            private readonly VirtualMountItem m_Item;
            public VirtualMount(VirtualMountItem item)
            {
                m_Item = item;
            }

            public Mobile Rider
            {
                get
                {
                    return m_Item.Rider;
                }
                set
                {
                }
            }
            public virtual void OnRiderDamaged(Mobile from, ref int amount, bool willKill)
            {
            }
        }


        private class VirtualMountItem : Item, IMountItem
        {
            private readonly VirtualMount m_Mount;
            private Mobile m_Rider;
            public VirtualMountItem(Mobile mob)
                : base(0x3EBB)
            {
                Layer = Layer.Mount;

                Movable = false;

                m_Rider = mob;
                m_Mount = new VirtualMount(this);
            }

            public VirtualMountItem(Serial serial)
                : base(serial)
            {
                m_Mount = new VirtualMount(this);
            }

            public Mobile Rider => m_Rider;
            public IMount Mount => m_Mount;
            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write(0); // version

                writer.Write(m_Rider);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                m_Rider = reader.ReadMobile();

                if (m_Rider == null)
                    Delete();
            }
        }

        public bool ProcessTarget()
        {
            Target targ = this.Target;

            if (targ == null)
                return false;

            bool harmful = IsHarmful(targ);
            bool beneficial = IsBeneficial(targ);

            if (harmful)
            {
                if (Combatant != null)
                {
                    targ.Invoke(this, Combatant);
                    return true;
                }
                else
                {
                    targ.Invoke(this, this);
                }
            }

            if (beneficial)
            {
                targ.Invoke(this, this);
                return true;
            }

            return false;
        }

        public virtual bool IsBeneficial(Target targ)
        {
            return (targ.Flags & TargetFlags.Beneficial) != 0 || targ is ArchCureSpell.InternalTarget;
        }

        public virtual bool IsHarmful(Target targ)
        {
            return (targ.Flags & TargetFlags.Harmful) != 0 || targ is HailStormSpell.InternalTarget ||
                          targ is WildfireSpell.InternalTarget;
        }

        public override void OnThink()
        {
            if (this.ProcessTarget())
            {
                return;
            }

            base.OnThink();

            if (Alive)
            {
                // try to heal self.
                if (Hits < 0.5 * HitsMax)
                {

                }
            }
        }
    }
}
