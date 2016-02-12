﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame_2DPlatformer;
using MonoGame_2DPlatformer.Core;

using FarseerPhysics.DebugView;

namespace MonoGame_2DPlatformer
{
    class Level
    {
        //Sky and Background
        Texture2D sky;
        Texture2D clouds;
        Texture2D clouds2;
        Texture2D mountains;
        float cloudMove;
        float cloudMove2;
        Vector2 cloudOrigin;
        Vector2 cloudOrigin2;
        Player player;
        TestActor testActor;

        Ray ray = new Ray();
        
        List<ItemTile> mapItems;
        List<Coins> mapCoins;

        GUI coinFont = new GUI();      

        int coins = 0;

        //level loader
        public void LoadLevel(string name)
        {

            sky = Game1.content.Load<Texture2D>("Sprites/sky");
            clouds = Game1.content.Load<Texture2D>("Sprites/clouds");
            clouds2 = Game1.content.Load<Texture2D>("Sprites/clouds");
            mountains = Game1.content.Load<Texture2D>("Sprites/mountains");

            coinFont.Load("Fonts/70sPixel_20");
            coinFont.Color = Color.Yellow;

            mapItems = new List<ItemTile>();
            mapCoins = new List<Coins>();


            string filePath = Game1.content.RootDirectory.ToString() + "\\Levels\\" + name;

            int x = 0;
            int y = 0;

            foreach (string line in File.ReadAllLines(filePath))
            {
                x = 0;

                foreach(char token in line)
                {
                    switch(token)
                    {
                        // Blank space
                        case '.':
                        // mapItems.Add(new ItemTile(new Vector2(x * 32, y * 32), 0f, ItemTileType.Blank));
                            break;

                        case '#':
                            mapItems.Add(new ItemTile(new Vector2(x * 32, y * 32), 1f, ItemTileType.Block));
                            break;

                        case 'c':
                            //mapItems.Add(new ItemTile(new Vector2(x * 32, y * 32), 1f, ItemTileType.Coin));
                           mapCoins.Add(new Coins(new Vector2(x * 32, y * 32)));
                            break;

                        case 'p':
                            player = new Player(new Vector2(x * 32, y * 32));
                            testActor = new TestActor();
                            break;

                        // Unknown tile type character
                    //    default:
                      //      throw new Exception(String.Format("Wrong Char"));
                    }
                    x++;
                }

                y++;
            }

        }

        public void Update(GameTime gameTime)
        {

            coinFont.Text("Coins: " + coins);

            testActor.Update();
            player.Update(gameTime);
            MoveClouds();
            CheckCollision(gameTime);
       //     Collision(player.TileBoundingBox);

        }

        public bool Collision(Rectangle rigidbodyToCheck)
        {
                foreach (var obj in mapItems)
            {
                if (obj.TileBoundingBox.Intersects(rigidbodyToCheck))
                {
                    return false;
                    GameDebug.Log("Moo");
                }
            }
            return true;
        }

        private void CheckCollision(GameTime gameTime)
        {

            testActor.Update();

       //     ray.Position = new Vector3(player.Position.X + player.Rect.Width/2, player.Position.Y + player.Rect.Height, 0);
            ray.Direction = new Vector3(0, 1, 0);

            for (int i = mapCoins.Count - 1; i >= 0; i--)
            {
                mapCoins[i].Update(gameTime);

                if (mapCoins[i].TileBoundingBox.Intersects(player.TileBoundingBox))
                {
                    coins += 1;
                    mapCoins[i].rigidbody.Dispose();
                    mapCoins.RemoveAt(i);
                }
            }
            
        }

        public void HUD(SpriteBatch spriteBatch)
        {
            coinFont.Draw(spriteBatch);
        }

        public void Sky(SpriteBatch spriteBatch)
        {
            Rectangle screenRectangle = new Rectangle(0, 0, Screen.width, Screen.height);
            spriteBatch.Draw(sky, screenRectangle, Color.White);
           spriteBatch.Draw(clouds, screenRectangle, null, Color.WhiteSmoke, 0f, cloudOrigin, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(clouds2, screenRectangle, null, Color.White, 0f, cloudOrigin2, SpriteEffects.None, 0);
            spriteBatch.Draw(mountains, screenRectangle, Color.White);
        }

        void MoveClouds()
        {

            // This stuff is ugly !!!!

            const float speed = 1f;
            const float offset = 300f;
            cloudOrigin = new Vector2(cloudMove, 0);
            cloudOrigin2 = new Vector2(cloudMove2 + offset, 0);

            cloudMove += speed * Time.DeltaTime;
            cloudMove2 += speed * Time.DeltaTime;

            if (cloudOrigin.X >= clouds.Width)
            {
                cloudMove = -clouds.Width;
            }

            if (cloudOrigin2.X >= clouds2.Width)
            {
                cloudMove2 = -clouds2.Width - offset;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (ItemTile it in mapItems)
            {
                it.Draw(spriteBatch);
            }

            foreach (Coins c in mapCoins)
            {
              c.Draw(spriteBatch);
            }
            
            player.Draw(spriteBatch);
            testActor.Draw(spriteBatch);

            SpriteBatchEx.GraphicsDevice = Game1.graphics.GraphicsDevice;
          //  spriteBatch.DrawLine(new Vector2(ray.Position.X, ray.Position.Y), new Vector2(ray.Position.X + ray.Direction.X, ray.Position.Y + ray.Direction.Y), Color.Red);
        }

    }
}

