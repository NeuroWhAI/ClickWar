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
                int temp = -1;

                lock(m_indexLock)
                {
                    int result = Util.Utility.DecodeValue(m_index, m_indexKey);

                    if (result == -m_realIndex)
                        temp = -m_realIndex;
                    else
                        throw new Exception("Fuck you hacker.");
                }

                return temp;
            }
            set
            {
                lock(m_indexLock)
                {
                    m_realIndex = -value;
                    m_index = Util.Utility.EncodeValue(value, out m_indexKey);
                }

                this.IsLastVersion = false;
            }
        }

        public string Owner
        {
            get
            {
                string temp;

                lock(m_ownerLock)
                {
                    string result = new string(m_owner.Reverse().ToArray());

                    if (result == m_realOwner)
                        temp = m_realOwner;
                    else
                        throw new Exception("Fuck you hacker.");
                }

                return temp;
            }
            set
            {
                lock(m_ownerLock)
                {
                    m_realOwner = value;
                    m_owner = new string(m_realOwner.Reverse().ToArray());
                }

                this.IsLastVersion = false;
            }
        }

        public int Power
        {
            get
            {
                int temp = 0;

                lock(m_powerLock)
                {
                    int result = Util.Utility.DecodeValue(m_power, m_powerKey);

                    if (result == -m_realPower)
                        temp = -m_realPower;
                    else
                        throw new Exception("Fuck you hacker.");
                }

                return temp;
            }
            set
            {
                lock(m_powerLock)
                {
                    m_realPower = -value;
                    m_power = Util.Utility.EncodeValue(value, out m_powerKey);
                }

                this.IsLastVersion = false;
            }
        }

        public string Sign
        {
            get
            {
                string temp;

                lock (m_signLock)
                {
                    temp = new string(m_sign.Reverse().ToArray());
                }

                return temp;
            }
            set
            {
                lock(m_signLock)
                {
                    m_realSign = value;
                    m_sign = new string(value.Reverse().ToArray());
                }

                this.IsLastVersion = false;
            }
        }

        //##################################################################################

        protected int m_index = -1, m_indexKey = 0, m_realIndex = -1;
        protected readonly object m_indexLock = new object();

        protected string m_owner = "", m_realOwner = "";
        protected readonly object m_ownerLock = new object();

        protected int m_power = 0, m_powerKey = 0, m_realPower = 0;
        protected readonly object m_powerLock = new object();

        protected string m_sign = "", m_realSign = "";
        protected readonly object m_signLock = new object();

        //##################################################################################

        public bool HaveOwner
        {
            get { return (Owner.Length > 0); }
        }

        public bool HaveSign
        {
            get { return (Sign.Length > 0); }
        }

        public bool IsLastVersion
        { get; set; } = false;

        public bool WasIgnored
        { get; set; } = false;

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

            this.IsLastVersion = true;
        }
    }
}
