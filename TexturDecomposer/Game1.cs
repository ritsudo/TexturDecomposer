using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        private Plane controlPlane;

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

            controlPlane = new Plane();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _effect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = Matrix.CreateLookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.Up),
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
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void HandleInput(KeyboardState currentKeyboardState)
        {
            /* KEYBOARD:
             * WSADQE - Move, RTFGVB - rotate
             * Z - reset position and rotation
             * X C V - orient front, side, horizontal
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

            if (currentKeyboardState.IsKeyDown(Keys.Z))
            {
                controlPlane.ResetPosition();
            }

            if (currentKeyboardState.IsKeyDown(Keys.X))
            {
                controlPlane.Orient();
            }

            if (currentKeyboardState.IsKeyDown(Keys.C))
            {
                rotationAxis = Vector3.UnitX;
                controlPlane.OrientAndRotate(rotationAxis);
            }

            if (currentKeyboardState.IsKeyDown(Keys.V))
            {
                rotationAxis = Vector3.UnitY;
                controlPlane.OrientAndRotate(rotationAxis);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Enable wireframe mode
            RasterizerState rasterizerState = new RasterizerState
            {
                FillMode = FillMode.WireFrame,
                CullMode = CullMode.None
            };
            GraphicsDevice.RasterizerState = rasterizerState;

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
