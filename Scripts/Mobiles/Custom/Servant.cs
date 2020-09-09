using Server.Items;
using Server.Network;
using Server.Spells;
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
            : base(AIType.AI_Melee, FightMode.Aggressor, 7, -1, 2, 2)
        {
            Body = 400 + Utility.Random(2);
            Hue = Utility.RandomSkinHue();
            this.RawStr = 100;
            this.RawDex = 100;
            this.RawInt = 100;

            Skills[SkillName.DetectHidden].Base = 100;
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

            if (e.Mobile.AccessLevel >= AccessLevel.GameMaster)
            {
                if (e.Speech == "follow")
                {
                    this.Controlled = true;
                    this.SetControlMaster(e.Mobile);
                    this.IsBonded = true; 
                } else if (e.Speech == "fire")
                {
                    if (HasFireRing && Combatant != null && Alive && Hits > 0.8 * HitsMax && m_NextFireRing > Core.TickCount && Utility.RandomDouble() < FireRingChance)
                        FireRing();

                }
            }
        }

        public override bool AllowEquipFrom(Mobile from)
        {
            if (from == this.ControlMaster)
                return true;

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

        public override void OnThink()
        {
            base.OnThink();


            if (HasFireRing && Combatant != null && Alive && Hits > 0.8 * HitsMax && m_NextFireRing > Core.TickCount && Utility.RandomDouble() < FireRingChance)
                FireRing();


            Mobile combatant = Combatant as Mobile;

            if (combatant != null)
            {
                if (CanTakeLife(combatant))
                    TakeLife(combatant);
            }
        }


        #region Take Life
        private DateTime m_NextTakeLife;

        public bool CanTakeLife(Mobile from)
        {
            if (m_NextTakeLife > DateTime.UtcNow)
                return false;

            if (!CanBeHarmful(from))
                return false;

            if (Hits > 0.1 * HitsMax || Hits < 0.025 * HitsMax)
                return false;

            return true;
        }

        public void TakeLife(Mobile from)
        {
            Hits += from.Hits / (from.Player ? 2 : 6);

            FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
            PlaySound(0x1F2);

            Say(1075117);  // Muahahaha!  Your life essence is MINE!
            Say(1075120); // An unholy aura surrounds Lady Melisande as her wounds begin to close.

            m_NextTakeLife = DateTime.UtcNow + TimeSpan.FromSeconds(15 + Utility.RandomDouble() * 45);
        }

        #endregion

        #region Fire Ring
        private static readonly int[] m_North = new int[]
        {
            -1, -1,
            1, -1,
            -1, 2,
            1, 2
        };

        private static readonly int[] m_East = new int[]
        {
            -1, 0,
            2, 0
        };

        public virtual bool HasFireRing => false;
        public virtual double FireRingChance => 1.0;

        private long m_NextFireRing = Core.TickCount;

        public virtual void FireRing()
        {
            for (int i = 0; i < m_North.Length; i += 2)
            {
                Point3D p = Location;

                p.X += m_North[i];
                p.Y += m_North[i + 1];

                IPoint3D po = p as IPoint3D;

                SpellHelper.GetSurfaceTop(ref po);

                Effects.SendLocationEffect(po, Map, 0x3E27, 50);
            }

            for (int i = 0; i < m_East.Length; i += 2)
            {
                Point3D p = Location;

                p.X += m_East[i];
                p.Y += m_East[i + 1];

                IPoint3D po = p as IPoint3D;

                SpellHelper.GetSurfaceTop(ref po);

                Effects.SendLocationEffect(po, Map, 0x3E31, 50);
            }

            m_NextFireRing = Core.TickCount + 10000;
        }
        #endregion
    }
}
