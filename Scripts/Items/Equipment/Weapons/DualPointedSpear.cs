namespace Server.Items
{
    [FlipableAttribute(0x904, 0x406D)]
    public class DualPointedSpear : BaseSpear
    {
        [Constructable]
        public DualPointedSpear()
            : base(0x904)
        {
            //Weight = 7.0;
        }

        public DualPointedSpear(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.DoubleStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Disarm;
        public override int StrengthReq => 50;
        public override int MinDamage => 11;
        public override int MaxDamage => 14;
        public override float Speed => 2.25f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 80;

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
