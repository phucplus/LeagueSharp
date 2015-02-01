﻿using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using PennyJinx.Properties;
using SharpDX;
using SharpDX.Direct3D9;

namespace PennyJinx
{
    internal class SpriteManager
    {
        public class ScopeSprite
        {
            private static Render.Sprite _sprite;
            public static Texture Texture;
            /*private Vector2 TextPos
            {
                get
                {
                    return  Drawing.WorldToScreen(new Vector2(Pos.X,Pos.Y+25).To3D());
                }
            }

            private String GetHp
            {
                get
                {
                    var condition = (Hero != null && PennyJinx.IsMenuEnabled("SpriteDraw") && PennyJinx._r.IsReady());
                    return condition?"Killable! " + Hero.Health + " HP":"Error getting HP";
                }
            }*/
            //Constructor
            public ScopeSprite()
            {
                Texture = Texture.FromMemory(
                    Drawing.Direct3DDevice, (byte[]) new ImageConverter().ConvertTo(Resources.scope, typeof(byte[])),
                    180, 180, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
                // _sprite = new Sprite(Drawing.Direct3DDevice);


                _sprite = new Render.Sprite(Texture, new Vector2(0, 0))
                {
                    VisibleCondition = s => Condition,
                    PositionUpdate = () => Pos,
                    Scale = new Vector2(0.65f, 0.65f)
                };
                _sprite.Add();

                Drawing.OnEndScene += Drawing_OnEndScene;
                Drawing.OnPreReset += Drawing_OnPreReset;
                Drawing.OnPostReset += Drawing_OnPostReset;
                AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
                AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
            }

            /*private readonly Render.Text _KillableText;

            private static readonly Font _font = new Font(
            Drawing.Direct3DDevice,
            new FontDescription
            {
               FaceName = "Calibri",
               Height = 15,
               OutputPrecision = FontPrecision.Default,
               Quality = FontQuality.Default,
            });*/

            private static Obj_AI_Hero Hero
            {
                get
                {
                    //return ObjectManager.Player;
                    var hList =
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(
                                hero =>
                                    hero.IsValidTarget(PennyJinx.R.Range) &&
                                    PennyJinx.R.GetDamage(hero) >=
                                    HealthPrediction.GetHealthPrediction(
                                        hero, (int) (ObjectManager.Player.Distance(hero) / 2000f) * 1000))
                            .OrderBy(ph => ph.HealthPercentage())
                            .ToList();

                    /**  var hList = ObjectManager.Get<Obj_AI_Hero>()
                       .Where(
                           hero =>
                               hero.IsValidTarget() )
                       .OrderBy(ph => ph.HealthPercentage()).ToList();
                   * */
                    return !hList.Any() ? null : hList.First();
                }
            }

            private static Vector2 Pos
            {
                get
                {
                    return
                        new Vector2(
                            Drawing.WorldToScreen(Hero.Position).X - Hero.BoundingRadius * 2 +
                            Hero.BoundingRadius / 2.5f, Drawing.WorldToScreen(Hero.Position).Y - Hero.BoundingRadius * 2);
                }
            }

            private static bool Condition
            {
                //   get { return (Hero != null && PennyJinx.IsMenuEnabled("SpriteDraw") && PennyJinx.R.IsReady()); }
                get { return Hero != null; }
            }

            private static void CurrentDomainOnDomainUnload(object sender, EventArgs e)
            {
                _sprite.Dispose();
            }

            private static void Drawing_OnPostReset(EventArgs args)
            {
                _sprite.OnPostReset();
                //_sprite.OnResetDevice();
            }

            private static void Drawing_OnPreReset(EventArgs args)
            {
                _sprite.OnPreReset();
                //_sprite.OnLostDevice();
            }

            private static void Drawing_OnEndScene(EventArgs args)
            {
                /***
                if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
            {
                return;
            }

                try
                {
                    if (_sprite.IsDisposed || !Condition)
                    {
                        return;
                    }
                    DrawSprite();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Can't draw Sprite");
                }
                 * */
            }
        }
    }
}