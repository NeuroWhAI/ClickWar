using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Builders;

namespace ClickWar.Util
{
    public class DBHelper
    {
        public DBHelper()
        {
            string key = Properties.Resources.ShieldInfo;

            // 키 반전
            var keyBytes = key.ToCharArray();
            Array.Reverse(keyBytes);
            key = new string(keyBytes);

            // 암호 복호화
            m_dbURI = EncoderDecoder.Decode("ee5I9DdM8/y/jNaPsKAojoB4k83aGvwUWUY6ksQe2Oap86cb17PE6rufJxXHPD1OOCTP56t4AbL0g3XSdCRM+w==",
                Enumerable.Range(0, key.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(key.Substring(x, 2), 16))
                     .ToArray());
        }

        //##################################################################################

        protected readonly string m_dbURI = "";
        protected MongoClient m_client = null;
        protected IMongoDatabase m_db = null;

        //##################################################################################

        public delegate void ReceiveCollectionDelegate(IMongoCollection<BsonDocument> col);
        public delegate void ReceiveDocumentDelegate(BsonDocument doc);

        //##################################################################################

        protected Task m_task = null;

        //##################################################################################

        public int Connect()
        {
            m_client = new MongoClient(m_dbURI);
            m_db = m_client.GetDatabase("click_war");


            return 0;
        }

        public int WaitAllTask()
        {
            if (m_task != null)
            {
                m_task.Wait();
                m_task = null;
            }


            return 0;
        }

        //##################################################################################

        public IMongoCollection<BsonDocument> GetCollection(string name)
        {
            return m_db.GetCollection<BsonDocument>(name);
        }

        public void GetCollectionAsync(string name, ReceiveCollectionDelegate callback)
        {
            this.WaitAllTask();

            m_task = Task.Factory.StartNew(() => callback(GetCollection(name)));
        }

        public bool CheckPropertyOverlap(string collectionName, string propertyName, string checkValue)
        {
            if (propertyName.Length <= 0)
                throw new ArgumentException("propertyName의 길이가 너무 짧습니다.");

            var col = this.GetCollection(collectionName);

            var filter = Builders<BsonDocument>.Filter.Eq(propertyName, checkValue);
            if (col.Find(filter).Count() > 0)
            {
                return true;
            }


            return false;
        }

        public bool InsertToCollection(string collectionName, BsonDocument document)
        {
            var col = this.GetCollection(collectionName);
            col.InsertOne(document);


            return true;
        }

        //##################################################################################

        public BsonDocument CreateDocument(string collectionName, string documentName, BsonDocument document)
        {
            var col = this.GetCollection(collectionName);

            document.Add(new BsonElement(documentName, ""));

            col.InsertOne(document);


            return document;
        }

        public BsonDocument GetDocument(string collectionName, string documentName)
        {
            var col = this.GetCollection(collectionName);

            var filter = Builders<BsonDocument>.Filter.Exists(documentName);
            var docs = col.Find(filter).ToList();

            if (docs.Count > 0)
            {
                return docs[0];
            }


            return null;
        }

        public void GetDocumentAsync(string collectionName, string documentName,
            ReceiveDocumentDelegate callback)
        {
            WaitAllTask();

            m_task = Task.Factory.StartNew(() => callback(GetDocument(collectionName, documentName)));
        }

        public void UpdateDocument(string collectionName, string documentName, BsonDocument newDoc)
        {
            var col = this.GetCollection(collectionName);

            var filter = Builders<BsonDocument>.Filter.Exists(documentName);
            col.ReplaceOne(filter, newDoc);
        }

        public void UpdateDocumentArray(string collectionName, string documentName, string arrayProperty, string indexName,
            List<KeyValuePair<int, BsonValue>> indexItemListNeedUpdate)
        {
            var col = this.GetCollection(collectionName);

            foreach (var itemInfo in indexItemListNeedUpdate)
            {
                var filter = Builders<BsonDocument>.Filter.Eq(arrayProperty + "." + indexName, itemInfo.Key);
                var update = Builders<BsonDocument>.Update.Set(arrayProperty + "." + itemInfo.Key, itemInfo.Value);

                col.UpdateOne(filter, update);
            }
        }
    }
}
