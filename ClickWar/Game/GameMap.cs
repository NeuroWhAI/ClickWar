﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Drawing;

namespace ClickWar.Game
{
    public class GameMap
    {
        public GameMap()
        {

        }

        public GameMap(Util.DBHelper db)
        {
            SyncAll(db);
        }

        //##################################################################################

        protected GameTile[,] m_tileMap = null;

        //##################################################################################

        public int Width
        { get { return m_tileMap.GetLength(0); } }

        public int Height
        { get { return m_tileMap.GetLength(1); } }

        //##################################################################################

        public delegate void GameEventDelegate(int x, int y);
        public event GameEventDelegate WhenTileUnderAttack; // 타일이 공격받고 있을때
        public event GameEventDelegate WhenTileCaptured; // 타일이 점령되었을때
        public event GameEventDelegate WhenTileUpgraded; // 타일의 힘이 강해졌을때

        //##################################################################################

        protected string m_prevShutMsg = "";

        public delegate void ShutdownEventDelegate(string msg, bool shutdownFlag);
        public event ShutdownEventDelegate WhenShutdownMessageChanged;

        public string ShutdownMessage
        { get; set; } = "";

        public bool ShutdownFlag
        { get; set; } = false;

        //##################################################################################

        public int SyncAll(Util.DBHelper db)
        {
            SyncMapSize(db);
            SyncTile(db);
            SyncServerMessage(db);


            return 0;
        }

        public int SyncAllRect(Util.DBHelper db, Point startIdx, Point endIdx)
        {
            SyncMapSize(db);
            SyncTileRect(db, startIdx, endIdx);
            SyncServerMessage(db);


            return 0;
        }

        public int SyncServerMessage(Util.DBHelper db)
        {
            var shutdownDoc = db.GetDocument("Server", "Shutdown");
            if (shutdownDoc == null)
            {
                shutdownDoc = db.CreateDocument("Server", "Shutdown",
                    new BsonDocument()
                    {
                        { "Flag", false },
                        { "Message", "" }
                    });
            }
            else
            {
                var flag = shutdownDoc["Flag"].AsBoolean;
                var msg = shutdownDoc["Message"].AsString;

                this.ShutdownFlag = flag;
                this.ShutdownMessage = msg;


                // 이벤트 발생
                if (m_prevShutMsg != msg && this.WhenShutdownMessageChanged != null)
                    this.WhenShutdownMessageChanged(msg, flag);


                m_prevShutMsg = msg;
            }


            return 0;
        }

        public int SyncMapSize(Util.DBHelper db)
        {
            // 맵 크기 정보를 얻어옴.
            var mapSizeDoc = db.GetDocument("MapInfo", "MapSize");
            if (mapSizeDoc == null)
            {
                mapSizeDoc = db.CreateDocument("MapInfo", "MapSize",
                    new BsonDocument
                    {
                        { "Width", 64 },
                        { "Height", 64 }
                    });
            }

            int width = mapSizeDoc["Width"].AsInt32;
            int height = mapSizeDoc["Height"].AsInt32;

            // 타일맵이 없거나 DB의 맵 크기와 다르면 새로 생성한다.
            if (m_tileMap == null
                ||
                width != m_tileMap.GetLength(0) || height != m_tileMap.GetLength(1))
            {
                if (m_tileMap == null)
                    m_tileMap = new GameTile[width, height];

                for (int w = 0; w < width; ++w)
                {
                    for (int h = 0; h < height; ++h)
                    {
                        m_tileMap[w, h] = new GameTile();
                        m_tileMap[w, h].Index = w * height + h;
                    }
                }
            }


            return 0;
        }

        public int SyncTile(Util.DBHelper db)
        {
            // 타일의 배열이 존재하는 문서를 얻어옴.
            var tileArrayDoc = db.GetDocument("Tiles", "Tiles");

            // 문서가 존재하지 않으면 만든 뒤 새 타일정보로 채우고
            // 존재한다면 DB의 타일정보를 받아와 적용시킨다.
            if (tileArrayDoc == null)
            {
                // 타일 배열을 만들고 타일의 정보를 등록.
                var tileArr = new BsonArray(m_tileMap.Length);
                foreach (var tile in m_tileMap)
                {
                    tileArr.Add(tile.ToBsonDocument());
                }

                // 타일목록 문서를 생성.
                tileArrayDoc = db.CreateDocument("Tiles", "Tiles",
                    new BsonDocument
                    {
                        { "List", tileArr }
                    });
            }
            else
            {
                // 문서에서 타일배열을 얻어옴.
                var tileArray = tileArrayDoc["List"].AsBsonArray;

                // DB타일의 정보를 인덱스에 해당하는 타일에 설정.
                for (int index = 0;
                    index < tileArray.Count && index < m_tileMap.Length;
                    ++index)
                {
                    SyncTileAt(db, tileArray, index);
                }
            }


            return 0;
        }

