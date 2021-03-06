using Server.Items;

using System.Linq;

namespace Server.Engines.ArtisanFestival
{
    public enum TreeStage
    {
        One = 1,
        Two,
        Three,
        Four,
        Five
    }

    public class TownTree : BaseAddon
    {
        private TreeStage _Stage;

        [CommandProperty(AccessLevel.GameMaster)]
        public TreeStage Stage
        {
            get { return _Stage; }
            set { _Stage = value; CheckTreeID(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TreeType { get; private set; }

        [Constructable]
        public TownTree(int type)
        {
            _Stage = TreeStage.One;
            TreeType = type;

            int[,] list;
            Point3D treeOffset;

            switch (type)
            {
                default:
                    list = m_Type1;
                    treeOffset = new Point3D(1, 0, 0);
                    break;
                case 2:
                    list = m_Type2;
                    treeOffset = new Point3D(1, 1, 0);
                    break;
                case 3:
                    list = m_Type3;
                    treeOffset = new Point3D(0, 0, 0);
                    break;
                case 4:
                    list = m_Type4;
                    treeOffset = new Point3D(-1, 0, 0);
                    break;
            }

            AddComponent(new TownTreeComponent(), treeOffset.X, treeOffset.Y, treeOffset.Z);

            for (int i = 0; i < list.Length / 4; i++)
            {
                var id = list[i, 0];

                if (id == 6077)
                {
                    id = Utility.RandomList(6077, 6078, 6079, 6080);
                }

                AddComponent(new AddonComponent(id), list[i, 1], list[i, 2], list[i, 3]);
            }
        }

        public TownTree(Serial serial)
            : base(serial)
        {
        }

        public void CheckTreeID()
        {
            var tree = Components.FirstOrDefault(comp => comp is TownTreeComponent);

            if (tree != null)
            {
                switch ((int)_Stage)
                {
                    case 1:
                        if (tree.ItemID != 0x4C18 && tree.ItemID != 0x4C19)
                        {
                            tree.ItemID = Utility.RandomList(0x4C18, 0x4C19);
                        }
                        break;
                    case 2:
                        if (tree.ItemID != 0x4C17)
                        {
                            tree.ItemID = 0x4C17;
                        }
                        break;
                    case 3:
                        if (tree.ItemID != 0x9DB7)
                        {
                            tree.ItemID = 0x9DB7;
                        }
                        break;
                    case 4:
                        if (tree.ItemID != 0x9DB8)
                        {
                            tree.ItemID = 0x9DB8;
                        }
                        break;
                    case 5:
                        if (tree.ItemID != 0x9DBB)
                        {
                            tree.ItemID = 0x9DBB;
                        }

                        if (!Components.Any(c => c.ItemID == 0x9E97))
                        {
                            AddComponent(new AddonComponent(0x9E97) { Hue = 1265 }, tree.Offset.X + 1, tree.Offset.Y + 1, tree.Offset.Z + 65);
                        }

                        break;
                }
            }

            if ((int)_Stage != 5)
            {
                var star = Components.FirstOrDefault(comp => comp.ItemID == 0x9E97);

                if (star != null)
                {
                    star.Addon = null;
                    Components.Remove(star);
                    
                    star.Delete();
                }
            }      
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)Stage);
            writer.Write(TreeType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            Stage = (TreeStage)reader.ReadInt();
            TreeType = reader.ReadInt();
        }

        private static int[,] m_Type1 = new int[,] {
              {6094, -2, 5, 0}, {6094, 7, -3, 0}, {6094, 6, 4, 0}// 1	2	3	
			, {6094, 7, 2, 0}, {6094, 5, 5, 0}, {6094, -4, -1, 0}// 4	5	6	
			, {6093, -3, -2, 0}, {6093, -4, 2, 0}, {6093, -2, 2, 0}// 7	8	9	
			, {6093, -3, 3, 0}, {6093, -2, 4, 0}, {6093, -1, 5, 0}// 10	11	12	
			, {6093, 0, 6, 0}, {6093, 1, 6, 0}, {6093, 2, 6, 0}// 13	14	15	
			, {6093, 3, 5, 0}, {6093, 4, 6, 0}, {6093, 4, 5, 0}// 16	17	18	
			, {6093, 5, 4, 0}, {6093, 6, 3, 0}, {6093, 6, 2, 0}// 19	20	21	
			, {6093, 7, 1, 0}, {6093, 7, 0, 0}, {6093, 7, -1, 0}// 22	23	24	
			, {6093, 7, -2, 0}, {6093, 6, -3, 0}, {6093, -4, -3, 0}// 25	26	27	
			, {6093, -4, -2, 0}, {6082, -2, 2, 0}, {6077, 3, 4, 0}// 28	29	30	
			, {6077, 2, 4, 0}, {6077, 2, 3, 0}, {6077, 1, 4, 0}// 31	32	33	
			, {6077, 0, 4, 0}, {6077, 0, 3, 0}, {6077, 1, 3, 0}// 34	35	36	
			, {6077, 3, 3, 0}, {6077, 4, 3, 0}, {6077, 4, 2, 0}// 37	38	39	
			, {6077, 4, 1, 0}, {6077, 5, 1, 0}, {6077, 4, 0, 0}// 40	41	42	
			, {6077, 5, 0, 0}, {6077, 5, -1, 0}, {6077, 5, -2, 0}// 43	44	45	
			, {6077, 4, -3, 0}, {6077, 3, -3, 0}, {6077, 2, -3, 0}// 46	47	48	
			, {6077, 1, -3, 0}, {6077, 0, -3, 0}, {6077, -1, -3, 0}// 49	50	51	
			, {6077, -2, -3, 0}, {6077, -1, 3, 0}, {6090, -3, -3, 0}// 52	53	54	
			, {6084, 5, -3, 0}, {6086, 6, -3, 0}, {6087, 6, 2, 0}// 55	56	57	
			, {6092, 6, -2, 0}, {6092, 6, -1, 0}, {6092, 6, 0, 0}// 58	59	60	
			, {6092, 6, 1, 0}, {6081, 5, 2, 0}, {6092, 5, 3, 0}// 61	62	63	
			, {6087, 5, 4, 0}, {6081, 4, 4, 0}, {6087, 4, 5, 0}// 64	65	66	
			, {6089, 3, 5, 0}, {6090, -3, -2, 0}, {6089, 2, 5, 0}// 67	68	69	
			, {6089, 1, 5, 0}, {6088, -1, 5, 0}, {6082, -1, 4, 0}// 70	71	72	
			, {6088, -2, 4, 0}, {6090, -2, 3, 0}, {6089, -3, 2, 0}// 73	74	75	
			, {6088, -4, 2, 0}, {6082, -4, 1, 0}, {6083, -3, -1, 0}// 76	77	78	
			, {6091, -4, -1, 0}, {6089, 0, 5, 0}, {6077, 0, 2, 0}// 79	80	81	
			, {6077, -3, 0, 0}, {6077, -4, 0, 0}, {6077, 4, -1, 0}// 82	83	84	
			, {6077, 4, -2, 0}, {6077, 2, 2, 0}, {6077, 1, 2, 0}// 85	86	87	
			, {6077, -1, 2, 0}, {6077, -2, -2, 0}, {6077, -2, -1, 0}// 88	89	90	
			, {6077, -2, 0, 0}, {6077, -3, 1, 0}, {6077, 3, 2, 0}// 91	92	93	
			, {6077, -2, 1, 0}, {6077, 3, 1, 0}, {6077, 2, 1, 0}// 94	95	96	
			, {6077, 1, 1, 0}, {6077, 0, 1, 0}, {6077, -1, 1, 0}// 97	98	99	
			, {6077, 3, 0, 0}, {6077, 3, -1, 0}, {6077, 3, -2, 0}// 100	101	102	
			, {6077, -1, -2, 0}, {6077, -1, -1, 0}, {6077, -1, 0, 0}// 103	104	105	
			, {6077, 0, 0, 0}, {6077, 2, 0, 0}, {6077, 2, -2, 0}// 106	107	108	
			, {6077, 1, -2, 0}, {6077, 0, -2, 0}, {6077, 2, -1, 0}// 109	110	111	
			, {6077, 1, 0, 0}, {6077, 0, -1, 0}, {6077, 1, -1, 0}// 112	113	114	
			, {6094, 5, -5, 0}, {6094, 4, -5, 0}//	116	117	
			, {6094, 2, -6, 0}, {6093, 6, -4, 0}, {6093, 5, -4, 0}// 118	119	120	
			, {6093, 4, -5, 0}, {6093, 3, -5, 0}, {6093, 2, -5, 0}// 121	122	123	
			, {6093, 1, -6, 0}, {6093, 0, -6, 0}, {6093, -1, -6, 0}// 124	125	126	
			, {6093, -2, -6, 0}, {6093, -3, -5, 0}, {6093, -3, -4, 0}// 127	128	129	
			, {6077, 1, -4, 0}, {6077, 0, -4, 0}, {6085, -3, -4, 0}// 130	131	132	
			, {6083, -2, -4, 0}, {6090, -2, -5, 0}, {6085, -2, -6, 0}// 133	134	135	
			, {6086, 0, -6, 0}, {6091, -1, -6, 0}, {6084, 0, -5, 0}// 136	137	138	
			, {6091, 1, -5, 0}, {6086, 2, -5, 0}, {6084, 2, -4, 0}// 139	140	141	
			, {6091, 3, -4, 0}, {6091, 4, -4, 0}, {6086, 5, -4, 0}// 142	143	144	
			, {6077, -1, -5, 0}, {6077, -1, -4, 0}, {6094, -5, 3, 0}// 145	146	147	
			, {6093, -5, -1, 0}, {6093, -6, 0, 0}, {6093, -5, 1, 0}// 148	149	150	
			, {6085, -5, -1, 0}, {6090, -5, 0, 0}, {6088, -5, 1, 0}// 151	152	153	
        };

        private static int[,] m_Type2 = new int[,]
        {
              {6093, 4, 5, 5}// 34	 35	 36	
			, {6093, 4, 6, 5}, {6093, 5, 5, 5}, {6093, 6, 5, 5}// 37	38	39	
			, {6093, 3, 5, 5}, {6093, 5, 4, 5}, {6093, 5, 3, 5}// 40	41	42	
			, {6093, 4, 2, 5}, {6093, 3, 1, 5}, {6093, 4, 4, 5}// 43	44	45	
			, {6093, 3, 4, 5}, {6093, 4, 3, 5}, {6093, 3, 0, 5}// 46	47	48	
			, {6093, 3, 2, 5}, {6093, -1, 5, 10}, {6093, -2, 5, 10}// 49	50	51	
			, {6093, -3, -5, 5}, {6093, -2, -3, 5}// 52	53	54	
			, {6093, -1, -4, 5}, {6093, -2, -4, 5}, {6093, -1, -1, 5}// 55	56	57	
			, {6093, -3, 4, 10}, {6093, -1, 3, 10}, {6093, 2, 3, 5}// 58	59	60	
			, {6093, -5, -4, 5}, {6093, 0, 0, 5}, {6093, -2, -1, 5}// 61	62	63	
			, {6093, -3, -3, 5}, {6093, -1, -5, 5}, {6093, 0, -4, 5}// 64	65	66	
			, {6093, -1, -3, 5}, {6093, -4, -3, 5}, {6093, -2, -4, 5}// 67	68	69	
			, {6093, -2, 0, 5}, {6093, -3, -2, 5}, {6093, -1, 4, 10}// 70	71	72	
			, {6093, -2, 3, 10}, {6093, 2, 3, 5}, {6093, 2, 2, 5}// 73	74	75	
			, {6093, 1, 1, 5}, {6093, 0, 2, 6}, {6093, -1, 2, 9}// 76	77	78	
			, {6093, 0, 4, 10}, {6093, 2, 3, 5}, {6093, 2, 3, 5}// 79	80	81	
			, {6093, 2, 1, 5}, {6093, 1, 2, 5}, {6093, 2, 0, 5}// 82	83	84	
			, {6093, 1, 0, 5}, {6093, 0, 1, 5}, {6093, -1, 0, 5}// 85	86	87	
			, {6093, 0, -1, 5}, {6093, -1, -2, 5}, {6093, -2, -2, 5}// 88	89	90	
		};

        private static int[,] m_Type3 = new int[,]
        {
              {6093, 3, 2, 0}, {6093, 2, 3, 0}, {6093, 1, 3, 0}// 1	2	3	
			, {6093, 0, 3, 0}, {6093, 0, -3, 0}, {6093, 1, -3, 0}// 4	5	6	
			, {6093, 2, -3, 0}, {6093, 3, -2, 0}, {6093, 3, -1, 0}// 7	8	9	
			, {6093, 3, 0, 0}, {6093, 3, 1, 0} // 10	11	12	
            , {6077, 1, 2, 0}, {6077, 0, 2, 0}, {6077, 0, 1, 0}// 13	14	15	
			, {6077, 0, 0, 0}, {6077, 1, 0, 0}, {6077, 1, 1, 0}// 16	17	18	
			, {6077, 2, 1, 0}, {6077, 2, 0, 0}, {6077, 2, -1, 0}// 19	20	21	
			, {6077, 1, -1, 0}, {6077, 0, -1, 0}, {6077, 1, -2, 0}// 22	23	24	
			, {6077, 0, -2, 0}, {6086, 2, -3, 0}, {6087, 2, 3, 0}// 25	26	27	
			, {6089, 1, 3, 0}, {6089, 0, 3, 0}, {6091, 0, -3, 0}// 28	29	30	
			, {6091, 1, -3, 0}, {6081, 2, 2, 0}, {6087, 3, 2, 0}// 31	32	33	
			, {6086, 3, -2, 0}, {6084, 2, -2, 0}, {6092, 3, 1, 0}// 34	35	36	
			, {6092, 3, 0, 0}, {6092, 3, -1, 0}, {6093, -1, 3, 0}// 37	38	39	
			, {6093, -2, 3, 0}, {6093, -3, 2, 0}, {6093, -3, -1, 0}// 40	41	42	
			, {6093, -3, 1, 0}, {6093, -3, 0, 0}, {6093, -3, -2, 0}// 43	44	45	
			, {6093, -2, -3, 0}, {6093, -1, -3, 0}, {6077, -1, 2, 0}// 46	47	48	
			, {6077, -1, 1, 0}, {6077, -1, 0, 0}, {6077, -1, -1, 0}// 49	50	51	
			, {6077, -1, -2, 0}, {6077, -2, 1, 0}, {6077, -2, 0, 0}// 52	53	54	
			, {6077, -2, -1, 0}, {6088, -2, 3, 0}, {6088, -3, 2, 0}// 55	56	57	
			, {6085, -2, -3, 0}, {6085, -3, -2, 0}, {6089, -1, 3, 0}// 58	59	60	
			, {6082, -2, 2, 0}, {6090, -3, 1, 0}, {6090, -3, 0, 0}// 61	62	63	
			, {6090, -3, -1, 0}, {6083, -2, -2, 0}, {6091, -1, -3, 0}// 64	65	66	
		};

        private static int[,] m_Type4 = new int[,]
        {
              {6093, -2, -4, 0}, {6093, -3, -3, 0}, {6093, -3, -2, 0}// 1	2	3	
			, {6093, -4, -1, 0}, {6093, -4, 0, 0}, {6093, -4, 1, 0}// 4	5	6	
			, {6093, -3, 2, 0}, {6093, -3, 3, 0}, {6093, -2, 3, 0}// 7	8	9	
			, {6093, -1, 3, 0}, {6093, 0, 4, 0}, {6093, 1, 4, 0}// 10	11	12	
			, {6093, 2, 4, 0}, {6093, 3, 3, 0}, {6093, 4, 2, 0}// 13	14	15	
			, {6093, 3, 1, 0}, {6093, 3, 0, 0}, {6093, 2, 0, 0}// 16	17	18	
			, {6093, 2, -1, 0}, {6093, 1, -2, 0}, {6093, 1, -3, 0}// 19	20	21	
			, {6093, 1, -4, 0}, {6093, 0, -4, 0}, {6093, -1, -4, 0}// 22	23	24	
			, {6077, 0, 2, 0}, {6077, 0, 1, 0}, {6077, -1, 1, 0}// 25	26	27	
			, {6077, -1, 2, 0}, {6077, -2, 2, 0}, {6077, -2, 1, 0}// 28	29	30	
			, {6077, 1, 3, 0}, {6077, 1, 2, 0}, {6077, 2, 2, 0}// 31	32	33	
			, {6077, 2, 1, 0}, {6077, 1, 1, 0}, {6077, 1, 0, 0}// 34	35	36	
			, {6077, -2, 0, 0}, {6077, -1, 0, 0}, {6077, 0, 0, 0}// 37	38	39	
			, {6077, 0, -1, 0}, {6077, -1, -1, 0}, {6077, -2, -1, 0}// 40	41	42	
			, {6077, 0, -2, 0}, {6077, -1, -2, 0}, {6077, -2, -2, 0}// 43	44	45	
			, {6077, 0, -3, 0}, {6077, -1, -3, 0}, {6077, -2, -3, 0}// 46	47	48	
			, {6077, -3, 0, 0}, {6088, 0, 4, 0}, {6089, 1, 4, 0}// 49	50	51	
			, {6082, 0, 3, 0}, {6089, -1, 3, 0}, {6089, -2, 3, 0}// 52	53	54	
			, {6088, -3, 3, 0}, {6087, 2, 4, 0}, {6081, 2, 3, 0}// 55	56	57	
			, {6087, 3, 3, 0}, {6091, 0, -4, 0}// 58	59	60	
			, {6092, 3, 2, 0}, {6092, 3, 1, 0}, {6086, 3, 0, 0}// 61	62	63	
			, {6084, 2, 0, 0}, {6086, 2, -1, 0}, {6084, 1, -1, 0}// 64	65	66	
			, {6090, -3, 2, 0}, {6082, -3, 1, 0}, {6088, -4, 1, 0}// 67	68	69	
			, {6090, -4, 0, 0}, {6085, -4, -1, 0}, {6083, -3, -1, 0}// 70	71	72	
			, {6090, -3, -2, 0}, {6090, -3, -3, 0}, {6092, 1, -2, 0}// 73	74	75	
			, {6092, 1, -3, 0}, {6086, 1, -4, 0}, {6085, -3, -4, 0}// 76	77	78	
			, {6091, -1, -4, 0}, {6091, -2, -4, 0}// 79	80	
		};
    }

    public class TownTreeComponent : AddonComponent
    {
        public TownTreeComponent()
            : base(Utility.RandomList(0x4C18, 0x4C19))
        {
        }

        public TownTreeComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
