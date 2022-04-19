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

        public int UnvSquares { get { return unvSquares; } }

        public int Diff { set { diff = value; } }

        public Room CurrentRoom { get { return currentRoom; } }

        public Map(int mapSpace, List<Texture2D> roomList, int difficulty)
        {
            rng = new Random();
            placedExit = false;
            diff = difficulty;
            SetUpMap(mapSpace, roomList);

        }

        public void SetUpMap(int numtiles, List<Texture2D> list)
        {
            unvSquares = numtiles * numtiles;
            roomMap = new Room[numtiles, numtiles];

            for (int i = 0; i < roomMap.GetLength(0); i++)
            {
                for (int j = 0; j < roomMap.GetLength(1) - 1; j++)
                {
                    if(i == j)
                    {
                        roomMap[i, j] = new Room(0, list[0], "start");
                        currentRoom = roomMap[i, j];
                    }
                    else
                    {
                        int random = rng.Next(1, 7);
                        if(!placedExit && random == 6)
                        {
                            roomMap[i, j] = new Room(0, list[6], "exit");
                            placedExit = true;
                        }
                        else
                        {
                            roomMap[i, j] = new Room(1 + diff/5, list[random], "normal");
                        }

                    }
                }
            }

            if(!placedExit)
            {
                roomMap[roomMap.GetLength(0), roomMap.GetLength(0)] = new Room(0, list[6], "exit");
                placedExit = true;
            }
        }

        public void MoveRoom(Vector2 pos)
        {

        }

        public void Nextlevel()
        {

        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentRoom.Draw(spriteBatch);
        }



    }
}