        public int SyncTileRect(Util.DBHelper db, Point startIdx, Point endIdx)
        {
            // 타일의 배열이 존재하는 문서를 얻어옴.
            var tileArrayDoc = db.GetDocument("Tiles", "Tiles");
            
            // 문서가 존재한다면 DB의 타일정보를 받아와 적용시킨다.
            if (tileArrayDoc != null)
            {
                // 문서에서 타일배열을 얻어옴.
                var tileArray = tileArrayDoc["List"].AsBsonArray;

                // DB타일의 정보를 인덱스에 해당하는 타일에 설정.
                for (int w = startIdx.X;
                    w <= endIdx.X && w < m_tileMap.GetLength(0); ++w)
                {
                    for (int h = startIdx.Y;
                        h <= endIdx.Y && h < m_tileMap.GetLength(1); ++h)
                    {
                        SyncTileAt(db, tileArray, w * m_tileMap.GetLength(1) + h);
                    }
                }
            }


            return 0;
        }

        protected int SyncTileAt(Util.DBHelper db, BsonArray tileArray, int index)
        {
            int h = index % m_tileMap.GetLength(1);
            int w = index / m_tileMap.GetLength(1);

            if (w < m_tileMap.GetLength(0) && h < m_tileMap.GetLength(1))
            {
                string oldOwner = m_tileMap[w, h].Owner;
                int oldPower = m_tileMap[w, h].Power;

                // 타일 갱신
                m_tileMap[w, h].FromBsonDocument(tileArray[index].AsBsonDocument);


                // 이벤트 발생
                if (oldOwner != m_tileMap[w, h].Owner)
                    WhenTileCaptured(w, h);
                if (oldPower > m_tileMap[w, h].Power)
                    WhenTileUpgraded(w, h);
                else if (oldPower < m_tileMap[w, h].Power)
                    WhenTileUnderAttack(w, h);
            }


            return 0;
        }

        //##################################################################################

        protected int GetDBTileAt(Util.DBHelper db, int tileWidthIndex, int tileHeightIndex,
            out BsonDocument doc, out BsonArray arr)
        {
            // 타일의 배열이 존재하는 문서를 얻어옴.
            doc = db.GetDocument("Tiles", "Tiles");

            if (doc != null)
            {
                // 문서에서 타일배열을 얻어옴.
                arr = doc["List"].AsBsonArray;
                

                int index = tileWidthIndex * m_tileMap.GetLength(1) + tileHeightIndex;
                return index;
            }


            arr = null;
            return -1;
        }

        protected int UploadTileAt(Util.DBHelper db, int tileWidthIndex, int tileHeightIndex)
        {
            // 타일맵 범위 초과시 아무것도 안함.
            if (tileWidthIndex < 0 || tileWidthIndex >= m_tileMap.GetLength(0)
                ||
                tileHeightIndex < 0 || tileHeightIndex >= m_tileMap.GetLength(1))
            {
                return -1;
            }


            BsonDocument tileArrayDoc;
            BsonArray tileArray;
            int index = GetDBTileAt(db, tileWidthIndex, tileHeightIndex,
                out tileArrayDoc, out tileArray);

            // 타일배열 문서가 존재한다면
            if (tileArrayDoc != null && tileArray != null)
            {
                // 타일배열 크기 갱신
                while (index >= tileArray.Count)
                {
                    var newTile = new GameTile();
                    newTile.Index = index;
                    tileArray.Add(newTile.ToBsonDocument());
                }

                // 타일 내용 갱신.
                tileArray[index] = m_tileMap[tileWidthIndex, tileHeightIndex].ToBsonDocument();

                // 동기화
                //db.UpdateDocument("Tiles", "Tiles", tileArrayDoc);
                db.UpdateDocumentArray("Tiles", "Tiles", "List", "Index",
                    new List<KeyValuePair<int, BsonValue>>()
                    {
                        new KeyValuePair<int, BsonValue>(index, tileArray[index])
                    });
            }


            return 0;
        }

