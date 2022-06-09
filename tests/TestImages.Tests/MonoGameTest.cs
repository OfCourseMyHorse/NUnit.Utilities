using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Tests.Graphics;

using NUnit.Framework;

namespace Monogame
{    
    internal sealed class MonoGameTest : GraphicsDeviceTestFixtureBase, IAttachmentWriter
    {
        public AttachmentInfo Attach(string fileName, string description = null) => new AttachmentInfo(fileName, description);

        private SpriteBatch _spriteBatch;
        private Texture2D _texture;        
        

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _spriteBatch = new SpriteBatch(gd);            

            var data = new ushort[256 * 256];
            data.AsSpan().Fill(ushort.MaxValue);

            _texture = new Texture2D(gd, 256, 256, false, SurfaceFormat.Bgr565);
            _texture.SetData(data);
        }

        [TearDown]
        public override void TearDown()
        {
            _spriteBatch.Dispose();
            _texture.Dispose();
            base.TearDown();
        }        

        [Test]
        public void RenderTest()
        {
            PrepareFrameCapture();

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
            _spriteBatch.Draw(_texture, new Vector2(20, 20), Color.White);
            _spriteBatch.End();

            var frame = SubmitFrame();

            frame.SaveTo(Attach("render.bmp", "render result"));
        }

        
    }
}

