using Server.Mobiles;

namespace Server.Items.Custom
{
    public class BornStone : Item
    {
        [Constructable]
        public BornStone()
            : base(0xED4)
        {
            Movable = false;
            Hue = 0x56;
        }

        public BornStone(Serial serial)
            : base(serial)
        {
        }
        public override string DefaultName => "born stone";

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile)
            {
                // 技能全满
                for (int i = 0; i <= (int)SkillName.Throwing; i++)
                {
                    from.Skills[i].Base = 200;
                }

                from.RawStr = 200;
                from.RawDex = 200;
                from.RawInt = 200;


                // 道具
                {
                    Robe robe = new Robe();
                    robe.Attributes.CastRecovery = 100;
                    robe.Attributes.CastSpeed = 100;
                    from.AddToBackpack(robe);
                }
            }
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
    }
}