        protected int UploadTileForeach(Util.DBHelper db, System.Drawing.Point[] indexList)
        {
            BsonDocument tileArrayDoc;
            BsonArray tileArray;
            GetDBTileAt(db, -1, -1,
                out tileArrayDoc, out tileArray);

            if (tileArrayDoc != null && tileArray != null)
            {
                List<KeyValuePair<int, BsonValue>> indexItemListNeedUpdate = new List<KeyValuePair<int, BsonValue>>();

                foreach (var location in indexList)
                {
                    // 타일맵 범위 초과시 아무것도 안함.
                    if (location.X < 0 || location.X >= m_tileMap.GetLength(0)
                        ||
                        location.Y < 0 || location.Y >= m_tileMap.GetLength(1))
                    {
                        continue;
                    }


                    int index = location.X * m_tileMap.GetLength(1) + location.Y;

                    // 타일배열 크기 갱신
                    while (index >= tileArray.Count)
                    {
                        var newTile = new GameTile();
                        newTile.Index = index;
                        tileArray.Add(newTile.ToBsonDocument());
                    }

                    // 타일 내용 갱신.
                    tileArray[index] = m_tileMap[location.X, location.Y].ToBsonDocument();


                    indexItemListNeedUpdate.Add(new KeyValuePair<int, BsonValue>(index, tileArray[index]));
                }

                // 동기화
                //db.UpdateDocument("Tiles", "Tiles", tileArrayDoc);
                db.UpdateDocumentArray("Tiles", "Tiles", "List", "Index",
                    indexItemListNeedUpdate);
            }


            return 0;
        }

        //##################################################################################

        public int AddPowerAt(Util.DBHelper db, int tileWidthIndex, int tileHeightIndex, int delta)
        {
            // 타일맵 범위 초과시 아무것도 안함.
            if (tileWidthIndex < 0 || tileWidthIndex >= m_tileMap.GetLength(0)
                ||
                tileHeightIndex < 0 || tileHeightIndex >= m_tileMap.GetLength(1))
            {
                return -1;
            }

            // Power 증가.
            m_tileMap[tileWidthIndex, tileHeightIndex].Power += delta;

            // 동기화
            UploadTileAt(db, tileWidthIndex, tileHeightIndex);


            // 이벤트 발생
            if (delta > 0)
            {
                WhenTileUpgraded(tileWidthIndex, tileHeightIndex);
            }
            else if (delta < 0)
            {
                WhenTileUnderAttack(tileWidthIndex, tileHeightIndex);
            }


            return 0;
        }

        public int SetOwnerAt(Util.DBHelper db, int tileWidthIndex, int tileHeightIndex, string name)
        {
            // 타일맵 범위 초과시 아무것도 안함.
            if (tileWidthIndex < 0 || tileWidthIndex >= m_tileMap.GetLength(0)
                ||
                tileHeightIndex < 0 || tileHeightIndex >= m_tileMap.GetLength(1))
            {
                return -1;
            }


            // 이벤트 발생
            if (name != m_tileMap[tileWidthIndex, tileHeightIndex].Owner)
                WhenTileCaptured(tileWidthIndex, tileHeightIndex);


            // Owner 설정
            m_tileMap[tileWidthIndex, tileHeightIndex].Owner = name;

            // 동기화
            UploadTileAt(db, tileWidthIndex, tileHeightIndex);


            return 0;
        }

