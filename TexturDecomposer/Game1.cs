using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static System.Reflection.Metadata.BlobBuilder;

namespace TexturDecomposer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private VertexPositionColor[] _vertices;
        private short[] _indices;
        private BasicEffect _effect;
        private BasicEffect _textureEffect;
        private KeyboardState previousKeyboardState;

        private Plane controlPlane;
        private RenderTarget2D backgroundTextureTarget;
        private Texture2D backgroundTexture;
        private Rectangle backgroundRectangle;

        private RenderTarget2D renderTarget;
        private Texture2D spriteTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Set the game window size
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            // Enable wireframe mode
            RasterizerState rasterizerState = new RasterizerState
            {
                FillMode = FillMode.WireFrame,
                CullMode = CullMode.None
            };
            GraphicsDevice.RasterizerState = rasterizerState;

            controlPlane = new Plane();
            renderTarget = new RenderTarget2D(GraphicsDevice, controlPlane.TextureResolutionX, controlPlane.TextureResolutionY);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTextureTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            backgroundTexture = Content.Load<Texture2D>("background");
            backgroundRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            _textureEffect = new BasicEffect(GraphicsDevice);
            _textureEffect.TextureEnabled = true;
            _textureEffect.Texture = backgroundTextureTarget;

            _effect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = Matrix.CreateLookAt(new Vector3(0, 0, 2), Vector3.Zero, Vector3.Up),
                Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f
            )
            };

            // Define vertices of the plane
            _vertices = controlPlane._vertices;

            // Define indices for the wireframe plane
            _indices = controlPlane._indices;
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            HandleInput(currentKeyboardState);
            previousKeyboardState = currentKeyboardState;

            spriteTexture = Trapezoid.DrawPrimitivesToRenderTarget(GraphicsDevice, controlPlane, renderTarget, _textureEffect, _effect);

            base.Update(gameTime);
        }

        private void HandleInput(KeyboardState currentKeyboardState)
        {
            /* KEYBOARD:
             * WSADQE - Move, RTFGVB - rotate
             * N - reset position and rotation
             * Z X C - orient front, side, horizontal
             */

            KeyboardState keyboardState = Keyboard.GetState();
            Vector3 direction = Vector3.Zero;
            Vector3 rotationAxis = Vector3.Zero;
            float moveValue = 0.0f;
            float moveSpeed = 0.1f;
            float rotationAngle = 0.0f;
            float rotationSpeed = 0.0174533f;

            // Rotation cube
            if (currentKeyboardState.IsKeyDown(Keys.R))
            {
                rotationAxis = Vector3.UnitX;
                rotationAngle = rotationSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.T))
            {
                rotationAxis = Vector3.UnitX;
                rotationAngle = -rotationSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.F))
            {
                rotationAxis = Vector3.UnitY;
                rotationAngle = rotationSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.G))
            {
                rotationAxis = Vector3.UnitY;
                rotationAngle = -rotationSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.V))
            {
                rotationAxis = Vector3.UnitZ;
                rotationAngle = rotationSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.B))
            {
                rotationAxis = Vector3.UnitZ;
                rotationAngle = -rotationSpeed;
            }

            if (rotationAngle != 0)
            {
                controlPlane.Rotate(rotationAxis, rotationAngle);
            }
            

            // WSADQE movement for cube
            if (currentKeyboardState.IsKeyDown(Keys.W))
            {
                direction = Vector3.UnitY;
                moveValue += moveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.S))
            {
                direction = Vector3.UnitY;
                moveValue -= moveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.A))
            {
                direction = Vector3.UnitX;
                moveValue -= moveSpeed;;
            }
            if (currentKeyboardState.IsKeyDown(Keys.D))
            {
                direction = Vector3.UnitX;
                moveValue += moveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Q))
            {
                direction = Vector3.UnitZ;
                moveValue += moveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.E))
            {
                direction = Vector3.UnitZ;
                moveValue -= moveSpeed;
            }

            if (moveValue != 0)
            {
                controlPlane.Move(direction, moveValue);
            }

            if (currentKeyboardState.IsKeyDown(Keys.N))
            {
                controlPlane.ResetPosition();
            }

            if (currentKeyboardState.IsKeyDown(Keys.Z))
            {
                controlPlane.Orient();
            }

            if (currentKeyboardState.IsKeyDown(Keys.X))
            {
                rotationAxis = Vector3.UnitX;
                controlPlane.OrientAndRotate(rotationAxis);
            }

            if (currentKeyboardState.IsKeyDown(Keys.C))
            {
                rotationAxis = Vector3.UnitY;
                controlPlane.OrientAndRotate(rotationAxis);
            }

            //Save
            if (currentKeyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))  // Move cube backward along Z-axis (away from camera)
                SaveToFile(spriteTexture);
        }

        private void SaveToFile(Texture2D saveTarget)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss"); // Format: YearMonthDay_HourMinuteSecond
            string outputFolder = "output"; // Folder where the files will be saved
            string fileName = $"texture_{timestamp}.png";

            // Ensure the output folder exists
            if (!System.IO.Directory.Exists(outputFolder))
            {
                System.IO.Directory.CreateDirectory(outputFolder);
            }

            // Combine folder and file name to get the full path
            string filePath = System.IO.Path.Combine(outputFolder, fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                saveTarget.SaveAsPng(stream, controlPlane.TextureResolutionX, controlPlane.TextureResolutionY); // 128 - texture resolution
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.SetRenderTarget(backgroundTextureTarget);
            _spriteBatch.Begin();
            _spriteBatch.Draw(backgroundTexture, backgroundRectangle, Color.White);  // Draw the background texture
            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin();
            _spriteBatch.Draw(backgroundTexture, backgroundRectangle, Color.White);  // Draw the background texture
            _spriteBatch.Draw(spriteTexture, new Vector2(10, 10), Color.White);
            _spriteBatch.End();

            // Draw the plane
            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                controlPlane.Draw(GraphicsDevice);
            }

            base.Draw(gameTime);
        }
    }
}
