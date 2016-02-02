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
        {
            get
            {
                lock(m_indexLock)
                {
                    int result = m_index - m_indexKey;

                    if (result == m_realIndex)
                        return m_realIndex;
                    else
                        throw new Exception("Fuck you hacker.");
                }
            }
            set
            {
                lock(m_indexLock)
                {
                    m_realIndex = value;
                    m_indexKey = Util.Utility.Random.Next(128, 1024);
                    m_index = value + m_indexKey;
                }
            }
        }

        public string Owner
        {
            get
            {
                lock(m_ownerLock)
                {
                    string result = new string(m_owner.Reverse().ToArray());

                    if (result == m_realOwner)
                        return m_realOwner;
                    else
                        throw new Exception("Fuck you hacker.");
                }
            }
            set
            {
                lock(m_ownerLock)
                {
                    m_realOwner = value;
                    m_owner = new string(m_realOwner.Reverse().ToArray());
                }
            }
        }

        public int Power
        {
            get
            {
                lock(m_powerLock)
                {
                    int result = m_power - m_powerKey;

                    if (result == m_realPower)
                        return m_realPower;
                    else
                        throw new Exception("Fuck you hacker.");
                }
            }
            set
            {
                lock(m_powerLock)
                {
                    m_realPower = value;
                    m_powerKey = Util.Utility.Random.Next(128, 1024);
                    m_power = value + m_powerKey;
                }
            }
        }

        public string Sign
        {
            get { return m_sign; }
            set { m_sign = value; }
        }

        //##################################################################################

        protected int m_index = -1, m_indexKey = Util.Utility.Random.Next(1024), m_realIndex = -1;
        protected readonly object m_indexLock = new object();

        protected string m_owner = "", m_realOwner = "";
        protected readonly object m_ownerLock = new object();

        protected int m_power = 0, m_powerKey = Util.Utility.Random.Next(1024), m_realPower = 0;
        protected readonly object m_powerLock = new object();

        protected string m_sign = "";

        //##################################################################################

        public bool HaveOwner
        {
            get { return (Owner.Length > 0); }
        }

        public bool HaveSign
        {
            get { return (Sign.Length > 0); }
        }

        //##################################################################################

        public MongoDB.Bson.BsonDocument ToBsonDocument()
        {
            return new MongoDB.Bson.BsonDocument
            {
                { "Index", this.Index },
                { "Owner", this.Owner },
                { "Power", this.Power },
                { "Sign", this.Sign }
            };
        }

        public void FromBsonDocument(MongoDB.Bson.BsonDocument doc)
        {
            this.Owner = doc["Owner"].AsString;
            this.Power = doc["Power"].AsInt32;
            this.Sign = doc["Sign"].AsString;
        }
    }
}