        public bool AttackTile(Util.DBHelper db, int tileWidthIndex, int tileHeightIndex, string playerName)
        {
            // * 점령 여부
            bool bBuild = false;

            // * 공격할 타일
            var tile = this.GetTileAt(tileWidthIndex, tileHeightIndex);
            if (tile == null) return false;

            // 목표 타일과 인접한 자신의 타일에서 공격력 합산
            int attackPower = 0;

            int[] nearX = new int[]
            {
                tileWidthIndex, tileWidthIndex + 1, tileWidthIndex, tileWidthIndex - 1
            };
            int[] nearY = new int[]
            {
                tileHeightIndex - 1, tileHeightIndex, tileHeightIndex + 1, tileHeightIndex
            };

            for (int i = 0; i < 4; ++i)
            {
                var nearTile = this.GetTileAt(nearX[i], nearY[i]);

                if (nearTile != null
                    &&
                    nearTile.Owner == playerName)
                {
                    attackPower += nearTile.Power / 2;
                }
            }


            if (attackPower <= 0)
                return false;


            // 공격
            tile.Power -= attackPower;


            // 이벤트 발생
            if (attackPower > 0)
                WhenTileUnderAttack(tileWidthIndex, tileHeightIndex);
            else if (attackPower < 0)
                WhenTileUpgraded(tileWidthIndex, tileHeightIndex);


            // 공격에 사용된 타일의 힘 감소
            for (int i = 0; i < 4; ++i)
            {
                var nearTile = this.GetTileAt(nearX[i], nearY[i]);

                if (nearTile != null
                    &&
                    nearTile.Owner == playerName)
                {
                    nearTile.Power -= nearTile.Power / 2;
                }
            }

            // 점령판정
            if (tile.Power <= 0)
            {
                // 점령
                tile.Owner = playerName;
                tile.Power = Math.Abs(tile.Power) + 2;

                bBuild = true;


                // 이벤트 발생
                WhenTileCaptured(tileWidthIndex, tileHeightIndex);
            }

            // 변경사항 업로드
            this.UploadTileForeach(db, new Point[]
            {
                new Point(nearX[0], nearY[0]),
                new Point(nearX[1], nearY[1]),
                new Point(nearX[2], nearY[2]),
                new Point(nearX[3], nearY[3]),
                new Point(tileWidthIndex, tileHeightIndex),
            });


            return bBuild;
        }

        public void AbsorbTile(Util.DBHelper db, int tileWidthIndex, int tileHeightIndex, int dirNum,
            string playerName)
        {
            if (dirNum <= 0 || dirNum > 4)
                return;

            --dirNum;


            // 선택된 타일 얻기.
            var tile = this.GetTileAt(tileWidthIndex, tileHeightIndex);
            if (tile == null || tile.Owner != playerName)
                return;


            // 방향을 가지고 좌표 계산
            int[] dirX = new int[4]
            {
                tileWidthIndex, tileWidthIndex + 1, tileWidthIndex, tileWidthIndex - 1
            };
            int[] dirY = new int[4]
            {
                tileHeightIndex - 1, tileHeightIndex, tileHeightIndex + 1, tileHeightIndex
            };


            // 목표 타일 얻기
            var targetTile = this.GetTileAt(dirX[dirNum], dirY[dirNum]);

            if (targetTile != null)
            {
                // 힘 이동
                int cost = tile.Power / 2;
                tile.Power -= cost;
                targetTile.Power += cost;

                // 힘의 이동이 있었으면 변경사항 업로드.
                if (cost > 0)
                {
                    this.UploadTileForeach(db, new Point[]
                    {
                        new Point(dirX[dirNum], dirY[dirNum]),
                        new Point(tileWidthIndex, tileHeightIndex),
                    });
                }
            }
        }

        //##################################################################################

        public GameTile GetTileAt(int widthIndex, int heightIndex)
        {
            if (widthIndex < 0 || widthIndex >= m_tileMap.GetLength(0)
                ||
                heightIndex < 0 || heightIndex >= m_tileMap.GetLength(1))
            {
                return null;
            }


            return m_tileMap[widthIndex, heightIndex];
        }

        //##################################################################################

        public int GetPlayerPower(string playerName)
        {
            int power = 0;

            foreach (var tile in m_tileMap)
            {
                if(tile.Owner == playerName)
                    power += tile.Power;
            }


            return power;
        }

        public List<KeyValuePair<string, int>> GetRank()
        {
            Dictionary<string, int> power = new Dictionary<string, int>();

            foreach (var tile in m_tileMap)
            {
                if (tile.HaveOwner)
                {
                    if (power.ContainsKey(tile.Owner))
                        power[tile.Owner] += tile.Power;
                    else
                        power.Add(tile.Owner, tile.Power);

                }
            }


            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>();

            while (power.Count > 0)
            {
                string maxPlayer = power.First().Key;
                int maxPower = power.First().Value;

                foreach (var info in power)
                {
                    if (info.Value > maxPower)
                    {
                        maxPower = info.Value;
                        maxPlayer = info.Key;
                    }
                }


                result.Add(new KeyValuePair<string, int>(maxPlayer, maxPower));

                power.Remove(maxPlayer);
            }


            return result;
        }
    }
}
