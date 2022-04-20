using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheGame
{
    class Map
    {
        private int unvSquares;
        private Room[,] roomMap;
        private Room currentRoom;
        private Random rng;
        private bool placedExit;
        private int diff;
        private int x;
        private int y;
        private bool deadEnd;
        private int numTiles;

        public int X { get { return x; } }
        public int Y { get { return y; }  }

        public int NumTiles { get { return numTiles; } }

        public int UnvSquares { get { return unvSquares; } }

        public int Diff { get { return diff; } set { diff = value; } }

        public Room CurrentRoom { get { return currentRoom; } }

        public Map(int mapSpace, List<Texture2D> roomList, int difficulty)
        {
            rng = new Random();
            placedExit = false;
            diff = difficulty;
            SetUpMap(mapSpace, roomList);
            deadEnd = false;
            
        }

        public void SetUpMap(int numtiles, List<Texture2D> list)
        {
            numTiles = numtiles;
            unvSquares = numtiles * numtiles;
            roomMap = new Room[numtiles, numtiles];

            for (int i = 0; i < roomMap.GetLength(0); i++)
            {
                for (int j = 0; j < roomMap.GetLength(1); j++)
                {
                    if(i == numtiles/2 && j == numtiles/2)
                    {
                        roomMap[i, j] = new Room(list[0], "start");
                        currentRoom = roomMap[i, j];
                        x = i;
                        y = j;
                    }
                    else
                    {
                        int random = rng.Next(1, 11);
                        if(!placedExit && random == 11)
                        {
                            roomMap[i, j] = new Room(list[6], "exit");
                            placedExit = true;
                            
                        }
                        else if(random == 10)
                        {
                            roomMap[i, j] = new Room(list[5], "shop");
                        }
                        else
                        {
                            if(random == 11)
                            {
                                roomMap[i, j] = new Room(list[1], "normal");
                            }
                            else if(random > 3)
                            {
                                roomMap[i, j] = new Room(list[4], "normal");
                            }
                            else
                            {
                                roomMap[i, j] = new Room(list[random], "normal");
                            }
                            
                        }

                    }
                }
            }

            if(!placedExit)
            {
                roomMap[roomMap.GetLength(0) - 1, roomMap.GetLength(0) - 1] = new Room(list[6], "exit");
                placedExit = true;
            }
        }

        public void MoveRoom(Vector2 pos)
        {

            if (pos.X >= 1100 && (pos.Y > 250 && pos.Y < 290))
            {
                if(x+1 < roomMap.GetLength(1))
                {
                    unvSquares--;
                    x++;
                    currentRoom = roomMap[x, y];
                    currentRoom.WaveCount = 1 + diff / 5;
                }
                else
                {
                    deadEnd = true;
                }
                
            }
            else if (pos.X <= -75 && (pos.Y > 250 && pos.Y < 290))
            {
                if (x - 1 >= 0)
                {
                    unvSquares--;
                    x--;
                    currentRoom = roomMap[x, y];
                    currentRoom.WaveCount = 1 + diff / 5;
                }
                else
                {
                    deadEnd = true;
                }
                
            }
            else if (pos.Y <= 0 && (pos.X > 490 && pos.X < 530))
            {
                if (y - 1 >= 0)
                {
                    unvSquares--;
                    y--;
                    currentRoom = roomMap[x, y];
                    currentRoom.WaveCount = 1 + diff / 5;
                }
                else
                {
                    deadEnd = true;
                }
                
            }
            else if (pos.Y >= 575 && (pos.X > 490 && pos.X < 530))
            {
                if (y + 1 < roomMap.GetLength(0))
                {
                    unvSquares--;
                    y++;
                    currentRoom = roomMap[x, y];
                    currentRoom.WaveCount = 1 + diff / 5;
                }
                else
                {
                    deadEnd = true;
                }
                
            }
        }

        public void Nextlevel()
        {

        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            
            if(deadEnd)
            {
                deadEnd = false;
                spriteBatch.DrawString(font, "This Hallway is closed off...", new Vector2(270, 510), Color.White);
            }
            currentRoom.Draw(spriteBatch);
            for (int i = 0; i < roomMap.GetLength(0); i++)
            {
                for (int j = 0; j < roomMap.GetLength(1); j++)
                {
                    if(roomMap[i,j].RoomType == "start")
                    {
                        if(currentRoom == roomMap[i,j])
                        {
                            spriteBatch.DrawString(font, " (X) ", new Vector2(1050 + (i * 60), 10 + (j * 50)), Color.LimeGreen);
                        }
                        else
                        {
                            spriteBatch.DrawString(font, " (X) ", new Vector2(1050 + (i * 60), 10 + (j * 50)), Color.White);
                        }
                        
                    }
                    else if(roomMap[i, j].RoomType == "exit")
                    {
                        if (currentRoom == roomMap[i, j])
                        {
                            spriteBatch.DrawString(font, " (E) ", new Vector2(1050 + (i * 60), 10 + (j * 50)), Color.LimeGreen);
                        }
                        else
                        {
                            spriteBatch.DrawString(font, " (E) ", new Vector2(1050 + (i * 60), 10 + (j * 50)), Color.White);
                        }
                        
                    }
                    else if (roomMap[i, j].RoomType == "shop")
                    {
                        if (currentRoom == roomMap[i, j])
                        {
                            spriteBatch.DrawString(font, " (S) ", new Vector2(1050 + (i * 60), 10 + (j * 50)), Color.LimeGreen);
                        }
                        else
                        {
                            spriteBatch.DrawString(font, " (S) ", new Vector2(1050 + (i * 60), 10 + (j * 50)), Color.White);
                        }
                       
                    }
                    else
                    {
                        if (currentRoom == roomMap[i, j])
                        {
                            spriteBatch.DrawString(font, " (O) ", new Vector2(1050 + (i * 60), 10 + (j * 50)), Color.LimeGreen);
                        }
                        else
                        {
                            spriteBatch.DrawString(font, " (O) ", new Vector2(1050 + (i * 60), 10 + (j * 50)), Color.White);
                        }
                        
                    }
                }
            }

        }

        



    }
}
