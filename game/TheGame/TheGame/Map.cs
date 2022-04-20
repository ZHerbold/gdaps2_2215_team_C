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

        public int UnvSquares { get { return unvSquares; } set { unvSquares = value; } }

        public int Diff { get { return diff; } set { diff = value; } }

        public Room CurrentRoom { get { return currentRoom; } }

        public bool DeadEnd { get { return deadEnd; } }

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
            placedExit = false;
            numTiles = numtiles;
            unvSquares = 0;
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
                        int random = rng.Next(1, 12);
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
                            unvSquares++;
                            
                        }

                    }
                }
            }

            if(!placedExit)
            {
                roomMap[numtiles-1, numtiles-1] = new Room(list[6], "exit");
                unvSquares--;
                placedExit = true;
            }
        }

        public void MoveRoom(Vector2 pos)
        {

            if (pos.X >= 1100 && (pos.Y > 250 && pos.Y < 290))
            {
                if(x+1 < roomMap.GetLength(1))
                {
                   
                    x++;
                    currentRoom = roomMap[x, y];
                    currentRoom.WaveCount = 1 + diff / 5;
                }
                
                
            }
            else if (pos.X <= -75 && (pos.Y > 250 && pos.Y < 290))
            {
                if (x - 1 >= 0)
                {
                    
                    x--;
                    currentRoom = roomMap[x, y];
                    currentRoom.WaveCount = 1 + diff / 5;
                }
                
                
            }
            else if (pos.Y <= 0 && (pos.X > 490 && pos.X < 530))
            {
                if (y - 1 >= 0)
                {
                    
                    y--;
                    currentRoom = roomMap[x, y];
                    currentRoom.WaveCount = 1 + diff / 5;
                }
                
                
            }
            else if (pos.Y >= 575 && (pos.X > 490 && pos.X < 530))
            {
                if (y + 1 < roomMap.GetLength(0))
                {
                    
                    y++;
                    currentRoom = roomMap[x, y];
                    currentRoom.WaveCount = 1 + diff / 5;
                }
                
                
            }
        }

        public void Nextlevel()
        {

        }

        public void Update(GameTime gameTime, Vector2 pos)
        {
            deadEnd = false;
            if (pos.X >= 1100 && (pos.Y > 250 && pos.Y < 290))
            {
                if(x + 1 >= numTiles)
                {
                    deadEnd = true;
                }
                
            }
            else if (pos.X <= -75 && (pos.Y > 250 && pos.Y < 290))
            {
                if (x - 1 < 0)
                {
                    deadEnd = true;
                }
            }
            else if (pos.Y <= 0 && (pos.X > 490 && pos.X < 530))
            {
                if (y - 1 < 0)
                {
                    deadEnd = true;
                }
            }
            else if (pos.Y >= 575 && (pos.X > 490 && pos.X < 530))
            {
                if (y + 1 >= numTiles)
                {
                    deadEnd = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            
            
            currentRoom.Draw(spriteBatch);
            //map
            //IMPLEMENT A BETTER MAP USING RECTANGLE SHAPES PLEASE
            //ideally, it would show the 8 adjacent squares around the one your one, since each level increases the map size(width/height) by 1
            //color coded maybe>

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
