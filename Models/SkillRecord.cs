using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace LostKit.Models
{
    public class SkillRecord
    {
        public SkillType Type { get; set; }
        public int Level { get; set; }
        public double Value { get; set; }  // Divided by 10
        public DateTime Date { get; set; }
        public int Rank { get; set; }

        public SkillRecord(int type, int level, int value, string date, int rank)
        {
            Type = (SkillType)type;
            Level = level;
            Value = value / 10.0;  // Dividing value by 10
            Date = DateTime.Parse(date);
            Rank = rank;
        }
    }
    public enum SkillType
    {
        Overall = 0,
        Attack = 1,
        Defence = 2,
        Strength = 3,
        Hitpoints = 4,
        Ranged = 5,
        Prayer = 6,
        Magic = 7,
        Cooking = 8,
        Woodcutting = 9,
        Fletching = 10,
        Fishing = 11,
        Firemaking = 12,
        Crafting = 13,
        Smithing = 14,
        Mining = 15,
        Herblore = 16,
        Agility = 17,
        Thieving = 18,
        Runecrafting = 21
    }
}
