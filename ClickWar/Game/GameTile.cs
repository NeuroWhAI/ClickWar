using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar.Game
{
    public class GameTile
    {
        public GameTile()
        {
            this.Owner = "";
            this.Power = 0;
        }

        public GameTile(string owner, int power)
        {
            this.Owner = owner;
            this.Power = power;
        }

        //##################################################################################

        public int Index
        { get; set; }

        public string Owner
        { get; set; }

        public int Power
        { get; set; }

        //##################################################################################

        public bool HaveOwner
        {
            get
            {
                return (Owner.Length <= 0);
            }
        }

        //##################################################################################

        public MongoDB.Bson.BsonDocument ToBsonDocument()
        {
            return new MongoDB.Bson.BsonDocument
            {
                { "Index", this.Index },
                { "Owner", this.Owner },
                { "Power", this.Power }
            };
        }

        public void FromBsonDocument(MongoDB.Bson.BsonDocument doc)
        {
            this.Index = doc["Index"].AsInt32;
            this.Owner = doc["Owner"].AsString;
            this.Power = doc["Power"].AsInt32;
        }
    }
}
